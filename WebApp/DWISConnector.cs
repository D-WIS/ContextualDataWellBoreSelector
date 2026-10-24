using DWIS.API.DTO;
using DWIS.Client.ReferenceImplementation;
using DWIS.Client.ReferenceImplementation.OPCFoundation;
using DWIS.ContextualData.WellBoreSelector.WebApp;
using Microsoft.Extensions.Logging;

namespace WebApp
{
    public class DWISConnector
    {
        private static readonly Lazy<DWISConnector> _instance = new(() => new DWISConnector());

        public static DWISConnector Instance => _instance.Value;

        private volatile IOPCUADWISClient? _client;
        public IOPCUADWISClient? DWISClient { get => _client; set => _client = value; }


        public DWISConnector() : this(() => new DWISClientOPCF(new DefaultDWISClientConfiguration
        {
            UseWebAPI = false,
            ServerAddress = Configuration.Instance.GeneralBlackboard
        }, null)) { }

        private readonly Func<IOPCUADWISClient> _openClient;
        internal ILogger? ConnectionLogger { get; set; }
        private readonly TimeSpan _retryInterval;

        public DWISConnector(Func<IOPCUADWISClient> openClient, ILogger? logger = null,
            TimeSpan? retryInterval = null)
        {
            _openClient = openClient;
            ConnectionLogger = logger;
            _retryInterval = retryInterval ?? TimeSpan.FromSeconds(5);
        }

        public async Task RunAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    if (!IsHealthy(DWISClient))
                    {
                        var previous = DWISClient;
                        DWISClient = null;
                        (previous as IDisposable)?.Dispose();
                        IOPCUADWISClient? candidate = null;
                        try
                        {
                            candidate = _openClient();
                            if (candidate.Connected)
                            {
                                DWISClient = candidate;
                                candidate = null;
                                ConnectionLogger?.LogInformation("Connected to the Blackboard; selection bindings will be refreshed");
                            }
                        }
                        catch (Exception ex)
                        {
                            ConnectionLogger?.LogWarning(ex, "Cannot connect to the Blackboard; retrying in {Seconds} seconds", _retryInterval.TotalSeconds);
                        }
                        finally { (candidate as IDisposable)?.Dispose(); }
                    }
                    await Task.Delay(_retryInterval, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
            finally
            {
                var client = DWISClient;
                DWISClient = null;
                (client as IDisposable)?.Dispose();
            }
        }

        private bool IsHealthy(IOPCUADWISClient? client)
        {
            if (client?.Connected != true) return false;
            try
            {
                // A lost channel may still report Connected until its next request fails.
                return client is not DWISClientOPCF opc ||
                    opc.ReadValue(Opc.Ua.VariableIds.Server_ServerStatus_CurrentTime)?.Value is DateTime;
            }
            catch (Exception ex)
            {
                ConnectionLogger?.LogWarning(ex, "Blackboard health check failed; reconnecting");
                return false;
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
