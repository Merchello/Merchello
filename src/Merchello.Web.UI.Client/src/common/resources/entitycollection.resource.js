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
                getEntityCollectionsByEntity : function (entity, entityType, isFilter) {
                    if (isFilter === undefined) {
                        isFilter = false;
                    }
                    var url = baseUrl + 'PostGetEntityCollectionsByEntity';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            { key: entity.key, entityType: entityType, isFilter: isFilter }
                        ),
                        'Failed to get entity collections for entity');
                },
                // getEntitySpecifiedFilterCollections
                getEntityFilterGroups : function(entityType) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetEntityFilterGroups',
                            method: "GET",
                            params: { entityType: entityType}
                        }),
                        'Failed to get entity specified filter by the entityType');
                },
                // getEntitySpecifiedFilterCollectionProviders
                getEntityFilterGroupProviders : function(entityType) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetEntityFilterGroupProviders',
                            method: "GET",
                            params: { entityType: entityType}
                        }),
                        'Failed to get entity specified filter providers by the entityType');
                },
                // getEntitySpecifiedFilterCollectionAttributeProvider
                getEntityFilterGroupFilterProvider : function(collectionKey) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetEntityFilterGroupFilterProvider',
                            method: "GET",
                            params: { key: collectionKey}
                        }),
                        'Failed to get specified filter attribute provider by the entityType');
                },
                // getSpecifiedFilterCollectionsContainingProduct
                getEntityFilterGroupsContaining : function(entityType, entityKey) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetEntityFilterGroupsContaining',
                            method: "GET",
                            params: { entityType: entityType, entityKey: entityKey}
                        }),
                        'Failed to get specified filter attribute provider by the entityType');
                },
                // getSpecifiedFilterCollectionsNotContainingProduct
                getEntityFilterGroupsNotContaining : function(entityType, entityKey) {
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: baseUrl + 'GetEntityFilterGroupsNotContaining',
                            method: "GET",
                            params: { entityType: entityType, entityKey: entityKey}
                        }),
                        'Failed to get specified filter attribute provider by the entityType');
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
                    entityCollection.extendedData = entityCollection.extendedData.toArray();

                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            entityCollection
                        ),
                        'Failed to add an entity collection');
                },
                saveEntityCollection : function(collection) {
                    var url = baseUrl + 'PutEntityCollection';
                    collection.extendedData = collection.extendedData.toArray();
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            collection
                        ),
                        'Failed to save an entity collection');
                },
                //saveSpecifiedFilterCollection
                putEntityFilterGroup : function(collection) {
                    var url = baseUrl + 'PutEntityFilterGroup';
                    collection.extendedData = collection.extendedData.toArray();
                    _.each(collection.filters, function(ac) {
                       ac.extendedData = ac.extendedData.toArray();
                    });
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            collection
                        ),
                        'Failed to save speficed entity collection');
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
                // associateEntityWithFilterCollections
                associateEntityWithFilters: function (entityKey, collectionKeys) {
                    var url = baseUrl + 'PostAssociateEntityWithFilters';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            { entityKey: entityKey, collectionKeys: collectionKeys }
                        ),
                        'Failed to associate an entity with filter collections');
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
                    _.each(entityCollections, function(ec) {
                       ec.extendedData = ec.extendedData.toArray();
                    });
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
