ECHO Changing directory to Merchello.Web.UI.Client
cd src\Merchello.Web.UI.Client
CALL grunt build
ECHO Changing directory back to Merchello
cd ..\..

ECHO Changing directory to Merchello.Providers.Payment.UI.Client
cd src\Merchello.Providers.Payment.UI.Client
CALL grunt build
ECHO Changing directory back to Merchello
cd ..\..