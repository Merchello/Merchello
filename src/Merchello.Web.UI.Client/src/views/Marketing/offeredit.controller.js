/**
 * @ngdoc controller
 * @name Merchello.Backoffice.OffersListController
 * @function
 *
 * @description
 * The controller for offers list view controller
 */
angular.module('merchello').controller('Merchello.Backoffice.OfferEditController',
    ['$scope', '$location', 'assetsService', 'notificationsService', 'settingsResource', 'merchelloTabsFactory', 'dialogDataFactory',
    function($scope, $location, assetsService, notificationsService, settingsResource, merchelloTabsFactory, dialogDataFactory) {

        $scope.loaded = true;
        $scope.preValuesLoaded = true;
    }]);
