(function (merchelloServices, undefined) {


    /**
    * @ngdoc service
    * @name umbraco.resources.MerchelloWarehouseService
    * @description Loads in data for warehouses.
    **/
    merchelloServices.MerchelloWarehouseService = function ($q, $http, $cacheFactory, umbDataFormatter, umbRequestHelper) {

        /* cacheFactory instance for cached items in the merchelloWarehouseService */
        var _warehouseCache = $cacheFactory('merchelloWarehouse');

        /* helper method to get from cache or fall back to an http api call */
        function getCachedOrApi(cacheKey, apiMethod, entityName) {
            var deferred = $q.defer();

            var dataFromCache = _warehouseCache.get(cacheKey);

            if (dataFromCache) {
                deferred.resolve(dataFromCache);
            }
            else {
                var promiseFromApi = umbRequestHelper.resourcePromise(
                   $http.get(
                        umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', apiMethod)
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
                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'AddWarehouseCatalog'), catalog),
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
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'DeleteWarehouseCatalog'),
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
                return umbRequestHelper.resourcePromise(
                   $http({
                       url: umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'GetWarehouse'),
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

                //return getCachedOrApi("DefaultWarehouse", "GetDefaultWarehouse", "default warehouse");

                return umbRequestHelper.resourcePromise(
                   $http({
                       url: umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'GetDefaultWarehouse'),
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
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'GetWarehouseCatalgos'),
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
                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'PutWarehouseCatalog'), catalog),
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

                _warehouseCache.remove("DefaultWarehouse");

                return umbRequestHelper.resourcePromise(
                    $http.post(
                        umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'PutWarehouse'),
                        warehouse
                    ),
                    'Failed to save data for warehouse: ' + warehouse.id);
            },

        };
    }

    angular.module('umbraco.resources').factory('merchelloWarehouseService', ['$q', '$http', '$cacheFactory', 'umbDataFormatter', 'umbRequestHelper', merchello.Services.MerchelloWarehouseService]);

}(window.merchello.Services = window.merchello.Services || {}));
