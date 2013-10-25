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

        $scope.ensureInitialOption = function () {

            if ($scope.product.productOptions.length == 0)
            {
                $scope.product.addBlankOption();
            }
        };

        $scope.addOption = function () {

            $scope.product.addBlankOption();

        };
        
        $scope.updateVariants = function () {

            var choiceSets = [];
            var permutation = [];

            for(var i=0; i < $scope.product.productOptions.length; i++)
            {
                var currentOption = $scope.product.productOptions[i];
                choiceSets.push(currentOption.choices);
                permutation.push('');
            }
 
            $scope.possibleProductVariants = [];
            $scope.product.productVariants = [];

            permute(choiceSets, 0, permutation);

            for (var p = 0; p < $scope.possibleProductVariants.length; p++)
            {
                // Todo: check if already exists
                $scope.product.addVariant($scope.possibleProductVariants[p]);
            }
        };


        // Builds the possible variants
        // sets = array or arrays of choices
        // set = current iteration
        // permutation = array of variant combinations
        function permute(sets, set, permutation)
        {
            for (var i = 0; i < sets[set].length; ++i)
            {
                permutation[set] = sets[set][i];

                if (set < (sets.length - 1))
                {
                    permute(sets, set + 1, permutation);
                }
                else
                {
                    $scope.possibleProductVariants.push(permutation.slice(0));
                }
            }
        }

        ////////////////////////////////////////////////
    }

    angular.module("umbraco").controller("Merchello.Editors.ProductVariant.EditController", merchello.Controllers.ProductVariantEditController);

}(window.merchello.Controllers = window.merchello.Controllers || {}));

