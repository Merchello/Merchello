    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ReportsListController
     * @function
     *
     * @description
     * The controller for the invoice payments view
     */
    angular.module('merchello').controller('Merchello.Backoffice.ReportsListController',
    ['$scope', 'merchelloTabsFactory',
    function($scope, merchelloTabsFactory) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;
        $scope.tabs = [];

        function init() {
            $scope.tabs = merchelloTabsFactory.createReportsTabs();
            $scope.tabs.setActive('reportslist');
        }

        // initialize the controller
        init();

    }]);
