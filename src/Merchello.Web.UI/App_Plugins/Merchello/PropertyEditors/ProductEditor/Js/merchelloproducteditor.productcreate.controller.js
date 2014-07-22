'use strict';

(function () {

    function productEditorProductCreate($scope, $routeParams, merchelloProductService, notificationsService, dialogService, merchelloWarehouseService, merchelloSettingsService) {

        ////--------------------------------------------------------------------------------------
        //// Declare and initialize key scope properties
        ////--------------------------------------------------------------------------------------
        //// 




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
            //$scope.$on("formSubmitting", function (e, args) {
            //    //e.preventDefault();
            //    //e.stopPropagation();
            //    if (args.scope.contentForm.$valid) {
            //        notificationsService.success("Saving merchello product", "");
            //        $scope.save();
            //    }
            //});

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

            if ($scope.creatingProduct) // Save on initial create
            {
                notificationsService.success("Creating merchello product", "");

                if (!$scope.product.hasVariants && $scope.product.productOptions.length > 0) // The options checkbox was checked, a blank option was added, then the has options was unchecked
                {
                    $scope.product.productOptions = [];
                }

                // Copy from master variant
                $scope.product.copyFromVariant($scope.productVariant);

                var promiseCreate = merchelloProductService.createProduct($scope.product, function () {
                    $scope.creatingProduct = false;
                    //notificationsService.success("*** Product ", status);
                });
                promiseCreate.then(function (product) {

                    $scope.product = product;
                    $scope.productVariant.copyFromProduct($scope.product);

                    $scope.$parent.creatingProduct = false; // For the variant edit/create view.

                    $scope.$parent.model.value = $scope.product.key;

                    if ($scope.product.hasVariants) {
                        $scope.$parent.changeTemplate("/App_Plugins/Merchello/PropertyEditors/ProductEditor/Views/merchelloproducteditor.producteditwithoptions.html");
                    } else {
                        $scope.$parent.changeTemplate("/App_Plugins/Merchello/PropertyEditors/ProductEditor/Views/merchelloproducteditor.productedit.html");
                    }
                    

                    notificationsService.success("Product Created and Saved", "");

                }, function (reason) {
                    notificationsService.error("Product Create Failed", reason.message);
                });
            }
        };

        /**
        * @ngdoc method
        * @name updateVariants
        * @function
        * 
        * @description
        * Called when the Update button is pressed below the options.  This will create a new product if necessary 
        * and save the product.  Then the product variants are generated.
        * 
        * We have to create the product because the API cannot create the variants with a product with options.
        */
        $scope.updateVariants = function (thisForm) {

            if (thisForm.$valid) {
                // Copy from master variant
                $scope.product.copyFromVariant($scope.productVariant);

                var promise = merchelloProductService.updateProduct($scope.product);

                promise.then(function(product) {
                    notificationsService.success("Product Saved", "");

                    $scope.product = product;
                    $scope.productVariant.copyFromProduct($scope.product);

                    $scope.product = merchelloProductService.createVariantsFromOptions($scope.product);

                }, function(reason) {
                    notificationsService.error("Product Save Failed", reason.message);
                });
            }

        };


    };

    angular.module("umbraco").controller('Merchello.PropertyEditors.MerchelloProductEditor.ProductCreate', ['$scope', '$routeParams', 'merchelloProductService', 'notificationsService', 'dialogService', 'merchelloWarehouseService', 'merchelloSettingsService', productEditorProductCreate]);

})();