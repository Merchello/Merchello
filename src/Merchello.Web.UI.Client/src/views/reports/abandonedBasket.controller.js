angular.module('merchello').controller('Merchello.Backoffice.Reports.AbandonedBasketController',
    ['$scope', 'merchelloTabsFactory',
    function($scope, merchelloTabsFactory) {

        $scope.loaded = false;
        //$scope.tabs = [];

        var graphLoaded = false;
        var basketsLoaded = false;

        function init() {
            //$scope.tabs = merchelloTabsFactory.createReportsTabs();
            //$scope.tabs.setActive('abandonedBasket');
            $scope.loaded = true;

        }

        $scope.setGraphLoaded = function(value) {
            graphLoaded = value;
            setPreValuesLoaded();
        }

        $scope.setBasketsLoaded = function(value) {
            basketsLoaded = value;
            setPreValuesLoaded();
        }

        function setPreValuesLoaded() {
            $scope.preValuesLoaded = graphLoaded && basketsLoaded;
        }

        init();

}]);
