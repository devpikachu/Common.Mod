using System.Diagnostics.CodeAnalysis;
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

public abstract class System<TSystem> : ModSystem, ISystem
    where TSystem : System<TSystem>
{
    public event ISystem.ServerStartHandler? ServerStart;
    public event ISystem.ClientStartHandler? ClientStart;
    public event ISystem.ServerRegisterMessageTypesHandler? ServerRegisterMessageTypes;
    public event ISystem.ClientRegisterMessageTypesHandler? ClientRegisterMessageTypes;
    public event ISystem.ServerPlayerJoinedHandler? ServerPlayerJoined;
    public event ISystem.ClientPlayerJoinedHandler? ClientPlayerJoined;

    private const string ConfigLibModId = "configlib";

    private static TSystem? _serverInstance;
    private static TSystem? _clientInstance;

    [UsedImplicitly] public readonly IContainer Container = new Container();

    public abstract string ModId();
    public abstract string ModVersion();
    public abstract string ModName();

    protected abstract void RegisterConfigs(IConfigSystem configSystem);

    [SuppressMessage("ReSharper", "SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault")]
    public static TSystem Get(EnumAppSide side)
    {
        return side switch
        {
            EnumAppSide.Server => _serverInstance!,
            EnumAppSide.Client => _clientInstance!,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
    }

    public override void StartPre(ICoreAPI api)
    {
        if (api is ICoreServerAPI)
        {
            _serverInstance = this as TSystem;
        }
        else
        {
            _clientInstance = this as TSystem;
        }

        // Core API, side & network channel
        {
            Container.RegisterInstance(api);
            Container.RegisterInstance(api is ICoreServerAPI ? EnumAppSide.Server : EnumAppSide.Client);
            Container.RegisterInstance(api.Network.RegisterChannel(ModId()));
        }

        // Logging
        {
            Container.Register<ILogger, Logger>(Reuse.Singleton);
            var logger = Container.Resolve<ILogger>();
            var side = Container.Resolve<EnumAppSide>();

            var consoleSink = new ConsoleLogSink(ModId(), side, api.Logger);
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
            var channel = Container.Resolve<INetworkChannel>() as IServerNetworkChannel ?? throw new InvalidCastException();
            ServerRegisterMessageTypes?.Invoke(channel);
        }

        // Events
        {
            api.Event.PlayerJoin += OnServerPlayerJoined;
        }

        ServerStart?.Invoke(api);
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        // Networking
        {
            var channel = Container.Resolve<INetworkChannel>() as IClientNetworkChannel ?? throw new InvalidCastException();
            ClientRegisterMessageTypes?.Invoke(channel);
        }

        // Events
        {
            api.Event.PlayerJoin += OnClientPlayerJoined;
        }

        ClientStart?.Invoke(api);
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
            configSystem.Synchronize(player);
        }

        ServerPlayerJoined?.Invoke(player);
    }

    private void OnClientPlayerJoined(IClientPlayer player)
    {
        ClientPlayerJoined?.Invoke(player);
    }
}
