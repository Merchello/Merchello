/**
 * @ngdoc controller
 * @name Merchello.Product.Dialogs.AddProductContentTypeController
 * @function
 *
 * @description
 * The controller for the adding product content types
 */
angular.module('merchello').controller('Merchello.Product.Dialogs.AddProductContentTypeController',
    ['$scope', '$location', 'notificationsService', 'navigationService', 'eventsService',  'detachedContentResource', 'detachedContentTypeDisplayBuilder',
        function($scope, $location, notificationsService, navigationService, eventsService, detachedContentResource, detachedContentTypeDisplayBuilder) {
            $scope.loaded = true;
            $scope.wasFormSubmitted = false;
            $scope.contentType = {};
            $scope.name = '';
            $scope.description = '';
            $scope.associateType = 'Product';
            var eventName = 'merchello.contenttypedropdown.changed';

            $scope.save = function() {
                $scope.wasFormSubmitted = true;
                if ($scope.productContentTypeForm.name.$valid && $scope.contentType.key) {

                    var dtc = detachedContentTypeDisplayBuilder.createDefault();
                    dtc.umbContentType = $scope.contentType;
                    dtc.entityType = $scope.associateType;
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

            function init() {

                eventsService.on(eventName, onSelectionChanged);
            }

            function onSelectionChanged(e, contentType) {
                if (contentType.name !== null && contentType.name !== undefined) {
                    $scope.name = contentType.name;
                }
            }

            init();
        }]);
