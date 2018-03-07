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
                    localizationService.localize("merchelloNotification_successCouponSave").then(function (value) {
                        notificationsService.success(value);
                    });
                    if (isNew) {
                        $location.url($scope.offerProvider.editorUrl(settings.key), true);
                    } else {
                        $scope.offerSettings = undefined;
                        loadOffer(settings.key);
                    }
                }, function (reason) {
                    localizationService.localize("merchelloNotification_failCouponSave").then(function (value) {
                        notificationsService.error(value, reason.message);
                    });
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
