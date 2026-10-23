using DWIS.ContextualData.WellBoreSelector.ModelShared;

namespace WebApp;

public static class WellSelectionFilter
{
    public static bool IsVisible(Well? well, Guid? fieldID, Guid? clusterID,
        IReadOnlyDictionary<Guid, Cluster>? clusters)
    {
        // Single wells still have a proxy cluster that owns their field association.
        if (fieldID is null || well?.ClusterID is not Guid ownerID || clusters is null
            || !clusters.TryGetValue(ownerID, out var owner) || owner.FieldID != fieldID)
            return false;
        return well.IsSingleWell || ownerID == clusterID;
    }
}
