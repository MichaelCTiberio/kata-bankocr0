@echo off
setlocal

set SCRIPT_PATH=%~dp0
set BUILD_ROOT=%SCRIPT_PATH%..

echo.
echo ^> cd %BUILD_ROOT%
cd %BUILD_ROOT%
if errorlevel 1 exit /b 1
echo Build root directory:
cd

echo.
echo ^> call %SCRIPT_PATH%Build.cmd
call %SCRIPT_PATH%Build.cmd
if errorlevel 1 exit /b 1

echo.
echo ^> call %SCRIPT_PATH%VerifyTestCategories.cmd
call %SCRIPT_PATH%VerifyTestCategories.cmd
if errorlevel 1 exit /b 1

echo.
echo ^> call %SCRIPT_PATH%CommitTests.cmd
call %SCRIPT_PATH%CommitTests.cmd
if errorlevel 1 exit /b 1

endlocal
