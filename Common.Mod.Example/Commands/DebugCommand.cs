using Common.Mod.Extensions;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Common.Mod.Example.Commands;

public class DebugCommand
{
    private readonly ICoreAPI _api;

    public DebugCommand(ICoreAPI api)
    {
        _api = api;

        var command = api.ChatCommands.Create("ex-debug")
            .RequiresPlayer()
            .RequiresPrivilege(Privilege.chat);

        {
            var walkCommand = command.BeginSubCommand("walk");

            walkCommand.BeginSubCommand("cuboid")
                .WithArgs(
                    api.ChatCommands.Parsers.WorldPosition("position"),
                    api.ChatCommands.Parsers.Vec3i("size")
                )
                .HandleWith(DebugWalkCuboid)
                .EndSubCommand();

            walkCommand.BeginSubCommand("cube")
                .WithArgs(
                    api.ChatCommands.Parsers.WorldPosition("position"),
                    api.ChatCommands.Parsers.IntRange("size", 0, int.MaxValue)
                )
                .HandleWith(DebugWalkCube)
                .EndSubCommand();

            walkCommand.BeginSubCommand("cylinder")
                .WithArgs(
                    api.ChatCommands.Parsers.WorldPosition("position"),
                    api.ChatCommands.Parsers.IntRange("radius", 0, int.MaxValue)
                )
                .HandleWith(DebugWalkCylinder)
                .EndSubCommand();

            walkCommand.BeginSubCommand("sphere")
                .WithArgs(
                    api.ChatCommands.Parsers.WorldPosition("position"),
                    api.ChatCommands.Parsers.IntRange("radius", 0, int.MaxValue)
                )
                .HandleWith(DebugWalkSphere)
                .EndSubCommand();

            walkCommand.EndSubCommand();
        }
    }

    private TextCommandResult DebugWalkCuboid(TextCommandCallingArgs args)
    {
        var position = (Vec3d)args[0];
        var size = (Vec3i)args[1];

        var halfSize = (size / 2)!;
        var minPos = position.SubCopy(halfSize.X, halfSize.Y, halfSize.Z);
        var maxPos = position.AddCopy(halfSize.X, halfSize.Y, halfSize.Z);

        var devastationRock = _api.World.BlockAccessor.GetBlock(new AssetLocation("game", "drock"))!;
        _api.World.WalkBlocksCuboid(minPos.AsBlockPos, maxPos.AsBlockPos,
            (_, x, y, z) => { _api.World.BlockAccessor.SetBlock(devastationRock.Id, new Vec3i(x, y, z).AsBlockPos); });

        return TextCommandResult.Success();
    }

    private TextCommandResult DebugWalkCube(TextCommandCallingArgs args)
    {
        var position = (Vec3d)args[0];
        var size = (int)args[1];

        var devastationRock = _api.World.BlockAccessor.GetBlock(new AssetLocation("game", "drock"))!;
        _api.World.WalkBlocksCube(position.AsBlockPos, size / 2,
            (_, x, y, z) => { _api.World.BlockAccessor.SetBlock(devastationRock.Id, new Vec3i(x, y, z).AsBlockPos); });

        return TextCommandResult.Success();
    }

    private TextCommandResult DebugWalkCylinder(TextCommandCallingArgs args)
    {
        var position = (Vec3d)args[0];
        var radius = (int)args[1];

        const int minYPos = 0;
        var maxYPos = _api.World.BlockAccessor.MapSizeY;

        var devastationRock = _api.World.BlockAccessor.GetBlock(new AssetLocation("game", "drock"))!;
        _api.World.WalkBlocksCylinder(position.AsBlockPos, radius, minYPos, maxYPos,
            (_, x, y, z) => { _api.World.BlockAccessor.SetBlock(devastationRock.Id, new Vec3i(x, y, z).AsBlockPos); });

        return TextCommandResult.Success();
    }

    private TextCommandResult DebugWalkSphere(TextCommandCallingArgs args)
    {
        var position = (Vec3d)args[0];
        var radius = (int)args[1];

        var devastationRock = _api.World.BlockAccessor.GetBlock(new AssetLocation("game", "drock"))!;
        _api.World.WalkBlocksSphere(position.AsBlockPos, radius,
            (_, x, y, z) => { _api.World.BlockAccessor.SetBlock(devastationRock.Id, new Vec3i(x, y, z).AsBlockPos); });

        return TextCommandResult.Success();
    }
}
