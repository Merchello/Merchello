angular.module('merchello.resources').factory('abandonedBasketResource',
    ['$http', '$q', 'umbRequestHelper', 'queryResultDisplayBuilder', 'abandonedBasketResultBuilder',
    function($http, $q, umbRequestHelper, queryResultDisplayBuilder, abandonedBasketResultBuilder) {

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

            }
        };

}]);
