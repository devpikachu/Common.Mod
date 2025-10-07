namespace Common.Mod.Common.Config;

public interface IConfig<TPacket>
{
    public bool IsSynchronized();

    public void ApplyPacket(TPacket packet);
    public TPacket CreatePacket();
}

public interface IConfig : IConfig<object>;
