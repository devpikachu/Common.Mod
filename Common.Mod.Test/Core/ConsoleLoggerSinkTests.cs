using Common.Mod.Common.Core;
using Common.Mod.Common.Utils;
using Common.Mod.Core;
using JetBrains.Annotations;
using Vintagestory.API.Common;
using ICoreLogger = Vintagestory.API.Common.ILogger;

namespace Common.Mod.Test.Core;

[UsedImplicitly]
public class ConsoleLoggerSinkTests
{
    private readonly string _modId = StringUtils.Random();
    private readonly ICoreLogger _coreLogger = Substitute.For<ICoreLogger>();

    public static TheoryData<LogSeverity, EnumAppSide> Data = new MatrixTheoryData<LogSeverity, EnumAppSide>(
        [LogSeverity.Verbose, LogSeverity.Debug, LogSeverity.Info, LogSeverity.Warning, LogSeverity.Error, LogSeverity.Critical],
        [EnumAppSide.Client, EnumAppSide.Server]
    );

    [Theory]
    [MemberData(nameof(Data))]
    public void Ingest_WithoutEmitter_LogsCorrectly(LogSeverity severity, EnumAppSide appSide)
    {
        // Arrange
        var loggerSink = new ConsoleLoggerSink(_modId, appSide, _coreLogger);

        var timestamp = DateTime.UtcNow;
        var message = StringUtils.Random();

        var logEntry = new LogEntry
        {
            Timestamp = timestamp,
            Severity = severity,
            Emitter = null,
            Message = message
        };

        var expectedMessage = string.Format("[{0}] [{1}] {2}", _modId, appSide, message);

        // Act
        loggerSink.Ingest(logEntry);

        // Assert
        switch (severity)
        {
            case LogSeverity.Verbose:
                _coreLogger.Received(1).VerboseDebug(expectedMessage);
                break;

            case LogSeverity.Debug:
                _coreLogger.Received(1).Debug(expectedMessage);
                break;

            case LogSeverity.Info:
                _coreLogger.Received(1).Event(expectedMessage);
                break;

            case LogSeverity.Warning:
                _coreLogger.Received(1).Warning(expectedMessage);
                break;

            case LogSeverity.Error:
                _coreLogger.Received(1).Error(expectedMessage);
                break;

            case LogSeverity.Critical:
                _coreLogger.Received(1).Fatal(expectedMessage);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
        }
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Ingest_WithEmitter_LogsCorrectly(LogSeverity severity, EnumAppSide appSide)
    {
        // Arrange
        var loggerSink = new ConsoleLoggerSink(_modId, appSide, _coreLogger);

        var timestamp = DateTime.UtcNow;
        var emitter = StringUtils.Random();
        var message = StringUtils.Random();

        var logEntry = new LogEntry
        {
            Timestamp = timestamp,
            Severity = severity,
            Emitter = emitter,
            Message = message
        };

        var expectedMessage = string.Format("[{0}] [{1}] [{2}] {3}", _modId, appSide, emitter, message);

        // Act
        loggerSink.Ingest(logEntry);

        // Assert
        switch (severity)
        {
            case LogSeverity.Verbose:
                _coreLogger.Received(1).VerboseDebug(expectedMessage);
                break;

            case LogSeverity.Debug:
                _coreLogger.Received(1).Debug(expectedMessage);
                break;

            case LogSeverity.Info:
                _coreLogger.Received(1).Event(expectedMessage);
                break;

            case LogSeverity.Warning:
                _coreLogger.Received(1).Warning(expectedMessage);
                break;

            case LogSeverity.Error:
                _coreLogger.Received(1).Error(expectedMessage);
                break;

            case LogSeverity.Critical:
                _coreLogger.Received(1).Fatal(expectedMessage);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
        }
    }
}
