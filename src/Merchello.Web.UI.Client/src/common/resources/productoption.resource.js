angular.module('merchello.resources').factory('productOptionResource',
    ['$q', '$http', 'umbRequestHelper', 'queryResultDisplayBuilder', 'productOptionDisplayBuilder', 'productOptionUseCountBuilder',
        function($q, $http, umbRequestHelper, queryResultDisplayBuilder, productOptionDisplayBuilder, productOptionUseCountBuilder) {

            var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductOptionApiBaseUrl'];

            return {

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