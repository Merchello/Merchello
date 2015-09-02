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
