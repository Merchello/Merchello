    /**
     * @ngdoc resource
     * @name shipmentResource
     * @description Loads in data and allows modification for shipments
     **/
    angular.module('merchello.resources').factory('shipmentResource',
        ['$http', '$q', 'umbRequestHelper',
            function($http, $q, umbRequestHelper) {
        return {

            getAllShipmentStatuses: function () {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShipmentApiBaseUrl'] + 'GetAllShipmentStatuses';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: 'GET'
                    }),
                    'Failed to get shipment statuses');

            },

            getShipment: function (key) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShipmentApiBaseUrl'] + 'GetShipment';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: {id: key}
                    }),
                    'Failed to get shipment: ' + key);
            },

            getShipmentsByInvoice: function (invoice) {
                var shipmentKeys = [];

                angular.forEach(invoice.orders, function (order) {
                    var newShipmentKeys = _.map(order.items, function (orderLineItem) {
                        return orderLineItem.shipmentKey;
                    });
                    shipmentKeys = _.union(shipmentKeys, newShipmentKeys);
                });


                var shipmentKeysStr = shipmentKeys.join("&ids=");
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShipmentApiBaseUrl'] + 'GetShipments';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: {ids: shipmentKeys}
                    }),
                    'Failed to get shipments: ' + shipmentKeys);
            },

            getShipMethodAndAlternatives: function (shipMethodRequest) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShipmentApiBaseUrl'] + 'SearchShipMethodAndAlternatives';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        shipMethodRequest
                    ),
                    'Failed to get the ship methods');
            },

            newShipment: function (shipmentRequest) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShipmentApiBaseUrl'] + 'NewShipment';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        shipmentRequest
                    ),
                    'Failed to create shipment');
            },

            saveShipment: function (shipment) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShipmentApiBaseUrl'] + 'PutShipment';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        shipment
                    ),
                    'Failed to save shipment');
            },

            updateShippingAddressLineItem: function(shipment) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShipmentApiBaseUrl'] + 'UpdateShippingAddressLineItem';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        shipment
                    ),
                    'Failed to save shipment');
            },

            deleteShipment: function(shipment) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloShipmentApiBaseUrl'] + 'DeleteShipment';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: { id: shipment.key }
                    }), 'Failed to delete shipment');
            }

        };
    }]);