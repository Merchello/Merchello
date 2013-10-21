
(function(angular) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.Product.EditController
     * @function
     * 
     * @description
     * The controller for the product editor
     */
    function ProductEditController($scope, $routeParams, $location, notificationsService, dialogService, angularHelper, serverValidationManager, merchelloProductService) {

        if ($routeParams.create) {
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.product = {};
            $(".content-column-body").css('background-image', 'none');
        }
        else {

            //we are editing so get the product from the server
            var promise = merchelloProductService.getByKey($routeParams.id);

            promise.then(function (product) {

                $scope.product = product;
                if (product.productOptions.length > 0)
                {
                    $scope.product.hasoptions = true;
                }
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

        $scope.addOption = function () {
            $scope.product.productOptions.push({ name: "", required: 0, sortOrder: $scope.product.productOptions.length + 1, choices: [{ name: "one", sku: "one-sku", sortOrder: 1 }] });
            createVariants();
        };

        createVariants = function () {

            var choicesIndex = new Array();
            var choicesLength = new Array();

            $scope.product.productVariants = [];

            // Init indexes and lengths
            for (var o = 0; o < $scope.product.productOptions.length; o++)
            {
                choicesIndex.push(0);
                choicesLength.push($scope.product.productOptions[o].choices.length);
            }

            while (choicesIndex[0] < choicesLength[0])
            {
                // get current choices from indexes and create variants if they don't exist
                var variant = { productAttributes:[] }
                for (c = 0; c < choicesIndex.length; c++)
                {
                    variant.productAttributes.push($scope.product.productOptions[c].choices[choicesIndex[c]]);
                }

                $scope.product.productVariants.push(variant);

                // loop through indexes from the last one to the first one (check if the last exists and increment the next one up)
                for (var i = choicesIndex.length-1; i >= 0; i--) {

                    choicesIndex[i] = choicesIndex[i] + 1;
                
                    if (choicesIndex[i] <= choicesLength[i])
                    {
                        break;
                    }
                    else
                    {
                        choicesIndex[i] = 0;
                    }
                }

            }
        };
    }

    angular.module("umbraco").controller("Merchello.Editors.Product.EditController", ProductEditController);

}(angular))