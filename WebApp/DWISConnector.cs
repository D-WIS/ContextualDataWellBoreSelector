using DWIS.API.DTO;
using DWIS.Client.ReferenceImplementation;
using DWIS.Client.ReferenceImplementation.OPCFoundation;
using DWIS.ContextualData.WellBoreSelector.WebApp;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApp
{
    public class DWISConnector
    {
        private static readonly Lazy<DWISConnector> _instance = new(() => new DWISConnector());

        public static DWISConnector Instance => _instance.Value;

        public IOPCUADWISClient? DWISClient { get; set; } = null;


        public DWISConnector()
        {
        }

        public void ConnectToBlackboard(ILogger<DWISClientOPCF>? loggerDWISClient)
        {
            try
            {
                if (Configuration.Instance != null && !string.IsNullOrEmpty(Configuration.Instance.GeneralBlackboard))
                {
                    DefaultDWISClientConfiguration defaultDWISClientConfiguration = new DefaultDWISClientConfiguration();
                    defaultDWISClientConfiguration.UseWebAPI = false;
                    defaultDWISClientConfiguration.ServerAddress = Configuration.Instance.GeneralBlackboard;
                    DWISClient = new DWISClientOPCF(defaultDWISClientConfiguration, loggerDWISClient);
                }
            }
            catch (Exception e)
            {
                loggerDWISClient?.LogError(e.ToString());
            }
        }

        public bool SendValue(QueryResult? queryResult, string? value)
        {
            bool ok = false;
            if (DWISClient is not null && DWISClient.Connected && queryResult != null && queryResult != null && queryResult.Count > 0 && queryResult[0].Count > 0)
            {
                NodeIdentifier id = queryResult[0][0];
                if (id != null && !string.IsNullOrEmpty(id.ID) && !string.IsNullOrEmpty(id.NameSpace))
                {
                    // OPC-UA code to set the value at the node id = ID
                    (string nameSpace, string id, object value, DateTime sourceTimestamp)[] outputs = new (string nameSpace, string id, object value, DateTime sourceTimestamp)[1];
                    outputs[0].nameSpace = id.NameSpace;
                    outputs[0].id = id.ID;
                    outputs[0].value = value;
                    outputs[0].sourceTimestamp = DateTime.UtcNow;
                    ok = DWISClient.UpdateAnyVariables(outputs);
                }
            }
            return ok;
        }
    }
}
