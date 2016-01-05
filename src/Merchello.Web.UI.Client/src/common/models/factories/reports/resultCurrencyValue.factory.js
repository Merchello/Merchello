/**
 * @ngdoc factory
 * @name resultCurrencyValueBuilder
 * @function
 *
 * @description
 * Builds a ResultCurrencyValue
 */
angular.module('merchello.models').factory('resultCurrencyValueBuilder',
    ['genericModelBuilder', 'currencyDisplayBuilder', 'ResultCurrencyValue',
    function(genericModelBuilder, currencyDisplayBuilder, ResultCurrencyValue) {

        var Constructor = ResultCurrencyValue;

        return {
            createDefault: function() {
                var result = new Constructor();
                result.currency = currencyDisplayBuilder.createDefault();
            },
            transform: function(jsonResult) {
                var results = [];
                if (angular.isArray(jsonResult)) {
                    for(var i = 0; i < jsonResult.length; i++) {
                        var result = genericModelBuilder.transform(jsonResult[i], Constructor);
                        result.currency = currencyDisplayBuilder.transform(jsonResult[i].currency);
                        results.push(result);
                    }
                } else {
                    results = genericModelBuilder.transform(jsonResult, Constructor);
                    results.currency = currencyDisplayBuilder.transform(jsonResult.currency);
                }
                return results;
            }
        };

}]);
