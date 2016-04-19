    /**
     * @ngdoc resource
     * @name orderResource
     * @description Loads in data and allows modification for orders
     **/
    angular.module('merchello.resources')
        .factory('orderResource', ['$http', 'umbRequestHelper',
            function($http, umbRequestHelper) {

            return {

                getOrder: function (orderKey) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloOrderApiBaseUrl'] + 'GetOrder';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: orderKey }
                        }),
                        'Failed to get order: ' + orderKey);
                },

                getOrdersByInvoice: function (invoiceKey) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloOrderApiBaseUrl'] + 'GetOrdersByInvoiceKey';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: invoiceKey }
                        }),
                        'Failed to get orders by invoice: ' + invoiceKey);
                },

                getUnFulfilledItems: function (invoiceKey) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloOrderApiBaseUrl'] + 'GetUnFulfilledItems';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: invoiceKey }
                        }),
                        'Failed to get unfulfilled items by invoice: ' + invoiceKey);
                },

                getShippingAddress: function (invoiceKey) {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloOrderApiBaseUrl'] + 'GetShippingAddress';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET",
                            params: { id: invoiceKey }
                        }),
                        'Failed to get orders by invoice: ' + invoiceKey);
                },

                processesProductsToBackofficeOrder: function (customerKey, products, shippingAddress, billingAddress) {
                    var model = {};
                    model.CustomerKey = customerKey;
                    model.ProductKeys = products;
                    model.ShippingAddress = shippingAddress;
                    model.BillingAddress = billingAddress;
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloOrderApiBaseUrl'] + 'ProcessesProductsToBackofficeOrder';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            model
                        ),
                        'Failed to add products to invoice');
                },

                getShippingMethods: function (customerKey, products, shippingAddress, billingAddress) {
                    var model = {};
                    model.CustomerKey = customerKey;
                    model.ProductKeys = products;
                    model.ShippingAddress = shippingAddress;
                    model.BillingAddress = billingAddress;
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloOrderApiBaseUrl'] + 'GetShippingMethods';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            model
                        ),
                        'Failed to get shipping methods');
                },

                getPaymentMethods: function () {
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloOrderApiBaseUrl'] + 'GetPaymentMethods';
                    return umbRequestHelper.resourcePromise(
                        $http({
                            url: url,
                            method: "GET"
                        }),
                        'Failed to get payment methods');
                },

                finalizeBackofficeOrder: function (customerKey, products, shippingAddress, billingAddress, paymentKey, paymentProviderKey, shipmentKey) {
                    var model = {};
                    model.CustomerKey = customerKey;
                    model.ProductKeys = products;
                    model.ShippingAddress = shippingAddress;
                    model.BillingAddress = billingAddress;
                    model.PaymentKey = paymentKey;
                    model.PaymentProviderKey = paymentProviderKey;
                    model.ShipmentKey = shipmentKey;
                    var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloOrderApiBaseUrl'] + 'FinalizeBackofficeOrder';
                    return umbRequestHelper.resourcePromise(
                        $http.post(url,
                            model
                        ),
                        'Failed to finalize backoffice order');
                }
            };

        }]);
