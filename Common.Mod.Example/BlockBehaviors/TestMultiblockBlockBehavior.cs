using Common.Mod.Blocks;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace Common.Mod.Example.BlockBehaviors;

public class TestMultiblockBlockBehavior : StrongBlockBehavior, IMultiBlockColSelBoxes, IMultiBlockParticleColBoxes
{
    public const string RegistryId = "TestMultiblock";

    public TestMultiblockBlockBehavior(Block block) : base(block)
    {
    }

    public override bool CanPlaceBlock(IWorldAccessor world, IPlayer byPlayer, BlockSelection blockSel, ref EnumHandling handling, ref string failureCode)
    {
        var topBlockSel = blockSel.AddPosCopy(0, 1, 0);

        var bottomBlockPlaceable = world.BlockAccessor.GetBlock(blockSel.Position).IsReplacableBy(block);
        var topBlockPlaceable = world.BlockAccessor.GetBlock(topBlockSel.Position).IsReplacableBy(block);

        if (!bottomBlockPlaceable || !topBlockPlaceable)
        {
            handling = EnumHandling.PreventDefault;
            failureCode = "notenoughspace";
            return false;
        }

        return base.CanPlaceBlock(world, byPlayer, blockSel, ref handling, ref failureCode);
    }

    public override bool TryPlaceBlock(
        IWorldAccessor world,
        IPlayer byPlayer,
        ItemStack itemstack,
        BlockSelection blockSel,
        ref EnumHandling handling,
        ref string failureCode
    )
    {
        handling = EnumHandling.PreventDefault;

        if (!block.CanPlaceBlock(world, byPlayer, blockSel, ref failureCode))
        {
            return false;
        }

        world.BlockAccessor.SetBlock(block.Id, blockSel.Position);

        if (world.Side is EnumAppSide.Server)
        {
            var assetLocation = new AssetLocation("common:multiblock-monolithic-0-p1-0");
            var topBlockSel = blockSel.AddPosCopy(0, 1, 0);
            world.BlockAccessor.SetBlock(world.GetBlock(assetLocation).Id, topBlockSel.Position);
            world.BlockAccessor.TriggerNeighbourBlockUpdate(topBlockSel.Position);
        }

        return true;
    }

    public override void OnBlockRemoved(IWorldAccessor world, BlockPos pos, ref EnumHandling handling)
    {
        if (world.Side is EnumAppSide.Client)
        {
            return;
        }

        var topBlockPos = pos.Add(0, 1, 0);
        if (world.BlockAccessor.GetBlock(topBlockPos) is BlockMultiblock)
        {
            world.BlockAccessor.SetBlock(0, topBlockPos);
            if (world.Side is EnumAppSide.Server)
            {
                world.BlockAccessor.TriggerNeighbourBlockUpdate(topBlockPos);
            }
        }

        base.OnBlockRemoved(world, pos, ref handling);
    }

    public override Cuboidf[] GetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos, ref EnumHandling handled)
    {
        handled = EnumHandling.PreventSubsequent;
        return block.SelectionBoxes;
    }

    public override Cuboidf[] GetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos, ref EnumHandling handled)
    {
        handled = EnumHandling.PreventSubsequent;
        return block.SelectionBoxes;
    }

    public override Cuboidf[] GetParticleCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos, ref EnumHandling handled)
    {
        handled = EnumHandling.PreventSubsequent;
        return block.SelectionBoxes;
    }

    public Cuboidf[] MBGetCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset)
    {
        return block.SelectionBoxes.Select(box => box.OffsetCopy(offset)).ToArray();
    }

    public Cuboidf[] MBGetSelectionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset)
    {
        return block.SelectionBoxes.Select(box => box.OffsetCopy(offset)).ToArray();
    }

    public Cuboidf[] MBGetParticleCollisionBoxes(IBlockAccessor blockAccessor, BlockPos pos, Vec3i offset)
    {
        return block.SelectionBoxes.Select(box => box.OffsetCopy(offset)).ToArray();
    }
}
