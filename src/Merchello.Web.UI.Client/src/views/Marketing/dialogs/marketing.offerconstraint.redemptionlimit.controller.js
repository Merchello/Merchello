/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferConstraintRedemptionLimitController
 * @function
 *
 * @description
 * The controller to configure the maximum number of redemptions allowed.
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferConstraintRedemptionLimitController',
    ['$scope',
        function($scope) {

            $scope.loaded = false;
            $scope.maximum = 0;

            // exposed
            $scope.save = save;

            function init() {
                if ($scope.dialogData.component.isConfigured()) {
                    loadExistingConfigurations();
                    $scope.loaded = true;
                } else {
                    $scope.loaded = true;
                }
            }

            function loadExistingConfigurations() {
                var maximum = $scope.dialogData.getValue('maximum')
                $scope.maximum = maximum === '' ? 0 : maximum * 1;
            }

            function save() {
                $scope.dialogData.setValue('maximum', $scope.maximum);
                $scope.submit($scope.dialogData);
            }

            // Initialize the controller
            init();
        }]);
