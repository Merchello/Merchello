    /**
     * @ngdoc resource
     * @name invoiceResource
     * @description Loads in data and allows modification for invoices
     **/
    angular.module('merchello.resources')
        .factory('invoiceResource', [
            '$q', '$http', 'umbRequestHelper', 'invoiceItemItemizationDisplayBuilder',
            function($q, $http, umbRequestHelper, invoiceItemItemizationDisplayBuilder) {

                var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloInvoiceApiBaseUrl'];

                return {

                    /**
                     * @ngdoc method
                     * @name getByKey
                     * @description
                     **/
                    getByKey: function (id) {
                        var url = baseUrl + 'GetInvoice';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url,
                                method: "GET",
                                params: { id: id }
                            }),
                            'Failed to retreive data for invoice id: ' + id);
                    },

                    /**
                     * @ngdoc method
                     * @name getItemItemization
                     * @description
                     **/
                    getItemItemization: function (id) {
                        var url = baseUrl + 'GetInvoiceItemItemization';

                        var deferred = $q.defer();
                        $q.all([
                            umbRequestHelper.resourcePromise(
                                $http({
                                    url: url,
                                    method: "GET",
                                    params: { id: id }
                                }),
                                'Failed to retreive data for invoice itemization for id: ' + id)
                        ]).then(function(data) {
                            var results = invoiceItemItemizationDisplayBuilder.transform(data[0]);
                            deferred.resolve(results);
                        });

                        return deferred.promise;
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

                    searchByCustomer : function(query) {
                        var url = baseUrl + 'SearchByCustomer';
                        return umbRequestHelper.resourcePromise(
                            $http.post(url, query),
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
                        var url = baseUrl + 'SearchInvoices';
                        return umbRequestHelper.resourcePromise(
                            $http.post(url, query),
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
                        var url = baseUrl + 'SearchByDateRange';
                        return umbRequestHelper.resourcePromise(
                            $http.post(url, query),
                            'Failed to retreive invoices');
                    },

                    /**
                     * @ngdoc method
                     * @name saveInvoice
                     * @description
                     **/
                    saveInvoice: function (invoice) {
                        var url = baseUrl + 'PutInvoice';
                        return umbRequestHelper.resourcePromise(
                            $http.post(url,
                                invoice
                            ),
                            'Failed to save invoice');
                    },

                    /**
                     * @ngdoc method
                     * @name saveInvoice
                     * @description
                     **/
                    putInvoiceNewProducts: function (invoiceAddItems) {
                        var url = baseUrl + 'PutInvoiceNewProducts';
                        return umbRequestHelper.resourcePromise(
                            $http.post(url,
                                invoiceAddItems
                            ),
                            'Failed to save invoice');
                    },

                    saveInvoiceAdjustments: function(adjustments) {
                        var url = baseUrl + 'PutInvoiceAdjustments';
                        return umbRequestHelper.resourcePromise(
                            $http.post(url,
                                adjustments
                            ),
                            'Failed to save invoice');
                    },

                    saveInvoiceShippingAddress: function (data) {
                        var url = baseUrl + 'PutInvoiceShippingAddress';
                        return umbRequestHelper.resourcePromise(
                            $http.post(url,
                                data
                            ),
                            'Failed to save invoice');
                    },

                    /**
                     * @ngdoc method
                     * @name deleteInvoice
                     * @description
                     **/
                    deleteInvoice: function (invoiceKey) {
                        var url = baseUrl + 'DeleteInvoice';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url,
                                method: "GET",
                                params: { id: invoiceKey }
                            }),
                            'Failed to delete invoice');
                    },

                    deleteDiscount: function (invoiceKey, discountSku) {
                        var url = baseUrl + 'DeleteDiscount';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url,
                                method: "GET",
                                params: { invoiceId: invoiceKey, discountSku: discountSku }
                            }),
                            'Failed to delete discount');
                    },

                    /**
                     * @ngdoc method
                     * @name cancelInvoice
                     * @description
                     **/
                    cancelInvoice: function (invoiceKey) {
                        var url = baseUrl + 'CancelInvoice';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url,
                                method: "GET",
                                params: { id: invoiceKey }
                            }),
                            'Failed to cancel invoice');
                    }
                };
            }]);
