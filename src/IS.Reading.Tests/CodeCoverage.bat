@ECHO OFF

IF EXIST bin\tools\reportgenerator.exe GOTO run
ECHO ---
ECHO Instalando ReportGenerator...
ECHO ---
dotnet tool install dotnet-reportgenerator-globaltool --tool-path bin\tools
IF %ERRORLEVEL% NEQ 0 GOTO error

:run

ECHO ---
ECHO Executando testes...
ECHO ---
dotnet test --logger "trx;LogFileName=TestResults.trx" --logger "xunit;LogFileName=TestResults.xml" --results-directory ./bin/UnitTests /p:CollectCoverage=true /p:Include="[IS.Reading]*" /p:CoverletOutput=bin\Coverage\  /p:CoverletOutputFormat=cobertura
IF %ERRORLEVEL% NEQ 0 GOTO error

REM Limpando os arquivos para nao ocorrer warning de sobreposição do trx na proxima execucao
DEL /s /q bin\UnitTests
IF %ERRORLEVEL% NEQ 0 GOTO error

ECHO ---
ECHO Gerando relatório HTML...
ECHO ---
bin\tools\reportgenerator.exe "-reports:bin\Coverage\coverage.cobertura.xml" "-targetdir:bin\Coverage" -reporttypes:HTML;HTMLSummary
IF %ERRORLEVEL% NEQ 0 GOTO error

START bin\Coverage\index.htm

ECHO ---
ECHO Script encerrado com sucesso!
ECHO ---

GOTO end

:error
ECHO ---
ECHO Script encerrado com erro
ECHO ---
PAUSE

:end