/**
 * @ngdoc service
 * @name merchelloListViewHelper
 * @description Handles list view configurations.
 **/
angular.module('merchello.services').service('merchelloListViewHelper',
    ['$sessionStorage', '$localStorage',
    function($sessionStorage, $localStorage) {

        var configs = {
            product: {
                columns: [
                    { name: 'name', localizeKey: 'merchelloVariant_product' },
                    { name: 'sku', localizeKey: 'merchelloVariant_sku' },
                    { name: 'available', localizeKey: 'merchelloProducts_available' },
                    { name: 'shippable', localizeKey: 'merchelloProducts_shippable' },
                    { name: 'taxable', localizeKey: 'merchelloProducts_taxable' },
                    { name: 'totalInventory', localizeKey: 'merchelloGeneral_quantity', resultColumn: true },
                    { name: 'onSale', localizeKey: 'merchelloVariant_productOnSale', resultColumn: true },
                    { name: 'price', localizeKey: 'merchelloGeneral_price' }
                ],
                pageSize: 15
            },

            // TODO remove this
            productoption: {
                columns: [
                    { name: 'name', localizeKey: 'merchelloTableCaptions_optionName' },
                    { name: 'uiOption', localizeKey: 'merchelloTableCaptions_optionUi' },
                    { name: 'choices', localizeKey: 'merchelloTableCaptions_optionValues', resultColumn: true },
                    { name: 'shared', localizeKey: 'merchelloTableCaptions_shared' },
                    { name: 'sharedCount', localizeKey: 'merchelloTableCaptions_sharedCount' }
                ],
                pageSize: 10,
                orderBy: 'name',
                orderDirection: 'asc'
            },

            customer:  {
                columns: [
                    { name: 'loginName', localizeKey: 'merchelloCustomers_loginName' },
                    { name: 'firstName', localizeKey: 'general_name' },
                    { name: 'location', localizeKey: 'merchelloCustomers_location', resultColumn: true },
                    { name: 'lastInvoiceTotal', localizeKey: 'merchelloCustomers_lastInvoiceTotal', resultColumn: true }
                ]
            },

            invoice: {
                columns: [
                    { name: 'invoiceNumber', localizeKey: 'merchelloSales_invoiceNumber' },
                    { name: 'invoiceDate', localizeKey: 'general_date' },
                    { name: 'billToName', localizeKey: 'merchelloGeneral_customer' },
                    { name: 'paymentStatus', localizeKey: 'merchelloSales_paymentStatus', resultColumn: true },
                    { name: 'fulfillmentStatus', localizeKey: 'merchelloOrder_fulfillmentStatus', resultColumn: true },
                    { name: 'total', localizeKey: 'merchelloGeneral_total' }
                ],
                pageSize: 15,
                orderBy: 'invoiceNumber',
                orderDirection: 'desc'
            },

            saleshistory: {
                columns: [
                    { name: 'invoiceNumber', localizeKey: 'merchelloSales_invoiceNumber' },
                    { name: 'invoiceDate', localizeKey: 'general_date' },
                    { name: 'paymentStatus', localizeKey: 'merchelloSales_paymentStatus', resultColumn: true },
                    { name: 'fulfillmentStatus', localizeKey: 'merchelloOrder_fulfillmentStatus', resultColumn: true },
                    { name: 'total', localizeKey: 'merchelloGeneral_total' }
                ],
                orderBy: 'invoiceNumber',
                orderDirection: 'desc'
            },

            offer: {
                columns: [
                    { name: 'name', localizeKey: 'merchelloTableCaptions_name' },
                    { name: 'offerCode', localizeKey: 'merchelloMarketing_offerCode' },
                    { name: 'offerType', localizeKey: 'merchelloMarketing_offerType' },
                    { name: 'rewards', localizeKey: 'merchelloMarketing_offerRewardsInfo', resultColumn: true },
                    { name: 'offerStartDate', localizeKey: 'merchelloTableCaptions_startDate' },
                    { name: 'offerEndDate', localizeKey: 'merchelloTableCaptions_endDate' },
                    { name: 'createDate', localizeKey: 'merchelloTableCaptions_createDate' },
                    { name: 'active', localizeKey: 'merchelloTableCaptions_active' }
                ]
            },

            customerbaskets: {
                columns: [
                    { name: 'loginName', localizeKey: 'merchelloCustomers_loginName' },
                    { name: 'firstName', localizeKey: 'general_name' },
                    { name: 'lastActivityDate', localizeKey: 'merchelloCustomers_lastActivityDate' },
                    { name: 'items', localizeKey: 'merchelloCustomers_basket' }
                ],
                pageSize: 10,
                orderBy: 'lastActivityDate',
                orderDirection: 'desc'
            }

        };

        this.getConfig = function(listViewType) {
            var ensure = listViewType.toLowerCase();
            return configs[ensure];
        };

        this.cacheSettings = function(entityType, config) {
            var cacheKey = 'listview-settings-' + entityType;

            var settings = config;

            if(settings === undefined) {

                settings = $localStorage[cacheKey];
                if (settings === undefined) {
                    settings = {
                        stickyList: false,
                        stickyCollectionList: false,
                        stickListingTab: false,
                        collectionKey: ''
                    };
                    $localStorage[cacheKey] = settings;
                }

            } else {
                $localStorage[cacheKey] = settings;
            }

            return settings;
        },


        this.cache = function(entityType) {

            function getKey(entityType, key) {
                return entityType + '-' + key;
            }

            return {
                type: entityType,

                setValue: function(key, value) {
                    var cacheKey = getKey(entityType, key);
                    $sessionStorage[cacheKey] = value;
                },

                getValue: function(key) {
                    var cacheKey = getKey(entityType, key);
                    return $sessionStorage[cacheKey];
                },

                hasKey: function(key) {
                    var cacheKey = getKey(entityType, key);
                    return $sessionStorage[cacheKey] !== undefined;
                },

                removeValue: function(key) {
                    var cacheKey = getKey(entityType, key);
                    $sessionStorage[cacheKey] = undefined;
                }
            };
        };

}]);
