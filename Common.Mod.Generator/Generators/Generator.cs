using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Mod.Generator.Specs;
using Common.Mod.Generator.Utils;

namespace Common.Mod.Generator.Generators;

public abstract class Generator
{
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    protected static void AppendConfigEntry(
        IndentedStringBuilder sourceBuilder,
        ConfigEntrySpec spec,
        string nestedTypeSuffix = "",
        bool omitDescriptions = false
    )
    {
        string type;
        var nullable = spec.Nullable;
        var defaultValueBuilder = new StringBuilder(" = ");

        // Type
        {
            switch (spec.Type)
            {
                case ConfigEntryTypeSpec.Bool:
                    type = nullable ? "bool?" : "bool";
                    break;

                case ConfigEntryTypeSpec.Int:
                    type = nullable ? "int?" : "int";
                    break;

                case ConfigEntryTypeSpec.UInt:
                    type = nullable ? "uint?" : "uint";
                    break;

                case ConfigEntryTypeSpec.Float:
                    type = nullable ? "float?" : "float";
                    break;

                case ConfigEntryTypeSpec.String:
                    type = "string?";
                    nullable = true;
                    break;

                case ConfigEntryTypeSpec.Nested:
                    type = $"{spec.Nested!}{nestedTypeSuffix}?";
                    nullable = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Default value
        {
            if (spec.Type is not ConfigEntryTypeSpec.Nested && (spec.DefaultValue is not null || nullable))
            {
                defaultValueBuilder.Append(JsonSerializer.Serialize(spec.DefaultValue, JsonOptions));
            }
            else if (spec.Type is ConfigEntryTypeSpec.Nested)
            {
                defaultValueBuilder.Append("new()");
            }
            else
            {
                defaultValueBuilder.Append("default");
            }

            if (spec is { DefaultValue: not null, Type: ConfigEntryTypeSpec.Float })
            {
                defaultValueBuilder.Append('f');
            }

            defaultValueBuilder.Append(';');
        }

        if (!string.IsNullOrWhiteSpace(spec.Description) && !omitDescriptions)
        {
            // Entry description
            sourceBuilder.AppendLines(
                $"""

                 /// <summary>
                 /// {spec.Description}
                 /// </summary>
                 """
            );
        }

        sourceBuilder.AppendLine($"public {type} {spec.Name} {{ get; set; }}{defaultValueBuilder}");
    }

    protected static void ValidateSpec(RootConfigSpec? spec)
    {
        ThrowIf(() => spec is null);

        ThrowIf(() => string.IsNullOrWhiteSpace(spec!.ClassName));
        ThrowIf(() => string.IsNullOrWhiteSpace(spec!.ClassNamespace));
        ThrowIf(() => string.IsNullOrWhiteSpace(spec!.Version));

        ThrowIf(() => spec!.Entries.IsDefaultOrEmpty);
        foreach (var entrySpec in spec!.Entries)
        {
            ThrowIf(() => string.IsNullOrWhiteSpace(entrySpec.Name));
            ThrowIf(() => entrySpec.Type == ConfigEntryTypeSpec.Nested && string.IsNullOrWhiteSpace(entrySpec.Nested));
        }
    }

    protected static void ThrowIf(Func<bool> predicate)
    {
        if (predicate.Invoke())
        {
            throw new InvalidOperationException();
        }
    }
}
