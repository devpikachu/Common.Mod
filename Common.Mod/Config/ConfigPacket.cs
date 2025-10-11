using ProtoBuf;

namespace Common.Mod.Config;

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
public class ConfigPacket
{
    public string? Common { get; set; }
    public string? Server { get; set; }
}
