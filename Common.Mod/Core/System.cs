using Common.Mod.Common.Config;
using Common.Mod.Common.Core;
using Common.Mod.Config;
using Common.Mod.Network;
using DryIoc;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using ILogger = Common.Mod.Common.Core.ILogger;
using IServerPlayer = Vintagestory.API.Server.IServerPlayer;

namespace Common.Mod.Core;

public abstract class System<TSystem> : ModSystem, ISystem
    where TSystem : System<TSystem>
{
    public event ISystem.AssetsLoadedHandler? OnAssetsLoaded;
    public event ISystem.AssetsFinalizedHandler? OnAssetsFinalized;
    public event ISystem.ServerPlayerJoinedHandler? OnServerPlayerJoined;
    public event ISystem.ClientPlayerJoinedHandler? OnClientPlayerJoined;

    private const string ConfigLibModId = "configlib";

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public static TSystem? Instance { get; private set; }

    [UsedImplicitly] public readonly IContainer Container = new Container();

    protected System()
    {
        Instance = this as TSystem ?? throw new NullReferenceException();
    }

    public abstract string ModId();
    public abstract string ModVersion();
    public abstract string ModName();

    protected abstract void RegisterConfigs(IConfigSystem configSystem);

    public override void StartPre(ICoreAPI api)
    {
        // Core API, side, ISystem
        {
            Container.RegisterInstance(api);
            Container.RegisterInstance(api is ICoreServerAPI ? EnumAppSide.Server : EnumAppSide.Client);
            Container.RegisterInstance<ISystem>(this);
        }

        // Logging
        {
            Container.Register<ILogger, Logger>(Reuse.Singleton);
            var logger = Container.Resolve<ILogger>();
            var side = Container.Resolve<EnumAppSide>();

            var consoleSink = new ConsoleLogSink(ModId(), side, api.Logger);
            logger.AddSink(ConsoleLogSink.Key, consoleSink);
        }

        // File system
        Container.Register<IFileSystem, FileSystem>(Reuse.Singleton);

        // Networking
        {
            if (api is ICoreServerAPI serverApi)
            {
                var channel = serverApi.Network.RegisterChannel(ModId());
                Container.RegisterInstance(channel);
                Container.RegisterMapping<INetworkChannel, IServerNetworkChannel>();
                ServerRegisterNetworkMessages(channel);
            }

            if (api is ICoreClientAPI clientApi)
            {
                var channel = clientApi.Network.RegisterChannel(ModId());
                Container.RegisterInstance(channel);
                Container.RegisterMapping<INetworkChannel, IClientNetworkChannel>();
                ClientRegisterNetworkMessages(channel);
            }
        }

        // Translations
        {
            if (api is ICoreServerAPI)
            {
                Container.RegisterInstance<ITranslations>(new Translations(ModId()));
            }

            if (api is ICoreClientAPI)
            {
                Container.RegisterInstance<ITranslations>(new Translations(ModId(), Lang.CurrentLocale));
            }
        }

        // Config
        {
            Container.Register<IConfigUi, ConfigUi>(Reuse.Singleton);
            Container.Register<IConfigSystem, ConfigSystem>(Reuse.Singleton);
            var configSystem = Container.Resolve<IConfigSystem>();

            RegisterConfigs(configSystem);

            configSystem.Load();
            configSystem.Save();

            if (api is ICoreClientAPI && api.ModLoader.IsModEnabled(ConfigLibModId))
            {
                configSystem.Render();
            }
        }

        // Server/Client StartPre
        {
            if (api is ICoreServerAPI serverApi)
            {
                ServerStartPre(serverApi);
            }

            if (api is ICoreClientAPI clientApi)
            {
                ClientStartPre(clientApi);
            }
        }
    }

    public override void AssetsLoaded(ICoreAPI api)
    {
        OnAssetsLoaded?.Invoke();
    }

    public override void AssetsFinalize(ICoreAPI api)
    {
        OnAssetsFinalized?.Invoke();
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        api.Event.PlayerJoin += _OnServerPlayerJoined;
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        api.Event.PlayerJoin += _OnClientPlayerJoined;
    }

    public override void Dispose()
    {
        var api = Container.Resolve<ICoreAPI>();

        if (api is ICoreServerAPI serverApi)
        {
            serverApi.Event.PlayerJoin -= _OnServerPlayerJoined;
            return;
        }

        if (api is ICoreClientAPI clientApi)
        {
            clientApi.Event.PlayerJoin -= _OnClientPlayerJoined;
        }
    }

    [UsedImplicitly]
    protected virtual void ServerRegisterNetworkMessages(IServerNetworkChannel channel)
    {
        channel.RegisterMessageType<ConfigPacket>();
    }

    [UsedImplicitly]
    protected virtual void ClientRegisterNetworkMessages(IClientNetworkChannel channel)
    {
        channel.RegisterMessageType<ConfigPacket>();
    }

    [UsedImplicitly]
    protected virtual void ServerStartPre(ICoreServerAPI api)
    {
    }

    [UsedImplicitly]
    protected virtual void ClientStartPre(ICoreClientAPI api)
    {
    }

    private void _OnServerPlayerJoined(IServerPlayer player)
    {
        // Config synchronization
        {
            var configSystem = Container.Resolve<IConfigSystem>();
            configSystem.Synchronize(player);
        }

        OnServerPlayerJoined?.Invoke(player);
    }

    private void _OnClientPlayerJoined(IClientPlayer player)
    {
        OnClientPlayerJoined?.Invoke(player);
    }
}
