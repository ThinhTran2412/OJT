@echo off
REM ====================================================================
REM Master script to CREATE new migrations for ALL services
REM in OJT_Laboratory_Project
REM ====================================================================
REM Usage: create_all_migrations.bat "MigrationName"
REM Example: create_all_migrations.bat "AddNewTable"
REM ====================================================================

setlocal enabledelayedexpansion

REM Check if migration name is provided
if "%~1"=="" (
    powershell -Command "Write-Host 'Error: Migration name is required!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Usage: create_all_migrations.bat \"MigrationName\"' -ForegroundColor Yellow"
    powershell -Command "Write-Host 'Example: create_all_migrations.bat \"AddNewTable\"' -ForegroundColor Yellow"
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host 'If you want to create initial migrations, use:' -ForegroundColor Yellow"
    powershell -Command "Write-Host '  create_all_migrations.bat \"InitialCreate\"' -ForegroundColor Yellow"
    exit /b 1
)

set MIGRATION_NAME=%~1

REM Change to OJT_Laboratory_Project directory
cd /d "%~dp0OJT_Laboratory_Project"

if not exist "IAM_Service" (
    powershell -Command "Write-Host 'Error: OJT_Laboratory_Project folder not found!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please run setup_project.bat first.' -ForegroundColor Yellow"
    exit /b 1
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Creating Migrations for ALL Services' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Migration Name: %MIGRATION_NAME%' -ForegroundColor Cyan"
powershell -Command "Write-Host '  OJT_Laboratory_Project' -ForegroundColor Cyan"
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
powershell -Command "Write-Host '[1/4] Creating IAM_Service Migration: %MIGRATION_NAME%' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%~dp0OJT_Laboratory_Project\IAM_Service"
call dotnet restore >nul 2>&1
call dotnet build IAM_Service.sln --configuration Release --no-restore >nul 2>&1

echo Creating migration...
dotnet ef migrations add %MIGRATION_NAME% ^
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj ^
  --startup-project IAM_Service.API/IAM_Service.API.csproj

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ IAM_Service migration created!' -ForegroundColor DarkGreen"
    set /a SUCCESS_COUNT+=1
) else (
    powershell -Command "Write-Host '✗ IAM_Service migration creation failed!' -ForegroundColor DarkRed"
    set /a FAIL_COUNT+=1
)

cd /d "%~dp0OJT_Laboratory_Project"
echo.

REM ====================================================================
REM 2. Laboratory_Service Migration
REM ====================================================================
powershell -Command "Write-Host '[2/4] Creating Laboratory_Service Migration: %MIGRATION_NAME%' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%~dp0OJT_Laboratory_Project\Laboratory_Service"
call dotnet restore >nul 2>&1
call dotnet build Laboratory_Service.sln --configuration Release --no-restore >nul 2>&1

echo Creating migration...
dotnet ef migrations add %MIGRATION_NAME% ^
  --project Laboratory_Service.Infrastructure/Laboratory_Service.Infrastructure.csproj ^
  --startup-project Laboratory_Service.API/Laboratory_Service.API.csproj

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ Laboratory_Service migration created!' -ForegroundColor DarkGreen"
    set /a SUCCESS_COUNT+=1
) else (
    powershell -Command "Write-Host '✗ Laboratory_Service migration creation failed!' -ForegroundColor DarkRed"
    set /a FAIL_COUNT+=1
)

cd /d "%~dp0OJT_Laboratory_Project"
echo.

REM ====================================================================
REM 3. Monitoring_Service Migration
REM ====================================================================
powershell -Command "Write-Host '[3/4] Creating Monitoring_Service Migration: %MIGRATION_NAME%' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%~dp0OJT_Laboratory_Project\Monitoring_Service"
call dotnet restore Monitoring_Service.API/Monitoring_Service.API.csproj >nul 2>&1
call dotnet build Monitoring_Service.API/Monitoring_Service.API.csproj --configuration Release --no-restore >nul 2>&1

echo Creating migration...
dotnet ef migrations add %MIGRATION_NAME% ^
  --project Monitoring_Service.Infastructure/Monitoring_Service.Infastructure.csproj ^
  --startup-project Monitoring_Service.API/Monitoring_Service.API.csproj

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ Monitoring_Service migration created!' -ForegroundColor DarkGreen"
    set /a SUCCESS_COUNT+=1
) else (
    powershell -Command "Write-Host '✗ Monitoring_Service migration creation failed!' -ForegroundColor DarkRed"
    set /a FAIL_COUNT+=1
)

cd /d "%~dp0OJT_Laboratory_Project"
echo.

REM ====================================================================
REM 4. Simulator_Service Migration
REM ====================================================================
powershell -Command "Write-Host '[4/4] Creating Simulator_Service Migration: %MIGRATION_NAME%' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%~dp0OJT_Laboratory_Project\Simulator_Service"
call dotnet restore >nul 2>&1
call dotnet build Simulator_Service.sln --configuration Release --no-restore >nul 2>&1

echo Creating migration...
dotnet ef migrations add %MIGRATION_NAME% ^
  --project Simulator.Infastructure/Simulator.Infastructure.csproj ^
  --startup-project Simulator.API/Simulator.API.csproj

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ Simulator_Service migration created!' -ForegroundColor DarkGreen"
    set /a SUCCESS_COUNT+=1
) else (
    powershell -Command "Write-Host '✗ Simulator_Service migration creation failed!' -ForegroundColor DarkRed"
    set /a FAIL_COUNT+=1
)

cd /d "%~dp0OJT_Laboratory_Project"
echo.

REM ====================================================================
REM Summary
REM ====================================================================
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
powershell -Command "Write-Host '  Migration Creation Summary' -ForegroundColor Cyan"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
powershell -Command "Write-Host '  Successful: %SUCCESS_COUNT% service(s)' -ForegroundColor DarkGreen"
powershell -Command "Write-Host '  Failed:     %FAIL_COUNT% service(s)' -ForegroundColor DarkRed"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
echo.

if %FAIL_COUNT% equ 0 (
    powershell -Command "Write-Host 'All migrations created successfully! ✓' -ForegroundColor DarkGreen"
    powershell -Command "Write-Host 'Run update_all_databases.bat to apply migrations.' -ForegroundColor Yellow"
    exit /b 0
) else (
    powershell -Command "Write-Host 'Some migrations creation failed. Please check the errors above. ✗' -ForegroundColor DarkRed"
    exit /b 1
)

