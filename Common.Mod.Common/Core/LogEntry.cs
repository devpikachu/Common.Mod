namespace Common.Mod.Common.Core;

public record LogEntry
{
    public DateTime Timestamp { get; init; }
    public LogSeverity Severity { get; init; }
    public string? Emitter { get; init; }
    public string Message { get; init; } = string.Empty;
}
