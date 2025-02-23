@echo off
setlocal

if exist "tests\TestResult" (
    rmdir /s /q "tests\TestResult"
)
mkdir "tests\TestResult"

if exist "tests\AuthService.Tests\TestResults" (
    rmdir /s /q "tests\AuthService.Tests\TestResults"
)

dotnet test tests\AuthService.Tests\AuthService.Tests.csproj ^
    --collect:"XPlat Code Coverage" ^
    --settings coverlet.runsettings.xml

for /r ".\tests\AuthService.Tests\TestResults" %%f in (coverage.cobertura.xml) do (
    move "%%f" ".\tests\TestResult\AuthService.Tests.cobertura.xml"
)

reportgenerator ^
  -reports:".\tests\TestResult\*.cobertura.xml" ^
  -targetdir:".\tests\TestResult\Report" ^
  -reporttypes:Html
