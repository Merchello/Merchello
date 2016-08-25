/**
 * @ngdoc controller
 * @name Merchello.Common.Dialogs.CreateStaticCollectionController
 * @function
 *
 * @description
 * The controller for the delete confirmations
 */
angular.module('merchello')
    .controller('Merchello.EntityCollections.Dialogs.CreateStaticCollectionController',
    ['$scope', 'appState', 'treeService', 'notificationsService', 'navigationService', 'entityCollectionHelper', 'entityCollectionResource', 'entityCollectionProviderDisplayBuilder', 'entityCollectionDisplayBuilder',
        function($scope, appState, treeService, notificationsService, navigationService, entityCollectionHelper, entityCollectionResource, entityCollectionProviderDisplayBuilder, entityCollectionDisplayBuilder) {

            $scope.loaded = false;
            $scope.name = '';
            $scope.wasFormSubmitted = false;
            $scope.entityType = '';
            $scope.provider = {};
            $scope.dialogData = {};
            $scope.entityCollectionProviders = [];


            // exposed methods
            $scope.save = save;

            function init() {
                $scope.dialogData = $scope.$parent.currentAction.metaData.dialogData;
                $scope.entityType = entityCollectionHelper.getEntityTypeByTreeId($scope.dialogData.entityType);
                loadProviders();
            }

            function loadProviders() {
                var promise = entityCollectionResource.getDefaultEntityCollectionProviders();
                promise.then(function(results) {
                    $scope.entityCollectionProviders = entityCollectionProviderDisplayBuilder.transform(results);
                    $scope.provider = _.find($scope.entityCollectionProviders, function(p) { return p.entityType == $scope.entityType; });

                    // todo this needs to be handled better
                    if ($scope.provider == null || $scope.provider == undefined) {
                        navigationService.hideNavigation();
                    }

                    $scope.loaded = true;
                }, function(reason) {
                    notificationsService.error("Failted to load default providers", reason.message);
                });
            }

            function save() {
                $scope.wasFormSubmitted = true;
                if ($scope.collectionForm.name.$valid) {
                    var collection = entityCollectionDisplayBuilder.createDefault();
                    collection.providerKey = $scope.provider.key;
                    collection.entityTfKey = $scope.provider.entityTfKey;
                    collection.entityType = $scope.provider.entityType;
                    collection.parentKey = $scope.dialogData.parentKey;
                    collection.name = $scope.name;
                    var promise = entityCollectionResource.addEntityCollection(collection);
                    promise.then(function() {
                        navigationService.hideNavigation();

                        var reloadNodePromise = treeService.reloadNode($scope.currentNode);
                        reloadNodePromise.then(function() {
                            var promise = treeService.loadNodeChildren({ node: $scope.currentNode });
                            promise.then(function() {
                                notificationsService.success('New collection added.');
                            });
                        });

                    });
                }
            }

            init();
    }]);

