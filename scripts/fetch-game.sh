set -e

if [[ ! -d vendor ]]; then
    mkdir vendor
fi

curl -L https://files.omni.ms/protected/woodpecker/vintagestory/1.21.3/protobuf-net.dll?raw -o "vendor/protobuf-net.dll" -u "$1:$2"
curl -L https://files.omni.ms/protected/woodpecker/vintagestory/1.21.3/VintagestoryAPI.dll?raw -o "vendor/VintagestoryAPI.dll" -u "$1:$2"
