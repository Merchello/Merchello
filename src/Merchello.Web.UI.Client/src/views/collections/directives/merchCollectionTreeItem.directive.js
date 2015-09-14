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
