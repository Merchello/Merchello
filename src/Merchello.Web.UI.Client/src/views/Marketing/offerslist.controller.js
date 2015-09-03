/**
 * @ngdoc controller
 * @name Merchello.Backoffice.OffersListController
 * @function
 *
 * @description
 * The controller for offers list view controller
 */
angular.module('merchello').controller('Merchello.Backoffice.OffersListController',
    ['$scope', '$location', '$filter', 'notificationsService', 'localizationService', 'settingsResource', 'marketingResource', 'merchelloTabsFactory',
        'settingDisplayBuilder', 'offerProviderDisplayBuilder', 'offerSettingsDisplayBuilder',
    function($scope, $location, $filter, notificationsService, localizationService, settingsResource, marketingResource, merchelloTabsFactory,
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

                var promiseCurrency = settingsResource.getCurrencySymbol();
                promiseCurrency.then(function(symbol) {
                    $scope.currencySymbol = symbol;
                    localizationService.localize('general_yes').then(function(value) {
                        yes = value;
                    });
                    localizationService.localize('general_no').then(function(value) {
                        no = value;
                    });
                    localizationService.localize('merchelloGeneral_expired').then(function(value) {
                        expired = value;
                    });
                    loadOfferProviders();
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });

            }, function (reason) {
                notificationsService.error("Settings Load Failed", reason.message);
            });
        }

        function loadOfferProviders() {
            var providersPromise = marketingResource.getOfferProviders();
            providersPromise.then(function(providers) {
                $scope.offerProviders = offerProviderDisplayBuilder.transform(providers);
                $scope.preValuesLoaded = true;
            }, function(reason) {
                notificationsService.error("Offer providers load failed", reason.message);
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