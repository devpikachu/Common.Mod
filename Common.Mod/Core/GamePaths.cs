using Common.Mod.Common.Core;

namespace Common.Mod.Core;

public class GamePaths : IGamePaths
{
    public string Data() => Vintagestory.API.Config.GamePaths.DataPath;
}
