angular.module('merchello.salesreports').factory('salesByItemResource',
    ['$http', 'umbRequestHelper',
    function($http, umbRequestHelper) {

        var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloReportSalesByItem'];

        return {
            getDefaultData: function () {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: baseUrl + 'GetDefaultReportData',
                        method: "GET"
                    }),
                    'Failed to get default data from the service');
            },

            /**
             * @ngdoc method
             * @name searchInvoicesByDateRange
             * @description
             **/
            searchByDateRange: function (query) {

                var url = baseUrl + 'merchelloReportSalesByItem';

                return umbRequestHelper.resourcePromise(
                    $http.post(url, query),
                    'Failed to retreive report data from the service');
            }
        };
}]);
