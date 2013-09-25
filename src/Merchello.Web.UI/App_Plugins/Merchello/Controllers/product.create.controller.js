/**
 * @ngdoc controller
 * @name Merchello.Editors.Product.CreateController
 * @function
 * 
 * @description
 * The controller for the product creation dialog
 */
function ProductCreateController($scope, $routeParams, $location, merchelloProductService) {

    $scope.productname = "";
    $scope.sku = "";
    $scope.price = 0.0;

    $scope.create = function () {

        var promise = merchelloProductService.create($scope.productname, $scope.sku, $scope.price);

        promise.then(function (product) {
            alert('Product Key Created: ' + product.key);
            $location.path('merchello/merchello/edit/' + product.key);
        }, function (reason) {
            alert('Failed: ' + reason.message);
        });
    };
}

angular.module('umbraco').controller("Merchello.Editors.Product.CreateController", ProductCreateController);