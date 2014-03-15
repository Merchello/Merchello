(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name merchello.Services.MerchelloOrderService
        * @description Loads in data and allows modification for orders
        **/
    merchelloServices.MerchelloOrderService = function ($http, umbRequestHelper) {

        return {

            addOrder: function (order) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'AddOrder'),
                        order
                    ),
                    'Failed to create order');
            },

            saveOrder: function (order) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'PutOrder'),
                        order
                    ),
                    'Failed to save order');
            },

            deleteOrder: function (orderKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'DeleteOrder'),
                        method: "GET",
                        params: { id: orderKey }
                    }),
                    'Failed to delete order');
            },

        };
    };

    angular.module('umbraco.resources').factory('merchelloOrderService', merchello.Services.MerchelloOrderService);

}(window.merchello.Services = window.merchello.Services || {}));
