/**
 * @ngdoc controller
 * @name Merchello.Product.Dialogs.AddProductContentTypeController
 * @function
 *
 * @description
 * The controller for the adding product content types
 */
angular.module('merchello').controller('Merchello.Product.Dialogs.AddProductContentTypeController',
    ['$scope', '$location', 'notificationsService', 'navigationService',  'detachedContentResource', 'detachedContentTypeDisplayBuilder',
        function($scope, $location, notificationsService, navigationService, detachedContentResource, detachedContentTypeDisplayBuilder) {
            $scope.loaded = true;
            $scope.wasFormSubmitted = false;
            $scope.contentType = {};
            $scope.name = '';
            $scope.description = '';

            $scope.save = function() {
                $scope.wasFormSubmitted = true;
                if ($scope.productContentTypeForm.name.$valid && $scope.contentType.key) {
                    var dtc = detachedContentTypeDisplayBuilder.createDefault();
                    dtc.umbContentType = $scope.contentType;
                    dtc.entityType = 'Product';
                    dtc.name = $scope.name;
                    dtc.description = $scope.description;

                    detachedContentResource.addDetachedContentType(dtc).then(function(result) {
                        notificationsService.success("Content Type Saved", "");
                        navigationService.hideNavigation();
                        $location.url("/merchello/merchello/productcontenttypelist/" + result.key, true);
                    }, function(reason) {
                        notificationsService.error('Failed to add detached content type ' + reason);
                    });
                }
            }
        }]);
