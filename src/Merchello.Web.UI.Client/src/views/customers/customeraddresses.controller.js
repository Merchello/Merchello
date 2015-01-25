    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.CustomerListController
     * @function
     *
     * @description
     * The controller for customer addresses view
     */
    angular.module('merchello').controller('Merchello.Backoffice.CustomerAddressesController',
        ['$scope', '$routeParams', 'dialogService', 'notificationsService', 'merchelloTabsFactory', 'customerResource', 'customerDisplayBuilder',
        function($scope, $routeParams, dialogService, notificationsService, merchelloTabsFactory, customerResource, customerDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
            $scope.customer = {};
            $scope.billingAddresses = [];
            $scope.shippingAddresses = [];

            function init() {
                var key = $routeParams.id;
                loadCustomer(key);
            }

            function loadCustomer(key) {
                var promiseLoadCustomer = customerResource.GetCustomer(key);
                promiseLoadCustomer.then(function(customerResponse) {
                    $scope.customer = customerDisplayBuilder.transform(customerResponse);
                    $scope.tabs = merchelloTabsFactory.createCustomerOverviewTabs(key, $scope.customer.hasAddresses());
                    $scope.tabs.setActive('addresses');
                    $scope.shippingAddresses = $scope.customer.getShippingAddresses();
                    $scope.billingAddresses = $scope.customer.getBillingAddresses();
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;

                }, function(reason) {
                    notificationsService.error("Failed to load customer", reason.message);
                });
            }

            // initialize the controller
            init();
    }]);
