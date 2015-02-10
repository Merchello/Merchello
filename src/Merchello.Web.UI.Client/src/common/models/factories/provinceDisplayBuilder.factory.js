    /**
     * @ngdoc service
     * @name merchello.models.provinceDisplayBuilder
     *
     * @description
     * A utility service that builds ProvinceDisplay models
     */
    angular.module('merchello.models')
        .factory('provinceDisplayBuilder',
        ['genericModelBuilder', 'ProvinceDisplay',
            function(genericModelBuilder, ProvinceDisplay) {
                var Constructor = ProvinceDisplay;
                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);
