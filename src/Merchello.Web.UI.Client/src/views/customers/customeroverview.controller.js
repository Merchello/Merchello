    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.CustomerOverviewController
     * @function
     *
     * @description
     * The controller for customer overview view
     */
    angular.module('merchello').controller('Merchello.Backoffice.CustomerOverviewController',
        ['$scope', '$q', '$log', '$routeParams', '$timeout', '$filter', 'dialogService', 'notificationsService', 'localizationService', 'settingsResource', 'invoiceHelper', 'merchelloTabsFactory', 'dialogDataFactory',
            'customerResource', 'backOfficeCheckoutResource', 'customerDisplayBuilder', 'countryDisplayBuilder', 'currencyDisplayBuilder', 'settingDisplayBuilder', 'invoiceResource', 'invoiceDisplayBuilder', 'customerAddressDisplayBuilder',
            'itemCacheInstructionBuilder', 'addToItemCacheInstructionBuilder',
        function($scope, $q, $log, $routeParams, $timeout, $filter, dialogService, notificationsService, localizationService, settingsResource, invoiceHelper, merchelloTabsFactory, dialogDataFactory,
                 customerResource, backOfficeCheckoutResource, customerDisplayBuilder, countryDisplayBuilder, currencyDisplayBuilder, settingDisplayBuilder, invoiceResource, invoiceDisplayBuilder, customerAddressDisplayBuilder,
                 itemCacheInstructionBuilder, addToItemCacheInstructionBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
            $scope.avatarUrl;
            $scope.defaultShippingAddress = {};
            $scope.defaultBillingAddress = {};
            $scope.customer = {};
            $scope.invoiceTotals = [];
            $scope.settings = {};
            $scope.entityType = 'Customer';
            $scope.listViewEntityType = 'SalesHistory';

            // exposed methods
            $scope.moveToWishList = moveToWishList;
            $scope.moveToBasket = moveToBasket;
            $scope.removeFromItemCache = removeFromItemCache;
            $scope.editItemCacheItem = editItemCacheItem;
            $scope.addToItemCache = addToItemCache;

            $scope.getCurrency = getCurrency;
            $scope.openEditInfoDialog = openEditInfoDialog;
            $scope.openDeleteCustomerDialog = openDeleteCustomerDialog;
            $scope.openAddressAddEditDialog = openAddressAddEditDialog;
            $scope.saveCustomer = saveCustomer;
            $scope.deleteNote = deleteNote;

            $scope.load = load;
            $scope.getColumnValue = getColumnValue;
            $scope.invoiceDisplayBuilder = invoiceDisplayBuilder;

            // private properties
            var defaultCurrency = {};
            var countries = [];
            var currencies = [];
            const baseUrl = '#/merchello/merchello/saleoverview/';

            var paid = '';
            var unpaid = '';
            var partial = '';
            var unfulfilled = '';
            var fulfilled = '';
            var open = '';

            const label = '<i class="%0"></i> %1';

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Inititalizes the scope.
             */
            function init() {
                var key = $routeParams.id;
                loadSettings();
                localizationService.localize('merchelloSales_paid').then(function(value) {
                    paid = value;
                });
                localizationService.localize('merchelloSales_unpaid').then(function(value) {
                    unpaid = value;
                });
                localizationService.localize('merchelloSales_partial').then(function(value) {
                    partial = value;
                });
                localizationService.localize('merchelloOrder_fulfilled').then(function(value) {
                    fulfilled = value;
                });
                localizationService.localize('merchelloOrder_unfulfilled').then(function(value) {
                    unfulfilled = value;
                });
                localizationService.localize('merchelloOrder_open').then(function(value) {
                    open = value;
                });
                loadCustomer(key);

            }

            /**
             * @ngdoc method
             * @name loadCustomer
             * @function
             *
             * @description
             * Load the customer information if needed.
             */
            function loadCustomer(key) {
                var promiseLoadCustomer = customerResource.GetCustomer(key);
                promiseLoadCustomer.then(function(customerResponse) {
                    $scope.customer = customerDisplayBuilder.transform(customerResponse);
                    $scope.invoiceTotals = invoiceHelper.getTotalsByCurrencyCode($scope.customer.invoices);
                    customerResource.getGravatarUrl($scope.customer.email).then(function(url) {
                        $scope.avatarUrl = url;
                    });
                    $scope.defaultBillingAddress = $scope.customer.getDefaultBillingAddress();
                    $scope.defaultShippingAddress = $scope.customer.getDefaultShippingAddress();
                    $scope.tabs = merchelloTabsFactory.createCustomerOverviewTabs(key, $scope.customer.hasAddresses());
                    $scope.tabs.setActive('overview');
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function(reason) {
                    notificationsService.error("Failed to load customer", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadCountries
             * @function
             *
             * @description
             * Load a list of countries and provinces from the API.
             */
            function loadSettings() {
                // gets all of the countries
                var promiseCountries = settingsResource.getAllCountries();
                promiseCountries.then(function(countriesResponse) {
                    countries = countryDisplayBuilder.transform(countriesResponse);
                });

                // gets all of the settings
                var promiseSettings = settingsResource.getAllSettings();
                promiseSettings.then(function(settingsResponse) {
                    $scope.settings = settingDisplayBuilder.transform(settingsResponse);

                    // we need all of the currencies since invoices may be billed in various currencies
                    var promiseCurrencies = settingsResource.getAllCurrencies();
                    promiseCurrencies.then(function(currenciesResponse) {
                        currencies = currencyDisplayBuilder.transform(currenciesResponse);

                        // get the default currency from the settings in case we cannot determine
                        // the currency used in an invoice
                        defaultCurrency = _.find(currencies, function(c) {
                            return c.currencyCode === $scope.settings.currencyCode;
                        });
                    });
                });
            }

            function addToItemCache(items, itemCacheType) {
                var instruction = addToItemCacheInstructionBuilder.createDefault();
                instruction.customerKey = $scope.customer.key;
                instruction.items = items;
                instruction.itemCacheType = itemCacheType;
                backOfficeCheckoutResource.addItemCacheItem(instruction).then(function() {
                    loadCustomer($scope.customer.key);
                });
            }

            function removeFromItemCache(lineItem, itemCacheType) {
                var dialogData = {};
                dialogData.name = lineItem.name;
                dialogData.lineItem = lineItem;
                dialogData.itemCacheType = itemCacheType;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processRemoveFromItemCache,
                    dialogData: dialogData
                });

            }

            function processRemoveFromItemCache(dialogData) {
                var instruction = itemCacheInstructionBuilder.createDefault();
                instruction.customerKey = $scope.customer.key;
                instruction.entityKey = dialogData.lineItem.key;
                instruction.itemCacheType = dialogData.itemCacheType;
                backOfficeCheckoutResource.removeItemCacheItem(instruction).then(function() {
                    loadCustomer($scope.customer.key);
                });
            }

            function editItemCacheItem(lineItem, itemCacheType) {
                var dialogData = {};
                dialogData.name = lineItem.name;
                dialogData.lineItem = lineItem;
                dialogData.itemCacheType = itemCacheType;

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/customer.itemcachelineitem.quantity.html',
                    show: true,
                    callback: processEditItemCacheItem,
                    dialogData: dialogData
                });
            }

            function processEditItemCacheItem(dialogData) {
                var instruction = itemCacheInstructionBuilder.createDefault();
                instruction.customerKey = $scope.customer.key;
                instruction.entityKey = dialogData.lineItem.key;
                instruction.quantity = dialogData.lineItem.quantity;
                instruction.itemCacheType = dialogData.itemCacheType;
                backOfficeCheckoutResource.updateLineItemQuantity(instruction).then(function() {
                    loadCustomer($scope.customer.key);
                });

            }

            function moveToWishList(lineItem) {
                var instruction = itemCacheInstructionBuilder.createDefault();
                instruction.customerKey = $scope.customer.key;
                instruction.entityKey = lineItem.key;
                instruction.itemCacheType = 'Basket';
                backOfficeCheckoutResource.moveToWishlist(instruction).then(function() {
                    loadCustomer($scope.customer.key);
                });

            }

            function moveToBasket(lineItem) {
                var instruction = itemCacheInstructionBuilder.createDefault();
                instruction.customerKey = $scope.customer.key;
                instruction.entityKey = lineItem.key;
                instruction.itemCacheType = 'Wishlist';

                backOfficeCheckoutResource.moveToBasket(instruction).then(function() {
                    loadCustomer($scope.customer.key);
                });
            }

            function load(query) {
                var deferred = $q.defer();
                query.addCustomerKeyParam($scope.customer.key);
                invoiceResource.searchByCustomer(query).then(function(results) {
                    deferred.resolve(results);
                });
                return deferred.promise;
            }


            function getColumnValue(result, col) {
                switch(col.name) {
                    case 'invoiceNumber':
                        if (result.invoiceNumberPrefix !== '') {
                            return '<a href="' + getEditUrl(result) + '">' + result.invoiceNumberPrefix + '-' + result.invoiceNumber + '</a>';
                        } else {
                            return '<a href="' + getEditUrl(result) + '">' + result.invoiceNumber + '</a>';
                        }
                    case 'invoiceDate':
                        return $filter('date')(result.invoiceDate, $scope.settings.dateFormat);
                    case 'paymentStatus':
                        return getPaymentStatus(result);
                    case 'fulfillmentStatus':
                        return getFulfillmentStatus(result);
                    case 'total':
                        return $filter('currency')(result.total, getCurrencySymbol(result));
                    default:
                        return result[col.name];
                }
            }

            function getPaymentStatus(invoice) {
                var paymentStatus = invoice.getPaymentStatus();
                var cssClass, icon, text;
                switch(paymentStatus) {
                    case 'Paid':
                        //cssClass = 'label-success';
                        icon = 'icon-thumb-up';
                        text = paid;
                        break;
                    case 'Partial':
                        //cssClass = 'label-default';
                        icon = 'icon-handprint';
                        text = partial;
                        break;
                    default:
                        //cssClass = 'label-info';
                        icon = 'icon-thumb-down';
                        text = unpaid;
                        break;
                }
                return label.replace('%0', icon).replace('%1', text);
            }

            function getFulfillmentStatus(invoice) {
                var fulfillmentStatus = invoice.getFulfillmentStatus();
                var cssClass, icon, text;
                switch(fulfillmentStatus) {
                    case 'Fulfilled':
                        //cssClass = 'label-success';
                        icon = 'icon-truck';
                        text = fulfilled;
                        break;
                    case 'Open':
                        //cssClass = 'label-default';
                        icon = 'icon-loading';
                        text = open;
                        break;
                    default:
                        //cssClass = 'label-info';
                        icon = 'icon-box-open';
                        text = unfulfilled;
                        break;
                }
                return label.replace('%0', icon).replace('%1', text);
            }

            /**
             * @ngdoc method
             * @name getCurrencySymbol
             * @function
             *
             * @description
             * Utility method to get the currency symbol for an invoice
             */
            function getCurrencySymbol(invoice) {

                if (invoice.currency.symbol !== '') {
                    return invoice.currency.symbol;
                }

                var currencyCode = invoice.getCurrencyCode();
                var currency = _.find(currencies, function(currency) {
                    return currency.currencyCode === currencyCode;
                });
                if(currency === null || currency === undefined) {
                    return defaultCurrency;
                } else {
                    return currency.symbol;
                }
            }

            function getEditUrl(invoice) {
                return baseUrl + invoice.key;
            }

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
                if(address === null || address === undefined) {
                    dialogData.customerAddress = customerAddressDisplayBuilder.createDefault();
                    dialogData.selectedCountry = countries[0];
                } else {
                    dialogData.customerAddress = angular.extend(customerAddressDisplayBuilder.createDefault(), address);
                    dialogData.selectedCountry = address.countryCode === '' ? countries[0] :
                        _.find(countries, function(country) {
                        return country.countryCode === address.countryCode;
                    });
                }
                dialogData.countries = countries;
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
                if(!$scope.customer.hasDefaultAddressOfType(dialogData.customerAddress.addressType) || address.isDefault) {
                    dialogData.customerAddress.isDefault = true;
                    dialogData.setDefault = true;
                }
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/customer.customeraddress.addedit.html',
                    show: true,
                    callback: processAddEditAddressDialog,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name openDeleteCustomerDialog
             * @function
             *
             * @description
             * Opens the delete customer dialog via the Umbraco dialogService.
             */
            function openDeleteCustomerDialog() {
                var dialogData = dialogDataFactory.createDeleteCustomerDialogData();
                dialogData.customer = $scope.customer;
                dialogData.name = $scope.customer.firstName + ' ' + $scope.customer.lastName;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processDeleteCustomerDialog,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name openEditInfoDialog
             * @function
             *
             * @description
             * Opens the edit customer info dialog via the Umbraco dialogService.
             */
            function openEditInfoDialog() {

                var dialogData = dialogDataFactory.createAddEditCustomerDialogData();
                dialogData.firstName = $scope.customer.firstName;
                dialogData.lastName = $scope.customer.lastName;
                dialogData.email = $scope.customer.email;

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/customer.info.addedit.html',
                    show: true,
                    callback: processEditInfoDialog,
                    dialogData: dialogData
                });
            }


            /**
             * @ngdoc method
             * @name processEditAddressDialog
             * @function
             *
             * @description
             * Edit an address and update the associated lists.
             */
            function processAddEditAddressDialog(dialogData) {
                var defaultAddressOfType = $scope.customer.getDefaultAddress(dialogData.customerAddress.addressType);
                if(dialogData.customerAddress.key !== '') {
                    $scope.customer.addresses =_.reject($scope.customer.addresses, function(address) {
                      return address.key == dialogData.customerAddress.key;
                    });
                }
                if (dialogData.customerAddress.isDefault && defaultAddressOfType !== undefined) {
                    if(dialogData.customerAddress.key !== defaultAddressOfType.key) {
                        defaultAddressOfType.isDefault = false;
                    }
                }
                $scope.customer.addresses.push(dialogData.customerAddress);
                saveCustomer();
            }

            /**
             * @ngdoc method
             * @name processDeleteCustomerDialog
             * @function
             *
             * @description
             * Delete a customer.
             */
            function processDeleteCustomerDialog(dialogData) {
                notificationsService.info("Deleting " + dialogData.customer.firstName + " " + dialogData.customer.lastName, "");
                var promiseDeleteCustomer = customerResource.DeleteCustomer(dialogData.customer.key);
                promiseDeleteCustomer.then(function() {
                    notificationsService.success("Customer deleted.", "");
                    window.location.hash = "#/merchello/merchello/customerList/manage";
                }, function(reason) {
                    notificationsService.error("Customer Deletion Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name processEditInfoDialog
             * @function
             *
             * @description
             * Update the customer info and save.
             */
            function processEditInfoDialog(dialogData) {
                $scope.customer.firstName = dialogData.firstName;
                $scope.customer.lastName = dialogData.lastName;
                $scope.customer.email = dialogData.email;
                saveCustomer();
            }

            function deleteNote(note) {
                $scope.customer.notes = _.reject($scope.customer.notes, function(n) {
                    return n.key === note.key;
                });

                saveCustomer();
            }


            /**
             * @ngdoc method
             * @name saveCustomer
             * @function
             *
             * @description
             * Save the customer with the new note.
             */
            function saveCustomer() {
                $scope.preValuesLoaded = false;
                notificationsService.info(localizationService.localize("merchelloStatusNotifications_customerSaveSuccess"), "");
                var promiseSaveCustomer = customerResource.SaveCustomer($scope.customer);
                promiseSaveCustomer.then(function(customerResponse) {
                    $timeout(function() {
                    notificationsService.success(localizationService.localize("merchelloStatusNotifications_customerSaveSuccess"), "");
                        loadCustomer($scope.customer.key);
                    }, 400);

                }, function(reason) {
                    notificationsService.error(localizationService.localize("merchelloStatusNotifications_customerSaveError"), reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name getCurrencySymbol
             * @function
             *
             * @description
             * Gets the currency symbol for an invoice.
             */
            function getCurrency(currencyCode) {
                var currency = _.find(currencies, function(c) {
                    return c.currencyCode === currencyCode;
                });
                if (currency === null || currency === undefined) {
                    currency = defaultCurrency;
                }
                return currency;
            }

            // Initializes the controller
            init();
    }]);
