using DWIS.Client.ReferenceImplementation;

namespace WebApp.Pages;

public partial class SelectorMain
{
    private readonly CancellationTokenSource _pageLifetime = new();
    private readonly SemaphoreSlim _bindingGate = new(1, 1);
    private Task? _bindingMonitor;
    private IOPCUADWISClient? _boundClient;
    private bool _blackboardReady, _choicesEdited, _restoredSelections;

    private void MarkChoicesEdited() => _choicesEdited = true;

    private async Task RefreshBlackboardBindingsAsync()
    {
        await _bindingGate.WaitAsync();
        try
        {
            var client = DWISConnector.Instance.DWISClient;
            if (client?.Connected != true)
            {
                _blackboardReady = false;
                _boundClient = null;
                return;
            }
            if (ReferenceEquals(client, _boundClient) && _blackboardReady) return;
            _blackboardReady = false;
            _placeHolder = null;
            _contextNodes.Clear();
            bool restore = !_restoredSelections && !_choicesEdited;
            await ManageSignal(restore);
            if (_placeHolder is null) throw new IOException("Cannot register the selected wellbore signal.");
            await InitializeContextSelectionsAsync(restore && !_choicesEdited);
            if (_contextNodes.Count != 3 || !ReferenceEquals(client, DWISConnector.Instance.DWISClient) || !client.Connected)
                throw new IOException("Blackboard connection changed during registration.");
            _boundClient = client;
            _blackboardReady = true;
            _restoredSelections = true;
            logger.LogInformation("Blackboard selection bindings ready");
        }
        catch (Exception ex)
        {
            _blackboardReady = false;
            _boundClient = null;
            logger.LogWarning(ex, "Cannot register Blackboard selections; will retry without changing your choices");
        }
        finally { _bindingGate.Release(); }
    }

    private async Task MonitorBlackboardAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
            while (await timer.WaitForNextTickAsync(cancellationToken))
                await InvokeAsync(async () =>
                {
                    await RefreshBlackboardBindingsAsync();
                    StateHasChanged();
                });
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
    }

    public async ValueTask DisposeAsync()
    {
        _pageLifetime.Cancel();
        if (_bindingMonitor is not null) await _bindingMonitor;
        _pageLifetime.Dispose();
    }
}
