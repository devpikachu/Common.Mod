namespace Common.Mod.Common.Core;

public interface ILogger
{
    public ILogger Named(string name);

    public void AddSink<TSink>(string key, TSink sink) where TSink : ILogSink;
    public void RemoveSink(string key);

    public void Critical(string format, params object[] args);
    public void Critical(Exception ex, string format, params object[] args);
    public void Critical(Exception ex);

    public void Error(string format, params object[] args);
    public void Error(Exception ex, string format, params object[] args);
    public void Error(Exception ex);

    public void Warning(string format, params object[] args);
    public void Info(string format, params object[] args);
    public void Debug(string format, params object[] args);
    public void Verbose(string format, params object[] args);

    public void Log(LogSeverity severity, string format, params object[] args);
    public void Log(LogSeverity severity, Exception ex, string format, params object[] args);
    public void Log(LogSeverity severity, Exception ex);
}
