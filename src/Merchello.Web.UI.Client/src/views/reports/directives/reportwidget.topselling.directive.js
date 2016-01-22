angular.module('merchello.directives').directive('reportWidgetTopSelling',
    ['$log', '$filter', 'assetsService', 'localizationService', 'eventsService', 'salesByItemResource', 'settingsResource', 'queryDisplayBuilder',
    function($log, $filter, assetsService, localizationService, eventsService, salesByItemResource, settingsResource, queryDisplayBuilder) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            setloaded: '&'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/reportwidget.topselling.tpl.html',
        link: function(scope, elm, attr) {

            var datesChangeEventName = 'merchello.reportsdashboard.datechange';

            scope.loaded = false;
            scope.settings = {};
            scope.results = [];
            scope.chartData = [];
            scope.labels = [];

            scope.startDate = '';
            scope.endDate = '';

            scope.reload = reload;

            assetsService.loadCss('/App_Plugins/Merchello/lib/charts/angular-chart.min.css').then(function() {
                init();
            });

            function init() {

            }

            function loadReportData() {
                dataLoaded(false);
                scope.results = [];
                scope.chartData = [];
                scope.labels = [];

                var query = queryDisplayBuilder.createDefault();
                query.addInvoiceDateParam($filter('date')(scope.startDate, 'yyyy-MM-dd'), 'start');
                query.addInvoiceDateParam($filter('date')(scope.endDate, 'yyyy-MM-dd'), 'end');

                salesByItemResource.getCustomReportData(query).then(function(results) {

                    angular.forEach(results.items, function(item) {
                        scope.labels.push(item.productVariant.name);
                        scope.chartData.push(item.quantitySold);
                    });

                    if (scope.chartData.length === 0) {
                        scope.chartData.push(1);
                        scope.labels.push('No results');
                    }

                    scope.results = results.items;
                    dataLoaded(true);
                    scope.loaded = true;
                });
            }

            function reload(startDate, endDate) {
                scope.startDate = startDate;
                scope.endDate = endDate;
                loadReportData();
            }

            function dataLoaded(value) {
                scope.loaded = value;
                scope.setloaded()(value);
            }

        }
    };
}]);