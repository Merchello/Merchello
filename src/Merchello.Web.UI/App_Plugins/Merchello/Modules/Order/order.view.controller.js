(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Editors.Order.ViewController
     * @function
     * 
     * @description
     * The controller for the order view page
     */
    controllers.OrderViewController = function($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloProductService) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;
        $scope.order = { name: "This is a placeholder!" };
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


    angular.module("umbraco").controller("Merchello.Editors.Order.ViewController", merchello.Controllers.OrderViewController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));

