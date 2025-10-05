Push-Location Common.Mod.Cake
dotnet run --project Common.Mod.Cake.csproj -- $args
$ExitCode = $LASTEXITCODE
Pop-Location
exit $ExitCode
