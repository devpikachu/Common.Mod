using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Common.Mod.Generator.Specs;

[UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Members)]
public record ConfigEnumSpec
{
    public string EnumName { get; set; } = null!;
    public string EnumNamespace { get; set; } = null!;
    public string? Description { get; set; }
    public ImmutableArray<string> Values { get; set; }
}
