@echo off
REM ====================================================================
REM Master script to CLEAR all migrations and snapshots for ALL services
REM in OJT_Laboratory_Project
REM ====================================================================
REM WARNING: This script will DELETE all migration files and snapshots!
REM Use this when you want to start fresh with database migrations.
REM ====================================================================

setlocal enabledelayedexpansion

REM Change to script directory (OJT_Laboratory_Project root)
cd /d "%~dp0OJT_Laboratory_Project"

if not exist "IAM_Service" (
    powershell -Command "Write-Host 'Error: OJT_Laboratory_Project folder not found!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please run setup_project.bat first.' -ForegroundColor Yellow"
    exit /b 1
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Red"
powershell -Command "Write-Host '  WARNING: This will DELETE all migrations!' -ForegroundColor Red"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Red"
powershell -Command "Write-Host ''"

REM Counter for tracking
set CLEARED_COUNT=0

REM ====================================================================
REM 1. Clear IAM_Service Migrations
REM ====================================================================
powershell -Command "Write-Host '[1/4] Clearing IAM_Service Migrations...' -ForegroundColor DarkCyan"

cd /d "%~dp0OJT_Laboratory_Project\IAM_Service\IAM_Service.Infrastructure\Migrations"
if exist "*.*" (
    del /f /q *.* 2>nul
    if %ERRORLEVEL% equ 0 (
        powershell -Command "Write-Host '  ✓ Cleared IAM_Service migrations' -ForegroundColor DarkGreen"
        set /a CLEARED_COUNT+=1
    ) else (
        powershell -Command "Write-Host '  ✗ Failed to clear IAM_Service migrations' -ForegroundColor DarkRed"
    )
) else (
    powershell -Command "Write-Host '  - No migrations found in IAM_Service' -ForegroundColor DarkYellow"
)

cd /d "%~dp0OJT_Laboratory_Project"
echo.

REM ====================================================================
REM 2. Clear Laboratory_Service Migrations
REM ====================================================================
powershell -Command "Write-Host '[2/4] Clearing Laboratory_Service Migrations...' -ForegroundColor DarkCyan"

cd /d "%~dp0OJT_Laboratory_Project\Laboratory_Service\Laboratory_Service.Infrastructure\Migrations"
if exist "*.*" (
    del /f /q *.* 2>nul
    if %ERRORLEVEL% equ 0 (
        powershell -Command "Write-Host '  ✓ Cleared Laboratory_Service migrations' -ForegroundColor DarkGreen"
        set /a CLEARED_COUNT+=1
    ) else (
        powershell -Command "Write-Host '  ✗ Failed to clear Laboratory_Service migrations' -ForegroundColor DarkRed"
    )
) else (
    powershell -Command "Write-Host '  - No migrations found in Laboratory_Service' -ForegroundColor DarkYellow"
)

cd /d "%~dp0OJT_Laboratory_Project"
echo.

REM ====================================================================
REM 3. Clear Monitoring_Service Migrations
REM ====================================================================
powershell -Command "Write-Host '[3/4] Clearing Monitoring_Service Migrations...' -ForegroundColor DarkCyan"

cd /d "%~dp0OJT_Laboratory_Project\Monitoring_Service\Monitoring_Service.Infastructure\Migrations"
if exist "*.*" (
    del /f /q *.* 2>nul
    if %ERRORLEVEL% equ 0 (
        powershell -Command "Write-Host '  ✓ Cleared Monitoring_Service migrations' -ForegroundColor DarkGreen"
        set /a CLEARED_COUNT+=1
    ) else (
        powershell -Command "Write-Host '  ✗ Failed to clear Monitoring_Service migrations' -ForegroundColor DarkRed"
    )
) else (
    powershell -Command "Write-Host '  - No migrations found in Monitoring_Service' -ForegroundColor DarkYellow"
)

cd /d "%~dp0OJT_Laboratory_Project"
echo.

REM ====================================================================
REM 4. Clear Simulator_Service Migrations
REM ====================================================================
powershell -Command "Write-Host '[4/4] Clearing Simulator_Service Migrations...' -ForegroundColor DarkCyan"

cd /d "%~dp0OJT_Laboratory_Project\Simulator_Service\Simulator.Infastructure\Migrations"
if exist "*.*" (
    del /f /q *.* 2>nul
    if %ERRORLEVEL% equ 0 (
        powershell -Command "Write-Host '  ✓ Cleared Simulator_Service migrations' -ForegroundColor DarkGreen"
        set /a CLEARED_COUNT+=1
    ) else (
        powershell -Command "Write-Host '  ✗ Failed to clear Simulator_Service migrations' -ForegroundColor DarkRed"
    )
) else (
    powershell -Command "Write-Host '  - No migrations found in Simulator_Service' -ForegroundColor DarkYellow"
)

cd /d "%~dp0OJT_Laboratory_Project"
echo.

REM ====================================================================
REM Summary
REM ====================================================================
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
powershell -Command "Write-Host '  Migration Clear Summary' -ForegroundColor Cyan"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
powershell -Command "Write-Host '  Cleared: %CLEARED_COUNT% service(s)' -ForegroundColor DarkGreen"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
echo.

powershell -Command "Write-Host 'All migrations cleared successfully! ✓' -ForegroundColor DarkGreen"
powershell -Command "Write-Host 'You can now create new migrations with: create_all_migrations.bat' -ForegroundColor Yellow"
exit /b 0

