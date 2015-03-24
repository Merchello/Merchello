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
    function($scope, notificationsService, dialogService, merchelloTabsFactory, marketingCampaignResource, campaignSettingsDisplayBuilder) {

        $scope.loaded = false;
        $scope.activeOnly = true;
        $scope.preValuesLoaded = false;
        $scope.campaigns = [];
        $scope.tabs = [];

        function init()
        {
            loadCampaignSettings();
            $scope.tabs = merchelloTabsFactory.createCampaignTabs();
            $scope.tabs.setActive('campaignlist');
            $scope.loaded = true;
        }

        function loadCampaignSettings()
        {
            var promise;
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

        //// Initializes the controller
        init();
}]);
