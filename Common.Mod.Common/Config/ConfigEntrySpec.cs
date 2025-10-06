using JetBrains.Annotations;

namespace Common.Mod.Common.Config;

public sealed record ConfigEntrySpec
{
    public string Name { get; [UsedImplicitly] set; } = string.Empty;
    public ConfigEntryTypeSpec Type { get; [UsedImplicitly] set; }
    public bool Nullable { get; [UsedImplicitly] set; } = false;
    public string? Nested { get; [UsedImplicitly] set; }
    public string? Description { get; [UsedImplicitly] set; }
    public object? DefaultValue { get; [UsedImplicitly] set; }
}
