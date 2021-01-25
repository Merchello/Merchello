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
    ['$scope', '$element', '$routeParams', '$q', '$log', '$filter', 'notificationsService', 'localizationService', 'merchelloTabsFactory', 'settingsResource',
        'invoiceResource', 'entityCollectionResource', 'invoiceDisplayBuilder', 'settingDisplayBuilder',
        function($scope, $element, $routeParams, $q, $log, $filter, notificationService, localizationService, merchelloTabsFactory, settingsResource, invoiceResource, entityCollectionResource,
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
            var cancelled = '';


            const label = '<i class="%0"></i> %1';

            function init() {
                $scope.tabs = merchelloTabsFactory.createSalesListTabs();
                $scope.tabs.setActive('saleslist');

                // localize

                var promises = [
                    localizationService.localize('merchelloSales_paid'),
                    localizationService.localize('merchelloSales_unpaid'),
                    localizationService.localize('merchelloSales_partial'),
                    localizationService.localize('merchelloOrder_fulfilled'),
                    localizationService.localize('merchelloOrder_unfulfilled'),
                    localizationService.localize('merchelloOrder_open'),
                    settingsResource.getAllCombined(),
                    localizationService.localize('merchelloSales_cancelled')
                ];

                $q.all(promises).then(function(local) {
                    paid = local[0];
                    unpaid = local[1];
                    partial = local[2];
                    fulfilled = local[3];
                    unfulfilled = local[4];
                    open = local[5];
                    cancelled = local[7];

                    $scope.settings = local[6].settings;
                    allCurrencies = local[6].currencies;
                    globalCurrency = local[6].currencySymbol;
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                });
            }


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
                        if (result.invoiceNumberPrefix !== '') {
                            return '<a href="' + getEditUrl(result) + '">' + result.invoiceNumberPrefix + '-' + result.invoiceNumber + '</a>';
                        } else {
                            return '<a href="' + getEditUrl(result) + '">' + result.invoiceNumber + '</a>';
                        }
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
                    case 'Cancelled':
                        //cssClass = 'label-default';
                        icon = 'icon-wrong';
                        text = cancelled;
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
                    case 'Ordered':
                        //cssClass = 'label-success';
                        icon = 'icon-thumb-up';
                        text = 'Ordered';
                        break;
                    case 'Open':
                        //cssClass = 'label-default';
                        icon = 'icon-loading';
                        text = open;
                        break;
                    case 'Cancelled':
                        //cssClass = 'label-default';
                        icon = 'icon-wrong';
                        text = cancelled;
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

            init();
        }]);
