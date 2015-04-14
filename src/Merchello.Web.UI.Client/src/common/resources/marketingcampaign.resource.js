angular.module('merchello.resources').factory('marketingCampaignResource',
    ['$http', 'umbRequestHelper',
    function($http, umbRequestHelper) {

        var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloMarketingCampaignApiBaseUrl'];

        return {
            getActiveCampaigns: function () {
                var url = baseUrl + 'GetActiveCampaigns';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET"
                    }),
                    'Failed to get active marketing campaigns');
            },
            getAllCampaigns: function() {
                var url = baseUrl + 'GetAllCampaigns';

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET"
                    }),
                    'Failed to get all marketing campaigns');
            },
            getCampaignByKey: function(key) {
                var url = baseUrl + 'GetCampaignSettingsByKey';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: 'GET',
                        params: { key: key }
                    }),
                    'Failed to get a campaign setting by its unique key');
            },
            addCampaignSettings: function(campaign) {
                var url = baseUrl + 'PostAddCampaignSettings';
                return umbRequestHelper.resourcePromise(
                   $http.post(url, campaign),
                'Failed to add a new campaign');
            },
            updateCampaignSettings: function(campaign) {
                var url = baseUrl + 'PostUpdateCampaignSetting';
                return umbRequestHelper.resourcePromise(
                    $http.post(url, campaign),
                    'Failed to update the campaign');
            },
            deleteCampaignSettings: function(campaign) {
                var url = baseUrl + 'DeleteCampaignSetting';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: { key: campaign.key }
                    }),
                    'Failed to delete marketing campaign settings');
            }
        };
    }]);
