/**
 * @ngdoc controller
 * @name Merchello.Editors.Product.CreateController
 * @function
 * 
 * @description
 * The controller for the product creation dialog
 */
function ProductCreateController($scope, $routeParams, contentTypeResource, iconHelper) {

    $scope.productname = "";
    $scope.sku = "";
    $scope.price = 0.0;

    $scope.create = function () {
        
    };
}

angular.module('umbraco').controller("Merchello.Editors.Product.CreateController", ProductCreateController);