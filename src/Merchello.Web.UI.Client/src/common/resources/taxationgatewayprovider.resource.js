/**
 * @ngdoc resource
 * @name taxationGatewayProviderResource
 * @description Loads in data for taxation providers
 **/
angular.module('merchello.resources').factory('taxationGatewayProviderResource',
    ['$http', 'umbRequestHelper',
    function($http, umbRequestHelper) {
    return {
        getGatewayResources: function (taxationGatewayProviderKey) {
            var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloTaxationGatewayApiBaseUrl'] + 'GetGatewayResources';
            return umbRequestHelper.resourcePromise(
                $http({
                    url: url,
                    method: "GET",
                    params: {id: taxationGatewayProviderKey}
                }),
                'Failed to retreive gateway resource data for warehouse catalog');
        },

        getAllGatewayProviders: function () {
            var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloTaxationGatewayApiBaseUrl'] + 'GetAllGatewayProviders';
            return umbRequestHelper.resourcePromise(
                $http({
                    url: url,
                    method: "GET"
                }),
                'Failed to retreive data for all gateway providers');
        },

        getTaxationProviderTaxMethods: function (taxationGatewayProviderKey) {
            var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloTaxationGatewayApiBaseUrl'] + 'GetTaxationProviderTaxMethods';
            return umbRequestHelper.resourcePromise(
                $http({
                    url: url,
                    method: "GET",
                    params: {id: taxationGatewayProviderKey}
                }),
                'Failed to tax provider methods for: ' + taxationGatewayProviderKey);
        },

        addTaxMethod: function (taxMethod) {
            var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloTaxationGatewayApiBaseUrl'] + 'AddTaxMethod';
            return umbRequestHelper.resourcePromise(
                $http.post(url,
                    taxMethod
                ),
                'Failed to create taxMethod');
        },

        saveTaxMethod: function (taxMethod) {
            var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloTaxationGatewayApiBaseUrl'] + 'PutTaxMethod';
            return umbRequestHelper.resourcePromise(
                $http.post(url,
                    taxMethod
                ),
                'Failed to save taxMethod');
        },

        deleteTaxMethod: function (taxMethodKey) {
            var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloTaxationGatewayApiBaseUrl'] + 'DeleteTaxMethod';
            return umbRequestHelper.resourcePromise(
                $http({
                    url: url,
                    method: "GET",
                    params: {id: taxMethodKey}
                }),
                'Failed to delete tax method');
        }
    };
}]);
