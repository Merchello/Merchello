    /**
     * @ngdoc model
     * @name CampaignActivitySettingsDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's CampaignActivitySettingsDisplay object
     */
    var CampaignActivitySettingsDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.alias = '';
        self.description = '';
        self.active = true;
        self.campaignKey = '';
        self.campaignActivityTfKey = '';
        self.campaignActivityTypeField = {};
        self.campaignActivityType = '';
        self.startDate = '';
        self.endDate = '';
        self.extendedData = {};
    };

    angular.module('merchello.models').constant('CampaignActivitySettingsDisplay', CampaignActivitySettingsDisplay);