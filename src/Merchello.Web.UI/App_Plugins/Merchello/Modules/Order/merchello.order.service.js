(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name merchello.Services.MerchelloOrderService
        * @description Loads in data and allows modification for orders
        **/
    merchelloServices.MerchelloOrderService = function ($http, umbRequestHelper) {

        return {

            getOrder: function (orderKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetOrder'),
                        method: "GET",
                        params: { id: orderKey }
                    }),
                    'Failed to get order: ' + orderKey);
            },

            getOrdersByInvoice: function (invoiceKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetOrdersByInvoiceKey'),
                        method: "GET",
                        params: { id: invoiceKey }
                    }),
                    'Failed to get orders by invoice: ' + invoiceKey);
            },

            getUnFulfilledItems: function (invoiceKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetUnFulfilledItems'),
                        method: "GET",
                        params: { id: invoiceKey }
                    }),
                    'Failed to get unfulfilled items by invoice: ' + invoiceKey);
            },

            getShippingAddress: function (invoiceKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetShippingAddress'),
                        method: "GET",
                        params: { id: invoiceKey }
                    }),
                    'Failed to get orders by invoice: ' + invoiceKey);
            }

        };
    };

    angular.module('umbraco.resources').factory('merchelloOrderService', ['$http', 'umbRequestHelper', merchello.Services.MerchelloOrderService]);

}(window.merchello.Services = window.merchello.Services || {}));
