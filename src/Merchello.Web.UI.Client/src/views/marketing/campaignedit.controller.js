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
    function($scope, $routeParams, notificationsService, dialogService, merchelloTabsFactory) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.tabs = [];
        $scope.context = 'newcampaign';

        function init() {
            $scope.loaded = true;

            loadCampaign();
        }

        function loadCampaign() {
            var campaignKey = $routeParams.id;
            console.info(campaignKey);
            if (campaignKey === 'create') {
                $scope.tabs = merchelloTabsFactory.createCampaignTabs();
                $scope.tabs.addTab('newcampaign', 'New Campaign');
                $scope.tabs.setActive('newcampaign');
                $scope.context = 'newcampaign';
                $scope.preValuesLoaded = true;
            } else {
                $scope.tabs = merchelloTabsFactory.createCampaignEditTabs(campaignKey);
                $scope.tabs.setActive('campaignedit');
                $scope.context = 'editcampaign';
                $scope.preValuesLoaded = true;
            }
        }

        //// Initializes the controller
        init();
    }]);
