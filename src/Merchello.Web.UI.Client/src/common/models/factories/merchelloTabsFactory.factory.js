angular.module('merchello.models').factory('merchelloTabsFactory',
    ['MerchelloTabCollection',
        function(MerchelloTabCollection) {

            var Constructor = MerchelloTabCollection;

            // creates tabs for the product listing page
            function createProductListTabs() {
                var tabs = new Constructor();
                tabs.addTab('productlist', 'Product Listing', '#/merchello/merchello/productlist/manage');
                return tabs;
            }

            // creates tabs for the product editor page
            function createNewProductEditorTabs() {
                var tabs = new Constructor();
                tabs.addTab('productlist', 'Product Listing', '#/merchello/merchello/productlist/manage');
                tabs.addTab('createproduct', 'Product', '#/merchello/merchello/productedit/');
                return tabs;
            }

            // creates tabs for the product editor page
            function createProductEditorTabs(productKey) {
                var tabs = new Constructor();
                tabs.addTab('productlist', 'Product Listing', '#/merchello/merchello/productlist/manage');
                tabs.addTab('productedit', 'Product', '#/merchello/merchello/productedit/' + productKey);
                return tabs;
            }

            // creates tabs for the product editor with options tabs
            function createProductEditorWithOptionsTabs(productKey) {
                var tabs = new Constructor();
                tabs.addTab('productlist', 'Product Listing', '#/merchello/merchello/productlist/manage');
                tabs.addTab('variantlist', 'Product Variants', '#/merchello/merchello/producteditwithoptions/' + productKey);
                tabs.addTab('optionslist', 'Product Options', '#/merchello/merchello/productoptionseditor/' + productKey);
                return tabs;
            }

            // creates tabs for the product variant editor
            function createProductVariantEditorTabs(productKey, productVariantKey) {
                var tabs = new Constructor();
                tabs.addTab('productlist', 'Product Listing', '#/merchello/merchello/productlist/manage');
                tabs.addTab('variantlist', 'Product Variants', '#/merchello/merchello/producteditwithoptions/' + productKey);
                tabs.addTab('varianteditor', 'Product Variant Editor', '#/merchello/merchello/productvariantedit/' + productKey + '?variantid=' + productVariantKey);
                return tabs;
            }

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
            function createCustomerOverviewTabs(customerKey, hasAddresses) {
                var tabs = new Constructor();
                tabs.addTab('customerlist', 'Customer Listing', '#/merchello/merchello/customerlist/manage');
                tabs.addTab('overview', 'Customer', '#/merchello/merchello/customeroverview/' + customerKey);
                if(hasAddresses) {
                    tabs.addTab('addresses', 'Addresses', '#/merchello/merchello/customeraddresses/' + customerKey);
                }
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
                createNewProductEditorTabs: createNewProductEditorTabs,
                createProductListTabs: createProductListTabs,
                createProductEditorTabs: createProductEditorTabs,
                createProductEditorWithOptionsTabs: createProductEditorWithOptionsTabs,
                createSalesListTabs: createSalesListTabs,
                createSalesTabs: createSalesTabs,
                createCustomerListTabs: createCustomerListTabs,
                createCustomerOverviewTabs: createCustomerOverviewTabs,
                createGatewayProviderTabs: createGatewayProviderTabs,
                createReportsTabs: createReportsTabs,
                createProductVariantEditorTabs: createProductVariantEditorTabs
            };

}]);
