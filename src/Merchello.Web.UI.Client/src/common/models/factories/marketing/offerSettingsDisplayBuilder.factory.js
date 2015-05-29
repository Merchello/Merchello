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

        function formatDateString(val) {
            var raw = new Date(val.split('T')[0]);
            return new Date(raw.getTime() + raw.getTimezoneOffset()*60000);
        }

        return {
            createDefault: function() {
                return new Constructor();
            },
            transform: function(jsonResult) {
                var settings = [];
                if(angular.isArray(jsonResult)) {
                    angular.forEach(jsonResult, function(json) {
                        var setting = genericModelBuilder.transform(json, Constructor);
                        setting.offerStartsDate = formatDateString(setting.offerStartsDate);
                        setting.offerEndsDate = formatDateString(setting.offerEndsDate);
                        setting.componentDefinitions = offerComponentDefinitionDisplayBuilder.transform(json.componentDefinitions);
                        settings.push(setting);
                    });
                } else {
                    settings = genericModelBuilder.transform(jsonResult,Constructor);
                    settings.offerStartsDate = formatDateString(settings.offerStartsDate);
                    settings.offerEndsDate = formatDateString(settings.offerEndsDate);
                    settings.componentDefinitions = offerComponentDefinitionDisplayBuilder.transform(jsonResult.componentDefinitions);
                }
                return settings;
            }
        };
    }]);