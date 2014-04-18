(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingMethodController
     * @function
     * 
     * @description
     * The controller for the adding / editing shipping methods on the Shipping page
     */
    controllers.ShippingMethodController = function($scope) {

        $scope.isAddNewTier = false;
        $scope.newTier = {};

        $scope.removeRateTier = function(tier) {
            $scope.dialogData.method.rateTable.removeRow(tier);
        };

        $scope.addRateTier = function() {
            $scope.dialogData.method.rateTable.addRow($scope.newTier);
            $scope.isAddNewTier = false;
        };

        $scope.insertRateTier = function() {
            $scope.isAddNewTier = true;
            $scope.newTier = merchello.Models.ShippingRateTier();
        };

        $scope.allProvinces = false;
        $scope.toggleAllProvinces = function() {
            _.each($scope.dialogData.method.shipMethod.provinces, function (province) { province.allowShipping = $scope.allProvinces; });
        };
    };

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.Shipping.Dialogs.ShippingMethodController", ['$scope', merchello.Controllers.ShippingMethodController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
