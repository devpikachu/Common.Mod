namespace Common.Mod.Generator.Specs;

public record RootConfigSpec : ConfigSpec
{
    public string Version { get; set; } = null!;
    public RootConfigTypeSpec Type { get; set; } = RootConfigTypeSpec.Common;
}
