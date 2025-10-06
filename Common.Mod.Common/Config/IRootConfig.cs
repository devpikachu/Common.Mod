using JetBrains.Annotations;

namespace Common.Mod.Common.Config;

public interface IRootConfig
{
    [UsedImplicitly]
    public string Version();
}
