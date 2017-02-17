    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ProductListController
     * @function
     *
     * @description
     * The controller for product list view controller
     */
    angular.module('merchello').controller('Merchello.Backoffice.ProductListController',
        ['$scope', '$log', '$q', '$routeParams', '$location', '$filter', 'localizationService', 'notificationsService', 'settingsResource', 'entityCollectionResource',
            'merchelloTabsFactory', 'productResource', 'productDisplayBuilder',
        function($scope, $log, $q, $routeParams, $location, $filter, localizationService, notificationsService, settingsResource, entityCollectionResource,
                 merchelloTabsFactory, productResource, productDisplayBuilder) {

            $scope.productDisplayBuilder = productDisplayBuilder;
            $scope.load = load;
            $scope.entityType = 'Product';

            $scope.tabs = [];

            // exposed methods
            $scope.getColumnValue = getColumnValue;

            var yes = '';
            var no = '';
            var some = '';

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                loadSettings();
                $scope.tabs = merchelloTabsFactory.createProductListTabs();
                $scope.tabs.setActive('productlist');
            }


            function load(query) {
                if (query.hasCollectionKeyParam()) {
                    return entityCollectionResource.getCollectionEntities(query);
                } else {
                    return productResource.searchProducts(query);
                }
            }


            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description
             * Load the settings from the settings service to get the currency symbol
             */
            function loadSettings() {
                var deferred = $q.defer();
                var promises = [
                        settingsResource.getCurrencySymbol(),
                        localizationService.localize('general_yes'),
                        localizationService.localize('general_no'),
                        localizationService.localize('merchelloGeneral_some')
                    ];
                $q.all(promises).then(function(data) {
                    deferred.resolve(data);
                });
                deferred.promise.then(function(result) {
                    $scope.currencySymbol = result[0];
                    yes = result[1];
                    no = result[2];
                    some = result[3];
                    $scope.preValuesLoaded = true;
                    $scope.loaded = true;
                }, function(reason) {
                    notificationsService.error("Settings Load Failed", reason.message)
                });
            }

            function getColumnValue(result, col) {
               switch(col.name) {
                   case 'name':
                       return '<a href="' + getEditUrl(result) + '">' + result.name + '</a>';
                   case 'available':
                       return result.available ? yes : no;
                   case 'shippable':
                       return getShippableValue(result);
                   case 'taxable':
                       return getTaxableValue(result);
                   case 'totalInventory':
                       return '<span>' + result.totalInventory() + '</span>';
                   case 'onSale':
                       return getOnSaleValue(result);
                   case 'price':
                       return !result.hasVariants() ?
                           $filter('currency')(result.price, $scope.currencySymbol) :
                           $filter('currency')(result.variantsMinimumPrice(), $scope.currencySymbol) + ' - ' + $filter('currency')(result.variantsMaximumPrice(), $scope.currencySymbol);
                   default:
                       return result[col.name];
               }
            }

            function getShippableValue(product) {
                if ((product.hasVariants() && product.shippableVariants().length === product.productVariants.length) ||
                    (product.hasVariants() && product.shippableVariants().length === 0) ||
                    (!product.hasVariants() && product.shippable)) {
                    return yes;
                }
                if (product.hasVariants() && product.shippableVariants().length !== product.productVariants.length && product.shippableVariants().length > 0) {
                    return some;
                } else {
                    return no;
                }
            }

            function getTaxableValue(product) {
                if((product.hasVariants() && product.taxableVariants().length === product.productVariants.length) ||
                    (product.hasVariants() && product.taxableVariants().length === 0) ||
                    (!product.hasVariants() && product.taxable)) {
                    return yes;
                }
                if (product.hasVariants() && product.taxableVariants().length !== product.productVariants.length && product.taxableVariants().length > 0) {
                    return some;
                } else {
                    return no;
                }
            }

            function getOnSaleValue(product) {
                if((product.hasVariants() && !product.anyVariantsOnSale()) ||
                    (!product.hasVariants() && !product.onSale)) {
                    return no;
                }
                if(product.hasVariants() && product.anyVariantsOnSale()) {
                    return $filter('currency')(product.variantsMinimumPrice(true), $scope.currencySymbol) + ' - ' +
                        $filter('currency')(product.variantsMaximumPrice(true), $scope.currencySymbol);
                }
                if (!product.hasVariants() && product.onSale) {
                    return $filter('currency')(product.salePrice, $scope.currencySymbol)
                }
            }

            function getEditUrl(product) {
               // if (product.hasVariants()) {
               //     return '#/merchello/merchello/producteditwithoptions/' + product.key;
               // } else {
                    return "#/merchello/merchello/productedit/" + product.key;
               // }
            }

            // Initialize the controller
            init();

        }]);
