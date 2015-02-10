    /**
     * @ngdoc service
     * @name merchello.models.shipCountryDisplayBuilder
     *
     * @description
     * A utility service that builds ShipCountryDisplay models
     */
    angular.module('merchello.models')
        .factory('shipCountryDisplayBuilder',
        ['genericModelBuilder', 'shipProvinceDisplayBuilder', 'ShipCountryDisplay',
        function(genericModelBuilder, shipProvinceDisplayBuilder, ShipCountryDisplay) {
            var Constructor = ShipCountryDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    if(jsonResult === undefined || jsonResult === null) {
                        return;
                    }
                    var countries = genericModelBuilder.transform(jsonResult, Constructor);
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            countries[ i ].provinces = shipProvinceDisplayBuilder.transform(jsonResult[ i ].provinces);
                        }
                    } else {
                        countries.provinces = shipProvinceDisplayBuilder.transform(jsonResult.provinces);
                    }
                    return countries;
                }
            };
        }]);
