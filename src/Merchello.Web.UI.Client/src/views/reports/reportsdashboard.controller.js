angular.module('merchello').controller('Merchello.Backoffice.MerchelloReportsDashboardController',
    ['$scope', '$element', '$filter', 'assetsService', 'dialogService', 'eventsService', 'settingsResource', 'merchelloTabsFactory',
        function($scope, $element, $filter, assetsService, dialogService, eventsService, settingsResource, merchelloTabsFactory) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];

            function init() {
                $scope.tabs = merchelloTabsFactory.createReportsTabs();
                $scope.tabs.setActive("reportsdashboard");
               // loadSettings();
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            }

            init();

        }]);
