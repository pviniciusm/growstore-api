@ECHO OFF
setlocal

REM Procura automaticamente o .csproj do projeto de testes
set "TEST_PROJECT="
for /r %%f in (*.csproj) do (
    echo %%f | findstr /i "Tests" >nul
    if not errorlevel 1 set "TEST_PROJECT=%%f"
)

if "%TEST_PROJECT%"=="" (
    echo ERRO: Nao encontrei nenhum .csproj com "Tests" no nome.
    pause
    exit /b 1
)

echo Projeto de testes encontrado: %TEST_PROJECT%

for %%P in ("%TEST_PROJECT%") do set "TEST_DIR=%%~dpP"
if exist "%TEST_DIR%TestResults" rd /s /q "%TEST_DIR%TestResults"

REM Instala ferramentas globais se nao estiverem presentes (silencioso)
where coverlet >nul 2>&1 || dotnet tool install --global coverlet.console >nul 2>&1
where reportgenerator >nul 2>&1 || dotnet tool install --global dotnet-reportgenerator-globaltool >nul 2>&1

dotnet restore "%TEST_PROJECT%"
dotnet build "%TEST_PROJECT%" --configuration Release --no-restore

dotnet test "%TEST_PROJECT%" --no-restore --verbosity normal ^
/p:CollectCoverage=true ^
/p:CoverletOutputFormat=cobertura ^
/p:CoverletOutput=./TestResults/coverage.cobertura.xml ^
/p:Exclude="[*]*.Migrations.*"

set "COVERAGE_FILE="
for /r "%TEST_DIR%" %%f in (coverage.cobertura.xml) do if exist "%%f" set "COVERAGE_FILE=%%f"

if "%COVERAGE_FILE%"=="" (
    echo ERRO: Arquivo coverage.cobertura.xml nao encontrado!
    pause
    exit /b 1
)

echo Arquivo de cobertura encontrado em: %COVERAGE_FILE%

reportgenerator ^
-reports:"%COVERAGE_FILE%" ^
-targetdir:"%TEST_DIR%TestResults\CoverageReport" ^
-reporttypes:Html

echo.
echo Coverage report gerado em: %TEST_DIR%TestResults\CoverageReport\index.html

REM Abre no navegador padrao do sistema (nao depende de Chrome especifico)
start "" "%TEST_DIR%TestResults\CoverageReport\index.html"

pause