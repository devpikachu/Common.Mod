using Vintagestory.API.Client;
using Vintagestory.API.Server;

namespace Common.Mod.Common.Core;

public interface ISystem
{
    #region Event delegates

    public delegate void ServerStartHandler(ICoreServerAPI api);

    public delegate void ClientStartHandler(ICoreClientAPI api);

    public delegate void ServerRegisterMessageTypesHandler(IServerNetworkChannel channel);

    public delegate void ClientRegisterMessageTypesHandler(IClientNetworkChannel channel);

    public delegate void ServerPlayerJoinedHandler(IServerPlayer player);

    public delegate void ClientPlayerJoinedHandler(IClientPlayer player);

    #endregion Event delegates

    public event ServerStartHandler? ServerStart;
    public event ClientStartHandler? ClientStart;
    public event ServerRegisterMessageTypesHandler? ServerRegisterMessageTypes;
    public event ClientRegisterMessageTypesHandler? ClientRegisterMessageTypes;
    public event ServerPlayerJoinedHandler? ServerPlayerJoined;
    public event ClientPlayerJoinedHandler? ClientPlayerJoined;

    public string ModId();
    public string ModVersion();
    public string ModName();
}
