/**
 * @ngdoc controller
 * @name Merchello.Directives.DetachedContentTypeListController
 * @function
 *
 * @description
 * The controller for the detached content type list directive
 */
angular.module('merchello').controller('Merchello.Directives.DetachedContentTypeListController',
    ['$scope', 'notificationsService', 'localizationService', 'dialogService', 'detachedContentResource', 'dialogDataFactory', 'detachedContentTypeDisplayBuilder',
    function($scope, notificationsService, localizationService, dialogService, detachedContentResource, dialogDataFactory, detachedContentTypeDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.detachedContentTypes = [];
        $scope.args =  { test: 'action hit' };

        $scope.edit = editContentType;
        $scope.delete = deleteContentType;

        $scope.debugAllowDelete = false;

        function init() {
            $scope.debugAllowDelete = Umbraco.Sys.ServerVariables.isDebuggingEnabled;
            loadDetachedContentTypes();
        }

        function loadDetachedContentTypes() {
            detachedContentResource.getDetachedContentTypeByEntityType($scope.entityType).then(function(results) {
                $scope.detachedContentTypes = detachedContentTypeDisplayBuilder.transform(results);
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            });
        }

        function editContentType(contentType) {
            var dialogData = dialogDataFactory.createEditDetachedContentTypeDialogData();

            // we need to clone this so that the actual model in the scope is not updated in case the user
            // does not hit save.
            dialogData.contentType = detachedContentTypeDisplayBuilder.transform(contentType);
            dialogService.open({
                template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/detachedcontenttype.edit.html',
                show: true,
                callback: processEditDialog,
                dialogData: dialogData
            });
        }

        function processEditDialog(dialogData) {
            detachedContentResource.saveDetachedContentType(dialogData.contentType).then(function(dct) {
                loadDetachedContentTypes();
                notificationsService.success('Saved successfully');
            }, function(reason) {
                notificationsService.error('Failed to save detached content type' + reason);
            });
        }

        /**
         * @ngdoc method
         * @name deleteContentType
         * @function
         *
         * @description - Opens the delete content type dialog.
         */
        function deleteContentType(contentType) {
            var dialogData = {};
            dialogData.name = contentType.name;
            dialogData.contentType = contentType;
            localizationService.localize('merchelloDetachedContent_deleteWarning').then(function(warning) {
                dialogData.warning = warning;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processDeleteDialog,
                    dialogData: dialogData
                });
            });
        }

        function processDeleteDialog(dialogData) {
            detachedContentResource.deleteDetachedContentType(dialogData.contentType.key).then(function() {
                loadDetachedContentTypes();
                notificationsService.success('Deleted successfully');
            }, function(reason) {
                console.info(reason);
              notificationsService.error('Failed to delete detached content type' + reason);
            });
        }


        // initialize the controller
        init();
}]);
