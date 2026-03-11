#!/bin/bash

set -e

# vsChannel="stable"
vsChannel="unstable"
vsVersion="1.22.0-rc.2"
vsImGuiId="76347"
vsImGuiVersion="1.2.0"
configLibId="76348"
configLibVersion="1.11.0"
autoConfigLibId="79793"
autoConfigLibVersion="2.0.10"

directories=(
    "Common.Mod.Example/run/Mods"
    "vendor"
)

download_and_extract_mod() {
    # Download 3rd party mod
    if [[ ! -f "vendor/$1.zip" ]]; then
        curl -L "https://mods.vintagestory.at/download/$2" -o "vendor/$1.zip"
    fi

    # Copy 3rd party mod to example runtime
    if [[ ! -f "Common.Mod.Example/run/Mods/$1.zip" ]]; then
        cp "vendor/$1.zip" "Common.Mod.Example/run/Mods/$1.zip"
    fi

    # Extract 3rd party mod
    if [[ ! -d "vendor/$1" ]]; then
        mkdir "vendor/$1"
        unzip "vendor/$1.zip" -d "vendor/$1"
    fi
}

# Delete directories
if [[ "$1" == "force" ]]; then
    for directory in "${directories[@]}"; do
        if [[ -d "$directory" ]]; then
            rm -rf "$directory"
        fi
    done
fi

# Create directories
for directory in "${directories[@]}"; do
    if [[ ! -d "$directory" ]]; then
        mkdir -p "$directory"
    fi
done

# Download Vintage Story
if [[ ! -f "vendor/vs.tar.gz" ]]; then
    curl -L "https://cdn.vintagestory.at/gamefiles/${vsChannel}/vs_client_linux-x64_${vsVersion}.tar.gz" -o "vendor/vs.tar.gz"
fi

# Extract Vintage Story
if [[ ! -d "vendor/vs" ]]; then
    mkdir vendor/vs
    tar xfz vendor/vs.tar.gz --strip-components=1 -C vendor/vs
fi

# Download, copy and extract 3rd party mods
download_and_extract_mod "vsimgui" "${vsImGuiId}/vsimgui_${vsImGuiVersion}.zip"
download_and_extract_mod "configlib" "${configLibId}/configlib_${configLibVersion}.zip"
download_and_extract_mod "autoconfiglib" "${autoConfigLibId}/autoconfiglib_${autoConfigLibVersion}.zip"
