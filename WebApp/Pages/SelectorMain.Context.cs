using DWIS.API.DTO;
using MudBlazor;
using OSDC.DotnetLibraries.Drilling.DrillingProperties;
using System.Text.Json;

namespace WebApp.Pages;

public partial class SelectorMain
{
    private readonly ContextSelectionData _contextSignals = new();
    private readonly Dictionary<string, QueryResult> _contextNodes = new();
    private List<ContextOption> _architectures = new(), _trajectories = new(), _drillStrings = new();
    private Guid? _architectureID, _trajectoryID, _drillStringID;
    private bool _contextLoading, _contextLoadFailed;
    private int _contextLoadVersion;

    private async Task InitializeContextSelectionsAsync()
    {
        var client = DWISConnector.Instance.DWISClient;
        if (client?.Connected != true) return;
        foreach (var property in typeof(ContextSelectionData).GetProperties())
        {
            var info = (DWIS.RigOS.Common.Model.SemanticInfo)property.GetValue(_contextSignals)!;
            info.Manifest = GeneratorSparQLManifestFile.GetManifestFile(typeof(ContextSelectionData).Assembly,
                typeof(ContextSelectionData).FullName, property.Name, "DWIS.ContextualSelections", "DWIS", "ContextSelections");
            var query = GeneratorSparQLManifestFile.GetSparQLQueries(typeof(ContextSelectionData).Assembly,
                typeof(ContextSelectionData).FullName, property.Name)
                ?? throw new InvalidDataException("Missing selection semantics.");
            var specification = query.First().Value;
            info.SparQLQuery = specification.SparQL;
            info.SparQLVariables = specification.Variables;
            QueryResult? node = null;
            if (!RegisterToBlackboard(info, client, ref node) || node is null)
                throw new IOException("Cannot register contextual selection " + property.Name);
            _contextNodes[property.Name] = node;
        }
        await LoadContextChoicesAsync();
        _architectureID = ReadSelection("WellBoreArchitecture", _architectures);
        _trajectoryID = ReadSelection("Trajectory", _trajectories);
        _drillStringID = ReadSelection("DrillString", _drillStrings);
    }

    private Guid? ReadSelection(string kind, List<ContextOption> options)
    {
        if (!_contextNodes.TryGetValue(kind, out var query) || query.Count == 0 || query[0].Count == 0) return null;
        var node = query[0][0];
        return DWISConnector.Instance.DWISClient!.ReadAnyVariables(out object[] values, (node.NameSpace, node.ID))
            && values.FirstOrDefault() is string text && Guid.TryParse(text, out var id)
            && options.Any(x => x.ID == id) ? id : null;
    }

    private async Task LoadContextChoicesAsync()
    {
        int version = ++_contextLoadVersion;
        _architectureID = _trajectoryID = _drillStringID = null;
        _architectures.Clear(); _trajectories.Clear(); _drillStrings.Clear();
        if (_selectedWellBore?.MetaInfo is null) { _contextLoading = false; _contextLoadFailed = false; return; }
        _contextLoading = true;
        _contextLoadFailed = false;
        try
        {
            var id = _selectedWellBore.MetaInfo.ID;
            var architectures = await APIUtils.LoadArchitectureOptionsAsync(id);
            var trajectories = await APIUtils.LoadTrajectoryOptionsAsync(id);
            var drillStrings = await APIUtils.LoadDrillStringOptionsAsync(id);
            if (version != _contextLoadVersion || _selectedWellBore?.MetaInfo?.ID != id) return;
            _architectures = architectures; _trajectories = trajectories; _drillStrings = drillStrings;
        }
        catch (Exception ex)
        {
            _contextLoadFailed = true;
            logger.LogError(ex, "Cannot load contextual choices");
            Snackbar.Add("Cannot load contextual choices. Check service availability.", Severity.Error);
        }
        finally { if (version == _contextLoadVersion) _contextLoading = false; }
    }

    private async Task ApplySelectionsAsync()
    {
        try
        {
            var client = DWISConnector.Instance.DWISClient;
            if (client?.Connected != true || _placeHolder is null || _selectedWellBore?.MetaInfo is null)
                throw new IOException("The Blackboard is unavailable.");
            var wellBore = await APIUtils.ClientWellBore.GetWellBoreByIdAsync(_selectedWellBore.MetaInfo.ID);
            if (wellBore.MetaInfo.ID != _selectedWellBore.MetaInfo.ID) throw new InvalidDataException("Wellbore identity mismatch.");
            var values = new List<(string nameSpace, string id, object value, DateTime sourceTimestamp)>();
            var now = DateTime.UtcNow;
            void Add(QueryResult query, string value)
            {
                if (query.Count == 0 || query[0].Count == 0) throw new IOException("Selection signal is not registered.");
                var node = query[0][0];
                values.Add((node.NameSpace, node.ID, value, now));
            }
            Add(_placeHolder, JsonSerializer.Serialize(wellBore));
            foreach (var (kind, id, options) in new[] {
                ("WellBoreArchitecture", _architectureID, _architectures),
                ("Trajectory", _trajectoryID, _trajectories),
                ("DrillString", _drillStringID, _drillStrings) })
            {
                if (id is not null && !options.Any(x => x.ID == id)) throw new InvalidDataException("Incompatible contextual selection.");
                Add(_contextNodes[kind], id?.ToString("D") ?? "");
            }
            if (!client.UpdateAnyVariables(values.ToArray())) throw new IOException("The Blackboard rejected the selections.");
            Snackbar.Add("Wellbore and contextual selections published to the Blackboard.", Severity.Success);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Cannot apply contextual selections");
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
}
