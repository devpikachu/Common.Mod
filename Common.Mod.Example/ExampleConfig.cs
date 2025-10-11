using Common.Mod.Common.Config;

namespace Common.Mod.Example;

public class ExampleConfig : IRootConfig
{
    public string TestString { get; set; } = "string";
    public int TestInt { get; set; } = 123456;

    public string Version() => "0.0.0";
    public RootConfigType Type() => RootConfigType.Common;
}
