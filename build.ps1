Push-Location Common.Build.Cake
dotnet run --project Common.Build.Cake.csproj -- $args
$ExitCode = $LASTEXITCODE
Pop-Location
exit $ExitCode
