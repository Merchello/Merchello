    /**
     * @ngdoc service
     * @name merchello.models.orderStatusDisplayBuilder
     *
     * @description
     * A utility service that builds OrderStatusDisplay models
     */
    angular.module('merchello.models')
        .factory('orderStatusDisplayBuilder',
        ['genericModelBuilder', 'OrderStatusDisplay',
            function(genericModelBuilder, OrderStatusDisplay) {

                var Constructor = OrderStatusDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);
