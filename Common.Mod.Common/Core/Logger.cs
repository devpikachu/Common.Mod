namespace Common.Mod.Common.Core;

public class Logger : ILogger
{
    private readonly string? _name;
    private readonly Dictionary<string, ILogSink> _sinks;

    public Logger()
    {
        _name = null;
        _sinks = new Dictionary<string, ILogSink>();
    }

    private Logger(string name, Dictionary<string, ILogSink> sinks)
    {
        _name = name;
        _sinks = sinks;
    }

    public ILogger Named(string name) => new Logger(name, _sinks);

    public void AddSink<TSink>(string key, TSink sink) where TSink : ILogSink => _sinks.Add(key, sink);
    public void RemoveSink(string key) => _sinks.Remove(key);

    public void Critical(string format, params object[] args) => Log(LogSeverity.Critical, format, args);
    public void Critical(Exception ex, string format, params object[] args) => Log(LogSeverity.Critical, ex, format, args);
    public void Critical(Exception ex) => Log(LogSeverity.Critical, ex);

    public void Error(string format, params object[] args) => Log(LogSeverity.Error, format, args);
    public void Error(Exception ex, string format, params object[] args) => Log(LogSeverity.Error, ex, format, args);
    public void Error(Exception ex) => Log(LogSeverity.Error, ex);

    public void Warning(string format, params object[] args) => Log(LogSeverity.Warning, format, args);
    public void Info(string format, params object[] args) => Log(LogSeverity.Info, format, args);
    public void Debug(string format, params object[] args) => Log(LogSeverity.Debug, format, args);
    public void Verbose(string format, params object[] args) => Log(LogSeverity.Verbose, format, args);

    public void Log(LogSeverity severity, string format, params object[] args)
    {
        var entry = new LogEntry
        {
            Timestamp = DateTime.UtcNow,
            Severity = severity,
            Emitter = _name,
            Message = string.Format(format, args)
        };

        foreach (var sink in _sinks.Values)
        {
            sink.Ingest(entry);
        }
    }

    public void Log(LogSeverity severity, Exception ex, string format, params object[] args)
    {
        Log(severity, format, args);
        Log(severity, ex);
    }

    public void Log(LogSeverity severity, Exception ex)
    {
        if (string.IsNullOrWhiteSpace(ex.StackTrace))
        {
            Log(severity, ex.Message);
            return;
        }

        Log(severity, "{0}\n{1}", ex.Message, ex.StackTrace);
    }
}
