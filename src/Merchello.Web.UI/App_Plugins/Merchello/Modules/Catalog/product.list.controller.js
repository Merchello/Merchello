(function (controllers, undefined) {
    
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Product.ListController
     * @function
     * 
     * @description
     * The controller for the product editor
     */
    controllers.ProductListController = function($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

        $scope.filtertext = "";
        $scope.products = [];
        $scope.filteredproducts = [];
        $scope.watchCount = 0;
        $scope.sortProperty = "name";
        $scope.sortOrder = "asc";
        $scope.limitAmount = 10;
        $scope.currentPage = 0;

        $scope.numberOfPages = function () {
            return Math.ceil($scope.products.length / $scope.limitAmount);
        }

        $scope.loadProducts = function () {

            var promise = merchelloProductService.getAllProducts();

            promise.then(function (products) {

                $scope.products = _.map(products, function (productFromServer) {
                    return new merchello.Models.Product(productFromServer);
                });

                //$scope.filteredproducts = $scope.products;
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                $(".content-column-body").css('background-image', 'none');

            }, function (reason) {

                alert('Failed: ' + reason.message);

            });

        };

        $scope.loadProducts();

        $scope.changeSortOrder = function (propertyToSort) {

            if ($scope.sortProperty == propertyToSort)
            {
                if ($scope.sortOrder == "asc")
                {
                    $scope.sortProperty = "-" + propertyToSort;
                    $scope.sortOrder = "desc";
                }
                else
                {
                    $scope.sortProperty = propertyToSort;
                    $scope.sortOrder = "asc";
                }
            }
            else
            {
                $scope.sortProperty = propertyToSort;
                $scope.sortOrder = "asc";
            }

        }

        $scope.getFilteredProducts = function (filter)
        {
            notificationsService.info("Filtering...", "");

            var promise = merchelloProductService.filterProducts(filter);

            promise.then(function (products) {

                $scope.products = _.map(products, function (productFromServer) {
                    return new merchello.Models.Product(productFromServer);
                });

                notificationsService.success("Filtered Products Loaded", "");

            }, function (reason) {

                alert('Failed: ' + reason.message);

            });
        }

    }

    angular.module("umbraco").controller("Merchello.Dashboards.Product.ListController", merchello.Controllers.ProductListController);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

