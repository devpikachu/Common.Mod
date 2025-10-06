using Common.Mod.Config;
using Common.Mod.Core;
using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace Common.Mod.Example;

[UsedImplicitly]
public class ExampleSystem : System<ExampleConfig, ExampleConfig, ExampleConfig>
{
    public override string ModId() => "example";
    public override string ModVersion() => "12.34.56";

    public override bool ShouldLoad(EnumAppSide forSide) => true;
}
