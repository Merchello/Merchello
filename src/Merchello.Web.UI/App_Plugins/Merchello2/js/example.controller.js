angular.module('merchello').controller('exampleController', [
    $scope, 'injections',
    function($scope, injections)
    {
        function init() {
            /// some stuff
        }

        function displayThis()
        {

        }

        // expose on scope
        $scope.displayThis = displayThis();

        init();
    }
]);
