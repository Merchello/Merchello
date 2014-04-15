(function(merchelloServices, undefined) {


    /**
    * @ngdoc service
    * @name umbraco.resources.MerchelloProductService
    * @description Loads in data for data types
    **/
    merchelloServices.MerchelloProductService = function($q, $http, umbRequestHelper, notificationsService, merchelloProductVariantService) {

        var prodservice = {
            possibleProductVariants: [],

            // Builds the possible variants
            // sets = array or arrays of choices
            // set = current iteration
            // permutation = array of variant combinations
            permute: function(sets, set, permutation) {
                for (var i = 0; i < sets[set].length; ++i) {
                    permutation[set] = sets[set][i];

                    if (set < (sets.length - 1)) {
                        prodservice.permute(sets, set + 1, permutation);
                    } else {
                        prodservice.possibleProductVariants.push(permutation.slice(0));
                    }
                }
            },

            /// Server http requests

            /**
            * @ngdoc method
            * @name create
            * @description Creates a new product with an API call to the server using the name, sku, and price
            **/
            create: function (productname, sku, price) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'NewProduct'),
                        method: "POST",
                        params: { sku: sku, name: productname, price: price }
                    }),
                    'Failed to create product sku ' + sku);
            },

            /**
            * @ngdoc method
            * @name createFromProduct
            * @description Creates a new product with an API call to the server
            **/
            createFromProduct: function (product) {

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'NewProductFromProduct'),
                        product
                    ),
                    'Failed to create product sku ' + sku);
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
            * @name getByKey
            * @description Gets all products in the data store with an API call to the server
            **/
            getAllProducts: function () {

                return umbRequestHelper.resourcePromise(
                    $http.get(
                        umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'GetAllProducts')
                    ),
                    'Failed to get all products');

            },

            /**
            * @ngdoc method
            * @name getByKey
            * @description Gets all products matching teh 'term' in the data store with an API call to the server
            **/
            filterProducts: function (term) {

                return umbRequestHelper.resourcePromise(
                    $http.get(
                        umbRequestHelper.getApiUrl('merchelloProductApiBaseUrl', 'GetFilteredProducts'),
                        //'/umbraco/Merchello/ProductApi/GetFilteredProducts',
                        { params: { term: term } }
                    ),
                    'Failed to get filtered products');

            },

            /// Business logic

            /**
            * @ngdoc method
            * @name createProduct
            * @description Creates product and delivers the new Product model in the promise data
            **/
            createProduct: function(product, notifyMethodCallback) {

                var deferred = $q.defer();

                var promiseCreate = prodservice.createFromProduct(product);
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

                promise.then(function() {

                    // Get updated product and options
                    var promiseProduct = prodservice.getByKey(product.key);
                    promiseProduct.then(function(dbproduct) {

                        product = new merchello.Models.Product(dbproduct);

                        deferred.resolve(product);

                    }, function(reason) {
                        deferred.reject(reason);
                    });

                }, function(reason) {
                    deferred.reject(reason);
                });

                return deferred.promise;
            },

            updateProductWithVariants: function(product, tocreatevariants) {

                var deferred = $q.defer();

                var promisesArray = [];

                // Save product
                promisesArray.push(prodservice.save(product));

                // Create Variants
                for (var v = 0; v < product.productVariants.length; v++) {
                    var currentVariant = product.productVariants[v];

                    if (currentVariant.key.length > 0) {
                        promisesArray.push(merchelloProductVariantService.save(currentVariant));
                    } else {
                        if (currentVariant.selected) {
                            promisesArray.push(merchelloProductVariantService.create(currentVariant));
                        }
                    }
                }

                var promise = $q.all(promisesArray);

                promise.then(function() {
                    //deferred.notify("saved");

                    // Get updated product and options
                    var promiseProduct = prodservice.getByKey(product.key);
                    promiseProduct.then(function(dbproduct) {

                        product = new merchello.Models.Product(dbproduct);

                        deferred.resolve(product);

                    }, function(reason) {
                        deferred.reject(reason);
                    });

                }, function(reason) {
                    deferred.reject(reason);
                });

                return deferred.promise;
            },

            createVariantsFromOptions: function(product) {
                var choiceSets = [];
                var permutation = [];
                prodservice.possibleProductVariants = [];

                for (var i = 0; i < product.productOptions.length; i++) {
                    var currentOption = product.productOptions[i];
                    choiceSets.push(currentOption.choices);
                    permutation.push('');
                }

                product.productVariants = [];

                prodservice.permute(choiceSets, 0, permutation);

                for (var p = 0; p < prodservice.possibleProductVariants.length; p++) {
                    var variant = prodservice.possibleProductVariants[p];

                    // Todo: check if already exists?
                    var merchVariant = product.addVariant(variant);

                    merchVariant.fixAttributeSortOrders(product.productOptions);
                }

                return product;
            },

            // Used when duplicating variants with a new option
            createVariantsFromDetachedOptionsList: function(product, options) {
                var choiceSets = [];
                var permutation = [];
                prodservice.possibleProductVariants = [];

                for (var i = 0; i < options.length; i++) {
                    var currentOption = options[i];
                    choiceSets.push(currentOption.choices);
                    permutation.push('');
                }

                prodservice.permute(choiceSets, 0, permutation);

                for (var p = 0; p < prodservice.possibleProductVariants.length; p++) {
                    var variant = prodservice.possibleProductVariants[p];

                    // Todo: check if already exists?
                    var merchVariant = product.addVariant(variant);

                    merchVariant.fixAttributeSortOrders(options);
                }

                return product;
            }

        };

        return prodservice;
    };

    angular.module('umbraco.resources').factory('merchelloProductService', ['$q', '$http', 'umbRequestHelper', 'notificationsService', 'merchelloProductVariantService', merchello.Services.MerchelloProductService]);

}(window.merchello.Services = window.merchello.Services || {}));