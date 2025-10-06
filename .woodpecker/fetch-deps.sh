set -e

mkdir -p "${VINTAGE_STORY}/Lib"

curl -L https://files.omni.ms/protected/woodpecker/vintagestory/1.21.3/protobuf-net.dll -o "${VINTAGE_STORY}/Lib/protobuf-net.dll" -u "$1:$2"
curl -L https://files.omni.ms/protected/woodpecker/vintagestory/1.21.3/VintagestoryAPI.dll -o "${VINTAGE_STORY}/VintagestoryAPI.dll" -u "$1:$2"
