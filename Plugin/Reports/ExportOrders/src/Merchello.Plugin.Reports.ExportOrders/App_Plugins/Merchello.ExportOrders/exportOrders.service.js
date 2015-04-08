(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name merchello.Services.ReportPluginExportOrders
        * @description Loads in data and allows modification for orders
        **/
    merchelloServices.ReportPluginExportOrders = function ($http, umbRequestHelper) {

        return {

            getAllOrders: function () {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloReportExportOrders', 'GetOrderReportData'),
                        method: "GET"
                    }),
                    'Failed to get orders');
            },

            /**
             * @ngdoc method
             * @name getOrdersByDateRange
             * @description
             **/
            getOrdersByDateRange: function (query) {
                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloReportExportOrders', 'GetOrderReportData'), query),
                    'Failed to retreive report data from the service');
            }
        };
    };

    angular.module('umbraco.resources').factory('merchelloPluginReportOrderExportService', ['$http', 'umbRequestHelper', merchello.Services.ReportPluginExportOrders]);

}(window.merchello.Services = window.merchello.Services || {}));
