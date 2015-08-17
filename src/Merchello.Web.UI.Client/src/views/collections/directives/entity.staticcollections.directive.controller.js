angular.module('merchello').controller('Merchello.Directives.EntityStaticCollectionsDirectiveController',
    ['$scope', 'notificationsService', 'dialogService', 'dialogDataFactory',
    function($scope, notificationsService, dialogService, dialogDataFactory) {


        $scope.openStaticEntityCollectionPicker = function() {
            var dialogData = dialogDataFactory.createAddEditEntityStaticCollectionDialog();
            dialogData.entityType = $scope.entityType;
            dialogData.collectionKeys = [];

            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/pick.staticcollection.html',
                show: true,
                callback: processAddEditStaticCollection,
                dialogData: dialogData
            });
        }

        function processAddEditStaticCollection(dialogData) {

        }
}]);
