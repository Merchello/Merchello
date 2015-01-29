    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ProductVariantEditController
     * @function
     *
     * @description
     * The controller for product variant edit view
     */
    angular.module('merchello').controller('Merchello.Backoffice.ProductVariantEditController',
        ['$scope', '$routeParams', '$location', 'assetsService', 'notificationsService', 'dialogService', 'serverValidationManager', 'merchelloTabsFactory',
            'productResource', 'warehouseResource', 'settingsResource', 'settingDisplayBuilder', 'warehouseDisplayBuilder', 'productDisplayBuilder',
        function($scope, $routeParams, $location, assetsService, notificationsService, dialogService, serverValidationManager, merchelloTabsFactory,
        productResource, warehouseResource, settingsResource, settingDisplayBuilder, warehouseDisplayBuilder, productDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];

            // settings - contains defaults for the checkboxes
            $scope.settings = {};


            function init() {
                var key = $routeParams.id;
                var productVariantKey = $routeParams.variantid;
                $scope.tabs = merchelloTabsFactory.createProductVariantEditorTabs(key, productVariantKey);
                $scope.tabs.setActive('varianteditor');
                loadProduct(key, productVariantKey);
            }

            /**
             * @ngdoc method
             * @name loadProduct
             * @function
             *
             * @description
             * Load a product by the product key.
             */
            function loadProduct(key, productVariantKey) {
                console.info(key + ' ' + productVariantKey);
                var promiseProduct = productResource.getByKey(key);
                promiseProduct.then(function (product) {
                    $scope.product = productDisplayBuilder.transform(product);
                    // we use the master variant context so that we can use directives associated with variants
                    $scope.productVariant = $scope.product.getProductVariant(productVariantKey);
                    loadSettings();
                }, function (reason) {
                    notificationsService.error("Product Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description
             * Loads in store settings from server into the scope.  Called in init().
             */
            function loadSettings() {
                var promiseSettings = settingsResource.getAllSettings();
                promiseSettings.then(function(settings) {
                    $scope.settings = settingDisplayBuilder.transform(settings);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }

            // Initializes the controller
            init();
    }]);
