using System.Text.Json;

namespace DWIS.ContextualData.WellBoreSelector.WebApp
{
    public sealed class Configuration
    {
        private static readonly Lazy<Configuration> _instance = new(() => new Configuration());

        public static Configuration Instance => _instance.Value;

        public Configuration()
        {
        }

        public string? WellBoreSelectorHostURL { get; set; }
        public string? UnitConversionHostURL { get; set; }
        public string? WellBoreHostURL { get; set; }
        public string? WellHostURL { get; set; }
        public string? FieldHostURL { get; set; }
        public string? ClusterHostURL { get; set; }
        public string? GeneralBlackboard { get; set; } = "opc.tcp://localhost:48030";

        public static void LoadFromHome(string contentRootPath)
        {
            Instance.LoadFromHomeInternal(contentRootPath);
        }

        private void LoadFromHomeInternal(string contentRootPath)
        {
            var homeDir = Path.GetFullPath(Path.Combine(contentRootPath, "..", "home"));
            var configPath = Path.Combine(homeDir, "Config.json");

            if (!Directory.Exists(homeDir))
            {
                Directory.CreateDirectory(homeDir);
            }

            if (!File.Exists(configPath))
            {
                WriteDefaults(configPath);
                return;
            }

            var json = File.ReadAllText(configPath);
            var loaded = JsonSerializer.Deserialize<Configuration>(json);
            if (loaded == null)
            {
                return;
            }

            ApplyFrom(loaded);
        }

        private void ApplyFrom(Configuration loaded)
        {
            WellBoreSelectorHostURL = loaded.WellBoreSelectorHostURL;
            UnitConversionHostURL = loaded.UnitConversionHostURL;
            WellBoreHostURL = loaded.WellBoreHostURL;
            WellHostURL = loaded.WellHostURL;
            FieldHostURL = loaded.FieldHostURL;
            ClusterHostURL = loaded.ClusterHostURL;
            GeneralBlackboard = loaded.GeneralBlackboard;
        }

        private void WriteDefaults(string configPath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(configPath, json);
        }
    }
}
