/**
 * @ngdoc controller
 * @name detachedContentResource
 * @function
 *
 * @description
 * Handles the detached content API
 */
angular.module('merchello.resources').factory('detachedContentResource',
    ['$http', 'umbRequestHelper',
    function($http, umbRequestHelper) {

        var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloDetachedContentApiBaseUrl'];

        return {
            getAllLanguages: function() {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: baseUrl + 'GetAllLanguages',
                        method: "GET"
                    }),
                    'Failed to get Umbraco languages');
            },
            getContentTypes: function() {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: baseUrl + 'GetContentTypes',
                        method: "GET"
                    }),
                    'Failed to get Umbraco content types');
            },
            getDetachedContentTypeByEntityType: function(enumValue) {
                var url = baseUrl + 'GetDetachedContentTypesByEntityType';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: { enumValue: enumValue}
                    }),
                    'Failed to get detached content types');
            },
            addDetachedContentType : function(detachedContentType) {
                var url = baseUrl + 'PostAddDetachedContentType';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        detachedContentType
                    ),
                    'Failed to add an detached content type');
            }

        };
}]);
