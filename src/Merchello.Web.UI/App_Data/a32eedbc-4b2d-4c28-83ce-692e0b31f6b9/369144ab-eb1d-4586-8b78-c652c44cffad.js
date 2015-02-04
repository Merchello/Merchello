angular.module('merchello.salespreports').factory('salesByItemResource',
    ['$http', 'umbRequestHelper',
    function($http, umbRequestHelper) {

        return {
            getDefaultData: function () {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloReportSalesByItem', 'GetDefaultReportData'),
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
                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloReportSalesByItem', 'SearchByDateRange'), query),
                    'Failed to retreive report data from the service');
            }
        };
}]);
