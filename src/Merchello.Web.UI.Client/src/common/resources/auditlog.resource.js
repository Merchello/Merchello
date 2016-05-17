    /**
     * @ngdoc resource
     * @name auditLogResource
     * @description Loads in data and allows modification of audit logs
     **/
    angular.module('merchello.resources').factory('auditLogResource', [
        '$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {
        return {
            /**
             * @ngdoc method
             * @name getSalesHistoryByInvoiceKey
             * @description
             **/
            getByEntityKey: function(key) {
                var url = Umbraco.Sys.ServerVariables["merchelloUrls"]["merchelloAuditLogApiBaseUrl"] + 'GetByEntityKey';
                return umbRequestHelper.resourcePromise(
                $http({
                    url: url,
                    method: "GET",
                    params: { id: key }
                }),
                'Failed to audit logs for entity with following key: ' + key);
            },

            /**
             * @ngdoc method
             * @name getSalesHistoryByInvoiceKey
             * @description
             **/
            getSalesHistoryByInvoiceKey: function (key) {
                var url = Umbraco.Sys.ServerVariables["merchelloUrls"]["merchelloAuditLogApiBaseUrl"] + 'GetSalesHistoryByInvoiceKey';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: { id: key }
                    }),
                    'Failed to retreive sales history log for invoice with following key: ' + key);
            }
        };
    }]);
