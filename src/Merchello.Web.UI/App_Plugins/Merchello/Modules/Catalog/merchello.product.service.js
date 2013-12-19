(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name umbraco.resources.MerchelloProductService
        * @description Loads in data for data types
        **/
    merchelloServices.MerchelloProductService = function ($q, $http, umbDataFormatter, umbRequestHelper, notificationsService, merchelloProductVariantService) {

        var prodservice = {

            possibleProductVariants: [],

            // Builds the possible variants
            // sets = array or arrays of choices
            // set = current iteration
            // permutation = array of variant combinations
            permute: function (sets, set, permutation) {
                for (var i = 0; i < sets[set].length; ++i) {
                    permutation[set] = sets[set][i];

                    if (set < (sets.length - 1)) {
                        prodservice.permute(sets, set + 1, permutation);
                    }
                    else {
                        prodservice.possibleProductVariants.push(permutation.slice(0));
                    }
                }
            },

            /// Server http requests
 
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

            },

            /// Business logic
            
            createProduct: function (product, notifyMethodCallback) {  // The notify callback is here until the angular version is revved to 1.2.x+

                var deferred = $q.defer();

                var promiseCreate = prodservice.create(product.name, product.sku, product.price);
                promiseCreate.then(function (newproduct) {
                    //deferred.notify("created");

                    product.key = newproduct.key;

                    // Created, now save the initial settings from the model
                    var promiseSave = prodservice.save(product);
                    promiseSave.then(function () {
                        //deferred.notify("saved");
                        notifyMethodCallback();

                        // Get updated product and options
                        var promiseProduct = prodservice.getByKey(product.key);
                        promiseProduct.then(function (dbproduct) {

                            product = new merchello.Models.Product(dbproduct);

                            deferred.resolve(product);

                        }, function (reason) {
                            deferred.reject(reason);
                        });

                    }, function (reason) {
                        deferred.reject(reason);
                    });

                }, function (reason) {
                    deferred.reject(reason);
                });

                return deferred.promise;
            },

            updateProduct: function (product) {

                var deferred = $q.defer();

                var promise = prodservice.save(product);

                promise.then(function () {
                    //deferred.notify("saved");

                    // Get updated product and options
                    var promiseProduct = prodservice.getByKey(product.key);
                    promiseProduct.then(function (dbproduct) {

                        product = new merchello.Models.Product(dbproduct);

                        deferred.resolve(product);

                    }, function (reason) {
                        deferred.reject(reason);
                    });

                }, function (reason) {
                    deferred.reject(reason);
                });

                return deferred.promise;
            },

            updateProductWithVariants: function (product, tocreatevariants) {

                var deferred = $q.defer();

                var promisesArray = [];

                // Save product
                promisesArray.push(prodservice.save(product));

                // Create Variants
                for (var v = 0; v < product.productVariants.length; v++) {
                    var currentVariant = product.productVariants[v];

                    if (currentVariant.key.length > 0)
                    {
                        promisesArray.push(merchelloProductVariantService.save(currentVariant));
                    }
                    else
                    {
                        if (currentVariant.selected) {
                            promisesArray.push(merchelloProductVariantService.create(currentVariant));
                        }
                    }
                }

                var promise = $q.all(promisesArray);

                promise.then(function () {
                    //deferred.notify("saved");

                    // Get updated product and options
                    var promiseProduct = prodservice.getByKey(product.key);
                    promiseProduct.then(function (dbproduct) {

                        product = new merchello.Models.Product(dbproduct);

                        deferred.resolve(product);

                    }, function (reason) {
                        deferred.reject(reason);
                    });

                }, function (reason) {
                    deferred.reject(reason);
                });

                return deferred.promise;
            },

            createVariantsFromOptions: function (product) {
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
            createVariantsFromDetachedOptionsList: function (product, options) {
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
    
    angular.module('umbraco.resources').factory('merchelloProductService', merchello.Services.MerchelloProductService);

}(window.merchello.Services = window.merchello.Services || {}));
