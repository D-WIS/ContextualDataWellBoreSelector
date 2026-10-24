# Well selector regression tests
Run dotnet test DWIS.ContextualData.WellBoreSelector.sln -c Release from the repository root.
Tests cover single-well field ownership through proxy clusters, missing parent links,
no selected field, and ordinary clustered well filtering. No network or database is used.

SelectorSearchTests cover case-insensitive substring matching, empty/no-match searches and
retention of field scoping when text filtering is applied.

## Blackboard reconnection
The host retries an unavailable or disconnected Blackboard every five seconds and disposes
replaced sessions. Each open selector page retries registration of the wellbore and three
context-selection signals, including after reconnect. A warning remains visible until all
bindings are ready. No selections are published automatically. Unsaved dropdown choices
survive connection loss; Blackboard selections are restored only on the first successful
binding when the user has not already edited the page. Closing the page cancels its monitor.
Regression tests cover startup failures, lost connections, disposal, registration failures,
re-binding and preservation of unsaved choices.
