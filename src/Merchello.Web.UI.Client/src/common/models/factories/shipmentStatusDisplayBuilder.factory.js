    /**
     * @ngdoc service
     * @name merchello.services.shipmentStatusDisplayBuilder
     *
     * @description
     * A utility service that builds ShipmentStatusDisplay models
     */
    angular.module('merchello.models')
    .factory('shipmentStatusDisplayBuilder',
    ['genericModelBuilder',
        function(genericModelBuilder) {

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
