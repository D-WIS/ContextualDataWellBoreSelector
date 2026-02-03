using DWIS.ContextualData.WellBoreSelector.ModelShared;
using Plotly.Blazor.Traces.SankeyLib;

public static class APIUtils
{
    // API parameters
    public static readonly string HostName = DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.Instance.WellBoreSelectorHostURL!;
    public static readonly string HostBasePath = "WellBoreSelector/api/";
    public static readonly HttpClient HttpClient = APIUtils.SetHttpClient(HostName, HostBasePath);
    public static readonly Client ClientDWIS = new Client(APIUtils.HttpClient.BaseAddress!.ToString(), APIUtils.HttpClient);

    // Field api
    public static readonly string HostNameField = DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.Instance.FieldHostURL!;
    public static readonly string HostBasePathField = "Field/api/";
    public static readonly HttpClient HttpClientField = APIUtils.SetHttpClient(HostNameField, HostBasePathField);
    public static readonly DWIS.ContextualData.WellBoreSelector.ModelShared.Client ClientField = new DWIS.ContextualData.WellBoreSelector.ModelShared.Client(APIUtils.HttpClientField.BaseAddress!.ToString(), APIUtils.HttpClientField);
    // Cluster api
    public static readonly string HostNameCluster = DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.Instance.ClusterHostURL!;
    public static readonly string HostBasePathCluster = "Cluster/api/";
    public static readonly HttpClient HttpClientCluster = APIUtils.SetHttpClient(HostNameCluster, HostBasePathCluster);
    public static readonly DWIS.ContextualData.WellBoreSelector.ModelShared.Client ClientCluster = new DWIS.ContextualData.WellBoreSelector.ModelShared.Client(APIUtils.HttpClientCluster.BaseAddress!.ToString(), APIUtils.HttpClientCluster);
    // Well api
    public static readonly string HostNameWell = DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.Instance.WellHostURL!;
    public static readonly string HostBasePathWell = "Well/api/";
    public static readonly HttpClient HttpClientWell = APIUtils.SetHttpClient(HostNameWell, HostBasePathWell);
    public static readonly DWIS.ContextualData.WellBoreSelector.ModelShared.Client ClientWell = new DWIS.ContextualData.WellBoreSelector.ModelShared.Client(APIUtils.HttpClientWell.BaseAddress!.ToString(), APIUtils.HttpClientWell);
    // WellBore api
    public static readonly string HostNameWellBore = DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.Instance.WellBoreHostURL!;
    public static readonly string HostBasePathWellBore = "WellBore/api/";
    public static readonly HttpClient HttpClientWellBore = APIUtils.SetHttpClient(HostNameWellBore, HostBasePathWellBore);
    public static readonly DWIS.ContextualData.WellBoreSelector.ModelShared.Client ClientWellBore = new DWIS.ContextualData.WellBoreSelector.ModelShared.Client(APIUtils.HttpClientWellBore.BaseAddress!.ToString(), APIUtils.HttpClientWellBore);

    public static readonly string HostNameUnitConversion = DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.Instance.UnitConversionHostURL!;
    public static readonly string HostBasePathUnitConversion = "UnitConversion/api/";

    // API utility methods
    public static HttpClient SetHttpClient(string host, string microServiceUri)
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; // temporary workaround for testing purposes: bypass certificate validation (not recommended for production environments due to security risks)
        HttpClient httpClient = new(handler)
        {
            BaseAddress = new Uri(host + microServiceUri)
        };
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        return httpClient;
    }
}
