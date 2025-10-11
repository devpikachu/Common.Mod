using Vintagestory.API.Client;
using Vintagestory.API.Server;

namespace Common.Mod.Common.Core;

public interface ISystem
{
    #region Event Delegates

    public delegate void ServerRegisterMessageTypesHandler(IServerNetworkChannel channel);

    public delegate void ClientRegisterMessageTypesHandler(IClientNetworkChannel channel);

    public delegate void ServerPlayerJoinedHandler(IServerPlayer player);

    public delegate void ClientPlayerJoinedHandler(IClientPlayer player);

    #endregion Event Delegates

    public event ServerRegisterMessageTypesHandler? ServerRegisterMessageTypes;
    public event ClientRegisterMessageTypesHandler? ClientRegisterMessageTypes;
    public event ServerPlayerJoinedHandler? ServerPlayerJoined;
    public event ClientPlayerJoinedHandler? ClientPlayerJoined;

    public string ModId();
    public string ModVersion();
    public string ModName();
}
