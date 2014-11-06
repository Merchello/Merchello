(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name merchello.Services.ReportPluginExportOrders
        * @description Loads in data and allows modification for orders
        **/
    merchelloServices.ReportPluginExportOrders = function ($http, umbRequestHelper) {

        return {

            //getOrder: function (orderKey) {

            //    return umbRequestHelper.resourcePromise(
            //        $http({
            //            url: umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetOrder'),
            //            method: "GET",
            //            params: { id: orderKey }
            //        }),
            //        'Failed to get order: ' + orderKey);
            //},

        };
    };

    angular.module('umbraco.resources').factory('merchelloOrderService', ['$http', 'umbRequestHelper', merchello.Services.ReportPluginExportOrders]);

}(window.merchello.Services = window.merchello.Services || {}));
