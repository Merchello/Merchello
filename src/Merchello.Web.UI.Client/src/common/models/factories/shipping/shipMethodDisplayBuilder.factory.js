    /**
     * @ngdoc service
     * @name merchello.models.shipMethodDisplayBuilder
     *
     * @description
     * A utility service that builds ShipMethodDisplay models
     */
    angular.module('merchello.models')
        .factory('shipMethodDisplayBuilder',
            ['genericModelBuilder', 'dialogEditorViewDisplayBuilder', 'shipProvinceDisplayBuilder', 'ShipMethodDisplay',
            function(genericModelBuilder, dialogEditorViewDisplayBuilder, shipProvinceDisplayBuilder, ShipMethodDisplay) {

                var Constructor = ShipMethodDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var shipMethods = genericModelBuilder.transform(jsonResult, Constructor);
                        if(!jsonResult) {
                            return;
                        }
                        if (angular.isArray(jsonResult))
                        {
                            for(var i = 0; i < jsonResult.length; i++) {
                                // todo these should never be returned by the api
                                if(jsonResult[i] !== null) {
                                    shipMethods[ i ].provinces = shipProvinceDisplayBuilder.transform(jsonResult[ i ].provinces);
                                }
                            }
                        } else {
                            if(jsonResult.provinces) {
                                shipMethods.provinces = shipProvinceDisplayBuilder.transform(jsonResult.provinces);
                            }
                        }
                        return shipMethods;
                    }
                };

        }]);