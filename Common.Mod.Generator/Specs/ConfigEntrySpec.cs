using JetBrains.Annotations;

namespace Common.Mod.Generator.Specs;

public sealed record ConfigEntrySpec
{
    public string Name { get; set; } = string.Empty;
    public ConfigEntryTypeSpec Type { get; set; }
    public bool Nullable { get; set; } = false;
    public string? Nested { get; set; }
    public string? Description { get; set; }
    public object? DefaultValue { get; set; }
}
