angular.module('merchello').controller('Merchello.Backoffice.MerchelloReportsDashboardController',
    ['$scope', '$element', 'assetsService',
        function($scope, $element, assetsService) {

            $scope.loaded = false;

            assetsService.loadCss('/App_Plugins/Merchello/lib/charts/angular-chart.min.css').then(function() {
                $scope.annualLabels = ["January", "February", "March", "April", "May", "June", "July"];
                $scope.series = ['Series A', 'Series B'];
                $scope.data = [
                    [65, 59, 80, 81, 56, 55, 40],
                    [28, 48, 40, 19, 86, 27, 90]
                ];

                $scope.pielabels = ["Download Sales", "In-Store Sales", "Mail-Order Sales"];
                $scope.piedata = [300, 500, 100];

                $scope.onClick = function (points, evt) {
                    console.log(points, evt);
                };
                $scope.loaded = true;
            });

            function init() {
                loadAnnual();

            }

            function loadAnnual() {


            }
        }]);
