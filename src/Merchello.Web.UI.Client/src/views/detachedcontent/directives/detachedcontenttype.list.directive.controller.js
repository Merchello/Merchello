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
        $scope.args =  { test: 'action hit' };

        $scope.showAlert = showAlert;

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

        function showAlert(value) {
            if(value !== undefined) {
                alert('there was a value');
            } else {
                alert('there was not a value');
            }
        }


        // initialize the controller
        init();
}]);
