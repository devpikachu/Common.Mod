using ProtoBuf;

namespace Common.Mod.Config;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ConfigPacket
{
    [ProtoMember(1)]
    public ConfigPacketOperation Operation { get; set; }

    [ProtoMember(2)]
    public string? Common { get; set; }

    [ProtoMember(3)]
    public string? Server { get; set; }
}
