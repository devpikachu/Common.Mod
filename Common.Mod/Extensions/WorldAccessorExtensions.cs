using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Common.Mod.Extensions;

[UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Members)]
[SuppressMessage("ReSharper", "MoveLocalFunctionAfterJumpStatement")]
public static class WorldAccessorExtensions
{
    public delegate void WalkBlocksHandler(Block block, int x, int y, int z);

    public static void WalkBlocksCuboid(this IWorldAccessor worldAccessor, BlockPos minPos, BlockPos maxPos, WalkBlocksHandler onBlock)
    {
        worldAccessor.BlockAccessor.WalkBlocks(minPos, maxPos, (block, x, y, z) => onBlock(block, x, y, z), centerOrder: false);
    }

    public static void WalkBlocksCube(this IWorldAccessor worldAccessor, BlockPos centerPos, int halfSize, WalkBlocksHandler onBlock)
    {
        var minPos = centerPos.SubCopy(halfSize, halfSize, halfSize);
        var maxPos = centerPos.AddCopy(halfSize, halfSize, halfSize);
        worldAccessor.WalkBlocksCuboid(minPos, maxPos, onBlock);
    }

    public static void WalkBlocksCylinder(
        this IWorldAccessor worldAccessor,
        BlockPos centerPos,
        int radius,
        int minYPos,
        int maxYPos,
        WalkBlocksHandler onBlock
    )
    {
        var minPos = centerPos.SubCopy(radius, 0, radius);
        minPos.Y = minYPos;
        var maxPos = centerPos.AddCopy(radius, 0, radius);
        maxPos.Y = maxYPos;

        bool Predicate(int x, int y, int z) => IsInCircle(centerPos.ToVec3i(), radius, x, z);
        worldAccessor.WalkBlocks(minPos, maxPos, Predicate, onBlock);
    }

    public static void WalkBlocksCylinder(
        this IWorldAccessor worldAccessor,
        BlockPos centerPos,
        int radius,
        int halfHeight,
        WalkBlocksHandler onBlock
    )
    {
        var minYPos = centerPos.Y - halfHeight;
        var maxYPos = centerPos.Y + halfHeight;
        worldAccessor.WalkBlocksCylinder(centerPos, radius, minYPos, maxYPos, onBlock);
    }

    public static void WalkBlocksSphere(
        this IWorldAccessor worldAccessor,
        BlockPos centerPos,
        int radius,
        WalkBlocksHandler onBlock
    )
    {
        var minPos = centerPos.SubCopy(radius, radius, radius);
        var maxPos = centerPos.AddCopy(radius, radius, radius);

        bool Predicate(int x, int y, int z) => IsInSphere(centerPos.ToVec3i(), radius, x, y, z);
        worldAccessor.WalkBlocks(minPos, maxPos, Predicate, onBlock);
    }

    private static void WalkBlocks(
        this IWorldAccessor worldAccessor,
        BlockPos minPos,
        BlockPos maxPos,
        System.Func<int, int, int, bool> predicate,
        WalkBlocksHandler onBlock
    )
    {
        var blockAccessor = worldAccessor.BlockAccessor;
        var mapSize = blockAccessor.MapSize!;

        var minXPos = GameMath.Clamp(Math.Min(minPos.X, maxPos.X), 0, mapSize.X);
        var maxXPos = GameMath.Clamp(Math.Max(minPos.X, maxPos.X), 0, mapSize.X);
        var minYPos = GameMath.Clamp(Math.Min(minPos.Y, maxPos.Y), 0, mapSize.Y);
        var maxYPos = GameMath.Clamp(Math.Max(minPos.Y, maxPos.Y), 0, mapSize.Y);
        var minZPos = GameMath.Clamp(Math.Min(minPos.Z, maxPos.Z), 0, mapSize.Z);
        var maxZPos = GameMath.Clamp(Math.Max(minPos.Z, maxPos.Z), 0, mapSize.Z);

        var minChunkXPos = minXPos / GlobalConstants.ChunkSize;
        var maxChunkXPos = maxXPos / GlobalConstants.ChunkSize;
        var minChunkYPos = minYPos / GlobalConstants.ChunkSize;
        var maxChunkYPos = maxYPos / GlobalConstants.ChunkSize;
        var minChunkZPos = minZPos / GlobalConstants.ChunkSize;
        var maxChunkZPos = maxZPos / GlobalConstants.ChunkSize;

        var dimensionYOffset = minPos.dimension * GlobalConstants.DimensionSizeInChunks;

        var chunks = blockAccessor.LoadChunksToCache(
            new Vec3i(minChunkXPos, minChunkYPos + dimensionYOffset, minChunkZPos),
            new Vec3i(maxChunkXPos, maxChunkYPos + dimensionYOffset, maxChunkZPos)
        );
        var chunkCountX = maxChunkXPos - minChunkXPos + 1;
        var chunkCountZ = maxChunkZPos - minChunkZPos + 1;

        for (var y = minYPos; y <= maxYPos; y++)
        {
            var chunkIndexY = (y / GlobalConstants.ChunkSize - minChunkYPos) * chunkCountZ - minChunkZPos;
            for (var z = minZPos; z <= maxZPos; z++)
            {
                var chunkIndexBase = (chunkIndexY + z / GlobalConstants.ChunkSize) * chunkCountX - minChunkXPos;
                var blockIndexBase = (y % GlobalConstants.ChunkSize * GlobalConstants.ChunkSize + z % GlobalConstants.ChunkSize) * GlobalConstants.ChunkSize;
                for (var x = minXPos; x <= maxXPos; x++)
                {
                    if (!predicate.Invoke(x, y, z))
                    {
                        continue;
                    }

                    var chunk = chunks[chunkIndexBase + x / GlobalConstants.ChunkSize];

                    if (chunk is null)
                    {
                        continue;
                    }

                    var blockIndex = blockIndexBase + x % GlobalConstants.ChunkSize;
                    var blockId = chunk.GetFluid(blockIndex);

                    if (blockId != 0)
                    {
                        onBlock(worldAccessor.Blocks[blockId], x, y, z);
                    }

                    blockId = chunk.GetSolidBlock(blockIndex);
                    onBlock(worldAccessor.Blocks[blockId], x, y, z);
                }
            }
        }
    }

    // https://stackoverflow.com/a/7227057
    private static bool IsInCircle(Vec3i center, int radius, int x, int z)
    {
        var dX = Math.Abs(x - center.X);
        var dZ = Math.Abs(z - center.Z);

        if (dX + dZ <= radius)
        {
            return true;
        }

        if (dX > radius || dZ > radius)
        {
            return false;
        }

        return Math.Pow(dX, 2) + Math.Pow(dX, 2) <= Math.Pow(radius, 2);
    }

    private static bool IsInSphere(Vec3i center, int radius, int x, int y, int z)
    {
        var dX = Math.Abs(x - center.X);
        var dY = Math.Abs(y - center.Y);
        var dZ = Math.Abs(z - center.Z);

        if (dX + dY + dZ <= radius)
        {
            return true;
        }

        if (dX > radius || dY > radius || dZ > radius)
        {
            return false;
        }

        return Math.Pow(dX, 2) + Math.Pow(dY, 2) + Math.Pow(dX, 2) <= Math.Pow(radius, 2);
    }
}
