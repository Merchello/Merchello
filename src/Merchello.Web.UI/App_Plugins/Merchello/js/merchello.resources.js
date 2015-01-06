/*! merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2015 Merchello;
 * Licensed MIT
 */

(function() { 

    angular.module('merchello.resources').factory('auditlogResource', [
        '$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {
        return {

            /**
             * @ngdoc method
             * @name getSalesHistoryByInvoiceKey
             * @description
             **/
            getSalesHistoryByInvoiceKey: function (key) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloAuditLogApiBaseUrl', 'GetSalesHistoryByInvoiceKey'),
                        method: "GET",
                        params: { id: key }
                    }),
                    'Failed to retreive sales history log for invoice with following key: ' + key);
            }

        };

    }]);

    /**
     * @ngdoc service
     * @name merchello.resources.invoiceResource
     * @description Loads in data and allows modification for invoices
     **/
    angular.module('merchello.resources')
        .factory('invoiceResource', [
            '$q', '$http', 'umbRequestHelper',
            function($q, $http, umbRequestHelper) {

                return {

                    /**
                     * @ngdoc method
                     * @name getByKey
                     * @description
                     **/
                    getByKey: function (id) {
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'GetInvoice'),
                                method: "GET",
                                params: { id: id }
                            }),
                            'Failed to retreive data for invoice id: ' + id);
                    },

                    /**
                     * @ngdoc method
                     * @name getByCustomerKey
                     * @description
                     **/
                    getByCustomerKey: function(customerKey) {
                        var query = queryDisplayBuilder.createDefault();
                        query.applyInvoiceQueryDefaults();
                        query.addCustomerKeyParam(customerKey);

                        return umbRequestHelper.resourcePromise(
                            $http.post(umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'SearchByCustomer'), query),
                            'Failed to retreive invoices');
                    },

                    /**
                     * @ngdoc method
                     * @name searchInvoices
                     * @description
                     **/
                    searchInvoices: function (query) {
                        if (query === undefined) {
                            query = queryDisplayBuilder.createDefault();
                            query.applyInvoiceQueryDefaults();
                        }

                        return umbRequestHelper.resourcePromise(
                            $http.post(umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'SearchInvoices'), query),
                            'Failed to retreive invoices');
                    },

                    nextsearchInvoices: function(query) {
                        var deferred = $q.defer();
                        var promises = [];
                        promises.push(this.doSearchInvoices(query));

                    },

                    /**
                     * @ngdoc method
                     * @name searchInvoicesByDateRange
                     * @description
                     **/
                    searchInvoicesByDateRange: function (query) {
                        if (query === undefined) {
                            query = queryDisplayBuilder.createDefault();
                            query.applyInvoiceQueryDefaults();
                        }
                        return umbRequestHelper.resourcePromise(
                            $http.post(umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'SearchByDateRange'), query),
                            'Failed to retreive invoices');
                    },

                    /**
                     * @ngdoc method
                     * @name saveInvoice
                     * @description
                     **/
                    saveInvoice: function (invoice) {
                        return umbRequestHelper.resourcePromise(
                            $http.post(umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'PutInvoice'),
                                invoice
                            ),
                            'Failed to save invoice');
                    },

                    /**
                     * @ngdoc method
                     * @name deleteInvoice
                     * @description
                     **/
                    deleteInvoice: function (invoiceKey) {
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'DeleteInvoice'),
                                method: "GET",
                                params: { id: invoiceKey }
                            }),
                            'Failed to delete invoice');
                    }

                };
            }]);

    angular.module('merchello.resources').factory('settingsResource', [function() {

        /* cacheFactory instance for cached items in the merchelloSettingsService */
        var _settingsCache = $cacheFactory('merchelloSettings');

        /* helper method to get from cache or fall back to an http api call */
        function getCachedOrApi(cacheKey, apiMethod, entityName)
        {
            var deferred = $q.defer();

            var dataFromCache = _settingsCache.get(cacheKey);

            if (dataFromCache) {
                deferred.resolve(dataFromCache);
            }
            else {
                var promiseFromApi = umbRequestHelper.resourcePromise(
                    $http.get(
                        umbRequestHelper.getApiUrl('merchelloSettingsApiBaseUrl', apiMethod)
                    ),
                    'Failed to get all ' + entityName);

                promiseFromApi.then(function (dataFromApi) {
                    _settingsCache.put(cacheKey, dataFromApi);
                    deferred.resolve(dataFromApi);
                }, function (reason) {
                    deferred.reject(reason);
                });
            }

            return deferred.promise;
        }

        /**
         * @class merchelloSettingsService
         */
        var settingsServices = {

            /**
             * @ngdoc method
             * @name merchello.services.merchelloSettingsService#getAllCountries
             * @methodOf merchello.services.merchelloSettingsService
             * @function
             *
             * @description
             * Gets the supported countries / provinces from the merchello.config
             *
             * @returns {object} an angularjs promise object
             */
            getAllCountries: function () {

                return getCachedOrApi("SettingsCountries", "GetAllCountries", "countries");

            },

            /**
             * @ngdoc method
             * @name merchello.services.merchelloSettingsService#save
             * @methodOf merchello.services.merchelloSettingsService
             * @function
             *
             * @description
             * Saves or updates a store setting value
             *
             * @param {object} storeSettings StoreSettings object for the key/value pairs
             *
             * @returns {object} an angularjs promise object
             */
            save: function (storeSettings) {

                _settingsCache.remove("AllSettings");

                return umbRequestHelper.resourcePromise(
                    $http.post(
                        umbRequestHelper.getApiUrl('merchelloSettingsApiBaseUrl', 'PutSettings'),
                        storeSettings
                    ),
                    'Failed to save data for Store Settings');
            },

            /**
             * @ngdoc method
             * @name merchello.services.merchelloSettingsService#getAllSettings
             * @methodOf merchello.services.merchelloSettingsService
             * @function
             *
             * @description
             * Gets all store setting values
             *
             * @returns {object} an angularjs promise object
             */
            getAllSettings: function () {

                return getCachedOrApi("AllSettings", "GetAllSettings", "settings");

            },

            getCurrentSettings: function() {
                var deferred = $q.defer();

                var promiseArray = [];

                promiseArray.push(settingsServices.getAllSettings());

                var promise = $q.all(promiseArray);
                promise.then(function (data) {
                    deferred.resolve(new merchello.Models.StoreSettings(data[0]));
                }, function(reason) {
                    deferred.reject(reason);
                });

                return deferred.promise;
            },

            /**
             * @ngdoc method
             * @name merchello.services.merchelloSettingsService#getAllCurrencies
             * @methodOf merchello.services.merchelloSettingsService
             * @function
             *
             * @description
             * Gets all the currencies
             *
             * @returns {object} an angularjs promise object
             */
            getAllCurrencies: function () {

                return getCachedOrApi("AllCurrency", "GetAllCurrencies", "settings");

            },

            getCurrencySymbol: function () {
                var deferred = $q.defer();

                var promiseArray = [];

                promiseArray.push(settingsServices.getAllSettings());
                promiseArray.push(settingsServices.getAllCurrencies());

                var promise = $q.all(promiseArray);
                promise.then(function (data) {
                    var settingsFromServer = new merchello.Models.StoreSettings(data[0]);
                    var currenciesFromServer = data[1];

                    var currencyList = _.map(currenciesFromServer, function (currencyAttrs) {
                        return new merchello.Models.Currency(currencyAttrs);
                    });

                    var selectedCurrency = _.find(currencyList, function (currency) {
                        return currency.currencyCode == settingsFromServer.currencyCode;
                    });

                    deferred.resolve(selectedCurrency.symbol);
                }, function (reason) {
                    deferred.reject(reason);
                });

                return deferred.promise;
            },


            /**
             * @ngdoc method
             * @name merchello.services.merchelloSettingsService#getTypeFields
             * @methodOf merchello.services.merchelloSettingsService
             * @function
             *
             * @description
             * Gets all the type fields
             *
             * @returns {object} an angularjs promise object
             */
            getTypeFields: function () {

                return getCachedOrApi("AllTypeFields", "GetTypeFields", "settings");

                //return umbRequestHelper.resourcePromise(
                //   $http.get(
                //        umbRequestHelper.getApiUrl('merchelloSettingsApiBaseUrl', 'GetTypeFields')
                //    ),
                //    'Failed to get all settings');

            }

        };

        return settingsServices;

    }]);


})();