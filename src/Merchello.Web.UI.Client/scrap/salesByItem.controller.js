
angular.module('merchello').controller('Merchello.Backoffice.Reports.SalesByItemController',
    ['$scope', '$q', '$log', 'assetsService', 'settingsResource', 'invoiceHelper', 'merchelloTabsFactory',
        function($scope, $q, $log, assetsService, settingsResource, invoiceHelper, merchelloTabsFactory) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];

            $scope.itemTotalLabels = [];
            $scope.itemTotalData = [];

            $scope.historyLables = [];
            $scope.historyData = [];
            $scope.variantNames = [];


            assetsService.loadCss('/App_Plugins/Merchello/lib/charts/angular-chart.min.css').then(function() {
                init();
            });

            function init() {
                $scope.tabs = merchelloTabsFactory.createReportsTabs();
                $scope.tabs.setActive("salesByItem");
                compileReports();
            }

            function compileReports() {

                $scope.itemTotalLabels = ["Download Sales", "In-Store Sales", "Mail-Order Sales"];
                $scope.itemTotalData = [300, 500, 100];

                $scope.historyLabels = ["January", "February", "March", "April", "May", "June", "July"];
                $scope.variantNames = ['Download Sales', 'In-Store Sales', 'Mail-Order Sales'];
                $scope.historyData = [
                    [65, 59, 80, 81, 56, 55, 40],
                    [28, 48, 40, 19, 86, 27, 90],
                    [20, 38, 59, 72, 61, 12, 43]
                ];

                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            }

        }]);
