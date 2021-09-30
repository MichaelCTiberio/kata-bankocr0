@echo off
setlocal

if "%1"=="" (
    echo ERROR: Must specify target location.
    exit /b 1
)

set SOURCE=%~dp0BAnkOcr.exe
set DESTINATION=%1

echo Copying [%SOURCE%] to [%DESTINATION%] 
xcopy %SOURCE% %DESTINATION%

endlocal
