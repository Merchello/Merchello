(function (merchelloServices, undefined) {


	/**
        * @ngdoc service
        * @name umbraco.resources.MerchelloNotificationsService
        * @description Loads in data for notifications
        **/
    merchelloServices.MerchelloNotificationsService = function ($q, $http, umbRequestHelper) {

		var notificationsService =  {

			getGatewayResources: function(key) {

		    return umbRequestHelper.resourcePromise(
		            $http({
		                url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'GetGatewayResources') + "?id=" + key,
                        method: "GET"
		            }),
                    'Failed to save data for Notification');
			},

			getAllGatewayProviders: function () {

			    return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'GetAllGatewayProviders'),
                        method: "GET"
                    }),
                    'Failed to retreive data for all gateway providers');
			},

		    getAllNotificationTriggers: function() {
		        return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'GetAllNotificationMonitors'),
                        method: "GET"
                    }),
                    'Failed to retreive data for all gateway providers');
		    },

		    getNotificationProviderNotificationMethods: function (id) {

		        return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'GetNotificationProviderNotificationMethods') + "?id=" + id,
                            method: "GET"
                        }),
                        'Failed to save data for Notification');
		    },

		    saveNotificationMethod: function (method) {

		        return umbRequestHelper.resourcePromise(
                   $http.post(
                        umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'AddNotificationMethod'),
                        angular.toJson(method)
                    ),
                    'Failed to save data for Notification');
		    },

		    deleteNotificationMethod: function (methodKey) {

		        return umbRequestHelper.resourcePromise(
                   $http({
                       url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'DeleteNotificationMethod') + "?id=" + methodKey,
                       method: "DELETE"
                   }),
                    'Failed to delete data for Notification');
		    },

		    getNotificationMessagesByKey: function (id) {

		        return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'GetNotificationMessagesByKey') + "?id=" + id,
                            method: "GET"
                        }),
                        'Failed to save data for Notification');

		    },

		    saveNotificationMessage: function (notification) {

		        return umbRequestHelper.resourcePromise(
				   $http.post(
						umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'PutNotificationMessage'),
						angular.toJson(notification)
					),
					'Failed to save data for Notification');
		    },

		    deleteNotificationMessage: function (methodKey) {

		        return umbRequestHelper.resourcePromise(
                   $http({
                       url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'DeleteNotificationMessage') + "?id=" + methodKey,
                       method: "DELETE"
                   }),
                    'Failed to delete data for Notification');
		    },

			updateNotificationMessage: function (notification) {

			    return umbRequestHelper.resourcePromise(
				   $http.post(
						umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'UpdateNotificationMessage'),
						angular.toJson(notification)
					),
					'Failed to save data for Notification');
			}

		};

		return notificationsService;
	};

	angular.module('umbraco.resources').factory('merchelloNotificationsService', ['$q', '$http', 'umbRequestHelper', merchello.Services.MerchelloNotificationsService]);

}(window.merchello.Services = window.merchello.Services || {}));
