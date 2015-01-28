
angular.module('merchello').controller('Merchello.Directives.ProductVariantShippingDirectiveController',
    ['$scope', '$q', 'notificationsService', 'dialogService', 'warehouseResource', 'warehouseDisplayBuilder', 'catalogInventoryDisplayBuilder',
        function($scope, $q, notificationsService, dialogService, warehouseResource, warehouseDisplayBuilder, catalogInventoryDisplayBuilder) {

            $scope.warehouses = [];
            $scope.defaultWarehouse = {};

            // exposed methods
            $scope.getUnits = getUnits;
            $scope.mapToCatalog = mapToCatalog;

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
                var deferred = $q.defer();
                var promiseWarehouse = warehouseResource.getDefaultWarehouse();
                promiseWarehouse.then(function (warehouse) {
                    $scope.defaultWarehouse = warehouseDisplayBuilder.transform(warehouse);
                    $scope.warehouses.push($scope.defaultWarehouse);
                    deferred.resolve();
                }, function (reason) {
                    notificationsService.error("Default Warehouse Load Failed", reason.message);
                    deferred.reject(reason);
                });
                // TODO: load other warehouses when implemented
                return deferred.promise;
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
