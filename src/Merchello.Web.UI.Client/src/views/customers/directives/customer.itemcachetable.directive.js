angular.module('merchello.directives').directive('customerItemCacheTable',
    ['$q', 'localizationService', 'settingsResource', 'customerResource',
    function($q, localizationService, settingsResource, customerResource) {

        return {
            restrict: 'E',
            replace: true,
            scope: {
                customer: '=',
                itemCacheType: '@'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/customer.itemcachetable.tpl.html',
            link: function (scope, elm, attr) {

                scope.loaded = true;
                scope.title = '';
                scope.settings = {};
                scope.items = [];

                scope.getTotal = getTotal;

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
                    });
                }

                function getTotal() {
                    var total = 0;
                    angular.forEach(scope.items, function(item) {
                       total += item.quantity * item.price;
                    });
                    return total;
                }

                init();
            }

        }

    }]);
