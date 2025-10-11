using ProtoBuf;

namespace Common.Mod.Config;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ConfigPacket
{
    public string Data { get; set; } = string.Empty;
}
