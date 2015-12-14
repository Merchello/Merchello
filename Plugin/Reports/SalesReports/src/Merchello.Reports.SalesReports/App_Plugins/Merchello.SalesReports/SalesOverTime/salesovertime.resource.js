angular.module('merchello.salesreports').factory('salesOverTimeResource',
    ['$http', 'umbRequestHelper',
    function($http, umbRequestHelper) {

        var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloReportSalesOverTime'];

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
                console.info(query);
                var url = baseUrl + 'SearchByDateRange';
                return umbRequestHelper.resourcePromise(
                    $http.post(url, query),
                    'Failed to retreive report data from the service');
            }
        };

}]);
