/*! MerchelloPaymentProviders
 * https://github.com/Merchello/Merchello
 * Copyright (c) 2016 Across the Pond, LLC.
 * Licensed MIT
 */

(function() { 

angular.module('merchello.providers').controller('Merchello.Providers.Dialogs.BraintreeProviderSettingsController',
    ['$scope', 'braintreeProviderSettingsBuilder',
        function($scope, braintreeProviderSettingsBuilder) {

            $scope.providerSettings = {};


            function init() {
                var json = JSON.parse($scope.dialogData.provider.extendedData.getValue('braintreeProviderSettings'));
                $scope.providerSettings = braintreeProviderSettingsBuilder.transform(json);

                $scope.$watch(function () {
                    return $scope.providerSettings;
                }, function (newValue, oldValue) {
                    $scope.dialogData.provider.extendedData.setValue('braintreeProviderSettings', angular.toJson(newValue));
                }, true);

            }

            // initialize the controller
            init();

        }]);

angular.module('merchello.providers').controller('Merchello.Providers.Dialogs.BraintreeStandardAuthorizeCapturePaymentController',
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
            $scope.braintreeClient = {};

            // Exposed methods
            $scope.save = save;



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
                    $scope.braintreeClient = new braintree.api.Client(setup);
                    $scope.loaded = true;
                    console.info($scope.dialogData);
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
                if(invoiceHelper.valueIsInRage($scope.dialogData.amount, 0, $scope.dialogData.invoiceBalance) && $scope.authCaptureForm.cardholderName.$valid
                    && $scope.authCaptureForm.cardNumber.$valid && $scope.authCaptureForm.cvv.$valid && $scope.authCaptureForm.postalCode.$valid) {

                    var cc = braintreeCreditCardBuilder.createDefault();
                    cc.cardholderName = $scope.cardholderName;
                    cc.number = $scope.cardNumber;
                    cc.cvv = $scope.cvv;
                    cc.expirationMonth = invoiceHelper.padLeft($scope.selectedMonth.monthNumber + 1, '0', 2);
                    cc.expirationYear = $scope.expirationYear;
                    cc.billingAddress.postalCode = $scope.postalCode;
                    $scope.dialogData.showSpinner();
                    $scope.braintreeClient.tokenizeCard(cc, function (err, nonce) {
                        // Send nonce to your server
                        if(err !== null) {
                            console.info(err);
                            return;
                        }
                        $scope.dialogData.processorArgs.setValue('nonce-from-the-client', nonce);
                        $scope.submit($scope.dialogData);
                    });
                } else {
                    if(!invoiceHelper.valueIsInRage($scope.dialogData.amount, 0, $scope.dialogData.invoiceBalance)) {
                        $scope.authCaptureForm.amount.$setValidity('amount', false);
                    }
                }
            }


            // Initialize the controller
            init();
        }]);


angular.module('merchello.providers').controller('Merchello.Providers.Dialogs.BraintreeStandardCapturePaymentController',
    ['$scope', 'invoiceHelper',
        function($scope, invoiceHelper) {

            $scope.transaction = {};

            function init() {
                $scope.dialogData.amount = invoiceHelper.round($scope.dialogData.invoiceBalance, 2);
                var transactionStr = $scope.dialogData.payment.extendedData.getValue('braintreeTransaction');
                $scope.transaction = JSON.parse(transactionStr);
                $scope.dialogData.warning = 'This action will submit a previously authorized transaction for settlement.';
            }


            init();
            // initialize the controller
        }]);


angular.module('merchello.providers').controller('Merchello.Providers.Dialogs.RefundPaymentController',
    ['$scope', 'invoiceHelper',
        function($scope, invoiceHelper) {

            $scope.wasFormSubmitted = false;
            $scope.save = save;

            function init() {
                $scope.dialogData.amount = invoiceHelper.round($scope.dialogData.appliedAmount, 2);
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


angular.module('merchello.providers').controller('Merchello.Providers.Dialogs.PayPalProviderSettingsController',
    ['$scope', 'payPalProviderSettingsBuilder',
        function($scope, payPalProviderSettingsBuilder) {

            $scope.providerSettings = {};


            function init() {

                var json = JSON.parse($scope.dialogData.provider.extendedData.getValue('paypalprovidersettings'));
                $scope.providerSettings = payPalProviderSettingsBuilder.transform(json);

                console.info($scope.providerSettings);

                $scope.$watch(function () {
                    return $scope.providerSettings;
                }, function (newValue, oldValue) {
                    $scope.dialogData.provider.extendedData.setValue('paypalprovidersettings', angular.toJson(newValue));
                }, true);

            }

            // initialize the controller
            init();

        }]);

})();