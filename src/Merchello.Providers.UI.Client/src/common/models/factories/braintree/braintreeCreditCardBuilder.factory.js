
// OBSOLETE
angular.module('merchello.providers.models').factory('braintreeCreditCardBuilder',
    ['genericModelBuilder', 'BraintreeCreditCard',
        function(genericModelBuilder, BraintreeCreditCard) {

            var Constructor = BraintreeCreditCard;

            return {
                createDefault: function () {
                    return new Constructor();
                },
                transform: function (jsonResult) {
                    return genericModelBuilder.transform(jsonResult, Constructor);
                }
            };
        }]);
