angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloCheckoutWorkflowStagePickerController', [
    '$scope', 'backOfficeCheckoutResource',
    function($scope, backOfficeCheckoutResource) {
        
        $scope.loaded = false;
        $scope.stages = [];
        
        function init() {

            if (_.isString($scope.model.value)) {
                backOfficeCheckoutResource.getCheckoutStages().then(function(stages) {
                    $scope.stages = stages;
                    $scope.loaded = true;
                    if ($scope.model.value === '') {
                        $scope.model.value = $scope.stages[0];
                    }
                });
            } else {
                $scope.loaded = true;
            }
        }
        
        init();
    }]);
