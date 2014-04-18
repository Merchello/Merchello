(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name merchello.Services.MerchelloShipmentService
        * @description Loads in data and allows modification for shipments
        **/
    merchelloServices.MerchelloShipmentService = function ($http, umbRequestHelper) {

        return {

            getShipment: function (key) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'GetShipment'),
                        method: "GET",
                        params: { id: key }
                    }),
                    'Failed to get shipment: ' + key);
            },

            getShipMethod: function (order) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'GetShipMethod'),
                        order
                    ),
                    'Failed to get shipment method');
            },

            newShipment: function (order) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'NewShipment'),
                        order
                    ),
                    'Failed to create shipment');
            },

            putShipment: function (shipment) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'PutShipment'),
                        shipment
                    ),
                    'Failed to save shipment');
            }

        };
    };

    angular.module('umbraco.resources').factory('merchelloShipmentService', ['$http', 'umbRequestHelper', merchello.Services.MerchelloShipmentService]);

}(window.merchello.Services = window.merchello.Services || {}));
