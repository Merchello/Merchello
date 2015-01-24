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

            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.tabs = [];
            $scope.customer = {}

            function init() {
                var key = $routeParams.id;
                $scope.tabs = merchelloTabsFactory.createCustomerOverviewTabs(key);
                $scope.tabs.setActive('addresses');
            }

            // initialize the controller
            init();
    }]);
