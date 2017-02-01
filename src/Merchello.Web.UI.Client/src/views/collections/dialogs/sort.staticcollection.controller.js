/**
 * @ngdoc controller
 * @name Merchello.Common.Dialogs.SortStaticCollectionController
 * @function
 *
 * @description
 * The controller for the delete confirmations
 */
angular.module('merchello')
    .controller('Merchello.EntityCollections.Dialogs.SortStaticCollectionController',
    ['$scope', 'appState', 'treeService', 'notificationsService', 'navigationService', 'entityCollectionHelper', 'entityCollectionResource', 'entityCollectionProviderDisplayBuilder', 'entityCollectionDisplayBuilder',
        function($scope, appState, treeService, notificationsService, navigationService, entityCollectionHelper, entityCollectionResource, entityCollectionProviderDisplayBuilder, entityCollectionDisplayBuilder) {

            $scope.loaded = false;
            $scope.name = '';
            $scope.entityType = '';
            $scope.dialogData = {};
            $scope.entityCollectionProviders = [];
            $scope.entityCollections = [];
            $scope.sortableProviderKeys = [];
            $scope.isRootNode = false;
            $scope.nodePath = [];

            // exposed methods
            $scope.save = save;

            function init() {
                $scope.dialogData = $scope.$parent.currentAction.metaData.dialogData;
                $scope.entityType = entityCollectionHelper.getEntityTypeByTreeId($scope.dialogData.entityType);
                loadValidSortableProviderKeys();
                $scope.nodePath = treeService.getPath($scope.currentNode);
            }

            function loadValidSortableProviderKeys() {
                var promise = entityCollectionResource.getSortableProviderKeys();
                promise.then(function(keys) {
                    $scope.sortableProviderKeys = keys;
                    if ($scope.dialogData.parentKey == undefined || $scope.dialogData.parentKey == '') {
                        loadRootLevelCollections();
                    } else {
                        loadParentCollection();
                    }
                });
            }

            function loadRootLevelCollections() {
                $scope.isRootNode = true;
                var parentPromise = entityCollectionResource.getRootCollectionsByEntityType($scope.entityType);
                parentPromise.then(function(collections) {
                    var transformed = [];
                    if (!angular.isArray(collections)) {
                        collections.sortOrder = 0;
                        transformed.push(entityCollectionDisplayBuilder.transform(collections));
                    } else {
                        transformed = entityCollectionDisplayBuilder.transform(collections);
                    }

                    // we need to weed out the non static providers if any
                    $scope.entityCollections = _.filter(transformed, function(c) {

                        return _.find($scope.sortableProviderKeys, function(k) {
                            return k === c.providerKey;
                        }) !== undefined;
                    });
                    $scope.loaded = true;
                });
            }

            function loadParentCollection() {
                var parentPromise = entityCollectionResource.getChildEntityCollections($scope.dialogData.parentKey);
                parentPromise.then(function(collections) {
                    if (!angular.isArray(collections)) {
                        $scope.entityCollections.push(entityCollectionDisplayBuilder.transform(collections));
                    } else {
                        $scope.entityCollections = entityCollectionDisplayBuilder.transform(collections);
                    }
                    $scope.loaded = true;
                });
            }

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description - Saves the newly sorted nodes and updates the tree UI.
             */
            function save() {

                // set the sorts here
                for(var i = 0; i < $scope.entityCollections.length; i++) {
                    $scope.entityCollections[i].sortOrder = i;
                }


                // save updated sort orders
                var promise = entityCollectionResource.updateSortOrders($scope.entityCollections);
                promise.then(function() {

                    // reload the children of the parent

                    var childPromise = treeService.loadNodeChildren({ node: $scope.currentNode });
                    childPromise.then(function(children) {
                        var reloadPromise = treeService.reloadNode($scope.currentNode);
                        reloadPromise.then(function() {
                            navigationService.hideNavigation();
                            notificationsService.success('Collections sorted success.');
                        });

                    }, function(reason) {
                        notificationsService.error('failed to load node children ' + reason)
                    });
                });
            }

            // Sortable available offers
            /// -------------------------------------------------------------------

            $scope.sortableCollections = {
                start : function(e, ui) {
                    ui.item.data('start', ui.item.index());
                },
                stop: function (e, ui) {
                    var start = ui.item.data('start'),
                        end =  ui.item.index();
                   // console.info(start + ' ' + end);
                    //$scope.offerSettings.reorderComponent(start, end);
                },
                disabled: false,
                cursor: "move"
            }

            init();
        }]);


