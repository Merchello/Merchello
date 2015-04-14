    /**
     * @ngdoc model
     * @name CampaignSettingsDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's CampaignSettingsDisplay object
     */
    var CampaignSettingsDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.alias = '';
        self.description = '';
        self.active = true;
        self.activitySettings = [];
    };

    CampaignSettingsDisplay.prototype = (function() {

        function clone() {
            var settings = this.activitySettings;
            var campaign = angular.extend(new CampaignSettingsDisplay(), this);
            campaign.activitySettings = [];
            angular.forEach(settings, function(act) {
              campaign.activitySettings.push(act);
            });
            return campaign;
        }

        return {
            clone: clone
        }

    }());

    angular.module('merchello.models').constant('CampaignSettingsDisplay', CampaignSettingsDisplay);