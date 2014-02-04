(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Dashboards.Settings.PageController
     * @function
     * 
     * @description
     * The controller for the reports list page
     */
    controllers.SettingsPageController = function ($scope, $routeParams, $location, notificationsService, angularHelper, serverValidationManager, merchelloSettingsService) {

        ////////////////////////////////////////////////
            $scope.loaded = true;
            $scope.preValuesLoaded = true;            
            $(".content-column-body").css('background-image', 'none');                
            $scope.savingStoreSettings = false;
            $scope.settingsDisplay = new merchello.Models.StoreSettings();
        ////////////////////////////////////////////////


        $scope.loadSettings = function () {

            var promise = merchelloSettingsService.getAllSettings();

            promise.then(function (settingsFromServer) {

                $scope.settingsDisplay = new merchello.Models.StoreSettings(settingsFromServer);

                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                $(".content-column-body").css('background-image', 'none');

            }, function (reason) {

                alert('Failed: ' + reason.message);

            });
        };


        $scope.loadSettings();

        $scope.save = function (thisForm) {

            if (thisForm.$valid) {

                notificationsService.info("Saving...", "");

                $scope.savingStoreSettings = true;
                var promise = merchelloSettingsService.save($scope.settingsDisplay);

                promise.then(function (settingDisplay) {
                    notificationsService.success("Store Settings Saved", "H5YR!");
                    $scope.savingStoreSettings = false;
                    $scope.settingDisplay = new merchello.Models.StoreSettings(settingDisplay);

                }, function (reason) {
                    notificationsService.error("Store Settings Save Failed", reason.message);
                });
            }
            else {     
                notificationsService.info("Store Settings Save Failed...The Form Was Not Valid", "");
            }
            }
        };

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.PageController", merchello.Controllers.SettingsPageController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
