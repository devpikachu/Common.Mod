using Vintagestory.API.Server;

namespace Common.Mod.Common.Config;

public interface IConfigSystem
{
    public void Register<TRootConfig>()
        where TRootConfig : class, IRootConfig, new();

    public TRootConfig Get<TRootConfig>(RootConfigType type)
        where TRootConfig : class, IRootConfig, new();

    public TRootConfig GetCommon<TRootConfig>()
        where TRootConfig : class, IRootConfig, new();

    public TRootConfig GetServer<TRootConfig>()
        where TRootConfig : class, IRootConfig, new();

    public TRootConfig GetClient<TRootConfig>()
        where TRootConfig : class, IRootConfig, new();

    public void Load();
    public void Save();

    public void Synchronize<TConfigPacket>(IServerPlayer player, TConfigPacket packet)
        where TConfigPacket : class, new();

    public void Synchronize<TConfigPacket>(TConfigPacket packet)
        where TConfigPacket : class, new();

    public void Synchronize(IServerPlayer player);

    public void Render();
}
