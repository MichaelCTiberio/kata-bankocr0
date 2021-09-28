@echo off
setlocal

set SCRIPT_PATH=%~dp0

call %SCRIPT_PATH%RunCommit.cmd && call %SCRIPT_PATH%RunAcceptance.cmd

endlocal
