(function (directives, undefined) {

    /**
     * @ngdoc directive
     * @name ProductOptionsReorder
     * @function
     * 
     * @description
     * directive to sort the options / attributes on the ProductEditWithOptions view
     */
    directives.ProductOptionsReorder = function () {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                isolateIsOpen: '=isOpen',
                product: '='
            },
            templateUrl: '/App_Plugins/Merchello/Modules/Catalog/Directives/product-options-reorder.html',            
            link: function ($scope, $element) {

                /**
                 * @ngdoc method
                 * @name close
                 * @function
                 * 
                 * @description
                 * Set the isOpen scope property to false to close the dialog
                 *
                 * TODO: This doesn't set the parent scope property even though it should be.  Investigate and fix or workaround.
                 */
                $scope.close = function ($event) {

                    $scope.isolateIsOpen = false;
                };

                // Settings for the sortable directive
                $scope.sortableOptions = {
                    stop: function (e, ui) {
                        for (var i = 0; i < $scope.product.productOptions.length; i++) {
                            $scope.product.productOptions[i].setSortOrder(i + 1);
                        }
                        $scope.product.fixAttributeSortOrders();
                    },
                    axis: 'y',
                    cursor: "move"
                };

                $scope.sortableChoices = {
                    start: function (e, ui) {
                        $(e.target).data("ui-sortable").floating = true;    // fix for jQui horizontal sorting issue https://github.com/angular-ui/ui-sortable/issues/19
                    },
                    stop: function (e, ui) {
                        var attr = ui.item.scope().attribute;
                        var attrOption = attr.findMyOption($scope.product.productOptions);
                        attrOption.resetChoiceSortOrder();
                    },
                    update: function (e, ui) {
                        var attr = ui.item.scope().attribute;
                        var attrOption = attr.findMyOption($scope.product.productOptions);
                        attrOption.resetChoiceSortOrder();
                    },
                    cursor: "move"
                };

            }

        };
    };

    angular.module("umbraco").directive('productOptionsReorder', merchello.Directives.ProductOptionsReorder);




    /**
     * @ngdoc directive
     * @name ProductOptionAttributesSelect
     * @function
     * 
     * @description
     * directive to select option attributes that is used for bulk variant editing
     */
    directives.ProductOptionAttributesSelect = function () {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                product: '=',
                'selectVariants': '&onSelectVariants'
            },
            templateUrl: '/App_Plugins/Merchello/Modules/Catalog/Directives/product-option-attributes-selection.html'
        };
    };

    angular.module("umbraco").directive('productOptionAttributesSelect', merchello.Directives.ProductOptionAttributesSelect);




    /**
     * @ngdoc directive
     * @name ProductVariantsViewTable
     * @function
     * 
     * @description
     * table of variants that can be selected for bulk editing.
     */
    directives.ProductVariantsViewTable = function ($q, dialogService, notificationsService, merchelloProductService) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                product: '=',
                allVariants: '=',
                'checkBulkVariantsSelected': '&onCheckBulkVariantsSelected',
                'selectedVariants': '&'
        },
            templateUrl: '/App_Plugins/Merchello/Modules/Catalog/Directives/product-variants-view-table.html',
            link: function ($scope, $element) {

                $scope.sortProperty = "sku";
                $scope.sortOrder = "asc";
                $scope.bulkAction = false,

                /**
                 * @ngdoc method
                 * @name changeSortOrder
                 * @function
                 * 
                 * @description
                 * Event handler for changing the rows sort by column when a header column item is clicked
                 *
                 */
                $scope.changeSortOrder = function (propertyToSort) {

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
                };

                /**
                 * @ngdoc method
                 * @name toggleAVariant
                 * @function
                 * 
                 * @description
                 * Event handler toggling the variant's selected state
                 *
                 */
                $scope.toggleAVariant = function (variant) {
                    variant.selected = !variant.selected;
                    $scope.checkBulkVariantsSelected();
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

                //--------------------------------------------------------------------------------------
                // Dialog Event Handlers
                //--------------------------------------------------------------------------------------

                /**
                 * @ngdoc method
                 * @name changePricesDialogConfirm
                 * @param {dialogData} contains the newPrice that is the price to adjust the variants price to.
                 * @function
                 * 
                 * @description
                 * Handles the new price passed back from the dialog and sets the variants price and saves them.
                 */
                $scope.changePricesDialogConfirm = function (dialogData) {
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
                };

                /**
                 * @ngdoc method
                 * @name changePrices
                 * @function
                 * 
                 * @description
                 * Opens the dialog for setting the new price
                 */
                $scope.changePrices = function () {
                    dialogService.open({
                        template: '/App_Plugins/Merchello/Modules/Catalog/Dialogs/product.changeprices.html',
                        show: true,
                        callback: $scope.changePricesDialogConfirm
                    });
                };

                /**
                 * @ngdoc method
                 * @name updateInventoryDialogConfirm
                 * @param {dialogData} contains the new inventory that is the price to adjust the variants price to.
                 * @function
                 * 
                 * @description
                 * Handles the new inventory passed back from the dialog and sets the variants inventory and saves them.
                 */
                $scope.updateInventoryDialogConfirm = function (dialogData) {
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
                };

                /**
                 * @ngdoc method
                 * @name updateInventory
                 * @function
                 * 
                 * @description
                 * Opens the dialog for setting the new inventory
                 */
                $scope.updateInventory = function () {
                    dialogService.open({
                        template: '/App_Plugins/Merchello/Modules/Catalog/Dialogs/product.updateinventory.html',
                        show: true,
                        callback: $scope.updateInventoryDialogConfirm
                    });
                };

                /**
                 * @ngdoc method
                 * @name deleteVariantsDialogConfirm
                 * @param {dialogData} contains the confirm text that validates they really want to delete the variants
                 * @function
                 * 
                 * @description
                 * Handles the new deletion of variants when the user correctly types in the word DELETE
                 *
                 * TODO: localize the word DELETE?
                 */
                $scope.deleteVariantsDialogConfirm = function (dialogData) {
                    if (dialogData.confirmText == "DELETE") {
                        var selected = $scope.selectedVariants();
                        var promisesArray = [];

                        //for (var i = 0; i < selected.length; i++) {
                        //    promisesArray.push(merchelloProductVariantService.deleteVariant(selected[i].key));
                        //}

                        //var promise = $q.all(promisesArray);

                        //promise.then(function () {

                        //    var promiseLoadProduct = merchelloProductService.getByKey($scope.product.key);
                        //    promiseLoadProduct.then(function (dbproduct) {
                        //        $scope.product = new merchello.Models.Product(dbproduct);

                        //        notificationsService.success("Variants Deleted");
                        //    }, function (reason) {
                        //        notificationsService.error("Product Variants Delete Failed", reason.message);
                        //    });
                        //}, function (reason) {
                        //    notificationsService.error("Product Variants Delete Failed", reason.message);
                        //});
                    }
                    else {
                        notificationsService.error("Type the word DELETE in the box to confirm deletion", "");
                    }
                };

                /**
                 * @ngdoc method
                 * @name deleteVariants
                 * @function
                 * 
                 * @description
                 * Opens the dialog for confirming deletion of variants
                 */
                $scope.deleteVariants = function () {
                    dialogService.open({
                        template: '/App_Plugins/Merchello/Modules/Catalog/Dialogs/product.deletevariants.html',
                        show: true,
                        callback: $scope.deleteVariantsDialogConfirm
                    });
                };

                /**
                 * @ngdoc method
                 * @name duplicateVariantsDialogConfirm
                 * @param {dialogData} contains the option that each variant should be duplicated with
                 * @function
                 * 
                 * @description
                 * Handles the duplication of a variant into a new variant
                 *
                 */
                //$scope.duplicateVariantsDialogConfirm = function (option) {
                //    var submittedChoiceName = option.newChoiceName;
                //    if (submittedChoiceName != undefined) {
                //        if (submittedChoiceName.length > 0) {

                //            var options = [];
                //            for (var o = 0; o < $scope.product.productOptions.length; o++) {

                //                var currentOption = $scope.product.productOptions[o];

                //                var tempOption = new merchello.Models.ProductOption();
                //                tempOption.name = currentOption.name;
                //                tempOption.key = currentOption.key;
                //                tempOption.sortOrder = currentOption.sortOrder;

                //                if (currentOption.key == option.key) {
                //                    var foundChoice = currentOption.findChoiceByName(submittedChoiceName);
                //                    if (foundChoice != undefined) {
                //                        tempOption.choices.push(foundChoice);
                //                    }
                //                    else {
                //                        currentOption.addChoice(submittedChoiceName);
                //                        tempOption.addChoice(submittedChoiceName);
                //                    }
                //                }

                //                options.push(tempOption);
                //            }

                //            var selected = $scope.selectedVariants();
                //            for (var i = 0; i < selected.length; i++) {

                //                var thisVariant = selected[i];

                //                for (var a = 0; a < thisVariant.attributes.length; a++) {
                //                    var attr = thisVariant.attributes[a];
                //                    if (attr.optionKey != option.key) {
                //                        // add this attribute to the correct option
                //                        var myOption = attr.findMyOption(options);

                //                        if (!myOption.choiceExists(attr)) {
                //                            myOption.choices.push(attr);
                //                        }
                //                    }
                //                }
                //            }

                //            merchelloProductService.createVariantsFromDetachedOptionsList($scope.product, options);

                //            // Save immediately
                //            var savepromise = merchelloProductService.updateProductWithVariants($scope.product);
                //            savepromise.then(function (product) {
                //                $scope.product = product;
                //                notificationsService.success("Confirmed variants duplicated");
                //            }, function (reason) {
                //                notificationsService.error("Product Save Failed", reason.message);
                //            });
                //        }
                //    }
                //};

                /**
                 * @ngdoc method
                 * @name duplicateVariants
                 * @function
                 * 
                 * @description
                 * Opens the dialog for duplication of variants
                 */
                //$scope.duplicateVariants = function (option) {
                //    if (option == undefined) {
                //        option = new merchello.Models.ProductOption();
                //    }

                //    dialogService.open({
                //        template: '/App_Plugins/Merchello/Modules/Catalog/Dialogs/product.duplicatevariants.html',
                //        show: true,
                //        callback: $scope.duplicateVariantsDialogConfirm,
                //        dialogData: option
                //    });
                //};
            }
        };
    };

    angular.module("umbraco").directive('productVariantsViewTable', ['$q', 'dialogService', 'notificationsService', 'merchelloProductService', merchello.Directives.ProductVariantsViewTable]);

}(window.merchello.Directives = window.merchello.Directives || {}));