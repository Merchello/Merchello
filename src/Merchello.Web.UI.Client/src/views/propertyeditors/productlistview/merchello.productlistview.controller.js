/**
 * @ngdoc controller
 * @name Merchello.PropertyEditors.MerchelloProductListView
 * @function
 *
 * @description
 * The controller for product collection list view property editor
 */
angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloProductListViewController',
    ['$scope', '$location', 'dialogService', 'userService', 'settingsResource', 'notificationsService', 'entityCollectionResource', 'productResource', 'productDisplayBuilder',
        'queryDisplayBuilder', 'queryResultDisplayBuilder',
    function($scope, $location, dialogService, userService, settingsResource, notificationsService, entityCollectionResource, productResource, productDisplayBuilder,
    queryDisplayBuilder, queryResultDisplayBuilder) {

        $scope.preValuesLoaded = false;
        $scope.currencySymbol = '';
        $scope.entityType = 'product';
        $scope.entityTypeName = 'Product';
        $scope.entityCollection = {};
        $scope.listViewResultSet = {
            totalItems: 0,
            items: []
        };

        $scope.options = {
            pageSize: 10,
            pageNumber: 1,
            filter: '',
            orderBy: ($scope.model.config.orderBy ? $scope.model.config.orderBy : 'name').trim(),
            orderDirection: $scope.model.config.orderDirection ? $scope.model.config.orderDirection.trim() : "asc"
        };

        $scope.pagination = [];

        $scope.openCollectionSelectionDialog = openCollectionSelectionDialog;
        $scope.getTreeId = getTreeId;
        $scope.sort = sort;
        $scope.isSortDirection = isSortDirection;
        $scope.next = next;
        $scope.prev = prev;
        $scope.goToPage = goToPage;
        $scope.enterSearch = enterSearch;
        $scope.loadProducts = loadProducts;
        $scope.goToEditor = goToEditor;

        var currentUser = null;
        var allowMerchello = false;
        const baseUrl = '/merchello/merchello/productedit/';
        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------
        // Load the product from the Guid key stored in the model.value
        if (_.isString($scope.model.value)) {
            userService.getCurrentUser().then(function(user) {
              currentUser = user;
                var fnd = _.find(currentUser.allowedSections, function(s) {
                  return s === 'merchello';
                });
                if (fnd !== undefined) {
                    allowMerchello = true;
                }
                loadSettings();
            });

        }

        function loadSettings() {
            var promise = settingsResource.getCurrencySymbol();
            promise.then(function(symbol) {
                $scope.currencySymbol = symbol;
                loadProducts();
            }, function (reason) {
                notificationsService.error('Could not retrieve currency symbol', reason.message);
            });
        }

        function loadProducts() {
            var page = $scope.options.pageNumber - 1;
            var perPage = $scope.options.pageSize;
            var sortBy = $scope.options.orderBy;
            var sortDirection = $scope.options.orderDirection === 'asc' ? 'Ascending' : 'Descending';

            var query = queryDisplayBuilder.createDefault();
            query.currentPage = page;
            query.itemsPerPage = perPage;
            query.sortBy = sortBy;
            query.sortDirection = sortDirection;
            query.addFilterTermParam($scope.options.filter);

            var promise;
            if ($scope.model.value !== '') {
                query.addCollectionKeyParam($scope.model.value);
                query.addEntityTypeParam('Product');
                var promise = entityCollectionResource.getCollectionEntities(query);
            } else {
                var promise = productResource.searchProducts(query);
            }

            promise.then(function (response) {
                var queryResult = queryResultDisplayBuilder.transform(response, productDisplayBuilder);
                $scope.listViewResultSet.items = queryResult.items;
                $scope.listViewResultSet.totalItems = queryResult.totalItems;
                $scope.listViewResultSet.totalPages = queryResult.totalPages;
               // $scope.products = queryResult.items;
               // $scope.maxPages = queryResult.totalPages;

                $scope.pagination = [];

                //list 10 pages as per normal
                if ($scope.listViewResultSet.totalPages <= 10) {
                    for (var i = 0; i < $scope.listViewResultSet.totalPages; i++) {
                        $scope.pagination.push({
                            val: (i + 1),
                            isActive: $scope.options.pageNumber == (i + 1)
                        });
                    }
                }
                else {
                    //if there is more than 10 pages, we need to do some fancy bits

                    //get the max index to start
                    var maxIndex = $scope.listViewResultSet.totalPages - 10;
                    //set the start, but it can't be below zero
                    var start = Math.max($scope.options.pageNumber - 5, 0);
                    //ensure that it's not too far either
                    start = Math.min(maxIndex, start);

                    for (var i = start; i < (10 + start) ; i++) {
                        $scope.pagination.push({
                            val: (i + 1),
                            isActive: $scope.options.pageNumber == (i + 1)
                        });
                    }

                    //now, if the start is greater than 0 then '1' will not be displayed, so do the elipses thing
                    if (start > 0) {
                        $scope.pagination.unshift({ name: "First", val: 1, isActive: false }, {val: "...",isActive: false});
                    }

                    //same for the end
                    if (start < maxIndex) {
                        $scope.pagination.push({ val: "...", isActive: false }, { name: "Last", val: $scope.listViewResultSet.totalPages, isActive: false });
                    }
                }



                $scope.preValuesLoaded = true;
            }, function(reason) {
                notificationsService.success("Products Load Failed:", reason.message);
            });
        }


        function sort(field, allow) {
            if (allow) {
                $scope.options.orderBy = field;

                if ($scope.options.orderDirection === "desc") {
                    $scope.options.orderDirection = "asc";
                }
                else {
                    $scope.options.orderDirection = "desc";
                }
                loadProducts();
            }
        };

        function next () {
            if ($scope.options.pageNumber < $scope.listViewResultSet.totalPages) {
                $scope.options.pageNumber++;
                loadProducts();
            }
        };

        function goToPage(pageNumber) {
            $scope.options.pageNumber = pageNumber + 1;
            loadProducts();
        }


        function prev() {
            if ($scope.options.pageNumber > 0) {
                $scope.options.pageNumber--;
                loadProducts();
            }
        }

        function enterSearch($event) {
            $($event.target).next().focus();
        }


        function isSortDirection(col, direction) {
            return $scope.options.orderBy.toUpperCase() == col.toUpperCase() && $scope.options.orderDirection == direction;
        }

        function goToEditor(product) {
            if(allowMerchello) {
                var url = baseUrl + product.key;
                $location.url(url, true);
            }
        }

        function getTreeId() {
            return "products";
        }

        function openCollectionSelectionDialog() {
            var dialogData = {};
            dialogData.collectionKey = '';
            dialogData.entityType =  $scope.entityType;

            dialogService.open({
                template: '/App_Plugins/Merchello/propertyeditors/productlistview/merchello.productlistview.dialog.html',
                show: true,
                callback: processCollectionSelection,
                dialogData: dialogData
            });
        }

        function processCollectionSelection(dialogData) {
            $scope.model.value = dialogData.collectionKey;
            loadProducts();
        }

    }]);