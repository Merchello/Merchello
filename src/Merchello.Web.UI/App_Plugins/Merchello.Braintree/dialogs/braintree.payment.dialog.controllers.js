(function() {

    angular.module('merchello.plugins.braintree').controller('Merchello.Plugins.Braintree.Dialogs.Standard.AuthorizeCapturePaymentController',
        ['$scope', 'assetsService', 'invoiceHelper', 'braintreeResource', 'braintreeCreditCardBuilder',
        function($scope, assetsService, invoiceHelper, braintreeResource, braintreeCreditCardBuilder) {

            $scope.loaded = false;
            $scope.wasFormSubmitted = false;
            $scope.cardholderName = '';
            $scope.cardNumber = '';
            $scope.selectedMonth = {};
            $scope.expirationYear = '';
            $scope.cvv = '';
            $scope.postalCode = '';
            $scope.months = [];
            $scope.years = [];

            // Exposed methods
            $scope.save = save;

            var braintreeClient = {};

            function init() {
                var filesToLoad = ['https://js.braintreegateway.com/v2/braintree.js'];
                $scope.dialogData.warning = 'All credit card information is tokenized. No values are passed to the server.';

                var billingAddress = $scope.dialogData.invoice.getBillToAddress();
                $scope.cardholderName = billingAddress.name;
                $scope.postalCode = billingAddress.postalCode;
                loadMonths();
                loadYears();
                assetsService.load(filesToLoad).then(function () {
                    setupBraintree();
                });
                $scope.dialogData.amount = invoiceHelper.round($scope.dialogData.invoiceBalance, 2);
            }

            function setupBraintree() {
                var promise = braintreeResource.getClientRequestToken($scope.dialogData.invoice.customerKey);
                promise.then(function(requestToken) {
                    var setup = {
                        clientToken: JSON.parse(requestToken)
                    };
                    console.info(setup);
                    braintreeClient = braintree.api.Client(setup);
                    $scope.loaded = true;
                });
            }

            function loadMonths() {
                var d = new Date();
                var monthNames = [ "January", "February", "March", "April", "May", "June",
                    "July", "August", "September", "October", "November", "December" ]
                for(var i = 0; i < monthNames.length; i++) {
                    $scope.months.push({ monthNumber: i,  monthName: monthNames[i] })
                }
            }

            function loadYears() {
                var d = new Date();
                var start = d.getFullYear();
                for(var i = 0; i < 15; i++) {
                    $scope.years.push(start + i);
                }
            }

            function save() {
                $scope.wasFormSubmitted = true;
                if(invoiceHelper.valueIsInRage($scope.dialogData.amount, 0, $scope.dialogData.appliedAmount)) {

                    var cc = braintreeCreditCardBuilder.createDefault();


                } else {
                    $scope.refundForm.amount.$setValidity('amount', false);
                }
            }


            // Initialize the controller
            init();
        }]);

    angular.module('merchello.plugins.braintree').controller('Merchello.Plugins.Braintree.Dialogs.Standard.VoidPaymentController',
        ['$scope',
        function($scope) {

            function init() {
                $scope.dialogData.warning = 'Please note this will only void the store payment record and this DOES NOT pass any information onto Braintree.'
            }

            // initialize the controller
            init();
        }]);

    angular.module('merchello').controller('Merchello.Plugins.Braintree.Dialogs.Dialogs.RefundPaymentController',
        ['$scope', 'invoiceHelper',
            function($scope, invoiceHelper) {

                $scope.wasFormSubmitted = false;
                $scope.save = save;

                function init() {
                    $scope.dialogData.amount = invoiceHelper.round($scope.dialogData.appliedAmount, 2);
                    $scope.dialogData.warning = 'Please note this operation will refund process a refund with Braintree.';
                }

                function save() {
                    $scope.wasFormSubmitted = true;
                    if(invoiceHelper.valueIsInRage($scope.dialogData.amount, 0, $scope.dialogData.appliedAmount)) {
                        $scope.submit($scope.dialogData);
                    } else {
                        $scope.refundForm.amount.$setValidity('amount', false);
                    }
                }
                // initializes the controller
                init();
            }]);

}());