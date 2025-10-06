namespace Common.Mod.Common.Config;

public interface IConfigSystem<out TCommonConfig, out TServerConfig, out TClientConfig>
    where TCommonConfig : IRootConfig, new()
    where TServerConfig : IRootConfig, new()
    where TClientConfig : IRootConfig, new()
{
    public TCommonConfig? Common();
    public TServerConfig? Server();
    public TClientConfig? Client();

    public void Load();
    public void Save();
}
