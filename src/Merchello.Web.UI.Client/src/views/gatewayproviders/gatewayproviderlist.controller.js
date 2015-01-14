/**
 * @ngdoc controller
 * @name Merchello.Backoffice.GatewayProvidersListController
 * @function
 *
 * @description
 * The controller for the gateway providers list view controller
 */
angular.module("umbraco").controller("Merchello.Backoffice.GatewayProvidersListController",
    ['$scope', 'assetsService', 'notificationsService', 'dialogService',
        'gatewayProviderResource', 'gatewayProviderDisplayBuilder',
        function($scope, assetsService, notificationsService, dialogService, gatewayProviderResource, gatewayProviderDisplayBuilder)
        {
            // load the css file
            assetsService.loadCss('/App_Plugins/Merchello/assets/css/merchello.css');

            $scope.loaded = true;
            $scope.notificationGatewayProviders = [];
            $scope.paymentGatewayProviders = [];
            $scope.shippingGatewayProviders = [];
            $scope.taxationGatewayProviders = [];

            // exposed methods
            $scope.activateProvider = activateProvider;
            $scope.deactivateProvider = deactivateProvider;
            $scope.editProviderConfigDialogOpen = editProviderConfigDialogOpen;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                loadAllNotificationGatwayProviders();
                loadAllPaymentGatewayProviders();
                loadAllShippingGatewayProviders();
                loadAllTaxationGatewayProviders();
            }

            /**
             * @ngdoc method
             * @name loadAllNotificationGatwayProviders
             * @function
             *
             * @description
             * Loads in notification gateway providers from server into the scope.  Called in init().
             */
            function loadAllNotificationGatwayProviders() {
                var promiseAllProviders = gatewayProviderResource.getResolvedNotificationGatewayProviders();
                promiseAllProviders.then(function(allProviders) {
                    $scope.notificationGatewayProviders = gatewayProviderDisplayBuilder.transform(allProviders);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function(reason) {
                    notificationsService.error("Available Notification Gateway Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadAllPaymentGatewayProviders
             * @function
             *
             * @description
             * Loads in payment gateway providers from server into the scope.  Called in init().
             */
            function loadAllPaymentGatewayProviders() {
                var promiseAllProviders = gatewayProviderResource.getResolvedPaymentGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.paymentGatewayProviders = gatewayProviderDisplayBuilder.transform(allProviders);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Available Payment Gateway Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadAllShippingGatewayProviders
             * @function
             *
             * @description
             * Loads in shipping gateway providers from server into the scope.  Called in init().
             */
            function loadAllShippingGatewayProviders() {
                var promiseAllProviders = gatewayProviderResource.getResolvedShippingGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.shippingGatewayProviders = gatewayProviderDisplayBuilder.transform(allProviders);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Available Shipping Gateway Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadAllTaxationGatewayProviders
             * @function
             *
             * @description
             * Loads in taxation gateway providers from server into the scope.  Called in init().
             */
            function loadAllTaxationGatewayProviders() {
                var promiseAllProviders = gatewayProviderResource.getResolvedTaxationGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.taxationGatewayProviders = gatewayProviderDisplayBuilder.transform(allProviders);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Available Taxation Gateway Providers Load Failed", reason.message);
                });
            }

            /* -------------------------------------------------------------------
                Events
            ----------------------------------------------------------------------- */

            /**
             * @ngdoc method
             * @name activateProvider
             * @param {GatewayProvider} provider The GatewayProvider to activate
             * @function
             *
             * @description
             * Calls the merchelloGatewayProviderService to activate the provider.
             */
            function activateProvider(provider) {
                var promiseActivate = gatewayProviderResource.activateGatewayProvider(provider);
                promiseActivate.then(function () {
                    provider.activated = true;
                    init();
                    notificationsService.success("Payment Method Activated");
                }, function (reason) {
                    notificationsService.error("Payment Method Activate Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name deactivateProvider
             * @param {GatewayProvider} provider The GatewayProvider to deactivate
             * @function
             *
             * @description
             * Calls the merchelloGatewayProviderService to deactivate the provider.
             */
            function deactivateProvider(provider) {
                var promiseDeactivate = gatewayProviderResource.deactivateGatewayProvider(provider);
                promiseDeactivate.then(function () {
                    provider.activated = false;
                    notificationsService.success("Payment Method Deactivated");
                }, function (reason) {
                    notificationsService.error("Payment Method Deactivate Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name editProviderConfigDialogOpen
             * @param {GatewayProvider} provider The GatewayProvider to configure
             * @function
             *
             * @description
             * Opens the dialog to allow user to add provider configurations
             */
            function editProviderConfigDialogOpen(provider) {
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
                    callback: providerConfigDialogConfirm,
                    dialogData: myDialogData
                });
            }

            /**
             * @ngdoc method
             * @name providerConfigDialogConfirm
             * @param {dialogData} model returned from the dialog view
             * @function
             *
             * @description
             * Handles the data passed back from the provider editor dialog and saves it to the database
             */
            function providerConfigDialogConfirm(data) {
                var promise = gatewayProviderResource.saveGatewayProvider(data.provider);
                promise.then(function (provider) {
                        notificationsService.success("Gateway Provider Saved", "");
                    },
                    function (reason) {
                        notificationsService.error("Gateway Provider Save Failed", reason.message);
                    }
                );
            }

            // Initialize the controller

            init();

        }]);






