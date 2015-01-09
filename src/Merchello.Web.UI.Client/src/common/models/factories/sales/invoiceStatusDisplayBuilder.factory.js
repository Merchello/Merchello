    /**
     * @ngdoc service
     * @name merchello.models.invoiceStatusDisplayBuilder
     *
     * @description
     * A utility service that builds InvoiceStatusDisplay models
     */
    angular.module('merchello.models')
        .factory('invoiceStatusDisplayBuilder',
        ['genericModelBuilder', 'InvoiceStatusDisplay',
            function(genericModelBuilder, InvoiceStatusDisplay) {
                var Constructor = InvoiceStatusDisplay;
                return {
                    createDefault: function() {
                        return new Constructor();
                    },
                    transform: function(jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);
