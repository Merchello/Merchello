angular.module('merchello.resources').factory('productOptionResource',
    ['$q', '$http', 'umbRequestHelper', 'queryResultDisplayBuilder',
        'productOptionDisplayBuilder', 'productOptionUseCountBuilder', 'productAttributeDisplayBuilder',
        function($q, $http, umbRequestHelper, queryResultDisplayBuilder,
                 productOptionDisplayBuilder, productOptionUseCountBuilder, productAttributeDisplayBuilder) {

            var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductOptionApiBaseUrl'];

            return {

                getOptionUiSettings: function() {
                    var url = baseUrl + 'GetOptionUiSettings';
                  return umbRequestHelper.resourcePromise(
                    $http({
                       url: url,
                        method: "GET"
                    }),
                      'Failed to get the option ui settings');
                },

                getByKey: function(key) {
                    var deferred = $q.defer();
                    var url = baseUrl + 'GetByKey';
                    umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: key }
                        }),
                        'Failed to get product option by key').then(function(data) {
                        var result = productOptionDisplayBuilder.transform(data);
                        deferred.resolve(result);
                    });

                    return deferred.promise;
                },

                /**
                 * @ngdoc method
                 * @name searchOptions
                 * @description Searches for all product options with a ListQuery object
                 **/
                searchOptions: function (query) {
                    var url =  baseUrl + 'SearchOptions';

                    var deferred = $q.defer();
                    umbRequestHelper.resourcePromise(
                        $http.post(
                            url,
                            query
                        ),
                        'Failed to search product options')
                        .then(function(data) {
                            var result = queryResultDisplayBuilder.transform(data, productOptionDisplayBuilder);
                            deferred.resolve(result);
                        });

                    return deferred.promise;
                },

                getUseCounts: function(option) {
                    var url = baseUrl + 'GetProductOptionUseCount';

                    var deferred = $q.defer();
                    umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: option.key }
                        }),
                        'Failed to retreive default report data')
                        .then(function(data) {
                            var counts = productOptionUseCountBuilder.transform(data);
                            deferred.resolve(counts);
                        });

                    return deferred.promise;
                },


                /**
                 * @ngdoc method
                 * @name addProductOption
                 * @description adds a 'shared' product option
                 **/
                addProductOption: function(option) {
                    var url = baseUrl + 'PostProductOption';

                    angular.forEach(option.choices, function(c) {
                        c.detachedDataValues = c.detachedDataValues.asDetachedValueArray();
                    });

                    var deferred = $q.defer();
                    umbRequestHelper.resourcePromise(
                        $http.post(url,
                            option
                        ),
                        'Failed to create new product option')
                        .then(function(po) {
                            var result = productOptionDisplayBuilder.transform(po);
                            deferred.resolve(result);
                        });

                    return deferred.promise;
                },

                saveProductOption: function(option) {
                    var url = baseUrl + 'PutProductOption';

                    angular.forEach(option.choices, function(c) {
                        c.detachedDataValues = c.detachedDataValues.asDetachedValueArray();
                    });

                    var deferred = $q.defer();
                    umbRequestHelper.resourcePromise(
                        $http.post(url,
                            option
                        ),
                        'Failed to save product option')
                        .then(function(po) {
                            var result = productOptionDisplayBuilder.transform(po);
                            deferred.resolve(result);
                        });

                    return deferred.promise;
                },

                saveAttributeContent: function(args, files) {

                    var attribute = args.content;
                    var contentType = args.contentType;

                    attribute.detachedDataValues = attribute.detachedDataValues.asDetachedValueArray();


                    var url = baseUrl + 'PutProductAttributeDetachedContent';
                    var deferred = $q.defer();
                    umbRequestHelper.postMultiPartRequest(
                        url,
                        { key: "detachedContentItem", value: { display: attribute, detachedContentType: contentType } },
                        function (data, formData) {
                            //now add all of the assigned files
                            for (var f in files) {
                                //each item has a property alias and the file object, we'll ensure that the alias is suffixed to the key
                                // so we know which property it belongs to on the server side
                                formData.append("file_" + files[f].alias, files[f].file);
                            }
                        },
                        function (data, status, headers, config) {

                            var choice = productAttributeDisplayBuilder.transform(data);
                            deferred.resolve(choice);

                        }, function(reason) {
                            deferred.reject('Failed to save product attribute ' + reason);
                        });

                    return deferred.promise;

                },

                /**
                 * @ngdoc method
                 * @name deleteProductOption
                 * @description deletes a shared product option.  options associated with products
                 * should be removed from the productOptions collection rather than deleted through this service.
                 **/
                deleteProductOption : function(option) {
                    var url = baseUrl + 'DeleteProductOption';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: option.key }
                        }),
                        'Failed to delete option');
                }

            };

        }]);