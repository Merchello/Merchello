    /**
     * @ngdoc resource
     * @name warehouseResource
     * @description Loads in data and allows modification of warehouses
     **/
    angular.module('merchello.resources').factory('warehouseResource',
        ['$q', '$http', '$cacheFactory', 'umbDataFormatter', 'umbRequestHelper',
        function($q, $http, $cacheFactory, umbDataFormatter, umbRequestHelper) {

            /* cacheFactory instance for cached items in the merchelloWarehouseService */
            var _warehouseCache = $cacheFactory('merchelloWarehouse');

            /* helper method to get from cache or fall back to an http api call */
            function getCachedOrApi(cacheKey, apiMethod, entityName) {
                var deferred = $q.defer();
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloWarehouseApiBaseUrl'] + apiMethod;
                var dataFromCache = _warehouseCache.get(cacheKey);

                if (dataFromCache) {
                    deferred.resolve(dataFromCache);
                }
                else {
                    var promiseFromApi = umbRequestHelper.resourcePromise(
                        $http.get(
                            url
                        ),
                        'Failed to get ' + entityName);

                    promiseFromApi.then(function (dataFromApi) {
                        _warehouseCache.put(cacheKey, dataFromApi);
                        deferred.resolve(dataFromApi);
                    }, function (reason) {
                        deferred.reject(reason);
                    });
                }

                return deferred.promise;
            }

            return {

                /**
                 * @ngdoc method
                 * @name addWarehouseCatalog
                 * @function
                 *
                 * @description
                 * Posts a new warehouse catalog to the API.
                 **/
                addWarehouseCatalog: function (catalog) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloWarehouseApiBaseUrl'] + 'AddWarehouseCatalog';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url, catalog),
                        'Failed to add warehouse catalog');
                },

                /**
                 * @ngdoc method
                 * @name deleteWarehouseCatalog
                 * @function
                 *
                 * @description
                 * Deletes a warehouse catalog in the API.
                 **/
                deleteWarehouseCatalog: function (key) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloWarehouseApiBaseUrl'] + 'DeleteWarehouseCatalog';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: 'GET',
                            params: { id: key }
                        }),
                        'Failed to delete warehouse catalog with key: ' + key);
                },

                /**
                 * @ngdoc method
                 * @name getById
                 * @function
                 *
                 * @description
                 * Gets a Warehouse from the API by its id.
                 **/
                getById: function (id) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloWarehouseApiBaseUrl'] + 'GetWarehouse';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: id }
                        }),
                        'Failed to retreive data for warehouse: ' + id);
                },

                /**
                 * @ngdoc method
                 * @name getDefaultWarehouse
                 * @function
                 *
                 * @description Gets the default warehouse from the API.
                 **/
                getDefaultWarehouse: function () {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloWarehouseApiBaseUrl'] + 'GetDefaultWarehouse';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET"
                        }),
                        'Failed to retreive data for default warehouse');
                },

                /**
                 * @ngdoc method
                 * @name getWarehouseCatalogs
                 * @function
                 *
                 * @description
                 * Gets the catalogs from the warehouse with the given warehouse key.
                 **/
                getWarehouseCatalogs: function (key) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloWarehouseApiBaseUrl'] + 'GetWarehouseCatalogs';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: 'GET',
                            params: { id: key }
                        }),
                        'Failed to get catalogs for warehouse: ' + key);
                },

                /**
                 * @ngdoc method
                 * @name putWarehouseCatalog
                 * @function
                 *
                 * @description
                 * Updates a warehouse catalog in the API.
                 **/
                putWarehouseCatalog: function (catalog) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloWarehouseApiBaseUrl'] + 'PutWarehouseCatalog';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url, catalog),
                        'Failed to update warehouse catalog');
                },

                /**
                 * @ngdoc method
                 * @name save
                 * @function
                 *
                 * @description
                 * Saves the provided warehouse to the API.
                 **/
                save: function (warehouse) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloWarehouseApiBaseUrl'] + 'PutWarehouse';
                    _warehouseCache.remove("DefaultWarehouse");

                    return umbRequestHelper.resourcePromise(
                        $http.post(
                            url,
                            warehouse
                        ),
                        'Failed to save data for warehouse: ' + warehouse.id);
                },

            };

        }]);