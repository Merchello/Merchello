    /**
     * @ngdoc service
     * @name taxCountryDisplayBuilder
     *
     * @description
     * A utility service that builds TaxCountryDisplay models
     */
    angular.module('merchello.models').factory('taxCountryDisplayBuilder', [
        'genericModelBuilder', 'gatewayResourceDisplayBuilder', 'TaxCountryDisplay',
        function(genericModelBuilder, gatewayResourceDisplayBuilder, TaxCountryDisplay) {

            var Constructor = TaxCountryDisplay;

            function buildSingle(resource) {
                var taxCountry = new Constructor();
                taxCountry.name = resource.name;
                taxCountry.gatewayResource = resource;
                return taxCountry;
            }

            return {
                createDefault: function() {
                    return buildSingle(gatewayResourceDisplayBuilder.createDefault());
                },
                transform: function(jsonResult) {
                    var resources = gatewayResourceDisplayBuilder.transform(jsonResult);
                    var countries = [];
                    if(angular.isArray(resources)) {
                        angular.forEach(resources, function(resource) {
                            countries.push(buildSingle(resource));
                        });
                    } else {
                        countries = buildSingle(resources);
                    }
                    return countries;
                }
            };
        }]);
