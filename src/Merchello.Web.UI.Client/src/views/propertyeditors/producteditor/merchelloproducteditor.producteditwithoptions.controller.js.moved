'use strict';

(function () {

    function productEditorProductEditWithOptions($scope, $routeParams, merchelloProductService, notificationsService) {

        ////--------------------------------------------------------------------------------------
        //// Declare and initialize key scope properties
        ////--------------------------------------------------------------------------------------
        //// 
        $scope.allVariants = false;
        $scope.reorderVariants = false;




        ////--------------------------------------------------------------------------------------
        //// Initialization methods
        ////--------------------------------------------------------------------------------------


        /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
        $scope.init = function () {

            // Handle the form submitting event to save the product information to Merchello tables
            $scope.$on("formSubmitting", function (e, args) {
                //e.preventDefault();
                //e.stopPropagation();
                if (args.scope.contentForm.$valid) {
                    notificationsService.success("Saving merchello product", "");
                    $scope.save();
                }
            });

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
        $scope.save = function () {

            var promise = merchelloProductService.updateProduct($scope.product);

            promise.then(function (product) {
                notificationsService.success("Product Saved", "");

                $scope.product = product;

                if (!$scope.product.hasVariants && !$scope.product.hasOptions) {
                    $scope.$parent.changeTemplate("/App_Plugins/Merchello/PropertyEditors/ProductEditor/Views/merchelloproducteditor.productedit.html");
                }

            }, function (reason) {
                notificationsService.error("Product Save Failed", reason.message);
            });
        };

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
                return _.filter($scope.product.productVariants, function (v) {
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

    angular.module("umbraco").controller('Merchello.PropertyEditors.MerchelloProductEditor.ProductEditWithOptions', ['$scope', '$routeParams', 'merchelloProductService', 'notificationsService', productEditorProductEditWithOptions]);

})();