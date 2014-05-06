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

        $scope.invoice = {};
        $scope.typeFields = [];
        $scope.shippingAddress = {};

        assetsService.loadCss("/App_Plugins/Merchello/Common/Css/merchello.css");

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        $scope.loadTypeFields = function (nextMethodCall) {

            var promise = merchelloSettingsService.getTypeFields();

            promise.then(function (typeFields) {

                $scope.typeFields = _.map(typeFields, function(type) {
                    return new merchello.Models.TypeField(type);
                });

                if (nextMethodCall != undefined) {
                    nextMethodCall();
                }

            }, function (reason) {
                notificationsService.error("TypeFields Load Failed", reason.message);
            });
        };

        $scope.loadInvoice = function (id) {

            var promise = merchelloInvoiceService.getByKey(id);

            promise.then(function (invoice) {

                $scope.invoice = new merchello.Models.Invoice(invoice);

                _.each($scope.invoice.items, function (lineItem) {
                    if (lineItem.lineItemTfKey) {
                        var matchedTypeField = _.find($scope.typeFields, function(type) {
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

        $scope.loadPayments = function (invoice) {

            var promise = merchelloPaymentService.getPaymentsByInvoice(invoice.key);

            promise.then(function (payments) {

                invoice.payments = _.map(payments, function (payment) {
                    return new merchello.Models.Payment(payment);
                });

                _.each(invoice.payments, function (payment) {
                    if (payment.paymentTypeFieldKey) {
                        var matchedTypeField = _.find($scope.typeFields, function (type) {
                            return type.typeKey == payment.paymentTypeFieldKey;
                        });
                        payment.paymentType = matchedTypeField;
                    }
                });

				// used for rendering the payment history
                invoice.groupedPayments = _.groupBy(invoice.payments, function(payment) {
                	return payment.paymentMethodName;
                });

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {
                notificationsService.error("Payments Load Failed", reason.message);
            });
        };

	    $scope.loadShipments = function(invoice) {
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
         * @name init
         * @function
         * 
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
        $scope.init = function () {

        	$scope.loadTypeFields(function () { $scope.loadInvoice($routeParams.id); });
	        $scope.loadSettings();

        };

        $scope.init();


        //--------------------------------------------------------------------------------------
        // Events methods
        //--------------------------------------------------------------------------------------




        //--------------------------------------------------------------------------------------
        // Dialogs
        //--------------------------------------------------------------------------------------

        $scope.capturePaymentDialogConfirm = function (paymentRequest) {

            var promiseSave = merchelloPaymentService.capturePayment(paymentRequest);

            promiseSave.then(function (payment) {

                notificationsService.success("Payment Captured");
                $scope.loadInvoice(paymentRequest.invoiceKey);

            }, function (reason) {
                notificationsService.error("Payment Capture Failed", reason.message);
            });

        };

        $scope.capturePayment = function () {

            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Order/Dialogs/capture.payment.html',
                show: true,
                callback: $scope.capturePaymentDialogConfirm,
                dialogData: $scope.invoice
            });

        };


        $scope.fulfillShipmentDialogConfirm = function (data) {

            var promiseNew = merchelloShipmentService.newShipment(data);
            
            promiseNew.then(function (shipment) {
                shipment.trackingCode = data.trackingNumber;

                var promiseSave = merchelloShipmentService.putShipment(shipment);

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

        $scope.fulfillShipment = function () {

            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Order/Dialogs/fulfill.shipment.html',
                show: true,
                callback: $scope.fulfillShipmentDialogConfirm,
                dialogData: $scope.invoice.orders[0]    // todo: pull from current order when multiple orders is avavilable
            });

        };

    };


    angular.module("umbraco").controller("Merchello.Editors.Order.ViewController", ['$scope', '$routeParams', 'assetsService', 'dialogService', 'notificationsService', 'merchelloInvoiceService', 'merchelloOrderService', 'merchelloPaymentService', 'merchelloShipmentService', 'merchelloSettingsService', merchello.Controllers.OrderViewController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));

