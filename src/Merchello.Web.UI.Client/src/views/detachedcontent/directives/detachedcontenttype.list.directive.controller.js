/**
 * @ngdoc controller
 * @name Merchello.Directives.DetachedContentTypeListController
 * @function
 *
 * @description
 * The controller for the detached content type list directive
 */
angular.module('merchello').controller('Merchello.Directives.DetachedContentTypeListController',
    ['$scope', 'notificationsService', 'detachedContentResource', 'detachedContentTypeDisplayBuilder',
    function($scope, notificationsService, detachedContentResource, detachedContentTypeDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.detachedContentTypes = [];

        function init() {
            loadDetachedContentTypes();
        }

        function loadDetachedContentTypes() {
            detachedContentResource.getDetachedContentTypeByEntityType($scope.entityType).then(function(results) {
                $scope.detachedContentTypes = detachedContentTypeDisplayBuilder.transform(results);
                console.info($scope.detachedContentTypes);
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
            });
        }


        // initialize the controller
        init();
}]);
