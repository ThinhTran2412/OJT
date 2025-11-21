@echo off
REM ====================================================================
REM Script to FIX missing schema laboratory_service
REM Auto-create schema if not exists, then run migration
REM ====================================================================
REM Usage: fix_schema.bat
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
powershell -Command "Write-Host '  Fix Missing Schema: laboratory_service' -ForegroundColor Cyan"
powershell -Command "Write-Host '  Target: Render Database (Production)' -ForegroundColor Cyan"
powershell -Command "Write-Host '==============================================================' -ForegroundColor Cyan"
powershell -Command "Write-Host ''"
powershell -Command "Write-Host 'IMPORTANT: Nếu bảng đã tồn tại nhưng migration fail,' -ForegroundColor Yellow"
powershell -Command "Write-Host 'chạy manual_fix_migration_history.sql TRƯỚC trong database tool!' -ForegroundColor Yellow"
powershell -Command "Write-Host 'Script sẽ insert migration history record để đánh dấu migration đã được apply.' -ForegroundColor Gray"
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
REM Step 1: Create schema using dotnet ef database update
REM Migration sẽ tự động tạo schema nếu chưa tồn tại (migrationBuilder.EnsureSchema)
REM ====================================================================
powershell -Command "Write-Host 'Step 1: Creating schema and running migration...' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"
powershell -Command "Write-Host 'Migration will automatically create schema if not exists' -ForegroundColor Gray"
powershell -Command "Write-Host ''"

call dotnet restore >nul 2>&1
call dotnet build Laboratory_Service.sln --configuration Release --no-restore >nul 2>&1

powershell -Command "Write-Host ''"
powershell -Command "Write-Host 'IMPORTANT: If migration fails with "relation already exists",' -ForegroundColor Yellow"
powershell -Command "Write-Host 'please run: fix_migration_history.bat' -ForegroundColor Yellow"
powershell -Command "Write-Host ''"

echo Running migration (this will create schema if not exists)...
dotnet ef database update ^
  --project Laboratory_Service.Infrastructure/Laboratory_Service.Infrastructure.csproj ^
  --startup-project Laboratory_Service.API/Laboratory_Service.API.csproj ^
  --configuration Release

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
    powershell -Command "Write-Host '✓ Schema and migration completed successfully!' -ForegroundColor DarkGreen"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host 'Schema laboratory_service should now exist in database.' -ForegroundColor Cyan"
    powershell -Command "Write-Host 'Please refresh your database tool to see the schema.' -ForegroundColor Yellow"
    exit /b 0
) else (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
    powershell -Command "Write-Host '✗ Migration failed!' -ForegroundColor DarkRed"
    powershell -Command "Write-Host 'Please check the errors above.' -ForegroundColor DarkRed"
    powershell -Command "Write-Host '==============================================================' -ForegroundColor Yellow"
    exit /b 1
)

