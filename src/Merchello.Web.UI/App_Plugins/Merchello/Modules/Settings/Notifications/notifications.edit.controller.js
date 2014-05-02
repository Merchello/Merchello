(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.NotificationsEditController
     * @function
     * 
     * @description
     * The controller for the Notifications page
     */
	controllers.NotificationsEditController = function ($scope, $routeParams, assetsService, merchelloNotificationsService) {
		$scope.loaded = true;
		$scope.preValuesLoaded = true;

		//--------------------------------------------------------------------------------------
		// Initialization methods
		//--------------------------------------------------------------------------------------

		$scope.init = function () {
			assetsService.loadCss("/App_Plugins/Merchello/lib/codemirror/Js/Lib/codemirror.css");
			assetsService.loadCss("/App_Plugins/Merchello/lib/codemirror/Css/umbracoCustom.css");

			$scope.loadEmailTemplates();

			$scope.cmOptions = {
				autofocus: true,
				indentUnit: 4,
				indentWithTabs: true,
				lineNumbers: true,
				matchBrackets: true,
				mode: "razor",
				value: $scope.cmModel
			}

		};

        $scope.loadEmailTemplates = function() {

        	var promise = merchelloNotificationsService.getNotification($routeParams.id);
        	promise.then(function (notification) {
        		$scope.emailTemplate = new merchello.Models.EmailTemplate(notification);
        	});

	        $scope.cmModel = ';; Scheme code in here.\n' +
				'(define (double x)\n\t(* x x))\n\n\n' +
				'<!-- XML code in here. -->\n' +
				'<root>\n\t<foo>\n\t</foo>\n\t<bar/>\n</root>\n\n\n' +
				'// Javascript code in here.\n' +
				'function foo(msg) {\n\tvar r = Math.random();\n\treturn "" + r + " : " + msg;\n}';

        };

		//--------------------------------------------------------------------------------------
		// Events methods
		//--------------------------------------------------------------------------------------

		$scope.save = function(emailTemplate) {
			// Note From Kyle: An API call will need to be wired in here to edit the existing Email Template in the database.
			var stuff = $scope.cmModel;
		};

		//--------------------------------------------------------------------------------------
		// Dialog methods
		//--------------------------------------------------------------------------------------

	};


	angular.module("umbraco").controller("Merchello.Dashboards.Settings.NotificationsEditController", ['$scope', '$routeParams', 'assetsService', 'merchelloNotificationsService', merchello.Controllers.NotificationsEditController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
