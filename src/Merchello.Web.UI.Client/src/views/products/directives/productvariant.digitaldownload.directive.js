    /**
     * @ngdoc controller
     * @name productVariantDigitalDownload
     * @function
     *
     * @description
     * The productVariantDigitalDownload directive
     */
    angular.module('merchello.directives').directive('productVariantDigitalDownload',
        function() {

            return {
                restrict: 'E',
                replace: true,
                scope: {
                    productVariant: '=',
                    preValuesLoaded: '='
                },
                templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/productvariant.digitaldownload.tpl.html',
                link: function(scope, element, attributes) {
                    scope.$watch(attributes.preValuesLoaded, function(value) {
                        scope.initialize();
                    });
                },
                controller: function ($scope, dialogService, mediaHelper, mediaResource) {

                    $scope.mediaItem = null;
                    $scope.thumbnail = '';
                    $scope.icon = '';

                    $scope.chooseMedia = chooseMedia;
                    $scope.removeMedia = removeMedia;
                    $scope.initialize = initialize;

                    function init() {
                        if ($scope.productVariant.download && $scope.productVariant.downloadMediaId != -1) {
                            mediaResource.getById($scope.productVariant.downloadMediaId).then(function (media) {
                                $scope.mediaItem = media;
                                $scope.mediaItem.umbracoFile = mediaHelper.resolveFile(media, false);
                                if(!media.thumbnail) {
                                    $scope.thumbnail = mediaHelper.resolveFile(media, true);
                                }
                                $scope.icon = media.icon;
                            });

                        }
                    }

                    /**
                     * @ngdoc method
                     * @name chooseMedia
                     * @function
                     *
                     * @description
                     * Called when the select media button is pressed for the digital download section.
                     *
                     */
                    function chooseMedia() {

                        dialogService.mediaPicker({
                            onlyImages: false,
                            callback: function (media) {
                                $scope.thumbnail = '';
                                if (!media.thumbnail) {
                                    $scope.thumbnail = mediaHelper.resolveFile(media, true);
                                }
                                $scope.mediaItem = media;
                                $scope.mediaItem.umbracoFile = mediaHelper.resolveFile(media, false);
                                $scope.productVariant.downloadMediaId = media.id;
                                $scope.icon = media.icon;
                            }
                        });
                    }

                    function removeMedia() {
                        $scope.productVariant.downloadMediaId = -1;
                        $scope.mediaItem = null;
                    }

                    function initialize() {
                        init();
                    }
                }
            };
    });
