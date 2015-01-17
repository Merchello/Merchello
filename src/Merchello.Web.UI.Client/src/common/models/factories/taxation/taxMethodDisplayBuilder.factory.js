    /**
     * @ngdoc service
     * @name taxMethodDisplayBuilder
     *
     * @description
     * A utility service that builds TaxMethodDisplay models
     */
    angular.module('merchello.models').factory('taxMethodDisplayBuilder',
        ['genericModelBuilder', 'TaxMethodDisplay',
            function(genericModelBuilder, TaxMethodDisplay) {

                var Constructor = TaxMethodDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
    }]);
