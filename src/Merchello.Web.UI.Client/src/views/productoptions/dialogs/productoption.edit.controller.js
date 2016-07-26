angular.module('merchello').controller('Merchello.ProductOption.Dialogs.ProductOptionEditController',
    ['$scope', 'eventsService',
    function($scope, eventsService) {

        $scope.visiblePanel = $scope.dialogData.sharedOptionEditor ? 'sharedoption' : 'newoption';

        var newName = 'merchNewProductOptionSave';
        var sharedEvent = 'merchSharedProductOptionSave';


        $scope.validate = function() {
            var validation = { valid: false };

            if ($scope.visiblePanel === 'newoption') {
                eventsService.emit(newName, validation);
            } else if ($scope.visiblePanel === 'sharedoption') {
                eventsService.emit(sharedEvent, validation);
            }

            if (validation.valid) {
                $scope.submit($scope.dialogData);
            }
        }

}]);