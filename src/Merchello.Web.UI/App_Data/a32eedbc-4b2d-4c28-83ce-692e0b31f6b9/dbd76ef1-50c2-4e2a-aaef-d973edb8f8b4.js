(function() {

    var SaleByItemResult = function() {
        var self = this;
        self.productVariant = {};
        self.quantity = 0;
        self.total = 0;
    };

    angular.module('merchello.salesreports.models').constant('SalesByItemResult', SaleByItemResult);

    angular.module('merchello.salesreports.models').factory('saleByItemResultBuilder',
        ['genrericModelBuilder', 'productVariantDisplayBuilder', 'SalesByItemResult',
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
        self.salestotal = 0;
        self.salescount = 0;
    };

    angular.module('merchello.salesreports.models').constant('SaleOverTimeResult', SaleOverTimeResult);

    angular.module('merchello.salesreports.models').factory('saleByItemResultBuilder',
        ['genrericModelBuilder', 'SalesByItemResult',
            function(genericModelBuilder, SalesOverTimeResult) {

                var Constructor = SalesOverTimeResult;

                return {
                    createDefault: function () {
                        return new Constructor();
                    },
                    transform: function (jsonResult) {
                        var results = genericModelBuilder.transform(jsonResult, Constructor);
                    }
                };
            }]);

}());
