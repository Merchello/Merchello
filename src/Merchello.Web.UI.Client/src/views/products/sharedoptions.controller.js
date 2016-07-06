angular.module('merchello').controller('Merchello.Backoffice.SharedProductOptionsController',
    ['$scope','$log', '$q', 'merchelloTabsFactory',
    function($scope, $log, $q, merchelloTabsFactory) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;

        $scope.tabs = [];

        function init() {

            $scope.tabs = merchelloTabsFactory.createProductListTabs();
            $scope.tabs.setActive('sharedoptions');

            $scope.loaded = true;
            $scope.preValuesLoaded = true;

        }


        init();
    }]);
