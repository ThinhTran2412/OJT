@echo off
REM ====================================================================
REM Script to update database migrations for Laboratory_Service ONLY
REM Using DEVELOPMENT configuration (Local database)
REM ====================================================================
REM Usage: Run from Scripts folder or anywhere within Laboratory_Service
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
powershell -Command "Write-Host "  Updating Laboratory_Service Database (DEVELOPMENT)" -ForegroundColor Cyan"
powershell -Command "Write-Host "  Target: Local Database (localhost:5432)" -ForegroundColor Cyan"
powershell -Command "Write-Host "  Schema: laboratory_service" -ForegroundColor Cyan"
powershell -Command "Write-Host "==============================================================" -ForegroundColor Cyan"
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
powershell -Command "Write-Host 'Laboratory_Service Migration (DEV)' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

call dotnet restore >nul 2>&1
call dotnet build Laboratory_Service.sln --configuration Development --no-restore >nul 2>&1

echo Applying migrations to DEVELOPMENT database...
dotnet ef database update ^
  --project Laboratory_Service.Infrastructure/Laboratory_Service.Infrastructure.csproj ^
  --startup-project Laboratory_Service.API/Laboratory_Service.API.csproj ^
  --configuration Development

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

