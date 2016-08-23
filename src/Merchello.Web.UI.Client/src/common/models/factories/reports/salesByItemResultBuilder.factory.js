angular.module('merchello.models').factory('salesByItemResultBuilder',
    ['genericModelBuilder', 'resultCurrencyValueBuilder', 'productVariantDisplayBuilder', 'SalesByItemResult',
    function(genericModelBuilder, resultCurrencyValueBuilder, productVariantDisplayBuilder, SalesByItemResult) {

        var Constructor = SalesByItemResult;

        return {
            createDefault: function() {
                return new Constructor();
            },
            transform: function(jsonResult) {
                var models = [];
                if(angular.isArray(jsonResult)) {
                    for(var i = 0; i < jsonResult.length; i++) {
                        var model = genericModelBuilder.transform(jsonResult[i], Constructor);
                        model.productVariant = productVariantDisplayBuilder.transform(jsonResult[i].productVariant);
                        model.totals = resultCurrencyValueBuilder.transform(jsonResult[i].totals);
                        models.push(model);
                    }
                } else {
                    models = genericModelBuilder.transform(jsonResult, Constructor);
                    models.productVariant = productVariantDisplayBuilder.transform(jsonResult.productVariant);
                    models.totals = resultCurrencyValueBuilder.transform(jsonResult.totals);
                }
                return models;
            }
        };
}]);
