angular.module('merchello.resources').factory('salesOverTimeResource',
    ['$http', '$q', 'umbRequestHelper', 'queryResultDisplayBuilder', 'salesOverTimeResultBuilder',
    function($http, $q, umbRequestHelper, queryResultDisplayBuilder, salesOverTimeResultBuilder) {

        var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloSalesOverTimeApiBaseUrl'];

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

                        var results = queryResultDisplayBuilder.transform(data[0], salesOverTimeResultBuilder);
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
                        var results = queryResultDisplayBuilder.transform(data[0], salesOverTimeResultBuilder);
                        deferred.resolve(results);
                    });

                return deferred.promise;
            },

            getWeeklyResult : function(query) {

                if (query === undefined) {
                    query = queryDisplayBuilder.createDefault();
                }

                var url = baseUrl + 'GetWeeklyResult';

                var deferred = $q.defer();
                $q.all([
                        umbRequestHelper.resourcePromise(
                            $http.post(url, query),
                            'Failed to retreive weekly report data')])
                    .then(function(data) {
                        var results = queryResultDisplayBuilder.transform(data[0], salesOverTimeResultBuilder);
                        deferred.resolve(results);
                    });

                return deferred.promise;
            }


        };

    }]);
