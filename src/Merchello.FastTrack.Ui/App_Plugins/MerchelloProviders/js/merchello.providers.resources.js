/*! MerchelloPaymentProviders
 * https://github.com/Merchello/Merchello
 * Copyright (c) 2021 Across the Pond, LLC.
 * Licensed MIT
 */

(function() { 

angular.module('merchello.providers.resources').factory('braintreeResource',
    ['$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {

            var baseUrl = Umbraco.Sys.ServerVariables["merchelloPaymentsUrls"]["merchelloBraintreeApiBaseUrl"];

            return {

                getClientRequestToken: function (customerKey) {
                    if (customerKey != null & customerKey != undefined) {
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: baseUrl + 'GetClientRequestToken',
                                method: "GET",
                                params: { customerKey: customerKey }
                            }),
                            'Failed to retreive customer request token');
                    } else {
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: baseUrl + 'GetClientRequestToken',
                                method: "GET"
                            }),
                            'Failed to retreive customer request token');
                    }
                }
            };
        }]);


})();