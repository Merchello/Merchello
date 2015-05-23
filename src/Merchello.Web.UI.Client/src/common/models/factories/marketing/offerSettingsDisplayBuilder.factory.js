/**
 * @ngdoc service
 * @name merchello.models.offerSettingsDisplayBuilder
 *
 * @description
 * A utility service that builds OfferSettingsDisplay models
 */
angular.module('merchello.models').factory('offerSettingsDisplayBuilder',
    ['genericModelBuilder', 'offerComponentDefinitionDisplayBuilder', 'OfferSettingsDisplay',
    function(genericModelBuilder, offerComponentDefinitionDisplayBuilder, OfferSettingsDisplay) {
        var Constructor = OfferSettingsDisplay;
        return {
            createDefault: function() {
                return new Constructor();
            },
            transform: function(jsonResult) {
                var settings = [];
                if(angular.isArray(jsonResult)) {
                    angular.forEach(jsonResult, function(json) {
                        var setting = genericModelBuilder.transform(json, Constructor);
                        setting.componentDefinitions = offerComponentDefinitionDisplayBuilder.transform(json.componentDefinitions);
                        settings.push(setting);
                    });
                } else {
                    settings = genericModelBuilder.transform(jsonResult,Constructor);
                    settings.componentDefinitions = offerComponentDefinitionDisplayBuilder.transform(jsonResult.componentDefinitions);
                }
                return settings;
            }
        };
    }]);