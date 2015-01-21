angular.module('merchello.models').factory('merchelloTabsFactory',
    ['MerchelloTabCollection',
        function(MerchelloTabCollection) {

            var Constructor = MerchelloTabCollection;

            // creates tabs for the sales listing page
            function createSalesListTabs() {
                var tabs = new Constructor();
                tabs.addTab('saleslist', 'Sales Listing', '#/merchello/merchello/saleslist/manage');
                return tabs;
            }

            // creates the tabs for sales overview section
            function createSalesTabs(invoiceKey) {
                var tabs = new Constructor();
                tabs.addTab('saleslist', 'Sales Listing', '#/merchello/merchello/saleslist/manage');
                tabs.addTab('overview', 'Sale', '#/merchello/merchello/saleoverview/' + invoiceKey);
                tabs.addTab('payments', 'Payments', '#/merchello/merchello/invoicepayments/' + invoiceKey);
                tabs.addTab('shipments', 'Shipments', '#/merchello/merchello/ordershipments/' + invoiceKey);
                return tabs;
            }

            // creates the tabs for the customer list page
            function createCustomerListTabs() {
                var tabs = new Constructor();
                tabs.addTab('customerlist', 'Customer Listing', '#/merchello/merchello/customerlist/manage');
                return tabs;
            }

            // creates the customer overview tabs
            function createCustomerOverviewTabs(customerKey) {
                var tabs = new Constructor();
                tabs.addTab('customerlist', 'Customer Listing', '#/merchello/merchello/customerlist/manage');
                tabs.addTab('overview', 'Customer', '#/merchello/merchello/customeroverview/' + customerKey);
                return tabs;
            }

            // creates the tabs for the gateway provider section
            function createGatewayProviderTabs() {
                var tabs = new Constructor();
                tabs.addTab('providers', 'Gateway Providers', '#/merchello/merchello/gatewayproviderlist/manage');
                tabs.addTab('notification', 'Notification', '#/merchello/merchello/notificationproviders/manage');
                tabs.addTab('payment', 'Payment', '#/merchello/merchello/paymentproviders/manage');
                tabs.addTab('shipping', 'Shipping', '#/merchello/merchello/shippingproviders/manage');
                tabs.addTab('taxation', 'Taxation', '#/merchello/merchello/taxationproviders/manage');
                return tabs;
            }

            function createReportsTabs() {
                var tabs = new Constructor();
                tabs.addTab('reportslist', 'Reports', '#/merchello/merchello/reportslist/manage');
                return tabs;
            }

            return {
                createSalesListTabs: createSalesListTabs,
                createSalesTabs: createSalesTabs,
                createCustomerListTabs: createCustomerListTabs,
                createCustomerOverviewTabs: createCustomerOverviewTabs,
                createGatewayProviderTabs: createGatewayProviderTabs,
                createReportsTabs: createReportsTabs
            };

}]);
