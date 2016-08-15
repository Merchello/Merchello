    /**
     * @ngdoc models
     * @name productAttributeDisplayBuilder
     *
     * @description
     * A utility service that builds ProductAttributeDisplay models
     */
    angular.module('merchello.models').factory('productAttributeDisplayBuilder',
        ['genericModelBuilder', 'extendedDataDisplayBuilder', 'ProductAttributeDisplay',
        function(genericModelBuilder, extendedDataDisplayBuilder, ProductAttributeDisplay) {

            var Constructor = ProductAttributeDisplay;
            return {
                createDefault: function() {
                    var att = new Constructor();
                    att.detachedDataValues = extendedDataDisplayBuilder.createDefault();
                    return att;
                },
                transform: function(jsonResult) {
                    var results = [];
                    if (angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var result = genericModelBuilder.transform(jsonResult[i], Constructor);
                            result.detachedDataValues = extendedDataDisplayBuilder.transform(jsonResult[i].detachedDataValues);
                            results.push(result);
                        }
                    } else {
                        results = genericModelBuilder.transform(jsonResult, Constructor);
                        results.detachedDataValues = extendedDataDisplayBuilder.transform(jsonResult.detachedDataValues);
                    }
                    return results;
                }
            };
    }]);
