(function (merchelloServices, undefined) {


    /**
    * @ngdoc service
    * @name merchello.Services.MerchelloInvoiceService
    * @description Loads in data and allows modification for invoices
    **/
    merchelloServices.MerchelloInvoiceService = function ($http, umbRequestHelper) {

        return {

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
            },

            /**
            * @ngdoc method
            * @name getByCustomerKey
            * @description 
            **/
            getByCustomerKey: function(customerKey) {

                var listQuery;
                
                var query = new merchello.Models.ListQuery({
                        currentPage: 0,
                        itemsPerPage: 100,
                        sortBy: '',
                        sortDirection: ''
                    });
                
                listQuery = new merchello.Models.ListQuery(query);
                listQuery.parameters.push(new merchello.Models.ListQueryParameter({ fieldName: 'customerKey', value: customerKey }));
                return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'SearchByCustomer'), listQuery),
                        'Failed to retreive invoices');
            },

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
            * @name searchInvoices
            * @description 
            **/
            searchInvoices: function (query) {
                var listQuery;
                if (query === undefined) {
                    query = new merchello.Models.ListQuery({
                        currentPage: 0,
                        itemsPerPage: 100,
                        sortBy: '',
                        sortDirection: ''
                    });
                }
                listQuery = new merchello.Models.ListQuery(query);
                return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'SearchInvoices'), listQuery),
                        'Failed to retreive invoices');
            },

            /**
            * @ngdoc method
            * @name searchInvoicesByDateRange
            * @description 
            **/
            searchInvoicesByDateRange: function (query) {
                var listQuery;
                if (query === undefined) {
                    query = new merchello.Models.ListQuery({
                        currentPage: 0,
                        itemsPerPage: 100,
                        sortBy: '',
                        sortDirection: ''
                    });
                }
                listQuery = new merchello.Models.ListQuery(query);
                return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'SearchByDateRange'), listQuery),
                        'Failed to retreive invoices');
            }
        };
    };

    angular.module('umbraco.resources').factory('merchelloInvoiceService', ['$http', 'umbRequestHelper', merchello.Services.MerchelloInvoiceService]);

}(window.merchello.Services = window.merchello.Services || {}));
