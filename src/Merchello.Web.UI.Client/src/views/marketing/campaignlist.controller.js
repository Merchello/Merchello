/**
 * @ngdoc controller
 * @name Merchello.Backoffice.CampaignListController
 * @function
 *
 * @description
 * The controller for the marketing campaign list
 */
angular.module('merchello').controller('Merchello.Backoffice.CampaignListController',
    ['$scope', 'assetsService', 'notificationsService', 'dialogService',
    function($scope, assetsService, notificationsService, dialogService) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;

}]);
