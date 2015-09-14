    /**
     * @ngdoc models
     * @name productDisplayBuilder
     *
     * @description
     * A utility service that builds ProductDisplay models
     */
    angular.module('merchello.models').factory('productDisplayBuilder',
        ['genericModelBuilder', 'productVariantDisplayBuilder', 'productOptionDisplayBuilder', 'catalogInventoryDisplayBuilder',
            'productVariantDetachedContentDisplayBuilder', 'ProductDisplay',
        function(genericModelBuilder, productVariantDisplayBuilder, productOptionDisplayBuilder, catalogInventoryDisplayBuilder,
                 productVariantDetachedContentDisplayBuilder, ProductDisplay) {

            var Constructor = ProductDisplay;
            return {
                createDefault: function() {
                    return new Constructor();
                },
                transform: function(jsonResult) {
                    var products = [];
                    if(angular.isArray(jsonResult)) {
                        for(var i = 0; i < jsonResult.length; i++) {
                            var product = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                            product.productVariants = productVariantDisplayBuilder.transform(jsonResult[ i ].productVariants);
                            product.productOptions = productOptionDisplayBuilder.transform(jsonResult[ i ].productOptions);
                            product.catalogInventories = catalogInventoryDisplayBuilder.transform(jsonResult[ i ].catalogInventories);
                            product.detachedContents = productVariantDetachedContentDisplayBuilder.transform(jsonResult[i].detachedContents);
                            products.push(product);
                        }
                    } else {
                        products = genericModelBuilder.transform(jsonResult, Constructor);
                        products.productVariants = productVariantDisplayBuilder.transform(jsonResult.productVariants);
                        products.productOptions = productOptionDisplayBuilder.transform(jsonResult.productOptions);
                        products.catalogInventories = catalogInventoryDisplayBuilder.transform(jsonResult.catalogInventories);
                        products.detachedContents = productVariantDetachedContentDisplayBuilder.transform(jsonResult.detachedContents);
                    }
                    return products;

                }
            };

    }]);
