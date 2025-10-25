using JetBrains.Annotations;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Common.Mod.Blocks;

// TODO: Remove this class if https://github.com/anegostudios/vsessentialsmod/pull/31 gets merged
[UsedImplicitly]
public class MultiblockBlock : BlockMultiblock
{
    public const string RegistryId = "MultiblockBlock";

    public override Cuboidf[] GetParticleCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos)
    {
        return Handle<Cuboidf[], IMultiBlockParticleColBoxes>(
            blockAccessor,
            pos.X + OffsetInv.X, pos.InternalY + OffsetInv.Y, pos.Z + OffsetInv.Z,
            (inf) => inf.MBGetParticleCollisionBoxes(blockAccessor, pos, OffsetInv),
            _ => [Cuboidf.Default()],
            block => block.GetParticleCollisionBoxes(blockAccessor, pos.AddCopy(OffsetInv))
        );
    }

    // This is copy-pasted from the `BlockMultiblock` class where it is private
    private static T Handle<T, TK>(
        IBlockAccessor blockAccessor,
        int x,
        int y,
        int z,
        BlockCallDelegateInterface<T, TK> onImplementsInterface,
        BlockCallDelegateBlock<T> onIsMultiblock,
        BlockCallDelegateBlock<T> onOtherwise
    )
        where TK : class
    {
        var block = blockAccessor.GetBlock((new Vec3i(x, y, z)).ToBlockPos());
        var blockInf = block as TK;

        if (blockInf == null)
        {
            blockInf = block.GetBehavior(typeof(TK), true) as TK;
        }

        if (blockInf != null)
        {
            return onImplementsInterface(blockInf);
        }

        // This is to prevent endless recursion in situations where blocks become incorrectly arranged
        if (block is BlockMultiblock)
        {
            return onIsMultiblock(block);
        }

        return onOtherwise(block);
    }
}
