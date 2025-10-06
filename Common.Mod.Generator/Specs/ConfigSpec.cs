using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Common.Mod.Generator.Specs;

public record ConfigSpec
{
    public string ClassName { get; [UsedImplicitly] set; } = string.Empty;
    public string? Description { get; [UsedImplicitly] set; }
    public ImmutableArray<ConfigEntrySpec> Entries { get; [UsedImplicitly] set; }
}
