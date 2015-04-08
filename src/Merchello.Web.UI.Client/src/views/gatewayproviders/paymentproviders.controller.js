    angular.module('merchello').controller('Merchello.Backoffice.PaymentProvidersController',
        ['$scope', 'notificationsService', 'dialogService', 'paymentGatewayProviderResource', 'dialogDataFactory', 'merchelloTabsFactory',
           'gatewayResourceDisplayBuilder', 'paymentGatewayProviderDisplayBuilder', 'paymentMethodDisplayBuilder',
        function($scope, notificationsService, dialogService, paymentGatewayProviderResource, dialogDataFactory, merchelloTabsFactory,
                 gatewayResourceDisplayBuilder, paymentGatewayProviderDisplayBuilder, paymentMethodDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.paymentGatewayProviders = [];
            $scope.tabs = [];

            // exposed methods
            $scope.addEditPaymentMethod = addEditPaymentMethod;
            $scope.deletePaymentMethod = deletePaymentMethod;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                loadAllPaymentGatewayProviders();
                $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
                $scope.tabs.setActive('payment');
            }

            /**
             * @ngdoc method
             * @name getProviderByKey
             * @function
             *
             * @description
             * Helper method to get a provider from the paymentGatewayProviders array using the provider key passed in.
             */
            function getProviderByKey(providerkey) {
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
            function loadAllPaymentGatewayProviders() {

                var promiseAllProviders = paymentGatewayProviderResource.getAllGatewayProviders();
                promiseAllProviders.then(function(allProviders) {

                    $scope.paymentGatewayProviders = paymentGatewayProviderDisplayBuilder.transform(allProviders);

                    angular.forEach($scope.paymentGatewayProviders, function(provider) {
                        loadPaymentGatewayResources(provider.key);
                        loadPaymentMethods(provider.key);
                    });

                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;

                }, function(reason) {
                    notificationsService.error("Available Payment Providers Load Failed", reason.message);
                });
            }

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
            function loadPaymentGatewayResources(providerKey) {

                var provider = getProviderByKey(providerKey);
                provider.showSelectResource = false;
                var promiseAllResources = paymentGatewayProviderResource.getGatewayResources(provider.key);
                promiseAllResources.then(function (allResources) {
                    provider.gatewayResources = gatewayResourceDisplayBuilder.transform(allResources);
                    if (provider.gatewayResources.length > 0) {
                        provider.selectedGatewayResource = provider.gatewayResources[0];
                    }

                }, function (reason) {
                    notificationsService.error("Available Payment Provider Resources Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadPaymentMethods
             * @function
             *
             * @description
             * Load the payment gateway methods from the payment gateway service, then wrap the results
             * in Merchello models and add to the provider in the methods collection.
             */
            function loadPaymentMethods(providerKey) {

                var provider = getProviderByKey(providerKey);
                var promiseAllResources = paymentGatewayProviderResource.getPaymentProviderPaymentMethods(providerKey);
                promiseAllResources.then(function (allMethods) {
                    provider.paymentMethods = paymentMethodDisplayBuilder.transform(allMethods);
                }, function (reason) {
                    notificationsService.error("Payment Methods Load Failed", reason.message);
                });
            }



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
            function removeMethod(method) {
                $scope.preValuesLoaded = false;
                var promiseDelete = paymentGatewayProviderResource.deletePaymentMethod(method.key);
                promiseDelete.then(function () {
                    loadAllPaymentGatewayProviders();
                    notificationsService.success("Payment Method Deleted");
                }, function (reason) {
                    notificationsService.error("Payment Method Delete Failed", reason.message);
                });
            }


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
            function paymentMethodDialogConfirm(method) {
                $scope.preValuesLoaded = false;
                var promiseSave;
                if (method.key.length > 0) {
                    // Save existing method
                    promiseSave = paymentGatewayProviderResource.savePaymentMethod(method);
                } else {
                    // Create new method
                    promiseSave = paymentGatewayProviderResource.addPaymentMethod(method);
                }

                var provider = getProviderByKey(method.providerKey);
                provider.showSelectResource = false;

                promiseSave.then(function () {
                    loadPaymentMethods(method.providerKey);
                    loadPaymentGatewayResources(method.providerKey);
                    $scope.preValuesLoaded = true;
                    notificationsService.success("Payment Method Saved");
                }, function (reason) {
                    notificationsService.error("Payment Method Save Failed", reason.message);
                });
            }

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
            function addEditPaymentMethod(provider, method) {
                if (method === undefined) {
                    method = paymentMethodDisplayBuilder.createDefault();
                    method.providerKey = provider.key; //Todo: When able to add external providers, make this select the correct provider
                    method.paymentCode = provider.selectedGatewayResource.serviceCode;
                    method.name = provider.selectedGatewayResource.name;
                }

                // assert that there is a method editor
                //// http://issues.merchello.com/youtrack/issue/M-610
                if (method.dialogEditorView === undefined || method.dialogEditorView.editorView === '') {
                    method.dialogEditorView.editorView = '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.paymentmethod.addedit.html';
                }

                dialogService.open({
                    template: method.dialogEditorView.editorView,
                    show: true,
                    callback: paymentMethodDialogConfirm,
                    dialogData: method
                });
            }

            /// Method delete Dialog

            /**
             * @ngdoc method
             * @name paymentMethodDeleteDialogConfirm
             * @function
             *
             * @description
             * Handles the save after recieving the deleted method from the dialog view/controller
             */
            function paymentMethodDeleteDialogConfirm(dialogData) {
                removeMethod(dialogData.paymentMethod);
            }

            /**
             * @ngdoc method
             * @name deletePaymentMethod
             * @function
             *
             * @description
             * Opens the delete dialog via the Umbraco dialogService
             */
            function deletePaymentMethod(method) {
                var dialogData = dialogDataFactory.createDeletePaymentMethodDialogData();
                dialogData.paymentMethod = method;
                dialogData.name = method.name;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: paymentMethodDeleteDialogConfirm,
                    dialogData: dialogData
                });
            }

            // Initializes the controller
            init();

    }]);
