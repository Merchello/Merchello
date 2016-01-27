angular.module('merchello.resources').factory('abandonedBasketResource',
    ['$http', '$q', 'umbRequestHelper', 'queryResultDisplayBuilder', 'abandonedBasketResultBuilder', 'customerItemCacheDisplayBuilder',
    function($http, $q, umbRequestHelper, queryResultDisplayBuilder, abandonedBasketResultBuilder, customerItemCacheDisplayBuilder) {

        var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloAbandonedBasketApiBaseUrl'];

        return {
            getDefaultReportData : function() {

                var deferred = $q.defer();
                $q.all([
                        umbRequestHelper.resourcePromise(
                            $http({
                                url: baseUrl + 'GetDefaultReportData',
                                method: "GET"
                            }),
                            'Failed to retreive default report data')])
                    .then(function(data) {

                        var results = queryResultDisplayBuilder.transform(data[0], abandonedBasketResultBuilder);
                        deferred.resolve(results);
                    });

                return deferred.promise;
            },

            getCustomerSavedBasketsLegacy : function(query) {

                if (query === undefined) {
                    query = queryDisplayBuilder.createDefault();
                    query.applyInvoiceQueryDefaults();
                }
                var url = baseUrl + 'GetCustomerSavedBaskets';
                return umbRequestHelper.resourcePromise(
                    $http.post(url, query),
                    'Failed to retreive customer basket data');

            },

            getCustomerSavedBaskets : function(query) {
                if (query === undefined) {
                    query = queryDisplayBuilder.createDefault();
                }

                var url = baseUrl + 'GetCustomerSavedBaskets';
                var deferred = $q.defer();
                $q.all([
                        umbRequestHelper.resourcePromise(
                            $http.post(url, query),
                            'Failed to retreive customer basket data')])
                    .then(function(data) {
                        var results = queryResultDisplayBuilder.transform(data[0], customerItemCacheDisplayBuilder);
                        deferred.resolve(results);
                    });

                return deferred.promise;
            }
        };

}]);
