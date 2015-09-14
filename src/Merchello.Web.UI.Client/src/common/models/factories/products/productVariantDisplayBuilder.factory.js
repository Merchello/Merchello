    /**
     * @ngdoc models
     * @name productVariantDisplayBuilder
     *
     * @description
     * A utility service that builds ProductVariantDisplay models
     */
    angular.module('merchello.models').factory('productVariantDisplayBuilder',
        ['genericModelBuilder', 'productAttributeDisplayBuilder', 'catalogInventoryDisplayBuilder', 'productVariantDetachedContentDisplayBuilder', 'ProductVariantDisplay',
        function(genericModelBuilder, productAttributeDisplayBuilder, catalogInventoryDisplayBuilder, productVariantDetachedContentDisplayBuilder, ProductVariantDisplay) {

            var Constructor = ProductVariantDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var variants = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var variant = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            variant.attributes = productAttributeDisplayBuilder.transform(jsonResult[ i ].attributes);
                            variant.catalogInventories = catalogInventoryDisplayBuilder.transform(jsonResult[ i ].catalogInventories);
                            variant.detachedContents = productVariantDetachedContentDisplayBuilder.transform(jsonResult[i].detachedContents);
                            variants.push(variant);
                        }
                    } else {
                        variants = genericModelBuilder.transform(jsonResult, Constructor);
                        variants.attributes = productAttributeDisplayBuilder.transform(jsonResult.attributes);
                        variants.catalogInventories = catalogInventoryDisplayBuilder.transform(jsonResult.catalogInventories);
                        variants.detachedContents = productVariantDetachedContentDisplayBuilder.transform(jsonResult.detachedContents);
                    }
                    return variants;
                }
            };

    }]);
