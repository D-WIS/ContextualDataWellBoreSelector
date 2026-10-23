using DWIS.ContextualData.WellBoreSelector.ModelShared;
using NUnit.Framework;

namespace WebApp.Tests;

public class WellSelectionFilterTests
{
    [Test]
    public void SingleWellsAreScopedToTheirProxyClustersField()
    {
        var field = Guid.NewGuid(); var otherField = Guid.NewGuid();
        var ownProxy = Guid.NewGuid(); var otherProxy = Guid.NewGuid();
        var clusters = new Dictionary<Guid, Cluster> {
            [ownProxy] = new() { FieldID = field, IsSingleWell = true },
            [otherProxy] = new() { FieldID = otherField, IsSingleWell = true }
        };
        Assert.That(WellSelectionFilter.IsVisible(new Well { ClusterID = ownProxy, IsSingleWell = true }, field, null, clusters), Is.True);
        Assert.That(WellSelectionFilter.IsVisible(new Well { ClusterID = otherProxy, IsSingleWell = true }, field, null, clusters), Is.False);
        Assert.That(WellSelectionFilter.IsVisible(new Well { ClusterID = ownProxy, IsSingleWell = true }, null, null, clusters), Is.False);
        Assert.That(WellSelectionFilter.IsVisible(new Well { ClusterID = Guid.NewGuid(), IsSingleWell = true }, field, null, clusters), Is.False);
        Assert.That(WellSelectionFilter.IsVisible(new Well { IsSingleWell = true }, field, null, clusters), Is.False);
    }

    [Test]
    public void ClusteredWellsRequireBothSelectedClusterAndField()
    {
        var field = Guid.NewGuid(); var cluster = Guid.NewGuid();
        var clusters = new Dictionary<Guid, Cluster> { [cluster] = new() { FieldID = field } };
        var well = new Well { ClusterID = cluster };
        Assert.That(WellSelectionFilter.IsVisible(well, field, cluster, clusters), Is.True);
        Assert.That(WellSelectionFilter.IsVisible(well, field, null, clusters), Is.False);
        Assert.That(WellSelectionFilter.IsVisible(well, field, Guid.NewGuid(), clusters), Is.False);
        Assert.That(WellSelectionFilter.IsVisible(well, Guid.NewGuid(), cluster, clusters), Is.False);
    }
}
