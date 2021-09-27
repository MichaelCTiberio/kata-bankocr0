@echo off
dotnet test --no-build --filter "TestSuite=CommitTests" %*
