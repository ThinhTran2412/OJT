@echo off
REM ====================================================================
REM Script to CREATE new migration for Laboratory_Service ONLY
REM Using DEVELOPMENT configuration (recommended)
REM ====================================================================
REM Usage: create_migration.bat [MigrationName]
REM Example: create_migration.bat "AddNewTable"
REM Note: If MigrationName is not provided, script will prompt for input
REM ====================================================================

setlocal enabledelayedexpansion

REM Check if migration name is provided as parameter, otherwise ask for input
if not "%~1"=="" (
    set MIGRATION_NAME=%~1
) else (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
    powershell -Command "Write-Host '  Create Migration for Laboratory_Service (DEVELOPMENT)' -ForegroundColor Cyan"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host 'Enter migration name (e.g., InitialCreate, AddNewTable):' -ForegroundColor Yellow"
    set /p MIGRATION_NAME="Migration Name: "
    
    if "!MIGRATION_NAME!"=="" (
        powershell -Command "Write-Host 'Error: Migration name cannot be empty!' -ForegroundColor DarkRed"
        exit /b 1
    )
)

REM Change to Laboratory_Service root directory (up one level from Scripts)
cd /d "%~dp0.."

if not exist "Laboratory_Service.sln" (
    powershell -Command "Write-Host 'Error: Laboratory_Service.sln not found!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please run this script from Laboratory_Service/Scripts folder.' -ForegroundColor Yellow"
    exit /b 1
)

powershell -Command "Write-Host ''"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Creating Migration for Laboratory_Service (DEVELOPMENT)' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Migration Name: %MIGRATION_NAME%' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Target: Local Database (localhost:5432)' -ForegroundColor Cyan"
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
REM Laboratory_Service Migration Creation
REM ====================================================================
powershell -Command "Write-Host 'Creating Laboratory_Service Migration (DEV)' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

call dotnet restore >nul 2>&1
call dotnet build Laboratory_Service.sln --configuration Development --no-restore >nul 2>&1

echo Creating migration...
dotnet ef migrations add %MIGRATION_NAME% ^
  --project Laboratory_Service.Infrastructure/Laboratory_Service.Infrastructure.csproj ^
  --startup-project Laboratory_Service.API/Laboratory_Service.API.csproj ^
  --configuration Development

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
    powershell -Command "Write-Host '✓ Laboratory_Service migration created successfully!' -ForegroundColor DarkGreen"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host 'Next steps:' -ForegroundColor Cyan"
    powershell -Command "Write-Host '  1. Review the migration files in:' -ForegroundColor Yellow"
    powershell -Command "Write-Host '     Laboratory_Service.Infrastructure/Migrations/' -ForegroundColor Gray"
    powershell -Command "Write-Host '  2. Apply migration to Development:' -ForegroundColor Yellow"
    powershell -Command "Write-Host "     update_dev.bat" -ForegroundColor Gray
    powershell -Command "Write-Host '  3. Apply migration to Production:' -ForegroundColor Yellow"
    powershell -Command "Write-Host "     update_prod.bat" -ForegroundColor Gray
    exit /b 0
) else (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
    powershell -Command "Write-Host '✗ Laboratory_Service migration creation failed!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please check the errors above.' -ForegroundColor DarkRed"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
    exit /b 1
)

