/**
 * @ngdoc controller
 * @name Merchello.Backoffice.OffersListController
 * @function
 *
 * @description
 * The controller for offers list view controller
 */
angular.module('merchello').controller('Merchello.Backoffice.OfferEditController',
    ['$scope', '$routeParams', '$location', 'assetsService', 'notificationsService', 'settingsResource', 'marketingResource', 'merchelloTabsFactory',
        'dialogDataFactory', 'settingDisplayBuilder', 'offerProviderDisplayBuilder', 'offerSettingsDisplayBuilder',
    function($scope, $routeParams, $location, assetsService, notificationsService, settingsResource, marketingResource, merchelloTabsFactory,
             dialogDataFactory, settingDisplayBuilder, offerProviderDisplayBuilder, offerSettingsDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.offerSettings = {};
        $scope.context = 'create';
        $scope.tabs = {};
        $scope.settings = {};
        $scope.offerProvider = {};

        function init() {
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

        function loadOfferProviders() {
            var providersPromise = marketingResource.getOfferProviders();
            providersPromise.then(function(providers) {
                var offerProviders = offerProviderDisplayBuilder.transform(providers);
                $scope.offerProvider = _.find(offerProviders, function(provider) {
                    return provider.backOfficeTree.routeId === 'coupons';
                });
                var key = $routeParams.id;
                $scope.tabs = merchelloTabsFactory.createMarketingTabs();
                $scope.tabs.appendOfferTab(key, $scope.offerProvider.backOfficeTree);
                $scope.tabs.setActive('offer');
               loadOffer(key);
                console.info($scope.offerProvider);
            }, function(reason) {
                notificationsService.error("Offer providers load failed", reason.message);
            });
        }

        function loadOffer(key) {

            if (key === 'create' || key === '' || key === undefined) {
                $scope.context = 'create';
                $scope.offerSettings = offerSettingsDisplayBuilder.createDefault();
                console.info($scope.offerSettings);
                $scope.preValuesLoaded = true;
                $scope.loaded = true;
            } else {
                $scope.preValuesLoaded = true;
                $scope.loaded = true;
            }
        }

        init();
    }]);
