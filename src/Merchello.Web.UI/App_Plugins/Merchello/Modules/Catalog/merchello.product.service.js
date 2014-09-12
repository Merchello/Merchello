(function(merchelloServices, undefined) {


    /**
    * @ngdoc service
    * @name umbraco.resources.MerchelloProductService
    * @description Loads in data for data types
    **/
    merchelloServices.MerchelloProductService = function($q, $http, umbRequestHelper, notificationsService) {

        var prodservice = {
            possibleProductVariants: [],

            ///////////////////////////////////////////////////////////////////////////////////////////
            /// Helper methods
            ///////////////////////////////////////////////////////////////////////////////////////////

            ///**
            //* @ngdoc method
            //* @name permute
            //* @description 
            //* Builds the possible variants
            //*    - sets = array or arrays of choices
            //*    - set = current iteration
            //*    - permutation = array of variant combinations
            //**/
            //permute: function (sets, set, permutation) {
            //    for (var i = 0; i < sets[set].length; ++i) {
            //        permutation[set] = sets[set][i];

            //        if (set < (sets.length - 1)) {
            //            prodservice.permute(sets, set + 1, permutation);
            //        } else {
            //            prodservice.possibleProductVariants.push(permutation.slice(0));
            //        }
            //    }
            //},



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
            },


            ///**
            //* @ngdoc method
            //* @name createVariantsFromOptions
            //* @description Used to build the variants permutations when a new option has been added or an option was deleted
            //**/
            //createVariantsFromOptions: function (product) {
            //    var choiceSets = [];
            //    var permutation = [];
            //    prodservice.possibleProductVariants = [];

            //    for (var i = 0; i < product.productOptions.length; i++) {
            //        var currentOption = product.productOptions[i];
            //        choiceSets.push(currentOption.choices);
            //        permutation.push('');
            //    }

            //    product.productVariants = [];

            //    prodservice.permute(choiceSets, 0, permutation);

            //    for (var p = 0; p < prodservice.possibleProductVariants.length; p++) {
            //        var variant = prodservice.possibleProductVariants[p];

            //        // Todo: check if already exists?
            //        var merchVariant = product.addVariant(variant);

            //        merchVariant.fixAttributeSortOrders(product.productOptions);
            //    }

            //    return product;
            //},

            ///**
            //* @ngdoc method
            //* @name createVariantsFromDetachedOptionsList
            //* @description Used to build the variants permutations. Used when duplicating variants with a new option.
            //**/
            //createVariantsFromDetachedOptionsList: function (product, options) {
            //    var choiceSets = [];
            //    var permutation = [];
            //    prodservice.possibleProductVariants = [];

            //    for (var i = 0; i < options.length; i++) {
            //        var currentOption = options[i];
            //        choiceSets.push(currentOption.choices);
            //        permutation.push('');
            //    }

            //    prodservice.permute(choiceSets, 0, permutation);

            //    for (var p = 0; p < prodservice.possibleProductVariants.length; p++) {
            //        var variant = prodservice.possibleProductVariants[p];

            //        // Todo: check if already exists?
            //        var merchVariant = product.addVariant(variant);

            //        merchVariant.fixAttributeSortOrders(options);
            //    }

            //    return product;
            //}

        };

        return prodservice;
    };

    angular.module('umbraco.resources').factory('merchelloProductService', ['$q', '$http', 'umbRequestHelper', 'notificationsService', merchello.Services.MerchelloProductService]);

}(window.merchello.Services = window.merchello.Services || {}));