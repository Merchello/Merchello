(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Common.Dialogs.DebugDialog
     * @function
     * 
     * @description
     * The controller for adding a country
     */
    controllers.EditAddressController = function ($scope) {

        $scope.init = function() {
            console.info($scope.dialogData);
            $scope.setVariables();
        };

        $scope.setVariables = function() {
            $scope.address = $scope.dialogData.address;
        };

        $scope.save = function() {
            $scope.dialogData.address = $scope.address;
            $scope.submit();
        };

        $scope.init();
    };


    angular.module("umbraco").controller("Merchello.Common.Dialogs.EditAddressController", ['$scope', merchello.Controllers.EditAddressController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
