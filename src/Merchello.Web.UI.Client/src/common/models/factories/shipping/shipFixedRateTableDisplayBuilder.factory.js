/**
 * @ngdoc service
 * @name shipFixedRateTableDisplayBuilder
 *
 * @description
 * A utility service that builds ShipFixedRateTableDisplay models
 */
angular.module('merchello.models').factory('shipFixedRateTableDisplayBuilder',
    ['genericModelBuilder', 'shipRateTierDisplayBuilder', 'ShipFixedRateTableDisplay',
    function(genericModelBuilder, shipRateTierDisplayBuilder, ShipFixedRateTableDisplay) {

        var Constructor = ShipFixedRateTableDisplay;

        return {
            createDefault: function() {
                return new Constructor();
            },
            transform: function(jsonResult) {
                var rateTable = genericModelBuilder.transform(jsonResult, Constructor);
                rateTable.rows = shipRateTierDisplayBuilder.transform(jsonResult.rows);
                return rateTable;
            }
        };
    }]);
