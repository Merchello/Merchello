angular.module('merchello.mocks')
    .factory('settingResourceMock', [
        '$httpBackend', 'settingMock',
        function($httpBackend, settingMock) {

            function getAllCountries() {
                return settingMock.allCountries();
            }

            function getAllCurrencies() {
                return settingMock.allCurrencies();
            }

            function getCurrentSettings() {
                return settingMock.setting();
            }

            function getCurrencySymbol() {
                return settingMock.usdCurrency();
            }

            return {
                register: function() {

                    $httpBackend
                        .whenGET('/umbraco/backoffice/Merchello/SettingApi/GetAllCountries')
                        .respond(getAllCountries());

                    $httpBackend
                        .whenGET('/umbraco/backoffice/Merchello/SettingApi/GetAllCurrencies')
                        .respond(getAllCurrencies());

                    $httpBackend
                        .whenGET('/umbraco/backoffice/Merchello/SettingApi/GetAllSettings')
                        .respond(getCurrentSettings());

                    //$httpBackend
                    //    .whenGET('/umbraco/backoffice/Merchello/SettingApi/')
                }
            };

    }]);
