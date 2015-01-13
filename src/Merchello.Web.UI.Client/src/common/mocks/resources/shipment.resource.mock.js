angular.module('merchello.mocks')
    .factory('shipmentResourceMock', [
        '$httpBackend', 'mocksUtils', 'shipmentMocks',
        function($httpBackend, mocksUtils, shipmentMocks) {

            function getShipment()
            {
                return shipmentMocks.shipmentsArray()[0];
            }


            function shipmentStatues() {
                return shipmentMocks.shipmentStatuses();
            }

            function getShipments() {
                return shipmentMocks.shipmentsArray();
            }


            function getShipMethodAndAlternatives() {
                return {
                    selected: { },
                    alternatives: []
                };
            }

            return {
                register: function() {

                    $httpBackend
                        .whenGET(mocksUtils.urlRegex('/umbraco/backoffice/Merchello/ShipmentApi/GetShipment'))
                        .respond(getShipment);

                    $httpBackend
                        .whenGET(mocksUtils.urlRegex('/umbraco/backoffice/Merchello/ShipmentApi/GetShipments'))
                        .respond(getShipments);

                    $httpBackend
                        .whenGET(mocksUtils.urlRegex('/umbraco/backoffice/Merchello/ShipmentApi/GetAllShipmentStatuses'))
                        .respond(shipmentStatues);
                }
            };
        }]);
