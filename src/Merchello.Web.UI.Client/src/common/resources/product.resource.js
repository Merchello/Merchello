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

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'AddProduct'),
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

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'GetProduct', [{ id: key }]),
                            method: "GET"
                        }),
                        'Failed to retreive data for product key ' + key);
                },

                /**
                 * @ngdoc method
                 * @name getVariant
                 * @description Gets a product variant with an API call to the server
                 **/
                getVariant: function (key) {

                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'GetProductVariant', [{ id: key }]),
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

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'PutProduct'),
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

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'PutProductVariant'),
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

                    return umbRequestHelper.resourcePromise(
                        $http.post(umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'DeleteProduct'),
                            product.key,
                            { params: { id: product.key }}
                        ),
                        'Failed to delete product with key: ' + product.key);
                },

                /**
                 * @ngdoc method
                 * @name searchProducts
                 * @description Searches for all products with a ListQuery object
                 **/
                searchProducts: function (query) {

                    return umbRequestHelper.resourcePromise(
                        $http.post(
                            umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'SearchProducts'),
                            query
                        ),
                        'Failed to search products');

                },

                ///////////////////////////////////////////////////////////////////////////////////////////
                /// Business logic
                ///////////////////////////////////////////////////////////////////////////////////////////

                /**
                 * @ngdoc method
                 * @name createProduct
                 * @description Creates product and delivers the new Product model in the promise data
                 **/
                createProduct: function(product) {

                    var deferred = $q.defer();

                    var promiseCreate = prodservice.add(product);
                    promiseCreate.then(function(newproduct) {

                        product = new merchello.Models.Product(newproduct);
                        deferred.resolve(product);

                    }, function(reason) {
                        deferred.reject(reason);
                    });

                    return deferred.promise;
                },

                /**
                 * @ngdoc method
                 * @name updateProduct
                 * @description Saves product changes and delivers the new Product model in the promise data
                 **/
                updateProduct: function (product) {

                    var deferred = $q.defer();

                    var promise = prodservice.save(product);

                    promise.then(function (savedProduct) {

                        product = new merchello.Models.Product(savedProduct);

                        deferred.resolve(product);

                    }, function (reason) {
                        deferred.reject(reason);
                    });

                    return deferred.promise;
                },

                /**
                 * @ngdoc method
                 * @name updateProductVariant
                 * @description Saves product variant changes and delivers the new ProductVariant model in the promise data
                 **/
                updateProductVariant: function (productVariant) {
                    var deferred = $q.defer();
                    var promise = prodservice.saveVariant(productVariant);

                    promise.then(function (savedProductVariant) {

                        productVariant = new merchello.Models.ProductVariant(savedProductVariant);

                        deferred.resolve(productVariant);

                    }, function (reason) {
                        deferred.reject(reason);
                    });

                    return deferred.promise;
                }
            };
    }]);
