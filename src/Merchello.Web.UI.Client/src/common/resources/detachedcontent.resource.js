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

            getContentTypes: function() {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: baseUrl + 'GetContentTypes',
                        method: "GET"
                    }),
                    'Failed to get Umbraco content types');
            }

        };
}]);
