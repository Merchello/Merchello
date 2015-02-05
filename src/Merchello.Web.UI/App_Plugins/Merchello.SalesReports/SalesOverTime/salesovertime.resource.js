angular.module('merchello.salesreports').factory('salesOverTimeResource',
    ['$http', 'umbRequestHelper',
    function($http, umbRequestHelper) {

        return {

            getDefaultData: function () {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloReportSalesOverTime', 'GetDefaultReportData'),
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
                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloReportSalesOverTime', 'SearchByDateRange'), query),
                    'Failed to retreive report data from the service');
            }
        };

}]);
