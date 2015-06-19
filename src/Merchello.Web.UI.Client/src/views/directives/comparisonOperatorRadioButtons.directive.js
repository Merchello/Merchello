angular.module('merchello.directives').directive('comparisonOperatorRadioButtons', function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            operator: '='
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/comparisonOperatorRadioButtons.tpl.html',
        controller: function($scope) {

            function init() {
                if($scope.operator === undefined) {
                    $scope.operator = 'gt';
                }
            }

            init();
        }
    };
});
