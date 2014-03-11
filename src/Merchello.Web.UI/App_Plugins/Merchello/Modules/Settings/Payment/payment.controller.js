(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.PaymentController
     * @function
     * 
     * @description
     * The controller for the payment settings page
     */
    controllers.PaymentController = function ($scope, notificationsService, merchelloPaymentGatewayService) {

        $scope.paymentGatewayProviders = [];

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        $scope.loadAllPaymentGatewayProviders = function () {

            var promiseAllProviders = merchelloPaymentGatewayService.getAllGatewayProviders();
            promiseAllProviders.then(function (allProviders) {

                $scope.providers = _.map(allProviders, function (providerFromServer) {
                    return new merchello.Models.GatewayProvider(providerFromServer);
                });

            }, function (reason) {

                notificationsService.error("Available Payment Providers Load Failed", reason.message);

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

        };

        $scope.init();



        $scope.manualPaymentMethods = [];
        $scope.flyouts = {
            addManualPaymentMethod: false,
            deleteManualPaymentMethod: false
        };

        $scope.loadManualPaymentMethods = function () {

            // Note From Kyle: A mock of getting the manual payment method objects.
            var mockMethods = [
                {
                    pk: 0,
                    name: "Pay by Phone",
                    description: "Please call us at 1-888-699-7234 to pay for your order."
                },
                {
                    pk: 1,
                    name: "CoD",
                    description: "We will expect cash delivered upon receipt of your order."
                },
                {
                    pk: 2,
                    name: "Check",
                    description: "Please mail your check to 114 W. Magnolia St Suite 504 Bellingham WA 98225 USA. We will ship your order and notify you upon receipt of your check."
                }
            ];
            $scope.manualPaymentMethods = _.map(mockMethods, function (manualPaymentMethodFromServer) {
                return new merchello.Models.ManualPaymentMethod(manualPaymentMethodFromServer);
            });
            // End of Mocks
            $scope.loaded = true;
            $scope.preValuesLoaded = true;

        };

        $scope.addManualPaymentMethodFlyout = new merchello.Models.Flyout(
            $scope.flyouts.addManualPaymentMethod,
            function (isOpen) {
                $scope.flyouts.addManualPaymentMethod = isOpen;
            },
            {
                clear: function () {
                    var self = $scope.addManualPaymentMethodFlyout;
                    self.model = new merchello.Models.ManualPaymentMethod();
                },
                confirm: function () {
                    var self = $scope.addManualPaymentMethodFlyout;
                    if ((typeof self.model.pk) == "undefined") {
                        var newKey = $scope.manualPaymentMethods.length;
                        // Note From Kyle: This key-creation logic will need to be modified to fit whatever works for the database.
                        self.model.pk = newKey;
                        $scope.manualPaymentMethods.push(self.model);
                        // Note From Kyle: An API call will need to be wired in here to add the new Manual Payment Method to the database.
                    } else {
                        // Note From Kyle: An API call will need to be wired in here to edit the existing Payment Method in the database.
                    }
                    self.clear();
                    self.close();
                },
                open: function (model) {
                    if (!model) {
                        $scope.addManualPaymentMethodFlyout.clear();
                    }
                }
            });

        $scope.deleteManualPaymentMethodFlyout = new merchello.Models.Flyout(
            $scope.flyouts.deleteManualPaymentMethod,
            function (isOpen) {
                $scope.flyouts.deleteManualPaymentMethod = isOpen;
            }, {
                clear: function () {
                    var self = $scope.deleteManualPaymentMethodFlyout;
                    self.model = new merchello.Models.ManualPaymentMethod();
                },
                confirm: function () {
                    var self = $scope.deleteManualPaymentMethodFlyout;
                    var idx = -1;
                    for (i = 0; i < $scope.manualPaymentMethods.length; i++) {
                        if ($scope.manualPaymentMethods[i].pk == self.model.pk) {
                            idx = i;
                        }
                    }
                    if (idx > -1) {
                        $scope.manualPaymentMethods.splice(idx, 1);
                        // Note From Kyle: An API call will need to be wired in here to delete the Manual Payment Method in the database.
                    }
                    self.clear();
                    self.close();
                },
                open: function (model) {
                    if (!model) {
                        $scope.deleteManualPaymentMethodFlyout.clear();
                    }
                }
            });

        $scope.loadManualPaymentMethods();

    }

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.PaymentController", merchello.Controllers.PaymentController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
