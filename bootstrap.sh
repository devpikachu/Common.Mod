set -e

ImGuiVersion="1.1.14"
ConfigLibVersion="1.10.5"

# Create vendor directory
if [[ ! -d vendor ]]; then
    mkdir vendor
fi

# Create runtime mods directory
if [[ ! -d Common.Mod.Example/run/Mods ]]; then
    mkdir -p Common.Mod.Example/run/Mods
fi

# Vintage Story files
cp "${VINTAGE_STORY}/VintagestoryAPI.dll" vendor/VintagestoryAPI.dll
cp "${VINTAGE_STORY}/Lib/protobuf-net.dll" vendor/protobuf-net.dll

# 3rd party mods
## ImGui
if [[ ! -f "vendor/vsimgui_${ImGuiVersion}" ]]; then
    curl -L "https://mods.vintagestory.at/download/58527/vsimgui_${ImGuiVersion}.zip" -o "vendor/vsimgui_${ImGuiVersion}.zip"
    cp "vendor/vsimgui_${ImGuiVersion}.zip" "Common.Mod.Example/run/Mods/vsimgui_${ImGuiVersion}.zip"

    tempDir=$(mktemp)
    unzip "vendor/vsimgui_${ImGuiVersion}.zip" -d "$tempDir"

    cp "${tempDir}/ImGui.NET.dll" "vendor/ImGui.NET.dll"
    cp "${tempDir}/VSImGui.dll" "vendor/VSImGui.dll"

    rm -rf "$tempDir"
fi

## ConfigLib
if [[ ! -f "vendor/configlib_${ConfigLibVersion}" ]]; then
    curl -L "https://mods.vintagestory.at/download/57734/configlib_${ConfigLibVersion}.zip" -o "vendor/configlib_${ConfigLibVersion}.zip"
    cp "vendor/configlib_${ConfigLibVersion}.zip" "Common.Mod.Example/run/Mods/configlib_${ConfigLibVersion}.zip"

    tempDir=$(mktemp)
    unzip "vendor/configlib_${ConfigLibVersion}.zip" -d "$tempDir"

    cp "${tempDir}/configlib.dll" "vendor/configlib.dll"

    rm -rf "$tempDir"
fi
