/**
 * @ngdoc controller
 * @name Merchello.Directives.ProductVariantsViewTableDirectiveController
 * @function
 *
 * @description
 * The controller for the product variant view table view directive
 */
angular.module('merchello').controller('Merchello.Directives.ProductVariantsViewTableDirectiveController',
    ['$scope', 'productResource', 'productDisplayBuilder', 'productVariantDisplayBuilder',
    function($scope, productResource, productDisplayBuilder, productVariantDisplayBuilder) {

        $scope.sortProperty = "sku";
        $scope.sortOrder = "asc";
        $scope.bulkAction = true;
        $scope.allVariants = false;

        // exposed methods
        $scope.assertActiveShippingCatalog = assertActiveShippingCatalog;
        $scope.selectedVariants = selectedVariants;
        $scope.selectVariants = selectVariants;
        $scope.checkBulkVariantsSelected = checkBulkVariantsSelected;
        $scope.changeSortOrder = changeSortOrder;
        $scope.toggleAVariant = toggleAVariant;
        $scope.toggleAllVariants = toggleAllVariants;
        $scope.changePrices = changePrices;
        $scope.updateInventory = updateInventory;
        $scope.toggleOnSale = toggleOnSale;
        $scope.toggleAvailable = toggleAvailable;

        function init() {
            angular.forEach($scope.product.productVariants, function(pv) {
                pv.selected = false;
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
        function selectVariants(attributeToSelect) {
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
            checkBulkVariantsSelected();
        }

        /**
         * @ngdoc method
         * @name selectedVariants
         * @function
         *
         * @description
         * This is a helper method to get a collection of variants that are selected.
         *
         */
        function selectedVariants() {
            if ($scope.product !== undefined) {
                return _.filter($scope.product.productVariants, function(v) {
                    return v.selected;
                });
            } else {
                return [];
            }
        }

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
        function checkBulkVariantsSelected() {
            var v = selectedVariants();
            if (v.length >= 1) {
                $scope.allVariants = true;
            } else {
                $scope.allVariants = false;
            }
        }

        /**
         * @ngdoc method
         * @name changeSortOrder
         * @function
         *
         * @description
         * Event handler for changing the rows sort by column when a header column item is clicked
         *
         */
        function changeSortOrder(propertyToSort) {

            if ($scope.sortProperty == propertyToSort) {
                if ($scope.sortOrder == "asc") {
                    $scope.sortProperty = "-" + propertyToSort;
                    $scope.sortOrder = "desc";
                } else {
                    $scope.sortProperty = propertyToSort;
                    $scope.sortOrder = "asc";
                }
            } else {
                $scope.sortProperty = propertyToSort;
                $scope.sortOrder = "asc";
            }
        }

        /**
         * @ngdoc method
         * @name toggleAVariant
         * @function
         *
         * @description
         * Event handler toggling the variant's selected state
         *
         */
        function toggleAVariant(variant) {
            variant.selected = !variant.selected;
            $scope.checkBulkVariantsSelected();
        }

        /**
         * @ngdoc method
         * @name toggleAllVariants
         * @function
         *
         * @description
         * Event handler toggling the all of the product variant's selected state
         *
         */
        function toggleAllVariants(newstate) {
            for (var i = 0; i < $scope.product.productVariants.length; i++) {
                $scope.product.productVariants[i].selected = newstate;
            }
            $scope.allVariants = newstate;
        }

        //--------------------------------------------------------------------------------------
        // Dialog Event Handlers
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name changePrices
         * @function
         *
         * @description
         * Opens the dialog for setting the new price
         */
        function changePrices() {
            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Catalog/Dialogs/product.changeprices.html',
                show: true,
                callback: $scope.changePricesDialogConfirm
            });
        }

        /**
         * @ngdoc method
         * @name updateInventory
         * @function
         *
         * @description
         * Opens the dialog for setting the new inventory
         */
        function updateInventory() {
            dialogService.open({
                template: '/App_Plugins/Merchello/Modules/Catalog/Dialogs/product.updateinventory.html',
                show: true,
                callback: $scope.updateInventoryDialogConfirm
            });
        }

        /**
         * @ngdoc method
         * @name toggleAvailable
         * @function
         *
         * @description
         * Toggles the variant available setting
         */
        function toggleAvailable() {
            var success = true;
            var selected = $scope.selectedVariants();
            for (var i = 0; i < selected.length; i++) {
                selected[i].available = !selected[i].available;
                var savepromise = merchelloProductService.updateProductVariant(selected[i]);
                savepromise.then(function () {
                    //notificationsService.success("Product Variant Saved", "");
                }, function (reason) {
                    success = false;
                    //notificationsService.error("Product Variant Save Failed", reason.message);
                });
            }
            if (success) {
                notificationsService.success("Confirmed available update", "");
            } else {
                notificationsService.error("Failed to update available", "");
            }
        }

        function toggleOnSale() {
            var success = true;
            var selected = $scope.selectedVariants();
            for (var i = 0; i < selected.length; i++) {
                selected[i].onSale = !selected[i].onSale;
                var savepromise = merchelloProductService.updateProductVariant(selected[i]);
                savepromise.then(function () {
                    //notificationsService.success("Product Variant Saved", "");
                }, function (reason) {
                    success = false;
                    //notificationsService.error("Product Variant Save Failed", reason.message);
                });
            }
            if (success) {
                notificationsService.success("Confirmed on sale update", "");
            } else {
                notificationsService.error("Failed to update on sale setting", "");
            }
        }

        /**
         * @ngdoc method
         * @name updateInventoryDialogConfirm
         * @param {dialogData} contains the new inventory that is the price to adjust the variants price to.
         * @function
         *
         * @description
         * Handles the new inventory passed back from the dialog and sets the variants inventory and saves them.
         */
        function updateInventoryDialogConfirm(dialogData) {
            var success = true;
            var selected = $scope.selectedVariants();
            for (var i = 0; i < selected.length; i++) {
                selected[i].globalInventoryChanged(dialogData.newInventory);
                var savepromise = merchelloProductService.updateProductVariant(selected[i]);
                savepromise.then(function () {
                    //notificationsService.success("Product Variant Saved", "");
                }, function (reason) {
                    success = false;
                    //notificationsService.error("Product Variant Save Failed", reason.message);
                });
            }
            if (success) {
                notificationsService.success("Confirmed inventory update", "");
            } else {
                notificationsService.error("Failed to update inventory", "");
            }
        }

        /**
         * @ngdoc method
         * @name changePricesDialogConfirm
         * @param {dialogData} contains the newPrice that is the price to adjust the variants price to.
         * @function
         *
         * @description
         * Handles the new price passed back from the dialog and sets the variants price and saves them.
         */
        function changePricesDialogConfirm(dialogData) {
            var success = true;
            var selected = $scope.selectedVariants();
            for (var i = 0; i < selected.length; i++) {
                selected[i].price = dialogData.newPrice;
                var savepromise = merchelloProductService.updateProductVariant(selected[i]);
                savepromise.then(function () {
                    //notificationsService.success("Variant saved ", "");
                }, function (reason) {
                    success = false;
                    //notificationsService.error("Product Variant Save Failed", reason.message);
                });
            }
            if (success) {
                notificationsService.success("Confirmed prices update", "");
            } else {
                notificationsService.error("Failed to update prices", "");
            }
        }

        function assertActiveShippingCatalog() {
            $scope.assertCatalog();
        }

        // initialize the controller
        init();
}]);
