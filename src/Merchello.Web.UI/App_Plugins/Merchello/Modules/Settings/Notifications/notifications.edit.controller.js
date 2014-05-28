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

        $scope.currentTab = "Template";
        $scope.notificationTriggers = [];
        if ($scope.notificationMessage == undefined) {
            $scope.notificationMessage = new merchello.Models.NotificationMessage();
        }

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

			var promise = $scope.loadNotificationMessage();
		    promise.then(function(data) {

		    	$scope.cmOptions = {
		    		autofocus: true,
		    		indentUnit: 4,
		    		indentWithTabs: true,
		    		lineNumbers: true,
		    		matchBrackets: true,
		    		mode: "razor",
		    		value: $scope.notificationMessage.bodyText
		    	};

		    	$scope.loaded = true;
		    	$scope.preValuesLoaded = true;
		    });
			
		};
		$scope.loadAllNotificationTriggers = function () {
		    var promise = merchelloNotificationsService.getAllNotificationTriggers();
		    promise.then(function (notificationTriggers) {
		        $scope.notificationTriggers = notificationTriggers;
		    });
		};

        $scope.loadNotificationMessage = function() {

            var promise = merchelloNotificationsService.getNotificationMessagesByKey($routeParams.id);
        	promise.then(function (notification) {
        	    $scope.notificationMessage = new merchello.Models.NotificationMessage(notification);
        	});

	        return promise;
        };

		//--------------------------------------------------------------------------------------
		// Events methods
		//--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name paymentMethodDialogConfirm
         * @function
         * 
         * @description
         * Handles the save after recieving the edited method from the dialog view/controller
         */
		$scope.saveNotificationMessage = function (message) {
		    var promiseSave;
		    message.monitorKey = message.monitorKey;
		    promiseSave = merchelloNotificationsService.updateNotificationMessage(message);

		    promiseSave.then(function () {
		        notificationsService.success("Payment Method Saved");
		        window.location.hash = "#/merchello/merchello/Notifications/manage";
		    }, function (reason) {
		        notificationsService.error("Payment Method Save Failed", reason.message);
		    });
		};

		$scope.selectTab = function (tabname) {
		    $scope.currentTab = tabname;
		};
		//--------------------------------------------------------------------------------------
		// Dialog methods
		//--------------------------------------------------------------------------------------


		$scope.loadAllNotificationTriggers();
        $scope.loadNotificationMessage();
    };


	angular.module("umbraco").controller("Merchello.Dashboards.Settings.NotificationsEditController", ['$scope', '$routeParams', 'assetsService', 'notificationsService', 'merchelloNotificationsService', merchello.Controllers.NotificationsEditController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
