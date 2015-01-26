/*! merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2015 Merchello;
 * Licensed MIT
 */

(function() { 

/**
 * @ngdoc directive
 * @name customer-address-table
 * @function
 *
 * @description
 * Directive to list customer addresses.
 */
angular.module('merchello.directives').directive('customerAddressTable', function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            customer: '=',
            countries: '=',
            addresses: '=',
            addressType: '@',
            save: '&'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/customer.customeraddresstable.tpl.html',
        controller: function($scope, dialogService, notificationsService, dialogDataFactory, customerAddressDisplayBuilder, CustomerAddressDisplay) {

            // exposed methods
            $scope.openDeleteAddressDialog = openDeleteAddressDialog;
            $scope.openAddressAddEditDialog = openAddressAddEditDialog;

            console.info($scope.addressType);

            /**
             * @ngdoc method
             * @name openAddressEditDialog
             * @function
             *
             * @description
             * Opens the edit address dialog via the Umbraco dialogService.
             */
            function openAddressAddEditDialog(address) {
                var dialogData = dialogDataFactory.createAddEditCustomerAddressDialogData();
                // if the address is not defined we need to create a default (empty) CustomerAddressDisplay
                dialogData.customerAddress = customerAddressDisplayBuilder.createDefault();
                if(address === null || address === undefined) {

                    dialogData.selectedCountry = $scope.countries[0];
                } else {
                    dialogData.customerAddress = angular.extend(dialogData.customerAddress, address); //address;
                    dialogData.selectedCountry = address.countryCode === '' ? $scope.countries[0] :
                        _.find($scope.countries, function(country) {
                            return country.countryCode === address.countryCode;
                        });
                }
                dialogData.countries = $scope.countries;
                dialogData.customerAddress.customerKey = $scope.customer.key;
                if (dialogData.selectedCountry.hasProvinces()) {
                    if(dialogData.customerAddress.region !== '') {
                        dialogData.selectedProvince = _.find(dialogData.selectedCountry.provinces, function(province) {
                            return province.code === address.region;
                        });
                    }
                    if(dialogData.selectedProvince === null || dialogData.selectedProvince === undefined) {
                        dialogData.selectedProvince = dialogData.selectedCountry.provinces[0];
                    }
                }
                // if the customer has not addresses of the given type we are going to force an added
                // address to be the primary address

                dialogData.customerAddress.addressType = $scope.addressType;
                console.info(dialogData);
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/customer.customeraddress.addedit.html',
                    show: true,
                    callback: processAddEditAddressDialog,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name openDeleteAddressDialog
             * @function
             *
             * @description
             * Opens a dialog for deleting an address.
             */
            function openDeleteAddressDialog(address) {
                var dialogData = dialogDataFactory.createDeleteCustomerAddressDialogData();
                dialogData.customerAddress = address;
                dialogData.name = address.label;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processDeleteAddress,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name processDeleteAddress
             * @function
             *
             * @description
             * Save the customer with a deleted address.
             */
            function processDeleteAddress(dialogData) {
                //console.info(dialogData);
                $scope.customer.addresses = _.reject($scope.customer.addresses, function(address) {
                    return address.key === dialogData.customerAddress.key;
                });
                save();
            }

            function processAddEditAddressDialog(dialogData) {
                var defaultAddressOfType = $scope.customer.getDefaultAddress(dialogData.customerAddress.addressType);
                if(dialogData.customerAddress.key !== '') {
                    $scope.customer.addresses = _.reject($scope.customer.addresses, function(address) {
                        return address.key == dialogData.customerAddress.key;
                    });
                }
                if (dialogData.customerAddress.isDefault && defaultAddressOfType !== undefined) {
                    if(dialogData.customerAddress.key !== defaultAddressOfType.key) {
                        defaultAddressOfType.isDefault = false;
                    }
                }
                $scope.customer.addresses.push(dialogData.customerAddress);
                console.info($scope.customer);
                save();
            }

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Saves the customer.
             */
            function save() {
                $scope.save();
            }

        }
    };
});


    /**
     * @ngdoc directive
     * @name customer-location
     * @function
     *
     * @description
     * Directive to display a customer location.
     */
    angular.module('merchello.directives').directive('customerLocation',
        [function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                address: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/customer.customerlocation.tpl.html',
            controller: function($scope) {
                console.info($scope.address);
            }
        };
    }]);

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
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/html/merchelloslidepanelopen.tpl.html',
            link: function ($scope, $element, attrs) {

                if ($scope.classes == undefined) {
                    $scope.classes = 'control-group umb-control-group';
                }
            }
        };
    });

angular.module('merchello.directives').directive('merchelloTabs', [function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            tabs: '='
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/html/merchellotabs.tpl.html'
    };
}]);

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
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/merchelloaddress.tpl.html'
        };
    }).directive('merchelloAddress', function() {
        return {
            restrict: 'A',
            transclude: true,
            scope: {
                setAddress: '&setAddress'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/merchelloaddress.tpl.html',
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
     * @name resetListfilters
     * @function
     *
     * @description
     * directive to clear list filters.
     *
     * TODO: Currently, makes assumptions using the parent scope.  In future, make this work as an isolate scope.
     */
    angular.module('merchello.directives').directive('resetListFilters', [function() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/resetlistfilters.tpl.html'
        };
    }]);


    angular.module('merchello.directives').directive('notificationMethods', function() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/notificationmethods.tpl.html',
            controller: function($scope) {

                $scope.getMonitorName = function(key) {
                    var monitor = _.find($scope.notificationMonitors, function(monitor) {
                        return monitor.key === key;
                    });
                    if(monitor !== null || monitory !== undefined) {
                        return monitor.name;
                    } else {
                        return 'Not found';
                    }
                }

            }
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

angular.module('merchello.directives').directive('shipCountryGatewayProviders', function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            country: '=',
            reload: '&',
            delete: '&'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/shipcountrygatewayproviders.tpl.html',
        controller: 'Merchello.Directives.ShipCountryGatewaysProviderDirectiveController'
    };
});

})();