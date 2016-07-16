angular.module('merchello').controller('Merchello.Backoffice.SharedProductOptionsController',
    ['$scope','$log', '$q', 'merchelloTabsFactory', 'localizationService', 'productOptionResource', 'dialogDataFactory', 'dialogService',
        'merchelloListViewHelper',
    function($scope, $log, $q, merchelloTabsFactory, localizationService, productOptionResource, dialogDataFactory, dialogService, merchelloListViewHelper) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

        $scope.tabs = [];

        // In the initial release of this feature we are only going to allow sharedOnly params
        // to be managed here.  We may open this up at a later date depending on feedback.
        $scope.sharedOnly = false;

        function init() {
            $scope.tabs = merchelloTabsFactory.createProductListTabs();
            $scope.tabs.setActive('sharedoptions');
        }


        $scope.load = function(query) {
            query.addSharedOptionOnlyParam($scope.sharedOnly);
            return productOptionResource.searchOptions(query);
        }


        init();
    }]);
