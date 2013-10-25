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

        $scope.loadProducts = function () {

            //we are editing so get the product from the server
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

    }

    angular.module("umbraco").controller("Merchello.Dashboards.Product.ListController", merchello.Controllers.ProductListController);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

