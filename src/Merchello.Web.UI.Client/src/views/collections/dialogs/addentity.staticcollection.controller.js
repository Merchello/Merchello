
angular.module('merchello').controller('Merchello.EntityCollections.Dialogs.AddEntityStaticCollectionController',
    ['$scope',  'treeService', 'notificationsService', 'navigationService', 'entityCollectionHelper',
        'settingsResource', 'entityCollectionResource', 'productDisplayBuilder', 'invoiceDisplayBuilder', 'customerDisplayBuilder',
        'queryDisplayBuilder', 'queryResultDisplayBuilder',
    function($scope, treeService, notificationsService, navigationService, entityCollectionHelper,
        settingsResource, entityCollectionResource, productDisplayBuilder, invoiceDisplayBuilder, customerDisplayBuilder,
        queryDisplayBuilder, queryResultDisplayBuilder) {

        $scope.loaded = false;
        $scope.wasFormSubmitted = false;
        $scope.collectionKey = '';
        $scope.entityType = '';
        $scope.dialogData = {};

        $scope.sortProperty = '';
        $scope.sortOrder = 'Ascending';
        $scope.filterText = '';
        $scope.limitAmount = 10;
        $scope.currentPage = 0;
        $scope.maxPages = 0;
        $scope.invoices = [];
        $scope.products = [];
        $scope.customers = [];

        // exposed methods
        $scope.changePage = changePage;
        $scope.limitChanged = limitChanged;
        $scope.changeSortOrder = changeSortOrder;
        $scope.getFilteredEntities = getFilteredEntities;
        $scope.numberOfPages = numberOfPages;

        function init() {
            $scope.dialogData = $scope.$parent.currentAction.metaData.dialogData;
            $scope.collectionKey = $scope.dialogData.collectionKey;
            $scope.entityType = entityCollectionHelper.getEntityTypeByTreeId($scope.dialogData.entityType);
            loadSettings();
        }

        /**
         * @ngdoc method
         * @name loadSettings
         * @function
         *
         * @description
         * Load the settings from the settings service to get the currency symbol
         */
        function loadSettings() {
            var currencySymbolPromise = settingsResource.getCurrencySymbol();
            currencySymbolPromise.then(function (currencySymbol) {
                $scope.currencySymbol = currencySymbol;
                loadEntities();
            }, function (reason) {
                notificationsService.error("Settings Load Failed", reason.message);
            });
        }

        function loadEntities() {

            var page = $scope.currentPage;
            var perPage = $scope.limitAmount;
            var sortBy = $scope.sortProperty.replace("-", "");
            var sortDirection = $scope.sortOrder;
            var query = queryDisplayBuilder.createDefault();
            query.currentPage = page;
            query.itemsPerPage = perPage;
            query.sortBy = sortBy;
            query.sortDirection = sortDirection;
            query.addFilterTermParam($scope.filterText);
            query.addCollectionKeyParam($scope.collectionKey);
            query.addEntityTypeParam($scope.entityType);

            var promise = entityCollectionResource.getEntitiesNotInCollection(query);
            promise.then(function(results) {
                console.info(queryResult);
                switch($scope.entityType) {
                    case 'Invoice' :
                        $scope.invoices = queryResultDisplayBuilder.transform(results, invoiceDisplayBuilder);
                        break;
                    case 'Customer' :
                        $scope.customers = queryResultDisplayBuilder.transform(results, customerDisplayBuilder);
                    default :
                        $scope.products = queryResultDisplayBuilder.transform(results, productDisplayBuilder);
                        break
                };
                $scope.maxPages = queryResult.totalPages;
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            }, function(reason) {
                notificationsService.error('Failed to load entities ' + reason);
            });
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
            loadEntities();
        }

        /**
         * @ngdoc method
         * @name changePage
         * @function
         *
         * @description
         * Helper function re-search the products after the page has changed
         */
        function changePage (newPage) {
            $scope.currentPage = newPage;
            loadEntities();
        }

        /**
         * @ngdoc method
         * @name changeSortOrder
         * @function
         *
         * @description
         * Helper function to set the current sort on the table and switch the
         * direction if the property is already the current sort column.
         */
        function changeSortOrder(propertyToSort) {

            if ($scope.sortProperty == propertyToSort) {
                if ($scope.sortOrder == "Ascending") {
                    $scope.sortProperty = "-" + propertyToSort;
                    $scope.sortOrder = "Descending";
                } else {
                    $scope.sortProperty = propertyToSort;
                    $scope.sortOrder = "Ascending";
                }
            } else {
                $scope.sortProperty = propertyToSort;
                $scope.sortOrder = "Ascending";
            }

            loadEntities();
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
        }


        /**
         * @ngdoc method
         * @name getFilteredEntities
         * @function
         *
         * @description
         * Calls the product service to search for products via a string search
         * param.  This searches the Examine index in the core.
         */
        function getFilteredEntities(filter) {
            $scope.filterText = filter;
            $scope.currentPage = 0;
            loadEntities();
        }


        // initialize the controller
        init();
}]);
