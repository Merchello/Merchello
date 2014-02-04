(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name umbraco.resources.MerchelloWarehouseService
        * @description Loads in data for data types
        **/
    merchelloServices.MerchelloWarehouseService = function ($q, $http, umbDataFormatter, umbRequestHelper) {

        return {

            getDefaultWarehouse: function () {

                return umbRequestHelper.resourcePromise(
                   $http({
                       url: umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'GetDefaultWarehouse'),
                       method: "GET"
                   }),
                   'Failed to retreive data for default warehouse');
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

                return umbRequestHelper.resourcePromise(
                    $http.post(
                        umbRequestHelper.getApiUrl('merchelloWarehouseApiBaseUrl', 'PutWarehouse'),
                        warehouse
                    ),
                    'Failed to save data for warehouse: ' + warehouse.id);
            },

        };
    }

    angular.module('umbraco.resources').factory('merchelloWarehouseService', merchello.Services.MerchelloWarehouseService);

}(window.merchello.Services = window.merchello.Services || {}));
