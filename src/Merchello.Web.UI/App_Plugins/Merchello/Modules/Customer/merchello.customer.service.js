(function (merchelloServices, undefined) {


    /**
    * @ngdoc service
    * @name umbraco.resources.MerchelloCustomerService
    * @description Deals with customers api.
    **/
    merchelloServices.MerchelloCustomerService = function ($http, umbRequestHelper) {

        return {

            GetAllCustomers: function () {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloCustomerApiBaseUrl', 'GetAllCustomers'),
                        method: "GET",
                        params: {}
                    }),
                    'Failed to load customers');

            }

        };
    };

    angular.module('umbraco.resources').factory('merchelloCustomerService', ['$http', 'umbRequestHelper', merchello.Services.MerchelloCustomerService]);

}(window.merchello.Services = window.merchello.Services || {}));
