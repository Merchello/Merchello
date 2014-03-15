(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name merchello.Services.MerchelloInvoiceService
        * @description Loads in data and allows modification for invoices
        **/
    merchelloServices.MerchelloInvoiceService = function ($http, umbRequestHelper) {

        return {

            addInvoice: function (invoice) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'AddInvoice'),
                        invoice
                    ),
                    'Failed to create invoice');
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

    angular.module('umbraco.resources').factory('merchelloInvoiceService', merchello.Services.MerchelloInvoiceService);

}(window.merchello.Services = window.merchello.Services || {}));
