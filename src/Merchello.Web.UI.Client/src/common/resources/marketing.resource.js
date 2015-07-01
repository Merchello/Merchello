/**
 * @ngdoc resource
 * @name marketingResource
 * @description Loads in data and allows modification for marketing information
 **/
angular.module('merchello.resources')
    .factory('marketingResource',
       ['$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {

            var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloMarketingApiBaseUrl'];

            function setUtcDates(offerSettings) {
                console.info(offerSettings);
            }


            return {
                getOfferProviders: function() {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetOfferProviders',
                            method: "GET"
                        }),
                        'Failed to get offer providers');
                },
                getOfferSettings: function(key) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetOfferSettings',
                            method: "GET",
                            params: { id: key }
                        }),
                        'Failed to get offer settings');
                },
                searchOffers: function(query) {
                    return umbRequestHelper.resourcePromise(
                        $http.post(baseUrl + "SearchOffers",
                            query
                        ),
                        'Failed to search offers');
                },
                getAllOfferSettings: function() {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetAllOfferSettings',
                            method: "GET"
                        }),
                        'Failed to get offer settings');
                },
                getAvailableOfferComponents: function(offerProviderKey) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetAvailableOfferComponents',
                            method: "GET",
                            params: { offerProviderKey: offerProviderKey}
                        }),
                        'Failed to get offer components for the provider');
                },
                checkOfferCodeIsUnique: function(offerCode) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'OfferCodeIsUnique',
                            method: "GET",
                            params: { offerCode: offerCode }
                        }),
                        'Failed to get offer components for the provider');
                },
                newOfferSettings: function (offerSettings) {
                    offerSettings.componentDefinitionExtendedDataToArray();

                    return umbRequestHelper.resourcePromise(
                        $http.post(baseUrl + "PostAddOfferSettings",
                            offerSettings
                        ),
                        'Failed to create offer');
                },
                saveOfferSettings: function(offerSettings) {
                    offerSettings.componentDefinitionExtendedDataToArray();
                    setUtcDates(offerSettings);
                    return umbRequestHelper.resourcePromise(
                        $http.post(baseUrl + "PutUpdateOfferSettings",
                            offerSettings
                        ),
                        'Failed to create offer');
                },
                deleteOfferSettings: function(offerSettings) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'DeleteOfferSettings',
                            method: "GET",
                            params: { id: offerSettings.key }
                        }),
                        'Failed to delete offer settings');
                }

            };
        }]);
