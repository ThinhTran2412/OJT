@echo off
REM ====================================================================
REM Script to CLEAR all migrations and snapshots for Laboratory_Service ONLY
REM ====================================================================
REM Usage: clean_migrations.bat
REM WARNING: This will DELETE all migration files and snapshots!
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
powershell -Command "Write-Host '==============================================================' -ForegroundColor Red"
powershell -Command "Write-Host '  WARNING: This will DELETE all migrations!' -ForegroundColor Red"
powershell -Command "Write-Host '  Service: Laboratory_Service' -ForegroundColor Red"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Red"
powershell -Command "Write-Host ''"
powershell -Command "Write-Host 'This will delete:' -ForegroundColor Yellow"
powershell -Command "Write-Host '  - All migration files (*.cs) in Migrations folder' -ForegroundColor Yellow"
powershell -Command "Write-Host '  - AppDbContextModelSnapshot.cs' -ForegroundColor Yellow"
powershell -Command "Write-Host ''"
powershell -Command "Write-Host 'Are you sure you want to continue? (y/n)' -ForegroundColor Yellow"
set /p CONFIRM="> "

if /i not "%CONFIRM%"=="y" (
    powershell -Command "Write-Host 'Operation cancelled.' -ForegroundColor DarkYellow"
    exit /b 0
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host "==============================================================" -ForegroundColor Cyan"
powershell -Command "Write-Host '  Clearing Laboratory_Service Migrations' -ForegroundColor Cyan"
powershell -Command "Write-Host "==============================================================" -ForegroundColor Cyan"
powershell -Command "Write-Host ''"

REM ====================================================================
REM Clear Laboratory_Service Migrations
REM ====================================================================
set MIGRATIONS_PATH=Laboratory_Service.Infrastructure\Migrations

if exist "%MIGRATIONS_PATH%" (
    cd /d "%MIGRATIONS_PATH%"
    
    if exist "*.*" (
        powershell -Command "Write-Host 'Deleting migration files...' -ForegroundColor DarkCyan"
        del /f /q *.* 2>nul
        
        if %ERRORLEVEL% equ 0 (
            powershell -Command "Write-Host ''"
            powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
            powershell -Command "Write-Host '✓ Laboratory_Service migrations cleared successfully!' -ForegroundColor DarkGreen"
            powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
            powershell -Command "Write-Host ''"
            powershell -Command "Write-Host 'Next steps:' -ForegroundColor Cyan"
            powershell -Command "Write-Host '  1. Create new migrations:' -ForegroundColor Yellow"
            powershell -Command "Write-Host "     create_migration.bat \"InitialCreate\"" -ForegroundColor Gray
            powershell -Command "Write-Host '  2. Apply migrations:' -ForegroundColor Yellow"
            powershell -Command "Write-Host "     update_dev.bat" -ForegroundColor Gray
            exit /b 0
        ) else (
            powershell -Command "Write-Host '✗ Failed to clear migrations!' -ForegroundColor DarkRed"
            exit /b 1
        )
    ) else (
        powershell -Command "Write-Host 'No migration files found in Laboratory_Service.' -ForegroundColor DarkYellow"
        exit /b 0
    )
) else (
    powershell -Command "Write-Host 'Migrations folder not found: %MIGRATIONS_PATH%' -ForegroundColor DarkYellow"
    powershell -Command "Write-Host 'Nothing to clear.' -ForegroundColor DarkYellow"
    exit /b 0
)

