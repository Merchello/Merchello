angular.module('merchello').controller('Merchello.Backoffice.SharedProductOptionsController',
    ['$scope','$log', '$q', 'merchelloTabsFactory', 'localizationService', 'productOptionResource', 'dialogDataFactory', 'dialogService',
        'merchelloListViewHelper',
    function($scope, $log, $q, merchelloTabsFactory, localizationService, productOptionResource, dialogDataFactory, dialogService, merchelloListViewHelper) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

        $scope.tabs = [];

        // In the initial release of this feature we are only going to allow sharedOnly params
        // to be managed here.  We may open this up at a later date depending on feedback.
        $scope.sharedOnly = true;

        // list view
        //$scope.entityType = 'ProductOption';
        //$scope.load = load;
        //$scope.getColumnValue = getColumnValue;




        function init() {

            $scope.tabs = merchelloTabsFactory.createProductListTabs();
            $scope.tabs.setActive('sharedoptions');


        }

        /*
        function load(query) {
            query.addSharedOptionOnlyParam($scope.sharedOnly);
            return productOptionResource.searchOptions(query);
        }

        function getColumnValue(result, col) {

            switch(col.name) {
                case 'name':
                    return '<a href="#">' + result.name + '</a>';
                case 'shared':
                    return result.shared ? yes : no;

                case 'sharedCount':
                    return result.sharedCount.toString();
                case 'uiOption':
                    return !result.uiElement ? '-' : result.uiElement;
                case 'choices':
                    return result.choices.length + ' ' + values;
            }
        }
        */


        init();
    }]);
