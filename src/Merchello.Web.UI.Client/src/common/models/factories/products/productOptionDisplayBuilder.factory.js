    /**
     * @ngdoc models
     * @name productOptionDisplayBuilder
     *
     * @description
     * A utility service that builds ProductOptionDisplay models
     */
    angular.module('merchello.models').factory('productOptionDisplayBuilder',
        ['genericModelBuilder', 'productAttributeDisplayBuilder', 'ProductOptionDisplay',
        function(genericModelBuilder, productAttributeDisplayBuilder, ProductOptionDisplay) {

            var Constructor = ProductOptionDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var options = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var option = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            option.choices = productAttributeDisplayBuilder.transform(jsonResult[ i ].choices);
                            options.push(option);
                        }
                    } else {
                        options = genericModelBuilder.transform(jsonResult, Constructor);
                        options.choices = productAttributeDisplayBuilder.transform(jsonResult.choices);
                    }


                    return options;
                }
            };

    }]);
