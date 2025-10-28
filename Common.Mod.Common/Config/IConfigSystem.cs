using JetBrains.Annotations;
using Vintagestory.API.Server;

namespace Common.Mod.Common.Config;

[UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Members)]
public interface IConfigSystem
{
    #region Event delegates

    public delegate void UpdatedHandler(RootConfigType type);

    #endregion Event delegates

    public event UpdatedHandler? Updated;

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

    public void MutateCommon<TRootConfig>(Func<TRootConfig, TRootConfig> mutator)
        where TRootConfig : class, IRootConfig, new();

    public void MutateServer<TRootConfig>(Func<TRootConfig, TRootConfig> mutator)
        where TRootConfig : class, IRootConfig, new();

    public void MutateClient<TRootConfig>(Func<TRootConfig, TRootConfig> mutator)
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
