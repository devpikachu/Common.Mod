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

    private bool _canEditServerConfig = false;

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

    public void Synchronize<TConfigPacket>(IServerPlayer player, TConfigPacket packet) where TConfigPacket : class, new()
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

    public TConfigPacket Synchronize<TConfigPacket>() where TConfigPacket : class, new()
    {
        _logger.Debug("Received configuration synchronization request");

        var packet = new TConfigPacket();
        if (packet is not ConfigPacket configPacket)
        {
            throw new InvalidCastException($"Requested packet type does not match {nameof(ConfigPacket)}");
        }

        try
        {
            var commonJson = JsonSerializer.Serialize(_configs[RootConfigType.Common], _configTypes[RootConfigType.Common], _jsonOptions);
            configPacket.Common = commonJson;

            var serverJson = JsonSerializer.Serialize(_configs[RootConfigType.Server], _configTypes[RootConfigType.Server], _jsonOptions);
            configPacket.Server = serverJson;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to construct synchronization packet");
        }

        return packet;
    }

    public void Synchronize<TConfigPacket>(TConfigPacket packet)
        where TConfigPacket : class, new()
    {
        _logger.Debug("Received configuration synchronization packet");

        if (packet is not ConfigPacket configPacket)
        {
            throw new InvalidCastException($"Received packet type does not match {nameof(ConfigPacket)}");
        }

        try
        {
            if (configPacket.Common is not null)
            {
                _configs[RootConfigType.Common] =
                    JsonSerializer.Deserialize(configPacket.Common, _configTypes[RootConfigType.Common], _jsonOptions) as IRootConfig ??
                    throw new NullReferenceException();
            }

            if (configPacket.Server is not null)
            {
                _configs[RootConfigType.Server] =
                    JsonSerializer.Deserialize(configPacket.Server, _configTypes[RootConfigType.Server], _jsonOptions) as IRootConfig ??
                    throw new NullReferenceException();
            }

            Synchronized?.Invoke();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to deconstruct synchronization packet");
        }
    }

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
            _logger.Debug("Registering ConfigLib {0} renderer", nameof(RootConfigType.Common));
            _configLibSystem!.RegisterCustomConfig(
                domain: $"{_system.ModName()} ({nameof(RootConfigType.Common)})",
                drawDelegate: (_, controlButtons) => RenderCommon(controlButtons)
            );
        }

        if (_configs.ContainsKey(RootConfigType.Server))
        {
            _logger.Debug("Registering ConfigLib {0} renderer", nameof(RootConfigType.Server));
            _configLibSystem!.RegisterCustomConfig(
                domain: $"{_system.ModName()} ({nameof(RootConfigType.Server)})",
                drawDelegate: (_, controlButtons) => RenderServer(controlButtons)
            );
        }

        if (_configs.ContainsKey(RootConfigType.Client))
        {
            _logger.Debug("Registering ConfigLib {0} renderer", nameof(RootConfigType.Client));
            _configLibSystem!.RegisterCustomConfig(
                domain: $"{_system.ModName()} ({nameof(RootConfigType.Client)})",
                drawDelegate: (_, controlButtons) => RenderClient(controlButtons)
            );
        }
    }

    private void OnServerRegisterMessageTypes(IServerNetworkChannel channel)
    {
        channel
            .RegisterMessageType<ConfigPacket>()
            .SetMessageHandler<ConfigPacket>(Synchronize);
    }

    private void OnClientRegisterMessageTypes(IClientNetworkChannel channel)
    {
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

    private ControlButtons RenderCommon(ControlButtons controlButtons)
    {
        return new ControlButtons
        {
            Save = false,
            Restore = false,
            Defaults = false,
            Reload = false
        };
    }

    private ControlButtons RenderServer(ControlButtons controlButtons)
    {
        ImGui.BeginDisabled(!_canEditServerConfig);

        if (!_canEditServerConfig)
        {
            ImGui.Text("You don't have permission to edit the server's configuration.");
            ImGui.NewLine();
        }

        ImGui.EndDisabled();

        return new ControlButtons
        {
            Save = false,
            Restore = false,
            Defaults = false,
            Reload = false
        };
    }

    private ControlButtons RenderClient(ControlButtons controlButtons)
    {
        return new ControlButtons
        {
            Save = false,
            Restore = false,
            Defaults = false,
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
