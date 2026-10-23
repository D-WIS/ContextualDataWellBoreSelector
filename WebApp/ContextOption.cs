namespace WebApp;

/// <summary>A lightweight menu projection; complete records remain owned by their source services.</summary>
public sealed record ContextOption(Guid ID, string Name);
