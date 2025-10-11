using JetBrains.Annotations;

namespace Common.Mod.Common.Core;

public interface ILogger
{
    public ILogger Named(string name);

    public void AddSink<TSink>(string key, TSink sink) where TSink : ILogSink;
    public void RemoveSink(string key);

    [StringFormatMethod("format")]
    public void Critical(string format, params object[] args);

    [StringFormatMethod("format")]
    public void Critical(Exception ex, string format, params object[] args);

    public void Critical(Exception ex);

    [StringFormatMethod("format")]
    public void Error(string format, params object[] args);

    [StringFormatMethod("format")]
    public void Error(Exception ex, string format, params object[] args);

    public void Error(Exception ex);

    [StringFormatMethod("format")]
    public void Warning(string format, params object[] args);

    [StringFormatMethod("format")]
    public void Info(string format, params object[] args);

    [StringFormatMethod("format")]
    public void Debug(string format, params object[] args);

    [StringFormatMethod("format")]
    public void Verbose(string format, params object[] args);

    [StringFormatMethod("format")]
    public void Log(LogSeverity severity, string format, params object[] args);

    [StringFormatMethod("format")]
    public void Log(LogSeverity severity, Exception ex, string format, params object[] args);

    public void Log(LogSeverity severity, Exception ex);
}
