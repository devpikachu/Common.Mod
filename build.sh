pushd Common.Build.Cake || exit 1
dotnet run --project Common.Build.Cake.csproj -- "$@"
ExitCode=$?
popd || exit 1
exit $ExitCode
