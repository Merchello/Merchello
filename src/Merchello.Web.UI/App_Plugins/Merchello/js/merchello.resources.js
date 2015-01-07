/*! merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2015 Merchello;
 * Licensed MIT
 */

(function() { 

    angular.module('merchello.resources').factory('auditLogResource', [
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

    /**
     * @ngdoc service
     * @name merchello.resources.paymentResource
     * @description Loads in data and allows modification for payments
     **/
    angular.module('merchello.resources').factory('paymentResource',
        ['$q', '$http', 'umbRequestHelper',
        function($q, $http, umbRequestHelper) {

        return {

            getPayment: function (key) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'GetPayment'),
                        method: "GET",
                        params: { id: key }
                    }),
                    'Failed to get payment: ' + key);
            },

            getPaymentsByInvoice: function (invoiceKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'GetPaymentsByInvoice'),
                        method: "GET",
                        params: { id: invoiceKey }
                    }),
                    'Failed to get payments by invoice: ' + invoiceKey);
            },

            getAppliedPaymentsByInvoice: function (invoiceKey) {
                var deferred = $q.defer();
                var appliedPayments = [];

                var promise = paymentService.getPaymentsByInvoice(invoiceKey);
                promise.then(function (data) {
                    var payments = _.map(data, function (payment) {
                        return new merchello.Models.Payment(payment);
                    });

                    angular.forEach(payments, function (payment) {
                        angular.forEach(payment.appliedPayments, function (appliedPayment) {
                            var tempAppliedPayment = appliedPayment;
                            payment.appliedPayments = [];
                            tempAppliedPayment.payment = payment;
                            appliedPayments.push(tempAppliedPayment);
                        });
                    });

                    deferred.resolve(appliedPayments);
                }, function (reason) {
                    deferred.reject(reason);
                });

                return deferred.promise;
            },

            authorizePayment: function (paymentRequest) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'AuthorizePayment'),
                        paymentRequest
                    ),
                    'Failed to authorize payment');
            },

            capturePayment: function (paymentRequest) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'CapturePayment'),
                        paymentRequest
                    ),
                    'Failed to capture payment');
            },

            authorizeCapturePayment: function (paymentRequest) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'AuthorizeCapturePayment'),
                        paymentRequest
                    ),
                    'Failed to authorize capture payment');
            },

            refundPayment: function (paymentRequest) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloPaymentApiBaseUrl', 'RefundPayment'),
                        paymentRequest
                    ),
                    'Failed to refund payment');
            }
        };
    }]);

    /**
     * @ngdoc service
     * @name merchello.resources.settingsResource
     * @description Loads in data and allows modification for invoices
     **/
    angular.module('merchello.resources').factory('settingsResource',
        ['$q', '$http', '$cacheFactory', 'umbRequestHelper',
            function($q, $http, $cacheFactory, umbRequestHelper) {

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
                    deferred.resolve(data[0]);
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
                    var settingsFromServer = data[0];
                    var currenciesFromServer = data[1];

                    var currencyList =  data[1];

                    var selectedCurrency = _.find(currencyList, function (currency) {
                        return currency.currencyCode === settingsFromServer.currencyCode;
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
            }

        };

        return settingsServices;

    }]);

    /**
     * @ngdoc service
     * @name merchello.resources.shipmentResource
     * @description Loads in data and allows modification for shipments
     **/
    angular.module('merchello.resources').factory('shipmentResource',
        ['$http', 'umbRequestHelper', function($http, umbRequestHelper) {
        return {

            getAllShipmentStatuses: function() {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'GetAllShipmentStatuses'),
                        method: 'GET'}),
                    'Failed to get shipment statuses');

            },

            getShipment: function (key) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'GetShipment'),
                        method: "GET",
                        params: { id: key }
                    }),
                    'Failed to get shipment: ' + key);
            },

           getShipmentsByInvoice: function (invoice) {
                var shipmentKeys = [];

                angular.forEach(invoice.orders, function(order) {
                    var newShipmentKeys = _.map(order.items, function(orderLineItem) {
                        return orderLineItem.shipmentKey;
                    });
                    shipmentKeys = _.union(shipmentKeys, newShipmentKeys);
                });


                var shipmentKeysStr = shipmentKeys.join("&ids=");

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'GetShipments', shipmentKeysStr),
                        method: "GET",
                        params: { ids: shipmentKeys }
                    }),
                    'Failed to get shipments: ' + shipmentKeys);
            },


            getShipMethod: function (order) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'GetShipMethod'),
                        order
                    ),
                    'Failed to get shipment method');
            },

            newShipment: function (order) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'NewShipment'),
                        order
                    ),
                    'Failed to create shipment');
            },

            putShipment: function (shipment, order) {
                var shipmentOrder = {}
                shipmentOrder.ShipmentDisplay = shipment;
                shipmentOrder.OrderDisplay = order;

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'PutShipment'),
                        shipmentOrder
                    ),
                    'Failed to save shipment');
            }

        };
    }]);

})();