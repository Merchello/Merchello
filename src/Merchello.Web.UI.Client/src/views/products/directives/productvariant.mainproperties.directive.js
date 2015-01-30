    /**
     * @ngdoc controller
     * @name productVariantMainProperties
     * @function
     *
     * @description
     * The productVariantMainProperties directive
     */
    angular.module('merchello.directives').directive('productVariantMainProperties',
        [function() {

            return {
                restrict: 'E',
                replace: true,
                scope: {
                    product: '=',
                    productVariant: '=',
                    context: '=',
                    settings: '='
                },
                templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/productvariant.mainproperties.tpl.html',
                controller: function ($scope, warehouseResource, warehouseDisplayBuilder, catalogInventoryDisplayBuilder) {

                    // Get the default warehouse for the ensureCatalogInventory() function below
                    $scope.defaultWarehouse = {};

                    function init() {
                        var promiseWarehouse = warehouseResource.getDefaultWarehouse();
                        promiseWarehouse.then(function (warehouse) {
                            $scope.defaultWarehouse = warehouseDisplayBuilder.transform(warehouse);
                            // set defaults in case of a createproduct
                            if($scope.context === 'createproduct') {
                                $scope.productVariant.shippable = $scope.settings.globalShippable;
                                $scope.productVariant.taxable = $scope.settings.globalTaxable;
                                $scope.productVariant.trackInventory = $scope.settings.globalTrackInventory;
                            }
                        });
                    }

                    // Initialize the controller
                    init();
                }
            };
    }]);
