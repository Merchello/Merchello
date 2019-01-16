angular.module('merchello.directives').directive('reportWidgeThisWeekVsLast',
    ['$q', '$log', '$filter', 'assetsService', 'localizationService', 'merchDateHelper',  'settingsResource', 'salesOverTimeResource', 'queryDisplayBuilder',
        function($q, $log, $filter, assetsService, localizationService, dateHelper, settingsResource, salesOverTimeResource, queryDisplayBuilder) {

            return {
                restrict: 'E',
                replace: true,
                scope: {
                    ready: '=?'
                },
                templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/reportwidget.thisweekvslast.tpl.html',
                link: function(scope, elm, attr) {


                    scope.loaded = false;
                    scope.busy = false;
                    scope.settings = {};
                    scope.resultData = [];
                    scope.chartData = [];
                    scope.labels = [];
                    scope.series = [];
                    scope.weekdays = [];

                    scope.getTotalsColumn = getTotalsColumn;

                    assetsService.loadCss('/App_Plugins/Merchello/lib/charts/angular-chart.min.css').then(function() {
                        init();
                    });

                    function init() {

                        var deferred = $q.defer();

                        $q.all([
                            dateHelper.getLocalizedDaysOfWeek(),
                            settingsResource.getAllCombined()
                        ]).then(function(data) {
                            scope.weekdays = data[0];
                            scope.settings = data[1];
                            loadReportData();
                        });

                    }

                    function loadReportData() {
                        //var today = dateHelper.getGmt0EquivalentDate(new Date());
                        var today = new Date();
                        var last = today;
                        var thisWeekEnd = $filter('date')(today, scope.settings.dateFormat);
                        var lastWeekEnd = $filter('date')(last.setDate(last.getDate() - 7), scope.settings.dateFormat);
                        var lastQuery = queryDisplayBuilder.createDefault();
                        var currentQuery = queryDisplayBuilder.createDefault();
                        currentQuery.addInvoiceDateParam(thisWeekEnd, 'end');
                        lastQuery.addInvoiceDateParam(lastWeekEnd, 'end');

                        var deferred = $q.defer();
                        $q.all([
                            salesOverTimeResource.getWeeklyResult(currentQuery),
                            salesOverTimeResource.getWeeklyResult(lastQuery)

                        ]).then(function(data) {
                            scope.resultData = [ data[0].items, data[1].items];
                            compileChart();
                        });

                    }

                    function compileChart() {

                        scope.labels = [];
                        scope.series = [];
                        scope.chartData = [];

                        if (scope.resultData.length > 0) {

                            _.each(scope.resultData[0], function(days) {

                                //var dt = dateHelper.getGmt0EquivalentDate(new Date(days.startDate));
                                var dt = new Date(days.startDate);
                                var dd = dt.getDay();

                                scope.labels.push(scope.weekdays[dd]);
                            });

                            // list the series
                            // we'll have 'This Week' and 'Last Week' for every currency code
                            var seriesTemplate = [];
                            _.each(scope.resultData[0][0].totals, function(t) {
                                seriesTemplate.push({label: t.currency.symbol + ' ' + t.currency.currencyCode, currencyCode: t.currency.currencyCode});
                            });

                            var dataSeriesLength = seriesTemplate.length * 2;

                            buildSeries(seriesTemplate, 'This Week');
                            buildSeries(seriesTemplate, 'Last Week');



                            var seriesIndex = 0;
                            _.each(scope.resultData, function(dataSet) {
                                for(var j = 0; j < seriesTemplate.length; j++) {
                                    addChartData(dataSet, seriesTemplate[j].currencyCode, seriesIndex);
                                    seriesIndex++;
                                }
                            });

                        }

                        scope.preValuesLoaded = true;
                        scope.loaded = true;
                    }

                    function buildSeries(template, prefix) {
                        _.each(template, function(item) {
                            scope.series.push(prefix + ': ' + item.label);
                            scope.chartData.push([]);
                        });
                    }

                    function addChartData(dataSeries, currencyCode, chartDataIndex) {

                        _.each(dataSeries, function(item) {
                           var total = _.find(item.totals, function(tot) {
                               return tot.currency.currencyCode === currencyCode;
                           });
                            scope.chartData[chartDataIndex].push(total.value);
                        });
                    }

                    function getTotalsColumn(resultIndex, valueIndex) {
                        var result = scope.resultData[resultIndex][valueIndex];

                        var ret = '';
                        _.each(result.totals, function(total) {
                            if (ret !== '') ret += '<br />';
                            ret += total.currency.currencyCode + ': ' + $filter('currency')(total.value, total.currency.symbol);
                        });

                        return ret;
                    }
                }


            };
        }]);