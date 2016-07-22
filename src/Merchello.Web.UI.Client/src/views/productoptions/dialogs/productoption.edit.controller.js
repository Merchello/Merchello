angular.module('merchello').controller('Merchello.ProductOption.Dialogs.ProductOptionEditController',
    ['$scope', 'eventsService',
    function($scope, eventsService) {

        $scope.visiblePanel = 'newoption';

        var eventName = 'merchNewProductOptionSave';



        $scope.validate = function() {
            var validation = { valid: false };

            eventsService.emit(eventName, validation);

            if (validation.valid) {
                $scope.submit($scope.dialogData);
            }
        }

}]);