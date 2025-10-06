using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Mod.Common.Config;
using Common.Mod.Common.Core;
using JetBrains.Annotations;

namespace Common.Mod.Config;

public class ConfigSystem<TCommonConfig, TServerConfig, TClientConfig> : IConfigSystem<TCommonConfig, TServerConfig, TClientConfig>
    where TCommonConfig : IRootConfig, new()
    where TServerConfig : IRootConfig, new()
    where TClientConfig : IRootConfig, new()
{
    private const string CommonFileName = "common.json";
    private const string ServerFileName = "server.json";
    private const string ClientFileName = "client.json";

    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;

    private readonly JsonSerializerOptions _jsonOptions;

    private TCommonConfig? _common;
    private TServerConfig? _server;
    private TClientConfig? _client;

    public TCommonConfig? Common() => _common;
    public TServerConfig? Server() => _server;
    public TClientConfig? Client() => _client;

    public ConfigSystem(ILogger logger, IFileSystem fileSystem)
    {
        _logger = logger.Named("Config");
        _fileSystem = fileSystem;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = true
        };
    }

    public void Load()
    {
        if (typeof(TCommonConfig) == typeof(DummyConfig)
            && typeof(TClientConfig) == typeof(DummyConfig)
            && typeof(TServerConfig) == typeof(DummyConfig))
        {
            _logger.Verbose("All config types are set to a dummy value, skipping config loading");
            return;
        }

        if (typeof(TCommonConfig) != typeof(DummyConfig))
        {
            _logger.Debug("Loading common config");
            _common = Load<TCommonConfig>(CommonFileName);
            if (_common is null)
            {
                _logger.Debug("Couldn't load common config from file, setting defaults");
                _common = new TCommonConfig();
            }
        }

        if (typeof(TServerConfig) != typeof(DummyConfig))
        {
            _logger.Debug("Loading server config");
            _server = Load<TServerConfig>(ServerFileName);
            if (_server is null)
            {
                _logger.Debug("Couldn't load server config from file, setting defaults");
                _server = new TServerConfig();
            }
        }

        if (typeof(TClientConfig) != typeof(DummyConfig))
        {
            _logger.Debug("Loading client config");
            _client = Load<TClientConfig>(ClientFileName);
            if (_client is null)
            {
                _logger.Debug("Couldn't load client config from file, setting defaults");
                _client = new TClientConfig();
            }
        }

        Save();
    }

    public void Save()
    {
        if (typeof(TCommonConfig) == typeof(DummyConfig)
            && typeof(TClientConfig) == typeof(DummyConfig)
            && typeof(TServerConfig) == typeof(DummyConfig))
        {
            _logger.Verbose("All config types are set to a dummy value, skipping config saving");
            return;
        }

        if (typeof(TCommonConfig) != typeof(DummyConfig) && _common is not null)
        {
            _logger.Debug("Saving common config");
            Save(CommonFileName, _common);
        }

        if (typeof(TServerConfig) != typeof(DummyConfig) && _server is not null)
        {
            _logger.Debug("Saving server config");
            Save(ServerFileName, _server);
        }

        if (typeof(TClientConfig) != typeof(DummyConfig) && _client is not null)
        {
            _logger.Debug("Saving client config");
            Save(ClientFileName, _client);
        }
    }

    private TConfig? Load<TConfig>(string fileName)
        where TConfig : IRootConfig, new()
    {
        if (!_fileSystem.ConfigFileExists(fileName))
        {
            return default;
        }

        try
        {
            var json = _fileSystem.ReadConfigFile(fileName);
            return JsonSerializer.Deserialize<TConfig>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to load config from {0}", fileName);
            return default;
        }
    }

    private void Save<TConfig>(string fileName, TConfig value)
        where TConfig : IRootConfig, new()
    {
        try
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            _fileSystem.WriteConfigFile(fileName, json);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to save config to {0}", fileName);
        }
    }
}

[UsedImplicitly]
public class ConfigSystem<TCommonConfig> : ConfigSystem<TCommonConfig, DummyConfig, DummyConfig>
    where TCommonConfig : IRootConfig, new()
{
    public ConfigSystem(ILogger logger, IFileSystem fileSystem) : base(logger, fileSystem)
    {
    }
}
