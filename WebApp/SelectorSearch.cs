namespace WebApp;

public static class SelectorSearch
{
    public static IEnumerable<T> Filter<T>(IEnumerable<T>? options, string? pattern, Func<T, string?> label)
    {
        string query = pattern?.Trim() ?? "";
        return (options ?? Enumerable.Empty<T>())
            .Where(option => query.Length == 0 || (label(option)?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false));
    }
}
