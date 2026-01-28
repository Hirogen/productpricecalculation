@echo off
echo ========================================
echo Product Price Calculator - Build Script
echo ========================================
echo.

echo Checking for .NET SDK...
dotnet --version > nul 2>&1
if errorlevel 1 (
    echo ERROR: .NET SDK is not installed!
    echo Please download from: https://dotnet.microsoft.com/download
    echo.
    pause
    exit /b 1
)

echo .NET SDK found!
echo.

echo Building standalone executable...
echo This may take a few moments...
echo.

dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true

if errorlevel 1 (
    echo.
    echo ERROR: Build failed!
    pause
    exit /b 1
)

echo.
echo ========================================
echo BUILD SUCCESSFUL!
echo ========================================
echo.
echo Your standalone executable is ready at:
echo bin\Release\net10.0-windows\win-x64\publish\ProductPriceCalculator.exe
echo.
echo This file can be:
echo - Run immediately (no installation needed)
echo - Copied to any Windows PC
echo - Shared with others
echo.
echo Would you like to run the application now? (Y/N)
set /p choice=

if /i "%choice%"=="Y" (
    echo.
    echo Launching application...
    start "" "bin\Release\net10.0-windows\win-x64\publish\ProductPriceCalculator.exe"
)

echo.
echo Build script complete!
pause
