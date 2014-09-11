(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Product.EditWithOptionsController
     * @function
     * 
     * @description
     * The controller for the product editor
     */
    controllers.ProductEditWithOptionsController = function ($scope, $routeParams, $location, $q, assetsService, notificationsService, dialogService, angularHelper, serverValidationManager, merchelloProductService) {

        //load the seperat css for the editor to avoid it blocking our js loading
        assetsService.loadCss("/App_Plugins/Merchello/Common/Css/merchello.css");

        //--------------------------------------------------------------------------------------
        // Declare and initialize key scope properties
        //--------------------------------------------------------------------------------------
        // 

        $scope.currentTab = "Variants";

        $scope.allVariants = false;
        $scope.reorderVariants = false;



        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name loadProduct
         * @function
         * 
         * @description
         * Load a product by the product key.
         */
        function loadProduct(key) {

            var promise = merchelloProductService.getByKey(key);

            promise.then(function (product) {

                $scope.product = new merchello.Models.Product(product);

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {
                notificationsService.error("Product Load Failed", reason.message);
            });

        }

        /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
        $scope.init = function () {

            if ($routeParams.create) {
                // TODO: this should redirect to product variant edit
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                $scope.product = new merchello.Models.Product();
            }
            else {
                //we are editing so get the product from the server
                loadProduct($routeParams.id);
            }
        };

        $scope.init();


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
        $scope.save = function (thisForm, rebuildVariants) {

            if (thisForm) {
                if (thisForm.$valid) {

                    notificationsService.info("Saving Product...", "");

                    var promise = merchelloProductService.updateProduct($scope.product);

                    promise.then(function (product) {
                        notificationsService.success("Product Saved", "");

                        $scope.product = product;

                        //if (rebuildVariants) {
                        //    notificationsService.info("rebuilding Variants", "");
                        //    $scope.product = merchelloProductService.createVariantsFromOptions($scope.product);

                        //    promise = merchelloProductService.updateProductWithVariants($scope.product, false);
                        //    promise.then(function(product2) {
                        //        notificationsService.success("Product Saved 2", "");
                        //        $scope.product = product2;
                        //        $scope.toggleAllVariants(false);
                        //    });
                        //}

                        if (!$scope.product.hasVariants && !$scope.product.hasOptions) {
                            $location.url("/merchello/merchello/ProductEdit/" + $scope.product.key, true);
                        }

                    }, function (reason) {
                        notificationsService.error("Product Save Failed", reason.message);
                    });
                }              
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
        $scope.deleteProductDialogConfirmation = function () {
            var promiseDel = merchelloProductService.deleteProduct($scope.product);

            promiseDel.then(function() {
                notificationsService.success("Product Deleted", "");

                $location.url("/merchello/merchello/ProductList/manage", true);

            }, function(reason) {
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
        $scope.deleteProductDialog = function () {
            
            dialogService.open({
                template: '/App_Plugins/Merchello/Common/Js/Dialogs/deleteconfirmation.html',
                show: true,
                callback: $scope.deleteProductDialogConfirmation,
                dialogData: $scope.product
            });
        }

        /**
         * @ngdoc method
         * @name selectVariants
         * @function
         * 
         * @description
         * Called by the ProductOptionAttributesSelection directive when an attribute is selected.
         * It will select the variants that have that option attribute and mark their selected property to true.
         * All other variants will have selected set to false.
         *
         */
        $scope.selectVariants = function (attributeToSelect) {

            // Reset the selected state to false for all variants
            for (var i = 0; i < $scope.product.productVariants.length; i++) {
                $scope.product.productVariants[i].selected = false;
            }

            // Build a list of variants to select
            var filteredVariants = [];

            if (attributeToSelect == "All") {
                filteredVariants = $scope.product.productVariants;
            } else if (attributeToSelect == "None") {
            } else {
                filteredVariants = _.filter($scope.product.productVariants,
                    function (variant) {
                        return _.where(variant.attributes, { name: attributeToSelect }).length > 0;
                    });
            }

            // Set the selected state to true for all variants
            for (var v = 0; v < filteredVariants.length; v++) {
                filteredVariants[v].selected = true;
            }

            // Set the property to toggle in the bulk menu in the table header
            $scope.checkBulkVariantsSelected();
        };

        /**
        * @ngdoc method
        * @name selectTab
        * @function
        * 
        * @description
        * Event handler to set the current selected tab.
        *
        */
        $scope.selectTab = function (tabname) {
            $scope.currentTab = tabname;
        };


        //--------------------------------------------------------------------------------------
        // Helper Methods
        //--------------------------------------------------------------------------------------

        /**
        * @ngdoc method
        * @name selectedVariants
        * @function
        * 
        * @description
        * This is a helper method to get a collection of variants that are selected.
        *
        */
        $scope.selectedVariants = function () {
            if ($scope.product != undefined) {
                return _.filter($scope.product.productVariants, function(v) {
                    return v.selected;
                });
            } else {
                return [];
            }
        };

        /**
        * @ngdoc method
        * @name checkBulkVariantsSelected
        * @function
        * 
        * @description
        * This is a helper method to set the allVariants flag when one or more variants on selected 
        * in the Variant Information table on the Variant tab.
        *
        */
        $scope.checkBulkVariantsSelected = function () {
            var v = $scope.selectedVariants();
            if (v.length >= 1) {
                $scope.allVariants = true;
            } else {
                $scope.allVariants = false;
            }
        };

        /**
         * @ngdoc method
         * @name toggleAllVariants
         * @function
         * 
         * @description
         * Event handler toggling the all of the product variant's selected state
         *
         */
        $scope.toggleAllVariants = function (newstate) {
            for (var i = 0; i < $scope.product.productVariants.length; i++) {
                $scope.product.productVariants[i].selected = newstate;
            }
            $scope.allVariants = newstate;
        };

    };

    angular.module("umbraco").controller("Merchello.Editors.Product.EditWithOptionsController", ['$scope', '$routeParams', '$location', '$q', 'assetsService', 'notificationsService', 'dialogService', 'angularHelper', 'serverValidationManager', 'merchelloProductService', merchello.Controllers.ProductEditWithOptionsController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

