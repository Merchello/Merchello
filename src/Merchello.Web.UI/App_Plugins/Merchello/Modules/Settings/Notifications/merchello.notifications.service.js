(function (merchelloServices, undefined) {


	/**
        * @ngdoc service
        * @name umbraco.resources.MerchelloNotificationsService
        * @description Loads in data for notifications
        **/
    merchelloServices.MerchelloNotificationsService = function ($q, $http, umbRequestHelper) {

		var notificationsService =  {

			getNotifications: function () {
				// This needs to make an api call to get real data ~Mark Bowser
				//var deferred = $q.defer();
				//deferred.resolve(mockTemplates);

				//return deferred.promise;

			    return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'GetNotifications'),
			            method: "GET"
			        }),
                    'Failed to retreive data for notifications');
			},

			getGatewayResources: function(key) {

		    return umbRequestHelper.resourcePromise(
		            $http({
		                url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'GetGatewayResources'),
                        method: "GET",
		                data: angular.toJson(key)
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
                        url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'GetAllNotificationTriggers'),
                        method: "GET"
                    }),
                    'Failed to retreive data for all gateway providers');
		    },

		    GetNotificationProviderNotificationMethods: function (id) {

		        return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'GetNotificationProviderNotificationMethods') + "?id=" + id,
                            method: "GET"
                        }),
                        'Failed to save data for Notification');
		    },

		    getNotificationMessage: function (id) {

		        return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'GetNotification') + "?id=" + id,
                            method: "GET"
                        }),
                        'Failed to save data for Notification');

		    },

		    getNotificationMessages: function (id) {

		        return umbRequestHelper.resourcePromise(
                        $http({
                            url: umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'GetNotificationMessages') + "?id=" + id,
                            method: "GET"
                        }),
                        'Failed to save data for Notification');

			},

			saveNotification: function (notification) {

				return umbRequestHelper.resourcePromise(
				   $http.post(
						umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'SaveNotificationMessage'),
						angular.toJson(notification)
					),
					'Failed to save data for Notification');
			},

		    saveNotificationMethod: function (method) {

		    return umbRequestHelper.resourcePromise(
               $http.post(
                    umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'AddNotificationMethod'),
                    angular.toJson(method)
                ),
                'Failed to save data for Notification');
		}
		};

		return notificationsService;
	};

	angular.module('umbraco.resources').factory('merchelloNotificationsService', ['$q', '$http', 'umbRequestHelper', merchello.Services.MerchelloNotificationsService]);

}(window.merchello.Services = window.merchello.Services || {}));
