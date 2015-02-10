    /**
     * @ngdoc service
     * @name merchello.models.appliedPaymentDisplayBuilder
     *
     * @description
     * A utility service that builds applieddPaymentDisplaybuilder
     */
    angular.module('merchello.models')
        .factory('appliedPaymentDisplayBuilder',
        ['genericModelBuilder', 'AppliedPaymentDisplay',
            function(genericModelBuilder, AppliedPaymentDisplay) {

                var Constructor = AppliedPaymentDisplay;

                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);
