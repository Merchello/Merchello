(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Product.EditController
     * @function
     * 
     * @description
     * The controller for the product editor
     */
    controllers.ProductEditController = function ($scope, $routeParams, $location, notificationsService, dialogService, angularHelper, serverValidationManager, merchelloProductService, merchelloProductVariantService) {

        if ($routeParams.create) {

            // TODO: this should redirect to product variant edit
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.product = new merchello.Models.Product();
            $(".content-column-body").css('background-image', 'none');
        }
        else {

            //we are editing so get the product from the server
            var promise = merchelloProductService.getByKey($routeParams.id);

            promise.then(function (product) {

                $scope.product = new merchello.Models.Product(product);

                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                $(".content-column-body").css('background-image', 'none');

            }, function (reason) {

                notificationsService.error("Product Load Failed", reason.message);

            });
        }

        $scope.save = function () {

            notificationsService.info("Saving...", "");

            //we are editing so get the product from the server
            var promise = merchelloProductService.save($scope.product);

            promise.then(function (product) {

                notificationsService.success("Product Saved", "H5YR!");

            }, function (reason) {

                notificationsService.error("Product Save Failed", reason.message);

            });
        };

        $scope.chooseMedia = function () {

            dialogService.mediaPicker({
                multipicker: true,
                callback: function (data) {
                    _.each(data.selection, function (media, i) {
                        //shortcuts
                        //TODO, do something better then this for searching

                        var img = {};
                        img.id = media.id;
                        img.src = imageHelper.getImagePropertyValue({ imageModel: media });
                        img.thumbnail = imageHelper.getThumbnailFromPath(img.src);
                        $scope.images.push(img);
                        $scope.ids.push(img.id);
                    });

                    $scope.sync();
                }
            });

        };

    }

    angular.module("umbraco").controller("Merchello.Editors.Product.EditController", merchello.Controllers.ProductEditController);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

