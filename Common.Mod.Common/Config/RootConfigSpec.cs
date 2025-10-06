using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Common.Mod.Common.Config;

public sealed record RootConfigSpec : ConfigSpec
{
    public string ClassNamespace { get; [UsedImplicitly] set; } = string.Empty;
    public string Version { get; [UsedImplicitly] set; } = string.Empty;
    public ImmutableArray<ConfigSpec> Nestable { get; [UsedImplicitly] set; }
}
