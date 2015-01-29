    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ProductEditController
     * @function
     *
     * @description
     * The controller for product edit view
     */
    angular.module('merchello').controller('Merchello.Backoffice.ProductEditController',
        ['$scope', '$routeParams', '$location', '$q', 'assetsService', 'notificationsService', 'dialogService', 'merchelloTabsFactory', 'dialogDataFactory',
            'serverValidationManager', 'productResource', 'warehouseResource', 'settingsResource',
            'productDisplayBuilder', 'productVariantDisplayBuilder', 'warehouseDisplayBuilder', 'settingDisplayBuilder',
        function($scope, $routeParams, $location, $q, assetsService, notificationsService, dialogService, merchelloTabsFactory, dialogDataFactory,
            serverValidationManager, productResource, warehouseResource, settingsResource,
            productDisplayBuilder, productVariantDisplayBuilder, warehouseDisplayBuilder, settingDisplayBuilder) {

            //--------------------------------------------------------------------------------------
            // Declare and initialize key scope properties
            //--------------------------------------------------------------------------------------
            //

            // To help umbraco directives show our page
            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];

            // settings - contains defaults for the checkboxes
            $scope.settings = {};

            // this is for the slide panel directive to get rid of the close button since we'll
            // be handling it in a different way in this case
            $scope.hideClose = true;

            $scope.product = {};
            $scope.productVariant = {};
            $scope.context = 'productedit';

            // Exposed methods
            $scope.save = save;


            //--------------------------------------------------------------------------------------
            // Initialization methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                var key = $routeParams.id;
                var productVariantKey = $routeParams.variantid;
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
                var promiseProduct = productResource.getByKey(key);
                promiseProduct.then(function (product) {
                    $scope.product = productDisplayBuilder.transform(product);
                    if(productVariantKey === '' || productVariantKey === undefined) {
                        // this is a product edit.
                        // we use the master variant context so that we can use directives associated with variants
                        $scope.productVariant = $scope.product.getMasterVariant();
                        $scope.context = 'productedit';
                        $scope.tabs = merchelloTabsFactory.createProductEditorTabs(key);

                    } else {
                        // this is a product variant edit
                        // in this case we need the specific variant
                        $scope.productVariant = $scope.product.getProductVariant(productVariantKey);
                        $scope.context = 'varianteditor';
                        $scope.tabs = merchelloTabsFactory.createProductVariantEditorTabs(key, productVariantKey);
                    }
                    $scope.tabs.setActive($scope.context);
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

            //--------------------------------------------------------------------------------------
            // Event Handlers
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Called when the Save button is pressed.  See comments below.
             */
            function save(thisForm) {
                // TODO we should unbind the return click event
                // so that we can quickly add the options and remove the following
                if(thisForm === undefined) {
                    return;
                }
                if (thisForm.$valid) {
                    $scope.preValuesLoaded = false;

                    if($scope.context === 'productedit') {
                        // Copy from master variant
                        var productOptions = $scope.product.productOptions;
                        $scope.product = $scope.productVariant.getProductForMasterVariant();
                        $scope.product.productOptions = productOptions;
                    }  else {
                        console.info($scope.product);
                    }// otherwise the variant is updated in the collection so we just need to save the product

                    var promise = productResource.save($scope.product);

                    promise.then(function (product) {
                        notificationsService.success("Product Saved", "");

                        $scope.product = productDisplayBuilder.transform(product);
                        if($scope.context === 'productedit') {
                            $scope.productVariant = $scope.product.getMasterVariant();
                            if ($scope.product.hasVariants()) {
                                $location.url("/merchello/merchello/producteditwithoptions/" + $scope.product.key, true);
                            }
                        } else {
                            $scope.productVariant = $scope.product.getProductVariant($scope.productVariantKey);
                        }

                        $scope.preValuesLoaded = true;
                    }, function (reason) {
                        notificationsService.error("Product Save Failed", reason.message);
                    });
                }
            };


            /**
             * @ngdoc method
             * @name deleteProductDialogConfirmation
             * @function
             *
             * @description
             * Called when the Delete Product button is pressed.
             */
            function deleteProductDialogConfirmation() {
                var promiseDel = merchelloProductService.deleteProduct($scope.product);

                promiseDel.then(function () {
                    notificationsService.success("Product Deleted", "");

                    $location.url("/merchello/merchello/ProductList/manage", true);

                }, function (reason) {
                    notificationsService.error("Product Deletion Failed", reason.message);
                });
            };

            /**
             * @ngdoc method
             * @name deleteProductDialog
             * @function
             *
             * @description
             * Opens the delete confirmation dialog via the Umbraco dialogService.
             */
            function deleteProductDialog() {

                dialogService.open({
                    template: '/App_Plugins/Merchello/Common/Js/Dialogs/deleteconfirmation.html',
                    show: true,
                    callback: $scope.deleteProductDialogConfirmation,
                    dialogData: $scope.product
                });
            }

            // Initialize the controller
            init();
    }]);
