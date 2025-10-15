using Vintagestory.API.Common;

namespace Common.Mod.Exceptions;

[Serializable]
public class InvalidSideException : Exception
{
    public InvalidSideException(EnumAppSide side, Exception? innerException = null) : base($"Reached code that's invalid for the {side} side", innerException)
    {
    }
}
