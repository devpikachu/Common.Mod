namespace Common.Mod.Common.Config;

public record DummyConfig : IConfig, IRootConfig
{
    public string Version() => string.Empty;

    public bool IsSynchronized() => false;
    public void ApplyPacket(object packet) => throw new NotImplementedException();
    public object CreatePacket() => throw new NotImplementedException();
}
