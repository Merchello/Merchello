/*! merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2015 Merchello;
 * Licensed MIT
 */

(function() { 

    /**
     * @ngdoc controller
     * @name Merchello.Common.Dialogs.EditAddressController
     * @function
     * 
     * @description
     * The controller for adding a country
     */
    angular.module('merchello')
        .controller('Merchello.Common.Dialogs.EditAddressController',
        function ($scope) {

        $scope.init = function() {
            //console.info($scope.dialogData);
            $scope.setVariables();
        };

        $scope.setVariables = function() {
            $scope.address = $scope.dialogData.address;
        };

        $scope.save = function() {
            $scope.submit();
        };

        $scope.init();
    });


    'use strict';
    /**
     * @ngdoc controller
     * @name Merchello.Sales.Dialog.CapturePaymentController
     * @function
     *
     * @description
     * The controller for the dialog used in capturing payments on the sales overview page
     */
    angular.module('merchello')
        .controller('Merchello.Sales.Dialog.CapturePaymentController',
        ['$scope', function($scope) {

            function round(num, places) {
                return +(Math.round(num + "e+" + places) + "e-" + places);
            }

            $scope.payments = {};

            $scope.paymentRequest = {}; //new merchello.Models.PaymentRequest();
            $scope.paymentRequest.invoiceKey = $scope.dialogData.key;
            $scope.paymentRequest.amount = round($scope.dialogData.total, 2);

            var payments = _.map($scope.dialogData.appliedPayments, function(appliedPayment) {
                return appliedPayment.payment;
            });

            if (payments.length > 0) {
                $scope.payments = payments[0];
                $scope.paymentRequest.paymentKey = $scope.payments.key;
                $scope.paymentRequest.paymentMethodKey = $scope.payments.paymentMethodKey;
            }

    }]);
'use strict';
/**
 * @ngdoc controller
 * @name Merchello.Dashboards.Sales.ListController
 * @function
 *
 * @description
 * The controller for the orders list page
 */
angular.module('merchello').controller('Merchello.Dashboards.Sales.ListController',
    ['$scope', '$element', 'angularHelper', 'assetsService', 'notificationsService',
        'invoiceResource', 'queryDisplayBuilder', 'queryResultDisplayBuilder', 'invoiceDisplayBuilder',
        function($scope, $element, angularHelper, assetsService, notificationService, invoiceResource,
                 queryDisplayBuilder, queryResultDisplayBuilder, invoiceDisplayBuilder)
        {

            // expose on scope
            $scope.loaded = true;
            $scope.currentPage = 0;
            $scope.filterText = '';
            $scope.filterStartDate = '';
            $scope.filterEndDate = '';
            $scope.invoices = [];
            $scope.limitAmount = '25';
            $scope.maxPages = 0;
            $scope.orderIssues = [];
            $scope.salesLoaded = true;
            $scope.selectAllOrders = false;
            $scope.selectedOrderCount = 0;
            $scope.settings = {};
            $scope.sortOrder = "desc";
            $scope.sortProperty = "-invoiceNumber";
            $scope.visible = {};
            $scope.visible.bulkActionDropdown = false;
            $scope.currentFilters = [];

            // for testing
            $scope.itemCount = 0;

            //--------------------------------------------------------------------------------------
            // Event Handlers
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name changePage
             * @function
             *
             * @description
             * Changes the current page.
             */
            $scope.changePage = function (page) {
                $scope.currentPage = page;
                var query = buildQuery($scope.filterText);
                loadInvoices(query);
            };

            /**
             * @ngdoc method
             * @name changeSortOrder
             * @function
             *
             * @description
             * Helper function to set the current sort on the table and switch the
             * direction if the property is already the current sort column.
             */
            $scope.changeSortOrder = function (propertyToSort) {
                if ($scope.sortProperty == propertyToSort) {
                    if ($scope.sortOrder == "asc") {
                        $scope.sortProperty = "-" + propertyToSort;
                        $scope.sortOrder = "desc";
                    } else {
                        $scope.sortProperty = propertyToSort;
                        $scope.sortOrder = "asc";
                    }
                } else {
                    $scope.sortProperty = propertyToSort;
                    $scope.sortOrder = "asc";
                }
                var query = buildQuery($scope.filterText);
                loadInvoices(query);
            };

            /**
             * @ngdoc method
             * @name limitChanged
             * @function
             *
             * @description
             * Helper function to set the amount of items to show per page for the paging filters and calculations
             */
            $scope.limitChanged = function (newVal) {
                $scope.limitAmount = newVal;
                $scope.currentPage = 0;
                var query = buildQuery($scope.filterText);
                loadInvoices(query);
            };

            /**
             * @ngdoc method
             * @name filterWithWildcard
             * @function
             *
             * @description
             * Fired when the filter button next to the filter text box at the top of the page is clicked.
             */
            $scope.filterWithWildcard = function (filterText) {
                var query = buildQuery(filterText);
                loadInvoices(query);
            };

            /**
             * @ngdoc method
             * @name filterWithDates
             * @function
             *
             * @description
             * Fired when the filter button next to the filter text box at the top of the page is clicked.
             */
            $scope.filterWithDates = function(filterStartDate, filterEndDate) {
                var query = buildQueryDates(filterStartDate, filterEndDate);
                loadInvoices(query);
            };

            /**
             * @ngdoc method
             * @name resetFilters
             * @function
             *
             * @description
             * Fired when the reset filter button is clicked.
             */
            $scope.resetFilters = function () {
                var query = buildQuery();
                $scope.currentFilters = [];
                $scope.filterText = "";
                $scope.filterStartDate = "";
                $scope.filterEndDate = "";
                loadInvoices(query);
                $scope.filterAction = false;
            };



            //--------------------------------------------------------------------------------------
            // Helper Methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name setVariables
             * @function
             *
             * @description
             * Returns sort information based off the current $scope.sortProperty.
             */
            $scope.sortInfo = function() {
                var sortDirection, sortBy;
                // If the sortProperty starts with '-', it's representing a descending value.
                if ($scope.sortProperty.indexOf('-') > -1) {
                    // Get the text after the '-' for sortBy
                    sortBy = $scope.sortProperty.split('-')[1];
                    sortDirection = 'Descending';
                    // Otherwise it is ascending.
                } else {
                    sortBy = $scope.sortProperty;
                    sortDirection = 'Ascending';
                }
                return {
                    sortBy: sortBy.toLowerCase(), // We'll want the sortBy all lower case for API purposes.
                    sortDirection: sortDirection
                }
            };

            /**
             * @ngdoc method
             * @name numberOfPages
             * @function
             *
             * @description
             * Helper function to get the amount of items to show per page for the paging
             */
            $scope.numberOfPages = function () {
                return $scope.maxPages;
                //return Math.ceil($scope.products.length / $scope.limitAmount);
            };

            // PRIVATE
            function init() {
                $scope.currencySymbol = '$';
                loadInvoices(buildQuery());
                $scope.loaded = true;
            }

            function loadInvoices(query) {
                $scope.salesLoaded = false;
                $scope.salesLoaded = false;

                var promise = invoiceResource.searchInvoices(query);
                promise.then(function (response) {
                    var queryResult = queryResultDisplayBuilder.transform(response, invoiceDisplayBuilder);
                    $scope.invoices = queryResult.items;
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                    $scope.salesLoaded = true;
                    $scope.maxPages = queryResult.totalPages;
                    $scope.itemCount = queryResult.totalItems;
                }, function (reason) {
                    notificationsService.error("Failed To Load Invoices", reason.message);
                });

            }

            /**
             * @ngdoc method
             * @name buildQuery
             * @function
             *
             * @description
             * Perpares a new query object for passing to the ApiController
             */
            function buildQuery(filterText) {
                var page = $scope.currentPage;
                var perPage = $scope.limitAmount;
                var sortBy = $scope.sortInfo().sortBy;
                var sortDirection = $scope.sortInfo().sortDirection;
                if (filterText === undefined) {
                    filterText = '';
                }
                if (filterText !== $scope.filterText) {
                    page = 0;
                    $scope.currentPage = 0;
                }
                $scope.filterText = filterText;

                var query = queryDisplayBuilder.createDefault();
                query.currentPage = page;
                query.itemsPerPage = perPage;
                query.sortBy = sortBy;
                query.sortDirection = sortDirection;
                query.addFilterTermParam(filterText)

                if (query.parameters.length > 0) {
                    $scope.currentFilters = query.parameters;
                }

                return query;
            };

            /**
             * @ngdoc method
             * @name buildQueryDates
             * @function
             *
             * @description
             * Perpares a new query object for passing to the ApiController
             */
             function buildQueryDates(startDate, endDate) {
                var page = $scope.currentPage;
                var perPage = $scope.limitAmount;
                var sortBy = $scope.sortInfo().sortBy;
                var sortDirection = $scope.sortInfo().sortDirection;
                if (startDate === undefined && endDate === undefined) {
                    $scope.currentFilters = [];
                } else {
                    $scope.currentFilters = [{
                        fieldName: 'invoiceDateStart',
                        value: startDate
                    }, {
                        fieldName: 'invoiceDateEnd',
                        value: endDate
                    }];
                }
                if (startDate !== $scope.filterStartDate) {
                    page = 0;
                    $scope.currentPage = 0;
                }
                $scope.filterStartDate = startDate;
                var query = buildQuery();
                query.addInvoiceDateParam(startDate, 'start');
                query.addInvoiceDateParam(endDate, 'end');

                return query;
            };

            /**
             * @ngdoc method
             * @name setupDatePicker
             * @function
             *
             * @description
             * Sets up the datepickers
             */
            function setupDatePicker(pickerId) {

                // Open the datepicker and add a changeDate eventlistener
                $element.find(pickerId).datetimepicker({
                    pickTime: false
                });

                //Ensure to remove the event handler when this instance is destroyted
                $scope.$on('$destroy', function () {
                    $element.find(pickerId).datetimepicker("destroy");
                });
            };

            //// Initialize
            assetsService.loadCss('lib/datetimepicker/bootstrap-datetimepicker.min.css').then(function () {
                var filesToLoad = [
                    'lib/moment/moment-with-locales.js',
                    'lib/datetimepicker/bootstrap-datetimepicker.js'];
                assetsService.load(filesToLoad).then(
                    function () {
                        //The Datepicker js and css files are available and all components are ready to use.

                        setupDatePicker("#filterStartDate");
                        $element.find("#filterStartDate").datetimepicker().on("changeDate", $scope.applyDateStart);

                        setupDatePicker("#filterEndDate");
                        $element.find("#filterEndDate").datetimepicker().on("changeDate", $scope.applyDateEnd);
                    });
            });

            init();
        }]);

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Sales.OverviewController
     * @function
     *
     * @description
     * The controller for the sales overview page
     */
    angular.module('merchello').controller('Merchello.Dashboards.SalesOverviewController',
        ['$scope', '$routeParams', 'assetsService', 'dialogService', 'localizationService', 'notificationsService',
            'auditLogResource', 'invoiceResource', 'settingsResource', 'paymentResource', 'salesHistoryDisplayBuilder',
            'invoiceDisplayBuilder', 'paymentDisplayBuilder',
        function($scope, $routeParams, assetsService, dialogService, localizationService, notificationsService,
                 auditLogResource, invoiceResource, settingsResource, paymentResource,
                 salesHistoryDisplayBuilder, invoiceDisplayBuilder, paymentDisplayBuilder) {

            $scope.historyLoaded = false;
            $scope.invoice = {};
            $scope.currencySymbol = '';
            $scope.settings = {};
            $scope.salesHistory = {};
            $scope.payments = [];
            $scope.billingAddress = {};

            // exposed methods
            $scope.capturePayment = capturePayment;
            $scope.capturePaymentDialogConfirm = capturePaymentDialogConfirm,
            $scope.openDeleteInvoiceDialog = openDeleteInvoiceDialog;
            $scope.openFulfillShipmentDialog = openFulfillShipmentDialog;
            $scope.localizeMessage = localizeMessage;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description - Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init () {
                loadInvoice($routeParams.id);
                loadSettings();
                $scope.loaded = true;
            };

            function localizeMessage(msg) {
                return msg.localize(localizationService);
            }

            /**
             * @ngdoc method
             * @name loadAuditLog
             * @function
             *
             * @description
             * Load the Audit Log for the invoice via API.
             */
            function loadAuditLog(key) {
                if (key !== undefined) {
                    var promise = auditLogResource.getSalesHistoryByInvoiceKey(key);
                    promise.then(function (response) {
                        var history = salesHistoryDisplayBuilder.transform(response);
                        // TODO this is a patch for a problem in the API
                        if (history.dailyLogs.length) {
                            $scope.salesHistory = history.dailyLogs;
                            angular.forEach(history.dailyLogs, function(daily) {
                              angular.forEach(daily.logs, function(log) {
                                 localizationService.localize(log.message.localizationKey(), log.message.localizationTokens()).then(function(value) {
                                    log.message.formattedMessage = value;
                                 });
                              });
                            });
                        }
                        $scope.historyLoaded = history.dailyLogs.length > 0;
                    }, function (reason) {
                        notificationsService.error('Failed to load sales history', reason.message);
                    });
                }
            };

            /**
             * @ngdoc method
             * @name loadInvoice
             * @function
             *
             * @description - Load an invoice with the associated id.
             */
            function loadInvoice(id) {
                var promise = invoiceResource.getByKey(id);
                promise.then(function (invoice) {
                    $scope.invoice = invoiceDisplayBuilder.transform(invoice);
                    $scope.billingAddress = $scope.invoice.getBillToAddress();
                    loadPayments(id);
                    loadAuditLog(id);

                }, function (reason) {
                    notificationsService.error("Invoice Load Failed", reason.message);
                });

                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            };


           /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description - Load the Merchello settings.
             */
            function loadSettings() {

               var settingsPromise = settingsResource.getAllSettings();
               settingsPromise.then(function(settings) {
                   $scope.settings = settings;
               }, function(reason) {
                   notificationsService.error('Failed to load global settings', reason.message);
               })

                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                }, function (reason) {
                    alert('Failed: ' + reason.message);
                });
            };

            /**
             * @ngdoc method
             * @name loadPayments
             * @function
             *
             * @description - Load the Merchello payments.
             */
            function loadPayments(key) {
                var paymentsPromise = paymentResource.getPaymentsByInvoice(key);
                paymentsPromise.then(function(payments) {
                    $scope.payments = paymentDisplayBuilder.transform(payments);
                }, function(reason) {
                    notificationsService.error('Failed to load payments for invoice', reason.message);
                });

            }

            /*-------------------------------------------------------------------
             * Event Handler Methods
             *-------------------------------------------------------------------*/

            /**
             * @ngdoc method
             * @name capturePayment
             * @function
             *
             * @description - Open the capture shipment dialog.
             */
            function capturePayment() {

                if ($scope.payments.length === 0) {
                    return;
                }
                var payment = payments[0];
                var data = {
                    paymentMethodKey: payment.paymentMethodKey,
                    paymentMethodName: payment.paymentMethodName,
                    remainingBalance: $scope.invoice(payments),
                    captureAmount: 0.0
                };

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/capture.payment.html',
                    show: true,
                    callback: $scope.capturePaymentDialogConfirm,
                    dialogData: data
                });
            };

            /**
             * @ngdoc method
             * @name capturePaymentDialogConfirm
             * @function
             *
             * @description - Capture the payment after the confirmation dialog was passed through.
             */
            function capturePaymentDialogConfirm(paymentRequest) {
                var promiseSave = paymentRequest.capturePayment(paymentRequest);
                promiseSave.then(function (payment) {
                    notificationsService.success("Payment Captured");
                    $scope.loadInvoice(paymentRequest.invoiceKey);
                }, function (reason) {
                    notificationsService.error("Payment Capture Failed", reason.message);
                });
            };

            /**
             * @ngdoc method
             * @name openDeleteInvoiceDialog
             * @function
             *
             * @description - Open the delete payment dialog.
             */
            function openDeleteInvoiceDialog() {
                var dialogData = {};
                dialogData.name = 'Invoice #' + $scope.invoice.invoiceNumber;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Common/Js/Dialogs/deleteconfirmation.html',
                    show: true,
                    callback: processDeleteInvoiceDialog,
                    dialogData: dialogData
                });
            };

            /**
             * @ngdoc method
             * @name openFulfillShipmentDialog
             * @function
             *
             * @description - Open the fufill shipment dialog.
             */
            function openFulfillShipmentDialog() {
                console.info($scope.invoice);
                dialogService.open({
                    template: '/App_Plugins/Merchello/Modules/Order/Dialogs/fulfill.shipment.html',
                    show: true,
                    callback: processFulfillShipmentDialog,
                    dialogData: $scope.invoice.orders[0]    // todo: pull from current order when multiple orders is available
                });
            };

            /**
             * @ngdoc method
             * @name processDeleteInvoiceDialog
             * @function
             *
             * @description - Delete the invoice.
             */
            function processDeleteInvoiceDialog() {
                var promiseDeleteInvoice = invoiceResource.deleteInvoice($scope.invoice.key);
                promiseDeleteInvoice.then(function (response) {
                    notificationsService.success('Invoice Deleted');
                    window.location.href = '#/merchello/merchello/invoicelist/manage';
                }, function (reason) {
                    notificationsService.error('Failed to Delete Invoice', reason.message);
                });
            };

            /**
             * @ngdoc method
             * @name processFulfillPaymentDialog
             * @function
             *
             * @description - Process the fulfill shipment functionality on callback from the dialog service.
             */
            function processFulfillShipmentDialog(data) {
                var promiseNewShipment = shipmentResource.newShipment(data);
                promiseNewShipment.then(function (shipment) {
                    // TODO this is a total hack.  A new model should be defined.
                    shipment.trackingCode = data.trackingNumber;
                    shipment.shipmentStatus = data.shipmentStatus;
                    var promiseSave = shipmentResource.putShipment(shipment, data);
                    promiseSave.then(function () {
                        notificationsService.success("Shipment Created");

                        // todo - this needs to be done outside of the promise
                        $scope.loadInvoice(data.invoiceKey);

                    }, function (reason) {
                        notificationsService.error("Save Shipment Failed", reason.message);
                    });
                }, function (reason) {
                    notificationsService.error("New Shipment Failed", reason.message);
                });
            };

            /*-------------------------------------------------------------------*/

            init();
    }]);


})();