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
            }
        };
    }]);
