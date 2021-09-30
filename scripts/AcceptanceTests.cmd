@echo off
dotnet test --noLogo --no-build --no-restore --filter "TestSuite=AcceptanceTests" %*
