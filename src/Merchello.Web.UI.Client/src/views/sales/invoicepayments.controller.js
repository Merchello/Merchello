/**
 * @ngdoc controller
 * @name Merchello.Dashboards.InvoicePaymentsController
 * @function
 *
 * @description
 * The controller for the invoice payments view
 */
angular.module('merchello').controller('Merchello.Backoffice.InvoicePaymentsController',
    ['$scope', '$log', '$routeParams', 'merchelloTabsFactory',
        'invoiceResource', 'paymentResource', 'settingsResource',
        'invoiceDisplayBuilder', 'paymentDisplayBuilder',
        function($scope, $log, $routeParams, merchelloTabsFactory, invoiceResource, paymentResource, settingsResource,
        invoiceDisplayBuilder, paymentDisplayBuilder) {

            $scope.loaded = false;
            $scope.tabs = [];
            $scope.invoice = {};
            $scope.payments = [];
            $scope.settings = {};
            $scope.currencySymbol = '';
            $scope.remainingBalance = 0;

        function init() {
            var key = $routeParams.id;
            loadInvoice(key);
            $scope.tabs = merchelloTabsFactory.createSalesTabs(key);
            $scope.tabs.setActive('payments');
        }

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
                loadSettings();
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                //console.info($scope.invoice);
            }, function (reason) {
                notificationsService.error("Invoice Load Failed", reason.message);
            });
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

            var currencySymbolPromise = settingsResource.getAllCurrencies();
            currencySymbolPromise.then(function (symbols) {
                var currency = _.find(symbols, function(symbol) {
                    return symbol.currencyCode === $scope.invoice.getCurrencyCode()
                });
                $scope.currencySymbol = currency.symbol;
            }, function (reason) {
                alert('Failed: ' + reason.message);
            });
        };

        function loadPayments(key)
        {
            var paymentsPromise = paymentResource.getPaymentsByInvoice(key);
            paymentsPromise.then(function(payments) {
                $scope.payments = paymentDisplayBuilder.transform(payments);
                $scope.remainingBalance = $scope.invoice.remainingBalance($scope.payments);
            }, function(reason) {
                notificationsService.error('Failed to load payments for invoice', reason.message);
            });
        }

        init();
}]);
