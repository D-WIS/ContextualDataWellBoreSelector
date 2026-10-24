# Well selector regression tests
Run dotnet test DWIS.ContextualData.WellBoreSelector.sln -c Release from the repository root.
Tests cover single-well field ownership through proxy clusters, missing parent links,
no selected field, and ordinary clustered well filtering. No network or database is used.

SelectorSearchTests cover case-insensitive substring matching, empty/no-match searches and
retention of field scoping when text filtering is applied.
