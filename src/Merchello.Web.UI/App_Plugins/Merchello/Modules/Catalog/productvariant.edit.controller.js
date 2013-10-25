(function (controllers, undefined) {

    /**
     * @ngdoc controller
     * @name Merchello.Editors.ProductVariant.EditController
     * @function
     * 
     * @description
     * The controller for the product editor
     */
    controllers.ProductVariantEditController = function ($scope, $routeParams, $location, notificationsService, dialogService, angularHelper, serverValidationManager, merchelloProductService, merchelloProductVariantService) {

        function loadProductVariant(id) {

            var promiseVariant = merchelloProductVariantService.getById(id);
            promiseVariant.then(function (productVariant) {

                $scope.productVariant = new merchello.Models.ProductVariant(productVariant);

                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                $(".content-column-body").css('background-image', 'none');

            }, function (reason) {

                notificationsService.error("Product Variant Load Failed", reason.message);

            });
        }

        function loadProduct(key, hasVariants) {

            var promiseProduct = merchelloProductService.getByKey(key);
            promiseProduct.then(function (product) {

                $scope.product = new merchello.Models.Product(product);

                if (!hasVariants)
                {
                    // create the master variant (since most form items are bound to the productVariant model)
                    $scope.productVariant.copyFromProduct($scope.product);
                }

                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                $(".content-column-body").css('background-image', 'none');

            }, function (reason) {

                notificationsService.error("Product Load Failed", reason.message);

            });
        }

        ////////////////////////////////////////////////
        // Initialize state
        if ($routeParams.create) {
            $scope.creatingVariant = true;
            $scope.loaded = true;
            $scope.preValuesLoaded = true;
            $scope.productVariant = new merchello.Models.ProductVariant();
            $scope.product = new merchello.Models.Product();
            $(".content-column-body").css('background-image', 'none');
        }
        else {

            $scope.creatingVariant = false;
            $scope.productVariant = new merchello.Models.ProductVariant();
            $scope.product = {};

            if ($routeParams.variantid == undefined)
            {
                //we are editing a product with no options so get the product from the server
                loadProduct($routeParams.id, false);
            }
            else
            {
                //we are editing a variant so get the product variant and product from the server
                loadProductVariant($routeParams.variantid);

                loadProduct($routeParams.id, true);
            }

        }
        ////////////////////////////////////////////////


        ////////////////////////////////////////////////
        // EVENTS

        $scope.save = function (thisForm) {

            if (thisForm.$valid)
            {

                notificationsService.info("Saving...", "");


                if ($scope.creatingVariant)
                {
                    // Copy from master variant
                    $scope.product.copyFromVariant($scope.productVariant);

                    var promiseCreate = merchelloProductService.create($scope.productVariant.name, $scope.productVariant.sku, $scope.productVariant.price);
                    promiseCreate.then(function (product) {

                        $scope.product.key = product.key;
                    
                        $scope.creatingVariant = false;

                        // Created, now save the initial settings from the model
                        var promiseSave = merchelloProductService.save($scope.product);
                        promiseSave.then(function (product) {

                            notificationsService.success("Product Saved! H5YR!");

                            if ($scope.product.hasVariants)
                            {
                                $location.url("/merchello/merchello/ProductEdit/" + $scope.product.key, true);
                            }

                        }, function (reason) {

                            notificationsService.error("Product Save Failed", reason.message);

                        });

                        notificationsService.success("Product Created", $scope.product.Key);

                    }, function (reason) {

                        notificationsService.error("Product Creation Failed", reason.message);

                    });
                }
                else
                {
                    if ($scope.product.hasVariants)
                    {
                        notificationsService.error("Save Not Implemented", "");
                    }
                    else
                    {
                        // Copy from master variant
                        $scope.product.copyFromVariant($scope.productVariant);

                        var promise = merchelloProductService.save($scope.product);

                        promise.then(function (product) {

                            notificationsService.success("Product Saved", "H5YR!");

                        }, function (reason) {

                            notificationsService.error("Product Save Failed", reason.message);

                        });
                    }
                }
            }
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

            $scope.product.addBlankOption();

            //$scope.product.productOptions.push({ name: "", required: 0, sortOrder: $scope.product.productOptions.length + 1, choices: [{ name: "one", sku: "one-sku", sortOrder: 1 }] });
            //createVariants();
        };

        //createVariants = function () {

        //    var choicesIndex = new Array();
        //    var choicesLength = new Array();

        //    $scope.product.productVariants = [];

        //    // Init indexes and lengths
        //    for (var o = 0; o < $scope.product.productOptions.length; o++) {
        //        choicesIndex.push(0);
        //        choicesLength.push($scope.product.productOptions[o].choices.length);
        //    }

        //    while (choicesIndex[0] < choicesLength[0]) {
        //        // get current choices from indexes and create variants if they don't exist
        //        var variant = { productAttributes: [] }
        //        for (c = 0; c < choicesIndex.length; c++) {
        //            variant.productAttributes.push($scope.product.productOptions[c].choices[choicesIndex[c]]);
        //        }

        //        $scope.product.productVariants.push(variant);

        //        // loop through indexes from the last one to the first one (check if the last exists and increment the next one up)
        //        for (var i = choicesIndex.length - 1; i >= 0; i--) {

        //            choicesIndex[i] = choicesIndex[i] + 1;

        //            if (choicesIndex[i] <= choicesLength[i]) {
        //                break;
        //            }
        //            else {
        //                choicesIndex[i] = 0;
        //            }
        //        }

        //    }
        //};


        ////////////////////////////////////////////////
    }

    angular.module("umbraco").controller("Merchello.Editors.ProductVariant.EditController", merchello.Controllers.ProductVariantEditController);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

