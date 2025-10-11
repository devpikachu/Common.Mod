using Common.Mod.Common.Config;
using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace Common.Mod.Example;

[UsedImplicitly]
public class ExampleSystem : Core.System
{
    public override string ModId() => "example";
    public override string ModVersion() => "12.34.56";

    public override bool ShouldLoad(EnumAppSide forSide) => true;

    protected override void RegisterConfigs(IConfigSystem configSystem)
    {
        configSystem.Register<ExampleConfig>();
    }
}
