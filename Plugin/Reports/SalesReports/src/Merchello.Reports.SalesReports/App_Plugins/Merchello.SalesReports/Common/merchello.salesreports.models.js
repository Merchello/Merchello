(function() {

    var SaleByItemResult = function() {
        var self = this;
        self.productVariant = {};
        self.quantity = 0;
        self.total = 0;
    };

    angular.module('merchello.salesreports.models').constant('SalesByItemResult', SaleByItemResult);

    angular.module('merchello.salesreports.models').factory('saleByItemResultBuilder',
        ['genericModelBuilder', 'productVariantDisplayBuilder', 'SalesByItemResult',
        function(genericModelBuilder, productVariantDisplayBuilder, SalesByItemResult) {

            var Constructor = SalesByItemResult;

            return {
                createDefault: function () {
                    var result = new Constructor();
                    result.productVariant = productVariantDisplayBuilder.createDefault();
                    return result;
                },
                transform: function (jsonResult) {
                    var results = [];
                    if (angular.isArray(jsonResult)) {
                        for (var i = 0; i < jsonResult.length; i++) {
                            var result = genericModelBuilder.transform(jsonResult[i], Constructor);
                            result.productVariant = productVariantDisplayBuilder.transform(jsonResult[i].productVariant);
                        }
                    } else {
                        results = genericModelBuilder.transform(jsonResult, Constructor);
                        results.productVariant = productVariantDisplayBuilder.transform(jsonResult.productVariant);
                    }
                    return results;
                }
            };
    }]);

    var SaleOverTimeResult = function() {
        self.date = {};
        self.salesTotal = 0;
        self.salesCount = 0;
    };

    angular.module('merchello.salesreports.models').constant('SaleOverTimeResult', SaleOverTimeResult);

    angular.module('merchello.salesreports.models').factory('salesOverTimeResultBuilder',
        ['genericModelBuilder', 'SaleOverTimeResult',
            function(genericModelBuilder, SaleOverTimeResult) {

                var Constructor = SaleOverTimeResult;

                return {
                    createDefault: function () {
                        return new Constructor();
                    },
                    transform: function (jsonResult) {
                        return genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

}());
