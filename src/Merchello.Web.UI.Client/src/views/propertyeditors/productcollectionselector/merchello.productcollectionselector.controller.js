/**
 * @ngdoc controller
 * @name Merchello.PropertyEditors.MerchelloProductCollectionPicker
 * @function
 *
 * @description
 * The controller for product collection picker property editor view
 */
angular.module('merchello').controller('Merchello.PropertyEditors.MerchelloProductCollectionSelectionController',
    ['$scope', 'dialogService', 'settingsResource', 'notificationsService',
    function($scope, dialogService, settingsResource, notificationsService) {

        $scope.currencySymbol = '';
        $scope.entityType = 'product';

        $scope.listViewResultSet = {
            totalItems: 0
        };

        $scope.openCollectionSelectionDialog = openCollectionSelectionDialog;
        $scope.getTreeId = getTreeId;

        //--------------------------------------------------------------------------------------
        // Initialization methods
        //--------------------------------------------------------------------------------------
        // Load the product from the Guid key stored in the model.value
        if (_.isString($scope.model.value)) {
            loadSettings();
            if ($scope.model.value.length > 0) {

            }
        }

        function loadSettings() {
            var promise = settingsResource.getCurrencySymbol();
            promise.then(function(symbol) {
                $scope.currencySymbol = symbol;
            }, function (reason) {
                notificationsService.error('Could not retrieve currency symbol', reason.message);
            });
        }


        function openCollectionSelectionDialog() {
            var dialogData = {};
            dialogData.collectionKey = $scope.model.value;
            dialogData.entityType =  $scope.entityType;

            dialogService.open({
                template: '/App_Plugins/Merchello/propertyeditors/productcollectionselector/merchello.productcollectionselector.dialog.html',
                show: true,
                callback: processCollectionSelection,
                dialogData: dialogData
            });
        }

        function getTreeId() {
            return "products";
        }

        function processCollectionSelection(dialogData) {
            console.info(dialogData);
        }

    }]);