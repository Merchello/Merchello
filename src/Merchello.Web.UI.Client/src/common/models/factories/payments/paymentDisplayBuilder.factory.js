    /**
     * @ngdoc service
     * @name merchello.models.paymentDisplayBuilder
     *
     * @description
     * A utility service that builds PaymentDisplay models
     */
    angular.module('merchello.models')
        .factory('paymentDisplayBuilder',
        ['genericModelBuilder', 'appliedPaymentDisplayBuilder', 'extendedDataDisplayBuilder', 'PaymentDisplay',
            function(genericModelBuilder, appliedPaymentDisplayBuilder, extendedDataDisplayBuilder, PaymentDisplay) {

                var Constructor = PaymentDisplay;

                return {
                    createDefault: function() {
                        var payment = new Constructor();
                        payment.extendedData = extendedDataDisplayBuilder.createDefault();
                        return payment;
                    },
                    transform: function(jsonResult) {
                        var payments = [];
                        if (angular.isArray(jsonResult)) {
                            for(var i = 0; i < jsonResult.length; i++) {
                                var payment = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                                payment.appliedPayments = appliedPaymentDisplayBuilder.transform(jsonResult[ i ].appliedPayments);
                                payment.extendedData = extendedDataDisplayBuilder.transform(jsonResult[ i ].extendedData);
                                payments.push(payment);
                            }
                        } else {
                            payments = genericModelBuilder.transform(jsonResult, Constructor);
                            payments.appliedPayments = appliedPaymentDisplayBuilder.transform(jsonResult.appliedPayments);
                            payments.extendedData = extendedDataDisplayBuilder.transform(jsonResult.extendedData);
                        }
                        return payments;
                    }
                };
            }]);