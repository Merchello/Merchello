    /**
     * @ngdoc resource
     * @name settingsResource
     * @description Loads in data and allows modification for invoices
     **/
    angular.module('merchello.resources').factory('settingsResource',
        ['$q', '$http', '$cacheFactory', 'umbRequestHelper', 'countryDisplayBuilder', 'settingDisplayBuilder',
            function($q, $http, $cacheFactory, umbRequestHelper, countryDisplayBuilder, settingDisplayBuilder) {

        /* cacheFactory instance for cached items in the merchelloSettingsService */
        var _settingsCache = $cacheFactory('merchelloSettings');

        /* helper method to get from cache or fall back to an http api call */
        function getCachedOrApi(cacheKey, apiMethod, entityName)
        {
            var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloSettingsApiBaseUrl'] + apiMethod;
            var deferred = $q.defer();

            var dataFromCache = _settingsCache.get(cacheKey);

            if (dataFromCache) {
                deferred.resolve(dataFromCache);
            }
            else {
                var promiseFromApi = umbRequestHelper.resourcePromise(
                    $http.get(
                        url
                    ),
                    'Failed to get all ' + entityName);

                promiseFromApi.then(function (dataFromApi) {
                    _settingsCache.put(cacheKey, dataFromApi);
                    deferred.resolve(dataFromApi);
                }, function (reason) {
                    deferred.reject(reason);
                });
            }

            return deferred.promise;
        }

        /**
         * @class merchelloSettingsService
         */
        var settingsServices = {

            recordDomain: function(record) {
                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloSettingsApiBaseUrl'] + 'RecordDomain';
                    return umbRequestHelper.resourcePromise(
                        $http.post(
                            url,
                            record
                        ),
                        'Failed to save data for domain record');
            },

            /**
             * @ngdoc method
             * @name getMerchelloVersion
             * @methodOf settingsResource
             * @function
             *
             * @description
             * Gets the current Merchello Version
             *
             * @returns {object} an angularjs promise object
             */
            getMerchelloVersion: function() {
                return getCachedOrApi("MerchelloVersion", "GetMerchelloVersion", "merchelloversion");
            },

            /**
             * @ngdoc method
             * @name merchello.services.merchelloSettingsService#getAllCountries
             * @methodOf merchello.services.merchelloSettingsService
             * @function
             *
             * @description
             * Gets the supported countries / provinces from the merchello.config
             *
             * @returns {object} an angularjs promise object
             */
            getAllCountries: function () {
                return getCachedOrApi("SettingsCountries", "GetAllCountries", "countries");
            },

            /**
             * @ngdoc method
             * @name merchello.services.merchelloSettingsService#save
             * @methodOf merchello.services.merchelloSettingsService
             * @function
             *
             * @description
             * Saves or updates a store setting value
             *
             * @param {object} storeSettings StoreSettings object for the key/value pairs
             *
             * @returns {object} an angularjs promise object
             */
            save: function (storeSettings) {

                _settingsCache.remove("AllSettings");

                var url = Umbraco.Sys.ServerVariables['merchelloUrls']['merchelloSettingsApiBaseUrl'] + 'PutSettings';
                return umbRequestHelper.resourcePromise(
                    $http.post(
                        url,
                        storeSettings
                    ),
                    'Failed to save data for Store Settings');
            },

            /**
             * @ngdoc method
             * @name merchello.services.merchelloSettingsService#getAllSettings
             * @methodOf merchello.services.merchelloSettingsService
             * @function
             *
             * @description
             * Gets all store setting values
             *
             * @returns {object} an angularjs promise object
             */
            getAllSettings: function () {
                return getCachedOrApi("AllSettings", "GetAllSettings", "settings");
            },

            getAllCombined: function() {
                var deferred = $q.defer();
                var promises = [
                    this.getAllSettings(),
                    this.getAllCurrencies(),
                    this.getAllCountries()
                ];
                $q.all(promises).then(function(data) {
                    var result = {
                        settings: settingDisplayBuilder.transform(data[0]),
                        currencies: data[1],
                        currencySymbol: _.find(data[1], function(c) {
                            return c.currencyCode === data[0].currencyCode;
                        }).symbol,
                        countries: countryDisplayBuilder.transform(data[2])
                    };
                    deferred.resolve(result);
                });

                return deferred.promise;
            },

            getCurrentSettings: function() {
                var deferred = $q.defer();

                var promiseArray = [];

                promiseArray.push(this.getAllSettings());

                var promise = $q.all(promiseArray);
                promise.then(function (data) {
                    deferred.resolve(data[0]);
                }, function(reason) {
                    deferred.reject(reason);
                });

                return deferred.promise;
            },

            /**
             * @ngdoc method
             * @name merchello.services.merchelloSettingsService#getAllCurrencies
             * @methodOf merchello.services.merchelloSettingsService
             * @function
             *
             * @description
             * Gets all the currencies
             *
             * @returns {object} an angularjs promise object
             */
            getAllCurrencies: function () {
                return getCachedOrApi("AllCurrency", "GetAllCurrencies", "settings");
            },

            getCurrencySymbol: function () {
                var deferred = $q.defer();

                var promiseArray = [];

                promiseArray.push(this.getAllSettings());
                promiseArray.push(this.getAllCurrencies());

                var promise = $q.all(promiseArray);
                promise.then(function (data) {
                    var settingsFromServer = data[0];
                    var currencyList =  data[1];
                    var selectedCurrency = _.find(currencyList, function (currency) {
                        return currency.currencyCode === settingsFromServer.currencyCode;
                    });

                    deferred.resolve(selectedCurrency.symbol);
                }, function (reason) {
                    deferred.reject(reason);
                });

                return deferred.promise;
            },

            /**
             * @ngdoc method
             * @name merchello.services.merchelloSettingsService#getTypeFields
             * @methodOf merchello.services.merchelloSettingsService
             * @function
             *
             * @description
             * Gets all the type fields
             *
             * @returns {object} an angularjs promise object
             */
            getTypeFields: function () {
                return getCachedOrApi("AllTypeFields", "GetTypeFields", "settings");
            },

            getReportBackofficeTrees: function() {
                return getCachedOrApi("pluginReports", "GetReportBackofficeTrees", "BackofficeTree");
            }

        };

        return settingsServices;

    }]);
