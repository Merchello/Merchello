(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Order.EditController
     * @function
     * 
     * @description
     * The controller for the customers list page
     */
    controllers.OrderEditController = function ($scope, $routeParams, $location, notificationsService) {

        if ($routeParams.create) {
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.order = {};
            $(".content-column-body").css('background-image', 'none');
        }
        else {
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.order = {};
            $(".content-column-body").css('background-image', 'none');

            //we are editing so get the product from the server
            //var promise = merchelloProductService.getByKey($routeParams.id);

            //promise.then(function (product) {

            //    $scope.product = product;
            //    $scope.loaded = true;
            //    $scope.preValuesLoaded = true;
            //    $(".content-column-body").css('background-image', 'none');

            //}, function (reason) {

            //    alert('Failed: ' + reason.message);

            //});
        }

        $scope.save = function () {

            notificationsService.info("Saving...", "");

            //we are editing so get the product from the server
            //var promise = merchelloProductService.save($scope.product);

            //promise.then(function (product) {

            //    notificationsService.success("Order Saved", "H5YR!");

            //}, function (reason) {

            //    notificationsService.error("Order Save Failed", reason.message);

            //});
        };

    }


    angular.module("umbraco").controller("Merchello.Editors.Order.EditController", ['$scope', '$routeParams', '$location', 'notificationsService', merchello.Controllers.OrderEditController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
