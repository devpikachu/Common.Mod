using JetBrains.Annotations;

namespace Common.Mod.Generator.Specs;

[UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.Members)]
public record ConfigEntrySpec
{
    public string Name { get; set; } = null!;
    public ConfigEntryTypeSpec Type { get; set; }
    public object? DefaultValue { get; set; }
    public string? Enum { get; set; }
    public string? Nested { get; set; }
    public string? Label { get; set; }
    public string? Description { get; set; }
}
