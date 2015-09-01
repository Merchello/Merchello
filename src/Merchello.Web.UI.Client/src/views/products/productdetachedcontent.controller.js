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
            $scope.detachedContent = {};
            $scope.tabs = [];

            // Umbraco properties
            $scope.contentTabs = [];
            $scope.currentTab = {};

            // navigation switches
            var showUmbracoTabs = true;
            var merchelloTabs = ['productcontent','variantlist', 'optionslist'];
            var umbracoTabs = [];

            // TODO wire in event handler to watch for content changes so that we can display a notice when swapping languages

            $scope.save = save;
            $scope.saveContentType = createDetachedContent;
            $scope.setLanguage = setLanguage;

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
                    console.info($scope.languages);
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

                    $scope.loaded = true;

                    if ($scope.productVariant.hasDetachedContent()) {
                        $scope.productVariant.assertLanguageContent($scope.languages);
                        $scope.detachedContent = $scope.productVariant.getDetachedContent($scope.language.isoCode);
                        $scope.isConfigured = true;
                        loadScaffold();
                    } else {
                        $scope.preValuesLoaded = true;
                    }
                    $scope.tabs.setActive('productcontent');
                }, function (reason) {
                    notificationsService.error("Product Load Failed", reason.message);
                });
            }

            function loadScaffold() {
                // every detached content associated with a variant MUST share the same content type,
                var detachedContentType = $scope.productVariant.detachedContentType();

                contentResource.getScaffold(-20, detachedContentType.umbContentType.alias).then(function(scaffold) {
                    filterTabs(scaffold);
                    fillValues();
                    if ($scope.contentTabs.length > 0) {
                        $scope.currentTab = $scope.contentTabs[0];
                        $scope.tabs.setActive($scope.currentTab.id);
                        setTabVisibility();
                    }
                    $scope.preValuesLoaded = true;
                });
            }

            function save() {
                if ($scope.productVariant.hasDetachedContent()) {
                    // save the current language only
                    angular.forEach($scope.contentTabs, function(ct) {
                      angular.forEach(ct.properties, function (p) {
                          if (typeof p.value !== "function") {
                              $scope.detachedContent.detachedDataValues.setValue(p.alias, p.value);
                          }
                      });
                    });
                    doSave();
                }
            }

            function createDetachedContent(detachedContent) {
                if(!$scope.productVariant.hasDetachedContent()) {
                    // create detached content values for each language present
                    var isoCodes = _.pluck($scope.languages, 'isoCode');

                    contentResource.getScaffold(-20, detachedContent.umbContentType.alias).then(function(scaffold) {

                        filterTabs(scaffold);

                        angular.forEach(isoCodes, function(cultureName) {
                           var productVariantContent = buildProductVariantDetachedContent(cultureName, detachedContent, $scope.contentTabs);
                            $scope.productVariant.detachedContents.push(productVariantContent);
                        });

                        doSave();
                    });
                }
            }

            function setLanguage(lang) {
                $scope.language = lang;
                save();
            }

            function doSave() {
                var promise;
                if ($scope.productVariant.master) {
                    promise = productResource.save(product);
                } else {
                    promise = productResource.saveVariant($scope.productVariant);
                }

                promise.then(function(p) {
                    $scope.loaded = false;
                    $scope.preValuesLoaded = false;
                    loadProduct(loadArgs);
                    notificationsService.success('Saved successfully');
                }, function(reason) {
                    notificationsService.error('Failed to save product ' + reason)
                });
            }

            function buildProductVariantDetachedContent(cultureName, detachedContent, tabs) {
                var productVariantContent = productVariantDetachedContentDisplayBuilder.createDefault();
                productVariantContent.cultureName = cultureName;
                productVariantContent.productVariantKey = $scope.productVariantKey;
                productVariantContent.detachedContentType = detachedContent;
                angular.forEach(tabs, function(tab) {
                    angular.forEach(tab.properties, function(prop) {
                        productVariantContent.detachedDataValues.setValue(prop.alias, prop.value);
                    })
                });
                return productVariantContent;
            }

            function filterTabs(scaffold) {
                $scope.contentTabs = _.filter(scaffold.tabs, function(t) { return t.id !== 0 });
                if ($scope.contentTabs.length > 0) {
                    angular.forEach($scope.contentTabs, function(ct) {
                        $scope.tabs.addActionTab(ct.id, ct.label, switchTab);
                        umbracoTabs.push(ct.id);
                    });
                   // $scope.tabs.insertActionTab('merchello', 'Merchello', switchTab, 0);
                }
            }

            function setTabVisibility() {
                if (showUmbracoTabs) {
                    angular.forEach(merchelloTabs, function(mt) {
                      $scope.tabs.hideTab(mt);
                    });
                }
            }

            function switchTab(id) {
                var exists = _.find(umbracoTabs, function(ut) {
                    return ut === id;
                });
                if (exists !== undefined) {
                    var fnd = _.find($scope.contentTabs, function (ct) {
                        return ct.id === id;
                    });
                    $scope.currentTab = fnd;
                    $scope.tabs.setActive(id);
                }
            }

            function fillValues() {
                if ($scope.contentTabs.length > 0) {
                    angular.forEach($scope.contentTabs, function(ct) {
                        angular.forEach(ct.properties, function(p) {
                            p.value = $scope.detachedContent.detachedDataValues.getValue(p.alias);
                        });
                    });
                }
            }

            // Initialize the controller
            init();
    }]);
