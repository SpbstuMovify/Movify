@echo off
setlocal

if exist "tests\TestResult" (
    rmdir /s /q "tests\TestResult"
)
mkdir "tests\TestResult"

if exist "tests\ChunkerService.Tests\TestResults" (
    rmdir /s /q "tests\ChunkerService.Tests\TestResults"
)

dotnet test tests\ChunkerService.Tests\ChunkerService.Tests.csproj ^
    --collect:"XPlat Code Coverage" ^
    --settings coverlet.runsettings.xml

for /r ".\tests\ChunkerService.Tests\TestResults" %%f in (coverage.cobertura.xml) do (
    move "%%f" ".\tests\TestResult\ChunkerService.Tests.cobertura.xml"
)

reportgenerator ^
  -reports:".\tests\TestResult\*.cobertura.xml" ^
  -targetdir:".\tests\TestResult\Report" ^
  -reporttypes:Html
