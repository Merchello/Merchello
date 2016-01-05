
angular.module('merchello').controller('Merchello.Backoffice.Reports.SalesOverTimeController',
    ['$scope', '$q', '$log', '$filter', 'assetsService', 'settingsResource', 'invoiceHelper', 'merchelloTabsFactory', 'salesOverTimeResource',
        function($scope, $q, $log, $filter, assetsService, settingsResource, invoiceHelper, merchelloTabsFactory, salesOverTimeResource) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
            $scope.labels = [];
            $scope.series = [];
            $scope.chartData = [];
            $scope.reportData = [];

            assetsService.loadCss('/App_Plugins/Merchello/lib/charts/angular-chart.min.css').then(function() {
                init();
            });


            function init() {
                $scope.tabs = merchelloTabsFactory.createReportsTabs();
                $scope.tabs.addTab("salesOverTime", "merchelloTree_salesOverTime", '#/merchello/merchello/salesOverTime/manage');
                $scope.tabs.setActive("salesOverTime");

                loadDefaultData();
            }

            function loadDefaultData() {
                salesOverTimeResource.getDefaultReportData().then(function(result) {
                    compileChart(result.items);
                });
            }

            function compileChart(results) {

                if (results.length > 0) {
                    _.each(results[0].totals, function(t) {
                        $scope.series.push(t.currency.symbol + ' ' + t.currency.currencyCode);
                        $scope.chartData.push([]);
                    })
                }

                _.each(results, function(item) {
                    var j = 0;
                    for(var i = 0; i < $scope.series.length; i++) {
                        $scope.chartData[j].push(item.totals[i].value.toFixed(2));
                        j++;
                    }

                    $scope.labels.push(item.getDateLabel());
                });


                $scope.preValuesLoaded = true;
                $scope.loaded = true;
            }

        }]);
