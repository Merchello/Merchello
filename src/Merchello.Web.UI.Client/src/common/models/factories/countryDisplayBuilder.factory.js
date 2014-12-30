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
                        var country = genericModelBuilder.transform(jsonResult, Constructor);
                        country.provinces = _.map(jsonResult.provinces, function(province) {
                            return provinceDisplayBuilder.transform(province);
                        });
                        return country;
                    }
                };

        }]);
