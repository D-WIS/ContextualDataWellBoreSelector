using NUnit.Framework;

namespace WebApp.Tests;

public class SelectorSearchTests
{
    [TestCase("", 3)]
    [TestCase("  ", 3)]
    [TestCase("u3", 2)]
    [TestCase("AGGregated", 1)]
    [TestCase("not present", 0)]
    public void SearchUsesCaseInsensitiveSubstringMatching(string pattern, int count)
    {
        string[] names = ["U3 Truncated", "U3 Aggregated Trajectory", "U4 Main"];
        Assert.That(SelectorSearch.Filter(names, pattern, x => x).Count(), Is.EqualTo(count));
    }

    [Test]
    public void SearchDoesNotReintroduceChoicesExcludedByHierarchy()
    {
        var field = Guid.NewGuid(); var otherField = Guid.NewGuid();
        var ownCluster = Guid.NewGuid(); var otherCluster = Guid.NewGuid();
        var clusters = new Dictionary<Guid, DWIS.ContextualData.WellBoreSelector.ModelShared.Cluster> {
            [ownCluster] = new() { FieldID = field }, [otherCluster] = new() { FieldID = otherField }
        };
        DWIS.ContextualData.WellBoreSelector.ModelShared.Well[] wells = [
            new() { Name = "Match here", ClusterID = ownCluster, IsSingleWell = true },
            new() { Name = "Match elsewhere", ClusterID = otherCluster, IsSingleWell = true }
        ];
        var visible = wells.Where(w => WellSelectionFilter.IsVisible(w, field, null, clusters));
        Assert.That(SelectorSearch.Filter(visible, "match", w => w.Name).Select(w => w.Name), Is.EqualTo(new[] { "Match here" }));
    }
}
