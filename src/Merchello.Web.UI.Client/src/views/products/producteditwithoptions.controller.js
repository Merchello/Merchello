    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ProductEditWithOptionsController
     * @function
     *
     * @description
     * The controller for product edit with options view
     */
    angular.module('merchello').controller('Merchello.Backoffice.ProductEditWithOptionsController',
        ['$scope', '$routeParams', '$location', '$q', 'assetsService', 'notificationsService', 'dialogService', 'serverValidationManager',
            'merchelloTabsFactory', 'productResource', 'settingsResource', 'productDisplayBuilder',
        function($scope, $routeParams, $location, $q, assetsService, notificationsService, dialogService, serverValidationManager,
            merchelloTabsFactory, productResource, settingsResource, productDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.product = productDisplayBuilder.createDefault();
            $scope.currencySymbol = '';
            $scope.reorderVariants = false;

            // exposed methods
            $scope.save = save;

            function init() {
                var key = $routeParams.id;
                $scope.tabs = merchelloTabsFactory.createProductEditorWithOptionsTabs(key);
                $scope.tabs.setActive('variantlist');
                loadSettings();
                loadProduct(key);
            }

            /**
             * @ngdoc method
             * @name loadProduct
             * @function
             *
             * @description
             * Load a product by the product key.
             */
            function loadProduct(key) {
                var promise = productResource.getByKey(key);
                promise.then(function (product) {
                    $scope.product = productDisplayBuilder.transform(product);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;

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
             * Load the settings from the settings service to get the currency symbol
             */
            function loadSettings() {
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function(currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }

            //--------------------------------------------------------------------------------------
            // Event Handlers
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Saves the product - used for changing the master variant name
             */
            function save(thisForm) {
                if (thisForm) {
                    if (thisForm.$valid) {
                        notificationsService.info("Saving Product...", "");
                        var promise = productResource.save($scope.product);
                        promise.then(function (product) {
                            notificationsService.success("Product Saved", "");
                            $scope.product = productDisplayBuilder.transform(product);
                        }, function (reason) {
                            notificationsService.error("Product Save Failed", reason.message);
                        });
                    }
                }
            }

            // Initialize the controller
            init();
    }]);