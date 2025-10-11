using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Common.Mod.Generator.Specs;

public record ConfigSpec
{
    public string ClassName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ImmutableArray<ConfigEntrySpec> Entries { get; set; }
}
