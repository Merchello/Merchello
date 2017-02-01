/*! MerchelloPaymentProviders
 * https://github.com/Merchello/Merchello
 * Copyright (c) 2017 Across the Pond, LLC.
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
                //var filesToLoad = ['https://js.braintreegateway.com/v2/braintree.js'];
                var filesToLoad = [ 'https://js.braintreegateway.com/web/3.6.2/js/client.min.js',
                                    'https://js.braintreegateway.com/web/3.6.2/js/hosted-fields.js'];

                $scope.dialogData.warning = 'All credit card information is tokenized. No values are passed to the server.';

                var billingAddress = $scope.dialogData.invoice.getBillToAddress();
                //$scope.cardholderName = billingAddress.name;
                $scope.postalCode = billingAddress.postalCode;

                assetsService.load(filesToLoad).then(function () {
                    setupBraintree();
                });

                $scope.dialogData.amount = invoiceHelper.round($scope.dialogData.invoiceBalance, 2);
            }

            function setupBraintree() {
                var promise = braintreeResource.getClientRequestToken($scope.dialogData.invoice.customerKey);
                promise.then(function(requestToken) {

                    var form = document.querySelector('#cardForm');
                    var authorization = JSON.parse(requestToken);

                    braintree.client.create({
                        authorization: authorization
                    }, function(err, clientInstance) {
                        if (err) {
                            console.error(err);
                            return;
                        }
                        createHostedFields(clientInstance);
                    });

                    function createHostedFields(clientInstance) {

                        braintree.hostedFields.create({
                            client: clientInstance,
                            styles: {
                                'input': {
                                    'font-size': '16px',
                                    'font-family': 'courier, monospace',
                                    'font-weight': 'lighter',
                                    'color': '#ccc'
                                },
                                ':focus': {
                                    'color': 'black'
                                },
                                '.valid': {
                                    'color': '#8bdda8'
                                }
                            },
                            fields: {
                                number: {
                                    selector: '#card-number',
                                    placeholder: '4111 1111 1111 1111'
                                },
                                cvv: {
                                    selector: '#cvv',
                                    placeholder: '123'
                                },
                                expirationDate: {
                                    selector: '#expiration-date',
                                    placeholder: 'MM/YYYY'
                                },
                                postalCode: {
                                    selector: '#postal-code',
                                    placeholder: '11111'
                                }
                            }
                        }, function (err, hostedFieldsInstance) {

                            var teardown = function (event) {
                                event.preventDefault();

                                hostedFieldsInstance.tokenize(function (tokenizeErr, payload) {
                                    if (tokenizeErr) {
                                        switch (tokenizeErr.code) {
                                            case 'HOSTED_FIELDS_FIELDS_EMPTY':
                                                console.error('All fields are empty! Please fill out the form.');
                                                break;
                                            case 'HOSTED_FIELDS_FIELDS_INVALID':
                                                console.error('Some fields are invalid:', tokenizeErr.details.invalidFieldKeys);
                                                break;
                                            case 'HOSTED_FIELDS_FAILED_TOKENIZATION':
                                                console.error('Tokenization failed server side. Is the card valid?');
                                                break;
                                            case 'HOSTED_FIELDS_TOKENIZATION_NETWORK_ERROR':
                                                console.error('Network error occurred when tokenizing.');
                                                break;
                                            default:
                                                console.error('Something bad happened!', tokenizeErr);
                                        }
                                    } else {
                                        save(payload.nonce);
                                    }

                                    hostedFieldsInstance.teardown(function () {
                                        createHostedFields(clientInstance);
                                        form.removeEventListener('submit', teardown, false);
                                    });
                                });


                            };

                            form.addEventListener('submit', teardown, false);
                        });

                        $scope.loaded = true;
                    }

                });
            }


            function save(nonce) {
                $scope.wasFormSubmitted = true;
                if(invoiceHelper.valueIsInRage($scope.dialogData.amount, 0, $scope.dialogData.invoiceBalance)) {
                    $scope.dialogData.showSpinner();
                    $scope.dialogData.processorArgs.setValue('nonce-from-the-client', nonce);
                    $scope.submit($scope.dialogData);

                } else {
                    if(!invoiceHelper.valueIsInRage($scope.dialogData.amount, 0, $scope.dialogData.invoiceBalance)) {
                        $scope.cardForm.amount.$setValidity('amount', false);
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
                console.info(transactionStr);
                if (transactionStr !== '') {
                    $scope.transaction = JSON.parse(transactionStr);
                } else {
                    $scope.close();
                }

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