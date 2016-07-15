angular.module('merchello.resources').factory('productOptionResource',
    ['$q', '$http', 'umbRequestHelper',
        function($q, $http, umbRequestHelper) {

            return {

                /**
                 * @ngdoc method
                 * @name searchOptions
                 * @description Searches for all product options with a ListQuery object
                 **/
                searchOptions: function (query) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloProductOptionApiBaseUrl'] + 'SearchOptions';
                    return umbRequestHelper.resourcePromise(
                        $http.post(
                            url,
                            query
                        ),
                        'Failed to search product options');
                }


            }

        }]);