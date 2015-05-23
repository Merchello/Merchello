/**
 * @ngdoc resource
 * @name marketingResource
 * @description Loads in data and allows modification for marketing information
 **/
angular.module('merchello.resources')
    .factory('marketingResource', ['$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {
            var baseUrl = Umbraco.Sys.ServerVariables['merchello']['merchelloMarketingApiBaseUrl'];
            return {
                getOfferSettings: function() {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetOfferSettings',
                            method: "GET"
                        }),
                        'Failed to get offer settings');
                }
            };
        }]);
