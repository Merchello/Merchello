/*! merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2015 Merchello;
 * Licensed MIT
 */

(function() { 

/**
 * @ngdoc directive
 * @name offerComponents
 *
 * @description
 * Common form elements for Merchello's OfferComponents
 */
angular.module('merchello.directives').directive('offerComponents', function() {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            offerSettings: '=',
            components: '=',
            preValuesLoaded: '=',
            settings: '=',
            saveOfferSettings: '&',
            componentType: '@'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/offer.components.tpl.html',
        controller:  'Merchello.Directives.OfferComponentsDirectiveController'
    }
})


/**
 * @ngdoc directive
 * @name offerMainProperties
 *
 * @description
 * Common form elements for Merchello's OfferSettings
 */
angular.module('merchello.directives').directive('offerMainProperties', function() {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            offer: '=',
            context: '=',
            settings: '=',
            toggleOfferExpires: '&'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/offer.mainproperties.tpl.html'
    };
})

angular.module('merchello.directives').directive('uniqueOfferCode', function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            offer: '=',
            offerCode: '=',
            offerForm: '='
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/offer.uniqueoffercode.tpl.html',
        controller: function($scope, eventsService, marketingResource) {

            $scope.loaded = false;
            $scope.checking = false;
            $scope.isUnique = true;

            var eventOfferSavingName = 'merchello.offercoupon.saving';
            var input = angular.element( document.querySelector( '#offerCode' ) );
            var container = angular.element( document.querySelector("#unique-offer-check") );

            var currentCode = '';

            function init() {
                container.hide();
                eventsService.on(eventOfferSavingName, onOfferSaving)
                input.bind("keyup keypress", function (event) {
                    var code = event.which;
                    // alpha , numbers, ! and backspace

                    if ((code >47 && code <58) || (code >64 && code <91) || (code >96 && code <123) || code === 33 || code == 8) {
                        $scope.$apply(function () {
                            if ($scope.offerCode !== '') {
                                checkUniqueOfferCode($scope.offerCode);
                                currentCode = $scope.offerCode;
                            }
                        });
                    } else {
                        $scope.checking = true;
                        event.preventDefault();
                    }
                });
                $scope.$watch('offerCode', function(oc) {
                    if($scope.offerCode !== undefined) {
                        if (!$scope.loaded) {
                            $scope.loaded = true;
                            currentCode = $scope.offer.offerCode;
                            checkUniqueOfferCode($scope.offer.offerCode);
                        }
                    }
                });
            }
            function checkUniqueOfferCode(offerCode) {
                $scope.checking = true;
                if (offerCode === '') {
                    $scope.checking = false;
                } else {
                    container.show();
                    if (offerCode === currentCode) {
                        $scope.checking = false;
                        return true;
                    }
                    var checkPromise = marketingResource.checkOfferCodeIsUnique(offerCode);
                    checkPromise.then(function(result) {
                        $scope.checking = false;
                        $scope.isUnique = result;
                    });
                }
            }

            function onOfferSaving(e, frm) {
                var valid = $scope.offer.offerCode !== '';
                if (valid) {
                    checkUniqueOfferCode($scope.offer.offerCode);
                    valid = $scope.isUnique;
                    $scope.offerCode = $scope.offer.offerCode
                }
                frm.offerCode.$setValidity('offerCode', valid);
            }
            // Initialize
            init();
        }
    };
});

angular.module('merchello.directives').directive('entityCollectionTitleBar', function($compile, localizationService, entityCollectionResource, entityCollectionDisplayBuilder, entityCollectionProviderDisplayBuilder) {
  return {
    restrict: 'E',
    replace: true,
    scope: {
      collectionKey: '=',
      entityType: '='
    },
    template: '<h2>{{ collection.name }}</h2>',
    link: function(scope, element, attrs) {

      scope.collection = {};

      function init() {
        scope.$watch('collectionKey', function(newValue, oldValue) {
          loadCollection();
        });
      }

      function loadCollection() {
        if(scope.collectionKey === 'manage' || scope.collectionKey === '') {
          var key = 'merchelloCollections_all' + scope.entityType;
          localizationService.localize(key).then(function (value) {
            scope.collection.name = value;
          });
        } else {
          entityCollectionResource.getByKey(scope.collectionKey).then(function(collection) {
            var retrieved = entityCollectionDisplayBuilder.transform(collection);
            entityCollectionResource.getEntityCollectionProviders().then(function(results) {
              var providers = entityCollectionProviderDisplayBuilder.transform(results);
              var provider = _.find(providers, function(p) {
                return p.key === retrieved.providerKey;
              });
              if (provider !== undefined && provider.managesUniqueCollection && provider.localizedNameKey !== '') {
                localizationService.localize(provider.localizedNameKey.replace('/', '_')).then(function(value) {
                  scope.collection.name = value;
                });
              } else {
                scope.collection = retrieved;
              }
            });
          });
        }
      }

      init();
    }
  }
});

/**
 * @ngdoc directive
 * @name static-collection-tree-picker
 * @function
 *
 * @description
 * Directive to pick static entity collections.
 */
angular.module('merchello.directives').directive('entityStaticCollections',
    function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                preValuesLoaded: '=',
                entity: '=',
                entityType: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/entity.staticcollections.tpl.html',
            controller: 'Merchello.Directives.EntityStaticCollectionsDirectiveController'
        }
});

angular.module('merchello.directives').directive('merchCollectionTreeItem', function($compile, treeService, eventsService) {
    return {
        restrict: 'E',
        replace: true,

        scope: {
            section: '@',
            currentNode: '=',
            node: '=',
            tree: '=',
            hasSelection: '&?',
            mode: '@'
        },

        template: '<li ng-class="{\'current\': (node == currentNode)}">' +
        '<div ng-class="getNodeCssClass(node)" id="{{node.id}}">' +
        '<ins style="width:18px;"></ins>' +
        '<ins ng-class="{\'icon-navigation-right\': !node.expanded, \'icon-navigation-down\': node.expanded}" ng-click="load(node)"></ins>' +
        '<i class="icon umb-tree-icon sprTree"></i>' +
        '<a ng-click="select(node, $event)"></a>' +
        '<div ng-show="node.loading" class="l"><div></div></div>' +
        '</div>' +
        '</li>',

        link: function (scope, element, attrs) {

            var eventName = 'merchello.entitycollection.selected';

            // updates the node's DOM/styles
            function setupNodeDom(node, tree) {

                //get the first div element
                element.children(":first")
                    //set the padding
                    .css("padding-left", (node.level * 20) + "px");

                //remove first 'ins' if there is no children
                //show/hide last 'ins' depending on children
                if (!node.hasChildren) {
                    element.find("ins:first").remove();
                    element.find("ins").last().hide();
                }
                else {
                    element.find("ins").last().show();
                }

                var icon = element.find("i:first");
                icon.addClass(node.cssClass);
                icon.attr("title", node.routePath);

                element.find("a:first").html(node.name);

                if (!node.menuUrl) {
                    element.find("a.umb-options").remove();
                }

                if (node.style) {
                    element.find("i:first").attr("style", node.style);
                }
            }

            /**
             Method called when an item is clicked in the tree, this passes the
             DOM element, the tree node object and the original click
             and emits it as a treeNodeSelect element if there is a callback object
             defined on the tree
             */
            scope.select = function(n, ev) {
               var args = buildArgs(n);
                var el = $('#' + n.id + ' i.icon');
                if ($(el).hasClass('icon-list')) {
                    // single mode
                    if (scope.mode === 'single' && scope.hasSelection()) {
                        return;
                    }
                    args.key = 'addCollection';
                    $(el).removeClass('icon-list').addClass('icon-check');
                } else {
                    $(el).removeClass('icon-check').addClass('icon-list');
                    args.key = 'removeCollection';
                }
                eventsService.emit(eventName, args, ev);
                //emitEvent(eventName, args);
                //emitEvent("treeNodeSelect", { element: element, tree: scope.tree, node: n, event: ev });
            };

            function buildArgs(n) {
                var args = { key: '', value: '' };
                var id = n.id + '';
                var ids = id.split('_');
                args.value = ids[1];
                return args;
            }

            /** method to set the current animation for the node.
             *  This changes dynamically based on if we are changing sections or just loading normal tree data.
             *  When changing sections we don't want all of the tree-ndoes to do their 'leave' animations.
             */
            scope.animation = function () {
                if (scope.node.showHideAnimation) {
                    return scope.node.showHideAnimation;
                }
                else {
                    return {};
                }
            };

            /**
             Method called when a node in the tree is expanded, when clicking the arrow
             takes the arrow DOM element and node data as parameters
             emits treeNodeCollapsing event if already expanded and treeNodeExpanding if collapsed
             */
            scope.load = function (node) {
                if (node.expanded) {
                    node.expanded = false;
                }
                else {
                    scope.loadChildren(node, false);
                }
            };

            /* helper to force reloading children of a tree node */
            scope.loadChildren = function (node, forceReload) {
                //emit treeNodeExpanding event, if a callback object is set on the tree
               // emitEvent("treeNodeExpanding", { tree: scope.tree, node: node });

                if (node.hasChildren && (forceReload || !node.children || (angular.isArray(node.children) && node.children.length === 0))) {
                    //get the children from the tree service
                    treeService.loadNodeChildren({ node: node, section: scope.section })
                        .then(function (data) {
                            //emit expanded event
                            //emitEvent("treeNodeExpanded", { tree: scope.tree, node: node, children: data });
                        });
                }
                else {
                    //emitEvent("treeNodeExpanded", { tree: scope.tree, node: node, children: node.children });
                    node.expanded = true;
                }
            };

            //if the current path contains the node id, we will auto-expand the tree item children

            setupNodeDom(scope.node, scope.tree);

            var template = '<ul ng-class="{collapsed: !node.expanded}"><merch-collection-tree-item  ng-repeat="child in node.children" eventhandler="eventhandler" tree="tree" current-node="currentNode" mode="{{mode}}" has-selection="hasSelection()" node="child" section="{{section}}" ng-animate="animation()"></merch-collection-tree-item></ul>';
            var newElement = angular.element(template);
            $compile(newElement)(scope);
            element.append(newElement);

        }
    };
});

angular.module('merchello.directives').directive('merchCollectionTreePicker', function($q, $timeout, treeService, userService) {
    return {
        restrict: 'E',
        replace: true,
        terminal: false,

        scope: {
            subTreeId : '=',
            entityType: '=',
            mode: '@?',
            hasSelection: '&?'
        },

        compile: function(element, attrs) {

            // makes multiple selection default
            if (!attrs.mode) { attrs.mode = 'multiple'; }

            //config
            var template = '<ul class="umb-tree"><li class="root">';
            template += '<div ng-hide="hideheader" on-right-click="altSelect(tree.root, $event)">' +
                '<h5>' +
                '<i ng-if="enablecheckboxes == \'true\'" ng-class="selectEnabledNodeClass(tree.root)"></i>' +
                '<span class="root-link">{{tree.root.name}}</span></h5>' +
                '</div>';
            template += '<ul>' +
               '<merch-collection-tree-item ng-repeat="child in tree.root.children" eventhandler="eventhandler" node="child" current-node="currentNode" tree="this" mode="{{mode}}" has-selection="hasSelection()" section="{{section}}" ng-animate="animation()"></merch-collection-tree-item>' +
                '</ul>' +
                '</li>' +
                '</ul>';

            element.replaceWith(template);

            return function(scope, elem, attr, controller) {

                var lastSection = "";

                /** Method to load in the tree data */

                function loadTree() {
                    scope.section = 'merchello';
                    if (!scope.loading && scope.section) {
                        scope.loading = true;

                        //default args
                        var args = { section: scope.section, tree: scope.treealias, cacheKey: scope.cachekey, isDialog:  true };
                        treeService.getTree(args)
                            .then(function(data) {
                                //set the data once we have it
                                scope.tree = data;

                                scope.loading = false;

                                //set the root as the current active tree

                                scope.tree.root = _.find(scope.tree.root.children, function(c) {
                                    return c.id === scope.subTreeId;
                                });
                                scope.loadChildren(scope.tree.root, true).then(function(data) {
                                    scope.activeTree = scope.tree.root;
                                    // todo - this is really hacky but we need to remove the dynamic collections since
                                    // they are not valid in this context
                                    var invalidNodes = _.filter(scope.activeTree.children, function(c) {
                                        var found = _.find(c.cssClasses, function(css) {
                                            return css === 'static-collection';
                                        });
                                        return found === undefined;
                                    });
                                    angular.forEach(invalidNodes, function(n) {
                                        treeService.removeNode(n);
                                    });
                                });

                               // emitEvent("treeLoaded", { tree: scope.tree });
                                //emitEvent("treeNodeExpanded", { tree: scope.tree, node: scope.tree.root, children: scope.tree.root.children });

                            }, function(reason) {
                                scope.loading = false;
                                notificationsService.error("Tree Error", reason);
                            });
                    }
                }

                /** syncs the tree, the treeNode can be ANY tree node in the tree that requires syncing */
                function syncTree(treeNode, path, forceReload, activate) {

                    deleteAnimations = false;

                    treeService.syncTree({
                        node: treeNode,
                        path: path,
                        forceReload: forceReload
                    }).then(function (data) {

                        if (activate === undefined || activate === true) {
                            scope.currentNode = data;
                        }

                        //emitEvent("treeSynced", { node: data, activate: activate });
                    });

                }

                /* helper to force reloading children of a tree node */
                scope.loadChildren = function(node, forceReload) {
                    var deferred = $q.defer();

                    //standardising
                    if (!node.children) {
                        node.children = [];
                    }

                    if (forceReload || (node.hasChildren && node.children.length === 0)) {
                        //get the children from the tree service
                        treeService.loadNodeChildren({ node: node, section: scope.section })
                            .then(function(data) {
                                deferred.resolve(data);
                            });
                    }
                    else {
                        node.expanded = true;

                        deferred.resolve(node.children);
                    }

                    return deferred.promise;
                };


                /**
                 Method called when an item is clicked in the tree, this passes the
                 DOM element, the tree node object and the original click
                 and emits it as a treeNodeSelect element if there is a callback object
                 defined on the tree
                 */
                scope.select = function (n, ev) {
                    //on tree select we need to remove the current node -
                    // whoever handles this will need to make sure the correct node is selected
                    //reset current node selection
                    scope.currentNode = null;

                };


                //watch for section changes
                scope.$watch("section", function(newVal, oldVal) {

                    if (!scope.tree) {
                        loadTree();
                    }

                    if (!newVal) {
                        //store the last section loaded
                        lastSection = oldVal;
                    }
                    else if (newVal !== oldVal && newVal !== lastSection) {
                        //only reload the tree data and Dom if the newval is different from the old one
                        // and if the last section loaded is different from the requested one.
                        loadTree();

                        //store the new section to be loaded as the last section
                        //clear any active trees to reset lookups
                        lastSection = newVal;
                    }
                });

                // Loads the tree
                loadTree();
            };
        }
    };
});

/**
 * @ngdoc directive
 * @name customer-address-table
 * @function
 *
 * @description
 * Directive to list customer addresses.
 */
angular.module('merchello.directives').directive('customerAddressTable', function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            customer: '=',
            countries: '=',
            addresses: '=',
            addressType: '@',
            save: '&'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/customer.customeraddresstable.tpl.html',
        controller: function($scope, dialogService, notificationsService, dialogDataFactory, customerAddressDisplayBuilder, CustomerAddressDisplay) {

            // exposed methods
            $scope.openDeleteAddressDialog = openDeleteAddressDialog;
            $scope.openAddressAddEditDialog = openAddressAddEditDialog;

            console.info($scope.addressType);

            /**
             * @ngdoc method
             * @name openAddressEditDialog
             * @function
             *
             * @description
             * Opens the edit address dialog via the Umbraco dialogService.
             */
            function openAddressAddEditDialog(address) {
                var dialogData = dialogDataFactory.createAddEditCustomerAddressDialogData();
                // if the address is not defined we need to create a default (empty) CustomerAddressDisplay
                dialogData.customerAddress = customerAddressDisplayBuilder.createDefault();
                if(address === null || address === undefined) {

                    dialogData.selectedCountry = $scope.countries[0];
                } else {
                    dialogData.customerAddress = angular.extend(dialogData.customerAddress, address); //address;
                    dialogData.selectedCountry = address.countryCode === '' ? $scope.countries[0] :
                        _.find($scope.countries, function(country) {
                            return country.countryCode === address.countryCode;
                        });
                }
                dialogData.countries = $scope.countries;
                dialogData.customerAddress.customerKey = $scope.customer.key;
                if (dialogData.selectedCountry.hasProvinces()) {
                    if(dialogData.customerAddress.region !== '') {
                        dialogData.selectedProvince = _.find(dialogData.selectedCountry.provinces, function(province) {
                            return province.code === address.region;
                        });
                    }
                    if(dialogData.selectedProvince === null || dialogData.selectedProvince === undefined) {
                        dialogData.selectedProvince = dialogData.selectedCountry.provinces[0];
                    }
                }
                // if the customer has not addresses of the given type we are going to force an added
                // address to be the primary address

                dialogData.customerAddress.addressType = $scope.addressType;
                console.info(dialogData);
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/customer.customeraddress.addedit.html',
                    show: true,
                    callback: processAddEditAddressDialog,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name openDeleteAddressDialog
             * @function
             *
             * @description
             * Opens a dialog for deleting an address.
             */
            function openDeleteAddressDialog(address) {
                var dialogData = dialogDataFactory.createDeleteCustomerAddressDialogData();
                dialogData.customerAddress = address;
                dialogData.name = address.label;
                dialogService.open({
                    template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                    show: true,
                    callback: processDeleteAddress,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name processDeleteAddress
             * @function
             *
             * @description
             * Save the customer with a deleted address.
             */
            function processDeleteAddress(dialogData) {
                //console.info(dialogData);
                $scope.customer.addresses = _.reject($scope.customer.addresses, function(address) {
                    return address.key === dialogData.customerAddress.key;
                });
                save();
            }

            function processAddEditAddressDialog(dialogData) {
                var defaultAddressOfType = $scope.customer.getDefaultAddress(dialogData.customerAddress.addressType);
                if(dialogData.customerAddress.key !== '') {
                    $scope.customer.addresses = _.reject($scope.customer.addresses, function(address) {
                        return address.key == dialogData.customerAddress.key;
                    });
                }
                if (dialogData.customerAddress.isDefault && defaultAddressOfType !== undefined) {
                    if(dialogData.customerAddress.key !== defaultAddressOfType.key) {
                        defaultAddressOfType.isDefault = false;
                    }
                }
                $scope.customer.addresses.push(dialogData.customerAddress);
                console.info($scope.customer);
                save();
            }

            /**
             * @ngdoc method
             * @name save
             * @function
             *
             * @description
             * Saves the customer.
             */
            function save() {
                $scope.save();
            }

        }
    };
});


    /**
     * @ngdoc directive
     * @name customer-location
     * @function
     *
     * @description
     * Directive to display a customer location.
     */
    angular.module('merchello.directives').directive('customerLocation',
        [function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                address: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/customer.customerlocation.tpl.html'
        };
    }]);

angular.module('merchello.directives').directive('detachedContentType', function() {

    return {
        restrict: 'E',
        replace: true,
        terminal: false,

        scope: {
            entityType: '@'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/detachedcontenttype.list.tpl.html',
        controller: 'Merchello.Directives.DetachedContentTypeListController'
    };

});

angular.module('merchello.directives').directive('detachedContentTypeSelect',
        function(detachedContentResource, localizationService, detachedContentTypeDisplayBuilder) {
        return {
            restrict: 'E',
            replace: true,
            terminal: false,

            scope: {
                entityType: '@',
                selectedContentType: '=',
                save: '&'
            },
            template:         '<div class="detached-content-select">' +
            '<div data-ng-show="detachedContentTypes.length > 0">' +
            '<label><localize key="merchelloDetachedContent_productContentTypes" /></label>' +
            '<select data-ng-model="selectedContentType" data-ng-options="ct.name for ct in detachedContentTypes track by ct.key" data-ng-show="loaded">' +
            '<option value="">{{ noSelection }}</option>' +
            '</select>' +
            ' <merchello-save-icon show-save="true" do-save="save()"></merchello-save-icon>' +
            '</div>' +
                '<div data-ng-hide="detachedContentTypes.length > 0 && loaded" style="text-align: center">' +
                '<localize key="merchelloDetachedContent_noDetachedContentTypes" />' +
                '</div>' +
            '</div>',
            link: function(scope, elm, attr) {

                scope.loaded = false;
                scope.detachedContentTypes = [];
                scope.noSelection = '';

                function init() {
                    localizationService.localize('merchelloDetachedContent_selectContentType').then(function(value) {
                        scope.noSelection = value;
                        loadDetachedContentTypes();
                    });
                }

                function loadDetachedContentTypes() {
                    detachedContentResource.getDetachedContentTypeByEntityType(scope.entityType).then(function(results) {
                        scope.detachedContentTypes = detachedContentTypeDisplayBuilder.transform(results);
                        scope.loaded = true;
                    });
                }

                // initialize the directive
                init();
            }
        };

});

angular.module('merchello.directives').directive('comparisonOperatorRadioButtons', function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            operator: '='
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/comparisonOperatorRadioButtons.tpl.html',
        controller: function($scope) {

            function init() {
                if($scope.operator === undefined) {
                    $scope.operator = 'gt';
                }
            }

            init();
        }
    };
});

angular.module('merchello.directives').directive('contentTypeDropDown',
    function(localizationService, detachedContentResource, umbContentTypeDisplayBuilder) {
    return {
        restrict: "E",
        replace: true,
        scope: {
            selectedContentType: '=',
        },
        template:
        '<div class="control-group">' +
        '<label><localize key="merchelloDetachedContent_productContentTypes" /></label>' +
        '<select class="span11" data-ng-model="selectedContentType" data-ng-options="ct.name for ct in contentTypes track by ct.key" data-ng-show="loaded">' +
            '<option value="">{{ noSelection }}</option>' +
        '</select>' +
        '</div>',
        link: function (scope, element, attrs, ctrl) {

            scope.loaded = false;
            scope.contentTypes = [];
            scope.noSelection = '';

            function init() {
                localizationService.localize('merchelloDetachedContent_selectContentType').then(function(value) {
                    scope.noSelection = value;
                    loadContentTypes();
                });
            }

            function loadContentTypes() {
                detachedContentResource.getContentTypes().then(function(results) {
                    scope.contentTypes = umbContentTypeDisplayBuilder.transform(results);
                    scope.loaded = true;
                });
            }
            init();
        }
    };
});

    /**
     * @ngdoc directive
     * @name filter-by-date-range
     * @function
     *
     * @description
     * Directive to wrap all Merchello Mark up.
     */
    angular.module('merchello.directives').directive('filterByDateRange', function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                hideDatesLabel: '=',
                filterStartDate: '=',
                filterEndDate: '=',
                filterButtonText: '@filterButtonText',
                filterWithDates: '&',
                hideFilterButton: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/filterbydaterange.tpl.html',
            controller: function($scope, $element, $q, assetsService, angularHelper, notificationsService, settingsResource, settingDisplayBuilder) {

                $scope.settings = {};

                // exposed methods
                $scope.changeDateFilters = changeDateFilters;

                function init() {
                    var promises = loadAssets();
                    promises.push(loadSettings());

                    $q.all(promises).then(function() {
                       // $scope.filterStartDate = moment(new Date().setMonth(new Date().getMonth()-1)).format($scope.settings.dateFormat.toUpperCase());
                       // $scope.filterEndDate = moment(new Date()).format($scope.settings.dateFormat.toUpperCase());
                    });
                }

                /**
                 * @ngdoc method
                 * @name loadAssets
                 * @function
                 *
                 * @description - Loads needed and js stylesheets for the view.
                 */
                function loadAssets() {
                    var promises = [];
                    var cssPromise = assetsService.loadCss('lib/datetimepicker/bootstrap-datetimepicker.min.css');
                    var jsPromise = assetsService.load(['lib/moment/moment-with-locales.js', 'lib/datetimepicker/bootstrap-datetimepicker.js']);

                    promises.push(cssPromise);
                    promises.push(jsPromise);

                    //The Datepicker js and css files are available and all components are ready to use.
                    $q.all(promises).then(function() {
                        setupDatePicker("#filterStartDate");
                        $element.find("#filterStartDate").datetimepicker().on("changeDate", applyDateStart);

                        setupDatePicker("#filterEndDate");
                        $element.find("#filterEndDate").datetimepicker().on("changeDate", applyDateEnd);
                    });

                    return promises;
                }

                function loadSettings() {
                    var promise = settingsResource.getAllSettings();
                    return promise.then(function(allSettings) {
                        $scope.settings = settingDisplayBuilder.transform(allSettings);
                    }, function(reason) {
                        notificationsService.error('Failed to load settings', reason.message);
                    });
                }

                /**
                 * @ngdoc method
                 * @name setupDatePicker
                 * @function
                 *
                 * @description
                 * Sets up the datepickers
                 */
                function setupDatePicker(pickerId) {

                    // Open the datepicker and add a changeDate eventlistener
                    $element.find(pickerId).datetimepicker({
                        format: $scope.settings.dateFormat
                    });

                    //Ensure to remove the event handler when this instance is destroyted
                    $scope.$on('$destroy', function () {
                        $element.find(pickerId).datetimepicker("destroy");
                    });
                }

                /*-------------------------------------------------------------------
                 * Event Handler Methods
                 *-------------------------------------------------------------------*/

                /**
                 * @ngdoc method
                 * @name changeDateFilters
                 * @function
                 *
                 * @param {string} start - String representation of start date.
                 * @param {string} end - String representation of end date.
                 * @description - Change the date filters, then trigger new API call to load the reports.
                 */
                function changeDateFilters(start, end) {
                    $scope.filterStartDate = start;
                    $scope.filterEndDate = end;
                    $scope.currentPage = 0;
                    $scope.filterWithDates();
                }

                /*-------------------------------------------------------------------
                 * Helper Methods
                 * ------------------------------------------------------------------*/

                //handles the date changing via the api
                function applyDateStart(e) {
                    angularHelper.safeApply($scope, function () {
                        // when a date is changed, update the model
                        if (e.localDate) {
                            $scope.filterStartDate = moment(e.localDate).format($scope.settings.dateFormat.toUpperCase());
                        }
                    });
                }

                //handles the date changing via the api
                function applyDateEnd(e) {
                    angularHelper.safeApply($scope, function () {
                        // when a date is changed, update the model
                        if (e.localDate) {
                            $scope.filterEndDate = moment(e.localDate).format($scope.settings.dateFormat.toUpperCase());
                        }
                    });
                }

                // Initialize the controller
                init();
            },
            compile: function (element, attrs) {
                if (!attrs.filterButtonText) {
                    attrs.filterButtonText = 'Filter';
                }
            }
        };
    });

    /**
     * @ngdoc directive
     * @name merchello-panel
     * @function
     *
     * @description
     * Directive to wrap all Merchello Mark up and provide common classes.
     */
     angular.module('merchello.directives').directive('merchelloPanel', function() {
         return {
             restrict: 'E',
             replace: true,
             transclude: 'true',
             templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/html/merchellopanel.tpl.html'
         };
     });

    /**
     * @ngdoc directive
     * @name merchello-slide-open-panel
     * @function
     *
     * @description
     * Directive to allow a section of content to slide open/closed based on a boolean value
     */
    angular.module('merchello.directives').directive('merchelloSlideOpenPanel', function() {
        return {
            restrict: 'E',
            replace: true,
            transclude: 'true',
            scope: {
                isOpen: '=',
                classes: '=?',
                hideClose: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/html/merchelloslidepanelopen.tpl.html',
            link: function ($scope, $element, attrs) {

                if ($scope.classes == undefined) {
                    $scope.classes = 'control-group umb-control-group';
                }


            }
        };
    });

/**
 * @ngdoc directive
 * @name merchello-panel
 * @function
 *
 * @description
 * Directive to wrap all Merchello Mark up and provide common classes.
 */
angular.module('merchello.directives').directive('merchelloSpinner', function() {
    return {
        restrict: 'E',
        replace: true,
        transclude: 'true',
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/html/merchellospinner.tpl.html'
    };
});

angular.module('merchello.directives').directive('merchelloTabs', [function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            tabs: '='
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/html/merchellotabs.tpl.html'
    };
}]);

/**
 * @ngdoc directive
 * @name address directive
 * @function
 *
 * @description
 * Directive to maintain a consistent format for displaying addresses
 */
angular.module('merchello.directives').directive('merchelloAddress', function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                address: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/merchelloaddress.tpl.html'
        };
    }).directive('merchelloAddress', function() {
        return {
            restrict: 'A',
            transclude: true,
            scope: {
                setAddress: '&setAddress'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/merchelloaddress.tpl.html',
            link: function(scope, elm, attr) {
                scope.address = scope.setAddress();
            }
        }
    });

angular.module('merchello.directives').directive('merchelloIconBar', function(localizationService) {

    return {
        restrict: 'E',
        replace: true,
        scope: {
            showAdd: '=?',
            showEdit: '=?',
            showActivate: '=?',
            showDelete: '=?',
            doAdd: '&?',
            doEdit: '&?',
            doActivate: '&?',
            doDelete: '&?',
            args: '=?'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/merchelloiconbar.tpl.html',
        link: function(scope, elm, attr) {
            scope.editTitle = '';
            scope.deleteTitle = '';
            scope.activateTitle = '';
            scope.addTitle = '';

            localizationService.localize('general_add').then(function(value) {
              scope.addTitle = value;
            });
            localizationService.localize('general_edit').then(function(value) {
                scope.editTitle = value;
            });
            localizationService.localize('general_delete').then(function(value) {
                scope.deleteTitle = value;
            });
            localizationService.localize('merchelloGatewayProvider_activate').then(function(value) {
                scope.activateTitle = value;
            });
        }
    };

});

// a save icon
angular.module('merchello.directives').directive('merchelloSaveIcon', function(localizationService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            showSave: '=',
            doSave: '&',
        },
        template: '<span class="merchello-icons">' +
        '<a class="merchello-icon merchello-icon-provinces" data-ng-show="showSave" ng-click="doSave()" title="{{title}}" prevent-default>' +
        '<i class="icon icon-save"></i>' +
        '</a></span>',
        link: function(scope, elm, attr) {
            scope.title = '';
            localizationService.localize('buttons_save').then(function(value) {
                scope.title = value;
            });
        }
    }
});

// the add icon
angular.module('merchello.directives').directive('merchelloAddIcon', function(localizationService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            doAdd: '&',
        },
        template: '<span class="merchello-icons">' +
        '<a class="merchello-icon merchello-icon-add" ng-click="doAdd()" title="{{title}}" prevent-default>' +
        '<i class="icon icon-add"></i>' +
        '</a></span>',
        link: function(scope, elm, attr) {
            scope.title = '';
            localizationService.localize('general_add').then(function(value) {
                scope.title = value;
            });
        }
    }
});

// the edit icon
angular.module('merchello.directives').directive('merchelloEditIcon', function(localizationService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            doEdit: '&',
        },
        template: '<span class="merchello-icons">' +
           '<a class="merchello-icon merchello-icon-edit" ng-click="doEdit()" title="{{title}}" prevent-default>' +
            '<i class="icon icon-edit"></i>' +
            '</a></span>',
        link: function(scope, elm, attr) {
            scope.title = '';
            localizationService.localize('general_edit').then(function(value) {
                scope.title = value;
            });
        }
    }
});

// the delete icon
angular.module('merchello.directives').directive('merchelloDeleteIcon', function(localizationService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            doDelete: '&',
        },
        template: '<span class="merchello-icons">' +
        '<a class="merchello-icon merchello-icon-delete" ng-click="doDelete()" title="{{title}}" prevent-default>' +
        '<i class="icon icon-trash"></i>' +
        '</a></span>',
        link: function(scope, elm, attr) {
            scope.title = '';
            localizationService.localize('general_edit').then(function(value) {
                scope.title = value;
            });
        }
    }
});

// the provinces icon
angular.module('merchello.directives').directive('merchelloProvincesIcon', function(localizationService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            showProvinces: '=',
            doProvinces: '&',
        },
        template: '<span class="merchello-icons">' +
        '<a class="merchello-icon merchello-icon-provinces" data-ng-show="showProvinces" ng-click="doProvinces()" title="{{title}}" prevent-default>' +
        '<i class="icon icon-globe-alt"></i>' +
        '</a></span>',
        link: function(scope, elm, attr) {
            scope.title = '';
            localizationService.localize('merchelloShippingMethod_adjustIndividualRegions').then(function(value) {
                scope.title = value;
            });
        }
    }
});



angular.module('merchello.directives').directive('merchelloListView',
    ['$routeParams', '$log', '$filter', 'dialogService', 'localizationService', 'merchelloListViewHelper', 'queryDisplayBuilder', 'queryResultDisplayBuilder',
    function($routeParams, $log, $filter, dialogService, localizationService, merchelloListViewHelper, queryDisplayBuilder, queryResultDisplayBuilder) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                builder: '=',
                entityType: '=',
                getColumnValue: '&',
                load: '&',
                ready: '=?',
                disableCollections: '@?',
                includeDateFilter: '@?',
                noTitle: '@?',
                noFilter: '@?'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/merchellolistview.tpl.html',
            link: function (scope, elm, attr) {

                scope.collectionKey = '';
                scope.sort = sort;
                scope.isSortDirection = isSortDirection;
                scope.next = next;
                scope.prev = prev;
                scope.goToPage = goToPage;
                scope.enterSearch = enterSearch;
                scope.search = search;
                scope.setPageSize = setPageSize;
                scope.openDateRangeDialog = openDateRangeDialog;

                scope.hasFilter = true;
                scope.hasCollections = true;
                scope.enableDateFilter = false;
                scope.showTitle = true;
                scope.isReady = false;

                // date filtering
                scope.clearDates = clearDates;
                scope.startDate = '';
                scope.endDate = '';
                scope.dateBtnText = ''
                var allDates = '';

                scope.config = merchelloListViewHelper.getConfig(scope.entityType);

                //scope.goToEditor = goToEditor;

                scope.listViewResultSet = {
                    totalItems: 0,
                    items: []
                };

                scope.options = {
                    pageSize: scope.config.pageSize ? scope.config.pageSize : 10,
                    pageNumber: 1,
                    filter: '',
                    orderBy: (scope.config.orderBy ? scope.config.orderBy : 'name').trim(),
                    orderDirection: scope.config.orderDirection ? scope.config.orderDirection.trim() : "asc"
                };

                scope.pagination = [];

                function init() {
                    if (!('ready' in attr)) {
                        scope.isReady = true;
                    }
                    scope.hasCollections = !('disableCollections' in attr);
                    scope.enableDateFilter = 'includeDateFilter' in attr;
                    scope.hasFilter = !('noFilter' in attr);
                    scope.showTitle = !('noTitle' in attr);
                    if(scope.hasCollections) {
                        scope.collectionKey = $routeParams.id !== 'manage' ? $routeParams.id : '';
                        // none of the collections have the capability to filter by dates
                        if (scope.collectionKey !== '' && scope.enableDateFilter) {
                            scope.enableDateFilter = false;
                        }
                    }
                    localizationService.localize('merchelloGeneral_allDates').then(function(value) {
                        allDates = value;
                        scope.dateBtnText = allDates;
                    });

                    scope.$watch('ready', function(newVal, oldVal) {
                        if (newVal === true) {
                            scope.isReady = newVal;
                        }
                          if(scope.isReady) {
                              search();
                          }
                    });

                }

                function search() {
                    var page = scope.options.pageNumber - 1;
                    var perPage = scope.options.pageSize;
                    var sortBy = scope.options.orderBy;
                    var sortDirection = scope.options.orderDirection === 'asc' ? 'Ascending' : 'Descending';

                    var query = queryDisplayBuilder.createDefault();
                    query.currentPage = page;
                    query.itemsPerPage = perPage;
                    query.sortBy = sortBy;
                    query.sortDirection = sortDirection;
                    query.addFilterTermParam(scope.options.filter);

                    if (scope.collectionKey !== '') {
                        query.addCollectionKeyParam(scope.collectionKey);
                        query.addEntityTypeParam(scope.entityType);
                    }

                    if (scope.enableDateFilter && scope.startDate !== '' && scope.endDate !== '') {
                        // just to be safe
                        var start = $filter('date')(scope.startDate, 'yyyy-MM-dd');
                        var end = $filter('date')(scope.endDate, 'yyyy-MM-dd');
                        query.addInvoiceDateParam(start, 'start');
                        query.addInvoiceDateParam(end, 'end');
                        console.info('hello');
                        scope.dateBtnText = scope.startDate + ' - ' + scope.endDate;
                    }

                    scope.load()(query).then(function (response) {
                        var queryResult = queryResultDisplayBuilder.transform(response, scope.builder);
                        scope.listViewResultSet.items = queryResult.items;
                        scope.listViewResultSet.totalItems = queryResult.totalItems;
                        scope.listViewResultSet.totalPages = queryResult.totalPages;


                        scope.pagination = [];

                        //list 10 pages as per normal
                        if (scope.listViewResultSet.totalPages <= 10) {
                            for (var i = 0; i < scope.listViewResultSet.totalPages; i++) {
                                scope.pagination.push({
                                    val: (i + 1),
                                    isActive: scope.options.pageNumber == (i + 1)
                                });
                            }
                        }
                        else {
                            //if there is more than 10 pages, we need to do some fancy bits

                            //get the max index to start
                            var maxIndex = scope.listViewResultSet.totalPages - 10;
                            //set the start, but it can't be below zero
                            var start = Math.max(scope.options.pageNumber - 5, 0);
                            //ensure that it's not too far either
                            start = Math.min(maxIndex, start);

                            for (var i = start; i < (10 + start) ; i++) {
                                scope.pagination.push({
                                    val: (i + 1),
                                    isActive: scope.options.pageNumber == (i + 1)
                                });
                            }

                            //now, if the start is greater than 0 then '1' will not be displayed, so do the elipses thing
                            if (start > 0) {
                                scope.pagination.unshift({ name: "First", val: 1, isActive: false }, {val: "...",isActive: false});
                            }

                            //same for the end
                            if (start < maxIndex) {
                                scope.pagination.push({ val: "...", isActive: false }, { name: "Last", val: scope.listViewResultSet.totalPages, isActive: false });
                            }
                        }

                        scope.preValuesLoaded = true;
                    }, function(reason) {
                        notificationsService.success("Entity Load Failed:", reason.message);
                    });
                }

                function sort(field, allow) {
                    if (allow) {
                        scope.options.orderBy = field;

                        if (scope.options.orderDirection === "desc") {
                            scope.options.orderDirection = "asc";
                        }
                        else {
                            scope.options.orderDirection = "desc";
                        }
                        search();
                    }
                };

                function next () {
                    if (scope.options.pageNumber < scope.listViewResultSet.totalPages) {
                        scope.options.pageNumber++;
                        search();
                    }
                };

                function goToPage(pageNumber) {
                    scope.options.pageNumber = pageNumber + 1;
                    search();
                }

                function prev() {
                    if (scope.options.pageNumber - 1 > 0) {
                        scope.options.pageNumber--;
                        search();
                    }
                }

                function enterSearch($event) {
                    $($event.target).next().focus();
                }

                function setPageSize() {
                    scope.options.pageNumber = 1;
                    search();
                }

                function isSortDirection(col, direction) {
                    return scope.options.orderBy.toUpperCase() == col.toUpperCase() && scope.options.orderDirection == direction;
                }

                function clearDates() {
                    scope.startDate = '';
                    scope.endDate = '';
                    scope.dateBtnText = allDates;
                    search();
                }

                function openDateRangeDialog() {
                    var dialogData = {
                        startDate: scope.startDate,
                        endDate: scope.endDate
                    };

                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/daterange.selection.html',
                        show: true,
                        callback: processDateRange,
                        dialogData: dialogData
                    });
                }

                function processDateRange(dialogData) {
                    scope.startDate = dialogData.startDate;
                    scope.endDate = dialogData.endDate;
                    console.info('got here');
                    search();
                }

                init();
            }
        }
}]);

    /**
     * @ngdoc directive
     * @name MerchelloPagerDirective
     * @function
     *
     * @description
     * directive to display display a pager for orders, products, and others.
     *
     * TODO: Currently, makes assumptions using the parent scope.  In future, make this work as an isolate scope.
     */
    angular.module('merchello.directives').directive('merchelloPager', function() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/merchellopager.tpl.html'
        };
    });
    /**
     * @ngdoc directive
     * @name resetListfilters
     * @function
     *
     * @description
     * directive to clear list filters.
     *
     * TODO: Currently, makes assumptions using the parent scope.  In future, make this work as an isolate scope.
     */
    angular.module('merchello.directives').directive('resetListFilters', [function() {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/resetlistfilters.tpl.html'
        };
    }]);

    /**
     * @ngdoc directive
     * @name tagsManager
     * @function
     *
     * @description
     * directive for a tags manager.
     */
    angular.module('merchello.directives').directive('tagManager', function() {
        return {
            restrict: 'E',
            scope: { option: '=' },
            template:
            '<div class="tags">' +
            '<a ng-repeat="(idx, choice) in option.choices" class="tag" ng-click="remove(idx)">{{choice.name}}</a>' +
            '</div>' +
            '<input type="text" placeholder="Add a choice..." ng-model="newChoiceName" /> ' +
            '<merchello-add-icon do-add="add()"></merchello-add-icon>',
            link: function ($scope, $element) {
                // FIXME: this is lazy and error-prone
                // this is the option name input
                var input = angular.element($element.children()[1]);

                // This adds the new tag to the tags array
                $scope.add = function () {
                    if ($scope.newChoiceName.length > 0) {
                        $scope.option.addAttributeChoice($scope.newChoiceName);
                        $scope.newChoiceName = "";
                    }
                };

                // This is the ng-click handler to remove an item
                $scope.remove = function (idx) {
                    $scope.option.removeAttributeChoice(idx);
                };

                // Capture all keypresses
                input.bind('keypress', function (event) {
                    // But we only care when Enter was pressed
                    if (event.keyCode == 13) {
                        // There's probably a better way to handle this...
                        $scope.add();
                    }
                });

            }
        };
    });


    angular.module('merchello.directives').directive('notificationMethods', function($location) {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/notificationmethods.tpl.html',
            controller: function($scope) {

                // Exposed monitors
                $scope.getMonitorName = getMonitorName;

                $scope.redirectForEdit = function(key) {
                    $location.url('/merchello/merchello/notification.messageeditor/' + key, true);
                }

                function getMonitorName(key) {
                    var monitor = _.find($scope.notificationMonitors, function(monitor) {
                        return monitor.monitorKey === key;
                    });
                    if(monitor !== null || monitory !== undefined) {
                        return monitor.name;
                    } else {
                        return 'Not found';
                    }
                }
            }
        };
    });

angular.module('merchello.directives').directive('resolvedGatewayProviders', [function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            providerList: '=',
            'activate': '&onActivate',
            'deactivate': '&onDeactivate',
            'configure': '&onConfigure'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/resolvedgatewayproviders.tpl.html'
    };
}]);

angular.module('merchello.directives').directive('shipCountryGatewayProviders', function() {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            country: '=',
            reload: '&',
            delete: '&'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/shipcountrygatewayproviders.tpl.html',
        controller: 'Merchello.Directives.ShipCountryGatewaysProviderDirectiveController'
    };
});
    /**
     * @ngdoc controller
     * @name productOptionsManage
     * @function
     *
     * @description
     * The productOptionsManage directive
     */
    angular.module('merchello.directives').directive('productOptionsManage', function() {

        return {
            restrict: 'E',
            replace: true,
            scope: {
                product: '=',
                parentForm: '=',
                classes: '='
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/product.optionsmanage.tpl.html',

            controller: function ($scope) {
                $scope.rebuildVariants = false;
                $scope.addOption = addOption;
                $scope.removeOption = removeOption;

                /**
                 * @ngdoc method
                 * @name addOption
                 * @function
                 *
                 * @description
                 * Called when the Add Option button is pressed.  Creates a new option ready to fill out.
                 */
                function addOption() {
                    $scope.product.addEmptyOption();
                }

                /**
                 * @ngdoc method
                 * @name removeOption
                 * @function
                 *
                 * @description
                 * Called when the Trash can icon button is pressed next to an option. Removes the option from the product.
                 */
                function removeOption (option) {
                    $scope.product.removeOption(option);
                }
            }
        };

    });

/**
 * @ngdoc controller
 * @name productVariantsViewTable
 * @function
 *
 * @description
 * The productVariantsViewTable directive
 */
angular.module('merchello.directives').directive('productVariantsViewTable', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            product: '=',
            currencySymbol: '=',
            reload: '&'
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/product.productvariantsviewtable.tpl.html',
        controller: 'Merchello.Directives.ProductVariantsViewTableDirectiveController'
    };
});


    angular.module('merchello.directives').directive('productReorderOptions', [function() {

        return {
            restrict: 'E',
            replace: true,
            scope: {
                isolateIsOpen: '=isOpen',
                product: '=',
                reload: '&'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/product.reorderoptions.tpl.html',
            link: function ($scope, $element) {
                /**
                 * @ngdoc method
                 * @name close
                 * @function
                 *
                 * @description
                 * Set the isOpen scope property to false to close the dialog
                 */
                $scope.close = function ($event) {
                    $scope.isolateIsOpen = false;
                };

                // Settings for the sortable directive
                $scope.sortableOptions = {
                    stop: function (e, ui) {
                        for (var i = 0; i < $scope.product.productOptions.length; i++) {
                            $scope.product.productOptions[i].sortOrder(i + 1);
                        }
                        $scope.product.fixAttributeSortOrders();
                    },
                    axis: 'y',
                    cursor: "move"
                };

                $scope.sortableChoices = {
                    start: function (e, ui) {
                        $(e.target).data("ui-sortable").floating = true;    // fix for jQui horizontal sorting issue https://github.com/angular-ui/ui-sortable/issues/19
                    },
                    stop: function (e, ui) {
                        var attr = ui.item.scope().attribute;
                        var attrOption = _.find($scope.product.productOptions, function(po) { return po.key === attr.optionKey; });
                        attrOption.resetChoiceSortOrders();
                    },
                    update: function (e, ui) {
                        var attr = ui.item.scope().attribute;
                        var attrOption = _.find($scope.product.productOptions, function(po) { return po.key === attr.optionKey; });
                        attrOption.resetChoiceSortOrders();
                    },
                    cursor: "move"
                };
            }
        };
    }]);

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

    /**
     * @ngdoc controller
     * @name productVariantMainProperties
     * @function
     *
     * @description
     * The productVariantMainProperties directive
     */
    angular.module('merchello.directives').directive('productVariantMainProperties',
        [function() {

            return {
                restrict: 'E',
                replace: true,
                scope: {
                    product: '=',
                    productVariant: '=',
                    context: '=',
                    settings: '='
                },
                templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/productvariant.mainproperties.tpl.html',
                controller: function ($scope, warehouseResource, warehouseDisplayBuilder, catalogInventoryDisplayBuilder) {

                    // Get the default warehouse for the ensureCatalogInventory() function below
                    $scope.defaultWarehouse = {};
                    $scope.defaultWarehouseCatalog = {};

                    function init() {
                        var promiseWarehouse = warehouseResource.getDefaultWarehouse();
                        promiseWarehouse.then(function (warehouse) {
                            $scope.defaultWarehouse = warehouseDisplayBuilder.transform(warehouse);
                            $scope.defaultWarehouseCatalog = _.find($scope.defaultWarehouse.warehouseCatalogs, function (dwc) { return dwc.isDefault; });
                            // set defaults in case of a createproduct
                            if($scope.context === 'createproduct') {
                                $scope.productVariant.shippable = $scope.settings.globalShippable;
                                $scope.productVariant.taxable = $scope.settings.globalTaxable;
                                $scope.productVariant.trackInventory = $scope.settings.globalTrackInventory;
                                if($scope.productVariant.shippable || $scope.productVariant.trackInventory)
                                {
                                    $scope.productVariant.ensureCatalogInventory($scope.defaultWarehouseCatalog.key);
                                }
                            }
                        });
                    }

                    // Initialize the controller
                    init();
                }
            };
    }]);

    /**
     * @ngdoc controller
     * @name productVariantShipping
     * @function
     *
     * @description
     * The productVariantShipping directive
     */
    angular.module('merchello.directives').directive('productVariantShipping', function() {

        return {
            restrict: 'E',
            replace: true,
            scope: {
                product: '=',
                productVariant: '=',
                settings: '=',
                context: '@'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/productvariant.shipping.tpl.html',
            controller: 'Merchello.Directives.ProductVariantShippingDirectiveController'
        };

    });

/**
 * @ngdoc directive
 * @name merchello-slide-open-panel
 * @function
 *
 * @description
 * Directive to allow a section of content to slide open/closed based on a boolean value
 */
angular.module('merchello.directives').directive('addPaymentTable', function() {
    return {
        restrict: 'E',
        replace: true,
        transclude: 'true',
        scope: {
            isOpen: '=',
            currencySymbol: '=',
            reload: '&',
            toggleOpen: '&',
            showSpinner: '&',
            invoice: '=',
            payments: '=',
            paymentMethods: '=',
        },
        templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/addpaymenttable.tpl.html',
        controller: function($scope, $timeout, notificationsService, dialogService, dialogDataFactory, paymentResource) {
            $scope.loaded = false;
            $scope.authorizePaymentOnly = false;

            // exposed methods
            $scope.openAddPaymentDialog = openAddPaymentDialog;
            $scope.filterPaymentMethods = filterPaymentMethods;

            /**
             * @ngdoc method
             * @name init
             * @function
             *
             * @description - Initializes the controller
             */
            function init() {
                $scope.$watch('paymentMethods', function(methods) {
                  if (methods.length > 0) {
                      $scope.$watch('invoice', function(inv) {
                          if (inv.key !== null && inv.key !== undefined) {
                              $scope.loaded = true;
                          }
                      });
                  }
                });
            }

            /**
             * @ngdoc method
             * @name filterPaymentMethods
             * @function
             *
             * @description - Filters payment methods for methods that offer authorize / authorize capture dialogs
             */
            function filterPaymentMethods() {
                var methods = [];
                if (!$scope.loaded) {
                    return methods;
                }
                if ($scope.authorizePaymentOnly) {
                    methods = _.filter($scope.paymentMethods, function(auth) { return auth.authorizePaymentEditorView.editorView !== ''; });
                } else {
                    methods = _.filter($scope.paymentMethods, function(capture) { return capture.authorizeCapturePaymentEditorView.editorView !== ''; });
                }
                if ($scope.invoice.isAnonymous()) {
                    methods = _.filter(methods, function(m) { return !m.requiresCustomer; })
                }
                return methods;
            }

            /**
             * @ngdoc method
             * @name openAddPaymentDialog
             * @function
             *
             * @description - Opens a dialog to authorize and/or capture a new payment
             */
            function openAddPaymentDialog(paymentMethod) {

                var dialogData = dialogDataFactory.createAddPaymentDialogData();
                dialogData.showSpinner = $scope.showSpinner;
                dialogData.paymentMethod = paymentMethod;
                dialogData.paymentMethodName = paymentMethod.name;
                dialogData.invoiceBalance = $scope.invoice.remainingBalance($scope.payments);
                dialogData.currencySymbol = $scope.currencySymbol;
                dialogData.invoice = $scope.invoice;
                dialogData.authorizePaymentOnly = $scope.authorizePaymentOnly;
                var dialogView = $scope.authorizePaymentOnly ? paymentMethod.authorizePaymentEditorView.editorView : paymentMethod.authorizeCapturePaymentEditorView.editorView;

                dialogService.open({
                    template: dialogView,
                    show: true,
                    callback: addPaymentDialogConfirm,
                    dialogData: dialogData
                });
            }

            /**
             * @ngdoc method
             * @name addPaymentDialogConfirm
             * @function
             *
             * @description - Authorizes and/or captures a new payment
             */
            function addPaymentDialogConfirm(dialogData) {
                $scope.showSpinner();
                var paymentRequest = dialogData.asPaymentRequestDisplay();
                var promise;
                var note =  ' Authorize/Capture ';
                if (dialogData.authorizePaymentOnly) {
                    promise = paymentResource.authorizePayment(paymentRequest);
                    name = ' Authorize ';
                } else {
                    promise = paymentResource.authorizeCapturePayment(paymentRequest);
                }
                promise.then(function (payment) {
                    // added a timeout here to give the examine index
                    $timeout(function() {
                        notificationsService.success('Payment ' + note + 'success');
                        reload()
                    }, 400);
                }, function (reason) {
                    notificationsService.error('Payment ' + note + 'Failed', reason.message);
                });
            }

            function reload() {
                $scope.toggleOpen();
                $scope.reload();
            }

            // initialize the controller
            init();
        }
    };
});

    /**
     * @ngdoc directive
     * @name filter-by-date-range
     * @function
     *
     * @description
     * Directive to wrap all Merchello Mark up.
     */
    angular.module('merchello.directives').directive('filterInvoices', function() {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                filterStartDate: '=',
                filterEndDate: '=',
                filterText: '=',
                showDateFilter: '=',
                filterButtonText: '@filterButtonText',
                dateFilterOpen: '=',
                filterCallback: '&',
                filterTermCallback: '&',
                toggleDateFilterOpen: '&'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/filterinvoices.tpl.html',
            controller: function($scope, $element, $q, assetsService, angularHelper, notificationsService, settingsResource, settingDisplayBuilder) {

                $scope.settings = {};

                // exposed methods
                $scope.changeDateFilters = changeDateFilters;
                $scope.changeTermFilter = changeTermFilter;

                function init() {
                    var promises = loadAssets();
                    promises.push(loadSettings());

                    $q.all(promises).then(function() {
                        $scope.filterStartDate = moment(new Date().setMonth(new Date().getMonth() - 1)).format($scope.settings.dateFormat.toUpperCase());
                        $scope.filterEndDate = moment(new Date()).format($scope.settings.dateFormat.toUpperCase());
                    });
                }

                /**
                 * @ngdoc method
                 * @name loadAssets
                 * @function
                 *
                 * @description - Loads needed and js stylesheets for the view.
                 */
                function loadAssets() {
                    var promises = [];
                    var cssPromise = assetsService.loadCss('lib/datetimepicker/bootstrap-datetimepicker.min.css');
                    var jsPromise = assetsService.load(['lib/moment/moment-with-locales.js', 'lib/datetimepicker/bootstrap-datetimepicker.js']);

                    promises.push(cssPromise);
                    promises.push(jsPromise);

                    //The Datepicker js and css files are available and all components are ready to use.
                    $q.all(promises).then(function() {
                        setupDatePicker("#filterStartDate");
                        $element.find("#filterStartDate").datetimepicker().on("changeDate", applyDateStart);

                        setupDatePicker("#filterEndDate");
                        $element.find("#filterEndDate").datetimepicker().on("changeDate", applyDateEnd);
                    });

                    return promises;
                }

                function loadSettings() {
                    var promise = settingsResource.getAllSettings();
                    return promise.then(function(allSettings) {
                        $scope.settings = settingDisplayBuilder.transform(allSettings);
                    }, function(reason) {
                        notificationsService.error('Failed to load settings', reason.message);
                    });
                }

                /**
                 * @ngdoc method
                 * @name setupDatePicker
                 * @function
                 *
                 * @description
                 * Sets up the datepickers
                 */
                function setupDatePicker(pickerId) {

                    // Open the datepicker and add a changeDate eventlistener
                    $element.find(pickerId).datetimepicker({
                        format: $scope.settings.dateFormat
                    });

                    //Ensure to remove the event handler when this instance is destroyted
                    $scope.$on('$destroy', function () {
                        $element.find(pickerId).datetimepicker("destroy");
                    });
                }

                /*-------------------------------------------------------------------
                 * Event Handler Methods
                 *-------------------------------------------------------------------*/

                /**
                 * @ngdoc method
                 * @name changeDateFilters
                 * @function
                 *
                 * @param {string} start - String representation of start date.
                 * @param {string} end - String representation of end date.
                 * @description - Change the date filters, then triggera new API call to load the reports.
                 */
                function changeDateFilters(start, end) {
                    $scope.filterStartDate = start;
                    $scope.filterEndDate = end;
                    $scope.currentPage = 0;
                    $scope.filterCallback();
                }

                /**
                 * @ngdoc method
                 * @name changeTermFilter
                 * @function
                 *
                 * @description - Triggers new API call to load the reports.
                 */
                function changeTermFilter() {
                    $scope.filterTermCallback();
                }

                /*-------------------------------------------------------------------
                 * Helper Methods
                 * ------------------------------------------------------------------*/

                //handles the date changing via the api
                function applyDateStart(e) {
                    angularHelper.safeApply($scope, function () {
                        // when a date is changed, update the model
                        if (e.localDate) {
                            $scope.filterStartDate = moment(e.localDate).format($scope.settings.dateFormat.toUpperCase());
                        }
                    });
                }

                //handles the date changing via the api
                function applyDateEnd(e) {
                    angularHelper.safeApply($scope, function () {
                        // when a date is changed, update the model
                        if (e.localDate) {
                            $scope.filterEndDate = moment(e.localDate).format($scope.settings.dateFormat.toUpperCase());
                        }
                    });
                }

                // Initialize the controller
                init();
            },
            compile: function (element, attrs) {
                if (!attrs.filterButtonText) {
                    attrs.filterButtonText = 'Filter';
                }
            }
        };
    });


})();