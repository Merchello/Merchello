angular.module('merchello').controller('Merchello.Backoffice.ProductDetachedContentController',
    ['$scope', '$routeParams', '$location', 'notificationsService', 'merchelloTabsFactory', 'contentResource', 'detachedContentResource', 'productResource',
        'productDisplayBuilder', 'detachedContentTypeDisplayBuilder',
    function($scope, $routeParams, $location, notificationsService, merchelloTabsFactory, contentResource, detachedContentResource, productResource,
             productDisplayBuilder, detachedContentTypeDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.productVariant = {};
        $scope.language = '';
        $scope.languages = [];
        $scope.isVariant = false;
        $scope.isConfigured = false;
        $scope.defaultLanguage = 'en-US';
        function init() {
            var key = $routeParams.id;

            // extended content is not valid unless we have a product to attach it to
            if (key === '' || key === undefined) {
                $location.url('/merchello/merchello/productlist/manage', true);
            }
            var productVariantKey = $routeParams.variantid;
            loadLanguages(key, productVariantKey);
        }

        function loadLanguages(key, productVariantKey) {
            detachedContentResource.getAllLanguages().then(function(languages) {
                $scope.languages = languages;
                if($scope.defaultLanguage !== '' && $scope.defaultLanguage !== undefined) {
                    $scope.language = _.find($scope.languages, function(l) { return l.isoCode === $scope.defaultLanguage; });
                }
                loadProduct(key, productVariantKey);
            }, function(reason) {
                notificationsService.error('Failed to load Umbraco languages' + reason);
            });
        }

        /**
         * @ngdoc method
         * @name loadProduct
         * @function
         *
         * @description
         * Load a product by the product key.
         */
        function loadProduct(key, productVariantKey) {

            var promiseProduct = productResource.getByKey(key);
            promiseProduct.then(function (product) {
                $scope.product = productDisplayBuilder.transform(product);
                if(productVariantKey === '' || productVariantKey === undefined) {
                    // this is a product edit.
                    // we use the master variant context so that we can use directives associated with variants
                    $scope.productVariant = $scope.product.getMasterVariant();

                    if (!$scope.product.hasVariants()) {
                        $scope.tabs = merchelloTabsFactory.createProductEditorTabs(key);
                    }
                    else
                    {
                        $scope.tabs = merchelloTabsFactory.createProductEditorWithOptionsTabs(key);
                    }
                } else {
                    // this is a product variant edit
                    // in this case we need the specific variant
                    $scope.productVariant = $scope.product.getProductVariant(productVariantKey);
                    $scope.tabs = merchelloTabsFactory.createProductVariantEditorTabs(key, productVariantKey);
                    $scope.isVariant = true;
                }
                $scope.loaded = true;
                $scope.preValuesLoaded = true;

                $scope.tabs.setActive('productcontent');
            }, function (reason) {
                notificationsService.error("Product Load Failed", reason.message);
            });
        }



        // Initialize the controller
        init();
    }]);
