using System.Diagnostics.CodeAnalysis;
using Common.Mod.Common.Config;
using DryIoc.ImTools;
using ImGuiNET;

namespace Common.Mod.Config;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class ConfigUI
{
    private const float _floatStep = 1.0f;
    private const double _doubleStep = 1.0d;
    private const float _floatStepFast = 10.0f;
    private const double _doubleStepFast = 10.0d;
    private const uint _stringMaxLength = 256;

    private static readonly int _int32Step = 1;
    private static readonly long _int64Step = 1;
    private static readonly uint _uint32Step = 1;
    private static readonly ulong _uint64Step = 1;
    private static readonly int _int32StepFast = 10;
    private static readonly long _int64StepFast = 10;
    private static readonly uint _uint32StepFast = 10;
    private static readonly ulong _uint64StepFast = 10;

    public static void Label(string value, bool muted = false)
    {
        ImGui.PushTextWrapPos(ImGui.GetContentRegionAvail().X);

        if (muted)
        {
            ImGui.TextDisabled(value);
        }
        else
        {
            ImGui.Text(value);
        }

        ImGui.PopTextWrapPos();
    }

    public static void Bool(ref bool value, bool defaultValue, string identifier, string label, string? description = null)
    {
        ImGui.PushID(identifier);
        ImGui.BeginGroup();

        ResetButton(ref value, defaultValue);
        ImGui.Checkbox(label, ref value);
        Description(description);

        ImGui.EndGroup();
        ImGui.PopID();
    }

    public static unsafe void Int32(ref int value, int defaultValue, string identifier, string label, string? description = null)
    {
        fixed (int* valuePtr = &value, stepPtr = &_int32Step, stepFastPtr = &_int32StepFast)
        {
            ImGui.PushID(identifier);
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
            ImGui.BeginGroup();

            ResetButton(ref value, defaultValue);
            ImGui.InputScalar(label, ImGuiDataType.S32, new IntPtr(valuePtr), new IntPtr(stepPtr), new IntPtr(stepFastPtr));
            Description(description);

            ImGui.EndGroup();
            ImGui.PopItemWidth();
            ImGui.PopID();
        }
    }

    public static unsafe void Int64(ref long value, long defaultValue, string identifier, string label, string? description = null)
    {
        fixed (long* valuePtr = &value, stepPtr = &_int64Step, stepFastPtr = &_int64StepFast)
        {
            ImGui.PushID(identifier);
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
            ImGui.BeginGroup();

            ResetButton(ref value, defaultValue);
            ImGui.InputScalar(label, ImGuiDataType.S64, new IntPtr(valuePtr), new IntPtr(stepPtr), new IntPtr(stepFastPtr));
            Description(description);

            ImGui.EndGroup();
            ImGui.PopItemWidth();
            ImGui.PopID();
        }
    }

    public static unsafe void UInt32(ref uint value, uint defaultValue, string identifier, string label, string? description = null)
    {
        fixed (uint* valuePtr = &value, stepPtr = &_uint32Step, stepFastPtr = &_uint32StepFast)
        {
            ImGui.PushID(identifier);
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
            ImGui.BeginGroup();

            ResetButton(ref value, defaultValue);
            ImGui.InputScalar(label, ImGuiDataType.U32, new IntPtr(valuePtr), new IntPtr(stepPtr), new IntPtr(stepFastPtr));
            Description(description);

            ImGui.EndGroup();
            ImGui.PopItemWidth();
            ImGui.PopID();
        }
    }

    public static unsafe void UInt64(ref ulong value, ulong defaultValue, string identifier, string label, string? description = null)
    {
        fixed (ulong* valuePtr = &value, stepPtr = &_uint64Step, stepFastPtr = &_uint64StepFast)
        {
            ImGui.PushID(identifier);
            ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
            ImGui.BeginGroup();

            ResetButton(ref value, defaultValue);
            ImGui.InputScalar(label, ImGuiDataType.U64, new IntPtr(valuePtr), new IntPtr(stepPtr), new IntPtr(stepFastPtr));
            Description(description);

            ImGui.EndGroup();
            ImGui.PopItemWidth();
            ImGui.PopID();
        }
    }

    public static void Float(ref float value, float defaultValue, string identifier, string label, string? description = null)
    {
        ImGui.PushID(identifier);
        ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
        ImGui.BeginGroup();

        ResetButton(ref value, defaultValue);
        ImGui.InputFloat(label, ref value, _floatStep, _floatStepFast);
        Description(description);

        ImGui.EndGroup();
        ImGui.PopItemWidth();
        ImGui.PopID();
    }

    public static void Double(ref double value, double defaultValue, string identifier, string label, string? description = null)
    {
        ImGui.PushID(identifier);
        ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
        ImGui.BeginGroup();

        ResetButton(ref value, defaultValue);
        ImGui.InputDouble(label, ref value, _doubleStep, _doubleStepFast);
        Description(description);

        ImGui.EndGroup();
        ImGui.PopItemWidth();
        ImGui.PopID();
    }

    public static void String(ref string value, string defaultValue, string identifier, string label, string? description = null)
    {
        ImGui.PushID(identifier);
        ImGui.PushItemWidth(ImGui.GetWindowWidth() * 0.5f);
        ImGui.BeginGroup();

        ResetButton(ref value, defaultValue);
        ImGui.InputText(label, ref value, _stringMaxLength);
        Description(description);

        ImGui.EndGroup();
        ImGui.PopItemWidth();
        ImGui.PopID();
    }

    public static void Enum<TEnumConfig>(ref TEnumConfig value, TEnumConfig defaultValue, string identifier, string label, string? description = null)
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

        if (ImGui.BeginCombo(label, currentValue))
        {
            for (var i = 0; i < values.Length; i++)
            {
                var selected = currentIndex == i;

                if (ImGui.Selectable(values[i], selected))
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

    public static void Nested<TNestedConfig>(TNestedConfig config, string identifier, string label)
        where TNestedConfig : IConfig
    {
        ImGui.NewLine();
        ImGui.SeparatorText(label);
        ImGui.PushID(identifier);
        config.Render();
        ImGui.PopID();
    }

    private static bool ResetButton<TValue>(ref TValue value, TValue defaultValue)
    {
        var result = false;

        if (ImGui.Button($"~"))
        {
            value = defaultValue;
            result = true;
        }

        ImGui.SetItemTooltip($"Reset to default: {defaultValue}");
        ImGui.SameLine();

        return result;
    }

    private static void Description(string? description)
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
