/**
 * @ngdoc controller
 * @name Merchello.Directives.ProductVariantsViewTableDirectiveController
 * @function
 *
 * @description
 * The controller for the product variant view table view directive
 */
angular.module('merchello').controller('Merchello.Directives.ProductVariantsViewTableDirectiveController',
    ['$scope', '$q', '$timeout', '$location', 'notificationsService', 'dialogService', 'localizationService', 'dialogDataFactory', 'productResource', 'productDisplayBuilder', 'productVariantDisplayBuilder',
    function($scope, $q, $timeout, $location, notificationsService, dialogService, localizationService, dialogDataFactory, productResource, productDisplayBuilder, productVariantDisplayBuilder) {

        $scope.sortProperty = "sku";
        $scope.sortOrder = "asc";
        $scope.bulkAction = true;
        $scope.allVariants = false;

        $scope.checkAll = false;

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
        $scope.redirectToEditor = redirectToEditor;
        $scope.getVariantAttributeForOption = getVariantAttributeForOption;
        $scope.regenSkus = regenSkus;

        $scope.toggleChecks = function() {
            if ($scope.checkAll === true) {
                selectVariants('All');
            } else {
                selectVariants('None');
            }
        }


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

            if (attributeToSelect === "All") {
                filteredVariants = $scope.product.productVariants;
            } else if (attributeToSelect === "None") {
                filteredVariants = [];
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

        function getVariantAttributeForOption(productVariant, option) {
            var att = _.find(productVariant.attributes, function(pa) {
               return pa.optionKey === option.key;
            });

            if (att) {
                return att.name;
            } else {
                return '';
            }
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

            var promises = [];
            var selected = $scope.selectedVariants();
            for (var i = 0; i < selected.length; i++) {
                selected[i].available = !selected[i].available;
                promises.push(productResource.saveVariant(selected[i]));
            }
            persistPromises(promises, "Confirmed available update");
        }


        $scope.toggleTracksInventory = function() {
            var promises = [];
            var selected = $scope.selectedVariants();
            for(var i = 0; i < selected.length; i++) {
                selected[i].trackInventory = !selected[i].trackInventory;
                promises.push(productResource.saveVariant(selected[i]));
            }

            persistPromises(promises, "Confirmed tracks inventory update");

        }

        function persistPromises(promises, msg) {
            if (promises.length === 0) {
                return;
            }

            $q.all(promises).then(function(data) {
                notificationsService.success(msg, "");
                $scope.checkAll = false;
                selectVariants('None');
                $timeout(function() {
                    reload();
                }, 400);
            });
        }

        //--------------------------------------------------------------------------------------
        // Dialog Event Handlers
        //--------------------------------------------------------------------------------------

        function regenSkus() {
            var dialogData = { name: $scope.product.name }
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/productvariant.bulk.skuupdate.html',
                show: true,
                callback: regenSkusConfirm,
                dialogData: dialogData
            });
        }

        /**
         * @ngdoc method
         * @name changePrices
         * @function
         *
         * @description
         * Opens the dialog for setting the new price
         */
        function changePrices() {
            var dialogData = dialogDataFactory.createBulkVariantChangePricesDialogData();
            dialogData.productVariants = $scope.selectedVariants();
            dialogData.price = _.min(dialogData.productVariants, function(v) { return v.price;}).price;
            dialogData.salePrice = _.min(dialogData.productVariants, function (v) { return v.salePrice; }).price;
            dialogData.costOfGoods = _.min(dialogData.productVariants, function (v) { return v.costOfGoods; }).costOfGoods;
            dialogData.currencySymbol = $scope.currencySymbol;
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/productvariant.bulk.changeprice.html',
                show: true,
                callback: changePricesDialogConfirm,
                dialogData: dialogData
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
            var dialogData = dialogDataFactory.createBulkEditInventoryCountsDialogData();
            dialogData.warning = localizationService.localize('merchelloVariant_bulkChangeInventoryNote');
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/productvariant.bulk.updateinventory.html',
                show: true,
                callback: updateInventoryDialogConfirm,
                dialogData: dialogData
            });
        }


        function toggleOnSale() {
            var promises = [];
            var selected = $scope.selectedVariants();
            for (var i = 0; i < selected.length; i++) {
                selected[i].onSale = !selected[i].onSale;
                promises.push(productResource.saveVariant(selected[i]));

            }
            persistPromises(promises, "Confirmed on sale update");
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
            var promises = [];
            var selected = $scope.selectedVariants();
            for (var i = 0; i < selected.length; i++) {
                selected[i].setAllInventoryCount(dialogData.count);
                if(dialogData.includeLowCount) {
                    selected[i].setAllInventoryLowCount(dialogData.lowCount);
                }
                promises.push(productResource.saveVariant(selected[i]));
            }
            persistPromises(promises, "Confirmed inventory update");
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
            angular.forEach(dialogData.productVariants,
                function(pv) {
                    pv.price = dialogData.price;
                    if (dialogData.includeCostOfGoods) {
                        pv.costOfGoods = dialogData.costOfGoods;
                    }
                    if (dialogData.includeSalePrice) {
                        pv.salePrice = dialogData.salePrice;
                    }
                    productResource.saveVariant(pv);
                });
            notificationsService.success("Updated prices");
        }

        function regenSkusConfirm() {
            console.info($scope.product);
            productResource.resetSkus($scope.product).then(function() {
                $scope.reload();
            });
            console.info('Got here');
        }

        function assertActiveShippingCatalog() {
            $scope.assertCatalog();
        }

        function reload() {
            $scope.reload();
        }

        function redirectToEditor(variant) {
           $location.url('/merchello/merchello/productedit/' + variant.productKey + '?variantid=' + variant.key, true);
        }

        // initialize the controller
        init();
}]);
