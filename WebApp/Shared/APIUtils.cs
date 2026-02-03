using DWIS.ContextualData.WellBoreSelector.ModelShared;
using Plotly.Blazor.Traces.SankeyLib;

public static class APIUtils
{
    // API parameters
    public static string HostBasePath => "WellBoreSelector/api/";
    public static string HostBasePathField => "Field/api/";
    public static string HostBasePathCluster => "Cluster/api/";
    public static string HostBasePathWell => "Well/api/";
    public static string HostBasePathWellBore => "WellBore/api/";
    public static string HostBasePathUnitConversion => "UnitConversion/api/";

    private static readonly Lazy<HttpClient> _httpClient = new(() =>
        SetHttpClient(GetRequiredUrl(nameof(DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.WellBoreSelectorHostURL)), HostBasePath));
    private static readonly Lazy<Client> _clientDwis = new(() =>
        new Client(HttpClient.BaseAddress!.ToString(), HttpClient));

    private static readonly Lazy<HttpClient> _httpClientField = new(() =>
        SetHttpClient(GetRequiredUrl(nameof(DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.FieldHostURL)), HostBasePathField));
    private static readonly Lazy<DWIS.ContextualData.WellBoreSelector.ModelShared.Client> _clientField = new(() =>
        new DWIS.ContextualData.WellBoreSelector.ModelShared.Client(HttpClientField.BaseAddress!.ToString(), HttpClientField));

    private static readonly Lazy<HttpClient> _httpClientCluster = new(() =>
        SetHttpClient(GetRequiredUrl(nameof(DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.ClusterHostURL)), HostBasePathCluster));
    private static readonly Lazy<DWIS.ContextualData.WellBoreSelector.ModelShared.Client> _clientCluster = new(() =>
        new DWIS.ContextualData.WellBoreSelector.ModelShared.Client(HttpClientCluster.BaseAddress!.ToString(), HttpClientCluster));

    private static readonly Lazy<HttpClient> _httpClientWell = new(() =>
        SetHttpClient(GetRequiredUrl(nameof(DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.WellHostURL)), HostBasePathWell));
    private static readonly Lazy<DWIS.ContextualData.WellBoreSelector.ModelShared.Client> _clientWell = new(() =>
        new DWIS.ContextualData.WellBoreSelector.ModelShared.Client(HttpClientWell.BaseAddress!.ToString(), HttpClientWell));

    private static readonly Lazy<HttpClient> _httpClientWellBore = new(() =>
        SetHttpClient(GetRequiredUrl(nameof(DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.WellBoreHostURL)), HostBasePathWellBore));
    private static readonly Lazy<DWIS.ContextualData.WellBoreSelector.ModelShared.Client> _clientWellBore = new(() =>
        new DWIS.ContextualData.WellBoreSelector.ModelShared.Client(HttpClientWellBore.BaseAddress!.ToString(), HttpClientWellBore));

    public static HttpClient HttpClient => _httpClient.Value;
    public static Client ClientDWIS => _clientDwis.Value;
    public static HttpClient HttpClientField => _httpClientField.Value;
    public static DWIS.ContextualData.WellBoreSelector.ModelShared.Client ClientField => _clientField.Value;
    public static HttpClient HttpClientCluster => _httpClientCluster.Value;
    public static DWIS.ContextualData.WellBoreSelector.ModelShared.Client ClientCluster => _clientCluster.Value;
    public static HttpClient HttpClientWell => _httpClientWell.Value;
    public static DWIS.ContextualData.WellBoreSelector.ModelShared.Client ClientWell => _clientWell.Value;
    public static HttpClient HttpClientWellBore => _httpClientWellBore.Value;
    public static DWIS.ContextualData.WellBoreSelector.ModelShared.Client ClientWellBore => _clientWellBore.Value;

    public static string HostNameUnitConversion =>
        GetRequiredUrl(nameof(DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.UnitConversionHostURL));

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

    private static string GetRequiredUrl(string configProperty)
    {
        var config = DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.Instance;
        var value = configProperty switch
        {
            nameof(DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.WellBoreSelectorHostURL) => config.WellBoreSelectorHostURL,
            nameof(DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.UnitConversionHostURL) => config.UnitConversionHostURL,
            nameof(DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.WellBoreHostURL) => config.WellBoreHostURL,
            nameof(DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.WellHostURL) => config.WellHostURL,
            nameof(DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.FieldHostURL) => config.FieldHostURL,
            nameof(DWIS.ContextualData.WellBoreSelector.WebApp.Configuration.ClusterHostURL) => config.ClusterHostURL,
            _ => null
        };

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Missing required configuration value '{configProperty}' in Config.json.");
        }

        return value;
    }
}
