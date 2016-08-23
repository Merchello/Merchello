angular.module('merchello').controller('Merchello.Backoffice.ProductFilterSpecificationListController',
    ['$scope', 'entityCollectionResource', 'merchelloTabsFactory',
    function($scope, entityCollectionResource, merchelloTabsFactory) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;

        $scope.entityType = 'Product';

        $scope.tabs = [];

        $scope.add = function(collection) {
            $scope.preValuesLoaded = false;
            entityCollectionResource.addEntityCollection(collection).then(function(result) {
                $scope.preValuesLoaded = true;
            });
        }

        $scope.edit = function(collection) {
            $scope.preValuesLoaded = false;
            entityCollectionResource.saveSpecifiedFilterCollection(collection).then(function(result) {
                $scope.preValuesLoaded = true;
            });
        }

        function init() {

            $scope.tabs = merchelloTabsFactory.createProductListTabs();
            $scope.tabs.setActive('productfilterspecs');

            $scope.loaded = true;
            $scope.preValuesLoaded = true;
        }

        init();
}]);
