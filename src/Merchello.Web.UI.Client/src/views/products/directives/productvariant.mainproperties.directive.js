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
                controller: function ($scope, productResource, warehouseResource, warehouseDisplayBuilder, catalogInventoryDisplayBuilder) {

                    // Get the default warehouse for the ensureCatalogInventory() function below
                    $scope.defaultWarehouse = {};
                    $scope.defaultWarehouseCatalog = {};

                    $scope.manufacturers = [];
                    $scope.showSuggest = false;

                    // grab the manufacturer text box to apply the filters
                    var input = angular.element( document.querySelector( '#manufacturer' ) );

                    $scope.populate = function(txt) {
                        $scope.productVariant.manufacturer = txt;
                    }


                    function init() {

                        // get the list of existing manufacturers to make it easier to enter
                        productResource.getManufacturers().then(function(data) {
                            $scope.manufacturers = data;
                        });

                        var promiseWarehouse = warehouseResource.getDefaultWarehouse();
                        promiseWarehouse.then(function (warehouse) {
                            $scope.defaultWarehouse = warehouseDisplayBuilder.transform(warehouse);
                            $scope.defaultWarehouseCatalog = _.find($scope.defaultWarehouse.warehouseCatalogs, function (dwc) { return dwc.isDefault; });
                            // set defaults in case of a createproduct
                            if($scope.context === 'createproduct') {
                                $scope.productVariant.shippable = $scope.settings.globalShippable;
                                $scope.productVariant.taxable = $scope.settings.globalTaxable;
                                $scope.productVariant.trackInventory = $scope.settings.globalTrackInventory;
                                if($scope.productVariant.shippable || $scope.productVariant.trackInventory)
                                {
                                    $scope.productVariant.ensureCatalogInventory($scope.defaultWarehouseCatalog.key);
                                }
                            }
                        });

                        input.bind("keyup focusout", function (event) {
                            var code = event.which;
                            // alpha , numbers, ! and backspace

                            if ( code === 45 ||
                                (code >47 && code <58) ||
                                (code >64 && code <91) ||
                                (code >96 && code <123) || code === 33 || code == 8) {
                                $scope.$apply(function () {
                                    if ($scope.productVariant.manufacturer !== '') {
                                        $scope.showSuggest = true;
                                    } else {
                                        $scope.showSuggest = false;
                                    }
                                });
                            } else {
                                event.preventDefault();
                                $scope.showSuggest = false;
                            }
                        });
                    }

                    // Initialize the controller
                    init();
                }
            };
    }]);
