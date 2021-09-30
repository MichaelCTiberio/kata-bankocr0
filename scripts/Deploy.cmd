@echo off

if "%BUILD_ROOT%"=="" (
    echo ^>
    echo ^> ERROR: BUILD_ROOT not set.
    exit /b 1
)

if "%SCRIPT_PATH%"=="" (
    echo ^>
    echo ^> ERROR: SCRIPT_PATH not set.
    exit /b 1
)

set PUB_DIR=%BUILD_ROOT%pub\BankOcr\
set INSTALL_DIR=%BUILD_ROOT%AcceptanceTests\BankOcrInstall\

if exist %INSTALL_DIR% call %SCRIPT_PATH%Task.cmd rd /s/q %INSTALL_DIR%

call %SCRIPT_PATH%Task.cmd %PUB_DIR%Deploy.cmd %INSTALL_DIR%
if errorlevel 1 exit /b 1
