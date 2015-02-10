    /**
     * @ngdoc service
     * @name merchello.models.shipmentStatusDisplayBuilder
     *
     * @description
     * A utility service that builds ShipmentStatusDisplay models
     */
    angular.module('merchello.models')
    .factory('shipmentStatusDisplayBuilder',
    ['genericModelBuilder', 'ShipmentStatusDisplay',
        function(genericModelBuilder, ShipmentStatusDisplay) {

            var Constructor = ShipmentStatusDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);
