$ImGuiVersion = "1.1.14"
$ConfigLibVersion = "1.10.5"

# Create vendor directory
if (-Not (Test-Path vendor))
{
    New-Item -ItemType Directory -Path vendor
}

# Create runtime mods directory
if (-Not (Test-Path Common.Mod.Example/run/Mods))
{
    New-Item -ItemType Directory -Path Common.Mod.Example/run/Mods
}

# Vintage Story files
Copy-Item -Path "$Env:VINTAGE_STORY/VintagestoryAPI.dll" -Destination vendor/VintagestoryAPI.dll
Copy-Item -Path "$Env:VINTAGE_STORY/Lib/protobuf-net.dll" -Destination vendor/protobuf-net.dll

# 3rd party mods
## ImGui
if (-Not (Test-Path "vendor/vsimgui_$ImGuiVersion.zip"))
{
    Invoke-WebRequest "https://mods.vintagestory.at/download/58527/vsimgui_$ImGuiVersion.zip" -OutFile "vendor/vsimgui_$ImGuiVersion.zip"
    Copy-Item -Path "vendor/vsimgui_$ImGuiVersion.zip" -Destination "Common.Mod.Example/run/Mods/vsimgui_$ImGuiVersion.zip"

    try
    {
        $tempDir = Join-Path ([IO.Path]::GetTempPath()) ([System.Guid]::NewGuid().ToString('n'))
        Expand-Archive -Path "vendor/vsimgui_$ImGuiVersion.zip" -DestinationPath $tempDir

        Copy-Item -Path "$tempDir/ImGui.NET.dll" -Destination vendor/ImGui.NET.dll
        Copy-Item -Path "$tempDir/VSImGui.dll" -Destination vendor/VSImGui.dll
    }
    finally
    {
        if (Test-Path $tempDir)
        {
            Remove-Item $tempDir -Recurse -Force -EA Continue
        }
    }
}

## ConfigLib
if (-Not (Test-Path "vendor/configlib_$ConfigLibVersion.zip"))
{
    Invoke-WebRequest "https://mods.vintagestory.at/download/57734/configlib_$ConfigLibVersion.zip" -OutFile "vendor/configlib_$ConfigLibVersion.zip"
    Copy-Item -Path "vendor/configlib_$ConfigLibVersion.zip" -Destination "Common.Mod.Example/run/Mods/configlib_$ConfigLibVersion.zip"

    try
    {
        $tempDir = Join-Path ([IO.Path]::GetTempPath()) ([System.Guid]::NewGuid().ToString('n'))
        Expand-Archive -Path "vendor/configlib_$ConfigLibVersion.zip" -DestinationPath $tempDir

        Copy-Item -Path "$tempDir/configlib.dll" -Destination vendor/configlib.dll
    }
    finally
    {
        if (Test-Path $tempDir)
        {
            Remove-Item $tempDir -Recurse -Force -EA Continue
        }
    }
}
