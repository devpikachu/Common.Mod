using Common.Mod.Common.Config;
using Common.Mod.Config;
using ImGuiNET;

namespace Common.Mod.Example;

public enum ExampleEnumConfig
{
    Value1,
    Value2,
    Value3
}

public class ExampleNestedConfig : IConfig
{
    private static readonly ExampleNestedConfig Defaults = new();

    private bool _boolValue;
    private int _int32Value = -123;
    private long _int64Value = -123;
    private uint _uint32Value = 456;
    private ulong _uint64Value = 456;
    private float _floatValue = 78.9f;
    private double _doubleValue = 78.9d;
    private string _stringValue = "string";
    private ExampleEnumConfig _enumValue = ExampleEnumConfig.Value1;

    public bool BoolValue
    {
        get => _boolValue;
        set => _boolValue = value;
    }

    public int Int32Value
    {
        get => _int32Value;
        set => _int32Value = value;
    }

    public long Int64Value
    {
        get => _int64Value;
        set => _int64Value = value;
    }

    public uint UInt32Value
    {
        get => _uint32Value;
        set => _uint32Value = value;
    }

    public ulong UInt64Value
    {
        get => _uint64Value;
        set => _uint64Value = value;
    }

    public float FloatValue
    {
        get => _floatValue;
        set => _floatValue = value;
    }

    public double DoubleValue
    {
        get => _doubleValue;
        set => _doubleValue = value;
    }

    public string StringValue
    {
        get => _stringValue;
        set => _stringValue = value ?? throw new ArgumentNullException(nameof(value));
    }

    public ExampleEnumConfig EnumValue
    {
        get => _enumValue;
        set => _enumValue = value;
    }

    public void Reset()
    {
        _boolValue = Defaults._boolValue;
        _int32Value = Defaults._int32Value;
        _int64Value = Defaults._int64Value;
        _uint32Value = Defaults._uint32Value;
        _uint64Value = Defaults._uint64Value;
        _floatValue = Defaults._floatValue;
        _doubleValue = Defaults._doubleValue;
        _stringValue = Defaults._stringValue;
        _enumValue = Defaults._enumValue;
    }

    public void Render(IConfigUi configUi)
    {
        configUi.Bool(ref _boolValue, Defaults._boolValue, "bool", "Bool", "This is a boolean");
        configUi.Int32(ref _int32Value, Defaults._int32Value, "int32", "Int32", "This is a 32-bit integer");
        configUi.Int64(ref _int64Value, Defaults._int64Value, "int64", "Int64", "This is a 64-bit integer");
        configUi.UInt32(ref _uint32Value, Defaults._uint32Value, "uint32", "UInt32", "This is an unsigned 32-bit integer");
        configUi.UInt64(ref _uint64Value, Defaults._uint64Value, "uint64", "UInt64", "This is an unsigned 64-bit integer");
        configUi.Float(ref _floatValue, Defaults._floatValue, "float", "Float", "This is a float");
        configUi.Double(ref _doubleValue, Defaults._doubleValue, "double", "Double", "This is a double");
        configUi.String(ref _stringValue, Defaults._stringValue, "string", "String", "This is a string");
        configUi.Enum(ref _enumValue, Defaults._enumValue, "enum", "Enum", "This is an enum");
    }
}

public class ExampleConfig : IRootConfig
{
    private static readonly ExampleConfig Defaults = new();

    private bool _boolValue;
    private int _int32Value = -123;
    private long _int64Value = -123;
    private uint _uint32Value = 456;
    private ulong _uint64Value = 456;
    private float _floatValue = 78.9f;
    private double _doubleValue = 78.9d;
    private string _stringValue = "string";
    private ExampleEnumConfig _enumValue = ExampleEnumConfig.Value1;
    private ExampleNestedConfig _nestedValue = new();

    public bool BoolValue
    {
        get => _boolValue;
        set => _boolValue = value;
    }

    public int Int32Value
    {
        get => _int32Value;
        set => _int32Value = value;
    }

    public long Int64Value
    {
        get => _int64Value;
        set => _int64Value = value;
    }

    public uint UInt32Value
    {
        get => _uint32Value;
        set => _uint32Value = value;
    }

    public ulong UInt64Value
    {
        get => _uint64Value;
        set => _uint64Value = value;
    }

    public float FloatValue
    {
        get => _floatValue;
        set => _floatValue = value;
    }

    public double DoubleValue
    {
        get => _doubleValue;
        set => _doubleValue = value;
    }

    public string StringValue
    {
        get => _stringValue;
        set => _stringValue = value ?? throw new ArgumentNullException(nameof(value));
    }

    public ExampleEnumConfig EnumValue
    {
        get => _enumValue;
        set => _enumValue = value;
    }

    public ExampleNestedConfig NestedValue
    {
        get => _nestedValue;
        set => _nestedValue = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Version() => "0.0.0";
    public virtual RootConfigType Type() => RootConfigType.Common;

    public void Reset()
    {
        _boolValue = Defaults._boolValue;
        _int32Value = Defaults._int32Value;
        _int64Value = Defaults._int64Value;
        _uint32Value = Defaults._uint32Value;
        _uint64Value = Defaults._uint64Value;
        _floatValue = Defaults._floatValue;
        _doubleValue = Defaults._doubleValue;
        _stringValue = Defaults._stringValue;
        _enumValue = Defaults._enumValue;
    }

    public void Render(IConfigUi configUi)
    {
        configUi.Bool(ref _boolValue, Defaults._boolValue, "bool", "Bool", "This is a boolean");
        configUi.Int32(ref _int32Value, Defaults._int32Value, "int32", "Int32", "This is a 32-bit integer");
        configUi.Int64(ref _int64Value, Defaults._int64Value, "int64", "Int64", "This is a 64-bit integer");
        configUi.UInt32(ref _uint32Value, Defaults._uint32Value, "uint32", "UInt32", "This is an unsigned 32-bit integer");
        configUi.UInt64(ref _uint64Value, Defaults._uint64Value, "uint64", "UInt64", "This is an unsigned 64-bit integer");
        configUi.Float(ref _floatValue, Defaults._floatValue, "float", "Float", "This is a float");
        configUi.Double(ref _doubleValue, Defaults._doubleValue, "double", "Double", "This is a double");
        configUi.String(ref _stringValue, Defaults._stringValue, "string", "String", "This is a string");
        configUi.Enum(ref _enumValue, Defaults._enumValue, "enum", "Enum", "This is an enum");
        configUi.Nested(_nestedValue, "nested", "Nested");
    }
}

public class ExampleCommonConfig : ExampleConfig
{
    public override RootConfigType Type() => RootConfigType.Common;
}

public class ExampleServerConfig : ExampleConfig
{
    public override RootConfigType Type() => RootConfigType.Server;
}

public class ExampleClientConfig : ExampleConfig
{
    public override RootConfigType Type() => RootConfigType.Client;
}
