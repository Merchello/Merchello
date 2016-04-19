    /**
     * @ngdoc resource
     * @name notificationGatewayProviderResource
     * @description Loads in data for notification providers
     **/
    angular.module('merchello.resources').factory('notificationGatewayProviderResource',
        ['$http', 'umbRequestHelper',
            function($http, umbRequestHelper) {

                return {

                    getGatewayResources: function (key) {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'GetGatewayResources';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url + "?id=" + key,
                                method: "GET"
                            }),
                            'Failed to save data for Notification');
                    },

                    getAllGatewayProviders: function () {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'GetAllGatewayProviders';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url,
                                method: "GET"
                            }),
                            'Failed to retreive data for all gateway providers');
                    },

                    getAllNotificationMonitors: function () {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'GetAllNotificationMonitors';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url,
                                method: "GET"
                            }),
                            'Failed to retreive data for all gateway providers');
                    },

                    getNotificationProviderNotificationMethods: function (id) {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'GetNotificationProviderNotificationMethods';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url + "?id=" + id,
                                method: "GET"
                            }),
                            'Failed to save data for Notification');
                    },

                    addNotificationMethod: function (method) {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'AddNotificationMethod';
                        return umbRequestHelper.resourcePromise(
                            $http.post(
                                url,
                                angular.toJson(method)
                            ),
                            'Failed to save data for Notification');
                    },

                    saveNotificationMethod: function(method) {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'PutNotificationMethod';
                        return umbRequestHelper.resourcePromise(
                            $http.post(
                                url,
                                angular.toJson(method)
                            ),
                            'Failed to save data for Notification');
                    },
                    
                    deleteNotificationMethod: function (methodKey) {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'DeleteNotificationMethod';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url + "?id=" + methodKey,
                                method: "DELETE"
                            }),
                            'Failed to delete data for Notification');
                    },

                    getNotificationMessagesByKey: function (id) {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'GetNotificationMessagesByKey';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url + "?id=" + id,
                                method: "GET"
                            }),
                            'Failed to save data for Notification');

                    },

                    saveNotificationMessage: function (notification) {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'PutNotificationMessage';
                        return umbRequestHelper.resourcePromise(
                            $http.post(
                                url,
                                angular.toJson(notification)
                            ),
                            'Failed to save data for Notification');
                    },

                    deleteNotificationMessage: function (methodKey) {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'DeleteNotificationMessage';
                        return umbRequestHelper.resourcePromise(
                            $http({
                                url: url + "?id=" + methodKey,
                                method: "DELETE"
                            }),
                            'Failed to delete data for Notification');
                    },

                    updateNotificationMessage: function (notification) {
                        var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloNotificationApiBaseUrl'] + 'UpdateNotificationMessage';
                        return umbRequestHelper.resourcePromise(
                            $http.post(
                                url,
                                angular.toJson(notification)
                            ),
                            'Failed to save data for Notification');
                    }
                };
    }]);
