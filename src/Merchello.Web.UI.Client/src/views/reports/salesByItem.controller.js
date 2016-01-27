angular.module('merchello').controller('Merchello.Backoffice.Reports.SalesByItemController',
    ['$scope', '$filter', 'assetsService', 'dialogService', 'eventsService', 'settingsResource', 'merchelloTabsFactory',
    function($scope, $filter, assetsService, dialogService, eventsService, settingsResource, merchelloTabsFactory) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.tabs = [];
        $scope.settings = {};

        $scope.startDate = '';
        $scope.endDate = '';


        function init() {
            $scope.tabs = merchelloTabsFactory.createReportsTabs();
            $scope.tabs.setActive("salesByItem");
            $scope.loaded = true;

        };

        $scope.setPreValuesLoaded = function(value) {
            $scope.preValuesLoaded = value;
        }

        init();

    }]);