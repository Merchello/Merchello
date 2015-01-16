    /**
     * @ngdoc service
     * @name merchello.models.shipMethodDisplayBuilder
     *
     * @description
     * A utility service that builds ShipMethodDisplay models
     */
    angular.module('merchello.services')
        .factory('shipMethodDisplayBuilder',
            ['genericModelBuilder', 'dialogEditorViewDisplayBuilder', 'shipProvinceDisplayBuilder', 'ShipMethodDisplay',
            function(genericModelBuilder, dialogEditorViewDisplayBuilder, shipProvinceDisplayBuilder, ShipMethodDisplay) {

                var Constructor = ShipMethodDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        if(jsonResult === undefined) {
                            return;
                        }
                        var shipMethods = genericModelBuilder.transform(jsonResult, Constructor);
                        if (angular.isArray(jsonResult))
                        {
                            for(var i = 0; i < jsonResult.length; i++) {
                                shipMethods[ i ].provinces = shipProvinceDisplayBuilder.transform(jsonResult[ i ].provinces);
                            }
                        } else {
                            shipMethods.provinces = shipProvinceDisplayBuilder.transform(jsonResult.provinces);
                        }
                        return shipMethods;
                    }
                };

        }]);