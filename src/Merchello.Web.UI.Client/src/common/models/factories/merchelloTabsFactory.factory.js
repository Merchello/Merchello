angular.module('merchello.models').factory('merchelloTabsFactory',
    ['MerchelloTabCollection',
        function(MerchelloTabCollection) {

            var Constructor = MerchelloTabCollection;

            // creates the tabs for sales overview section
            function createSalesTabs(invoiceKey) {
                var tabs = new Constructor();
                tabs.addTab('overview', 'Overview', '#/merchello/merchello/saleoverview/' + invoiceKey);
                tabs.addTab('payments', 'Payments', '#/merchello/merchello/invoicepayments/' + invoiceKey);
                tabs.addTab('shipments', 'Shipments', '#/merchello/merchello/ordershipments/' + invoiceKey);
                return tabs;
            }

            // creates the tabs for the gateway provider section
            function createGatewayProviderTabs() {
                var tabs = new Constructor();
                tabs.addTab('providers', 'Gateway Providers', '#/merchello/merchello/gatewayproviderlist/manage');
                tabs.addTab('payment', 'Payment', '#/merchello/merchello/paymentproviders/manage');
                tabs.addTab('shipping', 'Shipping', '#/merchello/merchello/shippingproviders/manage');
                tabs.addTab('taxation', 'Taxation', '#/merchello/merchello/taxationproviders/manage');
                return tabs;
            }

            return {
                createSalesTabs: createSalesTabs,
                createGatewayProviderTabs: createGatewayProviderTabs
            };

}]);
