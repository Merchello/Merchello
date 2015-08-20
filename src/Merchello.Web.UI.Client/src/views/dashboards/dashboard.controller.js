angular.module('merchello').controller('Merchello.Backoffice.MerchelloDashboardController',
    ['$scope', 'settingsResource',
    function($scope, settingsResource) {

        $scope.loaded = false;
        $scope.merchelloVersion = '';

        function init() {
            var promise = settingsResource.getMerchelloVersion();
            promise.then(function(version) {
              $scope.merchelloVersion = version.replace(/['"]+/g, '');
                $scope.loaded = true;
            });
        }

        // initialize the controller
        init();
    }]);
