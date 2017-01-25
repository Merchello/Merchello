    /**
     * @ngdoc controller
     * @name Merchello.Customer.Dialogs.CustomerInfoEditController
     * @function
     *
     * @description
     * The controller for editing customer information
     */
    angular.module('merchello').controller('Merchello.Customer.Dialogs.CustomerInfoEditController',
        ['$scope',
        function($scope) {

            $scope.wasFormSubmitted = false;

            // exposed methods
            $scope.save = save;

            /**
             * @ngdoc method
             * @name submitIfValid
             * @function
             *
             * @description
             * Submit form if valid.
             */
            function save() {
                $scope.wasFormSubmitted = true;
                if ($scope.editInfoForm.email.$valid) {
                    $scope.submit($scope.dialogData);
                }
            }

        }]);
