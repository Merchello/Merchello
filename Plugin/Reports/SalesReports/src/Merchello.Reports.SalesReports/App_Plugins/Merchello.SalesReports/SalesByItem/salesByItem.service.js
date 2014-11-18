(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name merchello.Services.ReportPluginExportOrders
        * @description Loads in data and allows modification for orders
        **/
    merchelloServices.ReportPluginSalesByItem = function ($http, umbRequestHelper) {

        return {

            getAllOrders: function () {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloReportSalesByItem', 'GetDefaultReportData'),
                        method: "GET"
                    }),
                    'Failed to get invoices');
            }
        };
    };

    angular.module('umbraco.resources').factory('merchelloPluginReportSalesByItemService', ['$http', 'umbRequestHelper', merchello.Services.ReportPluginSalesByItem]);

}(window.merchello.Services = window.merchello.Services || {}));