    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.CustomerListController
     * @function
     *
     * @description
     * The controller for customer addresses view
     */
    angular.module('merchello').controller('Merchello.Backoffice.CustomerAddressesController',
        ['$scope', '$routeParams', '$timeout', 'dialogService', 'notificationsService', 'settingsResource', 'merchelloTabsFactory', 'customerResource',
            'countryDisplayBuilder', 'customerDisplayBuilder',
        function($scope, $routeParams, $timeout, dialogService, notificationsService, settingsResource, merchelloTabsFactory, customerResource,
                 countryDisplayBuilder, customerDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
            $scope.customer = {};
            $scope.billingAddresses = [];
            $scope.shippingAddresses = [];
            $scope.countries = [];

            // exposed methods
            $scope.reload = init;
            $scope.save = save;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Initialize the controller.
             */
            function init() {
                var key = $routeParams.id;
                loadCustomer(key);
                loadCountries();
            }

            /**
             * @ngdoc method
             * @name loadCustomer
             * @function
             *
             * @description
             * Loads the customer.
             */
            function loadCustomer(key) {
                var promiseLoadCustomer = customerResource.GetCustomer(key);
                promiseLoadCustomer.then(function(customerResponse) {
                    $scope.customer = customerDisplayBuilder.transform(customerResponse);
                    $scope.tabs = merchelloTabsFactory.createCustomerOverviewTabs(key, $scope.customer.hasAddresses());
                    $scope.tabs.setActive('addresses');
                    $scope.shippingAddresses = _.sortBy($scope.customer.getShippingAddresses(), function(adr) { return adr.label; });
                    $scope.billingAddresses = _.sortBy($scope.customer.getBillingAddresses(), function(adr) { return adr.label; });
                    loadCountries();
                }, function(reason) {
                    notificationsService.error("Failed to load customer", reason.message);
                });
            }

            function loadCountries() {
                var promise = settingsResource.getAllCountries();
                promise.then(function(countries) {
                    $scope.countries = countryDisplayBuilder.transform(countries);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function(reason) {
                    notificationsService.error('Failed to load add countries', reason);
                });
            }

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Save the customer.
             */
            function save(customer) {
                $scope.preValuesLoaded = false;
                notificationsService.info(localizationService.localize("merchelloStatusNotifications_customerSaveInProgress"), "");
                var promiseSaveCustomer = customerResource.SaveCustomer($scope.customer);
                promiseSaveCustomer.then(function(customer) {
                    $timeout(function() {
                        notificationsService.success(localizationService.localize("merchelloStatusNotifications_customerSaveSuccess"), "");
                        init();
                    }, 400);

                }, function(reason) {
                    notificationsService.error(localizationService.localize("merchelloStatusNotifications_customerSaveError"), reason.message);
                });
            }

            // initialize the controller
            init();
    }]);
