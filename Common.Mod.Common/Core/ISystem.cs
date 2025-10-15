using Vintagestory.API.Client;
using Vintagestory.API.Server;

namespace Common.Mod.Common.Core;

public interface ISystem
{
    #region Event delegates

    public delegate void ServerPlayerJoinedHandler(IServerPlayer player);

    public delegate void ClientPlayerJoinedHandler(IClientPlayer player);

    #endregion Event delegates

    public event ServerPlayerJoinedHandler? ServerPlayerJoined;
    public event ClientPlayerJoinedHandler? ClientPlayerJoined;

    public string ModId();
    public string ModVersion();
    public string ModName();
}
