    /**
     * @ngdoc service
     * @name merchello.services.currencyDisplayBuilder
     *
     * @description
     * A utility service that builds CurrencyDisplay models
     */
    angular.module('merchello.models')
        .factory('currencyDisplayBuilder',
        ['genericModelBuilder',
            function(genericModelBuilder) {

                var Constructor = CurrencyDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);
