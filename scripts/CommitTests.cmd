@echo off
dotnet test --no-build --no-restore --filter "TestSuite=CommitTests" %*
