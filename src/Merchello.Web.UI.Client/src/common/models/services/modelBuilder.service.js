    /**
     * @ngdoc service
     * @name merchello.services.modelBuilder
     * @function
     *
     * @description
     * A utility service that builds local models for API query results
     *  http://odetocode.com/blogs/scott/archive/2014/03/17/building-better-models-for-angularjs.aspx
     */
    angular.module('merchello.models')
        .factory('modelBuilder', ['modelTransformer',
            function(modelTransformer) {
                var transformer = modelTransformer,

                    buildAddress = function(result) {
                        return transformer.transform(result, Merchello.Models.Address);
                    },

                    buildCountry = function(result) {
                        return transformer.transform(result, Merchello.Models.Country);
                    },

                    buildCurrency = function(result) {
                        return transformer.transform(result, Merchello.Models.Currency);
                    },

                    buildProvince = function(result) {
                        return transformer.transform(result, Merchello.Models.Province);
                    },

                    buildSetting = function(result) {
                        return transformer.transform(result, Merchello.Models.Setting);
                    },

                    buildShipment = function(result) {
                        var shipment = transformer.transform(result, Merchello.Models.Shipment);
                        shipment.shipmentStatus = transformer.transform(result.shipmentStatus, Merchello.Models.ShipmentStatus);
                        shipment.items = $_.each(result.items, function(item) {
                          return transformer.transform(item, Merchello.Models.OrderLineItem);
                        });
                        return shipment;
                    };

            return {
                buildAddress: buildAddress,
                buildCountry: buildCountry,
                buildCurrency: buildCurrency,
                buildProvince: buildProvince,
                buildSetting: buildSetting,
                buildShipment: buildShipment
            };
        }]);
