using Common.Mod.Common.Config;
using Common.Mod.Common.Core;
using Common.Mod.Config;
using ConfigLib;
using DryIoc;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using ILogger = Common.Mod.Common.Core.ILogger;
using IServerPlayer = Vintagestory.API.Server.IServerPlayer;

namespace Common.Mod.Core;

public abstract class System : ModSystem, ISystem
{
    public event ISystem.ServerRegisterMessageTypesHandler? ServerRegisterMessageTypes;
    public event ISystem.ClientRegisterMessageTypesHandler? ClientRegisterMessageTypes;
    public event ISystem.ServerPlayerJoinedHandler? ServerPlayerJoined;
    public event ISystem.ClientPlayerJoinedHandler? ClientPlayerJoined;

    private const string ConfigLibModId = "configlib";

    [UsedImplicitly] protected readonly Container Container = new();

    public abstract string ModId();
    public abstract string ModVersion();
    public abstract string ModName();

    protected abstract void RegisterConfigs(IConfigSystem configSystem);

    public override void StartPre(ICoreAPI api)
    {
        // Core API & side
        {
            Container.RegisterInstance(api);
            Container.RegisterInstance(api is ICoreServerAPI ? SystemSide.Server : SystemSide.Client);
        }

        // Logging
        {
            Container.Register<ILogger, Logger>(Reuse.Singleton);
            var logger = Container.Resolve<ILogger>();

            var consoleSink = new ConsoleLogSink(ModId(), api.Logger);
            logger.AddSink(ConsoleLogSink.Key, consoleSink);
        }

        Container.RegisterInstance<ISystem>(this);
        Container.Register<IFileSystem, FileSystem>();

        // ConfigLib
        {
            if (api.ModLoader.IsModEnabled(ConfigLibModId))
            {
                var configLibSystem = api.ModLoader.GetModSystem<ConfigLibModSystem>();
                Container.RegisterInstance(configLibSystem);
            }
        }

        // Config
        {
            Container.Register<IConfigSystem, ConfigSystem>(Reuse.Singleton);
            var configSystem = Container.Resolve<IConfigSystem>();

            RegisterConfigs(configSystem);

            configSystem.Load();
            configSystem.Save();
            configSystem.Render();
        }
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        // Networking
        {
            var channel = api.Network.RegisterChannel(ModId());
            ServerRegisterMessageTypes?.Invoke(channel);
            Container.RegisterInstance(channel);
        }

        // Events
        {
            api.Event.PlayerJoin += OnServerPlayerJoined;
        }
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        // Networking
        {
            var channel = api.Network.RegisterChannel(ModId());
            ClientRegisterMessageTypes?.Invoke(channel);
            Container.RegisterInstance(channel);
        }

        // Events
        {
            api.Event.PlayerJoin += OnClientPlayerJoined;
        }
    }

    public override void Dispose()
    {
        var api = Container.Resolve<ICoreAPI>();

        if (api is ICoreServerAPI serverApi)
        {
            serverApi.Event.PlayerJoin -= OnServerPlayerJoined;
            return;
        }

        if (api is ICoreClientAPI clientApi)
        {
            clientApi.Event.PlayerJoin -= OnClientPlayerJoined;
        }
    }

    private void OnServerPlayerJoined(IServerPlayer player)
    {
        // Config synchronization
        {
            var configSystem = Container.Resolve<IConfigSystem>();
            var channel = Container.Resolve<IServerNetworkChannel>();
            channel.SendPacket(configSystem.Synchronize<ConfigPacket>(), player);
        }

        ServerPlayerJoined?.Invoke(player);
    }

    private void OnClientPlayerJoined(IClientPlayer player)
    {
        ClientPlayerJoined?.Invoke(player);
    }
}
