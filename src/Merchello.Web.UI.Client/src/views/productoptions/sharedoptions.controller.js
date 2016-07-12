angular.module('merchello').controller('Merchello.Backoffice.SharedProductOptionsController',
    ['$scope','$log', '$q', 'merchelloTabsFactory', 'localizationService', 'productOptionResource', 'dialogDataFactory', 'dialogService',
    function($scope, $log, $q, merchelloTabsFactory, localizationService, productOptionResource, dialogDataFactory, dialogService) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.entityType = 'ProductOption';
        $scope.tabs = [];

        // In the initial release of this feature we are only going to allow sharedOnly params
        // to be managed here.  We may open this up at a later date depending on feedback.
        $scope.sharedOnly = false;

        // list view
        $scope.load = load;
        $scope.getColumnValue = getColumnValue;

        var yes = '';
        var no = '';
        var values = '';

        function init() {

            $scope.tabs = merchelloTabsFactory.createProductListTabs();
            $scope.tabs.setActive('sharedoptions');

            $q.all([
                localizationService.localize('general_yes'),
                localizationService.localize('general_no'),
                localizationService.localize('merchelloTableCaptions_optionValues')
            ]).then(function(data) {
                yes = data[0];
                no = data[1];
                values = data[2];
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            });

        }

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


        init();
    }]);
