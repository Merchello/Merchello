'use strict';
/**
 * @ngdoc controller
 * @name Merchello.Dashboards.Sales.ListController
 * @function
 *
 * @description
 * The controller for the orders list page
 */
angular.module('merchello').controller('Merchello.Backoffice.SalesListController',
    ['$scope', '$element', '$routeParams', '$log', '$filter', 'notificationsService', 'localizationService', 'merchelloTabsFactory', 'settingsResource',
        'invoiceResource', 'entityCollectionResource', 'invoiceDisplayBuilder', 'settingDisplayBuilder',
        function($scope, $element, $routeParams, $log, $filter, notificationService, localizationService, merchelloTabsFactory, settingsResource, invoiceResource, entityCollectionResource,
                 invoiceDisplayBuilder, settingDisplayBuilder)
        {
            $scope.loaded = false;
            $scope.tabs = [];

            $scope.filterStartDate = '';
            $scope.filterEndDate = '';

            $scope.settings = {};
            $scope.entityType = 'Invoice';

            $scope.invoiceDisplayBuilder = invoiceDisplayBuilder;
            $scope.load = load;
            $scope.getColumnValue = getColumnValue;


            var allCurrencies = [];
            var globalCurrency = '$';
            const baseUrl = '#/merchello/merchello/saleoverview/';

            var paid = '';
            var unpaid = '';
            var partial = '';
            var unfulfilled = '';
            var fulfilled = '';
            var open = '';


            const label = '<i class="%0"></i> %1';

            function init() {
                $scope.tabs = merchelloTabsFactory.createSalesListTabs();
                $scope.tabs.setActive('saleslist');
                loadSettings();
                localizationService.localize('merchelloSales_paid').then(function(value) {
                    paid = value;
                });
                localizationService.localize('merchelloSales_unpaid').then(function(value) {
                    unpaid = value;
                });
                localizationService.localize('merchelloSales_partial').then(function(value) {
                    partial = value;
                });
                localizationService.localize('merchelloOrder_fulfilled').then(function(value) {
                    fulfilled = value;
                });
                localizationService.localize('merchelloOrder_unfulfilled').then(function(value) {
                    unfulfilled = value;
                });
                localizationService.localize('merchelloOrder_open').then(function(value) {
                    open = value;
                });
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description - Load the Merchello settings.
             */
            // TODO refactor to use $q.defer
            function loadSettings() {
                // this is needed for the date format
                var settingsPromise = settingsResource.getAllSettings();
                settingsPromise.then(function(allSettings) {
                    $scope.settings = settingDisplayBuilder.transform(allSettings);
                    // currency matching
                    var currenciesPromise = settingsResource.getAllCurrencies();
                    currenciesPromise.then(function(currencies) {
                        allCurrencies = currencies;
                        // default currency
                        var currencySymbolPromise = settingsResource.getCurrencySymbol();
                        currencySymbolPromise.then(function (currencySymbol) {
                            globalCurrency = currencySymbol;
                            $scope.loaded = true;
                            $scope.preValuesLoaded = true;
                        }, function (reason) {
                            notificationService.error('Failed to load the currency symbol', reason.message);
                        });

                    }, function(reason) {
                        notificationService.error('Failed to load all currencies', reason.message);
                    });
                }, function(reason) {
                    notificationService.error('Failed to load all settings', reason.message);
                });
            };

            function load(query) {
                if (query.hasCollectionKeyParam()) {
                    return entityCollectionResource.getCollectionEntities(query);
                } else {
                    return invoiceResource.searchInvoices(query);
                }
            }

            function getColumnValue(result, col) {
                switch(col.name) {
                    case 'invoiceNumber':
                        return '<a href="' + getEditUrl(result) + '">' + result.invoiceNumber + '</a>';
                    case 'invoiceDate':
                        return $filter('date')(result.invoiceDate, $scope.settings.dateFormat);
                    case 'paymentStatus':
                        return getPaymentStatus(result);
                    case 'fulfillmentStatus':
                        return getFulfillmentStatus(result);
                    case 'total':
                        return $filter('currency')(result.total, getCurrencySymbol(result));
                    default:
                        return result[col.name];
                }
            }

            function getPaymentStatus(invoice) {
                var paymentStatus = invoice.getPaymentStatus();
                var cssClass, icon, text;
                switch(paymentStatus) {
                    case 'Paid':
                        //cssClass = 'label-success';
                        icon = 'icon-thumb-up';
                        text = paid;
                        break;
                    case 'Partial':
                        //cssClass = 'label-default';
                        icon = 'icon-handprint';
                        text = partial;
                        break;
                    default:
                        //cssClass = 'label-info';
                        icon = 'icon-thumb-down';
                        text = unpaid;
                        break;
                }
                return label.replace('%0', icon).replace('%1', text);
            }

            function getFulfillmentStatus(invoice) {
                var fulfillmentStatus = invoice.getFulfillmentStatus();
                var cssClass, icon, text;
                switch(fulfillmentStatus) {
                    case 'Fulfilled':
                        //cssClass = 'label-success';
                        icon = 'icon-truck';
                        text = fulfilled;
                        break;
                    case 'Open':
                        //cssClass = 'label-default';
                        icon = 'icon-loading';
                        text = open;
                        break;
                    default:
                        //cssClass = 'label-info';
                        icon = 'icon-box-open';
                        text = unfulfilled;
                        break;
                }
                return label.replace('%0', icon).replace('%1', text);
            }

            /**
             * @ngdoc method
             * @name getCurrencySymbol
             * @function
             *
             * @description
             * Utility method to get the currency symbol for an invoice
             */
            function getCurrencySymbol(invoice) {

                if (invoice.currency.symbol !== '') {
                    return invoice.currency.symbol;
                }

                var currencyCode = invoice.getCurrencyCode();
                var currency = _.find(allCurrencies, function(currency) {
                    return currency.currencyCode === currencyCode;
                });
                if(currency === null || currency === undefined) {
                    return globalCurrency;
                } else {
                    return currency.symbol;
                }
            }

            function getEditUrl(invoice) {
                return baseUrl + invoice.key;
            }

            /**
            * @ngdoc method
            * @name setDefaultDates
            * @function
            *
            * @description
            * Sets the default dates
            */
            function setDefaultDates(actual) {
                var month = actual.getMonth() == 0 ? 11 : actual.getMonth() - 1;
                var start = new Date(actual.getFullYear(), month, actual.getDate());
                var end = new Date(actual.getFullYear(), actual.getMonth(), actual.getDate());
                $scope.filterStartDate = start.toLocaleDateString();
                $scope.filterEndDate = end.toLocaleDateString();
            }

            init();
        }]);
