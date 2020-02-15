del /Q Release

dotnet publish OpenHab.UniFiProxy.csproj ^
    /property:GenerateFullPaths=true ^
    /consoleloggerparameters:NoSummary ^
    -c Release ^
    -r win-x64 ^
    --self-contained false ^
    -o .\bin\Release\netcoreapp3.1\win-x64\publish\

7z a .\Release\OpenHabUnifiProxy-win-x64.zip .\bin\Release\netcoreapp3.1\win-x64\publish\*

dotnet publish OpenHab.UniFiProxy.csproj ^
    /property:GenerateFullPaths=true ^
    /consoleloggerparameters:NoSummary ^
    -c Release ^
    -r win-x64 ^
    --self-contained true ^
    -o .\bin\Release\netcoreapp3.1\win-x64-standalone\publish\

7z a .\Release\OpenHabUnifiProxy-win-x64-standalone.zip .\bin\Release\netcoreapp3.1\win-x64-standalone\publish\*

dotnet publish OpenHab.UniFiProxy.csproj ^
    /property:GenerateFullPaths=true ^
    /consoleloggerparameters:NoSummary ^
    -c Release ^
    -r linux-x64 ^
    --self-contained true ^
    -o .\bin\Release\netcoreapp3.1\linux-x64-standalone\publish\

7z a .\Release\OpenHabUnifiProxy-linux-x64-standalone.zip .\bin\Release\netcoreapp3.1\linux-x64-standalone\publish\*

dotnet publish OpenHab.UniFiProxy.csproj ^
    /property:GenerateFullPaths=true ^
    /consoleloggerparameters:NoSummary ^
    -c Release ^
    -r linux-x64 ^
    --self-contained false ^
    -o .\bin\Release\netcoreapp3.1\linux-x64\publish\

7z a .\Release\OpenHabUnifiProxy-linux-x64.zip .\bin\Release\netcoreapp3.1\linux-x64\publish\*
