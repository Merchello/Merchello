angular.module('merchello.providers.resources').factory('braintreeResource',
    ['$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {

            var baseUrl = Umbraco.Sys.ServerVariables["merchelloPaymentsUrls"]["merchelloBraintreeApiBaseUrl"];

            return {

                getClientRequestToken : function(customerKey) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetClientRequestToken',
                            method: "GET",
                            params: { customerKey: customerKey }
                        }),
                        'Failed to retreive customer request token');
                }
            };
        }]);
