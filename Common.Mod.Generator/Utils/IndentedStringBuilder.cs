using System.IO;
using System.Text;

namespace Common.Mod.Generator.Utils;

public class IndentedStringBuilder
{
    private const byte IndentSize = 4;
    private int _indent;
    private bool _indentPending = true;

    private readonly StringBuilder _stringBuilder = new();

    public void Append(string value)
    {
        DoIndent();
        _stringBuilder.Append(value);
    }

    public void AppendLine()
    {
        AppendLine(string.Empty);
    }

    public void AppendLine(string value)
    {
        if (value.Length != 0)
        {
            DoIndent();
        }

        _stringBuilder.AppendLine(value);
        _indentPending = true;
    }

    public void AppendLines(string value, bool skipFinalNewline = false)
    {
        using (var reader = new StringReader(value))
        {
            var first = true;
            while (reader.ReadLine() is { } line)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    AppendLine();
                }

                if (line.Length != 0)
                {
                    Append(line);
                }
            }
        }

        if (!skipFinalNewline)
        {
            AppendLine();
        }
    }

    public void IncrementIndent()
    {
        _indent++;
    }

    public void DecrementIndent()
    {
        if (_indent > 0)
        {
            _indent--;
        }
    }

    public override string ToString() => _stringBuilder.ToString();

    private void DoIndent()
    {
        if (_indentPending && _indent > 0)
        {
            _stringBuilder.Append(' ', _indent * IndentSize);
        }

        _indentPending = false;
    }
}
