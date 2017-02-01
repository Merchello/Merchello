/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferConstraintMaximumQuantityController
 * @function
 *
 * @description
 * The controller to configure the line item quantity component constraint
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferConstraintMaximumQuantityController',
    ['$scope',
    function($scope) {

    $scope.loaded = false;
    $scope.maximum = 1;

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
        $scope.maximum = maximum === '' ? 1 : maximum * 1;
    }

    function save() {
        $scope.dialogData.setValue('maximum', $scope.maximum);
        $scope.submit($scope.dialogData);
    }

    // Initialize the controller
    init();
}]);