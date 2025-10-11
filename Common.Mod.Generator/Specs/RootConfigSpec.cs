using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Common.Mod.Generator.Specs;

public record RootConfigSpec : ConfigSpec
{
    public string ClassNamespace { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public RootConfigTypeSpec Type { get; set; }
    public ImmutableArray<ConfigSpec> Nestable { get; set; }
}
