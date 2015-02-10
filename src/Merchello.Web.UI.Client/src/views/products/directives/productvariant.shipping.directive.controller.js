
angular.module('merchello').controller('Merchello.Directives.ProductVariantShippingDirectiveController',
    ['$scope', 'notificationsService', 'dialogService', 'warehouseResource', 'warehouseDisplayBuilder', 'catalogInventoryDisplayBuilder',
        function($scope, notificationsService, dialogService, warehouseResource, warehouseDisplayBuilder, catalogInventoryDisplayBuilder) {

            $scope.warehouses = [];
            $scope.defaultWarehouse = {};
            $scope.defaultWarehouseCatalog = {};

            // exposed methods
            $scope.getUnits = getUnits;
            $scope.mapToCatalog = mapToCatalog;
            $scope.toggleCatalog = toggleCatalog;

            function init() {
                loadAllWarehouses();
            }

            /**
             * @ngdoc method
             * @name loadAllWarehouses
             * @function
             *
             * @description
             * Loads in default warehouse and all other warehouses from server into the scope.  Called in init().
             */
            function loadAllWarehouses() {
                var promiseWarehouse = warehouseResource.getDefaultWarehouse();
                promiseWarehouse.then(function (warehouse) {
                    $scope.defaultWarehouse = warehouseDisplayBuilder.transform(warehouse);
                    $scope.warehouses.push($scope.defaultWarehouse);
                    $scope.defaultWarehouseCatalog = _.find($scope.defaultWarehouse.warehouseCatalogs, function (dc) { return dc.isDefault; });
                }, function (reason) {
                    notificationsService.error("Default Warehouse Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name mapToCatalog
             * @function
             *
             * @description
             * Maps a catalog to a product variant catalog inventory
             */
            function mapToCatalog(catalog) {
                var mapped = _.find($scope.productVariant.catalogInventories, function(ci) { return ci.catalogKey === catalog.key;});
                if(mapped === undefined) {
                    var catalogInventory = catalogInventoryDisplayBuilder.createDefault();
                    catalogInventory.productVariantKey = $scope.productVariant.key;
                    catalogInventory.catalogKey = catalog.key;
                    catalogInventory.active = false;
                    $scope.productVariant.catalogInventories.push(catalogInventory);
                    mapped = catalogInventory;
                }
                return mapped;
            }

            function toggleCatalog() {
                $scope.productVariant.ensureCatalogInventory($scope.defaultWarehouseCatalog.key);
            }

            function getUnits(settings, type) {
                if(settings.unitSystem === 'Imperial') {
                    return type === 'weight' ? '(pounds)' : '(inches)';
                } else {
                    return type === 'weight' ? '(kg)' : '(cm)';
                }
            }

            // Initializes the controller
            init();
}]);
