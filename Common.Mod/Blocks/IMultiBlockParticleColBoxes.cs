using System.Diagnostics.CodeAnalysis;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Common.Mod.Blocks;

public interface IMultiBlockParticleColBoxes
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public Cuboidf[] MBGetParticleCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset);
}
