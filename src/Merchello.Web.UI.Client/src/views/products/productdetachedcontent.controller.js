angular.module('merchello').controller('Merchello.Backoffice.ProductDetachedContentController',
    ['$scope', '$routeParams', '$location', 'notificationsService', 'merchelloTabsFactory', 'contentResource', 'detachedContentResource', 'productResource',
        'productDisplayBuilder', 'productVariantDetachedContentDisplayBuilder',
        function($scope, $routeParams, $location, notificationsService, merchelloTabsFactory, contentResource, detachedContentResource, productResource,
             productDisplayBuilder, productVariantDetachedContentDisplayBuilder) {

        $scope.loaded = false;
        $scope.preValuesLoaded = false;
        $scope.productVariant = {};
        $scope.language = '';
        $scope.languages = [];
        $scope.isVariant = false;
        $scope.isConfigured = false;
        $scope.defaultLanguage = 'en-US';
        $scope.contentType = {};

        $scope.save = saveContent;

        var product = {};
        var loadArgs = {
            key: '',
            productVariantKey: ''
        };

        function init() {
            var key = $routeParams.id;

            // extended content is not valid unless we have a product to attach it to
            if (key === '' || key === undefined) {
                $location.url('/merchello/merchello/productlist/manage', true);
            }
            var productVariantKey = $routeParams.variantid;
            loadArgs.key = key;
            loadArgs.productVariantKey = productVariantKey;
            loadLanguages(loadArgs);
        }

        function loadLanguages(args) {
            detachedContentResource.getAllLanguages().then(function(languages) {
                $scope.languages = languages;
                if($scope.defaultLanguage !== '' && $scope.defaultLanguage !== undefined) {
                    $scope.language = _.find($scope.languages, function(l) { return l.isoCode === $scope.defaultLanguage; });
                }
                loadProduct(args);
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
        function loadProduct(args) {

            var promiseProduct = productResource.getByKey(args.key);
            promiseProduct.then(function (p) {
                product = productDisplayBuilder.transform(p);
                if(args.productVariantKey === '' || args.productVariantKey === undefined) {
                    // this is a product edit.
                    // we use the master variant context so that we can use directives associated with variants
                    $scope.productVariant = product.getMasterVariant();

                    if (!product.hasVariants()) {
                        $scope.tabs = merchelloTabsFactory.createProductEditorTabs(args.key);
                    }
                    else
                    {
                        $scope.tabs = merchelloTabsFactory.createProductEditorWithOptionsTabs(args.key);
                    }
                } else {
                    // this is a product variant edit
                    // in this case we need the specific variant
                    $scope.productVariant = product.getProductVariant(args.productVariantKey);
                    $scope.tabs = merchelloTabsFactory.createProductVariantEditorTabs(args.key, args.productVariantKey);
                    $scope.isVariant = true;
                }

                if ($scope.productVariant.hasDetachedContent()) {
                    $scope.productVariant.assertLanguageContent($scope.languages);
                }

                $scope.loaded = true;
                $scope.preValuesLoaded = true;

                $scope.tabs.setActive('productcontent');
            }, function (reason) {
                notificationsService.error("Product Load Failed", reason.message);
            });
        }


        function saveContent(detachedContent) {
            if(!$scope.productVariant.hasDetachedContent()) {
                // create detached content values for each language present
                var isoCodes = _.pluck($scope.languages, 'isoCode');

                contentResource.getScaffold(-20, detachedContent.umbContentType.alias).then(function(scaffold) {

                    var tabs = _.filter(scaffold.tabs, function(t) { return t.id !== 0 });

                    angular.forEach(isoCodes, function(cultureName) {
                        var productVariantContent = productVariantDetachedContentDisplayBuilder.createDefault();
                        productVariantContent.cultureName = cultureName;
                        productVariantContent.productVariantKey = $scope.productVariantKey;
                        productVariantContent.detachedContentType = detachedContent;
                        angular.forEach(tabs, function(tab) {
                          angular.forEach(tab.properties, function(prop) {
                              productVariantContent.detachedDataValues.setValue(prop.alias, prop.value);
                          })
                        });
                        $scope.productVariant.detachedContents.push(productVariantContent);
                    });

                    productResource.save(product).then(function(p) {
                        $scope.loaded = false;
                        $scope.preValuesLoaded = false;
                        loadProduct(loadArgs);
                        notificationsService.success('Saved successfully');
                    }, function(reason) {
                        notificationsService.error('Failed to save product ' + reason)
                    });
                });

            }



        }

        function ensureLanguageContent() {

        }

        // Initialize the controller
        init();
    }]);
