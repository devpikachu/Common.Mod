using Common.Mod.Common.Config;
using Common.Mod.Common.Core;
using Common.Mod.Config;
using DryIoc;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using ILogger = Common.Mod.Common.Core.ILogger;

namespace Common.Mod.Core;

public abstract class System<TCommonConfig, TServerConfig, TClientConfig> : ModSystem, ISystem
    where TCommonConfig : IRootConfig, new()
    where TServerConfig : IRootConfig, new()
    where TClientConfig : IRootConfig, new()
{
    [UsedImplicitly] protected readonly Container Container = new();

    public abstract string ModId();
    public abstract string ModVersion();

    public override void StartPre(ICoreAPI api)
    {
        // Logging
        {
            Container.Register<ILogger, Logger>(Reuse.Singleton);
            var logger = Container.Resolve<ILogger>();

            var consoleSink = new ConsoleLogSink(api.Logger);
            logger.AddSink(ConsoleLogSink.Key, consoleSink);
        }

        Container.RegisterInstance(api);
        Container.RegisterInstance<ISystem>(this);
        Container.Register<IFileSystem, FileSystem>();

        // Config
        {
            Container.Register<IConfigSystem<TCommonConfig, TServerConfig, TClientConfig>, ConfigSystem<TCommonConfig, TServerConfig, TClientConfig>>();
            var configSystem = Container.Resolve<IConfigSystem<TCommonConfig, TServerConfig, TClientConfig>>();
            configSystem.Load();
        }

        // Server-specific
        if (api is ICoreServerAPI serverApi)
        {
            Container.RegisterInstance(serverApi);
        }

        // Client-specific
        if (api is ICoreClientAPI clientApi)
        {
            Container.RegisterInstance(clientApi);
        }
    }
}

[UsedImplicitly]
public abstract class System<TCommonConfig> : System<TCommonConfig, DummyConfig, DummyConfig>
    where TCommonConfig : IRootConfig, new();
