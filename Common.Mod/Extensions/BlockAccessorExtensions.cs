using JetBrains.Annotations;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.Common;

namespace Common.Mod.Extensions;

[UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Members)]
public static class BlockAccessorExtensions
{
    public delegate void ChunkMissingHandler(int x, int y, int z);

    public static ChunkData?[] LoadChunksToCache(this IBlockAccessor blockAccessor, Vec3i minPos, Vec3i maxPos, ChunkMissingHandler? onChunkMissing = null)
    {
        var countX = maxPos.X - minPos.X + 1;
        var countY = maxPos.Y - minPos.Y + 1;
        var countZ = maxPos.Z - minPos.Z + 1;
        var chunks = new ChunkData?[countX * countY * countZ];

        for (var y = minPos.Y; y <= maxPos.Y; y++)
        {
            var indexY = (y - minPos.Y) * countZ - minPos.Z;
            for (var z = minPos.Z; z <= maxPos.Z; z++)
            {
                var indexBase = (indexY + z) * countX - minPos.X;
                for (var x = minPos.X; x <= maxPos.X; x++)
                {
                    var chunk = blockAccessor.GetChunk(x, y, z);

                    if (chunk is null)
                    {
                        chunks[indexBase + x] = null;
                        onChunkMissing?.Invoke(x, y, z);
                        continue;
                    }

                    chunk.Unpack();
                    chunks[indexBase + x] = chunk.Data as ChunkData;
                }
            }
        }

        return chunks;
    }
}
