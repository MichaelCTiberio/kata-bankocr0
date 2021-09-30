@echo off
setlocal

set SCRIPT_PATH=%~dp0
set BUILD_ROOT=%SCRIPT_PATH%..\

echo ^>
echo ^> ---------- ACCEPTANCE STAGE RUN ----------

REM Set the build root directory
    call %SCRIPT_PATH%Task.cmd cd %BUILD_ROOT%
    if errorlevel 1 exit /b 1

    REM The mess below will echo the output of 'cd' with some decoration.
    for /f %%i in ('cd') do echo ^> Build root directory: %%i

REM Deploy the app
    call %SCRIPT_PATH%Task.cmd %SCRIPT_PATH%Deploy.cmd

REM Confirm the parameters are set for the acceptance test run.
    echo ^>
    echo ^> INSTALL_DIR: %INSTALL_DIR%

REM Run the Acceptance tests
    call %SCRIPT_PATH%Task.cmd %SCRIPT_PATH%AcceptanceTests.cmd
    if errorlevel 1 exit /b 1

REM Clean up.
    call %SCRIPT_PATH%Task.cmd rd /s/q %INSTALL_DIR%

echo ^>
endlocal
