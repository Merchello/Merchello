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

        /**
         * @ngdoc method
         * @name getProviderByKey
         * @function
         * 
         * @description
         * Helper method to get a provider from the paymentGatewayProviders array using the provider key passed in.
         */
        $scope.getProviderByKey = function (providerkey) {
            return _.find($scope.paymentGatewayProviders, function (gatewayprovider) { return gatewayprovider.key == providerkey; });
        }

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name loadAllPaymentGatewayProviders
         * @function
         * 
         * @description
         * Load the payment gateway providers from the payment gateway service, then wrap the results
         * in Merchello models and add to the scope via the paymentGatewayProviders collection.
         */
        $scope.loadAllPaymentGatewayProviders = function () {

            var promiseAllProviders = merchelloPaymentGatewayService.getAllGatewayProviders();
            promiseAllProviders.then(function(allProviders) {

                $scope.paymentGatewayProviders = _.map(allProviders, function(providerFromServer) {
                    return new merchello.Models.PaymentGatewayProvider(providerFromServer);
                });

                _.each($scope.paymentGatewayProviders, function(provider) {
                    $scope.loadPaymentGatewayResources(provider.key);
                    $scope.loadPaymentMethods(provider.key);
                });

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function(reason) {

                notificationsService.error("Available Payment Providers Load Failed", reason.message);

            });

        };

        /**
         * @ngdoc method
         * @name loadPaymentGatewayResources
         * @function
         * 
         * @description
         * Load the payment gateway resources from the payment gateway service, then wrap the results
         * in Merchello models and add to the provider in the resources collection.  This will only 
         * return resources that haven't already been added via other methods on the provider.
         */
        $scope.loadPaymentGatewayResources = function (providerKey) {

            var provider = $scope.getProviderByKey(providerKey);

            var promiseAllResources = merchelloPaymentGatewayService.getGatewayResources(provider.key);
            promiseAllResources.then(function (allResources) {

                provider.resources = _.map(allResources, function (resourceFromServer) {
                    return new merchello.Models.GatewayResource(resourceFromServer);
                });

                if (provider.resources.length > 0) {
                    provider.selectedResource = provider.resources[0];                    
                }

            }, function (reason) {

                notificationsService.error("Available Payment Provider Resources Load Failed", reason.message);

            });

        };

        /**
         * @ngdoc method
         * @name loadPaymentMethods
         * @function
         * 
         * @description
         * Load the payment gateway methods from the payment gateway service, then wrap the results
         * in Merchello models and add to the provider in the methods collection.
         */
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

        /**
         * @ngdoc method
         * @name removeMethod
         * @function
         * 
         * @description
         * Calls the payment gateway service to delete the method passed in via the method parameter
         */
        $scope.removeMethod = function (method) {

            var promiseDelete = merchelloPaymentGatewayService.deletePaymentMethod(method.key);
            promiseDelete.then(function () {

                $scope.loadPaymentMethods(method.providerKey);
                $scope.loadPaymentGatewayResources(method.providerKey);
                notificationsService.success("Payment Method Deleted");

            }, function (reason) {

                notificationsService.error("Payment Method Delete Failed", reason.message);

            });
        };


        //--------------------------------------------------------------------------------------
        // Dialogs
        //--------------------------------------------------------------------------------------

        /// Method add/edit Dialog

        /**
         * @ngdoc method
         * @name paymentMethodDialogConfirm
         * @function
         * 
         * @description
         * Handles the save after recieving the edited method from the dialog view/controller
         */
        $scope.paymentMethodDialogConfirm = function (method) {
            var promiseSave;
            if (method.key.length > 0) {
                // Save existing method
                promiseSave = merchelloPaymentGatewayService.savePaymentMethod(method);
            } else {
                // Create new method
                promiseSave = merchelloPaymentGatewayService.addPaymentMethod(method);
            }

            var provider = $scope.getProviderByKey(method.providerKey);
            provider.showSelectResource = false;

            promiseSave.then(function () {
                $scope.loadPaymentMethods(method.providerKey);
                $scope.loadPaymentGatewayResources(method.providerKey);
                notificationsService.success("Payment Method Saved");
            }, function (reason) {
                notificationsService.error("Payment Method Save Failed", reason.message);
            });
        };

        /**
         * @ngdoc method
         * @name addEditPaymentMethod
         * @function
         * 
         * @description
         * Opens the payment method dialog via the Umbraco dialogService.  This will default to the dialog view in Merchello
         * unless specified on the custom method in the payment provider.  Also, if it is an add (not edit) then it will 
         * initialize a new method and pass that to the dialog service.
         */
        $scope.addEditPaymentMethod = function (provider, method) {
            if (method == undefined) {
                method = new merchello.Models.PaymentMethod();
                method.providerKey = provider.key; //Todo: When able to add external providers, make this select the correct provider
                method.paymentCode = provider.selectedResource.serviceCode;
                method.name = provider.selectedResource.name;
            }

            var editorTemplate = '/App_Plugins/Merchello/Modules/Settings/Payment/Dialogs/paymentmethod.html';
            if (method.displayEditor()) {
                editorTemplate = method.dialogEditorView.editorView;
            }

            dialogService.open({
                template: editorTemplate,
                show: true,
                callback: $scope.paymentMethodDialogConfirm,
                dialogData: method
            });
        };

        /// Method delete Dialog

        /**
         * @ngdoc method
         * @name paymentMethodDeleteDialogConfirm
         * @function
         * 
         * @description
         * Handles the save after recieving the deleted method from the dialog view/controller
         */
        $scope.paymentMethodDeleteDialogConfirm = function (method) {
            $scope.removeMethod(method);
        };

        /**
         * @ngdoc method
         * @name deletePaymentMethod
         * @function
         * 
         * @description
         * Opens the delete dialog via the Umbraco dialogService
         */
        $scope.deletePaymentMethod = function (method) {
            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Settings/Payment/Dialogs/paymentdelete.html',
                show: true,
                callback: $scope.paymentMethodDeleteDialogConfirm,
                dialogData: method
            });
        };

    };

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.PaymentController", ['$scope', 'notificationsService', 'dialogService', 'merchelloPaymentGatewayService', merchello.Controllers.PaymentController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
