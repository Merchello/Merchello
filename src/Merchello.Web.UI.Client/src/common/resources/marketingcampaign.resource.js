angular.module('merchello.resources').factory('marketingCampaignResource',
    ['$http', 'umbRequestHelper',
    function($http, umbRequestHelper) {

        var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloMarketingCampaignApiBaseUrl'];

        return {
            getActiveCampaigns: function () {
                var url = baseUrl + 'GetActiveCampaigns';
                console.info(url);
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET"
                    }),
                    'Failed to get active marketing campaigns');
            },
            getAllCampaigns: function() {
                var url = baseUrl + 'GeAllCampaigns';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET"
                    }),
                    'Failed to get all marketing campaigns');
            }
        };
    }]);
