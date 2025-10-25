using System.Diagnostics.CodeAnalysis;
using Common.Mod.Common.Core;
using Common.Mod.Common.Utils;
using Common.Mod.Core;
using Common.Mod.Test.Shims;
using DryIoc.ImTools;
using JetBrains.Annotations;
using NSubstitute.Routing.Handlers;
using Vintagestory.API.Common;

namespace Common.Mod.Test.Core;

[UsedImplicitly]
[SuppressMessage("ReSharper", "MoveLocalFunctionAfterJumpStatement")]
public class FileSystemTests : IDisposable
{
    private readonly string _tempPath;
    private readonly string _configPath;
    private readonly string _dataPath;
    private readonly FileSystem _fileSystem;

    public FileSystemTests()
    {
        var modId = StringUtils.Random();
        _tempPath = Directory.CreateTempSubdirectory().FullName;
        _configPath = Path.Combine(_tempPath, "ModConfig", modId);
        _dataPath = Path.Combine(_tempPath, "ModData", modId);

        ICoreAPI api = new CoreApiStub(_tempPath);

        var system = Substitute.For<ISystem>();
        system.ModId().Returns(modId);

        var gamePaths = Substitute.For<IGamePaths>();
        gamePaths.Data().Returns(_tempPath);

        _fileSystem = new FileSystem(api, system, gamePaths);
    }

    public void Dispose()
    {
        Directory.Delete(_tempPath, recursive: true);
    }

    [Fact]
    public void GetConfigDirPath_ReturnsCorrectPath()
    {
        // Act
        var path = _fileSystem.GetConfigDirPath();

        // Assert
        Assert.Equal(_configPath, path);
    }

    [Fact]
    public void GetDataDirPath_ReturnsCorrectPath()
    {
        // Act
        var path = _fileSystem.GetDataDirPath();

        // Assert
        Assert.Equal(_dataPath, path);
    }

    [Fact]
    public void ConfigFileExists_ExistingFile_ReturnsTrue()
    {
        // Arrange
        var fileName = StringUtils.Random();
        var fileContents = StringUtils.Random();

        Directory.CreateDirectory(Path.Combine(_configPath));
        File.WriteAllText(Path.Combine(_configPath, fileName), fileContents);

        // Act
        var result = _fileSystem.ConfigFileExists(fileName);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ConfigFileExists_MissingFile_ReturnsFalse()
    {
        // Arrange
        var fileName = StringUtils.Random();

        // Act
        var result = _fileSystem.ConfigFileExists(fileName);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DataFileExists_ExistingFile_ReturnsTrue()
    {
        // Arrange
        var fileName = StringUtils.Random();
        var fileContents = StringUtils.Random();

        Directory.CreateDirectory(Path.Combine(_dataPath));
        File.WriteAllText(Path.Combine(_dataPath, fileName), fileContents);

        // Act
        var result = _fileSystem.DataFileExists(fileName);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DataFileExists_MissingFile_ReturnsFalse()
    {
        // Arrange
        var fileName = StringUtils.Random();

        // Act
        var result = _fileSystem.DataFileExists(fileName);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ReadConfigFile_ExistingFile_ReturnsContents()
    {
        // Arrange
        var fileName = StringUtils.Random();
        var fileContents = StringUtils.Random();

        Directory.CreateDirectory(Path.Combine(_configPath));
        File.WriteAllText(Path.Combine(_configPath, fileName), fileContents);

        // Act
        var contents = _fileSystem.ReadConfigFile(fileName);

        // Assert
        Assert.Equal(fileContents, contents);
    }

    [Fact]
    public void ReadConfigFile_MissingFile_Throws()
    {
        // Arrange
        var fileName = StringUtils.Random();

        // Act
        var exception = Record.Exception(() => _fileSystem.ReadConfigFile(fileName));

        // Assert
        Assert.IsType<FileNotFoundException>(exception);
    }

    [Fact]
    public void ReadDataFile_ExistingFile_ReturnsContents()
    {
        // Arrange
        var fileName = StringUtils.Random();
        var fileContents = StringUtils.Random();

        Directory.CreateDirectory(Path.Combine(_dataPath));
        File.WriteAllText(Path.Combine(_dataPath, fileName), fileContents);

        // Act
        var contents = _fileSystem.ReadDataFile(fileName);

        // Assert
        Assert.Equal(fileContents, contents);
    }

    [Fact]
    public void ReadDataFile_MissingFile_Throws()
    {
        // Arrange
        var fileName = StringUtils.Random();

        // Act
        var exception = Record.Exception(() => _fileSystem.ReadDataFile(fileName));

        // Assert
        Assert.IsType<FileNotFoundException>(exception);
    }

    [Fact]
    public void WriteConfigFile_NewFile_WritesContents()
    {
        // Arrange
        var fileName = StringUtils.Random();
        var fileContents = StringUtils.Random();

        // Act
        _fileSystem.WriteConfigFile(fileName, fileContents);
        var contents = File.ReadAllText(Path.Combine(_configPath, fileName));

        // Assert
        Assert.Equal(fileContents, contents);
    }

    [Fact]
    public void WriteConfigFile_ExistingFile_WritesContents()
    {
        // Arrange
        var fileName = StringUtils.Random();
        var fileContents = StringUtils.Random();

        Directory.CreateDirectory(Path.Combine(_configPath));
        File.WriteAllText(Path.Combine(_configPath, fileName), StringUtils.Random());

        // Act
        _fileSystem.WriteConfigFile(fileName, fileContents);
        var contents = File.ReadAllText(Path.Combine(_configPath, fileName));

        // Assert
        Assert.Equal(fileContents, contents);
    }

    [Fact]
    public void WriteDataFile_NewFile_WritesContents()
    {
        // Arrange
        var fileName = StringUtils.Random();
        var fileContents = StringUtils.Random();

        // Act
        _fileSystem.WriteDataFile(fileName, fileContents);
        var contents = File.ReadAllText(Path.Combine(_dataPath, fileName));

        // Assert
        Assert.Equal(fileContents, contents);
    }

    [Fact]
    public void WriteDataFile_ExistingFile_WritesContents()
    {
        // Arrange
        var fileName = StringUtils.Random();
        var fileContents = StringUtils.Random();

        Directory.CreateDirectory(Path.Combine(_dataPath));
        File.WriteAllText(Path.Combine(_dataPath, fileName), StringUtils.Random());

        // Act
        _fileSystem.WriteDataFile(fileName, fileContents);
        var contents = File.ReadAllText(Path.Combine(_dataPath, fileName));

        // Assert
        Assert.Equal(fileContents, contents);
    }

    [Fact]
    public void OpenConfigFile_NewFileDefaultParams_CreatesFile()
    {
        // Arrange
        var fileName = StringUtils.Random();

        // Act
        var stream = _fileSystem.OpenConfigFile(fileName);

        // Assert
        Assert.Equal(0, stream.Length);
    }

    [Fact]
    public void OpenConfigFile_ExistingFileDefaultParams_OpensFile()
    {
        // Arrange
        var fileName = StringUtils.Random();
        var fileContents = StringUtils.Random();

        Directory.CreateDirectory(Path.Combine(_configPath));
        File.WriteAllText(Path.Combine(_configPath, fileName), fileContents);

        // Act
        var stream = _fileSystem.OpenConfigFile(fileName);

        // Assert
        Assert.Equal(fileContents.Length, stream.Length);
    }

    [Fact]
    public void OpenDataFile_NewFileDefaultParams_CreatesFile()
    {
        // Arrange
        var fileName = StringUtils.Random();

        // Act
        var stream = _fileSystem.OpenDataFile(fileName);

        // Assert
        Assert.Equal(0, stream.Length);
    }

    [Fact]
    public void OpenDataFile_ExistingFileDefaultParams_OpensFile()
    {
        // Arrange
        var fileName = StringUtils.Random();
        var fileContents = StringUtils.Random();

        Directory.CreateDirectory(Path.Combine(_dataPath));
        File.WriteAllText(Path.Combine(_dataPath, fileName), fileContents);

        // Act
        var stream = _fileSystem.OpenDataFile(fileName);

        // Assert
        Assert.Equal(fileContents.Length, stream.Length);
    }
}
