angular.module('merchello.models').factory('basketDisplayBuilder',
    ['genericModelBuilder', 'customerDisplayBuilder', 'invoiceLineItemDisplayBuilder', 'BasketDisplay',
    function(genericModelBuilder, customerDisplayBuilder, invoiceLineItemDisplayBuilder, BasketDisplay) {

        var Constructor = BasketDisplay;
        return {
            createDefault: function() {
                var basket = new Constructor();
                return basket;
            },
            transform: function(jsonResult) {
                var baskets = [];
                if(angular.isArray(jsonResult)) {
                    for(var i = 0; i < jsonResult.length; i++) {
                        var basket = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                        basket.customer = customerDisplayBuilder.transform(jsonResult[ i ].customer);
                        basket.items = invoiceLineItemDisplayBuilder.transform(jsonResult[ i ].items);
                        baskets.push(basket);
                    }
                } else {
                    baskets = genericModelBuilder.transform(jsonResult, Constructor);
                    baskets.customer = customerDisplayBuilder.transform(jsonResult.customer);
                    baskets.items = invoiceLineItemDisplayBuilder.transform(jsonResult.items);
                }
                return baskets;
            }
        };

}]);
