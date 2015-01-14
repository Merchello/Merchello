/**
 * @ngdoc controller
 * @name Merchello.Backoffice.SettingsController
 * @function
 *
 * @description
 * The controller for the settings management page
 */
angular.module('merchello').controller('Merchello.Backoffice.SettingsController',
    ['$scope', '$log', 'serverValidationManager', 'notificationsService', 'settingsResource', 'settingDisplayBuilder',
        'currencyDisplayBuilder', 'countryDisplayBuilder',
        function($scope, $log, serverValidationManager, notificationsService, settingsResource, settingDisplayBuilder, currencyDisplayBuilder) {

            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.savingStoreSettings = false;
            $scope.settingsDisplay = settingDisplayBuilder.createDefault();
            $scope.currencies = [];
            $scope.selectedCurrency = {};

            // exposed methods
            $scope.currencyChanged = currencyChanged;
            $scope.save = save;

            function loadCurrency() {
                var promise = settingsResource.getAllCurrencies();
                promise.then(function(currenices) {
                    $scope.currencies = _.sortBy(currencyDisplayBuilder.transform(currenices), function(currency) {
                        return currency.name;
                    });
                    $scope.selectedCurrency = _.find($scope.currencies, function(currency) {
                      return currency.currencyCode === $scope.settingsDisplay.currencyCode;
                    });

                    $scope.loaded = true;
                    $scope.preValuesLoaded = true;
                }, function (reason) {
                    alert('Failed: ' + reason.message);
                });
            }

            function loadSettings() {
                var promise = settingsResource.getCurrentSettings();
                promise.then(function (settings) {
                    $scope.settingsDisplay = settingDisplayBuilder.transform(settings);
                    loadCurrency();
                }, function (reason) {
                    alert('Failed: ' + reason.message);
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
                        loadSettings();
                    }, function(reason) {
                        notificationsService.error("Store Settings Save Failed", reason.message);
                    });
                });

            }

            function currencyChanged(currency) {
                $scope.settingsDisplay.currencyCode = currency.currencyCode;
            }

            loadSettings();
}]);
