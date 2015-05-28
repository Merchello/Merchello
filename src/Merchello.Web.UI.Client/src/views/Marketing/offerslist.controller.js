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
        'settingDisplayBuilder', 'offerProviderDisplayBuilder',
    function($scope, $location, assetsService, dialogService, notificationsService, settingsResource, marketingResource, merchelloTabsFactory, dialogDataFactory,
             settingDisplayBuilder, offerProviderDisplayBuilder) {

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

        // exposed methods
        $scope.getEditUrl = getEditUrl;
        $scope.limitChanged = limitChanged;
        $scope.numberOfPages = numberOfPages;
        $scope.changePage = changePage;
        $scope.providerSelectDialogOpen = providerSelectDialogOpen;


        function init() {
            $scope.tabs = merchelloTabsFactory.createMarketingTabs();
            $scope.tabs.setActive('offers');
            loadSettings();
            loadOfferProviders();
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
            }, function (reason) {
                notificationsService.error("Settings Load Failed", reason.message);
            });
        }

        function loadOfferProviders() {
            var providersPromise = marketingResource.getOfferProviders();
            providersPromise.then(function(providers) {
                $scope.offerProviders = offerProviderDisplayBuilder.transform(providers);
                console.info($scope.offerProviders);
            }, function(reason) {
                notificationsService.error("Offer providers load failed", reason.message);
            });
        }

        function loadOffers() {

        }

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
            return '#';
        }

        // Initialize the controller
        init();
    }]);