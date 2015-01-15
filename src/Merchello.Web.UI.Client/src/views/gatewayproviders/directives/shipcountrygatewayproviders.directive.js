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

                        $scope.loaded = true;
                        $scope.preValuesLoaded = true;

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

