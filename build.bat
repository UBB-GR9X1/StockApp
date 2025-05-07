@echo off
echo Building StockApp with Entity Framework Core...
dotnet build -clp:ErrorsOnly
if %ERRORLEVEL% EQU 0 (
    echo Build successful!
) else (
    echo Build failed with error code %ERRORLEVEL%
)
pause 