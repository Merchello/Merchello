(function (controllers, undefined) {
        /**
     * @ngdoc controller
     * @name Merchello.Editors.Product.CreateController
     * @function
     * 
     * @description
     * The controller for the product creation dialog
     */
    controllers.ProductCreateController = function($scope, $routeParams, $location, assetsService, merchelloProductService) {

        $scope.productname = "";
        $scope.sku = "";
        $scope.price = 0.0;

        assetsService.loadCss("/App_Plugins/Merchello/Common/Css/merchello.css");

        $scope.create = function () {

            var promise = merchelloProductService.create($scope.sku, $scope.productname, $scope.price);

            promise.then(function (product) {
                $location.path('merchello/merchello/edit/' + product.key);
                $scope.nav.hideNavigation();
            }, function (reason) {
                alert('Failed: ' + reason.message);
            });
        };
    }

    angular.module('umbraco').controller("Merchello.Editors.Product.CreateController", merchello.Controllers.ProductCreateController);

}(window.merchello.Controllers = window.merchello.Controllers || {}));