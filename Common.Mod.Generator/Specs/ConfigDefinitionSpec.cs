using System.Collections.Immutable;

namespace Common.Mod.Generator.Specs;

public record ConfigDefinitionSpec
{
    public ImmutableArray<RootConfigSpec> RootConfigs { get; set; }
    public ImmutableArray<ConfigSpec>? Configs { get; set; }
}
