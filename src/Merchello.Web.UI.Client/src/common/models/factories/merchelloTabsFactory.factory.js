angular.module('merchello.models').factory('merchelloTabsFactory',
    ['MerchelloTabCollection', 'merchelloListViewHelper',
        function(MerchelloTabCollection, merchelloListViewHelper) {

            var Constructor = MerchelloTabCollection;

            function createDefault() {
                return new Constructor();
            }

            // creates tabs for the product listing page
            function createProductListTabs() {
                var entityType = 'Product';
                var settings = getCacheSettings(entityType);
                var tabs = new Constructor();
                if (settings.stickListingTab && settings.collectionKey !== '') {
                    tabs.addTab('productlist', 'merchelloTabs_productListing', '#/merchello/merchello/productlist/' + settings.collectionKey);
                } else {
                    tabs.addTab('productlist', 'merchelloTabs_productListing', '#/merchello/merchello/productlist/manage');
                }
                tabs.addTab('sharedoptions', 'merchelloTabs_sharedProductOptions', '#/merchello/merchello/sharedoptions/manage');
                tabs.addTab('filtergroups', 'merchelloTabs_filterGroups', '#/merchello/merchello/productfiltergroups/manage');
                tabs.addTab('contentTypeList', 'merchelloTabs_contentTypes', '#/merchello/merchello/productcontenttypelist/manage');
                return tabs;
            }

           // creates tabs for the product editor page
            function createNewProductEditorTabs() {
                var entityType = 'Product';
                var settings = getCacheSettings(entityType);
                var tabs = new Constructor();
                if (settings.stickListingTab && settings.collectionKey !== '') {
                    tabs.addTab('productlist', 'merchelloTabs_productListing', '#/merchello/merchello/productlist/' + settings.collectionKey);
                } else {
                    tabs.addTab('productlist', 'merchelloTabs_productListing', '#/merchello/merchello/productlist/manage');
                }
                tabs.addTab('createproduct', 'merchelloTabs_product', '#/merchello/merchello/productedit/');
                return tabs;
            }

            // creates tabs for the product editor page
            function createProductEditorTabs(productKey, hasVariants) {
                var entityType = 'Product';
                var settings = getCacheSettings(entityType);
                if (hasVariants !== undefined && hasVariants === true)
                {
                    return createProductEditorWithOptionsTabs(productKey);
                }
                var tabs = new Constructor();
                if (settings.stickListingTab && settings.collectionKey !== '') {
                    tabs.addTab('productlist', 'merchelloTabs_productListing', '#/merchello/merchello/productlist/' + settings.collectionKey);
                } else {
                    tabs.addTab('productlist', 'merchelloTabs_productListing', '#/merchello/merchello/productlist/manage');
                }
                tabs.addTab('productedit', 'merchelloTabs_product', '#/merchello/merchello/productedit/' + productKey);
                tabs.addTab('productcontent', 'merchelloTabs_detachedContent', '#/merchello/merchello/productdetachedcontent/' + productKey);
                tabs.addTab('optionslist', 'merchelloTabs_productOptions', '#/merchello/merchello/productoptionsmanager/' + productKey);
                return tabs;
            }

            // create tabs for product options editors
            function createProductOptionAddTabs() {
                var tabs = new Constructor();
                return tabs;
            }

            // creates tabs for the product editor with options tabs
            function createProductEditorWithOptionsTabs(productKey) {
                var entityType = 'Product';
                var settings = getCacheSettings(entityType);
                var tabs = new Constructor();
                if (settings.stickListingTab && settings.collectionKey !== '') {
                    tabs.addTab('productlist', 'merchelloTabs_productListing', '#/merchello/merchello/productlist/' + settings.collectionKey);
                } else {
                    tabs.addTab('productlist', 'merchelloTabs_productListing', '#/merchello/merchello/productlist/manage');
                }
                tabs.addTab('productedit', 'merchelloTabs_product', '#/merchello/merchello/productedit/' + productKey);
                tabs.addTab('productcontent', 'merchelloTabs_detachedContent', '#/merchello/merchello/productdetachedcontent/' + productKey);
                tabs.addTab('variantlist', 'merchelloTabs_productVariants', '#/merchello/merchello/producteditwithoptions/' + productKey);
                tabs.addTab('optionslist', 'merchelloTabs_productOptions', '#/merchello/merchello/productoptionsmanager/' + productKey);
                return tabs;
            }

            // creates tabs for the product variant editor
           function createProductVariantEditorTabs(productKey, productVariantKey) {
               var entityType = 'Product';
               var settings = getCacheSettings(entityType);
                var tabs = new Constructor();
               if (settings.stickListingTab && settings.collectionKey !== '') {
                   tabs.addTab('productlist', 'merchelloTabs_productListing', '#/merchello/merchello/productlist/' + settings.collectionKey);
               } else {
                   tabs.addTab('productlist', 'merchelloTabs_productListing', '#/merchello/merchello/productlist/manage');
               }
                tabs.addTab('productedit', 'merchelloTabs_product', '#/merchello/merchello/productedit/' + productKey);
                tabs.addTab('variantlist', 'merchelloTabs_productVariants', '#/merchello/merchello/producteditwithoptions/' + productKey);
                tabs.addTab('varianteditor', 'merchelloTabs_productVariantEditor', '#/merchello/merchello/productedit/' + productKey + '?variantid=' + productVariantKey);
               tabs.addTab('productcontent', 'merchelloTabs_detachedContent', '#/merchello/merchello/productdetachedcontent/' + productKey + '?variantid=' + productVariantKey);
                return tabs;
            }


            // creates tabs for the sales listing page
            function createSalesListTabs() {
                var entityType = 'Invoice';
                var settings = getCacheSettings(entityType);
                var tabs = new Constructor();
                if (settings.stickListingTab && settings.collectionKey !== '') {
                    tabs.addTab('saleslist', 'merchelloTabs_salesListing', '#/merchello/merchello/saleslist/' + settings.collectionKey);
                } else {
                    tabs.addTab('saleslist', 'merchelloTabs_salesListing', '#/merchello/merchello/saleslist/manage');
                }

                return tabs;
            }

            // creates the tabs for sales overview section
            function createSalesTabs(invoiceKey) {
                var entityType = 'Invoice';
                var settings = getCacheSettings(entityType);
                var tabs = new Constructor();
                if (settings.stickListingTab && settings.collectionKey !== '') {
                    tabs.addTab('saleslist', 'merchelloTabs_salesListing', '#/merchello/merchello/saleslist/' + settings.collectionKey);
                } else {
                    tabs.addTab('saleslist', 'merchelloTabs_salesListing', '#/merchello/merchello/saleslist/manage');
                }
                tabs.addTab('overview', 'merchelloTabs_sales', '#/merchello/merchello/saleoverview/' + invoiceKey);
                tabs.addTab('payments', 'merchelloTabs_payments', '#/merchello/merchello/invoicepayments/' + invoiceKey);
                tabs.addTab('shipments', 'merchelloTabs_shipments', '#/merchello/merchello/ordershipments/' + invoiceKey);
                return tabs;
            }

            // creates the tabs for the customer list page
            function createCustomerListTabs() {
                var entityType = 'Customer';
                var settings = getCacheSettings(entityType);
                var tabs = new Constructor();
                if (settings.stickListingTab && settings.collectionKey !== '') {
                    tabs.addTab('customerlist', 'merchelloTabs_customerListing', '#/merchello/merchello/customerlist/' + settings.collectionKey);
                } else {
                    tabs.addTab('customerlist', 'merchelloTabs_customerListing', '#/merchello/merchello/customerlist/manage');
                }

                return tabs;
            }

            // creates the customer overview tabs
            function createCustomerOverviewTabs(customerKey, hasAddresses) {
                var entityType = 'Customer';
                var settings = getCacheSettings(entityType);
                var tabs = new Constructor();
                if (settings.stickListingTab && settings.collectionKey !== '') {
                    tabs.addTab('customerlist', 'merchelloTabs_customerListing', '#/merchello/merchello/customerlist/' + settings.collectionKey);
                } else {
                    tabs.addTab('customerlist', 'merchelloTabs_customerListing', '#/merchello/merchello/customerlist/manage');
                }
                tabs.addTab('overview', 'merchelloTabs_customer', '#/merchello/merchello/customeroverview/' + customerKey);
                if(hasAddresses) {
                    tabs.addTab('addresses', 'merchelloTabs_customerAddresses', '#/merchello/merchello/customeraddresses/' + customerKey);
                }
                return tabs;
            }

            // creates the tabs for the gateway provider section
            function createGatewayProviderTabs() {
                var tabs = new Constructor();
                tabs.addTab('providers', 'merchelloTabs_gatewayProviders', '#/merchello/merchello/gatewayproviderlist/manage');
                tabs.addTab('notification', 'merchelloTabs_notification', '#/merchello/merchello/notificationproviders/manage');
                tabs.addTab('payment', 'merchelloTabs_payment', '#/merchello/merchello/paymentproviders/manage');
                tabs.addTab('shipping', 'merchelloTabs_shipping', '#/merchello/merchello/shippingproviders/manage');
                tabs.addTab('taxation', 'merchelloTabs_taxation', '#/merchello/merchello/taxationproviders/manage');
                return tabs;
            }

            // creates the tabs for the marketing section
            function createMarketingTabs() {
                var entityType = 'Offer';
                var settings = getCacheSettings(entityType);
                var tabs = new Constructor();
                if (settings.stickListingTab && settings.collectionKey !== '') {
                    tabs.addTab('offers', 'merchelloTabs_offerListing', '#/merchello/merchello/offerslist/' + settings.collectionKey);
                } else {
                    tabs.addTab('offers', 'merchelloTabs_offerListing', '#/merchello/merchello/offerslist/manage');
                }

                return tabs;
            }

            function createReportsTabs() {
                var tabs = new Constructor();
                tabs.addTab('reportsdashboard', 'merchelloTabs_reports', '#/merchello/merchello/reportsdashboard/manage');
                tabs.addTab('salesOverTime', 'merchelloTabs_salesOverTime', '#/merchello/merchello/salesOverTime/manage');
                tabs.addTab("salesByItem", "merchelloTabs_salesByItem", '#/merchello/merchello/salesByItem/manage');
                //tabs.addTab("abandonedBasket", "merchelloTabs_abandonedBasket", '#/merchello/merchello/abandonedBasket/manage');
                // throw event here:

                return tabs;
            }

            function getCacheSettings(entityType) {
                return merchelloListViewHelper.cacheSettings(entityType);
            }

            return {
                createDefault: createDefault,
                createNewProductEditorTabs: createNewProductEditorTabs,
                createProductListTabs: createProductListTabs,
                createProductEditorTabs: createProductEditorTabs,
                createProductEditorWithOptionsTabs: createProductEditorWithOptionsTabs,
                createProductOptionAddTabs: createProductOptionAddTabs,
                createSalesListTabs: createSalesListTabs,
                createSalesTabs: createSalesTabs,
                createCustomerListTabs: createCustomerListTabs,
                createCustomerOverviewTabs: createCustomerOverviewTabs,
                createGatewayProviderTabs: createGatewayProviderTabs,
                createReportsTabs: createReportsTabs,
                createProductVariantEditorTabs: createProductVariantEditorTabs,
                createMarketingTabs: createMarketingTabs
            };

}]);
