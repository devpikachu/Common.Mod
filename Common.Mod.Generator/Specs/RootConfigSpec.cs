using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Common.Mod.Generator.Specs;

public record RootConfigSpec : ConfigSpec
{
    public string ClassNamespace { get; [UsedImplicitly] set; } = string.Empty;
    public string Version { get; [UsedImplicitly] set; } = string.Empty;
    public bool Synchronize { get; [UsedImplicitly] set; }
    public ImmutableArray<ConfigSpec> Nestable { get; [UsedImplicitly] set; }
}
