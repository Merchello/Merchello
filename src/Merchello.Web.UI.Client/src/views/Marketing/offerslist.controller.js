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