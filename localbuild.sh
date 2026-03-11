#!/bin/bash

set -e

bash build.sh --target Package --general-project Common.Mod --general-skipSubstitution true
bash build.sh --target Package --general-project Common.Mod.Common --general-skipSubstitution true
bash build.sh --target Package --general-project Common.Mod.Generator --general-skipSubstitution true

mkdir -p out/Common.Mod/12.34.56
mkdir -p out/Common.Mod.Common/12.34.56
mkdir -p out/Common.Mod.Generator/12.34.56

cp out/Common.Mod.12.34.56.nupkg out/Common.Mod/12.34.56/Common.Mod.12.34.56.nupkg
cp out/Common.Mod.Common.12.34.56.nupkg out/Common.Mod.Common/12.34.56/Common.Mod.Common.12.34.56.nupkg
cp out/Common.Mod.Generator.12.34.56.nupkg out/Common.Mod.Generator/12.34.56/Common.Mod.Generator.12.34.56.nupkg

dotnet nuget locals all --clear
dotnet restore
