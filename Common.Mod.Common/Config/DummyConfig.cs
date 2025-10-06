namespace Common.Mod.Common.Config;

public record DummyConfig : IRootConfig
{
    public string Version() => string.Empty;
}
