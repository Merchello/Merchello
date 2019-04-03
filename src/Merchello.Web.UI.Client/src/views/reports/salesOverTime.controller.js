
angular.module('merchello').controller('Merchello.Backoffice.Reports.SalesOverTimeController',
    ['$scope', '$q', '$log', '$filter', 'assetsService', 'dialogService', 'queryDisplayBuilder',
        'settingsResource', 'invoiceHelper', 'merchelloTabsFactory', 'salesOverTimeResource',
        function($scope, $q, $log, $filter, assetsService, dialogService, queryDisplayBuilder,
                 settingsResource, invoiceHelper, merchelloTabsFactory, salesOverTimeResource) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
            $scope.labels = [];
            $scope.series = [];
            $scope.chartData = [];
            $scope.reportData = [];
            $scope.startDate = '';
            $scope.endDate = '';
            $scope.settings = {};
            $scope.dateBtnText = '';

            $scope.getColumnValue = getColumnValue;
            $scope.getColumnTotal = getColumnTotal;
            $scope.openDateRangeDialog = openDateRangeDialog;
            $scope.clearDates = clearDates;
            $scope.reverse = reverse;


            assetsService.loadCss('/App_Plugins/Merchello/lib/charts/angular-chart.min.css').then(function() {
                init();
            });

            function init() {
                $scope.tabs = merchelloTabsFactory.createReportsTabs();
                $scope.tabs.setActive("salesOverTime");

                loadSettings();
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description - Load the Merchello settings.
             */
            function loadSettings() {
                settingsResource.getAllCombined().then(function(combined) {
                    $scope.settings = combined.settings;
                    loadDefaultData();
                });
            };

            function loadDefaultData() {
                salesOverTimeResource.getDefaultReportData().then(function(result) {
                    compileChart(result);
                });
            }

            function loadCustomData() {

                var query = queryDisplayBuilder.createDefault();
                query.addInvoiceDateParam($scope.startDate, 'start');
                query.addInvoiceDateParam($scope.endDate, 'end');

                salesOverTimeResource.getCustomReportData(query).then(function(result) {
                   compileChart(result);
                });
            }

            function compileChart(result) {

                $scope.labels = [];
                $scope.series = [];
                $scope.chartData = [];
                $scope.reportData = [];

                $scope.reportData = result.items;

                if ($scope.reportData.length > 0) {
                    $scope.startDate = $filter('date')($scope.reportData[0].startDate, $scope.settings.dateFormat);
                    $scope.endDate = $filter('date')($scope.reportData[$scope.reportData.length - 1].endDate, $scope.settings.dateFormat);
                }

                setDateButtonText();

                if ($scope.reportData.length > 0) {
                    _.each($scope.reportData[0].totals, function(t) {
                        $scope.series.push(t.currency.symbol + ' ' + t.currency.currencyCode);
                        $scope.chartData.push([]);
                    })
                }

                _.each($scope.reportData, function(item) {
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

            function reverse(data) {
                return data.slice().reverse();
            }

            function getColumnValue(data, series) {

                var total = _.find(data.totals, function(t) {
                   return series.indexOf(t.currency.currencyCode) > -1;
                });

                if (total !== null && total !== undefined) {
                    return $filter('currency')(total.value, total.currency.symbol);
                } else {
                    return '-';
                }
            }

            function openDateRangeDialog() {
                var dialogData = {
                    startDate: $scope.startDate,
                    endDate: $scope.endDate
                };

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/daterange.selection.html',
                    show: true,
                    callback: processDateRange,
                    dialogData: dialogData
                });
            }

            function getColumnTotal(series) {

                if ($scope.reportData.length > 0) {
                    var total = 0;
                    var symbol = '';
                    _.each($scope.reportData, function(data) {
                        var itemTotal = _.find(data.totals, function(t) {
                            return series.indexOf(t.currency.currencyCode) > -1;
                        });

                        total += itemTotal.value;
                        if (symbol === '') {
                            symbol = itemTotal.currency.symbol;
                        }
                    });

                    return $filter('currency')(total, symbol);

                } else {
                    return '-';
                }
            }

            function setDateButtonText() {
                $scope.dateBtnText = $scope.startDate + ' - ' + $scope.endDate;
            }

            function processDateRange(dialogData) {
                $scope.startDate = dialogData.startDate;
                $scope.endDate = dialogData.endDate;
                loadCustomData();
            }

            function clearDates() {
                $scope.loaded = false;
                $scope.preValuesLoaded = false;
                loadDefaultData();
            }

        }]);
