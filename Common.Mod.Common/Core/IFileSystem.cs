namespace Common.Mod.Common.Core;

public interface IFileSystem
{
    public string GetConfigDirPath();
    public string GetDataDirPath();

    public bool ConfigFileExists(string fileName);
    public bool DataFileExists(string fileName);

    public string ReadConfigFile(string fileName);
    public string ReadDataFile(string fileName);

    public void WriteConfigFile(string fileName, string contents);
    public void WriteDataFile(string fileName, string contents);

    public FileStream OpenConfigFile(string fileName, FileMode mode = FileMode.OpenOrCreate);
    public FileStream OpenDataFile(string fileName, FileMode mode = FileMode.OpenOrCreate);
}
