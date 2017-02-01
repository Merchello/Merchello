angular.module('merchello').controller('Merchello.EntityCollections.Dialogs.DeleteEntityCollectionController', [
    '$scope', '$location', 'treeService', 'navigationService', 'notificationsService', 'entityCollectionResource', 'entityCollectionDisplayBuilder',
    function($scope, $location, treeService, navigationService, notificationsService, entityCollectionResource, entityCollectionDisplayBuilder) {

        $scope.loaded = false;
        $scope.dialogData = {};
        $scope.entityCollection = {};
        $scope.refreshPath = {};
        $scope.confirmDelete = confirmDelete;

        function init() {
            $scope.dialogData = $scope.$parent.currentAction.metaData.dialogData;
            $scope.refreshPath = treeService.getPath($scope.$parent.currentNode);
            loadEntityCollection();
        }

        function loadEntityCollection() {
            var promise = entityCollectionResource.getByKey($scope.dialogData.collectionKey);
            promise.then(function(collection) {
                $scope.entityCollection = entityCollectionDisplayBuilder.transform(collection);
                $scope.dialogData.name = $scope.entityCollection.name;
                $scope.loaded = true;
            }, function(reason) {
                notificationsService.error("Failted to entity collection", reason.message);
            });
        }

        function confirmDelete() {
            var promise = entityCollectionResource.deleteEntityCollection($scope.dialogData.collectionKey);
            promise.then(function(){
                navigationService.hideNavigation();
                treeService.removeNode($scope.currentNode);
                notificationsService.success('Collection deleted');
            }, function(reason) {
                notificationsService.error("Failted to delete entity collection", reason.message);
            });
        }

        // initialize the controller
        init();
    }]);
