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
            	//var deferred = $q.defer();
	            var shipments = [];
	            //var orders = _.map(invoice.orders, function(order) {
		        //    return new merchello.Models.Order(order);
	            //});

	            //angular.forEach(orders, function(order) {
		        //    var orderlineitems = _.map(order.items, function(orderlineitem) {
			    //        return new merchello.Models.OrderLineItem(orderlineitem);
		        //    });

				//	var shipment = _.map(orderlineitems)
	            //});

	            //deferred.resolve(selectedCurrency.symbol);

	            //return umbRequestHelper.resourcePromise(
	            //    $http({
	            //    	url: umbRequestHelper.getApiUrl('merchelloShipmentApiBaseUrl', 'GetShipmentsByInvoice'),
	            //    	method: "GET",
	            //    	params: { id: invoiceKey }
	            //    }),
	            //    'Failed to get shipments by invoice: ' + invoiceKey);
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
