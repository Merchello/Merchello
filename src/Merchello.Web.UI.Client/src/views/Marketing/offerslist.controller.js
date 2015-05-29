/**
 * @ngdoc controller
 * @name Merchello.Backoffice.OffersListController
 * @function
 *
 * @description
 * The controller for offers list view controller
 */
angular.module('merchello').controller('Merchello.Backoffice.OffersListController',
    ['$scope', '$location', 'assetsService', 'dialogService', 'notificationsService', 'settingsResource', 'marketingResource', 'merchelloTabsFactory', 'dialogDataFactory',
        'settingDisplayBuilder', 'offerProviderDisplayBuilder', 'offerSettingsDisplayBuilder', 'queryDisplayBuilder', 'queryResultDisplayBuilder',
    function($scope, $location, assetsService, dialogService, notificationsService, settingsResource, marketingResource, merchelloTabsFactory, dialogDataFactory,
             settingDisplayBuilder, offerProviderDisplayBuilder, offerSettingsDisplayBuilder, queryDisplayBuilder, queryResultDisplayBuilder) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;
        $scope.filterText = '';
        $scope.tabs = [];
        $scope.offers = [];
        $scope.currentFilters = [];
        $scope.sortProperty = 'name';
        $scope.sortOrder = 'Ascending';
        $scope.limitAmount = 25;
        $scope.currentPage = 0;
        $scope.maxPages = 0;
        $scope.settings = {};
        $scope.offerProviders = [];
        $scope.includeInactive = false;

        // exposed methods
        $scope.getEditUrl = getEditUrl;
        $scope.limitChanged = limitChanged;
        $scope.numberOfPages = numberOfPages;
        $scope.changePage = changePage;
        $scope.providerSelectDialogOpen = providerSelectDialogOpen;
        $scope.getOfferType = getOfferType;


        function init() {
            $scope.tabs = merchelloTabsFactory.createMarketingTabs();
            $scope.tabs.setActive('offers');
            loadSettings();
        }

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         *
         * @description
         * Loads in store settings from server into the scope.  Called in init().
         */
        function loadSettings() {
            var promiseSettings = settingsResource.getAllSettings();
            promiseSettings.then(function(settings) {
                $scope.settings = settingDisplayBuilder.transform(settings);
                loadOfferProviders();
            }, function (reason) {
                notificationsService.error("Settings Load Failed", reason.message);
            });
        }

        function loadOfferProviders() {
            var providersPromise = marketingResource.getOfferProviders();
            providersPromise.then(function(providers) {
                $scope.offerProviders = offerProviderDisplayBuilder.transform(providers);
                resetFilters();
            }, function(reason) {
                notificationsService.error("Offer providers load failed", reason.message);
            });
        }

        function loadOffers() {
           // var query = buildQuery();
            var offersPromise = marketingResource.getAllOfferSettings();
            offersPromise.then(function(result) {
                $scope.offers = offerSettingsDisplayBuilder.transform(result);
                $scope.preValuesLoaded = true;
            });
        }

        function resetFilters() {
            $scope.filterText = '';
            $scope.currentPage = 0;
            loadOffers();
        }


        function buildQuery(filterText) {
            var page = $scope.currentPage;
            var perPage = $scope.limitAmount;
            var sortBy = sortInfo().sortBy;
            var sortDirection = sortInfo().sortDirection;


            if (filterText === undefined) {
                filterText = '';
            }
            $scope.filterText = filterText;
            var query = queryDisplayBuilder.createDefault();
            query.currentPage = page;
            query.itemsPerPage = perPage;
            query.sortBy = sortBy;
            query.sortDirection = sortDirection;
            query.addFilterTermParam(filterText);

            if (query.parameters.length > 0) {
                $scope.currentFilters = query.parameters;
            }
            return query;
        }

        /**
         * @ngdoc method
         * @name setVariables
         * @function
         *
         * @description
         * Returns sort information based off the current $scope.sortProperty.
         */
        function sortInfo() {
            var sortDirection, sortBy;
            // If the sortProperty starts with '-', it's representing a descending value.
            if ($scope.sortProperty.indexOf('-') > -1) {
                // Get the text after the '-' for sortBy
                sortBy = $scope.sortProperty.split('-')[1];
                sortDirection = 'Descending';
                // Otherwise it is ascending.
            } else {
                sortBy = $scope.sortProperty;
                sortDirection = 'Ascending';
            }
            return {
                sortBy: sortBy.toLowerCase(), // We'll want the sortBy all lower case for API purposes.
                sortDirection: sortDirection
            }
        };


        //--------------------------------------------------------------------------------------
        // Events methods
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name limitChanged
         * @function
         *
         * @description
         * Helper function to set the amount of items to show per page for the paging filters and calculations
         */
        function limitChanged(newVal) {
            $scope.limitAmount = newVal;
            $scope.currentPage = 0;
            loadOffers();
        }

        /**
         * @ngdoc method
         * @name changePage
         * @function
         *
         * @description
         * Helper function re-search the products after the page has changed
         */
        function changePage(newPage) {
            $scope.currentPage = newPage;
            loadOffers();
        }

        /**
         * @ngdoc method
         * @name getFilteredProducts
         * @function
         *
         * @description
         * Calls the offer settings service to search for offers via a string search
         * param.
         */
        function getFilteredOffers(filter) {
            $scope.preValuesLoaded = false;
            $scope.filterText = filter;
            $scope.currentPage = 0;
            loadOffers();
        }

        //--------------------------------------------------------------------------------------
        // Dialogs
        //--------------------------------------------------------------------------------------
        /**
         * @ngdoc method
         * @name providerSelectDialogOpen
         * @function
         *
         * @description
         * Opens the dialog to allow user to select an offer provider to use to create an offer
         */
        function providerSelectDialogOpen() {
            var dialogData = dialogDataFactory.createSelectOfferProviderDialogData();
            dialogData.offerProviders = $scope.offerProviders;
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.offerproviderselection.html',
                show: true,
                callback: providerSelectDialogConfirm,
                dialogData: dialogData
            });
        }

        /**
         * @ngdoc method
         * @name providerSelectDialogConfirm
         * @param {dialogData} model returned from the dialog view
         * @function
         *
         * @description
         * Handles the data passed back from the provider editor dialog and redirects to the appropriate editor
         */
        function providerSelectDialogConfirm(dialogData) {
            var view = dialogData.selectedProvider.backOfficeTree.routePath.replace('{0}', 'create');
            $location.url(view, true);
        }

        //--------------------------------------------------------------------------------------
        // Calculations
        //--------------------------------------------------------------------------------------

        /**
         * @ngdoc method
         * @name numberOfPages
         * @function
         *
         * @description
         * Helper function to get the amount of items to show per page for the paging
         */
        function numberOfPages() {
            return $scope.maxPages;
            //return Math.ceil($scope.products.length / $scope.limitAmount);
        }

        /**
         * @ngdoc method
         * @name resetFilters
         * @function
         *
         * @description
         * Fired when the reset filter button is clicked.
         */
        function resetFilters() {
            $scope.preValuesLoaded = false;
            $scope.currentFilters = [];
            $scope.filterText = '';
            loadOffers();
        }

        function getEditUrl(offer) {
            var url = '#';
            var provider = _.find($scope.offerProviders, function(p) { return p.key === offer.offerProviderKey; });
            if (provider === null || provider === undefined) {
                return url;
            }
            return url + '/' + provider.editorUrl(offer.key);
        }

        function getOfferType(offer) {
            var provider = _.find($scope.offerProviders, function(p) { return p.key === offer.offerProviderKey; });
            if (provider === null || provider === undefined) {
                return 'could not find';
            }
            return provider.backOfficeTree.title;
        }

        // Initialize the controller
        init();
    }]);