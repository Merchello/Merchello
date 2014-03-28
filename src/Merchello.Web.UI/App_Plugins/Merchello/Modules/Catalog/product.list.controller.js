(function (controllers, undefined) {
    
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Product.ListController
     * @function
     * 
     * @description
     * The controller for the product editor
     */
    controllers.ProductListController = function($scope, $routeParams, $location, assetsService, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

        $scope.filtertext = "";
        $scope.products = [];
        $scope.filteredproducts = [];
        $scope.watchCount = 0;
        $scope.sortProperty = "name";
        $scope.sortOrder = "asc";
        $scope.limitAmount = 10;
        $scope.currentPage = 0;

        assetsService.loadCss("/App_Plugins/Merchello/Common/Css/merchello.css");

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        $scope.loadProducts = function() {

            var promise = merchelloProductService.getAllProducts();

            promise.then(function(products) {

                $scope.products = _.map(products, function(productFromServer) {
                    return new merchello.Models.Product(productFromServer, true);
                });

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function(reason) {

                notificationsService.success("Products Load Failed:", reason.message);

            });

        };

        $scope.loadProducts();


        //--------------------------------------------------------------------------------------
        // Events methods
        //--------------------------------------------------------------------------------------

        $scope.limitChanged = function (newVal) {
            $scope.limitAmount = newVal;
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

        $scope.getFilteredProducts = function(filter) {
            notificationsService.info("Filtering...", "");

            if (merchello.Helpers.Strings.isNullOrEmpty(filter)) {
                $scope.loadProducts();
                notificationsService.success("Filtered Products Loaded", "");
            } else {
                var promise = merchelloProductService.filterProducts(filter);

                promise.then(function(products) {

                    $scope.products = _.map(products, function(productFromServer) {
                        return new merchello.Models.Product(productFromServer, true);
                    });

                    notificationsService.success("Filtered Products Loaded", "");

                }, function(reason) {

                    notificationsService.success("Filtered Products Load Failed:", reason.message);

                });
            }
        };


        //--------------------------------------------------------------------------------------
        // Calculations
        //--------------------------------------------------------------------------------------

        $scope.numberOfPages = function () {
            return Math.ceil($scope.products.length / $scope.limitAmount);
        };


    };

    angular.module("umbraco").controller("Merchello.Dashboards.Product.ListController", ['$scope', '$routeParams', '$location', 'assetsService', 'notificationsService', 'angularHelper', 'serverValidationManager', 'merchelloProductService', merchello.Controllers.ProductListController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

