/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferConstraintFilterQuantityRulesController
 * @function
 *
 * @description
 * The controller to configure the line item quantity component constraint
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferConstraintFilterQuantityRulesController',
    ['$scope',
    function($scope) {
        $scope.loaded = false;

        $scope.operator = 'gt';
        $scope.quantity = 0;

        // exposed methods
        $scope.save = save;

        function init() {
            if ($scope.dialogData.component.isConfigured()) {
                loadExistingConfigurations()
            } else {
                $scope.loaded = true;
            }

        }

        function loadExistingConfigurations() {
            var operator = $scope.dialogData.getValue('operator');
            var quantity = $scope.dialogData.getValue('quantity');
            $scope.operator = operator === '' ? 'gt' : operator;
            $scope.quantity = quantity === '' ? 0 : quantity * 1;
            $scope.loaded = true;
        }

        /**
         * @ngdoc method
         * @name save
         * @function
         *
         * @description
         * Saves the configuration
         */
        function save() {
            $scope.dialogData.setValue('quantity', Math.abs($scope.quantity*1));
            $scope.dialogData.setValue('operator', $scope.operator);
            $scope.submit($scope.dialogData);
        }

        // Initialize the controller
        init();
    }]);
