angular.module('merchello.mocks').
    factory('shipmentMocks', [
        function () {
            'use strict';

            var Constructor = Merchello.Models.Shipment;

            // Private
            function getEmptyShipment(modelTransformer, addressMocks) {
                var shipment = new Constructor();
                shipment.setDestinationAddress(addressMocks.getRandomAddress(modelTransformer));
                shipment.setOriginAddress(addressMocks.getRandomAddress(modelTransformer));
                return shipment;
            }

            // Public
            return {
                getEmptyShipment: getEmptyShipment
            };
        }]);

