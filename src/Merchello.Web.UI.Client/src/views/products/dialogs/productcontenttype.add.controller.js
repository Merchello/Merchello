/**
 * @ngdoc controller
 * @name Merchello.Product.Dialogs.AddProductContentTypeController
 * @function
 *
 * @description
 * The controller for the adding product content types
 */
angular.module('merchello').controller('Merchello.Product.Dialogs.AddProductContentTypeController',
    ['$scope', 'detachedContentResource',
        function($scope, detachedContentResource) {
            $scope.loaded = true;
            $scope.wasFormSubmitted = false;
            $scope.contentType = '';

            $scope.save = function() {
                $scope.wasFormSubmitted = true;
                if ($scope.productContentTypeForm.name.$valid) {
                    console.info($scope.contentType);
                }
            }

        }]);
