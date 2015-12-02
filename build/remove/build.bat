@ECHO OFF

SET MSBuildLoc=
SET MSBuild_VS12=C:\windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe
SET MSBuild_VS13_32="C:\Program Files\MSBuild\12.0\bin\msbuild.exe"
SET MSBuild_VS13_64="C:\Program Files (x86)\MSBuild\12.0\bin\msbuild.exe"

if exist %MSBuild_VS12% SET MSBuildLoc=%MSBuild_VS12%
if exist %MSBuild_VS13_32% SET MSBuildLoc=%MSBuild_VS13_32%
if exist %MSBuild_VS13_64% SET MSBuildLoc=%MSBuild_VS13_64%

%MSBuildLoc% "Merchello.build" /target:Build

IF ERRORLEVEL 1 GOTO :showerror

GOTO :showerror

:showerror
PAUSE
