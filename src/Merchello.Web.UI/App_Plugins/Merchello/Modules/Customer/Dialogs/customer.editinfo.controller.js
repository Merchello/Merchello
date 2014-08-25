(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Customer.CustomerEditInfoController
     * @function
     * 
     * @description
     * The controller for adding or editing a customer's info.
     */
    controllers.CustomerEditInfoController = function ($scope) {

        /**
         * @ngdoc method
         * @name init
         * @function
         * 
         * @description
         * Inits the scope.
         */
        $scope.init = function () {
            $scope.setVariables();
        };

        /**
         * @ngdoc method
         * @name setVariables
         * @function
         * 
         * @description
         * Set the $scope variables.
         */
        $scope.setVariables = function () {
            $scope.wasFormSubmitted = false;
        }

        /**
         * @ngdoc method
         * @name submitIfValid
         * @function
         * 
         * @description
         * Submit form if valid.
         */
        $scope.submitIfValid = function() {
            $scope.wasFormSubmitted = true;
            if ($scope.editInfoForm.email.$valid) {
                $scope.submit($scope.dialogData);
            }
        };

        $scope.init();

    };

    angular.module("umbraco").controller("Merchello.Dashboards.Customer.Dialogs.CustomerEditInfoController", ['$scope', merchello.Controllers.CustomerEditInfoController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
