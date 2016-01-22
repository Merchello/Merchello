angular.module('merchello.directives').directive('reportWidgetAbandonedBasketRadar',
    ['$q', '$filter', 'assetsService', 'localizationService', 'settingsResource', 'invoiceHelper', 'abandonedBasketResource',
    function($q, $filter, assetsService, localizationService, settingsResource, invoiceHelper, abandonedBasketResource) {

        return {
            restrict: 'E',
            replace: true,
            scope: {
              setLoaded: '&'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/reportwidget.abandonedbasketradar.tpl.html',
            link: function (scope, elm, attr) {

                scope.labels = [];
                scope.data = [];
                scope.settings = {};
                scope.result = {};
                scope.anonymousBaskets = '';
                scope.anonymousCheckouts = '';
                scope.customerBaskets = '';
                scope.customerCheckouts = '';
                scope.anonymousPercentLabel = '';
                scope.customerPercentLabel = '';

                scope.anonymousCheckoutPercent = 0;
                scope.customerCheckoutPercent = 0;


                assetsService.loadCss('/App_Plugins/Merchello/lib/charts/angular-chart.min.css').then(function() {
                    init();
                });

                function init() {
                    scope.setLoaded()(false);

                    $q.all([
                        localizationService.localize('merchelloReports_anonymousBaskets'),
                        localizationService.localize('merchelloReports_anonymousCheckouts'),
                        localizationService.localize('merchelloReports_customerBaskets'),
                        localizationService.localize('merchelloReports_customerCheckouts'),
                        localizationService.localize('merchelloReports_anonymousCheckoutPercent'),
                        localizationService.localize('merchelloReports_customerCheckoutPercent'),
                        abandonedBasketResource.getDefaultReportData(),
                        settingsResource.getAllSettings()

                    ]).then(function(data) {

                        scope.anonymousBaskets = data[0];
                        scope.anonymousCheckouts = data[1];
                        scope.customerBaskets = data[2];
                        scope.customerCheckouts = data[3];
                        scope.anonymousPercentLabel = data[4];
                        scope.customerPercentLabel = data[5];
                        scope.result = data[6].items[0];
                        scope.settings = data[7];

                        scope.anonymousCheckoutPercent = invoiceHelper.round(scope.result.anonymousCheckoutPercent, 2);
                        scope.customerCheckoutPercent = invoiceHelper.round(scope.result.customerCheckoutPercent, 2);

                        scope.labels.push(data[0], data[1], data[2], data[3]);
                        scope.data.push(
                            scope.result.anonymousBasketCount,
                            scope.result.anonymousCheckoutCount,
                            scope.result.customerBasketCount,
                            scope.result.customerCheckoutCount);

                        scope.setLoaded()(true);

                        scope.loaded = true;
                    });

                }

            }
        }
    }]);
