angular.module('merchello.resources').factory('vieweditorResource',
    ['$q', '$http', 'umbRequestHelper', 'pluginViewEditorContentBuilder',
    function($q, $http, umbRequestHelper, pluginViewEditorContentBuilder) {

        var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloPluginViewEditorApiBaseUrl'];

        return {

            getAllViews: function () {
                var url = baseUrl + 'GetAllAppPluginsViews';
                var deferred = $q.defer();
                $q.all([
                    umbRequestHelper.resourcePromise($http({ url: url, method: "GET" }), 'Failed to get all views')
                ]).then(function(data) {
                    var results = pluginViewEditorContentBuilder.transform(data[0]);
                    deferred.resolve(results);
                });

                return deferred.promise;
            },

            getAllNotificationViews: function() {
                var url = baseUrl + 'GetAllNotificationViews';
                var deferred = $q.defer();
                $q.all([
                    umbRequestHelper.resourcePromise($http({ url: url, method: "GET" }), 'Failed to get all notification views')
                ]).then(function(data) {
                    var results = pluginViewEditorContentBuilder.transform(data[0]);
                    deferred.resolve(results);
                });

                return deferred.promise;
            },

            addNewView: function(viewData) {
                var url = baseUrl + 'AddNewView';
                var deferred = $q.defer();
                
                $q.all([umbRequestHelper.resourcePromise(
                    $http.post(url,
                        viewData
                    ), 'Failed to create a notification view')])
                    .then(function(data) {
                        var results = pluginViewEditorContentBuilder.transform(data[0]);
                        deferred.resolve(results);
                });
                
                return deferred.promise;
            },

            saveView: function(viewData) {
                var url = baseUrl + 'SaveView';
                var deferred = $q.defer();

                $q.all([umbRequestHelper.resourcePromise(
                    $http.post(url,
                        viewData
                    ), 'Failed to save a notification view')])
                    .then(function(data) {
                        var results = pluginViewEditorContentBuilder.transform(data[0]);
                        deferred.resolve(results);
                    });

                return deferred.promise;
            }

        };
}]);
