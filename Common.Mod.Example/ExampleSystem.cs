using Common.Mod.Common.Config;
using Common.Mod.Example.BlockBehaviors;
using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace Common.Mod.Example;

[UsedImplicitly]
public class ExampleSystem : System<ExampleSystem>
{
    public override string ModId() => "example";
    public override string ModVersion() => "12.34.56";
    public override string ModName() => "Example Mod";

    public override bool ShouldLoad(EnumAppSide forSide) => true;

    protected override void RegisterConfigs(IConfigSystem configSystem)
    {
        configSystem.Register<ExampleCommonConfig>();
        configSystem.Register<ExampleServerConfig>();
        configSystem.Register<ExampleClientConfig>();
    }

    protected override void RegisterClasses(ICoreAPI api)
    {
        base.RegisterClasses(api);
        RegisterBlockBehaviorClass<TestMultiblockBlockBehavior>(api, TestMultiblockBlockBehavior.RegistryId);
    }
}
