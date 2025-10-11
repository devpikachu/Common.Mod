using Common.Mod.Common.Core;
using ICoreLogger = Vintagestory.API.Common.ILogger;

namespace Common.Mod.Core;

public class ConsoleLogSink : ILogSink
{
    public const string Key = "CONSOLE";

    private readonly string _modId;
    private readonly ICoreLogger _logger;

    public ConsoleLogSink(string modId, ICoreLogger logger)
    {
        _modId = modId;
        _logger = logger;
    }

    public void Ingest(LogEntry entry)
    {
        var message = string.IsNullOrWhiteSpace(entry.Emitter)
            ? entry.Message
            : string.Format("[{0}] [{1}] {2}", _modId, entry.Emitter, entry.Message);

        switch (entry.Severity)
        {
            case LogSeverity.Verbose:
                _logger.VerboseDebug(message);
                break;

            case LogSeverity.Debug:
                _logger.Debug(message);
                break;

            case LogSeverity.Info:
                _logger.Event(message);
                break;

            case LogSeverity.Warning:
                _logger.Warning(message);
                break;

            case LogSeverity.Error:
                _logger.Error(message);
                break;

            case LogSeverity.Critical:
                _logger.Fatal(message);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
