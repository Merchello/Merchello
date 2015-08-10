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
            $scope.wasFormSubmitted = false;
            $scope.entityType = '';
            $scope.dialogData = {};
            $scope.entityCollectionProviders = [];
            $scope.entityCollections = [];

            // exposed methods
            $scope.save = save;

            function init() {
                $scope.dialogData = $scope.$parent.currentAction.metaData.dialogData;
                $scope.entityType = entityCollectionHelper.getEntityTypeByTreeId($scope.dialogData.entityType);
                if ($scope.dialogData.parentKey == undefined || $scope.dialogData.parentKey == '') {
                    loadRootLevelCollections();
                } else {
                    loadParentCollection();
                }
            }

            function loadRootLevelCollections() {
                var parentPromise = entityCollectionResource.getRootCollectionsByEntityType($scope.entityType);
                parentPromise.then(function(collections) {
                    if (!angular.isArray(collections)) {
                        $scope.entityCollections.push(entityCollectionDisplayBuilder.transform(collections));
                    } else {
                        $scope.entityCollections = entityCollectionDisplayBuilder.transform(collections);
                    }
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

            function save() {
                $scope.wasFormSubmitted = true;
                for(var i = 0; i < $scope.entityCollections.length; i++) {
                    $scope.entityCollections[i].sortOrder = i;
                }

                var promise = entityCollectionResource.updateSortOrders($scope.entityCollections);
                promise.then(function() {
                    var reloadNodePromise = treeService.reloadNode($scope.currentNode);
                    reloadNodePromise.then(function() {
                        var promise = treeService.loadNodeChildren({ node: $scope.currentNode });
                        promise.then(function() {
                            navigationService.hideNavigation();
                            notificationsService.success('Collections sorted success.');
                        });
                    });
                });
            }

            // Sortable available offers
            /// -------------------------------------------------------------------

            $scope.sortableOptions = {
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


