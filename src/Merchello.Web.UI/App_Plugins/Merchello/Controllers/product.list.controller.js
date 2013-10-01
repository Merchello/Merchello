/**
 * @ngdoc controller
 * @name Merchello.Dashboards.Product.ListController
 * @function
 * 
 * @description
 * The controller for the product editor
 */
function ProductListController($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

    $scope.filtertext = "";
    $scope.products = [];
    $scope.filteredproducts = [];
    $scope.watchCount = 0;

    $scope.$watch('filtertext', function (newfilter, oldfilter) {
        $scope.watchCount = $scope.watchCount + 1;

        //var newFilteredProducts = jQuery.grep($scope.products, function (p, i) {
        //    return p.name.test($scope.filtertext);
        //});

        //$scope.filteredproducts = newFilteredProducts;
    }, true);

    $scope.loadProducts = function () {

        //we are editing so get the product from the server
        var promise = merchelloProductService.getAllProducts();

        promise.then(function (products) {

            $scope.products = products;
            $scope.filteredproducts = products;
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $(".content-column-body").css('background-image', 'none');

        }, function (reason) {

            alert('Failed: ' + reason.message);

        });

    };

    $scope.loadProducts();
}

angular.module("umbraco").controller("Merchello.Dashboards.Product.ListController", ProductListController);