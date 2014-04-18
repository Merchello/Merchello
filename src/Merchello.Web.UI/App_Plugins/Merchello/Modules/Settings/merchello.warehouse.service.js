(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name umbraco.resources.MerchelloWarehouseService
        * @description Loads in data for data types
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

            getDefaultWarehouse: function () {

                return getCachedOrApi("DefaultWarehouse", "GetDefaultWarehouse", "default warehouse");

                //return umbRequestHelper.resourcePromise(
                //   $http({
                //       url: umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'GetDefaultWarehouse'),
                //       method: "GET"
                //   }),
                //   'Failed to retreive data for default warehouse');
            },

            getById: function (id) {

                return umbRequestHelper.resourcePromise(
                   $http({
                       url: umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'GetWarehouse'),
                       method: "GET",
                       params: { id: id }
                   }),
                   'Failed to retreive data for warehouse: ' + id);
            },

            /** saves or updates a product variant object */
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
