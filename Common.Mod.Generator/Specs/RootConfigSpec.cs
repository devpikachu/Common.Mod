using JetBrains.Annotations;

namespace Common.Mod.Generator.Specs;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Members)]
public record RootConfigSpec : ConfigSpec
{
    public string Version { get; set; } = null!;
    public RootConfigTypeSpec Type { get; set; } = RootConfigTypeSpec.Common;
}
