(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.NotificationsEditController
     * @function
     * 
     * @description
     * The controller for the Notifications page
     */
	controllers.NotificationsEditController = function ($scope, $routeParams, assetsService) {
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

            // Note From Kyle: A mock of getting the Email Template objects.
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
                }
            ];
            $scope.emailTemplates = _.map(mockTemplates, function(emailTemplateFromServer) {
                return new merchello.Models.EmailTemplate(emailTemplateFromServer);
            });
        	// End of Mocks

	        $scope.emailTemplate = _.find($scope.emailTemplates, function(template) {
		        return template.pk == $routeParams.id;
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


	angular.module("umbraco").controller("Merchello.Dashboards.Settings.NotificationsEditController", ['$scope', '$routeParams', 'assetsService', merchello.Controllers.NotificationsEditController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
