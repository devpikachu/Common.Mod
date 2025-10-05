using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace Common.Mod.Example;

[UsedImplicitly]
public class ExampleSystem : ModSystem
{
    public override bool ShouldLoad(EnumAppSide forSide) => true;
}
