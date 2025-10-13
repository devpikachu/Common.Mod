using Common.Mod.Common.Config;
using Common.Mod.Common.Core;
using DryIoc.ImTools;
using ImGuiNET;

namespace Common.Mod.Config;

public class ConfigUi : IConfigUi
{
    private const float FloatStep = 1.0f;
    private const double DoubleStep = 1.0d;
    private const float FloatStepFast = 10.0f;
    private const double DoubleStepFast = 10.0d;
    private const uint StringMaxLength = 256;

    private static readonly int Int32Step = 1;
    private static readonly long Int64Step = 1;
    private static readonly uint UInt32Step = 1;
    private static readonly ulong UInt64Step = 1;
    private static readonly int Int32StepFast = 10;
    private static readonly long Int64StepFast = 10;
    private static readonly uint UInt32StepFast = 10;
    private static readonly ulong UInt64StepFast = 10;

    private readonly ITranslations _translations;

    public ConfigUi(ITranslations translations)
    {
        _translations = translations;
    }

    public void Label(string value, bool muted = false)
    {
        ImGui.PushTextWrapPos(ImGui.GetContentRegionAvail().X);

        if (muted)
        {
            ImGui.TextDisabled(_translations.Get(value));
        }
        else
        {
            ImGui.Text(_translations.Get(value));
        }

        ImGui.PopTextWrapPos();
    }

    public void Bool(ref bool value, bool defaultValue, string identifier, string label, string? description = null)
    {
        ImGui.PushID(identifier);
        ImGui.BeginGroup();

        ResetButton(ref value, defaultValue);
        ImGui.Checkbox(_translations.Get(label), ref value);
        Description(description);

        ImGui.EndGroup();
        ImGui.PopID();
    }

    public unsafe void Int32(ref int value, int defaultValue, string identifier, string label, string? description = null)
    {
        fixed (int* valuePtr = &value, stepPtr = &Int32Step, stepFastPtr = &Int32StepFast)
        {
            ImGui.PushID(identifier);
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
            ImGui.BeginGroup();

            ResetButton(ref value, defaultValue);
            ImGui.InputScalar(_translations.Get(label), ImGuiDataType.S32, new IntPtr(valuePtr), new IntPtr(stepPtr), new IntPtr(stepFastPtr));
            Description(description);

            ImGui.EndGroup();
            ImGui.PopItemWidth();
            ImGui.PopID();
        }
    }

    public unsafe void Int64(ref long value, long defaultValue, string identifier, string label, string? description = null)
    {
        fixed (long* valuePtr = &value, stepPtr = &Int64Step, stepFastPtr = &Int64StepFast)
        {
            ImGui.PushID(identifier);
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
            ImGui.BeginGroup();

            ResetButton(ref value, defaultValue);
            ImGui.InputScalar(_translations.Get(label), ImGuiDataType.S64, new IntPtr(valuePtr), new IntPtr(stepPtr), new IntPtr(stepFastPtr));
            Description(description);

            ImGui.EndGroup();
            ImGui.PopItemWidth();
            ImGui.PopID();
        }
    }

    public unsafe void UInt32(ref uint value, uint defaultValue, string identifier, string label, string? description = null)
    {
        fixed (uint* valuePtr = &value, stepPtr = &UInt32Step, stepFastPtr = &UInt32StepFast)
        {
            ImGui.PushID(identifier);
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
            ImGui.BeginGroup();

            ResetButton(ref value, defaultValue);
            ImGui.InputScalar(_translations.Get(label), ImGuiDataType.U32, new IntPtr(valuePtr), new IntPtr(stepPtr), new IntPtr(stepFastPtr));
            Description(description);

            ImGui.EndGroup();
            ImGui.PopItemWidth();
            ImGui.PopID();
        }
    }

    public unsafe void UInt64(ref ulong value, ulong defaultValue, string identifier, string label, string? description = null)
    {
        fixed (ulong* valuePtr = &value, stepPtr = &UInt64Step, stepFastPtr = &UInt64StepFast)
        {
            ImGui.PushID(identifier);
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
            ImGui.BeginGroup();

            ResetButton(ref value, defaultValue);
            ImGui.InputScalar(_translations.Get(label), ImGuiDataType.U64, new IntPtr(valuePtr), new IntPtr(stepPtr), new IntPtr(stepFastPtr));
            Description(description);

            ImGui.EndGroup();
            ImGui.PopItemWidth();
            ImGui.PopID();
        }
    }

    public void Float(ref float value, float defaultValue, string identifier, string label, string? description = null)
    {
        ImGui.PushID(identifier);
        ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
        ImGui.BeginGroup();

        ResetButton(ref value, defaultValue);
        ImGui.InputFloat(_translations.Get(label), ref value, FloatStep, FloatStepFast);
        Description(description);

        ImGui.EndGroup();
        ImGui.PopItemWidth();
        ImGui.PopID();
    }

    public void Double(ref double value, double defaultValue, string identifier, string label, string? description = null)
    {
        ImGui.PushID(identifier);
        ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
        ImGui.BeginGroup();

        ResetButton(ref value, defaultValue);
        ImGui.InputDouble(_translations.Get(label), ref value, DoubleStep, DoubleStepFast);
        Description(description);

        ImGui.EndGroup();
        ImGui.PopItemWidth();
        ImGui.PopID();
    }

    public void String(ref string value, string defaultValue, string identifier, string label, string? description = null)
    {
        ImGui.PushID(identifier);
        ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
        ImGui.BeginGroup();

        ResetButton(ref value, defaultValue);
        ImGui.InputText(_translations.Get(label), ref value, StringMaxLength);
        Description(description);

        ImGui.EndGroup();
        ImGui.PopItemWidth();
        ImGui.PopID();
    }

    public void Enum<TEnumConfig>(ref TEnumConfig value, TEnumConfig defaultValue, string identifier, string label, string? description = null)
        where TEnumConfig : struct, Enum
    {
        var values = System.Enum.GetNames<TEnumConfig>();
        var currentValue = value.ToString();
        var currentIndex = values.IndexOf(v => v == currentValue);
        var newIndex = currentIndex;

        ImGui.PushID(identifier);
        ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
        ImGui.BeginGroup();

        var reset = ResetButton(ref value, defaultValue);

        if (ImGui.BeginCombo(_translations.Get(label), _translations.Get(currentValue)))
        {
            for (var i = 0; i < values.Length; i++)
            {
                var selected = currentIndex == i;

                if (ImGui.Selectable(_translations.Get(values[i]), selected))
                {
                    newIndex = i;
                }

                if (selected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }

            ImGui.EndCombo();
        }

        Description(description);

        ImGui.EndGroup();
        ImGui.PopItemWidth();
        ImGui.PopID();

        if (reset)
        {
            value = defaultValue;
            return;
        }

        var newValue = values[newIndex];
        value = System.Enum.Parse<TEnumConfig>(newValue);
    }

    public void Nested<TNestedConfig>(TNestedConfig config, string identifier, string label)
        where TNestedConfig : IConfig
    {
        ImGui.NewLine();
        ImGui.SeparatorText(_translations.Get(label));
        ImGui.PushID(identifier);
        config.Render(this);
        ImGui.PopID();
    }

    private bool ResetButton<TValue>(ref TValue value, TValue defaultValue)
    {
        var result = false;

        if (ImGui.Button($"~"))
        {
            value = defaultValue;
            result = true;
        }

        ImGui.SetItemTooltip(_translations.Get(key: "config--button--reset", defaultValue!.ToString()!));
        ImGui.SameLine();

        return result;
    }

    private void Description(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return;
        }

        ImGui.Indent();
        Label(description, muted: true);
        ImGui.Unindent();
    }
}
