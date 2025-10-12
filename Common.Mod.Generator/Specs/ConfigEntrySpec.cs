namespace Common.Mod.Generator.Specs;

public record ConfigEntrySpec
{
    public string Name { get; set; } = null!;
    public ConfigEntryTypeSpec Type { get; set; }
    public object? DefaultValue { get; set; } = null;
    public string? Enum { get; set; } = null;
    public string? Nested { get; set; } = null;
    public string? Label { get; set; } = null;
    public string? Description { get; set; } = null;
}
