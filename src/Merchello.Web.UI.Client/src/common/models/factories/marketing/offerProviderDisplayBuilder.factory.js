/**
 * @ngdoc service
 * @name offerProviderDisplayBuilder
 *
 * @description
 * A utility service that builds OfferProviderDisplay models
 */
angular.module('merchello.models').factory('offerProviderDisplayBuilder', [
    'genericModelBuilder', 'backOfficeTreeDisplayBuilder', 'OfferProviderDisplay',
    function(genericModelBuilder, backOfficeTreeDisplayBuilder, OfferProviderDisplay) {

        var Constructor = OfferProviderDisplay;

        return {
            createDefault: function () {
                var provider = new Constructor();
                provider.backOfficeTree = backOfficeTreeDisplayBuilder.createDefault();
                return provider;
            },
            transform: function (jsonResult) {
                var providers = genericModelBuilder.transform(jsonResult, Constructor);
                if (angular.isArray(providers)) {
                    for (var i = 0; i < providers.length; i++) {
                        providers[i].backOfficeTree = backOfficeTreeDisplayBuilder.transform(jsonResult[i].backOfficeTree);
                    }
                } else {
                    providers.backOfficeTree = backOfficeTreeDisplayBuilder.transform(jsonResult.backOfficeTree);
                }
                return providers;
            }
        };

    }]);