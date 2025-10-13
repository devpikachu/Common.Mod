namespace Common.Mod.Common.Config;

public interface IConfigUi
{
    public void Label(string value, bool muted = false);
    public void Bool(ref bool value, bool defaultValue, string identifier, string label, string? description = null);
    public void Int32(ref int value, int defaultValue, string identifier, string label, string? description = null);
    public void Int64(ref long value, long defaultValue, string identifier, string label, string? description = null);
    public void UInt32(ref uint value, uint defaultValue, string identifier, string label, string? description = null);
    public void UInt64(ref ulong value, ulong defaultValue, string identifier, string label, string? description = null);
    public void Float(ref float value, float defaultValue, string identifier, string label, string? description = null);
    public void Double(ref double value, double defaultValue, string identifier, string label, string? description = null);
    public void String(ref string value, string defaultValue, string identifier, string label, string? description = null);

    public void Enum<TEnumConfig>(ref TEnumConfig value, TEnumConfig defaultValue, string identifier, string label, string? description = null)
        where TEnumConfig : struct, Enum;

    public void Nested<TNestedConfig>(TNestedConfig config, string identifier, string label)
        where TNestedConfig : IConfig;
}
