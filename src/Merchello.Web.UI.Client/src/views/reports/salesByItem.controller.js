
angular.module('merchello').controller('Merchello.Backoffice.Reports.SalesByItemController',
    ['$scope', '$q', '$log','settingsResource', 'invoiceHelper', 'merchelloTabsFactory',
        function($scope, $q, $log, settingsResource, invoiceHelper, merchelloTabsFactory) {

                $scope.loaded = false;
                $scope.preValuesLoaded = false;
                $scope.tabs = [];

                function init() {
                        $scope.tabs = merchelloTabsFactory.createReportsTabs();
                        $scope.tabs.setActive("salesByItem");
                        $scope.loaded = true;
                        $scope.preValuesLoaded = true;
                }

                init();
        }]);
