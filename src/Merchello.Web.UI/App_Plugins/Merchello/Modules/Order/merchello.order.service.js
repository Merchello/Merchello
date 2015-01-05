(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name merchello.Services.MerchelloOrderService
        * @description Loads in data and allows modification for orders
        **/
    merchelloServices.MerchelloOrderService = function ($http, umbRequestHelper) {

        return {

            getOrder: function (orderKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetOrder'),
                        method: "GET",
                        params: { id: orderKey }
                    }),
                    'Failed to get order: ' + orderKey);
            },

            getOrdersByInvoice: function (invoiceKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetOrdersByInvoiceKey'),
                        method: "GET",
                        params: { id: invoiceKey }
                    }),
                    'Failed to get orders by invoice: ' + invoiceKey);
            },

            getUnFulfilledItems: function (invoiceKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetUnFulfilledItems'),
                        method: "GET",
                        params: { id: invoiceKey }
                    }),
                    'Failed to get unfulfilled items by invoice: ' + invoiceKey);
            },

            getShippingAddress: function (invoiceKey) {

                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetShippingAddress'),
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

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'ProcessesProductsToBackofficeOrder'),
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

                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetShippingMethods'),
                        model
                    ),
                    'Failed to get shipping methods');
            },

            getPaymentMethods: function () {             
                return umbRequestHelper.resourcePromise(
                    $http({
                        url: umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'GetPaymentMethods'),
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
                return umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl('merchelloOrderApiBaseUrl', 'FinalizeBackofficeOrder'),
                        model
                    ),
                    'Failed to finalize backoffice order');
                }
        };
    };

    angular.module('umbraco.resources').factory('merchelloOrderService', ['$http', 'umbRequestHelper', merchello.Services.MerchelloOrderService]);

}(window.merchello.Services = window.merchello.Services || {}));
