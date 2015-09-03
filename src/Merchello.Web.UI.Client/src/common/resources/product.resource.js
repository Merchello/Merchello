    /**
     * @ngdoc resource
     * @name productResource
     * @description Loads in data and allows modification of products
     **/
    angular.module('merchello.resources').factory('productResource',
        ['$q', '$http', 'umbRequestHelper',
        function($q, $http, umbRequestHelper) {

            return {

                ///////////////////////////////////////////////////////////////////////////////////////////
                /// Server http requests
                ///////////////////////////////////////////////////////////////////////////////////////////

                /**
                 * @ngdoc method
                 * @name add
                 * @description Creates a new product with an API call to the server
                 **/
                add: function (product) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'AddProduct';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            product
                        ),
                        'Failed to create product sku ' + product.sku);
                },

                /**
                 * @ngdoc method
                 * @name getByKey
                 * @description Gets a product with an API call to the server
                 **/
                getByKey: function (key) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'GetProductFromService';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url + '?id=' + key,
                            method: "GET"
                        }),
                        'Failed to retreive data for product key ' + key);
                },

                getByKeys: function(keys) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'GetByKeys';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            keys
                        ),
                        'Failed to retreive data for product key ' + keys);
                },

                /**
                 * @ngdoc method
                 * @name getVariant
                 * @description Gets a product variant with an API call to the server
                 **/
                getVariant: function (key) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'GetProductVariant';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url + '?id=' + key,
                            method: "GET"
                        }),
                        'Failed to retreive data for product variant key ' + key);
                },

                /**
                 * @ngdoc method
                 * @name save
                 * @description Saves / updates product with an api call back to the server
                 **/
                save: function (product) {
                    angular.forEach(product.detachedContents, function(dc) {
                        dc.detachedDataValues = dc.detachedDataValues.toArray();
                    });
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'PutProduct';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            product
                        ),
                        'Failed to save data for product key ' + product.key);
                },

                /**
                 * @ngdoc method
                 * @name saveVariant
                 * @description Saves / updates product variant with an api call back to the server
                 **/
                saveVariant: function (productVariant) {
                    angular.forEach(productVariant.detachedContents, function(dc) {
                        dc.detachedDataValues = dc.detachedDataValues.toArray();
                    });
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'PutProductVariant';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            productVariant
                        ),
                        'Failed to save data for product variant key ' + productVariant.key);
                },

                /**
                 * @ngdoc method
                 * @name deleteProduct
                 * @description Deletes product with an api call back to the server
                 **/
                deleteProduct: function (product) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'DeleteProduct';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            product.key,
                            { params: { id: product.key }}
                        ),
                        'Failed to delete product with key: ' + product.key);
                },

                deleteDetachedContent: function(variant) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'DeleteDetachedContent';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            variant
                        ),
                        'Failed to delete detached content');
                },

                /**
                 * @ngdoc method
                 * @name searchProducts
                 * @description Searches for all products with a ListQuery object
                 **/
                searchProducts: function (query) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'SearchProducts';
                    return umbRequestHelper.resourcePromise(
                        $http.post(
                            url,
                            query
                        ),
                        'Failed to search products');
                }
            };
    }]);
