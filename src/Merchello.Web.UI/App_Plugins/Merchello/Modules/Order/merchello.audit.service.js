(function (merchelloServices, undefined) {

    /**
    * @ngdoc service
    * @name merchello.Services.MerchelloInvoiceService
    * @description Loads in data and allows modification for invoices
    **/
    merchelloServices.MerchelloAuditService = function ($http, umbRequestHelper) {

        return {

            /**
            * @ngdoc method
            * @name getByKey
            * @description 
            **/
            getSalesHistoryByInvoiceKey: function (key) {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloInvoiceApiBaseUrl', 'GetInvoice'),
                        method: "GET",
                        params: { id: key }
                    }),
                    'Failed to retreive data for invoice id: ' + id);
            }

        };
    };

    angular.module('umbraco.resources').factory('merchelloAuditService', ['$http', 'umbRequestHelper', merchello.Services.MerchelloAuditService]);

}(window.merchello.Services = window.merchello.Services || {}));
