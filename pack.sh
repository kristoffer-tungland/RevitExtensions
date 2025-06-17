#!/usr/bin/env bash
set -e
set -x


OUTPUT=$(pwd)/nupkgs
mkdir -p "$OUTPUT"

for year in 2019 2020 2021 2022 2023 2024 2025 2026; do
  if [ "$year" -ge 2025 ]; then
    tf=net8.0
  else
    tf=net48
  fi

  case $year in
    2019) api_ver=2019.0.1 ;;
    2020) api_ver=2020.0.1 ;;
    2021) api_ver=2021.1.9 ;;
    2022) api_ver=2022.1.0 ;;
    2023) api_ver=2023.0.0 ;;
    2024) api_ver=2024.2.0 ;;
    2025) api_ver=2025.0.0 ;;
    2026) api_ver=2026.0.0 ;;
  esac

  # build define constants
  defines="REVIT${year}"
  for y in {2019..2026}; do
    if [ "$year" -ge "$y" ]; then
      defines+=";REVIT${y}_OR_ABOVE"
    fi
    if [ "$year" -le "$y" ]; then
      defines+=";REVIT${y}_OR_LESS"
    fi
  done

  encoded_defs=${defines//;/\%3B}
  dotnet msbuild RevitExtensions/RevitExtensions.csproj -t:pack \
    -p:Configuration=Release \
    -p:PackageVersion=1.0.0-revit${year} \
    -p:PackageId=RevitExtensions${year} \
    -p:PackageOutputPath="$OUTPUT" \
    -p:TargetFramework=${tf} \
    -p:DefineConstants=${encoded_defs} \
    -p:RevitApiPackageVersion=${api_ver} \
    -p:UseRevitApiStubs=false
done
