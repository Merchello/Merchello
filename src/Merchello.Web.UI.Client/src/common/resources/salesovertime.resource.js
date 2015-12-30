angular.module('merchello.resources').factory('salesOverTimeResource',
    ['$http', 'umbRequestHelper',
    function($http, umbRequestHelper) {

        var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloSalesOverTimeApiBaseUrl'];

        return {

            getDefaultReportData : function() {
                console.info(baseUrl);
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: baseUrl + 'GetDefaultReportData',
                        method: "GET"
                    }),
                    'Failed to retreive default report data');
            }

        };

    }]);
