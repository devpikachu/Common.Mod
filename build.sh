pushd Common.Mod.Cake || exit 1
dotnet run --project Common.Mod.Cake.csproj -- "$@"
ExitCode=$?
popd || exit 1
exit $ExitCode
