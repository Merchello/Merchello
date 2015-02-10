/**
 * @ngdoc service
 * @name shipRateTierDisplayBuilder
 *
 * @description
 * A utility service that builds ShipRateTierDisplay models
 */
angular.module('merchello.models').factory('shipRateTierDisplayBuilder',
    ['genericModelBuilder', 'ShipRateTierDisplay',
        function(genericModelBuilder, ShipRateTierDisplay) {
            var Constructor = ShipRateTierDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);
