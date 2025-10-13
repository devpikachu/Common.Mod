using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Common.Mod.Generator.Specs;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Members)]
public record ConfigSpec
{
    public string ClassName { get; set; } = null!;
    public string ClassNamespace { get; set; } = null!;
    public string? Description { get; set; }
    public ImmutableArray<ConfigEntrySpec> Entries { get; set; }
}
