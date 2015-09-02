angular.module('merchello.services').service('merchelloListViewHelper',
    ['$filter',
    function() {

        var configs = {
            product: {
                columns: [
                    { name: 'name', localizeKey: 'merchelloVariant_product' },
                    { name: 'sku', localizeKey: 'merchelloVariant_sku' },
                    { name: 'shippable', localizeKey: 'merchelloProducts_shippable' },
                    { name: 'taxable', localizeKey: 'merchelloProducts_taxable' },
                    { name: 'totalInventory', localizeKey: 'merchelloGeneral_quantity' },
                    { name: 'onSale', localizeKey: 'merchelloVariant_productOnSale' },
                    { name: 'price', localizeKey: 'merchelloGeneral_price' }
                ]
            },

            customer:  {
                columns: [
                    { name: 'loginName', localizeKey: 'merchelloCustomers_loginName' },
                    { name: 'firstName', localizeKey: 'general_name' },
                    { name: 'location', localizeKey: 'merchelloCustomers_location' },
                    { name: 'lastInvoiceTotal', localizeKey: 'merchelloCustomers_lastInvoiceTotal' }
                ]
            },

            invoice: {
                columns: [
                    { name: 'invoiceNumber', localizeKey: 'merchelloSales_invoiceNumber' },
                    { name: 'invoiceDate', localizeKey: 'general_date' },
                    { name: 'billToName', localizeKey: 'merchelloGeneral_customer' },
                    { name: 'paymentStatus', localizeKey: 'merchelloSales_paymentStatus' },
                    { name: 'fulfillmentStatus', localizeKey: 'merchelloOrder_fulfillmentStatus' },
                    { name: 'total', localizeKey: 'merchelloGeneral_total' }
                ]
            },

            offer: {
                columns: [
                    { name: 'name', localizeKey: 'merchelloTableCaptions_name' },
                    { name: 'offerCode', localizeKey: 'merchelloMarketing_offerCode' },
                    { name: 'offerType', localizeKey: 'merchelloMarketing_offerType' },
                    { name: 'rewards', localizeKey: 'merchelloMarketing_offerRewardsInfo' },
                    { name: 'offerStartDate', localizeKey: 'merchelloTableCaptions_startDate' },
                    { name: 'offerEndDate', localizeKey: 'merchelloTableCaptions_endDate' },
                    { name: 'active', localizeKey: 'merchelloTableCaptions_active' }
                ]
            }

        };

        this.getConfig = function(entityType) {
            var ensure = entityType.toLowerCase();
            return configs[ensure];
        };

}]);
