using Common.Mod.Common.Core;
using Vintagestory.API.Common;
using ICoreLogger = Vintagestory.API.Common.ILogger;

namespace Common.Mod.Core;

public class ConsoleLoggerSink : ILoggerSink
{
    public const string Key = "CONSOLE";

    private readonly string _modId;
    private readonly string _side;
    private readonly ICoreLogger _logger;

    public ConsoleLoggerSink(string modId, EnumAppSide side, ICoreLogger logger)
    {
        _modId = modId;
        _side = side is EnumAppSide.Server ? "Server" : "Client";
        _logger = logger;
    }

    public void Ingest(LogEntry entry)
    {
        var message = string.IsNullOrWhiteSpace(entry.Emitter)
            ? entry.Message
            : string.Format("[{0}] [{1}] [{2}] {3}", _modId, _side, entry.Emitter, entry.Message);

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
