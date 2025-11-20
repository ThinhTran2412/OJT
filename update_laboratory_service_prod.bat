@echo off
REM ====================================================================
REM Script to update database migrations for Laboratory_Service ONLY
REM Using PRODUCTION configuration (Render database)
REM ====================================================================
REM Usage: update_laboratory_service_prod.bat
REM WARNING: This will update the PRODUCTION database on Render!
REM ====================================================================

setlocal enabledelayedexpansion

REM Change to OJT_Laboratory_Project directory
cd /d "%~dp0OJT_Laboratory_Project"

if not exist "Laboratory_Service" (
    powershell -Command "Write-Host 'Error: Laboratory_Service folder not found!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please run setup_project.bat first.' -ForegroundColor Yellow"
    exit /b 1
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Red"
powershell -Command "Write-Host '  WARNING: PRODUCTION DATABASE UPDATE' -ForegroundColor Red"
powershell -Command "Write-Host '  Target: Render Database (Production)' -ForegroundColor Red"
powershell -Command "Write-Host '  Service: Laboratory_Service' -ForegroundColor Red"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Red"
powershell -Command "Write-Host ''"
powershell -Command "Write-Host 'This will update the PRODUCTION database on Render.' -ForegroundColor Yellow"
powershell -Command "Write-Host 'Are you sure you want to continue? (y/n)' -ForegroundColor Yellow"
set /p CONFIRM="> "

if /i not "%CONFIRM%"=="y" (
    powershell -Command "Write-Host 'Operation cancelled.' -ForegroundColor DarkYellow"
    exit /b 0
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Updating Laboratory_Service Database (PRODUCTION)' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Target: Render Database' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Schema: laboratory_service' -ForegroundColor Cyan"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host ''"

REM Check if dotnet-ef tool is installed
powershell -Command "Write-Host 'Checking EF Core Tools...' -ForegroundColor DarkCyan"
dotnet tool list -g | findstr "dotnet-ef" >nul
if %ERRORLEVEL% neq 0 (
    powershell -Command "Write-Host 'EF Core Tools not found. Installing...' -ForegroundColor DarkYellow"
    dotnet tool install --global dotnet-ef >nul 2>&1
    powershell -Command "Write-Host 'EF Core Tools installed' -ForegroundColor DarkGreen"
) else (
    powershell -Command "Write-Host 'EF Core Tools found' -ForegroundColor DarkGreen"
)

echo.

REM ====================================================================
REM Laboratory_Service Migration
REM ====================================================================
powershell -Command "Write-Host 'Laboratory_Service Migration (PROD)' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%~dp0OJT_Laboratory_Project\Laboratory_Service"
call dotnet restore >nul 2>&1
call dotnet build Laboratory_Service.sln --configuration Production --no-restore >nul 2>&1

echo Applying migrations to PRODUCTION database...
dotnet ef database update ^
  --project Laboratory_Service.Infrastructure/Laboratory_Service.Infrastructure.csproj ^
  --startup-project Laboratory_Service.API/Laboratory_Service.API.csproj ^
  --configuration Production

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
    powershell -Command "Write-Host '✓ Laboratory_Service migration completed successfully!' -ForegroundColor DarkGreen"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
    exit /b 0
) else (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
    powershell -Command "Write-Host '✗ Laboratory_Service migration failed!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please check the errors above.' -ForegroundColor DarkRed"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
    exit /b 1
)

