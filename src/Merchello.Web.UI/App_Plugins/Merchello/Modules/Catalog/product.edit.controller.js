(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Product.EditController
     * @function
     * 
     * @description
     * The controller for the product editor
     */
    controllers.ProductEditController = function ($scope, $routeParams, $location, $q, assetsService, notificationsService, dialogService, angularHelper, serverValidationManager, merchelloProductService, merchelloProductVariantService) {

        $scope.currentTab = "Variants";

        $scope.sortProperty = "sku";
        $scope.sortOrder = "asc";
        $scope.allVariants = false;
        $scope.bulkAction = false,
        $scope.selectedVariants = [];
        $scope.rebuildVariants = false;

        $scope.flyouts = {
            reorderVariants: false,
        };

        //load the seperat css for the editor to avoid it blocking our js loading
        assetsService.loadCss("/App_Plugins/Merchello/Common/Css/merchello.css");
        assetsService.loadCss("/App_Plugins/Merchello/lib/JsonTree/jsontree.css");

        if ($routeParams.create) {

            // TODO: this should redirect to product variant edit
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.product = new merchello.Models.Product();
        }
        else {

            //we are editing so get the product from the server
            var promise = merchelloProductService.getByKey($routeParams.id);

            promise.then(function (product) {

                $scope.product = new merchello.Models.Product(product);

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {
                notificationsService.error("Product Load Failed", reason.message);
            });
        }

        $scope.save = function (thisForm) {

            if (thisForm.$valid) {

                notificationsService.info("Saving...", "");

                //we are editing so get the product from the server
                var promise = merchelloProductService.updateProduct($scope.product);

                promise.then(function (product) {
                    notificationsService.success("Product Saved", "H5YR!");

                    $scope.product = product;

                    if ($scope.rebuildVariants) {
                        $scope.rebuildAndSaveVariants();
                        $scope.rebuildVariants = false;
                        $scope.toggleAllVariants(false);
                    }

                }, function (reason) {
                    notificationsService.error("Product Save Failed", reason.message);
                });
            }
        };

        $scope.delete = function() {
            var promiseDel = merchelloProductService.deleteProduct($scope.product);

            promiseDel.then(function() {
                notificationsService.success("Product Deleted", "H5YR!");

                $location.url("/merchello/merchello/ProductList/manage", true);

            }, function(reason) {
                notificationsService.error("Product Deletion Failed", reason.message);
            });
        };

        $scope.changeSortOrder = function(propertyToSort) {

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
        
        $scope.sortableOptions = {
            stop: function (e, ui) {
                for (var i = 0; i < $scope.product.productOptions.length; i++)
                {
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

        $scope.selectedVariants = function() {
            if ($scope.product != undefined) {
                return _.filter($scope.product.productVariants, function(v) {
                    return v.selected;
                });
            } else {
                return [];
            }
        };

        $scope.checkBulkVariantsSelected = function() {
            var v = $scope.selectedVariants();
            if (v.length >= 1) {
                $scope.allVariants = true;
            } else {
                $scope.allVariants = false;
            }
        };

        $scope.toggleAVariant = function(variant) {
            variant.selected = !variant.selected;
            $scope.checkBulkVariantsSelected();
        };

        $scope.toggleAllVariants = function(newstate) {
            for (var i = 0; i < $scope.product.productVariants.length; i++) {
                $scope.product.productVariants[i].selected = newstate;
            }
        };

        $scope.selectVariants = function(attributeToSelect) {

            for (var i = 0; i < $scope.product.productVariants.length; i++) {
                $scope.product.productVariants[i].selected = false;
            }

            var filteredVariants = [];

            if (attributeToSelect == "All") {
                filteredVariants = $scope.product.productVariants;
            } else if (attributeToSelect == "None") {
            } else {
                filteredVariants = _.filter($scope.product.productVariants,
                    function(variant) {
                        return _.where(variant.attributes, { name: attributeToSelect }).length > 0;
                    });
            }

            for (var v = 0; v < filteredVariants.length; v++) {
                filteredVariants[v].selected = true;
            }

            $scope.checkBulkVariantsSelected();
        };

        $scope.changePricesDialogConfirm = function (dialogData) {
        	var selected = $scope.selectedVariants();
        	for (var i = 0; i < selected.length; i++) {
        		selected[i].price = dialogData.newPrice;
        		var savepromise = merchelloProductVariantService.save(selected[i]);
        		savepromise.then(function () {
        			notificationsService.success("Variant saved ", "");
        		}, function (reason) {
        			notificationsService.error("Product Variant Save Failed", reason.message);
        		});
        	}
        	notificationsService.success("Confirmed prices update", "");
        };

        $scope.changePrices = function () {
        	dialogService.open({
        		template: '/App_Plugins/Merchello/Modules/Catalog/Dialogs/product.changeprices.html',
        		show: true,
        		callback: $scope.changePricesDialogConfirm
        	});
        };

        $scope.updateInventoryDialogConfirm = function (dialogData) {
        	var selected = $scope.selectedVariants();
        	for (var i = 0; i < selected.length; i++) {
        		selected[i].globalInventoryChanged(dialogData.newInventory);
        		var savepromise = merchelloProductVariantService.save(selected[i]);
        		savepromise.then(function () {
        			notificationsService.success("Product Variant Saved", "");
        		}, function (reason) {
        			notificationsService.error("Product Variant Save Failed", reason.message);
        		});
        	}
        	notificationsService.success("Confirmed inventory update", "");
		};

        $scope.updateInventory = function () {
        	dialogService.open({
        		template: '/App_Plugins/Merchello/Modules/Catalog/Dialogs/product.updateinventory.html',
        		show: true,
        		callback: $scope.updateInventoryDialogConfirm
        	});
        };

        $scope.deleteVariantsDialogConfirm = function (dialogData) {
        	if (dialogData.confirmText == "DELETE") {
        		var selected = $scope.selectedVariants();
        		var promisesArray = [];

        		for (var i = 0; i < selected.length; i++) {
        			promisesArray.push(merchelloProductVariantService.deleteVariant(selected[i].key));
        		}

        		var promise = $q.all(promisesArray);

        		promise.then(function () {

        			var promiseLoadProduct = merchelloProductService.getByKey($scope.product.key);
        			promiseLoadProduct.then(function (dbproduct) {
        				$scope.product = new merchello.Models.Product(dbproduct);

        				notificationsService.success("Variants deleted");
        			}, function (reason) {
        				notificationsService.error("Product Variant Delete Failed", reason.message);
        			});
        		}, function (reason) {
        			notificationsService.error("Product Variant Delete Failed", reason.message);
        		});
        	}
        	else {
        		notificationsService.error("Type the word DELETE in the box to confirm deletion", "");
        	}
        };

        $scope.deleteVariants = function () {
        	dialogService.open({
        		template: '/App_Plugins/Merchello/Modules/Catalog/Dialogs/product.deletevariants.html',
        		show: true,
        		callback: $scope.deleteVariantsDialogConfirm
        	});
        };

        $scope.duplicateVariantsDialogConfirm = function (option) {
        	var submittedChoiceName = option.newChoiceName;
        	if (submittedChoiceName != undefined) {
        		if (submittedChoiceName.length > 0) {

        			var options = [];
        			for (var o = 0; o < $scope.product.productOptions.length; o++) {

        				var currentOption = $scope.product.productOptions[o];

        				var tempOption = new merchello.Models.ProductOption();
        				tempOption.name = currentOption.name;
        				tempOption.key = currentOption.key;
        				tempOption.sortOrder = currentOption.sortOrder;

        				if (currentOption.key == option.key) {
        					var foundChoice = currentOption.findChoiceByName(submittedChoiceName);
        					if (foundChoice != undefined) {
        						tempOption.choices.push(foundChoice);
        					}
        					else {
        						currentOption.addChoice(submittedChoiceName);
        						tempOption.addChoice(submittedChoiceName);
        					}
        				}

        				options.push(tempOption);
        			}

        			var selected = $scope.selectedVariants();
        			for (var i = 0; i < selected.length; i++) {

        				var thisVariant = selected[i];

        				for (var a = 0; a < thisVariant.attributes.length; a++) {
        					var attr = thisVariant.attributes[a];
        					if (attr.optionKey != option.key) {
        						// add this attribute to the correct option
        						var myOption = attr.findMyOption(options);

        						if (!myOption.choiceExists(attr)) {
        							myOption.choices.push(attr);
        						}
        					}
        				}
        			}

        			merchelloProductService.createVariantsFromDetachedOptionsList($scope.product, options);

        			// Save immediately
        			var savepromise = merchelloProductService.updateProductWithVariants($scope.product);
        			savepromise.then(function (product) {
        				$scope.product = product;
        				notificationsService.success("Confirmed variants duplicated");
        			}, function (reason) {
        				notificationsService.error("Product Save Failed", reason.message);
        			});
        		}
        	}
        };

        $scope.duplicateVariants = function (option) {
        	if (option == undefined) {
        		option = new merchello.Models.ProductOption();
        	}

        	dialogService.open({
        		template: '/App_Plugins/Merchello/Modules/Catalog/Dialogs/product.duplicatevariants.html',
        		show: true,
        		callback: $scope.duplicateVariantsDialogConfirm,
        		dialogData: option
        	});
        };

        $scope.selectTab = function(tabname) {
            $scope.currentTab = tabname;
        };

        $scope.addOption = function () {
            $scope.rebuildVariants = true;
            $scope.product.addBlankOption();
        };

        $scope.removeOption = function (option) {
            $scope.rebuildVariants = true;
            $scope.product.removeOption(option);
        };

        $scope.rebuildAndSaveVariants = function() {
            merchelloProductVariantService.deleteAllByProduct($scope.product.key);

            //$scope.product = merchelloProductService.createVariantsFromOptions($scope.product);

            // Save immediately
            //var savepromise = merchelloProductService.updateProductWithVariants($scope.product);
            //savepromise.then(function(product) {
            //    $scope.product = product;
            //}, function(reason) {
            //    notificationsService.error("Product Save Failed", reason.message);
            //});
        };

        $scope.prettyJson = function() {
            var $jsonInfo = $(".jsonInfo");
            $jsonInfo.jsontree($jsonInfo.html());
        };

    };

    angular.module("umbraco").controller("Merchello.Editors.Product.EditController", ['$scope', '$routeParams', '$location', '$q', 'assetsService', 'notificationsService', 'dialogService', 'angularHelper', 'serverValidationManager', 'merchelloProductService', 'merchelloProductVariantService', merchello.Controllers.ProductEditController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

