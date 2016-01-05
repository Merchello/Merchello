angular.module('merchello.resources').factory('salesOverTimeResource',
    ['$http', '$q', 'umbRequestHelper', 'queryResultDisplayBuilder', 'salesOverTimeResultBuilder',
    function($http, $q, umbRequestHelper, queryResultDisplayBuilder, salesOverTimeResultBuilder) {

        var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloSalesOverTimeApiBaseUrl'];

        return {

            getDefaultReportData : function() {
                console.info(salesOverTimeResultBuilder);
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

                //return umbRequestHelper.resourcePromise(
                //    $http({
                //        url: baseUrl + 'GetDefaultReportData',
                //        method: "GET"
                //    }),
                //    'Failed to retreive default report data');
            }

        };

    }]);
