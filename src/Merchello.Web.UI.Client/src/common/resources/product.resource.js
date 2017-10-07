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
                // TODO this method is obsolete but it is still possible to get here so leave it
                // Remove in version 3.0.0 or in Angular 2.x refactor
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
                 * @name add
                 * @description Creates a new product with an API call to the server
                 **/
                create: function(product) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'CreateProduct';
                    angular.forEach(product.detachedContents, function(dc) {
                        dc.detachedDataValues = dc.detachedDataValues.asDetachedValueArray();
                    });
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            product
                        ),
                        'Failed to create product sku ' + product.sku);
                },

                /**
                 * @ngdoc method
                 * @name getByKey
                 * @description Gets a value indicating whether or not a SKU exists
                 **/
                getSkuExists: function(sku) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'GetSkuExists';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { sku: sku }
                        }),
                        'Failed to test SKU');
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

                /**
                 * @ngdoc method
                 * @name getBySku
                 * @description Gets a product via it's SKU with an API call to the server
                 **/
                getBySku: function (sku) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'GetProductBySku';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url + '?sku=' + sku,
                            method: "GET"
                        }),
                        'Failed to retreive data for product sku ' + sku);
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
                 * @name getVariantBySku
                 * @description Gets a product variant via it's sku with an API call to the server
                 **/
                getVariantBySku: function (sku) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'GetProductVariantBySku';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url + '?sku=' + sku,
                            method: "GET"
                        }), 'Failed to retreive data for product variant sku ' + sku);
                },

                getManufacturers: function()
                {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'GetAllManufacturers';
                    return umbRequestHelper.resourcePromise(
                        $http({url: url, method: 'GET' }), 'Failed to retreive list of manufacturers'
                    );
                },

                /**
                 * @ngdoc method
                 * @name save
                 * @description Saves / updates product with an api call back to the server
                 **/
                save: function (product) {

                    product.prepForSave();

                    /*
                    angular.forEach(product.detachedContents, function(dc) {
                        dc.detachedDataValues = dc.detachedDataValues.asDetachedValueArray();
                    });

                    angular.forEach(product.productOptions, function(po) {
                        angular.forEach(po.choices, function(c) {
                            c.detachedDataValues = c.detachedDataValues.asDetachedValueArray();
                        })
                    });
                    */
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'PutProduct';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            product
                        ),
                        'Failed to save data for product key ' + product.key);
                },

                saveProductContent: function(args, files) {

                    var product = args.content;
                    var cultureName = args.scope.language.isoCode;
                    product.prepForSave();


                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'PutProductWithDetachedContent';
                    var deferred = $q.defer();
                    umbRequestHelper.postMultiPartRequest(
                        url,
                        { key: "detachedContentItem", value: { display: product, cultureName: cultureName} },
                        function (data, formData) {
                            //now add all of the assigned files
                            for (var f in files) {
                                //each item has a property alias and the file object, we'll ensure that the alias is suffixed to the key
                                // so we know which property it belongs to on the server side
                                formData.append("file_" + files[f].alias, files[f].file);
                            }
                        },
                        function (data, status, headers, config) {

                            deferred.resolve(data);

                        }, function(reason) {
                            deferred.reject('Failed to save product content ' + reason);
                        });

                    return deferred.promise;

                },


                /**
                 * @ngdoc method
                 * @name saveVariant
                 * @description Saves / updates product variant with an api call back to the server
                 **/
                saveVariant: function (productVariant) {

                    productVariant.prepForSave();

                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'PutProductVariant';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            productVariant
                        ),
                        'Failed to save data for product variant key ' + productVariant.key);
                },

                saveVariantContent: function(args, files) {

                    var productVariant = args.content;
                    var cultureName = args.scope.language.isoCode;
                    productVariant.prepForSave();

                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'PutProductVariantWithDetachedContent';

                    var deferred = $q.defer();
                    umbRequestHelper.postMultiPartRequest(
                        url,
                        { key: "detachedContentItem", value: { display: productVariant, cultureName: cultureName} },
                        function (data, formData) {
                            //now add all of the assigned files
                            for (var f in files) {
                                //each item has a property alias and the file object, we'll ensure that the alias is suffixed to the key
                                // so we know which property it belongs to on the server side
                                formData.append("file_" + files[f].alias, files[f].file);
                            }
                        },
                        function (data, status, headers, config) {
                            deferred.resolve(data);
                        }, function(reason) {
                            deferred.reject(reason);
                        });

                    return deferred.promise;
                },

                copyProduct: function(product, name, sku) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'PostCopyProduct';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            { product: product, name: name, sku: sku }
                        ),
                        'Failed to delete detached content');
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

                resetSkus: function(product)
                {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'PutProductWithResetSkus';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { productKey: product.key }
                        }),
                        'Failed to reset skus');
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
                },

                advancedSearchProducts: function(query) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'GetByAdvancedSearch';
                    return umbRequestHelper.resourcePromise(
                        $http.post(
                            url,
                            query
                        ),
                        'Failed to advanced search products');
                },

                getRecentlyUpdated: function(query) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductApiBaseUrl'] + 'GetRecentlyUpdated';
                    return umbRequestHelper.resourcePromise(
                        $http.post(
                            url,
                            query
                        ),
                        'Failed to get recently updated products');
                }

            };
    }]);
