angular.module('merchello.mocks')
    .factory('settingResourceMock', [
        '$httpBackend', 'mocksUtils', 'settingMock',
        function($httpBackend, mocksUtils, settingMock) {

            function getAllCountries() {
                return settingMock.allCountries();
            }

            function getAllCurrencies() {
                return settingMock.allCurrencies();
            }

            function getAllSettings() {
                return settingMock.setting();
            }

            return {
                register: function() {

                    $httpBackend
                        .whenGET(mocksUtils.urlRegex('/umbraco/backoffice/Merchello/SettingApi/GetAllCountries'))
                        .respond(getAllCountries());

                    $httpBackend
                        .whenGET(mocksUtils.urlRegex('/umbraco/backoffice/Merchello/SettingApi/GetAllCurrencies'))
                        .respond(getAllCurrencies());

                    $httpBackend
                        .whenGET(mocksUtils.urlRegex('/umbraco/backoffice/Merchello/SettingApi/GetAllSettings'))
                        .respond(getAllSettings());

                    //$httpBackend
                    //    .whenGET('/umbraco/backoffice/Merchello/SettingApi/')
                }
            };

    }]);
