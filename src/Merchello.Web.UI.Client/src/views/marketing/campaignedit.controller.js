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
    '$scope', '$routeParams', 'notificationsService', 'dialogService', 'merchelloTabsFactory',
    'marketingCampaignResource', 'campaignSettingsDisplayBuilder',
    function($scope, $routeParams, notificationsService, dialogService, merchelloTabsFactory,
    marketingCampaignResource, campaignSettingsDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.tabs = [];
        $scope.context = 'newcampaign';
        $scope.campaign = campaignSettingsDisplayBuilder.createDefault();

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

        //// Initializes the controller
        init();
    }]);
