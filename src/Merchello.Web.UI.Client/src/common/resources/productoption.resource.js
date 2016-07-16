angular.module('merchello.resources').factory('productOptionResource',
    ['$q', '$http', 'umbRequestHelper', 'queryResultDisplayBuilder', 'productOptionDisplayBuilder',
        function($q, $http, umbRequestHelper, queryResultDisplayBuilder, productOptionDisplayBuilder) {

            var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductOptionApiBaseUrl'];

            return {

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
                }

            };

        }]);