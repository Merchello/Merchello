/**
 * @ngdoc controller
 * @name Merchello.Backoffice.OffersListController
 * @function
 *
 * @description
 * The controller for offers list view controller
 */
angular.module('merchello').controller('Merchello.Backoffice.OffersListController',
    ['$scope', '$location', 'assetsService', 'notificationsService', 'settingsResource', 'merchelloTabsFactory', 'dialogDataFactory',
    function($scope, $location, assetsService, notificationsService, settingsResource, merchelloTabsFactory, dialogDataFactory) {

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

        // exposed methods
        $scope.getEditUrl = getEditUrl;
        $scope.limitChanged = limitChanged;
        $scope.numberOfPages = numberOfPages;
        $scope.changePage = changePage;


        function init() {
            $scope.tabs = merchelloTabsFactory.createMarketingTabs();
            $scope.tabs.setActive('offers');
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