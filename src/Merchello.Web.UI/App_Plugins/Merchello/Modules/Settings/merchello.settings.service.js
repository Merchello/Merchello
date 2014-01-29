(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name umbraco.resources.MerchelloProductService
        * @description Loads in data for data types
        **/
    merchelloServices.MerchelloSettingsService = function ($q, $http, umbDataFormatter, umbRequestHelper, notificationsService, merchelloProductVariantService) {

        var settingsservice = {     

            /** saves or updates a product object */
            save: function (settingDisplay) {
                
                return umbRequestHelper.resourcePromise(
                   $http.post(
                        umbRequestHelper.getApiUrl('merchellSettingsApiBaseUrl', 'PutSetting'),
                        settingDisplay
                    ),
                    'Failed to save data for Store Settings');
            },

            getAllSettings: function () {

                return umbRequestHelper.resourcePromise(
                    $http.get(
                        umbRequestHelper.getApiUrl('merchellSettingsApiBaseUrl', 'GetAllSettings')
                    ),
                    'Failed to get all Settings');     
            },                      

        };

        return settingsservice;
    };

    angular.module('umbraco.resources').factory('merchelloSettingsService', merchello.Services.MerchelloSettingsService);

}(window.merchello.Services = window.merchello.Services || {}));
