using System.Diagnostics.CodeAnalysis;
using Common.Mod.Common.Core;
using Common.Mod.Common.Utils;
using Common.Mod.Core;
using Common.Mod.Test.Shims;
using JetBrains.Annotations;
using Vintagestory.API.Common;

namespace Common.Mod.Test.Core;

[UsedImplicitly]
[SuppressMessage("ReSharper", "MoveLocalFunctionAfterJumpStatement")]
public class FileSystemTests : IDisposable
{
    private const string TestFileName = "test.txt";
    private const string TestFileContents = "TEST";

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
        Directory.CreateDirectory(Path.Combine(_configPath));
        File.WriteAllText(Path.Combine(_configPath, TestFileName), TestFileContents);

        // Act
        var result = _fileSystem.ConfigFileExists(TestFileName);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ConfigFileExists_MissingFile_ReturnsFalse()
    {
        // Act
        var result = _fileSystem.ConfigFileExists(TestFileName);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void DataFileExists_ExistingFile_ReturnsTrue()
    {
        // Arrange
        Directory.CreateDirectory(Path.Combine(_dataPath));
        File.WriteAllText(Path.Combine(_dataPath, TestFileName), TestFileContents);

        // Act
        var result = _fileSystem.DataFileExists(TestFileName);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DataFileExists_MissingFile_ReturnsFalse()
    {
        // Act
        var result = _fileSystem.DataFileExists(TestFileName);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ReadConfigFile_ExistingFile_ReturnsContents()
    {
        // Arrange
        Directory.CreateDirectory(Path.Combine(_configPath));
        File.WriteAllText(Path.Combine(_configPath, TestFileName), TestFileContents);

        // Act
        var contents = _fileSystem.ReadConfigFile(TestFileName);

        // Assert
        Assert.Equal(TestFileContents, contents);
    }

    [Fact]
    public void ReadConfigFile_MissingFile_Throws()
    {
        // Act
        string Action() => _fileSystem.ReadConfigFile(TestFileName);

        // Assert
        Assert.Throws<FileNotFoundException>(Action);
    }

    [Fact]
    public void ReadDataFile_ExistingFile_ReturnsContents()
    {
        // Arrange
        Directory.CreateDirectory(Path.Combine(_dataPath));
        File.WriteAllText(Path.Combine(_dataPath, TestFileName), TestFileContents);

        // Act
        var contents = _fileSystem.ReadDataFile(TestFileName);

        // Assert
        Assert.Equal(TestFileContents, contents);
    }

    [Fact]
    public void ReadDataFile_MissingFile_Throws()
    {
        // Act
        string Action() => _fileSystem.ReadDataFile(TestFileName);

        // Assert
        Assert.Throws<FileNotFoundException>(Action);
    }

    [Fact]
    public void WriteConfigFile_NewFile_WritesContents()
    {
        // Act
        _fileSystem.WriteConfigFile(TestFileName, TestFileContents);
        var contents = File.ReadAllText(Path.Combine(_configPath, TestFileName));

        // Assert
        Assert.Equal(TestFileContents, contents);
    }

    [Fact]
    public void WriteConfigFile_ExistingFile_WritesContents()
    {
        // Arrange
        Directory.CreateDirectory(Path.Combine(_configPath));
        File.WriteAllText(Path.Combine(_configPath, TestFileName), StringUtils.Random());

        // Act
        _fileSystem.WriteConfigFile(TestFileName, TestFileContents);
        var contents = File.ReadAllText(Path.Combine(_configPath, TestFileName));

        // Assert
        Assert.Equal(TestFileContents, contents);
    }

    [Fact]
    public void WriteDataFile_NewFile_WritesContents()
    {
        // Act
        _fileSystem.WriteDataFile(TestFileName, TestFileContents);
        var contents = File.ReadAllText(Path.Combine(_dataPath, TestFileName));

        // Assert
        Assert.Equal(TestFileContents, contents);
    }

    [Fact]
    public void WriteDataFile_ExistingFile_WritesContents()
    {
        // Arrange
        Directory.CreateDirectory(Path.Combine(_dataPath));
        File.WriteAllText(Path.Combine(_dataPath, TestFileName), StringUtils.Random());

        // Act
        _fileSystem.WriteDataFile(TestFileName, TestFileContents);
        var contents = File.ReadAllText(Path.Combine(_dataPath, TestFileName));

        // Assert
        Assert.Equal(TestFileContents, contents);
    }

    [Fact]
    public void OpenConfigFile_NewFileDefaultParams_CreatesFile()
    {
        // Act
        var stream = _fileSystem.OpenConfigFile(TestFileName);

        // Assert
        Assert.Equal(0, stream.Length);
    }

    [Fact]
    public void OpenConfigFile_ExistingFileDefaultParams_OpensFile()
    {
        // Arrange
        Directory.CreateDirectory(Path.Combine(_configPath));
        File.WriteAllText(Path.Combine(_configPath, TestFileName), TestFileContents);

        // Act
        var stream = _fileSystem.OpenConfigFile(TestFileName);

        // Assert
        Assert.Equal(TestFileContents.Length, stream.Length);
    }

    [Fact]
    public void OpenDataFile_NewFileDefaultParams_CreatesFile()
    {
        // Act
        var stream = _fileSystem.OpenDataFile(TestFileName);

        // Assert
        Assert.Equal(0, stream.Length);
    }

    [Fact]
    public void OpenDataFile_ExistingFileDefaultParams_OpensFile()
    {
        // Arrange
        Directory.CreateDirectory(Path.Combine(_dataPath));
        File.WriteAllText(Path.Combine(_dataPath, TestFileName), TestFileContents);

        // Act
        var stream = _fileSystem.OpenDataFile(TestFileName);

        // Assert
        Assert.Equal(TestFileContents.Length, stream.Length);
    }
}
