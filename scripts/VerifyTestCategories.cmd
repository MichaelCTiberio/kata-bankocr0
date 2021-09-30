@echo off
setlocal

set TEST_SUITE=TestSuite!=
set COMMIT_TESTS=%TEST_SUITE%CommitTests
set ACCEPTANCE_TESTS=%TEST_SUITE%AcceptanceTests

set TEST_FILTER="%COMMIT_TESTS%&%ACCEPTANCE_TESTS%"

set NO_MATCH_STRING=/c:"No test matches the given testcase filter"

set DOTNET_LIST_TESTS_CMD=dotnet test --noLogo --no-build --no-restore --list-tests --filter %TEST_FILTER%


call %DOTNET_LIST_TESTS_CMD% | findstr %NO_MATCH_STRING% > nul


rem FINDSTR sets the ERRORLEVEL
if errorlevel 2 (
    echo ERROR: FINDSTR failed.
    exit /b 1
) else if errorlevel 1 (
    echo ERROR: Some tests not in a test suite.
    echo.
    call %DOTNET_LIST_TESTS_CMD%
    exit /b 1
) else (
    echo All tests are in a test suite.
    exit /b 0
)

endlocal
