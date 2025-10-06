set -e

pushd Common.Mod.Cake
dotnet run --project Common.Mod.Cake.csproj -- "$@"
popd

