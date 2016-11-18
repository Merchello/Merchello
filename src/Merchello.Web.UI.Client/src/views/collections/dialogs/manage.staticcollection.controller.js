
angular.module('merchello').controller('Merchello.EntityCollections.Dialogs.ManageStaticCollectionController',
    ['$scope',  'treeService', 'notificationsService', 'navigationService', 'assetsService', 'eventsService', 'entityCollectionHelper', 'merchelloTabsFactory',
        'settingsResource', 'entityCollectionResource', 'settingDisplayBuilder', 'productDisplayBuilder', 'invoiceDisplayBuilder', 'customerDisplayBuilder',
        'queryDisplayBuilder', 'queryResultDisplayBuilder', 'entityCollectionDisplayBuilder',
    function($scope, treeService, notificationsService, navigationService, assetsService, eventsService, entityCollectionHelper, merchelloTabsFactory,
        settingsResource, entityCollectionResource, settingDisplayBuilder, productDisplayBuilder, invoiceDisplayBuilder, customerDisplayBuilder,
        queryDisplayBuilder, queryResultDisplayBuilder, entityCollectionDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.addToCollection = true;
        $scope.wasFormSubmitted = false;
        $scope.collectionKey = '';
        $scope.entityType = '';
        $scope.dialogData = {};
        $scope.settings = {};
        $scope.entityCount = 0;
        $scope.collection = {};
        $scope.editCollection = false;

        $scope.sortProperty = '';
        $scope.sortOrder = 'Ascending';
        $scope.filterText = '';
        $scope.limitAmount = 5;
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
        $scope.toggleMode = toggleMode;
        $scope.handleEntity = handleEntity;
        $scope.saveCollection = saveCollection;

        var collectionChanged = "merchello.collection.changed";

        function init() {
            var cssPromise = assetsService.loadCss('/App_Plugins/Merchello/assets/css/merchello.css');
            cssPromise.then(function() {
                $scope.dialogData = $scope.$parent.currentAction.metaData.dialogData;
                $scope.collectionKey = $scope.dialogData.collectionKey;
                $scope.entityType = entityCollectionHelper.getEntityTypeByTreeId($scope.dialogData.entityType);
                loadSettings();
            });
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
            // this is needed for the date format
            var settingsPromise = settingsResource.getAllSettings();
            settingsPromise.then(function(allSettings) {
                $scope.settings = settingDisplayBuilder.transform(allSettings);
                var currencySymbolPromise = settingsResource.getCurrencySymbol();
                currencySymbolPromise.then(function (currencySymbol) {
                    $scope.currencySymbol = currencySymbol;
                    loadCollection();
                }, function (reason) {
                    notificationsService.error("Settings Load Failed", reason.message);
                });
            }, function(reason) {
                notificationService.error('Failed to load all settings', reason.message);
            });
        }

        function loadCollection() {
            var promise = entityCollectionResource.getByKey($scope.collectionKey);
            promise.then(function(collection) {
                $scope.collection = entityCollectionDisplayBuilder.transform(collection);
                loadEntities();
            }, function(reason) {
                notificationsService.error('Failed to load the collection ' + reason);
            });
        }

        function loadEntities() {
            $scope.preValuesLoaded = false;
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

            if ($scope.addToCollection) {
                var promise = entityCollectionResource.getEntitiesNotInCollection(query);
            } else {
                var promise = entityCollectionResource.getCollectionEntities(query);
            }
            promise.then(function(results) {
                var queryResult;
                switch($scope.entityType) {
                    case 'Invoice' :
                        queryResult = queryResultDisplayBuilder.transform(results, invoiceDisplayBuilder);
                        $scope.invoices = queryResult.items;
                        break;
                    case 'Customer' :
                        queryResult = queryResultDisplayBuilder.transform(results, customerDisplayBuilder);
                        $scope.customers = queryResult.items;
                        break;
                    default :
                        queryResult = queryResultDisplayBuilder.transform(results, productDisplayBuilder);
                        $scope.products = queryResult.items;
                        break
                };
                $scope.maxPages = queryResult.totalPages;
                $scope.entityCount = queryResult.totalItems;
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            }, function(reason) {
                notificationsService.error('Failed to load entities ' + reason);
            });
        }

        function handleEntity(entity) {
            var promise;
            if ($scope.addToCollection) {
                promise = entityCollectionResource.addEntityToCollection(entity.key, $scope.collectionKey);
            } else {
                promise = entityCollectionResource.removeEntityFromCollection(entity.key, $scope.collectionKey);
            }

            promise.then(function() {
                eventsService.emit(collectionChanged);
              loadEntities();
            }, function(reason) {
                notificationsService.error('Failed to add entity to collection ' + reason);
            });
        }

        function saveCollection() {
            $scope.wasFormSubmitted = true;
            if ($scope.entitycollectionForm.name.$valid) {
                var promise = entityCollectionResource.saveEntityCollection($scope.collection);
                promise.then(function(result) {
                    $scope.collection = entityCollectionDisplayBuilder.transform(result);
                    $scope.editCollection = false;
                    treeService.reloadNode($scope.currentNode);
                }, function(reason) {
                  notificationsService.error('Failed to save entity collection');
                });
            }
        }

        function toggleMode() {
            $scope.currentPage = 0;
            loadEntities();
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
