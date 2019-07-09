(function () {
    angular.module('umbraco.directives', [
        'umbraco.directives.editors',
        'umbraco.directives.html',
        'umbraco.directives.validation',
        'ui.sortable'
    ]);
    angular.module('umbraco.directives.editors', []);
    angular.module('umbraco.directives.html', []);
    angular.module('umbraco.directives.validation', []);
    /**
* @ngdoc directive
* @name umbraco.directives.directive:autoScale
* @element div
* @deprecated
* We plan to remove this directive in the next major version of umbraco (8.0). The directive is not recommended to use.
* @function
* @description
* Resize div's automatically to fit to the bottom of the screen, as an optional parameter an y-axis offset can be set
* So if you only want to scale the div to 70 pixels from the bottom you pass "70"

* @example
* <example module="umbraco.directives">
*    <file name="index.html">
*        <div auto-scale="70" class="input-block-level"></div>
*    </file>
* </example>
**/
    angular.module('umbraco.directives').directive('autoScale', function ($window) {
        return function (scope, el, attrs) {
            var totalOffset = 0;
            var offsety = parseInt(attrs.autoScale, 10);
            var window = angular.element($window);
            if (offsety !== undefined) {
                totalOffset += offsety;
            }
            setTimeout(function () {
                el.height(window.height() - (el.offset().top + totalOffset));
            }, 500);
            window.bind('resize', function () {
                el.height(window.height() - (el.offset().top + totalOffset));
            });
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:detectFold
* @deprecated
* We plan to remove this directive in the next major version of umbraco (8.0). The directive is not recommended to use.
* @description This is used for the editor buttons to ensure they are displayed correctly if the horizontal overflow of the editor
* exceeds the height of the window
**/
    angular.module('umbraco.directives.html').directive('detectFold', function ($timeout, $log, windowResizeListener) {
        return {
            require: '^?umbTabs',
            restrict: 'A',
            link: function (scope, el, attrs, tabsCtrl) {
                var firstRun = false;
                var parent = $('.umb-panel-body');
                var winHeight = $(window).height();
                var calculate = function () {
                    if (el && el.is(':visible') && !el.hasClass('umb-bottom-bar')) {
                        //now that the element is visible, set the flag in a couple of seconds, 
                        // this will ensure that loading time of a current tab get's completed and that
                        // we eventually stop watching to save on CPU time
                        $timeout(function () {
                            firstRun = true;
                        }, 4000);
                        //var parent = el.parent();
                        var hasOverflow = parent.innerHeight() < parent[0].scrollHeight;
                        //var belowFold = (el.offset().top + el.height()) > winHeight;
                        if (hasOverflow) {
                            el.addClass('umb-bottom-bar');
                            //I wish we didn't have to put this logic here but unfortunately we 
                            // do. This needs to calculate the left offest to place the bottom bar
                            // depending on if the left column splitter has been moved by the user
                            // (based on the nav-resize directive)
                            var wrapper = $('#mainwrapper');
                            var contentPanel = $('#leftcolumn').next();
                            var contentPanelLeftPx = contentPanel.css('left');
                            el.css({ left: contentPanelLeftPx });
                        }
                    }
                    return firstRun;
                };
                var resizeCallback = function (size) {
                    winHeight = size.height;
                    el.removeClass('umb-bottom-bar');
                    calculate();
                };
                windowResizeListener.register(resizeCallback);
                //Only execute the watcher if this tab is the active (first) tab on load, otherwise there's no reason to execute
                // the watcher since it will be recalculated when the tab changes!
                if (el.closest('.umb-tab-pane').index() === 0) {
                    //run a watcher to ensure that the calculation occurs until it's firstRun but ensure
                    // the calculations are throttled to save a bit of CPU
                    var listener = scope.$watch(_.throttle(calculate, 1000), function (newVal, oldVal) {
                        if (newVal !== oldVal) {
                            listener();
                        }
                    });
                }
                //listen for tab changes
                if (tabsCtrl != null) {
                    tabsCtrl.onTabShown(function (args) {
                        calculate();
                    });
                }
                //ensure to unregister
                scope.$on('$destroy', function () {
                    windowResizeListener.unregister(resizeCallback);
                });
            }
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbContentName
* @deprecated
* We plan to remove this directive in the next major version of umbraco (8.0). The directive is not recommended to use.
* @restrict E
* @function
* @description
* Used by editors that require naming an entity. Shows a textbox/headline with a required validator within it's own form.
**/
    angular.module('umbraco.directives').directive('umbContentName', function ($timeout, localizationService) {
        return {
            require: 'ngModel',
            restrict: 'E',
            replace: true,
            templateUrl: 'views/directives/_obsolete/umb-content-name.html',
            scope: {
                placeholder: '@placeholder',
                model: '=ngModel',
                ngDisabled: '='
            },
            link: function (scope, element, attrs, ngModel) {
                var inputElement = element.find('input');
                if (scope.placeholder && scope.placeholder[0] === '@') {
                    localizationService.localize(scope.placeholder.substring(1)).then(function (value) {
                        scope.placeholder = value;
                    });
                }
                var mX, mY, distance;
                function calculateDistance(elem, mouseX, mouseY) {
                    var cx = Math.max(Math.min(mouseX, elem.offset().left + elem.width()), elem.offset().left);
                    var cy = Math.max(Math.min(mouseY, elem.offset().top + elem.height()), elem.offset().top);
                    return Math.sqrt((mouseX - cx) * (mouseX - cx) + (mouseY - cy) * (mouseY - cy));
                }
                var mouseMoveDebounce = _.throttle(function (e) {
                    mX = e.pageX;
                    mY = e.pageY;
                    // not focused and not over element
                    if (!inputElement.is(':focus') && !inputElement.hasClass('ng-invalid')) {
                        // on page
                        if (mX >= inputElement.offset().left) {
                            distance = calculateDistance(inputElement, mX, mY);
                            if (distance <= 155) {
                                distance = 1 - 100 / 150 * distance / 100;
                                inputElement.css('border', '1px solid rgba(175,175,175, ' + distance + ')');
                                inputElement.css('background-color', 'rgba(255,255,255, ' + distance + ')');
                            }
                        }
                    }
                }, 15);
                $(document).bind('mousemove', mouseMoveDebounce);
                $timeout(function () {
                    if (!scope.model) {
                        scope.goEdit();
                    }
                }, 100, false);
                scope.goEdit = function () {
                    scope.editMode = true;
                    $timeout(function () {
                        inputElement.focus();
                    }, 100, false);
                };
                scope.exitEdit = function () {
                    if (scope.model && scope.model !== '') {
                        scope.editMode = false;
                    }
                };
                //unbind doc event!
                scope.$on('$destroy', function () {
                    $(document).unbind('mousemove', mouseMoveDebounce);
                });
            }
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbHeader
* @deprecated
* We plan to remove this directive in the next major version of umbraco (8.0). The directive is not recommended to use.
* @restrict E
* @function
* @description
* The header on an editor that contains tabs using bootstrap tabs - THIS IS OBSOLETE, use umbTabHeader instead
**/
    angular.module('umbraco.directives').directive('umbHeader', function ($parse, $timeout) {
        return {
            restrict: 'E',
            replace: true,
            transclude: 'true',
            templateUrl: 'views/directives/_obsolete/umb-header.html',
            //create a new isolated scope assigning a tabs property from the attribute 'tabs'
            //which is bound to the parent scope property passed in
            scope: { tabs: '=' },
            link: function (scope, iElement, iAttrs) {
                scope.showTabs = iAttrs.tabs ? true : false;
                scope.visibleTabs = [];
                //since tabs are loaded async, we need to put a watch on them to determine
                // when they are loaded, then we can close the watch
                var tabWatch = scope.$watch('tabs', function (newValue, oldValue) {
                    angular.forEach(newValue, function (val, index) {
                        var tab = {
                            id: val.id,
                            label: val.label
                        };
                        scope.visibleTabs.push(tab);
                    });
                    //don't process if we cannot or have already done so
                    if (!newValue) {
                        return;
                    }
                    if (!newValue.length || newValue.length === 0) {
                        return;
                    }
                    //we need to do a timeout here so that the current sync operation can complete
                    // and update the UI, then this will fire and the UI elements will be available.
                    $timeout(function () {
                        //use bootstrap tabs API to show the first one
                        iElement.find('.nav-tabs a:first').tab('show');
                        //enable the tab drop
                        iElement.find('.nav-pills, .nav-tabs').tabdrop();
                        //ensure to destroy tabdrop (unbinds window resize listeners)
                        scope.$on('$destroy', function () {
                            iElement.find('.nav-pills, .nav-tabs').tabdrop('destroy');
                        });
                        //stop watching now
                        tabWatch();
                    }, 200);
                });
            }
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbItemSorter
* @deprecated
* We plan to remove this directive in the next major version of umbraco (8.0). The directive is not recommended to use.
* @function
* @element ANY
* @restrict E
* @description A re-usable directive for sorting items
**/
    function umbItemSorter(angularHelper) {
        return {
            scope: { model: '=' },
            restrict: 'E',
            // restrict to an element
            replace: true,
            // replace the html element with the template
            templateUrl: 'views/directives/_obsolete/umb-item-sorter.html',
            link: function (scope, element, attrs, ctrl) {
                var defaultModel = {
                    okButton: 'Ok',
                    successMsg: 'Sorting successful',
                    complete: false
                };
                //assign user vals to default
                angular.extend(defaultModel, scope.model);
                //re-assign merged to user
                scope.model = defaultModel;
                scope.performSort = function () {
                    scope.$emit('umbItemSorter.sorting', { sortedItems: scope.model.itemsToSort });
                };
                scope.handleCancel = function () {
                    scope.$emit('umbItemSorter.cancel');
                };
                scope.handleOk = function () {
                    scope.$emit('umbItemSorter.ok');
                };
                //defines the options for the jquery sortable
                scope.sortableOptions = {
                    axis: 'y',
                    cursor: 'move',
                    placeholder: 'ui-sortable-placeholder',
                    update: function (ev, ui) {
                        //highlight the item when the position is changed
                        $(ui.item).effect('highlight', { color: '#049cdb' }, 500);
                    },
                    stop: function (ev, ui) {
                        //the ui-sortable directive already ensures that our list is re-sorted, so now we just
                        // need to update the sortOrder to the index of each item
                        angularHelper.safeApply(scope, function () {
                            angular.forEach(scope.itemsToSort, function (val, index) {
                                val.sortOrder = index + 1;
                            });
                        });
                    }
                };
            }
        };
    }
    angular.module('umbraco.directives').directive('umbItemSorter', umbItemSorter);
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbLogin
* @deprecated
* We plan to remove this directive in the next major version of umbraco (8.0). The directive is not recommended to use.
* @function
* @element ANY
* @restrict E
**/
    function loginDirective() {
        return {
            restrict: 'E',
            // restrict to an element
            replace: true,
            // replace the html element with the template
            templateUrl: 'views/directives/_obsolete/umb-login.html'
        };
    }
    angular.module('umbraco.directives').directive('umbLogin', loginDirective);
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbOptionsMenu
* @deprecated
* We plan to remove this directive in the next major version of umbraco (8.0). The directive is not recommended to use.
* @function
* @element ANY
* @restrict E
**/
    angular.module('umbraco.directives').directive('umbOptionsMenu', function ($injector, treeService, navigationService, umbModelMapper, appState) {
        return {
            scope: {
                currentSection: '@',
                currentNode: '='
            },
            restrict: 'E',
            replace: true,
            templateUrl: 'views/directives/_obsolete/umb-optionsmenu.html',
            link: function (scope, element, attrs, ctrl) {
                //adds a handler to the context menu item click, we need to handle this differently
                //depending on what the menu item is supposed to do.
                scope.executeMenuItem = function (action) {
                    navigationService.executeMenuAction(action, scope.currentNode, scope.currentSection);
                };
                //callback method to go and get the options async
                scope.getOptions = function () {
                    if (!scope.currentNode) {
                        return;
                    }
                    //when the options item is selected, we need to set the current menu item in appState (since this is synonymous with a menu)
                    appState.setMenuState('currentNode', scope.currentNode);
                    if (!scope.actions) {
                        treeService.getMenu({ treeNode: scope.currentNode }).then(function (data) {
                            scope.actions = data.menuItems;
                        });
                    }
                };
            }
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbPhotoFolder
* @deprecated
* We plan to remove this directive in the next major version of umbraco (8.0). The directive is not recommended to use.
* @restrict E
**/
    angular.module('umbraco.directives.html').directive('umbPhotoFolder', function ($compile, $log, $timeout, $filter, umbPhotoFolderHelper) {
        return {
            restrict: 'E',
            replace: true,
            require: '?ngModel',
            terminate: true,
            templateUrl: 'views/directives/_obsolete/umb-photo-folder.html',
            link: function (scope, element, attrs, ngModel) {
                var lastWatch = null;
                ngModel.$render = function () {
                    if (ngModel.$modelValue) {
                        $timeout(function () {
                            var photos = ngModel.$modelValue;
                            scope.clickHandler = scope.$eval(element.attr('on-click'));
                            var imagesOnly = element.attr('images-only') === 'true';
                            var margin = element.attr('border') ? parseInt(element.attr('border'), 10) : 5;
                            var startingIndex = element.attr('baseline') ? parseInt(element.attr('baseline'), 10) : 0;
                            var minWidth = element.attr('min-width') ? parseInt(element.attr('min-width'), 10) : 420;
                            var minHeight = element.attr('min-height') ? parseInt(element.attr('min-height'), 10) : 100;
                            var maxHeight = element.attr('max-height') ? parseInt(element.attr('max-height'), 10) : 300;
                            var idealImgPerRow = element.attr('ideal-items-per-row') ? parseInt(element.attr('ideal-items-per-row'), 10) : 5;
                            var fixedRowWidth = Math.max(element.width(), minWidth);
                            scope.containerStyle = { width: fixedRowWidth + 'px' };
                            scope.rows = umbPhotoFolderHelper.buildGrid(photos, fixedRowWidth, maxHeight, startingIndex, minHeight, idealImgPerRow, margin, imagesOnly);
                            if (attrs.filterBy) {
                                //we track the watches that we create, we don't want to create multiple, so clear it
                                // if it already exists before creating another.
                                if (lastWatch) {
                                    lastWatch();
                                }
                                //TODO: Need to debounce this so it doesn't filter too often!
                                lastWatch = scope.$watch(attrs.filterBy, function (newVal, oldVal) {
                                    if (newVal && newVal !== oldVal) {
                                        var p = $filter('filter')(photos, newVal, false);
                                        scope.baseline = 0;
                                        var m = umbPhotoFolderHelper.buildGrid(p, fixedRowWidth, maxHeight, startingIndex, minHeight, idealImgPerRow, margin, imagesOnly);
                                        scope.rows = m;
                                    }
                                });
                            }
                        }, 500);    //end timeout
                    }    //end if modelValue
                };    //end $render
            }
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbSort
* @deprecated
* We plan to remove this directive in the next major version of umbraco (8.0). The directive is not recommended to use.
*
* @element div
* @function
*
* @description
* Resize div's automatically to fit to the bottom of the screen, as an optional parameter an y-axis offset can be set
* So if you only want to scale the div to 70 pixels from the bottom you pass "70"
*
* @example
* <example module="umbraco.directives">
*     <file name="index.html">
*         <div umb-sort="70" class="input-block-level"></div>
*     </file>
* </example>
**/
    angular.module('umbraco.directives').value('umbSortContextInternal', {}).directive('umbSort', function ($log, umbSortContextInternal) {
        return {
            require: '?ngModel',
            link: function (scope, element, attrs, ngModel) {
                var adjustment;
                var cfg = scope.$eval(element.attr('umb-sort')) || {};
                scope.model = ngModel;
                scope.opts = cfg;
                scope.opts.containerSelector = cfg.containerSelector || '.umb-' + cfg.group + '-container', scope.opts.nested = cfg.nested || true, scope.opts.drop = cfg.drop || true, scope.opts.drag = cfg.drag || true, scope.opts.clone = cfg.clone || '<li/>';
                scope.opts.mode = cfg.mode || 'list';
                scope.opts.itemSelectorFull = $.trim(scope.opts.itemPath + ' ' + scope.opts.itemSelector);
                /*
                scope.opts.isValidTarget = function(item, container) {
                        if(container.el.is(".umb-" + scope.opts.group + "-container")){
                            return true;
                        }
                        return false;
                     };
                */
                element.addClass('umb-sort');
                element.addClass('umb-' + cfg.group + '-container');
                scope.opts.onDrag = function (item, position) {
                    if (scope.opts.mode === 'list') {
                        item.css({
                            left: position.left - adjustment.left,
                            top: position.top - adjustment.top
                        });
                    }
                };
                scope.opts.onDrop = function (item, targetContainer, _super) {
                    if (scope.opts.mode === 'list') {
                        //list mode
                        var clonedItem = $(scope.opts.clone).css({ height: 0 });
                        item.after(clonedItem);
                        clonedItem.animate({ 'height': item.height() });
                        item.animate(clonedItem.position(), function () {
                            clonedItem.detach();
                            _super(item);
                        });
                    }
                    var children = $(scope.opts.itemSelectorFull, targetContainer.el);
                    var targetIndex = children.index(item);
                    var targetScope = $(targetContainer.el[0]).scope();
                    if (targetScope === umbSortContextInternal.sourceScope) {
                        if (umbSortContextInternal.sourceScope.opts.onSortHandler) {
                            var _largs = {
                                oldIndex: umbSortContextInternal.sourceIndex,
                                newIndex: targetIndex,
                                scope: umbSortContextInternal.sourceScope
                            };
                            umbSortContextInternal.sourceScope.opts.onSortHandler.call(this, item, _largs);
                        }
                    } else {
                        if (targetScope.opts.onDropHandler) {
                            var args = {
                                sourceScope: umbSortContextInternal.sourceScope,
                                sourceIndex: umbSortContextInternal.sourceIndex,
                                sourceContainer: umbSortContextInternal.sourceContainer,
                                targetScope: targetScope,
                                targetIndex: targetIndex,
                                targetContainer: targetContainer
                            };
                            targetScope.opts.onDropHandler.call(this, item, args);
                        }
                        if (umbSortContextInternal.sourceScope.opts.onReleaseHandler) {
                            var _args = {
                                sourceScope: umbSortContextInternal.sourceScope,
                                sourceIndex: umbSortContextInternal.sourceIndex,
                                sourceContainer: umbSortContextInternal.sourceContainer,
                                targetScope: targetScope,
                                targetIndex: targetIndex,
                                targetContainer: targetContainer
                            };
                            umbSortContextInternal.sourceScope.opts.onReleaseHandler.call(this, item, _args);
                        }
                    }
                };
                scope.changeIndex = function (from, to) {
                    scope.$apply(function () {
                        var i = ngModel.$modelValue.splice(from, 1)[0];
                        ngModel.$modelValue.splice(to, 0, i);
                    });
                };
                scope.move = function (args) {
                    var from = args.sourceIndex;
                    var to = args.targetIndex;
                    if (args.sourceContainer === args.targetContainer) {
                        scope.changeIndex(from, to);
                    } else {
                        scope.$apply(function () {
                            var i = args.sourceScope.model.$modelValue.splice(from, 1)[0];
                            args.targetScope.model.$modelvalue.splice(to, 0, i);
                        });
                    }
                };
                scope.opts.onDragStart = function (item, container, _super) {
                    var children = $(scope.opts.itemSelectorFull, container.el);
                    var offset = item.offset();
                    umbSortContextInternal.sourceIndex = children.index(item);
                    umbSortContextInternal.sourceScope = $(container.el[0]).scope();
                    umbSortContextInternal.sourceContainer = container;
                    //current.item = ngModel.$modelValue.splice(current.index, 1)[0];
                    var pointer = container.rootGroup.pointer;
                    adjustment = {
                        left: pointer.left - offset.left,
                        top: pointer.top - offset.top
                    };
                    _super(item, container);
                };
                element.sortable(scope.opts);
            }
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbTabView
* @deprecated
* We plan to remove this directive in the next major version of umbraco (8.0). The directive is not recommended to use.
*
* @restrict E
**/
    angular.module('umbraco.directives').directive('umbTabView', function ($timeout, $log) {
        return {
            restrict: 'E',
            replace: true,
            transclude: 'true',
            templateUrl: 'views/directives/_obsolete/umb-tab-view.html'
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbUploadDropzone
* @deprecated
* We plan to remove this directive in the next major version of umbraco (8.0). The directive is not recommended to use.
*
* @restrict E
**/
    angular.module('umbraco.directives.html').directive('umbUploadDropzone', function () {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: 'views/directives/_obsolete/umb-upload-dropzone.html'
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:navResize
* @restrict A
 *
 * @description
 * Handles how the navigation responds to window resizing and controls how the draggable resize panel works
**/
    angular.module('umbraco.directives').directive('navResize', function (appState, eventsService, windowResizeListener) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs, ctrl) {
                var minScreenSize = 1100;
                var resizeEnabled = false;
                function setTreeMode() {
                    appState.setGlobalState('showNavigation', appState.getGlobalState('isTablet') === false);
                }
                function enableResize() {
                    //only enable when the size is correct and it's not already enabled
                    if (!resizeEnabled && appState.getGlobalState('isTablet') === false) {
                        element.resizable({
                            containment: $('#mainwrapper'),
                            autoHide: true,
                            handles: 'e',
                            alsoResize: '.navigation-inner-container',
                            resize: function (e, ui) {
                                var wrapper = $('#mainwrapper');
                                var contentPanel = $('#contentwrapper');
                                var umbNotification = $('#umb-notifications-wrapper');
                                var apps = $('#applications');
                                var bottomBar = contentPanel.find('.umb-bottom-bar');
                                var navOffeset = $('#navOffset');
                                var leftPanelWidth = ui.element.width() + apps.width();
                                contentPanel.css({ left: leftPanelWidth });
                                bottomBar.css({ left: leftPanelWidth });
                                umbNotification.css({ left: leftPanelWidth });
                                navOffeset.css({ 'margin-left': ui.element.outerWidth() });
                            },
                            stop: function (e, ui) {
                            }
                        });
                        resizeEnabled = true;
                    }
                }
                function resetResize() {
                    if (resizeEnabled) {
                        //kill the resize
                        element.resizable('destroy');
                        element.css('width', '');
                        var navInnerContainer = element.find('.navigation-inner-container');
                        navInnerContainer.css('width', '');
                        $('#contentwrapper').css('left', '');
                        $('#umb-notifications-wrapper').css('left', '');
                        $('#navOffset').css('margin-left', '');
                        resizeEnabled = false;
                    }
                }
                var evts = [];
                //Listen for global state changes
                evts.push(eventsService.on('appState.globalState.changed', function (e, args) {
                    if (args.key === 'showNavigation') {
                        if (args.value === false) {
                            resetResize();
                        } else {
                            enableResize();
                        }
                    }
                }));
                var resizeCallback = function (size) {
                    //set the global app state
                    appState.setGlobalState('isTablet', size.width <= minScreenSize);
                    setTreeMode();
                };
                windowResizeListener.register(resizeCallback);
                //ensure to unregister from all events and kill jquery plugins
                scope.$on('$destroy', function () {
                    windowResizeListener.unregister(resizeCallback);
                    for (var e in evts) {
                        eventsService.unsubscribe(evts[e]);
                    }
                    var navInnerContainer = element.find('.navigation-inner-container');
                    navInnerContainer.resizable('destroy');
                });
                //init
                //set the global app state
                appState.setGlobalState('isTablet', $(window).width() <= minScreenSize);
                setTreeMode();
            }
        };
    });
    angular.module('umbraco.directives').directive('sectionIcon', function ($compile, iconHelper) {
        return {
            restrict: 'E',
            replace: true,
            link: function (scope, element, attrs) {
                var icon = attrs.icon;
                if (iconHelper.isLegacyIcon(icon)) {
                    //its a known legacy icon, convert to a new one
                    element.html('<i class=\'' + iconHelper.convertFromLegacyIcon(icon) + '\'></i>');
                } else if (iconHelper.isFileBasedIcon(icon)) {
                    var convert = iconHelper.convertFromLegacyImage(icon);
                    if (convert) {
                        element.html('<i class=\'icon-section ' + convert + '\'></i>');
                    } else {
                        element.html('<img class=\'icon-section\' src=\'images/tray/' + icon + '\'>');
                    }    //it's a file, normally legacy so look in the icon tray images
                } else {
                    //it's normal
                    element.html('<i class=\'icon-section ' + icon + '\'></i>');
                }
            }
        };
    });
    (function () {
        'use strict';
        function BackdropDirective($timeout, $http) {
            function link(scope, el, attr, ctrl) {
                var events = [];
                scope.clickBackdrop = function (event) {
                    if (scope.disableEventsOnClick === true) {
                        event.preventDefault();
                        event.stopPropagation();
                    }
                };
                function onInit() {
                    if (scope.highlightElement) {
                        setHighlight();
                    }
                }
                function setHighlight() {
                    scope.loading = true;
                    $timeout(function () {
                        // The element to highlight
                        var highlightElement = angular.element(scope.highlightElement);
                        if (highlightElement && highlightElement.length > 0) {
                            var offset = highlightElement.offset();
                            var width = highlightElement.outerWidth();
                            var height = highlightElement.outerHeight();
                            // Rounding numbers
                            var topDistance = offset.top.toFixed();
                            var topAndHeight = (offset.top + height).toFixed();
                            var leftDistance = offset.left.toFixed();
                            var leftAndWidth = (offset.left + width).toFixed();
                            // The four rectangles
                            var rectTop = el.find('.umb-backdrop__rect--top');
                            var rectRight = el.find('.umb-backdrop__rect--right');
                            var rectBottom = el.find('.umb-backdrop__rect--bottom');
                            var rectLeft = el.find('.umb-backdrop__rect--left');
                            // Add the css
                            scope.rectTopCss = {
                                'height': topDistance,
                                'left': leftDistance + 'px',
                                opacity: scope.backdropOpacity
                            };
                            scope.rectRightCss = {
                                'left': leftAndWidth + 'px',
                                'top': topDistance + 'px',
                                'height': height,
                                opacity: scope.backdropOpacity
                            };
                            scope.rectBottomCss = {
                                'height': '100%',
                                'top': topAndHeight + 'px',
                                'left': leftDistance + 'px',
                                opacity: scope.backdropOpacity
                            };
                            scope.rectLeftCss = {
                                'width': leftDistance,
                                opacity: scope.backdropOpacity
                            };
                            // Prevent interaction in the highlighted area
                            if (scope.highlightPreventClick) {
                                var preventClickElement = el.find('.umb-backdrop__highlight-prevent-click');
                                preventClickElement.css({
                                    'width': width,
                                    'height': height,
                                    'left': offset.left,
                                    'top': offset.top
                                });
                            }
                        }
                        scope.loading = false;
                    });
                }
                function resize() {
                    setHighlight();
                }
                events.push(scope.$watch('highlightElement', function (newValue, oldValue) {
                    if (!newValue) {
                        return;
                    }
                    if (newValue === oldValue) {
                        return;
                    }
                    setHighlight();
                }));
                $(window).on('resize.umbBackdrop', resize);
                scope.$on('$destroy', function () {
                    // unbind watchers
                    for (var e in events) {
                        events[e]();
                    }
                    $(window).off('resize.umbBackdrop');
                });
                onInit();
            }
            var directive = {
                transclude: true,
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/application/umb-backdrop.html',
                link: link,
                scope: {
                    backdropOpacity: '=?',
                    highlightElement: '=?',
                    highlightPreventClick: '=?',
                    disableEventsOnClick: '=?'
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbBackdrop', BackdropDirective);
    }());
    angular.module('umbraco.directives').directive('umbContextMenu', function (navigationService) {
        return {
            scope: {
                menuDialogTitle: '@',
                currentSection: '@',
                currentNode: '=',
                menuActions: '='
            },
            restrict: 'E',
            replace: true,
            templateUrl: 'views/components/application/umb-contextmenu.html',
            link: function (scope, element, attrs, ctrl) {
                //adds a handler to the context menu item click, we need to handle this differently
                //depending on what the menu item is supposed to do.
                scope.executeMenuItem = function (action) {
                    navigationService.executeMenuAction(action, scope.currentNode, scope.currentSection);
                };
            }
        };
    });
    /**
@ngdoc directive
@name umbraco.directives.directive:umbDrawer
@restrict E
@scope

@description
The drawer component is a global component and is already added to the umbraco markup. It is registered in globalState and can be opened and configured by raising events.

<h3>Markup example - how to open the drawer</h3>
<pre>
    <div ng-controller="My.DrawerController as vm">

        <umb-button
            type="button"
            label="Toggle drawer"
            action="vm.toggleDrawer()">
        </umb-button>

    </div>
</pre>

<h3>Controller example - how to open the drawer</h3>
<pre>
    (function () {
        "use strict";

        function DrawerController(appState) {

            var vm = this;

            vm.toggleDrawer = toggleDrawer;

            function toggleDrawer() {

                var showDrawer = appState.getDrawerState("showDrawer");            

                var model = {
                    firstName: "Super",
                    lastName: "Man"
                };

                appState.setDrawerState("view", "/App_Plugins/path/to/drawer.html");
                appState.setDrawerState("model", model);
                appState.setDrawerState("showDrawer", !showDrawer);
                
            }

        }

        angular.module("umbraco").controller("My.DrawerController", DrawerController);

    })();
</pre>

<h3>Use the following components in the custom drawer to render the content</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbDrawerView umbDrawerView}</li>
    <li>{@link umbraco.directives.directive:umbDrawerHeader umbDrawerHeader}</li>
    <li>{@link umbraco.directives.directive:umbDrawerView umbDrawerContent}</li>
    <li>{@link umbraco.directives.directive:umbDrawerFooter umbDrawerFooter}</li>
</ul>

@param {string} view (<code>binding</code>): Set the drawer view
@param {string} model (<code>binding</code>): Pass in custom data to the drawer

**/
    function Drawer($location, $routeParams, helpService, userService, localizationService, dashboardResource) {
        return {
            restrict: 'E',
            // restrict to an element
            replace: true,
            // replace the html element with the template
            templateUrl: 'views/components/application/umbdrawer/umb-drawer.html',
            transclude: true,
            scope: {
                view: '=?',
                model: '=?'
            },
            link: function (scope, element, attr, ctrl) {
                function onInit() {
                    setView();
                }
                function setView() {
                    if (scope.view) {
                        //we do this to avoid a hidden dialog to start loading unconfigured views before the first activation
                        var configuredView = scope.view;
                        if (scope.view.indexOf('.html') === -1) {
                            var viewAlias = scope.view.toLowerCase();
                            configuredView = 'views/common/drawers/' + viewAlias + '/' + viewAlias + '.html';
                        }
                        if (configuredView !== scope.configuredView) {
                            scope.configuredView = configuredView;
                        }
                    }
                }
                onInit();
            }
        };
    }
    angular.module('umbraco.directives').directive('umbDrawer', Drawer);
    /**
@ngdoc directive
@name umbraco.directives.directive:umbDrawerContent
@restrict E
@scope

@description
Use this directive to render drawer content

<h3>Markup example</h3>
<pre>
	<umb-drawer-view>
        
        <umb-drawer-header
            title="Drawer Title"
            description="Drawer description">
        </umb-drawer-header>

        <umb-drawer-content>
            <!-- Your content here -->
            <pre>{{ model | json }}</pre>
        </umb-drawer-content>

        <umb-drawer-footer>
            <!-- Your content here -->
        </umb-drawer-footer>

	</umb-drawer-view>
</pre>


<h3>Use in combination with</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbDrawerView umbDrawerView}</li>
    <li>{@link umbraco.directives.directive:umbDrawerHeader umbDrawerHeader}</li>
    <li>{@link umbraco.directives.directive:umbDrawerFooter umbDrawerFooter}</li>
</ul>

**/
    (function () {
        'use strict';
        function DrawerContentDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/application/umbdrawer/umb-drawer-content.html'
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbDrawerContent', DrawerContentDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbDrawerFooter
@restrict E
@scope

@description
Use this directive to render a drawer footer

<h3>Markup example</h3>
<pre>
	<umb-drawer-view>
        
        <umb-drawer-header
            title="Drawer Title"
            description="Drawer description">
        </umb-drawer-header>

        <umb-drawer-content>
            <!-- Your content here -->
            <pre>{{ model | json }}</pre>
        </umb-drawer-content>

        <umb-drawer-footer>
            <!-- Your content here -->
        </umb-drawer-footer>

	</umb-drawer-view>
</pre>

<h3>Use in combination with</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbDrawerView umbDrawerView}</li>
    <li>{@link umbraco.directives.directive:umbDrawerHeader umbDrawerHeader}</li>
    <li>{@link umbraco.directives.directive:umbDrawerContent umbDrawerContent}</li>
</ul>

**/
    (function () {
        'use strict';
        function DrawerFooterDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/application/umbdrawer/umb-drawer-footer.html'
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbDrawerFooter', DrawerFooterDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbDrawerHeader
@restrict E
@scope

@description
Use this directive to render a drawer header

<h3>Markup example</h3>
<pre>
	<umb-drawer-view>
        
        <umb-drawer-header
            title="Drawer Title"
            description="Drawer description">
        </umb-drawer-header>

        <umb-drawer-content>
            <!-- Your content here -->
            <pre>{{ model | json }}</pre>
        </umb-drawer-content>

        <umb-drawer-footer>
            <!-- Your content here -->
        </umb-drawer-footer>

	</umb-drawer-view>
</pre>

<h3>Use in combination with</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbDrawerView umbDrawerView}</li>
    <li>{@link umbraco.directives.directive:umbDrawerContent umbDrawerContent}</li>
    <li>{@link umbraco.directives.directive:umbDrawerFooter umbDrawerFooter}</li>
</ul>

@param {string} title (<code>attribute</code>): Set a drawer title.
@param {string} description (<code>attribute</code>): Set a drawer description.
**/
    (function () {
        'use strict';
        function DrawerHeaderDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/application/umbdrawer/umb-drawer-header.html',
                scope: {
                    'title': '@?',
                    'description': '@?'
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbDrawerHeader', DrawerHeaderDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbDrawerView
@restrict E
@scope

@description
Use this directive to render drawer view

<h3>Markup example</h3>
<pre>
	<umb-drawer-view>
        
        <umb-drawer-header
            title="Drawer Title"
            description="Drawer description">
        </umb-drawer-header>

        <umb-drawer-content>
            <!-- Your content here -->
            <pre>{{ model | json }}</pre>
        </umb-drawer-content>

        <umb-drawer-footer>
            <!-- Your content here -->
        </umb-drawer-footer>

	</umb-drawer-view>
</pre>

<h3>Use in combination with</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbDrawerHeader umbDrawerHeader}</li>
    <li>{@link umbraco.directives.directive:umbDrawerContent umbDrawerContent}</li>
    <li>{@link umbraco.directives.directive:umbDrawerFooter umbDrawerFooter}</li>
</ul>

**/
    (function () {
        'use strict';
        function DrawerViewDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/application/umbdrawer/umb-drawer-view.html'
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbDrawerView', DrawerViewDirective);
    }());
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbNavigation
* @restrict E
**/
    function umbNavigationDirective() {
        return {
            restrict: 'E',
            // restrict to an element
            replace: true,
            // replace the html element with the template
            templateUrl: 'views/components/application/umb-navigation.html'
        };
    }
    angular.module('umbraco.directives').directive('umbNavigation', umbNavigationDirective);
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbSections
* @restrict E
**/
    function sectionsDirective($timeout, $window, navigationService, treeService, sectionService, appState, eventsService, $location, historyService) {
        return {
            restrict: 'E',
            // restrict to an element
            replace: true,
            // replace the html element with the template
            templateUrl: 'views/components/application/umb-sections.html',
            link: function (scope, element, attr, ctrl) {
                //setup scope vars
                scope.maxSections = 7;
                scope.overflowingSections = 0;
                scope.sections = [];
                scope.currentSection = appState.getSectionState('currentSection');
                scope.showTray = false;
                //appState.getGlobalState("showTray");
                scope.stickyNavigation = appState.getGlobalState('stickyNavigation');
                scope.needTray = false;
                scope.trayAnimation = function () {
                    if (scope.showTray) {
                        return 'slide';
                    } else if (scope.showTray === false) {
                        return 'slide';
                    } else {
                        return '';
                    }
                };
                function loadSections() {
                    sectionService.getSectionsForUser().then(function (result) {
                        scope.sections = result;
                        calculateHeight();
                    });
                }
                function calculateHeight() {
                    $timeout(function () {
                        //total height minus room for avatar and help icon
                        var height = $(window).height() - 200;
                        scope.totalSections = scope.sections.length;
                        scope.maxSections = Math.floor(height / 70);
                        scope.needTray = false;
                        if (scope.totalSections > scope.maxSections) {
                            scope.needTray = true;
                            scope.overflowingSections = scope.maxSections - scope.totalSections;
                        }
                    });
                }
                var evts = [];
                //Listen for global state changes
                evts.push(eventsService.on('appState.globalState.changed', function (e, args) {
                    if (args.key === 'showTray') {
                        scope.showTray = args.value;
                    }
                    if (args.key === 'stickyNavigation') {
                        scope.stickyNavigation = args.value;
                    }
                }));
                evts.push(eventsService.on('appState.sectionState.changed', function (e, args) {
                    if (args.key === 'currentSection') {
                        scope.currentSection = args.value;
                    }
                }));
                evts.push(eventsService.on('app.reInitialize', function (e, args) {
                    //re-load the sections if we're re-initializing (i.e. package installed)
                    loadSections();
                }));
                //ensure to unregister from all events!
                scope.$on('$destroy', function () {
                    for (var e in evts) {
                        eventsService.unsubscribe(evts[e]);
                    }
                });
                //on page resize
                window.onresize = calculateHeight;
                scope.avatarClick = function () {
                    if (scope.helpDialog) {
                        closeHelpDialog();
                    }
                    if (!scope.userDialog) {
                        scope.userDialog = {
                            view: 'user',
                            show: true,
                            close: function (oldModel) {
                                closeUserDialog();
                            }
                        };
                    } else {
                        closeUserDialog();
                    }
                };
                function closeUserDialog() {
                    scope.userDialog.show = false;
                    scope.userDialog = null;
                }
                //toggle the help dialog by raising the global app state to toggle the help drawer
                scope.helpClick = function () {
                    var showDrawer = appState.getDrawerState('showDrawer');
                    var drawer = {
                        view: 'help',
                        show: !showDrawer
                    };
                    appState.setDrawerState('view', drawer.view);
                    appState.setDrawerState('showDrawer', drawer.show);
                };
                scope.sectionClick = function (event, section) {
                    if (event.ctrlKey || event.shiftKey || event.metaKey || event.button && event.button === 1    // middle click, >IE9 + everyone else
) {
                        return;
                    }
                    if (scope.userDialog) {
                        closeUserDialog();
                    }
                    navigationService.hideSearch();
                    navigationService.showTree(section.alias);
                    //in some cases the section will have a custom route path specified, if there is one we'll use it
                    if (section.routePath) {
                        $location.path(section.routePath);
                    } else {
                        var lastAccessed = historyService.getLastAccessedItemForSection(section.alias);
                        var path = lastAccessed != null ? lastAccessed.link : section.alias;
                        $location.path(path).search('');
                    }
                };
                scope.sectionDblClick = function (section) {
                    navigationService.reloadSection(section.alias);
                };
                scope.trayClick = function () {
                    // close dialogs
                    if (scope.userDialog) {
                        closeUserDialog();
                    }
                    if (scope.helpDialog) {
                        closeHelpDialog();
                    }
                    if (appState.getGlobalState('showTray') === true) {
                        navigationService.hideTray();
                    } else {
                        navigationService.showTray();
                    }
                };
                loadSections();
            }
        };
    }
    angular.module('umbraco.directives').directive('umbSections', sectionsDirective);
    /**
@ngdoc directive
@name umbraco.directives.directive:umbTour
@restrict E
@scope

@description
<b>Added in Umbraco 7.8</b>. The tour component is a global component and is already added to the umbraco markup. 
In the Umbraco UI the tours live in the "Help drawer" which opens when you click the Help-icon in the bottom left corner of Umbraco. 
You can easily add you own tours to the Help-drawer or show and start tours from 
anywhere in the Umbraco backoffice. To see a real world example of a custom tour implementation, install <a href="https://our.umbraco.com/projects/starter-kits/the-starter-kit/">The Starter Kit</a> in Umbraco 7.8

<h1><b>Extending the help drawer with custom tours</b></h1>
The easiet way to add new tours to Umbraco is through the Help-drawer. All it requires is a my-tour.json file. 
Place the file in <i>App_Plugins/{MyPackage}/backoffice/tours/{my-tour}.json</i> and it will automatically be 
picked up by Umbraco and shown in the Help-drawer.

<h3><b>The tour object</b></h3>
The tour object consist of two parts - The overall tour configuration and a list of tour steps. We have split up the tour object for a better overview.
<pre>
// The tour config object
{
    "name": "My Custom Tour", // (required)
    "alias": "myCustomTour", // A unique tour alias (required)
    "group": "My Custom Group" // Used to group tours in the help drawer
    "groupOrder": 200 // Control the order of tour groups
    "allowDisable": // Adds a "Don't" show this tour again"-button to the intro step
    "culture" : // From v7.11+. Specifies the culture of the tour (eg. en-US), if set the tour will only be shown to users with this culture set on their profile. If omitted or left empty the tour will be visible to all users
    "requiredSections":["content", "media", "mySection"] // Sections that the tour will access while running, if the user does not have access to the required tour sections, the tour will not load.   
    "steps": [] // tour steps - see next example
}
</pre>
<pre>
// A tour step object
{
    "title": "Title",
    "content": "<p>Step content</p>",
    "type": "intro" // makes the step an introduction step,
    "element": "[data-element='my-table-row']", // the highlighted element
    "event": "click" // forces the user to click the UI to go to next step
    "eventElement": "[data-element='my-table-row'] [data-element='my-tour-button']" // specify an element to click inside a highlighted element
    "elementPreventClick": false // prevents user interaction in the highlighted element
    "backdropOpacity": 0.4 // the backdrop opacity
    "view": "" // add a custom view
    "customProperties" : {} // add any custom properties needed for the custom view
}
</pre>

<h1><b>Adding tours to other parts of the Umbraco backoffice</b></h1>
It is also possible to add a list of custom tours to other parts of the Umbraco backoffice, 
as an example on a Dashboard in a Custom section. You can then use the {@link umbraco.services.tourService tourService} to start and stop tours but you don't have to register them as part of the tour service.

<h1><b>Using the tour service</b></h1>
<h3>Markup example - show custom tour</h3>
<pre>
    <div ng-controller="My.TourController as vm">

        <div>{{vm.tour.name}}</div>
        <button type="button" ng-click="vm.startTour()">Start tour</button>

        <!-- This button will be clicked in the tour -->
        <button data-element="my-tour-button" type="button">Click me</button>

    </div>
</pre>

<h3>Controller example - show custom tour</h3>
<pre>
    (function () {
        "use strict";

        function TourController(tourService) {

            var vm = this;

            vm.tour = {
                "name": "My Custom Tour",
                "alias": "myCustomTour",
                "steps": [
                    {
                        "title": "Welcome to My Custom Tour",
                        "content": "",
                        "type": "intro"
                    },
                    {
                        "element": "[data-element='my-tour-button']",
                        "title": "Click the button",
                        "content": "Click the button",
                        "event": "click"
                    }
                ]
            };

            vm.startTour = startTour;

            function startTour() {
                tourService.startTour(vm.tour);
            }

        }

        angular.module("umbraco").controller("My.TourController", TourController);

    })();
</pre>

<h1><b>Custom step views</b></h1>
In some cases you will need a custom view for one of your tour steps. 
This could be for validation or for running any other custom logic for that step. 
We have added a couple of helper components to make it easier to get the step scaffolding to look like a regular tour step. 
In the following example you see how to run some custom logic before a step goes to the next step.

<h3>Markup example - custom step view</h3>
<pre>
    <div ng-controller="My.TourStep as vm">

        <umb-tour-step on-close="model.endTour()">
                
            <umb-tour-step-header
                title="model.currentStep.title">
            </umb-tour-step-header>
            
            <umb-tour-step-content
                content="model.currentStep.content">

                <!-- Add any custom content here  -->

            </umb-tour-step-content>

            <umb-tour-step-footer class="flex justify-between items-center">

                <umb-tour-step-counter
                    current-step="model.currentStepIndex + 1"
                    total-steps="model.steps.length">
                </umb-tour-step-counter>

                <div>
                    <umb-button 
                        size="xs" 
                        button-style="success" 
                        type="button" 
                        action="vm.initNextStep()" 
                        label="Next">
                    </umb-button>
                </div>

            </umb-tour-step-footer>

        </umb-tour-step>

    </div>
</pre>

<h3>Controller example - custom step view</h3>
<pre>
    (function () {
        "use strict";

        function StepController() {

            var vm = this;
            
            vm.initNextStep = initNextStep;

            function initNextStep() {
                // run logic here before going to the next step
                $scope.model.nextStep();
            }

        }

        angular.module("umbraco").controller("My.TourStep", StepController);

    })();
</pre>


<h3>Related services</h3>
<ul>
    <li>{@link umbraco.services.tourService tourService}</li>
</ul>

@param {string} model (<code>binding</code>): Tour object

**/
    (function () {
        'use strict';
        function TourDirective($timeout, $http, $q, tourService, backdropService) {
            function link(scope, el, attr, ctrl) {
                var popover;
                var pulseElement;
                var pulseTimer;
                scope.loadingStep = false;
                scope.elementNotFound = false;
                scope.model.nextStep = function () {
                    nextStep();
                };
                scope.model.endTour = function () {
                    unbindEvent();
                    tourService.endTour(scope.model);
                    backdropService.close();
                };
                scope.model.completeTour = function () {
                    unbindEvent();
                    tourService.completeTour(scope.model).then(function () {
                        backdropService.close();
                    });
                };
                scope.model.disableTour = function () {
                    unbindEvent();
                    tourService.disableTour(scope.model).then(function () {
                        backdropService.close();
                    });
                };
                function onInit() {
                    popover = el.find('.umb-tour__popover');
                    pulseElement = el.find('.umb-tour__pulse');
                    popover.hide();
                    scope.model.currentStepIndex = 0;
                    backdropService.open({ disableEventsOnClick: true });
                    startStep();
                }
                function setView() {
                    if (scope.model.currentStep.view && scope.model.alias) {
                        //we do this to avoid a hidden dialog to start loading unconfigured views before the first activation
                        var configuredView = scope.model.currentStep.view;
                        if (scope.model.currentStep.view.indexOf('.html') === -1) {
                            var viewAlias = scope.model.currentStep.view.toLowerCase();
                            var tourAlias = scope.model.alias.toLowerCase();
                            configuredView = 'views/common/tours/' + tourAlias + '/' + viewAlias + '/' + viewAlias + '.html';
                        }
                        if (configuredView !== scope.configuredView) {
                            scope.configuredView = configuredView;
                        }
                    } else {
                        scope.configuredView = null;
                    }
                }
                function nextStep() {
                    popover.hide();
                    pulseElement.hide();
                    $timeout.cancel(pulseTimer);
                    scope.model.currentStepIndex++;
                    // make sure we don't go too far
                    if (scope.model.currentStepIndex !== scope.model.steps.length) {
                        startStep();    // tour completed - final step
                    } else {
                        scope.loadingStep = true;
                        waitForPendingRerequests().then(function () {
                            scope.loadingStep = false;
                            // clear current step
                            scope.model.currentStep = {};
                            // set popover position to center
                            setPopoverPosition(null);
                            // remove backdrop hightlight and custom opacity
                            backdropService.setHighlight(null);
                            backdropService.setOpacity(null);
                        });
                    }
                }
                function startStep() {
                    scope.loadingStep = true;
                    backdropService.setOpacity(scope.model.steps[scope.model.currentStepIndex].backdropOpacity);
                    backdropService.setHighlight(null);
                    waitForPendingRerequests().then(function () {
                        scope.model.currentStep = scope.model.steps[scope.model.currentStepIndex];
                        setView();
                        // if highlight element is set - find it
                        findHighlightElement();
                        // if a custom event needs to be bound we do it now
                        if (scope.model.currentStep.event) {
                            bindEvent();
                        }
                        scope.loadingStep = false;
                    });
                }
                function findHighlightElement() {
                    scope.elementNotFound = false;
                    $timeout(function () {
                        // clear element when step as marked as intro, so it always displays in the center
                        if (scope.model.currentStep && scope.model.currentStep.type === 'intro') {
                            scope.model.currentStep.element = null;
                            scope.model.currentStep.eventElement = null;
                            scope.model.currentStep.event = null;
                        }
                        // if an element isn't set - show the popover in the center
                        if (scope.model.currentStep && !scope.model.currentStep.element) {
                            setPopoverPosition(null);
                            return;
                        }
                        var element = angular.element(scope.model.currentStep.element);
                        // we couldn't find the element in the dom - abort and show error
                        if (element.length === 0) {
                            scope.elementNotFound = true;
                            setPopoverPosition(null);
                            return;
                        }
                        var scrollParent = element.scrollParent();
                        var scrollToCenterOfContainer = element[0].offsetTop - scrollParent[0].clientHeight / 2 + element[0].clientHeight / 2;
                        // Detect if scroll is needed
                        if (element[0].offsetTop > scrollParent[0].clientHeight) {
                            scrollParent.animate({ scrollTop: scrollToCenterOfContainer }, function () {
                                // Animation complete.
                                setPopoverPosition(element);
                                setPulsePosition();
                                backdropService.setHighlight(scope.model.currentStep.element, scope.model.currentStep.elementPreventClick);
                            });
                        } else {
                            setPopoverPosition(element);
                            setPulsePosition();
                            backdropService.setHighlight(scope.model.currentStep.element, scope.model.currentStep.elementPreventClick);
                        }
                    });
                }
                function setPopoverPosition(element) {
                    $timeout(function () {
                        var position = 'center';
                        var margin = 20;
                        var css = {};
                        var popoverWidth = popover.outerWidth();
                        var popoverHeight = popover.outerHeight();
                        var popoverOffset = popover.offset();
                        var documentWidth = angular.element(document).width();
                        var documentHeight = angular.element(document).height();
                        if (element) {
                            var offset = element.offset();
                            var width = element.outerWidth();
                            var height = element.outerHeight();
                            // messure available space on each side of the target element
                            var space = {
                                'top': offset.top,
                                'right': documentWidth - (offset.left + width),
                                'bottom': documentHeight - (offset.top + height),
                                'left': offset.left
                            };
                            // get the posistion with most available space
                            position = findMax(space);
                            if (position === 'top') {
                                if (offset.left < documentWidth / 2) {
                                    css.top = offset.top - popoverHeight - margin;
                                    css.left = offset.left;
                                } else {
                                    css.top = offset.top - popoverHeight - margin;
                                    css.left = offset.left - popoverWidth + width;
                                }
                            }
                            if (position === 'right') {
                                if (offset.top < documentHeight / 2) {
                                    css.top = offset.top;
                                    css.left = offset.left + width + margin;
                                } else {
                                    css.top = offset.top + height - popoverHeight;
                                    css.left = offset.left + width + margin;
                                }
                            }
                            if (position === 'bottom') {
                                if (offset.left < documentWidth / 2) {
                                    css.top = offset.top + height + margin;
                                    css.left = offset.left;
                                } else {
                                    css.top = offset.top + height + margin;
                                    css.left = offset.left - popoverWidth + width;
                                }
                            }
                            if (position === 'left') {
                                if (offset.top < documentHeight / 2) {
                                    css.top = offset.top;
                                    css.left = offset.left - popoverWidth - margin;
                                } else {
                                    css.top = offset.top + height - popoverHeight;
                                    css.left = offset.left - popoverWidth - margin;
                                }
                            }
                        } else {
                            // if there is no dom element center the popover
                            css.top = 'calc(50% - ' + popoverHeight / 2 + 'px)';
                            css.left = 'calc(50% - ' + popoverWidth / 2 + 'px)';
                        }
                        popover.css(css).fadeIn('fast');
                    });
                }
                function setPulsePosition() {
                    if (scope.model.currentStep.event) {
                        pulseTimer = $timeout(function () {
                            var clickElementSelector = scope.model.currentStep.eventElement ? scope.model.currentStep.eventElement : scope.model.currentStep.element;
                            var clickElement = $(clickElementSelector);
                            var offset = clickElement.offset();
                            var width = clickElement.outerWidth();
                            var height = clickElement.outerHeight();
                            pulseElement.css({
                                'width': width,
                                'height': height,
                                'left': offset.left,
                                'top': offset.top
                            });
                            pulseElement.fadeIn();
                        }, 1000);
                    }
                }
                function waitForPendingRerequests() {
                    var deferred = $q.defer();
                    var timer = window.setInterval(function () {
                        // check for pending requests both in angular and on the document
                        if ($http.pendingRequests.length === 0 && document.readyState === 'complete') {
                            $timeout(function () {
                                deferred.resolve();
                                clearInterval(timer);
                            });
                        }
                    }, 50);
                    return deferred.promise;
                }
                function findMax(obj) {
                    var keys = Object.keys(obj);
                    var max = keys[0];
                    for (var i = 1, n = keys.length; i < n; ++i) {
                        var k = keys[i];
                        if (obj[k] > obj[max]) {
                            max = k;
                        }
                    }
                    return max;
                }
                function bindEvent() {
                    var bindToElement = scope.model.currentStep.element;
                    var eventName = scope.model.currentStep.event + '.step-' + scope.model.currentStepIndex;
                    var removeEventName = 'remove.step-' + scope.model.currentStepIndex;
                    var handled = false;
                    if (scope.model.currentStep.eventElement) {
                        bindToElement = scope.model.currentStep.eventElement;
                    }
                    $(bindToElement).on(eventName, function () {
                        if (!handled) {
                            unbindEvent();
                            nextStep();
                            handled = true;
                        }
                    });
                    // Hack: we do this to handle cases where ng-if is used and removes the element we need to click.
                    // for some reason it seems the elements gets removed before the event is raised. This is a temp solution which assumes:
                    // "if you ask me to click on an element, and it suddenly gets removed from the dom, let's go on to the next step".
                    $(bindToElement).on(removeEventName, function () {
                        if (!handled) {
                            unbindEvent();
                            nextStep();
                            handled = true;
                        }
                    });
                }
                function unbindEvent() {
                    var eventName = scope.model.currentStep.event + '.step-' + scope.model.currentStepIndex;
                    var removeEventName = 'remove.step-' + scope.model.currentStepIndex;
                    if (scope.model.currentStep.eventElement) {
                        angular.element(scope.model.currentStep.eventElement).off(eventName);
                        angular.element(scope.model.currentStep.eventElement).off(removeEventName);
                    } else {
                        angular.element(scope.model.currentStep.element).off(eventName);
                        angular.element(scope.model.currentStep.element).off(removeEventName);
                    }
                }
                function resize() {
                    findHighlightElement();
                }
                onInit();
                $(window).on('resize.umbTour', resize);
                scope.$on('$destroy', function () {
                    $(window).off('resize.umbTour');
                    unbindEvent();
                    $timeout.cancel(pulseTimer);
                });
            }
            var directive = {
                transclude: true,
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/application/umb-tour.html',
                link: link,
                scope: { model: '=' }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbTour', TourDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbTourStep
@restrict E
@scope

@description
<b>Added in Umbraco 7.8</b>. The tour step component is a component that can be used in custom views for tour steps.

@param {callback} onClose The callback which should be performened when the close button of the tour step is clicked
@param {boolean=} hideClose A boolean indicating if the close button needs to be shown

**/
    (function () {
        'use strict';
        function TourStepDirective() {
            function link(scope, element, attrs, ctrl) {
                scope.close = function () {
                    if (scope.onClose) {
                        scope.onClose();
                    }
                };
            }
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/application/umbtour/umb-tour-step.html',
                scope: {
                    size: '@?',
                    onClose: '&?',
                    hideClose: '=?'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbTourStep', TourStepDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbTourStepContent
@restrict E
@scope

@description
<b>Added in Umbraco 7.8</b>. The tour step content component is a component that can be used in custom views for tour steps.
It's meant to be used in the umb-tour-step directive.
All markup in the body of the directive will be shown after the content attribute

@param {string} content The content that needs to be shown
**/
    (function () {
        'use strict';
        function TourStepContentDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/application/umbtour/umb-tour-step-content.html',
                scope: { content: '=' }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbTourStepContent', TourStepContentDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbTourStepCounter
@restrict E
@scope

@description
<b>Added in Umbraco 7.8</b>. The tour step counter component is a component that can be used in custom views for tour steps.
It's meant to be used in the umb-tour-step-footer directive. It will show the progress you have made in a tour eg. step 2/12


@param {int} currentStep The current step the tour is on
@param {int} totalSteps The current step the tour is on
**/
    (function () {
        'use strict';
        function TourStepCounterDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/application/umbtour/umb-tour-step-counter.html',
                scope: {
                    currentStep: '=',
                    totalSteps: '='
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbTourStepCounter', TourStepCounterDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbTourStepFooter
@restrict E
@scope

@description
<b>Added in Umbraco 7.8</b>. The tour step footer component is a component that can be used in custom views for tour steps. It's meant to be used in the umb-tour-step directive.
All markup in the body of the directive will be shown as the footer of the tour step


**/
    (function () {
        'use strict';
        function TourStepFooterDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/application/umbtour/umb-tour-step-footer.html'
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbTourStepFooter', TourStepFooterDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbTourStepHeader
@restrict E
@scope

@description
<b>Added in Umbraco 7.8</b>. The tour step header component is a component that can be used in custom views for tour steps. It's meant to be used in the umb-tour-step directive.


@param {string} title The title that needs to be shown
**/
    (function () {
        'use strict';
        function TourStepHeaderDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/application/umbtour/umb-tour-step-header.html',
                scope: { title: '=' }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbTourStepHeader', TourStepHeaderDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbButton
@restrict E
@scope

@description
Use this directive to render an umbraco button. The directive can be used to generate all types of buttons, set type, style, translation, shortcut and much more.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <umb-button
            action="vm.clickButton()"
            type="button"
            button-style="success"
            state="vm.buttonState"
            shortcut="ctrl+c"
            label="My button"
            disabled="vm.buttonState === 'busy'">
        </umb-button>

    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {
        "use strict";

        function Controller(myService) {

            var vm = this;
            vm.buttonState = "init";

            vm.clickButton = clickButton;

            function clickButton() {

                vm.buttonState = "busy";

                myService.clickButton().then(function() {
                    vm.buttonState = "success";
                }, function() {
                    vm.buttonState = "error";
                });

            }
        }

        angular.module("umbraco").controller("My.Controller", Controller);

    })();
</pre>

@param {callback} action The button action which should be performed when the button is clicked.
@param {string=} href Url/Path to navigato to.
@param {string=} type Set the button type ("button" or "submit").
@param {string=} buttonStyle Set the style of the button. The directive uses the default bootstrap styles ("primary", "info", "success", "warning", "danger", "inverse", "link", "block"). Pass in array to add multple styles [success,block].
@param {string=} state Set a progress state on the button ("init", "busy", "success", "error").
@param {string=} shortcut Set a keyboard shortcut for the button ("ctrl+c").
@param {string=} label Set the button label.
@param {string=} labelKey Set a localization key to make a multi lingual button ("general_buttonText").
@param {string=} icon Set a button icon.
@param {string=} size Set a button icon ("xs", "m", "l", "xl").
@param {boolean=} disabled Set to <code>true</code> to disable the button.
**/
    (function () {
        'use strict';
        function ButtonDirective($timeout) {
            function link(scope, el, attr, ctrl) {
                scope.style = null;
                scope.innerState = 'init';
                function activate() {
                    scope.blockElement = false;
                    if (scope.buttonStyle) {
                        // make it possible to pass in multiple styles
                        if (scope.buttonStyle.startsWith('[') && scope.buttonStyle.endsWith(']')) {
                            // when using an attr it will always be a string so we need to remove square brackets
                            // and turn it into and array
                            var withoutBrackets = scope.buttonStyle.replace(/[\[\]']+/g, '');
                            // split array by , + make sure to catch whitespaces
                            var array = withoutBrackets.split(/\s?,\s?/g);
                            angular.forEach(array, function (item) {
                                scope.style = scope.style + ' ' + 'btn-' + item;
                                if (item === 'block') {
                                    scope.blockElement = true;
                                }
                            });
                        } else {
                            scope.style = 'btn-' + scope.buttonStyle;
                            if (scope.buttonStyle === 'block') {
                                scope.blockElement = true;
                            }
                        }
                    }
                }
                activate();
                var unbindStateWatcher = scope.$watch('state', function (newValue, oldValue) {
                    if (newValue) {
                        scope.innerState = newValue;
                    }
                    if (newValue === 'success' || newValue === 'error') {
                        $timeout(function () {
                            scope.innerState = 'init';
                        }, 2000);
                    }
                });
                scope.$on('$destroy', function () {
                    unbindStateWatcher();
                });
            }
            var directive = {
                transclude: true,
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/buttons/umb-button.html',
                link: link,
                scope: {
                    action: '&?',
                    href: '@?',
                    type: '@',
                    buttonStyle: '@?',
                    state: '=?',
                    shortcut: '@?',
                    shortcutWhenHidden: '@',
                    label: '@?',
                    labelKey: '@?',
                    icon: '@?',
                    disabled: '=',
                    size: '@?',
                    alias: '@?'
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbButton', ButtonDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbButtonGroup
@restrict E
@scope

@description
Use this directive to render a button with a dropdown of alternative actions.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <umb-button-group
           ng-if="vm.buttonGroup"
           default-button="vm.buttonGroup.defaultButton"
           sub-buttons="vm.buttonGroup.subButtons"
           direction="down"
           float="right">
        </umb-button-group>

    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {
        "use strict";

        function Controller() {

            var vm = this;

            vm.buttonGroup = {
                defaultButton: {
                    labelKey: "general_defaultButton",
                    hotKey: "ctrl+d",
                    hotKeyWhenHidden: true,
                    handler: function() {
                        // do magic here
                    }
                },
                subButtons: [
                    {
                        labelKey: "general_subButton",
                        hotKey: "ctrl+b",
                        hotKeyWhenHidden: true,
                        handler: function() {
                            // do magic here
                        }
                    }
                ]
            };
        }

        angular.module("umbraco").controller("My.Controller", Controller);

    })();
</pre>

<h3>Button model description</h3>
<ul>
    <li>
        <strong>labekKey</strong>
        <small>(string)</small> -
        Set a localization key to make a multi lingual button ("general_buttonText").
    </li>
    <li>
        <strong>hotKey</strong>
        <small>(array)</small> -
        Set a keyboard shortcut for the button ("ctrl+c").
    </li>
    <li>
        <strong>hotKeyWhenHidden</strong>
        <small>(boolean)</small> -
        As a default the hotkeys only works on elements visible in the UI. Set to <code>true</code> to set a hotkey on the hidden sub buttons.
    </li>
    <li>
        <strong>handler</strong>
        <small>(callback)</small> -
        Set a callback to handle button click events.
    </li>
</ul>

@param {object} defaultButton The model of the default button.
@param {array} subButtons Array of sub buttons.
@param {string=} state Set a progress state on the button ("init", "busy", "success", "error").
@param {string=} direction Set the direction of the dropdown ("up", "down").
@param {string=} float Set the float of the dropdown. ("left", "right").
**/
    (function () {
        'use strict';
        function ButtonGroupDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/buttons/umb-button-group.html',
                scope: {
                    defaultButton: '=',
                    subButtons: '=',
                    state: '=?',
                    direction: '@?',
                    float: '@?'
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbButtonGroup', ButtonGroupDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbToggle
@restrict E
@scope

@description
<b>Added in Umbraco version 7.7.0</b> Use this directive to render an umbraco toggle.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <umb-toggle
            checked="vm.checked"
            on-click="vm.toggle()">
        </umb-toggle>

        <umb-toggle
            checked="vm.checked"
            disabled="vm.disabled"
            on-click="vm.toggle()"
            show-labels="true"
            label-on="Start"
            label-off="Stop"
            label-position="right"
            hide-icons="true">
        </umb-toggle>

    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {
        "use strict";

        function Controller() {

            var vm = this;
            vm.checked = false;
            vm.disabled = false;

            vm.toggle = toggle;

            function toggle() {
                vm.checked = !vm.checked;
            }
        }

        angular.module("umbraco").controller("My.Controller", Controller);

    })();
</pre>

@param {boolean} checked Set to <code>true</code> or <code>false</code> to toggle the switch.
@param {boolean} disabled Set to <code>true</code> or <code>false</code> to disable/enable the switch.
@param {callback} onClick The function which should be called when the toggle is clicked.
@param {string=} showLabels Set to <code>true</code> or <code>false</code> to show a "On" or "Off" label next to the switch.
@param {string=} labelOn Set a custom label for when the switched is turned on. It will default to "On".
@param {string=} labelOff Set a custom label for when the switched is turned off. It will default to "Off".
@param {string=} labelPosition Sets the label position to the left or right of the switch. It will default to "left" ("left", "right").
@param {string=} hideIcons Set to <code>true</code> or <code>false</code> to hide the icons on the switch.

**/
    (function () {
        'use strict';
        function ToggleDirective(localizationService, eventsService) {
            function link(scope, el, attr, ctrl) {
                scope.displayLabelOn = '';
                scope.displayLabelOff = '';
                function onInit() {
                    setLabelText();
                    eventsService.emit('toggleValue', { value: scope.checked });
                }
                function setLabelText() {
                    // set default label for "on"
                    if (scope.labelOn) {
                        scope.displayLabelOn = scope.labelOn;
                    } else {
                        localizationService.localize('general_on').then(function (value) {
                            scope.displayLabelOn = value;
                        });
                    }
                    // set default label for "Off"
                    if (scope.labelOff) {
                        scope.displayLabelOff = scope.labelOff;
                    } else {
                        localizationService.localize('general_off').then(function (value) {
                            scope.displayLabelOff = value;
                        });
                    }
                }
                scope.click = function () {
                    if (scope.onClick) {
                        eventsService.emit('toggleValue', { value: !scope.checked });
                        scope.onClick();
                    }
                };
                onInit();
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/buttons/umb-toggle.html',
                scope: {
                    checked: '=',
                    disabled: '=',
                    onClick: '&',
                    labelOn: '@?',
                    labelOff: '@?',
                    labelPosition: '@?',
                    showLabels: '@?',
                    hideIcons: '@?'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbToggle', ToggleDirective);
    }());
    (function () {
        'use strict';
        function ContentEditController($rootScope, $scope, $routeParams, $q, $timeout, $window, $location, appState, contentResource, entityResource, navigationService, notificationsService, angularHelper, serverValidationManager, contentEditingHelper, treeService, fileManager, formHelper, umbRequestHelper, keyboardService, umbModelMapper, editorState, $http, eventsService, relationResource) {
            var evts = [];
            //setup scope vars
            $scope.defaultButton = null;
            $scope.subButtons = [];
            $scope.page = {};
            $scope.page.loading = false;
            $scope.page.menu = {};
            $scope.page.menu.currentNode = null;
            $scope.page.menu.currentSection = appState.getSectionState('currentSection');
            $scope.page.listViewPath = null;
            $scope.page.isNew = $scope.isNew ? true : false;
            $scope.page.buttonGroupState = 'init';
            $scope.allowOpen = true;
            function init(content) {
                createButtons(content);
                editorState.set($scope.content);
                //We fetch all ancestors of the node to generate the footer breadcrumb navigation
                if (!$scope.page.isNew) {
                    if (content.parentId && content.parentId !== -1) {
                        entityResource.getAncestors(content.id, 'document').then(function (anc) {
                            $scope.ancestors = anc;
                        });
                    }
                }
                evts.push(eventsService.on('editors.content.changePublishDate', function (event, args) {
                    createButtons(args.node);
                }));
                evts.push(eventsService.on('editors.content.changeUnpublishDate', function (event, args) {
                    createButtons(args.node);
                }));
                // We don't get the info tab from the server from version 7.8 so we need to manually add it
                contentEditingHelper.addInfoTab($scope.content.tabs);
            }
            function getNode() {
                $scope.page.loading = true;
                //we are editing so get the content item from the server
                $scope.getMethod()($scope.contentId).then(function (data) {
                    $scope.content = data;
                    if (data.isChildOfListView && data.trashed === false) {
                        $scope.page.listViewPath = $routeParams.page ? '/content/content/edit/' + data.parentId + '?page=' + $routeParams.page : '/content/content/edit/' + data.parentId;
                    }
                    init($scope.content);
                    //in one particular special case, after we've created a new item we redirect back to the edit
                    // route but there might be server validation errors in the collection which we need to display
                    // after the redirect, so we will bind all subscriptions which will show the server validation errors
                    // if there are any and then clear them so the collection no longer persists them.
                    serverValidationManager.executeAndClearAllSubscriptions();
                    syncTreeNode($scope.content, data.path, true);
                    resetLastListPageNumber($scope.content);
                    eventsService.emit('content.loaded', { content: $scope.content });
                    $scope.page.loading = false;
                });
            }
            function createButtons(content) {
                $scope.page.buttonGroupState = 'init';
                var buttons = contentEditingHelper.configureContentEditorButtons({
                    create: $scope.page.isNew,
                    content: content,
                    methods: {
                        saveAndPublish: $scope.saveAndPublish,
                        sendToPublish: $scope.sendToPublish,
                        save: $scope.save,
                        unPublish: $scope.unPublish
                    }
                });
                $scope.defaultButton = buttons.defaultButton;
                $scope.subButtons = buttons.subButtons;
            }
            /** Syncs the content item to it's tree node - this occurs on first load and after saving */
            function syncTreeNode(content, path, initialLoad) {
                if (!$scope.content.isChildOfListView) {
                    navigationService.syncTree({
                        tree: $scope.treeAlias,
                        path: path.split(','),
                        forceReload: initialLoad !== true
                    }).then(function (syncArgs) {
                        $scope.page.menu.currentNode = syncArgs.node;
                    });
                } else if (initialLoad === true) {
                    //it's a child item, just sync the ui node to the parent
                    navigationService.syncTree({
                        tree: $scope.treeAlias,
                        path: path.substring(0, path.lastIndexOf(',')).split(','),
                        forceReload: initialLoad !== true
                    });
                    //if this is a child of a list view and it's the initial load of the editor, we need to get the tree node 
                    // from the server so that we can load in the actions menu.
                    umbRequestHelper.resourcePromise($http.get(content.treeNodeUrl), 'Failed to retrieve data for child node ' + content.id).then(function (node) {
                        $scope.page.menu.currentNode = node;
                    });
                }
            }
            // This is a helper method to reduce the amount of code repitition for actions: Save, Publish, SendToPublish
            function performSave(args) {
                var deferred = $q.defer();
                $scope.page.buttonGroupState = 'busy';
                eventsService.emit('content.saving', {
                    content: $scope.content,
                    action: args.action
                });
                contentEditingHelper.contentEditorPerformSave({
                    statusMessage: args.statusMessage,
                    saveMethod: args.saveMethod,
                    scope: $scope,
                    content: $scope.content,
                    action: args.action
                }).then(function (data) {
                    //success            
                    init($scope.content);
                    syncTreeNode($scope.content, data.path);
                    $scope.page.buttonGroupState = 'success';
                    deferred.resolve(data);
                    eventsService.emit('content.saved', {
                        content: $scope.content,
                        action: args.action
                    });
                }, function (err) {
                    //error
                    if (err) {
                        editorState.set($scope.content);
                    }
                    $scope.page.buttonGroupState = 'error';
                    deferred.reject(err);
                });
                return deferred.promise;
            }
            function resetLastListPageNumber(content) {
                // We're using rootScope to store the page number for list views, so if returning to the list
                // we can restore the page.  If we've moved on to edit a piece of content that's not the list or it's children
                // we should remove this so as not to confuse if navigating to a different list
                if (!content.isChildOfListView && !content.isContainer) {
                    $rootScope.lastListViewPageViewed = null;
                }
            }
            if ($scope.page.isNew) {
                $scope.page.loading = true;
                //we are creating so get an empty content item
                $scope.getScaffoldMethod()().then(function (data) {
                    $scope.content = data;
                    init($scope.content);
                    resetLastListPageNumber($scope.content);
                    $scope.page.loading = false;
                    eventsService.emit('content.newReady', { content: $scope.content });
                });
            } else {
                getNode();
            }
            $scope.unPublish = function () {
                // raising the event triggers the confirmation dialog			
                if (!notificationsService.hasView()) {
                    notificationsService.add({ view: 'confirmunpublish' });
                }
                $scope.page.buttonGroupState = 'busy';
                // actioning the dialog raises the confirmUnpublish event, act on it here
                var actioned = $rootScope.$on('content.confirmUnpublish', function (event, confirmed) {
                    if (confirmed && formHelper.submitForm({
                            scope: $scope,
                            statusMessage: 'Unpublishing...',
                            skipValidation: true
                        })) {
                        eventsService.emit('content.unpublishing', { content: $scope.content });
                        contentResource.unPublish($scope.content.id).then(function (data) {
                            formHelper.resetForm({
                                scope: $scope,
                                notifications: data.notifications
                            });
                            contentEditingHelper.handleSuccessfulSave({
                                scope: $scope,
                                savedContent: data,
                                rebindCallback: contentEditingHelper.reBindChangedProperties($scope.content, data)
                            });
                            init($scope.content);
                            syncTreeNode($scope.content, data.path);
                            $scope.page.buttonGroupState = 'success';
                            eventsService.emit('content.unpublished', { content: $scope.content });
                        }, function (err) {
                            formHelper.showNotifications(err.data);
                            $scope.page.buttonGroupState = 'error';
                        });
                    } else {
                        $scope.page.buttonGroupState = 'init';
                    }
                    // unsubscribe to avoid queueing notifications
                    // listener is re-bound when the unpublish button is clicked so it is created just-in-time				
                    actioned();
                });
            };
            $scope.sendToPublish = function () {
                return performSave({
                    saveMethod: contentResource.sendToPublish,
                    statusMessage: 'Sending...',
                    action: 'sendToPublish'
                });
            };
            $scope.saveAndPublish = function () {
                return performSave({
                    saveMethod: contentResource.publish,
                    statusMessage: 'Publishing...',
                    action: 'publish'
                });
            };
            $scope.save = function () {
                return performSave({
                    saveMethod: $scope.saveMethod(),
                    statusMessage: 'Saving...',
                    action: 'save'
                });
            };
            $scope.preview = function (content) {
                if (!$scope.busy) {
                    // Chromes popup blocker will kick in if a window is opened 
                    // without the initial scoped request. This trick will fix that.
                    //  
                    var previewWindow = $window.open('preview/?init=true&id=' + content.id, 'umbpreview');
                    // Build the correct path so both /#/ and #/ work.
                    var redirect = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath + '/preview/?id=' + content.id;
                    //The user cannot save if they don't have access to do that, in which case we just want to preview
                    //and that's it otherwise they'll get an unauthorized access message
                    if (!_.contains(content.allowedActions, 'A')) {
                        previewWindow.location.href = redirect;
                    } else {
                        $scope.save().then(function (data) {
                            previewWindow.location.href = redirect;
                        });
                    }
                }
            };
            $scope.restore = function (content) {
                $scope.page.buttonRestore = 'busy';
                relationResource.getByChildId(content.id, 'relateParentDocumentOnDelete').then(function (data) {
                    var relation = null;
                    var target = null;
                    var error = {
                        headline: 'Cannot automatically restore this item',
                        content: 'Use the Move menu item to move it manually'
                    };
                    if (data.length == 0) {
                        notificationsService.error(error.headline, 'There is no \'restore\' relation found for this node. Use the Move menu item to move it manually.');
                        $scope.page.buttonRestore = 'error';
                        return;
                    }
                    relation = data[0];
                    if (relation.parentId == -1) {
                        target = {
                            id: -1,
                            name: 'Root'
                        };
                        moveNode(content, target);
                    } else {
                        contentResource.getById(relation.parentId).then(function (data) {
                            target = data;
                            // make sure the target item isn't in the recycle bin
                            if (target.path.indexOf('-20') !== -1) {
                                notificationsService.error(error.headline, 'The item you want to restore it under (' + target.name + ') is in the recycle bin. Use the Move menu item to move the item manually.');
                                $scope.page.buttonRestore = 'error';
                                return;
                            }
                            moveNode(content, target);
                        }, function (err) {
                            $scope.page.buttonRestore = 'error';
                            notificationsService.error(error.headline, error.content);
                        });
                    }
                }, function (err) {
                    $scope.page.buttonRestore = 'error';
                    notificationsService.error(error.headline, error.content);
                });
            };
            function moveNode(node, target) {
                contentResource.move({
                    'parentId': target.id,
                    'id': node.id
                }).then(function (path) {
                    // remove the node that we're working on
                    if ($scope.page.menu.currentNode) {
                        treeService.removeNode($scope.page.menu.currentNode);
                    }
                    // sync the destination node
                    navigationService.syncTree({
                        tree: 'content',
                        path: path,
                        forceReload: true,
                        activate: false
                    });
                    $scope.page.buttonRestore = 'success';
                    notificationsService.success('Successfully restored ' + node.name + ' to ' + target.name);
                    // reload the node
                    getNode();
                }, function (err) {
                    $scope.page.buttonRestore = 'error';
                    notificationsService.error('Cannot automatically restore this item', err);
                });
            }
            //ensure to unregister from all events!
            $scope.$on('$destroy', function () {
                for (var e in evts) {
                    eventsService.unsubscribe(evts[e]);
                }
            });
        }
        function createDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/content/edit.html',
                controller: 'Umbraco.Editors.Content.EditorDirectiveController',
                scope: {
                    contentId: '=',
                    isNew: '=?',
                    treeAlias: '@',
                    page: '=?',
                    saveMethod: '&',
                    getMethod: '&',
                    getScaffoldMethod: '&?'
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').controller('Umbraco.Editors.Content.EditorDirectiveController', ContentEditController);
        angular.module('umbraco.directives').directive('contentEditor', createDirective);
    }());
    (function () {
        'use strict';
        function ContentNodeInfoDirective($timeout, $location, logResource, eventsService, userService, localizationService, dateHelper, redirectUrlsResource) {
            function link(scope, element, attrs, ctrl) {
                var evts = [];
                var isInfoTab = false;
                scope.publishStatus = {};
                scope.disableTemplates = Umbraco.Sys.ServerVariables.features.disabledFeatures.disableTemplates;
                function onInit() {
                    // If logged in user has access to the settings section
                    // show the open anchors - if the user doesn't have 
                    // access, documentType is null, see ContentModelMapper
                    scope.allowOpen = scope.node.documentType !== null;
                    scope.datePickerConfig = {
                        pickDate: true,
                        pickTime: true,
                        useSeconds: false,
                        format: 'YYYY-MM-DD HH:mm',
                        icons: {
                            time: 'icon-time',
                            date: 'icon-calendar',
                            up: 'icon-chevron-up',
                            down: 'icon-chevron-down'
                        }
                    };
                    scope.auditTrailOptions = { 'id': scope.node.id };
                    // get available templates
                    scope.availableTemplates = scope.node.allowedTemplates;
                    // get document type details
                    scope.documentType = scope.node.documentType;
                    // make sure dates are formatted to the user's locale
                    formatDatesToLocal();
                    // Make sure to set the node status
                    setNodePublishStatus(scope.node);
                    //default setting for redirect url management
                    scope.urlTrackerDisabled = false;
                    // Declare a fallback URL for the <umb-node-preview/> directive
                    if (scope.documentType !== null) {
                        scope.previewOpenUrl = '#/settings/documenttypes/edit/' + scope.documentType.id;
                    }
                    // only allow configuring scheduled publishing if the user has publish ("U") and unpublish ("Z") permissions on this node
                    scope.allowScheduledPublishing = _.contains(scope.node.allowedActions, 'U') && _.contains(scope.node.allowedActions, 'Z');
                    ensureUniqueUrls();
                }
                // make sure we don't show duplicate URLs in case multiple URL providers assign the same URLs to the content (see issue 3842 for details)
                function ensureUniqueUrls() {
                    scope.node.urls = _.uniq(scope.node.urls);
                }
                scope.auditTrailPageChange = function (pageNumber) {
                    scope.auditTrailOptions.pageNumber = pageNumber;
                    loadAuditTrail();
                };
                scope.openDocumentType = function (documentType) {
                    var url = '/settings/documenttypes/edit/' + documentType.id;
                    $location.url(url);
                };
                scope.openTemplate = function () {
                    var url = '/settings/templates/edit/' + scope.node.templateId;
                    $location.url(url);
                };
                scope.updateTemplate = function (templateAlias) {
                    // update template value
                    scope.node.template = templateAlias;
                };
                scope.datePickerChange = function (event, type) {
                    if (type === 'publish') {
                        setPublishDate(event.date.format('YYYY-MM-DD HH:mm'));
                    } else if (type === 'unpublish') {
                        setUnpublishDate(event.date.format('YYYY-MM-DD HH:mm'));
                    }
                };
                scope.clearPublishDate = function () {
                    clearPublishDate();
                };
                scope.clearUnpublishDate = function () {
                    clearUnpublishDate();
                };
                function loadAuditTrail() {
                    scope.loadingAuditTrail = true;
                    logResource.getPagedEntityLog(scope.auditTrailOptions).then(function (data) {
                        // get current backoffice user and format dates
                        userService.getCurrentUser().then(function (currentUser) {
                            angular.forEach(data.items, function (item) {
                                item.timestampFormatted = dateHelper.getLocalDate(item.timestamp, currentUser.locale, 'LLL');
                            });
                        });
                        scope.auditTrail = data.items;
                        scope.auditTrailOptions.pageNumber = data.pageNumber;
                        scope.auditTrailOptions.pageSize = data.pageSize;
                        scope.auditTrailOptions.totalItems = data.totalItems;
                        scope.auditTrailOptions.totalPages = data.totalPages;
                        setAuditTrailLogTypeColor(scope.auditTrail);
                        scope.loadingAuditTrail = false;
                    });
                }
                function loadRedirectUrls() {
                    scope.loadingRedirectUrls = true;
                    //check if Redirect Url Management is enabled
                    redirectUrlsResource.getEnableState().then(function (response) {
                        scope.urlTrackerDisabled = response.enabled !== true;
                        if (scope.urlTrackerDisabled === false) {
                            redirectUrlsResource.getRedirectsForContentItem(scope.node.udi).then(function (data) {
                                scope.redirectUrls = data.searchResults;
                                scope.hasRedirects = typeof data.searchResults !== 'undefined' && data.searchResults.length > 0;
                                scope.loadingRedirectUrls = false;
                            });
                        } else {
                            scope.loadingRedirectUrls = false;
                        }
                    });
                }
                function setAuditTrailLogTypeColor(auditTrail) {
                    angular.forEach(auditTrail, function (item) {
                        switch (item.logType) {
                        case 'Publish':
                            item.logTypeColor = 'success';
                            break;
                        case 'UnPublish':
                        case 'Delete':
                            item.logTypeColor = 'danger';
                            break;
                        default:
                            item.logTypeColor = 'gray';
                        }
                    });
                }
                function setNodePublishStatus(node) {
                    // deleted node
                    if (node.trashed === true) {
                        scope.publishStatus.label = localizationService.localize('general_deleted');
                        scope.publishStatus.color = 'danger';
                    }
                    // unpublished node
                    if (node.published === false && node.trashed === false) {
                        scope.publishStatus.label = localizationService.localize('content_unpublished');
                        scope.publishStatus.color = 'gray';
                    }
                    // published node
                    if (node.hasPublishedVersion === true && node.publishDate && node.published === true) {
                        scope.publishStatus.label = localizationService.localize('content_published');
                        scope.publishStatus.color = 'success';
                    }
                    // published node with pending changes
                    if (node.hasPublishedVersion === true && node.publishDate && node.published === false) {
                        scope.publishStatus.label = localizationService.localize('content_publishedPendingChanges');
                        scope.publishStatus.color = 'success';
                    }
                }
                function setPublishDate(date) {
                    if (!date) {
                        return;
                    }
                    //The date being passed in here is the user's local date/time that they have selected
                    //we need to convert this date back to the server date on the model.
                    var serverTime = dateHelper.convertToServerStringTime(moment(date), Umbraco.Sys.ServerVariables.application.serverTimeOffset);
                    // update publish value
                    scope.node.releaseDate = serverTime;
                    // make sure dates are formatted to the user's locale
                    formatDatesToLocal();
                    // emit event
                    var args = {
                        node: scope.node,
                        date: date
                    };
                    eventsService.emit('editors.content.changePublishDate', args);
                }
                function clearPublishDate() {
                    // update publish value
                    scope.node.releaseDate = null;
                    // emit event
                    var args = {
                        node: scope.node,
                        date: null
                    };
                    eventsService.emit('editors.content.changePublishDate', args);
                }
                function setUnpublishDate(date) {
                    if (!date) {
                        return;
                    }
                    //The date being passed in here is the user's local date/time that they have selected
                    //we need to convert this date back to the server date on the model.
                    var serverTime = dateHelper.convertToServerStringTime(moment(date), Umbraco.Sys.ServerVariables.application.serverTimeOffset);
                    // update publish value
                    scope.node.removeDate = serverTime;
                    // make sure dates are formatted to the user's locale
                    formatDatesToLocal();
                    // emit event
                    var args = {
                        node: scope.node,
                        date: date
                    };
                    eventsService.emit('editors.content.changeUnpublishDate', args);
                }
                function clearUnpublishDate() {
                    // update publish value
                    scope.node.removeDate = null;
                    // emit event
                    var args = {
                        node: scope.node,
                        date: null
                    };
                    eventsService.emit('editors.content.changeUnpublishDate', args);
                }
                function ucfirst(string) {
                    return string.charAt(0).toUpperCase() + string.slice(1);
                }
                function formatDatesToLocal() {
                    // get current backoffice user and format dates
                    userService.getCurrentUser().then(function (currentUser) {
                        scope.node.createDateFormatted = dateHelper.getLocalDate(scope.node.createDate, currentUser.locale, 'LLL');
                        scope.node.releaseDateYear = scope.node.releaseDate ? ucfirst(dateHelper.getLocalDate(scope.node.releaseDate, currentUser.locale, 'YYYY')) : null;
                        scope.node.releaseDateMonth = scope.node.releaseDate ? ucfirst(dateHelper.getLocalDate(scope.node.releaseDate, currentUser.locale, 'MMMM')) : null;
                        scope.node.releaseDateDayNumber = scope.node.releaseDate ? ucfirst(dateHelper.getLocalDate(scope.node.releaseDate, currentUser.locale, 'DD')) : null;
                        scope.node.releaseDateDay = scope.node.releaseDate ? ucfirst(dateHelper.getLocalDate(scope.node.releaseDate, currentUser.locale, 'dddd')) : null;
                        scope.node.releaseDateTime = scope.node.releaseDate ? ucfirst(dateHelper.getLocalDate(scope.node.releaseDate, currentUser.locale, 'HH:mm')) : null;
                        scope.node.removeDateYear = scope.node.removeDate ? ucfirst(dateHelper.getLocalDate(scope.node.removeDate, currentUser.locale, 'YYYY')) : null;
                        scope.node.removeDateMonth = scope.node.removeDate ? ucfirst(dateHelper.getLocalDate(scope.node.removeDate, currentUser.locale, 'MMMM')) : null;
                        scope.node.removeDateDayNumber = scope.node.removeDate ? ucfirst(dateHelper.getLocalDate(scope.node.removeDate, currentUser.locale, 'DD')) : null;
                        scope.node.removeDateDay = scope.node.removeDate ? ucfirst(dateHelper.getLocalDate(scope.node.removeDate, currentUser.locale, 'dddd')) : null;
                        scope.node.removeDateTime = scope.node.removeDate ? ucfirst(dateHelper.getLocalDate(scope.node.removeDate, currentUser.locale, 'HH:mm')) : null;
                    });
                }
                // load audit trail and redirects when on the info tab
                evts.push(eventsService.on('app.tabChange', function (event, args) {
                    $timeout(function () {
                        if (args.id === -1) {
                            isInfoTab = true;
                            loadAuditTrail();
                            loadRedirectUrls();
                        } else {
                            isInfoTab = false;
                        }
                    });
                }));
                // watch for content updates - reload content when node is saved, published etc.
                scope.$watch('node.updateDate', function (newValue, oldValue) {
                    if (!newValue) {
                        return;
                    }
                    if (newValue === oldValue) {
                        return;
                    }
                    if (isInfoTab) {
                        loadAuditTrail();
                        loadRedirectUrls();
                        formatDatesToLocal();
                        setNodePublishStatus(scope.node);
                        ensureUniqueUrls();
                    }
                });
                //ensure to unregister from all events!
                scope.$on('$destroy', function () {
                    for (var e in evts) {
                        eventsService.unsubscribe(evts[e]);
                    }
                });
                onInit();
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/content/umb-content-node-info.html',
                scope: { node: '=' },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbContentNodeInfo', ContentNodeInfoDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbEditorSubHeader
@restrict E

@description
Use this directive to construct a sub header in the main editor window.
The sub header is sticky and will follow along down the page when scrolling.

<h3>Markup example</h3>
<pre>
    <div ng-controller="MySection.Controller as vm">

        <form name="mySectionForm" novalidate>

            <umb-editor-view>

                <umb-editor-container>

                    <umb-editor-sub-header>
                        // sub header content here
                    </umb-editor-sub-header>

                </umb-editor-container>

            </umb-editor-view>

        </form>

    </div>
</pre>

<h3>Use in combination with</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbEditorSubHeaderContentLeft umbEditorSubHeaderContentLeft}</li>
    <li>{@link umbraco.directives.directive:umbEditorSubHeaderContentRight umbEditorSubHeaderContentRight}</li>
    <li>{@link umbraco.directives.directive:umbEditorSubHeaderSection umbEditorSubHeaderSection}</li>
</ul>
**/
    (function () {
        'use strict';
        function EditorSubHeaderDirective() {
            var directive = {
                transclude: true,
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/editor/subheader/umb-editor-sub-header.html'
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbEditorSubHeader', EditorSubHeaderDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbEditorSubHeaderContentLeft
@restrict E

@description
Use this directive to left align content in a sub header in the main editor window.

<h3>Markup example</h3>
<pre>
    <div ng-controller="MySection.Controller as vm">

        <form name="mySectionForm" novalidate>

            <umb-editor-view>

                <umb-editor-container>

                    <umb-editor-sub-header>

                        <umb-editor-sub-header-content-left>
                            // left content here
                        </umb-editor-sub-header-content-left>

                        <umb-editor-sub-header-content-right>
                            // right content here
                        </umb-editor-sub-header-content-right>

                    </umb-editor-sub-header>

                </umb-editor-container>

            </umb-editor-view>

        </form>

    </div>
</pre>

<h3>Use in combination with</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbEditorSubHeader umbEditorSubHeader}</li>
    <li>{@link umbraco.directives.directive:umbEditorSubHeaderContentRight umbEditorSubHeaderContentRight}</li>
    <li>{@link umbraco.directives.directive:umbEditorSubHeaderSection umbEditorSubHeaderSection}</li>
</ul>
**/
    (function () {
        'use strict';
        function EditorSubHeaderContentLeftDirective() {
            var directive = {
                transclude: true,
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/editor/subheader/umb-editor-sub-header-content-left.html'
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbEditorSubHeaderContentLeft', EditorSubHeaderContentLeftDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbEditorSubHeaderContentRight
@restrict E

@description
Use this directive to rigt align content in a sub header in the main editor window.

<h3>Markup example</h3>
<pre>
    <div ng-controller="MySection.Controller as vm">

        <form name="mySectionForm" novalidate>

            <umb-editor-view>

                <umb-editor-container>

                    <umb-editor-sub-header>

                        <umb-editor-sub-header-content-left>
                            // left content here
                        </umb-editor-sub-header-content-left>

                        <umb-editor-sub-header-content-right>
                            // right content here
                        </umb-editor-sub-header-content-right>

                    </umb-editor-sub-header>

                </umb-editor-container>

            </umb-editor-view>

        </form>

    </div>
</pre>

<h3>Use in combination with</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbEditorSubHeader umbEditorSubHeader}</li>
    <li>{@link umbraco.directives.directive:umbEditorSubHeaderContentLeft umbEditorSubHeaderContentLeft}</li>
    <li>{@link umbraco.directives.directive:umbEditorSubHeaderSection umbEditorSubHeaderSection}</li>
</ul>
**/
    (function () {
        'use strict';
        function EditorSubHeaderContentRightDirective() {
            var directive = {
                transclude: true,
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/editor/subheader/umb-editor-sub-header-content-right.html'
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbEditorSubHeaderContentRight', EditorSubHeaderContentRightDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbEditorSubHeaderSection
@restrict E

@description
Use this directive to create sections, divided by borders, in a sub header in the main editor window.

<h3>Markup example</h3>
<pre>
    <div ng-controller="MySection.Controller as vm">

        <form name="mySectionForm" novalidate>

            <umb-editor-view>

                <umb-editor-container>

                    <umb-editor-sub-header>

                        <umb-editor-sub-header-content-right>

                            <umb-editor-sub-header-section>
                                // section content here
                            </umb-editor-sub-header-section>

                            <umb-editor-sub-header-section>
                                // section content here
                            </umb-editor-sub-header-section>

                            <umb-editor-sub-header-section>
                                // section content here
                            </umb-editor-sub-header-section>

                        </umb-editor-sub-header-content-right>

                    </umb-editor-sub-header>

                </umb-editor-container>

            </umb-editor-view>

        </form>

    </div>
</pre>

<h3>Use in combination with</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbEditorSubHeader umbEditorSubHeader}</li>
    <li>{@link umbraco.directives.directive:umbEditorSubHeaderContentLeft umbEditorSubHeaderContentLeft}</li>
    <li>{@link umbraco.directives.directive:umbEditorSubHeaderContentRight umbEditorSubHeaderContentRight}</li>
</ul>
**/
    (function () {
        'use strict';
        function EditorSubHeaderSectionDirective() {
            var directive = {
                transclude: true,
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/editor/subheader/umb-editor-sub-header-section.html'
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbEditorSubHeaderSection', EditorSubHeaderSectionDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbBreadcrumbs
@restrict E
@scope

@description
Use this directive to generate a list of breadcrumbs.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">
        <umb-breadcrumbs
            ng-if="vm.ancestors && vm.ancestors.length > 0"
            ancestors="vm.ancestors"
            entity-type="content">
        </umb-breadcrumbs>
    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {
        "use strict";

        function Controller(myService) {

            var vm = this;
            vm.ancestors = [];

            myService.getAncestors().then(function(ancestors){
                vm.ancestors = ancestors;
            });

        }

        angular.module("umbraco").controller("My.Controller", Controller);
    })();
</pre>

@param {array} ancestors Array of ancestors
@param {string} entityType The content entity type (member, media, content).
@param {callback} Callback when an ancestor is clicked. It will override the default link behaviour.
**/
    (function () {
        'use strict';
        function BreadcrumbsDirective() {
            function link(scope, el, attr, ctrl) {
                scope.allowOnOpen = false;
                scope.open = function (ancestor) {
                    if (scope.onOpen && scope.allowOnOpen) {
                        scope.onOpen({ 'ancestor': ancestor });
                    }
                };
                function onInit() {
                    if ('onOpen' in attr) {
                        scope.allowOnOpen = true;
                    }
                }
                onInit();
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/editor/umb-breadcrumbs.html',
                scope: {
                    ancestors: '=',
                    entityType: '@',
                    onOpen: '&'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbBreadcrumbs', BreadcrumbsDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbEditorContainer
@restrict E

@description
Use this directive to construct a main content area inside the main editor window.

<h3>Markup example</h3>
<pre>
    <div ng-controller="Umbraco.Controller as vm">

        <umb-editor-view>

            <umb-editor-header
                // header configuration>
            </umb-editor-header>

            <umb-editor-container>
                // main content here
            </umb-editor-container>

            <umb-editor-footer>
                // footer content here
            </umb-editor-footer>

        </umb-editor-view>

    </div>
</pre>

<h3>Use in combination with</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbEditorView umbEditorView}</li>
    <li>{@link umbraco.directives.directive:umbEditorHeader umbEditorHeader}</li>
    <li>{@link umbraco.directives.directive:umbEditorFooter umbEditorFooter}</li>
</ul>
**/
    (function () {
        'use strict';
        function EditorContainerDirective(overlayHelper) {
            function link(scope, el, attr, ctrl) {
                scope.numberOfOverlays = 0;
                scope.$watch(function () {
                    return overlayHelper.getNumberOfOverlays();
                }, function (newValue) {
                    scope.numberOfOverlays = newValue;
                });
            }
            var directive = {
                transclude: true,
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/editor/umb-editor-container.html',
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbEditorContainer', EditorContainerDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbEditorFooter
@restrict E

@description
Use this directive to construct a footer inside the main editor window.

<h3>Markup example</h3>
<pre>
    <div ng-controller="MySection.Controller as vm">

        <form name="mySectionForm" novalidate>

            <umb-editor-view>

                <umb-editor-header
                    // header configuration>
                </umb-editor-header>

                <umb-editor-container>
                    // main content here
                </umb-editor-container>

                <umb-editor-footer>
                    // footer content here
                </umb-editor-footer>

            </umb-editor-view>

        </form>

    </div>
</pre>

<h3>Use in combination with</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbEditorView umbEditorView}</li>
    <li>{@link umbraco.directives.directive:umbEditorHeader umbEditorHeader}</li>
    <li>{@link umbraco.directives.directive:umbEditorContainer umbEditorContainer}</li>
    <li>{@link umbraco.directives.directive:umbEditorFooterContentLeft umbEditorFooterContentLeft}</li>
    <li>{@link umbraco.directives.directive:umbEditorFooterContentRight umbEditorFooterContentRight}</li>
</ul>
**/
    (function () {
        'use strict';
        function EditorFooterDirective() {
            var directive = {
                transclude: true,
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/editor/umb-editor-footer.html'
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbEditorFooter', EditorFooterDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbEditorFooterContentLeft
@restrict E

@description
Use this directive to align content left inside the main editor footer.

<h3>Markup example</h3>
<pre>
    <div ng-controller="MySection.Controller as vm">

        <form name="mySectionForm" novalidate>

            <umb-editor-view>

                <umb-editor-footer>

                    <umb-editor-footer-content-left>
                        // align content left
                    </umb-editor-footer-content-left>

                    <umb-editor-footer-content-right>
                        // align content right
                    </umb-editor-footer-content-right>

                </umb-editor-footer>

            </umb-editor-view>

        </form>

    </div>
</pre>

<h3>Use in combination with</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbEditorView umbEditorView}</li>
    <li>{@link umbraco.directives.directive:umbEditorHeader umbEditorHeader}</li>
    <li>{@link umbraco.directives.directive:umbEditorContainer umbEditorContainer}</li>
    <li>{@link umbraco.directives.directive:umbEditorFooter umbEditorFooter}</li>
    <li>{@link umbraco.directives.directive:umbEditorFooterContentRight umbEditorFooterContentRight}</li>
</ul>
**/
    (function () {
        'use strict';
        function EditorFooterContentLeftDirective() {
            var directive = {
                transclude: true,
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/editor/umb-editor-footer-content-left.html'
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbEditorFooterContentLeft', EditorFooterContentLeftDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbEditorFooterContentRight
@restrict E

@description
Use this directive to align content right inside the main editor footer.

<h3>Markup example</h3>
<pre>
    <div ng-controller="MySection.Controller as vm">

        <form name="mySectionForm" novalidate>

            <umb-editor-view>

                <umb-editor-footer>

                    <umb-editor-footer-content-left>
                        // align content left
                    </umb-editor-footer-content-left>

                    <umb-editor-footer-content-right>
                        // align content right
                    </umb-editor-footer-content-right>

                </umb-editor-footer>

            </umb-editor-view>

        </form>

    </div>
</pre>

<h3>Use in combination with</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbEditorView umbEditorView}</li>
    <li>{@link umbraco.directives.directive:umbEditorHeader umbEditorHeader}</li>
    <li>{@link umbraco.directives.directive:umbEditorContainer umbEditorContainer}</li>
    <li>{@link umbraco.directives.directive:umbEditorFooter umbEditorFooter}</li>
    <li>{@link umbraco.directives.directive:umbEditorFooterContentLeft umbEditorFooterContentLeft}</li>
</ul>
**/
    (function () {
        'use strict';
        function EditorFooterContentRightDirective() {
            var directive = {
                transclude: true,
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/editor/umb-editor-footer-content-right.html'
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbEditorFooterContentRight', EditorFooterContentRightDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbEditorHeader
@restrict E
@scope

@description
Use this directive to construct a header inside the main editor window.

<h3>Markup example</h3>
<pre>
    <div ng-controller="MySection.Controller as vm">

        <form name="mySectionForm" novalidate>

            <umb-editor-view>

                <umb-editor-header
                    name="vm.content.name"
                    hide-alias="true"
                    hide-description="true"
                    hide-icon="true">
                </umb-editor-header>

                <umb-editor-container>
                    // main content here
                </umb-editor-container>

                <umb-editor-footer>
                    // footer content here
                </umb-editor-footer>

            </umb-editor-view>

        </form>

    </div>
</pre>

<h3>Markup example - with tabs</h3>
<pre>
    <div ng-controller="MySection.Controller as vm">

        <form name="mySectionForm" val-form-manager novalidate>

            <umb-editor-view umb-tabs>

                <umb-editor-header
                    name="vm.content.name"
                    tabs="vm.content.tabs"
                    hide-alias="true"
                    hide-description="true"
                    hide-icon="true">
                </umb-editor-header>

                <umb-editor-container>
                    <umb-tabs-content class="form-horizontal" view="true">
                        <umb-tab id="tab{{tab.id}}" ng-repeat="tab in vm.content.tabs" rel="{{tab.id}}">

                            <div ng-show="tab.alias==='tab1'">
                                // tab 1 content
                            </div>

                            <div ng-show="tab.alias==='tab2'">
                                // tab 2 content
                            </div>

                        </umb-tab>
                    </umb-tabs-content>
                </umb-editor-container>

                <umb-editor-footer>
                    // footer content here
                </umb-editor-footer>

            </umb-editor-view>

        </form>

    </div>
</pre>

<h3>Controller example - with tabs</h3>
<pre>
    (function () {
        "use strict";

        function Controller() {

            var vm = this;
            vm.content = {
                name: "",
                tabs: [
                    {
                        id: 1,
                        label: "Tab 1",
                        alias: "tab1",
                        active: true
                    },
                    {
                        id: 2,
                        label: "Tab 2",
                        alias: "tab2",
                        active: false
                    }
                ]
            };

        }

        angular.module("umbraco").controller("MySection.Controller", Controller);
    })();
</pre>

<h3>Markup example - with sub views</h3>
<pre>
    <div ng-controller="MySection.Controller as vm">

        <form name="mySectionForm" val-form-manager novalidate>

            <umb-editor-view>

                <umb-editor-header
                    name="vm.content.name"
                    navigation="vm.content.navigation"
                    hide-alias="true"
                    hide-description="true"
                    hide-icon="true">
                </umb-editor-header>

                <umb-editor-container>

                    <umb-editor-sub-views
                        sub-views="vm.content.navigation"
                        model="vm.content">
                    </umb-editor-sub-views>

                </umb-editor-container>

                <umb-editor-footer>
                    // footer content here
                </umb-editor-footer>

            </umb-editor-view>

        </form>

    </div>
</pre>

<h3>Controller example - with sub views</h3>
<pre>
    (function () {

        "use strict";

        function Controller() {

            var vm = this;
            vm.content = {
                name: "",
                navigation: [
                    {
                        "name": "Section 1",
                        "icon": "icon-document-dashed-line",
                        "view": "/App_Plugins/path/to/html.html",
                        "active": true
                    },
                    {
                        "name": "Section 2",
                        "icon": "icon-list",
                        "view": "/App_Plugins/path/to/html.html",
                    }
                ]
            };

        }

        angular.module("umbraco").controller("MySection.Controller", Controller);
    })();
</pre>

<h3>Use in combination with</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbEditorView umbEditorView}</li>
    <li>{@link umbraco.directives.directive:umbEditorContainer umbEditorContainer}</li>
    <li>{@link umbraco.directives.directive:umbEditorFooter umbEditorFooter}</li>
</ul>

@param {string} name The content name.
@param {array=} tabs Array of tabs. See example above.
@param {array=} navigation Array of sub views. See example above.
@param {boolean=} nameLocked Set to <code>true</code> to lock the name.
@param {object=} menu Add a context menu to the editor.
@param {string=} icon Show and edit the content icon. Opens an overlay to change the icon.
@param {boolean=} hideIcon Set to <code>true</code> to hide icon.
@param {string=} alias show and edit the content alias.
@param {boolean=} hideAlias Set to <code>true</code> to hide alias.
@param {string=} description Add a description to the content.
@param {boolean=} hideDescription Set to <code>true</code> to hide description.

**/
    (function () {
        'use strict';
        function EditorHeaderDirective(iconHelper) {
            function link(scope, el, attr, ctrl) {
                scope.openIconPicker = function () {
                    scope.dialogModel = {
                        view: 'iconpicker',
                        show: true,
                        icon: scope.icon.split(' ')[0],
                        color: scope.icon.split(' ')[1],
                        submit: function (model) {
                            /* ensure an icon is selected, because on focus on close button
                           or an element in background no icon is submitted. So don't clear/update existing icon/preview.
                        */
                            if (model.icon) {
                                if (model.color) {
                                    scope.icon = model.icon + ' ' + model.color;
                                } else {
                                    scope.icon = model.icon;
                                }
                                // set the icon form to dirty
                                scope.iconForm.$setDirty();
                            }
                            scope.dialogModel.show = false;
                            scope.dialogModel = null;
                        }
                    };
                };
            }
            var directive = {
                transclude: true,
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/editor/umb-editor-header.html',
                scope: {
                    tabs: '=',
                    actions: '=',
                    name: '=',
                    nameLocked: '=',
                    menu: '=',
                    icon: '=',
                    hideIcon: '@',
                    alias: '=',
                    hideAlias: '=',
                    description: '=',
                    hideDescription: '@',
                    descriptionLocked: '@',
                    navigation: '=',
                    key: '='
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbEditorHeader', EditorHeaderDirective);
    }());
    (function () {
        'use strict';
        function EditorMenuDirective($injector, treeService, navigationService, umbModelMapper, appState) {
            function link(scope, el, attr, ctrl) {
                //adds a handler to the context menu item click, we need to handle this differently
                //depending on what the menu item is supposed to do.
                scope.executeMenuItem = function (action) {
                    navigationService.executeMenuAction(action, scope.currentNode, scope.currentSection);
                };
                //callback method to go and get the options async
                scope.getOptions = function () {
                    if (!scope.currentNode) {
                        return;
                    }
                    //when the options item is selected, we need to set the current menu item in appState (since this is synonymous with a menu)
                    appState.setMenuState('currentNode', scope.currentNode);
                    if (!scope.actions) {
                        treeService.getMenu({ treeNode: scope.currentNode }).then(function (data) {
                            scope.actions = data.menuItems;
                        });
                    }
                };
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/editor/umb-editor-menu.html',
                link: link,
                scope: {
                    currentNode: '=',
                    currentSection: '@'
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbEditorMenu', EditorMenuDirective);
    }());
    (function () {
        'use strict';
        function EditorNavigationDirective() {
            function link(scope, el, attr, ctrl) {
                scope.showNavigation = true;
                scope.clickNavigationItem = function (selectedItem) {
                    setItemToActive(selectedItem);
                    runItemAction(selectedItem);
                };
                function runItemAction(selectedItem) {
                    if (selectedItem.action) {
                        selectedItem.action(selectedItem);
                    }
                }
                function setItemToActive(selectedItem) {
                    // set all other views to inactive
                    if (selectedItem.view) {
                        for (var index = 0; index < scope.navigation.length; index++) {
                            var item = scope.navigation[index];
                            item.active = false;
                        }
                        // set view to active
                        selectedItem.active = true;
                    }
                }
                function activate() {
                    // hide navigation if there is only 1 item
                    if (scope.navigation.length <= 1) {
                        scope.showNavigation = false;
                    }
                }
                activate();
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/editor/umb-editor-navigation.html',
                scope: { navigation: '=' },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives.html').directive('umbEditorNavigation', EditorNavigationDirective);
    }());
    (function () {
        'use strict';
        function EditorSubViewsDirective() {
            function link(scope, el, attr, ctrl) {
                scope.activeView = {};
                // set toolbar from selected navigation item
                function setActiveView(items) {
                    for (var index = 0; index < items.length; index++) {
                        var item = items[index];
                        if (item.active && item.view) {
                            scope.activeView = item;
                        }
                    }
                }
                // watch for navigation changes
                scope.$watch('subViews', function (newValue, oldValue) {
                    if (newValue) {
                        setActiveView(newValue);
                    }
                }, true);
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/editor/umb-editor-sub-views.html',
                scope: {
                    subViews: '=',
                    model: '='
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbEditorSubViews', EditorSubViewsDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbEditorView
@restrict E
@scope

@description
Use this directive to construct the main editor window.

<h3>Markup example</h3>
<pre>
    <div ng-controller="MySection.Controller as vm">

        <form name="mySectionForm" novalidate>

            <umb-editor-view>

                <umb-editor-header
                    name="vm.content.name"
                    hide-alias="true"
                    hide-description="true"
                    hide-icon="true">
                </umb-editor-header>

                <umb-editor-container>
                    // main content here
                </umb-editor-container>

                <umb-editor-footer>
                    // footer content here
                </umb-editor-footer>

            </umb-editor-view>

        </form>

    </div>
</pre>
<h3>Controller example</h3>
<pre>
    (function () {

        "use strict";

        function Controller() {

            var vm = this;

        }

        angular.module("umbraco").controller("MySection.Controller", Controller);
    })();
</pre>


<h3>Use in combination with</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbEditorHeader umbEditorHeader}</li>
    <li>{@link umbraco.directives.directive:umbEditorContainer umbEditorContainer}</li>
    <li>{@link umbraco.directives.directive:umbEditorFooter umbEditorFooter}</li>
</ul>
**/
    (function () {
        'use strict';
        function EditorViewDirective() {
            function link(scope, el, attr) {
                if (attr.footer) {
                    scope.footer = attr.footer;
                }
            }
            var directive = {
                transclude: true,
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/editor/umb-editor-view.html',
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbEditorView', EditorViewDirective);
    }());
    /**
* @description Utillity directives for key and field events
**/
    angular.module('umbraco.directives').directive('onKeyup', function () {
        return {
            link: function (scope, elm, attrs) {
                var f = function () {
                    scope.$apply(attrs.onKeyup);
                };
                elm.on('keyup', f);
                scope.$on('$destroy', function () {
                    elm.off('keyup', f);
                });
            }
        };
    }).directive('onKeydown', function () {
        return {
            link: function (scope, elm, attrs) {
                var f = function () {
                    scope.$apply(attrs.onKeydown);
                };
                elm.on('keydown', f);
                scope.$on('$destroy', function () {
                    elm.off('keydown', f);
                });
            }
        };
    }).directive('onBlur', function () {
        return {
            link: function (scope, elm, attrs) {
                var f = function () {
                    scope.$apply(attrs.onBlur);
                };
                elm.on('blur', f);
                scope.$on('$destroy', function () {
                    elm.off('blur', f);
                });
            }
        };
    }).directive('onFocus', function () {
        return {
            link: function (scope, elm, attrs) {
                var f = function () {
                    scope.$apply(attrs.onFocus);
                };
                elm.on('focus', f);
                scope.$on('$destroy', function () {
                    elm.off('focus', f);
                });
            }
        };
    }).directive('onDragEnter', function () {
        return {
            link: function (scope, elm, attrs) {
                var f = function () {
                    scope.$apply(attrs.onDragEnter);
                };
                elm.on('dragenter', f);
                scope.$on('$destroy', function () {
                    elm.off('dragenter', f);
                });
            }
        };
    }).directive('onDragLeave', function () {
        return function (scope, elm, attrs) {
            var f = function (event) {
                var rect = this.getBoundingClientRect();
                var getXY = function getCursorPosition(event) {
                    var x, y;
                    if (typeof event.clientX === 'undefined') {
                        // try touch screen
                        x = event.pageX + document.documentElement.scrollLeft;
                        y = event.pageY + document.documentElement.scrollTop;
                    } else {
                        x = event.clientX + document.body.scrollLeft + document.documentElement.scrollLeft;
                        y = event.clientY + document.body.scrollTop + document.documentElement.scrollTop;
                    }
                    return {
                        x: x,
                        y: y
                    };
                };
                var e = getXY(event.originalEvent);
                // Check the mouseEvent coordinates are outside of the rectangle
                if (e.x > rect.left + rect.width - 1 || e.x < rect.left || e.y > rect.top + rect.height - 1 || e.y < rect.top) {
                    scope.$apply(attrs.onDragLeave);
                }
            };
            elm.on('dragleave', f);
            scope.$on('$destroy', function () {
                elm.off('dragleave', f);
            });
        };
    }).directive('onDragOver', function () {
        return {
            link: function (scope, elm, attrs) {
                var f = function () {
                    scope.$apply(attrs.onDragOver);
                };
                elm.on('dragover', f);
                scope.$on('$destroy', function () {
                    elm.off('dragover', f);
                });
            }
        };
    }).directive('onDragStart', function () {
        return {
            link: function (scope, elm, attrs) {
                var f = function () {
                    scope.$apply(attrs.onDragStart);
                };
                elm.on('dragstart', f);
                scope.$on('$destroy', function () {
                    elm.off('dragstart', f);
                });
            }
        };
    }).directive('onDragEnd', function () {
        return {
            link: function (scope, elm, attrs) {
                var f = function () {
                    scope.$apply(attrs.onDragEnd);
                };
                elm.on('dragend', f);
                scope.$on('$destroy', function () {
                    elm.off('dragend', f);
                });
            }
        };
    }).directive('onDrop', function () {
        return {
            link: function (scope, elm, attrs) {
                var f = function () {
                    scope.$apply(attrs.onDrop);
                };
                elm.on('drop', f);
                scope.$on('$destroy', function () {
                    elm.off('drop', f);
                });
            }
        };
    }).directive('onOutsideClick', function ($timeout) {
        return function (scope, element, attrs) {
            var eventBindings = [];
            function oneTimeClick(event) {
                var el = event.target.nodeName;
                //ignore link and button clicks
                var els = [
                    'INPUT',
                    'A',
                    'BUTTON'
                ];
                if (els.indexOf(el) >= 0) {
                    return;
                }
                // ignore clicks on new overlay
                var parents = $(event.target).parents('a,button,.umb-overlay,.umb-tour');
                if (parents.length > 0) {
                    return;
                }
                // ignore clicks on dialog from old dialog service
                var oldDialog = $(event.target).parents('#old-dialog-service');
                if (oldDialog.length === 1) {
                    return;
                }
                // ignore clicks in tinyMCE dropdown(floatpanel)
                var floatpanel = $(event.target).closest('.mce-floatpanel');
                if (floatpanel.length === 1) {
                    return;
                }
                //ignore clicks inside this element
                if ($(element).has($(event.target)).length > 0) {
                    return;
                }
                scope.$apply(attrs.onOutsideClick);
            }
            $timeout(function () {
                if ('bindClickOn' in attrs) {
                    eventBindings.push(scope.$watch(function () {
                        return attrs.bindClickOn;
                    }, function (newValue) {
                        if (newValue === 'true') {
                            $(document).on('click', oneTimeClick);
                        } else {
                            $(document).off('click', oneTimeClick);
                        }
                    }));
                } else {
                    $(document).on('click', oneTimeClick);
                }
                scope.$on('$destroy', function () {
                    $(document).off('click', oneTimeClick);
                    // unbind watchers
                    for (var e in eventBindings) {
                        eventBindings[e]();
                    }
                });
            });    // Temp removal of 1 sec timeout to prevent bug where overlay does not open. We need to find a better solution.
        };
    }).directive('onRightClick', function () {
        document.oncontextmenu = function (e) {
            if (e.target.hasAttribute('on-right-click')) {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }
        };
        return function (scope, el, attrs) {
            el.on('contextmenu', function (e) {
                e.preventDefault();
                e.stopPropagation();
                scope.$apply(attrs.onRightClick);
                return false;
            });
        };
    }).directive('onDelayedMouseleave', function ($timeout, $parse) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs, ctrl) {
                var active = false;
                var fn = $parse(attrs.onDelayedMouseleave);
                var leave_f = function (event) {
                    var callback = function () {
                        fn(scope, { $event: event });
                    };
                    active = false;
                    $timeout(function () {
                        if (active === false) {
                            scope.$apply(callback);
                        }
                    }, 650);
                };
                var enter_f = function (event, args) {
                    active = true;
                };
                element.on('mouseleave', leave_f);
                element.on('mouseenter', enter_f);
                //unsub events
                scope.$on('$destroy', function () {
                    element.off('mouseleave', leave_f);
                    element.off('mouseenter', enter_f);
                });
            }
        };
    });
    /*
  
  https://vitalets.github.io/checklist-model/
  <label ng-repeat="role in roles">
    <input type="checkbox" checklist-model="user.roles" checklist-value="role.id"> {{role.text}}
  </label>
*/
    angular.module('umbraco.directives').directive('checklistModel', [
        '$parse',
        '$compile',
        function ($parse, $compile) {
            // contains
            function contains(arr, item) {
                if (angular.isArray(arr)) {
                    for (var i = 0; i < arr.length; i++) {
                        if (angular.equals(arr[i], item)) {
                            return true;
                        }
                    }
                }
                return false;
            }
            // add 
            function add(arr, item) {
                arr = angular.isArray(arr) ? arr : [];
                for (var i = 0; i < arr.length; i++) {
                    if (angular.equals(arr[i], item)) {
                        return arr;
                    }
                }
                arr.push(item);
                return arr;
            }
            // remove
            function remove(arr, item) {
                if (angular.isArray(arr)) {
                    for (var i = 0; i < arr.length; i++) {
                        if (angular.equals(arr[i], item)) {
                            arr.splice(i, 1);
                            break;
                        }
                    }
                }
                return arr;
            }
            // https://stackoverflow.com/a/19228302/1458162
            function postLinkFn(scope, elem, attrs) {
                // compile with `ng-model` pointing to `checked`
                $compile(elem)(scope);
                // getter / setter for original model
                var getter = $parse(attrs.checklistModel);
                var setter = getter.assign;
                // value added to list
                var value = $parse(attrs.checklistValue)(scope.$parent);
                // watch UI checked change
                scope.$watch('checked', function (newValue, oldValue) {
                    if (newValue === oldValue) {
                        return;
                    }
                    var current = getter(scope.$parent);
                    if (newValue === true) {
                        setter(scope.$parent, add(current, value));
                    } else {
                        setter(scope.$parent, remove(current, value));
                    }
                });
                // watch original model change
                scope.$parent.$watch(attrs.checklistModel, function (newArr, oldArr) {
                    scope.checked = contains(newArr, value);
                }, true);
            }
            return {
                restrict: 'A',
                priority: 1000,
                terminal: true,
                scope: true,
                compile: function (tElement, tAttrs) {
                    if (tElement[0].tagName !== 'INPUT' || !tElement.attr('type', 'checkbox')) {
                        throw 'checklist-model should be applied to `input[type="checkbox"]`.';
                    }
                    if (!tAttrs.checklistValue) {
                        throw 'You should provide `checklist-value`.';
                    }
                    // exclude recursion
                    tElement.removeAttr('checklist-model');
                    // local scope var storing individual checkbox model
                    tElement.attr('ng-model', 'checked');
                    return postLinkFn;
                }
            };
        }
    ]);
    angular.module('umbraco.directives').directive('contenteditable', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ngModel) {
                function read() {
                    ngModel.$setViewValue(element.html());
                }
                ngModel.$render = function () {
                    element.html(ngModel.$viewValue || '');
                };
                element.bind('focus', function () {
                    var range = document.createRange();
                    range.selectNodeContents(element[0]);
                    var sel = window.getSelection();
                    sel.removeAllRanges();
                    sel.addRange(range);
                });
                element.bind('blur keyup change', function () {
                    scope.$apply(read);
                });
            }
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:fixNumber
* @restrict A
* @description Used in conjunction with type='number' input fields to ensure that the bound value is converted to a number when using ng-model
*  because normally it thinks it's a string and also validation doesn't work correctly due to an angular bug.
**/
    function fixNumber($parse) {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, elem, attrs, ctrl) {
                //parse ngModel onload
                var modelVal = scope.$eval(attrs.ngModel);
                if (modelVal) {
                    var asNum = parseFloat(modelVal, 10);
                    if (!isNaN(asNum)) {
                        $parse(attrs.ngModel).assign(scope, asNum);
                    }
                }
                //always return an int to the model
                ctrl.$parsers.push(function (value) {
                    if (value === 0) {
                        return 0;
                    }
                    return parseFloat(value || '', 10);
                });
                //always try to format the model value as an int
                ctrl.$formatters.push(function (value) {
                    if (angular.isString(value)) {
                        return parseFloat(value, 10);
                    }
                    return value;
                });
                //This fixes this angular issue: 
                //https://github.com/angular/angular.js/issues/2144
                // which doesn't actually validate the number input properly since the model only changes when a real number is entered
                // but the input box still allows non-numbers to be entered which do not validate (only via html5)
                if (typeof elem.prop('validity') === 'undefined') {
                    return;
                }
                elem.bind('input', function (e) {
                    var validity = elem.prop('validity');
                    scope.$apply(function () {
                        ctrl.$setValidity('number', !validity.badInput);
                    });
                });
            }
        };
    }
    angular.module('umbraco.directives').directive('fixNumber', fixNumber);
    angular.module('umbraco.directives').directive('focusWhen', function ($timeout) {
        return {
            restrict: 'A',
            link: function (scope, elm, attrs, ctrl) {
                attrs.$observe('focusWhen', function (newValue) {
                    if (newValue === 'true') {
                        $timeout(function () {
                            elm.focus();
                        });
                    }
                });
            }
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:hexBgColor
* @restrict A
* @description Used to set a hex background color on an element, this will detect valid hex and when it is valid it will set the color, otherwise
* a color will not be set.
**/
    function hexBgColor() {
        return {
            restrict: 'A',
            link: function (scope, element, attr, formCtrl) {
                // Only add inline hex background color if defined and not "true".
                if (attr.hexBgInline === undefined || attr.hexBgInline !== undefined && attr.hexBgInline === 'true') {
                    var origColor = null;
                    if (attr.hexBgOrig) {
                        // Set the orig based on the attribute if there is one.
                        origColor = attr.hexBgOrig;
                    }
                    attr.$observe('hexBgColor', function (newVal) {
                        if (newVal) {
                            if (!origColor) {
                                // Get the orig color before changing it.
                                origColor = element.css('border-color');
                            }
                            // Validate it - test with and without the leading hash.
                            if (/^([0-9a-f]{3}|[0-9a-f]{6})$/i.test(newVal)) {
                                element.css('background-color', '#' + newVal);
                                return;
                            }
                            if (/^#([0-9a-f]{3}|[0-9a-f]{6})$/i.test(newVal)) {
                                element.css('background-color', newVal);
                                return;
                            }
                        }
                        element.css('background-color', origColor);
                    });
                }
            }
        };
    }
    angular.module('umbraco.directives').directive('hexBgColor', hexBgColor);
    /**
* @ngdoc directive
* @name umbraco.directives.directive:hotkey
**/
    angular.module('umbraco.directives').directive('hotkey', function ($window, keyboardService, $log) {
        return function (scope, el, attrs) {
            var options = {};
            var keyCombo = attrs.hotkey;
            if (!keyCombo) {
                //support data binding
                keyCombo = scope.$eval(attrs['hotkey']);
            }
            function activate() {
                if (keyCombo) {
                    // disable shortcuts in input fields if keycombo is 1 character
                    if (keyCombo.length === 1) {
                        options = { inputDisabled: true };
                    }
                    keyboardService.bind(keyCombo, function () {
                        var element = $(el);
                        var activeElementType = document.activeElement.tagName;
                        var clickableElements = [
                            'A',
                            'BUTTON'
                        ];
                        if (element.is('a,div,button,input[type=\'button\'],input[type=\'submit\'],input[type=\'checkbox\']') && !element.is(':disabled')) {
                            if (element.is(':visible') || attrs.hotkeyWhenHidden) {
                                if (attrs.hotkeyWhen && attrs.hotkeyWhen === 'false') {
                                    return;
                                }
                                // when keycombo is enter and a link or button has focus - click the link or button instead of using the hotkey
                                if (keyCombo === 'enter' && clickableElements.indexOf(activeElementType) === 0) {
                                    document.activeElement.click();
                                } else {
                                    element.click();
                                }
                            }
                        } else {
                            element.focus();
                        }
                    }, options);
                    el.on('$destroy', function () {
                        keyboardService.unbind(keyCombo);
                    });
                }
            }
            activate();
        };
    });
    /**
@ngdoc directive
@name umbraco.directives.directive:preventDefault

@description
Use this directive to prevent default action of an element. Effectively implementing <a href="https://api.jquery.com/event.preventdefault/">jQuery's preventdefault</a>

<h3>Markup example</h3>

<pre>
    <a href="https://umbraco.com" prevent-default>Don't go to Umbraco.com</a>
</pre>

**/
    angular.module('umbraco.directives').directive('preventDefault', function () {
        return function (scope, element, attrs) {
            var enabled = true;
            //check if there's a value for the attribute, if there is and it's false then we conditionally don't
            //prevent default.
            if (attrs.preventDefault) {
                attrs.$observe('preventDefault', function (newVal) {
                    enabled = newVal === 'false' || newVal === 0 || newVal === false ? false : true;
                });
            }
            $(element).click(function (event) {
                if (event.metaKey || event.ctrlKey) {
                    return;
                } else {
                    if (enabled === true) {
                        event.preventDefault();
                    }
                }
            });
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:preventEnterSubmit
* @description prevents a form from submitting when the enter key is pressed on an input field
**/
    angular.module('umbraco.directives').directive('preventEnterSubmit', function () {
        return function (scope, element, attrs) {
            var enabled = true;
            //check if there's a value for the attribute, if there is and it's false then we conditionally don't 
            //prevent default.
            if (attrs.preventEnterSubmit) {
                attrs.$observe('preventEnterSubmit', function (newVal) {
                    enabled = newVal === 'false' || newVal === 0 || newVal === false ? false : true;
                });
            }
            $(element).keypress(function (event) {
                if (event.which === 13) {
                    event.preventDefault();
                }
            });
        };
    });
    /**
 * @ngdoc directive
 * @name umbraco.directives.directive:resizeToContent
 * @element div
 * @function
 *
 * @description
 * Resize iframe's automatically to fit to the content they contain
 *
 * @example
   <example module="umbraco.directives">
     <file name="index.html">
         <iframe resize-to-content src="meh.html"></iframe>
     </file>
   </example>
 */
    angular.module('umbraco.directives').directive('resizeToContent', function ($window, $timeout) {
        return function (scope, el, attrs) {
            var iframe = el[0];
            var iframeWin = iframe.contentWindow || iframe.contentDocument.parentWindow;
            if (iframeWin.document.body) {
                $timeout(function () {
                    var height = iframeWin.document.documentElement.scrollHeight || iframeWin.document.body.scrollHeight;
                    el.height(height);
                }, 3000);
            }
        };
    });
    angular.module('umbraco.directives').directive('selectOnFocus', function () {
        return function (scope, el, attrs) {
            $(el).bind('click', function () {
                var editmode = $(el).data('editmode');
                //If editmode is true a click is handled like a normal click
                if (!editmode) {
                    //Initial click, select entire text
                    this.select();
                    //Set the edit mode so subsequent clicks work normally
                    $(el).data('editmode', true);
                }
            }).bind('blur', function () {
                //Reset on focus lost
                $(el).data('editmode', false);
            });
        };
    });
    angular.module('umbraco.directives').directive('umbAutoFocus', function ($timeout) {
        return function (scope, element, attr) {
            var update = function () {
                //if it uses its default naming
                if (element.val() === '' || attr.focusOnFilled) {
                    element.focus();
                }
            };
            $timeout(function () {
                update();
            });
        };
    });
    angular.module('umbraco.directives').directive('umbAutoResize', function ($timeout) {
        return {
            require: [
                '^?umbTabs',
                'ngModel'
            ],
            link: function (scope, element, attr, controllersArr) {
                var domEl = element[0];
                var domElType = domEl.type;
                var umbTabsController = controllersArr[0];
                var ngModelController = controllersArr[1];
                // IE elements
                var isIEFlag = false;
                var wrapper = angular.element('#umb-ie-resize-input-wrapper');
                var mirror = angular.element('<span style="white-space:pre;"></span>');
                function isIE() {
                    var ua = window.navigator.userAgent;
                    var msie = ua.indexOf('MSIE ');
                    if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./) || navigator.userAgent.match(/Edge\/\d+/)) {
                        return true;
                    } else {
                        return false;
                    }
                }
                function activate() {
                    // check if browser is Internet Explorere
                    isIEFlag = isIE();
                    // scrollWidth on element does not work in IE on inputs
                    // we have to do some dirty dom element copying.
                    if (isIEFlag === true && domElType === 'text') {
                        setupInternetExplorerElements();
                    }
                }
                function setupInternetExplorerElements() {
                    if (!wrapper.length) {
                        wrapper = angular.element('<div id="umb-ie-resize-input-wrapper" style="position:fixed; top:-999px; left:0;"></div>');
                        angular.element('body').append(wrapper);
                    }
                    angular.forEach([
                        'fontFamily',
                        'fontSize',
                        'fontWeight',
                        'fontStyle',
                        'letterSpacing',
                        'textTransform',
                        'wordSpacing',
                        'textIndent',
                        'boxSizing',
                        'borderRightWidth',
                        'borderLeftWidth',
                        'borderLeftStyle',
                        'borderRightStyle',
                        'paddingLeft',
                        'paddingRight',
                        'marginLeft',
                        'marginRight'
                    ], function (value) {
                        mirror.css(value, element.css(value));
                    });
                    wrapper.append(mirror);
                }
                function resizeInternetExplorerInput() {
                    mirror.text(element.val() || attr.placeholder);
                    element.css('width', mirror.outerWidth() + 1);
                }
                function resizeInput() {
                    if (domEl.scrollWidth !== domEl.clientWidth) {
                        if (ngModelController.$modelValue) {
                            element.width(domEl.scrollWidth);
                        }
                    }
                    if (!ngModelController.$modelValue && attr.placeholder) {
                        attr.$set('size', attr.placeholder.length);
                        element.width('auto');
                    }
                }
                function resizeTextarea() {
                    if (domEl.scrollHeight !== domEl.clientHeight) {
                        element.height(domEl.scrollHeight);
                    }
                }
                var update = function (force) {
                    if (force === true) {
                        if (domElType === 'textarea') {
                            element.height(0);
                        } else if (domElType === 'text') {
                            element.width(0);
                        }
                    }
                    if (isIEFlag === true && domElType === 'text') {
                        resizeInternetExplorerInput();
                    } else {
                        if (domElType === 'textarea') {
                            resizeTextarea();
                        } else if (domElType === 'text') {
                            resizeInput();
                        }
                    }
                };
                activate();
                //listen for tab changes
                if (umbTabsController != null) {
                    umbTabsController.onTabShown(function (args) {
                        update();
                    });
                }
                // listen for ng-model changes
                var unbindModelWatcher = scope.$watch(function () {
                    return ngModelController.$modelValue;
                }, function (newValue) {
                    update(true);
                });
                scope.$on('$destroy', function () {
                    element.unbind('keyup keydown keypress change', update);
                    element.unbind('blur', update(true));
                    unbindModelWatcher();
                    // clean up IE dom element
                    if (isIEFlag === true && domElType === 'text') {
                        mirror.remove();
                    }
                });
            }
        };
    });
    /**
@ngdoc directive
@name umbraco.directives.directive:umbCheckbox
@restrict E
@scope

@description
<b>Added in Umbraco version 7.14.0</b> Use this directive to render an umbraco checkbox.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <umb-checkbox
            name="checkboxlist"
            value="{{key}}"
            model="true"
            text="{{text}}">
        </umb-checkbox>

    </div>
</pre>

@param {boolean} model Set to <code>true</code> or <code>false</code> to set the checkbox to checked or unchecked.
@param {string} value Set the value of the checkbox.
@param {string} name Set the name of the checkbox.
@param {string} text Set the text for the checkbox label.


**/
    (function () {
        'use strict';
        function CheckboxDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/forms/umb-checkbox.html',
                scope: {
                    model: '=',
                    value: '@',
                    name: '@',
                    text: '@',
                    required: '='
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbCheckbox', CheckboxDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbRadiobutton
@restrict E
@scope

@description
<b>Added in Umbraco version 7.14.0</b> Use this directive to render an umbraco radio button.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <umb-radiobutton
            name="checkboxlist"
            value="{{key}}"
            model="true"
            text="{{text}}">
        </umb-radiobutton>

    </div>
</pre>

@param {boolean} model Set to <code>true</code> or <code>false</code> to set the radiobutton to checked or unchecked.
@param {string} value Set the value of the radiobutton.
@param {string} name Set the name of the radiobutton.
@param {string} text Set the text for the radiobutton label.


**/
    (function () {
        'use strict';
        function RadiobuttonDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/forms/umb-radiobutton.html',
                scope: {
                    model: '=',
                    value: '@',
                    name: '@',
                    text: '@'
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbRadiobutton', RadiobuttonDirective);
    }());
    /*
example usage: <textarea json-edit="myObject" rows="8" class="form-control"></textarea>

jsonEditing is a string which we edit in a textarea. we try parsing to JSON with each change. when it is valid, propagate model changes via ngModelCtrl

use isolate scope to prevent model propagation when invalid - will update manually. cannot replace with template, or will override ngModelCtrl, and not hide behind facade

will override element type to textarea and add own attribute ngModel tied to jsonEditing
 */
    angular.module('umbraco.directives').directive('umbRawModel', function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            template: '<textarea ng-model="jsonEditing"></textarea>',
            replace: true,
            scope: {
                model: '=umbRawModel',
                validateOn: '='
            },
            link: function (scope, element, attrs, ngModelCtrl) {
                function setEditing(value) {
                    scope.jsonEditing = angular.copy(jsonToString(value));
                }
                function updateModel(value) {
                    scope.model = stringToJson(value);
                }
                function setValid() {
                    ngModelCtrl.$setValidity('json', true);
                }
                function setInvalid() {
                    ngModelCtrl.$setValidity('json', false);
                }
                function stringToJson(text) {
                    try {
                        return angular.fromJson(text);
                    } catch (err) {
                        setInvalid();
                        return text;
                    }
                }
                function jsonToString(object) {
                    // better than JSON.stringify(), because it formats + filters $$hashKey etc.
                    // NOTE that this will remove all $-prefixed values
                    return angular.toJson(object, true);
                }
                function isValidJson(model) {
                    var flag = true;
                    try {
                        angular.fromJson(model);
                    } catch (err) {
                        flag = false;
                    }
                    return flag;
                }
                //init
                setEditing(scope.model);
                var onInputChange = function (newval, oldval) {
                    if (newval !== oldval) {
                        if (isValidJson(newval)) {
                            setValid();
                            updateModel(newval);
                        } else {
                            setInvalid();
                        }
                    }
                };
                if (scope.validateOn) {
                    element.on(scope.validateOn, function () {
                        scope.$apply(function () {
                            onInputChange(scope.jsonEditing);
                        });
                    });
                } else {
                    //check for changes going out
                    scope.$watch('jsonEditing', onInputChange, true);
                }
                //check for changes coming in
                scope.$watch('model', function (newval, oldval) {
                    if (newval !== oldval) {
                        setEditing(newval);
                    }
                }, true);
            }
        };
    });
    (function () {
        'use strict';
        function SelectWhen($timeout) {
            function link(scope, el, attr, ctrl) {
                attr.$observe('umbSelectWhen', function (newValue) {
                    if (newValue === 'true') {
                        $timeout(function () {
                            el.select();
                        });
                    }
                });
            }
            var directive = {
                restrict: 'A',
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbSelectWhen', SelectWhen);
    }());
    angular.module('umbraco.directives').directive('gridRte', function (tinyMceService, stylesheetResource, angularHelper, assetsService, $q, $timeout, eventsService) {
        return {
            scope: {
                uniqueId: '=',
                value: '=',
                onClick: '&',
                onFocus: '&',
                onBlur: '&',
                configuration: '=',
                onMediaPickerClick: '=',
                onEmbedClick: '=',
                onMacroPickerClick: '=',
                onLinkPickerClick: '='
            },
            template: '<textarea ng-model="value" rows="10" class="mceNoEditor" style="overflow:hidden" id="{{uniqueId}}"></textarea>',
            replace: true,
            link: function (scope, element, attrs) {
                var initTiny = function () {
                    //we always fetch the default one, and then override parts with our own
                    tinyMceService.configuration().then(function (tinyMceConfig) {
                        //config value from general tinymce.config file
                        var validElements = tinyMceConfig.validElements;
                        var fallbackStyles = [
                            {
                                title: 'Page header',
                                block: 'h2'
                            },
                            {
                                title: 'Section header',
                                block: 'h3'
                            },
                            {
                                title: 'Paragraph header',
                                block: 'h4'
                            },
                            {
                                title: 'Normal',
                                block: 'p'
                            },
                            {
                                title: 'Quote',
                                block: 'blockquote'
                            },
                            {
                                title: 'Code',
                                block: 'code'
                            }
                        ];
                        //These are absolutely required in order for the macros to render inline
                        //we put these as extended elements because they get merged on top of the normal allowed elements by tiny mce
                        var extendedValidElements = '@[id|class|style],-div[id|dir|class|align|style],ins[datetime|cite],-ul[class|style],-li[class|style],-h1[id|dir|class|align|style],-h2[id|dir|class|align|style],-h3[id|dir|class|align|style],-h4[id|dir|class|align|style],-h5[id|dir|class|align|style],-h6[id|style|dir|class|align],span[id|class|style]';
                        var invalidElements = tinyMceConfig.inValidElements;
                        var plugins = _.map(tinyMceConfig.plugins, function (plugin) {
                            if (plugin.useOnFrontend) {
                                return plugin.name;
                            }
                        }).join(' ') + ' autoresize';
                        //config value on the data type
                        var toolbar = [
                            'code',
                            'styleselect',
                            'bold',
                            'italic',
                            'alignleft',
                            'aligncenter',
                            'alignright',
                            'bullist',
                            'numlist',
                            'link',
                            'umbmediapicker',
                            'umbembeddialog'
                        ].join(' | ');
                        var stylesheets = [];
                        var styleFormats = [];
                        var await = [];
                        //queue file loading
                        if (typeof tinymce === 'undefined') {
                            await.push(assetsService.loadJs('lib/tinymce/tinymce.min.js', scope));
                        }
                        if (scope.configuration && scope.configuration.toolbar) {
                            toolbar = scope.configuration.toolbar.join(' | ');
                        }
                        if (scope.configuration && scope.configuration.stylesheets) {
                            angular.forEach(scope.configuration.stylesheets, function (stylesheet, key) {
                                stylesheets.push(Umbraco.Sys.ServerVariables.umbracoSettings.cssPath + '/' + stylesheet + '.css');
                                await.push(stylesheetResource.getRulesByName(stylesheet).then(function (rules) {
                                    angular.forEach(rules, function (rule) {
                                        var r = {};
                                        var split = '';
                                        r.title = rule.name;
                                        if (rule.selector[0] === '.') {
                                            r.inline = 'span';
                                            r.classes = rule.selector.substring(1);
                                        } else if (rule.selector[0] === '#') {
                                            //Even though this will render in the style drop down, it will not actually be applied
                                            // to the elements, don't think TinyMCE even supports this and it doesn't really make much sense
                                            // since only one element can have one id.
                                            r.inline = 'span';
                                            r.attributes = { id: rule.selector.substring(1) };
                                        } else if (rule.selector[0] !== '.' && rule.selector.indexOf('.') > -1) {
                                            split = rule.selector.split('.');
                                            r.block = split[0];
                                            r.classes = rule.selector.substring(rule.selector.indexOf('.') + 1).replace('.', ' ');
                                        } else if (rule.selector[0] !== '#' && rule.selector.indexOf('#') > -1) {
                                            split = rule.selector.split('#');
                                            r.block = split[0];
                                            r.classes = rule.selector.substring(rule.selector.indexOf('#') + 1);
                                        } else {
                                            r.block = rule.selector;
                                        }
                                        styleFormats.push(r);
                                    });
                                }));
                            });
                        } else {
                            stylesheets.push('views/propertyeditors/grid/config/grid.default.rtestyles.css');
                            styleFormats = fallbackStyles;
                        }
                        //stores a reference to the editor
                        var tinyMceEditor = null;
                        $q.all(await).then(function () {
                            var uniqueId = scope.uniqueId;
                            //create a baseline Config to exten upon
                            var baseLineConfigObj = {
                                mode: 'exact',
                                skin: 'umbraco',
                                plugins: plugins,
                                valid_elements: validElements,
                                invalid_elements: invalidElements,
                                extended_valid_elements: extendedValidElements,
                                menubar: false,
                                statusbar: false,
                                relative_urls: false,
                                toolbar: toolbar,
                                content_css: stylesheets,
                                style_formats: styleFormats,
                                autoresize_bottom_margin: 0,
                                //see http://archive.tinymce.com/wiki.php/Configuration:cache_suffix
                                cache_suffix: '?umb__rnd=' + Umbraco.Sys.ServerVariables.application.cacheBuster
                            };
                            if (tinyMceConfig.customConfig) {
                                //if there is some custom config, we need to see if the string value of each item might actually be json and if so, we need to
                                // convert it to json instead of having it as a string since this is what tinymce requires
                                for (var i in tinyMceConfig.customConfig) {
                                    var val = tinyMceConfig.customConfig[i];
                                    if (val) {
                                        val = val.toString().trim();
                                        if (val.detectIsJson()) {
                                            try {
                                                tinyMceConfig.customConfig[i] = JSON.parse(val);
                                                //now we need to check if this custom config key is defined in our baseline, if it is we don't want to
                                                //overwrite the baseline config item if it is an array, we want to concat the items in the array, otherwise
                                                //if it's an object it will overwrite the baseline
                                                if (angular.isArray(baseLineConfigObj[i]) && angular.isArray(tinyMceConfig.customConfig[i])) {
                                                    //concat it and below this concat'd array will overwrite the baseline in angular.extend
                                                    tinyMceConfig.customConfig[i] = baseLineConfigObj[i].concat(tinyMceConfig.customConfig[i]);
                                                }
                                            } catch (e) {
                                            }
                                        }
                                    }
                                    if (val === 'true') {
                                        tinyMceConfig.customConfig[i] = true;
                                    }
                                    if (val === 'false') {
                                        tinyMceConfig.customConfig[i] = false;
                                    }
                                }
                                angular.extend(baseLineConfigObj, tinyMceConfig.customConfig);
                            }
                            //set all the things that user configs should not be able to override
                            baseLineConfigObj.elements = uniqueId;
                            baseLineConfigObj.setup = function (editor) {
                                //set the reference
                                tinyMceEditor = editor;
                                //enable browser based spell checking
                                editor.on('init', function (e) {
                                    editor.getBody().setAttribute('spellcheck', true);
                                    //force overflow to hidden to prevent no needed scroll
                                    editor.getBody().style.overflow = 'hidden';
                                    $timeout(function () {
                                        if (scope.value === null) {
                                            editor.focus();
                                        }
                                    }, 400);
                                });
                                // pin toolbar to top of screen if we have focus and it scrolls off the screen
                                var pinToolbar = function () {
                                    var _toolbar = $(editor.editorContainer).find('.mce-toolbar');
                                    var toolbarHeight = _toolbar.height();
                                    var _tinyMce = $(editor.editorContainer);
                                    var tinyMceRect = _tinyMce[0].getBoundingClientRect();
                                    var tinyMceTop = tinyMceRect.top;
                                    var tinyMceBottom = tinyMceRect.bottom;
                                    var tinyMceWidth = tinyMceRect.width;
                                    var _tinyMceEditArea = _tinyMce.find('.mce-edit-area');
                                    // set padding in top of mce so the content does not "jump" up
                                    _tinyMceEditArea.css('padding-top', toolbarHeight);
                                    if (tinyMceTop < 160 && 160 + toolbarHeight < tinyMceBottom) {
                                        _toolbar.css('visibility', 'visible').css('position', 'fixed').css('top', '160px').css('margin-top', '0').css('width', tinyMceWidth);
                                    } else {
                                        _toolbar.css('visibility', 'visible').css('position', 'absolute').css('top', 'auto').css('margin-top', '0').css('width', tinyMceWidth);
                                    }
                                };
                                // unpin toolbar to top of screen
                                var unpinToolbar = function () {
                                    var _toolbar = $(editor.editorContainer).find('.mce-toolbar');
                                    var _tinyMce = $(editor.editorContainer);
                                    var _tinyMceEditArea = _tinyMce.find('.mce-edit-area');
                                    // reset padding in top of mce so the content does not "jump" up
                                    _tinyMceEditArea.css('padding-top', '0');
                                    _toolbar.css('position', 'static');
                                };
                                //when we leave the editor (maybe)
                                editor.on('blur', function (e) {
                                    editor.save();
                                    angularHelper.safeApply(scope, function () {
                                        scope.value = editor.getContent();
                                        var _toolbar = $(editor.editorContainer).find('.mce-toolbar');
                                        if (scope.onBlur) {
                                            scope.onBlur();
                                        }
                                        unpinToolbar();
                                        $('.umb-panel-body').off('scroll', pinToolbar);
                                    });
                                });
                                // Focus on editor
                                editor.on('focus', function (e) {
                                    angularHelper.safeApply(scope, function () {
                                        if (scope.onFocus) {
                                            scope.onFocus();
                                        }
                                        pinToolbar();
                                        $('.umb-panel-body').on('scroll', pinToolbar);
                                    });
                                });
                                // Click on editor
                                editor.on('click', function (e) {
                                    angularHelper.safeApply(scope, function () {
                                        if (scope.onClick) {
                                            scope.onClick();
                                        }
                                        pinToolbar();
                                        $('.umb-panel-body').on('scroll', pinToolbar);
                                    });
                                });
                                //when buttons modify content
                                editor.on('ExecCommand', function (e) {
                                    editor.save();
                                    angularHelper.safeApply(scope, function () {
                                        scope.value = editor.getContent();
                                    });
                                });
                                // Update model on keypress
                                editor.on('KeyUp', function (e) {
                                    editor.save();
                                    angularHelper.safeApply(scope, function () {
                                        scope.value = editor.getContent();
                                    });
                                });
                                // Update model on change, i.e. copy/pasted text, plugins altering content
                                editor.on('SetContent', function (e) {
                                    if (!e.initial) {
                                        editor.save();
                                        angularHelper.safeApply(scope, function () {
                                            scope.value = editor.getContent();
                                        });
                                    }
                                });
                                editor.on('ObjectResized', function (e) {
                                    var qs = '?width=' + e.width + '&height=' + e.height;
                                    var srcAttr = $(e.target).attr('src');
                                    var path = srcAttr.split('?')[0];
                                    $(e.target).attr('data-mce-src', path + qs);
                                });
                                //Create the insert link plugin
                                tinyMceService.createLinkPicker(editor, scope, function (currentTarget, anchorElement) {
                                    if (scope.onLinkPickerClick) {
                                        scope.onLinkPickerClick(editor, currentTarget, anchorElement);
                                    }
                                });
                                //Create the insert media plugin
                                tinyMceService.createMediaPicker(editor, scope, function (currentTarget, userData) {
                                    if (scope.onMediaPickerClick) {
                                        scope.onMediaPickerClick(editor, currentTarget, userData);
                                    }
                                });
                                //Create the embedded plugin
                                tinyMceService.createInsertEmbeddedMedia(editor, scope, function () {
                                    if (scope.onEmbedClick) {
                                        scope.onEmbedClick(editor);
                                    }
                                });
                                //Create the insert macro plugin
                                tinyMceService.createInsertMacro(editor, scope, function (dialogData) {
                                    if (scope.onMacroPickerClick) {
                                        scope.onMacroPickerClick(editor, dialogData);
                                    }
                                });
                            };
                            /** Loads in the editor */
                            function loadTinyMce() {
                                //we need to add a timeout here, to force a redraw so TinyMCE can find
                                //the elements needed
                                $timeout(function () {
                                    tinymce.DOM.events.domLoaded = true;
                                    tinymce.init(baseLineConfigObj);
                                }, 150, false);
                            }
                            loadTinyMce();
                            //here we declare a special method which will be called whenever the value has changed from the server
                            //this is instead of doing a watch on the model.value = faster
                            //scope.model.onValueChanged = function (newVal, oldVal) {
                            //    //update the display val again if it has changed from the server;
                            //    tinyMceEditor.setContent(newVal, { format: 'raw' });
                            //    //we need to manually fire this event since it is only ever fired based on loading from the DOM, this
                            //    // is required for our plugins listening to this event to execute
                            //    tinyMceEditor.fire('LoadContent', null);
                            //};
                            var tabShownListener = eventsService.on('app.tabChange', function (e, args) {
                                var tabId = args.id;
                                var myTabId = element.closest('.umb-tab-pane').attr('rel');
                                if (String(tabId) === myTabId) {
                                    //the tab has been shown, trigger the mceAutoResize (as it could have timed out before the tab was shown)
                                    if (tinyMceEditor !== undefined && tinyMceEditor != null) {
                                        tinyMceEditor.execCommand('mceAutoResize', false, null, null);
                                    }
                                }
                            });
                            //listen for formSubmitting event (the result is callback used to remove the event subscription)
                            var formSubmittingListener = scope.$on('formSubmitting', function () {
                                //TODO: Here we should parse out the macro rendered content so we can save on a lot of bytes in data xfer
                                // we do parse it out on the server side but would be nice to do that on the client side before as well.
                                scope.value = tinyMceEditor ? tinyMceEditor.getContent() : null;
                            });
                            //when the element is disposed we need to unsubscribe!
                            // NOTE: this is very important otherwise if this is part of a modal, the listener still exists because the dom
                            // element might still be there even after the modal has been hidden.
                            scope.$on('$destroy', function () {
                                formSubmittingListener();
                                eventsService.unsubscribe(tabShownListener);
                                if (tinyMceEditor !== undefined && tinyMceEditor != null) {
                                    tinyMceEditor.destroy();
                                }
                            });
                        });
                    });
                };
                initTiny();
            }
        };
    });
    /** 
@ngdoc directive
@name umbraco.directives.directive:umbBox
@restrict E

@description
Use this directive to render an already styled empty div tag.

<h3>Markup example</h3>
<pre>
    <umb-box>
        <umb-box-header title="this is a title"></umb-box-header>
        <umb-box-content>
            // Content here
        </umb-box-content>
    </umb-box>
</pre>

<h3>Use in combination with:</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbBoxHeader umbBoxHeader}</li>
    <li>{@link umbraco.directives.directive:umbBoxContent umbBoxContent}</li>
</ul>
**/
    (function () {
        'use strict';
        function BoxDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/html/umb-box/umb-box.html'
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbBox', BoxDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbBoxContent
@restrict E

@description
Use this directive to render an empty container. Recommended to use it inside an {@link umbraco.directives.directive:umbBox umbBox} directive. See documentation for {@link umbraco.directives.directive:umbBox umbBox}.

<h3>Markup example</h3>
<pre>
    <umb-box>
        <umb-box-header title="this is a title"></umb-box-header>
        <umb-box-content>
            // Content here
        </umb-box-content>
    </umb-box>
</pre>

<h3>Use in combination with:</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbBox umbBox}</li>
    <li>{@link umbraco.directives.directive:umbBoxHeader umbBoxHeader}</li>
</ul>
**/
    (function () {
        'use strict';
        function BoxContentDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/html/umb-box/umb-box-content.html'
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbBoxContent', BoxContentDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbBoxHeader
@restrict E
@scope

@description
Use this directive to construct a title. Recommended to use it inside an {@link umbraco.directives.directive:umbBox umbBox} directive. See documentation for {@link umbraco.directives.directive:umbBox umbBox}.

<h3>Markup example</h3>
<pre>
    <umb-box>
        <umb-box-header title="This is a title" description="I can enter a description right here"></umb-box-header>
        <umb-box-content>
            // Content here
        </umb-box-content>
    </umb-box>
</pre>

<h3>Markup example with using titleKey</h3>
<pre>
    <umb-box>
        // the title-key property needs an areaAlias_keyAlias from the language files
        <umb-box-header title-key="areaAlias_keyAlias" description-key="areaAlias_keyAlias"></umb-box-header>
        <umb-box-content>
            // Content here
        </umb-box-content>
    </umb-box>
</pre>
{@link https://our.umbraco.com/documentation/extending/language-files/ Here you can see more about the language files}

<h3>Use in combination with:</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbBox umbBox}</li>
    <li>{@link umbraco.directives.directive:umbBoxContent umbBoxContent}</li>
</ul>

@param {string=} title (<code>attrbute</code>): Custom title text.
@param {string=} titleKey (<code>attrbute</code>): The translation key from the language xml files.
@param {string=} description (<code>attrbute</code>): Custom description text.
@param {string=} descriptionKey (<code>attrbute</code>): The translation key from the language xml files.
**/
    (function () {
        'use strict';
        function BoxHeaderDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/html/umb-box/umb-box-header.html',
                scope: {
                    titleKey: '@?',
                    title: '@?',
                    descriptionKey: '@?',
                    description: '@?'
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbBoxHeader', BoxHeaderDirective);
    }());
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbControlGroup
* @restrict E
**/
    angular.module('umbraco.directives.html').directive('umbControlGroup', function (localizationService) {
        return {
            scope: {
                label: '@label',
                description: '@',
                hideLabel: '@',
                alias: '@',
                labelFor: '@',
                required: '@?'
            },
            require: '?^form',
            transclude: true,
            restrict: 'E',
            replace: true,
            templateUrl: 'views/components/html/umb-control-group.html',
            link: function (scope, element, attr, formCtrl) {
                scope.formValid = function () {
                    if (formCtrl && scope.labelFor) {
                        //if a label-for has been set, use that for the validation
                        return formCtrl[scope.labelFor].$valid;
                    }
                    //there is no form.
                    return true;
                };
                if (scope.label && scope.label[0] === '@') {
                    scope.labelstring = localizationService.localize(scope.label.substring(1));
                } else {
                    scope.labelstring = scope.label;
                }
                if (scope.description && scope.description[0] === '@') {
                    scope.descriptionstring = localizationService.localize(scope.description.substring(1));
                } else {
                    scope.descriptionstring = scope.description;
                }
            }
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbPane
* @restrict E
**/
    angular.module('umbraco.directives.html').directive('umbPane', function () {
        return {
            transclude: true,
            restrict: 'E',
            replace: true,
            templateUrl: 'views/components/html/umb-pane.html'
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbPanel
* @restrict E
**/
    angular.module('umbraco.directives.html').directive('umbPanel', function ($timeout, $log) {
        return {
            restrict: 'E',
            replace: true,
            transclude: 'true',
            templateUrl: 'views/components/html/umb-panel.html'
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbImageCrop
* @restrict E
* @function
**/
    angular.module('umbraco.directives').directive('umbImageCrop', function ($timeout, localizationService, cropperHelper, $log) {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: 'views/components/imaging/umb-image-crop.html',
            scope: {
                src: '=',
                width: '@',
                height: '@',
                crop: '=',
                center: '=',
                maxSize: '@'
            },
            link: function (scope, element, attrs) {
                scope.width = 400;
                scope.height = 320;
                scope.dimensions = {
                    image: {},
                    cropper: {},
                    viewport: {},
                    margin: 20,
                    scale: {
                        min: 0.3,
                        max: 3,
                        current: 1
                    }
                };
                //live rendering of viewport and image styles
                scope.style = function () {
                    return {
                        'height': parseInt(scope.dimensions.viewport.height, 10) + 'px',
                        'width': parseInt(scope.dimensions.viewport.width, 10) + 'px'
                    };
                };
                //elements
                var $viewport = element.find('.viewport');
                var $image = element.find('img');
                var $overlay = element.find('.overlay');
                var $container = element.find('.crop-container');
                //default constraints for drag n drop
                var constraints = {
                    left: {
                        max: scope.dimensions.margin,
                        min: scope.dimensions.margin
                    },
                    top: {
                        max: scope.dimensions.margin,
                        min: scope.dimensions.margin
                    }
                };
                scope.constraints = constraints;
                //set constaints for cropping drag and drop
                var setConstraints = function () {
                    constraints.left.min = scope.dimensions.margin + scope.dimensions.cropper.width - scope.dimensions.image.width;
                    constraints.top.min = scope.dimensions.margin + scope.dimensions.cropper.height - scope.dimensions.image.height;
                };
                var setDimensions = function (originalImage) {
                    originalImage.width('auto');
                    originalImage.height('auto');
                    var image = {};
                    image.originalWidth = originalImage.width();
                    image.originalHeight = originalImage.height();
                    image.width = image.originalWidth;
                    image.height = image.originalHeight;
                    image.left = originalImage[0].offsetLeft;
                    image.top = originalImage[0].offsetTop;
                    scope.dimensions.image = image;
                    //unscaled editor size
                    //var viewPortW =  $viewport.width();
                    //var viewPortH =  $viewport.height();
                    var _viewPortW = parseInt(scope.width, 10);
                    var _viewPortH = parseInt(scope.height, 10);
                    //if we set a constraint we will scale it down if needed
                    if (scope.maxSize) {
                        var ratioCalculation = cropperHelper.scaleToMaxSize(_viewPortW, _viewPortH, scope.maxSize);
                        //so if we have a max size, override the thumb sizes
                        _viewPortW = ratioCalculation.width;
                        _viewPortH = ratioCalculation.height;
                    }
                    scope.dimensions.viewport.width = _viewPortW + 2 * scope.dimensions.margin;
                    scope.dimensions.viewport.height = _viewPortH + 2 * scope.dimensions.margin;
                    scope.dimensions.cropper.width = _viewPortW;
                    // scope.dimensions.viewport.width - 2 * scope.dimensions.margin;
                    scope.dimensions.cropper.height = _viewPortH;    //  scope.dimensions.viewport.height - 2 * scope.dimensions.margin;
                };
                //resize to a given ratio
                var resizeImageToScale = function (ratio) {
                    //do stuff
                    var size = cropperHelper.calculateSizeToRatio(scope.dimensions.image.originalWidth, scope.dimensions.image.originalHeight, ratio);
                    scope.dimensions.image.width = size.width;
                    scope.dimensions.image.height = size.height;
                    setConstraints();
                    validatePosition(scope.dimensions.image.left, scope.dimensions.image.top);
                };
                //resize the image to a predefined crop coordinate
                var resizeImageToCrop = function () {
                    scope.dimensions.image = cropperHelper.convertToStyle(scope.crop, {
                        width: scope.dimensions.image.originalWidth,
                        height: scope.dimensions.image.originalHeight
                    }, scope.dimensions.cropper, scope.dimensions.margin);
                    var ratioCalculation = cropperHelper.calculateAspectRatioFit(scope.dimensions.image.originalWidth, scope.dimensions.image.originalHeight, scope.dimensions.cropper.width, scope.dimensions.cropper.height, true);
                    scope.dimensions.scale.current = scope.dimensions.image.ratio;
                    //min max based on original width/height
                    scope.dimensions.scale.min = ratioCalculation.ratio;
                    scope.dimensions.scale.max = 2;
                };
                var validatePosition = function (left, top) {
                    if (left > constraints.left.max) {
                        left = constraints.left.max;
                    }
                    if (left <= constraints.left.min) {
                        left = constraints.left.min;
                    }
                    if (top > constraints.top.max) {
                        top = constraints.top.max;
                    }
                    if (top <= constraints.top.min) {
                        top = constraints.top.min;
                    }
                    if (scope.dimensions.image.left !== left) {
                        scope.dimensions.image.left = left;
                    }
                    if (scope.dimensions.image.top !== top) {
                        scope.dimensions.image.top = top;
                    }
                };
                //sets scope.crop to the recalculated % based crop
                var calculateCropBox = function () {
                    scope.crop = cropperHelper.pixelsToCoordinates(scope.dimensions.image, scope.dimensions.cropper.width, scope.dimensions.cropper.height, scope.dimensions.margin);
                };
                //Drag and drop positioning, using jquery ui draggable
                var onStartDragPosition, top, left;
                $overlay.draggable({
                    drag: function (event, ui) {
                        scope.$apply(function () {
                            validatePosition(ui.position.left, ui.position.top);
                        });
                    },
                    stop: function (event, ui) {
                        scope.$apply(function () {
                            //make sure that every validates one more time...
                            validatePosition(ui.position.left, ui.position.top);
                            calculateCropBox();
                            scope.dimensions.image.rnd = Math.random();
                        });
                    }
                });
                var init = function (image) {
                    scope.loaded = false;
                    //set dimensions on image, viewport, cropper etc
                    setDimensions(image);
                    //create a default crop if we haven't got one already
                    var createDefaultCrop = !scope.crop;
                    if (createDefaultCrop) {
                        calculateCropBox();
                    }
                    resizeImageToCrop();
                    //if we're creating a new crop, make sure to zoom out fully
                    if (createDefaultCrop) {
                        scope.dimensions.scale.current = scope.dimensions.scale.min;
                        resizeImageToScale(scope.dimensions.scale.min);
                    }
                    //sets constaints for the cropper
                    setConstraints();
                    scope.loaded = true;
                };
                /// WATCHERS ////
                scope.$watchCollection('[width, height]', function (newValues, oldValues) {
                    //we have to reinit the whole thing if
                    //one of the external params changes
                    if (newValues !== oldValues) {
                        setDimensions($image);
                        setConstraints();
                    }
                });
                var throttledResizing = _.throttle(function () {
                    resizeImageToScale(scope.dimensions.scale.current);
                    calculateCropBox();
                }, 16);
                //happens when we change the scale
                scope.$watch('dimensions.scale.current', function () {
                    if (scope.loaded) {
                        throttledResizing();
                    }
                });
                //ie hack
                if (window.navigator.userAgent.indexOf('MSIE ')) {
                    var ranger = element.find('input');
                    ranger.bind('change', function () {
                        scope.$apply(function () {
                            scope.dimensions.scale.current = ranger.val();
                        });
                    });
                }
                //// INIT /////
                $image.load(function () {
                    $timeout(function () {
                        init($image);
                    });
                });
            }
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbImageGravity
* @restrict E
* @function
* @description
**/
    angular.module('umbraco.directives').directive('umbImageGravity', function ($timeout, localizationService, $log) {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: 'views/components/imaging/umb-image-gravity.html',
            scope: {
                src: '=',
                center: '=',
                onImageLoaded: '&',
                onGravityChanged: '&'
            },
            link: function (scope, element, attrs) {
                //Internal values for keeping track of the dot and the size of the editor
                scope.dimensions = {
                    width: 0,
                    height: 0,
                    left: 0,
                    top: 0
                };
                scope.loaded = false;
                //elements
                var $viewport = element.find('.viewport');
                var $image = element.find('img');
                var $overlay = element.find('.overlay');
                scope.style = function () {
                    if (scope.dimensions.width <= 0 || scope.dimensions.height <= 0) {
                        setDimensions();
                    }
                    return {
                        'top': scope.dimensions.top + 'px',
                        'left': scope.dimensions.left + 'px'
                    };
                };
                scope.setFocalPoint = function (event) {
                    scope.$emit('imageFocalPointStart');
                    var offsetX = event.offsetX - 10;
                    var offsetY = event.offsetY - 10;
                    calculateGravity(offsetX, offsetY);
                    gravityChanged();
                };
                var setDimensions = function () {
                    if (scope.isCroppable) {
                        scope.dimensions.width = $image.width();
                        scope.dimensions.height = $image.height();
                        if (scope.center) {
                            scope.dimensions.left = scope.center.left * scope.dimensions.width - 10;
                            scope.dimensions.top = scope.center.top * scope.dimensions.height - 10;
                        } else {
                            scope.center = {
                                left: 0.5,
                                top: 0.5
                            };
                        }
                    }
                };
                var calculateGravity = function (offsetX, offsetY) {
                    scope.dimensions.left = offsetX;
                    scope.dimensions.top = offsetY;
                    scope.center.left = (scope.dimensions.left + 10) / scope.dimensions.width;
                    scope.center.top = (scope.dimensions.top + 10) / scope.dimensions.height;
                };
                var gravityChanged = function () {
                    if (angular.isFunction(scope.onGravityChanged)) {
                        scope.onGravityChanged();
                    }
                };
                //Drag and drop positioning, using jquery ui draggable
                //TODO ensure that the point doesnt go outside the box
                $overlay.draggable({
                    containment: 'parent',
                    start: function () {
                        scope.$apply(function () {
                            scope.$emit('imageFocalPointStart');
                        });
                    },
                    stop: function () {
                        scope.$apply(function () {
                            var offsetX = $overlay[0].offsetLeft;
                            var offsetY = $overlay[0].offsetTop;
                            calculateGravity(offsetX, offsetY);
                        });
                        gravityChanged();
                    }
                });
                //// INIT /////
                $image.load(function () {
                    $timeout(function () {
                        scope.isCroppable = true;
                        scope.hasDimensions = true;
                        if (scope.src) {
                            if (scope.src.endsWith('.svg')) {
                                scope.isCroppable = false;
                                scope.hasDimensions = false;
                            } else {
                                // From: https://stackoverflow.com/a/51789597/5018
                                var type = scope.src.substring(scope.src.indexOf('/') + 1, scope.src.indexOf(';base64'));
                                if (type.startsWith('svg')) {
                                    scope.isCroppable = false;
                                    scope.hasDimensions = false;
                                }
                            }
                        }
                        setDimensions();
                        scope.loaded = true;
                        if (angular.isFunction(scope.onImageLoaded)) {
                            scope.onImageLoaded({
                                'isCroppable': scope.isCroppable,
                                'hasDimensions': scope.hasDimensions
                            });
                        }
                    });
                });
                $(window).on('resize.umbImageGravity', function () {
                    scope.$apply(function () {
                        $timeout(function () {
                            setDimensions();
                        });
                        // Make sure we can find the offset values for the overlay(dot) before calculating
                        // fixes issue with resize event when printing the page (ex. hitting ctrl+p inside the rte)
                        if ($overlay.is(':visible')) {
                            var offsetX = $overlay[0].offsetLeft;
                            var offsetY = $overlay[0].offsetTop;
                            calculateGravity(offsetX, offsetY);
                        }
                    });
                });
                scope.$on('$destroy', function () {
                    $(window).off('.umbImageGravity');
                });
            }
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbImageThumbnail
* @restrict E
* @function
* @description
**/
    angular.module('umbraco.directives').directive('umbImageThumbnail', function ($timeout, localizationService, cropperHelper, $log) {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: 'views/components/imaging/umb-image-thumbnail.html',
            scope: {
                src: '=',
                width: '@',
                height: '@',
                center: '=',
                crop: '=',
                maxSize: '@'
            },
            link: function (scope, element, attrs) {
                //// INIT /////
                var $image = element.find('img');
                scope.loaded = false;
                $image.load(function () {
                    $timeout(function () {
                        $image.width('auto');
                        $image.height('auto');
                        scope.image = {};
                        scope.image.width = $image[0].width;
                        scope.image.height = $image[0].height;
                        //we force a lower thumbnail size to fit the max size
                        //we do not compare to the image dimensions, but the thumbs
                        if (scope.maxSize) {
                            var ratioCalculation = cropperHelper.calculateAspectRatioFit(scope.width, scope.height, scope.maxSize, scope.maxSize, false);
                            //so if we have a max size, override the thumb sizes
                            scope.width = ratioCalculation.width;
                            scope.height = ratioCalculation.height;
                        }
                        setPreviewStyle();
                        scope.loaded = true;
                    });
                });
                /// WATCHERS ////
                scope.$watchCollection('[crop, center]', function (newValues, oldValues) {
                    //we have to reinit the whole thing if
                    //one of the external params changes
                    setPreviewStyle();
                });
                scope.$watch('center', function () {
                    setPreviewStyle();
                }, true);
                function setPreviewStyle() {
                    if (scope.crop && scope.image) {
                        scope.preview = cropperHelper.convertToStyle(scope.crop, scope.image, {
                            width: scope.width,
                            height: scope.height
                        }, 0);
                    } else if (scope.image) {
                        //returns size fitting the cropper
                        var p = cropperHelper.calculateAspectRatioFit(scope.image.width, scope.image.height, scope.width, scope.height, true);
                        if (scope.center) {
                            var xy = cropperHelper.alignToCoordinates(p, scope.center, {
                                width: scope.width,
                                height: scope.height
                            });
                            p.top = xy.top;
                            p.left = xy.left;
                        } else {
                        }
                        p.position = 'absolute';
                        scope.preview = p;
                    }
                }
            }
        };
    });
    angular.module('umbraco.directives')    /**
    * @ngdoc directive
    * @name umbraco.directives.directive:localize
    * @restrict EA
    * @function
    * @description
    * <div>
    *   <strong>Component</strong><br />
    *   Localize a specific token to put into the HTML as an item
    * </div>
    * <div>
    *   <strong>Attribute</strong><br />
    *   Add a HTML attribute to an element containing the HTML attribute name you wish to localise
    *   Using the format of '@section_key' or 'section_key'
    * </div>
    * ##Usage
    * <pre>
    * <!-- Component -->
    * <localize key="general_close">Close</localize>
    * <localize key="section_key">Fallback value</localize>
    *
    * <!-- Attribute -->
    * <input type="text" localize="placeholder" placeholder="@placeholders_entername" />
    * <input type="text" localize="placeholder,title" title="@section_key" placeholder="@placeholders_entername" />
    * <div localize="title" title="@section_key"></div>
    * </pre>
    **/.directive('localize', function ($log, localizationService) {
        return {
            restrict: 'E',
            scope: { key: '@' },
            replace: true,
            link: function (scope, element, attrs) {
                var key = scope.key;
                localizationService.localize(key).then(function (value) {
                    element.html(value);
                });
            }
        };
    }).directive('localize', function ($log, localizationService) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                //Support one or more attribute properties to update
                var keys = attrs.localize.split(',');
                angular.forEach(keys, function (value, key) {
                    var attr = element.attr(value);
                    if (attr) {
                        if (attr[0] === '@') {
                            //If the translation key starts with @ then remove it
                            attr = attr.substring(1);
                        }
                        var t = localizationService.tokenize(attr, scope);
                        localizationService.localize(t.key, t.tokens).then(function (val) {
                            element.attr(value, val);
                        });
                    }
                });
            }
        };
    });
    (function () {
        'use strict';
        function MediaNodeInfoDirective($timeout, $location, eventsService, userService, dateHelper) {
            function link(scope, element, attrs, ctrl) {
                var evts = [];
                function onInit() {
                    // If logged in user has access to the settings section
                    // show the open anchors - if the user doesn't have 
                    // access, contentType is null, see MediaModelMapper
                    scope.allowOpen = scope.node.contentType !== null;
                    // get document type details
                    scope.mediaType = scope.node.contentType;
                    // set the media link initially
                    setMediaLink();
                    // make sure dates are formatted to the user's locale
                    formatDatesToLocal();
                }
                function formatDatesToLocal() {
                    // get current backoffice user and format dates
                    userService.getCurrentUser().then(function (currentUser) {
                        scope.node.createDateFormatted = dateHelper.getLocalDate(scope.node.createDate, currentUser.locale, 'LLL');
                        scope.node.updateDateFormatted = dateHelper.getLocalDate(scope.node.updateDate, currentUser.locale, 'LLL');
                    });
                }
                function setMediaLink() {
                    scope.nodeUrl = scope.node.mediaLink;
                }
                scope.openMediaType = function (mediaType) {
                    // remove first "#" from url if it is prefixed else the path won't work
                    var url = '/settings/mediaTypes/edit/' + mediaType.id;
                    $location.path(url);
                };
                // watch for content updates - reload content when node is saved, published etc.
                scope.$watch('node.updateDate', function (newValue, oldValue) {
                    if (!newValue) {
                        return;
                    }
                    if (newValue === oldValue) {
                        return;
                    }
                    // Update the media link
                    setMediaLink();
                    // Update the create and update dates
                    formatDatesToLocal();
                });
                //ensure to unregister from all events!
                scope.$on('$destroy', function () {
                    for (var e in evts) {
                        eventsService.unsubscribe(evts[e]);
                    }
                });
                onInit();
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/media/umb-media-node-info.html',
                scope: { node: '=' },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbMediaNodeInfo', MediaNodeInfoDirective);
    }());
    /**
 * @ngdoc directive
 * @name umbraco.directives.directive:umbNotifications
 */
    (function () {
        'use strict';
        function NotificationDirective(notificationsService) {
            function link(scope, el, attr, ctrl) {
                //subscribes to notifications in the notification service
                scope.notifications = notificationsService.current;
                scope.$watch('notificationsService.current', function (newVal, oldVal, scope) {
                    if (newVal) {
                        scope.notifications = newVal;
                    }
                });
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/notifications/umb-notifications.html',
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbNotifications', NotificationDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbOverlay
@restrict E
@scope

@description

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <button type="button" ng-click="vm.openOverlay()"></button>

        <umb-overlay
            ng-if="vm.overlay.show"
            model="vm.overlay"
            view="vm.overlay.view"
            position="right">
        </umb-overlay>

    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {

        "use strict";

        function Controller() {

            var vm = this;

            vm.openOverlay = openOverlay;

            function openOverlay() {

                vm.overlay = {
                    view: "mediapicker",
                    show: true,
                    submit: function(model) {

                        vm.overlay.show = false;
                        vm.overlay = null;
                    },
                    close: function(oldModel) {
                        vm.overlay.show = false;
                        vm.overlay = null;
                    }
                }

            };

        }

        angular.module("umbraco").controller("My.Controller", Controller);
    })();
</pre>

<h1>General Options</h1>
<table>
    <thead>
        <tr>
            <th>Param</th>
            <th>Type</th>
            <th>Details</th>
        </tr>
    </thead>
    <tr>
        <td>model.title</td>
        <td>String</td>
        <td>Set the title of the overlay.</td>
    </tr>
    <tr>
        <td>model.subtitle</td>
        <td>String</td>
        <td>Set the subtitle of the overlay.</td>
    </tr>
    <tr>
        <td>model.submitButtonLabel</td>
        <td>String</td>
        <td>Set an alternate submit button text</td>
    </tr>
    <tr>
        <td>model.submitButtonLabelKey</td>
        <td>String</td>
        <td>Set an alternate submit button label key for localized texts</td>
    </tr>
    <tr>
        <td>model.hideSubmitButton</td>
        <td>Boolean</td>
        <td>Hides the submit button</td>
    </tr>
    <tr>
        <td>model.closeButtonLabel</td>
        <td>String</td>
        <td>Set an alternate close button text</td>
    </tr>
    <tr>
        <td>model.closeButtonLabelKey</td>
        <td>String</td>
        <td>Set an alternate close button label key for localized texts</td>
    </tr>
    <tr>
        <td>model.show</td>
        <td>Boolean</td>
        <td>Show/hide the overlay</td>
    </tr>
    <tr>
        <td>model.submit</td>
        <td>Function</td>
        <td>Callback function when the overlay submits. Returns the overlay model object</td>
    </tr>
    <tr>
        <td>model.close</td>
        <td>Function</td>
        <td>Callback function when the overlay closes. Returns a copy of the overlay model object before being modified</td>
    </tr>
</table>


<h1>Content Picker</h1>
Opens a content picker.</br>
<strong>view: </strong>contentpicker
<table>
    <thead>
        <tr>
            <th>Param</th>
            <th>Type</th>
            <th>Details</th>
        </tr>
    </thead>
    <tr>
        <td>model.multiPicker</td>
        <td>Boolean</td>
        <td>Pick one or multiple items</td>
    </tr>
</table>
<table>
    <thead>
        <tr>
            <th>Returns</th>
            <th>Type</th>
            <th>Details</th>
        </tr>
    </thead>
    <tr>
        <td>model.selection</td>
        <td>Array</td>
        <td>Array of content objects</td>
    </tr>
</table>


<h1>Icon Picker</h1>
Opens an icon picker.</br>
<strong>view: </strong>iconpicker
<table>
    <thead>
        <tr>
            <th>Returns</th>
            <th>Type</th>
            <th>Details</th>
        </tr>
    </thead>
    <tr>
        <td>model.icon</td>
        <td>String</td>
        <td>The icon class</td>
    </tr>
</table>

<h1>Item Picker</h1>
Opens an item picker.</br>
<strong>view: </strong>itempicker
<table>
    <thead>
        <tr>
            <th>Param</th>
            <th>Type</th>
            <th>Details</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>model.availableItems</td>
            <td>Array</td>
            <td>Array of available items</td>
        </tr>
        <tr>
            <td>model.selectedItems</td>
            <td>Array</td>
            <td>Array of selected items. When passed in the selected items will be filtered from the available items.</td>
        </tr>
        <tr>
            <td>model.filter</td>
            <td>Boolean</td>
            <td>Set to false to hide the filter</td>
        </tr>
    </tbody>
</table>
<table>
    <thead>
        <tr>
            <th>Returns</th>
            <th>Type</th>
            <th>Details</th>
        </tr>
    </thead>
    <tr>
        <td>model.selectedItem</td>
        <td>Object</td>
        <td>The selected item</td>
    </tr>
</table>

<h1>Macro Picker</h1>
Opens a media picker.</br>
<strong>view: </strong>macropicker
<table>
    <thead>
        <tr>
            <th>Param</th>
            <th>Type</th>
            <th>Details</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>model.dialogData</td>
            <td>Object</td>
            <td>Object which contains array of allowedMacros. Set to <code>null</code> to allow all.</td>
        </tr>
    </tbody>
</table>
<table>
    <thead>
        <tr>
            <th>Returns</th>
            <th>Type</th>
            <th>Details</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>model.macroParams</td>
            <td>Array</td>
            <td>Array of macro params</td>
        </tr>
        <tr>
            <td>model.selectedMacro</td>
            <td>Object</td>
            <td>The selected macro</td>
        </tr>
    </tbody>
</table>

<h1>Media Picker</h1>
Opens a media picker.</br>
<strong>view: </strong>mediapicker
<table>
    <thead>
        <tr>
            <th>Param</th>
            <th>Type</th>
            <th>Details</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>model.multiPicker</td>
            <td>Boolean</td>
            <td>Pick one or multiple items</td>
        </tr>
        <tr>
            <td>model.onlyImages</td>
            <td>Boolean</td>
            <td>Only display files that have an image file-extension</td>
        </tr>
        <tr>
            <td>model.disableFolderSelect</td>
            <td>Boolean</td>
            <td>Disable folder selection</td>
        </tr>
    </tbody>
</table>
<table>
    <thead>
        <tr>
            <th>Returns</th>
            <th>Type</th>
            <th>Details</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>model.selectedImages</td>
            <td>Array</td>
            <td>Array of selected images</td>
        </tr>
    </tbody>
</table>

<h1>Member Group Picker</h1>
Opens a member group picker.</br>
<strong>view: </strong>membergrouppicker
<table>
    <thead>
        <tr>
            <th>Param</th>
            <th>Type</th>
            <th>Details</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>model.multiPicker</td>
            <td>Boolean</td>
            <td>Pick one or multiple items</td>
        </tr>
    </tbody>
</table>
<table>
    <thead>
        <tr>
            <th>Returns</th>
            <th>Type</th>
            <th>Details</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>model.selectedMemberGroup</td>
            <td>String</td>
            <td>The selected member group</td>
        </tr>
        <tr>
            <td>model.selectedMemberGroups (multiPicker)</td>
            <td>Array</td>
            <td>The selected member groups</td>
        </tr>
    </tbody>
</table>

<h1>Member Picker</h1>
Opens a member picker. </br>
<strong>view: </strong>memberpicker
<table>
    <thead>
        <tr>
            <th>Param</th>
            <th>Type</th>
            <th>Details</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>model.multiPicker</td>
            <td>Boolean</td>
            <td>Pick one or multiple items</td>
        </tr>
    </tbody>
</table>
<table>
    <thead>
        <tr>
            <th>Returns</th>
            <th>Type</th>
            <th>Details</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>model.selection</td>
            <td>Array</td>
            <td>Array of selected members/td>
        </tr>
    </tbody>
</table>

<h1>YSOD</h1>
Opens an overlay to show a custom YSOD. </br>
<strong>view: </strong>ysod
<table>
    <thead>
        <tr>
            <th>Param</th>
            <th>Type</th>
            <th>Details</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>model.error</td>
            <td>Object</td>
            <td>Error object</td>
        </tr>
    </tbody>
</table>

@param {object} model Overlay options.
@param {string} view Path to view or one of the default view names.
@param {string} position The overlay position ("left", "right", "center": "target").
**/
    (function () {
        'use strict';
        function OverlayDirective($timeout, formHelper, overlayHelper, localizationService) {
            function link(scope, el, attr, ctrl) {
                scope.directive = { enableConfirmButton: false };
                var overlayNumber = 0;
                var numberOfOverlays = 0;
                var isRegistered = false;
                var modelCopy = {};
                function activate() {
                    setView();
                    setButtonText();
                    modelCopy = makeModelCopy(scope.model);
                    $timeout(function () {
                        if (scope.position === 'target') {
                            setTargetPosition();
                        }
                        // this has to be done inside a timeout to ensure the destroy
                        // event on other overlays is run before registering a new one
                        registerOverlay();
                        setOverlayIndent();
                    });
                }
                function setView() {
                    if (scope.view) {
                        if (scope.view.indexOf('.html') === -1) {
                            var viewAlias = scope.view.toLowerCase();
                            scope.view = 'views/common/overlays/' + viewAlias + '/' + viewAlias + '.html';
                        }
                    }
                }
                function setButtonText() {
                    if (!scope.model.closeButtonLabelKey && !scope.model.closeButtonLabel) {
                        scope.model.closeButtonLabel = localizationService.localize('general_close');
                    }
                    if (!scope.model.submitButtonLabelKey && !scope.model.submitButtonLabel) {
                        scope.model.submitButtonLabel = localizationService.localize('general_submit');
                    }
                }
                function registerOverlay() {
                    overlayNumber = overlayHelper.registerOverlay();
                    $(document).bind('keydown.overlay-' + overlayNumber, function (event) {
                        if (event.which === 27) {
                            numberOfOverlays = overlayHelper.getNumberOfOverlays();
                            if (numberOfOverlays === overlayNumber) {
                                scope.$apply(function () {
                                    scope.closeOverLay();
                                });
                            }
                            event.preventDefault();
                        }
                        if (event.which === 13) {
                            numberOfOverlays = overlayHelper.getNumberOfOverlays();
                            if (numberOfOverlays === overlayNumber) {
                                var activeElementType = document.activeElement.tagName;
                                var clickableElements = [
                                    'A',
                                    'BUTTON'
                                ];
                                var submitOnEnter = document.activeElement.hasAttribute('overlay-submit-on-enter');
                                var submitOnEnterValue = submitOnEnter ? document.activeElement.getAttribute('overlay-submit-on-enter') : '';
                                if (clickableElements.indexOf(activeElementType) === 0) {
                                    document.activeElement.click();
                                    event.preventDefault();
                                } else if (activeElementType === 'TEXTAREA' && !submitOnEnter) {
                                } else if (submitOnEnter && submitOnEnterValue === 'false') {
                                } else {
                                    scope.$apply(function () {
                                        scope.submitForm(scope.model);
                                    });
                                    event.preventDefault();
                                }
                            }
                        }
                    });
                    isRegistered = true;
                }
                function unregisterOverlay() {
                    if (isRegistered) {
                        overlayHelper.unregisterOverlay();
                        $(document).unbind('keydown.overlay-' + overlayNumber);
                        isRegistered = false;
                    }
                }
                function makeModelCopy(object) {
                    var newObject = {};
                    for (var key in object) {
                        if (key !== 'event') {
                            newObject[key] = angular.copy(object[key]);
                        }
                    }
                    return newObject;
                }
                function setOverlayIndent() {
                    var overlayIndex = overlayNumber - 1;
                    var indentSize = overlayIndex * 20;
                    var overlayWidth = el.context.clientWidth;
                    el.css('width', overlayWidth - indentSize);
                    if (scope.position === 'center' || scope.position === 'target') {
                        var overlayTopPosition = el.context.offsetTop;
                        el.css('top', overlayTopPosition + indentSize);
                    }
                }
                function setTargetPosition() {
                    var container = $('#contentwrapper');
                    var containerLeft = container[0].offsetLeft;
                    var containerRight = containerLeft + container[0].offsetWidth;
                    var containerTop = container[0].offsetTop;
                    var containerBottom = containerTop + container[0].offsetHeight;
                    var mousePositionClickX = null;
                    var mousePositionClickY = null;
                    var elementHeight = null;
                    var elementWidth = null;
                    var position = {
                        right: 'inherit',
                        left: 'inherit',
                        top: 'inherit',
                        bottom: 'inherit'
                    };
                    // if mouse click position is know place element with mouse in center
                    if (scope.model.event && scope.model.event) {
                        // click position
                        mousePositionClickX = scope.model.event.pageX;
                        mousePositionClickY = scope.model.event.pageY;
                        // element size
                        elementHeight = el.context.clientHeight;
                        elementWidth = el.context.clientWidth;
                        // move element to this position
                        position.left = mousePositionClickX - elementWidth / 2;
                        position.top = mousePositionClickY - elementHeight / 2;
                        // check to see if element is outside screen
                        // outside right
                        if (position.left + elementWidth > containerRight) {
                            position.right = 10;
                            position.left = 'inherit';
                        }
                        // outside bottom
                        if (position.top + elementHeight > containerBottom) {
                            position.bottom = 10;
                            position.top = 'inherit';
                        }
                        // outside left
                        if (position.left < containerLeft) {
                            position.left = containerLeft + 10;
                            position.right = 'inherit';
                        }
                        // outside top
                        if (position.top < containerTop) {
                            position.top = 10;
                            position.bottom = 'inherit';
                        }
                        el.css(position);
                    }
                }
                scope.submitForm = function (model) {
                    if (scope.model.submit) {
                        if (formHelper.submitForm({ scope: scope })) {
                            formHelper.resetForm({ scope: scope });
                            if (scope.model.confirmSubmit && scope.model.confirmSubmit.enable && !scope.directive.enableConfirmButton) {
                                scope.model.submit(model, modelCopy, scope.directive.enableConfirmButton);
                            } else {
                                unregisterOverlay();
                                scope.model.submit(model, modelCopy, scope.directive.enableConfirmButton);
                            }
                        }
                    }
                };
                scope.cancelConfirmSubmit = function () {
                    scope.model.confirmSubmit.show = false;
                };
                scope.closeOverLay = function () {
                    unregisterOverlay();
                    if (scope.model.close) {
                        scope.model = modelCopy;
                        scope.model.close(scope.model);
                    } else {
                        scope.model.show = false;
                        scope.model = null;
                    }
                };
                // angular does not support ng-show on custom directives
                // width isolated scopes. So we have to make our own.
                if (attr.hasOwnProperty('ngShow')) {
                    scope.$watch('ngShow', function (value) {
                        if (value) {
                            el.show();
                            activate();
                        } else {
                            unregisterOverlay();
                            el.hide();
                        }
                    });
                } else {
                    activate();
                }
                scope.$on('$destroy', function () {
                    unregisterOverlay();
                });
            }
            var directive = {
                transclude: true,
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/overlays/umb-overlay.html',
                scope: {
                    ngShow: '=',
                    model: '=',
                    view: '=',
                    position: '@'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbOverlay', OverlayDirective);
    }());
    (function () {
        'use strict';
        function OverlayBackdropDirective(overlayHelper) {
            function link(scope, el, attr, ctrl) {
                scope.numberOfOverlays = 0;
                scope.$watch(function () {
                    return overlayHelper.getNumberOfOverlays();
                }, function (newValue) {
                    scope.numberOfOverlays = newValue;
                });
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/overlays/umb-overlay-backdrop.html',
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbOverlayBackdrop', OverlayBackdropDirective);
    }());
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbProperty
* @restrict E
**/
    angular.module('umbraco.directives').directive('umbProperty', function (umbPropEditorHelper, userService) {
        return {
            scope: { property: '=' },
            transclude: true,
            restrict: 'E',
            replace: true,
            templateUrl: 'views/components/property/umb-property.html',
            link: function (scope) {
                userService.getCurrentUser().then(function (u) {
                    var isAdmin = u.userGroups.indexOf('admin') !== -1;
                    scope.propertyAlias = Umbraco.Sys.ServerVariables.isDebuggingEnabled === true || isAdmin ? scope.property.alias : null;
                });
            },
            //Define a controller for this directive to expose APIs to other directives
            controller: function ($scope, $timeout) {
                var self = this;
                //set the API properties/methods
                self.property = $scope.property;
                self.setPropertyError = function (errorMsg) {
                    $scope.property.propertyErrorMessage = errorMsg;
                };
            }
        };
    });
    /**
* @ngdoc directive
* @function
* @name umbraco.directives.directive:umbPropertyEditor 
* @requires formController
* @restrict E
**/
    //share property editor directive function
    var _umbPropertyEditor = function (umbPropEditorHelper) {
        return {
            scope: {
                model: '=',
                isPreValue: '@',
                preview: '@'
            },
            require: '^form',
            restrict: 'E',
            replace: true,
            templateUrl: 'views/components/property/umb-property-editor.html',
            link: function (scope, element, attrs, ctrl) {
                //we need to copy the form controller val to our isolated scope so that
                //it get's carried down to the child scopes of this!
                //we'll also maintain the current form name.
                scope[ctrl.$name] = ctrl;
                if (!scope.model.alias) {
                    scope.model.alias = Math.random().toString(36).slice(2);
                }
                scope.$watch('model.view', function (val) {
                    scope.propertyEditorView = umbPropEditorHelper.getViewPath(scope.model.view, scope.isPreValue);
                });
            }
        };
    };
    //Preffered is the umb-property-editor as its more explicit - but we keep umb-editor for backwards compat
    angular.module('umbraco.directives').directive('umbPropertyEditor', _umbPropertyEditor);
    angular.module('umbraco.directives').directive('umbEditor', _umbPropertyEditor);
    angular.module('umbraco.directives.html').directive('umbPropertyGroup', function () {
        return {
            transclude: true,
            restrict: 'E',
            replace: true,
            templateUrl: 'views/components/property/umb-property-group.html'
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbTab
* @restrict E
**/
    angular.module('umbraco.directives').directive('umbTab', function ($parse, $timeout) {
        return {
            restrict: 'E',
            replace: true,
            transclude: 'true',
            templateUrl: 'views/components/tabs/umb-tab.html'
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbTabs 
* @restrict A
* @description Used to bind to bootstrap tab events so that sub directives can use this API to listen to tab changes
**/
    angular.module('umbraco.directives').directive('umbTabs', function () {
        return {
            restrict: 'A',
            controller: function ($scope, $element, $attrs, eventsService) {
                var callbacks = [];
                this.onTabShown = function (cb) {
                    callbacks.push(cb);
                };
                function tabShown(event) {
                    var curr = $(event.target);
                    // active tab
                    var prev = $(event.relatedTarget);
                    // previous tab
                    // emit tab change event
                    var tabId = Number(curr.context.hash.replace('#tab', ''));
                    var args = {
                        id: tabId,
                        hash: curr.context.hash
                    };
                    eventsService.emit('app.tabChange', args);
                    $scope.$apply();
                    for (var c in callbacks) {
                        callbacks[c].apply(this, [{
                                current: curr,
                                previous: prev
                            }]);
                    }
                }
                //NOTE: it MUST be done this way - binding to an ancestor element that exists
                // in the DOM to bind to the dynamic elements that will be created.
                // It would be nicer to create this event handler as a directive for which child
                // directives can attach to.
                $element.on('shown', '.nav-tabs a', tabShown);
                //ensure to unregister
                $scope.$on('$destroy', function () {
                    $element.off('shown', '.nav-tabs a', tabShown);
                    for (var c in callbacks) {
                        delete callbacks[c];
                    }
                    callbacks = null;
                });
            }
        };
    });
    (function () {
        'use strict';
        function UmbTabsContentDirective() {
            function link(scope, el, attr, ctrl) {
                scope.view = attr.view;
            }
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: 'true',
                templateUrl: 'views/components/tabs/umb-tabs-content.html',
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbTabsContent', UmbTabsContentDirective);
    }());
    (function () {
        'use strict';
        function UmbTabsNavDirective($timeout) {
            function link(scope, el, attr) {
                function activate() {
                    $timeout(function () {
                        //use bootstrap tabs API to show the first one
                        el.find('a:first').tab('show');
                        //enable the tab drop
                        el.tabdrop();
                    });
                }
                var unbindModelWatch = scope.$watch('model', function (newValue, oldValue) {
                    activate();
                });
                scope.$on('$destroy', function () {
                    //ensure to destroy tabdrop (unbinds window resize listeners)
                    el.tabdrop('destroy');
                    unbindModelWatch();
                });
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/tabs/umb-tabs-nav.html',
                scope: {
                    model: '=',
                    tabdrop: '=',
                    idSuffix: '@'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbTabsNav', UmbTabsNavDirective);
    }());
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbTree
* @restrict E
**/
    function umbTreeDirective($compile, $log, $q, $rootScope, treeService, notificationsService, $timeout, userService) {
        return {
            restrict: 'E',
            replace: true,
            terminal: false,
            scope: {
                section: '@',
                treealias: '@',
                hideoptions: '@',
                hideheader: '@',
                cachekey: '@',
                isdialog: '@',
                onlyinitialized: '@',
                //Custom query string arguments to pass in to the tree as a string, example: "startnodeid=123&something=value"
                customtreeparams: '@',
                eventhandler: '=',
                enablecheckboxes: '@',
                enablelistviewsearch: '@',
                enablelistviewexpand: '@'
            },
            compile: function (element, attrs) {
                //config
                //var showheader = (attrs.showheader !== 'false');
                var hideoptions = attrs.hideoptions === 'true' ? 'hide-options' : '';
                var template = '<ul class="umb-tree ' + hideoptions + '"><li class="root">';
                template += '<div data-element="tree-root" ng-class="getNodeCssClass(tree.root)" ng-hide="hideheader" on-right-click="altSelect(tree.root, $event)">' + '<h5>' + '<a href="#/{{section}}" ng-click="select(tree.root, $event)"  class="root-link"><i ng-if="enablecheckboxes == \'true\'" ng-class="selectEnabledNodeClass(tree.root)"></i> {{tree.name}}</a></h5>' + '<a data-element="tree-item-options" class="umb-options" ng-hide="tree.root.isContainer || !tree.root.menuUrl" ng-click="options(tree.root, $event)" ng-swipe-right="options(tree.root, $event)"><i></i><i></i><i></i></a>' + '</div>';
                template += '<ul>' + '<umb-tree-item ng-repeat="child in tree.root.children" enablelistviewexpand="{{enablelistviewexpand}}" eventhandler="eventhandler" node="child" current-node="currentNode" tree="this" section="{{section}}" ng-animate="animation()"></umb-tree-item>' + '</ul>' + '</li>' + '</ul>';
                element.replaceWith(template);
                return function (scope, elem, attr, controller) {
                    //flag to track the last loaded section when the tree 'un-loads'. We use this to determine if we should
                    // re-load the tree again. For example, if we hover over 'content' the content tree is shown. Then we hover
                    // outside of the tree and the tree 'un-loads'. When we re-hover over 'content', we don't want to re-load the
                    // entire tree again since we already still have it in memory. Of course if the section is different we will
                    // reload it. This saves a lot on processing if someone is navigating in and out of the same section many times
                    // since it saves on data retreival and DOM processing.
                    var lastSection = '';
                    //setup a default internal handler
                    if (!scope.eventhandler) {
                        scope.eventhandler = $({});
                    }
                    //flag to enable/disable delete animations
                    var deleteAnimations = false;
                    /** Helper function to emit tree events */
                    function emitEvent(eventName, args) {
                        if (scope.eventhandler) {
                            $(scope.eventhandler).trigger(eventName, args);
                        }
                    }
                    /** This will deleteAnimations to true after the current digest */
                    function enableDeleteAnimations() {
                        //do timeout so that it re-enables them after this digest
                        $timeout(function () {
                            //enable delete animations
                            deleteAnimations = true;
                        }, 0, false);
                    }
                    /*this is the only external interface a tree has */
                    function setupExternalEvents() {
                        if (scope.eventhandler) {
                            scope.eventhandler.clearCache = function (section) {
                                treeService.clearCache({ section: section });
                            };
                            scope.eventhandler.load = function (section) {
                                scope.section = section;
                                loadTree();
                            };
                            scope.eventhandler.reloadNode = function (node) {
                                if (!node) {
                                    node = scope.currentNode;
                                }
                                if (node) {
                                    scope.loadChildren(node, true);
                                }
                            };
                            /**
                            Used to do the tree syncing. If the args.tree is not specified we are assuming it has been
                            specified previously using the _setActiveTreeType
                        */
                            scope.eventhandler.syncTree = function (args) {
                                if (!args) {
                                    throw 'args cannot be null';
                                }
                                if (!args.path) {
                                    throw 'args.path cannot be null';
                                }
                                var deferred = $q.defer();
                                //this is super complex but seems to be working in other places, here we're listening for our
                                // own events, once the tree is sycned we'll resolve our promise.
                                scope.eventhandler.one('treeSynced', function (e, syncArgs) {
                                    deferred.resolve(syncArgs);
                                });
                                //this should normally be set unless it is being called from legacy
                                // code, so set the active tree type before proceeding.
                                if (args.tree) {
                                    loadActiveTree(args.tree);
                                }
                                if (angular.isString(args.path)) {
                                    args.path = args.path.replace('"', '').split(',');
                                }
                                //reset current node selection
                                //scope.currentNode = null;
                                //Filter the path for root node ids (we don't want to pass in -1 or 'init')
                                args.path = _.filter(args.path, function (item) {
                                    return item !== 'init' && item !== '-1';
                                });
                                loadPath(args.path, args.forceReload, args.activate);
                                return deferred.promise;
                            };
                            /**
                            Internal method that should ONLY be used by the legacy API wrapper, the legacy API used to
                            have to set an active tree and then sync, the new API does this in one method by using syncTree.
                            loadChildren is optional but if it is set, it will set the current active tree and load the root
                            node's children - this is synonymous with the legacy refreshTree method - again should not be used
                            and should only be used for the legacy code to work.
                        */
                            scope.eventhandler._setActiveTreeType = function (treeAlias, loadChildren) {
                                loadActiveTree(treeAlias, loadChildren);
                            };
                        }
                    }
                    //helper to load a specific path on the active tree as soon as its ready
                    function loadPath(path, forceReload, activate) {
                        if (scope.activeTree) {
                            syncTree(scope.activeTree, path, forceReload, activate);
                        } else {
                            scope.eventhandler.one('activeTreeLoaded', function (e, args) {
                                syncTree(args.tree, path, forceReload, activate);
                            });
                        }
                    }
                    //given a tree alias, this will search the current section tree for the specified tree alias and
                    //set that to the activeTree
                    //NOTE: loadChildren is ONLY used for legacy purposes, do not use this when syncing the tree as it will cause problems
                    // since there will be double request and event handling operations.
                    function loadActiveTree(treeAlias, loadChildren) {
                        if (!treeAlias) {
                            return;
                        }
                        scope.activeTree = undefined;
                        function doLoad(tree) {
                            var childrenAndSelf = [tree].concat(tree.children);
                            scope.activeTree = _.find(childrenAndSelf, function (node) {
                                if (node && node.metaData && node.metaData.treeAlias) {
                                    return node.metaData.treeAlias.toUpperCase() === treeAlias.toUpperCase();
                                }
                                return false;
                            });
                            if (!scope.activeTree) {
                                throw 'Could not find the tree ' + treeAlias + ', activeTree has not been set';
                            }
                            //This is only used for the legacy tree method refreshTree!
                            if (loadChildren) {
                                scope.activeTree.expanded = true;
                                scope.loadChildren(scope.activeTree, false).then(function () {
                                    emitEvent('activeTreeLoaded', { tree: scope.activeTree });
                                });
                            } else {
                                emitEvent('activeTreeLoaded', { tree: scope.activeTree });
                            }
                        }
                        if (scope.tree) {
                            doLoad(scope.tree.root);
                        } else {
                            scope.eventhandler.one('treeLoaded', function (e, args) {
                                doLoad(args.tree.root);
                            });
                        }
                    }
                    /** Method to load in the tree data */
                    function loadTree() {
                        if (!scope.loading && scope.section) {
                            scope.loading = true;
                            //anytime we want to load the tree we need to disable the delete animations
                            deleteAnimations = false;
                            //default args
                            var args = {
                                section: scope.section,
                                tree: scope.treealias,
                                cacheKey: scope.cachekey,
                                isDialog: scope.isdialog ? scope.isdialog : false,
                                onlyinitialized: scope.onlyinitialized
                            };
                            //add the extra query string params if specified
                            if (scope.customtreeparams) {
                                args['queryString'] = scope.customtreeparams;
                            }
                            treeService.getTree(args).then(function (data) {
                                //set the data once we have it
                                scope.tree = data;
                                enableDeleteAnimations();
                                scope.loading = false;
                                //set the root as the current active tree
                                scope.activeTree = scope.tree.root;
                                emitEvent('treeLoaded', { tree: scope.tree });
                                emitEvent('treeNodeExpanded', {
                                    tree: scope.tree,
                                    node: scope.tree.root,
                                    children: scope.tree.root.children
                                });
                            }, function (reason) {
                                scope.loading = false;
                                notificationsService.error('Tree Error', reason);
                            });
                        }
                    }
                    /** syncs the tree, the treeNode can be ANY tree node in the tree that requires syncing */
                    function syncTree(treeNode, path, forceReload, activate) {
                        deleteAnimations = false;
                        treeService.syncTree({
                            node: treeNode,
                            path: path,
                            forceReload: forceReload,
                            //when the tree node is expanding during sync tree, handle it and raise appropriate events
                            treeNodeExpanded: function (args) {
                                emitEvent('treeNodeExpanded', {
                                    tree: scope.tree,
                                    node: args.node,
                                    children: args.children
                                });
                            }
                        }).then(function (data) {
                            if (activate === undefined || activate === true) {
                                scope.currentNode = data;
                            }
                            emitEvent('treeSynced', {
                                node: data,
                                activate: activate
                            });
                            enableDeleteAnimations();
                        });
                    }
                    /** Returns the css classses assigned to the node (div element) */
                    scope.getNodeCssClass = function (node) {
                        if (!node) {
                            return '';
                        }
                        //TODO: This is called constantly because as a method in a template it's re-evaluated pretty much all the time
                        // it would be better if we could cache the processing. The problem is that some of these things are dynamic.
                        var css = [];
                        if (node.cssClasses) {
                            _.each(node.cssClasses, function (c) {
                                css.push(c);
                            });
                        }
                        return css.join(' ');
                    };
                    scope.selectEnabledNodeClass = function (node) {
                        return node ? node.selected ? 'icon umb-tree-icon sprTree icon-check green temporary' : '' : '';
                    };
                    /** method to set the current animation for the node.
                 *  This changes dynamically based on if we are changing sections or just loading normal tree data.
                 *  When changing sections we don't want all of the tree-ndoes to do their 'leave' animations.
                 */
                    scope.animation = function () {
                        if (deleteAnimations && scope.tree && scope.tree.root && scope.tree.root.expanded) {
                            return { leave: 'tree-node-delete-leave' };
                        } else {
                            return {};
                        }
                    };
                    /* helper to force reloading children of a tree node */
                    scope.loadChildren = function (node, forceReload) {
                        var deferred = $q.defer();
                        //emit treeNodeExpanding event, if a callback object is set on the tree
                        emitEvent('treeNodeExpanding', {
                            tree: scope.tree,
                            node: node
                        });
                        //standardising
                        if (!node.children) {
                            node.children = [];
                        }
                        if (forceReload || node.hasChildren && node.children.length === 0) {
                            //get the children from the tree service
                            treeService.loadNodeChildren({
                                node: node,
                                section: scope.section
                            }).then(function (data) {
                                //emit expanded event
                                emitEvent('treeNodeExpanded', {
                                    tree: scope.tree,
                                    node: node,
                                    children: data
                                });
                                enableDeleteAnimations();
                                deferred.resolve(data);
                            });
                        } else {
                            emitEvent('treeNodeExpanded', {
                                tree: scope.tree,
                                node: node,
                                children: node.children
                            });
                            node.expanded = true;
                            enableDeleteAnimations();
                            deferred.resolve(node.children);
                        }
                        return deferred.promise;
                    };
                    /**
                  Method called when the options button next to the root node is called.
                  The tree doesnt know about this, so it raises an event to tell the parent controller
                  about it.
                */
                    scope.options = function (n, ev) {
                        emitEvent('treeOptionsClick', {
                            element: elem,
                            node: n,
                            event: ev
                        });
                    };
                    /**
                  Method called when an item is clicked in the tree, this passes the
                  DOM element, the tree node object and the original click
                  and emits it as a treeNodeSelect element if there is a callback object
                  defined on the tree
                */
                    scope.select = function (n, ev) {
                        if (n.metaData && n.metaData.noAccess === true) {
                            ev.preventDefault();
                            return;
                        }
                        //on tree select we need to remove the current node -
                        // whoever handles this will need to make sure the correct node is selected
                        //reset current node selection
                        scope.currentNode = null;
                        emitEvent('treeNodeSelect', {
                            element: elem,
                            node: n,
                            event: ev
                        });
                    };
                    scope.altSelect = function (n, ev) {
                        emitEvent('treeNodeAltSelect', {
                            element: elem,
                            tree: scope.tree,
                            node: n,
                            event: ev
                        });
                    };
                    //watch for section changes
                    scope.$watch('section', function (newVal, oldVal) {
                        if (!scope.tree) {
                            loadTree();
                        }
                        if (!newVal) {
                            //store the last section loaded
                            lastSection = oldVal;
                        } else if (newVal !== oldVal && newVal !== lastSection) {
                            //only reload the tree data and Dom if the newval is different from the old one
                            // and if the last section loaded is different from the requested one.
                            loadTree();
                            //store the new section to be loaded as the last section
                            //clear any active trees to reset lookups
                            lastSection = newVal;
                        }
                    });
                    setupExternalEvents();
                    loadTree();
                };
            }
        };
    }
    angular.module('umbraco.directives').directive('umbTree', umbTreeDirective);
    /**
 * @ngdoc directive
 * @name umbraco.directives.directive:umbTreeItem
 * @element li
 * @function
 *
 * @description
 * Renders a list item, representing a single node in the tree.
 * Includes element to toggle children, and a menu toggling button
 *
 * **note:** This directive is only used internally in the umbTree directive
 *
 * @example
   <example module="umbraco">
    <file name="index.html">
         <umb-tree-item ng-repeat="child in tree.children" node="child" callback="callback" section="content"></umb-tree-item>
    </file>
   </example>
 */
    angular.module('umbraco.directives').directive('umbTreeItem', function ($compile, $http, $templateCache, $interpolate, $log, $location, $rootScope, $window, treeService, $timeout, localizationService, appState) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                section: '@',
                eventhandler: '=',
                currentNode: '=',
                enablelistviewexpand: '@',
                node: '=',
                tree: '='
            },
            //TODO: Remove more of the binding from this template and move the DOM manipulation to be manually done in the link function,
            // this will greatly improve performance since there's potentially a lot of nodes being rendered = a LOT of watches!
            template: '<li data-element="tree-item-{{node.dataElement}}" ng-class="{\'current\': (node == currentNode), \'has-children\': node.hasChildren}" on-right-click="altSelect(node, $event)">' + '<div ng-class="getNodeCssClass(node)" ng-swipe-right="options(node, $event)" ng-dblclick="load(node)" >' + //NOTE: This ins element is used to display the search icon if the node is a container/listview and the tree is currently in dialog
            //'<ins ng-if="tree.enablelistviewsearch && node.metaData.isContainer" class="umb-tree-node-search icon-search" ng-click="searchNode(node, $event)" alt="searchAltText"></ins>' + 
            '<ins data-element="tree-item-expand" ng-class="{\'icon-navigation-right\': !node.expanded || node.metaData.isContainer, \'icon-navigation-down\': node.expanded && !node.metaData.isContainer}" ng-click="load(node)">&nbsp;</ins>' + '<i class="icon umb-tree-icon sprTree" ng-click="select(node, $event)"></i>' + '<a class="umb-tree-item__label" href="#/{{node.routePath}}" ng-click="select(node, $event)"></a>' + //NOTE: These are the 'option' elipses
            '<a data-element="tree-item-options" class="umb-options" ng-click="options(node, $event)"><i></i><i></i><i></i></a>' + '<div ng-show="node.loading" class="l"><div></div></div>' + '</div>' + '</li>',
            link: function (scope, element, attrs) {
                localizationService.localize('general_search').then(function (value) {
                    scope.searchAltText = value;
                });
                //flag to enable/disable delete animations, default for an item is true
                var deleteAnimations = true;
                // Helper function to emit tree events
                function emitEvent(eventName, args) {
                    if (scope.eventhandler) {
                        $(scope.eventhandler).trigger(eventName, args);
                    }
                }
                // updates the node's DOM/styles
                function setupNodeDom(node, tree) {
                    //get the first div element
                    element.children(':first')    //set the padding
.css('padding-left', node.level * 20 + 'px');
                    //toggle visibility of last 'ins' depending on children
                    //visibility still ensure the space is "reserved", so both nodes with and without children are aligned.
                    if (node.hasChildren || node.metaData.isContainer && scope.enablelistviewexpand === 'true') {
                        element.find('ins').last().css('visibility', 'visible');
                    } else {
                        element.find('ins').last().css('visibility', 'hidden');
                    }
                    var icon = element.find('i:first');
                    icon.addClass(node.cssClass);
                    icon.attr('title', node.routePath);
                    element.find('a:first').text(node.name);
                    if (!node.menuUrl) {
                        element.find('a.umb-options').remove();
                    }
                    if (node.style) {
                        element.find('i:first').attr('style', node.style);
                    }
                    // add a unique data element to each tree item so it is easy to navigate with code
                    if (!node.metaData.treeAlias) {
                        node.dataElement = node.name;
                    } else {
                        node.dataElement = node.metaData.treeAlias;
                    }
                }
                //This will deleteAnimations to true after the current digest
                function enableDeleteAnimations() {
                    //do timeout so that it re-enables them after this digest
                    $timeout(function () {
                        //enable delete animations
                        deleteAnimations = true;
                    }, 0, false);
                }
                /** Returns the css classses assigned to the node (div element) */
                scope.getNodeCssClass = function (node) {
                    if (!node) {
                        return '';
                    }
                    //TODO: This is called constantly because as a method in a template it's re-evaluated pretty much all the time
                    // it would be better if we could cache the processing. The problem is that some of these things are dynamic.
                    var css = [];
                    if (node.cssClasses) {
                        _.each(node.cssClasses, function (c) {
                            css.push(c);
                        });
                    }
                    if (node.selected) {
                        css.push('umb-tree-node-checked');
                    }
                    //is this the current action node (this is not the same as the current selected node!)
                    var actionNode = appState.getMenuState('currentNode');
                    if (actionNode) {
                        if (actionNode.id === node.id && actionNode.id !== '-1') {
                            css.push('active');
                        }
                        // special handling of root nodes with id -1 
                        // as there can be many nodes with id -1 in a tree we need to check the treeAlias instead
                        if (actionNode.id === '-1' && actionNode.metaData.treeAlias === node.metaData.treeAlias) {
                            css.push('active');
                        }
                    }
                    return css.join(' ');
                };
                //add a method to the node which we can use to call to update the node data if we need to ,
                // this is done by sync tree, we don't want to add a $watch for each node as that would be crazy insane slow
                // so we have to do this
                scope.node.updateNodeData = function (newNode) {
                    _.extend(scope.node, newNode);
                    //now update the styles
                    setupNodeDom(scope.node, scope.tree);
                };
                /**
              Method called when the options button next to a node is called
              In the main tree this opens the menu, but internally the tree doesnt
              know about this, so it simply raises an event to tell the parent controller
              about it.
            */
                scope.options = function (n, ev) {
                    emitEvent('treeOptionsClick', {
                        element: element,
                        tree: scope.tree,
                        node: n,
                        event: ev
                    });
                };
                /**
              Method called when an item is clicked in the tree, this passes the 
              DOM element, the tree node object and the original click
              and emits it as a treeNodeSelect element if there is a callback object
              defined on the tree
            */
                scope.select = function (n, ev) {
                    if (ev.ctrlKey || ev.shiftKey || ev.metaKey || ev.button && ev.button === 1    // middle click, >IE9 + everyone else
) {
                        return;
                    }
                    if (n.metaData && n.metaData.noAccess === true) {
                        ev.preventDefault();
                        return;
                    }
                    emitEvent('treeNodeSelect', {
                        element: element,
                        tree: scope.tree,
                        node: n,
                        event: ev
                    });
                    ev.preventDefault();
                };
                /**
              Method called when an item is right-clicked in the tree, this passes the 
              DOM element, the tree node object and the original click
              and emits it as a treeNodeSelect element if there is a callback object
              defined on the tree
            */
                scope.altSelect = function (n, ev) {
                    emitEvent('treeNodeAltSelect', {
                        element: element,
                        tree: scope.tree,
                        node: n,
                        event: ev
                    });
                };
                /** method to set the current animation for the node. 
            *  This changes dynamically based on if we are changing sections or just loading normal tree data. 
            *  When changing sections we don't want all of the tree-ndoes to do their 'leave' animations.
            */
                scope.animation = function () {
                    if (scope.node.showHideAnimation) {
                        return scope.node.showHideAnimation;
                    }
                    if (deleteAnimations && scope.node.expanded) {
                        return { leave: 'tree-node-delete-leave' };
                    } else {
                        return {};
                    }
                };
                /**
              Method called when a node in the tree is expanded, when clicking the arrow
              takes the arrow DOM element and node data as parameters
              emits treeNodeCollapsing event if already expanded and treeNodeExpanding if collapsed
            */
                scope.load = function (node) {
                    if (node.expanded && !node.metaData.isContainer) {
                        deleteAnimations = false;
                        emitEvent('treeNodeCollapsing', {
                            tree: scope.tree,
                            node: node,
                            element: element
                        });
                        node.expanded = false;
                    } else {
                        scope.loadChildren(node, false);
                    }
                };
                /* helper to force reloading children of a tree node */
                scope.loadChildren = function (node, forceReload) {
                    //emit treeNodeExpanding event, if a callback object is set on the tree
                    emitEvent('treeNodeExpanding', {
                        tree: scope.tree,
                        node: node
                    });
                    if (node.hasChildren && (forceReload || !node.children || angular.isArray(node.children) && node.children.length === 0)) {
                        //get the children from the tree service
                        treeService.loadNodeChildren({
                            node: node,
                            section: scope.section
                        }).then(function (data) {
                            //emit expanded event
                            emitEvent('treeNodeExpanded', {
                                tree: scope.tree,
                                node: node,
                                children: data
                            });
                            enableDeleteAnimations();
                        });
                    } else {
                        emitEvent('treeNodeExpanded', {
                            tree: scope.tree,
                            node: node,
                            children: node.children
                        });
                        node.expanded = true;
                        enableDeleteAnimations();
                    }
                };
                //if the current path contains the node id, we will auto-expand the tree item children
                setupNodeDom(scope.node, scope.tree);
                // load the children if the current user don't have access to the node
                // it is used to auto expand the tree to the start nodes the user has access to
                if (scope.node.hasChildren && scope.node.metaData.noAccess) {
                    scope.loadChildren(scope.node);
                }
                var template = '<ul ng-class="{collapsed: !node.expanded}"><umb-tree-item  ng-repeat="child in node.children" enablelistviewexpand="{{enablelistviewexpand}}" eventhandler="eventhandler" tree="tree" current-node="currentNode" node="child" section="{{section}}" ng-animate="animation()"></umb-tree-item></ul>';
                var newElement = angular.element(template);
                $compile(newElement)(scope);
                element.append(newElement);
            }
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbTreeSearchBox
* @function
* @element ANY
* @restrict E
**/
    function treeSearchBox(localizationService, searchService, $q) {
        return {
            scope: {
                searchFromId: '@',
                searchFromName: '@',
                showSearch: '@',
                section: '@',
                datatypeId: '@',
                hideSearchCallback: '=',
                searchCallback: '='
            },
            restrict: 'E',
            // restrict to an element
            replace: true,
            // replace the html element with the template
            templateUrl: 'views/components/tree/umb-tree-search-box.html',
            link: function (scope, element, attrs, ctrl) {
                scope.term = '';
                scope.hideSearch = function () {
                    scope.term = '';
                    scope.hideSearchCallback();
                };
                localizationService.localize('general_typeToSearch').then(function (value) {
                    scope.searchPlaceholderText = value;
                });
                if (!scope.showSearch) {
                    scope.showSearch = 'false';
                }
                //used to cancel any request in progress if another one needs to take it's place
                var canceler = null;
                function performSearch() {
                    if (scope.term) {
                        scope.results = [];
                        //a canceler exists, so perform the cancelation operation and reset
                        if (canceler) {
                            canceler.resolve();
                            canceler = $q.defer();
                        } else {
                            canceler = $q.defer();
                        }
                        var searchArgs = {
                            term: scope.term,
                            canceler: canceler
                        };
                        //append a start node context if there is one
                        if (scope.searchFromId) {
                            searchArgs['searchFrom'] = scope.searchFromId;
                        }
                        //append dataTypeId value if there is one
                        if (scope.datatypeId) {
                            searchArgs['dataTypeId'] = scope.datatypeId;
                        }
                        searcher(searchArgs).then(function (data) {
                            scope.searchCallback(data);
                            //set back to null so it can be re-created
                            canceler = null;
                        });
                    }
                }
                scope.$watch('term', _.debounce(function (newVal, oldVal) {
                    scope.$apply(function () {
                        if (newVal !== null && newVal !== undefined && newVal !== oldVal) {
                            performSearch();
                        }
                    });
                }, 200));
                var searcher = searchService.searchContent;
                //search
                if (scope.section === 'member') {
                    searcher = searchService.searchMembers;
                } else if (scope.section === 'media') {
                    searcher = searchService.searchMedia;
                }
            }
        };
    }
    angular.module('umbraco.directives').directive('umbTreeSearchBox', treeSearchBox);
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbTreeSearchResults
* @function
* @element ANY
* @restrict E
**/
    function treeSearchResults() {
        return {
            scope: {
                results: '=',
                selectResultCallback: '='
            },
            restrict: 'E',
            // restrict to an element
            replace: true,
            // replace the html element with the template
            templateUrl: 'views/components/tree/umb-tree-search-results.html',
            link: function (scope, element, attrs, ctrl) {
            }
        };
    }
    angular.module('umbraco.directives').directive('umbTreeSearchResults', treeSearchResults);
    (function () {
        'use strict';
        function AceEditorDirective(umbAceEditorConfig, assetsService, angularHelper) {
            /**
         * Sets editor options such as the wrapping mode or the syntax checker.
         *
         * The supported options are:
         *
         *   <ul>
         *     <li>showGutter</li>
         *     <li>useWrapMode</li>
         *     <li>onLoad</li>
         *     <li>theme</li>
         *     <li>mode</li>
         *   </ul>
         *
         * @param acee
         * @param session ACE editor session
         * @param {object} opts Options to be set
         */
            var setOptions = function (acee, session, opts) {
                // sets the ace worker path, if running from concatenated
                // or minified source
                if (angular.isDefined(opts.workerPath)) {
                    var config = window.ace.require('ace/config');
                    config.set('workerPath', opts.workerPath);
                }
                // ace requires loading
                if (angular.isDefined(opts.require)) {
                    opts.require.forEach(function (n) {
                        window.ace.require(n);
                    });
                }
                // Boolean options
                if (angular.isDefined(opts.showGutter)) {
                    acee.renderer.setShowGutter(opts.showGutter);
                }
                if (angular.isDefined(opts.useWrapMode)) {
                    session.setUseWrapMode(opts.useWrapMode);
                }
                if (angular.isDefined(opts.showInvisibles)) {
                    acee.renderer.setShowInvisibles(opts.showInvisibles);
                }
                if (angular.isDefined(opts.showIndentGuides)) {
                    acee.renderer.setDisplayIndentGuides(opts.showIndentGuides);
                }
                if (angular.isDefined(opts.useSoftTabs)) {
                    session.setUseSoftTabs(opts.useSoftTabs);
                }
                if (angular.isDefined(opts.showPrintMargin)) {
                    acee.setShowPrintMargin(opts.showPrintMargin);
                }
                // commands
                if (angular.isDefined(opts.disableSearch) && opts.disableSearch) {
                    acee.commands.addCommands([{
                            name: 'unfind',
                            bindKey: {
                                win: 'Ctrl-F',
                                mac: 'Command-F'
                            },
                            exec: function () {
                                return false;
                            },
                            readOnly: true
                        }]);
                }
                // Basic options
                if (angular.isString(opts.theme)) {
                    acee.setTheme('ace/theme/' + opts.theme);
                }
                if (angular.isString(opts.mode)) {
                    session.setMode('ace/mode/' + opts.mode);
                }
                // Advanced options
                if (angular.isDefined(opts.firstLineNumber)) {
                    if (angular.isNumber(opts.firstLineNumber)) {
                        session.setOption('firstLineNumber', opts.firstLineNumber);
                    } else if (angular.isFunction(opts.firstLineNumber)) {
                        session.setOption('firstLineNumber', opts.firstLineNumber());
                    }
                }
                // advanced options
                var key, obj;
                if (angular.isDefined(opts.advanced)) {
                    for (key in opts.advanced) {
                        // create a javascript object with the key and value
                        obj = {
                            name: key,
                            value: opts.advanced[key]
                        };
                        // try to assign the option to the ace editor
                        acee.setOption(obj.name, obj.value);
                    }
                }
                // advanced options for the renderer
                if (angular.isDefined(opts.rendererOptions)) {
                    for (key in opts.rendererOptions) {
                        // create a javascript object with the key and value
                        obj = {
                            name: key,
                            value: opts.rendererOptions[key]
                        };
                        // try to assign the option to the ace editor
                        acee.renderer.setOption(obj.name, obj.value);
                    }
                }
                // onLoad callbacks
                angular.forEach(opts.callbacks, function (cb) {
                    if (angular.isFunction(cb)) {
                        cb(acee);
                    }
                });
            };
            function link(scope, el, attr, ngModel) {
                // Load in ace library
                assetsService.load([
                    'lib/ace-builds/src-min-noconflict/ace.js',
                    'lib/ace-builds/src-min-noconflict/ext-language_tools.js'
                ], scope).then(function () {
                    if (angular.isUndefined(window.ace)) {
                        throw new Error('ui-ace need ace to work... (o rly?)');
                    } else {
                        // init editor
                        init();
                    }
                });
                function init() {
                    /**
                 * Corresponds the umbAceEditorConfig ACE configuration.
                 * @type object
                 */
                    var options = umbAceEditorConfig.ace || {};
                    /**
                 * umbAceEditorConfig merged with user options via json in attribute or data binding
                 * @type object
                 */
                    var opts = angular.extend({}, options, scope.umbAceEditor);
                    //load ace libraries here... 
                    /**
                 * ACE editor
                 * @type object
                 */
                    var acee = window.ace.edit(el[0]);
                    acee.$blockScrolling = Infinity;
                    /**
                 * ACE editor session.
                 * @type object
                 * @see [EditSession]{@link https://ace.c9.io/#nav=api&api=edit_session}
                 */
                    var session = acee.getSession();
                    /**
                 * Reference to a change listener created by the listener factory.
                 * @function
                 * @see listenerFactory.onChange
                 */
                    var onChangeListener;
                    /**
                 * Reference to a blur listener created by the listener factory.
                 * @function
                 * @see listenerFactory.onBlur
                 */
                    var onBlurListener;
                    /**
                 * Calls a callback by checking its existing. The argument list
                 * is variable and thus this function is relying on the arguments
                 * object.
                 * @throws {Error} If the callback isn't a function
                 */
                    var executeUserCallback = function () {
                        /**
                     * The callback function grabbed from the array-like arguments
                     * object. The first argument should always be the callback.
                     *
                     * @see [arguments]{@link https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Functions_and_function_scope/arguments}
                     * @type {*}
                     */
                        var callback = arguments[0];
                        /**
                     * Arguments to be passed to the callback. These are taken
                     * from the array-like arguments object. The first argument
                     * is stripped because that should be the callback function.
                     *
                     * @see [arguments]{@link https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Functions_and_function_scope/arguments}
                     * @type {Array}
                     */
                        var args = Array.prototype.slice.call(arguments, 1);
                        if (angular.isDefined(callback)) {
                            scope.$evalAsync(function () {
                                if (angular.isFunction(callback)) {
                                    callback(args);
                                } else {
                                    throw new Error('ui-ace use a function as callback.');
                                }
                            });
                        }
                    };
                    /**
                 * Listener factory. Until now only change listeners can be created.
                 * @type object
                 */
                    var listenerFactory = {
                        /**
                     * Creates a change listener which propagates the change event
                     * and the editor session to the callback from the user option
                     * onChange. It might be exchanged during runtime, if this
                     * happens the old listener will be unbound.
                     *
                     * @param callback callback function defined in the user options
                     * @see onChangeListener
                     */
                        onChange: function (callback) {
                            return function (e) {
                                var newValue = session.getValue();
                                angularHelper.safeApply(scope, function () {
                                    scope.model = newValue;
                                });
                                executeUserCallback(callback, e, acee);
                            };
                        },
                        /**
                     * Creates a blur listener which propagates the editor session
                     * to the callback from the user option onBlur. It might be
                     * exchanged during runtime, if this happens the old listener
                     * will be unbound.
                     *
                     * @param callback callback function defined in the user options
                     * @see onBlurListener
                     */
                        onBlur: function (callback) {
                            return function () {
                                executeUserCallback(callback, acee);
                            };
                        }
                    };
                    attr.$observe('readonly', function (value) {
                        acee.setReadOnly(!!value || value === '');
                    });
                    // Value Blind
                    if (scope.model) {
                        session.setValue(scope.model);
                    }
                    // Listen for option updates
                    var updateOptions = function (current, previous) {
                        if (current === previous) {
                            return;
                        }
                        opts = angular.extend({}, options, scope.umbAceEditor);
                        opts.callbacks = [opts.onLoad];
                        if (opts.onLoad !== options.onLoad) {
                            // also call the global onLoad handler
                            opts.callbacks.unshift(options.onLoad);
                        }
                        // EVENTS
                        // unbind old change listener
                        session.removeListener('change', onChangeListener);
                        // bind new change listener
                        onChangeListener = listenerFactory.onChange(opts.onChange);
                        session.on('change', onChangeListener);
                        // unbind old blur listener
                        //session.removeListener('blur', onBlurListener);
                        acee.removeListener('blur', onBlurListener);
                        // bind new blur listener
                        onBlurListener = listenerFactory.onBlur(opts.onBlur);
                        acee.on('blur', onBlurListener);
                        setOptions(acee, session, opts);
                    };
                    scope.$watch(scope.umbAceEditor, updateOptions, /* deep watch */
                    true);
                    // set the options here, even if we try to watch later, if this
                    // line is missing things go wrong (and the tests will also fail)
                    updateOptions(options);
                    el.on('$destroy', function () {
                        acee.session.$stopWorker();
                        acee.destroy();
                    });
                    scope.$watch(function () {
                        return [
                            el[0].offsetWidth,
                            el[0].offsetHeight
                        ];
                    }, function () {
                        acee.resize();
                        acee.renderer.updateFull();
                    }, true);
                }
            }
            var directive = {
                restrict: 'EA',
                scope: {
                    'umbAceEditor': '=',
                    'model': '='
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').constant('umbAceEditorConfig', {}).directive('umbAceEditor', AceEditorDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbAvatar
@restrict E
@scope

@description
Use this directive to render an avatar.

<h3>Markup example</h3>
<pre>
	<div ng-controller="My.Controller as vm">

        <umb-avatar
            size="xs"
            img-src="{{vm.avatar[0].value}}"
            img-srcset="{{vm.avatar[1].value}} 2x, {{vm.avatar[2].value}} 3x">
        </umb-avatar>

	</div>
</pre>

<h3>Controller example</h3>
<pre>
	(function () {
		"use strict";

		function Controller() {

            var vm = this;

            vm.avatar = [
                { value: "assets/logo.png" },
                { value: "assets/logo@2x.png" },
                { value: "assets/logo@3x.png" }
            ];

        }

		angular.module("umbraco").controller("My.Controller", Controller);

	})();
</pre>

@param {string} size (<code>attribute</code>): The size of the avatar (xs, s, m, l, xl).
@param {string} img-src (<code>attribute</code>): The image source to the avatar.
@param {string} img-srcset (<code>atribute</code>): Reponsive support for the image source.
**/
    (function () {
        'use strict';
        function AvatarDirective() {
            function link(scope, element, attrs, ctrl) {
                var eventBindings = [];
                scope.initials = '';
                function onInit() {
                    if (!scope.unknownChar) {
                        scope.unknownChar = '?';
                    }
                    scope.initials = getNameInitials(scope.name);
                }
                function getNameInitials(name) {
                    if (name) {
                        var names = name.split(' '), initials = names[0].substring(0, 1);
                        if (names.length > 1) {
                            initials += names[names.length - 1].substring(0, 1);
                        }
                        return initials.toUpperCase();
                    }
                    return null;
                }
                eventBindings.push(scope.$watch('name', function (newValue, oldValue) {
                    if (newValue === oldValue) {
                        return;
                    }
                    if (oldValue === undefined || newValue === undefined) {
                        return;
                    }
                    scope.initials = getNameInitials(newValue);
                }));
                onInit();
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-avatar.html',
                scope: {
                    size: '@',
                    name: '@',
                    color: '@',
                    imgSrc: '@',
                    imgSrcset: '@',
                    unknownChar: '@'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbAvatar', AvatarDirective);
    }());
    (function () {
        'use strict';
        function BadgeDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/umb-badge.html',
                scope: {
                    size: '@?',
                    color: '@?'
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbBadge', BadgeDirective);
    }());
    (function () {
        'use strict';
        function CheckmarkDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/umb-checkmark.html',
                scope: {
                    size: '@?',
                    checked: '='
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbCheckmark', CheckmarkDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbChildSelector
@restrict E
@scope

@description
Use this directive to render a ui component for selecting child items to a parent node.

<h3>Markup example</h3>
<pre>
	<div ng-controller="My.Controller as vm">

        <umb-child-selector
                selected-children="vm.selectedChildren"
                available-children="vm.availableChildren"
                parent-name="vm.name"
                parent-icon="vm.icon"
                parent-id="vm.id"
                on-add="vm.addChild"
                on-remove="vm.removeChild">
        </umb-child-selector>

        <!-- use overlay to select children from -->
        <umb-overlay
           ng-if="vm.overlay.show"
           model="vm.overlay"
           position="target"
           view="vm.overlay.view">
        </umb-overlay>

	</div>
</pre>

<h3>Controller example</h3>
<pre>
	(function () {
		"use strict";

		function Controller() {

            var vm = this;

            vm.id = 1;
            vm.name = "My Parent element";
            vm.icon = "icon-document";
            vm.selectedChildren = [];
            vm.availableChildren = [
                {
                    id: 1,
                    alias: "item1",
                    name: "Item 1",
                    icon: "icon-document"
                },
                {
                    id: 2,
                    alias: "item2",
                    name: "Item 2",
                    icon: "icon-document"
                }
            ];

            vm.addChild = addChild;
            vm.removeChild = removeChild;

            function addChild($event) {
                vm.overlay = {
                    view: "itempicker",
                    title: "Choose child",
                    availableItems: vm.availableChildren,
                    selectedItems: vm.selectedChildren,
                    event: $event,
                    show: true,
                    submit: function(model) {

                        // add selected child
                        vm.selectedChildren.push(model.selectedItem);

                        // close overlay
                        vm.overlay.show = false;
                        vm.overlay = null;
                    }
                };
            }

            function removeChild($index) {
                vm.selectedChildren.splice($index, 1);
            }

        }

		angular.module("umbraco").controller("My.Controller", Controller);

	})();
</pre>

@param {array} selectedChildren (<code>binding</code>): Array of selected children.
@param {array} availableChildren (<code>binding</code>: Array of items available for selection.
@param {string} parentName (<code>binding</code>): The parent name.
@param {string} parentIcon (<code>binding</code>): The parent icon.
@param {number} parentId (<code>binding</code>): The parent id.
@param {callback} onRemove (<code>binding</code>): Callback when the remove button is clicked on an item.
    <h3>The callback returns:</h3>
    <ul>
        <li><code>child</code>: The selected item.</li>
        <li><code>$index</code>: The selected item index.</li>
    </ul>
@param {callback} onAdd (<code>binding</code>): Callback when the add button is clicked.
    <h3>The callback returns:</h3>
    <ul>
        <li><code>$event</code>: The select event.</li>
    </ul>
**/
    (function () {
        'use strict';
        function ChildSelectorDirective() {
            function link(scope, el, attr, ctrl) {
                var eventBindings = [];
                scope.dialogModel = {};
                scope.showDialog = false;
                scope.removeChild = function (selectedChild, $index) {
                    if (scope.onRemove) {
                        scope.onRemove(selectedChild, $index);
                    }
                };
                scope.addChild = function ($event) {
                    if (scope.onAdd) {
                        scope.onAdd($event);
                    }
                };
                function syncParentName() {
                    // update name on available item
                    angular.forEach(scope.availableChildren, function (availableChild) {
                        if (availableChild.id === scope.parentId) {
                            availableChild.name = scope.parentName;
                        }
                    });
                    // update name on selected child
                    angular.forEach(scope.selectedChildren, function (selectedChild) {
                        if (selectedChild.id === scope.parentId) {
                            selectedChild.name = scope.parentName;
                        }
                    });
                }
                function syncParentIcon() {
                    // update icon on available item
                    angular.forEach(scope.availableChildren, function (availableChild) {
                        if (availableChild.id === scope.parentId) {
                            availableChild.icon = scope.parentIcon;
                        }
                    });
                    // update icon on selected child
                    angular.forEach(scope.selectedChildren, function (selectedChild) {
                        if (selectedChild.id === scope.parentId) {
                            selectedChild.icon = scope.parentIcon;
                        }
                    });
                }
                eventBindings.push(scope.$watch('parentName', function (newValue, oldValue) {
                    if (newValue === oldValue) {
                        return;
                    }
                    if (oldValue === undefined || newValue === undefined) {
                        return;
                    }
                    syncParentName();
                }));
                eventBindings.push(scope.$watch('parentIcon', function (newValue, oldValue) {
                    if (newValue === oldValue) {
                        return;
                    }
                    if (oldValue === undefined || newValue === undefined) {
                        return;
                    }
                    syncParentIcon();
                }));
                // clean up
                scope.$on('$destroy', function () {
                    // unbind watchers
                    for (var e in eventBindings) {
                        eventBindings[e]();
                    }
                });
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-child-selector.html',
                scope: {
                    selectedChildren: '=',
                    availableChildren: '=',
                    parentName: '=',
                    parentIcon: '=',
                    parentId: '=',
                    onRemove: '=',
                    onAdd: '='
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbChildSelector', ChildSelectorDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbClipboard
@restrict E
@scope

@description
<strong>Added in Umbraco v. 7.7:</strong> Use this directive to copy content to the clipboard

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.ClipBoardController as vm">
        
        <!-- Copy text from an element -->
        <div id="copy-text">Copy me!</div>
        
        <umb-button
            umb-clipboard
            umb-clipboard-success="vm.copySuccess()"
            umb-clipboard-error="vm.copyError()"
            umb-clipboard-target="#copy-text"
            state="vm.clipboardButtonState"
            type="button"
            label="Copy">
        </umb-button>

        <!-- Cut text from a textarea -->
        <textarea id="cut-text" ng-model="vm.cutText"></textarea>

        <umb-button
            umb-clipboard
            umb-clipboard-success="vm.copySuccess()"
            umb-clipboard-error="vm.copyError()"
            umb-clipboard-target="#cut-text"
            umb-clipboard-action="cut"
            state="vm.clipboardButtonState"
            type="button"
            label="Copy">
        </umb-button>

        <!-- Copy text without an element -->
        <umb-button
            ng-if="vm.copyText"
            umb-clipboard
            umb-clipboard-success="vm.copySuccess()"
            umb-clipboard-error="vm.copyError()"
            umb-clipboard-text="vm.copyText"
            state="vm.clipboardButtonState"
            type="button"
            label="Copy">
        </umb-button>
    
    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {
        "use strict";

        function Controller() {

            var vm = this;

            vm.copyText = "Copy text without element";
            vm.cutText = "Text to cut";

            vm.copySuccess = copySuccess;
            vm.copyError = copyError;

            function copySuccess() {
                vm.clipboardButtonState = "success";
            }
            
            function copyError() {
                vm.clipboardButtonState = "error";
            }

        }

        angular.module("umbraco").controller("My.ClipBoardController", Controller);

    })();
</pre>

@param {callback} umbClipboardSuccess (<code>expression</code>): Callback function when the content is copied.
@param {callback} umbClipboardError (<code>expression</code>): Callback function if the copy fails.
@param {string} umbClipboardTarget (<code>attribute</code>): The target element to copy.
@param {string} umbClipboardAction (<code>attribute</code>): Specify if you want to copy or cut content ("copy", "cut"). Cut only works on <code>input</code> and <code>textarea</code> elements.
@param {string} umbClipboardText (<code>attribute</code>): Use this attribute if you don't have an element to copy from.

**/
    (function () {
        'use strict';
        function umbClipboardDirective($timeout, assetsService) {
            function link(scope, element, attrs, ctrl) {
                var clipboard;
                var target = element[0];
                assetsService.loadJs('lib/clipboard/clipboard.min.js', scope).then(function () {
                    if (scope.umbClipboardTarget) {
                        target.setAttribute('data-clipboard-target', scope.umbClipboardTarget);
                    }
                    if (scope.umbClipboardAction) {
                        target.setAttribute('data-clipboard-action', scope.umbClipboardAction);
                    }
                    if (scope.umbClipboardText) {
                        target.setAttribute('data-clipboard-text', scope.umbClipboardText);
                    }
                    clipboard = new Clipboard(target);
                    clipboard.on('success', function (e) {
                        e.clearSelection();
                        if (scope.umbClipboardSuccess) {
                            scope.$apply(function () {
                                scope.umbClipboardSuccess({ e: e });
                            });
                        }
                    });
                    clipboard.on('error', function (e) {
                        if (scope.umbClipboardError) {
                            scope.$apply(function () {
                                scope.umbClipboardError({ e: e });
                            });
                        }
                    });
                });
                // clean up
                scope.$on('$destroy', function () {
                    clipboard.destroy();
                });
            }
            ////////////
            var directive = {
                restrict: 'A',
                scope: {
                    umbClipboardSuccess: '&?',
                    umbClipboardError: '&?',
                    umbClipboardTarget: '@?',
                    umbClipboardAction: '@?',
                    umbClipboardText: '=?'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbClipboard', umbClipboardDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbColorSwatches
@restrict E
@scope
@description
Use this directive to generate color swatches to pick from.
<h3>Markup example</h3>
<pre>
    <umb-color-swatches
        colors="colors"
        selected-color="color"
        size="s">
    </umb-color-swatches>
</pre>
@param {array} colors (<code>attribute</code>): The array of colors.
@param {string} selectedColor (<code>attribute</code>): The selected color.
@param {string} size (<code>attribute</code>): The size (s, m).
@param {string} useLabel (<code>attribute</code>): Specify if labels should be used.
@param {string} useColorClass (<code>attribute</code>): Specify if color values are css classes.
@param {function} onSelect (<code>expression</code>): Callback function when the item is selected.
**/
    (function () {
        'use strict';
        function ColorSwatchesDirective() {
            function link(scope, el, attr, ctrl) {
                // Set default to true if not defined
                if (angular.isUndefined(scope.useColorClass)) {
                    scope.useColorClass = false;
                }
                scope.setColor = function (color) {
                    //scope.selectedColor({color: color });
                    scope.selectedColor = color;
                    if (scope.onSelect) {
                        scope.onSelect({ color: color });
                    }
                };
            }
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/umb-color-swatches.html',
                scope: {
                    colors: '=?',
                    size: '@',
                    selectedColor: '=',
                    onSelect: '&',
                    useLabel: '=',
                    useColorClass: '=?'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbColorSwatches', ColorSwatchesDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbConfirm
@restrict E
@scope

@description
A confirmation dialog


<h3>Markup example</h3>
<pre>
	<div ng-controller="My.Controller as vm">

       <umb-confirm caption="Title" on-confirm="vm.onConfirm()" on-cancel="vm.onCancel()"></umb-confirm>

	</div>
</pre>

<h3>Controller example</h3>
<pre>
	(function () {
		"use strict";

		function Controller() {

            var vm = this;

            vm.onConfirm = function() {
                alert('Confirm clicked');
            };

            vm.onCancel = function() {
                alert('Cancel clicked');
            }


        }

		angular.module("umbraco").controller("My.Controller", Controller);

	})();
</pre>

@param {string} caption (<code>attribute</code>): The caption shown above the buttons
@param {callback} on-confirm (<code>attribute</code>): The call back when the "OK" button is clicked. If not set the button will not be shown
@param {callback} on-cancel (<code>atribute</code>): The call back when the "Cancel" button is clicked. If not set the button will not be shown
**/
    function confirmDirective() {
        return {
            restrict: 'E',
            // restrict to an element
            replace: true,
            // replace the html element with the template
            templateUrl: 'views/components/umb-confirm.html',
            scope: {
                onConfirm: '=',
                onCancel: '=',
                caption: '@'
            },
            link: function (scope, element, attr, ctrl) {
                scope.showCancel = false;
                scope.showConfirm = false;
                if (scope.onConfirm) {
                    scope.showConfirm = true;
                }
                if (scope.onCancel) {
                    scope.showCancel = true;
                }
            }
        };
    }
    angular.module('umbraco.directives').directive('umbConfirm', confirmDirective);
    /**
@ngdoc directive
@name umbraco.directives.directive:umbConfirmAction
@restrict E
@scope

@description
<p>Use this directive to toggle a confirmation prompt for an action.
The prompt consists of a checkmark and a cross to confirm or cancel the action.
The prompt can be opened in four direction up, down, left or right.</p>

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <div class="my-action" style="position:relative;">
            <i class="icon-trash" ng-click="vm.showPrompt()"></i>
            <umb-confirm-action
                ng-if="vm.promptIsVisible"
                direction="left"
                on-confirm="vm.confirmAction()"
                on-cancel="vm.hidePrompt()">
            </umb-confirm-action>
        </div>

    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {

        "use strict";

        function Controller() {

            var vm = this;
            vm.promptIsVisible = false;

            vm.confirmAction = confirmAction;
            vm.showPrompt = showPrompt;
            vm.hidePrompt = hidePrompt;

            function confirmAction() {
                // confirm logic here
            }

            function showPrompt() {
                vm.promptIsVisible = true;
            }

            function hidePrompt() {
                vm.promptIsVisible = false;
            }

        }

        angular.module("umbraco").controller("My.Controller", Controller);
    })();
</pre>

@param {string} direction The direction the prompt opens ("up", "down", "left", "right").
@param {callback} onConfirm Callback when the checkmark is clicked.
@param {callback} onCancel Callback when the cross is clicked.
**/
    (function () {
        'use strict';
        function ConfirmAction() {
            function link(scope, el, attr, ctrl) {
                scope.clickConfirm = function () {
                    if (scope.onConfirm) {
                        scope.onConfirm();
                    }
                };
                scope.clickCancel = function () {
                    if (scope.onCancel) {
                        scope.onCancel();
                    }
                };
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-confirm-action.html',
                scope: {
                    direction: '@',
                    onConfirm: '&',
                    onCancel: '&'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbConfirmAction', ConfirmAction);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbContentGrid
@restrict E
@scope

@description
Use this directive to generate a list of content items presented as a flexbox grid.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <umb-content-grid
            content="vm.contentItems"
            content-properties="vm.includeProperties"
            on-click="vm.selectItem"
            on-click-name="vm.clickItem">
        </umb-content-grid>

    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {
        "use strict";

        function Controller() {

            var vm = this;
            vm.contentItems = [
                {
                    "name": "Cape",
                    "published": true,
                    "icon": "icon-document",
                    "updateDate": "15-02-2016",
                    "owner": "Mr. Batman",
                    "selected": false
                },
                {
                    "name": "Utility Belt",
                    "published": true,
                    "icon": "icon-document",
                    "updateDate": "15-02-2016",
                    "owner": "Mr. Batman",
                    "selected": false
                },
                {
                    "name": "Cave",
                    "published": true,
                    "icon": "icon-document",
                    "updateDate": "15-02-2016",
                    "owner": "Mr. Batman",
                    "selected": false
                }
            ];
            vm.includeProperties = [
                {
                  "alias": "updateDate",
                  "header": "Last edited"
                },
                {
                  "alias": "owner",
                  "header": "Created by"
                }
            ];

            vm.clickItem = clickItem;
            vm.selectItem = selectItem;


            function clickItem(item, $event, $index){
                // do magic here
            }

            function selectItem(item, $event, $index) {
                // set item.selected = true; to select the item
                // do magic here
            }

        }

        angular.module("umbraco").controller("My.Controller", Controller);
    })();
</pre>

@param {array} content (<code>binding</code>): Array of content items.
@param {array=} contentProperties (<code>binding</code>): Array of content item properties to include in the item. If left empty the item will only show the item icon and name.
@param {callback=} onClick (<code>binding</code>): Callback method to handle click events on the content item.
    <h3>The callback returns:</h3>
    <ul>
        <li><code>item</code>: The clicked item</li>
        <li><code>$event</code>: The select event</li>
        <li><code>$index</code>: The item index</li>
    </ul>
@param {callback=} onClickName (<code>binding</code>): Callback method to handle click events on the checkmark icon.
    <h3>The callback returns:</h3>
    <ul>
        <li><code>item</code>: The selected item</li>
        <li><code>$event</code>: The select event</li>
        <li><code>$index</code>: The item index</li>
    </ul>
**/
    (function () {
        'use strict';
        function ContentGridDirective() {
            function link(scope, el, attr, ctrl) {
                scope.clickItem = function (item, $event, $index) {
                    if (scope.onClick) {
                        scope.onClick(item, $event, $index);
                    }
                };
                scope.clickItemName = function (item, $event, $index) {
                    if (scope.onClickName) {
                        scope.onClickName(item, $event, $index);
                    }
                };
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-content-grid.html',
                scope: {
                    content: '=',
                    contentProperties: '=',
                    onClick: '=',
                    onClickName: '='
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbContentGrid', ContentGridDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbDateTimePicker
@restrict E
@scope

@description
<b>Added in Umbraco version 7.6</b>
This directive is a wrapper of the bootstrap datetime picker version 3.1.3. Use it to render a date time picker.
For extra details about options and events take a look here: https://eonasdan.github.io/bootstrap-datetimepicker/

Use this directive to render a date time picker

<h3>Markup example</h3>
<pre>
	<div ng-controller="My.Controller as vm">

        <umb-date-time-picker
            options="vm.config"
            on-change="vm.datePickerChange(event)"
            on-error="vm.datePickerError(event)">
        </umb-date-time-picker>

	</div>
</pre>

<h3>Controller example</h3>
<pre>
	(function () {
		"use strict";

		function Controller() {

            var vm = this;

            vm.date = "";

            vm.config = {
                pickDate: true,
                pickTime: true,
                useSeconds: true,
                format: "YYYY-MM-DD HH:mm:ss",
                icons: {
                    time: "icon-time",
                    date: "icon-calendar",
                    up: "icon-chevron-up",
                    down: "icon-chevron-down"
                }
            };

            vm.datePickerChange = datePickerChange;
            vm.datePickerError = datePickerError;

            function datePickerChange(event) {
                // handle change
                if(event.date && event.date.isValid()) {
                    var date = event.date.format(vm.datePickerConfig.format);
                }
            }

            function datePickerError(event) {
                // handle error
            }

        }

		angular.module("umbraco").controller("My.Controller", Controller);

	})();
</pre>

@param {object} options (<code>binding</code>): Config object for the date picker.
@param {callback} onHide (<code>callback</code>): Hide callback.
@param {callback} onShow (<code>callback</code>): Show callback.
@param {callback} onChange (<code>callback</code>): Change callback.
@param {callback} onError (<code>callback</code>): Error callback.
@param {callback} onUpdate (<code>callback</code>): Update callback.
**/
    (function () {
        'use strict';
        function DateTimePickerDirective(assetsService) {
            function link(scope, element, attrs, ctrl) {
                scope.hasTranscludedContent = false;
                function onInit() {
                    // check for transcluded content so we can hide the defualt markup
                    scope.hasTranscludedContent = element.find('.js-datePicker__transcluded-content')[0].children.length > 0;
                    // load css file for the date picker
                    assetsService.loadCss('lib/datetimepicker/bootstrap-datetimepicker.min.css', scope);
                    // load the js file for the date picker
                    assetsService.loadJs('lib/datetimepicker/bootstrap-datetimepicker.js', scope).then(function () {
                        // init date picker
                        initDatePicker();
                    });
                }
                function onHide(event) {
                    if (scope.onHide) {
                        scope.$apply(function () {
                            // callback
                            scope.onHide({ event: event });
                        });
                    }
                }
                function onShow() {
                    if (scope.onShow) {
                        scope.$apply(function () {
                            // callback
                            scope.onShow();
                        });
                    }
                }
                function onChange(event) {
                    if (scope.onChange && event.date && event.date.isValid()) {
                        scope.$apply(function () {
                            // callback
                            scope.onChange({ event: event });
                        });
                    }
                }
                function onError(event) {
                    if (scope.onError) {
                        scope.$apply(function () {
                            // callback
                            scope.onError({ event: event });
                        });
                    }
                }
                function onUpdate(event) {
                    if (scope.onUpdate) {
                        scope.$apply(function () {
                            // callback
                            scope.onUpdate({ event: event });
                        });
                    }
                }
                function initDatePicker() {
                    // Open the datepicker and add a changeDate eventlistener
                    element.datetimepicker(scope.options).on('dp.hide', onHide).on('dp.show', onShow).on('dp.change', onChange).on('dp.error', onError).on('dp.update', onUpdate);
                }
                onInit();
            }
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/umb-date-time-picker.html',
                scope: {
                    options: '=',
                    onHide: '&',
                    onShow: '&',
                    onChange: '&',
                    onError: '&',
                    onUpdate: '&'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbDateTimePicker', DateTimePickerDirective);
    }());
    (function () {
        'use strict';
        function UmbDisableFormValidation() {
            var directive = {
                restrict: 'A',
                require: '?form',
                link: function (scope, elm, attrs, ctrl) {
                    //override the $setValidity function of the form to disable validation
                    ctrl.$setValidity = function () {
                    };
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbDisableFormValidation', UmbDisableFormValidation);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbDropdown
@restrict E
@scope

@description
<b>Added in versions 7.7.0</b>: Use this component to render a dropdown menu.

<h3>Markup example</h3>
<pre>
    <div ng-controller="MyDropdown.Controller as vm">

        <div style="position: relative;">

            <umb-button
                type="button"
                label="Toggle dropdown"
                action="vm.toggle()">
            </umb-button>

            <umb-dropdown ng-if="vm.dropdownOpen" on-close="vm.close()" umb-keyboard-list>
                <umb-dropdown-item
                    ng-repeat="item in vm.items">
                    <a href="" ng-click="vm.select(item)">{{ item.name }}</a>
                </umb-dropdown-item>
            </umb-dropdown>

        </div>

    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {
        "use strict";

        function Controller() {

            var vm = this;

            vm.dropdownOpen = false;
            vm.items = [
                { "name": "Item 1" },
                { "name": "Item 2" },
                { "name": "Item 3" }
            ];

            vm.toggle = toggle;
            vm.close = close;
            vm.select = select;

            function toggle() {
                vm.dropdownOpen = true;
            }

            function close() {
                vm.dropdownOpen = false;
            }

            function select(item) {
                // Do your magic here
            }

        }

        angular.module("umbraco").controller("MyDropdown.Controller", Controller);
    })();
</pre>

<h3>Use in combination with</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbDropdownItem umbDropdownItem}</li>
    <li>{@link umbraco.directives.directive:umbKeyboardList umbKeyboardList}</li>
</ul>

@param {callback} onClose Callback when the dropdown menu closes. When you click outside or press esc.

**/
    (function () {
        'use strict';
        function umbDropdown($document) {
            function link(scope, element, attr, ctrl) {
                scope.close = function () {
                    if (scope.onClose) {
                        scope.onClose();
                    }
                };
                // Handle keydown events
                function keydown(event) {
                    // press escape
                    if (event.keyCode === 27) {
                        scope.onClose();
                    }
                }
                // Stop to listen typing.
                function stopListening() {
                    $document.off('keydown', keydown);
                }
                // Start listening to key typing.
                $document.on('keydown', keydown);
                // Stop listening when scope is destroyed.
                scope.$on('$destroy', stopListening);
            }
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/umb-dropdown.html',
                scope: { onClose: '&' },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbDropdown', umbDropdown);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbDropdownItem
@restrict E

@description
<b>Added in versions 7.7.0</b>: Use this directive to construct a dropdown item. See documentation for {@link umbraco.directives.directive:umbDropdown umbDropdown}.

**/
    (function () {
        'use strict';
        function umbDropdownItem() {
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/umb-dropdown-item.html'
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbDropdownItem', umbDropdownItem);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbEmptyState
@restrict E
@scope

@description
Use this directive to show an empty state message.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <umb-empty-state
            ng-if="!vm.items"
            position="center">
            // Empty state content
        </umb-empty-state>

    </div>
</pre>

@param {string=} size Set the size of the text ("small", "large").
@param {string=} position Set the position of the text ("center").
**/
    (function () {
        'use strict';
        function EmptyStateDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                transclude: true,
                templateUrl: 'views/components/umb-empty-state.html',
                scope: {
                    size: '@',
                    position: '@'
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbEmptyState', EmptyStateDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbFolderGrid
@restrict E
@scope

@description
Use this directive to generate a list of folders presented as a flexbox grid.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">
        <umb-folder-grid
            ng-if="vm.folders.length > 0"
            folders="vm.folders"
            on-click="vm.clickFolder"
            on-select="vm.selectFolder">
        </umb-folder-grid>
    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {
        "use strict";

        function Controller(myService) {

            var vm = this;
            vm.folders = [
                {
                    "name": "Folder 1",
                    "icon": "icon-folder",
                    "selected": false
                },
                {
                    "name": "Folder 2",
                    "icon": "icon-folder",
                    "selected": false
                }

            ];

            vm.clickFolder = clickFolder;
            vm.selectFolder = selectFolder;

            myService.getFolders().then(function(folders){
                vm.folders = folders;
            });

            function clickFolder(folder){
                // Execute when clicking folder name/link
            }

            function selectFolder(folder, event, index) {
                // Execute when clicking folder
                // set folder.selected = true; to show checkmark icon
            }

        }

        angular.module("umbraco").controller("My.Controller", Controller);
    })();
</pre>

@param {array} folders (<code>binding</code>): Array of folders
@param {callback=} onClick (<code>binding</code>): Callback method to handle click events on the folder.
    <h3>The callback returns:</h3>
    <ul>
        <li><code>folder</code>: The selected folder</li>
    </ul>
@param {callback=} onSelect (<code>binding</code>): Callback method to handle click events on the checkmark icon.
    <h3>The callback returns:</h3>
    <ul>
        <li><code>folder</code>: The selected folder</li>
        <li><code>$event</code>: The select event</li>
        <li><code>$index</code>: The folder index</li>
    </ul>
**/
    (function () {
        'use strict';
        function FolderGridDirective() {
            function link(scope, el, attr, ctrl) {
                scope.clickFolder = function (folder, $event, $index) {
                    if (scope.onClick) {
                        scope.onClick(folder, $event, $index);
                        $event.stopPropagation();
                    }
                };
                scope.clickFolderName = function (folder, $event, $index) {
                    if (scope.onClickName) {
                        scope.onClickName(folder, $event, $index);
                        $event.stopPropagation();
                    }
                };
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-folder-grid.html',
                scope: {
                    folders: '=',
                    onClick: '=',
                    onClickName: '='
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbFolderGrid', FolderGridDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbGenerateAlias
@restrict E
@scope

@description
Use this directive to generate a camelCased umbraco alias.
When the aliasFrom value is changed the directive will get a formatted alias from the server and update the alias model. If "enableLock" is set to <code>true</code>
the directive will use {@link umbraco.directives.directive:umbLockedField umbLockedField} to lock and unlock the alias.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <input type="text" ng-model="vm.name" />

        <umb-generate-alias
            enable-lock="true"
            alias-from="vm.name"
            alias="vm.alias">
        </umb-generate-alias>

    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {
        "use strict";

        function Controller() {

            var vm = this;

            vm.name = "";
            vm.alias = "";

        }

        angular.module("umbraco").controller("My.Controller", Controller);
    })();
</pre>

@param {string} alias (<code>binding</code>): The model where the alias is bound.
@param {string} aliasFrom (<code>binding</code>): The model to generate the alias from.
@param {boolean=} enableLock (<code>binding</code>): Set to <code>true</code> to add a lock next to the alias from where it can be unlocked and changed.
**/
    angular.module('umbraco.directives').directive('umbGenerateAlias', function ($timeout, entityResource, localizationService) {
        return {
            restrict: 'E',
            templateUrl: 'views/components/umb-generate-alias.html',
            replace: true,
            scope: {
                alias: '=',
                aliasFrom: '=',
                enableLock: '=?',
                serverValidationField: '@'
            },
            link: function (scope, element, attrs, ctrl) {
                var eventBindings = [];
                var bindWatcher = true;
                var generateAliasTimeout = '';
                var updateAlias = false;
                scope.locked = true;
                scope.labels = {
                    idle: 'Enter alias...',
                    busy: 'Generating alias...'
                };
                scope.placeholderText = scope.labels.idle;
                localizationService.localize('placeholders_enterAlias').then(function (value) {
                    scope.labels.idle = scope.placeholderText = value;
                });
                localizationService.localize('placeholders_generatingAlias').then(function (value) {
                    scope.labels.busy = value;
                });
                function generateAlias(value) {
                    if (generateAliasTimeout) {
                        $timeout.cancel(generateAliasTimeout);
                    }
                    if (value !== undefined && value !== '' && value !== null) {
                        scope.alias = '';
                        scope.placeholderText = scope.labels.busy;
                        generateAliasTimeout = $timeout(function () {
                            updateAlias = true;
                            entityResource.getSafeAlias(encodeURIComponent(value), true).then(function (safeAlias) {
                                if (updateAlias) {
                                    scope.alias = safeAlias.alias;
                                }
                                scope.placeholderText = scope.labels.idle;
                            });
                        }, 500);
                    } else {
                        updateAlias = true;
                        scope.alias = '';
                        scope.placeholderText = scope.labels.idle;
                    }
                }
                // if alias gets unlocked - stop watching alias
                eventBindings.push(scope.$watch('locked', function (newValue, oldValue) {
                    if (newValue === false) {
                        bindWatcher = false;
                    }
                }));
                // validate custom entered alias
                eventBindings.push(scope.$watch('alias', function (newValue, oldValue) {
                    if (scope.alias === '' || scope.alias === null || scope.alias === undefined) {
                        if (bindWatcher === true) {
                            // add watcher
                            eventBindings.push(scope.$watch('aliasFrom', function (newValue, oldValue) {
                                if (bindWatcher) {
                                    generateAlias(newValue);
                                }
                            }));
                        }
                    }
                }));
                // clean up
                scope.$on('$destroy', function () {
                    // unbind watchers
                    for (var e in eventBindings) {
                        eventBindings[e]();
                    }
                });
            }
        };
    });
    (function () {
        'use strict';
        function GridSelector($location) {
            function link(scope, el, attr, ctrl) {
                var eventBindings = [];
                scope.dialogModel = {};
                scope.showDialog = false;
                scope.itemLabel = '';
                // set default item name
                if (!scope.itemName) {
                    scope.itemLabel = 'item';
                } else {
                    scope.itemLabel = scope.itemName;
                }
                scope.removeItem = function (selectedItem) {
                    var selectedItemIndex = scope.selectedItems.indexOf(selectedItem);
                    scope.selectedItems.splice(selectedItemIndex, 1);
                };
                scope.removeDefaultItem = function () {
                    // it will be the last item so we can clear the array
                    scope.selectedItems = [];
                    // remove as default item
                    scope.defaultItem = null;
                };
                scope.openItemPicker = function ($event) {
                    scope.dialogModel = {
                        view: 'itempicker',
                        title: 'Choose ' + scope.itemLabel,
                        availableItems: scope.availableItems,
                        selectedItems: scope.selectedItems,
                        event: $event,
                        show: true,
                        submit: function (model) {
                            scope.selectedItems.push(model.selectedItem);
                            // if no default item - set item as default
                            if (scope.defaultItem === null) {
                                scope.setAsDefaultItem(model.selectedItem);
                            }
                            scope.dialogModel.show = false;
                            scope.dialogModel = null;
                        }
                    };
                };
                scope.openTemplate = function (selectedItem) {
                    var url = '/settings/templates/edit/' + selectedItem.id;
                    $location.url(url);
                };
                scope.setAsDefaultItem = function (selectedItem) {
                    // clear default item
                    scope.defaultItem = {};
                    // set as default item
                    scope.defaultItem = selectedItem;
                };
                function updatePlaceholders() {
                    // update default item
                    if (scope.defaultItem !== null && scope.defaultItem.placeholder) {
                        scope.defaultItem.name = scope.name;
                        if (scope.alias !== null && scope.alias !== undefined) {
                            scope.defaultItem.alias = scope.alias;
                        }
                    }
                    // update selected items
                    angular.forEach(scope.selectedItems, function (selectedItem) {
                        if (selectedItem.placeholder) {
                            selectedItem.name = scope.name;
                            if (scope.alias !== null && scope.alias !== undefined) {
                                selectedItem.alias = scope.alias;
                            }
                        }
                    });
                    // update availableItems
                    angular.forEach(scope.availableItems, function (availableItem) {
                        if (availableItem.placeholder) {
                            availableItem.name = scope.name;
                            if (scope.alias !== null && scope.alias !== undefined) {
                                availableItem.alias = scope.alias;
                            }
                        }
                    });
                }
                function activate() {
                    // add watchers for updating placeholde name and alias
                    if (scope.updatePlaceholder) {
                        eventBindings.push(scope.$watch('name', function (newValue, oldValue) {
                            updatePlaceholders();
                        }));
                        eventBindings.push(scope.$watch('alias', function (newValue, oldValue) {
                            updatePlaceholders();
                        }));
                    }
                }
                activate();
                // clean up
                scope.$on('$destroy', function () {
                    // clear watchers
                    for (var e in eventBindings) {
                        eventBindings[e]();
                    }
                });
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-grid-selector.html',
                scope: {
                    name: '=',
                    alias: '=',
                    selectedItems: '=',
                    availableItems: '=',
                    defaultItem: '=',
                    itemName: '@',
                    updatePlaceholder: '='
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbGridSelector', GridSelector);
    }());
    (function () {
        'use strict';
        function GroupsBuilderDirective(contentTypeHelper, contentTypeResource, mediaTypeResource, dataTypeHelper, dataTypeResource, $filter, iconHelper, $q, $timeout, notificationsService, localizationService) {
            function link(scope, el, attr, ctrl) {
                var validationTranslated = '';
                var tabNoSortOrderTranslated = '';
                scope.sortingMode = false;
                scope.toolbar = [];
                scope.sortableOptionsGroup = {};
                scope.sortableOptionsProperty = {};
                scope.sortingButtonKey = 'general_reorder';
                function activate() {
                    setSortingOptions();
                    // set placeholder property on each group
                    if (scope.model.groups.length !== 0) {
                        angular.forEach(scope.model.groups, function (group) {
                            addInitProperty(group);
                        });
                    }
                    // add init tab
                    addInitGroup(scope.model.groups);
                    activateFirstGroup(scope.model.groups);
                    // localize texts
                    localizationService.localize('validation_validation').then(function (value) {
                        validationTranslated = value;
                    });
                    localizationService.localize('contentTypeEditor_tabHasNoSortOrder').then(function (value) {
                        tabNoSortOrderTranslated = value;
                    });
                }
                function setSortingOptions() {
                    scope.sortableOptionsGroup = {
                        distance: 10,
                        tolerance: 'pointer',
                        opacity: 0.7,
                        scroll: true,
                        cursor: 'move',
                        placeholder: 'umb-group-builder__group-sortable-placeholder',
                        zIndex: 6000,
                        handle: '.umb-group-builder__group-handle',
                        items: '.umb-group-builder__group-sortable',
                        start: function (e, ui) {
                            ui.placeholder.height(ui.item.height());
                        },
                        stop: function (e, ui) {
                            updateTabsSortOrder();
                        }
                    };
                    scope.sortableOptionsProperty = {
                        distance: 10,
                        tolerance: 'pointer',
                        connectWith: '.umb-group-builder__properties',
                        opacity: 0.7,
                        scroll: true,
                        cursor: 'move',
                        placeholder: 'umb-group-builder__property_sortable-placeholder',
                        zIndex: 6000,
                        handle: '.umb-group-builder__property-handle',
                        items: '.umb-group-builder__property-sortable',
                        start: function (e, ui) {
                            ui.placeholder.height(ui.item.height());
                        },
                        stop: function (e, ui) {
                            updatePropertiesSortOrder();
                        }
                    };
                }
                function updateTabsSortOrder() {
                    var first = true;
                    var prevSortOrder = 0;
                    scope.model.groups.map(function (group) {
                        var index = scope.model.groups.indexOf(group);
                        if (group.tabState !== 'init') {
                            // set the first not inherited tab to sort order 0
                            if (!group.inherited && first) {
                                // set the first tab sort order to 0 if prev is 0
                                if (prevSortOrder === 0) {
                                    group.sortOrder = 0;    // when the first tab is inherited and sort order is not 0
                                } else {
                                    group.sortOrder = prevSortOrder + 1;
                                }
                                first = false;
                            } else if (!group.inherited && !first) {
                                // find next group
                                var nextGroup = scope.model.groups[index + 1];
                                // if a groups is dropped in the middle of to groups with
                                // same sort order. Give it the dropped group same sort order
                                if (prevSortOrder === nextGroup.sortOrder) {
                                    group.sortOrder = prevSortOrder;
                                } else {
                                    group.sortOrder = prevSortOrder + 1;
                                }
                            }
                            // store this tabs sort order as reference for the next
                            prevSortOrder = group.sortOrder;
                        }
                    });
                }
                function filterAvailableCompositions(selectedContentType, selecting) {
                    //selecting = true if the user has check the item, false if the user has unchecked the item
                    var selectedContentTypeAliases = selecting ? //the user has selected the item so add to the current list
                    _.union(scope.compositionsDialogModel.compositeContentTypes, [selectedContentType.alias]) : //the user has unselected the item so remove from the current list
                    _.reject(scope.compositionsDialogModel.compositeContentTypes, function (i) {
                        return i === selectedContentType.alias;
                    });
                    //get the currently assigned property type aliases - ensure we pass these to the server side filer
                    var propAliasesExisting = _.filter(_.flatten(_.map(scope.model.groups, function (g) {
                        return _.map(g.properties, function (p) {
                            return p.alias;
                        });
                    })), function (f) {
                        return f !== null && f !== undefined;
                    });
                    //use a different resource lookup depending on the content type type
                    var resourceLookup = scope.contentType === 'documentType' ? contentTypeResource.getAvailableCompositeContentTypes : mediaTypeResource.getAvailableCompositeContentTypes;
                    return resourceLookup(scope.model.id, selectedContentTypeAliases, propAliasesExisting).then(function (filteredAvailableCompositeTypes) {
                        _.each(scope.compositionsDialogModel.availableCompositeContentTypes, function (current) {
                            //reset first
                            current.allowed = true;
                            //see if this list item is found in the response (allowed) list
                            var found = _.find(filteredAvailableCompositeTypes, function (f) {
                                return current.contentType.alias === f.contentType.alias;
                            });
                            //allow if the item was  found in the response (allowed) list -
                            // and ensure its set to allowed if it is currently checked,
                            // DO not allow if it's a locked content type.
                            current.allowed = scope.model.lockedCompositeContentTypes.indexOf(current.contentType.alias) === -1 && selectedContentTypeAliases.indexOf(current.contentType.alias) !== -1 || (found !== null && found !== undefined ? found.allowed : false);
                        });
                    });
                }
                function updatePropertiesSortOrder() {
                    angular.forEach(scope.model.groups, function (group) {
                        if (group.tabState !== 'init') {
                            group.properties = contentTypeHelper.updatePropertiesSortOrder(group.properties);
                        }
                    });
                }
                function setupAvailableContentTypesModel(result) {
                    scope.compositionsDialogModel.availableCompositeContentTypes = result;
                    //iterate each one and set it up
                    _.each(scope.compositionsDialogModel.availableCompositeContentTypes, function (c) {
                        //enable it if it's part of the selected model
                        if (scope.compositionsDialogModel.compositeContentTypes.indexOf(c.contentType.alias) !== -1) {
                            c.allowed = true;
                        }
                        //set the inherited flags
                        c.inherited = false;
                        if (scope.model.lockedCompositeContentTypes.indexOf(c.contentType.alias) > -1) {
                            c.inherited = true;
                        }
                        // convert icons for composite content types
                        iconHelper.formatContentTypeIcons([c.contentType]);
                    });
                }
                /* ---------- DELETE PROMT ---------- */
                scope.togglePrompt = function (object) {
                    object.deletePrompt = !object.deletePrompt;
                };
                scope.hidePrompt = function (object) {
                    object.deletePrompt = false;
                };
                /* ---------- TOOLBAR ---------- */
                scope.toggleSortingMode = function (tool) {
                    if (scope.sortingMode === true) {
                        var sortOrderMissing = false;
                        for (var i = 0; i < scope.model.groups.length; i++) {
                            var group = scope.model.groups[i];
                            if (group.tabState !== 'init' && group.sortOrder === undefined) {
                                sortOrderMissing = true;
                                group.showSortOrderMissing = true;
                                notificationsService.error(validationTranslated + ': ' + group.name + ' ' + tabNoSortOrderTranslated);
                            }
                        }
                        if (!sortOrderMissing) {
                            scope.sortingMode = false;
                            scope.sortingButtonKey = 'general_reorder';
                        }
                    } else {
                        scope.sortingMode = true;
                        scope.sortingButtonKey = 'general_reorderDone';
                    }
                };
                scope.openCompositionsDialog = function () {
                    scope.compositionsDialogModel = {
                        title: 'Compositions',
                        contentType: scope.model,
                        compositeContentTypes: scope.model.compositeContentTypes,
                        view: 'views/common/overlays/contenttypeeditor/compositions/compositions.html',
                        confirmSubmit: {
                            title: 'Warning',
                            description: 'Removing a composition will delete all the associated property data. Once you save the document type there\'s no way back, are you sure?',
                            checkboxLabel: 'I know what I\'m doing',
                            enable: true
                        },
                        submit: function (model, oldModel, confirmed) {
                            var compositionRemoved = false;
                            // check if any compositions has been removed
                            for (var i = 0; oldModel.compositeContentTypes.length > i; i++) {
                                var oldComposition = oldModel.compositeContentTypes[i];
                                if (_.contains(model.compositeContentTypes, oldComposition) === false) {
                                    compositionRemoved = true;
                                }
                            }
                            // show overlay confirm box if compositions has been removed.
                            if (compositionRemoved && confirmed === false) {
                                scope.compositionsDialogModel.confirmSubmit.show = true;    // submit overlay if no compositions has been removed
                                                                                            // or the action has been confirmed
                            } else {
                                // make sure that all tabs has an init property
                                if (scope.model.groups.length !== 0) {
                                    angular.forEach(scope.model.groups, function (group) {
                                        addInitProperty(group);
                                    });
                                }
                                // remove overlay
                                scope.compositionsDialogModel.show = false;
                                scope.compositionsDialogModel = null;
                            }
                        },
                        close: function (oldModel) {
                            // reset composition changes
                            scope.model.groups = oldModel.contentType.groups;
                            scope.model.compositeContentTypes = oldModel.contentType.compositeContentTypes;
                            // remove overlay
                            scope.compositionsDialogModel.show = false;
                            scope.compositionsDialogModel = null;
                        },
                        selectCompositeContentType: function (selectedContentType) {
                            //first check if this is a new selection - we need to store this value here before any further digests/async
                            // because after that the scope.model.compositeContentTypes will be populated with the selected value.
                            var newSelection = scope.model.compositeContentTypes.indexOf(selectedContentType.alias) === -1;
                            if (newSelection) {
                                //merge composition with content type
                                //use a different resource lookup depending on the content type type
                                var resourceLookup = scope.contentType === 'documentType' ? contentTypeResource.getById : mediaTypeResource.getById;
                                resourceLookup(selectedContentType.id).then(function (composition) {
                                    //based on the above filtering we shouldn't be able to select an invalid one, but let's be safe and
                                    // double check here.
                                    var overlappingAliases = contentTypeHelper.validateAddingComposition(scope.model, composition);
                                    if (overlappingAliases.length > 0) {
                                        //this will create an invalid composition, need to uncheck it
                                        scope.compositionsDialogModel.compositeContentTypes.splice(scope.compositionsDialogModel.compositeContentTypes.indexOf(composition.alias), 1);
                                        //dissallow this until something else is unchecked
                                        selectedContentType.allowed = false;
                                    } else {
                                        contentTypeHelper.mergeCompositeContentType(scope.model, composition);
                                    }
                                    //based on the selection, we need to filter the available composite types list
                                    filterAvailableCompositions(selectedContentType, newSelection).then(function () {
                                    });
                                });
                            } else {
                                // split composition from content type
                                contentTypeHelper.splitCompositeContentType(scope.model, selectedContentType);
                                //based on the selection, we need to filter the available composite types list
                                filterAvailableCompositions(selectedContentType, newSelection).then(function () {
                                });
                            }
                        }
                    };
                    //select which resource methods to use, eg document Type or Media Type versions
                    var availableContentTypeResource = scope.contentType === 'documentType' ? contentTypeResource.getAvailableCompositeContentTypes : mediaTypeResource.getAvailableCompositeContentTypes;
                    var whereUsedContentTypeResource = scope.contentType === 'documentType' ? contentTypeResource.getWhereCompositionIsUsedInContentTypes : mediaTypeResource.getWhereCompositionIsUsedInContentTypes;
                    var countContentTypeResource = scope.contentType === 'documentType' ? contentTypeResource.getCount : mediaTypeResource.getCount;
                    //get the currently assigned property type aliases - ensure we pass these to the server side filer
                    var propAliasesExisting = _.filter(_.flatten(_.map(scope.model.groups, function (g) {
                        return _.map(g.properties, function (p) {
                            return p.alias;
                        });
                    })), function (f) {
                        return f !== null && f !== undefined;
                    });
                    $q.all([
                        //get available composite types
                        availableContentTypeResource(scope.model.id, [], propAliasesExisting).then(function (result) {
                            setupAvailableContentTypesModel(result);
                        }),
                        //get where used document types
                        whereUsedContentTypeResource(scope.model.id).then(function (whereUsed) {
                            //pass to the dialog model the content type eg documentType or mediaType 
                            scope.compositionsDialogModel.section = scope.contentType;
                            //pass the list of 'where used' document types
                            scope.compositionsDialogModel.whereCompositionUsed = whereUsed;
                        }),
                        //get content type count
                        countContentTypeResource().then(function (result) {
                            scope.compositionsDialogModel.totalContentTypes = parseInt(result, 10);
                        })
                    ]).then(function () {
                        //resolves when both other promises are done, now show it
                        scope.compositionsDialogModel.show = true;
                    });
                };
                /* ---------- GROUPS ---------- */
                scope.addGroup = function (group) {
                    // set group sort order
                    var index = scope.model.groups.indexOf(group);
                    var prevGroup = scope.model.groups[index - 1];
                    if (index > 0) {
                        // set index to 1 higher than the previous groups sort order
                        group.sortOrder = prevGroup.sortOrder + 1;
                    } else {
                        // first group - sort order will be 0
                        group.sortOrder = 0;
                    }
                    // activate group
                    scope.activateGroup(group);
                };
                scope.activateGroup = function (selectedGroup) {
                    // set all other groups that are inactive to active
                    angular.forEach(scope.model.groups, function (group) {
                        // skip init tab
                        if (group.tabState !== 'init') {
                            group.tabState = 'inActive';
                        }
                    });
                    selectedGroup.tabState = 'active';
                };
                scope.removeGroup = function (groupIndex) {
                    scope.model.groups.splice(groupIndex, 1);
                    addInitGroup(scope.model.groups);
                };
                scope.updateGroupTitle = function (group) {
                    if (group.properties.length === 0) {
                        addInitProperty(group);
                    }
                };
                scope.changeSortOrderValue = function (group) {
                    if (group.sortOrder !== undefined) {
                        group.showSortOrderMissing = false;
                    }
                    scope.model.groups = $filter('orderBy')(scope.model.groups, 'sortOrder');
                };
                function addInitGroup(groups) {
                    // check i init tab already exists
                    var addGroup = true;
                    angular.forEach(groups, function (group) {
                        if (group.tabState === 'init') {
                            addGroup = false;
                        }
                    });
                    if (addGroup) {
                        groups.push({
                            properties: [],
                            parentTabContentTypes: [],
                            parentTabContentTypeNames: [],
                            name: '',
                            tabState: 'init'
                        });
                    }
                    return groups;
                }
                function activateFirstGroup(groups) {
                    if (groups && groups.length > 0) {
                        var firstGroup = groups[0];
                        if (!firstGroup.tabState || firstGroup.tabState === 'inActive') {
                            firstGroup.tabState = 'active';
                        }
                    }
                }
                /* ---------- PROPERTIES ---------- */
                scope.addProperty = function (property, group) {
                    // set property sort order
                    var index = group.properties.indexOf(property);
                    var prevProperty = group.properties[index - 1];
                    if (index > 0) {
                        // set index to 1 higher than the previous property sort order
                        property.sortOrder = prevProperty.sortOrder + 1;
                    } else {
                        // first property - sort order will be 0
                        property.sortOrder = 0;
                    }
                    // open property settings dialog
                    scope.editPropertyTypeSettings(property, group);
                };
                scope.editPropertyTypeSettings = function (property, group) {
                    if (!property.inherited) {
                        scope.propertySettingsDialogModel = {};
                        scope.propertySettingsDialogModel.title = 'Property settings';
                        scope.propertySettingsDialogModel.property = property;
                        scope.propertySettingsDialogModel.contentType = scope.contentType;
                        scope.propertySettingsDialogModel.contentTypeName = scope.model.name;
                        scope.propertySettingsDialogModel.view = 'views/common/overlays/contenttypeeditor/propertysettings/propertysettings.html';
                        scope.propertySettingsDialogModel.show = true;
                        // set state to active to access the preview
                        property.propertyState = 'active';
                        // set property states
                        property.dialogIsOpen = true;
                        scope.propertySettingsDialogModel.submit = function (model) {
                            property.inherited = false;
                            property.dialogIsOpen = false;
                            // update existing data types
                            if (model.updateSameDataTypes) {
                                updateSameDataTypes(property);
                            }
                            // remove dialog
                            scope.propertySettingsDialogModel.show = false;
                            scope.propertySettingsDialogModel = null;
                            // push new init property to group
                            addInitProperty(group);
                            // set focus on init property
                            var numberOfProperties = group.properties.length;
                            group.properties[numberOfProperties - 1].focus = true;
                            // push new init tab to the scope
                            addInitGroup(scope.model.groups);
                        };
                        scope.propertySettingsDialogModel.close = function (oldModel) {
                            // reset all property changes
                            property.label = oldModel.property.label;
                            property.alias = oldModel.property.alias;
                            property.description = oldModel.property.description;
                            property.config = oldModel.property.config;
                            property.editor = oldModel.property.editor;
                            property.view = oldModel.property.view;
                            property.dataTypeId = oldModel.property.dataTypeId;
                            property.dataTypeIcon = oldModel.property.dataTypeIcon;
                            property.dataTypeName = oldModel.property.dataTypeName;
                            property.validation.mandatory = oldModel.property.validation.mandatory;
                            property.validation.pattern = oldModel.property.validation.pattern;
                            property.showOnMemberProfile = oldModel.property.showOnMemberProfile;
                            property.memberCanEdit = oldModel.property.memberCanEdit;
                            property.isSensitiveValue = oldModel.property.isSensitiveValue;
                            // because we set state to active, to show a preview, we have to check if has been filled out
                            // label is required so if it is not filled we know it is a placeholder
                            if (oldModel.property.editor === undefined || oldModel.property.editor === null || oldModel.property.editor === '') {
                                property.propertyState = 'init';
                            } else {
                                property.propertyState = oldModel.property.propertyState;
                            }
                            // remove dialog
                            scope.propertySettingsDialogModel.show = false;
                            scope.propertySettingsDialogModel = null;
                        };
                    }
                };
                scope.deleteProperty = function (tab, propertyIndex) {
                    // remove property
                    tab.properties.splice(propertyIndex, 1);
                    // if the last property in group is an placeholder - remove add new tab placeholder
                    if (tab.properties.length === 1 && tab.properties[0].propertyState === 'init') {
                        angular.forEach(scope.model.groups, function (group, index, groups) {
                            if (group.tabState === 'init') {
                                groups.splice(index, 1);
                            }
                        });
                    }
                };
                function addInitProperty(group) {
                    var addInitPropertyBool = true;
                    var initProperty = {
                        label: null,
                        alias: null,
                        propertyState: 'init',
                        validation: {
                            mandatory: false,
                            pattern: null
                        }
                    };
                    // check if there already is an init property
                    angular.forEach(group.properties, function (property) {
                        if (property.propertyState === 'init') {
                            addInitPropertyBool = false;
                        }
                    });
                    if (addInitPropertyBool) {
                        group.properties.push(initProperty);
                    }
                    return group;
                }
                function updateSameDataTypes(newProperty) {
                    // find each property
                    angular.forEach(scope.model.groups, function (group) {
                        angular.forEach(group.properties, function (property) {
                            if (property.dataTypeId === newProperty.dataTypeId) {
                                // update property data
                                property.config = newProperty.config;
                                property.editor = newProperty.editor;
                                property.view = newProperty.view;
                                property.dataTypeId = newProperty.dataTypeId;
                                property.dataTypeIcon = newProperty.dataTypeIcon;
                                property.dataTypeName = newProperty.dataTypeName;
                            }
                        });
                    });
                }
                var unbindModelWatcher = scope.$watch('model', function (newValue, oldValue) {
                    if (newValue !== undefined && newValue.groups !== undefined) {
                        activate();
                    }
                });
                // clean up
                scope.$on('$destroy', function () {
                    unbindModelWatcher();
                });
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-groups-builder.html',
                scope: {
                    model: '=',
                    compositions: '=',
                    sorting: '=',
                    contentType: '@'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbGroupsBuilder', GroupsBuilderDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbkeyboardShortcutsOverview
@restrict E
@scope

@description

<p>Use this directive to show an overview of keyboard shortcuts in an editor.
The directive will render an overview trigger wich shows how the overview is opened.
When this combination is hit an overview is opened with shortcuts based on the model sent to the directive.</p>

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <umb-keyboard-shortcuts-overview
            model="vm.keyboardShortcutsOverview">
        </umb-keyboard-shortcuts-overview>

    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {

        "use strict";

        function Controller() {

            var vm = this;

            vm.keyboardShortcutsOverview = [
                {
                    "name": "Sections",
                    "shortcuts": [
                        {
                            "description": "Navigate sections",
                            "keys": [
                                {"key": "1"},
                                {"key": "4"}
                            ],
                            "keyRange": true
                        }
                    ]
                },
                {
                    "name": "Design",
                    "shortcuts": [
                        {
                            "description": "Add tab",
                            "keys": [
                                {"key": "alt"},
                                {"key": "shift"},
                                {"key": "t"}
                            ]
                        }
                    ]
                }
            ];

        }

        angular.module("umbraco").controller("My.Controller", Controller);
    })();
</pre>

<h3>Model description</h3>
<ul>
    <li>
        <strong>name</strong>
        <small>(string)</small> -
        Sets the shortcut section name.
    </li>
    <li>
        <strong>shortcuts</strong>
        <small>(array)</small> -
        Array of available shortcuts in the section.
    </li>
    <ul>
        <li>
            <strong>description</strong>
            <small>(string)</small> -
            Short description of the shortcut.
        </li>
        <li>
            <strong>keys</strong>
            <small>(array)</small> -
            Array of keys in the shortcut.
        </li>
        <ul>
            <li>
                <strong>key</strong>
                <small>(string)</small> -
                The invidual key in the shortcut.
            </li>
        </ul>
        <li>
            <strong>keyRange</strong>
            <small>(boolean)</small> -
            Set to <code>true</code> to show a key range. It combines the shortcut keys with "-" instead of "+".
        </li>
    </ul>
</ul>

@param {object} model keyboard shortcut model. See description and example above.
**/
    (function () {
        'use strict';
        function KeyboardShortcutsOverviewDirective(platformService) {
            function link(scope, el, attr, ctrl) {
                var eventBindings = [];
                var isMac = platformService.isMac();
                scope.toggleShortcutsOverlay = function () {
                    scope.showOverlay = !scope.showOverlay;
                    scope.onToggle();
                };
                function onInit() {
                    angular.forEach(scope.model, function (shortcutGroup) {
                        angular.forEach(shortcutGroup.shortcuts, function (shortcut) {
                            shortcut.platformKeys = [];
                            // get shortcut keys for mac
                            if (isMac && shortcut.keys && shortcut.keys.mac) {
                                shortcut.platformKeys = shortcut.keys.mac;    // get shortcut keys for windows
                            } else if (!isMac && shortcut.keys && shortcut.keys.win) {
                                shortcut.platformKeys = shortcut.keys.win;    // get default shortcut keys
                            } else if (shortcut.keys && shortcut && shortcut.keys.length > 0) {
                                shortcut.platformKeys = shortcut.keys;
                            }
                        });
                    });
                }
                onInit();
                eventBindings.push(scope.$watch('model', function (newValue, oldValue) {
                    if (newValue !== oldValue) {
                        onInit();
                    }
                }));
                // clean up
                scope.$on('$destroy', function () {
                    // unbind watchers
                    for (var e in eventBindings) {
                        eventBindings[e]();
                    }
                });
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-keyboard-shortcuts-overview.html',
                link: link,
                scope: {
                    model: '=',
                    onToggle: '&',
                    showOverlay: '=?'
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbKeyboardShortcutsOverview', KeyboardShortcutsOverviewDirective);
    }());
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbLaunchMiniEditor 
* @restrict E
* @function
* @description 
* Used on a button to launch a mini content editor editor dialog
**/
    angular.module('umbraco.directives').directive('umbLaunchMiniEditor', function (miniEditorHelper) {
        return {
            restrict: 'A',
            replace: false,
            scope: { node: '=umbLaunchMiniEditor' },
            link: function (scope, element, attrs) {
                element.click(function () {
                    miniEditorHelper.launchMiniEditor(scope.node);
                });
            }
        };
    });
    (function () {
        'use strict';
        function LayoutSelectorDirective() {
            function link(scope, el, attr, ctrl) {
                scope.layoutDropDownIsOpen = false;
                scope.showLayoutSelector = true;
                function activate() {
                    setVisibility();
                    setActiveLayout(scope.layouts);
                }
                function setVisibility() {
                    var numberOfAllowedLayouts = getNumberOfAllowedLayouts(scope.layouts);
                    if (numberOfAllowedLayouts === 1) {
                        scope.showLayoutSelector = false;
                    }
                }
                function getNumberOfAllowedLayouts(layouts) {
                    var allowedLayouts = 0;
                    for (var i = 0; layouts.length > i; i++) {
                        var layout = layouts[i];
                        if (layout.selected === true) {
                            allowedLayouts++;
                        }
                    }
                    return allowedLayouts;
                }
                function setActiveLayout(layouts) {
                    for (var i = 0; layouts.length > i; i++) {
                        var layout = layouts[i];
                        if (layout.path === scope.activeLayout.path) {
                            layout.active = true;
                        }
                    }
                }
                scope.pickLayout = function (selectedLayout) {
                    if (scope.onLayoutSelect) {
                        scope.onLayoutSelect(selectedLayout);
                        scope.layoutDropDownIsOpen = false;
                    }
                };
                scope.toggleLayoutDropdown = function () {
                    scope.layoutDropDownIsOpen = !scope.layoutDropDownIsOpen;
                };
                scope.closeLayoutDropdown = function () {
                    scope.layoutDropDownIsOpen = false;
                };
                activate();
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-layout-selector.html',
                scope: {
                    layouts: '=',
                    activeLayout: '=',
                    onLayoutSelect: '='
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbLayoutSelector', LayoutSelectorDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbLightbox
@restrict E
@scope

@description
<p>Use this directive to open a gallery in a lightbox overlay.</p>

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <div class="my-gallery">
            <a href="" ng-repeat="image in images" ng-click="vm.openLightbox($index, images)">
                <img ng-src="image.source" />
            </a>
        </div>

        <umb-lightbox
            ng-if="vm.lightbox.show"
            items="vm.lightbox.items"
            active-item-index="vm.lightbox.activeIndex"
            on-close="vm.closeLightbox">
        </umb-lightbox>

    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {

        "use strict";

        function Controller() {

            var vm = this;

            vm.images = [
                {
                    "source": "linkToImage"
                },
                {
                    "source": "linkToImage"
                }
            ]

            vm.openLightbox = openLightbox;
            vm.closeLightbox = closeLightbox;

            function openLightbox(itemIndex, items) {
                vm.lightbox = {
                    show: true,
                    items: items,
                    activeIndex: itemIndex
                };
            }

            function closeLightbox() {
                vm.lightbox.show = false;
                vm.lightbox = null;
            }

        }

        angular.module("umbraco").controller("My.Controller", Controller);
    })();
</pre>

@param {array} items Array of gallery items.
@param {callback} onClose Callback when the lightbox is closed.
@param {number} activeItemIndex Index of active item.
**/
    (function () {
        'use strict';
        function LightboxDirective() {
            function link(scope, el, attr, ctrl) {
                function activate() {
                    var eventBindings = [];
                    el.appendTo('body');
                    // clean up
                    scope.$on('$destroy', function () {
                        // unbind watchers
                        for (var e in eventBindings) {
                            eventBindings[e]();
                        }
                    });
                }
                scope.next = function () {
                    var nextItemIndex = scope.activeItemIndex + 1;
                    if (nextItemIndex < scope.items.length) {
                        scope.items[scope.activeItemIndex].active = false;
                        scope.items[nextItemIndex].active = true;
                        scope.activeItemIndex = nextItemIndex;
                    }
                };
                scope.prev = function () {
                    var prevItemIndex = scope.activeItemIndex - 1;
                    if (prevItemIndex >= 0) {
                        scope.items[scope.activeItemIndex].active = false;
                        scope.items[prevItemIndex].active = true;
                        scope.activeItemIndex = prevItemIndex;
                    }
                };
                scope.close = function () {
                    if (scope.onClose) {
                        scope.onClose();
                    }
                };
                activate();
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-lightbox.html',
                scope: {
                    items: '=',
                    onClose: '=',
                    activeItemIndex: '='
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbLightbox', LightboxDirective);
    }());
    (function () {
        'use strict';
        function ListViewLayoutDirective() {
            function link(scope, el, attr, ctrl) {
                scope.getContent = function (contentId) {
                    if (scope.onGetContent) {
                        scope.onGetContent(contentId);
                    }
                };
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-list-view-layout.html',
                scope: {
                    contentId: '=',
                    folders: '=',
                    items: '=',
                    selection: '=',
                    options: '=',
                    entityType: '@',
                    onGetContent: '='
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbListViewLayout', ListViewLayoutDirective);
    }());
    (function () {
        'use strict';
        function ListViewSettingsDirective(dataTypeResource, dataTypeHelper, listViewPrevalueHelper) {
            function link(scope) {
                scope.dataType = {};
                scope.editDataTypeSettings = false;
                scope.customListViewCreated = false;
                /* ---------- INIT ---------- */
                function activate() {
                    if (scope.enableListView) {
                        dataTypeResource.getByName(scope.listViewName).then(function (dataType) {
                            scope.dataType = dataType;
                            listViewPrevalueHelper.setPrevalues(dataType.preValues);
                            scope.customListViewCreated = checkForCustomListView();
                        });
                    } else {
                        scope.dataType = {};
                    }
                }
                /* ----------- LIST VIEW SETTINGS --------- */
                scope.toggleEditListViewDataTypeSettings = function () {
                    scope.editDataTypeSettings = !scope.editDataTypeSettings;
                };
                scope.saveListViewDataType = function () {
                    var preValues = dataTypeHelper.createPreValueProps(scope.dataType.preValues);
                    dataTypeResource.save(scope.dataType, preValues, false).then(function (dataType) {
                        // store data type
                        scope.dataType = dataType;
                        // hide settings panel
                        scope.editDataTypeSettings = false;
                    });
                };
                /* ---------- CUSTOM LIST VIEW ---------- */
                scope.createCustomListViewDataType = function () {
                    dataTypeResource.createCustomListView(scope.modelAlias).then(function (dataType) {
                        // store data type
                        scope.dataType = dataType;
                        // set list view name on scope
                        scope.listViewName = dataType.name;
                        // change state to custom list view
                        scope.customListViewCreated = true;
                        // show settings panel
                        scope.editDataTypeSettings = true;
                    });
                };
                scope.removeCustomListDataType = function () {
                    scope.editDataTypeSettings = false;
                    // delete custom list view data type
                    dataTypeResource.deleteById(scope.dataType.id).then(function (dataType) {
                        // set list view name on scope
                        if (scope.contentType === 'documentType') {
                            scope.listViewName = 'List View - Content';
                        } else if (scope.contentType === 'mediaType') {
                            scope.listViewName = 'List View - Media';
                        }
                        // get default data type
                        dataTypeResource.getByName(scope.listViewName).then(function (dataType) {
                            // store data type
                            scope.dataType = dataType;
                            // change state to default list view
                            scope.customListViewCreated = false;
                        });
                    });
                };
                scope.toggle = function () {
                    if (scope.enableListView) {
                        scope.enableListView = false;
                        return;
                    }
                    scope.enableListView = true;
                };
                /* ----------- SCOPE WATCHERS ----------- */
                var unbindEnableListViewWatcher = scope.$watch('enableListView', function (newValue) {
                    if (newValue !== undefined) {
                        activate();
                    }
                });
                // clean up
                scope.$on('$destroy', function () {
                    unbindEnableListViewWatcher();
                });
                /* ----------- METHODS ---------- */
                function checkForCustomListView() {
                    return scope.dataType.name === 'List View - ' + scope.modelAlias;
                }
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-list-view-settings.html',
                scope: {
                    enableListView: '=',
                    listViewName: '=',
                    modelAlias: '=',
                    contentType: '@'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbListViewSettings', ListViewSettingsDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbLoadIndicator
@restrict E

@description
Use this directive to generate a loading indicator.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <umb-load-indicator
            ng-if="vm.loading">
        </umb-load-indicator>

        <div class="content" ng-if="!vm.loading">
            <p>{{content}}</p>
        </div>

    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {
        "use strict";

        function Controller(myService) {

            var vm = this;

            vm.content = "";
            vm.loading = true;

            myService.getContent().then(function(content){
                vm.content = content;
                vm.loading = false;
            });

        }
½
        angular.module("umbraco").controller("My.Controller", Controller);
    })();
</pre>
**/
    (function () {
        'use strict';
        function UmbLoadIndicatorDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-load-indicator.html'
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbLoadIndicator', UmbLoadIndicatorDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbLockedField
@restrict E
@scope

@description
Use this directive to render a value with a lock next to it. When the lock is clicked the value gets unlocked and can be edited.

<h3>Markup example</h3>
<pre>
	<div ng-controller="My.Controller as vm">

		<umb-locked-field
			ng-model="vm.value"
			placeholder-text="'Click to unlock...'">
		</umb-locked-field>

	</div>
</pre>

<h3>Controller example</h3>
<pre>
	(function () {
		"use strict";

		function Controller() {

			var vm = this;
			vm.value = "My locked text";

        }

		angular.module("umbraco").controller("My.Controller", Controller);

	})();
</pre>

@param {string} ngModel (<code>binding</code>): The locked text.
@param {boolean=} locked (<code>binding</code>): <Code>true</code> by default. Set to <code>false</code> to unlock the text.
@param {string=} placeholderText (<code>binding</code>): If ngModel is empty this text will be shown.
@param {string=} regexValidation (<code>binding</code>): Set a regex expression for validation of the field.
@param {string=} serverValidationField (<code>attribute</code>): Set a server validation field.
**/
    (function () {
        'use strict';
        function LockedFieldDirective($timeout, localizationService) {
            function link(scope, el, attr, ngModelCtrl) {
                function activate() {
                    // if locked state is not defined as an attr set default state
                    if (scope.locked === undefined || scope.locked === null) {
                        scope.locked = true;
                    }
                    // if regex validation is not defined as an attr set default state
                    // if this is set to an empty string then regex validation can be ignored.
                    if (scope.regexValidation === undefined || scope.regexValidation === null) {
                        scope.regexValidation = '^[a-zA-Z]\\w.*$';
                    }
                    if (scope.serverValidationField === undefined || scope.serverValidationField === null) {
                        scope.serverValidationField = '';
                    }
                    // if locked state is not defined as an attr set default state
                    if (scope.placeholderText === undefined || scope.placeholderText === null) {
                        scope.placeholderText = 'Enter value...';
                    }
                }
                scope.lock = function () {
                    scope.locked = true;
                };
                scope.unlock = function () {
                    scope.locked = false;
                };
                activate();
            }
            var directive = {
                require: 'ngModel',
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-locked-field.html',
                scope: {
                    ngModel: '=',
                    locked: '=?',
                    placeholderText: '=?',
                    regexValidation: '=?',
                    serverValidationField: '@'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbLockedField', LockedFieldDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbMediaGrid
@restrict E
@scope

@description
Use this directive to generate a thumbnail grid of media items.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <umb-media-grid
           items="vm.mediaItems"
           on-click="vm.clickItem"
           on-click-name="vm.clickItemName">
        </umb-media-grid>

    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {
        "use strict";

        function Controller() {

            var vm = this;
            vm.mediaItems = [];

            vm.clickItem = clickItem;
            vm.clickItemName = clickItemName;

            myService.getMediaItems().then(function (mediaItems) {
                vm.mediaItems = mediaItems;
            });

            function clickItem(item, $event, $index){
                // do magic here
            }

            function clickItemName(item, $event, $index) {
                // set item.selected = true; to select the item
                // do magic here
            }

        }

        angular.module("umbraco").controller("My.Controller", Controller);
    })();
</pre>

@param {array} items (<code>binding</code>): Array of media items.
@param {callback=} onDetailsHover (<code>binding</code>): Callback method when the details icon is hovered.
    <h3>The callback returns:</h3>
    <ul>
        <li><code>item</code>: The hovered item</li>
        <li><code>$event</code>: The hover event</li>
        <li><code>hover</code>: Boolean to tell if the item is hovered or not</li>
    </ul>
@param {callback=} onClick (<code>binding</code>): Callback method to handle click events on the media item.
    <h3>The callback returns:</h3>
    <ul>
        <li><code>item</code>: The clicked item</li>
        <li><code>$event</code>: The click event</li>
        <li><code>$index</code>: The item index</li>
    </ul>
@param {callback=} onClickName (<code>binding</code>): Callback method to handle click events on the media item name.
    <h3>The callback returns:</h3>
    <ul>
        <li><code>item</code>: The clicked item</li>
        <li><code>$event</code>: The click event</li>
        <li><code>$index</code>: The item index</li>
    </ul>
@param {string=} filterBy (<code>binding</code>): String to filter media items by
@param {string=} itemMaxWidth (<code>attribute</code>): Sets a max width on the media item thumbnails.
@param {string=} itemMaxHeight (<code>attribute</code>): Sets a max height on the media item thumbnails.
@param {string=} itemMinWidth (<code>attribute</code>): Sets a min width on the media item thumbnails.
@param {string=} itemMinHeight (<code>attribute</code>): Sets a min height on the media item thumbnails.

**/
    (function () {
        'use strict';
        function MediaGridDirective($filter, mediaHelper) {
            function link(scope, el, attr, ctrl) {
                var itemDefaultHeight = 200;
                var itemDefaultWidth = 200;
                var itemMaxWidth = 200;
                var itemMaxHeight = 200;
                var itemMinWidth = 125;
                var itemMinHeight = 125;
                function activate() {
                    if (scope.itemMaxWidth) {
                        itemMaxWidth = scope.itemMaxWidth;
                    }
                    if (scope.itemMaxHeight) {
                        itemMaxHeight = scope.itemMaxHeight;
                    }
                    if (scope.itemMinWidth) {
                        itemMinWidth = scope.itemMinWidth;
                    }
                    if (scope.itemMinHeight) {
                        itemMinHeight = scope.itemMinHeight;
                    }
                    for (var i = 0; scope.items.length > i; i++) {
                        var item = scope.items[i];
                        setItemData(item);
                        setOriginalSize(item, itemMaxHeight);
                        // remove non images when onlyImages is set to true
                        if (scope.onlyImages === 'true' && !item.isFolder && !item.thumbnail) {
                            scope.items.splice(i, 1);
                            i--;
                        }
                        // If subfolder search is not enabled remove the media items that's not needed
                        // Make sure that includeSubFolder is not undefined since the directive is used
                        // in contexts where it should not be used. Currently only used when we trigger
                        // a media picker
                        if (scope.includeSubFolders !== undefined) {
                            if (scope.includeSubFolders !== 'true') {
                                if (item.parentId !== parseInt(scope.currentFolderId)) {
                                    scope.items.splice(i, 1);
                                    i--;
                                }
                            }
                        }
                    }
                    if (scope.items.length > 0) {
                        setFlexValues(scope.items);
                    }
                }
                function setItemData(item) {
                    // check if item is a folder
                    if (item.image) {
                        // if is has an image path, it is not a folder
                        item.isFolder = false;
                    } else {
                        item.isFolder = !mediaHelper.hasFilePropertyType(item);
                    }
                    if (!item.isFolder) {
                        // handle entity
                        if (item.image) {
                            item.thumbnail = mediaHelper.resolveFileFromEntity(item, true);
                            item.extension = mediaHelper.getFileExtension(item.image);    // handle full media object
                        } else {
                            item.thumbnail = mediaHelper.resolveFile(item, true);
                            item.image = mediaHelper.resolveFile(item, false);
                            var fileProp = _.find(item.properties, function (v) {
                                return v.alias === 'umbracoFile';
                            });
                            if (fileProp && fileProp.value) {
                                item.file = fileProp.value;
                            }
                            var extensionProp = _.find(item.properties, function (v) {
                                return v.alias === 'umbracoExtension';
                            });
                            if (extensionProp && extensionProp.value) {
                                item.extension = extensionProp.value;
                            }
                        }
                    }
                }
                function setOriginalSize(item, maxHeight) {
                    //set to a square by default
                    item.width = itemDefaultWidth;
                    item.height = itemDefaultHeight;
                    item.aspectRatio = 1;
                    var widthProp = _.find(item.properties, function (v) {
                        return v.alias === 'umbracoWidth';
                    });
                    if (widthProp && widthProp.value) {
                        item.width = parseInt(widthProp.value, 10);
                        if (isNaN(item.width)) {
                            item.width = itemDefaultWidth;
                        }
                    }
                    var heightProp = _.find(item.properties, function (v) {
                        return v.alias === 'umbracoHeight';
                    });
                    if (heightProp && heightProp.value) {
                        item.height = parseInt(heightProp.value, 10);
                        if (isNaN(item.height)) {
                            item.height = itemDefaultWidth;
                        }
                    }
                    item.aspectRatio = item.width / item.height;
                    // set max width and height
                    // landscape
                    if (item.aspectRatio >= 1) {
                        if (item.width > itemMaxWidth) {
                            item.width = itemMaxWidth;
                            item.height = itemMaxWidth / item.aspectRatio;
                        }    // portrait
                    } else {
                        if (item.height > itemMaxHeight) {
                            item.height = itemMaxHeight;
                            item.width = itemMaxHeight * item.aspectRatio;
                        }
                    }
                }
                function setFlexValues(mediaItems) {
                    var flexSortArray = mediaItems;
                    var smallestImageWidth = null;
                    var widestImageAspectRatio = null;
                    // sort array after image width with the widest image first
                    flexSortArray = $filter('orderBy')(flexSortArray, 'width', true);
                    // find widest image aspect ratio
                    widestImageAspectRatio = flexSortArray[0].aspectRatio;
                    // find smallest image width
                    smallestImageWidth = flexSortArray[flexSortArray.length - 1].width;
                    for (var i = 0; flexSortArray.length > i; i++) {
                        var mediaItem = flexSortArray[i];
                        var flex = 1 / (widestImageAspectRatio / mediaItem.aspectRatio);
                        if (flex === 0) {
                            flex = 1;
                        }
                        var imageMinFlexWidth = smallestImageWidth * flex;
                        var flexStyle = {
                            'flex': flex + ' 1 ' + imageMinFlexWidth + 'px',
                            'max-width': mediaItem.width + 'px',
                            'min-width': itemMinWidth + 'px',
                            'min-height': itemMinHeight + 'px'
                        };
                        mediaItem.flexStyle = flexStyle;
                    }
                }
                scope.clickItem = function (item, $event, $index) {
                    if (scope.onClick) {
                        scope.onClick(item, $event, $index);
                        $event.stopPropagation();
                    }
                };
                scope.clickItemName = function (item, $event, $index) {
                    if (scope.onClickName) {
                        scope.onClickName(item, $event, $index);
                        $event.stopPropagation();
                    }
                };
                scope.hoverItemDetails = function (item, $event, hover) {
                    if (scope.onDetailsHover) {
                        scope.onDetailsHover(item, $event, hover);
                    }
                };
                var unbindItemsWatcher = scope.$watch('items', function (newValue, oldValue) {
                    if (angular.isArray(newValue)) {
                        activate();
                    }
                });
                scope.$on('$destroy', function () {
                    unbindItemsWatcher();
                });
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-media-grid.html',
                scope: {
                    items: '=',
                    onDetailsHover: '=',
                    onClick: '=',
                    onClickName: '=',
                    filterBy: '=',
                    itemMaxWidth: '@',
                    itemMaxHeight: '@',
                    itemMinWidth: '@',
                    itemMinHeight: '@',
                    onlyImages: '@',
                    includeSubFolders: '@',
                    currentFolderId: '@'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbMediaGrid', MediaGridDirective);
    }());
    (function () {
        'use strict';
        function MiniListViewDirective(entityResource, iconHelper) {
            function link(scope, el, attr, ctrl) {
                scope.search = '';
                scope.miniListViews = [];
                scope.breadcrumb = [];
                var miniListViewsHistory = [];
                var goingForward = true;
                var skipAnimation = true;
                function onInit() {
                    open(scope.node);
                }
                function open(node) {
                    // convert legacy icon for node
                    if (node && node.icon) {
                        node.icon = iconHelper.convertFromLegacyIcon(node.icon);
                    }
                    goingForward = true;
                    var miniListView = {
                        node: node,
                        loading: true,
                        pagination: {
                            pageSize: 10,
                            pageNumber: 1,
                            filter: '',
                            orderDirection: 'Ascending',
                            orderBy: 'SortOrder',
                            orderBySystemField: true
                        }
                    };
                    // clear and push mini list view in dom so we only render 1 view
                    scope.miniListViews = [];
                    scope.miniListViews.push(miniListView);
                    // store in history so we quickly can navigate back
                    miniListViewsHistory.push(miniListView);
                    // get children
                    getChildrenForMiniListView(miniListView);
                    makeBreadcrumb();
                }
                function getChildrenForMiniListView(miniListView) {
                    // start loading animation list view
                    miniListView.loading = true;
                    entityResource.getPagedChildren(miniListView.node.id, scope.entityType, miniListView.pagination).then(function (data) {
                        // update children
                        miniListView.children = data.items;
                        _.each(miniListView.children, function (c) {
                            // child allowed by default
                            c.allowed = true;
                            // convert legacy icon for node
                            if (c.icon) {
                                c.icon = iconHelper.convertFromLegacyIcon(c.icon);
                            }
                            // set published state for content
                            if (c.metaData) {
                                c.hasChildren = c.metaData.HasChildren;
                                if (scope.entityType === 'Document') {
                                    c.published = c.metaData.IsPublished;
                                }
                            }
                            // filter items if there is a filter and it's not advanced
                            // ** ignores advanced filter at the moment
                            if (scope.entityTypeFilter && !scope.entityTypeFilter.filterAdvanced) {
                                var a = scope.entityTypeFilter.filter.toLowerCase().replace(/\s/g, '').split(',');
                                var found = a.indexOf(c.metaData.ContentTypeAlias.toLowerCase()) >= 0;
                                if (!scope.entityTypeFilter.filterExclude && !found || scope.entityTypeFilter.filterExclude && found) {
                                    c.allowed = false;
                                }
                            }
                        });
                        // update pagination
                        miniListView.pagination.totalItems = data.totalItems;
                        miniListView.pagination.totalPages = data.totalPages;
                        // stop load indicator
                        miniListView.loading = false;
                    });
                }
                scope.openNode = function (event, node) {
                    open(node);
                    event.stopPropagation();
                };
                scope.selectNode = function (node) {
                    if (scope.onSelect && node.allowed) {
                        scope.onSelect({ 'node': node });
                    }
                };
                /* Pagination */
                scope.goToPage = function (pageNumber, miniListView) {
                    // set new page number
                    miniListView.pagination.pageNumber = pageNumber;
                    // get children
                    getChildrenForMiniListView(miniListView);
                };
                /* Breadcrumb */
                scope.clickBreadcrumb = function (ancestor) {
                    var found = false;
                    goingForward = false;
                    angular.forEach(miniListViewsHistory, function (historyItem, index) {
                        // We need to make sure we can compare the two id's. 
                        // Some id's are integers and others are strings.
                        // Members have string ids like "all-members".
                        if (historyItem.node.id.toString() === ancestor.id.toString()) {
                            // load the list view from history
                            scope.miniListViews = [];
                            scope.miniListViews.push(historyItem);
                            // clean up history - remove all children after
                            miniListViewsHistory.splice(index + 1, miniListViewsHistory.length);
                            found = true;
                        }
                    });
                    if (!found) {
                        // if we can't find the view in the history - close the list view
                        scope.exitMiniListView();
                    }
                    // update the breadcrumb
                    makeBreadcrumb();
                };
                scope.showBackButton = function () {
                    // don't show the back button if the start node is a list view
                    if (scope.node.metaData && scope.node.metaData.IsContainer || scope.node.isContainer) {
                        return false;
                    } else {
                        return true;
                    }
                };
                scope.exitMiniListView = function () {
                    miniListViewsHistory = [];
                    scope.miniListViews = [];
                    if (scope.onClose) {
                        scope.onClose();
                    }
                };
                function makeBreadcrumb() {
                    scope.breadcrumb = [];
                    angular.forEach(miniListViewsHistory, function (historyItem) {
                        scope.breadcrumb.push(historyItem.node);
                    });
                }
                /* Search */
                scope.searchMiniListView = function (search, miniListView) {
                    // set search value
                    miniListView.pagination.filter = search;
                    // reset pagination
                    miniListView.pagination.pageNumber = 1;
                    // start loading animation list view
                    miniListView.loading = true;
                    searchMiniListView(miniListView);
                };
                var searchMiniListView = _.debounce(function (miniListView) {
                    scope.$apply(function () {
                        getChildrenForMiniListView(miniListView);
                    });
                }, 500);
                /* Animation */
                scope.getMiniListViewAnimation = function () {
                    // disable the first "slide-in-animation"" if the start node is a list view
                    if (scope.node.metaData && scope.node.metaData.IsContainer && skipAnimation || scope.node.isContainer && skipAnimation) {
                        skipAnimation = false;
                        return;
                    }
                    if (goingForward) {
                        return 'umb-mini-list-view--forward';
                    } else {
                        return 'umb-mini-list-view--backwards';
                    }
                };
                onInit();
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-mini-list-view.html',
                scope: {
                    node: '=',
                    entityType: '@',
                    startNodeId: '=',
                    onSelect: '&',
                    onClose: '&',
                    entityTypeFilter: '='
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbMiniListView', MiniListViewDirective);
    }());
    angular.module('umbraco.directives').directive('umbNestedContentEditor', [function () {
            var link = function ($scope) {
                // Clone the model because some property editors
                // do weird things like updating and config values
                // so we want to ensure we start from a fresh every
                // time, we'll just sync the value back when we need to
                $scope.model = angular.copy($scope.ngModel);
                $scope.nodeContext = $scope.model;
                // Find the selected tab
                var selectedTab = $scope.model.tabs[0];
                if ($scope.tabAlias) {
                    angular.forEach($scope.model.tabs, function (tab) {
                        if (tab.alias.toLowerCase() === $scope.tabAlias.toLowerCase()) {
                            selectedTab = tab;
                            return;
                        }
                    });
                }
                $scope.tab = selectedTab;
                // Listen for sync request
                var unsubscribe = $scope.$on('ncSyncVal', function (ev, args) {
                    if (args.key === $scope.model.key) {
                        // Tell inner controls we are submitting
                        $scope.$broadcast('formSubmitting', { scope: $scope });
                        // Sync the values back
                        angular.forEach($scope.ngModel.tabs, function (tab) {
                            if (tab.alias.toLowerCase() === selectedTab.alias.toLowerCase()) {
                                var localPropsMap = selectedTab.properties.reduce(function (map, obj) {
                                    map[obj.alias] = obj;
                                    return map;
                                }, {});
                                angular.forEach(tab.properties, function (prop) {
                                    if (localPropsMap.hasOwnProperty(prop.alias)) {
                                        prop.value = localPropsMap[prop.alias].value;
                                    }
                                });
                            }
                        });
                    }
                });
                $scope.$on('$destroy', function () {
                    unsubscribe();
                });
            };
            return {
                restrict: 'E',
                replace: true,
                templateUrl: Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath + '/views/propertyeditors/nestedcontent/nestedcontent.editor.html',
                scope: {
                    ngModel: '=',
                    tabAlias: '='
                },
                link: link
            };
        }]);
    //angular.module("umbraco.directives").directive('nestedContentSubmitWatcher', function () {
    //    var link = function (scope) {
    //        // call the load callback on scope to obtain the ID of this submit watcher
    //        var id = scope.loadCallback();
    //        scope.$on("formSubmitting", function (ev, args) {
    //            // on the "formSubmitting" event, call the submit callback on scope to notify the nestedContent controller to do it's magic
    //            if (id === scope.activeSubmitWatcher) {
    //                scope.submitCallback();
    //            }
    //        });
    //    }
    //    return {
    //        restrict: "E",
    //        replace: true,
    //        template: "",
    //        scope: {
    //            loadCallback: '=',
    //            submitCallback: '=',
    //            activeSubmitWatcher: '='
    //        },
    //        link: link
    //    }
    //});
    /**
@ngdoc directive
@name umbraco.directives.directive:umbNodePreview
@restrict E
@scope

@description
<strong>Added in Umbraco v. 7.6:</strong> Use this directive to render a node preview.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.NodePreviewController as vm">
        
        <div ui-sortable ng-model="vm.nodes">
            <umb-node-preview
                ng-repeat="node in vm.nodes"
                icon="node.icon"
                name="node.name"
                alias="node.alias"
                published="node.published"
                description="node.description"
                sortable="vm.sortable"
                allow-remove="vm.allowRemove"
                allow-open="vm.allowOpen"
                on-remove="vm.remove($index, vm.nodes)"
                on-open="vm.open(node)">
            </umb-node-preview>
        </div>
    
    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {
        "use strict";
    
        function Controller() {
    
            var vm = this;
    
            vm.allowRemove = true;
            vm.allowOpen = true;
            vm.sortable = true;
    
            vm.nodes = [
                {
                    "icon": "icon-document",
                    "name": "My node 1",
                    "published": true,
                    "description": "A short description of my node"
                },
                {
                    "icon": "icon-document",
                    "name": "My node 2",
                    "published": true,
                    "description": "A short description of my node"
                }
            ];
    
            vm.remove = remove;
            vm.open = open;
    
            function remove(index, nodes) {
                alert("remove node");
            }
    
            function open(node) {
                alert("open node");
            }
    
        }
    
        angular.module("umbraco").controller("My.NodePreviewController", Controller);
    
    })();
</pre>

@param {string} icon (<code>binding</code>): The node icon.
@param {string} name (<code>binding</code>): The node name.
@param {string} alias (<code>binding</code>): The node document type alias will be displayed on hover if in debug mode or logged in as admin
@param {boolean} published (<code>binding</code>): The node published state.
@param {string} description (<code>binding</code>): A short description.
@param {boolean} sortable (<code>binding</code>): Will add a move cursor on the node preview. Can used in combination with ui-sortable.
@param {boolean} allowRemove (<code>binding</code>): Show/Hide the remove button.
@param {boolean} allowOpen (<code>binding</code>): Show/Hide the open button.
@param {boolean} allowEdit (<code>binding</code>): Show/Hide the edit button (Added in version 7.7.0).
@param {function} onRemove (<code>expression</code>): Callback function when the remove button is clicked.
@param {function} onOpen (<code>expression</code>): Callback function when the open button is clicked.
@param {function} onEdit (<code>expression</code>): Callback function when the edit button is clicked (Added in version 7.7.0).
@param {string} openUrl (<code>binding</code>): Fallback URL for <code>onOpen</code> (Added in version 7.12.0).
@param {string} editUrl (<code>binding</code>): Fallback URL for <code>onEdit</code> (Added in version 7.12.0).
@param {string} removeUrl (<code>binding</code>): Fallback URL for <code>onRemove</code> (Added in version 7.12.0).
**/
    (function () {
        'use strict';
        function NodePreviewDirective(userService) {
            function link(scope, el, attr, ctrl) {
                if (!scope.editLabelKey) {
                    scope.editLabelKey = 'general_edit';
                }
                userService.getCurrentUser().then(function (u) {
                    var isAdmin = u.userGroups.indexOf('admin') !== -1;
                    scope.alias = Umbraco.Sys.ServerVariables.isDebuggingEnabled === true || isAdmin ? scope.alias : null;
                });
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-node-preview.html',
                scope: {
                    icon: '=?',
                    name: '=',
                    alias: '=?',
                    description: '=?',
                    permissions: '=?',
                    published: '=?',
                    sortable: '=?',
                    allowOpen: '=?',
                    allowRemove: '=?',
                    allowEdit: '=?',
                    onOpen: '&?',
                    onRemove: '&?',
                    onEdit: '&?',
                    openUrl: '=?',
                    editUrl: '=?',
                    removeUrl: '=?'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbNodePreview', NodePreviewDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbPagination
@restrict E
@scope

@description
Use this directive to generate a pagination.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <umb-pagination
            page-number="vm.pagination.pageNumber"
            total-pages="vm.pagination.totalPages"
            on-next="vm.nextPage"
            on-prev="vm.prevPage"
            on-go-to-page="vm.goToPage">
        </umb-pagination>

    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {
        "use strict";

        function Controller() {

            var vm = this;

            vm.pagination = {
                pageNumber: 1,
                totalPages: 10
            }

            vm.nextPage = nextPage;
            vm.prevPage = prevPage;
            vm.goToPage = goToPage;

            function nextPage(pageNumber) {
                // do magic here
                console.log(pageNumber);
                alert("nextpage");
            }

            function prevPage(pageNumber) {
                // do magic here
                console.log(pageNumber);
                alert("prevpage");
            }

            function goToPage(pageNumber) {
                // do magic here
                console.log(pageNumber);
                alert("go to");
            }

        }

        angular.module("umbraco").controller("My.Controller", Controller);
    })();
</pre>

@param {number} pageNumber (<code>binding</code>): Current page number.
@param {number} totalPages (<code>binding</code>): The total number of pages.
@param {callback} onNext (<code>binding</code>): Callback method to go to the next page.
    <h3>The callback returns:</h3>
    <ul>
        <li><code>pageNumber</code>: The page number</li>
    </ul>
@param {callback=} onPrev (<code>binding</code>): Callback method to go to the previous page.
    <h3>The callback returns:</h3>
    <ul>
        <li><code>pageNumber</code>: The page number</li>
    </ul>
@param {callback=} onGoToPage (<code>binding</code>): Callback method to go to a specific page.
    <h3>The callback returns:</h3>
    <ul>
        <li><code>pageNumber</code>: The page number</li>
    </ul>
**/
    (function () {
        'use strict';
        function PaginationDirective(localizationService) {
            function link(scope, el, attr, ctrl) {
                function activate() {
                    scope.pagination = [];
                    var i = 0;
                    if (scope.totalPages <= 10) {
                        for (i = 0; i < scope.totalPages; i++) {
                            scope.pagination.push({
                                val: i + 1,
                                isActive: scope.pageNumber === i + 1
                            });
                        }
                    } else {
                        //if there is more than 10 pages, we need to do some fancy bits
                        //get the max index to start
                        var maxIndex = scope.totalPages - 10;
                        //set the start, but it can't be below zero
                        var start = Math.max(scope.pageNumber - 5, 0);
                        //ensure that it's not too far either
                        start = Math.min(maxIndex, start);
                        for (i = start; i < 10 + start; i++) {
                            scope.pagination.push({
                                val: i + 1,
                                isActive: scope.pageNumber === i + 1
                            });
                        }
                        //now, if the start is greater than 0 then '1' will not be displayed, so do the elipses thing
                        if (start > 0) {
                            scope.pagination.unshift({
                                name: localizationService.localize('general_first'),
                                val: 1,
                                isActive: false
                            }, {
                                val: '...',
                                isActive: false
                            });
                        }
                        //same for the end
                        if (start < maxIndex) {
                            scope.pagination.push({
                                val: '...',
                                isActive: false
                            }, {
                                name: localizationService.localize('general_last'),
                                val: scope.totalPages,
                                isActive: false
                            });
                        }
                    }
                }
                scope.next = function () {
                    if (scope.pageNumber < scope.totalPages) {
                        scope.pageNumber++;
                        if (scope.onNext) {
                            scope.onNext(scope.pageNumber);
                        }
                        if (scope.onChange) {
                            scope.onChange({ 'pageNumber': scope.pageNumber });
                        }
                    }
                };
                scope.prev = function (pageNumber) {
                    if (scope.pageNumber > 1) {
                        scope.pageNumber--;
                        if (scope.onPrev) {
                            scope.onPrev(scope.pageNumber);
                        }
                        if (scope.onChange) {
                            scope.onChange({ 'pageNumber': scope.pageNumber });
                        }
                    }
                };
                scope.goToPage = function (pageNumber) {
                    scope.pageNumber = pageNumber + 1;
                    if (scope.onGoToPage) {
                        scope.onGoToPage(scope.pageNumber);
                    }
                    if (scope.onChange) {
                        if (scope.onChange) {
                            scope.onChange({ 'pageNumber': scope.pageNumber });
                        }
                    }
                };
                var unbindPageNumberWatcher = scope.$watchCollection('[pageNumber, totalPages]', function (newValues, oldValues) {
                    activate();
                });
                scope.$on('$destroy', function () {
                    unbindPageNumberWatcher();
                });
                activate();
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-pagination.html',
                scope: {
                    pageNumber: '=',
                    totalPages: '=',
                    onNext: '=',
                    onPrev: '=',
                    onGoToPage: '=',
                    onChange: '&'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbPagination', PaginationDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbPasswordToggle
@restrict E
@scope

@description
<strong>Added in Umbraco v. 7.7.4:</strong> Use this directive to render a password toggle.

**/
    (function () {
        'use strict';
        // comes from https://codepen.io/jakob-e/pen/eNBQaP
        // works fine with Angular 1.6.5 - alas not with 1.1.5 - binding issue
        function PasswordToggleDirective($compile) {
            var directive = {
                restrict: 'A',
                scope: {},
                link: function (scope, elem, attrs) {
                    scope.tgl = function () {
                        elem.attr('type', elem.attr('type') === 'text' ? 'password' : 'text');
                    };
                    var lnk = angular.element('<a data-ng-click="tgl()">Toggle</a>');
                    $compile(lnk)(scope);
                    elem.wrap('<div class="password-toggle"/>').after(lnk);
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbPasswordToggle', PasswordToggleDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbProgressBar
@restrict E
@scope

@description
Use this directive to generate a progress bar.

<h3>Markup example</h3>
<pre>
    <umb-progress-bar
        percentage="60">
    </umb-progress-bar>
</pre>

@param {number} percentage (<code>attribute</code>): The progress in percentage.
@param {string} size (<code>attribute</code>): The size (s, m).

**/
    (function () {
        'use strict';
        function ProgressBarDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-progress-bar.html',
                scope: {
                    percentage: '@',
                    size: '@?'
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbProgressBar', ProgressBarDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbProgressCircle
@restrict E
@scope

@description
Use this directive to render a circular progressbar.

<h3>Markup example</h3>
<pre>
    <div>
    
        <umb-progress-circle
            percentage="80"
            size="60"
            color="secondary">
        </umb-progress-circle>

	</div>
</pre>

@param {string} size (<code>attribute</code>): This parameter defines the width and the height of the circle in pixels.
@param {string} percentage (<code>attribute</code>): Takes a number between 0 and 100 and applies it to the circle's highlight length.
@param {string} color (<code>attribute</code>): the color of the highlight (primary, secondary, success, warning, danger). Success by default. 
**/
    (function () {
        'use strict';
        function ProgressCircleDirective($http, $timeout) {
            function link(scope, element, $filter) {
                function onInit() {
                    // making sure we get the right numbers
                    var percent = scope.percentage;
                    if (percent > 100) {
                        percent = 100;
                    } else if (percent < 0) {
                        percent = 0;
                    }
                    // calculating the circle's highlight
                    var circle = element.find('.umb-progress-circle__highlight');
                    var r = circle.attr('r');
                    var strokeDashArray = r * Math.PI * 2;
                    // Full circle length
                    scope.strokeDashArray = strokeDashArray;
                    var strokeDashOffsetDifference = percent / 100 * strokeDashArray;
                    var strokeDashOffset = strokeDashArray - strokeDashOffsetDifference;
                    // Distance for the highlight dash's offset
                    scope.strokeDashOffset = strokeDashOffset;
                    // set font size
                    scope.percentageSize = scope.size * 0.3 + 'px';
                }
                onInit();
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-progress-circle.html',
                scope: {
                    size: '@?',
                    percentage: '@',
                    color: '@'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbProgressCircle', ProgressCircleDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbStickyBar
@restrict A

@description
Use this directive make an element sticky and follow the page when scrolling.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <div
           class="my-sticky-bar"
           umb-sticky-bar
           scrollable-container=".container">
        </div>

    </div>
</pre>

<h3>CSS example</h3>
<pre>
    .my-sticky-bar {
        padding: 15px 0;
        background: #000000;
        position: relative;
        top: 0;
    }

    .my-sticky-bar.-umb-sticky-bar {
        top: 100px;
    }
</pre>

@param {string} scrollableContainer Set the class (".element") or the id ("#element") of the scrollable container element.
**/
    (function () {
        'use strict';
        function StickyBarDirective($rootScope) {
            function link(scope, el, attr, ctrl) {
                var bar = $(el);
                var scrollableContainer = null;
                var clonedBar = null;
                var cloneIsMade = false;
                function activate() {
                    if (bar.parents('.umb-property').length > 1) {
                        bar.addClass('nested');
                        return;
                    }
                    if (attr.scrollableContainer) {
                        scrollableContainer = $(attr.scrollableContainer);
                    } else {
                        scrollableContainer = $(window);
                    }
                    scrollableContainer.on('scroll.umbStickyBar', determineVisibility).trigger('scroll');
                    $(window).on('resize.umbStickyBar', determineVisibility);
                    scope.$on('$destroy', function () {
                        scrollableContainer.off('.umbStickyBar');
                        $(window).off('.umbStickyBar');
                    });
                }
                function determineVisibility() {
                    var barTop = bar[0].offsetTop;
                    var scrollTop = scrollableContainer.scrollTop();
                    if (scrollTop > barTop) {
                        if (!cloneIsMade) {
                            createClone();
                            clonedBar.css({ 'visibility': 'visible' });
                        } else {
                            calculateSize();
                        }
                    } else {
                        if (cloneIsMade) {
                            //remove cloned element (switched places with original on creation)
                            bar.remove();
                            bar = clonedBar;
                            clonedBar = null;
                            bar.removeClass('-umb-sticky-bar');
                            bar.css({
                                position: 'relative',
                                'width': 'auto',
                                'height': 'auto',
                                'z-index': 'auto',
                                'visibility': 'visible'
                            });
                            cloneIsMade = false;
                        }
                    }
                }
                function calculateSize() {
                    clonedBar.css({
                        width: bar.outerWidth(),
                        height: bar.height()
                    });
                }
                function createClone() {
                    //switch place with cloned element, to keep binding intact
                    clonedBar = bar;
                    bar = clonedBar.clone();
                    clonedBar.after(bar);
                    clonedBar.addClass('-umb-sticky-bar');
                    clonedBar.css({
                        'position': 'fixed',
                        'z-index': 500,
                        'visibility': 'hidden'
                    });
                    cloneIsMade = true;
                    calculateSize();
                }
                activate();
            }
            var directive = {
                restrict: 'A',
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbStickyBar', StickyBarDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbTable
@restrict E
@scope

@description
<strong>Added in Umbraco v. 7.4:</strong> Use this directive to render a data table.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.TableController as vm">
        
        <umb-table
            ng-if="items"
            items="vm.items"
            item-properties="vm.options.includeProperties"
            allow-select-all="vm.allowSelectAll"
            on-select="vm.selectItem"
            on-click="vm.clickItem"
            on-select-all="vm.selectAll"
            on-selected-all="vm.isSelectedAll"
            on-sorting-direction="vm.isSortDirection"
            on-sort="vm.sort">
        </umb-table>
    
    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {
        "use strict";
    
        function Controller() {
    
            var vm = this;
    
            vm.items = [
                {
                    "icon": "icon-document",
                    "name": "My node 1",
                    "published": true,
                    "description": "A short description of my node",
                    "author": "Author 1"
                },
                {
                    "icon": "icon-document",
                    "name": "My node 2",
                    "published": true,
                    "description": "A short description of my node",
                    "author": "Author 2"
                }
            ];

            vm.options = {
                includeProperties: [
                    { alias: "description", header: "Description" },
                    { alias: "author", header: "Author" }
                ]
            };
    
            vm.selectItem = selectItem;
            vm.clickItem = clickItem;
            vm.selectAll = selectAll;
            vm.isSelectedAll = isSelectedAll;
            vm.isSortDirection = isSortDirection;
            vm.sort = sort;

            function selectAll($event) {
                alert("select all");
            }

            function isSelectedAll() {
                
            }
    
            function clickItem(item) {
                alert("click node");
            }

            function selectItem(selectedItem, $index, $event) {
                alert("select node");
            }
            
            function isSortDirection(col, direction) {
                
            }
            
            function sort(field, allow, isSystem) {
                
            }
    
        }
    
        angular.module("umbraco").controller("My.TableController", Controller);
    
    })();
</pre>

@param {string} icon (<code>binding</code>): The node icon.
@param {string} name (<code>binding</code>): The node name.
@param {string} published (<code>binding</code>): The node published state.
@param {function} onSelect (<code>expression</code>): Callback function when the row is selected.
@param {function} onClick (<code>expression</code>): Callback function when the "Name" column link is clicked.
@param {function} onSelectAll (<code>expression</code>): Callback function when selecting all items.
@param {function} onSelectedAll (<code>expression</code>): Callback function when all items are selected.
@param {function} onSortingDirection (<code>expression</code>): Callback function when sorting direction is changed.
@param {function} onSort (<code>expression</code>): Callback function when sorting items.
**/
    (function () {
        'use strict';
        function TableDirective(iconHelper) {
            function link(scope, el, attr, ctrl) {
                scope.clickItem = function (item, $event) {
                    if (scope.onClick) {
                        scope.onClick(item);
                        $event.stopPropagation();
                    }
                };
                scope.selectItem = function (item, $index, $event) {
                    if (scope.onSelect) {
                        scope.onSelect(item, $index, $event);
                        $event.stopPropagation();
                    }
                };
                scope.selectAll = function ($event) {
                    if (scope.onSelectAll) {
                        scope.onSelectAll($event);
                    }
                };
                scope.isSelectedAll = function () {
                    if (scope.onSelectedAll && scope.items && scope.items.length > 0) {
                        return scope.onSelectedAll();
                    }
                };
                scope.isSortDirection = function (col, direction) {
                    if (scope.onSortingDirection) {
                        return scope.onSortingDirection(col, direction);
                    }
                };
                scope.sort = function (field, allow, isSystem) {
                    if (scope.onSort) {
                        scope.onSort(field, allow, isSystem);
                    }
                };
                scope.getIcon = function (entry) {
                    return iconHelper.convertFromLegacyIcon(entry.icon);
                };
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/umb-table.html',
                scope: {
                    items: '=',
                    itemProperties: '=',
                    allowSelectAll: '=',
                    onSelect: '=',
                    onClick: '=',
                    onSelectAll: '=',
                    onSelectedAll: '=',
                    onSortingDirection: '=',
                    onSort: '='
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbTable', TableDirective);
    }());
    /**
@ngdoc directive
@name umbraco.directives.directive:umbTooltip
@restrict E
@scope

@description
Use this directive to render a tooltip.

<h3>Markup example</h3>
<pre>
    <div ng-controller="My.Controller as vm">

        <div
            ng-mouseover="vm.mouseOver($event)"
            ng-mouseleave="vm.mouseLeave()">
            Hover me
        </div>

        <umb-tooltip
           ng-if="vm.tooltip.show"
           event="vm.tooltip.event">
           // tooltip content here
        </umb-tooltip>

    </div>
</pre>

<h3>Controller example</h3>
<pre>
    (function () {
        "use strict";

        function Controller() {

            var vm = this;
            vm.tooltip = {
                show: false,
                event: null
            };

            vm.mouseOver = mouseOver;
            vm.mouseLeave = mouseLeave;

            function mouseOver($event) {
                vm.tooltip = {
                    show: true,
                    event: $event
                };
            }

            function mouseLeave() {
                vm.tooltip = {
                    show: false,
                    event: null
                };
            }

        }

        angular.module("umbraco").controller("My.Controller", Controller);

    })();
</pre>

@param {string} event Set the $event from the target element to position the tooltip relative to the mouse cursor.
**/
    (function () {
        'use strict';
        function TooltipDirective($timeout) {
            function link(scope, el, attr, ctrl) {
                scope.tooltipStyles = {};
                scope.tooltipStyles.left = 0;
                scope.tooltipStyles.top = 0;
                function activate() {
                    $timeout(function () {
                        setTooltipPosition(scope.event);
                    });
                }
                function setTooltipPosition(event) {
                    var container = $('#contentwrapper');
                    var containerLeft = container[0].offsetLeft;
                    var containerRight = containerLeft + container[0].offsetWidth;
                    var containerTop = container[0].offsetTop;
                    var containerBottom = containerTop + container[0].offsetHeight;
                    var elementHeight = null;
                    var elementWidth = null;
                    var position = {
                        right: 'inherit',
                        left: 'inherit',
                        top: 'inherit',
                        bottom: 'inherit'
                    };
                    // element size
                    elementHeight = el.context.clientHeight;
                    elementWidth = el.context.clientWidth;
                    position.left = event.pageX - elementWidth / 2;
                    position.top = event.pageY;
                    // check to see if element is outside screen
                    // outside right
                    if (position.left + elementWidth > containerRight) {
                        position.right = 10;
                        position.left = 'inherit';
                    }
                    // outside bottom
                    if (position.top + elementHeight > containerBottom) {
                        position.bottom = 10;
                        position.top = 'inherit';
                    }
                    // outside left
                    if (position.left < containerLeft) {
                        position.left = containerLeft + 10;
                        position.right = 'inherit';
                    }
                    // outside top
                    if (position.top < containerTop) {
                        position.top = 10;
                        position.bottom = 'inherit';
                    }
                    scope.tooltipStyles = position;
                    el.css(position);
                }
                activate();
            }
            var directive = {
                restrict: 'E',
                transclude: true,
                replace: true,
                templateUrl: 'views/components/umb-tooltip.html',
                scope: { event: '=' },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbTooltip', TooltipDirective);
    }());
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbFileDropzone
* @restrict E
* @function
* @description
* Used by editors that require naming an entity. Shows a textbox/headline with a required validator within it's own form.
**/
    /*
TODO
.directive("umbFileDrop", function ($timeout, $upload, localizationService, umbRequestHelper){
    return{
        restrict: "A",
        link: function(scope, element, attrs){
            //load in the options model
        }
    }
})
*/
    angular.module('umbraco.directives').directive('umbFileDropzone', function ($timeout, Upload, localizationService, umbRequestHelper) {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: 'views/components/upload/umb-file-dropzone.html',
            scope: {
                parentId: '@',
                contentTypeAlias: '@',
                propertyAlias: '@',
                accept: '@',
                maxFileSize: '@',
                compact: '@',
                hideDropzone: '@',
                acceptedMediatypes: '=',
                filesQueued: '=',
                handleFile: '=',
                filesUploaded: '='
            },
            link: function (scope, element, attrs) {
                scope.queue = [];
                scope.done = [];
                scope.rejected = [];
                scope.currentFile = undefined;
                function _filterFile(file) {
                    var ignoreFileNames = ['Thumbs.db'];
                    var ignoreFileTypes = ['directory'];
                    // ignore files with names from the list
                    // ignore files with types from the list
                    // ignore files which starts with "."
                    if (ignoreFileNames.indexOf(file.name) === -1 && ignoreFileTypes.indexOf(file.type) === -1 && file.name.indexOf('.') !== 0) {
                        return true;
                    } else {
                        return false;
                    }
                }
                function _filesQueued(files, event) {
                    //Push into the queue
                    angular.forEach(files, function (file) {
                        if (_filterFile(file) === true) {
                            if (file.$error) {
                                scope.rejected.push(file);
                            } else {
                                scope.queue.push(file);
                            }
                        }
                    });
                    //when queue is done, kick the uploader
                    if (!scope.working) {
                        // Upload not allowed
                        if (!scope.acceptedMediatypes || !scope.acceptedMediatypes.length) {
                            files.map(function (file) {
                                file.uploadStatus = 'error';
                                file.serverErrorMessage = 'File type is not allowed here';
                                scope.rejected.push(file);
                            });
                            scope.queue = [];
                        }
                        // One allowed type
                        if (scope.acceptedMediatypes && scope.acceptedMediatypes.length === 1) {
                            // Standard setup - set alias to auto select to let the server best decide which media type to use
                            if (scope.acceptedMediatypes[0].alias === 'Image') {
                                scope.contentTypeAlias = 'umbracoAutoSelect';
                            } else {
                                scope.contentTypeAlias = scope.acceptedMediatypes[0].alias;
                            }
                            _processQueueItem();
                        }
                        // More than one, open dialog
                        if (scope.acceptedMediatypes && scope.acceptedMediatypes.length > 1) {
                            _chooseMediaType();
                        }
                    }
                }
                function _processQueueItem() {
                    if (scope.queue.length > 0) {
                        scope.currentFile = scope.queue.shift();
                        _upload(scope.currentFile);
                    } else if (scope.done.length > 0) {
                        if (scope.filesUploaded) {
                            //queue is empty, trigger the done action
                            scope.filesUploaded(scope.done);
                        }
                        //auto-clear the done queue after 3 secs
                        var currentLength = scope.done.length;
                        $timeout(function () {
                            scope.done.splice(0, currentLength);
                        }, 3000);
                    }
                }
                function _upload(file) {
                    scope.propertyAlias = scope.propertyAlias ? scope.propertyAlias : 'umbracoFile';
                    scope.contentTypeAlias = scope.contentTypeAlias ? scope.contentTypeAlias : 'Image';
                    Upload.upload({
                        url: umbRequestHelper.getApiUrl('mediaApiBaseUrl', 'PostAddFile'),
                        fields: {
                            'currentFolder': scope.parentId,
                            'contentTypeAlias': scope.contentTypeAlias,
                            'propertyAlias': scope.propertyAlias,
                            'path': file.path
                        },
                        file: file
                    }).progress(function (evt) {
                        if (file.uploadStat !== 'done' && file.uploadStat !== 'error') {
                            // calculate progress in percentage
                            var progressPercentage = parseInt(100 * evt.loaded / evt.total, 10);
                            // set percentage property on file
                            file.uploadProgress = progressPercentage;
                            // set uploading status on file
                            file.uploadStatus = 'uploading';
                        }
                    }).success(function (data, status, headers, config) {
                        if (data.notifications && data.notifications.length > 0) {
                            // set error status on file
                            file.uploadStatus = 'error';
                            // Throw message back to user with the cause of the error
                            file.serverErrorMessage = data.notifications[0].message;
                            // Put the file in the rejected pool
                            scope.rejected.push(file);
                        } else {
                            // set done status on file
                            file.uploadStatus = 'done';
                            file.uploadProgress = 100;
                            // set date/time for when done - used for sorting
                            file.doneDate = new Date();
                            // Put the file in the done pool
                            scope.done.push(file);
                        }
                        scope.currentFile = undefined;
                        //after processing, test if everthing is done
                        _processQueueItem();
                    }).error(function (evt, status, headers, config) {
                        // set status done
                        file.uploadStatus = 'error';
                        //if the service returns a detailed error
                        if (evt.InnerException) {
                            file.serverErrorMessage = evt.InnerException.ExceptionMessage;
                            //Check if its the common "too large file" exception
                            if (evt.InnerException.StackTrace && evt.InnerException.StackTrace.indexOf('ValidateRequestEntityLength') > 0) {
                                file.serverErrorMessage = 'File too large to upload';
                            }
                        } else if (evt.Message) {
                            file.serverErrorMessage = evt.Message;
                        }
                        // If file not found, server will return a 404 and display this message
                        if (status === 404) {
                            file.serverErrorMessage = 'File not found';
                        }
                        //after processing, test if everthing is done
                        scope.rejected.push(file);
                        scope.currentFile = undefined;
                        _processQueueItem();
                    });
                }
                function _chooseMediaType() {
                    scope.mediatypepickerOverlay = {
                        view: 'mediatypepicker',
                        title: 'Choose media type',
                        acceptedMediatypes: scope.acceptedMediatypes,
                        hideSubmitButton: true,
                        show: true,
                        submit: function (model) {
                            scope.contentTypeAlias = model.selectedType.alias;
                            scope.mediatypepickerOverlay.show = false;
                            scope.mediatypepickerOverlay = null;
                            _processQueueItem();
                        },
                        close: function (oldModel) {
                            scope.queue.map(function (file) {
                                file.uploadStatus = 'error';
                                file.serverErrorMessage = 'Cannot upload this file, no mediatype selected';
                                scope.rejected.push(file);
                            });
                            scope.queue = [];
                            scope.mediatypepickerOverlay.show = false;
                            scope.mediatypepickerOverlay = null;
                        }
                    };
                }
                scope.handleFiles = function (files, event) {
                    if (scope.filesQueued) {
                        scope.filesQueued(files, event);
                    }
                    _filesQueued(files, event);
                };
            }
        };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbFileUpload
* @function
* @restrict A
* @scope
* @description
*  Listens for file input control changes and emits events when files are selected for use in other controllers.
**/
    function umbFileUpload() {
        return {
            restrict: 'A',
            scope: true,
            //create a new scope
            link: function (scope, el, attrs) {
                el.bind('change', function (event) {
                    var files = event.target.files;
                    //emit event upward
                    scope.$emit('filesSelected', { files: files });
                });
            }
        };
    }
    angular.module('umbraco.directives').directive('umbFileUpload', umbFileUpload);
    /**
* @ngdoc directive
* @name umbraco.directives.directive:umbSingleFileUpload
* @function
* @restrict A
* @scope
* @description
*  A single file upload field that will reset itself based on the object passed in for the rebuild parameter. This
*  is required because the only way to reset an upload control is to replace it's html.
**/
    function umbSingleFileUpload($compile) {
        return {
            restrict: 'E',
            scope: { rebuild: '=' },
            replace: true,
            template: '<div><input type=\'file\' umb-file-upload /></div>',
            link: function (scope, el, attrs) {
                scope.$watch('rebuild', function (newVal, oldVal) {
                    if (newVal && newVal !== oldVal) {
                        //recompile it!
                        el.html('<input type=\'file\' umb-file-upload />');
                        $compile(el.contents())(scope);
                    }
                });
            }
        };
    }
    angular.module('umbraco.directives').directive('umbSingleFileUpload', umbSingleFileUpload);
    (function () {
        'use strict';
        function ChangePasswordController($scope) {
            function resetModel(isNew) {
                //the model config will contain an object, if it does not we'll create defaults
                //NOTE: We will not support doing the password regex on the client side because the regex on the server side
                //based on the membership provider cannot always be ported to js from .net directly.        
                /*
      {
          hasPassword: true/false,
          requiresQuestionAnswer: true/false,
          enableReset: true/false,
          enablePasswordRetrieval: true/false,
          minPasswordLength: 10
      }
      */
                $scope.showReset = false;
                //set defaults if they are not available
                if ($scope.config.disableToggle === undefined) {
                    $scope.config.disableToggle = false;
                }
                if ($scope.config.hasPassword === undefined) {
                    $scope.config.hasPassword = false;
                }
                if ($scope.config.enablePasswordRetrieval === undefined) {
                    $scope.config.enablePasswordRetrieval = true;
                }
                if ($scope.config.requiresQuestionAnswer === undefined) {
                    $scope.config.requiresQuestionAnswer = false;
                }
                //don't enable reset if it is new - that doesn't make sense
                if (isNew === 'true') {
                    $scope.config.enableReset = false;
                } else if ($scope.config.enableReset === undefined) {
                    $scope.config.enableReset = true;
                }
                if ($scope.config.minPasswordLength === undefined) {
                    $scope.config.minPasswordLength = 0;
                }
                //set the model defaults
                if (!angular.isObject($scope.passwordValues)) {
                    //if it's not an object then just create a new one
                    $scope.passwordValues = {
                        newPassword: null,
                        oldPassword: null,
                        reset: null,
                        answer: null
                    };
                } else {
                    //just reset the values
                    if (!isNew) {
                        //if it is new, then leave the generated pass displayed
                        $scope.passwordValues.newPassword = null;
                        $scope.passwordValues.oldPassword = null;
                    }
                    $scope.passwordValues.reset = null;
                    $scope.passwordValues.answer = null;
                }
                //the value to compare to match passwords
                if (!isNew) {
                    $scope.passwordValues.confirm = '';
                } else if ($scope.passwordValues.newPassword && $scope.passwordValues.newPassword.length > 0) {
                    //if it is new and a new password has been set, then set the confirm password too
                    $scope.passwordValues.confirm = $scope.passwordValues.newPassword;
                }
            }
            resetModel($scope.isNew);
            //if there is no password saved for this entity , it must be new so we do not allow toggling of the change password, it is always there
            //with validators turned on.
            $scope.changing = $scope.config.disableToggle === true || !$scope.config.hasPassword;
            //we're not currently changing so set the model to null
            if (!$scope.changing) {
                $scope.passwordValues = null;
            }
            $scope.doChange = function () {
                resetModel();
                $scope.changing = true;
                //if there was a previously generated password displaying, clear it
                $scope.passwordValues.generatedPassword = null;
                $scope.passwordValues.confirm = null;
            };
            $scope.cancelChange = function () {
                $scope.changing = false;
                //set model to null
                $scope.passwordValues = null;
            };
            var unsubscribe = [];
            //listen for the saved event, when that occurs we'll 
            //change to changing = false;
            unsubscribe.push($scope.$on('formSubmitted', function () {
                if ($scope.config.disableToggle === false) {
                    $scope.changing = false;
                }
            }));
            unsubscribe.push($scope.$on('formSubmitting', function () {
                //if there was a previously generated password displaying, clear it
                if ($scope.changing && $scope.passwordValues) {
                    $scope.passwordValues.generatedPassword = null;
                } else if (!$scope.changing) {
                    //we are not changing, so the model needs to be null
                    $scope.passwordValues = null;
                }
            }));
            //when the scope is destroyed we need to unsubscribe
            $scope.$on('$destroy', function () {
                for (var u in unsubscribe) {
                    unsubscribe[u]();
                }
            });
            $scope.showOldPass = function () {
                return $scope.config.hasPassword && !$scope.config.allowManuallyChangingPassword && !$scope.config.enablePasswordRetrieval && !$scope.showReset;
            };
            //TODO: I don't think we need this or the cancel button, this can be up to the editor rendering this directive
            $scope.showCancelBtn = function () {
                return $scope.config.disableToggle !== true && $scope.config.hasPassword;
            };
        }
        function ChangePasswordDirective() {
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/users/change-password.html',
                controller: 'Umbraco.Editors.Users.ChangePasswordDirectiveController',
                scope: {
                    isNew: '=?',
                    passwordValues: '=',
                    config: '='
                }
            };
            return directive;
        }
        angular.module('umbraco.directives').controller('Umbraco.Editors.Users.ChangePasswordDirectiveController', ChangePasswordController);
        angular.module('umbraco.directives').directive('changePassword', ChangePasswordDirective);
    }());
    (function () {
        'use strict';
        function PermissionDirective() {
            function link(scope, el, attr, ctrl) {
                scope.change = function () {
                    scope.selected = !scope.selected;
                    if (scope.onChange) {
                        scope.onChange({ 'selected': scope.selected });
                    }
                };
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/users/umb-permission.html',
                scope: {
                    name: '=',
                    description: '=?',
                    selected: '=',
                    onChange: '&'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbPermission', PermissionDirective);
    }());
    /** 
@ngdoc directive
@name umbraco.directives.directive:umbUserGroupPreview
@restrict E
@scope

@description
Use this directive to render a user group preview, where you can see the permissions the user or group has in the back office.

<h3>Markup example</h3>
<pre>
    <div>
        <umb-user-group-preview
            ng-repeat="userGroup in vm.user.userGroups"
            icon="userGroup.icon"
            name="userGroup.name"
            sections="userGroup.sections"
            content-start-node="userGroup.contentStartNode"
            media-start-node="userGroup.mediaStartNode"
            allow-remove="!vm.user.isCurrentUser"
            on-remove="vm.removeSelectedItem($index, vm.user.userGroups)">
        </umb-user-group-preview>
    </div>
</pre>

@param {string} icon (<code>binding</code>): The user group icon.
@param {string} name (<code>binding</code>): The user group name.
@param {array} sections (<code>binding</code>) Lists out the sections where the user has authority to edit.
@param {string} contentStartNode (<code>binding</code>)
<ul>
    <li>The starting point in the tree of the content section.</li>
    <li>So the user has no authority to work on other branches, only on this branch in the content section.</li>
</ul>
@param {boolean} hideContentStartNode (<code>binding</code>) Hides the contentStartNode.
@param {string} mediaStartNode (<code>binding</code>)
<ul>
<li> The starting point in the tree of the media section.</li>
<li> So the user has no authority to work on other branches, only on this branch in the media section.</li>
</ul>
@param {boolean} hideMediaStartNode (<code>binding</code>) Hides the mediaStartNode.
@param {array} permissions (<code>binding<code>) A list of permissions, the user can have.
@param {boolean} allowRemove (<code>binding</code>): Shows or Hides the remove button.
@param {function} onRemove (<code>expression</code>): Callback function when the remove button is clicked.
@param {boolean} allowEdit (<code>binding</code>): Shows or Hides the edit button.
@param {function} onEdit (<code>expression</code>): Callback function when the edit button is clicked.
**/
    (function () {
        'use strict';
        function UserGroupPreviewDirective() {
            function link(scope, el, attr, ctrl) {
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/users/umb-user-group-preview.html',
                scope: {
                    icon: '=?',
                    name: '=',
                    sections: '=?',
                    contentStartNode: '=?',
                    hideContentStartNode: '@?',
                    mediaStartNode: '=?',
                    hideMediaStartNode: '@?',
                    permissions: '=?',
                    allowRemove: '=?',
                    allowEdit: '=?',
                    onRemove: '&?',
                    onEdit: '&?'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbUserGroupPreview', UserGroupPreviewDirective);
    }());
    (function () {
        'use strict';
        function UserPreviewDirective() {
            function link(scope, el, attr, ctrl) {
            }
            var directive = {
                restrict: 'E',
                replace: true,
                templateUrl: 'views/components/users/umb-user-preview.html',
                scope: {
                    avatars: '=?',
                    name: '=',
                    allowRemove: '=?',
                    onRemove: '&?'
                },
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbUserPreview', UserPreviewDirective);
    }());
    /**
 * Konami Code directive for AngularJS
 * @version v0.0.1
 * @license MIT License, https://www.opensource.org/licenses/MIT
 */
    angular.module('umbraco.directives').directive('konamiCode', [
        '$document',
        function ($document) {
            var konamiKeysDefault = [
                38,
                38,
                40,
                40,
                37,
                39,
                37,
                39,
                66,
                65
            ];
            return {
                restrict: 'A',
                link: function (scope, element, attr) {
                    if (!attr.konamiCode) {
                        throw 'Konami directive must receive an expression as value.';
                    }
                    // Let user define a custom code.
                    var konamiKeys = attr.konamiKeys || konamiKeysDefault;
                    var keyIndex = 0;
                    /**
               * Fired when konami code is type.
               */
                    function activated() {
                        if ('konamiOnce' in attr) {
                            stopListening();
                        }
                        // Execute expression.
                        scope.$eval(attr.konamiCode);
                    }
                    /**
               * Handle keydown events.
               */
                    function keydown(e) {
                        if (e.keyCode === konamiKeys[keyIndex++]) {
                            if (keyIndex === konamiKeys.length) {
                                keyIndex = 0;
                                activated();
                            }
                        } else {
                            keyIndex = 0;
                        }
                    }
                    /**
               * Stop to listen typing.
               */
                    function stopListening() {
                        $document.off('keydown', keydown);
                    }
                    // Start listening to key typing.
                    $document.on('keydown', keydown);
                    // Stop listening when scope is destroyed.
                    scope.$on('$destroy', stopListening);
                }
            };
        }
    ]);
    /**
@ngdoc directive
@name umbraco.directives.directive:umbKeyboardList
@restrict E

@description
<b>Added in versions 7.7.0</b>: Use this directive to add arrow up and down keyboard shortcuts to a list. Use this together with the {@link umbraco.directives.directive:umbDropdown umbDropdown} component to make easy accessible dropdown menus.

<h3>Markup example</h3>
<pre>
    <div>
        <ul umb-keyboard-list>
            <li><a href="">Item 1</a></li>
            <li><a href="">Item 2</a></li>
            <li><a href="">Item 3</a></li>
            <li><a href="">Item 4</a></li>
            <li><a href="">Item 5</a></li>
            <li><a href="">Item 6</a></li>
        </ul>
    </div>
</pre>

<h3>Use in combination with</h3>
<ul>
    <li>{@link umbraco.directives.directive:umbDropdown umbDropdown}</li>
</ul>

**/
    angular.module('umbraco.directives').directive('umbKeyboardList', [
        '$document',
        '$timeout',
        function ($document, $timeout) {
            return {
                restrict: 'A',
                link: function (scope, element, attr) {
                    var listItems = [];
                    var currentIndex = 0;
                    var focusSet = false;
                    $timeout(function () {
                        // get list of all links in the list
                        listItems = element.find('li a');
                    });
                    // Handle keydown events
                    function keydown(event) {
                        $timeout(function () {
                            checkFocus();
                            // arrow down
                            if (event.keyCode === 40) {
                                arrowDown();
                            }
                            // arrow up
                            if (event.keyCode === 38) {
                                arrowUp();
                            }
                        });
                    }
                    function checkFocus() {
                        var found = false;
                        // check if any element has focus
                        angular.forEach(listItems, function (item, index) {
                            if ($(item).is(':focus')) {
                                // if an element already has focus set the
                                // currentIndex so we navigate from that element
                                currentIndex = index;
                                focusSet = true;
                                found = true;
                            }
                        });
                        // If we don't find an element with focus we reset the currentIndex and the focusSet flag
                        // we do this because you can have navigated away from the list with tab and we want to reset it if you navigate back
                        if (!found) {
                            currentIndex = 0;
                            focusSet = false;
                        }
                    }
                    function arrowDown() {
                        if (currentIndex < listItems.length - 1) {
                            // only bump the current index if the focus is already 
                            // set else we just want to focus the first element
                            if (focusSet) {
                                currentIndex++;
                            }
                            listItems[currentIndex].focus();
                            focusSet = true;
                        }
                    }
                    function arrowUp() {
                        if (currentIndex > 0) {
                            currentIndex--;
                            listItems[currentIndex].focus();
                        }
                    }
                    // Stop to listen typing.
                    function stopListening() {
                        $document.off('keydown', keydown);
                    }
                    // Start listening to key typing.
                    $document.on('keydown', keydown);
                    // Stop listening when scope is destroyed.
                    scope.$on('$destroy', stopListening);
                }
            };
        }
    ]);
    /**
* @ngdoc directive
* @name umbraco.directives.directive:noDirtyCheck
* @restrict A
* @description Can be attached to form inputs to prevent them from setting the form as dirty (https://stackoverflow.com/questions/17089090/prevent-input-from-setting-form-dirty-angularjs)
**/
    function noDirtyCheck() {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                var alwaysFalse = {
                    get: function () {
                        return false;
                    },
                    set: function () {
                    }
                };
                Object.defineProperty(ctrl, '$pristine', alwaysFalse);
                Object.defineProperty(ctrl, '$dirty', alwaysFalse);
            }
        };
    }
    angular.module('umbraco.directives.validation').directive('noDirtyCheck', noDirtyCheck);
    (function () {
        'use strict';
        function SetDirtyOnChange() {
            function link(scope, el, attr, ctrl) {
                if (attr.ngModel) {
                    scope.$watch(attr.ngModel, function (newValue, oldValue) {
                        if (!newValue) {
                            return;
                        }
                        if (newValue === oldValue) {
                            return;
                        }
                        ctrl.$setDirty();
                    }, true);
                } else {
                    var initValue = attr.umbSetDirtyOnChange;
                    attr.$observe('umbSetDirtyOnChange', function (newValue) {
                        if (newValue !== initValue) {
                            ctrl.$setDirty();
                        }
                    });
                }
            }
            var directive = {
                require: '^form',
                restrict: 'A',
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('umbSetDirtyOnChange', SetDirtyOnChange);
    }());
    angular.module('umbraco.directives.validation').directive('valCompare', function () {
        return {
            require: [
                'ngModel',
                '^form'
            ],
            link: function (scope, elem, attrs, ctrls) {
                var ctrl = ctrls[0];
                var formCtrl = ctrls[1];
                var otherInput = formCtrl[attrs.valCompare];
                ctrl.$parsers.push(function (value) {
                    if (value === otherInput.$viewValue) {
                        ctrl.$setValidity('valCompare', true);
                        return value;
                    }
                    ctrl.$setValidity('valCompare', false);
                });
                otherInput.$parsers.push(function (value) {
                    ctrl.$setValidity('valCompare', value === ctrl.$viewValue);
                    return value;
                });
            }
        };
    });
    /**
 * General-purpose validator for ngModel.
 * angular.js comes with several built-in validation mechanism for input fields (ngRequired, ngPattern etc.) but using
 * an arbitrary validation function requires creation of a custom formatters and / or parsers.
 * The ui-validate directive makes it easy to use any function(s) defined in scope as a validator function(s).
 * A validator function will trigger validation on both model and input changes.
 *
 * @example <input val-custom=" 'myValidatorFunction($value)' ">
 * @example <input val-custom="{ foo : '$value > anotherModel', bar : 'validateFoo($value)' }">
 * @example <input val-custom="{ foo : '$value > anotherModel' }" val-custom-watch=" 'anotherModel' ">
 * @example <input val-custom="{ foo : '$value > anotherModel', bar : 'validateFoo($value)' }" val-custom-watch=" { foo : 'anotherModel' } ">
 *
 * @param val-custom {string|object literal} If strings is passed it should be a scope's function to be used as a validator.
 * If an object literal is passed a key denotes a validation error key while a value should be a validator function.
 * In both cases validator function should take a value to validate as its argument and should return true/false indicating a validation result.
 */
    /* 
  This code comes from the angular UI project, we had to change the directive name and module
  but other then that its unmodified
 */
    angular.module('umbraco.directives.validation').directive('valCustom', function () {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function (scope, elm, attrs, ctrl) {
                var validateFn, watch, validators = {}, validateExpr = scope.$eval(attrs.valCustom);
                if (!validateExpr) {
                    return;
                }
                if (angular.isString(validateExpr)) {
                    validateExpr = { validator: validateExpr };
                }
                angular.forEach(validateExpr, function (exprssn, key) {
                    validateFn = function (valueToValidate) {
                        var expression = scope.$eval(exprssn, { '$value': valueToValidate });
                        if (angular.isObject(expression) && angular.isFunction(expression.then)) {
                            // expression is a promise
                            expression.then(function () {
                                ctrl.$setValidity(key, true);
                            }, function () {
                                ctrl.$setValidity(key, false);
                            });
                            return valueToValidate;
                        } else if (expression) {
                            // expression is true
                            ctrl.$setValidity(key, true);
                            return valueToValidate;
                        } else {
                            // expression is false
                            ctrl.$setValidity(key, false);
                            return undefined;
                        }
                    };
                    validators[key] = validateFn;
                    ctrl.$parsers.push(validateFn);
                });
                function apply_watch(watch) {
                    //string - update all validators on expression change
                    if (angular.isString(watch)) {
                        scope.$watch(watch, function () {
                            angular.forEach(validators, function (validatorFn) {
                                validatorFn(ctrl.$modelValue);
                            });
                        });
                        return;
                    }
                    //array - update all validators on change of any expression
                    if (angular.isArray(watch)) {
                        angular.forEach(watch, function (expression) {
                            scope.$watch(expression, function () {
                                angular.forEach(validators, function (validatorFn) {
                                    validatorFn(ctrl.$modelValue);
                                });
                            });
                        });
                        return;
                    }
                    //object - update appropriate validator
                    if (angular.isObject(watch)) {
                        angular.forEach(watch, function (expression, validatorKey) {
                            //value is string - look after one expression
                            if (angular.isString(expression)) {
                                scope.$watch(expression, function () {
                                    validators[validatorKey](ctrl.$modelValue);
                                });
                            }
                            //value is array - look after all expressions in array
                            if (angular.isArray(expression)) {
                                angular.forEach(expression, function (intExpression) {
                                    scope.$watch(intExpression, function () {
                                        validators[validatorKey](ctrl.$modelValue);
                                    });
                                });
                            }
                        });
                    }
                }
                // Support for val-custom-watch
                if (attrs.valCustomWatch) {
                    apply_watch(scope.$eval(attrs.valCustomWatch));
                }
            }
        };
    });
    /**
    * @ngdoc directive
    * @name umbraco.directives.directive:valEmail
    * @restrict A
    * @description A custom directive to validate an email address string, this is required because angular's default validator is incorrect.
    **/
    function valEmail(valEmailExpression) {
        return {
            require: 'ngModel',
            restrict: 'A',
            link: function (scope, elm, attrs, ctrl) {
                var patternValidator = function (viewValue) {
                    //NOTE: we don't validate on empty values, use required validator for that
                    if (!viewValue || valEmailExpression.EMAIL_REGEXP.test(viewValue)) {
                        // it is valid
                        ctrl.$setValidity('valEmail', true);
                        //assign a message to the validator
                        ctrl.errorMsg = '';
                        return viewValue;
                    } else {
                        // it is invalid, return undefined (no model update)
                        ctrl.$setValidity('valEmail', false);
                        //assign a message to the validator
                        ctrl.errorMsg = 'Invalid email';
                        return undefined;
                    }
                };
                //if there is an attribute: type="email" then we need to remove those formatters and parsers
                if (attrs.type === 'email') {
                    //we need to remove the existing parsers = the default angular one which is created by
                    // type="email", but this has a regex issue, so we'll remove that and add our custom one
                    ctrl.$parsers.pop();
                    //we also need to remove the existing formatter - the default angular one will not render
                    // what it thinks is an invalid email address, so it will just be blank
                    ctrl.$formatters.pop();
                }
                ctrl.$parsers.push(patternValidator);
            }
        };
    }
    angular.module('umbraco.directives.validation').directive('valEmail', valEmail).factory('valEmailExpression', function () {
        var emailRegex = /^[a-z0-9!#$%&'*+\/=?^_`{|}~.-]+@[a-z0-9]([a-z0-9-]*[a-z0-9])?(\.[a-z0-9]([a-z0-9-]*[a-z0-9])?)*$/i;
        return { EMAIL_REGEXP: emailRegex };
    });
    /**
* @ngdoc directive
* @name umbraco.directives.directive:valFormManager
* @restrict A
* @require formController
* @description Used to broadcast an event to all elements inside this one to notify that form validation has 
* changed. If we don't use this that means you have to put a watch for each directive on a form's validation
* changing which would result in much higher processing. We need to actually watch the whole $error collection of a form
* because just watching $valid or $invalid doesn't acurrately trigger form validation changing.
* This also sets the show-validation (or a custom) css class on the element when the form is invalid - this lets
* us css target elements to be displayed when the form is submitting/submitted.
* Another thing this directive does is to ensure that any .control-group that contains form elements that are invalid will
* be marked with the 'error' css class. This ensures that labels included in that control group are styled correctly.
**/
    function valFormManager(serverValidationManager, $rootScope, $log, $timeout, notificationsService, eventsService, $routeParams) {
        return {
            require: 'form',
            restrict: 'A',
            controller: function ($scope) {
                //This exposes an API for direct use with this directive
                var unsubscribe = [];
                var self = this;
                //This is basically the same as a directive subscribing to an event but maybe a little
                // nicer since the other directive can use this directive's API instead of a magical event
                this.onValidationStatusChanged = function (cb) {
                    unsubscribe.push($scope.$on('valStatusChanged', function (evt, args) {
                        cb.apply(self, [
                            evt,
                            args
                        ]);
                    }));
                };
                //Ensure to remove the event handlers when this instance is destroyted
                $scope.$on('$destroy', function () {
                    for (var u in unsubscribe) {
                        unsubscribe[u]();
                    }
                });
            },
            link: function (scope, element, attr, formCtrl) {
                scope.$watch(function () {
                    return formCtrl.$error;
                }, function (e) {
                    scope.$broadcast('valStatusChanged', { form: formCtrl });
                    //find all invalid elements' .control-group's and apply the error class
                    var inError = element.find('.control-group .ng-invalid').closest('.control-group');
                    inError.addClass('error');
                    //find all control group's that have no error and ensure the class is removed
                    var noInError = element.find('.control-group .ng-valid').closest('.control-group').not(inError);
                    noInError.removeClass('error');
                }, true);
                var className = attr.valShowValidation ? attr.valShowValidation : 'show-validation';
                var savingEventName = attr.savingEvent ? attr.savingEvent : 'formSubmitting';
                var savedEvent = attr.savedEvent ? attr.savingEvent : 'formSubmitted';
                //This tracks if the user is currently saving a new item, we use this to determine 
                // if we should display the warning dialog that they are leaving the page - if a new item
                // is being saved we never want to display that dialog, this will also cause problems when there
                // are server side validation issues.
                var isSavingNewItem = false;
                //we should show validation if there are any msgs in the server validation collection
                if (serverValidationManager.items.length > 0) {
                    element.addClass(className);
                }
                var unsubscribe = [];
                //listen for the forms saving event
                unsubscribe.push(scope.$on(savingEventName, function (ev, args) {
                    element.addClass(className);
                    //set the flag so we can check to see if we should display the error.
                    isSavingNewItem = $routeParams.create;
                }));
                //listen for the forms saved event
                unsubscribe.push(scope.$on(savedEvent, function (ev, args) {
                    //remove validation class
                    element.removeClass(className);
                    //clear form state as at this point we retrieve new data from the server
                    //and all validation will have cleared at this point    
                    formCtrl.$setPristine();
                }));
                //This handles the 'unsaved changes' dialog which is triggered when a route is attempting to be changed but
                // the form has pending changes
                var locationEvent = $rootScope.$on('$locationChangeStart', function (event, nextLocation, currentLocation) {
                    if (!formCtrl.$dirty || isSavingNewItem) {
                        return;
                    }
                    var path = nextLocation.split('#')[1];
                    if (path) {
                        if (path.indexOf('%253') || path.indexOf('%252')) {
                            path = decodeURIComponent(path);
                        }
                        if (!notificationsService.hasView()) {
                            var msg = {
                                view: 'confirmroutechange',
                                args: {
                                    path: path,
                                    listener: locationEvent
                                }
                            };
                            notificationsService.add(msg);
                        }
                        //prevent the route!
                        event.preventDefault();
                        //raise an event
                        eventsService.emit('valFormManager.pendingChanges', true);
                    }
                });
                unsubscribe.push(locationEvent);
                //Ensure to remove the event handler when this instance is destroyted
                scope.$on('$destroy', function () {
                    for (var u in unsubscribe) {
                        unsubscribe[u]();
                    }
                });
                $timeout(function () {
                    formCtrl.$setPristine();
                }, 1000);
            }
        };
    }
    angular.module('umbraco.directives.validation').directive('valFormManager', valFormManager);
    /**
* @ngdoc directive
* @name umbraco.directives.directive:valHighlight
* @restrict A
* @description Used on input fields when you want to signal that they are in error, this will highlight the item for 1 second
**/
    function valHighlight($timeout) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs, ctrl) {
                attrs.$observe('valHighlight', function (newVal) {
                    if (newVal === 'true') {
                        element.addClass('highlight-error');
                        $timeout(function () {
                            //set the bound scope property to false
                            scope[attrs.valHighlight] = false;
                        }, 1000);
                    } else {
                        element.removeClass('highlight-error');
                    }
                });
            }
        };
    }
    angular.module('umbraco.directives.validation').directive('valHighlight', valHighlight);
    /**
* @ngdoc directive
* @name umbraco.directives.directive:valPropertyMsg
* @restrict A
* @element textarea
* @requires formController
* @description This directive is used to control the display of the property level validation message.
* We will listen for server side validation changes
* and when an error is detected for this property we'll show the error message.
* In order for this directive to work, the valStatusChanged directive must be placed on the containing form.
**/
    function valPropertyMsg(serverValidationManager) {
        return {
            scope: { property: '=' },
            require: '^form',
            //require that this directive is contained within an ngForm
            replace: true,
            //replace the element with the template
            restrict: 'E',
            //restrict to element
            template: '<div ng-show="errorMsg != \'\'" class=\'alert alert-error property-error\' >{{errorMsg}}</div>',
            /**
            Our directive requries a reference to a form controller 
            which gets passed in to this parameter
         */
            link: function (scope, element, attrs, formCtrl) {
                var watcher = null;
                // Gets the error message to display
                function getErrorMsg() {
                    //this can be null if no property was assigned
                    if (scope.property) {
                        //first try to get the error msg from the server collection
                        var err = serverValidationManager.getPropertyError(scope.property.alias, '');
                        //if there's an error message use it
                        if (err && err.errorMsg) {
                            return err.errorMsg;
                        } else {
                            return scope.property.propertyErrorMessage ? scope.property.propertyErrorMessage : 'Property has errors';
                        }
                    }
                    return 'Property has errors';
                }
                // We need to subscribe to any changes to our model (based on user input)
                // This is required because when we have a server error we actually invalidate 
                // the form which means it cannot be resubmitted. 
                // So once a field is changed that has a server error assigned to it
                // we need to re-validate it for the server side validator so the user can resubmit
                // the form. Of course normal client-side validators will continue to execute. 
                function startWatch() {
                    //if there's not already a watch
                    if (!watcher) {
                        watcher = scope.$watch('property.value', function (newValue, oldValue) {
                            if (!newValue || angular.equals(newValue, oldValue)) {
                                return;
                            }
                            var errCount = 0;
                            for (var e in formCtrl.$error) {
                                if (angular.isArray(formCtrl.$error[e])) {
                                    errCount++;
                                }
                            }
                            //we are explicitly checking for valServer errors here, since we shouldn't auto clear
                            // based on other errors. We'll also check if there's no other validation errors apart from valPropertyMsg, if valPropertyMsg
                            // is the only one, then we'll clear.
                            if (errCount === 1 && angular.isArray(formCtrl.$error.valPropertyMsg) || formCtrl.$invalid && angular.isArray(formCtrl.$error.valServer)) {
                                scope.errorMsg = '';
                                formCtrl.$setValidity('valPropertyMsg', true);
                                stopWatch();
                            }
                        }, true);
                    }
                }
                //clear the watch when the property validator is valid again
                function stopWatch() {
                    if (watcher) {
                        watcher();
                        watcher = null;
                    }
                }
                //if there's any remaining errors in the server validation service then we should show them.
                var showValidation = serverValidationManager.items.length > 0;
                var hasError = false;
                //create properties on our custom scope so we can use it in our template
                scope.errorMsg = '';
                var unsubscribe = [];
                //listen for form error changes
                unsubscribe.push(scope.$on('valStatusChanged', function (evt, args) {
                    if (args.form.$invalid) {
                        //first we need to check if the valPropertyMsg validity is invalid
                        if (formCtrl.$error.valPropertyMsg && formCtrl.$error.valPropertyMsg.length > 0) {
                            //since we already have an error we'll just return since this means we've already set the 
                            // hasError and errorMsg properties which occurs below in the serverValidationManager.subscribe
                            return;
                        } else if (element.closest('.umb-control-group').find('.ng-invalid').length > 0) {
                            //check if it's one of the properties that is invalid in the current content property
                            hasError = true;
                            //update the validation message if we don't already have one assigned.
                            if (showValidation && scope.errorMsg === '') {
                                scope.errorMsg = getErrorMsg();
                            }
                        } else {
                            hasError = false;
                            scope.errorMsg = '';
                        }
                    } else {
                        hasError = false;
                        scope.errorMsg = '';
                    }
                }, true));
                //listen for the forms saving event
                unsubscribe.push(scope.$on('formSubmitting', function (ev, args) {
                    showValidation = true;
                    if (hasError && scope.errorMsg === '') {
                        scope.errorMsg = getErrorMsg();
                    } else if (!hasError) {
                        scope.errorMsg = '';
                        stopWatch();
                    }
                }));
                //listen for the forms saved event
                unsubscribe.push(scope.$on('formSubmitted', function (ev, args) {
                    showValidation = false;
                    scope.errorMsg = '';
                    formCtrl.$setValidity('valPropertyMsg', true);
                    stopWatch();
                }));
                //listen for server validation changes
                // NOTE: we pass in "" in order to listen for all validation changes to the content property, not for
                // validation changes to fields in the property this is because some server side validators may not
                // return the field name for which the error belongs too, just the property for which it belongs.
                // It's important to note that we need to subscribe to server validation changes here because we always must
                // indicate that a content property is invalid at the property level since developers may not actually implement
                // the correct field validation in their property editors.
                if (scope.property) {
                    //this can be null if no property was assigned
                    serverValidationManager.subscribe(scope.property.alias, '', function (isValid, propertyErrors, allErrors) {
                        hasError = !isValid;
                        if (hasError) {
                            //set the error message to the server message
                            scope.errorMsg = propertyErrors[0].errorMsg;
                            //flag that the current validator is invalid
                            formCtrl.$setValidity('valPropertyMsg', false);
                            startWatch();
                        } else {
                            scope.errorMsg = '';
                            //flag that the current validator is valid
                            formCtrl.$setValidity('valPropertyMsg', true);
                            stopWatch();
                        }
                    });
                    //when the element is disposed we need to unsubscribe!
                    // NOTE: this is very important otherwise when this controller re-binds the previous subscriptsion will remain
                    // but they are a different callback instance than the above.
                    element.bind('$destroy', function () {
                        stopWatch();
                        serverValidationManager.unsubscribe(scope.property.alias, '');
                    });
                }
                //when the scope is disposed we need to unsubscribe
                scope.$on('$destroy', function () {
                    for (var u in unsubscribe) {
                        unsubscribe[u]();
                    }
                });
            }
        };
    }
    angular.module('umbraco.directives.validation').directive('valPropertyMsg', valPropertyMsg);
    /**
* @ngdoc directive
* @name umbraco.directives.directive:valPropertyValidator
* @restrict A
* @description Performs any custom property value validation checks on the client side. This allows property editors to be highly flexible when it comes to validation
                on the client side. Typically if a property editor stores a primitive value (i.e. string) then the client side validation can easily be taken care of 
                with standard angular directives such as ng-required. However since some property editors store complex data such as JSON, a given property editor
                might require custom validation. This directive can be used to validate an Umbraco property in any way that a developer would like by specifying a
                callback method to perform the validation. The result of this method must return an object in the format of 
                {isValid: true, errorKey: 'required', errorMsg: 'Something went wrong' }
                The error message returned will also be displayed for the property level validation message.
                This directive should only be used when dealing with complex models, if custom validation needs to be performed with primitive values, use the simpler 
                angular validation directives instead since this will watch the entire model. 
**/
    function valPropertyValidator(serverValidationManager) {
        return {
            scope: { valPropertyValidator: '=' },
            // The element must have ng-model attribute and be inside an umbProperty directive
            require: [
                'ngModel',
                '?^umbProperty'
            ],
            restrict: 'A',
            link: function (scope, element, attrs, ctrls) {
                var modelCtrl = ctrls[0];
                var propCtrl = ctrls.length > 1 ? ctrls[1] : null;
                // Check whether the scope has a valPropertyValidator method 
                if (!scope.valPropertyValidator || !angular.isFunction(scope.valPropertyValidator)) {
                    throw new Error('val-property-validator directive must specify a function to call');
                }
                var initResult = scope.valPropertyValidator();
                // Validation method
                var validate = function (viewValue) {
                    // Calls the validition method
                    var result = scope.valPropertyValidator();
                    if (!result.errorKey || result.isValid === undefined || !result.errorMsg) {
                        throw 'The result object from valPropertyValidator does not contain required properties: isValid, errorKey, errorMsg';
                    }
                    if (result.isValid === true) {
                        // Tell the controller that the value is valid
                        modelCtrl.$setValidity(result.errorKey, true);
                        if (propCtrl) {
                            propCtrl.setPropertyError(null);
                        }
                    } else {
                        // Tell the controller that the value is invalid
                        modelCtrl.$setValidity(result.errorKey, false);
                        if (propCtrl) {
                            propCtrl.setPropertyError(result.errorMsg);
                        }
                    }
                };
                // Parsers are called as soon as the value in the form input is modified
                modelCtrl.$parsers.push(validate);
            }
        };
    }
    angular.module('umbraco.directives.validation').directive('valPropertyValidator', valPropertyValidator);
    /**
    * @ngdoc directive
    * @name umbraco.directives.directive:valRegex
    * @restrict A
    * @description A custom directive to allow for matching a value against a regex string.
    *               NOTE: there's already an ng-pattern but this requires that a regex expression is set, not a regex string
    **/
    function valRegex() {
        return {
            require: 'ngModel',
            restrict: 'A',
            link: function (scope, elm, attrs, ctrl) {
                var flags = '';
                var regex;
                var eventBindings = [];
                attrs.$observe('valRegexFlags', function (newVal) {
                    if (newVal) {
                        flags = newVal;
                    }
                });
                attrs.$observe('valRegex', function (newVal) {
                    if (newVal) {
                        try {
                            var resolved = newVal;
                            if (resolved) {
                                regex = new RegExp(resolved, flags);
                            } else {
                                regex = new RegExp(attrs.valRegex, flags);
                            }
                        } catch (e) {
                            regex = new RegExp(attrs.valRegex, flags);
                        }
                    }
                });
                eventBindings.push(scope.$watch('ngModel', function (newValue, oldValue) {
                    if (newValue && newValue !== oldValue) {
                        patternValidator(newValue);
                    }
                }));
                var patternValidator = function (viewValue) {
                    if (regex) {
                        //NOTE: we don't validate on empty values, use required validator for that
                        if (!viewValue || regex.test(viewValue.toString())) {
                            // it is valid
                            ctrl.$setValidity('valRegex', true);
                            //assign a message to the validator
                            ctrl.errorMsg = '';
                            return viewValue;
                        } else {
                            // it is invalid, return undefined (no model update)
                            ctrl.$setValidity('valRegex', false);
                            //assign a message to the validator
                            ctrl.errorMsg = 'Value is invalid, it does not match the correct pattern';
                            return undefined;
                        }
                    }
                };
                scope.$on('$destroy', function () {
                    // unbind watchers
                    for (var e in eventBindings) {
                        eventBindings[e]();
                    }
                });
            }
        };
    }
    angular.module('umbraco.directives.validation').directive('valRegex', valRegex);
    (function () {
        'use strict';
        function ValRequireComponentDirective() {
            function link(scope, el, attr, ngModel) {
                var unbindModelWatcher = scope.$watch(function () {
                    return ngModel.$modelValue;
                }, function (newValue) {
                    if (newValue === undefined || newValue === null || newValue === '') {
                        ngModel.$setValidity('valRequiredComponent', false);
                    } else {
                        ngModel.$setValidity('valRequiredComponent', true);
                    }
                });
                // clean up
                scope.$on('$destroy', function () {
                    unbindModelWatcher();
                });
            }
            var directive = {
                require: 'ngModel',
                restrict: 'A',
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('valRequireComponent', ValRequireComponentDirective);
    }());
    /**
    * @ngdoc directive
    * @name umbraco.directives.directive:valServer
    * @restrict A
    * @description This directive is used to associate a content property with a server-side validation response
    *               so that the validators in angular are updated based on server-side feedback.
    **/
    function valServer(serverValidationManager) {
        return {
            require: [
                'ngModel',
                '?^umbProperty'
            ],
            restrict: 'A',
            link: function (scope, element, attr, ctrls) {
                var modelCtrl = ctrls[0];
                var umbPropCtrl = ctrls.length > 1 ? ctrls[1] : null;
                if (!umbPropCtrl) {
                    //we cannot proceed, this validator will be disabled
                    return;
                }
                var watcher = null;
                //Need to watch the value model for it to change, previously we had  subscribed to 
                //modelCtrl.$viewChangeListeners but this is not good enough if you have an editor that
                // doesn't specifically have a 2 way ng binding. This is required because when we
                // have a server error we actually invalidate the form which means it cannot be 
                // resubmitted. So once a field is changed that has a server error assigned to it
                // we need to re-validate it for the server side validator so the user can resubmit
                // the form. Of course normal client-side validators will continue to execute.
                function startWatch() {
                    //if there's not already a watch
                    if (!watcher) {
                        watcher = scope.$watch(function () {
                            return modelCtrl.$modelValue;
                        }, function (newValue, oldValue) {
                            if (!newValue || angular.equals(newValue, oldValue)) {
                                return;
                            }
                            if (modelCtrl.$invalid) {
                                modelCtrl.$setValidity('valServer', true);
                                stopWatch();
                            }
                        }, true);
                    }
                }
                function stopWatch() {
                    if (watcher) {
                        watcher();
                        watcher = null;
                    }
                }
                var currentProperty = umbPropCtrl.property;
                //default to 'value' if nothing is set
                var fieldName = 'value';
                if (attr.valServer) {
                    fieldName = scope.$eval(attr.valServer);
                    if (!fieldName) {
                        //eval returned nothing so just use the string
                        fieldName = attr.valServer;
                    }
                }
                //subscribe to the server validation changes
                serverValidationManager.subscribe(currentProperty.alias, fieldName, function (isValid, propertyErrors, allErrors) {
                    if (!isValid) {
                        modelCtrl.$setValidity('valServer', false);
                        //assign an error msg property to the current validator
                        modelCtrl.errorMsg = propertyErrors[0].errorMsg;
                        startWatch();
                    } else {
                        modelCtrl.$setValidity('valServer', true);
                        //reset the error message
                        modelCtrl.errorMsg = '';
                        stopWatch();
                    }
                });
                //when the element is disposed we need to unsubscribe!
                // NOTE: this is very important otherwise when this controller re-binds the previous subscriptsion will remain
                // but they are a different callback instance than the above.
                element.bind('$destroy', function () {
                    stopWatch();
                    serverValidationManager.unsubscribe(currentProperty.alias, fieldName);
                });
            }
        };
    }
    angular.module('umbraco.directives.validation').directive('valServer', valServer);
    /**
    * @ngdoc directive
    * @name umbraco.directives.directive:valServerField
    * @restrict A
    * @description This directive is used to associate a content field (not user defined) with a server-side validation response
    *               so that the validators in angular are updated based on server-side feedback.
    **/
    function valServerField(serverValidationManager) {
        return {
            require: 'ngModel',
            restrict: 'A',
            link: function (scope, element, attr, ngModel) {
                var fieldName = null;
                var eventBindings = [];
                attr.$observe('valServerField', function (newVal) {
                    if (newVal && fieldName === null) {
                        fieldName = newVal;
                        //subscribe to the changed event of the view model. This is required because when we
                        // have a server error we actually invalidate the form which means it cannot be 
                        // resubmitted. So once a field is changed that has a server error assigned to it
                        // we need to re-validate it for the server side validator so the user can resubmit
                        // the form. Of course normal client-side validators will continue to execute.
                        eventBindings.push(scope.$watch(function () {
                            return ngModel.$modelValue;
                        }, function (newValue) {
                            if (ngModel.$invalid) {
                                ngModel.$setValidity('valServerField', true);
                            }
                        }));
                        //subscribe to the server validation changes
                        serverValidationManager.subscribe(null, fieldName, function (isValid, fieldErrors, allErrors) {
                            if (!isValid) {
                                ngModel.$setValidity('valServerField', false);
                                //assign an error msg property to the current validator
                                ngModel.errorMsg = fieldErrors[0].errorMsg;
                            } else {
                                ngModel.$setValidity('valServerField', true);
                                //reset the error message
                                ngModel.errorMsg = '';
                            }
                        });
                        //when the element is disposed we need to unsubscribe!
                        // NOTE: this is very important otherwise when this controller re-binds the previous subscriptsion will remain
                        // but they are a different callback instance than the above.
                        element.bind('$destroy', function () {
                            serverValidationManager.unsubscribe(null, fieldName);
                        });
                    }
                });
                scope.$on('$destroy', function () {
                    // unbind watchers
                    for (var e in eventBindings) {
                        eventBindings[e]();
                    }
                });
            }
        };
    }
    angular.module('umbraco.directives.validation').directive('valServerField', valServerField);
    /**
* @ngdoc directive
* @name umbraco.directives.directive:valSubView
* @restrict A
* @description Used to show validation warnings for a editor sub view to indicate that the section content has validation errors in its data.
* In order for this directive to work, the valFormManager directive must be placed on the containing form.
**/
    (function () {
        'use strict';
        function valSubViewDirective() {
            function link(scope, el, attr, ctrl) {
                //if there are no containing form or valFormManager controllers, then we do nothing
                if (!ctrl || !angular.isArray(ctrl) || ctrl.length !== 2 || !ctrl[0] || !ctrl[1]) {
                    return;
                }
                var valFormManager = ctrl[1];
                scope.subView.hasError = false;
                //listen for form validation changes
                valFormManager.onValidationStatusChanged(function (evt, args) {
                    if (!args.form.$valid) {
                        var subViewContent = el.find('.ng-invalid');
                        if (subViewContent.length > 0) {
                            scope.subView.hasError = true;
                        } else {
                            scope.subView.hasError = false;
                        }
                    } else {
                        scope.subView.hasError = false;
                    }
                });
            }
            var directive = {
                require: [
                    '?^form',
                    '?^valFormManager'
                ],
                restrict: 'A',
                link: link
            };
            return directive;
        }
        angular.module('umbraco.directives').directive('valSubView', valSubViewDirective);
    }());
    /**
* @ngdoc directive
* @name umbraco.directives.directive:valTab
* @restrict A
* @description Used to show validation warnings for a tab to indicate that the tab content has validations errors in its data.
* In order for this directive to work, the valFormManager directive must be placed on the containing form.
**/
    function valTab() {
        return {
            require: [
                '^form',
                '^valFormManager'
            ],
            restrict: 'A',
            link: function (scope, element, attr, ctrs) {
                var valFormManager = ctrs[1];
                var tabId = 'tab' + scope.tab.id;
                scope.tabHasError = false;
                //listen for form validation changes
                valFormManager.onValidationStatusChanged(function (evt, args) {
                    if (!args.form.$valid) {
                        var tabContent = element.closest('.umb-panel').find('#' + tabId);
                        //check if the validation messages are contained inside of this tabs 
                        if (tabContent.find('.ng-invalid').length > 0) {
                            scope.tabHasError = true;
                        } else {
                            scope.tabHasError = false;
                        }
                    } else {
                        scope.tabHasError = false;
                    }
                });
            }
        };
    }
    angular.module('umbraco.directives.validation').directive('valTab', valTab);
    function valToggleMsg(serverValidationManager) {
        return {
            require: '^form',
            restrict: 'A',
            /**
            Our directive requries a reference to a form controller which gets passed in to this parameter
         */
            link: function (scope, element, attr, formCtrl) {
                if (!attr.valToggleMsg) {
                    throw 'valToggleMsg requires that a reference to a validator is specified';
                }
                if (!attr.valMsgFor) {
                    throw 'valToggleMsg requires that the attribute valMsgFor exists on the element';
                }
                if (!formCtrl[attr.valMsgFor]) {
                    throw 'valToggleMsg cannot find field ' + attr.valMsgFor + ' on form ' + formCtrl.$name;
                }
                //if there's any remaining errors in the server validation service then we should show them.
                var showValidation = serverValidationManager.items.length > 0;
                var hasCustomMsg = element.contents().length > 0;
                //add a watch to the validator for the value (i.e. myForm.value.$error.required )
                scope.$watch(function () {
                    //sometimes if a dialog closes in the middle of digest we can get null references here
                    return formCtrl && formCtrl[attr.valMsgFor] ? formCtrl[attr.valMsgFor].$error[attr.valToggleMsg] : null;
                }, function () {
                    //sometimes if a dialog closes in the middle of digest we can get null references here
                    if (formCtrl && formCtrl[attr.valMsgFor]) {
                        if (formCtrl[attr.valMsgFor].$error[attr.valToggleMsg] && showValidation) {
                            element.show();
                            //display the error message if this element has no contents
                            if (!hasCustomMsg) {
                                element.html(formCtrl[attr.valMsgFor].errorMsg);
                            }
                        } else {
                            element.hide();
                        }
                    }
                });
                var unsubscribe = [];
                //listen for the saving event (the result is a callback method which is called to unsubscribe)
                unsubscribe.push(scope.$on('formSubmitting', function (ev, args) {
                    showValidation = true;
                    if (formCtrl[attr.valMsgFor].$error[attr.valToggleMsg]) {
                        element.show();
                        //display the error message if this element has no contents
                        if (!hasCustomMsg) {
                            element.html(formCtrl[attr.valMsgFor].errorMsg);
                        }
                    } else {
                        element.hide();
                    }
                }));
                //listen for the saved event (the result is a callback method which is called to unsubscribe)
                unsubscribe.push(scope.$on('formSubmitted', function (ev, args) {
                    showValidation = false;
                    element.hide();
                }));
                //when the element is disposed we need to unsubscribe!
                // NOTE: this is very important otherwise if this directive is part of a modal, the listener still exists because the dom 
                // element might still be there even after the modal has been hidden.
                element.bind('$destroy', function () {
                    for (var u in unsubscribe) {
                        unsubscribe[u]();
                    }
                });
            }
        };
    }
    /**
* @ngdoc directive
* @name umbraco.directives.directive:valToggleMsg
* @restrict A
* @element input
* @requires formController
* @description This directive will show/hide an error based on: is the value + the given validator invalid? AND, has the form been submitted ?
**/
    angular.module('umbraco.directives.validation').directive('valToggleMsg', valToggleMsg);
    angular.module('umbraco.directives.validation').directive('valTriggerChange', function ($sniffer) {
        return {
            link: function (scope, elem, attrs) {
                elem.bind('click', function () {
                    $(attrs.valTriggerChange).trigger($sniffer.hasEvent('input') ? 'input' : 'change');
                });
            },
            priority: 1
        };
    });
}());