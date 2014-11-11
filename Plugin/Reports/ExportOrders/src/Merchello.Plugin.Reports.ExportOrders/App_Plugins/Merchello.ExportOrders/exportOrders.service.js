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
            }
        };
    };

    angular.module('umbraco.resources').factory('merchelloPluginReportOrderExportService', ['$http', 'umbRequestHelper', merchello.Services.ReportPluginExportOrders]);

}(window.merchello.Services = window.merchello.Services || {}));
