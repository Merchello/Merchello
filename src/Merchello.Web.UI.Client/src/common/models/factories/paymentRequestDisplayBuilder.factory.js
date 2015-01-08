/**
 * @ngdoc service
 * @name merchello.models.paymentRequestDisplayBuilder
 *
 * @description
 * A utility service that builds PaymentRequestDisplay models
 */
angular.module('merchello.models')
    .factory('paymentRequestDisplayBuilder',
    ['genericModelBuilder', 'PaymentRequestDisplay',
        function(genericModelBuilder, PaymentRequestDisplay) {
            var Constructor = PaymentRequestDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);
