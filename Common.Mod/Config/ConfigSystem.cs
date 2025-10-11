using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Mod.Common.Config;
using Common.Mod.Common.Core;
using ConfigLib;
using ImGuiNET;
using Vintagestory.API.Client;
using Vintagestory.API.Server;
using ILogger = Common.Mod.Common.Core.ILogger;

namespace Common.Mod.Config;

public class ConfigSystem : IConfigSystem
{
    public delegate void SynchronizedHandler();

    public event SynchronizedHandler? Synchronized;

    private readonly SystemSide _side;
    private readonly ILogger _logger;
    private readonly ISystem _system;
    private readonly IFileSystem _fileSystem;
    private readonly ConfigLibModSystem? _configLibSystem;

    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Dictionary<RootConfigType, Type> _configTypes;
    private readonly Dictionary<RootConfigType, IRootConfig> _configs;

    private bool _canEditServerConfig;
    private IServerNetworkChannel? _serverChannel;
    private IClientNetworkChannel? _clientChannel;

    public ConfigSystem(
        SystemSide side,
        ILogger logger,
        ISystem system,
        IFileSystem fileSystem,
        ConfigLibModSystem? configLibSystem
    )
    {
        _side = side;
        _logger = logger.Named("ConfigSystem");
        _system = system;
        _fileSystem = fileSystem;
        _configLibSystem = configLibSystem;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = true
        };
        _configTypes = new Dictionary<RootConfigType, Type>();
        _configs = new Dictionary<RootConfigType, IRootConfig>();

        _system.ServerRegisterMessageTypes += OnServerRegisterMessageTypes;
        _system.ClientRegisterMessageTypes += OnClientRegisterMessageTypes;
        _system.ClientPlayerJoined += OnClientPlayerJoined;
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

    public void Load()
    {
        if (_configs.ContainsKey(RootConfigType.Common))
        {
            Load(RootConfigType.Common);
        }

        if (_configs.ContainsKey(RootConfigType.Server) && _side == SystemSide.Server)
        {
            Load(RootConfigType.Server);
        }

        if (_configs.ContainsKey(RootConfigType.Client) && _side == SystemSide.Client)
        {
            Load(RootConfigType.Client);
        }
    }

    public void Save()
    {
        if (_configs.ContainsKey(RootConfigType.Common))
        {
            Save(RootConfigType.Common);
        }

        if (_configs.ContainsKey(RootConfigType.Server) && _side == SystemSide.Server)
        {
            Save(RootConfigType.Server);
        }

        if (_configs.ContainsKey(RootConfigType.Client) && _side == SystemSide.Client)
        {
            Save(RootConfigType.Client);
        }
    }

    public void Synchronize<TConfigPacket>(IServerPlayer player, TConfigPacket packet)
        where TConfigPacket : class, new()
    {
        if (!player.HasPrivilege(Privilege.controlserver))
        {
            _logger.Warning(
                "Player {0} (ID {1}, IP {2}) attempted to remotely change server config, but lacks permissions. This likely indicates that the player sent a manually crafted synchronization packet to the server.",
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

                if (_side is not SystemSide.Client)
                {
                    throw new InvalidOperationException("The server-to-client synchronization operation can only be received on the client");
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

                if (_side is not SystemSide.Server)
                {
                    throw new InvalidOperationException("The client-to-server synchronization operation can only be received on the server");
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

                if (_side is not SystemSide.Server)
                {
                    throw new InvalidOperationException("The server restore operation can only be received on the server");
                }

                if (_configs.ContainsKey(RootConfigType.Common))
                {
                    Load(RootConfigType.Common);
                }

                if (_configs.ContainsKey(RootConfigType.Server) && _side == SystemSide.Server)
                {
                    Load(RootConfigType.Server);
                }

                SendServerClientSync();

                break;
            }

            case ConfigPacketOperation.ServerReset:
            {
                _logger.Debug("Processing server reset packet");

                if (_side is not SystemSide.Server)
                {
                    throw new InvalidOperationException("The server reset operation can only be received on the server");
                }

                if (_configs.TryGetValue(RootConfigType.Common, out var commonConfig))
                {
                    commonConfig.Reset();
                    Save(RootConfigType.Common);
                }

                if (_configs.TryGetValue(RootConfigType.Server, out var serverConfig) && _side == SystemSide.Server)
                {
                    serverConfig.Reset();
                    Save(RootConfigType.Server);
                }

                SendServerClientSync();

                break;
            }

            default:
                throw new ArgumentOutOfRangeException();
        }

        Synchronized?.Invoke();
    }

    public void Synchronize(IServerPlayer player) => SendServerClientSync(player);

    public void Render()
    {
        if (_side == SystemSide.Server)
        {
            return;
        }

        if (_configLibSystem is null)
        {
            _logger.Debug("ConfigLib not found, skipping config UI rendering");
            return;
        }

        if (_configs.ContainsKey(RootConfigType.Common))
        {
            _logger.Debug("Registering ConfigLib common renderer");
            _configLibSystem!.RegisterCustomConfig(
                domain: $"{_system.ModName()} (Common)",
                drawDelegate: (_, controlButtons) => RenderCommon(controlButtons)
            );
        }

        if (_configs.ContainsKey(RootConfigType.Server))
        {
            _logger.Debug("Registering ConfigLib server renderer");
            _configLibSystem!.RegisterCustomConfig(
                domain: $"{_system.ModName()} (Server)",
                drawDelegate: (_, controlButtons) => RenderServer(controlButtons)
            );
        }

        if (_configs.ContainsKey(RootConfigType.Client))
        {
            _logger.Debug("Registering ConfigLib client renderer");
            _configLibSystem!.RegisterCustomConfig(
                domain: $"{_system.ModName()} (Client)",
                drawDelegate: (_, controlButtons) => RenderClient(controlButtons)
            );
        }
    }

    private void OnServerRegisterMessageTypes(IServerNetworkChannel channel)
    {
        _serverChannel = channel;
        channel
            .RegisterMessageType<ConfigPacket>()
            .SetMessageHandler<ConfigPacket>(Synchronize);
    }

    private void OnClientRegisterMessageTypes(IClientNetworkChannel channel)
    {
        _clientChannel = channel;
        channel
            .RegisterMessageType<ConfigPacket>()
            .SetMessageHandler<ConfigPacket>(Synchronize);
    }

    private void OnClientPlayerJoined(IClientPlayer player)
    {
        _canEditServerConfig = player.HasPrivilege(Privilege.controlserver);
    }

    private void Load(RootConfigType type)
    {
        _logger.Debug("Loading {0} configuration", type);

        var fileName = GetFileName(type);
        if (!_fileSystem.ConfigFileExists(fileName))
        {
            _logger.Debug("Failed to load {0} configuration: file {1} does not exist", type, fileName);
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
    }

    private void Load(RootConfigType type, string json)
    {
        try
        {
            _configs[type] = JsonSerializer.Deserialize(json, _configTypes[type], _jsonOptions) as IRootConfig ?? throw new NullReferenceException();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to load {0} configuration", type);
        }
    }

    private void Save(RootConfigType type)
    {
        _logger.Debug("Saving {0} configuration", type);

        try
        {
            var json = JsonSerializer.Serialize(_configs[type], _configTypes[type], _jsonOptions);
            _fileSystem.WriteConfigFile(GetFileName(type), json);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to save {0} configuration", type);
        }
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
            var commonJson = JsonSerializer.Serialize(_configs[RootConfigType.Common], _configTypes[RootConfigType.Common], _jsonOptions);
            packet.Common = commonJson;

            var serverJson = JsonSerializer.Serialize(_configs[RootConfigType.Server], _configTypes[RootConfigType.Server], _jsonOptions);
            packet.Server = serverJson;
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
            var commonJson = JsonSerializer.Serialize(_configs[RootConfigType.Common], _configTypes[RootConfigType.Common], _jsonOptions);
            packet.Common = commonJson;

            var serverJson = JsonSerializer.Serialize(_configs[RootConfigType.Server], _configTypes[RootConfigType.Server], _jsonOptions);
            packet.Server = serverJson;
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
        ConfigUI.Label($"{_system.ModName()} Common Configuration");
        ImGui.Separator();
        ImGui.NewLine();

        ImGui.BeginDisabled(!_canEditServerConfig);

        if (!_canEditServerConfig)
        {
            ConfigUI.Label("You don't have permission to edit the common configuration.");
            ImGui.NewLine();
        }

        _configs[RootConfigType.Common].Render();

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
        ConfigUI.Label($"{_system.ModName()} Server Configuration");
        ImGui.Separator();
        ImGui.NewLine();

        ImGui.BeginDisabled(!_canEditServerConfig);

        if (!_canEditServerConfig)
        {
            ConfigUI.Label("You don't have permission to edit the server configuration.");
            ImGui.NewLine();
        }

        _configs[RootConfigType.Server].Render();

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
        ConfigUI.Label($"{_system.ModName()} Client Configuration");
        ImGui.Separator();
        ImGui.NewLine();

        _configs[RootConfigType.Client].Render();

        if (controlButtons.Save)
        {
            Save(RootConfigType.Client);
        }

        if (controlButtons.Restore)
        {
            Load(RootConfigType.Client);
        }

        if (controlButtons.Defaults)
        {
            _configs[RootConfigType.Client].Reset();
            Save(RootConfigType.Client);
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
