using Vintagestory.API.Client;
using Vintagestory.API.Server;

namespace Common.Mod.Common.Core;

public interface ISystem
{
    #region Event delegates

    public delegate void AssetsLoadedHandler();

    public delegate void AssetsFinalizedHandler();

    public delegate void ServerPlayerJoinedHandler(IServerPlayer player);

    public delegate void ClientPlayerJoinedHandler(IClientPlayer player);

    #endregion Event delegates

    public event AssetsLoadedHandler? OnAssetsLoaded;
    public event AssetsFinalizedHandler? OnAssetsFinalized;
    public event ServerPlayerJoinedHandler? OnServerPlayerJoined;
    public event ClientPlayerJoinedHandler? OnClientPlayerJoined;

    public string ModId();
    public string ModVersion();
    public string ModName();
}
