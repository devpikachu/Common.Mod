namespace Common.Mod.Common.Core;

public interface ILogSink
{
    public void Ingest(LogEntry entry);
}
