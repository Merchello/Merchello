angular.module('merchello.directives').directive('reportWidgetTopFiveSelling',
    ['$log', '$filter', 'assetsService', 'localizationService', 'eventsService', 'salesByItemResource', 'settingsResource', 'queryDisplayBuilder',
    function($log, $filter, assetsService, localizationService, eventsService, salesByItemResource, settingsResource, queryDisplayBuilder) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            ready: '=?',
            startDate: '=',
            endDate: '='
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/reportwidget.topfiveselling.tpl.html',
        link: function(scope, elm, attr) {

            var datesChangeEventName = 'merchello.reportsdashboard.datechange';

            scope.loaded = false;
            scope.busy = false;
            scope.settings = {};
            scope.results = [];
            scope.chartData = [];
            scope.labels = [];


            assetsService.loadCss('/App_Plugins/Merchello/lib/charts/angular-chart.min.css').then(function() {
                init();
            });

            function init() {
                eventsService.on(datesChangeEventName, onOnDatesChanged);
                if (!scope.loaded && !scope.busy) {
                    loadReportData();
                }
            }

            function loadReportData() {
                scope.busy = true;

                scope.results = [];
                scope.chartData = [];
                scope.labels = [];

                var query = queryDisplayBuilder.createDefault();
                query.addInvoiceDateParam(scope.startDate, 'start');
                query.addInvoiceDateParam(scope.endDate, 'end');

                salesByItemResource.getCustomReportData(query).then(function(results) {

                    angular.forEach(results.items, function(item) {
                        scope.labels.push(item.productVariant.name);
                        scope.chartData.push(item.quantitySold);
                    });

                    if (scope.chartData.length === 0) {
                        scope.chartData.push(0);
                        scope.labels.push('No results');
                    }

                    scope.results = results.items;
                    scope.busy = false;
                    scope.loaded = true;
                });
            }


            function onOnDatesChanged(e, args) {

                scope.startDate = args.startDate;
                scope.endDate = args.endDate;

                loadReportData();
            }
        }
    };
}]);