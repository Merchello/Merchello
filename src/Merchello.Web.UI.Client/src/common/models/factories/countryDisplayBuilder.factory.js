    /**
     * @ngdoc service
     * @name merchello.models.countryDisplayBuilder
     *
     * @description
     * A utility service that builds CountryDisplay models
     */
    angular.module('merchello.models')
        .factory('countryDisplayBuilder',
        ['genericModelBuilder', 'provinceDisplayBuilder', 'CountryDisplay',
            function(genericModelBuilder, provinceDisplayBuilder, CountryDisplay) {

                var Constructor = CountryDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        var countries = genericModelBuilder.transform(jsonResult, Constructor);
                        for(var i = 0; i < countries.length; i++) {
                            countries[i].provinces = provinceDisplayBuilder.transform(jsonResult[ i ].provinces);
                        }
                        return countries;
                    }
                };
        }]);
