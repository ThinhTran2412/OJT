@echo off
REM ====================================================================
REM Script to update database migrations for IAM_Service and Laboratory_Service
REM Using PRODUCTION configuration (appsettings.Production.json)
REM ====================================================================
REM Usage: update_databases_prod.bat
REM WARNING: This will update the PRODUCTION database on Render!
REM          Uses: appsettings.Production.json (Render database connection)
REM ====================================================================

setlocal enabledelayedexpansion

REM Change to OJT_Laboratory_Project directory (go up one level from Scripts_Database_Pro)
set PROJECT_ROOT=%~dp0..\OJT_Laboratory_Project
cd /d "%PROJECT_ROOT%"

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
powershell -Command "Write-Host ''"
powershell -Command "Write-Host 'IMPORTANT: Nếu bảng đã tồn tại và migration fail,' -ForegroundColor Yellow"
powershell -Command "Write-Host 'chạy drop_schema.sql trong database tool để xóa schema cũ!' -ForegroundColor Yellow"
powershell -Command "Write-Host ''"
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
powershell -Command "Write-Host '[1/2] IAM_Service Migration (PROD)' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%PROJECT_ROOT%\IAM_Service"
call dotnet restore >nul 2>&1
call dotnet build IAM_Service.sln --configuration Release --no-restore >nul 2>&1

echo Applying migrations to PRODUCTION database (uses appsettings.Production.json)...
set ASPNETCORE_ENVIRONMENT=Production
dotnet ef database update ^
  --project IAM_Service.Infrastructure/IAM_Service.Infrastructure.csproj ^
  --startup-project IAM_Service.API/IAM_Service.API.csproj ^
  --configuration Release

if %ERRORLEVEL% equ 0 (
    powershell -Command "Write-Host '✓ IAM_Service migration completed!' -ForegroundColor DarkGreen"
    set /a SUCCESS_COUNT+=1
) else (
    powershell -Command "Write-Host '✗ IAM_Service migration failed!' -ForegroundColor DarkRed"
    set /a FAIL_COUNT+=1
)

cd /d "%PROJECT_ROOT%"
echo.

REM ====================================================================
REM 2. Laboratory_Service Migration
REM ====================================================================
powershell -Command "Write-Host '[2/2] Laboratory_Service Migration (PROD)' -ForegroundColor DarkCyan"
powershell -Command "Write-Host '--------------------------------------------------------------' -ForegroundColor DarkGray"

cd /d "%PROJECT_ROOT%\Laboratory_Service"
call dotnet restore >nul 2>&1
call dotnet build Laboratory_Service.sln --configuration Release --no-restore >nul 2>&1

powershell -Command "Write-Host ''"
powershell -Command "Write-Host 'Note: Nếu migration fail với "relation already exists",' -ForegroundColor Yellow"
powershell -Command "Write-Host 'chạy SQL này trong database tool để xóa schema cũ:' -ForegroundColor Yellow"
powershell -Command "Write-Host '  DROP SCHEMA IF EXISTS laboratory_service CASCADE;' -ForegroundColor Gray"
powershell -Command "Write-Host ''"

echo Applying migrations to PRODUCTION database (uses appsettings.Production.json)...
set ASPNETCORE_ENVIRONMENT=Production
dotnet ef database update ^
  --project Laboratory_Service.Infrastructure/Laboratory_Service.Infrastructure.csproj ^
  --startup-project Laboratory_Service.API/Laboratory_Service.API.csproj ^
  --configuration Release

REM Check if migration failed due to "relation already exists" error
if %ERRORLEVEL% neq 0 (
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host 'Migration failed. Checking if tables already exist...' -ForegroundColor DarkYellow"
    powershell -Command "Write-Host 'If tables exist, you need to:' -ForegroundColor Yellow"
    powershell -Command "Write-Host '  1. Xóa schema cũ: DROP SCHEMA IF EXISTS laboratory_service CASCADE;' -ForegroundColor Gray"
    powershell -Command "Write-Host '  2. Chạy lại update_databases_prod.bat' -ForegroundColor Gray"
    powershell -Command "Write-Host ''"
    powershell -Command "Write-Host '✗ Laboratory_Service migration failed!' -ForegroundColor DarkRed"
    set /a FAIL_COUNT+=1
) else (
    powershell -Command "Write-Host '✓ Laboratory_Service migration completed!' -ForegroundColor DarkGreen"
    set /a SUCCESS_COUNT+=1
)

cd /d "%PROJECT_ROOT%"
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

