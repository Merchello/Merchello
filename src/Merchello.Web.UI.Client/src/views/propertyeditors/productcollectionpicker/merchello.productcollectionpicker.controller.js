angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloProductCollectionPickerController', 
    ['$scope', '$q', 'dialogService', 'entityCollectionResource', 'entityCollectionDisplayBuilder',
    function($scope, $q, dialogService, entityCollectionResource, entityCollectionDisplayBuilder) {

        $scope.loaded = false;
        $scope.collection = {};
        $scope.entityType = 'product';
        

        $scope.delete = removeCollection;
        $scope.openCollectionSelectionDialog = openCollectionSelectionDialog;

        function init() {

            if (_.isString($scope.model.value)) {
                if ($scope.model.value !== '') {
                    getCollection().then(function(collection) {
                        $scope.loaded = true;
                    });
                } else {
                    $scope.loaded = true;
                }

            } else {
                $scope.loaded = true;
            }
        }

        function getCollection() {
            var deferred = $q.defer();
            entityCollectionResource.getByKey($scope.model.value).then(function(collection) {
                $scope.collection = entityCollectionDisplayBuilder.transform(collection);
               $q.resolve($scope.collection);
            });
            return deferred.promise;
        }

        function removeCollection() {
            $scope.collection = {};
            $scope.model.value = '';
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
            getCollection();

        }

        init();
        
    }]);
