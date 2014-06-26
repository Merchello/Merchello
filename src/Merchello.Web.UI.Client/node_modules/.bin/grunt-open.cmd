@IF EXIST "%~dp0\node.exe" (
  "%~dp0\node.exe"  "%~dp0\..\grunt-open\bin\grunt-open" %*
) ELSE (
  node  "%~dp0\..\grunt-open\bin\grunt-open" %*
)