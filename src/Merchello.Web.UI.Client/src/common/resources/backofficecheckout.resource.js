angular.module('merchello.resources').factory('backOfficeCheckoutResource',
    ['$http', '$q', 'umbRequestHelper', 'customerItemCacheDisplayBuilder',
    function($http, $q, umbRequestHelper, customerItemCacheDisplayBuilder) {

        var baseUrl = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloBackOfficeCheckoutApiBaseUrl'];

        return {

            addItemCacheItem : function(instruction) {
                var url = baseUrl + 'AddItemCacheItem';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        instruction
                    ),
                    'Failed to add item to the item cache');
            },

            removeItemCacheItem : function(instruction) {

                var url = baseUrl + 'RemoveItemCacheItem';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        instruction
                    ),
                    'Failed to remove item from the item cache');
            },

            updateLineItemQuantity : function(instruction) {

                var url = baseUrl + 'UpdateLineItemQuantity';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        instruction
                    ),
                    'Failed to update item quantity');
            },

            moveToWishlist : function(instruction) {
                var url = baseUrl + 'MoveToWishlist';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        instruction
                    ),
                    'Failed to move item to wish list');
            },

            moveToBasket : function(instruction) {
                var url = baseUrl + 'MoveToBasket';
                return umbRequestHelper.resourcePromise(
                    $http.post(url,
                        instruction
                    ),
                    'Failed to move item to basket');
            }

        };

    }]);
