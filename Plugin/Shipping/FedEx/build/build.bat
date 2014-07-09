@ECHO OFF

%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe "FedEx.build" /target:Build

IF ERRORLEVEL 1 GOTO :showerror

GOTO :showerror

:showerror
PAUSE