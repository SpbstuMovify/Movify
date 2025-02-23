@echo off
setlocal

if exist "tests\TestResult" (
    rmdir /s /q "tests\TestResult"
)
mkdir "tests\TestResult"

if exist "tests\MediaService.Tests\TestResults" (
    rmdir /s /q "tests\MediaService.Tests\TestResults"
)

dotnet test tests\MediaService.Tests\MediaService.Tests.csproj ^
    --collect:"XPlat Code Coverage" ^
    --settings coverlet.runsettings.xml

for /r ".\tests\MediaService.Tests\TestResults" %%f in (coverage.cobertura.xml) do (
    move "%%f" ".\tests\TestResult\MediaService.Tests.cobertura.xml"
)

reportgenerator ^
  -reports:".\tests\TestResult\*.cobertura.xml" ^
  -targetdir:".\tests\TestResult\Report" ^
  -reporttypes:Html
