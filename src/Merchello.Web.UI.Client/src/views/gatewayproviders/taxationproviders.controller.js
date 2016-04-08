angular.module('merchello').controller('Merchello.Backoffice.TaxationProvidersController',
    ['$scope', '$timeout', 'settingsResource', 'notificationsService', 'dialogService', 'taxationGatewayProviderResource', 'dialogDataFactory', 'merchelloTabsFactory',
        'settingDisplayBuilder', 'countryDisplayBuilder', 'taxCountryDisplayBuilder',
        'gatewayResourceDisplayBuilder', 'taxationGatewayProviderDisplayBuilder', 'taxMethodDisplayBuilder',
    function($scope, $timeout, settingsResource, notificationsService, dialogService, taxationGatewayProviderResource, dialogDataFactory, merchelloTabsFactory,
             settingDisplayBuilder, countryDisplayBuilder, taxCountryDisplayBuilder,
             gatewayResourceDisplayBuilder, taxationGatewayProviderDisplayBuilder, taxMethodDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.tabs = [];
        $scope.allCountries = [];
        $scope.availableCountries = [];
        $scope.taxationGatewayProviders = [];
        $scope.settings = {};

        // exposed methods
        $scope.ensureSingleProductTaxMethod = ensureSingleProductTaxMethod;
        $scope.save = save;
        $scope.editTaxMethodProvinces = editTaxMethodProvinces;

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name init
         * @function
         *
         * @description
         * Method called on intial page load.  Loads in data from server and sets up scope.
         */
        function init() {
            $scope.availableCountries = [];
            $scope.taxationGatewayProviders = [];
            loadSettings();
            $scope.tabs = merchelloTabsFactory.createGatewayProviderTabs();
            $scope.tabs.setActive('taxation');
            _.sortBy($scope.availableCountries, function(country) {
                return country.name;
            });
        }

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         *
         * @description - Load the Merchello settings.
         */
        function loadSettings() {
            var settingsPromise = settingsResource.getAllSettings();
            settingsPromise.then(function(settings) {
                $scope.settings = settings;
                loadAllAvailableCountries();
            }, function(reason) {
                notificationsService.error('Failed to load global settings', reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name loadAllAvailableCountries
         * @function
         *
         * @description
         * Method called on initial page load.  Loads in data from server and sets up scope.
         */
        function loadAllAvailableCountries() {
            var promiseAllCountries = settingsResource.getAllCountries();
            promiseAllCountries.then(function (allCountries) {
                $scope.allCountries = countryDisplayBuilder.transform(allCountries);
                loadAllTaxationGatewayProviders();
            }, function (reason) {
                notificationsService.error("Available Countries Load Failed", reason.message);
            });
        }

        /**
         * @ngdoc method
         * @name loadAllTaxationGatewayProviders
         * @function
         *
         * @description
         * Loads all taxation gateway providers.
         */
        function loadAllTaxationGatewayProviders() {

            var promiseAllProviders = taxationGatewayProviderResource.getAllGatewayProviders();
            promiseAllProviders.then(function (allProviders) {
                $scope.taxationGatewayProviders = taxationGatewayProviderDisplayBuilder.transform(allProviders);

                var noTaxProvider = taxationGatewayProviderDisplayBuilder.createDefault();
                noTaxProvider.key = -1;
                noTaxProvider.name = 'Not Taxed';

                if($scope.taxationGatewayProviders.length > 0) {
                    for(var i = 0; i < $scope.taxationGatewayProviders.length; i++) {
                        loadAvailableCountriesWithoutMethod($scope.taxationGatewayProviders[i], noTaxProvider);
                    }
                }
                $scope.taxationGatewayProviders.unshift(noTaxProvider);

            }, function (reason) {
                notificationsService.error("Available Taxation Providers Load Failed", reason.message);
            });
        }

        function loadAvailableCountriesWithoutMethod(taxationGatewayProvider, noTaxProvider) {
            var promiseGwResources = taxationGatewayProviderResource.getGatewayResources(taxationGatewayProvider.key);
            promiseGwResources.then(function (resources) {
                var newAvailableCountries = _.map(resources, function (resourceFromServer) {
                    var taxCountry = taxCountryDisplayBuilder.transform(resourceFromServer);
                    taxCountry.country = _.find($scope.allCountries, function (c) { return c.countryCode == taxCountry.gatewayResource.serviceCode; });
                    if (taxCountry.country) {
                        taxCountry.setCountryName(taxCountry.country.name);
                    } else {
                        if (taxCountry.gatewayResource.serviceCodeStartsWith('ELSE')) {
                            taxCountry.country = countryDisplayBuilder.createDefault();
                            taxCountry.setCountryName('Everywhere Else');
                            taxCountry.country.countryCode = 'ELSE';
                            taxCountry.sortOrder = 9999;
                        } else {
                            taxCountry.setCountryName(taxCountry.name);
                        }
                    }
                    return taxCountry;
                });
                angular.forEach(newAvailableCountries, function(country) {
                    country.taxMethod = taxMethodDisplayBuilder.createDefault();
                    country.provider = noTaxProvider;
                    country.taxMethod.providerKey = '-1';
                    $scope.availableCountries.push(country);
                });
                loadTaxMethods(taxationGatewayProvider);
                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {
                notificationsService.error("Available Gateway Resources Load Failed", reason.message);
            });
        }

        function loadTaxMethods(taxationGatewayProvider) {

            var promiseGwResources = taxationGatewayProviderResource.getTaxationProviderTaxMethods(taxationGatewayProvider.key);
            promiseGwResources.then(function (methods) {

                var newCountries = _.map(methods, function(methodFromServer) {
                    var taxMethod = taxMethodDisplayBuilder.transform(methodFromServer);
                    var taxCountry = taxCountryDisplayBuilder.createDefault();
                    taxCountry.addTaxesToProduct = taxMethod.productTaxMethod;
                    taxCountry.provider = taxationGatewayProvider;
                    taxCountry.taxMethod = taxMethod;
                    taxCountry.taxMethod.providerKey = taxationGatewayProvider.key;
                    if(taxCountry.taxMethod.countryCode === 'ELSE') {
                        taxCountry.country = countryDisplayBuilder.createDefault();
                        taxCountry.country.name = 'Everywhere Else';
                        taxCountry.country.countryCode = 'ELSE';
                    } else {
                        taxCountry.country = _.find($scope.allCountries, function(c) { return c.countryCode == taxMethod.countryCode; });
                    }
                    if (taxCountry.country) {
                        taxCountry.setCountryName(taxCountry.country.name);
                    } else {
                        taxCountry.setCountryName(taxMethod.name);
                    }
                    return taxCountry;
                });

                _.each(newCountries, function(country) {
                    if (country.country.countryCode === 'ELSE') {
                        country.sortOrder = 9999;
                    }
                    $scope.availableCountries.push(country);
                });


                $scope.loaded = true;
                $scope.preValuesLoaded = true;

            }, function (reason) {
                notificationsService.error("Available Gateway Resources Load Failed", reason.message);
            });

        }


        //--------------------------------------------------------------------------------------
        // Events methods
        //--------------------------------------------------------------------------------------
        function ensureSingleProductTaxMethod(taxCountry) {
            angular.forEach($scope.availableCountries, function(tc) {
                if (tc.country.countryCode !== taxCountry.country.countryCode) {
                    tc.addTaxesToProduct = false;
                }
            });
        }

        function save() {
            $scope.preValuesLoaded = false;
            angular.forEach($scope.availableCountries, function(country) {
                if(country.provider.key === -1) {
                    // delete the provider
                    if(country.taxMethod.key !== '') {
                        var promiseDelete = taxationGatewayProviderResource.deleteTaxMethod(country.taxMethod.key);
                        promiseDelete.then(function() {
                            notificationsService.success("TaxMethod Removed", "");
                        }, function(reason) {
                            notificationsService.error("TaxMethod Save Failed", reason.message);
                        });
                    }
                } else {
                    if(country.taxMethod.providerKey !== country.provider.key) {
                            country.taxMethod.providerKey = country.provider.key;
                            country.taxMethod.countryCode = country.country.countryCode;
                            country.taxMethod.productTaxMethod = country.addTaxesToProduct;
                            var promiseSave = taxationGatewayProviderResource.addTaxMethod(country.taxMethod);
                            promiseSave.then(function() {
                                notificationsService.success("TaxMethod Saved", "");
                            }, function(reason) {
                                notificationsService.error("TaxMethod Save Failed", reason.message);
                        });
                    } else {
                        country.taxMethod.productTaxMethod = country.addTaxesToProduct;
                        var promiseSave = taxationGatewayProviderResource.saveTaxMethod(country.taxMethod);
                        promiseSave.then(function() {
                            notificationsService.success("TaxMethod Saved", "");
                        }, function(reason) {
                            notificationsService.error("TaxMethod Save Failed", reason.message);
                        });
                    }
                }
            });
            $timeout(function() {
                init();
            }, 400);
        }

        //--------------------------------------------------------------------------------------
        // Dialogs
        //--------------------------------------------------------------------------------------

        function taxMethodDialogConfirm(dialogData) {
            var promiseSave;

            // Save existing method
            promiseSave = taxationGatewayProviderResource.saveTaxMethod(dialogData.country.taxMethod);
            promiseSave.then(function () {
                notificationsService.success("Taxation Method Saved");
            }, function (reason) {
                notificationsService.error("Taxation Method Save Failed", reason.message);
            });
        }

        function editTaxMethodProvinces(country) {
            if (country) {
                var dialogData = dialogDataFactory.createEditTaxCountryDialogData();
                dialogData.country = country;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/dialogs/taxation.edittaxmethod.html',
                    show: true,
                    callback: taxMethodDialogConfirm,
                    dialogData: dialogData
                });
            }
        }

        // Initialize the controller
        init();
}]);
