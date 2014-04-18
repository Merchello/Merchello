(function (controllers, undefined) {
    
    /**
     * @ngdoc controller
     * @name Merchello.Editors.Customer.ViewController
     * @function
     * 
     * @description
     * The controller for the Customer view page
     */
    controllers.CustomerViewController = function($scope) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;
        $scope.customer = { name: "This is a placeholder!" };
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

    };


    angular.module("umbraco").controller("Merchello.Editors.Customer.ViewController", ['$scope', merchello.Controllers.CustomerViewController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
