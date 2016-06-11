angular.module('merchello.models').factory('salesOverTimeResultBuilder',
    ['genericModelBuilder', 'resultCurrencyValueBuilder', 'SalesOverTimeResult',
        function(genericModelBuilder, resultCurrencyValueBuilder, SalesOverTimeResult) {

            var Constructor = SalesOverTimeResult;

            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var models = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var model = genericModelBuilder.transform(jsonResult[i], Constructor);
                            model.totals = resultCurrencyValueBuilder.transform(jsonResult[i].totals);
                            models.push(model);
                        }
                    } else {
                        models = genericModelBuilder.transform(jsonResult, Constructor);
                        models.totals = resultCurrencyValueBuilder.transform(jsonResult.totals);
                    }
                    return models;
                }
            };
}]);