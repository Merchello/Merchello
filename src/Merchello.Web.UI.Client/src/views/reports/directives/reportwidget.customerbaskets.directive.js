angular.module('merchello.directives').directive('reportWidgetCustomerBaskets',
    ['$q', '$filter', 'assetsService', 'localizationService', 'settingsResource', 'queryDisplayBuilder', 'invoiceHelper', 'abandonedBasketResource',
        function($q, $filter, assetsService, localizationService, settingsResource, queryDisplayBuilder, invoiceHelper, abandonedBasketResource) {

            return {
                restrict: 'E',
                replace: true,
                scope: {
                    setLoaded: '&'
                },
                templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/reportwidget.customerbaskets.tpl.html',
                link: function (scope, elm, attr) {

                    scope.loaded = true;

                    function init() {
                        getBasketData();
                    }

                    function getBasketData() {
                        scope.setLoaded()(false);

                        var query = queryDisplayBuilder.createDefault();
                        query.sortBy = 'lastActivityDate';
                        query.sortDirection = 'Descending';
                        abandonedBasketResource.getCustomerSavedBaskets(query).then(function(results) {
                            console.info(results);
                            scope.setLoaded()(true);
                        });
                    }

                    init();
                }

            }
        }]);