msbuild
cp bin/Debug/net461/VintageAuth.dll .
zip -r VintageAuth_$1.zip VintageAuth.dll resources LICENSE lgplsneed_small.png modinfo.json modicon.png
rm VintageAuth.dll