using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Mod.Common.Config;
using Common.Mod.Common.Core;
using Common.Mod.Exceptions;
using Common.Mod.Network;
using ConfigLib;
using ImGuiNET;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using ILogger = Common.Mod.Common.Core.ILogger;

namespace Common.Mod.Config;

public class ConfigSystem : IConfigSystem
{
    public event IConfigSystem.UpdatedHandler? Updated;

    private readonly ICoreAPI _api;
    private readonly EnumAppSide _side;
    private readonly ILogger _logger;
    private readonly ISystem _system;
    private readonly IFileSystem _fileSystem;
    private readonly IConfigUi _configUi;

    private readonly IServerNetworkChannel? _serverChannel;
    private readonly IClientNetworkChannel? _clientChannel;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Dictionary<RootConfigType, Type> _configTypes;
    private readonly Dictionary<RootConfigType, IRootConfig> _configs;

    private bool _canEditServerConfig;

    public ConfigSystem(
        ICoreAPI api,
        EnumAppSide side,
        ILogger logger,
        ISystem system,
        IFileSystem fileSystem,
        INetworkChannel channel,
        IConfigUi configUi
    )
    {
        _api = api;
        _side = side;
        _logger = logger.Named("ConfigSystem");
        _system = system;
        _fileSystem = fileSystem;
        _configUi = configUi;

        if (channel is IServerNetworkChannel serverChannel)
        {
            _serverChannel = serverChannel;
            _serverChannel.SetMessageHandler<ConfigPacket>(Synchronize);
        }

        if (channel is IClientNetworkChannel clientChannel)
        {
            _clientChannel = clientChannel;
            _clientChannel.SetMessageHandler<ConfigPacket>(Synchronize);
        }

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = true
        };
        _configTypes = new Dictionary<RootConfigType, Type>();
        _configs = new Dictionary<RootConfigType, IRootConfig>();

        _system.OnClientPlayerJoined += OnClientPlayerJoined;
    }

    public void Register<TRootConfig>()
        where TRootConfig : class, IRootConfig, new()
    {
        var config = new TRootConfig();
        var type = config.Type();

        _configTypes.Add(type, typeof(TRootConfig));
        _configs.Add(type, config);
    }

    public TRootConfig Get<TRootConfig>(RootConfigType type)
        where TRootConfig : class, IRootConfig, new()
    {
        return _configs[type] as TRootConfig ?? throw new NullReferenceException();
    }

    public TRootConfig GetCommon<TRootConfig>()
        where TRootConfig : class, IRootConfig, new() => Get<TRootConfig>(RootConfigType.Common);

    public TRootConfig GetServer<TRootConfig>()
        where TRootConfig : class, IRootConfig, new() => Get<TRootConfig>(RootConfigType.Server);

    public TRootConfig GetClient<TRootConfig>()
        where TRootConfig : class, IRootConfig, new() => Get<TRootConfig>(RootConfigType.Client);

    public void MutateCommon<TRootConfig>(System.Func<TRootConfig, TRootConfig> mutator) where TRootConfig : class, IRootConfig, new()
    {
        if (_side is not EnumAppSide.Server)
        {
            throw new InvalidSideException(_side);
        }

        _configs[RootConfigType.Common] = mutator.Invoke(GetCommon<TRootConfig>());
        Save(RootConfigType.Common);
        SendServerClientSync();
    }

    public void MutateServer<TRootConfig>(System.Func<TRootConfig, TRootConfig> mutator) where TRootConfig : class, IRootConfig, new()
    {
        if (_side is not EnumAppSide.Server)
        {
            throw new InvalidSideException(_side);
        }

        _configs[RootConfigType.Server] = mutator.Invoke(GetServer<TRootConfig>());
        Save(RootConfigType.Server);
        SendServerClientSync();
    }

    public void MutateClient<TRootConfig>(System.Func<TRootConfig, TRootConfig> mutator) where TRootConfig : class, IRootConfig, new()
    {
        if (_side is not EnumAppSide.Client)
        {
            throw new InvalidSideException(_side);
        }

        _configs[RootConfigType.Client] = mutator.Invoke(GetClient<TRootConfig>());
        Save(RootConfigType.Client);
        Updated?.Invoke(RootConfigType.Client);
    }

    public void Load()
    {
        _logger.Verbose("Loading all available configuration types");
        var stopwatch = Stopwatch.StartNew();

        if (_configs.ContainsKey(RootConfigType.Common))
        {
            Load(RootConfigType.Common);
        }

        if (_configs.ContainsKey(RootConfigType.Server) && _side == EnumAppSide.Server)
        {
            Load(RootConfigType.Server);
        }

        if (_configs.ContainsKey(RootConfigType.Client) && _side == EnumAppSide.Client)
        {
            Load(RootConfigType.Client);
        }

        stopwatch.Stop();
        _logger.Verbose("Done loading all available configuration types in {0} ms", stopwatch.ElapsedMilliseconds);
    }

    public void Save()
    {
        _logger.Verbose("Saving all available configuration types");
        var stopwatch = Stopwatch.StartNew();

        if (_configs.ContainsKey(RootConfigType.Common))
        {
            Save(RootConfigType.Common);
        }

        if (_configs.ContainsKey(RootConfigType.Server) && _side == EnumAppSide.Server)
        {
            Save(RootConfigType.Server);
        }

        if (_configs.ContainsKey(RootConfigType.Client) && _side == EnumAppSide.Client)
        {
            Save(RootConfigType.Client);
        }

        stopwatch.Stop();
        _logger.Verbose("Done saving all available configuration types in {0} ms", stopwatch.ElapsedMilliseconds);
    }

    public void Synchronize<TConfigPacket>(IServerPlayer player, TConfigPacket packet)
        where TConfigPacket : class, new()
    {
        _logger.Debug("Received configuration packet from player {0} (ID {1}, IP {2})", player.PlayerName, player.PlayerUID, player.IpAddress);

        if (player.Privileges is null || player.Privileges.Length == 0 || !player.Privileges.Contains(Privilege.controlserver))
        {
            _logger.Warning(
                "Player {0} (ID {1}, IP {2}) attempted to remotely change server config, but lacks permissions. This likely indicates that the player sent a manually crafted synchronization packet to the server",
                player.PlayerName, player.PlayerUID, player.IpAddress);
            return;
        }

        Synchronize(packet);
    }

    public void Synchronize<TConfigPacket>(TConfigPacket packet)
        where TConfigPacket : class, new()
    {
        _logger.Debug("Received configuration packet");

        if (packet is not ConfigPacket configPacket)
        {
            throw new InvalidCastException($"Received packet type does not match {nameof(ConfigPacket)}");
        }

        switch (configPacket.Operation)
        {
            case ConfigPacketOperation.ServerClientSync:
            {
                _logger.Debug("Processing server-to-client synchronization packet");

                if (_side is not EnumAppSide.Client)
                {
                    throw new InvalidSideException(_side);
                }

                if (configPacket.Common is not null)
                {
                    Load(RootConfigType.Common, configPacket.Common);
                }

                if (configPacket.Server is not null)
                {
                    Load(RootConfigType.Server, configPacket.Server);
                }

                break;
            }

            case ConfigPacketOperation.ClientServerSync:
            {
                _logger.Debug("Processing client-to-server synchronization packet");

                if (_side is not EnumAppSide.Server)
                {
                    throw new InvalidSideException(_side);
                }

                if (configPacket.Common is not null)
                {
                    Load(RootConfigType.Common, configPacket.Common);
                    Save(RootConfigType.Common);
                }

                if (configPacket.Server is not null)
                {
                    Load(RootConfigType.Server, configPacket.Server);
                    Save(RootConfigType.Server);
                }

                SendServerClientSync();

                break;
            }

            case ConfigPacketOperation.ServerRestore:
            {
                _logger.Debug("Processing server restore packet");

                if (_side is not EnumAppSide.Server)
                {
                    throw new InvalidSideException(_side);
                }

                if (_configs.ContainsKey(RootConfigType.Common))
                {
                    Load(RootConfigType.Common);
                }

                if (_configs.ContainsKey(RootConfigType.Server) && _side == EnumAppSide.Server)
                {
                    Load(RootConfigType.Server);
                }

                SendServerClientSync();

                break;
            }

            case ConfigPacketOperation.ServerReset:
            {
                _logger.Debug("Processing server reset packet");

                if (_side is not EnumAppSide.Server)
                {
                    throw new InvalidSideException(_side);
                }

                if (_configs.TryGetValue(RootConfigType.Common, out var commonConfig))
                {
                    commonConfig.Reset();
                    Save(RootConfigType.Common);
                    Updated?.Invoke(RootConfigType.Common);
                }

                if (_configs.TryGetValue(RootConfigType.Server, out var serverConfig) && _side == EnumAppSide.Server)
                {
                    serverConfig.Reset();
                    Save(RootConfigType.Server);
                    Updated?.Invoke(RootConfigType.Server);
                }

                SendServerClientSync();

                break;
            }

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Synchronize(IServerPlayer player) => SendServerClientSync(player);

    public void Render()
    {
        if (_side is not EnumAppSide.Client)
        {
            throw new InvalidSideException(_side);
        }

        var configLibSystem = _api.ModLoader.GetModSystem<ConfigLibModSystem>();

        if (_configs.ContainsKey(RootConfigType.Common))
        {
            _logger.Verbose("Registering ConfigLib common renderer");
            configLibSystem!.RegisterCustomConfig(
                domain: $"{_system.ModName()} (Common)",
                drawDelegate: (_, controlButtons) => RenderCommon(controlButtons)
            );
        }

        if (_configs.ContainsKey(RootConfigType.Server))
        {
            _logger.Verbose("Registering ConfigLib server renderer");
            configLibSystem!.RegisterCustomConfig(
                domain: $"{_system.ModName()} (Server)",
                drawDelegate: (_, controlButtons) => RenderServer(controlButtons)
            );
        }

        if (_configs.ContainsKey(RootConfigType.Client))
        {
            _logger.Verbose("Registering ConfigLib client renderer");
            configLibSystem!.RegisterCustomConfig(
                domain: $"{_system.ModName()} (Client)",
                drawDelegate: (_, controlButtons) => RenderClient(controlButtons)
            );
        }
    }

    private void OnClientPlayerJoined(IClientPlayer player)
    {
        if (player.Privileges is null || player.Privileges.Length == 0 || !player.Privileges.Contains(Privilege.controlserver))
        {
            _logger.Verbose("Player can't edit server configuration remotely");
            return;
        }

        _canEditServerConfig = true;
        _logger.Verbose("Player can edit server configuration remotely");
    }

    private void Load(RootConfigType type)
    {
        _logger.Debug("Loading {0} configuration", type);
        var stopwatch = Stopwatch.StartNew();

        var fileName = GetFileName(type);
        if (!_fileSystem.ConfigFileExists(fileName))
        {
            _logger.Verbose("Failed to load {0} configuration: file {1} does not exist", type, fileName);
            return;
        }

        try
        {
            var json = _fileSystem.ReadConfigFile(fileName);
            Load(type, json);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to load {0} configuration", type);
        }

        stopwatch.Stop();
        _logger.Debug("Done loading {0} configuration in {1} ms", type, stopwatch.ElapsedMilliseconds);
    }

    private void Load(RootConfigType type, string json)
    {
        try
        {
            _configs[type] = JsonSerializer.Deserialize(json, _configTypes[type], _jsonOptions) as IRootConfig ?? throw new NullReferenceException();
            Updated?.Invoke(type);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to load {0} configuration", type);
        }
    }

    private void Save(RootConfigType type)
    {
        _logger.Debug("Saving {0} configuration", type);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var json = JsonSerializer.Serialize(_configs[type], _configTypes[type], _jsonOptions);
            _fileSystem.WriteConfigFile(GetFileName(type), json);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to save {0} configuration", type);
        }

        stopwatch.Stop();
        _logger.Debug("Done saving {0} configuration in {1} ms", type, stopwatch.ElapsedMilliseconds);
    }

    private void SendServerClientSync(IServerPlayer? player = null)
    {
        _logger.Debug("Sending server-to-client synchronization request");

        var packet = new ConfigPacket
        {
            Operation = ConfigPacketOperation.ServerClientSync
        };

        try
        {
            if (_configs.TryGetValue(RootConfigType.Common, out var commonConfig))
            {
                var commonJson = JsonSerializer.Serialize(commonConfig, _configTypes[RootConfigType.Common], _jsonOptions);
                packet.Common = commonJson;
            }

            if (_configs.TryGetValue(RootConfigType.Server, out var serverConfig))
            {
                var serverJson = JsonSerializer.Serialize(serverConfig, _configTypes[RootConfigType.Server], _jsonOptions);
                packet.Server = serverJson;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to construct synchronization packet");
        }

        if (player is not null)
        {
            _serverChannel!.SendPacket(packet, player);
        }
        else
        {
            _serverChannel!.BroadcastPacket(packet);
        }
    }

    private void SendClientServerSync()
    {
        _logger.Debug("Sending client-to-server synchronization request");

        var packet = new ConfigPacket
        {
            Operation = ConfigPacketOperation.ClientServerSync
        };

        try
        {
            if (_configs.TryGetValue(RootConfigType.Common, out var commonConfig))
            {
                var commonJson = JsonSerializer.Serialize(commonConfig, _configTypes[RootConfigType.Common], _jsonOptions);
                packet.Common = commonJson;
            }

            if (_configs.TryGetValue(RootConfigType.Server, out var serverConfig))
            {
                var serverJson = JsonSerializer.Serialize(serverConfig, _configTypes[RootConfigType.Server], _jsonOptions);
                packet.Server = serverJson;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to construct synchronization packet");
        }

        _clientChannel!.SendPacket(packet);
    }

    private void SendServerRestore()
    {
        _logger.Debug("Sending server restore request");

        var packet = new ConfigPacket
        {
            Operation = ConfigPacketOperation.ServerRestore
        };

        _clientChannel!.SendPacket(packet);
    }

    private void SendServerReset()
    {
        _logger.Debug("Sending server reset request");

        var packet = new ConfigPacket
        {
            Operation = ConfigPacketOperation.ServerReset
        };

        _clientChannel!.SendPacket(packet);
    }

    private ControlButtons RenderCommon(ControlButtons controlButtons)
    {
        _configUi.Label($"config--header--common");
        ImGui.Separator();

        ImGui.BeginDisabled(!_canEditServerConfig);

        if (!_canEditServerConfig)
        {
            _configUi.Label("You don't have permission to edit the common configuration");
        }

        _configs[RootConfigType.Common].Render(_configUi);

        ImGui.EndDisabled();

        if (controlButtons.Save && _canEditServerConfig)
        {
            SendClientServerSync();
        }

        if (controlButtons.Restore && _canEditServerConfig)
        {
            SendServerRestore();
        }

        if (controlButtons.Defaults && _canEditServerConfig)
        {
            SendServerReset();
        }

        return new ControlButtons
        {
            Save = _canEditServerConfig,
            Restore = _canEditServerConfig,
            Defaults = _canEditServerConfig,
            Reload = false
        };
    }

    private ControlButtons RenderServer(ControlButtons controlButtons)
    {
        _configUi.Label($"config--header--server");
        ImGui.Separator();

        ImGui.BeginDisabled(!_canEditServerConfig);

        if (!_canEditServerConfig)
        {
            _configUi.Label("You don't have permission to edit the server configuration");
        }

        _configs[RootConfigType.Server].Render(_configUi);

        ImGui.EndDisabled();

        if (controlButtons.Save && _canEditServerConfig)
        {
            SendClientServerSync();
        }

        if (controlButtons.Restore && _canEditServerConfig)
        {
            SendServerRestore();
        }

        if (controlButtons.Defaults && _canEditServerConfig)
        {
            SendServerReset();
        }

        return new ControlButtons
        {
            Save = _canEditServerConfig,
            Restore = _canEditServerConfig,
            Defaults = _canEditServerConfig,
            Reload = false
        };
    }

    private ControlButtons RenderClient(ControlButtons controlButtons)
    {
        _configUi.Label($"config--header--client");
        ImGui.Separator();

        _configs[RootConfigType.Client].Render(_configUi);

        if (controlButtons.Save)
        {
            Save(RootConfigType.Client);
            Updated?.Invoke(RootConfigType.Client);
        }

        if (controlButtons.Restore)
        {
            Load(RootConfigType.Client);
        }

        if (controlButtons.Defaults)
        {
            _configs[RootConfigType.Client].Reset();
            Save(RootConfigType.Client);
            Updated?.Invoke(RootConfigType.Client);
        }

        return new ControlButtons
        {
            Save = true,
            Restore = true,
            Defaults = true,
            Reload = false
        };
    }

    private static string GetFileName(RootConfigType type)
    {
        return type switch
        {
            RootConfigType.Common => "common.json",
            RootConfigType.Server => "server.json",
            RootConfigType.Client => "client.json",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
