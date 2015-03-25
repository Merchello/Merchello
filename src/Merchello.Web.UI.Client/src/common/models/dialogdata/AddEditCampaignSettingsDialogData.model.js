    /**
     * @ngdoc model
     * @name AddEditCampaignSettingsDialogData
     * @function
     *
     * @description
     *  A dialog data object for adding or editing CampaignSettingsDisplay objects
     */

    var AddEditCampaignSettingsDialogData = function() {
        var self = this;
        self.campaign = {};
    };

    AddEditCampaignSettingsDialogData.prototype = (function() {

        function isEdit() {
            return this.campaign.key !== '';
        }

        function generateAlias() {
            this.campaign.alias = this.campaign.name.replace( /[^a-zA-Z0-9]/ , '-').toLowerCase();
        }

        return {
            isEdit: isEdit,
            generateAlias: generateAlias
        }

    }());

    angular.module('merchello.models').constant('AddEditCampaignSettingsDialogData', AddEditCampaignSettingsDialogData);