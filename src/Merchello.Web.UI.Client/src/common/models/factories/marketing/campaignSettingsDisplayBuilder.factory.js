/**
 * @ngdoc factory
 * @name campaignSettingsDisplayBuilder
 *
 * @description
 * A utility service that builds CampaignSettingsDisplay models
 */
angular.module('merchello.models').factory('campaignSettingsDisplayBuilder',
    ['genericModelBuilder', 'campaignActivitySettingsDisplayBuilder', 'CampaignSettingsDisplay',
    function(genericModelBuilder, campaignActivitySettingsDisplayBuilder, CampaignSettingsDisplay) {

        var Constructor = CampaignSettingsDisplay;

        return {
            createDefault: function() {
                return new Constructor();
            },
            transform: function(jsonResult) {
                var settings = [];
                if (angular.isArray(jsonResult)) {
                    for(var i = 0; i < jsonResult.length; i++) {
                        var setting = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                        setting.activitySettings = campaignActivitySettingsDisplayBuilder.transform(jsonResult[ i ].activitySettings);
                        settings.push(setting);
                    }
                } else {
                    settings = genericModelBuilder.transform(jsonResult, Constructor);
                    settings.activitySettings = campaignActivitySettingsDisplayBuilder.transform(jsonResult.activitySettings);
                }
                return settings;
            }
        };

}]);
