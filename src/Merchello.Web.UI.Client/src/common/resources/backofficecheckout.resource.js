angular.module('merchello.resources').factory('backOfficeCheckoutResource',
    ['$http', '$q', 'umbRequestHelper', 'customerItemCacheDisplayBuilder', 'invoiceDisplayBuilder', 'shipmentRateQuoteDisplayBuilder',
    function($http, $q, umbRequestHelper, customerItemCacheDisplayBuilder, invoiceDisplayBuilder, shipmentRateQuoteDisplayBuilder) {

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
                    $http.post(url, instruction),
                    'Failed to update item quantity');
            },

            createCheckoutInvoice: function(model) {
                var url = baseUrl + 'CreateCheckoutInvoice';

                var defer = $q.defer();

                umbRequestHelper.resourcePromise(
                    $http.post(url, model),
                    'Failed to update item quantity')
                    .then(function(result) {
                        var invoice = invoiceDisplayBuilder.transform(result);
                        defer.resolve(invoice);
                    });

                return defer.promise;
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
            },

            getShipmentRateQuotes: function(customerKey) {
                var url = baseUrl + 'GetShipmentRateQuotes';

                var defer = $q.defer();

                umbRequestHelper.resourcePromise(
                    $http({
                        url: url,
                        method: "GET",
                        params: { customerKey: customerKey }
                    }),
                    'Failed to quote shipments for customer basket')
                    .then(function(result) {
                        var quotes = shipmentRateQuoteDisplayBuilder.transform(result);
                        defer.resolve(quotes);
                    });

                return defer.promise;
            },

            getCheckoutStages: function() {
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: baseUrl + 'GetCheckoutStages',
                        method: "GET"
                    }),
                    'Failed to get checkout stages');
            }

        };

    }]);
