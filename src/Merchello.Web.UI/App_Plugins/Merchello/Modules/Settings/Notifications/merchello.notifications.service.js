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
				var deferred = $q.defer();
				//deferred.resolve(mockTemplates);

				return deferred.promise;

				//return umbRequestHelper.resourcePromise(
				//     $http.get(
				//        umbRequestHelper.getApiUrl('merchelloNotificationsApiBaseUrl', 'GetNotifications')
				//    ),
				//    'Failed to get all Notifications');
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

			getNotification: function (id) {
				// This needs to make an api call to get data from api instead of from getNotifications probably ~Mark Bowser
				var deferred = $q.defer();

				var promise = notificationsService.getNotifications();
				promise.then(function (notifications) {
					var notification = _.find(notifications, function (n) {
						return n.pk == id;
					});

					deferred.resolve(notification);
				}, function (reason) {
					deferred.reject(reason);
				});

				return deferred.promise;

				//return umbRequestHelper.resourcePromise(
                //    $http({
                //    	url: umbRequestHelper.getApiUrl('merchelloNotificationsApiBaseUrl', 'GetNotification'),
                //    	method: "GET",
                //    	params: { id: id }
                //    }),
                //    'Failed to retreive data for notification: ' + id);
			},

			saveNotification: function (notification) {

				return umbRequestHelper.resourcePromise(
				   $http.post(
						umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'SaveNotificationMessage'),
						angular.toJson(notification)
					),
					'Failed to save data for Notification');
			}
		};

		return notificationsService;
	};

	angular.module('umbraco.resources').factory('merchelloNotificationsService', ['$q', '$http', 'umbRequestHelper', merchello.Services.MerchelloNotificationsService]);

}(window.merchello.Services = window.merchello.Services || {}));
