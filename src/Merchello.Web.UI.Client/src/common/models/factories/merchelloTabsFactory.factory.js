angular.module('merchello.models').factory('merchelloTabsFactory',
    ['MerchelloTabCollection',
        function(MerchelloTabCollection) {

            var Constructor = MerchelloTabCollection;

            function createSalesTabs(invoiceKey) {
                var tabs = new Constructor();
                tabs.addTab('overview', 'Sales Overview', '#/merchello/merchello/saleoverview/' + invoiceKey);
                tabs.addTab('payments', 'Payments', '#/merchello/merchello/invoicepayments/' + invoiceKey);
                tabs.addTab('shipments', 'Shipments', '#/merchello/merchello/ordershipments/' + invoiceKey);
                return tabs;
            }

            return {
                createSalesTabs: createSalesTabs
            };

}]);
