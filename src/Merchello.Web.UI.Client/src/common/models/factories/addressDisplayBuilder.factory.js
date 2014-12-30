    /**
     * @ngdoc service
     * @name merchello.services.addressDisplayBuilder
     *
     * @description
     * A utility service that builds AddressDisplay models
     */
    angular.module('merchello.models')
        .factory('addressDisplayBuilder',
            ['genericModelBuilder',
                function(genericModelBuilder) {

                var Constructor = AddressDisplay;

                return {
                    getConstructor: function () {
                        return Constructor;
                    },
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
        }]);
