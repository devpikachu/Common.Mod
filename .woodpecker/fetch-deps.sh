set -e

curl -L https://files.omni.ms/protected/woodpecker/vintagestory/1.21.3/protobuf-net.dll -o "vendor/protobuf-net.dll" -u "$1:$2"
curl -L https://files.omni.ms/protected/woodpecker/vintagestory/1.21.3/VintagestoryAPI.dll -o "vendor/VintagestoryAPI.dll" -u "$1:$2"
