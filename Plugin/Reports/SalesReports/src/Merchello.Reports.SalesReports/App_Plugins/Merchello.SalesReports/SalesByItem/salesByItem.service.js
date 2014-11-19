(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name merchello.Services.ReportPluginExportOrders
        * @description Loads in data and allows modification for orders
        **/
    merchelloServices.ReportPluginSalesByItem = function ($http, umbRequestHelper) {

        return {

            getDefaultData: function () {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloReportSalesByItem', 'GetDefaultReportData'),
                        method: "GET"
                    }),
                    'Failed to get default data');
            }
        };
    };

    angular.module('umbraco.resources').factory('merchelloPluginReportSalesByItemService', ['$http', 'umbRequestHelper', merchello.Services.ReportPluginSalesByItem]);

}(window.merchello.Services = window.merchello.Services || {}));