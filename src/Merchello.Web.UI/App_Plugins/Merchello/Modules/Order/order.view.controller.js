(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Editors.Order.ViewController
     * @function
     * 
     * @description
     * The controller for the order view page
     */
    controllers.OrderViewController = function ($scope, $routeParams, assetsService, dialogService, notificationsService, merchelloInvoiceService, merchelloOrderService, merchelloPaymentService, merchelloShipmentService, merchelloSettingsService) {

        $scope.capturePayment = function () {
            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Order/Dialogs/capture.payment.html',
                show: true,
                callback: $scope.capturePaymentDialogConfirm,
                dialogData: $scope.invoice
            });
        };

        $scope.capturePaymentDialogConfirm = function (paymentRequest) {
            var promiseSave = merchelloPaymentService.capturePayment(paymentRequest);
            promiseSave.then(function (payment) {
                notificationsService.success("Payment Captured");
                $scope.loadInvoice(paymentRequest.invoiceKey);

            }, function (reason) {
                notificationsService.error("Payment Capture Failed", reason.message);
            });

        };

        $scope.fulfillShipment = function () {
            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Order/Dialogs/fulfill.shipment.html',
                show: true,
                callback: $scope.fulfillShipmentDialogConfirm,
                dialogData: $scope.invoice.orders[0]    // todo: pull from current order when multiple orders is avavilable
            });
        };

        $scope.fulfillShipmentDialogConfirm = function (data) {
            var promiseNew = merchelloShipmentService.newShipment(data);
            promiseNew.then(function (shipment) {
                shipment.trackingCode = data.trackingNumber;
                var promiseSave = merchelloShipmentService.putShipment(shipment, data);
                promiseSave.then(function () {
                    notificationsService.success("Shipment Created");
                    $scope.loadInvoice(data.invoiceKey);
                }, function (reason) {
                    notificationsService.error("Save Shipment Failed", reason.message);
                });
            }, function (reason) {
                notificationsService.error("New Shipment Failed", reason.message);
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
	        $scope.setVariables();
        	$scope.loadTypeFields(function () { $scope.loadInvoice($routeParams.id); });
	        $scope.loadSettings();
	    };

        /**
         * @ngdoc method
         * @name loadInvoice
         * @function
         * 
         * @description
         * Load an invoice with the associated id.
         */
	    $scope.loadInvoice = function (id) {
	        var promise = merchelloInvoiceService.getByKey(id);
	        promise.then(function (invoice) {
	            $scope.invoice = new merchello.Models.Invoice(invoice);
	            _.each($scope.invoice.items, function (lineItem) {
	                if (lineItem.lineItemTfKey) {
	                    var matchedTypeField = _.find($scope.typeFields, function (type) {
	                        return type.typeKey == lineItem.lineItemTfKey;
	                    });
	                    lineItem.lineItemType = matchedTypeField;
	                }
	            });
	            $scope.loadShippingAddress($scope.invoice);
	            $scope.loadPayments($scope.invoice);
	            $scope.loadShipments($scope.invoice);
	        }, function (reason) {
	            notificationsService.error("Invoice Load Failed", reason.message);
	        });
	    };

        /**
         * @ngdoc method
         * @name loadPayments
         * @function
         * 
         * @description
         * Load the payments for the provided invoice.
         */
	    $scope.loadPayments = function (invoice) {
	        var promise = merchelloPaymentService.getAppliedPaymentsByInvoice(invoice.key);
	        promise.then(function (appliedPayments) {
	            invoice.appliedPayments = appliedPayments;
	            invoice.payments = [];
	            if (invoice.appliedPayments.length > 0) {
	                invoice.payments = _.uniq(_.map(invoice.appliedPayments, function (appliedPayment) {
	                    return appliedPayment.payment;
	                }));
	            }
	            _.each(invoice.appliedPayments, function (appliedPayment) {
	                if (appliedPayment.appliedPaymentTfKey) {
	                    var matchedTypeField = _.find($scope.typeFields, function (type) {
	                        return type.typeKey == appliedPayment.appliedPaymentTfKey;
	                    });
	                    appliedPayment.appliedPaymentType = matchedTypeField;
	                }
	            });
	            // used for rendering the payment history
	            invoice.groupedAppliedPayments = _.groupBy(invoice.appliedPayments, function (appliedPayment) {
	                return appliedPayment.payment.paymentMethodName;
	            });
	            $scope.loaded = true;
	            $scope.preValuesLoaded = true;
	        }, function (reason) {
	            notificationsService.error("Payments Load Failed", reason.message);
	        });
	    };


        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         * 
         * @description
         * Load the Merchello settings.
         */
	    $scope.loadSettings = function () {
	        var currencySymbolPromise = merchelloSettingsService.getCurrencySymbol();
	        currencySymbolPromise.then(function (currencySymbol) {
	            $scope.currencySymbol = currencySymbol;

	        }, function (reason) {
	            alert('Failed: ' + reason.message);
	        });
	    };

        /**
         * @ngdoc method
         * @name loadShipments
         * @function
         * 
         * @description
         * Load the shipments associated with the provided invoice.
         */
	    $scope.loadShipments = function (invoice) {
	        var promise = merchelloShipmentService.getShipmentsByInvoice(invoice);
	        promise.then(function (shipments) {
	            invoice.shipments = _.map(shipments, function (shipment) {
	                return new merchello.Models.Shipment(shipment);
	            });
	            $scope.loaded = true;
	            $scope.preValuesLoaded = true;
	        }, function (reason) {
	            notificationsService.error("Shipments Load Failed", reason.message);
	        });
	    };

        /**
         * @ngdoc method
         * @name loadShippingAddress
         * @function
         * 
         * @description
         * Load the shipping address associated with the provided invoice.
         */
	    $scope.loadShippingAddress = function (invoice) {
	        var promise = merchelloOrderService.getShippingAddress(invoice.key);
	        promise.then(function (address) {
	            $scope.shippingAddress = new merchello.Models.Address(address);
	            $scope.loaded = true;
	            $scope.preValuesLoaded = true;
	        }, function (reason) {
	            notificationsService.error("Address Load Failed", reason.message);
	        });
	    };

        /**
         * @ngdoc method
         * @name loadTypeFields
         * @function
         * 
         * @description
         * Load in the type fields.
         */
	    $scope.loadTypeFields = function (nextMethodCall) {
	        var promise = merchelloSettingsService.getTypeFields();
	        promise.then(function (typeFields) {
	            $scope.typeFields = _.map(typeFields, function (type) {
	                return new merchello.Models.TypeField(type);
	            });
	            if (nextMethodCall != undefined) {
	                nextMethodCall();
	            }
	        }, function (reason) {
	            notificationsService.error("TypeFields Load Failed", reason.message);
	        });
	    };

        /**
         * @ngdoc method
         * @name setVariables
         * @function
         * 
         * @description
         * Sets the $scope variables.
         */
        $scope.setVariables = function() {
            $scope.invoice = {};
            $scope.typeFields = [];
            $scope.shippingAddress = {};
        };

        $scope.init();

    };


    angular.module("umbraco").controller("Merchello.Editors.Order.ViewController", ['$scope', '$routeParams', 'assetsService', 'dialogService', 'notificationsService', 'merchelloInvoiceService', 'merchelloOrderService', 'merchelloPaymentService', 'merchelloShipmentService', 'merchelloSettingsService', merchello.Controllers.OrderViewController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));

