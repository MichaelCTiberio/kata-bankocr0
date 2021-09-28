@echo off
setlocal

set SCRIPT_PATH=%~dp0
set BUILD_ROOT=%SCRIPT_PATH%..

echo ^>
echo ^> ---------- COMMIT TEST RUN ----------

REM Set the build root directory
    echo ^>
    echo ^> cd %BUILD_ROOT%
    cd %BUILD_ROOT%
    if errorlevel 1 exit /b 1

    REM The mess below will echo the output of 'cd' with some decoration.
    for /f %%i in ('cd') do echo ^> Build root directory: %%i

REM Call the build script
    echo ^>
    echo ^> call %SCRIPT_PATH%Build.cmd
    call %SCRIPT_PATH%Build.cmd
    if errorlevel 1 exit /b 1

REM Verify the test categories
    echo ^>
    echo ^> call %SCRIPT_PATH%VerifyTestCategories.cmd
    call %SCRIPT_PATH%VerifyTestCategories.cmd
    if errorlevel 1 exit /b 1

REM Run the commit tests
    echo ^>
    echo ^> call %SCRIPT_PATH%CommitTests.cmd
    call %SCRIPT_PATH%CommitTests.cmd
    if errorlevel 1 exit /b 1
    echo ^>

endlocal
