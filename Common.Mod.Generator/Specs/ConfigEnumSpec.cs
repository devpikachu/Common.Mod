using System.Collections.Immutable;

namespace Common.Mod.Generator.Specs;

public record ConfigEnumSpec
{
    public string EnumName { get; set; } = null!;
    public string EnumNamespace { get; set; } = null!;
    public string? Description { get; set; } = null;
    public ImmutableArray<string> Values { get; set; }
}
