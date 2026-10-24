using DWIS.ContextualData.WellBoreSelector.ModelShared;

namespace WebApp.Pages;

public partial class SelectorMain
{
    private static Task<IEnumerable<T>> Search<T>(IEnumerable<T>? options, string? text,
        Func<T, string?> label, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        return Task.FromResult(SelectorSearch.Filter(options, text, label));
    }

    private Task<IEnumerable<DWIS.ContextualData.WellBoreSelector.ModelShared.Field>> SearchFieldsAsync(string text, CancellationToken token) =>
        Search(Fields?.Values.Where(x => x.MetaInfo?.ID != Guid.Empty), text, x => x.Name, token);

    private Task<IEnumerable<Cluster>> SearchClustersAsync(string text, CancellationToken token) =>
        Search(Clusters?.Values.Where(x => _selectedField?.MetaInfo is not null && x.FieldID == _selectedField.MetaInfo.ID && !x.IsSingleWell), text, x => x.Name, token);

    private Task<IEnumerable<Well>> SearchWellsAsync(string text, CancellationToken token) =>
        Search(Wells?.Values.Where(x => WellSelectionFilter.IsVisible(x, _selectedField?.MetaInfo?.ID, _selectedCluster?.MetaInfo?.ID, Clusters)), text, x => x.Name, token);

    private Task<IEnumerable<WellBore>> SearchWellBoresAsync(string text, CancellationToken token) =>
        Search(WellBores?.Values.Where(x => _selectedWell?.MetaInfo is not null && x.WellID == _selectedWell.MetaInfo.ID), text, x => x.Name, token);

    private static string OptionLabel(Guid? id, List<ContextOption> options) =>
        id is null ? "" : options.FirstOrDefault(x => x.ID == id) is ContextOption option
            ? option.Name : "";

    private static Task<IEnumerable<Guid?>> SearchOptionsAsync(List<ContextOption> options, string text, CancellationToken token) =>
        Search(options.Select(x => (Guid?)x.ID), text, id =>
        {
            var option = options.First(x => x.ID == id);
            return $"{option.Name} {option.ID}";
        }, token);

    private Task<IEnumerable<Guid?>> SearchArchitecturesAsync(string text, CancellationToken token) => SearchOptionsAsync(_architectures, text, token);
    private Task<IEnumerable<Guid?>> SearchTrajectoriesAsync(string text, CancellationToken token) => SearchOptionsAsync(_trajectories, text, token);
    private Task<IEnumerable<Guid?>> SearchDrillStringsAsync(string text, CancellationToken token) => SearchOptionsAsync(_drillStrings, text, token);
}
