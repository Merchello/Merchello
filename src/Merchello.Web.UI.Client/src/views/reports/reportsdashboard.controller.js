angular.module('merchello').controller('Merchello.Backoffice.MerchelloReportsDashboardController',
    ['$scope', '$element', 'assetsService', 'merchelloTabsFactory',
        function($scope, $element, assetsService, merchelloTabsFactory) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];

            assetsService.loadCss('/App_Plugins/Merchello/lib/charts/angular-chart.min.css').then(function() {
                init();
            });

            $scope.onClick = function (points, evt) {
                console.log(points, evt);
            };

            function init() {
                $scope.tabs = merchelloTabsFactory.createReportsTabs();
                $scope.tabs.setActive("reportsdashboard");
                loadAnnual();
            }

            function loadAnnual() {
                $scope.annualLabels = ["January", "February", "March", "April", "May", "June", "July"];
                $scope.series = ['Series A', 'Series B'];
                $scope.data = [
                    [65, 59, 80, 81, 56, 55, 40],
                    [28, 48, 40, 19, 86, 27, 90]
                ];

                $scope.pielabels = ["Download Sales", "In-Store Sales", "Mail-Order Sales"];
                $scope.piedata = [300, 500, 100];

                $scope.preValuesLoaded = true;
                $scope.loaded = true;
            }

        }]);
