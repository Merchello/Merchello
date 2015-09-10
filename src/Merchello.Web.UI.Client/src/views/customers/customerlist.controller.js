    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.CustomerListController
     * @function
     *
     * @description
     * The controller for customer list view
     */
    angular.module('merchello').controller('Merchello.Backoffice.CustomerListController',
        ['$scope', '$routeParams', '$filter', 'notificationsService', 'localizationService', 'settingsResource', 'merchelloTabsFactory', 'customerResource', 'entityCollectionResource',
            'customerDisplayBuilder',
        function($scope, $routeParams, $filter, notificationsService, localizationService, settingsResource, merchelloTabsFactory, customerResource, entityCollectionResource,
                 customerDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;

            $scope.currencySymbol = '';

            $scope.customerDisplayBuilder = customerDisplayBuilder;
            $scope.load = load;
            $scope.entityType = 'Customer';


            // exposed methods
            $scope.getColumnValue = getColumnValue;

            var globalCurrency = '';
            var allCurrencies = [];
            const baseUrl = '#/merchello/merchello/customeroverview/';

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * initialized when the scope loads.
             */
            function init() {

                loadSettings();
                $scope.tabs = merchelloTabsFactory.createCustomerListTabs();
                $scope.tabs.setActive('customerlist');
                $scope.loaded = true;
            }

            function loadSettings() {
                // currency matching
                settingsResource.getAllCombined().then(function(combined) {
                    allCurrencies = combined.currencies;
                    globalCurrency = combined.currencySymbol;
                    $scope.preValuesLoaded = true;
                }, function(reason) {
                    notificationsService.error('Failed to load combined settings', reason.message);
                });
            }

            function load(query) {
                if (query.hasCollectionKeyParam()) {
                    return entityCollectionResource.getCollectionEntities(query);
                } else {
                    return customerResource.searchCustomers(query);
                }
            }

            function getColumnValue(result, col) {
                switch(col.name) {
                    case 'loginName':
                        return '<a href="' + getEditUrl(result) + '">' + result.loginName + '</a>';
                    case 'firstName':
                        return  '<a href="' + getEditUrl(result) + '">' + result.firstName + ' ' + result.lastName + '</a>';
                    case 'location':
                        var address = result.getPrimaryLocation();
                        var ret = address.locality;
                            ret += ' ' + address.region;
                        if (address.countryCode !== '') {
                            ret += ' ' + address.countryCode;
                        }
                        return ret.trim();
                    case 'lastInvoiceTotal':
                        return $filter('currency')(result.getLastInvoice().total, getCurrencySymbol(result.getLastInvoice()));
                    default:
                        return result[col.name];
                }
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
                if (invoice.currency.symbol !== '' && invoice.currency.symbol !== undefined) {
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

            function getEditUrl(customer) {
                return baseUrl + customer.key;
            }

            // Initializes the controller
            init();
    }]);
