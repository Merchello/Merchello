angular.module('merchello.directives').directive('reportWidgetTopFiveSelling',
    ['$log', '$filter', 'assetsService', 'localizationService', 'salesByItemResource', 'settingsResource', 'queryDisplayBuilder',
    function($log, $filter, assetsService, localizationService, salesByItemResource, settingsResource, queryDisplayBuilder) {

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

            scope.loaded = false;
            scope.settings = {};
            scope.results = [];
            scope.chartData = [];
            scope.labels = [];

            assetsService.loadCss('/App_Plugins/Merchello/lib/charts/angular-chart.min.css').then(function() {
                init();
            });

            function init() {
                if (!('ready' in attr)) {
                    scope.isReady = true;
                }

                scope.$watch('ready', function(newVal, oldVal) {
                    if (newVal === true) {
                        scope.isReady = newVal;
                    }
                    if(scope.isReady) {
                        loadSettings();
                    }
                });
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
                    scope.settings = combined.settings;
                    loadReportData();
                });
            };

            function loadReportData() {

                var query = queryDisplayBuilder.createDefault();
                query.addInvoiceDateParam(scope.startDate, 'start');
                query.addInvoiceDateParam(scope.endDate, 'end');

                salesByItemResource.getCustomReportData(query).then(function(results) {
                    
                    angular.forEach(results.items, function(item) {
                        scope.labels.push(item.productVariant.name);
                        scope.chartData.push(item.quantitySold);
                    });
                    scope.results = results.items;
                    console.info(scope.results);
                    scope.loaded = true;
                });
            }
        }
    };
}])