    /**
     * @ngdoc service
     * @name merchello.models.shipProvinceDisplayBuilder
     *
     * @description
     * A utility service that builds ShipProvinceDisplay models
     */
    angular.module('merchello.services').factory('shipProvinceDisplayBuilder',
        ['genericModelBuilder', 'ShipProvinceDisplay', function(genericModelBuilder, ShipProvinceDisplay) {

            var Constructor = ShipProvinceDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
    }]);