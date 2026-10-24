using System.Reflection;
using DWIS.API.DTO;
using DWIS.Client.ReferenceImplementation;
using NUnit.Framework;
using WebApp.Pages;
using Microsoft.Extensions.Logging.Abstractions;

namespace WebApp.Tests;

[NonParallelizable]
public class BlackboardReconnectTests
{
    public interface IDisposableClient : IOPCUADWISClient, IDisposable { }

    public class ClientProxy : DispatchProxy
    {
        public bool Connected;
        public int Disposals, Registrations;
        public bool RejectRegistration;
        protected override object? Invoke(MethodInfo? method, object?[]? args)
        {
            switch (method!.Name)
            {
                case "get_Connected": return Connected;
                case "Dispose": Disposals++; Connected = false; return null;
                case "GetQueryResult":
                    Registrations++;
                    if (RejectRegistration) throw new IOException("offline");
                    var result = new QueryResult();
                    result.Add(new QueryResultRow { Items = new List<NodeIdentifier> { new() { NameSpace = "test", ID = "selection" } } });
                    return result;
                default: return method.ReturnType == typeof(bool) ? false : null;
            }
        }
    }
    private static IOPCUADWISClient Client(bool connected)
    {
        var client = DispatchProxy.Create<IDisposableClient, ClientProxy>();
        ((ClientProxy)(object)client).Connected = connected;
        return client;
    }
    private static async Task UntilAsync(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        while (!condition()) await Task.Delay(5, timeout.Token);
    }

    [Test]
    public async Task RetriesStartupFailureAndLostConnectionAndDisposesSessions()
    {
        var failed = Client(false); var first = Client(true); var replacement = Client(true);
        int attempts = 0;
        var connector = new DWISConnector(() => Interlocked.Increment(ref attempts) switch
        {
            1 => throw new IOException("not started"), 2 => failed, 3 => first, _ => replacement
        }, retryInterval: TimeSpan.FromMilliseconds(10));
        using var stop = new CancellationTokenSource();
        var run = connector.RunAsync(stop.Token);
        try
        {
            await UntilAsync(() => ReferenceEquals(connector.DWISClient, first));
            Assert.That(((ClientProxy)(object)failed).Disposals, Is.EqualTo(1));
            ((ClientProxy)(object)first).Connected = false;
            await UntilAsync(() => ReferenceEquals(connector.DWISClient, replacement));
            Assert.That(((ClientProxy)(object)first).Disposals, Is.EqualTo(1));
        }
        finally { stop.Cancel(); await run; }
        Assert.That(((ClientProxy)(object)replacement).Disposals, Is.EqualTo(1));
        Assert.That(connector.DWISClient, Is.Null);
    }

    [Test]
    public async Task RebindsAfterReconnectAndRegistrationFailureWithoutOverwritingChoices()
    {
        var singleton = DWISConnector.Instance;
        var previous = singleton.DWISClient;
        var page = new SelectorMain();
        var type = typeof(SelectorMain);
        void Set(string name, object? value) => type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(page, value);
        object? Get(string name) => type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(page);
        Task Refresh() => (Task)type.GetMethod("RefreshBlackboardBindingsAsync", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(page, null)!;
        type.GetProperty("logger", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(page, NullLogger<SelectorMain>.Instance);
        var selected = new DWIS.ContextualData.WellBoreSelector.ModelShared.WellBore();
        var architecture = Guid.NewGuid(); var trajectory = Guid.NewGuid(); var drillString = Guid.NewGuid();
        Set("_selectedWellBore", selected); Set("_architectureID", architecture);
        Set("_trajectoryID", trajectory); Set("_drillStringID", drillString); Set("_choicesEdited", true);
        var data = (SelectedWellboreData)Get("_selectedWellboreData")!;
        data.WellBore.Manifest = new DWIS.API.DTO.ManifestFile();
        data.WellBore.SparQLQuery = "SELECT ?selection WHERE {}";
        data.WellBore.SparQLVariables = new List<string> { "selection" };
        try
        {
            singleton.DWISClient = Client(false);
            await Refresh(); Assert.That(Get("_blackboardReady"), Is.False);
            var first = Client(true); singleton.DWISClient = first;
            ((ClientProxy)(object)first).RejectRegistration = true;
            await Refresh(); Assert.That(Get("_blackboardReady"), Is.False);
            ((ClientProxy)(object)first).RejectRegistration = false;
            await Refresh(); Assert.That(Get("_blackboardReady"), Is.True);
            int count = ((ClientProxy)(object)first).Registrations;
            await Refresh(); Assert.That(((ClientProxy)(object)first).Registrations, Is.EqualTo(count));
            var second = Client(true); singleton.DWISClient = second;
            await Refresh(); Assert.That(Get("_blackboardReady"), Is.True);
            Assert.That(((ClientProxy)(object)second).Registrations, Is.EqualTo(4));
            Assert.That(Get("_selectedWellBore"), Is.SameAs(selected));
            Assert.That(Get("_architectureID"), Is.EqualTo(architecture));
            Assert.That(Get("_trajectoryID"), Is.EqualTo(trajectory));
            Assert.That(Get("_drillStringID"), Is.EqualTo(drillString));
        }
        finally { singleton.DWISClient = previous; await page.DisposeAsync(); }
    }
}
