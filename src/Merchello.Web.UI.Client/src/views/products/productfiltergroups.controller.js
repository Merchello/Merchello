angular.module('merchello').controller('Merchello.Backoffice.ProductFilterGroupsController',
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
            entityCollectionResource.putEntityFilterGroup(collection).then(function(result) {
                $scope.preValuesLoaded = true;
            });
        }

        function init() {

            $scope.tabs = merchelloTabsFactory.createProductListTabs();
            $scope.tabs.setActive('filtergroups');

            $scope.loaded = true;
            $scope.preValuesLoaded = true;
        }

        init();
}]);
