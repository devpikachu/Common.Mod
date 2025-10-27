using Common.Mod.Blocks;
using Common.Mod.Common.Config;
using Common.Mod.Common.Core;
using Common.Mod.Config;
using Common.Mod.Core;
using Common.Mod.Network;
using DryIoc;
using JetBrains.Annotations;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using GamePaths = Common.Mod.Core.GamePaths;
using ILogger = Common.Mod.Common.Core.ILogger;
using IServerPlayer = Vintagestory.API.Server.IServerPlayer;

namespace Common.Mod;

public abstract class System<TSystem> : ModSystem, ISystem
    where TSystem : System<TSystem>
{
    #region Events

    public event ISystem.AssetsLoadedHandler? OnAssetsLoaded;
    public event ISystem.AssetsFinalizedHandler? OnAssetsFinalized;
    public event ISystem.ServerPlayerJoinedHandler? OnServerPlayerJoined;
    public event ISystem.ClientPlayerJoinedHandler? OnClientPlayerJoined;

    #endregion Events

    private const string ConfigLibModId = "configlib";

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public static TSystem? Instance { get; private set; }

    [UsedImplicitly] public readonly IContainer Container = new Container();

    protected System()
    {
        Instance = this as TSystem ?? throw new NullReferenceException();
    }

    #region Abstracts

    public abstract string ModId();
    public abstract string ModVersion();
    public abstract string ModName();

    public abstract override bool ShouldLoad(EnumAppSide forSide);

    protected abstract void RegisterConfigs(IConfigSystem configSystem);

    #endregion Abstracts

    #region StartPre

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

            var consoleSink = new ConsoleLoggerSink(ModId(), side, api.Logger);
            logger.AddSink(ConsoleLoggerSink.Key, consoleSink);
        }

        // File system
        {
            Container.Register<IGamePaths, GamePaths>(Reuse.Singleton);
            Container.Register<IFileSystem, FileSystem>(Reuse.Singleton);
        }

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

    #endregion StartPre

    #region Start

    public override void Start(ICoreAPI api)
    {
        RegisterClasses(api);
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        api.Event.PlayerJoin += _OnServerPlayerJoined;
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        api.Event.PlayerJoin += _OnClientPlayerJoined;
    }

    #endregion Start

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

    #region Virtuals

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

    [UsedImplicitly]
    protected virtual void RegisterClasses(ICoreAPI api)
    {
        api.RegisterBlockClass($"common:{MultiblockBlock.RegistryId}", typeof(MultiblockBlock));
    }

    #endregion Virtuals

    #region Registry Helpers

    [UsedImplicitly]
    protected void RegisterEntity<TEntity>(ICoreAPI api, string className)
        where TEntity : Entity
    {
        api.RegisterEntity(PrefixName(className), typeof(TEntity));
    }

    [UsedImplicitly]
    protected void RegisterEntityClass(ICoreAPI api, string className, EntityProperties properties)
    {
        api.RegisterEntityClass(PrefixName(className), properties);
    }

    [UsedImplicitly]
    protected void RegisterEntityBehaviorClass<TEntityBehavior>(ICoreAPI api, string className)
        where TEntityBehavior : EntityBehavior
    {
        api.RegisterEntityBehaviorClass(PrefixName(className), typeof(TEntityBehavior));
    }

    [UsedImplicitly]
    protected void RegisterBlockClass<TBlock>(ICoreAPI api, string className)
        where TBlock : Block
    {
        api.RegisterBlockClass(PrefixName(className), typeof(TBlock));
    }

    [UsedImplicitly]
    protected void RegisterBlockBehaviorClass<TBlockBehavior>(ICoreAPI api, string className)
        where TBlockBehavior : BlockBehavior
    {
        api.RegisterBlockBehaviorClass(PrefixName(className), typeof(TBlockBehavior));
    }

    [UsedImplicitly]
    protected void RegisterBlockEntityClass<TBlockEntity>(ICoreAPI api, string className)
        where TBlockEntity : BlockEntity
    {
        api.RegisterBlockEntityClass(PrefixName(className), typeof(TBlockEntity));
    }

    [UsedImplicitly]
    protected void RegisterBlockEntityBehaviorClass<TBlockEntityBehavior>(ICoreAPI api, string className)
        where TBlockEntityBehavior : BlockEntityBehavior
    {
        api.RegisterBlockEntityBehaviorClass(PrefixName(className), typeof(TBlockEntityBehavior));
    }

    [UsedImplicitly]
    protected void RegisterCropBehavior<TCropBehavior>(ICoreAPI api, string className)
        where TCropBehavior : CropBehavior
    {
        api.RegisterCropBehavior(PrefixName(className), typeof(TCropBehavior));
    }

    [UsedImplicitly]
    protected void RegisterItemClass<TItem>(ICoreAPI api, string className)
        where TItem : Item
    {
        api.RegisterItemClass(PrefixName(className), typeof(TItem));
    }

    [UsedImplicitly]
    protected void RegisterCollectibleBehaviorClass<TCollectibleBehavior>(ICoreAPI api, string className)
        where TCollectibleBehavior : CollectibleBehavior
    {
        api.RegisterCollectibleBehaviorClass(PrefixName(className), typeof(TCollectibleBehavior));
    }

    [UsedImplicitly]
    protected void RegisterMountable(ICoreAPI api, string className, GetMountableDelegate mountableInstantiator)
    {
        api.RegisterMountable(PrefixName(className), mountableInstantiator);
    }

    #endregion Registry Helpers

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

    private string PrefixName(string name) => $"{ModId()}:{name}";
}
