/**
 * @ngdoc controller
 * @name Merchello.Product.Dialogs.AddDetachedContentTypeController
 * @function
 *
 * @description
 * The controller for the adding product content types
 */
angular.module('merchello').controller('Merchello.Product.Dialogs.AddDetachedContentTypeController',
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

                    $scope.dialogData.contentType.umbContentType = $scope.contentType;
                    $scope.submit($scope.dialogData);
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
