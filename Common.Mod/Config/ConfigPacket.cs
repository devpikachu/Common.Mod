using ProtoBuf;

namespace Common.Mod.Config;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ConfigPacket
{
    [ProtoMember(0)]
    public ConfigPacketOperation Operation { get; set; }

    [ProtoMember(1)]
    public string? Common { get; set; }

    [ProtoMember(2)]
    public string? Server { get; set; }
}
