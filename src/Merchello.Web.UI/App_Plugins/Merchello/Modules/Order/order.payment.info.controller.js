﻿(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Order.PaymentInfoController
     * @function
     * 
     * @description
     * The controller for the order payment info page
     */
    controllers.PaymentInfoController = function ($scope, $routeParams, dialogService, localizationService, notificationsService, merchelloInvoiceService, merchelloPaymentService, merchelloSettingsService, merchelloShipmentService) {

        //--------------------------------------------------------------------------------------
        // Initialization Methods
        //--------------------------------------------------------------------------------------

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
         * @name setVariables
         * @function
         * 
         * @description
         * Sets the $scope variables.
         */
        $scope.setVariables = function () {
            $scope.loaded = false;
            $scope.invoice = {};
            $scope.typeFields = [];
        };

        //--------------------------------------------------------------------------------------
        // Event Handler Methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name capturePayment
         * @function
         * 
         * @description
         * Open the capture shipment dialog.
         */
        $scope.capturePayment = function () {

            var dialogData = {
                invoice: $scope.invoice,
                currencySymbol: $scope.currencySymbol
            };

            dialogService.open({

                /// TODO: DFB - m-548 pass global settings into both dialogs
                /// in order to get rid of dependency on merchelloGeneral_moneySymbol 

                template: '/App_Plugins/Merchello/Modules/Order/Dialogs/capture.payment.html',
                show: true,
                callback: $scope.capturePaymentDialogConfirm,
                dialogData: dialogData
            });
        };

        //--------------------------------------------------------------------------------------
        // Helper Methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name formatDate
         * @function
         * 
         * @description
         * Format the provided date to something more readable.
         */
        $scope.formatDate = function (date) {
            // TODO: Provide localized date formatting.
            return date.split('T')[0];
        }

        /**
         * @ngdoc method
         * @name hasOrder
         * @function
         * 
         * @description
         * Returns false if the invoice has no orders.
         */
        $scope.hasOrder = function () {
            var result = false;
            if ($scope.invoice.orders !== undefined) {
                if ($scope.invoice.orders.length > 0) {
                    result = true;
                }
            }
            return result;
        };

        /**
         * @ngdoc method
         * @name hasShipments
         * @function
         * 
         * @description
         * Returns false if the invoice has no shipments.
         */
        $scope.hasShipments = function () {
            var result = false;
            if ($scope.invoice.shipments) {
                if ($scope.invoice.shipments.length > 0) {
                    result = true;
                }
            }
            return result;
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
                $scope.loaded = true;
                $scope.loadPayments($scope.invoice);
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
                }http://localhost:58400/Dialogs
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
                $scope.loadShipments($scope.invoice);
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
            if ($scope.hasOrder()) {
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
            }
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

        //--------------------------------------------------------------------------------------

        $scope.init();

    };


    angular.module("umbraco").controller("Merchello.Editors.Order.PaymentInfoController", ['$scope', '$routeParams', 'dialogService', 'localizationService', 'notificationsService', 'merchelloInvoiceService', 'merchelloPaymentService', 'merchelloSettingsService', 'merchelloShipmentService', merchello.Controllers.PaymentInfoController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));

