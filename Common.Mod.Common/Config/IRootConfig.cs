namespace Common.Mod.Common.Config;

public interface IRootConfig : IConfig
{
    public string Version { get; }

    public RootConfigType Type();
}
