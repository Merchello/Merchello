@ECHO OFF
SET release=1.0.0.0
SET comment=
SET version=%release%
SET package=true

IF [%comment%] EQU [] (SET version=%release%) ELSE (SET version=%release%-%comment%)

%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe "Merchello.build" /target:Build /p:BUILD_RELEASE=%release% /p:BUILD_COMMENT=%comment% /p:PACKAGE_ARCHIVES=%package%

IF ERRORLEVEL 1 GOTO :showerror

GOTO :showerror

:showerror
PAUSE