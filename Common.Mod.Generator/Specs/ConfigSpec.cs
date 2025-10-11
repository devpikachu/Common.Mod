using System.Collections.Immutable;

namespace Common.Mod.Generator.Specs;

public record ConfigSpec
{
    public string ClassName { get; set; } = null!;
    public string ClassNamespace { get; set; } = null!;
    public string? Description { get; set; } = null;
    public ImmutableArray<ConfigEntrySpec> Entries { get; set; }
}
