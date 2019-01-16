angular.module('merchello.directives').directive('customerItemCacheTable',
    ['$q', '$location', 'localizationService', 'notificationsService', 'dialogService', 'settingsResource', 'customerResource', 'backOfficeCheckoutResource',
    function($q, $location, localizationService, notificationsService, dialogService, settingsResource, customerResource, backOfficeCheckoutResource) {

        return {
            restrict: 'E',
            replace: true,
            scope: {
                customer: '=',
                doAdd: '&',
                doMove: '&',
                doEdit: '&',
                doDelete: '&',
                itemCacheType: '@'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/customer.itemcachetable.tpl.html',
            link: function (scope, elm, attr) {

                scope.loaded = true;
                scope.title = '';
                scope.settings = {};
                scope.items = [];

                scope.getTotal = getTotal;

                scope.openProductSelection = openProductSelectionDialog;
                scope.openCheckoutDialog = openCheckoutDialog;
                scope.showCheckout = false;

                const baseUrl = '/merchello/merchello/saleoverview/';

                function init() {

                    scope.$watch('customer', function(newVal, oldVal) {
                        if (newVal.key !== null && newVal.key !== undefined) {
                            getItemCacheData();
                        }
                    });
                }

                function getItemCacheData() {
                    $q.all([
                            localizationService.localize('merchelloCustomers_customer' + scope.itemCacheType),
                            settingsResource.getAllCombined(),
                            customerResource.getCustomerItemCache(scope.customer.key, scope.itemCacheType)
                        ]).then(function(data) {
                        scope.title = data[0];
                        scope.settings = data[1];
                        scope.items = data[2].items;
                        setCheckoutLink();
                    });
                }

                function getTotal() {
                    var total = 0;
                    angular.forEach(scope.items, function(item) {
                       total += item.quantity * item.price;
                    });
                    return total;
                }

                function openProductSelectionDialog() {
                    var dialogData = {};
                    dialogData.addItems = [];

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/customer.productselectionfilter.html',
                        show: true,
                        callback: processProductSelection,
                        dialogData: dialogData
                    });
                }

                function processProductSelection(dialogData) {
                    scope.doAdd()(dialogData.addItems, scope.itemCacheType);
                }

                function setCheckoutLink() {
                    var billingAddress = scope.customer.getDefaultBillingAddress();
                    var shippingAddress = scope.customer.getDefaultShippingAddress();

                    scope.showCheckout = (scope.items.length > 0) &&
                            billingAddress !== null && billingAddress !== undefined &&
                            shippingAddress !== null && shippingAddress !== undefined &&
                            scope.itemCacheType === 'Basket';
                }

                function openCheckoutDialog () {

                    backOfficeCheckoutResource.getShipmentRateQuotes(scope.customer.key)
                        .then(function(quotes) {

                            var q = quotes.length > 0 ? quotes[0] : {};

                            var dialogData = {
                                customer: scope.customer,
                                items: scope.items,
                                currencySymbol: scope.settings.currencySymbol,
                                total: getTotal(),
                                quotes: quotes,
                                selectedQuote: q
                            };

                            dialogService.open({
                                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/customer.checkout.html',
                                show: true,
                                callback: processCheckout,
                                dialogData: dialogData
                            });
                    });
                }
                
                
                function processCheckout(dialogData) {

                    var billingAddress = scope.customer.getDefaultBillingAddress();
                    var shippingAddress = scope.customer.getDefaultShippingAddress();


                    var checkoutData = {
                        customerKey: dialogData.customer.key,
                        billingAddressKey: billingAddress.key,
                        shippingAddressKey: shippingAddress.key,
                        shipMethodKey: dialogData.selectedQuote.shipMethod.key
                    };

                    backOfficeCheckoutResource
                        .createCheckoutInvoice(checkoutData)
                        .then(function(inv) {
                            $location.url(baseUrl + inv.key, true);
                        }, function(msg) {
                            notificationsService.error(msg);
                        });
                }
                
                function quoteShippingMethods() {
                    
                }
                
                init();
            }

        }

    }]);
