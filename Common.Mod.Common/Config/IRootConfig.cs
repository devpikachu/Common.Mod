namespace Common.Mod.Common.Config;

public interface IRootConfig : IConfig
{
    public string Version();
    public RootConfigType Type();
}
