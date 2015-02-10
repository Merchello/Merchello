    /**
     * @ngdoc service
     * @name merchello.services.shipMethodsQueryDisplayBuilder
     *
     * @description
     * A utility service that builds ShipMethodsQueryDisplay models
     */
    angular.module('merchello.services')
        .factory('shipMethodsQueryDisplayBuilder',
        ['genericModelBuilder', 'shipMethodDisplayBuilder', 'ShipMethodsQueryDisplay',
        function(genericModelBuilder, shipMethodDisplayBuilder, ShipMethodsQueryDisplay) {

            var Constructor = ShipMethodsQueryDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var query = new Constructor();
                    if (jsonResult) {
                        query.selected = shipMethodDisplayBuilder.transform(jsonResult.selected);
                        query.alternatives = shipMethodDisplayBuilder.transform(jsonResult.alternatives);
                    }
                    return query;
                }
            };
        }]);