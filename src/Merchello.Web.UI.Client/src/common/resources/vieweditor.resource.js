angular.module('merchello.resources').factory('vieweditorResource',
    ['$q', '$http', 'umbRequestHelper',
    function($q, $http, umbRequestHelper) {

        var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloPluginViewEditorApiBaseUrl'];

        return {

            getAllViews: function () {
                var url = baseUrl + 'GetAllAppPluginsViews';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET"
                    }),
                    'Failed to get all views');
            },

            getAllNotificationViews: function() {
                var url = baseUrl + 'GetAllNotificationViews';
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET"
                    }),
                    'Failed to get all notification views');
            }

        };
}]);
