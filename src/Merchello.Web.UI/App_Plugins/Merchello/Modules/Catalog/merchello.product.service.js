(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name umbraco.resources.MerchelloProductService
        * @description Loads in data for data types
        **/
    merchelloServices.MerchelloProductService = function ($q, $http, umbDataFormatter, umbRequestHelper) {

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
                       url: '/umbraco/Merchello/ProductApi/GetProduct/' + key,
                       method: "GET"
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
                        '/umbraco/Merchello/ProductApi/GetAllProducts'
                    ),
                    'Failed to get all products');

            },

            filterProducts: function (term) {

                return umbRequestHelper.resourcePromise(
                    $http.get(
                        '/umbraco/Merchello/ProductApi/GetFilteredProducts',
                        { params: { term: term } }
                    ),
                    'Failed to get filtered products');

            }
        };
    };
    
    angular.module('umbraco.resources').factory('merchelloProductService', merchello.Services.MerchelloProductService);

}(window.merchello.Services = window.merchello.Services || {}));
