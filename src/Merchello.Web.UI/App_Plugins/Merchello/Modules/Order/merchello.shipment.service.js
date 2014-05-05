(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name merchello.Services.MerchelloShipmentService
        * @description Loads in data and allows modification for shipments
        **/
    merchelloServices.MerchelloShipmentService = function ($q, $http, umbRequestHelper) {

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

            getShipmentsByInvoice: function (invoice) {
	            var shipmentKeys = [];
	            var orders = _.map(invoice.orders, function(order) {
		            return new merchello.Models.Order(order);
	            });

	            angular.forEach(orders, function(order) {
		            var newShipmentKeys = _.map(order.items, function(orderlineitem) {
		            	var oli = new merchello.Models.OrderLineItem(orderlineitem);
			            return oli.shipmentKey;
		            });


		            shipmentKeys = _.union(shipmentKeys, newShipmentKeys);
	            });

	            //shipmentKeys = _.map(shipmentKeys, function(shipmentKey) {
		        //    return "ids=" + shipmentKey;
	            //});
	            var shipmentKeysStr = shipmentKeys.join("&ids=");

	            return umbRequestHelper.resourcePromise(
	                $http({
	                	url: umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'GetShipments', shipmentKeysStr),
	                	method: "GET",
	                	params: { ids: shipmentKeys }
	                }),
	                'Failed to get shipments: ' + shipmentKeys);
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

    angular.module('umbraco.resources').factory('merchelloShipmentService', ['$q', '$http', 'umbRequestHelper', merchello.Services.MerchelloShipmentService]);

}(window.merchello.Services = window.merchello.Services || {}));
