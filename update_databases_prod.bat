@echo off
REM ====================================================================
REM Script to update database migrations for ALL services
REM Using PRODUCTION configuration (Render database)
REM ====================================================================
REM Usage: update_databases_prod.bat
REM WARNING: This will update the PRODUCTION database on Render!
REM ====================================================================

setlocal enabledelayedexpansion

REM Change to OJT_Laboratory_Project directory
cd /d "%~dp0OJT_Laboratory_Project"

if not exist "IAM_Service" (
    powershell -Command "Write-Host 'Error: OJT_Laboratory_Project folder not found!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please run setup_project.bat first.' -ForegroundColor Yellow"
    exit /b 1
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Red"
powershell -Command "Write-Host '  WARNING: PRODUCTION DATABASE UPDATE' -ForegroundColor Red"
powershell -Command "Write-Host '  Target: Render Database (Production)' -ForegroundColor Red"
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
powershell -Command "Write-Host '  Updating Database Migrations (PRODUCTION)' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Target: Render Database' -ForegroundColor Cyan"
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
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
powershell -Command "Write-Host ''"

REM Counter for tracking success/failure
set SUCCESS_COUNT=0
set FAIL_COUNT=0

REM ====================================================================
REM 1. IAM_Service Migration
REM ====================================================================
powershell -Command "Write-Host '[1/4] IAM_Service Migration (PROD)' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%~dp0OJT_Laboratory_Project\IAM_Service"
call dotnet restore >nul 2>&1
call dotnet build IAM_Service.sln --configuration Production --no-restore >nul 2>&1

echo Applying migrations to PRODUCTION database...
dotnet ef database update ^
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj ^
  --startup-project IAM_Service.API/IAM_Service.API.csproj ^
  --configuration Production

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ IAM_Service migration completed!' -ForegroundColor DarkGreen"
    set /a SUCCESS_COUNT+=1
) else (
    powershell -Command "Write-Host '✗ IAM_Service migration failed!' -ForegroundColor DarkRed"
    set /a FAIL_COUNT+=1
)

cd /d "%~dp0OJT_Laboratory_Project"
echo.

REM ====================================================================
REM 2. Laboratory_Service Migration
REM ====================================================================
powershell -Command "Write-Host '[2/4] Laboratory_Service Migration (PROD)' -ForegroundColor DarkCyan"
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
    powershell -Command "Write-Host '✓ Laboratory_Service migration completed!' -ForegroundColor DarkGreen"
    set /a SUCCESS_COUNT+=1
) else (
    powershell -Command "Write-Host '✗ Laboratory_Service migration failed!' -ForegroundColor DarkRed"
    set /a FAIL_COUNT+=1
)

cd /d "%~dp0OJT_Laboratory_Project"
echo.

REM ====================================================================
REM 3. Monitoring_Service Migration
REM ====================================================================
powershell -Command "Write-Host '[3/4] Monitoring_Service Migration (PROD)' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%~dp0OJT_Laboratory_Project\Monitoring_Service"
call dotnet restore Monitoring_Service.API/Monitoring_Service.API.csproj >nul 2>&1
call dotnet build Monitoring_Service.API/Monitoring_Service.API.csproj --configuration Production --no-restore >nul 2>&1

echo Applying migrations to PRODUCTION database...
dotnet ef database update ^
  --project Monitoring_Service.Infastructure/Monitoring_Service.Infastructure.csproj ^
  --startup-project Monitoring_Service.API/Monitoring_Service.API.csproj ^
  --configuration Production

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ Monitoring_Service migration completed!' -ForegroundColor DarkGreen"
    set /a SUCCESS_COUNT+=1
) else (
    powershell -Command "Write-Host '✗ Monitoring_Service migration failed!' -ForegroundColor DarkRed"
    set /a FAIL_COUNT+=1
)

cd /d "%~dp0OJT_Laboratory_Project"
echo.

REM ====================================================================
REM 4. Simulator_Service Migration
REM ====================================================================
powershell -Command "Write-Host '[4/4] Simulator_Service Migration (PROD)' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%~dp0OJT_Laboratory_Project\Simulator_Service"
call dotnet restore >nul 2>&1
call dotnet build Simulator_Service.sln --configuration Production --no-restore >nul 2>&1

echo Applying migrations to PRODUCTION database...
dotnet ef database update ^
  --project Simulator.Infastructure/Simulator.Infastructure.csproj ^
  --startup-project Simulator.API/Simulator.API.csproj ^
  --configuration Production

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ Simulator_Service migration completed!' -ForegroundColor DarkGreen"
    set /a SUCCESS_COUNT+=1
) else (
    powershell -Command "Write-Host '✗ Simulator_Service migration failed!' -ForegroundColor DarkRed"
    set /a FAIL_COUNT+=1
)

cd /d "%~dp0OJT_Laboratory_Project"
echo.

REM ====================================================================
REM Summary
REM ====================================================================
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
powershell -Command "Write-Host '  Migration Summary (PRODUCTION)' -ForegroundColor Cyan"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
powershell -Command "Write-Host '  Successful: %SUCCESS_COUNT% service(s)' -ForegroundColor DarkGreen"
powershell -Command "Write-Host '  Failed:     %FAIL_COUNT% service(s)' -ForegroundColor DarkRed"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
echo.

if %FAIL_COUNT% equ 0 (
    powershell -Command "Write-Host 'All database migrations completed successfully! ✓' -ForegroundColor DarkGreen"
    exit /b 0
) else (
    powershell -Command "Write-Host 'Some migrations failed. Please check the errors above. ✗' -ForegroundColor DarkRed"
    exit /b 1
)

