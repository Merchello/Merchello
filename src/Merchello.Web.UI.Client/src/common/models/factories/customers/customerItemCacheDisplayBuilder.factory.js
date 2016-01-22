angular.module('merchello.models').factory('customerItemCacheDisplayBuilder',
    ['genericModelBuilder', 'customerDisplayBuilder', 'itemCacheLineItemDisplayBuilder', 'CustomerItemCacheDisplay',
    function(genericModelBuilder, customerDisplayBuilder, itemCacheLineItemDisplayBuilder, CustomerItemCacheDisplay) {

        var Constructor = CustomerItemCacheDisplay;
        return {
            createDefault: function() {
                var itemCache = new Constructor();
                return itemCache;
            },
            transform: function(jsonResult) {
                var itemCaches = [];
                if(angular.isArray(jsonResult)) {
                    for(var i = 0; i < jsonResult.length; i++) {
                        var itemCache = genericModelBuilder.transform(jsonResult[ i ], Constructor);
                        itemCache.customer = customerDisplayBuilder.transform(jsonResult[ i ].customer);
                        itemCache.items = itemCacheLineItemDisplayBuilder.transform(jsonResult[ i ].items);
                        itemCaches.push(itemCache);
                    }
                } else {
                    itemCaches = genericModelBuilder.transform(jsonResult, Constructor);
                    itemCaches.customer = customerDisplayBuilder.transform(jsonResult.customer);
                    itemCaches.items = itemCacheLineItemDisplayBuilder.transform(jsonResult.items);
                }
                return itemCaches;
            }
        };

}]);
