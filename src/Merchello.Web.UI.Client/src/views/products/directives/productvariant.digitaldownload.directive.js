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
                    product: '=',
                    productVariant: '='
                },
                templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/productvariant.digitaldownload.tpl.html',

                controller: function ($scope, dialogService, mediaHelper, mediaResource) {

                    $scope.id = $scope.productVariant.downloadMediaId;
                    if ($scope.productVariant.download && $scope.id != -1) {
                        mediaResource.getById($scope.id).then(function (media) {
                            if (!media.thumbnail) {
                                media.thumbnail = mediaHelper.resolveFile(media, true);
                            }

                            $scope.mediaItem = media;
                            $scope.mediaItem.umbracoFile = mediaHelper.resolveFile(media, false);
                        });
                    }

                    /**
                     * @ngdoc method
                     * @name chooseMedia
                     * @function
                     *
                     * @description
                     * Called when the select media button is pressed for the digital download section.
                     *
                     * TODO: make a media selection dialog that works with PDFs, etc
                     */
                    $scope.chooseMedia = function () {

                        dialogService.mediaPicker({
                            onlyImages: false,
                            callback: function (media) {
                                if (!media.thumbnail) {
                                    media.thumbnail = mediaHelper.resolveFile(media, true);
                                }
                                $scope.mediaItem = media;
                                $scope.mediaItem.umbracoFile = mediaHelper.resolveFile(media, false);
                                $scope.id = media.id;
                                $scope.productVariant.downloadMediaId = media.id;
                            }
                        });

                    };
                }
            };
    });
