angular.module('merchello').controller('Merchello.Backoffice.ProductContentTypeListController',
    ['$scope', 'merchelloTabsFactory',
    function($scope, merchelloTabsFactory) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;
        $scope.tabs = {};

        function init() {
            $scope.tabs = merchelloTabsFactory.createProductListTabs();
            $scope.tabs.setActive('productContentTypeList');
        }

        // Initializes the controller
        init();
    }]);
