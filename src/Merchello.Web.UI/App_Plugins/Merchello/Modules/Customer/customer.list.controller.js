(function (controllers, undefined) {
   
    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Customer.ListController
     * @function
     * 
     * @description
     * The controller for the customers list page
     */
    controllers.CustomerListController = function($scope) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

    };


    angular.module("umbraco").controller("Merchello.Dashboards.Customer.ListController", ['$scope', merchello.Controllers.CustomerListController]);

}(window.merchello.Controllers = window.merchello.Controllers || {}));
