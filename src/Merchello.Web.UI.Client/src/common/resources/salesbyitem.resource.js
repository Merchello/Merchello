angular.module('merchello.resources').factory('salesByItemResource',
    ['$http', '$q', 'umbRequestHelper', 'queryDisplayBuilder', 'queryResultDisplayBuilder', 'salesByItemResultBuilder',
    function($http, $q, umbRequestHelper, queryDisplayBuilder, queryResultDisplayBuilder, salesByItemResultBuilder) {

        var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloSalesByItemApiBaseUrl'];

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

                        var results = queryResultDisplayBuilder.transform(data[0], salesByItemResultBuilder);
                        deferred.resolve(results);
                    });

                return deferred.promise;

            },

            getCustomReportData : function(query) {

                if (query === undefined) {
                    query = queryDisplayBuilder.createDefault();
                }

                var url = baseUrl + 'GetCustomDateRange';
                var deferred = $q.defer();
                $q.all([
                        umbRequestHelper.resourcePromise(
                            $http.post(url, query),
                            'Failed to retreive custom report data')])
                    .then(function(data) {
                        var results = queryResultDisplayBuilder.transform(data[0], salesByItemResultBuilder);
                        deferred.resolve(results);
                    });

                return deferred.promise;
            }
        };
}]);
