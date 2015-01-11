    /**
     * @ngdoc service
     * @name merchello.resources.shipmentResource
     * @description Loads in data and allows modification for shipments
     **/
    angular.module('merchello.resources').factory('shipmentResource',
        ['$http', 'umbRequestHelper', function($http, umbRequestHelper) {
        return {

            getAllShipmentStatuses: function() {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'GetAllShipmentStatuses'),
                        method: 'GET'}),
                    'Failed to get shipment statuses');

            },

            getShipment: function (key) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'GetShipment'),
                        method: "GET",
                        params: { id: key }
                    }),
                    'Failed to get shipment: ' + key);
            },

           getShipmentsByInvoice: function (invoice) {
                var shipmentKeys = [];

                angular.forEach(invoice.orders, function(order) {
                    var newShipmentKeys = _.map(order.items, function(orderLineItem) {
                        return orderLineItem.shipmentKey;
                    });
                    shipmentKeys = _.union(shipmentKeys, newShipmentKeys);
                });


                var shipmentKeysStr = shipmentKeys.join("&ids=");

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'GetShipments', shipmentKeysStr),
                        method: "GET",
                        params: { ids: shipmentKeys }
                    }),
                    'Failed to get shipments: ' + shipmentKeys);
            },


            getShipMethodAndAlternatives: function (shipMethodKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'GetShipMethodAndAlternatives'),
                        method: "GET",
                        params: { key: shipMethodKey }
                    }),
                    'Failed to get shipment method');
            },

            newShipment: function (shipmentRequest) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'NewShipment'),
                        shipmentRequest
                    ),
                    'Failed to create shipment');
            },

            putShipment: function (shipment, order) {
                var shipmentOrder = {};
                shipmentOrder.ShipmentDisplay = shipment;
                shipmentOrder.OrderDisplay = order;

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'PutShipment'),
                        shipmentOrder
                    ),
                    'Failed to save shipment');
            }
        };
    }]);