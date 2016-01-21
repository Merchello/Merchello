angular.module('merchello').controller('Merchello.Backoffice.Reports.AbandonedBasketController',
    ['$scope', 'merchelloTabsFactory',
    function($scope, merchelloTabsFactory) {

        $scope.loaded = false;
        $scope.tabs = [];

        function init() {
            $scope.tabs = merchelloTabsFactory.createReportsTabs();
            $scope.tabs.setActive('abandonedBasket');
            $scope.loaded = true;
        }


        $scope.setPreValuesLoaded = function(value) {
            $scope.preValuesLoaded = value;
        }

        init();

}]);
