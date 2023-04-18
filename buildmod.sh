#!/bin/bash

if [ $# -eq 0 ]; then
  echo "Error: input a version as first argument"
  exit 1
fi

mkdir -p build
mkdir tmp
cp -r resources src obj .vscode modinfo.json VintageAuth.csproj LICENSE lgplsneed_small.png modicon.png tmp
cd tmp
rm -rf obj/Debug
sed -i "s/%MODVERSION%/$1/g" modinfo.json

msbuild

mv bin/Debug/net461/VintageAuth.dll .
zip -r VintageAuth_$1.zip VintageAuth.dll resources LICENSE lgplsneed_small.png modinfo.json modicon.png
cd ..
mv tmp/VintageAuth_$1.zip build
rm -rf tmp