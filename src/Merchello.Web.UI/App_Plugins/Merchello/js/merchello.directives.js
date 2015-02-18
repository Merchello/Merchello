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
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/customer.customerlocation.tpl.html'
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
                filterStartDate: '=',
                filterEndDate: '=',
                filterWithDates: '&'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/filterbydaterange.tpl.html',
            controller: function($scope, $element, assetsService, angularHelper, notificationsService, settingsResource, settingDisplayBuilder) {

                $scope.settings = {};

                // exposed methods
                $scope.changeDateFilters = changeDateFilters;

                function init() {
                    loadCssAssets();
                    loadSettings();
                }

                /**
                 * @ngdoc method
                 * @name loadCssAssets
                 * @function
                 *
                 * @description - Loads needed stylesheets for the view.
                 */
                function loadCssAssets() {

                    assetsService.loadCss('lib/datetimepicker/bootstrap-datetimepicker.min.css').then(function () {
                        var filesToLoad = ["lib/datetimepicker/bootstrap-datetimepicker.min.js"];
                        assetsService.load(filesToLoad).then(
                            function () {
                                //The Datepicker js and css files are available and all components are ready to use.

                                setupDatePicker("#filterStartDate");
                                $element.find("#filterStartDate").datetimepicker().on("changeDate", applyDateStart);

                                setupDatePicker("#filterEndDate");
                                $element.find("#filterEndDate").datetimepicker().on("changeDate", applyDateEnd);
                            });
                    });
                }

                function loadSettings() {
                    var promise = settingsResource.getAllSettings();
                    promise.then(function(allSettings) {
                        console.info(allSettings);
                        $scope.settings = settingDisplayBuilder.transform(allSettings);
                    }, function(reason) {
                        notificationsService.error('Failed to load settings', reason.message);
                    });
                }

                /**
                 * @ngdoc method
                 * @name setupDatePicker
                 * @function
                 *
                 * @description
                 * Sets up the datepickers
                 */
                function setupDatePicker(pickerId) {

                    // Open the datepicker and add a changeDate eventlistener
                    $element.find(pickerId).datetimepicker();

                    //Ensure to remove the event handler when this instance is destroyted
                    $scope.$on('$destroy', function () {
                        $element.find(pickerId).datetimepicker("destroy");
                    });
                }

                /*-------------------------------------------------------------------
                 * Event Handler Methods
                 *-------------------------------------------------------------------*/

                /**
                 * @ngdoc method
                 * @name changeDateFilters
                 * @function
                 *
                 * @param {string} start - String representation of start date.
                 * @param {string} end - String representation of end date.
                 * @description - Change the date filters, then triggera new API call to load the reports.
                 */
                function changeDateFilters(start, end) {
                    $scope.filterStartDate = start;
                    $scope.filterEndDate = end;
                    $scope.currentPage = 0;
                    $scope.filterWithDates();
                }

                /*-------------------------------------------------------------------
                 * Helper Methods
                 * ------------------------------------------------------------------*/

                //handles the date changing via the api
                function applyDateStart(e) {
                    angularHelper.safeApply($scope, function () {
                        // when a date is changed, update the model
                        if (e.localDate) {
                            $scope.filterStartDate = e.localDate.toIsoDateString();
                        }
                    });
                }

                //handles the date changing via the api
                function applyDateEnd(e) {
                    angularHelper.safeApply($scope, function () {
                        // when a date is changed, update the model
                        if (e.localDate) {
                            $scope.filterEndDate = e.localDate.toIsoDateString();
                        }
                    });
                }

                // Initialize the controller
                init();
            }
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
                classes: '=?',
                hideClose: '='
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

    /**
     * @ngdoc directive
     * @name tagsManager
     * @function
     *
     * @description
     * directive for a tags manager.
     */
    angular.module('merchello.directives').directive('tagManager', function() {
        return {
            restrict: 'E',
            scope: { option: '=' },
            template:
            '<div class="tags">' +
            '<a ng-repeat="(idx, choice) in option.choices" class="tag" ng-click="remove(idx)">{{choice.name}}</a>' +
            '</div>' +
            '<input type="text" placeholder="Add a choice..." ng-model="newChoiceName"></input> ' +
            '<a class="btn btn-primary" ng-click="add()">Add</a>',
            link: function ($scope, $element) {
                // FIXME: this is lazy and error-prone
                // this is the option name input
                var input = angular.element($element.children()[1]);

                // This adds the new tag to the tags array
                $scope.add = function () {
                    if ($scope.newChoiceName.length > 0) {
                        $scope.option.addAttributeChoice($scope.newChoiceName);
                        $scope.newChoiceName = "";
                    }
                };

                // This is the ng-click handler to remove an item
                $scope.remove = function (idx) {
                    $scope.option.removeAttributeChoice(idx);
                };

                // Capture all keypresses
                input.bind('keypress', function (event) {
                    // But we only care when Enter was pressed
                    if (event.keyCode == 13) {
                        // There's probably a better way to handle this...
                        $scope.add();
                    }
                });

            }
        };
    });


    angular.module('merchello.directives').directive('notificationMethods', function() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/notificationmethods.tpl.html',
            controller: function($scope) {

                // Exposed monitors
                $scope.getMonitorName = getMonitorName;

                function getMonitorName(key) {
                    var monitor = _.find($scope.notificationMonitors, function(monitor) {
                        return monitor.monitorKey === key;
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
    /**
     * @ngdoc controller
     * @name productOptionsManage
     * @function
     *
     * @description
     * The productOptionsManage directive
     */
    angular.module('merchello.directives').directive('productOptionsManage', function() {

        return {
            restrict: 'E',
            replace: true,
            scope: {
                product: '=',
                parentForm: '=',
                classes: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/product.optionsmanage.tpl.html',

            controller: function ($scope) {
                $scope.rebuildVariants = false;
                $scope.addOption = addOption;
                $scope.removeOption = removeOption;

                /**
                 * @ngdoc method
                 * @name addOption
                 * @function
                 *
                 * @description
                 * Called when the Add Option button is pressed.  Creates a new option ready to fill out.
                 */
                function addOption() {
                    $scope.product.addEmptyOption();
                }

                /**
                 * @ngdoc method
                 * @name removeOption
                 * @function
                 *
                 * @description
                 * Called when the Trash can icon button is pressed next to an option. Removes the option from the product.
                 */
                function removeOption (option) {
                    $scope.product.removeOption(option);
                }
            }
        };

    });

/**
 * @ngdoc controller
 * @name productVariantsViewTable
 * @function
 *
 * @description
 * The productVariantsViewTable directive
 */
angular.module('merchello.directives').directive('productVariantsViewTable', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            product: '=',
            currencySymbol: '=',
            reload: '&'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/product.productvariantsviewtable.tpl.html',
        controller: 'Merchello.Directives.ProductVariantsViewTableDirectiveController'
    };
});


    angular.module('merchello.directives').directive('productReorderOptions', [function() {

        return {
            restrict: 'E',
            replace: true,
            scope: {
                isolateIsOpen: '=isOpen',
                product: '=',
                reload: '&'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/product.reorderoptions.tpl.html',
            link: function ($scope, $element) {
                /**
                 * @ngdoc method
                 * @name close
                 * @function
                 *
                 * @description
                 * Set the isOpen scope property to false to close the dialog
                 */
                $scope.close = function ($event) {
                    $scope.isolateIsOpen = false;
                };

                // Settings for the sortable directive
                $scope.sortableOptions = {
                    stop: function (e, ui) {
                        for (var i = 0; i < $scope.product.productOptions.length; i++) {
                            $scope.product.productOptions[i].sortOrder(i + 1);
                        }
                        $scope.product.fixAttributeSortOrders();
                    },
                    axis: 'y',
                    cursor: "move"
                };

                $scope.sortableChoices = {
                    start: function (e, ui) {
                        $(e.target).data("ui-sortable").floating = true;    // fix for jQui horizontal sorting issue https://github.com/angular-ui/ui-sortable/issues/19
                    },
                    stop: function (e, ui) {
                        var attr = ui.item.scope().attribute;
                        var attrOption = _.find($scope.product.productOptions, function(po) { return po.key === attr.optionKey; });
                        attrOption.resetChoiceSortOrders();
                    },
                    update: function (e, ui) {
                        var attr = ui.item.scope().attribute;
                        var attrOption = _.find($scope.product.productOptions, function(po) { return po.key === attr.optionKey; });
                        attrOption.resetChoiceSortOrders();
                    },
                    cursor: "move"
                };
            }
        };
    }]);

    /**
     * @ngdoc controller
     * @name productVariantDigitalDownload
     * @function
     *
     * @description
     * The productVariantDigitalDownload directive
     */
    angular.module('merchello.directives').directive('productVariantDigitalDownload',
        function() {

            return {
                restrict: 'E',
                replace: true,
                scope: {
                    productVariant: '=',
                    preValuesLoaded: '='
                },
                templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/productvariant.digitaldownload.tpl.html',
                link: function(scope, element, attributes) {
                    scope.$watch(attributes.preValuesLoaded, function(value) {
                        scope.initialize();
                    });
                },
                controller: function ($scope, dialogService, mediaHelper, mediaResource) {

                    $scope.mediaItem = null;
                    $scope.thumbnail = '';
                    $scope.icon = '';

                    $scope.chooseMedia = chooseMedia;
                    $scope.removeMedia = removeMedia;
                    $scope.initialize = initialize;

                    function init() {
                        if ($scope.productVariant.download && $scope.productVariant.downloadMediaId != -1) {
                            mediaResource.getById($scope.productVariant.downloadMediaId).then(function (media) {
                                $scope.mediaItem = media;
                                $scope.mediaItem.umbracoFile = mediaHelper.resolveFile(media, false);
                                if(!media.thumbnail) {
                                    $scope.thumbnail = mediaHelper.resolveFile(media, true);
                                }
                                $scope.icon = media.icon;
                            });

                        }
                    }

                    /**
                     * @ngdoc method
                     * @name chooseMedia
                     * @function
                     *
                     * @description
                     * Called when the select media button is pressed for the digital download section.
                     *
                     */
                    function chooseMedia() {

                        dialogService.mediaPicker({
                            onlyImages: false,
                            callback: function (media) {
                                $scope.thumbnail = '';
                                if (!media.thumbnail) {
                                    $scope.thumbnail = mediaHelper.resolveFile(media, true);
                                }
                                $scope.mediaItem = media;
                                $scope.mediaItem.umbracoFile = mediaHelper.resolveFile(media, false);
                                $scope.productVariant.downloadMediaId = media.id;
                                $scope.icon = media.icon;
                            }
                        });
                    }

                    function removeMedia() {
                        $scope.productVariant.downloadMediaId = -1;
                        $scope.mediaItem = null;
                    }

                    function initialize() {
                        init();
                    }
                }
            };
    });

    /**
     * @ngdoc controller
     * @name productVariantMainProperties
     * @function
     *
     * @description
     * The productVariantMainProperties directive
     */
    angular.module('merchello.directives').directive('productVariantMainProperties',
        [function() {

            return {
                restrict: 'E',
                replace: true,
                scope: {
                    product: '=',
                    productVariant: '=',
                    context: '=',
                    settings: '='
                },
                templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/productvariant.mainproperties.tpl.html',
                controller: function ($scope, warehouseResource, warehouseDisplayBuilder, catalogInventoryDisplayBuilder) {

                    // Get the default warehouse for the ensureCatalogInventory() function below
                    $scope.defaultWarehouse = {};
                    $scope.defaultWarehouseCatalog = {};

                    function init() {
                        var promiseWarehouse = warehouseResource.getDefaultWarehouse();
                        promiseWarehouse.then(function (warehouse) {
                            $scope.defaultWarehouse = warehouseDisplayBuilder.transform(warehouse);
                            $scope.defaultWarehouseCatalog = _.find($scope.defaultWarehouse.warehouseCatalogs, function (dwc) { return dwc.isDefault; });
                            // set defaults in case of a createproduct
                            if($scope.context === 'createproduct') {
                                $scope.productVariant.shippable = $scope.settings.globalShippable;
                                $scope.productVariant.taxable = $scope.settings.globalTaxable;
                                $scope.productVariant.trackInventory = $scope.settings.globalTrackInventory;
                                if($scope.productVariant.shippable || $scope.productVariant.trackInventory)
                                {
                                    $scope.productVariant.ensureCatalogInventory($scope.defaultWarehouseCatalog.key);
                                }
                            }
                        });
                    }

                    // Initialize the controller
                    init();
                }
            };
    }]);

    /**
     * @ngdoc controller
     * @name productVariantShipping
     * @function
     *
     * @description
     * The productVariantShipping directive
     */
    angular.module('merchello.directives').directive('productVariantShipping', function() {

        return {
            restrict: 'E',
            replace: true,
            scope: {
                product: '=',
                productVariant: '=',
                settings: '=',
                context: '@'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/productvariant.shipping.tpl.html',
            controller: 'Merchello.Directives.ProductVariantShippingDirectiveController'
        };

    });


})();