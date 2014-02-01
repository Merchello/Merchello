(function (merchelloServices, undefined) {


    /**
        * @ngdoc service
        * @name umbraco.resources.MerchelloProductService
        * @description Loads in data for data types
        **/
    merchelloServices.MerchelloSettingsService = function ($q, $http, umbDataFormatter, umbRequestHelper, notificationsService) {

        //var cache = $cacheFactory('merchelloSettings');

        var settingsservice = {

            getAllCountries: function () {

                //var deferred = $q.defer();

                //var allCountries = cache.get("SettingsCountries");

                //if (!allCountries) {
                    return umbRequestHelper.resourcePromise(
                       $http.get(
                            umbRequestHelper.getApiUrl('merchelloSettingsApiBaseUrl', 'GetAllCountries')
                        ),
                        'Failed to get all countries');
                //}
                //else {
                //    deferred.resolve(allCountries);
                //}

                //return deferred.promise;
            },

            /** saves or updates a setting value */
            save: function (settingDisplay) {
                
                return umbRequestHelper.resourcePromise(
                   $http.post(
                        umbRequestHelper.getApiUrl('merchelloSettingsApiBaseUrl', 'PutSettings'),
                        settingDisplay
                    ),
                    'Failed to save data for Store Settings');
            },

            getAllSettings: function () {

                return umbRequestHelper.resourcePromise(

                    $http.get(
                        umbRequestHelper.getApiUrl('merchelloSettingsApiBaseUrl', 'GetAllSettings')
                    ),
                    'Failed to get all Settings');     
            },

        };

        return settingsservice;
    };

    angular.module('umbraco.resources').factory('merchelloSettingsService', merchello.Services.MerchelloSettingsService);

}(window.merchello.Services = window.merchello.Services || {}));
