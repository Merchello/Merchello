/**
 * @ngdoc controller
 * @name Merchello.Backoffice.CampaignListController
 * @function
 *
 * @description
 * The controller for the marketing campaign list
 */
angular.module('merchello').controller('Merchello.Backoffice.CampaignListController',
    ['$scope', 'notificationsService', 'dialogService', 'merchelloTabsFactory', 'marketingCampaignResource', 'campaignSettingsDisplayBuilder',
    'dialogDataFactory',
    function($scope, notificationsService, dialogService, merchelloTabsFactory, marketingCampaignResource, campaignSettingsDisplayBuilder,
    dialogDataFactory) {

        $scope.loaded = false;
        $scope.activeOnly = true;
        $scope.preValuesLoaded = false;
        $scope.campaigns = [];
        $scope.tabs = [];
        $scope.filterText = '';

        // exposed methods
        $scope.toggleActiveOnly = toggleActiveOnly;
        $scope.openCreateCampaignDialog = openCreateCampaignDialog;

        function init()
        {
            loadCampaignSettings();
            $scope.tabs = merchelloTabsFactory.createCampaignTabs();
            $scope.tabs.setActive('campaignlist');
            $scope.loaded = true;
        }

        /**
         * @ngdoc method
         * @name loadCampaignSettings
         * @function
         *
         * @description
         * Loaps the campaign settings
         */
        function loadCampaignSettings()
        {
            $scope.preValuesLoaded = false;
            var promise;
            console.info($scope.activeOnly);
            if ($scope.activeOnly) {
                promise = marketingCampaignResource.getActiveCampaigns();
            } else {
                promise = marketingCampaignResource.getAllCampaigns();
            }
            promise.then(function(results) {
                $scope.campaigns = campaignSettingsDisplayBuilder.transform(results);
                $scope.preValuesLoaded = true;
            });
        }

        function toggleActiveOnly()
        {
            $scope.activeOnly = !$scope.activeOnly;
            loadCampaignSettings();
        }

        // dialogs
        function openCreateCampaignDialog() {
            var data = dialogDataFactory.createAddEditCampaignSettingsDialogData();
            data.campaign = campaignSettingsDisplayBuilder.createDefault();
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.create.campaign.html',
                show: true,
                callback: processCreateCampaign,
                dialogData: data
            });
        }

        function processCreateCampaign(dialogData) {
            console.info(dialogData);
        }

        //// Initializes the controller
        init();
}]);
