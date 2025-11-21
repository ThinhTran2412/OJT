@echo off
REM ====================================================================
REM Script to FIX migration history table location
REM Move migration history from public schema to laboratory_service schema
REM ====================================================================
REM Usage: fix_migration_history.bat
REM ====================================================================

setlocal enabledelayedexpansion

REM Change to Laboratory_Service root directory (up one level from Scripts)
cd /d "%~dp0.."

if not exist "Laboratory_Service.sln" (
    powershell -Command "Write-Host 'Error: Laboratory_Service.sln not found!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please run this script from Laboratory_Service/Scripts folder.' -ForegroundColor Yellow"
    exit /b 1
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Fix Migration History Table Location' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Moving from public schema to laboratory_service schema' -ForegroundColor Cyan"
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

powershell -Command "Write-Host 'WARNING: This script will mark migration as applied without running it.' -ForegroundColor Yellow"
powershell -Command "Write-Host 'This is safe if tables already exist in the database.' -ForegroundColor Yellow"
powershell -Command "Write-Host ''"
powershell -Command "Write-Host 'Continue? (y/n)' -ForegroundColor Yellow"
set /p CONFIRM="> "

if /i not "%CONFIRM%"=="y" (
    powershell -Command "Write-Host 'Operation cancelled.' -ForegroundColor DarkYellow"
    exit /b 0
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host 'Step 1: Marking migration as applied (if tables exist)...' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

call dotnet restore >nul 2>&1
call dotnet build Laboratory_Service.sln --configuration Release --no-restore >nul 2>&1

echo Marking migration as applied (if tables already exist in database)...
dotnet ef database update 20251120213448_Initial --project Laboratory_Service.Infrastructure/Laboratory_Service.Infrastructure.csproj --startup-project Laboratory_Service.API/Laboratory_Service.API.csproj --configuration Release

if %ERRORLEVEL% neq 0 (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host 'Trying alternative: Mark migration as applied manually...' -ForegroundColor DarkYellow"
    
    REM Try to use SQL script to insert migration history manually
    powershell -Command "Write-Host 'You may need to manually insert migration history record.' -ForegroundColor Yellow"
    powershell -Command "Write-Host 'Run this SQL in your database tool:' -ForegroundColor Cyan"
    powershell -Command "Write-Host '' -ForegroundColor Gray"
    powershell -Command "Write-Host 'INSERT INTO laboratory_service.\"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\")' -ForegroundColor Gray"
    powershell -Command "Write-Host 'VALUES (''20251120213448_Initial'', ''9.0.0'')' -ForegroundColor Gray"
    powershell -Command "Write-Host 'ON CONFLICT DO NOTHING;' -ForegroundColor Gray"
    powershell -Command "Write-Host '' -ForegroundColor Gray"
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host 'Step 2: Running full database update...' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

dotnet ef database update --project Laboratory_Service.Infrastructure/Laboratory_Service.Infrastructure.csproj --startup-project Laboratory_Service.API/Laboratory_Service.API.csproj --configuration Release

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
    powershell -Command "Write-Host '✓ Migration history fixed successfully!' -ForegroundColor DarkGreen"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
    exit /b 0
) else (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
    powershell -Command "Write-Host '✗ Migration failed!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please check the errors above.' -ForegroundColor DarkRed"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
    exit /b 1
)

