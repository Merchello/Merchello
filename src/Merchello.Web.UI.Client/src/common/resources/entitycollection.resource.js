/**
 * @ngdoc controller
 * @name entityCollectionResource
 * @function
 *
 * @description
 * Handles entity collection API
 */

angular.module('merchello.resources').factory('entityCollectionResource',
    ['$http', 'umbRequestHelper',
        function($http, umbRequestHelper) {

            var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloEntityCollectionApiBaseUrl'];

            return {

                getByKey : function(key) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetByKey',
                            method: "GET",
                            params: { key: key }
                        }),
                        'Failed to get entity collection by key');
                },
                getDefaultEntityCollectionProviders : function() {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetDefaultEntityCollectionProviders',
                            method: "GET"
                        }),
                        'Failed to get default entity collection providers');
                },
                addEntityCollection : function(entityCollection) {
                    var url = baseUrl + 'PostAddEntityCollection';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            entityCollection
                        ),
                        'Failed to add an entity collection');
                },
                deleteEntityCollection: function(key) {
                    var url = baseUrl + 'DeleteEntityCollection';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { key: key }
                        }),
                        'Failed to delete the entity collection');
                }

            }

        }]);
