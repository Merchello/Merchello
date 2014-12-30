    /**
     * @ngdoc service
     * @name merchello.services.countryDisplayBuilder
     *
     * @description
     * A utility service that builds CountryDisplay models
     */
    angular.module('merchello.models')
        .factory('countryDisplayBuilder',
        ['genericModelBuilder', 'provinceDisplayBuilder',
            function(genericModelBuilder, provinceDisplayBuilder) {

                var Constructor = CountryDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var countries = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < countries.length; i++) {
                            country.provinces = provinceDisplayBuilder.transform(jsonResult[ i ].provinces);
                        }
                        return countries;
                    }
                };

        }]);
