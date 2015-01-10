    /**
     * @ngdoc service
     * @name merchello.models.shipMethodDisplayBuilder
     *
     * @description
     * A utility service that builds ShipMethodDisplay models
     */
    angular.module('merchello.services')
        .factory('shipMethodDisplayBuilder',
            ['genericModelBuilder', 'shipProvinceDisplayBuilder', 'ShipMethodDisplay',
            function(genericModelBuilder, shipProvinceDisplayBuilder, ShipMethodDisplay) {

                var Constructor = ShipMethodDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var shipMethod = genericModelBuilder.transform(jsonResult, Constructor);
                        if (jsonResult.provinces) {
                            shipMethod.provinces = shipProvinceDisplayBuilder.transform(jsonResult.provinces);
                        }
                        return shipMethod;
                    }
                };

        }]);