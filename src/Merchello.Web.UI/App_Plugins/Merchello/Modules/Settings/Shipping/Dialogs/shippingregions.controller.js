(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingRegionsController
     * @function
     * 
     * @description
     * The controller for the adjusting shipping rates for specific regions on the Shipping page
     */
    controllers.ShippingRegionsController = function ($scope) {

        $scope.allProvinces = false;
        $scope.toggleAllProvinces = function () {
            _.each($scope.dialogData.method.shipMethod.provinces, function (province) { province.allowShipping = $scope.allProvinces; });
        };
    };

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingRegionsController", ['$scope', merchello.Controllers.ShippingRegionsController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
