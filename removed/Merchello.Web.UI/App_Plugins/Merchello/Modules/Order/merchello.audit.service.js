(function (merchelloServices, undefined) {

    /**
    * @ngdoc service
    * @name merchello.Services.MerchelloAuditService
    * @description Loads in audit logs via API calls.
    **/
    merchelloServices.MerchelloAuditService = function ($http, umbRequestHelper) {

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
    };

    angular.module('umbraco.resources').factory('merchelloAuditService', ['$http', 'umbRequestHelper', merchello.Services.MerchelloAuditService]);

}(window.merchello.Services = window.merchello.Services || {}));
