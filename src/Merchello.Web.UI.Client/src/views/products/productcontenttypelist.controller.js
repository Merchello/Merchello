angular.module('merchello').controller('Merchello.Backoffice.ProductContentTypeListController',
    ['$scope', 'merchelloTabsFactory',
    function($scope, merchelloTabsFactory) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;
        $scope.tabs = {};

        function init() {
            $scope.tabs = merchelloTabsFactory.createProductListTabs();
            $scope.tabs.setActive('contentTypeList');
        }


        // Initializes the controller
        init();
    }]);
