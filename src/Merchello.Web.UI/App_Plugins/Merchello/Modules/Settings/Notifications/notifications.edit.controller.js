(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.NotificationsEditController
     * @function
     * 
     * @description
     * The controller for the Notifications page
     */
	controllers.NotificationsEditController = function ($scope, $routeParams, assetsService, notificationsService, merchelloNotificationsService) {
		$scope.loaded = true;
		$scope.preValuesLoaded = true;

		//--------------------------------------------------------------------------------------
		// Initialization methods
		//--------------------------------------------------------------------------------------

		$scope.init = function () {
			assetsService.loadCss("/App_Plugins/Merchello/lib/codemirror/Js/Lib/codemirror.css");
			assetsService.loadCss("/App_Plugins/Merchello/lib/codemirror/Css/umbracoCustom.css");

			// declare empty options at startup, so ui-codemirror can watch it
			$scope.cmOptions = {};

			var promise = $scope.loadEmailTemplates();
			promise.then(function(data) {

				$scope.cmOptions = {
					autofocus: true,
					indentUnit: 4,
					indentWithTabs: true,
					lineNumbers: true,
					matchBrackets: true,
					mode: "razor",
					value: $scope.emailTemplate.description
				};

				$scope.loaded = true;
				$scope.preValuesLoaded = true;
			});
			
		};

        $scope.loadEmailTemplates = function() {

        	var promise = merchelloNotificationsService.getNotification($routeParams.id);
        	promise.then(function (notification) {
        		$scope.emailTemplate = new merchello.Models.EmailTemplate(notification);
        	});

	        return promise;
        };

		//--------------------------------------------------------------------------------------
		// Events methods
		//--------------------------------------------------------------------------------------

		$scope.save = function(emailTemplate) {
			var promise = merchelloNotificationsService.saveNotification($scope.emailTemplate);
			promise.then(function (notification) {
				notificationsService.success("Notification Saved", "H5YR!");
			}, function (reason) {
				notificationsService.error("Notification Save Failed", reason.message);
			});
		};

		//--------------------------------------------------------------------------------------
		// Dialog methods
		//--------------------------------------------------------------------------------------

	};


	angular.module("umbraco").controller("Merchello.Dashboards.Settings.NotificationsEditController", ['$scope', '$routeParams', 'assetsService', 'notificationsService', 'merchelloNotificationsService', merchello.Controllers.NotificationsEditController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
