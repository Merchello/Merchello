(function (merchelloServices, undefined) {


	/**
        * @ngdoc service
        * @name umbraco.resources.MerchelloNotificationsService
        * @description Loads in data for notifications
        **/
	merchelloServices.MerchelloNotificationsService = function ($q, $http, umbRequestHelper) {


		// This is mock data for the notifications. ~Mark Bowser
		var mockTemplates = [
		{
			pk: 0,
			name: "Order Confirmation",
			description: "Sent to the customers and users requesting notification (set below).",
			header: "Thank you for ordering through Geeky Soap! We're so excited to share our geekiness with you. Enjoy your products and if you have any questions, call our customer service line at 1-888-531-1234!",
			footer: "XOXO, <br/> The Geeky Soap Team"
		},
		{
			pk: 1,
			name: "Order Shipped",
			description: "Sent to the customer upon order fulfillment.",
			header: "",
			footer: ""
		},
		{
			pk: 2,
			name: "Problems with Payment Auth",
			description: "Sent with request to contact support when credit card is denied or there is an error such as wrong billing address.",
			header: "",
			footer: ""
		},
		{
			pk: 3,
			name: "Payment Received",
			description: "Sent to customers and users requesting notification upon payment processing.",
			header: "",
			footer: ""
		},
		{
			pk: 4,
			name: "Order Canceled",
			description: "Sent to the customer after an order is manually canceled.",
			header: "",
			footer: ""
		}];

		var notificationsService =  {

			getNotifications: function () {
				// This needs to make an api call to get real data ~Mark Bowser
				var deferred = $q.defer();
				deferred.resolve(mockTemplates);

				return deferred.promise;

				//return umbRequestHelper.resourcePromise(
				//     $http.get(
				//        umbRequestHelper.getApiUrl('merchelloNotificationsApiBaseUrl', 'GetNotifications')
				//    ),
				//    'Failed to get all Notifications');
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
						umbRequestHelper.getApiUrl('merchelloNotificationApiBaseUrl', 'PutNotification'),
						angular.toJson(notification)
					),
					'Failed to save data for Notification');
			}

		};

		return notificationsService;
	};

	angular.module('umbraco.resources').factory('merchelloNotificationsService', ['$q', '$http', 'umbRequestHelper', merchello.Services.MerchelloNotificationsService]);

}(window.merchello.Services = window.merchello.Services || {}));
