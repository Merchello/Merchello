(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.PaymentController
     * @function
     * 
     * @description
     * The controller for the payment settings page
     */
    controllers.PaymentController = function ($scope, notificationsService, dialogService, merchelloPaymentGatewayService) {

        $scope.paymentGatewayProviders = [];

        $scope.getProviderByKey = function(providerkey) {
            return _.find($scope.paymentGatewayProviders, function (gatewayprovider) { return gatewayprovider.key == providerkey; });
        }
        //$scope.paymentMethods = [];
        //$scope.paymentResources = [];

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        $scope.loadAllPaymentGatewayProviders = function() {

            var promiseAllProviders = merchelloPaymentGatewayService.getAllGatewayProviders();
            promiseAllProviders.then(function(allProviders) {

                $scope.paymentGatewayProviders = _.map(allProviders, function(providerFromServer) {
                    return new merchello.Models.GatewayProvider(providerFromServer);
                });

                _.each($scope.paymentGatewayProviders, function(provider) {
                    $scope.loadPaymentGatewayResources(provider);
                    $scope.loadPaymentMethods(provider.key);
                });

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function(reason) {

                notificationsService.error("Available Payment Providers Load Failed", reason.message);

            });

        };

        $scope.loadPaymentGatewayResources = function (provider) {

            var promiseAllResources = merchelloPaymentGatewayService.getGatewayResources(provider.key);
            promiseAllResources.then(function (allResources) {

                provider.resources = _.map(allResources, function (resourceFromServer) {
                    return new merchello.Models.GatewayResource(resourceFromServer);
                });

            }, function (reason) {

                notificationsService.error("Available Payment Provider Resources Load Failed", reason.message);

            });

        };

        $scope.loadPaymentMethods = function (providerKey) {

            var provider = $scope.getProviderByKey(providerKey);

            var promiseAllResources = merchelloPaymentGatewayService.getPaymentProviderPaymentMethods(providerKey);
            promiseAllResources.then(function (allMethods) {

                provider.methods = _.map(allMethods, function (methodFromServer) {
                    return new merchello.Models.PaymentMethod(methodFromServer);
                });

            }, function (reason) {

                notificationsService.error("Payment Methods Load Failed", reason.message);

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
        $scope.init = function() {

            $scope.loadAllPaymentGatewayProviders();

        };

        $scope.init();


        //--------------------------------------------------------------------------------------
        // Event Handlers
        //--------------------------------------------------------------------------------------

        $scope.removeMethod = function (method) {

            var promiseDelete = merchelloPaymentGatewayService.deletePaymentMethod(method.key);
            promiseDelete.then(function () {

                $scope.loadPaymentMethods(method.providerKey);
                notificationsService.success("Payment Method Deleted");

            }, function (reason) {

                notificationsService.error("Payment Method Delete Failed", reason.message);

            });
        };


        //--------------------------------------------------------------------------------------
        // Dialogs
        //--------------------------------------------------------------------------------------

        $scope.paymentMethodDialogConfirm = function (method) {
            var promiseSave;
            if (method.key.length > 0) {
                // Save existing method
                promiseSave = merchelloPaymentGatewayService.savePaymentMethod(method);
            } else {
                // Create new method
                promiseSave = merchelloPaymentGatewayService.addPaymentMethod(method);
            }

            promiseSave.then(function () {
                $scope.loadPaymentMethods(method.providerKey);
                notificationsService.success("Payment Method Saved");
            }, function (reason) {
                notificationsService.error("Payment Method Save Failed", reason.message);
            });
        };

        $scope.addEditPaymentMethod = function (provider, method) {
            if (method == undefined) {
                method = new merchello.Models.PaymentMethod();
                method.providerKey = provider.key; //Todo: When able to add external providers, make this select the correct provider
                method.paymentCode = provider.resources[0].serviceCode;
            }

            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Settings/Payment/Dialogs/paymentmethod.html',
                show: true,
                callback: $scope.paymentMethodDialogConfirm,
                dialogData: method
            });
        };


        ///////////////////////////////////////////////
        ////    TODO: Change to directive / service?

        $scope.flyouts = {
            deleteManualPaymentMethod: false
        };


        $scope.deleteManualPaymentMethodFlyout = new merchello.Models.Flyout(
            $scope.flyouts.deleteManualPaymentMethod,
            function(isOpen) {
                $scope.flyouts.deleteManualPaymentMethod = isOpen;
            }, {
                clear: function() {
                    self.model = {};
                },
                confirm: function() {
                    var self = $scope.deleteManualPaymentMethodFlyout;

                    $scope.removeMethod(self.model);

                    self.clear();
                    self.close();
                },
                open: function(model) {
                    if (!model) {
                        $scope.deleteManualPaymentMethodFlyout.clear();
                    }
                }
            });

    };

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.PaymentController", ['$scope', 'notificationsService', 'dialogService', 'merchelloPaymentGatewayService', merchello.Controllers.PaymentController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
