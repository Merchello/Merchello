    /**
     * @ngdoc service
     * @name taxProvinceDisplayBuilder
     *
     * @description
     * A utility service that builds TaxProvinceDisplay models
     */
    angular.module('merchello.models').factory('taxProvinceDisplayBuilder',
        ['genericModelBuilder', 'TaxProvinceDisplay',
        function(genericModelBuilder, TaxProvinceDisplay) {

            var Constructor = TaxProvinceDisplay;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);
