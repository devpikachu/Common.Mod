using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Common.Mod.Common.Core;
using Common.Mod.Common.Utils;
using Common.Mod.Core;
using JetBrains.Annotations;

namespace Common.Mod.Test.Core;

[UsedImplicitly]
[SuppressMessage("ReSharper", "MoveLocalFunctionAfterJumpStatement")]
public class LoggerTests
{
    private const string DefaultSinkKey = "DEFAULT";

    private readonly ILoggerSink _loggerSink;
    private readonly Logger _logger;

    public static TheoryData<LogSeverity, bool> Data = new MatrixTheoryData<LogSeverity, bool>(
        [LogSeverity.Verbose, LogSeverity.Debug, LogSeverity.Info, LogSeverity.Warning, LogSeverity.Error, LogSeverity.Critical],
        [false, true]
    );

    public LoggerTests()
    {
        _loggerSink = Substitute.For<ILoggerSink>();

        _logger = new Logger();
        _logger.AddSink(DefaultSinkKey, _loggerSink);
    }

    [Fact]
    public void AddSink_NewSink_DoesNotThrow()
    {
        // Arrange
        var newSinkKey = StringUtils.Random();
        var newSink = Substitute.For<ILoggerSink>();

        // Act
        var exception = Record.Exception(() => _logger.AddSink(newSinkKey, newSink));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void AddSink_ExistingSink_Throws()
    {
        // Arrange
        var newSink = Substitute.For<ILoggerSink>();

        // Act
        var exception = Record.Exception(() => _logger.AddSink(DefaultSinkKey, newSink));

        // Assert
        Assert.IsType<ArgumentException>(exception);
    }

    [Fact]
    public void RemoveSink_ExistingSink_DoesNotThrow()
    {
        // Arrange
        var newSinkKey = StringUtils.Random();
        var newSink = Substitute.For<ILoggerSink>();

        _logger.AddSink(newSinkKey, newSink);

        // Act
        var exception = Record.Exception(() => _logger.RemoveSink(newSinkKey));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void RemoveSink_MissingSink_DoesNotThrow()
    {
        // Arrange
        var newSinkKey = StringUtils.Random();

        // Act
        var exception = Record.Exception(() => _logger.RemoveSink(newSinkKey));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void Named_ReturnsNamedInstance()
    {
        // Arrange
        var name = StringUtils.Random();

        // Act
        var instance = _logger.Named(name);

        // Assert
        var nameField = typeof(Logger).GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.Equal(name, nameField!.GetValue(instance));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Critical_Format_CreatesCorrectEntry(bool named)
    {
        // Arrange
        var name = StringUtils.Random();
        var logger = named ? _logger.Named(name) : _logger;
        var message = StringUtils.Random();

        // Act
        logger.Critical(message);

        // Assert
        if (named)
        {
            _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Emitter == name));
        }

        _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Severity == LogSeverity.Critical && entry.Message == message));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Critical_ExceptionFormat_CreatesCorrectEntry(bool named)
    {
        // Arrange
        var name = StringUtils.Random();
        var logger = named ? _logger.Named(name) : _logger;
        var exceptionMessage = StringUtils.Random();
        var exception = new Exception(exceptionMessage);
        var message = StringUtils.Random();

        // Act
        logger.Critical(exception, message);

        // Assert
        if (named)
        {
            _loggerSink.Received(2).Ingest(Arg.Is<LogEntry>(entry => entry.Emitter == name));
        }

        _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Severity == LogSeverity.Critical && entry.Message == message));
        _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Severity == LogSeverity.Critical && entry.Message == exceptionMessage));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Critical_Exception_CreatesCorrectEntry(bool named)
    {
        // Arrange
        var name = StringUtils.Random();
        var logger = named ? _logger.Named(name) : _logger;
        var exceptionMessage = StringUtils.Random();
        var exception = new Exception(exceptionMessage);

        // Act
        logger.Critical(exception);

        // Assert
        if (named)
        {
            _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Emitter == name));
        }

        _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Severity == LogSeverity.Critical && entry.Message == exceptionMessage));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Error_Format_CreatesCorrectEntry(bool named)
    {
        // Arrange
        var name = StringUtils.Random();
        var logger = named ? _logger.Named(name) : _logger;
        var message = StringUtils.Random();

        // Act
        logger.Error(message);

        // Assert
        if (named)
        {
            _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Emitter == name));
        }

        _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Severity == LogSeverity.Error && entry.Message == message));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Error_ExceptionFormat_CreatesCorrectEntry(bool named)
    {
        // Arrange
        var name = StringUtils.Random();
        var logger = named ? _logger.Named(name) : _logger;
        var exceptionMessage = StringUtils.Random();
        var exception = new Exception(exceptionMessage);
        var message = StringUtils.Random();

        // Act
        logger.Error(exception, message);

        // Assert
        if (named)
        {
            _loggerSink.Received(2).Ingest(Arg.Is<LogEntry>(entry => entry.Emitter == name));
        }

        _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Severity == LogSeverity.Error && entry.Message == message));
        _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Severity == LogSeverity.Error && entry.Message == exceptionMessage));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Error_Exception_CreatesCorrectEntry(bool named)
    {
        // Arrange
        var name = StringUtils.Random();
        var logger = named ? _logger.Named(name) : _logger;
        var exceptionMessage = StringUtils.Random();
        var exception = new Exception(exceptionMessage);

        // Act
        logger.Error(exception);

        // Assert
        if (named)
        {
            _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Emitter == name));
        }

        _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Severity == LogSeverity.Error && entry.Message == exceptionMessage));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Warning_CreatesCorrectEntry(bool named)
    {
        // Arrange
        var name = StringUtils.Random();
        var logger = named ? _logger.Named(name) : _logger;
        var message = StringUtils.Random();

        // Act
        logger.Warning(message);

        // Assert
        if (named)
        {
            _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Emitter == name));
        }

        _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Severity == LogSeverity.Warning && entry.Message == message));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Info_CreatesCorrectEntry(bool named)
    {
        // Arrange
        var name = StringUtils.Random();
        var logger = named ? _logger.Named(name) : _logger;
        var message = StringUtils.Random();

        // Act
        logger.Info(message);

        // Assert
        if (named)
        {
            _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Emitter == name));
        }

        _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Severity == LogSeverity.Info && entry.Message == message));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Debug_CreatesCorrectEntry(bool named)
    {
        // Arrange
        var name = StringUtils.Random();
        var logger = named ? _logger.Named(name) : _logger;
        var message = StringUtils.Random();

        // Act
        logger.Debug(message);

        // Assert
        if (named)
        {
            _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Emitter == name));
        }

        _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Severity == LogSeverity.Debug && entry.Message == message));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Verbose_CreatesCorrectEntry(bool named)
    {
        // Arrange
        var name = StringUtils.Random();
        var logger = named ? _logger.Named(name) : _logger;
        var message = StringUtils.Random();

        // Act
        logger.Verbose(message);

        // Assert
        if (named)
        {
            _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Emitter == name));
        }

        _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Severity == LogSeverity.Verbose && entry.Message == message));
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Log_Format_CreatesCorrectEntry(LogSeverity severity, bool named)
    {
        // Arrange
        var name = StringUtils.Random();
        var logger = named ? _logger.Named(name) : _logger;
        var message = StringUtils.Random();

        // Act
        logger.Log(severity, message);

        // Assert
        if (named)
        {
            _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Emitter == name));
        }

        _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Severity == severity && entry.Message == message));
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Log_ExceptionFormat_CreatesCorrectEntry(LogSeverity severity, bool named)
    {
        // Arrange
        var name = StringUtils.Random();
        var logger = named ? _logger.Named(name) : _logger;
        var exceptionMessage = StringUtils.Random();
        var exception = new Exception(exceptionMessage);
        var message = StringUtils.Random();

        // Act
        logger.Log(severity, exception, message);

        // Assert
        if (named)
        {
            _loggerSink.Received(2).Ingest(Arg.Is<LogEntry>(entry => entry.Emitter == name));
        }

        _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Severity == severity && entry.Message == message));
        _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Severity == severity && entry.Message == exceptionMessage));
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Log_Exception_CreatesCorrectEntry(LogSeverity severity, bool named)
    {
        // Arrange
        var name = StringUtils.Random();
        var logger = named ? _logger.Named(name) : _logger;
        var exceptionMessage = StringUtils.Random();
        var exception = new Exception(exceptionMessage);

        // Act
        logger.Log(severity, exception);

        // Assert
        if (named)
        {
            _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Emitter == name));
        }

        _loggerSink.Received(1).Ingest(Arg.Is<LogEntry>(entry => entry.Severity == severity && entry.Message == exceptionMessage));
    }
}
