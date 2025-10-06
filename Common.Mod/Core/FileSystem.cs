using Common.Mod.Common.Core;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace Common.Mod.Core;

public class FileSystem : IFileSystem
{
    private readonly ICoreAPI _api;

    private readonly string _relativeConfigDirPath;
    private readonly string _absoluteConfigDirPath;
    private readonly string _relativeDataDirPath;
    private readonly string _absoluteDataDirPath;

    public FileSystem(ISystem system, ICoreAPI api)
    {
        _api = api;

        _relativeConfigDirPath = Path.Combine("ModConfig", system.ModId());
        _absoluteConfigDirPath = Path.Combine(GamePaths.DataPath, _relativeConfigDirPath);
        _relativeDataDirPath = Path.Combine("ModData", system.ModId());
        _absoluteDataDirPath = Path.Combine(GamePaths.DataPath, _relativeDataDirPath);
    }

    public string GetConfigDirPath()
    {
        if (!Path.Exists(_absoluteConfigDirPath))
        {
            _api.GetOrCreateDataPath(_relativeConfigDirPath);
        }

        return _absoluteConfigDirPath;
    }

    public string GetDataDirPath()
    {
        if (!Path.Exists(_absoluteDataDirPath))
        {
            _api.GetOrCreateDataPath(_relativeDataDirPath);
        }

        return _absoluteDataDirPath;
    }

    public bool ConfigFileExists(string fileName) => File.Exists(Path.Join(_absoluteConfigDirPath, fileName));
    public bool DataFileExists(string fileName) => File.Exists(Path.Join(_absoluteDataDirPath, fileName));

    public string ReadConfigFile(string fileName) => File.ReadAllText(Path.Join(GetConfigDirPath(), fileName));
    public string ReadDataFile(string fileName) => File.ReadAllText(Path.Join(GetDataDirPath(), fileName));

    public void WriteConfigFile(string fileName, string contents) => File.WriteAllText(Path.Join(GetConfigDirPath(), fileName), contents);
    public void WriteDataFile(string fileName, string contents) => File.WriteAllText(Path.Join(GetDataDirPath(), fileName), contents);

    public FileStream OpenConfigFile(string fileName, FileMode mode = FileMode.OpenOrCreate) => File.Open(Path.Join(GetConfigDirPath(), fileName), mode);
    public FileStream OpenDataFile(string fileName, FileMode mode = FileMode.OpenOrCreate) => File.Open(Path.Join(GetDataDirPath(), fileName), mode);
}
