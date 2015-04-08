/**
 * @ngdoc factory
 * @name campaignActivitySettingsDisplayBuilder
 *
 * @description
 * A utility service that builds CampaignActivitySettingsDisplay models
 */
angular.module('merchello.models').factory('campaignActivitySettingsDisplayBuilder',
    ['genericModelBuilder', 'extendedDataDisplayBuilder', 'typeFieldDisplayBuilder', 'CampaignActivitySettingsDisplay',
    function(genericModelBuilder, extendedDataDisplayBuilder, typeFieldDisplayBuilder, CampaignActivitySettingsDisplay) {

        var Constructor = CampaignActivitySettingsDisplay;

        return {
            createDefault: function() {
                var activity = new Constructor();
                activity.extendedData = extendedDataDisplayBuilder.createDefault();
                activity.campaignActivityTypeField = typeFieldDisplayBuilder.createDefault();
                return activity;
            },

            transform: function(jsonResult) {
                var activities = [];
                if (angular.isArray(jsonResult)) {
                    for(var i = 0; i < jsonResult.length; i++) {
                        var activity = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                        activity.extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                        activity.campaignActivityTypeField = typeFieldDisplayBuilder.transform(jsonResult[ i ].campaignActivityTypeField);
                        activities.push(activity);
                    }
                } else {
                    activities = genericModelBuilder.transform(jsonResult, Constructor);
                    activities.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                    activities.campaignActivityTypeField = typeFieldDisplayBuilder.transform(jsonResult.campaignActivityTypeField);
                }
                return activities;
            }
        };

    }]);
