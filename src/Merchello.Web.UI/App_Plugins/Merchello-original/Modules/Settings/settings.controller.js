(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.PageController
     * @function
     * 
     * @description
     * The controller for the settings management page
     */
    controllers.SettingsPageController = function ($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloSettingsService) {

        ////////////////////////////////////////////////
            $scope.loaded = true;
            $scope.preValuesLoaded = true;            
            $scope.savingStoreSettings = false;
            $scope.settingsDisplay = new merchello.Models.StoreSettings();
            $scope.currencies = [];
            $scope.selectedCurrency = {};
        ////////////////////////////////////////////////

        $scope.loadCurrency = function() {
            var promise = merchelloSettingsService.getAllCurrencies();

            promise.then(function(currencyFromServer) {
                _.each(currencyFromServer, function(element, index, list) {
                    var newCurrency = new merchello.Models.Currency(element);
                    $scope.currencies.push(newCurrency);
                });

                $scope.loadSettings();

            }, function (reason) {

                alert('Failed: ' + reason.message);

            });
        };

        $scope.loadSettings = function () {

            var promise = merchelloSettingsService.getCurrentSettings();

            promise.then(function (settingsFromServer) {

                $scope.settingsDisplay = new merchello.Models.StoreSettings(settingsFromServer);

                $scope.selectedCurrency = _.find($scope.currencies, function (element) {
                    if (_.isEmpty($scope.settingsDisplay.currencyCode)) {
                        return element.currencyCode == "USD";
                    } 
                    else {
                        return element.currencyCode == $scope.settingsDisplay.currencyCode;
                    }
                    
                });
                
                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {

                alert('Failed: ' + reason.message);

            });
        };

        $scope.loadCurrency();

        $scope.save = function(thisForm) {

            if (thisForm.$valid) {

                notificationsService.info("Saving...", "");

                $scope.savingStoreSettings = true;
                var promise = merchelloSettingsService.save($scope.settingsDisplay);

                promise.then(function(settingDisplay) {
                    notificationsService.success("Store Settings Saved", "");
                    $scope.savingStoreSettings = false;
                    $scope.settingDisplay = new merchello.Models.StoreSettings(settingDisplay);

                }, function(reason) {
                    notificationsService.error("Store Settings Save Failed", reason.message);
                });
            } else {
                notificationsService.info("Store Settings Save Failed...The Form Was Not Valid", "");
            }
        };

        $scope.currencyChanged = function(currency) {
            $scope.settingsDisplay.currencyCode = currency.currencyCode;
        };
    };

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.PageController", ['$scope', '$routeParams', '$location', 'notificationsService', 'angularHelper', 'serverValidationManager', 'merchelloSettingsService', merchello.Controllers.SettingsPageController]);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
