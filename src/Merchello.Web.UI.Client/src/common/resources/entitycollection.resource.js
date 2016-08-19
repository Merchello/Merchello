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
                getSortableProviderKeys : function() {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetSortableProviderKeys',
                            method: "GET"
                        }),
                        'Failed to get valid sortable provider keys');
                },
                getRootCollectionsByEntityType : function(entityType) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetRootEntityCollections',
                            method: "GET",
                            params: { entityType: entityType }
                        }),
                        'Failed to get entity collection by the entity type');
                },
                getChildEntityCollections : function(parentKey) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetChildEntityCollections',
                            method: "GET",
                            params: { parentKey: parentKey }
                        }),
                        'Failed to get entity collection by the parentKey');
                },
                getEntityCollectionsByEntity : function (entity, entityType) {
                    var url = baseUrl + 'PostGetEntityCollectionsByEntity';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            { key: entity.key, entityType: entityType }
                        ),
                        'Failed to get entity collections for entity');
                },
                getEntitySpecifiedFilterCollections : function(entityType) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetEntitySpecifiedFilterCollections',
                            method: "GET",
                            params: { entityType: entityType}
                        }),
                        'Failed to get entity specification collections by the entityType');
                },
                getEntitySpecifiedFilterCollectionProviders : function(entityType) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetEntitySpecifiedFilterCollectionProviders',
                            method: "GET",
                            params: { entityType: entityType}
                        }),
                        'Failed to get entity specification providers by the entityType');
                },
                getDefaultEntityCollectionProviders : function() {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetDefaultEntityCollectionProviders',
                            method: "GET"
                        }),
                        'Failed to get default entity collection providers');
                },
                getEntityCollectionProviders : function() {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetEntityCollectionProviders',
                            method: "GET"
                        }),
                        'Failed to get entity collection providers');
                },
                addEntityCollection : function(entityCollection) {
                    var url = baseUrl + 'PostAddEntityCollection';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            entityCollection
                        ),
                        'Failed to add an entity collection');
                },
                saveEntityCollection : function(collection) {
                    var url = baseUrl + 'PutEntityCollection';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            collection
                        ),
                        'Failed to save an entity collection');
                },
                addEntityToCollections: function(entityKey, collectionKeys) {
                    var url = baseUrl + 'PostAddEntityToCollections';
                    var data = [];
                    angular.forEach(collectionKeys, function(ck) {
                      data.push({ entityKey: entityKey, collectionKey: ck });
                    });
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            data
                        ),
                        'Failed to add an entity to a collection');
                },
                addEntityToCollection : function(entityKey, collectionKey) {
                    var url = baseUrl + 'PostAddEntityToCollection';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            { entityKey: entityKey, collectionKey: collectionKey }
                        ),
                        'Failed to add an entity to a collection');
                },
                removeEntityFromCollections : function(entityKey, collectionKeys) {
                    var url = baseUrl + 'DeleteEntityFromCollections';
                    var data = [];
                    angular.forEach(collectionKeys, function(ck) {
                        data.push({ entityKey: entityKey, collectionKey: ck });
                    });
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                           data
                        ),
                        'Failed to remove an entity from a collection');
                },
                removeEntityFromCollection : function(entityKey, collectionKey) {
                    var url = baseUrl + 'DeleteEntityFromCollection';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            { entityKey: entityKey, collectionKey: collectionKey }
                        ),
                        'Failed to remove an entity from a collection');
                },
                getCollectionEntities : function(query) {
                    var url = baseUrl + 'PostGetCollectionEntities';
                    return umbRequestHelper.resourcePromise(
                        $http.post(
                            url,
                            query
                        ),
                        'Failed to get colleciton entities');
                },
                getEntitiesNotInCollection: function(query) {
                    var url = baseUrl + 'PostGetEntitiesNotInCollection';
                    return umbRequestHelper.resourcePromise(
                        $http.post(
                            url,
                            query
                        ),
                        'Failed to get colleciton entities');
                },
                updateSortOrders : function(entityCollections) {
                    var url = baseUrl + 'PutUpdateSortOrders';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            entityCollections
                        ),
                        'Failed to update sort orders');
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

            };

        }]);
