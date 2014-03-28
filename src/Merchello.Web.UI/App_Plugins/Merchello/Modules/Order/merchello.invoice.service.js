(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name merchello.Services.MerchelloInvoiceService
        * @description Loads in data and allows modification for invoices
        **/
    merchelloServices.MerchelloInvoiceService = function ($http, umbRequestHelper) {

        return {

            getByKey: function (id) {

                return umbRequestHelper.resourcePromise(
                   $http({
                       url: umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'GetInvoice'),
                       method: "GET",
                       params: { id: id }
                   }),
                   'Failed to retreive data for invoice id: ' + id);
            },

            getAll: function () {

                return umbRequestHelper.resourcePromise(
                   $http({
                       url: umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'GetAllInvoices'),
                       method: "GET"
                   }),
                   'Failed to retreive invoices');
            },

            getFiltered: function (term) {

                return umbRequestHelper.resourcePromise(
                   $http({
                       url: umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'GetFilteredInvoices'),
                       method: "GET",
                       params: { term: term }
                   }),
                   'Failed to retreive filtered invoices');
            },

            saveInvoice: function (invoice) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'PutInvoice'),
                        invoice
                    ),
                    'Failed to save invoice');
            },

            deleteInvoice: function (invoiceKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'DeleteInvoice'),
                        method: "GET",
                        params: { id: invoiceKey }
                    }),
                    'Failed to delete invoice');
            },

        };
    };

    angular.module('umbraco.resources').factory('merchelloInvoiceService', ['$http', 'umbRequestHelper', merchello.Services.MerchelloInvoiceService]);

}(window.merchello.Services = window.merchello.Services || {}));
