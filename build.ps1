$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

Push-Location Common.Mod.Cake
dotnet run --project Common.Mod.Cake.csproj -- $args
Pop-Location
