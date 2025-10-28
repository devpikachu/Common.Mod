using Common.Mod.Common.Config;
using Common.Mod.Example.BlockBehaviors;
using Common.Mod.Example.Commands;
using DryIoc;
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

    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        Container.Register<DebugCommand>();

        // Initialize classes
        {
            // ReSharper disable UnusedVariable
            var debugCommand = Container.Resolve<DebugCommand>();
            // ReSharper restore UnusedVariable
        }
    }

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
