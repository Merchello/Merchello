/**
    * @ngdoc service
    * @name umbraco.resources.merchelloProductService
    * @description Loads in data for data types
    **/
function merchelloProductService($q, $http, umbDataFormatter, umbRequestHelper) {

    return {

        create: function (productname, sku, price) {

            return umbRequestHelper.resourcePromise(
               $http({
                   url: '/umbraco/Merchello/ProductApi/NewProduct',
                   method: "POST",
                   params: { sku: sku, name: productname, price: price }
               }),
               'Failed to create product sku ' + sku);
        },

        getByKey: function (key) {

            return umbRequestHelper.resourcePromise(
               $http({
                   url: '/umbraco/Merchello/ProductApi/GetProduct',
                   method: "GET",
                   params: { key: key }
               }),
               'Failed to retreive data for product key ' + key);
        },

        /** saves or updates a product object */
        save: function (product) {

            return umbRequestHelper.resourcePromise(
                $http.put(
                    '/umbraco/Merchello/ProductApi/PutProduct',
                    product
                ),
                'Failed to save data for product key ' + product.key);
        },

        getAllProducts: function () {

            return umbRequestHelper.resourcePromise(
                $http.get(
                    '/umbraco/Merchello/ProductApi/GetProducts'
                ),
                'Failed to get all products');

        }
    };
}

angular.module('umbraco.resources').factory('merchelloProductService', merchelloProductService);
