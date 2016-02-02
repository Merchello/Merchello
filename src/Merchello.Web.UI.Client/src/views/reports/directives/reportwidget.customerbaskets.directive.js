angular.module('merchello.directives').directive('reportWidgetCustomerBaskets',
    ['$q', '$compile', '$filter', 'assetsService', 'localizationService', 'settingsResource', 'queryDisplayBuilder', 'invoiceHelper', 'abandonedBasketResource',
        function($q, $compile, $filter, assetsService, localizationService, settingsResource, queryDisplayBuilder, invoiceHelper, abandonedBasketResource) {

            return {
                restrict: 'E',
                replace: true,
                scope: {
                    setLoaded: '&'
                },
                templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/reportwidget.customerbaskets.tpl.html',
                link: function (scope, elm, attr) {

                    const baseUrl = '#/merchello/merchello/customeroverview/';

                    scope.loaded = true;
                    scope.entityType = 'customerBaskets';
                    scope.settings = {};
                    scope.currencySymbol = '';
                    scope.load = load;
                    scope.getColumnValue = getColumnValue;
                    scope.preValuesLoaded = false;
                    scope.pageSize = 10;

                    scope.itemLabel = '';
                    scope.skuLabel = '';
                    scope.quantityLabel = '';
                    scope.priceLabel = '';
                    scope.subTotalLabel = '';


                    function init() {
                        $q.all([
                            settingsResource.getAllCombined(),
                            localizationService.localize('merchelloGeneral_item'),
                            localizationService.localize('merchelloVariant_sku'),
                            localizationService.localize('merchelloGeneral_quantity'),
                            localizationService.localize('merchelloGeneral_price'),
                            localizationService.localize('merchelloGeneral_subTotal')
                        ]).then(function(data) {
                            scope.settings = data[0].settings;
                            scope.currencySymbol = data[0].currencySymbol;
                            scope.itemLabel = data[1];
                            scope.skuLabel = data[2];
                            scope.quantityLabel = data[3];
                            scope.priceLabel = data[4];
                            scope.subTotalLabel = data[5];
                            scope.preValuesLoaded = true;
                        });

                    }

                      function getColumnValue(result, col) {
                        switch(col.name) {
                            case 'loginName':
                                return '<a href="' + getEditUrl(result.customer) + '">' + result.customer.loginName + '</a>';
                            case 'firstName':
                                return  '<a href="' + getEditUrl(result) + '">' + result.customer.firstName + ' ' + result.customer.lastName + '</a>';
                            case 'lastActivityDate':
                                return $filter('date')(result.customer.lastActivityDate, scope.settings.dateFormat);
                            case 'items':
                                return buildItemsTable(result.items);
                            default:
                                return result[col.name];
                        }
                    }


                    function getEditUrl(customer) {
                        return baseUrl + customer.key;
                    }

                    function load(query) {
                        scope.setLoaded()(false);
                        var deferred = $q.defer();

                        abandonedBasketResource.getCustomerSavedBasketsLegacy(query).then(function(results) {
                            console.info(results);
                            deferred.resolve(results);
                        });
                        scope.setLoaded()(true);
                        return deferred.promise;
                    }

                    function buildItemsTable(items) {
                        var html = '<table class=\'table table-striped\'>';
                        html += '<thead><tr>';
                        html += '<th>' + scope.itemLabel + '</th>';
                        html += '<th>' + scope.skuLabel + '</th>';
                        html += '<th>' + scope.quantityLabel + '</th>';
                        html += '<th>' + scope.priceLabel + '</th>';
                        html += '<th>' + scope.subTotalLabel + '</th>';
                        html += '</tr></thead>'
                        html += '<tbody>';
                        angular.forEach(items, function(item) {
                            html += '<tr>';
                            html += '<th>' + item.name + '</th>';
                            html += '<th>' + item.sku + '</th>';
                            html += '<th>' + item.quantity + '</th>';
                            html += '<th>' + $filter('currency')(item.price, scope.currencySymbol ) + '</th>';
                            html += '<th>' + $filter('currency')(item.price * item.quantity, scope.currencySymbol ) + '</th>';
                            html += '</tr>';
                        });
                        html += '</tbody></table>';

                        return html;
                    }

                    init();
                }

            }
        }]);