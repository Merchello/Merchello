/**
 * @ngdoc service
 * @name taxationGatewayProviderResource
 * @description Loads in data for taxation providers
 **/
angular.module('merchello.resources').factory('taxationGatewayProviderResource',
    ['$http', 'umbRequestHelper',
    function($http, umbRequestHelper) {
    return {
        getGatewayResources: function (taxationGatewayProviderKey) {

            return umbRequestHelper.resourcePromise(
                $http({
                    url: umbRequestHelper.getApiUrl('merchelloTaxationGatewayApiBaseUrl', 'GetGatewayResources'),
                    method: "GET",
                    params: {id: taxationGatewayProviderKey}
                }),
                'Failed to retreive gateway resource data for warehouse catalog');
        },

        getAllGatewayProviders: function () {

            return umbRequestHelper.resourcePromise(
                $http({
                    url: umbRequestHelper.getApiUrl('merchelloTaxationGatewayApiBaseUrl', 'GetAllGatewayProviders'),
                    method: "GET"
                }),
                'Failed to retreive data for all gateway providers');
        },

        getTaxationProviderTaxMethods: function (taxationGatewayProviderKey) {

            return umbRequestHelper.resourcePromise(
                $http({
                    url: umbRequestHelper.getApiUrl('merchelloTaxationGatewayApiBaseUrl', 'GetTaxationProviderTaxMethods'),
                    method: "GET",
                    params: {id: taxationGatewayProviderKey}
                }),
                'Failed to tax provider methods for: ' + taxationGatewayProviderKey);
        },

        addTaxMethod: function (taxMethod) {

            return umbRequestHelper.resourcePromise(
                $http.post(umbRequestHelper.getApiUrl('merchelloTaxationGatewayApiBaseUrl', 'AddTaxMethod'),
                    taxMethod
                ),
                'Failed to create taxMethod');
        },

        saveTaxMethod: function (taxMethod) {

            return umbRequestHelper.resourcePromise(
                $http.post(umbRequestHelper.getApiUrl('merchelloTaxationGatewayApiBaseUrl', 'PutTaxMethod'),
                    taxMethod
                ),
                'Failed to save taxMethod');
        },

        deleteTaxMethod: function (taxMethodKey) {

            return umbRequestHelper.resourcePromise(
                $http({
                    url: umbRequestHelper.getApiUrl('merchelloTaxationGatewayApiBaseUrl', 'DeleteTaxMethod'),
                    method: "GET",
                    params: {id: taxMethodKey}
                }),
                'Failed to delete tax method');
        }
    };
}]);
