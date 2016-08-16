angular.module('merchello').controller('Merchello.Backoffice.ProductFilterSpecificationListController',
    ['$scope', 'entityCollectionResource', 'merchelloTabsFactory',
    function($scope, entityCollectionResource, merchelloTabsFactory) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;

        $scope.tabs = [];

        function init() {

            $scope.tabs = merchelloTabsFactory.createProductListTabs();
            $scope.tabs.setActive('productfilterspecs');

            $scope.loaded = true;
            $scope.preValuesLoaded = true;
        }

        init();
}]);
