(function (merchelloServices, undefined) {


    /**
    * @ngdoc service
    * @name merchello.Services.MerchelloInvoiceService
    * @description Loads in data and allows modification for invoices
    **/
    merchelloServices.MerchelloInvoiceService = function ($http, umbRequestHelper) {

        return {

            deleteInvoice: function (invoiceKey) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'DeleteInvoice'),
                        method: "GET",
                        params: { id: invoiceKey }
                    }),
                    'Failed to delete invoice');
            },
            getByCustomerKey: function (customerKey) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'GetByCustomerKey'),
                        method: "GET",
                        params: {id: customerKey }
                    }),
                    'Failed to get invoices for customer ' + customerKey);
            },
            getByKey: function (id) {
                return umbRequestHelper.resourcePromise(
                   $http({
                       url: umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'GetInvoice'),
                       method: "GET",
                       params: { id: id }
                   }),
                   'Failed to retreive data for invoice id: ' + id);
            },
            saveInvoice: function (invoice) {
                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'PutInvoice'),
                        invoice
                    ),
                    'Failed to save invoice');
            },
            searchInvoices: function (query) {
                var listQuery;
                if (query === undefined) {
                    query = new merchello.Models.ListQuery({
                        currentPage: 0,
                        itemsPerPage: 100
                    });
                }
                listQuery = new merchello.Models.ListQuery(query);
                return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'SearchInvoices'), listQuery),
                        'Failed to retreive invoices');
            }
        };
    };

    angular.module('umbraco.resources').factory('merchelloInvoiceService', ['$http', 'umbRequestHelper', merchello.Services.MerchelloInvoiceService]);

}(window.merchello.Services = window.merchello.Services || {}));
