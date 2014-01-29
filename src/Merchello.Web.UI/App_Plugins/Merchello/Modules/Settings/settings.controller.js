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

            $scope.settingsDisplay = new merchello.Models.SettingDisplay();
        ////////////////////////////////////////////////


        $scope.loadSettings = function () {

            var promise = merchelloSettingsService.getAllSettings();

            promise.then(function (settingsFromServer) {

                $scope.settingsDisplay = new merchello.Models.SettingDisplay(settingsFromServer);

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
                
                    $scope.creatingStoreSettings = true;                    
                    var promise = merchelloSettingsService.save($scope.settingsDisplay);

                    promise.then(function (settingDisplay) {
                        notificationsService.success("Store Settings Saved", "H5YR!");

                        $scope.settingDisplay = new merchello.Models.SettingDisplay(settingDisplay);

                    }, function (reason) {
                        notificationsService.error("Store Settings Save Failed", reason.message);
                    });
                }
            }
        };

    angular.module("umbraco").controller("Merchello.Dashboards.Settings.PageController", merchello.Controllers.SettingsPageController);


}(window.merchello.Controllers = window.merchello.Controllers || {}));
