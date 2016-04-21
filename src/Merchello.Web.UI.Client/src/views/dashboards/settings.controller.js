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
