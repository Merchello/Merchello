/*! merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2015 Merchello;
 * Licensed MIT
 */

(function() { 

    /**
     * @ngdoc directive
     * @name filter-by-date-range
     * @function
     *
     * @description
     * Directive to wrap all Merchello Mark up.
     */
    angular.module('merchello.directives').directive('filterByDateRange', function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                dateFormat: '=',
                filterWithDates: '&'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/filterbydaterange.tpl.html'
        };
    });

    /**
     * @ngdoc directive
     * @name merchello-panel
     * @function
     *
     * @description
     * Directive to wrap all Merchello Mark up and provide common classes.
     */
     angular.module('merchello.directives').directive('merchelloPanel', function() {
         return {
             restrict: 'E',
             replace: true,
             transclude: 'true',
             templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/html/merchellopanel.tpl.html'
         };
     });

    /**
     * @ngdoc directive
     * @name merchello-slide-open-panel
     * @function
     *
     * @description
     * Directive to allow a section of content to slide open/closed based on a boolean value
     */
    angular.module('merchello.directives').directive('merchelloSlideOpenPanel', function() {
        return {
            restrict: 'E',
            replace: true,
            transclude: 'true',
            scope: {
                isOpen: '=',
                classes: '=?'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/html/merchelloslidepanelopen.tbl.html',
            link: function ($scope, $element, attrs) {

                if ($scope.classes == undefined) {
                    $scope.classes = 'control-group umb-control-group';
                }
            }
        };
    });

/**
 * @ngdoc directive
 * @name address directive
 * @function
 *
 * @description
 * Directive to maintain a consistent format for displaying addresses
 */
angular.module('merchello.directives').directive('merchelloAddress', function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                address: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/merchelloAddress.tpl.html'
        };
    }).directive('merchelloAddress', function() {
        return {
            restrict: 'A',
            transclude: true,
            scope: {
                setAddress: '&setAddress'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/merchelloAddress.tpl.html',
            link: function(scope, elm, attr) {
                scope.address = scope.setAddress();
            }
        }
    });

    /**
     * @ngdoc directive
     * @name MerchelloPagerDirective
     * @function
     *
     * @description
     * directive to display display a pager for orders, products, and others.
     *
     * TODO: Currently, makes assumptions using the parent scope.  In future, make this work as an isolate scope.
     */
    angular.module('merchello.directives').directive('merchelloPager', function() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/merchellopager.tpl.html'
        };
    });
    /**
     * @ngdoc directive
     * @name MerchelloSalesPageNav
     * @function
     *
     * @description
     * directive to display display a buttons on subordinate sales pages.
     */

    angular.module('merchello.directives').directive('merchelloSalesPageNav', function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                key: '=',
                hideShipments: '=',
                hidePayments: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/salespagesnav.tpl.html'
        };
    });

angular.module('merchello.directives').directive('resolvedGatewayProviders', [function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            providerList: '=',
            'activate': '&onActivate',
            'deactivate': '&onDeactivate',
            'configure': '&onConfigure'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/resolvedgatewayproviders.tpl.html'
    };
}]);

angular.module('merchello.directives').directive('shipCountryGatewayProviders',
    ['notificationsService', 'dialogService',
    'shippingGatewayProviderResource', 'shippingGatewayProviderDisplayBuilder',
    'gatewayResourceDisplayBuilder', 'shipCountryDisplayBuilder',
    function(notificationsService, dialogService, shippingGatewayProviderResource, shippingGatewayProviderDisplayBuilder,
    gatewayResourceDisplayBuilder, shipCountryDisplayBuilder) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            country: '='
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/shipcountrygatewayproviders.tpl.html',
        link: function(scope, elm, attr) {

                function init() {
                    loadCountryProviders();
                }

                /**
                 * @ngdoc method
                 * @name loadCountryProviders
                 * @function
                 *
                 * @description
                 * Load the shipping gateway providers from the shipping gateway service, then wrap the results
                 * in Merchello models and add to the scope via the shippingGatewayProviders collection on the country model.  After
                 * load is complete, it calls the loadProviderMethods to load in the methods.
                 */
                function loadCountryProviders() {
                    var promiseProviders = shippingGatewayProviderResource.getAllShipCountryProviders(scope.country);
                    promiseProviders.then(function (providers) {
                        if (angular.isArray(providers)) {
                            scope.assignedProviders = shippingGatewayProviderDisplayBuilder.transform(providers);
                            console.info(scope.shipCountryProviders);
                            //console.info($scope.shipCountryProviders);
                            /* _.each(providerFromServer, function (element, index, list) {
                             if (element) {
                             var newProvider = new merchello.Models.ShippingGatewayProvider(element);
                             // Need this to get the name for now.
                             var tempGlobalProvider = _.find($scope.providers, function (p) {
                             return p.key == newProvider.key;
                             });
                             newProvider.name = tempGlobalProvider.name;
                             newProvider.typeFullName = tempGlobalProvider.typeFullName;
                             newProvider.resources = tempGlobalProvider.resources;
                             newProvider.shipMethods = [];
                             country.shippingGatewayProviders.push(newProvider);
                             $scope.loadProviderMethods(newProvider, country);
                             } */
                        }

                    }, function (reason) {
                        notificationsService.error("Fixed Rate Shipping Countries Providers Load Failed", reason.message);
                    });
                }


                /**
                 * @ngdoc method
                 * @name loadAllAvailableGatewayResources
                 * @function
                 *
                 * @description
                 * Load the shipping gateway resources from the shipping gateway service, then wrap the results
                 * in Merchello models and add to the scope via the providers collection in the resources collection.
                 */
                function loadAllAvailableGatewayResources(shipProvider) {
                    var promiseAllResources = shippingGatewayProviderResource.getAllShipGatewayResourcesForProvider(shipProvider);
                    promiseAllResources.then(function (allResources) {
                        shipProvider.resources = gatewayResourceDisplayBuilder.transform(allResources);
                    }, function (reason) {
                        notificationsService.error("Available Gateway Resources Load Failed", reason.message);
                    });
                }

                /**
                 * @ngdoc method
                 * @name loadShipMethods
                 * @function
                 *
                 * @description
                 * Load the shipping methods from the shipping gateway service, then wrap the results
                 * in Merchello models and add to the scope via the provider in the shipMethods collection.
                 */
                function loadProviderMethods(shipProvider) {
                    var promiseShipMethods = merchelloCatalogShippingService.getShippingProviderShipMethodsByCountry(shipProvider, scope.country);
                    promiseShipMethods.then(function (shipMethods) {
                        //shipProvider.shipMethods = _.map(shipMethods, function (method) {
                        //    return new merchello.Models.ShippingMethod(method);
                        //});
                    }, function (reason) {
                        notificationsService.error("Available Shipping Methods Load Failed", reason.message);
                    });
                }

            // initialize the directive
            init();
        }
    };
}]);



})();