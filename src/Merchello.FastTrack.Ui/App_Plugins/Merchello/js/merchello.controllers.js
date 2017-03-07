/*! Merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2017 Across the Pond, LLC.
 * Licensed MIT
 */

(function() { 

/**
 * @ngdoc controller
 * @name Merchello.Backoffice.OffersListController
 * @function
 *
 * @description
 * The controller for offers list view controller
 */
angular.module('merchello').controller('Merchello.Backoffice.OfferEditController',
    ['$scope', '$routeParams', '$location', '$filter', 'merchDateHelper', 'assetsService', 'dialogService', 'eventsService', 'notificationsService', 'settingsResource', 'marketingResource', 'merchelloTabsFactory',
        'dialogDataFactory', 'settingDisplayBuilder', 'offerProviderDisplayBuilder', 'offerSettingsDisplayBuilder', 'offerComponentDefinitionDisplayBuilder',
    function($scope, $routeParams, $location, $filter, dateHelper, assetsService, dialogService, eventsService, notificationsService, settingsResource, marketingResource, merchelloTabsFactory,
             dialogDataFactory, settingDisplayBuilder, offerProviderDisplayBuilder, offerSettingsDisplayBuilder, offerComponentDefinitionDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.offerSettings = {};
        $scope.context = 'create';
        $scope.tabs = {};
        $scope.settings = {};
        $scope.offerProvider = {};
        $scope.allComponents = [];
        $scope.hasReward = false;
        $scope.lineItemName = '';

        // exposed methods
        $scope.saveOffer = saveOffer;
        $scope.toggleOfferExpires = toggleOfferExpires;
        $scope.openDeleteOfferDialog = openDeleteOfferDialog;
        $scope.toggleApplyToEachMatching = toggleApplyToEachMatching;
        $scope.setLineItemName = setLineItemName;
        var eventComponentsName = 'merchello.offercomponentcollection.changed';
        var eventOfferSavingName = 'merchello.offercoupon.saving';
        var eventOfferExpiresOpen = 'merchello.offercouponexpires.open';

        /**
         * @ngdoc method
         * @name init
         * @function
         *
         * @description
         * Initializes the controller
         */
        function init() {
            eventsService.on(eventComponentsName, onComponentCollectionChanged);
            loadSettings();
        }

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         *
         * @description
         * Loads in store settings from server into the scope.  Called in init().
         */
        function loadSettings() {
            var promiseSettings = settingsResource.getAllSettings();
            promiseSettings.then(function(settings) {
                $scope.settings = settingDisplayBuilder.transform(settings);
                loadOfferProviders();
            }, function (reason) {
                notificationsService.error("Settings Load Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name loadOfferProviders
         * @function
         *
         * @description
         * Loads the offer providers and sets the provider for this offer type
         */
        function loadOfferProviders() {
            var providersPromise = marketingResource.getOfferProviders();
            providersPromise.then(function(providers) {
                var offerProviders = offerProviderDisplayBuilder.transform(providers);
                $scope.offerProvider = _.find(offerProviders, function(provider) {
                    return provider.backOfficeTree.routeId === 'coupons';
                });
                var key = $routeParams.id;
               loadOfferComponents($scope.offerProvider.key, key);
            }, function(reason) {
                notificationsService.error("Offer providers load failed", reason.message);
            });
        }

        function loadOfferComponents(offerProviderKey, key) {

            var componentPromise = marketingResource.getAvailableOfferComponents(offerProviderKey);
            componentPromise.then(function(components) {
                $scope.allComponents = offerComponentDefinitionDisplayBuilder.transform(components);
                loadOffer(key);
            }, function(reason) {
                notificationsService.error("Failted to load offer offer components", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name loadOffer
         * @function
         *
         * @description
         * Loads in offer (in this case a coupon)
         */
        function loadOffer(key) {

            if (key === 'create' || key === '' || key === undefined) {
                $scope.context = 'create';
                $scope.offerSettings = offerSettingsDisplayBuilder.createDefault();
                setDefaultDates(new Date());
                $scope.offerSettings.dateFormat = $scope.settings.dateFormat;
                $scope.offerSettings.offerProviderKey = $scope.offerProvider.key;
                createTabs(key);
                $scope.preValuesLoaded = true;
                $scope.loaded = true;

            } else {
                $scope.context = 'existing';
                var offerSettingsPromise = marketingResource.getOfferSettings(key);
                offerSettingsPromise.then(function(settings) {

                    $scope.offerSettings = offerSettingsDisplayBuilder.transform(settings);
                    $scope.lineItemName = $scope.offerSettings.getLineItemName();
                    $scope.hasReward = $scope.offerSettings.hasRewards();
                    $scope.offerSettings.dateFormat = $scope.settings.dateFormat;
                    createTabs(key);
                    if ($scope.offerSettings.offerStartsDate === '0001-01-01' || !$scope.offerSettings.offerExpires) {
                        setDefaultDates(new Date());
                    } else {
                        $scope.offerSettings.offerStartsDate = formatDate($scope.offerSettings.offerStartsDate);
                        $scope.offerSettings.offerEndsDate = formatDate($scope.offerSettings.offerEndsDate);
                    }
                    $scope.preValuesLoaded = true;
                    $scope.loaded = true;
                }, function(reason) {
                    notificationsService.error("Failted to load offer settings", reason.message);
                });
            }
        }



        function createTabs(key) {
            $scope.tabs = merchelloTabsFactory.createMarketingTabs();
            //$scope.tabs.appendOfferTab(key, $scope.offerProvider.backOfficeTree);
            $scope.tabs.appendOfferTab(key, $scope.offerProvider.backOfficeTree);
            $scope.tabs.setActive('offer');
        }

        function toggleOfferExpires() {
            $scope.offerSettings.offerExpires = !$scope.offerSettings.offerExpires;
            if (!$scope.offerSettings.offerExpires) {
                setDefaultDates(new Date());
            } else {
                eventsService.emit(eventOfferExpiresOpen);
            }
        }


        function toggleApplyToEachMatching() {
            $scope.applyToEachMatching = !$scope.applyToEachMatching;
        }

        function setLineItemName(value) {
            $scope.offerSettings.setLineItemName(value);
        }

        function saveOffer() {

            eventsService.emit(eventOfferSavingName, $scope.offerForm);
            if($scope.offerForm.$valid) {
                var offerPromise;
                var isNew = false;
                $scope.preValuesLoaded = false;

                // validate the components
                $scope.offerSettings.validateComponents();

                // unify the date format before saving
                $scope.offerSettings.offerStartsDate = dateHelper.convertToIsoDate($scope.offerSettings.offerStartsDate, $scope.settings.dateFormat);
                $scope.offerSettings.offerEndsDate = dateHelper.convertToIsoDate($scope.offerSettings.offerEndsDate, $scope.settings.dateFormat);

                if ($scope.context === 'create' || $scope.offerSettings.key === '') {
                    isNew = true;
                    offerPromise = marketingResource.newOfferSettings($scope.offerSettings);
                } else {
                    var os = $scope.offerSettings.clone();
                    offerPromise = marketingResource.saveOfferSettings(os);
                }
                offerPromise.then(function (settings) {
                    notificationsService.success("Successfully saved the coupon.");
                    if (isNew) {
                        $location.url($scope.offerProvider.editorUrl(settings.key), true);
                    } else {
                        $scope.offerSettings = undefined;
                        loadOffer(settings.key);
                    }
                }, function (reason) {
                    notificationsService.error("Failed to save coupon", reason.message);
                });
            }
        }

        function openDeleteOfferDialog() {
            var dialogData = {};
            dialogData.name = 'Coupon with offer code: ' + $scope.offerSettings.name;
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                show: true,
                callback: processDeleteOfferConfirm,
                dialogData: dialogData
            });
        }

        function processDeleteOfferConfirm(dialogData) {
            var promiseDelete = marketingResource.deleteOfferSettings($scope.offerSettings);
            promiseDelete.then(function() {
                $location.url('/merchello/merchello/offerslist/manage', true);
            }, function(reason) {
                notificationsService.error("Failed to delete coupon", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name setDefaultDates
         * @function
         *
         * @description
         * Sets the default dates
         */
        function setDefaultDates(actual) {
            var month = actual.getMonth() + 1 == 0 ? 11 : actual.getMonth() + 1;
            var start = new Date(actual.getFullYear(), actual.getMonth(), actual.getDate());
            var end = new Date(actual.getFullYear(), month, actual.getDate());

            $scope.offerSettings.offerStartsDate = formatDate(start);
            $scope.offerSettings.offerEndsDate = formatDate(end);
        }

        function formatDate(d, format) {
            if (format === undefined) {
                format = $scope.settings.dateFormat;
            }
            return $filter('date')(d, format);
        }

        function onComponentCollectionChanged() {
            if(!$scope.offerSettings.hasRewards() || !$scope.offerSettings.componentsConfigured()) {
                $scope.offerSettings.active = false;
            }
        }

        // Initializes the controller
        init();
    }]);

/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferRewardCouponDiscountPriceController
 * @function
 *
 * @description
 * The controller to configure the discount for a coupon line item reward
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferRewardCouponDiscountPriceController',
    ['$scope', 'settingsResource', 'invoiceHelper',
        function($scope, settingsResource, invoiceHelper) {
            $scope.loaded = false;
            $scope.adjustmentType = 'flat';
            $scope.currencySymbol = '';
            $scope.amount = 0;

            // exposed methods
            $scope.save = save;

            function init() {
                loadSettings();
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description
             * Load the settings from the settings service to get the currency symbol
             */
            function loadSettings() {
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                    if ($scope.dialogData.component.isConfigured()) {
                        loadExistingConfigurations();
                    } else {
                        $scope.loaded = true;
                    }
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }

            function loadExistingConfigurations() {
                var amount = $scope.dialogData.getValue('amount');
                var adjustmentType = $scope.dialogData.getValue('adjustmentType');
                $scope.adjustmentType = adjustmentType === '' ? 'flat' : adjustmentType;
                $scope.amount = amount === '' ? 0 : invoiceHelper.round(amount, 2);
                $scope.loaded = true;
            }

            function save() {
                if ($scope.priceAdjustForm.$valid) {
                    $scope.dialogData.setValue('amount', Math.abs(invoiceHelper.round($scope.amount*1, 2)));
                    $scope.dialogData.setValue('adjustmentType', $scope.adjustmentType);
                    $scope.submit($scope.dialogData);
                }
            }

            // Initialize
            init();
        }]);


/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferProviderSelectionController
 * @function
 *
 * @description
 * The controller to handle offer provider selection
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.NewOfferProviderSelectionController',
    ['$scope', '$location', 'navigationService', 'marketingResource', 'offerProviderDisplayBuilder',
    function($scope, $location, navigationService, marketingResource, offerProviderDisplayBuilder) {
        
        $scope.loaded = false;
        $scope.offerProviders = [];

        // exposed methods
        $scope.setSelection = setSelection;

        function init() {
            loadOfferProviders();
        }

        function loadOfferProviders() {
            var providersPromise = marketingResource.getOfferProviders();
            providersPromise.then(function(providers) {
                $scope.offerProviders = offerProviderDisplayBuilder.transform(providers);
                $scope.loaded = true;
            }, function(reason) {
                notificationsService.error("Offer providers load failed", reason.message);
            });
        }

        function setSelection(selectedProvider) {
            navigationService.hideNavigation();
            var view = selectedProvider.backOfficeTree.routePath.replace('{0}', 'create');
            $location.url(view, true);
        }

        // initialize the controller
        init();
}]);

/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferConstraintCollectionPriceRulesController
 * @function
 *
 * @description
 * The controller to configure the collection price component
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferConstraintCollectionPriceRulesController',
    ['$scope', 'notificationsService', 'settingsResource', 'invoiceHelper',
        function($scope, notificationsService, settingsResource, invoiceHelper) {

            $scope.loaded = false;
            $scope.operator = 'gt';
            $scope.price = 0;
            $scope.currencySymbol = '';

            // exposed methods
            $scope.save = save;

            function init() {
                loadSettings();
                loadExistingConfigurations();
            }

            function loadExistingConfigurations() {
                var operator = $scope.dialogData.getValue('operator');
                var price = $scope.dialogData.getValue('price');
                $scope.operator = operator === '' ? 'gt' : operator;
                $scope.price = price === '' ? 0 : invoiceHelper.round(price, 2);
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description
             * Load the settings from the settings service to get the currency symbol
             */
            function loadSettings() {
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                    $scope.loaded = true;
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Saves the configuration
             */
            function save() {
                $scope.dialogData.setValue('price', Math.abs(invoiceHelper.round($scope.price*1, 2)));
                $scope.dialogData.setValue('operator', $scope.operator);
                $scope.submit($scope.dialogData);
            }

            // Initialize the controller
            init();
        }]);

/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferConstraintCollectionQuantityRulesController
 * @function
 *
 * @description
 * The controller to configure the collection quantity constraint
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferConstraintCollectionQuantityRulesController',
    ['$scope',
    function($scope) {
        $scope.loaded = false;

        $scope.operator = 'gt';
        $scope.quantity = 0;

        // exposed methods
        $scope.save = save;

        function init() {
            if ($scope.dialogData.component.isConfigured()) {
                loadExistingConfigurations()
            } else {
                $scope.loaded = true;
            }

        }

        function loadExistingConfigurations() {
            var operator = $scope.dialogData.getValue('operator');
            var quantity = $scope.dialogData.getValue('quantity');
            $scope.operator = operator === '' ? 'gt' : operator;
            $scope.quantity = quantity === '' ? 0 : quantity * 1;
            $scope.loaded = true;
        }

        /**
         * @ngdoc method
         * @name save
         * @function
         *
         * @description
         * Saves the configuration
         */
        function save() {
            $scope.dialogData.setValue('quantity', Math.abs($scope.quantity*1));
            $scope.dialogData.setValue('operator', $scope.operator);
            $scope.submit($scope.dialogData);
        }

        // Initialize the controller
        init();
    }]);

/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferConstraintPriceController
 * @function
 *
 * @description
 * The controller to configure the price component constraint
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferConstraintFilterPriceRulesController',
    ['$scope', 'settingsResource', 'invoiceHelper',
        function($scope, settingsResource, invoiceHelper) {

            $scope.loaded = false;
            $scope.operator = 'gt';
            $scope.price = 0;
            $scope.currencySymbol = '';

            // exposed methods
            $scope.save = save;

            function init() {
                loadSettings();
                loadExistingConfigurations()
            }

            function loadExistingConfigurations() {
                var operator = $scope.dialogData.getValue('operator');
                var price = $scope.dialogData.getValue('price');
                $scope.operator = operator === '' ? 'gt' : operator;
                $scope.price = price === '' ? 0 : invoiceHelper.round(price, 2);
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description
             * Load the settings from the settings service to get the currency symbol
             */
            function loadSettings() {
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                    $scope.loaded = true;
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Saves the configuration
             */
            function save() {
                $scope.dialogData.setValue('price', Math.abs(invoiceHelper.round($scope.price*1, 2)));
                $scope.dialogData.setValue('operator', $scope.operator);
                $scope.submit($scope.dialogData);
            }

            // Initialize the controller
            init();
        }]);

/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferConstraintFilterQuantityRulesController
 * @function
 *
 * @description
 * The controller to configure the line item quantity component constraint
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferConstraintFilterQuantityRulesController',
    ['$scope',
    function($scope) {
        $scope.loaded = false;

        $scope.operator = 'gt';
        $scope.quantity = 0;

        // exposed methods
        $scope.save = save;

        function init() {
            if ($scope.dialogData.component.isConfigured()) {
                loadExistingConfigurations()
            } else {
                $scope.loaded = true;
            }

        }

        function loadExistingConfigurations() {
            var operator = $scope.dialogData.getValue('operator');
            var quantity = $scope.dialogData.getValue('quantity');
            $scope.operator = operator === '' ? 'gt' : operator;
            $scope.quantity = quantity === '' ? 0 : quantity * 1;
            $scope.loaded = true;
        }

        /**
         * @ngdoc method
         * @name save
         * @function
         *
         * @description
         * Saves the configuration
         */
        function save() {
            $scope.dialogData.setValue('quantity', Math.abs($scope.quantity*1));
            $scope.dialogData.setValue('operator', $scope.operator);
            $scope.submit($scope.dialogData);
        }

        // Initialize the controller
        init();
    }]);

/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferConstraintMaximumQuantityController
 * @function
 *
 * @description
 * The controller to configure the line item quantity component constraint
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferConstraintMaximumQuantityController',
    ['$scope',
    function($scope) {

    $scope.loaded = false;
    $scope.maximum = 1;

    // exposed
    $scope.save = save;

    function init() {
        if ($scope.dialogData.component.isConfigured()) {
            loadExistingConfigurations();
            $scope.loaded = true;
        } else {
            $scope.loaded = true;
        }
    }

    function loadExistingConfigurations() {
        var maximum = $scope.dialogData.getValue('maximum')
        $scope.maximum = maximum === '' ? 1 : maximum * 1;
    }

    function save() {
        $scope.dialogData.setValue('maximum', $scope.maximum);
        $scope.submit($scope.dialogData);
    }

    // Initialize the controller
    init();
}]);
/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferConstraintPriceController
 * @function
 *
 * @description
 * The controller to configure the price component constraint
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferConstraintProductSelectionFilterController',
    ['$q', '$scope', 'notificationsService', 'productResource', 'settingsResource', 'productDisplayBuilder', 'queryDisplayBuilder', 'queryResultDisplayBuilder',
        function($q, $scope, notificationsService, productResource, settingsResource, productDisplayBuilder, queryDisplayBuilder, queryResultDisplayBuilder) {

            $scope.loaded = false;
            $scope.context = 'display';
            $scope.filterText = "";
            $scope.products = [];
            $scope.filteredproducts = [];
            $scope.watchCount = 0;
            $scope.sortProperty = "name";
            $scope.sortOrder = "Ascending";
            $scope.limitAmount = 10;
            $scope.currentPage = 0;
            $scope.maxPages = 0;

            // dialog properties
            $scope.selectedProducts = [];

            // exposed methods
            $scope.addProduct = addProduct;
            $scope.removeProduct = removeProduct;
            $scope.changePage = changePage;
            $scope.limitChanged = limitChanged;
            $scope.changeSortOrder = changeSortOrder;
            $scope.getFilteredProducts = getFilteredProducts;
            $scope.numberOfPages = numberOfPages;
            $scope.productIsSelected = productIsSelected;
            $scope.save = save;

            //--------------------------------------------------------------------------------------
            // Initialization methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                loadSettings();
            }

            /**
             * @ngdoc method
             * @name loadProducts
             * @function
             *
             * @description
             * Load the products from the product service, then wrap the results
             * in Merchello models and add to the scope via the products collection.
             */
            function loadProducts() {

                var page = $scope.currentPage;
                var perPage = $scope.limitAmount;
                var sortBy = $scope.sortProperty.replace("-", "");
                var sortDirection = $scope.sortOrder;

                var query = queryDisplayBuilder.createDefault();
                query.currentPage = page;
                query.itemsPerPage = perPage;
                query.sortBy = sortBy;
                query.sortDirection = sortDirection;
                query.addFilterTermParam($scope.filterText);

                var promise = productResource.searchProducts(query);
                promise.then(function (response) {
                    var queryResult = queryResultDisplayBuilder.transform(response, productDisplayBuilder);

                    $scope.products = queryResult.items;

                    $scope.maxPages = queryResult.totalPages;
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;

                }, function (reason) {
                    notificationsService.success("Products Load Failed:", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description
             * Load the settings from the settings service to get the currency symbol
             */
            function loadSettings() {
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    $scope.currencySymbol = currencySymbol;

                    loadExistingConfigurations();
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }

            function loadExistingConfigurations() {
                var existing = $scope.dialogData.getValue('productConstraints');
                if (existing !== undefined && existing !== '')
                {
                    var parsed = JSON.parse(existing);
                    var productKeys = _.pluck(parsed, 'productKey');

                    var    productsPromise = productResource.getByKeys(productKeys);
                    productsPromise.then(function(result) {
                     var products = productDisplayBuilder.transform(result);
                        angular.forEach(products, function(p) {
                            var constrainData = _.find(parsed, function(cd) { return cd.productKey === p.key; });
                            if(constrainData.specifiedVariants) {
                                addProduct(p, constrainData.variantKeys);
                            } else {
                                addProduct(p);
                            }
                        });
                     loadProducts();
                    });
                } else {
                    loadProducts();
                }

            }

            //--------------------------------------------------------------------------------------
            // Events methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name limitChanged
             * @function
             *
             * @description
             * Helper function to set the amount of items to show per page for the paging filters and calculations
             */
            function limitChanged(newVal) {
                $scope.limitAmount = newVal;
                $scope.currentPage = 0;
                loadProducts();
            }

            /**
             * @ngdoc method
             * @name changePage
             * @function
             *
             * @description
             * Helper function re-search the products after the page has changed
             */
            function changePage (newPage) {
                $scope.currentPage = newPage;
                loadProducts();
            }

            /**
             * @ngdoc method
             * @name changeSortOrder
             * @function
             *
             * @description
             * Helper function to set the current sort on the table and switch the
             * direction if the property is already the current sort column.
             */
            function changeSortOrder(propertyToSort) {

                if ($scope.sortProperty == propertyToSort) {
                    if ($scope.sortOrder == "Ascending") {
                        $scope.sortProperty = "-" + propertyToSort;
                        $scope.sortOrder = "Descending";
                    } else {
                        $scope.sortProperty = propertyToSort;
                        $scope.sortOrder = "Ascending";
                    }
                } else {
                    $scope.sortProperty = propertyToSort;
                    $scope.sortOrder = "Ascending";
                }

                loadProducts();
            }

            /**
             * @ngdoc method
             * @name getFilteredProducts
             * @function
             *
             * @description
             * Calls the product service to search for products via a string search
             * param.  This searches the Examine index in the core.
             */
            function getFilteredProducts(filter) {
                $scope.filterText = filter;
                $scope.currentPage = 0;
                loadProducts();
            }

            function addProduct(product, variantKeys) {
                var pc = new ProductConstraint();
                pc.product = product;
                if (product.hasVariants()) {
                    angular.forEach(product.productVariants, function(pv) {
                        var checked = true;
                        if (variantKeys !== undefined) {
                            var found = _.find(variantKeys, function(key) { return key === pv.key; });
                            if (found) {
                                checked = true;
                            } else {
                                checked = false;
                            }
                        }
                      var vc = new VariantConstraint();
                        vc.key = pv.key;
                        vc.name = pv.name;
                        vc.sku = pv.sku;
                        vc.checked = checked;
                        pc.selectedVariants.push(vc);
                    });
                }
                $scope.selectedProducts.push(pc);
                $scope.context = 'display';
            }

            function removeProduct(constraint) {
                $scope.selectedProducts = _.reject($scope.selectedProducts, function(sp) { return sp.product.key === constraint.product.key; });
            }

            function productIsSelected(product) {
                var pc = _.find($scope.selectedProducts, function(p) { return p.product.key === product.key; });
                return pc !== undefined;
            }

            //--------------------------------------------------------------------------------------
            // Calculations
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name numberOfPages
             * @function
             *
             * @description
             * Helper function to get the amount of items to show per page for the paging
             */
            function numberOfPages() {
                return $scope.maxPages;
            }

            // ---------------------------------------------------------------------------------------
            // Local scope models
            // ---------------------------------------------------------------------------------------
            var ProductConstraint = function() {
                var self = this;
                self.product = {};
                self.variantSpecific = false;
                self.selectedVariants = [];
                self.exclude = false;
                self.editorOpen = false;
            };

            var VariantConstraint = function() {
                var self = this;
                self.name = '';
                self.key = '';
                self.sku = '';
                self.checked = false;
            };


            function save() {
                if ($scope.selectedProducts.length === 0) {
                    return;
                }
                var saveData = [];
                angular.forEach($scope.selectedProducts, function(sp) {
                    var product = {};
                    product.productKey = sp.product.key;
                    product.variantKeys = [];
                    var variants = _.filter(sp.selectedVariants, function(sv) { return sv.checked; });
                    if (variants.length !== sp.product.productVariants.length) {
                        product.specifiedVariants = true;
                        angular.forEach(variants, function(v) {
                            product.variantKeys.push(v.key);
                        });
                    } else {
                        product.specifiedVariants = false;
                    }

                    saveData.push(product);
                });
                $scope.dialogData.setValue('productConstraints', JSON.stringify(saveData));
                $scope.submit($scope.dialogData);
            }

            // Initialize the controller
            init();

        }]);


/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferConstraintRedemptionLimitController
 * @function
 *
 * @description
 * The controller to configure the maximum number of redemptions allowed.
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferConstraintRedemptionLimitController',
    ['$scope',
        function($scope) {

            $scope.loaded = false;
            $scope.maximum = 0;

            // exposed
            $scope.save = save;

            function init() {
                if ($scope.dialogData.component.isConfigured()) {
                    loadExistingConfigurations();
                    $scope.loaded = true;
                } else {
                    $scope.loaded = true;
                }
            }

            function loadExistingConfigurations() {
                var maximum = $scope.dialogData.getValue('maximum')
                $scope.maximum = maximum === '' ? 0 : maximum * 1;
            }

            function save() {
                $scope.dialogData.setValue('maximum', $scope.maximum);
                $scope.submit($scope.dialogData);
            }

            // Initialize the controller
            init();
        }]);

/**
 * @ngdoc controller
 * @name Merchello.Marketing.Dialogs.OfferProviderSelectionController
 * @function
 *
 * @description
 * The controller to handle offer provider selection
 */
angular.module('merchello').controller('Merchello.Marketing.Dialogs.OfferProviderSelectionController',
    ['$scope', function($scope) {
        $scope.loaded = true;

        $scope.setSelection = function(provider) {
            if (provider === undefined) {
                return;
            }
            $scope.dialogData.selectedProvider = provider;
            $scope.submit($scope.dialogData);
        };

}]);

/**
 * @ngdoc controller
 * @name Merchello.Directives.OfferComponentsDirectiveController
 * @function
 *
 * @description
 * The controller to handle offer component association and configuration
 */
angular.module('merchello').controller('Merchello.Directives.OfferComponentsDirectiveController',
    ['$scope', '$timeout', '$filter', 'notificationsService', 'dialogService', 'eventsService', 'dialogDataFactory', 'marketingResource', 'settingsResource', 'offerComponentDefinitionDisplayBuilder',
    function($scope, $timeout, $filter, notificationsService, dialogService, eventsService, dialogDataFactory, marketingResource, settingsResource, offerComponentDefinitionDisplayBuilder) {

        $scope.componentsLoaded = false;
        $scope.availableComponents = [];
        $scope.assignedComponents = [];
        $scope.partition = [];
        $scope.currencySymbol = '';
        $scope.sortComponent = {};

        // exposed components methods
        $scope.assignComponent = assignComponent;
        $scope.removeComponentOpen = removeComponentOpen;
        $scope.configureComponentOpen = configureComponentOpen;
        $scope.isComponentConfigured = isComponentConfigured;
        $scope.applyDisplayConfigurationFormat = applyDisplayConfigurationFormat;


        var eventName = 'merchello.offercomponentcollection.changed';

        /**
         * @ngdoc method
         * @name init
         * @function
         *
         * @description
         * Initializes the controller
         */
        function init() {
            eventsService.on(eventName, onComponentCollectionChanged);

            // ensure that the parent scope promises have been resolved
            $scope.$watch('preValuesLoaded', function(pvl) {
                if(pvl === true) {
                   loadSettings();
                }
            });

            // if these are constraints, enable the sort
            if ($scope.componentType === 'Constraint') {
                $scope.sortableOptions.disabled = false;
            }
        }

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         *
         * @description
         * Load the settings from the settings service to get the currency symbol
         */
        function loadSettings() {
            var currencySymbolPromise = settingsResource.getCurrencySymbol();
            currencySymbolPromise.then(function (currencySymbol) {
                $scope.currencySymbol = currencySymbol;

                loadComponents();
            }, function (reason) {
                notificationsService.error("Settings Load Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name loadComponents
         * @function
         *
         * @description
         * Loads the components for this offer
         */
        function loadComponents() {
            // either assigned constraints or rewards
            $scope.assignedComponents = _.filter($scope.offerSettings.componentDefinitions, function(osc) { return osc.componentType === $scope.componentType; });

            var typeGrouping = $scope.offerSettings.getComponentsTypeGrouping();

            // there can only be one reward.
            if ($scope.componentType === 'Reward' && $scope.offerSettings.hasRewards()) {
                $scope.availableComponents = [];
                $scope.componentsLoaded = true;
                return;
            }

            $scope.availableComponents = _.filter($scope.components, function(c) {
                var ac = _.find($scope.assignedComponents, function(ac) { return ac.componentKey === c.componentKey; });
                if (ac === undefined && c.componentType === $scope.componentType && (typeGrouping === '' | typeGrouping === c.typeGrouping)) {
                    return c;
                }
            });

            $scope.componentsLoaded = true;
        }

        function applyDisplayConfigurationFormat(component) {
            if(component.displayConfigurationFormat !== undefined && component.displayConfigurationFormat !== '') {
                var value = eval(component.displayConfigurationFormat);
                if (value === undefined) {
                    return '';
                } else {
                    return value;
                }
            }
        }

        /**
         * @ngdoc method
         * @name assignComponent
         * @function
         *
         * @description
         * Adds a component from the offer
         */
        function assignComponent(component) {
            if($scope.offerSettings.assignComponent(component))
            {
                if ($scope.componentType === 'Reward') {
                    $scope.$parent.hasReward = true;
                }
                eventsService.emit(eventName);
            }
        }

        /**
         * @ngdoc method
         * @name configureComponentOpen
         * @function
         *
         * @description
         * Opens the component configuration dialog
         */
        function configureComponentOpen(component) {
            var dialogData = dialogDataFactory.createConfigureOfferComponentDialogData();
            dialogData.component = component.clone();

            dialogService.open({
                template: component.dialogEditorView.editorView,
                show: true,
                callback: processConfigureComponent,
                dialogData: dialogData
            });

        }

        function processConfigureComponent(dialogData) {
            $scope.offerSettings.updateAssignedComponent(dialogData.component);
            saveOffer();
            var component = _.find($scope.offerSettings.componentDefinitions, function(cd) { return cd.key === dialogData.component.key; } );
            component.updated = false;
        }

        /**
         * @ngdoc method
         * @name removeComponentOpen
         * @function
         *
         * @description
         * Opens the confirm dialog to a component from the offer
         */
        function removeComponentOpen(component) {
                var dialogData = {};
                dialogData.name = 'Component: ' + component.name;
                dialogData.componentKey = component.componentKey;
                if(!component.extendedData.isEmpty()) {
                    dialogData.warning = 'This will any delete any configurations for this component if saved.';
                }

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processRemoveComponent,
                    dialogData: dialogData
                });
        }

        /**
         * @ngdoc method
         * @name processRemoveComponent
         * @function
         *
         * @description
         * Removes a component from the offer
         */
        function processRemoveComponent(dialogData) {
            $scope.offerSettings.componentDefinitions = _.reject($scope.offerSettings.componentDefinitions, function(cd) { return cd.componentKey === dialogData.componentKey; })
            eventsService.emit(eventName);
        };

        function isComponentConfigured(component) {
            if(!component.updated) {
                return component.isConfigured();
            }
        }

        function onComponentCollectionChanged() {
            eventsService.unsubscribe(loadComponents);
        }

        function saveOffer() {
            $timeout(function() {
                $scope.saveOfferSettings();
            }, 500);
        }

        // Sortable available offers
        /// -------------------------------------------------------------------

        $scope.sortableOptions = {
            start : function(e, ui) {
               ui.item.data('start', ui.item.index());
            },
           stop: function (e, ui) {
               var component = ui.item.scope().component;
               var start = ui.item.data('start'),
                   end =  ui.item.index();
               // reorder the offerSettings.componentDefinitions
               if ($scope.offerSettings.hasRewards()) {
                   // the reward is always in position 0
                   start++;
                   end++;
               }
               $scope.offerSettings.reorderComponent(start, end);
            },
            disabled: true,
            cursor: "move"
        }

        // Initialize the controller
        init();
    }]);
/**
 * @ngdoc controller
 * @name Merchello.Backoffice.OffersListController
 * @function
 *
 * @description
 * The controller for offers list view controller
 */
angular.module('merchello').controller('Merchello.Backoffice.OffersListController',
    ['$scope', '$q', '$location', '$filter', 'notificationsService', 'localizationService', 'settingsResource', 'marketingResource', 'merchelloTabsFactory',
        'settingDisplayBuilder', 'offerProviderDisplayBuilder', 'offerSettingsDisplayBuilder',
    function($scope, $q, $location, $filter, notificationsService, localizationService, settingsResource, marketingResource, merchelloTabsFactory,
             settingDisplayBuilder, offerProviderDisplayBuilder, offerSettingsDisplayBuilder) {

        $scope.offerSettingsDisplayBuilder = offerSettingsDisplayBuilder;

        $scope.loaded = true;
        $scope.preValuesLoaded = true;
        $scope.tabs = [];

        $scope.settings = {};
        $scope.offerProviders = [];
        $scope.includeInactive = false;
        $scope.currencySymbol = '';

        $scope.entityType = 'Offer';

        // exposed methods
        $scope.load = load;
        $scope.getColumnValue = getColumnValue;

        var yes = '';
        var no = '';
        var expired = '';

        function init() {
            $scope.tabs = merchelloTabsFactory.createMarketingTabs();
            $scope.tabs.setActive('offers');

            var deferred = $q.defer();
            var promises = [
                settingsResource.getAllCombined(),
                localizationService.localize('general_yes'),
                localizationService.localize('general_no'),
                localizationService.localize('merchelloGeneral_expired'),
                marketingResource.getOfferProviders()
            ];

            $q.all(promises).then(function(data) {
                deferred.resolve(data);
            });

            deferred.promise.then(function(results) {
                $scope.settings = results[0].settings;
                $scope.currencySymbol = results[0].currencySymbol;
                yes = results[1];
                no = results[2];
                expired = results[3];
                $scope.offerProviders = offerProviderDisplayBuilder.transform(results[4]);
                $scope.preValuesLoaded = true;
            }, function(reason) {
                notificationsService.error("Failed to load promise queue", reason.message);
            });
        }

        function load(query) {
            return marketingResource.searchOffers(query);
        }

        function getColumnValue(result, col) {
            switch(col.name) {
                case 'name':
                    return '<a href="' + getEditUrl(result) + '">' + result.name + '</a>';
                case 'offerType':
                    return  getOfferType(result);
                case 'rewards':
                    return getOfferReward(result).trim();
                case 'offerStartDate':
                    return result.offerExpires ? $filter('date')(result.offerStartsDate, $scope.settings.dateFormat) : '-';
                case 'offerEndDate':
                    return result.offerExpires ? $filter('date')(result.offerEndsDate, $scope.settings.dateFormat) : '-';
                case 'active':
                    if(result.active && !result.expired) {
                        return yes;
                    }
                    if(!result.active) {
                        return no;
                    }
                    return expired;
                default:
                    return result[col.name];
            }
        }

        function getOfferReward(offerSettings) {
            if (offerSettings.hasRewards()) {
                var reward = offerSettings.getReward();
                if (reward.isConfigured()) {
                    return eval(reward.displayConfigurationFormat);
                } else {
                    return 'Not configured';
                }
            } else {
                return '-';
            }
        }

        function getEditUrl(offer) {
            var url = '#';
            var provider = _.find($scope.offerProviders, function(p) { return p.key === offer.offerProviderKey; });
            if (provider === null || provider === undefined) {
                return url;
            }
            return url + '/' + provider.editorUrl(offer.key);
        }

        function getOfferType(offer) {
            var provider = _.find($scope.offerProviders, function(p) { return p.key === offer.offerProviderKey; });
            if (provider === null || provider === undefined) {
                return 'could not find';
            }
            return provider.backOfficeTree.title;
        }

        // Initialize the controller
        init();
    }]);
angular.module('merchello').controller('Merchello.Backoffice.CollectionProviderListController',
    ['$scope', 'assetsService',
        function($scope, assetsService) {

            $scope.loaded = true;
            $scope.preValuesLoaded = true;

        }]);

/**
 * @ngdoc controller
 * @name Merchello.Common.Dialogs.CreateStaticCollectionController
 * @function
 *
 * @description
 * The controller for the delete confirmations
 */
angular.module('merchello')
    .controller('Merchello.EntityCollections.Dialogs.CreateStaticCollectionController',
    ['$scope', 'appState', 'treeService', 'notificationsService', 'navigationService', 'entityCollectionHelper', 'entityCollectionResource', 'entityCollectionProviderDisplayBuilder', 'entityCollectionDisplayBuilder',
        function($scope, appState, treeService, notificationsService, navigationService, entityCollectionHelper, entityCollectionResource, entityCollectionProviderDisplayBuilder, entityCollectionDisplayBuilder) {

            $scope.loaded = false;
            $scope.name = '';
            $scope.wasFormSubmitted = false;
            $scope.entityType = '';
            $scope.provider = {};
            $scope.dialogData = {};
            $scope.entityCollectionProviders = [];


            // exposed methods
            $scope.save = save;

            function init() {
                $scope.dialogData = $scope.$parent.currentAction.metaData.dialogData;
                $scope.entityType = entityCollectionHelper.getEntityTypeByTreeId($scope.dialogData.entityType);
                loadProviders();
            }

            function loadProviders() {
                var promise = entityCollectionResource.getDefaultEntityCollectionProviders();
                promise.then(function(results) {
                    $scope.entityCollectionProviders = entityCollectionProviderDisplayBuilder.transform(results);
                    $scope.provider = _.find($scope.entityCollectionProviders, function(p) { return p.entityType == $scope.entityType; });

                    // todo this needs to be handled better
                    if ($scope.provider == null || $scope.provider == undefined) {
                        navigationService.hideNavigation();
                    }

                    $scope.loaded = true;
                }, function(reason) {
                    notificationsService.error("Failted to load default providers", reason.message);
                });
            }

            function save() {
                $scope.wasFormSubmitted = true;
                if ($scope.collectionForm.name.$valid) {
                    var collection = entityCollectionDisplayBuilder.createDefault();
                    collection.providerKey = $scope.provider.key;
                    collection.entityTfKey = $scope.provider.entityTfKey;
                    collection.entityType = $scope.provider.entityType;
                    collection.parentKey = $scope.dialogData.parentKey;
                    collection.name = $scope.name;
                    var promise = entityCollectionResource.addEntityCollection(collection);
                    promise.then(function() {
                        navigationService.hideNavigation();

                        var reloadNodePromise = treeService.reloadNode($scope.currentNode);
                        reloadNodePromise.then(function() {
                            var promise = treeService.loadNodeChildren({ node: $scope.currentNode });
                            promise.then(function() {
                                notificationsService.success('New collection added.');
                            });
                        });

                    });
                }
            }

            init();
    }]);


angular.module('merchello').controller('Merchello.EntityCollections.Dialogs.DeleteEntityCollectionController', [
    '$scope', '$location', 'treeService', 'navigationService', 'notificationsService', 'entityCollectionResource', 'entityCollectionDisplayBuilder',
    function($scope, $location, treeService, navigationService, notificationsService, entityCollectionResource, entityCollectionDisplayBuilder) {

        $scope.loaded = false;
        $scope.dialogData = {};
        $scope.entityCollection = {};
        $scope.refreshPath = {};
        $scope.confirmDelete = confirmDelete;

        function init() {
            $scope.dialogData = $scope.$parent.currentAction.metaData.dialogData;
            $scope.refreshPath = treeService.getPath($scope.$parent.currentNode);
            loadEntityCollection();
        }

        function loadEntityCollection() {
            var promise = entityCollectionResource.getByKey($scope.dialogData.collectionKey);
            promise.then(function(collection) {
                $scope.entityCollection = entityCollectionDisplayBuilder.transform(collection);
                $scope.dialogData.name = $scope.entityCollection.name;
                $scope.loaded = true;
            }, function(reason) {
                notificationsService.error("Failted to entity collection", reason.message);
            });
        }

        function confirmDelete() {
            var promise = entityCollectionResource.deleteEntityCollection($scope.dialogData.collectionKey);
            promise.then(function(){
                navigationService.hideNavigation();
                treeService.removeNode($scope.currentNode);
                notificationsService.success('Collection deleted');
            }, function(reason) {
                notificationsService.error("Failted to delete entity collection", reason.message);
            });
        }

        // initialize the controller
        init();
    }]);

angular.module('merchello').controller('Merchello.EntityCollections.Dialogs.SpecFilterCollectionAddController',
['$scope',
    function($scope) {

    $scope.wasFormSubmitted = false;

    $scope.save = function() {
        $scope.wasFormSubmitted = true;
        if ($scope.collectionForm.name.$valid) {
            $scope.submit($scope.dialogData)
        }
    }

}]);

angular.module('merchello').controller('Merchello.EntityCollections.Dialogs.FilterGroupAddEditFilterController',
    ['$scope', 'entityCollectionDisplayBuilder',
    function($scope, entityCollectionDisplayBuilder) {

        $scope.loaded = true;

        $scope.filterName = '';
        $scope.wasFormSubmitted = false;

        $scope.save = function() {
            $scope.wasFormSubmitted = true;
            if ($scope.collectionForm.name.$valid) {
                $scope.submit($scope.dialogData);
            }
        }

        $scope.addFilter = function() {
            // first we need to clone the template so it can be reused (not modified)
            var filter = angular.extend(entityCollectionDisplayBuilder.createDefault(), $scope.dialogData.filterTemplate);
            filter.name = $scope.filterName;

            var exists = _.find($scope.dialogData.filterGroup.filters, function(c) { return c.name === filter.name; });
            if (!exists) {
                filter.sortOrder = $scope.dialogData.filterGroup.filters.length === 0 ?
                    0 :
                    $scope.dialogData.filterGroup.filters.length;

                $scope.dialogData.filterGroup.filters.push(filter);
            }

            $scope.filterName = '';

        }

        // removes a choice from the dialog data collection of choices
        $scope.remove = function(idx) {
            if ($scope.dialogData.filterGroup.filters.length > idx) {
                var remover = $scope.dialogData.filterGroup.filters[idx];
                var sort = remover.sortOrder;
                $scope.dialogData.filterGroup.filters.splice(idx, 1);
                _.each($scope.dialogData.filterGroup.filters, function(c) {
                    if (c.sortOrder > sort) {
                        c.sortOrder -= 1;
                    }
                });
            }
        };

        $scope.sortableFilters = {
            start : function(e, ui) {
                ui.item.data('start', ui.item.index());
            },
            stop: function (e, ui) {
                var start = ui.item.data('start'),
                    end =  ui.item.index();
                for(var i = 0; i < $scope.dialogData.filterGroup.filters.length; i++) {
                    $scope.dialogData.filterGroup.filters[i].sortOrder = i + 1;
                }
            },
            disabled: false,
            cursor: "move"
        }
}]);


angular.module('merchello').controller('Merchello.EntityCollections.Dialogs.ManageStaticCollectionController',
    ['$scope',  'treeService', 'notificationsService', 'navigationService', 'assetsService', 'eventsService', 'entityCollectionHelper', 'merchelloTabsFactory',
        'settingsResource', 'entityCollectionResource', 'settingDisplayBuilder', 'productDisplayBuilder', 'invoiceDisplayBuilder', 'customerDisplayBuilder',
        'queryDisplayBuilder', 'queryResultDisplayBuilder', 'entityCollectionDisplayBuilder',
    function($scope, treeService, notificationsService, navigationService, assetsService, eventsService, entityCollectionHelper, merchelloTabsFactory,
        settingsResource, entityCollectionResource, settingDisplayBuilder, productDisplayBuilder, invoiceDisplayBuilder, customerDisplayBuilder,
        queryDisplayBuilder, queryResultDisplayBuilder, entityCollectionDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.addToCollection = true;
        $scope.wasFormSubmitted = false;
        $scope.collectionKey = '';
        $scope.entityType = '';
        $scope.dialogData = {};
        $scope.settings = {};
        $scope.entityCount = 0;
        $scope.collection = {};
        $scope.editCollection = false;

        $scope.sortProperty = '';
        $scope.sortOrder = 'Ascending';
        $scope.filterText = '';
        $scope.limitAmount = 5;
        $scope.currentPage = 0;
        $scope.maxPages = 0;
        $scope.invoices = [];
        $scope.products = [];
        $scope.customers = [];

        // exposed methods
        $scope.changePage = changePage;
        $scope.limitChanged = limitChanged;
        $scope.changeSortOrder = changeSortOrder;
        $scope.getFilteredEntities = getFilteredEntities;
        $scope.numberOfPages = numberOfPages;
        $scope.toggleMode = toggleMode;
        $scope.handleEntity = handleEntity;
        $scope.saveCollection = saveCollection;

        var collectionChanged = "merchello.collection.changed";

        function init() {
            var cssPromise = assetsService.loadCss('/App_Plugins/Merchello/assets/css/merchello.css');
            cssPromise.then(function() {
                $scope.dialogData = $scope.$parent.currentAction.metaData.dialogData;
                $scope.collectionKey = $scope.dialogData.collectionKey;
                $scope.entityType = entityCollectionHelper.getEntityTypeByTreeId($scope.dialogData.entityType);
                loadSettings();
            });
        }

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         *
         * @description
         * Load the settings from the settings service to get the currency symbol
         */
        function loadSettings() {
            // this is needed for the date format
            var settingsPromise = settingsResource.getAllSettings();
            settingsPromise.then(function(allSettings) {
                $scope.settings = settingDisplayBuilder.transform(allSettings);
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                    loadCollection();
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }, function(reason) {
                notificationService.error('Failed to load all settings', reason.message);
            });
        }

        function loadCollection() {
            var promise = entityCollectionResource.getByKey($scope.collectionKey);
            promise.then(function(collection) {
                $scope.collection = entityCollectionDisplayBuilder.transform(collection);
                loadEntities();
            }, function(reason) {
                notificationsService.error('Failed to load the collection ' + reason);
            });
        }

        function loadEntities() {
            $scope.preValuesLoaded = false;
            var page = $scope.currentPage;
            var perPage = $scope.limitAmount;
            var sortBy = $scope.sortProperty.replace("-", "");
            var sortDirection = $scope.sortOrder;
            var query = queryDisplayBuilder.createDefault();
            query.currentPage = page;
            query.itemsPerPage = perPage;
            query.sortBy = sortBy;
            query.sortDirection = sortDirection;
            query.addFilterTermParam($scope.filterText);
            query.addCollectionKeyParam($scope.collectionKey);
            query.addEntityTypeParam($scope.entityType);

            if ($scope.addToCollection) {
                var promise = entityCollectionResource.getEntitiesNotInCollection(query);
            } else {
                var promise = entityCollectionResource.getCollectionEntities(query);
            }
            promise.then(function(results) {
                var queryResult;
                switch($scope.entityType) {
                    case 'Invoice' :
                        queryResult = queryResultDisplayBuilder.transform(results, invoiceDisplayBuilder);
                        $scope.invoices = queryResult.items;
                        break;
                    case 'Customer' :
                        queryResult = queryResultDisplayBuilder.transform(results, customerDisplayBuilder);
                        $scope.customers = queryResult.items;
                        break;
                    default :
                        queryResult = queryResultDisplayBuilder.transform(results, productDisplayBuilder);
                        $scope.products = queryResult.items;
                        break
                };
                $scope.maxPages = queryResult.totalPages;
                $scope.entityCount = queryResult.totalItems;
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            }, function(reason) {
                notificationsService.error('Failed to load entities ' + reason);
            });
        }

        function handleEntity(entity) {
            var promise;
            if ($scope.addToCollection) {
                promise = entityCollectionResource.addEntityToCollection(entity.key, $scope.collectionKey);
            } else {
                promise = entityCollectionResource.removeEntityFromCollection(entity.key, $scope.collectionKey);
            }

            promise.then(function() {
                eventsService.emit(collectionChanged);
              loadEntities();
            }, function(reason) {
                notificationsService.error('Failed to add entity to collection ' + reason);
            });
        }

        function saveCollection() {
            $scope.wasFormSubmitted = true;
            if ($scope.entitycollectionForm.name.$valid) {
                var promise = entityCollectionResource.saveEntityCollection($scope.collection);
                promise.then(function(result) {
                    $scope.collection = entityCollectionDisplayBuilder.transform(result);
                    $scope.editCollection = false;
                    treeService.reloadNode($scope.currentNode);
                }, function(reason) {
                  notificationsService.error('Failed to save entity collection');
                });
            }
        }

        function toggleMode() {
            $scope.currentPage = 0;
            loadEntities();
        }

        //--------------------------------------------------------------------------------------
        // Events methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name limitChanged
         * @function
         *
         * @description
         * Helper function to set the amount of items to show per page for the paging filters and calculations
         */
        function limitChanged(newVal) {
            $scope.limitAmount = newVal;
            $scope.currentPage = 0;
            loadEntities();
        }

        /**
         * @ngdoc method
         * @name changePage
         * @function
         *
         * @description
         * Helper function re-search the products after the page has changed
         */
        function changePage (newPage) {
            $scope.currentPage = newPage;
            loadEntities();
        }

        /**
         * @ngdoc method
         * @name changeSortOrder
         * @function
         *
         * @description
         * Helper function to set the current sort on the table and switch the
         * direction if the property is already the current sort column.
         */
        function changeSortOrder(propertyToSort) {

            if ($scope.sortProperty == propertyToSort) {
                if ($scope.sortOrder == "Ascending") {
                    $scope.sortProperty = "-" + propertyToSort;
                    $scope.sortOrder = "Descending";
                } else {
                    $scope.sortProperty = propertyToSort;
                    $scope.sortOrder = "Ascending";
                }
            } else {
                $scope.sortProperty = propertyToSort;
                $scope.sortOrder = "Ascending";
            }

            loadEntities();
        }

        //--------------------------------------------------------------------------------------
        // Calculations
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name numberOfPages
         * @function
         *
         * @description
         * Helper function to get the amount of items to show per page for the paging
         */
        function numberOfPages() {
            return $scope.maxPages;
        }


        /**
         * @ngdoc method
         * @name getFilteredEntities
         * @function
         *
         * @description
         * Calls the product service to search for products via a string search
         * param.  This searches the Examine index in the core.
         */
        function getFilteredEntities(filter) {
            $scope.filterText = filter;
            $scope.currentPage = 0;
            loadEntities();
        }


        // initialize the controller
        init();
}]);

angular.module('merchello').controller('Merchello.Product.Dialogs.PickStaticCollectionController',
    ['$scope', 'eventsService', 'treeService', 'localizationService',
    function($scope, eventsService, treeService, localizationService) {

        $scope.pickerTitle = '';

        $scope.getTreeId = getTreeId;

        var eventName = 'merchello.entitycollection.selected';

        function init() {
            eventsService.on(eventName, onEntityCollectionSelected);
            setTitle();
        }

        function onEntityCollectionSelected(eventName, args, ev) {
            //  {key: "addCollection", value: "4d026d91-fe13-49c7-8f06-da3d9f012181"}
            if (args.key === 'addCollection') {
                $scope.dialogData.addCollectionKey(args.value);
            }
            if (args.key === 'removeCollection') {
                $scope.dialogData.removeCollectionKey(args.value);
            }
        }


        function setTitle() {
            var key = 'merchelloCollections_' + $scope.dialogData.entityType.toLowerCase() + 'Collections';
            localizationService.localize(key).then(function (value) {
                $scope.pickerTitle = value;
                setTree();
            });
        }

        function setTree() {
            treeService.getTree({section: 'merchello'}).then(function(tree) {
                var root = tree.root;
                var treeId = getTreeId();

                $scope.pickerRootNode = _.find(root.children, function (child) {
                    return child.id === treeId;
                });
            });
        }

        function getTreeId() {
            switch ($scope.dialogData.entityType) {
                case 'product':
                    return 'products';
                case 'invoice':
                    return 'sales';
                case 'customer':
                    return 'customers';
                default:
                    return '';
            }
        }

        // intitialize
        init();
}]);

angular.module('merchello').controller('Merchello.EntityCollections.Dialogs.SelectFilterProviderController',
    ['$scope',
    function($scope) {

        $scope.loaded = true;

        $scope.setSelection = function(provider) {
            $scope.submit(provider);
        }

}]);

angular.module('merchello').controller('Merchello.EntityCollections.Dialogs.SortFilterGroupsController',
['$scope',
   function($scope) {


       $scope.sortableCollections = {
           start : function(e, ui) {
               ui.item.data('start', ui.item.index());
           },
           stop: function (e, ui) {
               var start = ui.item.data('start'),
                   end =  ui.item.index();
               for(var i = 0; i < $scope.dialogData.collections.length; i++) {
                   $scope.dialogData.collections[i].sortOrder = i;
               }
           },
           disabled: false,
           cursor: "move"
       }

   }]);

/**
 * @ngdoc controller
 * @name Merchello.Common.Dialogs.SortStaticCollectionController
 * @function
 *
 * @description
 * The controller for the delete confirmations
 */
angular.module('merchello')
    .controller('Merchello.EntityCollections.Dialogs.SortStaticCollectionController',
    ['$scope', 'appState', 'treeService', 'notificationsService', 'navigationService', 'entityCollectionHelper', 'entityCollectionResource', 'entityCollectionProviderDisplayBuilder', 'entityCollectionDisplayBuilder',
        function($scope, appState, treeService, notificationsService, navigationService, entityCollectionHelper, entityCollectionResource, entityCollectionProviderDisplayBuilder, entityCollectionDisplayBuilder) {

            $scope.loaded = false;
            $scope.name = '';
            $scope.entityType = '';
            $scope.dialogData = {};
            $scope.entityCollectionProviders = [];
            $scope.entityCollections = [];
            $scope.sortableProviderKeys = [];
            $scope.isRootNode = false;
            $scope.nodePath = [];

            // exposed methods
            $scope.save = save;

            function init() {
                $scope.dialogData = $scope.$parent.currentAction.metaData.dialogData;
                $scope.entityType = entityCollectionHelper.getEntityTypeByTreeId($scope.dialogData.entityType);
                loadValidSortableProviderKeys();
                $scope.nodePath = treeService.getPath($scope.currentNode);
            }

            function loadValidSortableProviderKeys() {
                var promise = entityCollectionResource.getSortableProviderKeys();
                promise.then(function(keys) {
                    $scope.sortableProviderKeys = keys;
                    if ($scope.dialogData.parentKey == undefined || $scope.dialogData.parentKey == '') {
                        loadRootLevelCollections();
                    } else {
                        loadParentCollection();
                    }
                });
            }

            function loadRootLevelCollections() {
                $scope.isRootNode = true;
                var parentPromise = entityCollectionResource.getRootCollectionsByEntityType($scope.entityType);
                parentPromise.then(function(collections) {
                    var transformed = [];
                    if (!angular.isArray(collections)) {
                        collections.sortOrder = 0;
                        transformed.push(entityCollectionDisplayBuilder.transform(collections));
                    } else {
                        transformed = entityCollectionDisplayBuilder.transform(collections);
                    }

                    // we need to weed out the non static providers if any
                    $scope.entityCollections = _.filter(transformed, function(c) {

                        return _.find($scope.sortableProviderKeys, function(k) {
                            return k === c.providerKey;
                        }) !== undefined;
                    });
                    $scope.loaded = true;
                });
            }

            function loadParentCollection() {
                var parentPromise = entityCollectionResource.getChildEntityCollections($scope.dialogData.parentKey);
                parentPromise.then(function(collections) {
                    if (!angular.isArray(collections)) {
                        $scope.entityCollections.push(entityCollectionDisplayBuilder.transform(collections));
                    } else {
                        $scope.entityCollections = entityCollectionDisplayBuilder.transform(collections);
                    }
                    $scope.loaded = true;
                });
            }

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description - Saves the newly sorted nodes and updates the tree UI.
             */
            function save() {

                // set the sorts here
                for(var i = 0; i < $scope.entityCollections.length; i++) {
                    $scope.entityCollections[i].sortOrder = i;
                }


                // save updated sort orders
                var promise = entityCollectionResource.updateSortOrders($scope.entityCollections);
                promise.then(function() {

                    // reload the children of the parent

                    var childPromise = treeService.loadNodeChildren({ node: $scope.currentNode });
                    childPromise.then(function(children) {
                        var reloadPromise = treeService.reloadNode($scope.currentNode);
                        reloadPromise.then(function() {
                            navigationService.hideNavigation();
                            notificationsService.success('Collections sorted success.');
                        });

                    }, function(reason) {
                        notificationsService.error('failed to load node children ' + reason)
                    });
                });
            }

            // Sortable available offers
            /// -------------------------------------------------------------------

            $scope.sortableCollections = {
                start : function(e, ui) {
                    ui.item.data('start', ui.item.index());
                },
                stop: function (e, ui) {
                    var start = ui.item.data('start'),
                        end =  ui.item.index();
                   // console.info(start + ' ' + end);
                    //$scope.offerSettings.reorderComponent(start, end);
                },
                disabled: false,
                cursor: "move"
            }

            init();
        }]);



angular.module('merchello').controller('Merchello.Directives.EntityStaticCollectionsDirectiveController',
    ['$scope', 'notificationsService', 'dialogService', 'entityCollectionHelper', 'entityCollectionResource', 'dialogDataFactory', 'entityCollectionDisplayBuilder',
    function($scope, notificationsService, dialogService, entityCollectionHelper, entityCollectionResource, dialogDataFactory, entityCollectionDisplayBuilder) {

        $scope.collections = [];
        $scope.remove = remove;

        // exposed methods
        $scope.openStaticEntityCollectionPicker = openStaticEntityCollectionPicker;

        function init() {
            $scope.$watch('preValuesLoaded', function(pvl) {
                if (pvl) {
                    loadCollections();
                }
            });
        }

        function loadCollections() {
            entityCollectionResource.getEntityCollectionsByEntity($scope.entity, $scope.entityType).then(function(collections) {
                $scope.collections = entityCollectionDisplayBuilder.transform(collections);
            }, function(reason) {
              notificationsService.error('Failed to get entity collections for entity ' + reason);
            });
        }

        function openStaticEntityCollectionPicker() {
            var dialogData = dialogDataFactory.createAddEditEntityStaticCollectionDialog();
            dialogData.entityType = $scope.entityType.toLocaleLowerCase();
            dialogData.collectionKeys = [];

            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/pick.staticcollection.html',
                show: true,
                callback: processAddEditStaticCollection,
                dialogData: dialogData
            });
        }

        function processAddEditStaticCollection(dialogData) {
            var key = $scope.entity.key;
            entityCollectionResource.addEntityToCollections(key, dialogData.collectionKeys).then(function() {
                loadCollections();
            }, function(reason) {
                notificationsService.error('Failed to add entity to collections ' + reason);
            });
        }

        function remove(collection) {
            entityCollectionResource.removeEntityFromCollection($scope.entity.key, collection.key).then(function() {
                loadCollections();
            });
        }

        // initialize the controller
        init();
}]);

angular.module('merchello').controller('Merchello.Common.Dialogs.DateRangeSelectionController',
    ['$scope', '$q', '$log', '$filter', '$element', 'assetsService', 'angularHelper', 'notificationsService', 'settingsResource', 'settingDisplayBuilder',
    function($scope, $q, $log, $filter, $element,  assetsService, angularHelper, notificationsService, settingsResource, settingDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;

        $scope.changeDateFilters = changeDateFilters;

        $scope.dateFormat = 'YYYY-MM-DD';
        $scope.rangeStart = '';
        $scope.rangeEnd = '';
        $scope.save = save;


        function init() {
            var promises = loadAssets();
            promises.push(loadSettings());

            $q.all(promises).then(function() {
                // arg!!! js
                $scope.dateFormat = $scope.settings.dateFormat.toUpperCase();

                var start, end;
                if ($scope.dialogData.startDate !== '' && $scope.dialogData.endDate !== '') {
                    start = $scope.dialogData.startDate;
                    end = $scope.dialogData.endDate;
                } else {
                    end = new Date();
                    start = new Date();
                    start = start.setMonth(start.getMonth() - 1);
                }

                // initial settings use standard
                $scope.rangeStart = $filter('date')(start, $scope.settings.dateFormat);
                $scope.rangeEnd =  $filter('date')(end, $scope.settings.dateFormat);

                setupDatePicker("#filterStartDate", $scope.rangeStart);
                $element.find("#filterStartDate").datetimepicker().on("changeDate", applyDateStart);

                setupDatePicker("#filterEndDate", $scope.rangeEnd);
                $element.find("#filterEndDate").datetimepicker().on("changeDate", applyDateEnd);

                $scope.preValuesLoaded = true;
            });
        }
        /**
         * @ngdoc method
         * @name loadAssets
         * @function
         *
         * @description - Loads needed and js stylesheets for the view.
         */
        function loadAssets() {
            var promises = [];
            var cssPromise = assetsService.loadCss('lib/datetimepicker/bootstrap-datetimepicker.min.css');
            var jsPromise = assetsService.load(['lib/moment/moment-with-locales.js', 'lib/datetimepicker/bootstrap-datetimepicker.js']);

            promises.push(cssPromise);
            promises.push(jsPromise);

            return promises;
        }

        function loadSettings() {
            var promise = settingsResource.getAllSettings();
            return promise.then(function(allSettings) {
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
        function setupDatePicker(pickerId, defaultDate, isStart) {

            // Open the datepicker and add a changeDate eventlistener
            $element.find(pickerId).datetimepicker({
                defaultDate: defaultDate,
                format: $scope.dateFormat
            });

            //Ensure to remove the event handler when this instance is destroyted
            $scope.$on('$destroy', function () {
                $element.find(pickerId).datetimepicker("destroy");
            });
        }

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
            $scope.rangeStart = start;
            $scope.rangeEnd = end;
        }

        /*-------------------------------------------------------------------
         * Helper Methods
         * ------------------------------------------------------------------*/

        //handles the date changing via the api
        function applyDateStart(e) {
            angularHelper.safeApply($scope, function () {
                // when a date is changed, update the model
                if (e.localDate) {
                    $scope.rangeStart = moment(e.localDate).format($scope.dateFormat);
                }
            });
        }

        //handles the date changing via the api
        function applyDateEnd(e) {
            angularHelper.safeApply($scope, function () {
                // when a date is changed, update the model
                if (e.localDate) {
                    $scope.rangeEnd = moment(e.localDate).format($scope.dateFormat);
                }
            });
        }

        function save() {
            $scope.dialogData.startDate = $scope.rangeStart;
            $scope.dialogData.endDate = $scope.rangeEnd;
            $scope.submit($scope.dialogData);
        }

        // Initialize the controller
        init();

}]);

    /**
     * @ngdoc controller
     * @name Merchello.Common.Dialogs.DeleteConfirmationController
     * @function
     *
     * @description
     * The controller for the delete confirmations
     */
    angular.module('merchello')
        .controller('Merchello.Common.Dialogs.DeleteConfirmationController',
        ['$scope', function($scope) {

        }]);

    /**
     * @ngdoc controller
     * @name Merchello.Common.Dialogs.EditAddressController
     * @function
     * 
     * @description
     * The controller for adding a country
     */
    angular.module('merchello')
        .controller('Merchello.Common.Dialogs.EditAddressController',
        function ($scope) {

            // public methods
            $scope.save = save;

            function init() {
                $scope.address = $scope.dialogData.address;
            };

            function save() {
                $scope.dialogData.address.countryCode = $scope.dialogData.selectedCountry.countryCode;
                if($scope.dialogData.selectedCountry.provinces.length > 0) {
                    $scope.dialogData.address.region = $scope.dialogData.selectedProvince.code;
                }
                $scope.submit($scope.dialogData);
            };

        init();
    });


angular.module('merchello').controller('Merchello.Common.Dialogs.ListViewSettingsController',
    ['$scope', '$q', 'localizationService',
     function($scope, $q, localizationService) {

         $scope.loaded = false;

         $scope.stickListTabTitle = '';

         function init() {

             var tokenKey = '';
             switch ($scope.dialogData.entityType) {
                 case 'Invoice':
                     tokenKey = 'merchelloTabs_' + 'sales';
                     break;
                 case 'Customer':
                     tokenKey = 'merchelloTabs_' + 'customer';
                     break;
                 case 'Offer':
                     tokenKey = 'merchelloTabs_' + 'offer';
                     break;
                 default:
                     tokenKey = 'merchelloTabs_' + 'product';
                     break;
             }


             localizationService.localize(tokenKey).then(function(token) {
                localizationService.localize('merchelloSettings_stickListingTab', [ token ]).then(function(title) {
                    $scope.stickListTabTitle = title;
                    if ($scope.dialogData.settingsComponent !== undefined) {
                        var appender = angular.element( document.querySelector( '#settingsComponent' ) );
                        appender.append($scope.dialogData.settingsComponent);
                    }
                    $scope.loaded = true;
                });
             });
         }

         init();

}]);

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
                notificationsService.info("Saving...", "");
                var promiseSaveCustomer = customerResource.SaveCustomer($scope.customer);
                promiseSaveCustomer.then(function(customer) {
                    $timeout(function() {
                        notificationsService.success("Customer Saved", "");
                        init();
                    }, 400);

                }, function(reason) {
                    notificationsService.error("Customer  Failed", reason.message);
                });
            }

            // initialize the controller
            init();
    }]);

    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.CustomerListController
     * @function
     *
     * @description
     * The controller for customer list view
     */
    angular.module('merchello').controller('Merchello.Backoffice.CustomerListController',
        ['$scope', '$routeParams', '$filter', 'notificationsService', 'localizationService', 'settingsResource', 'merchelloTabsFactory', 'customerResource', 'entityCollectionResource',
            'customerDisplayBuilder',
        function($scope, $routeParams, $filter, notificationsService, localizationService, settingsResource, merchelloTabsFactory, customerResource, entityCollectionResource,
                 customerDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;

            $scope.currencySymbol = '';

            $scope.customerDisplayBuilder = customerDisplayBuilder;
            $scope.load = load;
            $scope.entityType = 'Customer';


            // exposed methods
            $scope.getColumnValue = getColumnValue;

            var globalCurrency = '';
            var allCurrencies = [];
            const baseUrl = '#/merchello/merchello/customeroverview/';

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * initialized when the scope loads.
             */
            function init() {

                loadSettings();
                $scope.tabs = merchelloTabsFactory.createCustomerListTabs();
                $scope.tabs.setActive('customerlist');
                $scope.loaded = true;
            }

            function loadSettings() {
                // currency matching
                settingsResource.getAllCombined().then(function(combined) {
                    allCurrencies = combined.currencies;
                    globalCurrency = combined.currencySymbol;
                    $scope.preValuesLoaded = true;
                }, function(reason) {
                    notificationsService.error('Failed to load combined settings', reason.message);
                });
            }

            function load(query) {
                if (query.hasCollectionKeyParam()) {
                    return entityCollectionResource.getCollectionEntities(query);
                } else {
                    return customerResource.searchCustomers(query);
                }
            }

            function getColumnValue(result, col) {
                switch(col.name) {
                    case 'loginName':
                        return '<a href="' + getEditUrl(result) + '">' + result.loginName + '</a>';
                    case 'firstName':
                        return  '<a href="' + getEditUrl(result) + '">' + result.firstName + ' ' + result.lastName + '</a>';
                    case 'location':
                        var address = result.getPrimaryLocation();
                        var ret = address.locality;
                            ret += ' ' + address.region;
                        if (address.countryCode !== '') {
                            ret += ' ' + address.countryCode;
                        }
                        return ret.trim();
                    case 'lastInvoiceTotal':
                        return $filter('currency')(result.getLastInvoice().total, getCurrencySymbol(result.getLastInvoice()));
                    default:
                        return result[col.name];
                }
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
                if (invoice.currency.symbol !== '' && invoice.currency.symbol !== undefined) {
                    return invoice.currency.symbol;
                }
                var currencyCode = invoice.getCurrencyCode();
                var currency = _.find(allCurrencies, function(currency) {
                    return currency.currencyCode === currencyCode;
                });
                if(currency === null || currency === undefined) {
                    return globalCurrency;
                } else {
                    return currency.symbol;
                }
            }

            function getEditUrl(customer) {
                return baseUrl + customer.key;
            }

            // Initializes the controller
            init();
    }]);

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
                notificationsService.info("Saving...", "");
                var promiseSaveCustomer = customerResource.SaveCustomer($scope.customer);
                promiseSaveCustomer.then(function(customerResponse) {
                    $timeout(function() {
                    notificationsService.success("Customer Saved", "");
                        loadCustomer($scope.customer.key);
                    }, 400);

                }, function(reason) {
                    notificationsService.error("Customer  Failed", reason.message);
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

/**
 * @ngdoc controller
 * @name Merchello.Customer.Dialogs.CustomerCheckoutController
 * @function
 *
 * @description
 * The controller allowing to check a customer out from the back office
 */
angular.module('merchello').controller('Merchello.Customer.Dialogs.CustomerCheckoutController',
    ['$scope', '$filter',
    function($scope, $filter) {

        $scope.billingAddress = {};
        $scope.shippingAddress = {};

        // initializes the controller
        function init() {
            $scope.billingAddress = $scope.dialogData.customer.getDefaultBillingAddress();
            $scope.shippingAddress = $scope.dialogData.customer.getDefaultShippingAddress();
        }

        // formats a quote
        $scope.formatQuote = function(quote) {
            return quote.shipMethod.name + ' (' + $filter('currency')(quote.rate, $scope.dialogData.currencySymbol) + ')';
        }
        
        
        init();

    }]);
    /**
     * @ngdoc controller
     * @name Merchello.Customer.Dialogs.CustomerAddressAddEditController
     * @function
     *
     * @description
     * The controller for adding and editing customer addresses
     */
    angular.module('merchello').controller('Merchello.Customer.Dialogs.CustomerAddressAddEditController',
        ['$scope',
        function($scope) {

            $scope.wasFormSubmitted = false;

            // exposed methods
            $scope.updateProvinceList = updateProvinceList;
            $scope.toTitleCase = toTitleCase;
            $scope.save = save;

            function updateProvinceList() {
                // try to find the province matching the province code of the customer address
                var provinceCode = $scope.dialogData.customerAddress.region;
                if($scope.dialogData.selectedCountry.provinces.length > 0) {
                    var province = _.find($scope.dialogData.selectedCountry.provinces, function(p) {
                        return p.code === provinceCode;
                    });
                    if (province === null || province === undefined) {
                        $scope.dialogData.selectedProvince = $scope.dialogData.selectedCountry.provinces[0];
                    } else {
                        $scope.dialogData.selectedProvince = province;
                    }
                }
            }

            function save() {
                $scope.wasFormSubmitted = true;
                if($scope.editAddressForm.address1.$valid && $scope.editAddressForm.locality.$valid && $scope.editAddressForm.postalCode.$valid) {
                    if($scope.dialogData.selectedCountry.hasProvinces()) {
                        $scope.dialogData.customerAddress.region = $scope.dialogData.selectedProvince.code;
                    }
                    $scope.dialogData.customerAddress.countryCode = $scope.dialogData.selectedCountry.countryCode;
                    $scope.submit($scope.dialogData);
                }
            }

            function toTitleCase(str) {
                return str.charAt(0).toUpperCase() + str.slice(1);
            }

    }]);

    /**
     * @ngdoc controller
     * @name Merchello.Customer.Dialogs.CustomerInfoEditController
     * @function
     *
     * @description
     * The controller for editing customer information
     */
    angular.module('merchello').controller('Merchello.Customer.Dialogs.CustomerInfoEditController',
        ['$scope',
        function($scope) {

            $scope.wasFormSubmitted = false;

            // exposed methods
            $scope.save = save;

            /**
             * @ngdoc method
             * @name submitIfValid
             * @function
             *
             * @description
             * Submit form if valid.
             */
            function save() {
                $scope.wasFormSubmitted = true;
                if ($scope.editInfoForm.email.$valid) {
                    $scope.submit($scope.dialogData);
                }
            }

        }]);

angular.module('merchello').controller('Merchello.Customer.Dialogs.CustomerNewCustomerController',
    ['$scope', '$location', 'dialogDataFactory', 'customerResource', 'notificationsService', 'navigationService', 'customerDisplayBuilder',
        function($scope, $location, dialogDataFactory, customerResource, notificationsService, navigationService, customerDisplayBuilder) {
            $scope.wasFormSubmitted = false;

            $scope.firstName = '';
            $scope.lastName = '';
            $scope.email = '';

            // exposed methods
            $scope.save = save;

            /**
             * @ngdoc method
             * @name submitIfValid
             * @function
             *
             * @description
             * Submit form if valid.
             */
            function save() {
                $scope.wasFormSubmitted = true;
                if ($scope.editInfoForm.email.$valid) {
                    var customer = customerDisplayBuilder.createDefault();
                    customer.loginName = $scope.email;
                    customer.email = $scope.email;
                    customer.firstName = $scope.firstName;
                    customer.lastName = $scope.lastName;

                    var promiseSaveCustomer = customerResource.AddCustomer(customer);
                    promiseSaveCustomer.then(function (customerResponse) {
                        notificationsService.success("Customer Saved", "");
                        navigationService.hideNavigation();
                        $location.url("/merchello/merchello/customeroverview/" + customerResponse.key, true);
                    }, function (reason) {
                        notificationsService.error("Customer Save Failed", reason.message);
                    });
                }
            }
        }]);

/**
 * @ngdoc controller
 * @name Merchello.Customer.Dialogs.ProductSelectionController
 * @function
 *
 * @description
 * The controller to configure the price component constraint
 */
angular.module('merchello').controller('Merchello.Customer.Dialogs.ProductSelectionFilterController',
    ['$q', '$scope', 'notificationsService', 'productResource', 'settingsResource', 'productDisplayBuilder', 'queryDisplayBuilder', 'queryResultDisplayBuilder',
        function($q, $scope, notificationsService, productResource, settingsResource, productDisplayBuilder, queryDisplayBuilder, queryResultDisplayBuilder) {

            $scope.loaded = false;
            $scope.context = 'display';
            $scope.filterText = "";
            $scope.products = [];
            $scope.filteredproducts = [];
            $scope.watchCount = 0;
            $scope.sortProperty = "name";
            $scope.sortOrder = "Ascending";
            $scope.limitAmount = 10;
            $scope.currentPage = 0;
            $scope.maxPages = 0;

            // dialog properties
            $scope.selectedProducts = [];

            // exposed methods
            $scope.addProduct = addProduct;
            $scope.removeProduct = removeProduct;
            $scope.changePage = changePage;
            $scope.limitChanged = limitChanged;
            $scope.changeSortOrder = changeSortOrder;
            $scope.getFilteredProducts = getFilteredProducts;
            $scope.numberOfPages = numberOfPages;
            $scope.productIsSelected = productIsSelected;
            $scope.save = save;

            //--------------------------------------------------------------------------------------
            // Initialization methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                loadSettings();
            }

            /**
             * @ngdoc method
             * @name loadProducts
             * @function
             *
             * @description
             * Load the products from the product service, then wrap the results
             * in Merchello models and add to the scope via the products collection.
             */
            function loadProducts() {

                var page = $scope.currentPage;
                var perPage = $scope.limitAmount;
                var sortBy = $scope.sortProperty.replace("-", "");
                var sortDirection = $scope.sortOrder;

                var query = queryDisplayBuilder.createDefault();
                query.currentPage = page;
                query.itemsPerPage = perPage;
                query.sortBy = sortBy;
                query.sortDirection = sortDirection;
                query.addFilterTermParam($scope.filterText);

                var promise = productResource.searchProducts(query);
                promise.then(function (response) {
                    var queryResult = queryResultDisplayBuilder.transform(response, productDisplayBuilder);

                    $scope.products = queryResult.items;

                    $scope.maxPages = queryResult.totalPages;
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;

                }, function (reason) {
                    notificationsService.success("Products Load Failed:", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description
             * Load the settings from the settings service to get the currency symbol
             */
            function loadSettings() {
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    $scope.currencySymbol = currencySymbol;

                    loadProducts();
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }


            //--------------------------------------------------------------------------------------
            // Events methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name limitChanged
             * @function
             *
             * @description
             * Helper function to set the amount of items to show per page for the paging filters and calculations
             */
            function limitChanged(newVal) {
                $scope.limitAmount = newVal;
                $scope.currentPage = 0;
                loadProducts();
            }

            /**
             * @ngdoc method
             * @name changePage
             * @function
             *
             * @description
             * Helper function re-search the products after the page has changed
             */
            function changePage (newPage) {
                $scope.currentPage = newPage;
                loadProducts();
            }

            /**
             * @ngdoc method
             * @name changeSortOrder
             * @function
             *
             * @description
             * Helper function to set the current sort on the table and switch the
             * direction if the property is already the current sort column.
             */
            function changeSortOrder(propertyToSort) {

                if ($scope.sortProperty == propertyToSort) {
                    if ($scope.sortOrder == "Ascending") {
                        $scope.sortProperty = "-" + propertyToSort;
                        $scope.sortOrder = "Descending";
                    } else {
                        $scope.sortProperty = propertyToSort;
                        $scope.sortOrder = "Ascending";
                    }
                } else {
                    $scope.sortProperty = propertyToSort;
                    $scope.sortOrder = "Ascending";
                }

                loadProducts();
            }

            /**
             * @ngdoc method
             * @name getFilteredProducts
             * @function
             *
             * @description
             * Calls the product service to search for products via a string search
             * param.  This searches the Examine index in the core.
             */
            function getFilteredProducts(filter) {
                $scope.filterText = filter;
                $scope.currentPage = 0;
                loadProducts();
            }

            function addProduct(product, variantKeys) {
                var pi = new ProductItem();
                pi.product = product;
                if (product.hasVariants()) {
                    angular.forEach(product.productVariants, function(pv) {
                        var checked = false;
                        if (variantKeys !== undefined) {
                            var found = _.find(variantKeys, function(key) { return key === pv.key; });
                            if (found) {
                                checked = true;
                            } else {
                                checked = false;
                            }
                        }
                      var vi = new VariantItem();
                        vi.key = pv.key;
                        vi.name = pv.name;
                        vi.sku = pv.sku;
                        vi.checked = checked;
                        pi.selectedVariants.push(vi);
                    });
                }
                $scope.selectedProducts.push(pi);
                $scope.context = 'display';
            }

            function removeProduct(constraint) {
                $scope.selectedProducts = _.reject($scope.selectedProducts, function(sp) { return sp.product.key === constraint.product.key; });
            }

            function productIsSelected(product) {
                var pc = _.find($scope.selectedProducts, function(p) { return p.product.key === product.key; });
                return pc !== undefined;
            }

            //--------------------------------------------------------------------------------------
            // Calculations
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name numberOfPages
             * @function
             *
             * @description
             * Helper function to get the amount of items to show per page for the paging
             */
            function numberOfPages() {
                return $scope.maxPages;
            }

            // ---------------------------------------------------------------------------------------
            // Local scope models
            // ---------------------------------------------------------------------------------------
            var ProductItem = function() {
                var self = this;
                self.product = {};
                self.variantSpecific = false;
                self.selectedVariants = [];
                self.exclude = false;
                self.editorOpen = false;
            };

            var VariantItem = function() {
                var self = this;
                self.name = '';
                self.key = '';
                self.sku = '';
                self.checked = false;
            };


            var AddItem = function() {
                var self = this;
                self.key = '';
                self.isProductVariant = false;
            };

            function save() {
                if ($scope.selectedProducts.length === 0) {
                    return;
                }

                angular.forEach($scope.selectedProducts, function(sp) {

                    if(sp.selectedVariants.length > 0) {
                        var variants = _.filter(sp.selectedVariants, function(sv) { return sv.checked; });
                        angular.forEach(variants, function(v) {
                            var variantItem = new AddItem();
                            variantItem.key = v.key;
                            variantItem.isProductVariant = true;
                           $scope.dialogData.addItems.push(variantItem);
                        });
                    } else {
                        // get the master variant key for the product
                        var masterItem = new AddItem();
                        masterItem.key = sp.product.key;
                        $scope.dialogData.addItems.push(masterItem);
                    }

                });

                $scope.submit($scope.dialogData);
            }

            // Initialize the controller
            init();

        }]);


angular.module('merchello').controller('Merchello.Backoffice.MerchelloDashboardController',
    ['$scope', 'settingsResource',
    function($scope, settingsResource) {

        $scope.loaded = false;
        $scope.merchelloVersion = '';

        function init() {
            var promise = settingsResource.getMerchelloVersion();
            promise.then(function(version) {
              $scope.merchelloVersion = version.replace(/['"]+/g, '');
                $scope.loaded = true;
            });
        }

        // initialize the controller
        init();
    }]);

/**
 * @ngdoc controller
 * @name Merchello.Backoffice.SettingsController
 * @function
 *
 * @description
 * The controller for the settings management page
 */
angular.module('merchello').controller('Merchello.Backoffice.SettingsController',
    ['$scope', '$q', '$log', 'serverValidationManager', 'notificationsService', 'settingsResource', 'detachedContentResource', 'settingDisplayBuilder',
        'currencyDisplayBuilder', 'countryDisplayBuilder',
        function($scope, $q, $log, serverValidationManager, notificationsService, settingsResource, detachedContentResource, settingDisplayBuilder, currencyDisplayBuilder) {

            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.savingStoreSettings = false;
            $scope.settingsDisplay = settingDisplayBuilder.createDefault();
            $scope.currencies = [];
            $scope.selectedCurrency = {};
            $scope.languages = [];
            $scope.selectedLanguage = {};

            // exposed methods
            $scope.currencyChanged = currencyChanged;
            $scope.languageChanged = languageChanged;
            $scope.save = save;


            function init() {

                var deferred = $q.defer();
                $q.all([
                    detachedContentResource.getAllLanguages(),
                    settingsResource.getAllCombined()
                ]).then(function (data) {
                    deferred.resolve(data);
                });

                deferred.promise.then(function (results) {
                    $scope.languages = results[0];

                    var combined = results[1];
                    $scope.settingsDisplay = combined.settings;
                    $scope.currencies = _.sortBy(currencyDisplayBuilder.transform(combined.currencies), function (currency) {
                        return currency.name;
                    });
                    $scope.selectedCurrency = _.find($scope.currencies, function (currency) {
                        return currency.currencyCode === $scope.settingsDisplay.currencyCode;
                    });
                    $scope.selectedLanguage = _.find($scope.languages, function(lang) {
                        return lang.isoCode === $scope.settingsDisplay.defaultExtendedContentCulture;
                    });
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;

                    if ($scope.settingsDisplay.hasDomainRecord === false) {
                        try {
                            settingsResource.recordDomain({
                                migrationKey: $scope.settingsDisplay.migrationKey,
                                domainName: document.location.hostname
                            });
                        } catch(err) {
                            // catch the error so it does not display
                        }
                    }

                }, function (reason) {
                    otificationsService.error('Failed to load settings ' + reason);
                });
            }
            function save () {
                $scope.preValuesLoaded = false;

                notificationsService.info("Saving...", "");
                $scope.savingStoreSettings = true;
                $scope.$watch($scope.storeSettingsForm, function(value) {
                    var promise = settingsResource.save($scope.settingsDisplay);
                    promise.then(function(settingDisplay) {
                        notificationsService.success("Store Settings Saved", "");
                        $scope.savingStoreSettings = false;
                        $scope.settingDisplay = settingDisplayBuilder.transform(settingDisplay);
                        init();
                    }, function(reason) {
                        notificationsService.error("Store Settings Save Failed", reason.message);
                    });
                });
            }

            function currencyChanged(currency) {
                $scope.settingsDisplay.currencyCode = currency.currencyCode;
            }

            function languageChanged(language) {
                $scope.settingsDisplay.defaultExtendedContentCulture = language.isoCode;
            }

            init();
}]);

/**
 * @ngdoc controller
 * @name Merchello.Product.Dialogs.AddDetachedContentTypeController
 * @function
 *
 * @description
 * The controller for the adding product content types
 */
angular.module('merchello').controller('Merchello.Product.Dialogs.AddDetachedContentTypeController',
    ['$scope', '$location', 'notificationsService', 'navigationService', 'eventsService',  'detachedContentResource', 'detachedContentTypeDisplayBuilder',
        function($scope, $location, notificationsService, navigationService, eventsService, detachedContentResource, detachedContentTypeDisplayBuilder) {
            $scope.loaded = true;
            $scope.wasFormSubmitted = false;
            $scope.contentType = {};
            $scope.name = '';
            $scope.description = '';
            $scope.associateType = 'Product';
            var eventName = 'merchello.contenttypedropdown.changed';

            $scope.save = function() {
                $scope.wasFormSubmitted = true;
                if ($scope.productContentTypeForm.name.$valid && $scope.contentType.key) {

                    $scope.dialogData.contentType.umbContentType = $scope.contentType;
                    $scope.submit($scope.dialogData);
                }
            }

            function init() {

                eventsService.on(eventName, onSelectionChanged);
            }

            function onSelectionChanged(e, contentType) {
                if (contentType.name !== null && contentType.name !== undefined) {
                    $scope.name = contentType.name;
                }
            }

            init();
        }]);

angular.module('merchello').controller('Merchello.DetachedContentType.Dialogs.EditDetachedContentTypeController',
    ['$scope', function($scope) {
        $scope.wasFormSubmitted = false;
        $scope.save = save;

        function save() {
            $scope.wasFormSubmitted = true;
            if ($scope.productContentTypeForm.name.$valid) {
                $scope.submit($scope.dialogData);
            }
        }
    }]);

/**
 * @ngdoc controller
 * @name Merchello.Directives.DetachedContentTypeListController
 * @function
 *
 * @description
 * The controller for the detached content type list directive
 */
angular.module('merchello').controller('Merchello.Directives.DetachedContentTypeListController',
    ['$scope', 'notificationsService', 'localizationService', 'dialogService', 'detachedContentResource', 'dialogDataFactory', 'detachedContentTypeDisplayBuilder',
    function($scope, notificationsService, localizationService, dialogService, detachedContentResource, dialogDataFactory, detachedContentTypeDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.detachedContentTypes = [];
        $scope.args =  { test: 'action hit' };

        $scope.title = '';
        $scope.edit = editContentType;
        $scope.delete = deleteContentType;
        $scope.add = addContentType;

        $scope.debugAllowDelete = false;

        function init() {
            $scope.debugAllowDelete = Umbraco.Sys.ServerVariables.isDebuggingEnabled;

            var langKey =  $scope.entityType === 'Product' ? 'productContentTypes' : 'productOptionContentTypes';
            localizationService.localize('merchelloDetachedContent_' + langKey).then(function(result) {
               $scope.title = result;
                loadDetachedContentTypes();
            });

        }

        function loadDetachedContentTypes() {

            detachedContentResource.getDetachedContentTypeByEntityType($scope.entityType).then(function(results) {

                $scope.detachedContentTypes = detachedContentTypeDisplayBuilder.transform(results);
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            });
        }
        
        function addContentType() {
            var dialogData = dialogDataFactory.createAddDetachedContentTypeDialogData();
            dialogData.contentType = detachedContentTypeDisplayBuilder.createDefault();
            dialogData.contentType.entityType = $scope.entityType;
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/detachedcontenttype.add.right.html',
                show: true,
                callback: processAddDialog,
                dialogData: dialogData
            });
        }

        function processAddDialog(dialogData) {
            detachedContentResource.addDetachedContentType(dialogData.contentType).then(function(result) {
                notificationsService.success("Content Type Saved", "");
                loadDetachedContentTypes();
                notificationsService.success('Saved successfully');
            }, function(reason) {
                notificationsService.error('Failed to add detached content type ' + reason);
            });
        }

        function editContentType(contentType) {
            var dialogData = dialogDataFactory.createEditDetachedContentTypeDialogData();

            // we need to clone this so that the actual model in the scope is not updated in case the user
            // does not hit save.
            dialogData.contentType = detachedContentTypeDisplayBuilder.transform(contentType);
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/detachedcontenttype.edit.html',
                show: true,
                callback: processEditDialog,
                dialogData: dialogData
            });
        }

        function processEditDialog(dialogData) {
            detachedContentResource.saveDetachedContentType(dialogData.contentType).then(function(dct) {
                loadDetachedContentTypes();
                notificationsService.success('Saved successfully');
            }, function(reason) {
                notificationsService.error('Failed to save detached content type' + reason);
            });
        }

        /**
         * @ngdoc method
         * @name deleteContentType
         * @function
         *
         * @description - Opens the delete content type dialog.
         */
        function deleteContentType(contentType) {
            var dialogData = {};
            dialogData.name = contentType.name;
            dialogData.contentType = contentType;
            localizationService.localize('merchelloDetachedContent_deleteWarning').then(function(warning) {
                dialogData.warning = warning;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processDeleteDialog,
                    dialogData: dialogData
                });
            });
        }

        function processDeleteDialog(dialogData) {
            detachedContentResource.deleteDetachedContentType(dialogData.contentType.key).then(function() {
                loadDetachedContentTypes();
                notificationsService.success('Deleted successfully');
            }, function(reason) {
              notificationsService.error('Failed to delete detached content type' + reason);
            });
        }


        // initialize the controller
        init();
}]);

    /**
     * @ngdoc controller
     * @name Merchello.GatewayProviders.Dialogs.NotificationsMessageAddController
     * @function
     *
     * @description
     * The controller for the adding / editing Notification messages on the Notifications page
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.NotificationsMessageAddController',
        ['$scope',
        function($scope) {

            // exposed methods
            $scope.save = save;
            $scope.setMonitorDefaults = setMonitorDefaults;

            function init() {
                setMonitorDefaults();
            }

            function setMonitorDefaults() {
               var subject = $scope.dialogData.selectedMonitor.name.replace(' (Legacy)', '').replace(' (Razor)', '');
                $scope.dialogData.notificationMessage.name = subject;
                $scope.dialogData.notificationMessage.bodyTextIsFilePath = $scope.dialogData.selectedMonitor.useCodeEditor;
            }

            function save() {
                $scope.dialogData.notificationMessage.monitorKey = $scope.dialogData.selectedMonitor.monitorKey;
                $scope.submit($scope.dialogData);
            }

            init();
    }]);

angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.NotificationsMethodAddEditController',
    ['$scope',
    function($scope) {

}]);

    /**
     * @ngdoc controller
     * @name Merchello.GatewayProvider.Dialogs.NotificationsProviderSettingsSmtpController
     * @function
     *
     * @description
     * The controller for configuring the SMTP provider
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.NotificationsProviderSettingsSmtpController',
        ['$scope', function($scope) {

            $scope.notificationProviderSettings = {};

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                if ($scope.dialogData.provider.extendedData.items.length > 0) {
                    var extendedDataKey = 'merchSmtpProviderSettings';
                    var settingsString = $scope.dialogData.provider.extendedData.getValue(extendedDataKey);
                    $scope.notificationProviderSettings = angular.fromJson(settingsString);

                    // Watch with object equality to convert back to a string for the submit() call on the Save button
                    $scope.$watch(function () {
                        return $scope.notificationProviderSettings;
                    }, function (newValue, oldValue) {
                        $scope.dialogData.provider.extendedData.setValue(extendedDataKey, angular.toJson(newValue));
                    }, true);
                }
            }

            // Initialize
            init();

        }]);

'use strict';
/**
 * @ngdoc controller
 * @name Merchello.Sales.Dialog.CapturePaymentController
 * @function
 *
 * @description
 * The controller for the dialog used in capturing payments on the sales overview page
 */
angular.module('merchello')
    .controller('Merchello.GatewayProviders.Dialogs.CashPaymentMethodAuthorizeCapturePaymentController',
    ['$scope', 'invoiceHelper',
        function($scope, invoiceHelper) {

            function init() {
                $scope.dialogData.amount = invoiceHelper.round($scope.dialogData.invoiceBalance, 2);
            }

            init();

    }]);

angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.CashPaymentMethodAuthorizePaymentController',
    ['$scope', 'invoiceHelper',
    function($scope, invoiceHelper) {
        function init() {
            $scope.dialogData.amount = invoiceHelper.round($scope.dialogData.invoiceBalance, 2);
        }

        init();
}]);

    'use strict';
    /**
     * @ngdoc controller
     * @name Merchello.GatewayProviders.Dialogs.CashPaymentMethodRefundPaymentController
     * @function
     *
     * @description
     * The controller for the dialog used in refunding cash payments
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.CashPaymentMethodRefundPaymentController',
        ['$scope', 'invoiceHelper',
        function($scope, invoiceHelper) {

            $scope.wasFormSubmitted = false;
            $scope.save = save;

            function init() {
                $scope.dialogData.amount = invoiceHelper.round($scope.dialogData.appliedAmount, 2);
            }

            function save() {
                $scope.wasFormSubmitted = true;
                if(invoiceHelper.valueIsInRage($scope.dialogData.amount, 0, $scope.dialogData.appliedAmount)) {
                    $scope.submit($scope.dialogData);
                } else {
                    $scope.refundForm.amount.$setValidity('amount', false);
                }
            }



            // initializes the controller
            init();
    }]);

'use strict';
/**
 * @ngdoc controller
 * @name Merchello.GatewayProviders.Dialogs.CashPaymentMethodVoidPaymentController
 * @function
 *
 * @description
 * The controller for the dialog used in voiding cash payments
 */
angular.module('merchello')
    .controller('Merchello.GatewayProviders.Dialogs.CashPaymentMethodVoidPaymentController',
    ['$scope', function($scope) {


    }]);

    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.PaymentMethodAddEditController',
        ['$scope',
            function($scope) {

        }]);

    /**
     * @ngdoc controller
     * @name Merchello.GatewayProvider.Dialogs.ShippingAddCountryController
     * @function
     *
     * @description
     * The controller for associating countries with shipping providers and warehouse catalogs
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.ShippingAddCountryController',
        ['$scope', function($scope) {



        }]);

    /**
     * @ngdoc controller
     * @name Merchello.GatewayProviders.Dialogs.ShippingFixedRateShipMethodController
     * @function
     *
     * @description
     * The controller for configuring a fixed rate ship method
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.ShippingFixedRateShipMethodController',
        ['$scope', 'notificationsService',
        'shippingFixedRateProviderResource', 'shippingGatewayProviderResource',
        'shipFixedRateTableDisplayBuilder', 'shipRateTierDisplayBuilder',
        function($scope, notificationsService, shippingFixedRateProviderResource, shippingGatewayProviderResource,
                 shipFixedRateTableDisplayBuilder, shipRateTierDisplayBuilder) {

            $scope.loaded = false;
            $scope.isAddNewTier = false;
            $scope.newTier = {};
            $scope.filters = {};
            $scope.rateTable = {}; //shipFixedRateTableDisplayBuilder.createDefault();
            $scope.rateTable.shipMethodKey = ''; //$scope.dialogData.method.key;


            // exposed methods
            $scope.addRateTier = addRateTier;
            $scope.insertRateTier = insertRateTier;
            $scope.cancelRateTier = cancelRateTier;
            $scope.removeRateTier = removeRateTier;
            $scope.save = save;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Runs when the scope is initialized.
             */
            function init() {
                getRateTable();
            }

            /**
             * @ngdoc method
             * @name getRateTableIfRequired
             * @function
             *
             * @description
             * Get the rate table if it exists.
             */
            function getRateTable() {
                var promise = shippingFixedRateProviderResource.getRateTable($scope.dialogData.shippingGatewayMethod.shipMethod);
                promise.then(function(rateTable) {
                    $scope.rateTable = shipFixedRateTableDisplayBuilder.transform(rateTable);
                    $scope.rateTable.shipMethodKey = $scope.dialogData.shippingGatewayMethod.getKey();
                    $scope.loaded = true;
                }, function(reason) {
                    notificationsService.error('Could not retrieve rate table', reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name addRateTier
             * @function
             *
             * @description
             * Adds the edited, new rate tier to the method.
             */
            function addRateTier() {
                $scope.rateTable.addRow($scope.newTier);
                $scope.isAddNewTier = false;
            }

            /**
             * @ngdoc method
             * @name insertRateTier
             * @function
             *
             * @description
             * Inserts a new, blank row in the rate table.
             */
            function insertRateTier() {
                $scope.isAddNewTier = true;
                $scope.newTier = shipRateTierDisplayBuilder.createDefault();
            }

            /**
             * @ngdoc method
             * @name cancelRateTier
             * @function
             *
             * @description
             * Cancels the insert of a new blank row in the rate table.
             */
            function cancelRateTier() {
                $scope.isAddNewTier = false;
                $scope.newTier = {};
            }


            /**
             * @ngdoc method
             * @name removeRateTier
             * @function
             *
             * @description
             * Remove a rate tier from the method.
             */
            function removeRateTier(tier) {
                $scope.rateTable.removeRow(tier);
            }

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Saves the rate table and then submits.
             */
            function save() {
                var promiseSaveRateTable = shippingFixedRateProviderResource.saveRateTable($scope.rateTable);
                promiseSaveRateTable.then(function() {
                    $scope.submit($scope.dialogData);
                }, function(reason) {
                    notificationsService.error('Rate Table Save Failed', reason.message);
                });
            }

            // Initializes the controller
            init();
    }]);

    /**
     * @ngdoc controller
     * @name Merchello.GatewayProviders.Dialogs.ShippingAddCountryProviderController
     * @function
     *
     * @description
     * The controller for the adding / editing shipping providers on the Shipping page
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.ShippingAddCountryProviderController',
        function($scope) {

    });

    /**
     * @ngdoc controller
     * @name Merchello.GatewayProviders.Dialogs.ShipMethodRegionsController
     * @function
     *
     * @description
     * The controller for the adding / editing ship methods regions
     */
    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.ShipMethodRegionsController',
        ['$scope', function($scope) {

            $scope.allProvinces = false;

            // exposed methods
            $scope.toggleAllProvinces = toggleAllProvinces;


            /**
             * @ngdoc method
             * @name toggleAllProvinces
             * @function
             *
             * @description
             * Toggle the provinces.
             */
            function toggleAllProvinces() {
                _.each($scope.dialogData.shippingGatewayMethod.shipMethod.provinces, function (province)
                {
                    province.allowShipping = $scope.allProvinces;
                });
            }
    }]);

angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.WarehouseAddEditController',
    ['$scope',
    function($scope) {

        // exposed methods
        $scope.save = save;

        function save() {
            if($scope.dialogData.selectedCountry.provinces.length > 0) {
                $scope.dialogData.warehouse.region = $scope.dialogData.selectedProvince.code;
            }
            $scope.dialogData.warehouse.countryCode = $scope.dialogData.selectedCountry.countryCode;
            $scope.submit($scope.dialogData);
        }
}]);

    angular.module('merchello').controller('Merchello.GatewayProviders.Dialogs.TaxationEditTaxMethodController',
        ['$scope', function($scope) {



    }]);

angular.module('merchello').controller('Merchello.Directives.ShipCountryGatewaysProviderDirectiveController',
    ['$scope', 'notificationsService', 'dialogService', 'settingsResource',
        'shippingGatewayProviderResource', 'shippingGatewayProviderDisplayBuilder', 'shipMethodDisplayBuilder',
        'shippingGatewayMethodDisplayBuilder', 'gatewayResourceDisplayBuilder', 'dialogDataFactory',
        function($scope, notificationsService, dialogService, settingsResource,
                 shippingGatewayProviderResource, shippingGatewayProviderDisplayBuilder, shipMethodDisplayBuilder,
                 shippingGatewayMethodDisplayBuilder, gatewayResourceDisplayBuilder, dialogDataFactory) {

            $scope.providersLoaded = false;
            $scope.allProviders = [];
            $scope.assignedProviders = [];
            $scope.availableProviders = [];
            $scope.currencySymbol = '';

            // exposed methods
            $scope.deleteCountry = deleteCountry;
            $scope.addShippingProviderDialogOpen = addShippingProviderDialogOpen;
            $scope.addAddShipMethodDialogOpen = addAddShipMethodDialogOpen;
            $scope.deleteShipMethodOpen = deleteShipMethodOpen;
            $scope.editShippingMethodDialogOpen = editShippingMethodDialogOpen;
            $scope.editShippingMethodRegionsOpen = editShippingMethodRegionsOpen;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Initializes the controller
             */
            function init() {
                loadSettings();
                loadCountryProviders();
            }

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Loads the currency settings
             */
            function loadSettings() {
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function(currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
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
                var promiseAllProviders = shippingGatewayProviderResource.getAllShipGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.allProviders = shippingGatewayProviderDisplayBuilder.transform(allProviders);

                    var promiseProviders = shippingGatewayProviderResource.getAllShipCountryProviders($scope.country);
                    promiseProviders.then(function (assigned) {
                        if (angular.isArray(assigned)) {
                            $scope.assignedProviders = shippingGatewayProviderDisplayBuilder.transform(assigned);

                            var available = _.filter($scope.allProviders, function(provider) {
                                var found = _.find($scope.assignedProviders, function(ap) {
                                    return ap.key === provider.key;
                                });
                                return found === undefined || found === null;
                            });
                            angular.forEach(available, function(pusher) {
                                $scope.availableProviders.push(pusher);
                            });

                            loadProviderMethods();
                        }
                    }, function (reason) {
                        notificationsService.error("Fixed Rate Shipping Countries Providers Load Failed", reason.message);
                    });
                }, function (reason) {
                    notificationsService.error("Available Ship Providers Load Failed", reason.message);
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
            function loadProviderMethods() {
                angular.forEach($scope.assignedProviders, function(shipProvider) {
                    var promiseShipMethods = shippingGatewayProviderResource.getShippingGatewayMethodsByCountry(shipProvider, $scope.country);
                    promiseShipMethods.then(function (shipMethods) {
                        var shippingGatewayMethods = shippingGatewayMethodDisplayBuilder.transform(shipMethods);
                        shipProvider.shippingGatewayMethods = _.sortBy(shippingGatewayMethods, function(gatewayMethod) {
                            return gatewayMethod.getName();
                        });
                    }, function (reason) {
                        notificationsService.error("Available Shipping Methods Load Failed", reason.message);
                    });
                });
                $scope.providersLoaded = true;
            }

            /**
             * @ngdoc method
             * @name addEditShippingProviderDialogOpen
             * @function
             *
             * @description
             * Opens the shipping provider dialog via the Umbraco dialogService.
             */
             function addShippingProviderDialogOpen() {
                var dialogData = dialogDataFactory.createAddShipCountryProviderDialogData();
                //dialogData.country = country;
                dialogData.availableProviders = $scope.availableProviders;
                dialogData.selectedProvider = dialogData.availableProviders[0];
                dialogData.selectedResource = dialogData.selectedProvider.availableResources[0];
                dialogData.shipMethodName = 'New ship method';
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.shipcountry.addprovider.html',
                    show: true,
                    callback: shippingProviderDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name addAddShipMethodDialogOpen
             * @function
             *
             * @description
             * Opens the shipping provider dialog via the Umbraco dialogService.
             */
            function addAddShipMethodDialogOpen(provider) {
                var dialogData = dialogDataFactory.createAddShipCountryProviderDialogData();
                dialogData.selectedProvider = provider;
                dialogData.selectedResource = dialogData.selectedProvider.availableResources[0];
                dialogData.shipMethodName = $scope.country.name + " " + dialogData.selectedResource.name;
                dialogData.country = $scope.country;
                dialogData.showProvidersDropDown = false;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.shipcountry.addprovider.html',
                    show: true,
                    callback: shippingProviderDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name editShippingMethodDialogOpen
             * @function
             *
             * @description
             * Opens an injected dialog for editing a the shipping provider's ship method
             */
            function editShippingMethodRegionsOpen(gatewayMethod) {
                var dialogData = dialogDataFactory.createEditShippingGatewayMethodDialogData();
                dialogData.shippingGatewayMethod = gatewayMethod;
                dialogData.currencySymbol = $scope.currencySymbol;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.shipmethod.regions.html',
                    show: true,
                    callback: shippingMethodDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name shippingProviderDialogConfirm
             * @function
             *
             * @description
             * Handles the edit after receiving the dialogData from the dialog view/controller
             */
            function shippingProviderDialogConfirm(dialogData) {
                var newShippingMethod = shipMethodDisplayBuilder.createDefault();
                if(dialogData.shipMethodName ==='') {
                    newShippingMethod.name = $scope.country.name + " " + dialogData.selectedResource.name;
                } else {
                    newShippingMethod.name = dialogData.shipMethodName;
                }
                newShippingMethod.providerKey = dialogData.selectedProvider.key;
                newShippingMethod.serviceCode = dialogData.selectedResource.serviceCode;
                newShippingMethod.shipCountryKey = $scope.country.key;
                var promiseAddMethod;
                promiseAddMethod = shippingGatewayProviderResource.addShipMethod(newShippingMethod);
                promiseAddMethod.then(function () {
                    reload();
                }, function (reason) {
                    notificationsService.error("Shipping Provider / Initial Method Create Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name deleteShipMethodOpen
             * @function
             *
             * @description
             * Opens the delete confirmation dialog for deleting ship methods
             */
            function deleteShipMethodOpen(shipMethod) {
                var dialogData = dialogDataFactory.createDeleteShipCountryDialogData();
                dialogData.shipMethod = shipMethod;
                dialogData.name = shipMethod.name;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: deleteShipMethodDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name deleteShipMethodOpen
             * @function
             *
             * @description
             * Processes the deleting of a ship method
             */
            function deleteShipMethodDialogConfirm(dialogData) {
                var shipMethod = dialogData.shipMethod;
                var promise = shippingGatewayProviderResource.deleteShipMethod(shipMethod);
                promise.then(function() {
                    reload();
                });
            }

            // injected dialogs

            /**
             * @ngdoc method
             * @name editShippingMethodDialogOpen
             * @function
             *
             * @description
             * Opens an injected dialog for editing a the shipping provider's ship method
             */
            function editShippingMethodDialogOpen(gatewayMethod) {
                var dialogData = dialogDataFactory.createEditShippingGatewayMethodDialogData();
                dialogData.shippingGatewayMethod = gatewayMethod;
                dialogData.currencySymbol = $scope.currencySymbol;
                var editor = gatewayMethod.dialogEditorView.editorView;
                dialogService.open({
                    template: editor,
                    show: true,
                    callback: shippingMethodDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name shippingMethodDialogConfirm
             * @function
             *
             * @description
             * Handles the edit after recieving the dialogData from the dialog view/controller
             */
            function shippingMethodDialogConfirm(dialogData) {
                var promiseShipMethodSave = shippingGatewayProviderResource.saveShipMethod(dialogData.shippingGatewayMethod.shipMethod);
                promiseShipMethodSave.then(function() {
                }, function (reason) {
                    notificationsService.error("Shipping Method Save Failed", reason.message);
                });
                reload();
            }

            /**
             * @ngdoc method
             * @name reload
             * @function
             *
             * @description
             * Handles the reload after receiving the modifying ship country information
             */
            function reload() {
                $scope.reload();
            }

            /**
             * @ngdoc method
             * @name delete
             * @function
             *
             * @description
             * Handles the delete of a ship country view/controller
             */
            function deleteCountry() {
                $scope.delete();
            }

            // initialize the directive
            init();

        }]);

/**
 * @ngdoc controller
 * @name Merchello.Backoffice.GatewayProvidersListController
 * @function
 *
 * @description
 * The controller for the gateway providers list view controller
 */
angular.module("merchello").controller("Merchello.Backoffice.GatewayProvidersListController",
    ['$scope', '$q', 'assetsService', 'notificationsService', 'dialogService', 'merchelloTabsFactory',
        'gatewayProviderResource', 'gatewayProviderDisplayBuilder',
        function($scope, $q, assetsService, notificationsService, dialogService, merchelloTabsFactory, gatewayProviderResource, gatewayProviderDisplayBuilder)
        {
            // load the css file
            assetsService.loadCss('/App_Plugins/Merchello/assets/css/merchello.css');

            $scope.loaded = true;
            $scope.notificationGatewayProviders = [];
            $scope.paymentGatewayProviders = [];
            $scope.shippingGatewayProviders = [];
            $scope.taxationGatewayProviders = [];
            $scope.tabs = [];

            // exposed methods
            $scope.activateProvider = activateProvider;
            $scope.deactivateProvider = deactivateProvider;
            $scope.editProviderConfigDialogOpen = editProviderConfigDialogOpen;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                $q.all([
                    loadAllNotificationGatwayProviders(),
                    loadAllPaymentGatewayProviders(),
                    loadAllShippingGatewayProviders(),
                    loadAllTaxationGatewayProviders()
                ]).then(function() {
                    $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
                    $scope.tabs.setActive('providers');
                });
            }

            /**
             * @ngdoc method
             * @name loadAllNotificationGatwayProviders
             * @function
             *
             * @description
             * Loads in notification gateway providers from server into the scope.  Called in init().
             */
            function loadAllNotificationGatwayProviders() {
                var promiseAllProviders = gatewayProviderResource.getResolvedNotificationGatewayProviders();
                promiseAllProviders.then(function(allProviders) {
                    $scope.notificationGatewayProviders = sortProviders(gatewayProviderDisplayBuilder.transform(allProviders));
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function(reason) {
                    notificationsService.error("Available Notification Gateway Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadAllPaymentGatewayProviders
             * @function
             *
             * @description
             * Loads in payment gateway providers from server into the scope.  Called in init().
             */
            function loadAllPaymentGatewayProviders() {
                var promiseAllProviders = gatewayProviderResource.getResolvedPaymentGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.paymentGatewayProviders = sortProviders(gatewayProviderDisplayBuilder.transform(allProviders));
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Available Payment Gateway Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadAllShippingGatewayProviders
             * @function
             *
             * @description
             * Loads in shipping gateway providers from server into the scope.  Called in init().
             */
            function loadAllShippingGatewayProviders() {
                var promiseAllProviders = gatewayProviderResource.getResolvedShippingGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.shippingGatewayProviders = sortProviders(gatewayProviderDisplayBuilder.transform(allProviders));
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Available Shipping Gateway Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadAllTaxationGatewayProviders
             * @function
             *
             * @description
             * Loads in taxation gateway providers from server into the scope.  Called in init().
             */
            function loadAllTaxationGatewayProviders() {
                var promiseAllProviders = gatewayProviderResource.getResolvedTaxationGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.taxationGatewayProviders = sortProviders(gatewayProviderDisplayBuilder.transform(allProviders));
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Available Taxation Gateway Providers Load Failed", reason.message);
                });
            }

            /* -------------------------------------------------------------------
                Events
            ----------------------------------------------------------------------- */

            /**
             * @ngdoc method
             * @name activateProvider
             * @param {GatewayProvider} provider The GatewayProvider to activate
             * @function
             *
             * @description
             * Calls the merchelloGatewayProviderService to activate the provider.
             */
            function activateProvider(provider) {
                var promiseActivate = gatewayProviderResource.activateGatewayProvider(provider);
                promiseActivate.then(function () {
                    provider.activated = true;
                    init();
                    notificationsService.success(provider.name + " Method Activated");
                }, function (reason) {
                    notificationsService.error(provider.name + " Activate Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name deactivateProvider
             * @param {GatewayProvider} provider The GatewayProvider to deactivate
             * @function
             *
             * @description
             * Calls the merchelloGatewayProviderService to deactivate the provider.
             */
            function deactivateProvider(provider) {
                var dialogData = {};
                dialogData.name = 'Provider: ' + provider.name;
                dialogData.provider = provider;

                dialogData.warning = 'This will any delete any configurations, methods and messages you currently have saved.';

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processDeactivateProvider,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name deactivateProvider
             * @param {GatewayProvider} provider The GatewayProvider to deactivate
             * @function
             *
             * @description
             * Calls the merchelloGatewayProviderService to deactivate the provider.
             */
            function processDeactivateProvider(dialogData) {
                var provider = dialogData.provider;
                var promiseDeactivate = gatewayProviderResource.deactivateGatewayProvider(provider);
                promiseDeactivate.then(function () {
                    provider.activated = false;
                    notificationsService.success(provider.name + " Deactivated");
                }, function (reason) {
                    notificationsService.error(provider.name + " Deactivate Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name editProviderConfigDialogOpen
             * @param {GatewayProvider} provider The GatewayProvider to configure
             * @function
             *
             * @description
             * Opens the dialog to allow user to add provider configurations
             */
            function editProviderConfigDialogOpen(provider) {
                var dialogProvider = provider;
                if (!provider) {
                    return;
                }
                var myDialogData = {
                    provider: dialogProvider
                };
                dialogService.open({
                    template: provider.dialogEditorView.editorView,
                    show: true,
                    callback: providerConfigDialogConfirm,
                    dialogData: myDialogData
                });
            }

            /**
             * @ngdoc method
             * @name providerConfigDialogConfirm
             * @param {dialogData} model returned from the dialog view
             * @function
             *
             * @description
             * Handles the data passed back from the provider editor dialog and saves it to the database
             */
            function providerConfigDialogConfirm(data) {
                $scope.preValuesLoaded = false;
                var promise = gatewayProviderResource.saveGatewayProvider(data.provider);
                promise.then(function (provider) {
                        notificationsService.success("Gateway Provider Saved", "");
                        init();
                    },
                    function (reason) {
                        notificationsService.error("Gateway Provider Save Failed", reason.message);
                    }
                );
            }

            function sortProviders(providers) {
                return _.sortBy(providers, function (gwp) { return gwp.name; });
            }


            // Initialize the controller

            init();

        }]);








    angular.module('merchello').controller('Merchello.Backoffice.NotificationMessageEditorController',
    ['$scope', '$q', '$routeParams', '$location', 'assetsService', 'dialogService', 'eventsService', 'notificationsService', 'merchelloTabsFactory', 'dialogDataFactory',
    'notificationGatewayProviderResource', 'notificationMessageDisplayBuilder', 'notificationMonitorDisplayBuilder',
    function($scope, $q, $routeParams, $location, assetsService, dialogService, eventsService, notificationsService, merchelloTabsFactory, dialogDataFactory,
    notificationGatewayProviderResource, notificationMessageDisplayBuilder, notificationMonitorDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.tabs = [];
        $scope.notificationMessage = {};
        $scope.notificationMonitors = [];
        $scope.currentMonitor = {};
        
        $scope.reload = init;


        // exposed methods
        $scope.save = save;
        $scope.deleteMessage = deleteMessage;

        var saveEventName = 'notification.message.saved';
        
        function init() {
            $scope.loaded = false;
            var key = $routeParams.id;

            $q.all([
                notificationGatewayProviderResource.getNotificationMessagesByKey(key),
                notificationGatewayProviderResource.getAllNotificationMonitors()
            ]).then(function(data) {

                $scope.notificationMessage = notificationMessageDisplayBuilder.transform(data[0]);
                $scope.notificationMonitors = notificationMonitorDisplayBuilder.transform(data[1]);
                
                $scope.currentMonitor = _.find($scope.notificationMonitors, function(m) {
                   return m.monitorKey === $scope.notificationMessage.monitorKey;
                });

                $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
                $scope.tabs.insertTab('messageEditor', 'merchelloTabs_message', '#/merchello/merchello/notification.messageeditor/' + key, 2);
                $scope.tabs.setActive('messageEditor');

                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            });
        }
        

        function deleteMessage() {
            var dialogData = dialogDataFactory.createDeleteNotificationMessageDialogData();
            dialogData.notificationMessage = $scope.notificationMessage;
            dialogData.name = $scope.notificationMessage.name;

            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                show: true,
                callback: notificationsMessageDeleteDialogConfirm,
                dialogData: dialogData
            });
        }

        /**
         * @ngdoc method
         * @name notificationsMessageDeleteDialogConfirm
         * @function
         *
         * @description
         * Handles the delete after recieving the deleted command from the dialog view/controller
         */
        function notificationsMessageDeleteDialogConfirm(dialogData) {
            var promiseNotificationMethod = notificationGatewayProviderResource.deleteNotificationMessage(dialogData.notificationMessage.key);
            promiseNotificationMethod.then(function () {
                $location.url('merchello/merchello/notificationproviders/manage', true);
            }, function (reason) {
                notificationsService.error("Notification Method Deletion Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name save
         * @function
         *
         * @description
         * Saves the notification message
         */
        function save() {
            $scope.preValuesLoaded = false;

            var promiseSave = notificationGatewayProviderResource.updateNotificationMessage($scope.notificationMessage);
            promiseSave.then(function () {
                notificationsService.success("Notification Message Saved");
                eventsService.emit(saveEventName, $scope.notificationMessage);
                
                init();
            }, function (reason) {
                notificationsService.error("Notification Message Save Failed", reason.message);
            });
        }

        // Initialize the controller
        init();

    }]);

    angular.module('merchello').controller('Merchello.Backoffice.NotificationProvidersController',
        ['$scope', '$location', 'notificationsService', 'dialogService', 'merchelloTabsFactory', 'dialogDataFactory', 'gatewayResourceDisplayBuilder',
        'notificationGatewayProviderResource', 'notificationGatewayProviderDisplayBuilder', 'notificationMethodDisplayBuilder',
        'notificationMonitorDisplayBuilder', 'notificationMessageDisplayBuilder',
        function($scope, $location, notificationsService, dialogService, merchelloTabsFactory, dialogDataFactory, gatewayResourceDisplayBuilder,
        notificationGatewayProviderResource, notificationGatewayProviderDisplayBuilder, notificationMethodDisplayBuilder,
        notificationMonitorDisplayBuilder, notificationMessageDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.notificationMonitors = [];
            $scope.tabs = [];

            $scope.notificationGatewayProviders = [];

            // exposed methods
            $scope.addNotificationMethod = addNotificationMethod;
            $scope.editNotificationMethod = editNotificationMethod;
            $scope.deleteNotificationMethod = deleteNotificationMethod;
            $scope.addNotificationMessage = addNotificationMessage;
            $scope.deleteNotificationMessage = deleteNotificationMessage;
            
            $scope.goToEditor = goToEditor;

            function init() {
                loadAllNotificationGatewayProviders();
                loadAllNotificationMonitors();
                $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
                $scope.tabs.setActive('notification');
            }

            /**
             * @ngdoc method
             * @name getProviderByKey
             * @function
             *
             * @description
             * Helper method to get a provider from the notificationGatewayProviders array using the provider key passed in.
             */
            function getProviderByKey(providerkey) {
                return _.find($scope.notificationGatewayProviders, function (gatewayprovider) { return gatewayprovider.key == providerkey; });
            }

            /**
             * @ngdoc method
             * @name loadAllNotificationGatewayProviders
             * @function
             *
             * @description
             * Load the notification gateway providers from the notification gateway service, then wrap the results
             * in Merchello models and add to the scope via the notificationGatewayProviders collection.
             */
            function loadAllNotificationGatewayProviders() {
                var promiseAllProviders = notificationGatewayProviderResource.getAllGatewayProviders();
                promiseAllProviders.then(function (allProviders) {
                    $scope.notificationGatewayProviders = notificationGatewayProviderDisplayBuilder.transform(allProviders);
                    angular.forEach($scope.notificationGatewayProviders, function(provider) {
                        loadNotificationGatewayResources(provider.key);
                        loadNotificationMethods(provider.key);
                    });
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Available Notification Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadNotificationGatewayResources
             * @function
             *
             * @description
             * Load the notification gateway resources from the notification gateway service, then wrap the results
             * in Merchello models and add to the provider in the resources collection.  This will only
             * return resources that haven't already been added via other methods on the provider.
             */
            function loadNotificationGatewayResources(providerKey) {
                var provider = getProviderByKey(providerKey);
                var promiseAllResources = notificationGatewayProviderResource.getGatewayResources(provider.key);
                promiseAllResources.then(function (allResources) {
                    provider.gatewayResources = gatewayResourceDisplayBuilder.transform(allResources);
                    if (provider.gatewayResources.length > 0) {
                        provider.selectedGatewayResource = provider.gatewayResources[0];
                    }
                }, function (reason) {
                    notificationsService.error("Available Notification Provider Resources Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadAllNotificationTriggers
             * @function
             *
             * @description
             * Loads the triggers for the notification messages.
             */
            function loadAllNotificationMonitors() {
                var promise = notificationGatewayProviderResource.getAllNotificationMonitors();
                promise.then(function (notificationMonitors) {
                    $scope.notificationMonitors = notificationMonitorDisplayBuilder.transform(notificationMonitors);
                });
            }

            /**
             * @ngdoc method
             * @name loadNotificationMethods
             * @function
             *
             * @description
             * Load the notification gateway methods from the notification gateway service, then wrap the results
             * in Merchello models and add to the provider in the methods collection.
             */
            function loadNotificationMethods(providerKey) {

                var provider = getProviderByKey(providerKey);
                var promiseAllResources = notificationGatewayProviderResource.getNotificationProviderNotificationMethods(providerKey);
                promiseAllResources.then(function (allMethods) {
                    provider.notificationMethods = notificationMethodDisplayBuilder.transform(allMethods);
                }, function (reason) {
                    notificationsService.error("Notification Methods Load Failed", reason.message);
                });
            }

            //--------------------------------------------------------------------------------------
            // Dialog methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name addNotificationsDialogConfirm
             * @function
             *
             * @description
             * Handles the save after recieving the notification to add from the dialog view/controller
             */
            function addNotificationsDialogConfirm(dialogData) {
                $scope.preValuesLoaded = false;
                var promiseNotificationMethod = notificationGatewayProviderResource.addNotificationMethod(dialogData.notificationMethod);
                promiseNotificationMethod.then(function(notificationFromServer) {
                    notificationsService.success("Notification Method saved.", "");
                    init();
                }, function(reason) {
                    notificationsService.error("Notification Method save Failed", reason.message);
                });
            }

            function saveNotificationDialogConfirm(dialogData) {
                $scope.preValuesLoaded = false;
                var promiseNotificationMethod = notificationGatewayProviderResource.saveNotificationMethod(dialogData.notificationMethod);
                promiseNotificationMethod.then(function(notificationFromServer) {
                    notificationsService.success("Notification Method saved.", "");
                    init();
                }, function(reason) {
                    notificationsService.error("Notification Method save Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name addNotificationMethod
             * @function
             *
             * @description
             * Opens the add notification method dialog via the Umbraco dialogService.
             */
            function addNotificationMethod(provider, resource) {
                var dialogData = dialogDataFactory.createAddEditNotificationMethodDialogData();
                var method = notificationMethodDisplayBuilder.createDefault();
                method.name = resource.name;
                method.serviceCode = resource.serviceCode;
                method.providerKey = provider.key;
                dialogData.notificationMethod = method;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/notification.notificationmethod.addedit.html',
                    show: true,
                    callback: addNotificationsDialogConfirm,
                    dialogData: dialogData
                });
            }

            function editNotificationMethod(method) {
                var dialogData = {
                    notificationMethod: angular.extend(notificationMethodDisplayBuilder.createDefault(), method)
                };

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/notification.notificationmethod.addedit.html',
                    show: true,
                    callback: saveNotificationDialogConfirm,
                    dialogData: dialogData
                });
            }



            /**
             * @ngdoc method
             * @name notificationsMethodDeleteDialogConfirm
             * @function
             *
             * @description
             * Handles the delete after recieving the deleted command from the dialog view/controller
             */
            function notificationsMethodDeleteDialogConfirm(dialogData) {
                $scope.preValuesLoaded = false;
                var promiseNotificationMethod = notificationGatewayProviderResource.deleteNotificationMethod(dialogData.notificationMethod.key);
                promiseNotificationMethod.then(function () {
                    notificationsService.success("Notification Deleted");
                    init();
                }, function (reason) {
                    notificationsService.error("Notification Method Deletion Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name deleteNotificationMethod
             * @function
             *
             * @description
             * Opens the delete dialog via the Umbraco dialogService
             */
            function deleteNotificationMethod(method) {
                var dialogData = dialogDataFactory.createDeleteNotificationMethodDialogData();
                dialogData.notificationMethod = method;
                dialogData.name = method.name;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: notificationsMethodDeleteDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name notificationsMessageDeleteDialogConfirm
             * @function
             *
             * @description
             * Handles the delete after recieving the deleted command from the dialog view/controller
             */
            function notificationsMessageDeleteDialogConfirm(dialogData) {
                var promiseNotificationMethod = notificationGatewayProviderResource.deleteNotificationMessage(dialogData.notificationMessage.key);
                promiseNotificationMethod.then(function () {
                    notificationsService.success("Notification Deleted");
                    init();
                }, function (reason) {
                    notificationsService.error("Notification Method Deletion Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name deleteNotificationMessage
             * @function
             *
             * @description
             * Opens the delete dialog via the Umbraco dialogService
             */
            function deleteNotificationMessage(message) {
                var dialogData = dialogDataFactory.createDeleteNotificationMessageDialogData();
                dialogData.notificationMessage = message;
                dialogData.name = message.name;

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: notificationsMessageDeleteDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name notificationsMessageAddDialogConfirm
             * @function
             *
             * @description
             * Handles the save after recieving the save command from the dialog view/controller
             */
            function notificationsMessageAddDialogConfirm(dialogData) {
                var promiseNotificationMethod = notificationGatewayProviderResource.saveNotificationMessage(dialogData.notificationMessage);
                promiseNotificationMethod.then(function (keyFromServer) {
                    notificationsService.success("Notification Saved", "");
                    init();
                }, function (reason) {
                    notificationsService.error("Notification Message Saved Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name addNotificationMessage
             * @function
             *
             * @description
             * Opens the add notification dialog via the Umbraco dialogService
             */
            function addNotificationMessage(method) {
                var dialogData = dialogDataFactory.createAddEditNotificationMessageDialogData();
                dialogData.notificationMessage = notificationMessageDisplayBuilder.createDefault();

                dialogData.notificationMessage.methodKey = method.key;
                dialogData.notificationMessage.name = method.name;
                dialogData.notificationMonitors = $scope.notificationMonitors;
                dialogData.selectedMonitor = $scope.notificationMonitors[0];
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/notification.notificationmessage.add.html',
                    show: true,
                    callback: notificationsMessageAddDialogConfirm,
                    dialogData: dialogData
                });
            }

            
            function goToEditor(message) {
                var monitor = _.find($scope.notificationMonitors, function(m) {
                    return m.monitorKey === message.monitorKey;
                });

                if (monitor !== undefined) {
                    $location.url('merchello/merchello/notificationmessageeditor/' + message.key, true);
                }
            }
            
            // Initialize the controller
            init();
    }]);

    angular.module('merchello').controller('Merchello.Backoffice.PaymentProvidersController',
        ['$scope', 'notificationsService', 'dialogService', 'paymentGatewayProviderResource', 'dialogDataFactory', 'merchelloTabsFactory',
           'gatewayResourceDisplayBuilder', 'paymentGatewayProviderDisplayBuilder', 'paymentMethodDisplayBuilder',
        function($scope, notificationsService, dialogService, paymentGatewayProviderResource, dialogDataFactory, merchelloTabsFactory,
                 gatewayResourceDisplayBuilder, paymentGatewayProviderDisplayBuilder, paymentMethodDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.paymentGatewayProviders = [];
            $scope.tabs = [];

            // exposed methods
            $scope.addEditPaymentMethod = addEditPaymentMethod;
            $scope.deletePaymentMethod = deletePaymentMethod;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                loadAllPaymentGatewayProviders();
                $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
                $scope.tabs.setActive('payment');
            }

            /**
             * @ngdoc method
             * @name getProviderByKey
             * @function
             *
             * @description
             * Helper method to get a provider from the paymentGatewayProviders array using the provider key passed in.
             */
            function getProviderByKey(providerkey) {
                return _.find($scope.paymentGatewayProviders, function (gatewayprovider) { return gatewayprovider.key == providerkey; });
            }

            //--------------------------------------------------------------------------------------
            // Initialization methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name loadAllPaymentGatewayProviders
             * @function
             *
             * @description
             * Load the payment gateway providers from the payment gateway service, then wrap the results
             * in Merchello models and add to the scope via the paymentGatewayProviders collection.
             */
            function loadAllPaymentGatewayProviders() {

                var promiseAllProviders = paymentGatewayProviderResource.getAllGatewayProviders();
                promiseAllProviders.then(function(allProviders) {

                    $scope.paymentGatewayProviders = paymentGatewayProviderDisplayBuilder.transform(allProviders);

                    angular.forEach($scope.paymentGatewayProviders, function(provider) {
                        loadPaymentGatewayResources(provider.key);
                        loadPaymentMethods(provider.key);
                    });

                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;

                }, function(reason) {
                    notificationsService.error("Available Payment Providers Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadPaymentGatewayResources
             * @function
             *
             * @description
             * Load the payment gateway resources from the payment gateway service, then wrap the results
             * in Merchello models and add to the provider in the resources collection.  This will only
             * return resources that haven't already been added via other methods on the provider.
             */
            function loadPaymentGatewayResources(providerKey) {

                var provider = getProviderByKey(providerKey);
                provider.showSelectResource = false;
                var promiseAllResources = paymentGatewayProviderResource.getGatewayResources(provider.key);
                promiseAllResources.then(function (allResources) {
                    provider.gatewayResources = gatewayResourceDisplayBuilder.transform(allResources);
                    if (provider.gatewayResources.length > 0) {
                        provider.selectedGatewayResource = provider.gatewayResources[0];
                    }

                }, function (reason) {
                    notificationsService.error("Available Payment Provider Resources Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadPaymentMethods
             * @function
             *
             * @description
             * Load the payment gateway methods from the payment gateway service, then wrap the results
             * in Merchello models and add to the provider in the methods collection.
             */
            function loadPaymentMethods(providerKey) {

                var provider = getProviderByKey(providerKey);
                var promiseAllResources = paymentGatewayProviderResource.getPaymentProviderPaymentMethods(providerKey);
                promiseAllResources.then(function (allMethods) {
                    provider.paymentMethods = paymentMethodDisplayBuilder.transform(allMethods);
                }, function (reason) {
                    notificationsService.error("Payment Methods Load Failed", reason.message);
                });
            }



            //--------------------------------------------------------------------------------------
            // Event Handlers
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name removeMethod
             * @function
             *
             * @description
             * Calls the payment gateway service to delete the method passed in via the method parameter
             */
            function removeMethod(method) {
                $scope.preValuesLoaded = false;
                var promiseDelete = paymentGatewayProviderResource.deletePaymentMethod(method.key);
                promiseDelete.then(function () {
                    loadAllPaymentGatewayProviders();
                    notificationsService.success("Payment Method Deleted");
                }, function (reason) {
                    notificationsService.error("Payment Method Delete Failed", reason.message);
                });
            }


            //--------------------------------------------------------------------------------------
            // Dialogs
            //--------------------------------------------------------------------------------------

            /// Method add/edit Dialog

            /**
             * @ngdoc method
             * @name paymentMethodDialogConfirm
             * @function
             *
             * @description
             * Handles the save after recieving the edited method from the dialog view/controller
             */
            function paymentMethodDialogConfirm(method) {
                $scope.preValuesLoaded = false;
                var promiseSave;
                if (method.key.length > 0) {
                    // Save existing method
                    promiseSave = paymentGatewayProviderResource.savePaymentMethod(method);
                } else {
                    // Create new method
                    promiseSave = paymentGatewayProviderResource.addPaymentMethod(method);
                }

                var provider = getProviderByKey(method.providerKey);
                provider.showSelectResource = false;

                promiseSave.then(function () {
                    loadPaymentMethods(method.providerKey);
                    loadPaymentGatewayResources(method.providerKey);
                    $scope.preValuesLoaded = true;
                    notificationsService.success("Payment Method Saved");
                }, function (reason) {
                    notificationsService.error("Payment Method Save Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name addEditPaymentMethod
             * @function
             *
             * @description
             * Opens the payment method dialog via the Umbraco dialogService.  This will default to the dialog view in Merchello
             * unless specified on the custom method in the payment provider.  Also, if it is an add (not edit) then it will
             * initialize a new method and pass that to the dialog service.
             */
            function addEditPaymentMethod(provider, method) {
                if (method === undefined) {
                    method = paymentMethodDisplayBuilder.createDefault();
                    method.providerKey = provider.key; //Todo: When able to add external providers, make this select the correct provider
                    method.paymentCode = provider.selectedGatewayResource.serviceCode;
                    method.name = provider.selectedGatewayResource.name;
                }

                // assert that there is a method editor
                //// http://issues.merchello.com/youtrack/issue/M-610
                if (method.dialogEditorView === undefined || method.dialogEditorView.editorView === '') {
                    method.dialogEditorView.editorView = '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.paymentmethod.addedit.html';
                }

                dialogService.open({
                    template: method.dialogEditorView.editorView,
                    show: true,
                    callback: paymentMethodDialogConfirm,
                    dialogData: method
                });
            }

            /// Method delete Dialog

            /**
             * @ngdoc method
             * @name paymentMethodDeleteDialogConfirm
             * @function
             *
             * @description
             * Handles the save after recieving the deleted method from the dialog view/controller
             */
            function paymentMethodDeleteDialogConfirm(dialogData) {
                removeMethod(dialogData.paymentMethod);
            }

            /**
             * @ngdoc method
             * @name deletePaymentMethod
             * @function
             *
             * @description
             * Opens the delete dialog via the Umbraco dialogService
             */
            function deletePaymentMethod(method) {
                var dialogData = dialogDataFactory.createDeletePaymentMethodDialogData();
                dialogData.paymentMethod = method;
                dialogData.name = method.name;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: paymentMethodDeleteDialogConfirm,
                    dialogData: dialogData
                });
            }

            // Initializes the controller
            init();

    }]);

/**
 * @ngdoc service
 * @name Merchello.Backoffice.ShippingProvidersController
 *
 * @description
 * The controller for the shipment provider view
 */
angular.module('merchello').controller('Merchello.Backoffice.ShippingProvidersController',
    ['$scope', 'notificationsService', 'dialogService', 'merchelloTabsFactory',
    'settingsResource', 'warehouseResource', 'shippingGatewayProviderResource', 'dialogDataFactory',
    'settingDisplayBuilder', 'warehouseDisplayBuilder', 'warehouseCatalogDisplayBuilder', 'countryDisplayBuilder',
        'shippingGatewayProviderDisplayBuilder', 'shipCountryDisplayBuilder',
    function($scope, notificationsService, dialogService, merchelloTabsFactory,
             settingsResource, warehouseResource, shippingGatewayProviderResource, dialogDataFactory,
             settingDisplayBuilder, warehouseDisplayBuilder, warehouseCatalogDisplayBuilder, countryDisplayBuilder,
             shippingGatewayProviderDisplayBuilder, shipCountryDisplayBuilder) {

        $scope.loaded = true;
        $scope.tabs = [];
        $scope.countries = [];
        $scope.warehouses = [];
        $scope.primaryWarehouse = {};
        $scope.selectedCatalog = {};
        $scope.providers = [];
        $scope.primaryWarehouseAddress = {};
        $scope.visible = {
            catalogPanel: true,
            shippingMethodPanel: true,
            warehouseInfoPanel: false,
            warehouseListPanel: true
        };

        // exposed methods
        $scope.loadCountries = loadCountries;
        $scope.addCountry = addCountry;
        $scope.deleteCountryDialog = deleteCountryDialog;
        $scope.addEditWarehouseDialogOpen = addEditWarehouseDialogOpen;
        $scope.addEditWarehouseCatalogDialogOpen = addEditWarehouseCatalogDialogOpen;
        $scope.changeSelectedCatalogOpen = changeSelectedCatalogOpen;
        $scope.deleteWarehouseCatalogOpen = deleteWarehouseCatalogOpen;

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name init
         * @function
         *
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
        function init() {
            loadWarehouses();
            $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
            $scope.tabs.setActive('shipping');
        }

        /**
         * @ngdoc method
         * @name loadWarehouses
         * @function
         *
         * @description
         * Load the warehouses from the warehouse service, then wrap the results
         * in Merchello models and add to the scope via the warehouses collection.
         * Once loaded, it calls the loadCountries method.
         */
        function loadWarehouses() {
            var promiseWarehouses = warehouseResource.getDefaultWarehouse(); // Only a default warehouse in v1
            promiseWarehouses.then(function (warehouses) {
                $scope.warehouses.push(warehouseDisplayBuilder.transform(warehouses));
                changePrimaryWarehouse();
                loadCountries();
                //loadAllShipProviders();
            }, function (reason) {
                notificationsService.error("Warehouses Load Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name loadCountries
         * @function
         *
         * @description
         * Load the countries from the shipping service, then wrap the results
         * in Merchello models and add to the scope via the countries collection.
         * Once loaded, it calls the loadCountryProviders method for each
         * country.
         */
        function loadCountries() {
            if ($scope.primaryWarehouse.warehouseCatalogs.length > 0) {
                var catalogKey = $scope.selectedCatalog.key;
                var promiseShipCountries = shippingGatewayProviderResource.getWarehouseCatalogShippingCountries(catalogKey);
                promiseShipCountries.then(function (shipCountries) {
                    var transformed = shipCountryDisplayBuilder.transform(shipCountries);
                    $scope.countries = _.sortBy(
                        transformed, function(country) {
                        return country.name;
                    });
                    var elseCountry = _.find($scope.countries, function(country) {
                        return country.countryCode === 'ELSE';
                    });
                    if(elseCountry !== null && elseCountry !== undefined) {
                        $scope.countries = _.reject($scope.countries, function(country) {
                            return country.countryCode === 'ELSE';
                        });
                        $scope.countries.push(elseCountry);
                    }
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Shipping Countries Load Failed", reason.message);
                });
            }
        }

        //--------------------------------------------------------------------------------------
        // Helper methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name changePrimaryWarehouse
         * @function
         *
         * @description
         * Helper method to set the primary warehouse on the scope and to make sure the isDefault flag on
         * all warehouses is set properly.  If a warehouse is passed in, then it will find that warehouse
         * and set it as the primary and set isDefault to true.  All other warehouses will have their
         * isDefault flag reset to false.  If no warehouse is passed in (usually on loading data) then the
         * warehouse that has the isDefault == true will be set as the primary warehouse on the scope.
         */
        function changePrimaryWarehouse(warehouse) {
            $scope.primaryWarehouse = _.find($scope.warehouses, function(warehouse) {
                   return warehouse.isDefault;
            });
            $scope.primaryWarehouseAddress = $scope.primaryWarehouse.getAddress();
            //changeSelectedCatalog();
            $scope.selectedCatalog = _.find($scope.primaryWarehouse.warehouseCatalogs, function(catalog) {
                return catalog.isDefault === true;
            });
        }

        /**
         * @ngdoc method
         * @name changeSelectedCatalog
         * @function
         *
         * @description
         *
         */
        function changeSelectedCatalogOpen() {
            var dialogData = dialogDataFactory.createChangeWarehouseCatalogDialogData();
            dialogData.warehouse = $scope.primaryWarehouse;
            dialogData.selectedWarehouseCatalog = $scope.selectedCatalog;
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.warehousecatalog.select.html',
                show: true,
                callback: selectCatalogDialogConfirm,
                dialogData: dialogData
            });
        }


        //--------------------------------------------------------------------------------------
        // Dialog methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name addCountry
         * @function
         *
         * @description
         * Opens the add country dialog via the Umbraco dialogService.
         */
        function addCountry() {
            var promiseAllCountries = settingsResource.getAllCountries();
            promiseAllCountries.then(function (allCountries) {
                var countries = countryDisplayBuilder.transform(allCountries);

                // Add Everywhere Else as an option
                var elseCountry = countryDisplayBuilder.createDefault();
                elseCountry.key = '7501029f-5ab3-4733-935d-1dd37b37bf8';
                elseCountry.countryCode = 'ELSE';
                // TODO this should be localized
                elseCountry.name = 'Everywhere Else';
                countries.push(elseCountry);

                // we only want available countries that are not already in use
                var availableCountries = _.sortBy(
                    _.filter(countries, function(country) {
                        var found = _.find($scope.countries, function(assigned) {
                            return country.countryCode === assigned.countryCode;
                        });
                        return found === undefined || found === null;
                    }), function(country) {
                        return country.name;
                    });

                // construct the dialog data for the add ship country dialog
                var dialogData = dialogDataFactory.createAddShipCountryDialogData();
                dialogData.availableCountries = availableCountries;
                dialogData.selectedCountry = availableCountries[0];

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.addcountry.html',
                    show: true,
                    callback: addCountryDialogConfirm,
                    dialogData: dialogData
                });

            }, function (reason) {
                notificationsService.error("Available Countries Load Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name addCountryDialogConfirm
         * @function
         *
         * @description
         * Handles the save after recieving the country to add from the dialog view/controller
         */
        function addCountryDialogConfirm(dialogData) {
            $scope.preValuesLoaded = false;
            var catalogKey = $scope.selectedCatalog.key;
            var promiseShipCountries = shippingGatewayProviderResource.newWarehouseCatalogShippingCountry(catalogKey, dialogData.selectedCountry.countryCode);
            promiseShipCountries.then(function () {
                loadCountries()
            }, function (reason) {
                notificationsService.error("Shipping Countries Create Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name addEditWarehouseDialogOpen
         * @function
         *
         * @description
         * Handles opening a dialog for adding or editing a warehouse
         */
        function addEditWarehouseDialogOpen(warehouse) {
            // todo this will need to be refactored once we expose multiple warehouse
            var dialogData = dialogDataFactory.createAddEditWarehouseDialogData();
            dialogData.warehouse = warehouse;
            var promise = settingsResource.getAllCountries();
            promise.then(function(countries) {
                dialogData.availableCountries  = countryDisplayBuilder.transform(countries);
                dialogData.selectedCountry = _.find(dialogData.availableCountries, function(country) {
                    return country.countryCode === warehouse.countryCode;
                });
                if (dialogData.selectedCountry === undefined) {
                    dialogData.selectedCountry = dialogData.availableCountries[0];
                }
                if(dialogData.selectedCountry.provinces.length > 0) {
                    dialogData.selectedProvince = _.find(dialogData.selectedCountry.provinces, function(province) {
                        return province.code === warehouse.region;
                    });
                }
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.warehouse.addedit.html',
                    show: true,
                    callback: addEditWarehouseDialogConfirm,
                    dialogData: dialogData
                });
            });
        }

        /**
         * @ngdoc method
         * @name addEditWarehouseDialogConfirm
         * @function
         *
         * @description
         * Handles the saving of warehouse information
         */
        function addEditWarehouseDialogConfirm(dialogData) {
            $scope.preValuesLoaded = false;
            var promise = warehouseResource.save(dialogData.warehouse);
            promise.then(function() {
              loadWarehouses();
            });
        }

        /**
         * @ngdoc method
         * @name addEditWarehouseCatalogDialogOpen
         * @function
         *
         * @description
         * Opens the warehouse catalog dialog via the Umbraco dialogService.
         */
        function addEditWarehouseCatalogDialogOpen(catalog) {
            var dialogData = dialogDataFactory.createAddEditWarehouseCatalogDialogData();
            dialogData.warehouse = $scope.primaryWarehouse;
            if(catalog === undefined || catalog === null) {
                dialogData.warehouseCatalog = warehouseCatalogDisplayBuilder.createDefault();
                dialogData.warehouseCatalog.warehouseKey = dialogData.warehouse.key;
            } else {
                dialogData.warehouseCatalog = catalog;
            }

            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/shipping.warehousecatalog.addedit.html',
                show: true,
                callback: warehouseCatalogDialogConfirm,
                dialogData: dialogData
            });

        }

        /**
         * @ngdoc method
         * @name deleteCatalog
         * @function
         *
         * @description
         * Opens the delete catalog dialog via the Umbraco dialogService.
         */
        function deleteWarehouseCatalogOpen() {
            var dialogData = dialogDataFactory.createDeleteWarehouseCatalogDialogData();
            dialogData.name = $scope.selectedCatalog.name;
            dialogData.warehouseCatalog = $scope.selectedCatalog;
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                show: true,
                callback: deleteWarehouseCatalogConfirm,
                dialogData: dialogData
            });
        }

        /**
         * @ngdoc method
         * @name deleteCatalogConfirm
         * @function
         *
         * @description
         * Handles the delete after recieving the catalog to delete from the dialog view/controller
         */
        function deleteWarehouseCatalogConfirm(dialogData) {
            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            if(dialogData.warehouseCatalog.isDefault === false)
            {
                var promiseDeleteCatalog = warehouseResource.deleteWarehouseCatalog(dialogData.warehouseCatalog.key);
                promiseDeleteCatalog.then(function (responseCatalog) {
                    $scope.warehouses = [];
                    init();
                }, function (reason) {
                    notificationsService.error('Catalog Delete Failed', reason.message);
                });
            } else {
                notificationsService.warning('Cannot delete the default catalog.')
            }
        }

        /**
         * @ngdoc method
         * @name addCountry
         * @function
         *
         * @description
         * Opens the add country dialog via the Umbraco dialogService.
         */
        function deleteCountryDialog(country) {
            var dialogData = dialogDataFactory.createDeleteShipCountryDialogData();
            dialogData.country = country;
            dialogData.name = country.name;
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                show: true,
                callback: deleteCountryDialogConfirm,
                dialogData: dialogData
            });
        }

        /**
         * @ngdoc method
         * @name deleteCountry
         * @function
         *
         * @description
         * Calls the shipping service to delete the country passed in via the country parameter.
         * When complete, the countries are reloaded from the api to get the latest from the database.
         *
         */
        function deleteCountryDialogConfirm(dialogData) {
            $scope.preValuesLoaded = true;
            var promiseDelete = shippingGatewayProviderResource.deleteShipCountry(dialogData.country.key);
            promiseDelete.then(function () {
                notificationsService.success("Shipping Country Deleted");
                loadCountries();
            }, function (reason) {
                notificationsService.error("Shipping Country Delete Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name selectCatalogDialogConfirm
         * @function
         *
         * @description
         * Handles the catalog selection after recieving the dialogData from the dialog view/controller
         */
        function selectCatalogDialogConfirm(dialogData) {
            $scope.preValuesLoaded = false;
            $scope.selectedCatalog = _.find($scope.primaryWarehouse.warehouseCatalogs, function(catalog) {
                return catalog.key === dialogData.selectedWarehouseCatalog.key;
            });

            // Load the countries associated with this catalog.
            loadCountries();
        }

        /**
         * @ngdoc method
         * @name warehouseCatalogDialogConfirm
         * @function
         *
         * @description
         * Handles the add/edit after recieving the dialogData from the dialog view/controller.
         * If the selectedCatalog is set to be default, ensure that original default is no longer default.
         */
        function warehouseCatalogDialogConfirm(dialogData) {
            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            var promiseUpdateCatalog;
            if (dialogData.warehouseCatalog.key === "") {
                promiseUpdateCatalog = warehouseResource.addWarehouseCatalog(dialogData.warehouseCatalog);
            } else {
                promiseUpdateCatalog = warehouseResource.putWarehouseCatalog(dialogData.warehouseCatalog);
            }
            promiseUpdateCatalog.then(function(response) {
                $scope.warehouses = [];
                init();
            }, function(reason) {
                notificationsService.error('Catalog Update Failed', reason.message);
            });
        }

        // initialize the controller
        init();


    }]);

angular.module('merchello').controller('Merchello.Backoffice.TaxationProvidersController',
    ['$scope', '$timeout', 'settingsResource', 'notificationsService', 'dialogService', 'taxationGatewayProviderResource', 'dialogDataFactory', 'merchelloTabsFactory',
        'settingDisplayBuilder', 'countryDisplayBuilder', 'taxCountryDisplayBuilder',
        'gatewayResourceDisplayBuilder', 'taxationGatewayProviderDisplayBuilder', 'taxMethodDisplayBuilder',
    function($scope, $timeout, settingsResource, notificationsService, dialogService, taxationGatewayProviderResource, dialogDataFactory, merchelloTabsFactory,
             settingDisplayBuilder, countryDisplayBuilder, taxCountryDisplayBuilder,
             gatewayResourceDisplayBuilder, taxationGatewayProviderDisplayBuilder, taxMethodDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.tabs = [];
        $scope.allCountries = [];
        $scope.availableCountries = [];
        $scope.taxationGatewayProviders = [];
        $scope.settings = {};

        // exposed methods
        $scope.ensureSingleProductTaxMethod = ensureSingleProductTaxMethod;
        $scope.save = save;
        $scope.editTaxMethodProvinces = editTaxMethodProvinces;

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name init
         * @function
         *
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
        function init() {
            $scope.availableCountries = [];
            $scope.taxationGatewayProviders = [];
            loadSettings();
            $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
            $scope.tabs.setActive('taxation');
            _.sortBy($scope.availableCountries, function(country) {
                return country.name;
            });
        }

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         *
         * @description - Load the Merchello settings.
         */
        function loadSettings() {
            var settingsPromise = settingsResource.getAllSettings();
            settingsPromise.then(function(settings) {
                $scope.settings = settings;
                loadAllAvailableCountries();
            }, function(reason) {
                notificationsService.error('Failed to load global settings', reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name loadAllAvailableCountries
         * @function
         *
         * @description
         * Method called on initial page load.  Loads in data from server and sets up scope.
         */
        function loadAllAvailableCountries() {
            var promiseAllCountries = settingsResource.getAllCountries();
            promiseAllCountries.then(function (allCountries) {
                $scope.allCountries = countryDisplayBuilder.transform(allCountries);
                loadAllTaxationGatewayProviders();
            }, function (reason) {
                notificationsService.error("Available Countries Load Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name loadAllTaxationGatewayProviders
         * @function
         *
         * @description
         * Loads all taxation gateway providers.
         */
        function loadAllTaxationGatewayProviders() {

            var promiseAllProviders = taxationGatewayProviderResource.getAllGatewayProviders();
            promiseAllProviders.then(function (allProviders) {
                $scope.taxationGatewayProviders = taxationGatewayProviderDisplayBuilder.transform(allProviders);

                var noTaxProvider = taxationGatewayProviderDisplayBuilder.createDefault();
                noTaxProvider.key = -1;
                noTaxProvider.name = 'Not Taxed';

                if($scope.taxationGatewayProviders.length > 0) {
                    for(var i = 0; i < $scope.taxationGatewayProviders.length; i++) {
                        loadAvailableCountriesWithoutMethod($scope.taxationGatewayProviders[i], noTaxProvider);
                    }
                }
                $scope.taxationGatewayProviders.unshift(noTaxProvider);

            }, function (reason) {
                notificationsService.error("Available Taxation Providers Load Failed", reason.message);
            });
        }

        function loadAvailableCountriesWithoutMethod(taxationGatewayProvider, noTaxProvider) {
            var promiseGwResources = taxationGatewayProviderResource.getGatewayResources(taxationGatewayProvider.key);
            promiseGwResources.then(function (resources) {
                var newAvailableCountries = _.map(resources, function (resourceFromServer) {
                    var taxCountry = taxCountryDisplayBuilder.transform(resourceFromServer);
                    taxCountry.country = _.find($scope.allCountries, function (c) { return c.countryCode == taxCountry.gatewayResource.serviceCode; });
                    if (taxCountry.country) {
                        taxCountry.setCountryName(taxCountry.country.name);
                    } else {
                        if (taxCountry.gatewayResource.serviceCodeStartsWith('ELSE')) {
                            taxCountry.country = countryDisplayBuilder.createDefault();
                            taxCountry.setCountryName('Everywhere Else');
                            taxCountry.country.countryCode = 'ELSE';
                            taxCountry.sortOrder = 9999;
                        } else {
                            taxCountry.setCountryName(taxCountry.name);
                        }
                    }
                    return taxCountry;
                });
                angular.forEach(newAvailableCountries, function(country) {
                    country.taxMethod = taxMethodDisplayBuilder.createDefault();
                    country.provider = noTaxProvider;
                    country.taxMethod.providerKey = '-1';
                    $scope.availableCountries.push(country);
                });
                loadTaxMethods(taxationGatewayProvider);
                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {
                notificationsService.error("Available Gateway Resources Load Failed", reason.message);
            });
        }

        function loadTaxMethods(taxationGatewayProvider) {

            var promiseGwResources = taxationGatewayProviderResource.getTaxationProviderTaxMethods(taxationGatewayProvider.key);
            promiseGwResources.then(function (methods) {

                var newCountries = _.map(methods, function(methodFromServer) {
                    var taxMethod = taxMethodDisplayBuilder.transform(methodFromServer);
                    var taxCountry = taxCountryDisplayBuilder.createDefault();
                    taxCountry.addTaxesToProduct = taxMethod.productTaxMethod;
                    taxCountry.provider = taxationGatewayProvider;
                    taxCountry.taxMethod = taxMethod;
                    taxCountry.taxMethod.providerKey = taxationGatewayProvider.key;
                    if(taxCountry.taxMethod.countryCode === 'ELSE') {
                        taxCountry.country = countryDisplayBuilder.createDefault();
                        taxCountry.country.name = 'Everywhere Else';
                        taxCountry.country.countryCode = 'ELSE';
                    } else {
                        taxCountry.country = _.find($scope.allCountries, function(c) { return c.countryCode == taxMethod.countryCode; });
                    }
                    if (taxCountry.country) {
                        taxCountry.setCountryName(taxCountry.country.name);
                    } else {
                        taxCountry.setCountryName(taxMethod.name);
                    }
                    return taxCountry;
                });

                _.each(newCountries, function(country) {
                    if (country.country.countryCode === 'ELSE') {
                        country.sortOrder = 9999;
                    }
                    $scope.availableCountries.push(country);
                });


                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {
                notificationsService.error("Available Gateway Resources Load Failed", reason.message);
            });

        }


        //--------------------------------------------------------------------------------------
        // Events methods
        //--------------------------------------------------------------------------------------
        function ensureSingleProductTaxMethod(taxCountry) {
            angular.forEach($scope.availableCountries, function(tc) {
                if (tc.country.countryCode !== taxCountry.country.countryCode) {
                    tc.addTaxesToProduct = false;
                }
            });
        }

        function save() {
            $scope.preValuesLoaded = false;
            angular.forEach($scope.availableCountries, function(country) {
                if(country.provider.key === -1) {
                    // delete the provider
                    if(country.taxMethod.key !== '') {
                        var promiseDelete = taxationGatewayProviderResource.deleteTaxMethod(country.taxMethod.key);
                        promiseDelete.then(function() {
                            notificationsService.success("TaxMethod Removed", "");
                        }, function(reason) {
                            notificationsService.error("TaxMethod Save Failed", reason.message);
                        });
                    }
                } else {
                    if(country.taxMethod.providerKey !== country.provider.key) {
                            country.taxMethod.providerKey = country.provider.key;
                            country.taxMethod.countryCode = country.country.countryCode;
                            country.taxMethod.productTaxMethod = country.addTaxesToProduct;
                            var promiseSave = taxationGatewayProviderResource.addTaxMethod(country.taxMethod);
                            promiseSave.then(function() {
                                notificationsService.success("TaxMethod Saved", "");
                            }, function(reason) {
                                notificationsService.error("TaxMethod Save Failed", reason.message);
                        });
                    } else {
                        country.taxMethod.productTaxMethod = country.addTaxesToProduct;
                        var promiseSave = taxationGatewayProviderResource.saveTaxMethod(country.taxMethod);
                        promiseSave.then(function() {
                            notificationsService.success("TaxMethod Saved", "");
                        }, function(reason) {
                            notificationsService.error("TaxMethod Save Failed", reason.message);
                        });
                    }
                }
            });
            $timeout(function() {
                init();
            }, 400);
        }

        //--------------------------------------------------------------------------------------
        // Dialogs
        //--------------------------------------------------------------------------------------

        function taxMethodDialogConfirm(dialogData) {
            var promiseSave;

            // Save existing method
            promiseSave = taxationGatewayProviderResource.saveTaxMethod(dialogData.country.taxMethod);
            promiseSave.then(function () {
                notificationsService.success("Taxation Method Saved");
            }, function (reason) {
                notificationsService.error("Taxation Method Save Failed", reason.message);
            });
        }

        function editTaxMethodProvinces(country) {
            if (country) {
                var dialogData = dialogDataFactory.createEditTaxCountryDialogData();
                dialogData.country = country;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/dialogs/taxation.edittaxmethod.html',
                    show: true,
                    callback: taxMethodDialogConfirm,
                    dialogData: dialogData
                });
            }
        }

        // Initialize the controller
        init();
}]);

angular.module('merchello').controller('Merchello.Notes.Dialog.NoteAddEditController',
    ['$scope',
    function($scope) {
        $scope.wasFormSubmitted = false;
        $scope.rteProperties = {
            label: 'bodyText',
            view: 'rte',
            config: {
                editor: {
                    toolbar: ["bold", "italic", "bullist", "numlist", "link"],
                    stylesheets: [],
                    dimensions: { height: 350 }
                }
            },
            value: ""
        };

        function init() {
            if ($scope.dialogData.note.message !== '') {
                $scope.rteProperties.value = $scope.dialogData.note.message;
            }
        }

        $scope.save = function() {
            $scope.wasFormSubmitted = true;

            if ($scope.rteProperties.value !== '') {
                $scope.dialogData.note.message = $scope.rteProperties.value;
                $scope.submit($scope.dialogData);
            } else {
                $scope.addEditNoteForm.$valid = false;
            }
        }

        init();
}]);

angular.module('merchello').controller('Merchello.ProductOption.Dialogs.ProductOptionAddController',
    ['$scope', '$q', 'eventsService', 'merchelloTabsFactory', 'localizationService',
    function($scope, $q, eventsService, merchelloTabsFactory, localizationService) {

        $scope.tabs = [];

        $scope.visiblePanel = 'newoption';

        var newEvent = 'merchNewProductOptionSave';
        var sharedEvent = 'merchSharedProductOptionSave';

        if ($scope.dialogData.showTabs) {

            $scope.tabs = merchelloTabsFactory.createProductOptionAddTabs();

            $q.all([
                localizationService.localize('merchelloProductOptions_newOption'),
                localizationService.localize('merchelloProductOptions_sharedOption')
                ]).then(function(data) {

                $scope.tabs.addActionTab(
                    'newOption',
                    data[0],
                    function() {
                        $scope.tabs.setActive('newOption');
                        $scope.visiblePanel = 'newoption';
                    });

                $scope.tabs.addActionTab(
                    'sharedOption',
                    data[1],
                    function() {
                        $scope.tabs.setActive('sharedOption');
                        $scope.visiblePanel = 'sharedoption';
                    });

                $scope.tabs.setActive('newOption');
            });

        }


        $scope.validate = function() {

            var validation = { valid: false };

            if ($scope.visiblePanel === 'newoption') {
                eventsService.emit(newEvent, validation);
            } else if ($scope.visiblePanel === 'sharedoption') {
                eventsService.emit(sharedEvent, validation);
            }

            if (validation.valid) {
                $scope.submit($scope.dialogData);
            }
        }

}]);

angular.module('merchello').controller('Merchello.ProductOption.Dialogs.ProductOptionChoiceContentController',
    ['$scope', '$timeout', 'editorState', 'merchelloTabsFactory', 'contentResource', 'productOptionResource',
        'detachedContentResource', 'detachedContentHelper', 'fileManager',
    function($scope, $timeout, editorState, merchelloTabsFactory, contentResource, productOptionResource,
             detachedContentResource, detachedContentHelper, fileManager) {

        $scope.currentTabs = undefined;
        $scope.tabs = [];
        $scope.preValuesLoaded = false;
        $scope.choiceName = '';

        var umbracoTabs = [];

        var editor = {
            detachedContentType: null,
            scaffold: null,
            ready: function(dtc) {
                return this.detachedContentType !== null && dtc !== null && this.detachedContentType.key === dtc.key;
            }
        };

        $scope.save = function() {
            var args = {
                saveMethod: productOptionResource.saveAttributeContent,
                content: $scope.dialogData.choice,
                contentType: editor.detachedContentType,
                scope: $scope,
                statusMessage: 'Saving...'
            };

            detachedContentHelper.attributeContentPerformSave(args).then(function(att) {
                $scope.dialogData.choice = att;
                $scope.submit($scope.dialogData);
            });
        }

        function init() {
            editor.detachedContentType = $scope.dialogData.contentType;

            $scope.tabs = merchelloTabsFactory.createDefault();

            contentResource.getScaffold(-20, editor.detachedContentType.umbContentType.alias).then(function(scaffold) {
                editor.scaffold = scaffold;
                filterTabs(scaffold);
                fillValues();
                if (umbracoTabs.length > 0) {
                    switchTab(umbracoTabs[0]);
                }
                $scope.preValuesLoaded = true;
            });

            $scope.choiceName = $scope.dialogData.choice.name;
        }


        function filterTabs(scaffold) {
            $scope.contentTabs = _.filter(scaffold.tabs, function(t) { return t.id !== 0 });
            if ($scope.contentTabs.length > 0) {
                angular.forEach($scope.contentTabs, function(ct) {
                    $scope.tabs.addActionTab(ct.id, ct.label, switchTab);
                    umbracoTabs.push(ct.id);
                });
            }
        }

        function fillValues() {

            if ($scope.contentTabs.length > 0) {
                angular.forEach($scope.contentTabs, function(ct) {
                    angular.forEach(ct.properties, function(p) {
                        var stored = $scope.dialogData.choice.detachedDataValues.getValue(p.alias);
                        if (stored !== '') {
                            try {
                                p.value = angular.fromJson(stored);
                            }
                            catch (e) {
                                // Hack fix for some property editors
                                p.value = '';
                            }
                        }
                    });
                });
            }
        }

        function switchTab(id) {
            var exists = _.find(umbracoTabs, function(ut) {
                return ut === id;
            });
            if (exists !== undefined) {
                var fnd = _.find($scope.contentTabs, function (ct) {
                    return ct.id === id;
                });
                $scope.currentTab = fnd;
                $scope.tabs.setActive(id);
            }
        }


        init();

}]);

angular.module('merchello').controller('Merchello.ProductOption.Dialogs.ProductOptionEditController',
    ['$scope', 'eventsService',
    function($scope, eventsService) {

        $scope.visiblePanel = $scope.dialogData.sharedOptionEditor ? 'sharedoption' : 'newoption';

        var newName = 'merchNewProductOptionSave';
        var sharedEvent = 'merchSharedProductOptionSave';


        $scope.validate = function() {
            var validation = { valid: false };

            if ($scope.visiblePanel === 'newoption') {
                eventsService.emit(newName, validation);
            } else if ($scope.visiblePanel === 'sharedoption') {
                eventsService.emit(sharedEvent, validation);
            }

            if (validation.valid) {
                $scope.submit($scope.dialogData);
            }
        }

}]);
angular.module('merchello').controller('Merchello.Backoffice.SharedProductOptionsController',
    ['$scope','$log', '$q', 'merchelloTabsFactory', 'localizationService', 'productOptionResource', 'dialogDataFactory', 'dialogService',
        'merchelloListViewHelper',
    function($scope, $log, $q, merchelloTabsFactory, localizationService, productOptionResource, dialogDataFactory, dialogService, merchelloListViewHelper) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

        $scope.tabs = [];

        // In the initial release of this feature we are only going to allow sharedOnly params
        // to be managed here.  We may open this up at a later date depending on feedback.
        $scope.sharedOnly = true;

        function init() {
            $scope.tabs = merchelloTabsFactory.createProductListTabs();
            $scope.tabs.setActive('sharedoptions');
        }


        $scope.load = function(query) {
            query.addSharedOptionOnlyParam($scope.sharedOnly);
            return productOptionResource.searchOptions(query);
        }

        // adds an option
        $scope.add = function(option) {
            // this is the toggle to relead in the directive
            $scope.preValuesLoaded = false;

            productOptionResource.addProductOption(option).then(function(o) {
               $scope.preValuesLoaded = true;
            });
        }

        $scope.edit = function(option) {
            // this is the toggle to relead in the directive
            $scope.preValuesLoaded = false;
            productOptionResource.saveProductOption(option).then(function(o) {
                $scope.preValuesLoaded = true;
            });
        }

        $scope.delete = function(option) {
            if (option.canBeDeleted()) {
                $scope.preValuesLoaded = false;

                productOptionResource.deleteProductOption(option).then(function() {
                   $scope.preValuesLoaded = true;
                });
            }
        }

        init();
    }]);

/**
 * @ngdoc controller
 * @name Merchello.Product.Dialogs.ProductAddController
 * @function
 *
 * @description
 * The controller for the adding new products and allows for associating product content types
 */
angular.module('merchello').controller('Merchello.Product.Dialogs.ProductAddController',
    ['$scope', '$q', '$location', 'notificationsService', 'navigationService', 'contentResource', 'productResource','warehouseResource', 'settingsResource',
        'detachedContentResource', 'productDisplayBuilder', 'productVariantDetachedContentDisplayBuilder', 'warehouseDisplayBuilder',
        function($scope, $q, $location, notificationsService, navigationService, contentResource, productResource, warehouseResource, settingsResource,
                 detachedContentResource, productDisplayBuilder, productVariantDetachedContentDisplayBuilder, warehouseDisplayBuilder) {
            $scope.loaded = false;
            $scope.wasFormSubmitted = false;
            $scope.name = '';
            $scope.sku = '';
            $scope.price = 0;
            $scope.contentType = {};
            $scope.checking = false;
            $scope.isUnique = true;
            $scope.product = {};
            $scope.productVariant = {};
            $scope.languages = [];
            $scope.settings = {};
            $scope.currencySymbol = '';
            $scope.defaultWarehouseCatalog = {};
            
            // grab the sku text box to test if sku is unique
            var input = angular.element( document.querySelector( '#sku' ) );
            var currentSku = '';

            $scope.save = function() {
                $scope.wasFormSubmitted = true;
                if ($scope.addProductForm.name.$valid && $scope.addProductForm.sku.$valid && $scope.isUnique) {
                    // verify we have a detached content type to add to the variant
                    if ($scope.contentType !== undefined && $scope.contentType.umbContentType !== undefined) {
                        createDetachedContent().then(function(detached) {
                            $scope.product.detachedContents = detached;
                            finalize();
                        });
                    } else {
                        finalize();
                    }
                }


            }

            function finalize() {
                $scope.product.shippable = $scope.settings.globalShippable;
                $scope.product.taxable = $scope.settings.globalTaxable;
                $scope.product.trackInventory = $scope.settings.globalTrackInventory;
                if($scope.product.shippable || $scope.product.trackInventory)
                {
                    $scope.product.ensureCatalogInventory($scope.defaultWarehouseCatalog.key);
                }

                // make the API call to create the product
                productResource.create($scope.product).then(function(np) {
                    navigationService.hideNavigation();
                    $location.url('/merchello/merchello/productedit/' + np.key, true);
                });
            }

            function init() {
                // we need to get all the languages configured in Umbraco so that we can
                // create detached content for each one. We also need the currency symbol so we 
                // can append it to the price textbox
                $q.all([
                    detachedContentResource.getAllLanguages(),
                    settingsResource.getAllCombined(),
                    warehouseResource.getDefaultWarehouse()
                ]).then(function(data) {
                    var langArray = [];
                    if (!angular.isArray(data[0])) {
                        langArray.push(data[0]);
                    } else {
                        langArray = _.sortBy(data[0], 'name');
                    }
                    $scope.languages = langArray;
                    $scope.currencySymbol = data[1].currencySymbol;
                    $scope.settings = data[1].settings;
                    var defaultWarehouse = warehouseDisplayBuilder.transform(data[2]);
                    $scope.defaultWarehouseCatalog = _.find(defaultWarehouse.warehouseCatalogs, function (dwc) { return dwc.isDefault; });
                    $scope.loaded = true;
                });

                $scope.product = productDisplayBuilder.createDefault();

                input.bind("keyup onfocusout", function (event) {
                    var code = event.which;
                    // alpha , numbers, ! and backspace

                    if ( code === 45 ||
                        (code >47 && code <58) ||
                        (code >64 && code <91) ||
                        (code >96 && code <123) || code === 33 || code == 8) {
                        $scope.$apply(function () {
                            if ($scope.product.sku !== '') {
                                checkUniqueSku($scope.product.sku);
                            }
                        });
                    } else {
                        event.preventDefault();
                    }
                });
            }

            function createDetachedContent() {
                
                var deferred = $q.defer();

                var detachedContents = [];
                // we need to add a new productvariant detached content for each language configured
                var isoCodes =  _.pluck($scope.languages, 'isoCode');
                contentResource.getScaffold(-20, $scope.contentType.umbContentType.alias).then(function(scaffold) {
                    var contentTabs = _.filter(scaffold.tabs, function(t) { return t.id !== 0 });
                    angular.forEach(isoCodes, function(cultureName) {
                        var productVariantContent = buildProductVariantDetachedContent(cultureName, $scope.contentType, contentTabs);
                        detachedContents.push(productVariantContent);
                    });

                    deferred.resolve(detachedContents);
                });
                
                return deferred.promise;
            }

            function buildProductVariantDetachedContent(cultureName, detachedContent, tabs) {
                var productVariantContent = productVariantDetachedContentDisplayBuilder.createDefault();
                productVariantContent.cultureName = cultureName;
                productVariantContent.detachedContentType = detachedContent;
                productVariantContent.canBeRendered = true;
                angular.forEach(tabs, function(tab) {
                    angular.forEach(tab.properties, function(prop) {
                        //productVariantContent.detachedDataValues.setValue(prop.alias, angular.toJson(prop.value));
                        productVariantContent.detachedDataValues.setValue(prop.alias, '');
                    })
                });
                return productVariantContent;
            }
            
            function checkUniqueSku(sku) {
                $scope.checking = true;
                if (sku === undefined || sku === '') {
                    $scope.checking = false;
                    $scope.isUnique = true;
                } else {

                    if (sku === currentSku) {
                        $scope.checking = false;
                        return true;
                    }
                    var checkPromise = productResource.getSkuExists(sku).then(function(exists) {
                            $scope.checking = false;
                            currentSku = sku;
                            $scope.isUnique = exists === 'false';
                            $scope.checking = false;
                    });
                }
            }

            init();
        }]);

angular.module('merchello').controller('Merchello.Product.Dialogs.ProductCopyController',
    ['$scope', 'productResource',
    function($scope, productResource) {

        $scope.wasFormSubmitted = false;
        $scope.save = save;

        $scope.checking = false;
        $scope.isUnique = true;

        var currentSku = '';

        var input = angular.element( document.querySelector( '#copysku' ) );


        function init() {

            input.bind("keyup onfocusout", function (event) {
                var code = event.which;
                // alpha , numbers, ! and backspace

                if ( code === 45 ||
                    (code >47 && code <58) ||
                    (code >64 && code <91) ||
                    (code >96 && code <123) || code === 33 || code == 8) {
                    $scope.$apply(function () {
                        if ($scope.dialogData.sku !== '') {
                            checkUniqueSku($scope.dialogData.sku);
                        }
                    });
                } else {
                    event.preventDefault();
                }
            });
        }

        function save() {
            $scope.wasFormSubmitted = true;
            if ($scope.copyProductForm.name.$valid && $scope.copyProductForm.copysku.$valid && $scope.isUnique) {

                $scope.submit($scope.dialogData);
            }
        }

        function checkUniqueSku(sku) {

            $scope.checking = true;
            if (sku === undefined || sku === '') {
                $scope.checking = false;
                $scope.isUnique = true;
            } else {

                if (sku === currentSku) {
                    $scope.checking = false;
                    return true;
                }
                var checkPromise = productResource.getSkuExists(sku).then(function(exists) {
                    $scope.checking = false;
                    currentSku = sku;
                    $scope.isUnique = exists === 'false';
                    $scope.checking = false;
                });
            }
        }

        init();
}]);

angular.module('merchello').controller('Merchello.Product.Dialogs.PickSpecFilterCollectionsController',
    ['$scope',
    function($scope) {

        $scope.save = function() {
            // collections that have been modified will have attribute collections marked selected

            angular.forEach($scope.dialogData.available, function(collection) {
                setIntendedAssociations(collection);
            });

            $scope.submit($scope.dialogData.associate);
        }

        function setIntendedAssociations(collection) {
            var atts = _.filter(collection.filters, function(att) {
                if (att.selected) return att;
            });

            if (atts && atts.length > 0) {
                // we have to add the spec collection itself to be associated for certain system queries
                $scope.dialogData.associate.push(collection.key);
                angular.forEach(atts, function(a) {
                   $scope.dialogData.associate.push(a.key);
                });
            }
        }
}]);

/**
 * @ngdoc controller
 * @name Merchello.Product.Dialogs.AddProductContentTypeController
 * @function
 *
 * @description
 * The controller for the adding product content types
 */
angular.module('merchello').controller('Merchello.Product.Dialogs.AddProductContentTypeController',
    ['$scope', '$location', 'notificationsService', 'navigationService', 'eventsService',  'detachedContentResource', 'detachedContentTypeDisplayBuilder',
        function($scope, $location, notificationsService, navigationService, eventsService, detachedContentResource, detachedContentTypeDisplayBuilder) {
            $scope.loaded = true;
            $scope.wasFormSubmitted = false;
            $scope.contentType = {};
            $scope.name = '';
            $scope.description = '';
            $scope.associateType = 'Product';
            var eventName = 'merchello.contenttypedropdown.changed';

            $scope.save = function() {
                $scope.wasFormSubmitted = true;
                if ($scope.productContentTypeForm.name.$valid && $scope.contentType.key) {

                    var dtc = detachedContentTypeDisplayBuilder.createDefault();
                    dtc.umbContentType = $scope.contentType;
                    dtc.entityType = $scope.associateType;
                    dtc.name = $scope.name;
                    dtc.description = $scope.description;

                    detachedContentResource.addDetachedContentType(dtc).then(function(result) {
                        notificationsService.success("Content Type Saved", "");
                        navigationService.hideNavigation();
                        $location.url("/merchello/merchello/productcontenttypelist/" + result.key, true);
                    }, function(reason) {
                        notificationsService.error('Failed to add detached content type ' + reason);
                    });
                }
            }

            function init() {

                eventsService.on(eventName, onSelectionChanged);
            }

            function onSelectionChanged(e, contentType) {
                if (contentType.name !== null && contentType.name !== undefined) {
                    $scope.name = contentType.name;
                }
            }

            init();
        }]);

    /**
     * @ngdoc controller
     * @name Merchello.Product.Dialogs.ProductVariantBulkChangePricesController
     * @function
     *
     * @description
     * The controller for the adding / editing Notification messages on the Notifications page
     */
    angular.module('merchello').controller('Merchello.Product.Dialogs.ProductVariantBulkChangePricesController',
        ['$scope',
        function($scope) {

    }]);

    /**
     * @ngdoc controller
     * @name Merchello.Product.Dialogs.ProductVariantBulkChangePricesController
     * @function
     *
     * @description
     * The controller for the adding / editing Notification messages on the Notifications page
     */
    angular.module('merchello').controller('Merchello.Product.Dialogs.ProductVariantBulkUpdateInventoryController',
        ['$scope',
        function($scope) {

            function init() {

            }

            // Initialize the controller
            init();

        }]);

/**
 * @ngdoc controller
 * @name Merchello.Directives.ProductVariantsViewTableDirectiveController
 * @function
 *
 * @description
 * The controller for the product variant view table view directive
 */
angular.module('merchello').controller('Merchello.Directives.ProductVariantsViewTableDirectiveController',
    ['$scope', '$q', '$timeout', '$location', 'notificationsService', 'dialogService', 'dialogDataFactory', 'productResource', 'productDisplayBuilder', 'productVariantDisplayBuilder',
    function($scope, $q, $timeout, $location, notificationsService, dialogService, dialogDataFactory, productResource, productDisplayBuilder, productVariantDisplayBuilder) {

        $scope.sortProperty = "sku";
        $scope.sortOrder = "asc";
        $scope.bulkAction = true;
        $scope.allVariants = false;

        $scope.checkAll = false;

        // exposed methods
        $scope.assertActiveShippingCatalog = assertActiveShippingCatalog;
        $scope.selectedVariants = selectedVariants;
        $scope.selectVariants = selectVariants;
        $scope.checkBulkVariantsSelected = checkBulkVariantsSelected;
        $scope.changeSortOrder = changeSortOrder;
        $scope.toggleAVariant = toggleAVariant;
        $scope.toggleAllVariants = toggleAllVariants;
        $scope.changePrices = changePrices;
        $scope.updateInventory = updateInventory;
        $scope.toggleOnSale = toggleOnSale;
        $scope.toggleAvailable = toggleAvailable;
        $scope.redirectToEditor = redirectToEditor;
        $scope.getVariantAttributeForOption = getVariantAttributeForOption;


        $scope.toggleChecks = function() {
            if ($scope.checkAll === true) {
                selectVariants('All');
            } else {
                selectVariants('None');
            }
        }


        function init() {
            angular.forEach($scope.product.productVariants, function(pv) {
                pv.selected = false;
            });
        }



        /**
         * @ngdoc method
         * @name selectVariants
         * @function
         *
         * @description
         * Called by the ProductOptionAttributesSelection directive when an attribute is selected.
         * It will select the variants that have that option attribute and mark their selected property to true.
         * All other variants will have selected set to false.
         *
         */
        function selectVariants(attributeToSelect) {
            // Reset the selected state to false for all variants
            for (var i = 0; i < $scope.product.productVariants.length; i++) {
                $scope.product.productVariants[i].selected = false;
            }

            // Build a list of variants to select
            var filteredVariants = [];

            if (attributeToSelect === "All") {
                filteredVariants = $scope.product.productVariants;
            } else if (attributeToSelect === "None") {
                filteredVariants = [];
            } else {
                filteredVariants = _.filter($scope.product.productVariants,
                    function (variant) {
                        return _.where(variant.attributes, { name: attributeToSelect }).length > 0;
                    });
            }

            // Set the selected state to true for all variants
            for (var v = 0; v < filteredVariants.length; v++) {
                filteredVariants[v].selected = true;
            }

            // Set the property to toggle in the bulk menu in the table header
            checkBulkVariantsSelected();
        }

        /**
         * @ngdoc method
         * @name selectedVariants
         * @function
         *
         * @description
         * This is a helper method to get a collection of variants that are selected.
         *
         */
        function selectedVariants() {
            if ($scope.product !== undefined) {
                return _.filter($scope.product.productVariants, function(v) {
                    return v.selected;
                });
            } else {
                return [];
            }
        }

        /**
         * @ngdoc method
         * @name checkBulkVariantsSelected
         * @function
         *
         * @description
         * This is a helper method to set the allVariants flag when one or more variants on selected
         * in the Variant Information table on the Variant tab.
         *
         */
        function checkBulkVariantsSelected() {
            var v = selectedVariants();
            if (v.length >= 1) {
                $scope.allVariants = true;
            } else {
                $scope.allVariants = false;
            }
        }

        /**
         * @ngdoc method
         * @name changeSortOrder
         * @function
         *
         * @description
         * Event handler for changing the rows sort by column when a header column item is clicked
         *
         */
        function changeSortOrder(propertyToSort) {

            if ($scope.sortProperty == propertyToSort) {
                if ($scope.sortOrder == "asc") {
                    $scope.sortProperty = "-" + propertyToSort;
                    $scope.sortOrder = "desc";
                } else {
                    $scope.sortProperty = propertyToSort;
                    $scope.sortOrder = "asc";
                }
            } else {
                $scope.sortProperty = propertyToSort;
                $scope.sortOrder = "asc";
            }
        }

        /**
         * @ngdoc method
         * @name toggleAVariant
         * @function
         *
         * @description
         * Event handler toggling the variant's selected state
         *
         */
        function toggleAVariant(variant) {
            variant.selected = !variant.selected;
            $scope.checkBulkVariantsSelected();
        }

        /**
         * @ngdoc method
         * @name toggleAllVariants
         * @function
         *
         * @description
         * Event handler toggling the all of the product variant's selected state
         *
         */
        function toggleAllVariants(newstate) {
            for (var i = 0; i < $scope.product.productVariants.length; i++) {
                $scope.product.productVariants[i].selected = newstate;
            }
            $scope.allVariants = newstate;
        }

        function getVariantAttributeForOption(productVariant, option) {
            var att = _.find(productVariant.attributes, function(pa) {
               return pa.optionKey === option.key;
            });

            if (att) {
                return att.name;
            } else {
                return '';
            }
        }

        /**
         * @ngdoc method
         * @name toggleAvailable
         * @function
         *
         * @description
         * Toggles the variant available setting
         */
        function toggleAvailable() {

            var promises = [];
            var selected = $scope.selectedVariants();
            for (var i = 0; i < selected.length; i++) {
                selected[i].available = !selected[i].available;
                promises.push(productResource.saveVariant(selected[i]));
            }
            persistPromises(promises, "Confirmed available update");
        }


        $scope.toggleTracksInventory = function() {
            var promises = [];
            var selected = $scope.selectedVariants();
            for(var i = 0; i < selected.length; i++) {
                selected[i].trackInventory = !selected[i].trackInventory;
                promises.push(productResource.saveVariant(selected[i]));
            }

            persistPromises(promises, "Confirmed tracks inventory update");

        }

        function persistPromises(promises, msg) {
            if (promises.length === 0) {
                return;
            }

            $q.all(promises).then(function(data) {
                notificationsService.success(msg, "");
                $scope.checkAll = false;
                selectVariants('None');
                $timeout(function() {
                    reload();
                }, 400);
            });
        }

        //--------------------------------------------------------------------------------------
        // Dialog Event Handlers
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name changePrices
         * @function
         *
         * @description
         * Opens the dialog for setting the new price
         */
        function changePrices() {
            var dialogData = dialogDataFactory.createBulkVariantChangePricesDialogData();
            dialogData.productVariants = $scope.selectedVariants();
            dialogData.price = _.min(dialogData.productVariants, function(v) { return v.price;}).price;
            dialogData.salePrice = _.min(dialogData.productVariants, function (v) { return v.salePrice; }).price;
            dialogData.costOfGoods = _.min(dialogData.productVariants, function (v) { return v.costOfGoods; }).costOfGoods;
            dialogData.currencySymbol = $scope.currencySymbol;
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/productvariant.bulk.changeprice.html',
                show: true,
                callback: changePricesDialogConfirm,
                dialogData: dialogData
            });
        }

        /**
         * @ngdoc method
         * @name updateInventory
         * @function
         *
         * @description
         * Opens the dialog for setting the new inventory
         */
        function updateInventory() {
            var dialogData = dialogDataFactory.createBulkEditInventoryCountsDialogData();
            dialogData.warning = 'Note: This will update the inventory for all warehouses on all selected variants';
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/productvariant.bulk.updateinventory.html',
                show: true,
                callback: updateInventoryDialogConfirm,
                dialogData: dialogData
            });
        }


        function toggleOnSale() {
            var promises = [];
            var selected = $scope.selectedVariants();
            for (var i = 0; i < selected.length; i++) {
                selected[i].onSale = !selected[i].onSale;
                promises.push(productResource.saveVariant(selected[i]));

            }
            persistPromises(promises, "Confirmed on sale update");
        }

        /**
         * @ngdoc method
         * @name updateInventoryDialogConfirm
         * @param {dialogData} contains the new inventory that is the price to adjust the variants price to.
         * @function
         *
         * @description
         * Handles the new inventory passed back from the dialog and sets the variants inventory and saves them.
         */
        function updateInventoryDialogConfirm(dialogData) {
            var promises = [];
            var selected = $scope.selectedVariants();
            for (var i = 0; i < selected.length; i++) {
                selected[i].setAllInventoryCount(dialogData.count);
                if(dialogData.includeLowCount) {
                    selected[i].setAllInventoryLowCount(dialogData.lowCount);
                }
                promises.push(productResource.saveVariant(selected[i]));
            }
            persistPromises(promises, "Confirmed inventory update");
        }

        /**
         * @ngdoc method
         * @name changePricesDialogConfirm
         * @param {dialogData} contains the newPrice that is the price to adjust the variants price to.
         * @function
         *
         * @description
         * Handles the new price passed back from the dialog and sets the variants price and saves them.
         */
        function changePricesDialogConfirm(dialogData) {
            angular.forEach(dialogData.productVariants,
                function(pv) {
                    pv.price = dialogData.price;
                    if (dialogData.includeCostOfGoods) {
                        pv.costOfGoods = dialogData.costOfGoods;
                    }
                    if (dialogData.includeSalePrice) {
                        pv.salePrice = dialogData.salePrice;
                    }
                    productResource.saveVariant(pv);
                });
            notificationsService.success("Updated prices");
        }

        function assertActiveShippingCatalog() {
            $scope.assertCatalog();
        }

        function reload() {
            $scope.reload();
        }

        function redirectToEditor(variant) {
           $location.url('/merchello/merchello/productedit/' + variant.productKey + '?variantid=' + variant.key, true);
        }

        // initialize the controller
        init();
}]);


angular.module('merchello').controller('Merchello.Directives.ProductVariantShippingDirectiveController',
    ['$scope', 'notificationsService', 'dialogService', 'warehouseResource', 'warehouseDisplayBuilder', 'catalogInventoryDisplayBuilder',
        function($scope, notificationsService, dialogService, warehouseResource, warehouseDisplayBuilder, catalogInventoryDisplayBuilder) {

            $scope.warehouses = [];
            $scope.defaultWarehouse = {};
            $scope.defaultWarehouseCatalog = {};

            // exposed methods
            $scope.getUnits = getUnits;
            $scope.mapToCatalog = mapToCatalog;
            $scope.toggleCatalog = toggleCatalog;

            function init() {
                loadAllWarehouses();
            }

            /**
             * @ngdoc method
             * @name loadAllWarehouses
             * @function
             *
             * @description
             * Loads in default warehouse and all other warehouses from server into the scope.  Called in init().
             */
            function loadAllWarehouses() {
                var promiseWarehouse = warehouseResource.getDefaultWarehouse();
                promiseWarehouse.then(function (warehouse) {
                    $scope.defaultWarehouse = warehouseDisplayBuilder.transform(warehouse);
                    $scope.warehouses.push($scope.defaultWarehouse);
                    $scope.defaultWarehouseCatalog = _.find($scope.defaultWarehouse.warehouseCatalogs, function (dc) { return dc.isDefault; });
                }, function (reason) {
                    notificationsService.error("Default Warehouse Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name mapToCatalog
             * @function
             *
             * @description
             * Maps a catalog to a product variant catalog inventory
             */
            function mapToCatalog(catalog) {
                var mapped = _.find($scope.productVariant.catalogInventories, function(ci) { return ci.catalogKey === catalog.key;});
                if(mapped === undefined) {
                    var catalogInventory = catalogInventoryDisplayBuilder.createDefault();
                    catalogInventory.productVariantKey = $scope.productVariant.key;
                    catalogInventory.catalogKey = catalog.key;
                    catalogInventory.active = false;
                    $scope.productVariant.catalogInventories.push(catalogInventory);
                    mapped = catalogInventory;
                }
                return mapped;
            }

            function toggleCatalog() {
                $scope.productVariant.ensureCatalogInventory($scope.defaultWarehouseCatalog.key);
            }

            function getUnits(settings, type) {
                if(settings.unitSystem === 'Imperial') {
                    return type === 'weight' ? '(pounds)' : '(inches)';
                } else {
                    return type === 'weight' ? '(kg)' : '(cm)';
                }
            }

            // Initializes the controller
            init();
}]);

angular.module('merchello').controller('Merchello.Backoffice.ProductContentTypeListController',
    ['$scope', 'merchelloTabsFactory',
    function($scope, merchelloTabsFactory) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;
        $scope.tabs = {};

        function init() {
            $scope.tabs = merchelloTabsFactory.createProductListTabs();
            $scope.tabs.setActive('contentTypeList');
        }


        // Initializes the controller
        init();
    }]);

angular.module('merchello').controller('Merchello.Backoffice.ProductDetachedContentController',
    ['$scope', '$q', '$log', '$route', '$routeParams', '$timeout', '$location', 'editorState', 'notificationsService',
        'dialogService', 'localizationService', 'merchelloTabsFactory', 'dialogDataFactory',
        'contentResource', 'detachedContentResource', 'productResource', 'settingsResource',
        'detachedContentHelper', 'productDisplayBuilder', 'productVariantDetachedContentDisplayBuilder',
        function($scope, $q, $log, $route, $routeParams, $timeout, $location, editorState, notificationsService,
                 dialogService, localizationService, merchelloTabsFactory, dialogDataFactory,
                 contentResource, detachedContentResource, productResource, settingsResource,
                 detachedContentHelper, productDisplayBuilder, productVariantDetachedContentDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
           // $scope.currentSection = appState.getSectionState("currentSection");
            $scope.productVariant = {};
            $scope.language = '';
            $scope.languages = [];
            $scope.isVariant = false;
            $scope.isConfigured = false;
            $scope.defaultLanguage = 'en-US';
            $scope.contentType = {};
            $scope.detachedContent = {};
            $scope.tabs = [];

            // Umbraco properties
            $scope.contentTabs = [];
            $scope.currentTab = null;

            $scope.openRemoveDetachedContentDialog = openRemoveDetachedContentDialog;

            // navigation switches
            var render = '';
            var slugLabel = '';
            var slugLabelDescription = '';
            var selectTemplateLabel = '';
            var canBeRenderedLabel = '';
            var showUmbracoTabs = true;
            var merchelloTabs = ['productcontent','variantlist', 'optionslist'];
            var umbracoTabs = [];

            $scope.save = save;
            $scope.saveContentType = createDetachedContent;
            $scope.setLanguage = setLanguage;

            var settings = {};
            var product = {};
            var loadArgs = {
                key: '',
                productVariantKey: ''
            };

            var editor = {
                detachedContentType: null,
                scaffold: null,
                ready: function(dtc) {
                    return this.detachedContentType !== null && dtc !== null && this.detachedContentType.key === dtc.key;
                }
            };

            // initialize
            function init() {
                var key = $routeParams.id;

                // extended content is not valid unless we have a product to attach it to
                if (key === '' || key === undefined) {
                    $location.url('/merchello/merchello/productlist/manage', true);
                }


                var productVariantKey = $routeParams.variantid;
                loadArgs.key = key;
                loadArgs.productVariantKey = productVariantKey;

                var deferred = $q.defer();
                $q.all([
                    settingsResource.getAllSettings(),
                    detachedContentResource.getAllLanguages(),
                    localizationService.localize('merchelloTabs_render'),
                    localizationService.localize('merchelloDetachedContent_slug'),
                    localizationService.localize('merchelloDetachedContent_slugDescription'),
                    localizationService.localize('merchelloDetachedContent_selectTemplate'),
                    localizationService.localize('merchelloDetachedContent_canBeRendered')
                ]).then(function(results) {
                    deferred.resolve(results);
                });

                deferred.promise.then(function(data) {
                    settings = data[0];
                    $scope.languages = _.sortBy(data[1], 'name');
                    $scope.defaultLanguage = settings.defaultExtendedContentCulture;
                    if($scope.defaultLanguage !== '' && $scope.defaultLanguage !== undefined) {
                        $scope.language = _.find($scope.languages, function(l) { return l.isoCode === $scope.defaultLanguage; });
                    }
                    render = data[2];
                    slugLabel = data[3];
                    slugLabelDescription = data[4];
                    selectTemplateLabel = data[5];
                    canBeRenderedLabel = data[6];

                    loadProduct(loadArgs);

                }, function(reason) {
                    notificationsService.error('Failed to load ' + reason);
                });

            }

            // loads the product from the resource
            function loadProduct(args) {
                
                productResource.getByKey(args.key).then(function(p) {

                    product = productDisplayBuilder.transform(p);
                    if(args.productVariantKey === '' || args.productVariantKey === undefined) {
                        // this is a product edit.
                        // we use the master variant context so that we can use directives associated with variants
                        $scope.productVariant = product.getMasterVariant();

                        if (!product.hasVariants()) {
                            $scope.tabs = merchelloTabsFactory.createProductEditorTabs(args.key);
                        }
                        else
                        {
                            $scope.tabs = merchelloTabsFactory.createProductEditorWithOptionsTabs(args.key);
                        }
                    } else {
                        // this is a product variant edit
                        // in this case we need the specific variant
                        $scope.productVariant = product.getProductVariant(args.productVariantKey);

                        $scope.tabs = merchelloTabsFactory.createProductVariantEditorTabs(args.key, args.productVariantKey);
                        $scope.isVariant = true;
                    }
                    
                    $scope.loaded = true;

                    if ($scope.productVariant.hasDetachedContent()) {

                        var missing = $scope.productVariant.assertLanguageContent(_.pluck($scope.languages, 'isoCode'));
                        if (missing.length > 0) {
                            var detachedContentType = $scope.productVariant.detachedContentType();
                            createDetachedContent(detachedContentType, missing);
                        }

                        $scope.detachedContent = $scope.productVariant.getDetachedContent($scope.language.isoCode);
                        
                        $scope.isConfigured = true;

                        loadScaffold();
                    } else {
                        $scope.tabs.setActive('productcontent');
                        $scope.preValuesLoaded = true;
                    }

                });
            }

            // The content type scaffold
            function loadScaffold() {

                // every detached content associated with a variant MUST share the same content type,
                var detachedContentType = $scope.productVariant.detachedContentType();

                if (!editor.ready(detachedContentType)) {

                    contentResource.getScaffold(-20, detachedContentType.umbContentType.alias).then(function(scaffold) {

                        editor.detachedContentType = detachedContentType;
                        editor.scaffold = scaffold;
                        //currentScaffold.filterTabs($scope.contentTabs, $scope.tabs);
                        //currentScaffold.fillValues($scope.tabs, $scope.detachedContent);
                        filterTabs(scaffold);
                        fillValues();

                        stickTab();

                        $scope.preValuesLoaded = true;
                    });
                } else {
                    filterTabs(editor.scaffold);
                    fillValues();
                    stickTab();
                    $scope.preValuesLoaded = true;
                }
            }

            function save() {
                if ($scope.productVariant.hasDetachedContent()) {
                    saveDetachedContent();
                }
            }

            function createDetachedContent(detachedContent, missing) {
                if(!$scope.productVariant.hasDetachedContent() || missing !== undefined) {
                    // create detached content values for each language present
                    var isoCodes = missing === undefined ?  _.pluck($scope.languages, 'isoCode') : missing;
                    contentResource.getScaffold(-20, detachedContent.umbContentType.alias).then(function(scaffold) {
                        filterTabs(scaffold);
                        angular.forEach(isoCodes, function(cultureName) {
                           var productVariantContent = buildProductVariantDetachedContent(cultureName, detachedContent, $scope.contentTabs);
                            $scope.productVariant.detachedContents.push(productVariantContent);
                        });

                        // we have to save here without assigning the scope.detachedContent otherwise we will only save the scaffold for the current language
                        // but the helper is expecting the scope value to be set.
                        createDetachedContentSave();
                    });
                }
            }

            // save when the language is changed
            function setLanguage(lang) {
                $scope.language = lang;
                $scope.contentTabs = [];
                umbracoTabs = [];
                $scope.currentTab = null;
                saveDetachedContent();
            }


            function createDetachedContentSave() {
                var promise;
                if ($scope.productVariant.master) {
                    promise = productResource.save(product);
                } else {
                    promise = productResource.saveVariant($scope.productVariant);
                }
                promise.then(function(data) {
                    $scope.loaded = false;
                    $scope.preValuesLoaded = false;
                    loadProduct(loadArgs);
                });
            }

            function saveDetachedContent() {
                var promise;
                if ($scope.productVariant.master) {
                    promise = detachedContentHelper.detachedContentPerformSave({ saveMethod: productResource.saveProductContent, content: product, scope: $scope, statusMessage: 'Saving...' });
                } else {
                    promise = detachedContentHelper.detachedContentPerformSave({ saveMethod: productResource.saveVariantContent, content: $scope.productVariant, scope: $scope, statusMessage: 'Saving...' });
                }
                promise.then(function(data) {
                    
                    $scope.loaded = false;
                    $scope.preValuesLoaded = false;

                    loadProduct(loadArgs);
                });
            }


            // TODO move this to a service
            function buildProductVariantDetachedContent(cultureName, detachedContent, tabs) {
                var productVariantContent = productVariantDetachedContentDisplayBuilder.createDefault();
                productVariantContent.cultureName = cultureName;
                productVariantContent.productVariantKey = $scope.productVariantKey;
                productVariantContent.detachedContentType = detachedContent;
                productVariantContent.canBeRendered = true;
                angular.forEach(tabs, function(tab) {
                    angular.forEach(tab.properties, function(prop) {
                        //productVariantContent.detachedDataValues.setValue(prop.alias, angular.toJson(prop.value));
                        productVariantContent.detachedDataValues.setValue(prop.alias, prop.value);
                    })
                });
                return productVariantContent;
            }


            function fillValues() {

                if ($scope.contentTabs.length > 0) {
                    angular.forEach($scope.contentTabs, function(ct) {
                        angular.forEach(ct.properties, function(p) {
                            var stored = $scope.detachedContent.detachedDataValues.getValue(p.alias);
                            if (stored !== '') {
                                try {
                                    p.value = angular.fromJson(stored);
                                }
                                catch (e) {
                                    // Hack fix for some property editors
                                    p.value = '';
                                }
                            }
                        });
                    });
                }
            }

            function stickTab() {
                if ($scope.contentTabs.length > 0) {
                    if ($scope.currentTab === null) {
                        $scope.currentTab = $scope.contentTabs[0];
                    }
                    setTabVisibility();
                }
                ensureRenderTab();
                $scope.tabs.setActive($scope.currentTab.id);
            }

            function ensureRenderTab() {
                // add the rendering tab
                if ($scope.productVariant.master) {
                    var umbContentType = $scope.detachedContent.detachedContentType.umbContentType;
                    var args = {
                        tabId: 'render',
                        tabAlias: render,
                        tabLabel: render,
                        slugLabel: slugLabel,
                        slugDescription: slugLabelDescription,
                        templateLabel: selectTemplateLabel,
                        slug: $scope.detachedContent.slug,
                        templateId: $scope.detachedContent.templateId,
                        allowedTemplates: umbContentType.allowedTemplates,
                        defaultTemplateId: umbContentType.defaultTemplateId,
                        canBeRenderedLabel: canBeRenderedLabel,
                        canBeRendered: $scope.detachedContent.canBeRendered
                    };

                    var rt = detachedContentHelper.buildRenderTab(args);
                    $scope.contentTabs.push(rt);
                    umbracoTabs.push(rt.id);
                    $scope.tabs.addActionTab(rt.id, rt.label, switchTab)
                }
            }
            function filterTabs(scaffold) {
                $scope.contentTabs = _.filter(scaffold.tabs, function(t) { return t.id !== 0 });
                if ($scope.contentTabs.length > 0) {
                    angular.forEach($scope.contentTabs, function(ct) {
                        $scope.tabs.addActionTab(ct.id, ct.label, switchTab);
                        umbracoTabs.push(ct.id);
                    });
                }
            }

            function setTabVisibility() {
                if (showUmbracoTabs) {
                    angular.forEach(merchelloTabs, function(mt) {
                      $scope.tabs.hideTab(mt);
                    });
                }
            }

            function switchTab(id) {
                var exists = _.find(umbracoTabs, function(ut) {
                    return ut === id;
                });
                if (exists !== undefined) {
                    var fnd = _.find($scope.contentTabs, function (ct) {
                        return ct.id === id;
                    });
                    $scope.currentTab = fnd;
                    $scope.tabs.setActive(id);
                }
            }

            function openRemoveDetachedContentDialog() {
                var deferred = $q.defer();
                $q.all([
                    localizationService.localize('merchelloTabs_detachedContent'),
                    localizationService.localize('merchelloDetachedContent_removeDetachedContentWarning')
                ]).then(function(data) {
                    deferred.resolve(data);
                });

                deferred.promise.then(function(value) {

                    var dialogData = {
                        name : $scope.productVariant.name + ' ' + value[0],
                        warning: value[1]
                    };

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                        show: true,
                        callback: processRemoveDetachedContent,
                        dialogData: dialogData
                    });
                });
            }

            function processRemoveDetachedContent(dialogData) {
                $scope.loaded = true;
                $scope.preValuesLoaded = false;
                productResource.deleteDetachedContent($scope.productVariant).then(function(result) {
                    $route.reload();
                }, function(reason) {
                    notificationsService.error('Failed to delete detached content ' + reason);
                });
            }

            // Initialize the controller
            init();
    }]);

    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ProductEditController
     * @function
     *
     * @description
     * The controller for product edit view
     */
    angular.module('merchello').controller('Merchello.Backoffice.ProductEditController',
        ['$scope', '$routeParams', '$window', '$location', '$timeout', 'assetsService', 'notificationsService', 'dialogService', 'merchelloTabsFactory', 'dialogDataFactory',
            'serverValidationManager', 'productResource', 'warehouseResource', 'settingsResource',
            'productDisplayBuilder', 'productVariantDisplayBuilder', 'warehouseDisplayBuilder', 'settingDisplayBuilder', 'catalogInventoryDisplayBuilder',
        function($scope, $routeParams, $window, $location, $timeout, assetsService, notificationsService, dialogService, merchelloTabsFactory, dialogDataFactory,
            serverValidationManager, productResource, warehouseResource, settingsResource,
            productDisplayBuilder, productVariantDisplayBuilder, warehouseDisplayBuilder, settingDisplayBuilder, catalogInventoryDisplayBuilder) {

            //--------------------------------------------------------------------------------------
            // Declare and initialize key scope properties
            //--------------------------------------------------------------------------------------
            //

            // To help umbraco directives show our page
            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
            $scope.entityType = 'product';


            // settings - contains defaults for the checkboxes
            $scope.settings = {};

            // this is for the slide panel directive to get rid of the close button since we'll
            // be handling it in a different way in this case
            $scope.hideClose = true;

            $scope.product = productDisplayBuilder.createDefault();
            $scope.productVariant = productVariantDisplayBuilder.createDefault();
            $scope.warehouses = [];
            $scope.defaultWarehouse = {};
            $scope.context = 'createproduct';

            // Exposed methods
            $scope.save = save;
            $scope.openCopyProductDialog = openCopyProductDialog;
            $scope.loadAllWarehouses = loadAllWarehouses;
            $scope.deleteProductDialog = deleteProductDialog;


            //--------------------------------------------------------------------------------------
            // Initialization methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                var key = $routeParams.id;
                var productVariantKey = $routeParams.variantid;
                loadSettings();
                loadAllWarehouses(key, productVariantKey);
            }

            /**
             * @ngdoc method
             * @name loadAllWarehouses
             * @function
             *
             * @description
             * Loads in default warehouse and all other warehouses from server into the scope.  Called in init().
             */
            function loadAllWarehouses(key, productVariantKey) {
                var promiseWarehouse = warehouseResource.getDefaultWarehouse();
                promiseWarehouse.then(function (warehouse) {
                    $scope.defaultWarehouse = warehouseDisplayBuilder.transform(warehouse);
                    $scope.warehouses.push($scope.defaultWarehouse);
                    loadProduct(key, productVariantKey);
                }, function (reason) {
                    notificationsService.error("Default Warehouse Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadProduct
             * @function
             *
             * @description
             * Load a product by the product key.
             */
            function loadProduct(key, productVariantKey) {
                if (key === 'create' || key === '' || key === undefined) {
                    $scope.context = 'createproduct';
                    $scope.productVariant = $scope.product.getMasterVariant();
                    $scope.tabs = merchelloTabsFactory.createNewProductEditorTabs();
                    $scope.tabs.setActive($scope.context);
                    $scope.preValuesLoaded = true;
                    return;
                }
                var promiseProduct = productResource.getByKey(key);
                promiseProduct.then(function (product) {
                    $scope.product = productDisplayBuilder.transform(product);
                    if(productVariantKey === '' || productVariantKey === undefined) {
                        // this is a product edit.
                        // we use the master variant context so that we can use directives associated with variants
                        $scope.productVariant = $scope.product.getMasterVariant();
                        $scope.context = 'productedit';

                        if (!$scope.product.hasVariants()) {
                            $scope.tabs = merchelloTabsFactory.createProductEditorTabs(key);
                        }
                        else
                        {
                            $scope.tabs = merchelloTabsFactory.createProductEditorWithOptionsTabs(key);
                        }

                    } else {
                        // this is a product variant edit
                        // in this case we need the specific variant
                        $scope.productVariant = $scope.product.getProductVariant(productVariantKey);
                        $scope.context = 'varianteditor';
                        $scope.tabs = merchelloTabsFactory.createProductVariantEditorTabs(key, productVariantKey);
                    }
                    
                    $scope.preValuesLoaded = true;
                    $scope.tabs.setActive($scope.context);
                }, function (reason) {
                    notificationsService.error("Product Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description
             * Loads in store settings from server into the scope.  Called in init().
             */
            function loadSettings() {
                var promiseSettings = settingsResource.getAllSettings();
                promiseSettings.then(function(settings) {
                    $scope.settings = settingDisplayBuilder.transform(settings);
                    $scope.loaded = true;
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }

            //--------------------------------------------------------------------------------------
            // Event Handlers
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Called when the Save button is pressed.  See comments below.
             */
            function save(thisForm) {
                // TODO we should unbind the return click event
                // so that we can quickly add the options and remove the following
                if(thisForm === undefined) {
                    return;
                }
                if (thisForm.$valid) {
                    $scope.preValuesLoaded = false;
                    // convert the extended data objects to an array
                    switch ($scope.context) {
                        case "productedit":
                            // Copy from master variant
                            var productOptions = $scope.product.productOptions;
                            $scope.productVariant.ensureCatalogInventory();
                            $scope.product = $scope.productVariant.getProductForMasterVariant();
                            $scope.product.productOptions = productOptions;
                            saveProduct();
                            break;
                        case "varianteditor":
                            saveProductVariant();
                            break;
                        default:
                            var productOptions = $scope.product.productOptions;
                            $scope.product = $scope.productVariant.getProductForMasterVariant();
                            $scope.product.productOptions = productOptions;
                            createProduct();
                            break;
                    }
                }
            }

            /**
             * @ngdoc method
             * @name createProduct
             * @function
             *
             * @description
             * Creates a product.  See comments below.
             */
            function createProduct() {
                var promise = productResource.add($scope.product);
                promise.then(function (product) {
                    notificationsService.success("Product Saved", "");
                    $scope.product = productDisplayBuilder.transform(product);
                    // short pause to make sure examine index has a chance to update
                    $timeout(function() {
                       if ($scope.product.hasVariants()) {
                            $location.url("/merchello/merchello/producteditwithoptions/" + $scope.product.key, true);
                        } else {
                            $location.url("/merchello/merchello/productedit/" + $scope.product.key, true);
                        }
                    }, 400);
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Product Save Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name saveProduct
             * @function
             *
             * @description
             * Saves a product.  See comments below.
             */
            function saveProduct() {
                var promise = productResource.save($scope.product);
                promise.then(function (product) {
                    notificationsService.success("Product Saved", "");
                    $scope.product = productDisplayBuilder.transform(product);
                    $scope.productVariant = $scope.product.getMasterVariant();

                     //if ($scope.product.hasVariants()) {
                    // short pause to make sure examine index has a chance to update
                    //$timeout(function() {
                        //$location.url("/merchello/merchello/productedit/" + $scope.product.key, true);
                        loadProduct($scope.product.key);
                        //$route.reload();
                    //}, 400);
                     //}

                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Product Save Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name saveProductVariant
             * @function
             *
             * @description
             * Saves a product variant.  See comments below.
             */
            function saveProductVariant() {
                //var variant = $scope.productVariant.deepClone();
                $scope.productVariant.removeInActiveInventories();
                var variantPromise = productResource.saveVariant($scope.productVariant);
                variantPromise.then(function(resultVariant) {
                    notificationsService.success("Product Variant Saved");
                    $scope.productVariant = productVariantDisplayBuilder.transform(resultVariant);
                    $scope.preValuesLoaded = true;
                }, function(reason) {
                    notificationsService.error("Product Variant Save Failed", reason.message);
                });
            }

            function openCopyProductDialog() {
                var dialogData = {
                    product: $scope.product,
                    name: '',
                    sku: ''
                };
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/product.copy.html',
                    show: true,
                    callback: processCopyProduct,
                    dialogData: dialogData
                });
            }


            function processCopyProduct(dialogData) {
                productResource.copyProduct(dialogData.product, dialogData.name, dialogData.sku).then(function(result) {
                    notificationsService.success("Product copied");
                    $timeout(function() {
                        $location.url("/merchello/merchello/productedit/" + result.key, true);
                    }, 1000);
                });
            }

            /**
             * @ngdoc method
             * @name deleteProductDialogConfirmation
             * @function
             *
             * @description
             * Called when the Delete Product button is pressed.
             */
            function deleteProductDialogConfirmation() {
                var promiseDel = productResource.deleteProduct($scope.product);
                promiseDel.then(function () {
                    notificationsService.success("Product Deleted", "");
                    $location.url("/merchello/merchello/productlist/manage", true);
                }, function (reason) {
                    notificationsService.error("Product Deletion Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name deleteProductDialog
             * @function
             *
             * @description
             * Opens the delete confirmation dialog via the Umbraco dialogService.
             */
            function deleteProductDialog() {
                var dialogData = dialogDataFactory.createDeleteProductDialogData();
                dialogData.product = $scope.product;
                dialogData.name = $scope.product.name + ' (' + $scope.product.sku + ')';
                dialogData.warning = 'This action cannot be reversed.';

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: deleteProductDialogConfirmation,
                    dialogData: dialogData
                });
            }

            // Initialize the controller
            init();
    }]);

    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ProductEditWithOptionsController
     * @function
     *
     * @description
     * The controller for product edit with options view
     */
    angular.module('merchello').controller('Merchello.Backoffice.ProductEditWithOptionsController',
        ['$scope', '$routeParams', '$timeout', '$location', '$q', 'assetsService', 'notificationsService', 'dialogService', 'serverValidationManager',
            'merchelloTabsFactory', 'dialogDataFactory', 'productResource', 'settingsResource', 'productDisplayBuilder',
        function($scope, $routeParams, $timeout, $location, $q, assetsService, notificationsService, dialogService, serverValidationManager,
            merchelloTabsFactory, dialogDataFactory, productResource, settingsResource, productDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.product = productDisplayBuilder.createDefault();
            $scope.currencySymbol = '';
            $scope.reorderVariants = false;
            $scope.hideClose = true;

            // exposed methods
            $scope.save = save;
            $scope.openCopyProductDialog = openCopyProductDialog;
            $scope.deleteProductDialog = deleteProductDialog;
            $scope.init = init;

            function init() {
                var key = $routeParams.id;
                $scope.tabs = merchelloTabsFactory.createProductEditorWithOptionsTabs(key);
                $scope.tabs.hideTab('productcontent');
                $scope.tabs.setActive('variantlist');
                loadSettings();
                loadProduct(key);
            }

            /**
             * @ngdoc method
             * @name loadProduct
             * @function
             *
             * @description
             * Load a product by the product key.
             */
            function loadProduct(key) {
                var promise = productResource.getByKey(key);
                promise.then(function (product) {
                    $scope.product = productDisplayBuilder.transform(product);
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    notificationsService.error("Product Load Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description
             * Load the settings from the settings service to get the currency symbol
             */
            function loadSettings() {
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function(currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }

            //--------------------------------------------------------------------------------------
            // Event Handlers
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Saves the product - used for changing the master variant name
             */
            function save(thisForm) {
                if (thisForm) {
                    if (thisForm.$valid) {
                        notificationsService.info("Saving Product...", "");
                        var promise = productResource.save($scope.product);
                        promise.then(function (product) {
                            notificationsService.success("Product Saved", "");
                            $scope.product = productDisplayBuilder.transform(product);
                        }, function (reason) {
                            notificationsService.error("Product Save Failed", reason.message);
                        });
                    }
                }
            }

            /**
             * @ngdoc method
             * @name deleteProductDialog
             * @function
             *
             * @description
             * Opens the delete confirmation dialog via the Umbraco dialogService.
             */
            function deleteProductDialog() {
                var dialogData = dialogDataFactory.createDeleteProductDialogData();
                dialogData.product = $scope.product;
                dialogData.name = $scope.product.name + ' (' + $scope.product.sku + ')';
                dialogData.warning = 'This action cannot be reversed.';

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: deleteProductDialogConfirmation,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name deleteProductDialogConfirmation
             * @function
             *
             * @description
             * Called when the Delete Product button is pressed.
             */
            function deleteProductDialogConfirmation() {
                var promiseDel = productResource.deleteProduct($scope.product);
                promiseDel.then(function () {
                    notificationsService.success("Product Deleted", "");
                    $location.url("/merchello/merchello/productlist/manage", true);
                }, function (reason) {
                    notificationsService.error("Product Deletion Failed", reason.message);
                });
            }

            function openCopyProductDialog() {
                var dialogData = {
                    product: $scope.product,
                    name: '',
                    sku: ''
                };
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/product.copy.html',
                    show: true,
                    callback: processCopyProduct,
                    dialogData: dialogData
                });
            }


            function processCopyProduct(dialogData) {
                productResource.copyProduct(dialogData.product, dialogData.name, dialogData.sku).then(function(result) {
                    notificationsService.success("Product copied");
                    $timeout(function() {
                        $location.url("/merchello/merchello/productedit/" + result.key);
                    }, 1000);
                });
            }



            // Initialize the controller
            init();
    }]);
angular.module('merchello').controller('Merchello.Backoffice.ProductFilterGroupsController',
    ['$scope', 'entityCollectionResource', 'merchelloTabsFactory',
    function($scope, entityCollectionResource, merchelloTabsFactory) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;

        $scope.entityType = 'Product';

        $scope.tabs = [];

        $scope.add = function(collection) {
            $scope.preValuesLoaded = false;
            entityCollectionResource.addEntityCollection(collection).then(function(result) {
                $scope.preValuesLoaded = true;
            });
        }

        $scope.edit = function(collection) {
            $scope.preValuesLoaded = false;
            entityCollectionResource.putEntityFilterGroup(collection).then(function(result) {
                $scope.preValuesLoaded = true;
            });
        }

        function init() {

            $scope.tabs = merchelloTabsFactory.createProductListTabs();
            $scope.tabs.setActive('filtergroups');

            $scope.loaded = true;
            $scope.preValuesLoaded = true;
        }

        init();
}]);

    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ProductListController
     * @function
     *
     * @description
     * The controller for product list view controller
     */
    angular.module('merchello').controller('Merchello.Backoffice.ProductListController',
        ['$scope', '$log', '$q', '$routeParams', '$location', '$filter', '$compile', 'localizationService', 'notificationsService', 'settingsResource', 'entityCollectionResource',
            'merchelloTabsFactory', 'productResource', 'productDisplayBuilder',
        function($scope, $log, $q, $routeParams, $location, $filter, $compile, localizationService, notificationsService, settingsResource, entityCollectionResource,
                 merchelloTabsFactory, productResource, productDisplayBuilder) {

            $scope.productDisplayBuilder = productDisplayBuilder;
            $scope.load = load;
            $scope.entityType = 'Product';

            $scope.tabs = [];

            $scope.settingsComponent = 'product-list-view-filter-options';
            $scope.filterOptions = {
                fields: [
                    {
                        title: 'Name',
                        field: 'name',
                        selected: true,
                        input: {
                            src: 'search'
                        }
                    },
                    {
                        title: 'Sku',
                        field: 'sku',
                        selected: true,
                        input: {
                            src: 'search'
                        }
                    },
                    {
                        title: 'Manufacturer',
                        field: 'manufacturer',
                        selected: '',
                        input: {
                            src: 'custom',
                            values: []
                        }
                    }
                ]
            };

            // exposed methods
            $scope.getColumnValue = getColumnValue;

            var yes = '';
            var no = '';
            var some = '';

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                loadSettings();
                $scope.tabs = merchelloTabsFactory.createProductListTabs();
                $scope.tabs.setActive('productlist');
            }


            function load(query, filterOptions) {

                if (filterOptions !== undefined && filterOptions !== null) {
                    var includeFields = [];

                    var name = getField(filterOptions, 'name');
                    if (name.selected) {
                        includeFields.push(name.field);
                    }
                    var sku = getField(filterOptions, 'sku');
                    if (sku.selected) {
                        includeFields.push(sku.field);
                    }
                    var manufacturer = getField(filterOptions, 'manufacturer');
                    if (manufacturer.selected !== undefined && manufacturer.selected !== null && manufacturer.selected !== '') {
                        includeFields.push(manufacturer.field);
                        query.addCustomParam(manufacturer.field, manufacturer.selected);
                    }
                    if (includeFields.length > 0) {
                        query.addCustomParam('includedFields', includeFields.join());
                    }
                }

                return productResource.advancedSearchProducts(query);
            }


            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description
             * Load the settings from the settings service to get the currency symbol
             */
            function loadSettings() {
                var deferred = $q.defer();
                var promises = [
                        settingsResource.getCurrencySymbol(),
                        localizationService.localize('general_yes'),
                        localizationService.localize('general_no'),
                        localizationService.localize('merchelloGeneral_some'),
                        productResource.getManufacturers()
                    ];
                $q.all(promises).then(function(data) {
                    deferred.resolve(data);
                });
                deferred.promise.then(function(result) {
                    $scope.currencySymbol = result[0];
                    yes = result[1];
                    no = result[2];
                    some = result[3];

                    // load the manufacturers
                    var field = _.find($scope.filterOptions.fields, function(f) {
                        if (f.field === 'manufacturer') {
                            return f;
                        }
                    });

                    if (field !== undefined) {
                        field.input.values = result[4];
                    }

                    $scope.preValuesLoaded = true;
                    $scope.loaded = true;
                }, function(reason) {
                    notificationsService.error("Settings Load Failed", reason.message)
                });
            }

            function getColumnValue(result, col) {
               switch(col.name) {
                   case 'name':
                       return '<a href="' + getEditUrl(result) + '">' + result.name + '</a>';
                   case 'available':
                       return result.available ? yes : no;
                   case 'shippable':
                       return getShippableValue(result);
                   case 'taxable':
                       return getTaxableValue(result);
                   case 'totalInventory':
                       return '<span>' + result.totalInventory() + '</span>';
                   case 'onSale':
                       return getOnSaleValue(result);
                   case 'price':
                       return !result.hasVariants() ?
                           $filter('currency')(result.price, $scope.currencySymbol) :
                           $filter('currency')(result.variantsMinimumPrice(), $scope.currencySymbol) + ' - ' + $filter('currency')(result.variantsMaximumPrice(), $scope.currencySymbol);
                   default:
                       return result[col.name];
               }
            }

            function getShippableValue(product) {
                if ((product.hasVariants() && product.shippableVariants().length === product.productVariants.length) ||
                    (product.hasVariants() && product.shippableVariants().length === 0) ||
                    (!product.hasVariants() && product.shippable)) {
                    return yes;
                }
                if (product.hasVariants() && product.shippableVariants().length !== product.productVariants.length && product.shippableVariants().length > 0) {
                    return some;
                } else {
                    return no;
                }
            }

            function getTaxableValue(product) {
                if((product.hasVariants() && product.taxableVariants().length === product.productVariants.length) ||
                    (product.hasVariants() && product.taxableVariants().length === 0) ||
                    (!product.hasVariants() && product.taxable)) {
                    return yes;
                }
                if (product.hasVariants() && product.taxableVariants().length !== product.productVariants.length && product.taxableVariants().length > 0) {
                    return some;
                } else {
                    return no;
                }
            }

            function getOnSaleValue(product) {
                if((product.hasVariants() && !product.anyVariantsOnSale()) ||
                    (!product.hasVariants() && !product.onSale)) {
                    return no;
                }
                if(product.hasVariants() && product.anyVariantsOnSale()) {
                    return $filter('currency')(product.variantsMinimumPrice(true), $scope.currencySymbol) + ' - ' +
                        $filter('currency')(product.variantsMaximumPrice(true), $scope.currencySymbol);
                }
                if (!product.hasVariants() && product.onSale) {
                    return $filter('currency')(product.salePrice, $scope.currencySymbol)
                }
            }

            function getEditUrl(product) {
               // if (product.hasVariants()) {
               //     return '#/merchello/merchello/producteditwithoptions/' + product.key;
               // } else {
                    return "#/merchello/merchello/productedit/" + product.key;
               // }
            }

            function getField(filterOptions, fieldName) {
                if (filterOptions === undefined) {
                    throw new Error('Value has not been set on the scope');
                }

                return _.find(filterOptions.fields, function (f) {
                    if (f.field === fieldName) {
                        return f;
                    }
                });
            }

            // Initialize the controller
            init();

        }]);

angular.module('merchello').controller('Merchello.Backoffice.ProductOptionsManagerController', [
    '$scope', '$q', '$routeParams', '$location', '$timeout', 'notificationsService', 'dialogService',
        'merchelloTabsFactory', 'productResource', 'eventsService', 'settingsResource',
        'productOptionDisplayBuilder', 'productDisplayBuilder', 'queryResultDisplayBuilder',
    function($scope, $q, $routeParams, $location, $timeout, notificationsService, dialogService,
             merchelloTabsFactory, productResource, eventsService, settingsResource,
             productOptionDisplayBuilder, productDisplayBuilder, queryResultDisplayBuilder) {

        $scope.product = {};
        $scope.preValuesLoaded = false;

        $scope.save = save;
        $scope.deleteProductDialog = deleteProductDialog;

        var onAdd = 'merchelloProductOptionOnAddOpen';

        $scope.load = function(query) {

            var deferred = $q.defer();

            var hasOptions = $scope.product.productOptions.length > 0;
            var itemsPerPage = hasOptions ?
                $scope.product.productOptions.length : query.itemsPerPage;

            var result = queryResultDisplayBuilder.createDefault();
            result.currentPage = 1;
            result.itemsPerPage = itemsPerPage;
            result.totalPages = hasOptions ? 1 : 0;
            result.totalItems = $scope.product.productOptions.length;
            result.items = [];

            _.each($scope.product.productOptions, function(po) {
                result.items.push(po);
            });

            deferred.resolve(result);

            return deferred.promise;

        }

        $scope.doEdit = function(option) {
            executeReload(function() {
                var options = _.reject($scope.product.productOptions, function(po) {
                   return po.key === option.key;
                });

                options.push(option);
                options = _.sortBy(options, 'sortOrder');
                $scope.product.productOptions = options;
            });

        }

        $scope.doDelete = function(option) {
            executeReload(function() {
                $scope.product.removeOption(option);
            });
        }


        $scope.doAdd = function(option) {
            option.sortOrder = $scope.product.productOptions.length + 1;

            executeReload(function() {
                $scope.product.productOptions.push(option);
            });
        }

        function init() {

            eventsService.on(onAdd, function(name, args) {
                args.productKey = $scope.product.key;
            });

            var key = $routeParams.id;
            $q.all([
                settingsResource.getCurrencySymbol(),
                loadProduct(key)
            ]).then(function(data) {
                $scope.currencySymbol = data[0];
                $scope.product = data[1];
                setTabs();
            });
        }

        function setTabs() {
            $scope.tabs = merchelloTabsFactory.createProductEditorTabs($scope.product.key, $scope.product.hasVariants());
            $scope.tabs.hideTab('productcontent');
            $scope.tabs.setActive('optionslist');
            $scope.preValuesLoaded = true;
        }

        function loadProduct(key) {
            var deferred = $q.defer();
            productResource.getByKey(key).then(function(prod) {
                var p = productDisplayBuilder.transform(prod);
                deferred.resolve(p);
            });

            return deferred.promise;
        }

        /**
         * @ngdoc method
         * @name save
         * @function
         *
         * @description
         * Saves the product - used for changing the master variant name
         */
        function save(thisForm) {
            // TODO we should unbind the return click event
            // so that we can quickly add the options and remove the following
            if(thisForm === undefined) {
                return;
            }
            if (thisForm.$valid) {
                notificationsService.info("Saving Product...", "");

                $scope.product.productOptions = _.sortBy($scope.product.productOptions, function (po) {
                    return po.sortOrder;
                });

                $scope.preValuesLoaded = false;
                productResource.save($scope.product).then(function (product) {
                    notificationsService.success("Product Saved", "");
                    $scope.product = productDisplayBuilder.transform(product);
                    setTabs();
                }, function (reason) {
                    notificationsService.error("Product Save Failed", reason.message);
                });
            }
        }

        function executeReload(callback) {
            $scope.preValuesLoaded = false;
            // we need a timeout here so that the directive has time to catch the pre value toggle
            $timeout(function() {
                callback();
                $scope.preValuesLoaded = true;
            }, 400);
        }

        /**
         * @ngdoc method
         * @name deleteProductDialog
         * @function
         *
         * @description
         * Opens the delete confirmation dialog via the Umbraco dialogService.
         */
        function deleteProductDialog() {
            var dialogData = dialogDataFactory.createDeleteProductDialogData();
            dialogData.product = $scope.product;
            dialogData.name = $scope.product.name + ' (' + $scope.product.sku + ')';
            dialogData.warning = 'This action cannot be reversed.';

            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                show: true,
                callback: deleteProductDialogConfirmation,
                dialogData: dialogData
            });
        }

        /**
         * @ngdoc method
         * @name deleteProductDialogConfirmation
         * @function
         *
         * @description
         * Called when the Delete Product button is pressed.
         */
        function deleteProductDialogConfirmation() {
            var promiseDel = productResource.deleteProduct($scope.product);
            promiseDel.then(function () {
                notificationsService.success("Product Deleted", "");
                $location.url("/merchello/merchello/productlist/manage", true);
            }, function (reason) {
                notificationsService.error("Product Deletion Failed", reason.message);
            });
        }


        init();

    }]);

angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloCheckoutWorkflowStagePickerController', [
    '$scope', 'backOfficeCheckoutResource',
    function($scope, backOfficeCheckoutResource) {
        
        $scope.loaded = false;
        $scope.stages = [];
        
        function init() {

            if (_.isString($scope.model.value)) {
                backOfficeCheckoutResource.getCheckoutStages().then(function(stages) {
                    $scope.stages = stages;
                    $scope.loaded = true;
                    if ($scope.model.value === '') {
                        $scope.model.value = $scope.stages[0];
                    }
                });
            } else {
                $scope.loaded = true;
            }
        }
        
        init();
    }]);

angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloMultiProductPickerController',
    ['$scope', 'dialogService', 'productResource', 'productDisplayBuilder',
    function($scope, dialogService, productResource, productDisplayBuilder) {

        $scope.preValuesLoaded = false;
        $scope.keys = [];
        $scope.products = [];
        $scope.settings = {};
        $scope.remove = remove;
        $scope.openPickerDialog = openPickerDialog;

        $scope.sortProperty = '';
        $scope.sortOrder = 'Ascending';
        $scope.filterText = '';
        $scope.limitAmount = 5;
        $scope.currentPage = 0;
        $scope.maxPages = 0;

        function init() {


            if ($scope.model.value !== undefined && $scope.model.value !== '' && $scope.model.value.length > 0) {
                $scope.keys = $scope.model.value;
            }

            if ($scope.keys.length > 0) {
                productResource.getByKeys($scope.keys).then(function(data) {
                    $scope.products = productDisplayBuilder.transform(data);
                });
            }
        }

        function remove(product) {
            $scope.products = _.reject($scope.products, function (p) { return p.key === product.key });
            setModelValue();
        }

        function openPickerDialog() {
            var dialogData = {};
            var dataProducts = [];
            angular.copy($scope.products, dataProducts);
            dialogData.products = dataProducts;
            dialogService.open({
                template: '/App_Plugins/Merchello/propertyeditors/multiproductpicker/merchello.multiproductpicker.dialog.html',
                show: true,
                callback: selectProductFromDialog,
                dialogData: dialogData
            });
        }

        function selectProductFromDialog(dialogData) {
            $scope.products = dialogData.products;
            setModelValue();
        }

        function setModelValue() {
            $scope.model.value = _.pluck($scope.products, 'key');
        }

        init();

    }]);

angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloMultiProductPickerDialogController',
    ['$scope', '$q', 'productResource', 'settingsResource', 'settingDisplayBuilder', 'queryDisplayBuilder', 'queryResultDisplayBuilder', 'productDisplayBuilder',
    function($scope, $q, productResource, settingsResource, settingDisplayBuilder, queryDisplayBuilder, queryResultDisplayBuilder, productDisplayBuilder) {

        $scope.loaded = true;
        $scope.currencySymbol = '';

        $scope.sortProperty = '';
        $scope.sortOrder = 'Ascending';
        $scope.filterText = '';
        $scope.limitAmount = 5;
        $scope.currentPage = 0;

        // exposed methods
        $scope.changePage = changePage;
        $scope.limitChanged = limitChanged;
        $scope.changeSortOrder = changeSortOrder;
        $scope.numberOfPages = numberOfPages;
        $scope.handleProduct = handleProduct;
        $scope.getFilteredProducts = getFilteredProducts;

        function init() {
            loadSettings();
        }

        function loadProducts() {

            $scope.preValuesLoaded = false;
            var page = $scope.currentPage;
            var perPage = $scope.limitAmount;
            var sortBy = $scope.sortProperty.replace("-", "");
            var sortDirection = $scope.sortOrder;
            var query = queryDisplayBuilder.createDefault();
            query.currentPage = page;
            query.itemsPerPage = perPage;
            query.sortBy = sortBy;
            query.sortDirection = sortDirection;
            query.addFilterTermParam($scope.filterText);

            var promise = productResource.searchProducts(query);
            promise.then(function (response) {
                var queryResult = queryResultDisplayBuilder.transform(response, productDisplayBuilder);

                $scope.products = queryResult.items;

                selectProduct($scope.products);

                $scope.maxPages = queryResult.totalPages;
                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {
                notificationsService.success("Products Load Failed:", reason.message);
            });


        }

        /**
         * @ngdoc method
         * @name selectProduct
         * @function
         *
         * @description
         * Helper to set the selected bool on a product if it is the currently selected product
         */
        function selectProduct(products) {
            if ($scope.dialogData.products.length === 0) return;

            _.each($scope.products, function(p) {
               var displayed = _.find($scope.dialogData.products, function(dp) {
                  return dp.key === p.key;
               });

                if (displayed !== null && displayed !== undefined) {
                    p.selected = true;
                } else {
                    p.selected = false;
                }

            });
        }

        function handleProduct(product) {
            if (product.selected) {
                $scope.dialogData.products = _.reject($scope.dialogData.products, function(dp) {
                    return dp.key === product.key;
                });
            } else {
                $scope.dialogData.products.push(product);
            }

            loadProducts();
        }

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         *
         * @description
         * Load the settings from the settings service to get the currency symbol
         */
        function loadSettings() {
            // this is needed for the date format
            var settingsPromise = settingsResource.getAllSettings();
            settingsPromise.then(function(allSettings) {
                $scope.settings = settingDisplayBuilder.transform(allSettings);
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                    loadProducts();
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }, function(reason) {
                notificationService.error('Failed to load all settings', reason.message);
            });
        }

        //--------------------------------------------------------------------------------------
        // Events methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name limitChanged
         * @function
         *
         * @description
         * Helper function to set the amount of items to show per page for the paging filters and calculations
         */
        function limitChanged(newVal) {
            $scope.limitAmount = newVal;
            $scope.currentPage = 0;
            loadProducts();
        }

        /**
         * @ngdoc method
         * @name changePage
         * @function
         *
         * @description
         * Helper function re-search the products after the page has changed
         */
        function changePage (newPage) {
            $scope.currentPage = newPage;
            loadProducts();
        }

        /**
         * @ngdoc method
         * @name changeSortOrder
         * @function
         *
         * @description
         * Helper function to set the current sort on the table and switch the
         * direction if the property is already the current sort column.
         */
        function changeSortOrder(propertyToSort) {

            if ($scope.sortProperty == propertyToSort) {
                if ($scope.sortOrder == "Ascending") {
                    $scope.sortProperty = "-" + propertyToSort;
                    $scope.sortOrder = "Descending";
                } else {
                    $scope.sortProperty = propertyToSort;
                    $scope.sortOrder = "Ascending";
                }
            } else {
                $scope.sortProperty = propertyToSort;
                $scope.sortOrder = "Ascending";
            }

            loadProducts();
        }

        function getFilteredProducts(filter) {
            $scope.filterText = filter;
            $scope.currentPage = 0;
            loadProducts();
        }

        //--------------------------------------------------------------------------------------
        // Calculations
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name numberOfPages
         * @function
         *
         * @description
         * Helper function to get the amount of items to show per page for the paging
         */
        function numberOfPages() {
            return $scope.maxPages;
        }

        init();

    }]);

angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloProductCollectionPickerController', 
    ['$scope', '$q', 'dialogService', 'entityCollectionResource', 'entityCollectionDisplayBuilder',
    function($scope, $q, dialogService, entityCollectionResource, entityCollectionDisplayBuilder) {

        $scope.loaded = false;
        $scope.collection = {};
        $scope.entityType = 'product';
        

        $scope.delete = removeCollection;
        $scope.openCollectionSelectionDialog = openCollectionSelectionDialog;

        function init() {

            if (_.isString($scope.model.value)) {
                if ($scope.model.value !== '') {
                    getCollection().then(function(collection) {
                        $scope.loaded = true;
                    });
                } else {
                    $scope.loaded = true;
                }

            } else {
                $scope.loaded = true;
            }
        }

        function getCollection() {
            var deferred = $q.defer();
            entityCollectionResource.getByKey($scope.model.value).then(function(collection) {
                $scope.collection = entityCollectionDisplayBuilder.transform(collection);
               $q.resolve($scope.collection);
            });
            return deferred.promise;
        }

        function removeCollection() {
            $scope.collection = {};
            $scope.model.value = '';
        }
        
        function openCollectionSelectionDialog() {
            var dialogData = {};
            dialogData.collectionKey = '';
            dialogData.entityType =  $scope.entityType;

            dialogService.open({
                template: '/App_Plugins/Merchello/propertyeditors/productlistview/merchello.productlistview.dialog.html',
                show: true,
                callback: processCollectionSelection,
                dialogData: dialogData
            });
        }

        function processCollectionSelection(dialogData) {
            $scope.model.value = dialogData.collectionKey;
            getCollection();

        }

        init();
        
    }]);

/**
 * @ngdoc controller
 * @name Merchello.PropertyEditors.MerchelloProductListView
 * @function
 *
 * @description
 * The controller for product collection list view property editor
 */
angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloProductListViewController',
    ['$scope', '$location', 'dialogService', 'userService', 'settingsResource', 'notificationsService', 'entityCollectionResource', 'productResource', 'productDisplayBuilder',
        'queryDisplayBuilder', 'queryResultDisplayBuilder',
    function($scope, $location, dialogService, userService, settingsResource, notificationsService, entityCollectionResource, productResource, productDisplayBuilder,
    queryDisplayBuilder, queryResultDisplayBuilder) {

        $scope.preValuesLoaded = false;
        $scope.currencySymbol = '';
        $scope.entityType = 'product';
        $scope.entityTypeName = 'Product';
        $scope.entityCollection = {};
        $scope.listViewResultSet = {
            totalItems: 0,
            items: []
        };

        $scope.options = {
            pageSize: 10,
            pageNumber: 1,
            filter: '',
            orderBy: ($scope.model.config.orderBy ? $scope.model.config.orderBy : 'name').trim(),
            orderDirection: $scope.model.config.orderDirection ? $scope.model.config.orderDirection.trim() : "asc"
        };

        $scope.pagination = [];

        $scope.openCollectionSelectionDialog = openCollectionSelectionDialog;
        $scope.getTreeId = getTreeId;
        $scope.sort = sort;
        $scope.isSortDirection = isSortDirection;
        $scope.next = next;
        $scope.prev = prev;
        $scope.goToPage = goToPage;
        $scope.enterSearch = enterSearch;
        $scope.loadProducts = loadProducts;
        $scope.goToEditor = goToEditor;

        var currentUser = null;
        var allowMerchello = false;
        const baseUrl = '/merchello/merchello/productedit/';
        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------
        // Load the product from the Guid key stored in the model.value
        if (_.isString($scope.model.value)) {
            userService.getCurrentUser().then(function(user) {
              currentUser = user;
                var fnd = _.find(currentUser.allowedSections, function(s) {
                  return s === 'merchello';
                });
                if (fnd !== undefined) {
                    allowMerchello = true;
                }
                loadSettings();
            });

        }

        function loadSettings() {
            var promise = settingsResource.getCurrencySymbol();
            promise.then(function(symbol) {
                $scope.currencySymbol = symbol;
                loadProducts();
            }, function (reason) {
                notificationsService.error('Could not retrieve currency symbol', reason.message);
            });
        }

        function loadProducts() {
            var page = $scope.options.pageNumber - 1;
            var perPage = $scope.options.pageSize;
            var sortBy = $scope.options.orderBy;
            var sortDirection = $scope.options.orderDirection === 'asc' ? 'Ascending' : 'Descending';

            var query = queryDisplayBuilder.createDefault();
            query.currentPage = page;
            query.itemsPerPage = perPage;
            query.sortBy = sortBy;
            query.sortDirection = sortDirection;
            query.addFilterTermParam($scope.options.filter);

            var promise;
            if ($scope.model.value !== '') {
                query.addCollectionKeyParam($scope.model.value);
                query.addEntityTypeParam('Product');
                var promise = entityCollectionResource.getCollectionEntities(query);
            } else {
                var promise = productResource.searchProducts(query);
            }

            promise.then(function (response) {
                var queryResult = queryResultDisplayBuilder.transform(response, productDisplayBuilder);
                $scope.listViewResultSet.items = queryResult.items;
                $scope.listViewResultSet.totalItems = queryResult.totalItems;
                $scope.listViewResultSet.totalPages = queryResult.totalPages;
               // $scope.products = queryResult.items;
               // $scope.maxPages = queryResult.totalPages;

                $scope.pagination = [];

                //list 10 pages as per normal
                if ($scope.listViewResultSet.totalPages <= 10) {
                    for (var i = 0; i < $scope.listViewResultSet.totalPages; i++) {
                        $scope.pagination.push({
                            val: (i + 1),
                            isActive: $scope.options.pageNumber == (i + 1)
                        });
                    }
                }
                else {
                    //if there is more than 10 pages, we need to do some fancy bits

                    //get the max index to start
                    var maxIndex = $scope.listViewResultSet.totalPages - 10;
                    //set the start, but it can't be below zero
                    var start = Math.max($scope.options.pageNumber - 5, 0);
                    //ensure that it's not too far either
                    start = Math.min(maxIndex, start);

                    for (var i = start; i < (10 + start) ; i++) {
                        $scope.pagination.push({
                            val: (i + 1),
                            isActive: $scope.options.pageNumber == (i + 1)
                        });
                    }

                    //now, if the start is greater than 0 then '1' will not be displayed, so do the elipses thing
                    if (start > 0) {
                        $scope.pagination.unshift({ name: "First", val: 1, isActive: false }, {val: "...",isActive: false});
                    }

                    //same for the end
                    if (start < maxIndex) {
                        $scope.pagination.push({ val: "...", isActive: false }, { name: "Last", val: $scope.listViewResultSet.totalPages, isActive: false });
                    }
                }



                $scope.preValuesLoaded = true;
            }, function(reason) {
                notificationsService.success("Products Load Failed:", reason.message);
            });
        }


        function sort(field, allow) {
            if (allow) {
                $scope.options.orderBy = field;

                if ($scope.options.orderDirection === "desc") {
                    $scope.options.orderDirection = "asc";
                }
                else {
                    $scope.options.orderDirection = "desc";
                }
                loadProducts();
            }
        };

        function next () {
            if ($scope.options.pageNumber < $scope.listViewResultSet.totalPages) {
                $scope.options.pageNumber++;
                loadProducts();
            }
        };

        function goToPage(pageNumber) {
            $scope.options.pageNumber = pageNumber + 1;
            loadProducts();
        }


        function prev() {
            if ($scope.options.pageNumber > 0) {
                $scope.options.pageNumber--;
                loadProducts();
            }
        }

        function enterSearch($event) {
            $($event.target).next().focus();
        }


        function isSortDirection(col, direction) {
            return $scope.options.orderBy.toUpperCase() == col.toUpperCase() && $scope.options.orderDirection == direction;
        }

        function goToEditor(product) {
            if(allowMerchello) {
                var url = baseUrl + product.key;
                $location.url(url, true);
            }
        }

        function getTreeId() {
            return "products";
        }

        function openCollectionSelectionDialog() {
            var dialogData = {};
            dialogData.collectionKey = '';
            dialogData.entityType =  $scope.entityType;

            dialogService.open({
                template: '/App_Plugins/Merchello/propertyeditors/productlistview/merchello.productlistview.dialog.html',
                show: true,
                callback: processCollectionSelection,
                dialogData: dialogData
            });
        }

        function processCollectionSelection(dialogData) {
            $scope.model.value = dialogData.collectionKey;
            loadProducts();
        }

    }]);
angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloProductListViewDialogController',
    ['$scope', 'treeService', 'localizationService', 'eventsService',
    function($scope, treeService, localizationService, eventsService) {

        $scope.pickerRootNode = {};
        $scope.allowMultiple = false;
        $scope.getTreeId = getTreeId;
        $scope.hasSelection = hasSelection;
        $scope.setAllProducts = setAllProducts;

        var eventName = 'merchello.entitycollection.selected';

        function init() {
            eventsService.on(eventName, onEntityCollectionSelected);
            $('#all-items i.icon').removeClass('icon-add').addClass('icon-check');
            setTitle();
        }

        function setTitle() {
            var key = 'merchelloCollections_' + $scope.dialogData.entityType + 'Collections';
            localizationService.localize(key).then(function (value) {
                $scope.pickerTitle = value;
                setTree();
            });
        }

        function setTree() {
            treeService.getTree({section: 'merchello'}).then(function(tree) {
                var root = tree.root;
                var treeId = getTreeId();
                $scope.pickerRootNode = _.find(root.children, function (child) {
                    return child.id === treeId;
                });
            });
        }

        function setAllProducts() {
            if (!hasSelection()) {
                $('#all-items i.icon').removeClass('icon-add').addClass('icon-check');
            }
        }

        function onEntityCollectionSelected(eventName, args, ev) {
            //  {key: "addCollection", value: "4d026d91-fe13-49c7-8f06-da3d9f012181"}
            if (args.key === 'addCollection') {
               $scope.dialogData.collectionKey = args.value;
                $('#all-items i.icon').removeClass('icon-check').addClass('icon-add');
            }
            if (args.key === 'removeCollection') {
                $scope.dialogData.collectionKey = '';
                $('#all-items i.icon').removeClass('icon-add').addClass('icon-check');
            }
        }

        function hasSelection() {
            return $scope.dialogData.collectionKey !== '';
        }

        function getTreeId() {
            return "products";
        }

        init();
}]);

angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloMultiProductDialogController',
    ['$scope',
    function($scope) {

        $scope.loaded = false;

        function init() {
            $scope.loaded = true;
        }

        init();
}]);

    /**
     * @ngdoc controller
     * @name Merchello.PropertyEditors.MerchelloProductSelectorController
     * @function
     *
     * @description
     * The controller for product product selector property editor view
     */
    angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloProductSelectorController',
        ['$scope', 'notificationsService', 'dialogService', 'assetsService', 'dialogDataFactory', 'productResource', 'settingsResource', 'productDisplayBuilder',
        function($scope, notificationsService, dialogService, assetsService, dialogDataFactory, productResource, settingsResource, productDisplayBuilder) {

            $scope.product = {};
            $scope.currencySymbol = '';
            $scope.loaded = false;

            // exposed methods
            $scope.selectProduct = selectProduct;
            $scope.disableProduct = disableProduct;

            //--------------------------------------------------------------------------------------
            // Initialization methods
            //--------------------------------------------------------------------------------------
            // Load the product from the Guid key stored in the model.value
            if (_.isString($scope.model.value)) {
                loadSettings();
                if ($scope.model.value.length > 0) {
                    loadProduct($scope.model.value);
                }
            }

            /**
             * @ngdoc method
             * @name loadProduct
             * @function
             *
             * @description
             * Load the product from the product service.
             */
            function loadProduct(key) {
                var promise = productResource.getByKey(key);
                promise.then(function (product) {
                    $scope.product = productDisplayBuilder.transform(product);
                    $scope.loaded = true;
                }, function (reason) {
                    notificationsService.error("Product Load Failed", reason.message);
                });
            }

            function loadSettings() {
                var promise = settingsResource.getCurrencySymbol();
                promise.then(function(symbol) {
                    $scope.currencySymbol = symbol;
                }, function (reason) {
                    notificationsService.error('Could not retrieve currency symbol', reason.message);
                });
            }

            //--------------------------------------------------------------------------------------
            // Event Handlers
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name selectedProductFromDialog
             * @function
             *
             * @description
             * Handles the model update after recieving the product to add from the dialog view/controller
             */
            function selectedProductFromDialog(dialogData) {
                $scope.model.value = dialogData.product.key;
                $scope.product = dialogData.product;
            }

            /**
             * @ngdoc method
             * @name selectProduct
             * @function
             *
             * @description
             * Opens the product select dialog via the Umbraco dialogService.
             */
            function selectProduct() {
                var dialogData = dialogDataFactory.createProductSelectorDialogData();
                dialogData.product = $scope.product;
                dialogService.open({
                    template: '/App_Plugins/Merchello/propertyeditors/productpicker/merchello.productselector.dialog.html',
                    show: true,
                    callback: selectedProductFromDialog,
                    dialogData: dialogData
                });
            }

            function disableProduct() {
                $scope.model.value = '';
                $scope.product = {};
            }

    }]);

    /**
     * @ngdoc controller
     * @name Merchello.PropertyEditors.MerchelloProductSelectorDialogController
     * @function
     *
     * @description
     * The controller for product selector property editor dialog view
     */
    angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloProductSelectorDialogController',
        ['$scope', 'notificationsService', 'productResource', 'settingsResource', 'productDisplayBuilder', 'queryDisplayBuilder', 'queryResultDisplayBuilder',
        function($scope, notificationsService, productResource, settingsResource, productDisplayBuilder, queryDisplayBuilder, queryResultDisplayBuilder) {

            $scope.filterText = "";
            $scope.products = [];
            $scope.filteredproducts = [];
            $scope.watchCount = 0;
            $scope.sortProperty = "name";
            $scope.sortOrder = "Ascending";
            $scope.limitAmount = 10;
            $scope.currentPage = 0;
            $scope.maxPages = 0;

            // exposed methods
            $scope.changePage = changePage;
            $scope.limitChanged = limitChanged;
            $scope.changeSortOrder = changeSortOrder;
            $scope.getFilteredProducts = getFilteredProducts;
            $scope.numberOfPages = numberOfPages;
            $scope.setProduct = setProduct;

            //--------------------------------------------------------------------------------------
            // Initialization methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description
             * Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init() {
                loadProducts();
                loadSettings();
            }

            /**
             * @ngdoc method
             * @name loadProducts
             * @function
             *
             * @description
             * Load the products from the product service, then wrap the results
             * in Merchello models and add to the scope via the products collection.
             */
            function loadProducts() {

                var page = $scope.currentPage;
                var perPage = $scope.limitAmount;
                var sortBy = $scope.sortProperty.replace("-", "");
                var sortDirection = $scope.sortOrder;

                var query = queryDisplayBuilder.createDefault();
                query.currentPage = page;
                query.itemsPerPage = perPage;
                query.sortBy = sortBy;
                query.sortDirection = sortDirection;
                query.addFilterTermParam($scope.filterText);

                var promise = productResource.searchProducts(query);
                promise.then(function (response) {
                    var queryResult = queryResultDisplayBuilder.transform(response, productDisplayBuilder);

                    $scope.products = queryResult.items;

                    selectProduct($scope.products);

                    $scope.maxPages = queryResult.totalPages;
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;

                }, function (reason) {
                    notificationsService.success("Products Load Failed:", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description
             * Load the settings from the settings service to get the currency symbol
             */
            function loadSettings() {
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }

            //--------------------------------------------------------------------------------------
            // Events methods
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name limitChanged
             * @function
             *
             * @description
             * Helper function to set the amount of items to show per page for the paging filters and calculations
             */
            function limitChanged(newVal) {
                $scope.limitAmount = newVal;
                $scope.currentPage = 0;
                loadProducts();
            }

            /**
             * @ngdoc method
             * @name changePage
             * @function
             *
             * @description
             * Helper function re-search the products after the page has changed
             */
            function changePage (newPage) {
                $scope.currentPage = newPage;
                loadProducts();
            }

            /**
             * @ngdoc method
             * @name changeSortOrder
             * @function
             *
             * @description
             * Helper function to set the current sort on the table and switch the
             * direction if the property is already the current sort column.
             */
            function changeSortOrder(propertyToSort) {

                if ($scope.sortProperty == propertyToSort) {
                    if ($scope.sortOrder == "Ascending") {
                        $scope.sortProperty = "-" + propertyToSort;
                        $scope.sortOrder = "Descending";
                    } else {
                        $scope.sortProperty = propertyToSort;
                        $scope.sortOrder = "Ascending";
                    }
                } else {
                    $scope.sortProperty = propertyToSort;
                    $scope.sortOrder = "Ascending";
                }

                loadProducts();
            }

            /**
             * @ngdoc method
             * @name getFilteredProducts
             * @function
             *
             * @description
             * Calls the product service to search for products via a string search
             * param.  This searches the Examine index in the core.
             */
            function getFilteredProducts(filter) {
                $scope.filterText = filter;
                $scope.currentPage = 0;
                loadProducts();
            }

            //--------------------------------------------------------------------------------------
            // Helper methods
            //--------------------------------------------------------------------------------------


            /**
             * @ngdoc method
             * @name selectProduct
             * @function
             *
             * @description
             * Helper to set the selected bool on a product if it is the currently selected product
             */
            function selectProduct(products) {
                for (var i = 0; i < products.length; i++) {
                    if (products[i].key == $scope.dialogData.product.key) {
                        products[i].selected = true;
                    } else {
                        products[i].selected = false;
                    }
                }
            }

            //--------------------------------------------------------------------------------------
            // Calculations
            //--------------------------------------------------------------------------------------

            /**
             * @ngdoc method
             * @name numberOfPages
             * @function
             *
             * @description
             * Helper function to get the amount of items to show per page for the paging
             */
            function numberOfPages() {
                return $scope.maxPages;
            }

            function setProduct(product) {
                $scope.dialogData.product = product;
                selectProduct($scope.products);;
                $scope.submit($scope.dialogData);
            }




            // Initialize the controller
            init();

        }]);

angular.module('merchello').controller('Merchello.Backoffice.Reports.AbandonedBasketController',
    ['$scope', 'merchelloTabsFactory',
    function($scope, merchelloTabsFactory) {

        $scope.loaded = false;
        $scope.tabs = [];

        var graphLoaded = false;
        var basketsLoaded = false;

        function init() {
            $scope.tabs = merchelloTabsFactory.createReportsTabs();
            $scope.tabs.setActive('abandonedBasket');
            $scope.loaded = true;

        }

        $scope.setGraphLoaded = function(value) {
            graphLoaded = value;
            setPreValuesLoaded();
        }

        $scope.setBasketsLoaded = function(value) {
            basketsLoaded = value;
            setPreValuesLoaded();
        }

        function setPreValuesLoaded() {
            $scope.preValuesLoaded = graphLoaded && basketsLoaded;
        }

        init();

}]);

    /**
     * @ngdoc controller
     * @name Merchello.Backoffice.ReportsViewReportController
     * @function
     *
     * @description
     * The controller for the ViewReport page
     *
     * This is a bootstrapper to allow reports that are plugins to be loaded using the merchello application route.
     */
    angular.module('merchello').controller('Merchello.Backoffice.ReportsViewReportController',
        ['$scope', '$routeParams',
         function($scope, $routeParams) {

             $scope.loaded = true;
             $scope.preValuesLoaded = true;

             // Property to control the report to show
             $scope.reportParam = $routeParams.id;

             var re = /(\\)/g;
             var subst = '/';

             var result = $scope.reportParam.replace(re, subst);

             //$scope.reportPath = "/App_Plugins/Merchello.ExportOrders|ExportOrders.html";
             $scope.reportPath = "/App_Plugins/" + result + ".html";

    }]);

angular.module('merchello').controller('Merchello.Backoffice.MerchelloReportsDashboardController',
    ['$scope', '$element', '$filter', 'assetsService', 'dialogService', 'eventsService', 'settingsResource', 'merchelloTabsFactory',
        function($scope, $element, $filter, assetsService, dialogService, eventsService, settingsResource, merchelloTabsFactory) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];

            function init() {
                $scope.tabs = merchelloTabsFactory.createReportsTabs();
                $scope.tabs.setActive("reportsdashboard");
               // loadSettings();
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            }

            init();

        }]);

angular.module('merchello').controller('Merchello.Backoffice.Reports.SalesByItemController',
    ['$scope', '$filter', 'assetsService', 'dialogService', 'eventsService', 'settingsResource', 'merchelloTabsFactory',
    function($scope, $filter, assetsService, dialogService, eventsService, settingsResource, merchelloTabsFactory) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.tabs = [];
        $scope.settings = {};

        $scope.startDate = '';
        $scope.endDate = '';


        function init() {
            $scope.tabs = merchelloTabsFactory.createReportsTabs();
            $scope.tabs.setActive("salesByItem");
            $scope.loaded = true;

        };

        $scope.setPreValuesLoaded = function(value) {
            $scope.preValuesLoaded = value;
        }

        init();

    }]);

angular.module('merchello').controller('Merchello.Backoffice.Reports.SalesOverTimeController',
    ['$scope', '$q', '$log', '$filter', 'assetsService', 'dialogService', 'queryDisplayBuilder',
        'settingsResource', 'invoiceHelper', 'merchelloTabsFactory', 'salesOverTimeResource',
        function($scope, $q, $log, $filter, assetsService, dialogService, queryDisplayBuilder,
                 settingsResource, invoiceHelper, merchelloTabsFactory, salesOverTimeResource) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
            $scope.labels = [];
            $scope.series = [];
            $scope.chartData = [];
            $scope.reportData = [];
            $scope.startDate = '';
            $scope.endDate = '';
            $scope.settings = {};
            $scope.dateBtnText = '';

            $scope.getColumnValue = getColumnValue;
            $scope.getColumnTotal = getColumnTotal;
            $scope.openDateRangeDialog = openDateRangeDialog;
            $scope.clearDates = clearDates;
            $scope.reverse = reverse;


            assetsService.loadCss('/App_Plugins/Merchello/lib/charts/angular-chart.min.css').then(function() {
                init();
            });

            function init() {
                $scope.tabs = merchelloTabsFactory.createReportsTabs();
                $scope.tabs.setActive("salesOverTime");

                loadSettings();
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description - Load the Merchello settings.
             */
            function loadSettings() {
                settingsResource.getAllCombined().then(function(combined) {
                    $scope.settings = combined.settings;
                    loadDefaultData();
                });
            };

            function loadDefaultData() {
                salesOverTimeResource.getDefaultReportData().then(function(result) {
                    compileChart(result);
                });
            }

            function loadCustomData() {

                var query = queryDisplayBuilder.createDefault();
                query.addInvoiceDateParam($scope.startDate, 'start');
                query.addInvoiceDateParam($scope.endDate, 'end');

                salesOverTimeResource.getCustomReportData(query).then(function(result) {
                   compileChart(result);
                });
            }

            function compileChart(result) {

                $scope.labels = [];
                $scope.series = [];
                $scope.chartData = [];
                $scope.reportData = [];

                $scope.reportData = result.items;

                if ($scope.reportData.length > 0) {
                    $scope.startDate = $filter('date')($scope.reportData[0].startDate, $scope.settings.dateFormat);
                    $scope.endDate = $filter('date')($scope.reportData[$scope.reportData.length - 1].endDate, $scope.settings.dateFormat);
                }

                setDateButtonText();

                if ($scope.reportData.length > 0) {
                    _.each($scope.reportData[0].totals, function(t) {
                        $scope.series.push(t.currency.symbol + ' ' + t.currency.currencyCode);
                        $scope.chartData.push([]);
                    })
                }

                _.each($scope.reportData, function(item) {
                    var j = 0;
                    for(var i = 0; i < $scope.series.length; i++) {
                        $scope.chartData[j].push(item.totals[i].value.toFixed(2));
                        j++;
                    }

                    $scope.labels.push(item.getDateLabel());

                });

                $scope.preValuesLoaded = true;
                $scope.loaded = true;
            }

            function reverse(data) {
                return data.slice().reverse();
            }

            function getColumnValue(data, series) {

                var total = _.find(data.totals, function(t) {
                   return series.indexOf(t.currency.currencyCode) > -1;
                });

                if (total !== null && total !== undefined) {
                    return $filter('currency')(total.value, total.currency.symbol);
                } else {
                    return '-';
                }

            }

            function openDateRangeDialog() {
                var dialogData = {
                    startDate: $scope.startDate,
                    endDate: $scope.endDate
                };

                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/daterange.selection.html',
                    show: true,
                    callback: processDateRange,
                    dialogData: dialogData
                });
            }

            function getColumnTotal(series) {

                if ($scope.reportData.length > 0) {
                    var total = 0;
                    var symbol = '';
                    _.each($scope.reportData, function(data) {
                        var itemTotal = _.find(data.totals, function(t) {
                            return series.indexOf(t.currency.currencyCode) > -1;
                        });

                        total += itemTotal.value;
                        if (symbol === '') {
                            symbol = itemTotal.currency.symbol;
                        }
                    });

                    return $filter('currency')(total, symbol);

                } else {
                    return '-';
                }
            }

            function setDateButtonText() {
                $scope.dateBtnText = $scope.startDate + ' - ' + $scope.endDate;
            }

            function processDateRange(dialogData) {
                $scope.startDate = dialogData.startDate;
                $scope.endDate = dialogData.endDate;
                loadCustomData();
            }

            function clearDates() {
                $scope.loaded = false;
                $scope.preValuesLoaded = false;
                loadDefaultData();
            }

        }]);

angular.module('merchello').controller('Merchello.Sales.Dialogs.InvoiceHeaderController',
    ['$scope',
    function($scope) {

        function init() {
           $scope.loaded = true;
        }

        init();
}]);

'use strict';
/**
 * @ngdoc controller
 * @name Merchello.Sales.Dialog.CreateShipmentController
 * @function
 *
 * @description
 * The controller for the dialog used in creating a shipment
 */
angular.module('merchello')
    .controller('Merchello.Sales.Dialogs.CreateShipmentController',
    ['$scope', 'localizationService', 'ShipmentRequestDisplay', 'OrderDisplay', function($scope, localizationService, ShipmentRequestDisplay, OrderDisplay) {

        $scope.save = save;
        $scope.currencySymbol = '';
        $scope.loaded = false;
        $scope.updateViewSelectedForShipment = updateViewSelectedForShipment;
        $scope.shipmentSelection = {
            show: false,
            shipments: [],
            selected: {}
        };

        var customLabel = 'Custom';

        function init() {

            //$scope.dialogData.shipMethods.alternatives = _.sortBy($scope.dialogData.shipMethods.alternatives, function(methods) { return method.name; } );
            if($scope.dialogData.shipMethods.selected === null || $scope.dialogData.shipMethods.selected === undefined) {
                $scope.dialogData.shipMethods.selected = $scope.dialogData.shipMethods.alternatives[0];
            }

            localizationService.localize('merchelloGeneral_custom').then(function(data) {
                customLabel = data;
                buildShipmentSelection();
                $scope.dialogData.shipmentRequest = new ShipmentRequestDisplay();
                $scope.dialogData.shipmentRequest.order = angular.extend($scope.dialogData.order, OrderDisplay);
                $scope.currencySymbol = $scope.dialogData.currencySymbol;
                updateViewSelectedForShipment();
                $scope.loaded = true;
            });
        }

        function buildShipmentSelection() {
            if ($scope.dialogData.shipmentLineItems.length > 1) {
                // Figure out which shipments have not been shipped (completely) and make them
                // available to the drop down.
                angular.forEach($scope.dialogData.shipmentLineItems, function(s) {
                    var items = getShipmentItemsNotPackaged(s);
                    //console.info($scope.dialogData.order);
                    //console.info(s);
                    if (items.length > 0) {
                        $scope.shipmentSelection.shipments.push({ key: s.key, name: s.name, shipMethodKey: s.extendedData.getValue('merchShipMethodKey'), items: items });
                    }
                });
            }

            if ($scope.shipmentSelection.shipments.length > 0) {
                $scope.shipmentSelection.show = true;
            }

            $scope.shipmentSelection.shipments.push({ key: '', name: customLabel, shipMethodKey: $scope.dialogData.shipMethods.selected.key, items: $scope.dialogData.order.items });
            $scope.shipmentSelection.selected = $scope.shipmentSelection.shipments[0];
        }


        function updateViewSelectedForShipment() {
            // set the check boxes
            _.each($scope.dialogData.order.items, function(item) {
                var fnd = _.find($scope.shipmentSelection.selected.items, function(i) {
                    if (item.key === i.key) { return i; }
                });
                item.selected = fnd !== undefined;
            });

            // set the shipmethod
            var method = _.find($scope.dialogData.shipMethods.alternatives, function(sm) {
                if ($scope.shipmentSelection.selected.shipMethodKey === sm.key) {
                    return sm;
                }
            });
            if (method !== undefined) {
                $scope.dialogData.shipMethods.selected = method;
            }
        }


        // get the unpackaged items
        function getShipmentItemsNotPackaged(shipment) {

            var skus = getLineItemSkus(shipment.extendedData);
            if (skus.length === 0) {
                return [];
            }
            // get the order line items by matching the skus
            var remainingItems = _.filter($scope.dialogData.order.items, function (oi) {
                var fnd = _.find(skus, function(s) { return oi.sku === s; });
                if (fnd !== undefined) {
                    return oi;
                }
            });

            return remainingItems;
        }

        // parses the merchLineItemCollection xml stored in extended data to find individual skus
        function getLineItemSkus(extendedData) {
            var txt = extendedData.getValue('merchLineItemCollection');
            // get the SKU's of each item in the shipment
            var regex = /<merchSku>(.+?)<\/merchSku>/g;
            var matches = [];
            var match = regex.exec(txt);
            while (match != null) {
                matches.push(match[1]);
                match = regex.exec(txt);
            }

            return matches;
        }


        function save() {
            $scope.dialogData.shipmentRequest.shipMethodKey = $scope.dialogData.shipMethods.selected.key;
            $scope.dialogData.shipmentRequest.shipmentStatusKey = $scope.dialogData.shipmentStatus.key;
            $scope.dialogData.shipmentRequest.carrier = $scope.dialogData.carrier;
            $scope.dialogData.shipmentRequest.trackingNumber = $scope.dialogData.trackingNumber;
            $scope.dialogData.shipmentRequest.trackingUrl = $scope.dialogData.trackingUrl;

            $scope.dialogData.shipmentRequest.order.items = _.filter($scope.dialogData.order.items, function (item) {
                return item.selected === true;
            });
            $scope.submit($scope.dialogData);
        }

        init();

    }]);

'use strict';
/**
 * @ngdoc controller
 * @name Merchello.Sales.Dialog.CreateShipmentController
 * @function
 *
 * @description
 * The controller for the dialog used in creating a shipment
 */
angular.module('merchello')
    .controller('Merchello.Sales.Dialogs.EditShipmentController',
    ['$scope', 'ShipmentRequestDisplay', 'OrderDisplay', function($scope, ShipmentRequestDisplay, OrderDisplay) {

        $scope.save = save;
        $scope.loaded = false;

        $scope.checkboxDisabled = checkboxDisabled;

        function init() {
            _.each($scope.dialogData.shipment.items, function(item) {
                item.selected = true;
            });
            $scope.loaded = true;
        }

        function checkboxDisabled() {
            return $scope.dialogData.shipment.shipmentStatus.name === 'Shipped' || $scope.dialogData.shipment.shipmentStatus.name === 'Delivered'
        }

        function save() {
            $scope.dialogData.shipment.items = _.filter($scope.dialogData.shipment.items, function (item) {
                return item.selected === true;
            });
            $scope.submit($scope.dialogData);
        }

        init();

    }]);


angular.module('merchello').controller('Merchello.Sales.Dialogs.ManageAdjustmentsController',
    ['$scope', '$filter', 'invoiceLineItemDisplayBuilder',
    function($scope, $filter, invoiceLineItemDisplayBuilder) {

        $scope.deleteAdjustment = deleteAdjustment;
        $scope.addAdjustment = addAdjustment;

        $scope.preValuesLoaded = true;
        $scope.save = save;
        $scope.operator = '-';
        $scope.invoiceNumber = '';
        $scope.adjustments = [];
        $scope.amount = 0.0;
        $scope.name = '';

        function init() {
            $scope.invoiceNumber = $scope.dialogData.invoice.prefixedInvoiceNumber();
            var adjustments = $scope.dialogData.invoice.getAdjustmentLineItems();
            if (adjustments !== undefined && adjustments !== null) {
                $scope.adjustments = adjustments;
            }
        }
        
        function deleteAdjustment(item) {
            if (item.isNew !== undefined) {
                $scope.adjustments = _.reject($scope.adjustments, function(adj) {
                   return adj.isNew === true && adj.name === item.name;
                });
            } else {
                $scope.adjustments = _.reject($scope.adjustments, function(adj) {
                   return adj.key === item.key;
                });
            }
        }

        function addAdjustment() {
            if ($scope.name !== '') {
                var lineItem = invoiceLineItemDisplayBuilder.createDefault();
                lineItem.quantity = 1;
                lineItem.name = $scope.name;
                lineItem.containerKey = $scope.dialogData.invoice.key;
                lineItem.lineItemType = 'Adjustment';
                var amount = Math.abs($scope.amount);
                lineItem.price = $scope.operator === '+' ? amount : -1 * amount;
                lineItem.sku = 'adj';
                lineItem.isNew = true;
                $scope.name = '';
                $scope.amount = 0;
                $scope.operator = '-';
                $scope.adjustments.push(lineItem);
            }
        }

        function save() {
            
            var items = [];
            _.each($scope.adjustments, function(adj) {
               items.push({ key: adj.key, name: adj.name, price: adj.price });
            });

            var invoiceAdjustmentDisplay = {
                invoiceKey: $scope.dialogData.invoice.key,
                items: items
            };
            $scope.submit(invoiceAdjustmentDisplay);
        }

        init();

    }]);

/**
 * @ngdoc controller
 * @name Merchello.Dashboards.InvoicePaymentsController
 * @function
 *
 * @description
 * The controller for the invoice payments view
 */
angular.module('merchello').controller('Merchello.Backoffice.InvoicePaymentsController',
    ['$scope', '$log', '$routeParams', 'dialogService', 'notificationsService', 'merchelloTabsFactory', 'invoiceHelper', 'dialogDataFactory',
        'invoiceResource', 'paymentResource', 'paymentGatewayProviderResource', 'settingsResource',
        'invoiceDisplayBuilder', 'paymentDisplayBuilder', 'paymentMethodDisplayBuilder',
        function($scope, $log, $routeParams, dialogService, notificationsService, merchelloTabsFactory, invoiceHelper, dialogDataFactory, invoiceResource, paymentResource, paymentGatewayProviderResource, settingsResource,
        invoiceDisplayBuilder, paymentDisplayBuilder, paymentMethodDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
            $scope.invoice = {};
            $scope.payments = [];
            $scope.paymentMethods = [];
            $scope.remainingBalance = 0;
            $scope.settings = {};
            $scope.currencySymbol = '';
            $scope.showAddPayment = false;

            // exposed methods
            $scope.openVoidPaymentDialog = openVoidPaymentDialog;
            $scope.openRefundPaymentDialog = openRefundPaymentDialog;
            $scope.showVoid = showVoid;
            $scope.showRefund = showRefund;

            function init() {
                var key = $routeParams.id;
                loadInvoice(key);
                $scope.tabs = merchelloTabsFactory.createSalesTabs(key);
                $scope.tabs.setActive('payments');
            }

            /**
             * @ngdoc method
             * @name loadInvoice
             * @function
             *
             * @description - Load an invoice with the associated id.
             */
            function loadInvoice(id) {
                var promise = invoiceResource.getByKey(id);
                promise.then(function (invoice) {
                    $scope.invoice = invoiceDisplayBuilder.transform(invoice);
                    $scope.billingAddress = $scope.invoice.getBillToAddress();
                    // append the customer tab if needed
                    $scope.tabs.appendCustomerTab($scope.invoice.customerKey);
                    loadPayments(id);
                    loadSettings();
                    $scope.loaded = true;
                }, function (reason) {
                    notificationsService.error("Invoice Load Failed", reason.message);
                });
            };

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description - Load the Merchello settings.
             */
            function loadSettings() {

                var settingsPromise = settingsResource.getAllSettings();
                settingsPromise.then(function(settings) {
                    $scope.settings = settings;
                }, function(reason) {
                    notificationsService.error('Failed to load global settings', reason.message);
                })

                var currencySymbolPromise = settingsResource.getAllCurrencies();
                currencySymbolPromise.then(function (symbols) {
                    var currency = _.find(symbols, function(symbol) {
                        return symbol.currencyCode === $scope.invoice.getCurrencyCode();
                    });
                    if (currency !== undefined && currency !== null) {
                        $scope.currencySymbol = currency.symbol;
                    } else {
                        // this handles a legacy error where in some cases the invoice may not have saved the ISO currency code
                        // default currency
                        var defaultCurrencyPromise = settingsResource.getCurrencySymbol();
                        defaultCurrencyPromise.then(function (currencySymbol) {
                            $scope.currencySymbol = currencySymbol;
                        }, function (reason) {
                            notificationService.error('Failed to load the default currency symbol', reason.message);
                        });
                    }
                }, function (reason) {
                    alert('Failed: ' + reason.message);
                });
            };

            function showVoid(payment) {
                if (payment.voided) {
                    return false;
                }
                var exists = _.find($scope.paymentMethods, function(pm) { return pm.key === payment.paymentMethodKey; });
                if (exists !== undefined) {
                    return exists.voidPaymentEditorView.editorView !== '';
                } else {
                    return false;
                }
            }

            function showRefund(payment) {
                if (payment.voided || payment.appliedAmount() === 0) {
                    return false;
                }
                var exists = _.find($scope.paymentMethods, function(pm) { return pm.key === payment.paymentMethodKey; });
                if (exists !== undefined) {
                    return exists.refundPaymentEditorView.editorView !== '';
                } else {
                    return false;
                }
            }
            /**
             * @ngdoc method
             * @name loadPayments
             * @function
             *
             * @description - Load the payments applied to an invoice.
             */
            function loadPayments(key)
            {
                var paymentsPromise = paymentResource.getPaymentsByInvoice(key);
                paymentsPromise.then(function(payments) {
                    $scope.payments = paymentDisplayBuilder.transform(payments);
                    $scope.remainingBalance = invoiceHelper.round($scope.invoice.remainingBalance($scope.payments), 2);
                    loadPaymentMethods($scope.payments);
                }, function(reason) {
                    notificationsService.error('Failed to load payments for invoice', reason.message);
                });
            }

            function loadPaymentMethods(payments) {
                var keys = [];
                // we need to get unique keys here so we don't have to look up for every payment
                angular.forEach(payments, function(p) {
                    if(payments.length > 0) {
                        var found = false;
                        var i = 0;
                        while(i < keys.length && !found) {
                            if (keys[i] === p.paymentMethodKey) {
                                found = true;
                            } else {
                                i++;
                            }
                        }

                        if (!found) {
                            if (p.paymentMethodKey === null) {
                                keys.push('removed');
                            } else {
                                keys.push(p.paymentMethodKey);
                            }
                        }
                    }
                });

                angular.forEach(keys, function(key) {

                    if (key === 'removed') {
                        var empty = paymentMethodDisplayBuilder.createDefault();
                            $scope.paymentMethods.push(empty)
                        }   
                        var promise = paymentGatewayProviderResource.getPaymentMethodByKey(key);
                        promise.then(function(method) {
                            $scope.paymentMethods.push(method);
                            if ($scope.paymentMethods.length === keys.length) {
                                $scope.preValuesLoaded = true;
                            }
                        });
                    
                });

                if ($scope.paymentMethods.length === keys.length) {
                    $scope.preValuesLoaded = true;
                }
            }

            // dialog methods

            function openVoidPaymentDialog(payment) {

                var method = _.find($scope.paymentMethods, function(pm) { return pm.key === payment.paymentMethodKey; });
                if (method === undefined) {
                    return;
                }

                var dialogData = dialogDataFactory.createProcessVoidPaymentDialogData();
                dialogData.paymentMethodKey = payment.paymentMethodKey;
                dialogData.paymentKey = payment.key;
                dialogData.invoiceKey = $scope.invoice.key;
                dialogService.open({
                    template: method.voidPaymentEditorView.editorView,
                    show: true,
                    callback: processVoidPaymentDialog,
                    dialogData: dialogData
                });
            }

            function processVoidPaymentDialog(dialogData) {
                $scope.loaded = false;
                var request = dialogData.toPaymentRequestDisplay();
                var promise = paymentResource.voidPayment(request);
                promise.then(function(result) {
                    init();
                });
            }

            function openRefundPaymentDialog(payment) {
                var method = _.find($scope.paymentMethods, function(pm) { return pm.key === payment.paymentMethodKey; });
                if (method === undefined) {
                    return;
                }
                var dialogData = dialogDataFactory.createProcessRefundPaymentDialogData();
                dialogData.invoiceKey = $scope.invoice.key;
                dialogData.paymentKey = payment.key;
                dialogData.currencySymbol = $scope.currencySymbol;
                dialogData.paymentMethodKey = payment.paymentMethodKey;
                dialogData.paymentMethodName = method.name;

                dialogData.appliedAmount = payment.appliedAmount();
                dialogService.open({
                    template: method.refundPaymentEditorView.editorView,
                    show: true,
                    callback: processRefundPaymentDialog,
                    dialogData: dialogData
                });
            }

            function processRefundPaymentDialog(dialogData) {
                $scope.loaded = false;
                var request = dialogData.toPaymentRequestDisplay();
                var promise = paymentResource.refundPayment(request);
                promise.then(function(result) {
                    init();
                });
            }

            init();
}]);

/**
 * @ngdoc controller
 * @name Merchello.Dashboards.OrderShipmentsController
 * @function
 *
 * @description
 * The controller for the order shipments view
 */
angular.module('merchello').controller('Merchello.Backoffice.OrderShipmentsController',
    ['$scope', '$routeParams', '$log', 'notificationsService', 'dialogService', 'dialogDataFactory', 'merchelloTabsFactory',
        'invoiceResource', 'settingsResource', 'shipmentResource',
        'invoiceDisplayBuilder', 'shipmentDisplayBuilder',
        function($scope, $routeParams, $log, notificationsService, dialogService, dialogDataFactory, merchelloTabsFactory, invoiceResource,
                 settingsResource, shipmentResource, invoiceDisplayBuilder, shipmentDisplayBuilder) {

            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.tabs = [];
            $scope.invoice = {};
            $scope.settings = {};
            $scope.shipments = [];

            // methods
            $scope.isEditableAddress = isEditableAddress;
            $scope.updateShippingAddressLineItem = updateShippingAddressLineItem;

            // dialogs
            $scope.openShipmentDialog = openShipmentDialog;
            $scope.openAddressDialog = openAddressDialog;
            $scope.openDeleteShipmentDialog = openDeleteShipmentDialog;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description - Controller initialization.
             */
            function init() {
                var key = $routeParams.id;
                loadInvoice(key);
                $scope.tabs = merchelloTabsFactory.createSalesTabs(key);
                $scope.tabs.setActive('shipments');
                $scope.loaded = true;
            }

            /**
             * @ngdoc method
             * @name loadInvoice
             * @function
             *
             * @description - responsible for loading the invoice.
             */
            function loadInvoice(key) {
                var invoicePromise = invoiceResource.getByKey(key);
                invoicePromise.then(function(invoice) {
                    $scope.invoice = invoice;
                    // append the customer tab
                    $scope.tabs.appendCustomerTab($scope.invoice.customerKey);
                    loadSettings();
                    var shipmentsPromise = shipmentResource.getShipmentsByInvoice(invoice);
                    shipmentsPromise.then(function(shipments) {
                        $scope.shipments = shipmentDisplayBuilder.transform(shipments);
                        $scope.preValuesLoaded = true;
                    })
                }, function(reason) {
                    notificationsService.error('Failed to load invoice', reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description - Load the Merchello settings.
             */
            function loadSettings() {
                var settingsPromise = settingsResource.getAllSettings();
                settingsPromise.then(function (settings) {
                    $scope.settings = settings;
                }, function (reason) {
                    notificationsService.error('Failed to load global settings', reason.message);
                })
            }

            /**
             * @ngdoc method
             * @name isEditableStatus
             * @function
             *
             * @description - Returns a value indicating whether or not the shipment address can be edited.
             */
            function isEditableAddress(shipmentStatus) {
                if (shipmentStatus.name === 'Delivered' || shipmentStatus.name === 'Shipped') {
                    return false;
                }
                return true;
            }

            /*--------------------------------------------------------------------------------
                Dialogs
            ----------------------------------------------------------------------------------*/

            function updateShippingAddressLineItem(shipment) {
                var promise = shipmentResource.updateShippingAddressLineItem(shipment);
                promise.then(function() {
                    loadInvoice($scope.invoice.key);
                    notificationsService.success('Successfully updated sales shipping address.')
                }, function(reason) {
                    notificationsService.error('Failed to update shipping addresses on invoice', reason.message);
                })
            }


            function openDeleteShipmentDialog(shipment) {
                var dialogData = {};
                dialogData.name = 'Shipment #' + shipment.shipmentNumber;
                dialogData.shipment = shipment;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processDeleteShipmentDialog,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name openShipmentDialog
             * @function
             *
             * @description - responsible for opening the edit shipment dialog and passing the selected shipment.
             */
            function openShipmentDialog(shipment) {
                var promiseStatuses = shipmentResource.getAllShipmentStatuses();
                promiseStatuses.then(function(statuses) {
                    var dialogData = dialogDataFactory.createEditShipmentDialogData();
                    dialogData.shipment = shipment;
                    dialogData.shipmentStatuses = statuses;
                    dialogData.shipment.shipmentStatus = _.find(statuses, function(status) {
                      return status.key === dialogData.shipment.shipmentStatus.key;
                    });

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/sales.edit.shipment.html',
                        show: true,
                        callback: processUpdateShipment,
                        dialogData: dialogData
                    });
                });
            }

            /**
             * @ngdoc method
             * @name openAddressDialog
             * @function
             *
             * @description - responsible for opening the edit address dialog with the appropriate address to be edited
             */
            function openAddressDialog(shipment, addressType) {
                var dialogData = dialogDataFactory.createEditAddressDialogData();
                if (addressType === 'destination') {
                    dialogData.address = shipment.getDestinationAddress();
                    dialogData.showPhone = true;
                    dialogData.showEmail = true;
                    dialogData.showIsCommercial = true;
                }
                else
                {
                    dialogData.address = shipment.getOriginAddress();
                }

                // add the shipment -- this modifies the EditAddressDialogData model with an extra property
                dialogData.shipment = shipment;

                // get the list of countries to populate the countries drop down
                var countryPromise = settingsResource.getAllCountries();
                countryPromise.then(function(countries) {
                    dialogData.countries = countries;

                    dialogData.selectedCountry = _.find(countries, function(country) {
                        return country.countryCode === dialogData.address.countryCode;
                    });

                    // if this address has a region ... we need to get that too.
                    if(dialogData.address.region !== '' && dialogData.address.region !== null && dialogData.selectedCountry.provinces.length > 0) {
                        dialogData.selectedProvince = _.find(dialogData.selectedCountry.provinces, function(region) {
                            return region.code === dialogData.address.region;
                        });
                    }

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/edit.address.html',
                        show: true,
                        callback: addressType === 'destination' ? processUpdateDestinationAddress : processUpdateOriginAddress,
                        dialogData: dialogData
                    });
                });
            }

            /**
             * @ngdoc method
             * @name processUpdateOriginAddres
             * @function
             *
             * @description - updates the origin address on the shipment.
             */
            function processUpdateOriginAddress(dialogData) {
                $scope.preValuesLoaded = false;
                var shipment = dialogData.shipment;
                shipment.setOriginAddress(dialogData.address);
                saveShipment(shipment);
            }

            /**
             * @ngdoc method
             * @name processUpdateDestinationAddress
             * @function
             *
             * @description - updates the destination address of a shipment.
             */
            function processUpdateDestinationAddress(dialogData) {
                $scope.preValuesLoaded = false;
                var shipment = dialogData.shipment;
                shipment.setDestinationAddress(dialogData.address);
                saveShipment(shipment);
            }

            /**
             * @ngdoc method
             * @name processUpdateShipment
             * @function
             *
             * @description - responsible for handling dialog data for updating a shipment.
             */
            function processUpdateShipment(dialogData) {
                $scope.preValuesLoaded = false;
                if(dialogData.shipment.items.length > 0) {
                    saveShipment(dialogData.shipment);
                } else {
                    notificationsService.warning('Cannot remove all items from the shipment.  Instead, consider deleting the shipment.');
                    loadInvoice($scope.invoice.key);
                };
            }

            /**
             * @ngdoc method
             * @name processDeleteShipmentDialog
             * @function
             *
             * @description - responsible for deleting a shipment.
             */
            function processDeleteShipmentDialog(dialogData) {
                var promise = shipmentResource.deleteShipment(dialogData.shipment);
                promise.then(function() {
                    loadInvoice($scope.invoice.key);
                }, function(reason) {
                  notificationsService.error('Failed to delete the invoice.', reason.message)
                });
            }

            /**
             * @ngdoc method
             * @name saveShipment
             * @function
             *
             * @description - responsible for saving a shipment.
             */
            function saveShipment(shipment) {

                var promise = shipmentResource.saveShipment(shipment);
                promise.then(function(shipment) {
                    loadInvoice($scope.invoice.key);
                });
            }


            // initializes the controller
            init();
    }]);

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Sales.OverviewController
     * @function
     *
     * @description
     * The controller for the sales overview view
     */
    angular.module('merchello').controller('Merchello.Backoffice.SalesOverviewController',
        ['$scope', '$routeParams', '$timeout', '$log', '$location', 'assetsService', 'dialogService', 'localizationService', 'notificationsService', 'invoiceHelper',
            'auditLogResource', 'noteResource', 'invoiceResource', 'settingsResource', 'paymentResource', 'shipmentResource', 'paymentGatewayProviderResource',
            'orderResource', 'dialogDataFactory', 'merchelloTabsFactory', 'addressDisplayBuilder', 'countryDisplayBuilder', 'salesHistoryDisplayBuilder', 'noteDisplayBuilder',
            'invoiceDisplayBuilder', 'paymentDisplayBuilder', 'paymentMethodDisplayBuilder', 'shipMethodsQueryDisplayBuilder', 'noteDisplayBuilder',
        function($scope, $routeParams, $timeout, $log, $location, assetsService, dialogService, localizationService, notificationsService, invoiceHelper,
                 auditLogResource, noteResource, invoiceResource, settingsResource, paymentResource, shipmentResource, paymentGatewayProviderResource, orderResource, dialogDataFactory,
                 merchelloTabsFactory, addressDisplayBuilder, countryDisplayBuilder, salesHistoryDisplayBuilder, noteDisplayBuilder, invoiceDisplayBuilder, paymentDisplayBuilder, paymentMethodDisplayBuilder, shipMethodsQueryDisplayBuilder, noteDisplayBuilder) {

            // exposed properties
            $scope.loaded = false;
            $scope.preValuesLoaded = false;
            $scope.paymentMethodsLoaded = false;
            $scope.invoice = {};
            $scope.invoiceNumber = '';
            $scope.tabs = [];
            $scope.historyLoaded = false;
            $scope.currencySymbol = '';
            $scope.settings = {};
            $scope.salesHistory = {};

            $scope.paymentMethods = [];
            $scope.allPayments = [];
            $scope.payments = [];
            $scope.billingAddress = {};
            $scope.hasShippingAddress = false;
            $scope.discountLineItems = [];
            $scope.debugAllowDelete = false;
            $scope.newPaymentOpen = false;
            $scope.entityType = 'Invoice';
            

            // exposed methods
            //  dialogs
            $scope.capturePayment = capturePayment;
            $scope.showFulfill = true;
            $scope.openDeleteInvoiceDialog = openDeleteInvoiceDialog;
            $scope.processDeleteInvoiceDialog = processDeleteInvoiceDialog,
            $scope.openFulfillShipmentDialog = openFulfillShipmentDialog;
            $scope.processFulfillShipmentDialog = processFulfillShipmentDialog;

            $scope.toggleNewPaymentOpen = toggleNewPaymentOpen;
            $scope.reload = init;
            $scope.openAddressAddEditDialog = openAddressAddEditDialog;
            $scope.setNotPreValuesLoaded = setNotPreValuesLoaded;
            $scope.saveNotes = saveNotes;
            $scope.deleteNote = deleteNote;

            // localize the sales history message
            $scope.localizeMessage = localizeMessage;

            $scope.refresh = refresh;


            var countries = [];

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description - Method called on intial page load.  Loads in data from server and sets up scope.
             */
            function init () {
                $scope.preValuesLoaded = false;
                $scope.newPaymentOpen = false;
                loadInvoice($routeParams.id);
                $scope.tabs = merchelloTabsFactory.createSalesTabs($routeParams.id);
                $scope.tabs.setActive('overview');
                if(Umbraco.Sys.ServerVariables.isDebuggingEnabled) {
                    $scope.debugAllowDelete = true;
                }

            }

            function localizeMessage(msg) {
                return msg.localize(localizationService);
            }

            /**
             * @ngdoc method
             * @name loadAuditLog
             * @function
             *
             * @description
             * Load the Audit Log for the invoice via API.
             */
            function loadAuditLog(key) {
                if (key !== undefined) {
                    var promise = auditLogResource.getSalesHistoryByInvoiceKey(key);
                    promise.then(function (response) {
                        var history = salesHistoryDisplayBuilder.transform(response);
                        // TODO this is a patch for a problem in the API
                        if (history.dailyLogs.length) {
                            $scope.salesHistory = history.dailyLogs;
                            angular.forEach(history.dailyLogs, function(daily) {
                              angular.forEach(daily.logs, function(log) {
                                  if (log.message.formattedMessage ==='') {
                                     localizationService.localize(log.message.localizationKey(), log.message.localizationTokens()).then(function(value) {
                                        log.message.formattedMessage = value;
                                     });
                                  }
                              });
                            });
                        }
                        $scope.historyLoaded = history.dailyLogs.length > 0;
                    }, function (reason) {
                        notificationsService.error('Failed to load sales history', reason.message);
                    });
                }
            }



            /**
             * @ngdoc method
             * @name loadInvoice
             * @function
             *
             * @description - Load an invoice with the associated id.
             */
            function loadInvoice(id) {
                // assert the collections are reset before populating
                $scope.shipmentLineItems = [];
                $scope.customLineItems = [];
                $scope.discountLineItems = [];
                var promise = invoiceResource.getByKey(id);
                promise.then(function (invoice) {

                    $scope.invoice = invoiceDisplayBuilder.transform(invoice);
                    $scope.billingAddress = $scope.invoice.getBillToAddress();

                    $scope.invoiceNumber = $scope.invoice.prefixedInvoiceNumber();
                    loadSettings();
                    loadPayments(id);
                    loadAuditLog(id);

                    loadShippingAddress(id);

                    $scope.showFulfill = hasUnPackagedLineItems();
                    $scope.loaded = true;

                    var shipmentLineItem = $scope.invoice.getShippingLineItems();
                    if (shipmentLineItem) {
                        $scope.shipmentLineItems.push(shipmentLineItem);
                    }

                   $scope.tabs.appendCustomerTab($scope.invoice.customerKey);


                }, function (reason) {
                    notificationsService.error("Invoice Load Failed", reason.message);
                });
            }

           /**
             * @ngdoc method
             * @name loadSettings
             * @function
             *
             * @description - Load the Merchello settings.
             */
            function loadSettings() {
               settingsResource.getAllCombined().then(function(combined) {
                   $scope.settings = combined.settings;
                   countries = combined.countries;
                   if ($scope.invoice.currency.symbol === '') {
                       var currency = _.find(combined.currencies, function (symbol) {
                           return symbol.currecyCode === $scope.invoice.getCurrencyCode()
                       });
                       if (currency !== undefined) {
                           $scope.currencySymbol = currency.symbol;
                       } else {
                           $scope.currencySymbol = combined.currencySymbol;
                       }
                   } else {
                       $scope.currencySymbol = $scope.invoice.currency.symbol;
                   }
               });
           }


            /**
             * @ngdoc method
             * @name loadPayments
             * @function
             *
             * @description - Load the Merchello payments for the invoice.
             */
            function loadPayments(key) {

                var paymentsPromise = paymentResource.getPaymentsByInvoice(key);
                paymentsPromise.then(function(payments) {
                    $scope.allPayments = paymentDisplayBuilder.transform(payments);
                    $scope.payments = _.filter($scope.allPayments, function(p) { return !p.voided && !p.collected && p.authorized; });
                    loadPaymentMethods();
                    $scope.preValuesLoaded = true;
                }, function(reason) {
                    notificationsService.error('Failed to load payments for invoice', reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name loadPaymentMethods
             * @function
             *
             * @description - Load the available Merchello payment methods for the invoice.
             */
            function loadPaymentMethods() {
                if($scope.payments.length === 0) {
                    var promise = paymentGatewayProviderResource.getAvailablePaymentMethods();
                    promise.then(function(methods) {
                        $scope.paymentMethods = paymentMethodDisplayBuilder.transform(methods);
                        $scope.preValuesLoaded = true;
                        $scope.paymentMethodsLoaded = true;
                    });
                }
            }

            /**
             * @ngdoc method
             * @name loadShippingAddress
             * @function
             *
             * @description - Load the shipping address (if any) for an invoice.
             */
            function loadShippingAddress(key) {
                var shippingAddressPromise = orderResource.getShippingAddress(key);
                shippingAddressPromise.then(function(result) {
                      $scope.shippingAddress = addressDisplayBuilder.transform(result);
                      $scope.hasShippingAddress = true;
                }, function(reason) {
                    notificationsService.error('Failed to load shipping address', reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name toggleNewPaymentOpen
             * @function
             *
             * @description - Toggles the new payment open variable.
             */
            function toggleNewPaymentOpen() {
                $scope.newPaymentOpen = !$scope.newPaymentOpen;
            }

            /**
             * @ngdoc method
             * @name setNotPreValuesLoaded
             * @function
             *
             * @description - Sets preValuesLoaded to false.  For use in directives.
             */
            function setNotPreValuesLoaded() {
                $scope.preValuesLoaded = false;
            }

            /**
             * @ngdoc method
             * @name capturePayment
             * @function
             *
             * @description - Open the capture payment dialog.
             */
            function capturePayment() {
                var dialogData = dialogDataFactory.createCapturePaymentDialogData();
                dialogData.setPaymentData($scope.payments[0]);
                dialogData.setInvoiceData($scope.payments, $scope.invoice, $scope.currencySymbol);
                if (!dialogData.isValid()) {
                    return false;
                }

                /*
                    We need to be able to swap out the editor depending on the provider here.
                */

                var promise = paymentResource.getPaymentMethod(dialogData.paymentMethodKey);
                promise.then(function(paymentMethod) {
                    var pm = paymentMethodDisplayBuilder.transform(paymentMethod);
                    if (pm.capturePaymentEditorView.editorView !== '') {
                        dialogData.capturePaymentEditorView = pm.capturePaymentEditorView.editorView;
                    } else {
                        dialogData.capturePaymentEditorView = '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.cashpaymentmethod.authorizecapturepayment.html';
                    }
                    dialogService.open({
                        template: dialogData.capturePaymentEditorView,
                        show: true,
                        callback: capturePaymentDialogConfirm,
                        dialogData: dialogData
                    });
                });
            }

            /**
             * @ngdoc method
             * @name capturePaymentDialogConfirm
             * @function
             *
             * @description - Capture the payment after the confirmation dialog was passed through.
             */
            function capturePaymentDialogConfirm(paymentRequest) {
                $scope.preValuesLoaded = false;
                var promiseSave = paymentResource.capturePayment(paymentRequest);
                promiseSave.then(function (payment) {
                    // added a timeout here to give the examine index
                    $timeout(function() {
                        notificationsService.success("Payment Captured");
                        loadInvoice(paymentRequest.invoiceKey);
                    }, 800);
                }, function (reason) {
                    notificationsService.error("Payment Capture Failed", reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name openDeleteInvoiceDialog
             * @function
             *
             * @description - Open the delete payment dialog.
             */
            function openDeleteInvoiceDialog() {
                var dialogData = {};
                dialogData.name = 'Invoice #' + $scope.invoice.invoiceNumber;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processDeleteInvoiceDialog,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name openFulfillShipmentDialog
             * @function
             *
             * @description - Open the fufill shipment dialog.
             */
            function openFulfillShipmentDialog() {
                var promiseStatuses = shipmentResource.getAllShipmentStatuses();
                promiseStatuses.then(function(statuses) {
                    var data = dialogDataFactory.createCreateShipmentDialogData();
                    data.order = $scope.invoice.orders[0];
                    data.order.items = data.order.getUnShippedItems();
                    data.shipmentStatuses = statuses;
                    data.currencySymbol = $scope.currencySymbol;

                    // packaging
                    var quotedKey = '7342dcd6-8113-44b6-bfd0-4555b82f9503';
                    data.shipmentStatus = _.find(data.shipmentStatuses, function(status) {
                        return status.key === quotedKey;
                    });
                    data.invoiceKey = $scope.invoice.key;

                    // TODO this could eventually turn into an array
                    var shipmentLineItems = $scope.invoice.getShippingLineItems();
                    data.shipmentLineItems = shipmentLineItems;
                    if (shipmentLineItems.length) {
                        var shipMethodKey = shipmentLineItems[0].extendedData.getValue('merchShipMethodKey');
                        var request = { shipMethodKey: shipMethodKey, invoiceKey: data.invoiceKey, lineItemKey: shipmentLineItems[0].key };
                        var shipMethodPromise = shipmentResource.getShipMethodAndAlternatives(request);
                        shipMethodPromise.then(function(result) {
                            data.shipMethods = shipMethodsQueryDisplayBuilder.transform(result);
                            data.shipMethods.selected = _.find(data.shipMethods.alternatives, function(method) {
                                return method.key === shipMethodKey;
                            });

                            dialogService.open({
                                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/sales.create.shipment.html',
                                show: true,
                                callback: $scope.processFulfillShipmentDialog,
                                dialogData: data
                            });

                        });
                    }
                });
            }

            /**
             * @ngdoc method
             * @name processDeleteInvoiceDialog
             * @function
             *
             * @description - Delete the invoice.
             */
            function processDeleteInvoiceDialog() {
                var promiseDeleteInvoice = invoiceResource.deleteInvoice($scope.invoice.key);
                promiseDeleteInvoice.then(function (response) {
                    notificationsService.success('Invoice Deleted');
                    $location.url("/merchello/merchello/saleslist/manage", true);
                }, function (reason) {
                    notificationsService.error('Failed to Delete Invoice', reason.message);
                });
            }

            /**
             * @ngdoc method
             * @name processFulfillPaymentDialog
             * @function
             *
             * @description - Process the fulfill shipment functionality on callback from the dialog service.
             */
            function processFulfillShipmentDialog(data) {
                $scope.preValuesLoaded = false;
                if(data.shipmentRequest.order.items.length > 0) {
                    var promiseNewShipment = shipmentResource.newShipment(data.shipmentRequest);
                    promiseNewShipment.then(function (shipment) {
                        $timeout(function() {
                            notificationsService.success('Shipment #' + shipment.shipmentNumber + ' created');
                            //console.info(shipment);
                            loadInvoice(data.invoiceKey);
                        }, 800);

                    }, function (reason) {
                        notificationsService.error("New Shipment Failed", reason.message);
                    });
                } else {
                    $scope.preValuesLoaded = true;
                    notificationsService.warning('Shipment would not contain any items', 'The shipment was not created as it would not contain any items.');
                }
            }

            /**
             * @ngdoc method
             * @name hasUnPackagedLineItems
             * @function
             *
             * @description - Process the fulfill shipment functionality on callback from the dialog service.
             */
            function hasUnPackagedLineItems() {
                var fulfilled = $scope.invoice.getFulfillmentStatus() === 'Fulfilled';
                if (fulfilled) {
                    return false;
                }
                var i = 0; // order count
                var found = false;
                while(i < $scope.invoice.orders.length && !found) {
                    var item = _.find($scope.invoice.orders[ i ].items, function(item) {
                      return (item.shipmentKey === '' || item.shipmentKey === null) && item.extendedData.getValue('merchShippable').toLowerCase() === 'true';
                    });
                    if(item !== null && item !== undefined) {
                        found = true;
                    } else {
                        i++;
                    }
                }

                return found;
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
                var dialogData = dialogDataFactory.createEditAddressDialogData();
                // if the address is not defined we need to create a default (empty) AddressDisplay
                if(address === null || address === undefined) {
                    dialogData.address = addressDisplayBuilder.createDefault();
                    dialogData.selectedCountry = countries[0];
                } else {
                    dialogData.address = address.clone();
                    dialogData.selectedCountry = address.countryCode === '' ? countries[0] :
                        _.find(countries, function(country) {
                            return country.countryCode === address.countryCode;
                        });
                }
                dialogData.countries = countries;

                if (dialogData.selectedCountry.hasProvinces()) {
                    if(dialogData.address.region !== '') {
                        dialogData.selectedProvince = _.find(dialogData.selectedCountry.provinces, function(province) {
                            return province.code === address.region;
                        });
                    }
                    if(dialogData.selectedProvince === null || dialogData.selectedProvince === undefined) {
                        dialogData.selectedProvince = dialogData.selectedCountry.provinces[0];
                    }
                }

                if (address.addressType === 'Billing') {
                    dialogData.warning = localizationService.localize('merchelloSales_noteInvoiceAddressChange');
                } else {
                    dialogData.warning = localizationService.localize('merchelloSales_noteShipmentAddressChange');
                }


                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/edit.address.html',
                    show: true,
                    callback: processAddEditAddressDialog,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name processAddEditAddressDialog
             * @function
             *
             * @description
             * Responsible for editing an address
             */
            function processAddEditAddressDialog(dialogData) {
                var adr = dialogData.address;

                if (adr.addressType === 'Billing') {
                    $scope.invoice.setBillingAddress(adr);
                    $scope.preValuesLoaded = false;
                    var billingPromise = invoiceResource.saveInvoice($scope.invoice);
                    billingPromise.then(function () {
                        notificationsService.success('Billing address successfully updated.');
                        $timeout(function () {
                            loadInvoice($scope.invoice.key);
                        }, 400);
                    }, function (reason) {
                        notificationsService.error("Failed to update billing address", reason.message);
                    });
                } else {
                    // we need to update the shipment line item on the invoice
                    var adrData = {
                        invoiceKey: $scope.invoice.key,
                        address: dialogData.address
                    };
                    var shippingPromise = invoiceResource.saveInvoiceShippingAddress(adrData);
                    shippingPromise.then(function () {
                        notificationsService.success('Shipping address successfully updated.');
                        $timeout(function () {
                            loadInvoice($scope.invoice.key);
                        }, 400);
                    }, function (reason) {
                        notificationsService.error("Failed to update shippingaddress", reason.message);
                    });
                }
            }

            function saveNotes() {
                saveInvoice();
            }
            
            function deleteNote(note) {
                $scope.invoice.notes = _.reject($scope.invoice.notes, function(n) {
                    return n.key === note.key;
                });

                saveInvoice();
            }
            
            function saveInvoice() {
                invoiceResource.saveInvoice($scope.invoice).then(function(data) {
                    refresh();
                });
            }

            function refresh() {
                $timeout(function () {
                    loadInvoice($scope.invoice.key);
                }, 400);
            }

            // initialize the controller
            init();
    }]);

'use strict';
/**
 * @ngdoc controller
 * @name Merchello.Dashboards.Sales.ListController
 * @function
 *
 * @description
 * The controller for the orders list page
 */
angular.module('merchello').controller('Merchello.Backoffice.SalesListController',
    ['$scope', '$element', '$routeParams', '$q', '$log', '$filter', 'notificationsService', 'localizationService', 'merchelloTabsFactory', 'settingsResource',
        'invoiceResource', 'entityCollectionResource', 'invoiceDisplayBuilder', 'settingDisplayBuilder',
        function($scope, $element, $routeParams, $q, $log, $filter, notificationService, localizationService, merchelloTabsFactory, settingsResource, invoiceResource, entityCollectionResource,
                 invoiceDisplayBuilder, settingDisplayBuilder)
        {
            $scope.loaded = false;
            $scope.tabs = [];

            $scope.filterStartDate = '';
            $scope.filterEndDate = '';

            $scope.settings = {};
            $scope.entityType = 'Invoice';

            $scope.invoiceDisplayBuilder = invoiceDisplayBuilder;
            $scope.load = load;
            $scope.getColumnValue = getColumnValue;


            var allCurrencies = [];
            var globalCurrency = '$';
            const baseUrl = '#/merchello/merchello/saleoverview/';

            var paid = '';
            var unpaid = '';
            var partial = '';
            var unfulfilled = '';
            var fulfilled = '';
            var open = '';


            const label = '<i class="%0"></i> %1';

            function init() {
                $scope.tabs = merchelloTabsFactory.createSalesListTabs();
                $scope.tabs.setActive('saleslist');

                // localize

                var promises = [
                    localizationService.localize('merchelloSales_paid'),
                    localizationService.localize('merchelloSales_unpaid'),
                    localizationService.localize('merchelloSales_partial'),
                    localizationService.localize('merchelloOrder_fulfilled'),
                    localizationService.localize('merchelloOrder_unfulfilled'),
                    localizationService.localize('merchelloOrder_open'),
                    settingsResource.getAllCombined()
                ];

                $q.all(promises).then(function(local) {
                    paid = local[0];
                    unpaid = local[1];
                    partial = local[2];
                    fulfilled = local[3];
                    unfulfilled = local[4];
                    open = local[5];

                    $scope.settings = local[6].settings;
                    allCurrencies = local[6].currencies;
                    globalCurrency = local[6].currencySymbol;
                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                });
            }


            function load(query) {
                if (query.hasCollectionKeyParam()) {
                    return entityCollectionResource.getCollectionEntities(query);
                } else {
                    return invoiceResource.searchInvoices(query);
                }
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
                var currency = _.find(allCurrencies, function(currency) {
                    return currency.currencyCode === currencyCode;
                });
                if(currency === null || currency === undefined) {
                    return globalCurrency;
                } else {
                    return currency.symbol;
                }
            }

            function getEditUrl(invoice) {
                return baseUrl + invoice.key;
            }

            init();
        }]);


})();