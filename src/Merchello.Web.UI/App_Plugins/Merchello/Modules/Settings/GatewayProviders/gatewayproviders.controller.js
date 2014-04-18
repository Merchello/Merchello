(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.GatewayProvidersController
     * @function
     * 
     * @description
     * The controller for the gateway providers settings page
     */

    controllers.GatewayProvidersController = function ($scope, assetsService, notificationsService, dialogService, merchelloGatewayProviderService) {

        $scope.paymentGatewayProviders = [];
        $scope.shippingGatewayProviders = [];
        $scope.taxationGatewayProviders = [];

        assetsService.loadCss("/App_Plugins/Merchello/Common/Css/merchello.css");

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name loadAllPaymentGatewayProviders
         * @function
         * 
         * @description
         * Loads in payment gateway providers from server into the scope.  Called in init().
         */
        $scope.loadAllPaymentGatewayProviders = function () {

            var promiseAllProviders = merchelloGatewayProviderService.getResolvedPaymentGatewayProviders();
            promiseAllProviders.then(function (allProviders) {

                $scope.paymentGatewayProviders = _.map(allProviders, function (providerFromServer) {
                    return new merchello.Models.GatewayProvider(providerFromServer);
                });

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {

                notificationsService.error("Available Payment Gateway Providers Load Failed", reason.message);

            });

        };

        /**
         * @ngdoc method
         * @name loadAllShippingGatewayProviders
         * @function
         * 
         * @description
         * Loads in shipping gateway providers from server into the scope.  Called in init().
         */
        $scope.loadAllShippingGatewayProviders = function () {

            var promiseAllProviders = merchelloGatewayProviderService.getResolvedShippingGatewayProviders();
            promiseAllProviders.then(function (allProviders) {

                $scope.shippingGatewayProviders = _.map(allProviders, function (providerFromServer) {
                    return new merchello.Models.GatewayProvider(providerFromServer);
                });

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {

                notificationsService.error("Available Shipping Gateway Providers Load Failed", reason.message);

            });

        };

        /**
         * @ngdoc method
         * @name loadAllTaxationGatewayProviders
         * @function
         * 
         * @description
         * Loads in taxation gateway providers from server into the scope.  Called in init().
         */
        $scope.loadAllTaxationGatewayProviders = function () {

            var promiseAllProviders = merchelloGatewayProviderService.getResolvedTaxationGatewayProviders();
            promiseAllProviders.then(function (allProviders) {

                $scope.taxationGatewayProviders = _.map(allProviders, function (providerFromServer) {
                    return new merchello.Models.GatewayProvider(providerFromServer);
                });

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {

                notificationsService.error("Available Taxation Gateway Providers Load Failed", reason.message);

            });

        };

        /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
        $scope.init = function () {

            $scope.loadAllPaymentGatewayProviders();
            $scope.loadAllShippingGatewayProviders();
            $scope.loadAllTaxationGatewayProviders();

        };

        $scope.init();


        //--------------------------------------------------------------------------------------
        // Event Handlers
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name activateProvider
         * @param {GatewayProvider} provider The GatewayProvider to activate
         * @function
         * 
         * @description
         * Calls the merchelloGatewayProviderService to activate the provider.
         */
        $scope.activateProvider = function (provider) {

            var promiseActivate = merchelloGatewayProviderService.activateGatewayProvider(provider);
            promiseActivate.then(function () {

                provider.activated = true;
                
                notificationsService.success("Payment Method Activated");

            }, function (reason) {

                notificationsService.error("Payment Method Activate Failed", reason.message);

            });
        };

        /**
         * @ngdoc method
         * @name deactivateProvider
         * @param {GatewayProvider} provider The GatewayProvider to deactivate
         * @function
         * 
         * @description
         * Calls the merchelloGatewayProviderService to deactivate the provider.
         */
        $scope.deactivateProvider = function (provider) {

            var promiseDeactivate = merchelloGatewayProviderService.deactivateGatewayProvider(provider);
            promiseDeactivate.then(function () {

                provider.activated = false;
                
                notificationsService.success("Payment Method Deactivated");

            }, function (reason) {

                notificationsService.error("Payment Method Deactivate Failed", reason.message);

            });
        };

        /**
         * @ngdoc method
         * @name editProviderConfigDialogOpen
         * @param {GatewayProvider} provider The GatewayProvider to configure
         * @function
         * 
         * @description
         * Opens the dialog to allow user to add provider configurations
         */
        $scope.editProviderConfigDialogOpen = function (provider) {

           
            var dialogProvider = provider;
            if (!provider) {      
                return;                
            }

            var myDialogData = {
                provider: dialogProvider
            };
            
            dialogService.open({
                template: provider.dialogEditorView.editorView,
                show: true,
                callback: $scope.providerConfigDialogConfirm,
                dialogData: myDialogData
            });
        };

        $scope.providerConfigDialogConfirm = function(data) {

           

            notificationsService.info("Saving...", "");

            var promise = merchelloGatewayProviderService.saveGatewayProvider(data.provider);

            promise.then(function (provider) {
                notificationsService.success("Gateway provider Saved", "H5YR!");
            },
            function (reason)
                {
                notificationsService.error("Gateway provider Save Failed", reason.message);
                }
            );
        };

    };
    
    angular.module("umbraco").controller("Merchello.Dashboards.Settings.GatewayProvidersController", ['$scope', 'assetsService', 'notificationsService', 'dialogService', 'merchelloGatewayProviderService', merchello.Controllers.GatewayProvidersController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));

