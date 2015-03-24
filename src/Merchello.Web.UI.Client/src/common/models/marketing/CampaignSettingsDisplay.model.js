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

        return {

        }

    }());

    angular.module('merchello.models').constant('CampaignSettingsDisplay', CampaignSettingsDisplay);