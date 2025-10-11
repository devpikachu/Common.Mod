using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Mod.Common.Config;
using Common.Mod.Common.Core;

namespace Common.Mod.Config;

public class ConfigSystem : IConfigSystem
{
    public delegate void SynchronizedHandler();

    public event SynchronizedHandler? Synchronized;

    private readonly SystemSide _side;
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;

    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Dictionary<RootConfigType, Type> _configTypes;
    private readonly Dictionary<RootConfigType, IRootConfig> _configs;

    public ConfigSystem(SystemSide side, ILogger logger, IFileSystem fileSystem)
    {
        _side = side;
        _logger = logger.Named("ConfigSystem");
        _fileSystem = fileSystem;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = true
        };
        _configTypes = new Dictionary<RootConfigType, Type>();
        _configs = new Dictionary<RootConfigType, IRootConfig>();
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
            var json = JsonSerializer.Serialize(_configs[RootConfigType.Common], _configTypes[RootConfigType.Common], _jsonOptions);
            configPacket.Data = json;
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
            _configs[RootConfigType.Common] =
                JsonSerializer.Deserialize(configPacket.Data, _configTypes[RootConfigType.Common], _jsonOptions) as IRootConfig ??
                throw new NullReferenceException();

            Synchronized?.Invoke();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to deconstruct synchronization packet");
        }
    }

    private void Load(RootConfigType type)
    {
        _logger.Debug("Loading {0} configuration", type);

        var fileName = GetFileName(type);
        if (!_fileSystem.ConfigFileExists(fileName))
        {
            _logger.Error("Failed to load {0} configuration: file {1} does not exist", type, fileName);
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
