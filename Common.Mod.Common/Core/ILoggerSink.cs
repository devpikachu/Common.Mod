namespace Common.Mod.Common.Core;

public interface ILoggerSink
{
    public void Ingest(LogEntry entry);
}
