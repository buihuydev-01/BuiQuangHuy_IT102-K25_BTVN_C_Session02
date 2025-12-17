@echo off
REM Script to create project structure on Windows
REM Run this in Command Prompt or PowerShell

echo Creating SportsOddsApp project structure...
echo.

REM Change to your desired location
cd /d C:\Projects
mkdir SportsOddsApp
cd SportsOddsApp

REM Create folder structure
echo Creating folders...
mkdir Models
mkdir Services
mkdir ViewModels

echo.
echo ============================================
echo Folder structure created!
echo ============================================
echo.
echo Now you need to create these files:
echo.
echo ROOT FILES:
echo - SportsOddsApp.csproj
echo - SportsOddsApp.sln
echo - App.xaml
echo - App.xaml.cs
echo - MainWindow.xaml
echo - MainWindow.xaml.cs
echo - appsettings.json
echo.
echo MODELS:
echo - Models\Match.cs
echo.
echo SERVICES:
echo - Services\OddsApiService.cs
echo - Services\JavaScriptDataParser.cs
echo.
echo VIEWMODELS:
echo - ViewModels\MainViewModel.cs
echo.
echo DOCUMENTATION (optional):
echo - README.md
echo - QUICK_START.md
echo - HOW_TO_UPDATE_COOKIES.md
echo.
echo ============================================
echo Open each file in the workspace and copy content
echo to corresponding file on your local machine
echo ============================================
echo.
pause
