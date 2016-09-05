ECHO Changing directory to Merchello.Web.UI.Client
cd src\Merchello.Web.UI.Client
CALL grunt build
ECHO Changing directory back to Merchello
cd ..\..

ECHO Changing directory to Merchello.Providers.UI.Client
cd src\Merchello.Providers.UI.Client
CALL grunt build
ECHO Changing directory back to Merchello
cd ..\..

ECHO Changing directory to Merchello.Mui.Client
cd src\Merchello.Mui.Client
CALL grunt build
ECHO Changing directory back to Merchello
cd ..\..