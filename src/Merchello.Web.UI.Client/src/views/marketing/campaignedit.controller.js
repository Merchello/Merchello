/**
 * @ngdoc controller
 * @name Merchello.Backoffice.CampaignEditController
 * @function
 *
 * @description
 * The controller for adding and editing marketing campaigns
 * 'campaignResource', 'campaignSettingDisplayBuilder',
 * campaignResource, campaignSettingDisplayBuilder
 */
angular.module('merchello').controller('Merchello.Backoffice.CampaignEditController',[
    '$scope', '$routeParams', '$location', 'notificationsService', 'dialogService', 'merchelloTabsFactory', 'dialogDataFactory',
    'marketingCampaignResource', 'campaignSettingsDisplayBuilder',
    function($scope, $routeParams, $location, notificationsService, dialogService, merchelloTabsFactory, dialogDataFactory,
    marketingCampaignResource, campaignSettingsDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.tabs = [];
        $scope.context = 'newcampaign';
        $scope.campaign = campaignSettingsDisplayBuilder.createDefault();

        // exposed methods
        $scope.openDeleteCampaignDialog = openDeleteCampaignDialog;
        $scope.openEditCampaignDialog = openEditCampaignDialog;

        function init() {
            $scope.loaded = true;

            loadCampaign();
        }

        function loadCampaign() {
            var campaignKey = $routeParams.id;
            if (campaignKey === 'create') {
                $scope.tabs = merchelloTabsFactory.createCampaignTabs();
                $scope.tabs.addTab('newcampaign', 'New Campaign');
                $scope.tabs.setActive('newcampaign');
                $scope.context = 'newcampaign';
                $scope.preValuesLoaded = true;
            } else {
                var promise = marketingCampaignResource.getCampaignByKey(campaignKey);
                promise.then(function(result) {
                    $scope.tabs = merchelloTabsFactory.createCampaignEditTabs(campaignKey);
                    $scope.campaign = campaignSettingsDisplayBuilder.transform(result);
                    console.info($scope.campaign);
                    $scope.tabs.setActive('campaignedit');
                    $scope.context = 'editcampaign';
                    $scope.preValuesLoaded = true;
                });
            }
        }


        // dialogs
        function openEditCampaignDialog() {
            var data = dialogDataFactory.createAddEditCampaignSettingsDialogData();
            data.campaign = $scope.campaign.clone();
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.create.campaign.html',
                show: true,
                callback: processEditCampaign,
                dialogData: data
            });
        }

        function processEditCampaign(dialogData) {
            $scope.preValuesLoaded = false;
            var promise = marketingCampaignResource.updateCampaignSettings(dialogData.campaign);
            promise.then(function(result) {
                loadCampaign();
            }, function(reason) {
                notificationsService.error("Campaign save Failed", reason.message);
            });
        }

        function openDeleteCampaignDialog() {
            var dialogData = {};
            dialogData.name = 'Marketing Campaign ' + $scope.campaign.name;
            dialogData.warning = 'This action will also permanently delete any campaign activities associated with the campaign.';
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                show: true,
                callback: processDeleteCampaignDialog,
                dialogData: dialogData
            });
        }

        function processDeleteCampaignDialog() {
            var promise = marketingCampaignResource.deleteCampaignSettings($scope.campaign);
            promise.then(function() {
                notificationsService.success('Marketing Campaign Deleted');
                $location.url("/merchello/merchello/campaignlist/manage", true);
            }, function (reason) {
                notificationsService.error('Failed to Delete Marketing Campaign', reason.message);
            });
        }

        //// Initializes the controller
        init();
    }]);
