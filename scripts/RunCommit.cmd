@echo off
setlocal

set SCRIPT_PATH=%~dp0
set BUILD_ROOT=%SCRIPT_PATH%..

echo ^>
echo ^> ---------- COMMIT STAGE RUN ----------

REM Set the build root directory
    call %SCRIPT_PATH%Task.cmd cd %BUILD_ROOT%
    if errorlevel 1 exit /b 1

    REM The mess below will echo the output of 'cd' with some decoration.
    for /f %%i in ('cd') do echo ^> Build root directory: %%i

REM Call the build script
    call %SCRIPT_PATH%Task.cmd %SCRIPT_PATH%Build.cmd
    if errorlevel 1 exit /b 1

REM Verify the test categories
    call %SCRIPT_PATH%Task.cmd %SCRIPT_PATH%VerifyTestCategories.cmd
    if errorlevel 1 exit /b 1

REM Run the commit tests
    call %SCRIPT_PATH%Task.cmd %SCRIPT_PATH%CommitTests.cmd
    if errorlevel 1 exit /b 1
    echo ^>

REM Publish the binaries
    call %SCRIPT_PATH%Task.cmd  %SCRIPT_PATH%Publish.cmd
    if errorlevel 1 exit /b 1
    echo ^>

endlocal
