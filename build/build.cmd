@ECHO OFF

:: MERCHELLO BUILD FILE

ECHO.

:: ensure we have MerchelloVersion.txt
IF NOT EXIST MerchelloVersion.txt (
	ECHO MerchelloVersion.txt is missing!
	GOTO error
)

:: get the version and comment from MerchelloVersion.txt lines 2 and 3
SET RELEASE=
SET COMMENT=
FOR /F "skip=1 delims=" %%i IN (MerchelloVersion.txt) DO IF NOT DEFINED RELEASE SET RELEASE=%%i
FOR /F "skip=2 delims=" %%i IN (MerchelloVersion.txt) DO IF NOT DEFINED COMMENT SET COMMENT=%%i

:: process args

SET INTEGRATION=0
::SET nuGetFolder=%CD%\..\src\packages\
SET nuGetFolder=..\src\packages\
SET SKIPNUGET=0


:processArgs

:: grab the first parameter as a whole eg "/action:start"
:: end if no more parameter
SET SWITCHPARSE=%1
IF [%SWITCHPARSE%] == [] goto endProcessArgs

:: get switch and value
SET SWITCH=
SET VALUE=
FOR /F "tokens=1,* delims=: " %%a IN ("%SWITCHPARSE%") DO SET SWITCH=%%a& SET VALUE=%%b

:: route arg
IF '%SWITCH%'=='/release' GOTO argRelease
IF '%SWITCH%'=='-release' GOTO argRelease
IF '%SWITCH%'=='/comment' GOTO argComment
IF '%SWITCH%'=='-comment' GOTO argComment          
IF '%SWITCH%'=='/integration' GOTO argIntegration  
IF '%SWITCH%'=='-integration' GOTO argIntegration 
IF '%SWITCH%'=='/nugetfolder' GOTO argNugetFolder
IF '%SWITCH%'=='-nugetfolder' GOTO argNugetFolder
IF '%SWITCH%'=='/skipnuget' GOTO argSkipNuget
IF '%SWITCH%'=='-skipnuget' GOTO argSkipNuget
ECHO "Invalid switch %SWITCH%"
GOTO error

:: handle each arg

:argRelease
set RELEASE=%VALUE%
SHIFT
goto processArgs

:argComment
SET COMMENT=%VALUE%
SHIFT
GOTO processArgs

:argIntegration
SET INTEGRATION=1
SHIFT
GOTO processArgs

:argNugetFolder
SET nuGetFolder=%VALUE%
SHIFT
GOTO processArgs

:argSkipNuget
SET SKIPNUGET=1
SHIFT
GOTO processArgs

:endProcessArgs 

:: run


SET VERSION=%RELEASE%
IF [%COMMENT%] EQU [] (SET VERSION=%RELEASE%) ELSE (SET VERSION=%RELEASE%-%COMMENT%)

ECHO.
ECHO Building Merchello %VERSION%
ECHO.


SET MSBUILD="C:\Program Files (x86)\MSBuild\14.0\Bin\MsBuild.exe"
SET PATH=C:\Program Files (x86)\MSBuild\14.0\Bin;%PATH%

:: FastTrack
:: ReplaceIISExpressPortNumber.exe ..\src\Merchello.FastTrack.UI\Merchello.FastTrack.UI.csproj %RELEASE%

ECHO.
ECHO Removing the UI build folder and bower_components folder to make sure everything is clean as a whistle
RD ..\src\Merchello.Umbraco.UI.Client\build /Q /S
:: RD ..\src\Merchello.Umbraco.UI.Client\bower_components /Q /S


ECHO.
ECHO Restoring NuGet packages
ECHO Into %nuGetFolder%
..\src\.nuget\NuGet.exe restore ..\src\Merchello.Core\project.json -OutputDirectory %nuGetFolder% -Verbosity quiet
..\src\.nuget\NuGet.exe restore ..\src\Merchello.Web\project.json -OutputDirectory %nuGetFolder% -Verbosity quiet
..\src\.nuget\NuGet.exe restore ..\src\Merchello.Providers\project.json -OutputDirectory %nuGetFolder% -Verbosity quiet
..\src\.nuget\NuGet.exe restore ..\src\Merchello.Umbraco\project.json -OutputDirectory %nuGetFolder% -Verbosity quiet

ECHO.
ECHO Performing MSBuild and producing Merchello binaries zip files
ECHO This takes a few minutes and logging is set to report warnings
ECHO and errors only so it might seems like nothing is happening for a while. 
ECHO You can check the msbuild.log file for progress.
ECHO.
%MSBUILD% "Merchello.proj" /p:BUILD_RELEASE=%RELEASE% /p:BUILD_COMMENT=%COMMENT% /p:NugetPackagesDirectory=%nuGetFolder% /consoleloggerparameters:Summary;ErrorsOnly;WarningsOnly /fileLogger
IF ERRORLEVEL 1 GOTO error


:success
ECHO.
ECHO No errors were detected!
ECHO There may still be some in the output, which you would need to investigate.
ECHO Warnings are usually normal.
ECHO.
GOTO :EOF

:error

ECHO.
ECHO Errors were detected!
ECHO.

:: don't pause if continuous integration else the build server waits forever
:: before cancelling the build (and, there is noone to read the output anyways)
IF %INTEGRATION% NEQ 1 PAUSE