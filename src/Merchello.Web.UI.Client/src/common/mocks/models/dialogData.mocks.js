angular.module('merchello.mocks').
    factory('dialogDataMocks', ['addressMocks', 'paymentMocks',
        function (addressMocks, paymentMocks) {
            'use strict';

            // Private
            function getAddressDialogData() {
                var dialogData = {};
                var address = addressMocks.getSingleAddress();
                dialogData.address = address;
                return dialogData;
            }

            function getCapturePaymentDialogData() {
                var payments = paymentMocks.paymentsArray();
                var payment = payments[0];
                var data = {
                    paymentMethodKey: payment.paymentMethodKey,
                    paymentMethodName: payment.paymentMethodName,
                    remainingBalance: $scope.invoice(payments),
                    captureAmount: 0.0
                };
                return data;
            }

            // Public
            return {
                getAddressDialogData : getAddressDialogData,
                getCapturePaymentDialogData: getCapturePaymentDialogData
            };
        }]);
