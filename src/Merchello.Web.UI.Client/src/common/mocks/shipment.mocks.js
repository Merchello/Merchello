angular.module('merchello.mocks').
    factory('shipmentMocks', ['shipmentDisplayBuilder',
        function (shipmentDisplayBuilder) {
            'use strict';

            var builder = shipmentDisplayBuilder;

            // Private
            function getEmptyShipment(modelTransformer, addressMocks) {
                var shipment = bulder.createDefault();
                shipment.setDestinationAddress(addressMocks.getRandomAddress(modelTransformer));
                shipment.setOriginAddress(addressMocks.getRandomAddress(modelTransformer));
                return shipment;
            }

            // Public
            return {
                getEmptyShipment: getEmptyShipment
            };
        }]);

