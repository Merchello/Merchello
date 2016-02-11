/*! umbraco
 * https://github.com/umbraco/umbraco-cms/
 * Copyright (c) 2016 Umbraco HQ;
 * Licensed 
 */

(function() { 

angular.module("umbraco.directives", ["umbraco.directives.editors", "umbraco.directives.html", "umbraco.directives.validation", "ui.sortable"]);
angular.module("umbraco.directives.editors", []);
angular.module("umbraco.directives.html", []);
angular.module("umbraco.directives.validation", []);
/**
 * @ngdoc directive
 * @name umbraco.directives.directive:autoScale
 * @element div
 * @function
 *
 * @description
 * Resize div's automatically to fit to the bottom of the screen, as an optional parameter an y-axis offset can be set
 * So if you only want to scale the div to 70 pixels from the bottom you pass "70"
 *
 * @example
   <example module="umbraco.directives">
     <file name="index.html">
         <div auto-scale="70" class="input-block-level"></div>
     </file>
   </example>
 */
angular.module("umbraco.directives")
  .directive('autoScale', function ($window) {
    return function (scope, el, attrs) {

      var totalOffset = 0;
      var offsety = parseInt(attrs.autoScale, 10);
      var window = angular.element($window);
      if (offsety !== undefined){
        totalOffset += offsety;
      }

      setTimeout(function () {
        el.height(window.height() - (el.offset().top + totalOffset));
      }, 500);

      window.bind("resize", function () {
        el.height(window.height() - (el.offset().top + totalOffset));
      });

    };
  });
/**
* @ngdoc directive
* @name umbraco.directives.directive:umbPanel
* @description This is used for the editor buttons to ensure they are displayed correctly if the horizontal overflow of the editor
 * exceeds the height of the window
**/
angular.module("umbraco.directives.html")
	.directive('detectFold', function ($timeout, $log, windowResizeListener) {
	    return {
            require: "^?umbTabs",
			restrict: 'A',
			link: function (scope, el, attrs, tabsCtrl) {

			    var firstRun = false;
			    var parent = $(".umb-panel-body");
			    var winHeight = $(window).height();
			    var calculate = function () {
			        if (el && el.is(":visible") && !el.hasClass("umb-bottom-bar")) {

			            //now that the element is visible, set the flag in a couple of seconds, 
			            // this will ensure that loading time of a current tab get's completed and that
			            // we eventually stop watching to save on CPU time
			            $timeout(function() {
			                firstRun = true;
			            }, 4000);

			            //var parent = el.parent();
			            var hasOverflow = parent.innerHeight() < parent[0].scrollHeight;
			            //var belowFold = (el.offset().top + el.height()) > winHeight;
			            if (hasOverflow) {
			                el.addClass("umb-bottom-bar");

			                //I wish we didn't have to put this logic here but unfortunately we 
			                // do. This needs to calculate the left offest to place the bottom bar
			                // depending on if the left column splitter has been moved by the user
                            // (based on the nav-resize directive)
			                var wrapper = $("#mainwrapper");
			                var contentPanel = $("#leftcolumn").next();
			                var contentPanelLeftPx = contentPanel.css("left");

			                el.css({ left: contentPanelLeftPx });
			            }
			        }
			        return firstRun;
			    };

			    var resizeCallback = function(size) {
			        winHeight = size.height;
			        el.removeClass("umb-bottom-bar");
			        calculate();
			    };

			    windowResizeListener.register(resizeCallback);

			    //Only execute the watcher if this tab is the active (first) tab on load, otherwise there's no reason to execute
			    // the watcher since it will be recalculated when the tab changes!
                if (el.closest(".umb-tab-pane").index() === 0) {
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
			    scope.$on('$destroy', function() {
			        windowResizeListener.unregister(resizeCallback);			       
			    });
			}
		};
	});
/**
* @ngdoc directive
* @name umbraco.directives.directive:umbItemSorter
* @function
* @element ANY
* @restrict E
* @description A re-usable directive for sorting items
**/
function umbItemSorter(angularHelper) {
    return {
        scope: {
            model: "="
        },
        restrict: "E",    // restrict to an element
        replace: true,   // replace the html element with the template
        templateUrl: 'views/directives/_obsolete/umb-item-sorter.html',
        link: function(scope, element, attrs, ctrl) {
            var defaultModel = {
                okButton: "Ok",
                successMsg: "Sorting successful",
                complete: false
            };
            //assign user vals to default
            angular.extend(defaultModel, scope.model);
            //re-assign merged to user
            scope.model = defaultModel;

            scope.performSort = function() {
                scope.$emit("umbItemSorter.sorting", {
                    sortedItems: scope.model.itemsToSort
                });
            };

            scope.handleCancel = function () {
                scope.$emit("umbItemSorter.cancel");
            };

            scope.handleOk = function() {
                scope.$emit("umbItemSorter.ok");
            };

            //defines the options for the jquery sortable
            scope.sortableOptions = {
                axis: 'y',
                cursor: "move",
                placeholder: "ui-sortable-placeholder",
                update: function (ev, ui) {
                    //highlight the item when the position is changed
                    $(ui.item).effect("highlight", { color: "#049cdb" }, 500);
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

angular.module('umbraco.directives').directive("umbItemSorter", umbItemSorter);

/**
* @ngdoc directive
* @name umbraco.directives.directive:umbContentName
* @restrict E
* @function
* @description
* Used by editors that require naming an entity. Shows a textbox/headline with a required validator within it's own form.
**/
angular.module("umbraco.directives")
	.directive('umbContentName', function ($timeout, localizationService) {
	    return {
	        require: "ngModel",
			restrict: 'E',
			replace: true,
			templateUrl: 'views/directives/_obsolete/umb-content-name.html',
			
			scope: {
			    placeholder: '@placeholder',
			    model: '=ngModel',
                ngDisabled: '='
			},
			link: function(scope, element, attrs, ngModel) {

				var inputElement = element.find("input");
				if(scope.placeholder && scope.placeholder[0] === "@"){
					localizationService.localize(scope.placeholder.substring(1))
						.then(function(value){
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
				    if (!inputElement.is(":focus") && !inputElement.hasClass("ng-invalid")) {
				        // on page
				        if (mX >= inputElement.offset().left) {
				            distance = calculateDistance(inputElement, mX, mY);
				            if (distance <= 155) {

				                distance = 1 - (100 / 150 * distance / 100);
				                inputElement.css("border", "1px solid rgba(175,175,175, " + distance + ")");
				                inputElement.css("background-color", "rgba(255,255,255, " + distance + ")");
				            }
				        }

				    }

				}, 15);

				$(document).bind("mousemove", mouseMoveDebounce);

				$timeout(function(){
					if(!scope.model){
						scope.goEdit();
					}
				}, 100, false);

				scope.goEdit = function(){
					scope.editMode = true;

					$timeout(function () {
					    inputElement.focus();
					}, 100, false);
				};

				scope.exitEdit = function(){
					if(scope.model && scope.model !== ""){
						scope.editMode = false;
					}
				};

			    //unbind doc event!
				scope.$on('$destroy', function () {
				    $(document).unbind("mousemove", mouseMoveDebounce);
				});
			}
	    };
	});

/**
* @ngdoc directive
* @name umbraco.directives.directive:umbHeader
* @restrict E
* @function
* @description
* The header on an editor that contains tabs using bootstrap tabs - THIS IS OBSOLETE, use umbTabHeader instead
**/
angular.module("umbraco.directives")
.directive('umbHeader', function ($parse, $timeout) {
    return {
        restrict: 'E',
        replace: true,
        transclude: 'true',
        templateUrl: 'views/directives/_obsolete/umb-header.html',
        //create a new isolated scope assigning a tabs property from the attribute 'tabs'
        //which is bound to the parent scope property passed in
        scope: {
            tabs: "="
        },
        link: function (scope, iElement, iAttrs) {

            scope.showTabs = iAttrs.tabs ? true : false;
            scope.visibleTabs = [];

            //since tabs are loaded async, we need to put a watch on them to determine
            // when they are loaded, then we can close the watch
            var tabWatch = scope.$watch("tabs", function (newValue, oldValue) {

                angular.forEach(newValue, function(val, index){
                        var tab = {id: val.id, label: val.label};
                        scope.visibleTabs.push(tab);
                });

                //don't process if we cannot or have already done so
                if (!newValue) {return;}
                if (!newValue.length || newValue.length === 0){return;}

                //we need to do a timeout here so that the current sync operation can complete
                // and update the UI, then this will fire and the UI elements will be available.
                $timeout(function () {

                    //use bootstrap tabs API to show the first one
                    iElement.find(".nav-tabs a:first").tab('show');

                    //enable the tab drop
                    iElement.find('.nav-pills, .nav-tabs').tabdrop();

                    //ensure to destroy tabdrop (unbinds window resize listeners)
                    scope.$on('$destroy', function () {
                        iElement.find('.nav-pills, .nav-tabs').tabdrop("destroy");
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
* @name umbraco.directives.directive:login
* @function
* @element ANY
* @restrict E
**/
function loginDirective() {
    return {
        restrict: "E",    // restrict to an element
        replace: true,   // replace the html element with the template
        templateUrl: 'views/directives/_obsolete/umb-login.html'
    };
}

angular.module('umbraco.directives').directive("umbLogin", loginDirective);

angular.module("umbraco.directives")
.directive('umbOptionsMenu', function ($injector, treeService, navigationService, umbModelMapper, appState) {
    return {
        scope: {
            currentSection: "@",
            currentNode: "="
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
                appState.setMenuState("currentNode", scope.currentNode);

                if (!scope.actions) {
                    treeService.getMenu({ treeNode: scope.currentNode })
                        .then(function (data) {
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
* @restrict E
**/
angular.module("umbraco.directives.html")
    .directive('umbPhotoFolder', function($compile, $log, $timeout, $filter, umbPhotoFolderHelper) {

        return {
            restrict: 'E',
            replace: true,
            require: '?ngModel',
            terminate: true,
            templateUrl: 'views/directives/_obsolete/umb-photo-folder.html',
            link: function(scope, element, attrs, ngModel) {

                var lastWatch = null;

                ngModel.$render = function() {
                    if (ngModel.$modelValue) {

                        $timeout(function() {
                            var photos = ngModel.$modelValue;

                            scope.clickHandler = scope.$eval(element.attr('on-click'));


                            var imagesOnly =  element.attr('images-only') === "true";


                            var margin = element.attr('border') ? parseInt(element.attr('border'), 10) : 5;
                            var startingIndex = element.attr('baseline') ? parseInt(element.attr('baseline'), 10) : 0;
                            var minWidth = element.attr('min-width') ? parseInt(element.attr('min-width'), 10) : 420;
                            var minHeight = element.attr('min-height') ? parseInt(element.attr('min-height'), 10) : 100;
                            var maxHeight = element.attr('max-height') ? parseInt(element.attr('max-height'), 10) : 300;
                            var idealImgPerRow = element.attr('ideal-items-per-row') ? parseInt(element.attr('ideal-items-per-row'), 10) : 5;
                            var fixedRowWidth = Math.max(element.width(), minWidth);

                            scope.containerStyle = { width: fixedRowWidth + "px" };
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

                        }, 500); //end timeout
                    } //end if modelValue

                }; //end $render
            }
        };
    });

/**
 * @ngdoc directive
 * @name umbraco.directives.directive:umbSort
 * @element div
 * @function
 *
 * @description
 * Resize div's automatically to fit to the bottom of the screen, as an optional parameter an y-axis offset can be set
 * So if you only want to scale the div to 70 pixels from the bottom you pass "70"
 *
 * @example
   <example module="umbraco.directives">
     <file name="index.html">
         <div umb-sort="70" class="input-block-level"></div>
     </file>
   </example>
 */
angular.module("umbraco.directives")
  .value('umbSortContextInternal',{})
  .directive('umbSort', function($log,umbSortContextInternal) {
          return {
            require: '?ngModel',
            link: function(scope, element, attrs, ngModel) {
                var adjustment;
            
                var cfg = scope.$eval(element.attr('umb-sort')) || {};

                scope.model = ngModel;

                scope.opts = cfg;
                scope.opts.containerSelector= cfg.containerSelector || ".umb-" + cfg.group + "-container",
                scope.opts.nested= cfg.nested || true,
                scope.opts.drop= cfg.drop || true,
                scope.opts.drag= cfg.drag || true,
                scope.opts.clone = cfg.clone || "<li/>";
                scope.opts.mode = cfg.mode || "list";

                scope.opts.itemSelectorFull = $.trim(scope.opts.itemPath + " " + scope.opts.itemSelector);

                /*
                scope.opts.isValidTarget = function(item, container) {
                        if(container.el.is(".umb-" + scope.opts.group + "-container")){
                            return true;
                        }
                        return false;
                     };
                */

                element.addClass("umb-sort");
                element.addClass("umb-" + cfg.group + "-container");

                scope.opts.onDrag = function (item, position)  {
                    if(scope.opts.mode === "list"){
                      item.css({
                            left: position.left - adjustment.left,
                            top: position.top - adjustment.top
                          });  
                    }
                };


                scope.opts.onDrop = function (item, targetContainer, _super)  {
                      
                      if(scope.opts.mode === "list"){
                        //list mode
                        var clonedItem = $(scope.opts.clone).css({height: 0});
                        item.after(clonedItem);
                        clonedItem.animate({'height': item.height()});
                        
                        item.animate(clonedItem.position(), function  () {
                           clonedItem.detach();
                           _super(item);
                        });
                      }

                      var children = $(scope.opts.itemSelectorFull, targetContainer.el);
                      var targetIndex = children.index(item);
                      var targetScope = $(targetContainer.el[0]).scope();
                      

                      if(targetScope === umbSortContextInternal.sourceScope){
                          if(umbSortContextInternal.sourceScope.opts.onSortHandler){
                              var _largs = {
                                oldIndex: umbSortContextInternal.sourceIndex,
                                newIndex: targetIndex,
                                scope: umbSortContextInternal.sourceScope
                              };

                              umbSortContextInternal.sourceScope.opts.onSortHandler.call(this, item, _largs);
                          }
                      }else{
                        

                        if(targetScope.opts.onDropHandler){
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

                        if(umbSortContextInternal.sourceScope.opts.onReleaseHandler){
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

                scope.changeIndex = function(from, to){
                    scope.$apply(function(){
                      var i = ngModel.$modelValue.splice(from, 1)[0];
                      ngModel.$modelValue.splice(to, 0, i);
                    });
                };

                scope.move = function(args){
                    var from = args.sourceIndex;
                    var to = args.targetIndex;

                    if(args.sourceContainer === args.targetContainer){
                        scope.changeIndex(from, to);
                    }else{
                      scope.$apply(function(){
                        var i = args.sourceScope.model.$modelValue.splice(from, 1)[0];
                        args.targetScope.model.$modelvalue.splice(to,0, i);
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
                  
                element.sortable( scope.opts );
             }
          };

        });
/**
* @ngdoc directive
* @name umbraco.directives.directive:umbTabView
* @restrict E
**/
angular.module("umbraco.directives")
.directive('umbTabView', function($timeout, $log){
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
* @restrict E
**/
angular.module("umbraco.directives.html")
	.directive('umbUploadDropzone', function(){
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
angular.module("umbraco.directives")
    .directive('navResize', function (appState, eventsService, windowResizeListener) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs, ctrl) {

                var minScreenSize = 1100;
                var resizeEnabled = false;

                function setTreeMode() {
                    appState.setGlobalState("showNavigation", appState.getGlobalState("isTablet") === false);
                }

                function enableResize() {
                    //only enable when the size is correct and it's not already enabled
                    if (!resizeEnabled && appState.getGlobalState("isTablet") === false) {
                        element.resizable(
                        {
                            containment: $("#mainwrapper"),
                            autoHide: true,
                            handles: "e",
                            alsoResize: ".navigation-inner-container",
                            resize: function(e, ui) {
                                var wrapper = $("#mainwrapper");
                                var contentPanel = $("#contentwrapper");
                                var umbNotification = $("#umb-notifications-wrapper");
                                var apps = $("#applications");
                                var bottomBar = contentPanel.find(".umb-bottom-bar");
                                var navOffeset = $("#navOffset");

                                var leftPanelWidth = ui.element.width() + apps.width();

                                contentPanel.css({ left: leftPanelWidth });
                                bottomBar.css({ left: leftPanelWidth });
                                umbNotification.css({ left: leftPanelWidth });

                                navOffeset.css({ "margin-left": ui.element.outerWidth() });
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
                        element.resizable("destroy");
                        element.css("width", "");

                        var navInnerContainer = element.find(".navigation-inner-container");

                        navInnerContainer.css("width", "");
                        $("#contentwrapper").css("left", "");
                        $("#umb-notifications-wrapper").css("left", "");
                        $("#navOffset").css("margin-left", "");

                        resizeEnabled = false;
                    }
                }

                var evts = [];

                //Listen for global state changes
                evts.push(eventsService.on("appState.globalState.changed", function (e, args) {
                    if (args.key === "showNavigation") {
                        if (args.value === false) {
                            resetResize();
                        }
                        else {
                            enableResize();
                        }
                    }
                }));

                var resizeCallback = function(size) {
                    //set the global app state
                    appState.setGlobalState("isTablet", (size.width <= minScreenSize));
                    setTreeMode();
                };

                windowResizeListener.register(resizeCallback);

                //ensure to unregister from all events and kill jquery plugins
                scope.$on('$destroy', function () {
                    windowResizeListener.unregister(resizeCallback);
                    for (var e in evts) {
                        eventsService.unsubscribe(evts[e]);
                    }
                    var navInnerContainer = element.find(".navigation-inner-container");
                    navInnerContainer.resizable("destroy");
                });

                //init
                //set the global app state
                appState.setGlobalState("isTablet", ($(window).width() <= minScreenSize));
                setTreeMode();
            }
        };
    });

angular.module("umbraco.directives")
.directive('sectionIcon', function ($compile, iconHelper) {
    return {
        restrict: 'E',
        replace: true,

        link: function (scope, element, attrs) {

            var icon = attrs.icon;

            if (iconHelper.isLegacyIcon(icon)) {
                //its a known legacy icon, convert to a new one
                element.html("<i class='" + iconHelper.convertFromLegacyIcon(icon) + "'></i>");
            }
            else if (iconHelper.isFileBasedIcon(icon)) {
                var convert = iconHelper.convertFromLegacyImage(icon);
                if(convert){
                    element.html("<i class='icon-section " + convert + "'></i>");
                }else{
                    element.html("<img src='images/tray/" + icon + "'>");
                }
                //it's a file, normally legacy so look in the icon tray images
            }
            else {
                //it's normal
                element.html("<i class='icon-section " + icon + "'></i>");
            }
        }
    };
});
angular.module("umbraco.directives")
.directive('umbContextMenu', function (navigationService) {
    return {
        scope: {
            menuDialogTitle: "@",
            currentSection: "@",
            currentNode: "=",
            menuActions: "="
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
* @ngdoc directive
* @name umbraco.directives.directive:umbNavigation
* @restrict E
**/
function umbNavigationDirective() {
    return {
        restrict: "E",    // restrict to an element
        replace: true,   // replace the html element with the template
        templateUrl: 'views/components/application/umb-navigation.html'
    };
}

angular.module('umbraco.directives').directive("umbNavigation", umbNavigationDirective);

/**
* @ngdoc directive
* @name umbraco.directives.directive:umbSections
* @restrict E
**/
function sectionsDirective($timeout, $window, navigationService, treeService, sectionResource, appState, eventsService, $location) {
    return {
        restrict: "E",    // restrict to an element
        replace: true,   // replace the html element with the template
        templateUrl: 'views/components/application/umb-sections.html',
        link: function (scope, element, attr, ctrl) {

            //setup scope vars
			scope.maxSections = 7;
			scope.overflowingSections = 0;
            scope.sections = [];
            scope.currentSection = appState.getSectionState("currentSection");
            scope.showTray = false; //appState.getGlobalState("showTray");
            scope.stickyNavigation = appState.getGlobalState("stickyNavigation");
            scope.needTray = false;
            scope.trayAnimation = function() {
                if (scope.showTray) {
                    return 'slide';
                }
                else if (scope.showTray === false) {
                    return 'slide';
                }
                else {
                    return '';
                }
            };

			function loadSections(){
				sectionResource.getSections()
					.then(function (result) {
						scope.sections = result;
						calculateHeight();
					});
			}

			function calculateHeight(){
				$timeout(function(){
					//total height minus room for avatar and help icon
					var height = $(window).height()-200;
					scope.totalSections = scope.sections.length;
					scope.maxSections = Math.floor(height / 70);
					scope.needTray = false;

					if(scope.totalSections > scope.maxSections){
						scope.needTray = true;
						scope.overflowingSections = scope.maxSections - scope.totalSections;
					}
				});
			}

			var evts = [];

            //Listen for global state changes
            evts.push(eventsService.on("appState.globalState.changed", function(e, args) {
                if (args.key === "showTray") {
                    scope.showTray = args.value;
                }
                if (args.key === "stickyNavigation") {
                    scope.stickyNavigation = args.value;
                }
            }));

            evts.push(eventsService.on("appState.sectionState.changed", function(e, args) {
                if (args.key === "currentSection") {
                    scope.currentSection = args.value;
                }
            }));

            evts.push(eventsService.on("app.reInitialize", function(e, args) {
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

			scope.avatarClick = function(){

                if(scope.helpDialog) {
                    closeHelpDialog();
                }

                if(!scope.userDialog) {
                    scope.userDialog = {
                        view: "user",
                        show: true,
                        close: function(oldModel) {
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

			scope.helpClick = function(){

                if(scope.userDialog) {
                    closeUserDialog();
                }

                if(!scope.helpDialog) {
                    scope.helpDialog = {
                        view: "help",
                        show: true,
                        close: function(oldModel) {
                            closeHelpDialog();
                        }
                    };
                } else {
                    closeHelpDialog();
                }

			};

            function closeHelpDialog() {
                scope.helpDialog.show = false;
                scope.helpDialog = null;
            }

			scope.sectionClick = function (event, section) {

			    if (event.ctrlKey ||
			        event.shiftKey ||
			        event.metaKey || // apple
			        (event.button && event.button === 1) // middle click, >IE9 + everyone else
			    ) {
			        return;
			    }

                if (scope.userDialog) {
                    closeUserDialog();
			    }
			    if (scope.helpDialog) {
                    closeHelpDialog();
			    }

			    navigationService.hideSearch();
			    navigationService.showTree(section.alias);
			    $location.path("/" + section.alias);
			};

			scope.sectionDblClick = function(section){
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

			    if (appState.getGlobalState("showTray") === true) {
			        navigationService.hideTray();
			    } else {
			        navigationService.showTray();
			    }
			};

			loadSections();

        }
    };
}

angular.module('umbraco.directives').directive("umbSections", sectionsDirective);

(function() {
   'use strict';

   function ButtonDirective($timeout) {

      function link(scope, el, attr, ctrl) {

         scope.style = null;

         function activate() {

            if (!scope.state) {
               scope.state = "init";
            }

            if (scope.buttonStyle) {
               scope.style = "btn-" + scope.buttonStyle;
            }

         }

         activate();

         var unbindStateWatcher = scope.$watch('state', function(newValue, oldValue) {

            if (newValue === 'success' || newValue === 'error') {
               $timeout(function() {
                  scope.state = 'init';
               }, 2000);
            }

         });

         scope.$on('$destroy', function() {
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
            action: "&?",
            href: "@?",
            type: "@",
            buttonStyle: "@?",
            state: "=?",
            shortcut: "@?",
            shortcutWhenHidden: "@",
            label: "@?",
            labelKey: "@?",
            icon: "@?",
            disabled: "="
         }
      };

      return directive;

   }

   angular.module('umbraco.directives').directive('umbButton', ButtonDirective);

})();

(function() {
   'use strict';

   function ButtonGroupDirective() {

      var directive = {
         restrict: 'E',
         replace: true,
         templateUrl: 'views/components/buttons/umb-button-group.html',
         scope: {
            defaultButton: "=",
            subButtons: "=",
            state: "=?",
            direction: "@?",
            float: "@?"
         }
      };

      return directive;
   }

   angular.module('umbraco.directives').directive('umbButtonGroup', ButtonGroupDirective);

})();

(function() {
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

})();

(function() {
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

})();

(function() {
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

})();

(function() {
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

})();

(function() {
  'use strict';

  function BreadcrumbsDirective() {

    var directive = {
      restrict: 'E',
      replace: true,
      templateUrl: 'views/components/editor/umb-breadcrumbs.html',
      scope: {
        ancestors: "=",
        entityType: "@"
      }
    };

    return directive;

  }

  angular.module('umbraco.directives').directive('umbBreadcrumbs', BreadcrumbsDirective);

})();

(function() {
   'use strict';

   function EditorContainerDirective(overlayHelper) {

      function link(scope, el, attr, ctrl) {

         scope.numberOfOverlays = 0;

         scope.$watch(function(){
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

})();

(function() {
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

})();

(function() {
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

})();

(function() {
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

})();

(function() {
    'use strict';

    function EditorHeaderDirective(iconHelper) {

        function link(scope, el, attr, ctrl) {

            scope.openIconPicker = function() {
                scope.dialogModel = {
                    view: "iconpicker",
                    show: true,
                    submit: function(model) {
                        if (model.color) {
                            scope.icon = model.icon + " " + model.color;
                        } else {
                            scope.icon = model.icon;
                        }

                        // set form to dirty
                        ctrl.$setDirty();

                        scope.dialogModel.show = false;
                        scope.dialogModel = null;
                    }
                };
            };
        }

        var directive = {
            require: '^form',
            transclude: true,
            restrict: 'E',
            replace: true,
            templateUrl: 'views/components/editor/umb-editor-header.html',
            scope: {
                tabs: "=",
                actions: "=",
                name: "=",
                nameLocked: "=",
                menu: "=",
                icon: "=",
                hideIcon: "@",
                alias: "=",
                hideAlias: "@",
                description: "=",
                hideDescription: "@",
                navigation: "="
            },
            link: link
        };

        return directive;
    }

    angular.module('umbraco.directives').directive('umbEditorHeader', EditorHeaderDirective);

})();

(function() {
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
             appState.setMenuState("currentNode", scope.currentNode);

             if (!scope.actions) {
                 treeService.getMenu({ treeNode: scope.currentNode })
                     .then(function (data) {
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
            currentNode: "=",
            currentSection: "@"
         }
      };

      return directive;
   }

   angular.module('umbraco.directives').directive('umbEditorMenu', EditorMenuDirective);

})();

(function() {
   'use strict';

   function EditorNavigationDirective() {

      function link(scope, el, attr, ctrl) {

         scope.showNavigation = true;

         scope.clickNavigationItem = function(selectedItem) {
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
         scope: {
            navigation: "="
         },
         link: link
      };

      return directive;
   }

   angular.module('umbraco.directives.html').directive('umbEditorNavigation', EditorNavigationDirective);

})();

(function() {
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
         scope.$watch('subViews', function(newValue, oldValue) {
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
            subViews: "=",
            model: "="
         },
         link: link
      };

      return directive;
   }

   angular.module('umbraco.directives').directive('umbEditorSubViews', EditorSubViewsDirective);

})();

(function() {
   'use strict';

   function EditorViewDirective() {

       function link(scope, el, attr) {

           if(attr.footer) {
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

})();

/**
* @description Utillity directives for key and field events
**/
angular.module('umbraco.directives')

.directive('onKeyup', function () {
    return {
        link: function (scope, elm, attrs) {
            var f = function () {
                scope.$apply(attrs.onKeyup);
            };
            elm.on("keyup", f);
            scope.$on("$destroy", function(){ elm.off("keyup", f);} );
        }
    };
})

.directive('onKeydown', function () {
    return {
        link: function (scope, elm, attrs) {
            var f = function () {
                scope.$apply(attrs.onKeydown);
            };
            elm.on("keydown", f);
            scope.$on("$destroy", function(){ elm.off("keydown", f);} );
        }
    };
})

.directive('onBlur', function () {
    return {
        link: function (scope, elm, attrs) {
            var f = function () {
                scope.$apply(attrs.onBlur);
            };
            elm.on("blur", f);
            scope.$on("$destroy", function(){ elm.off("blur", f);} );
        }
    };
})

.directive('onFocus', function () {
    return {
        link: function (scope, elm, attrs) {
            var f = function () {
                scope.$apply(attrs.onFocus);
            };
            elm.on("focus", f);
            scope.$on("$destroy", function(){ elm.off("focus", f);} );
        }
    };
})

.directive('onDragEnter', function () {
    return {
        link: function (scope, elm, attrs) {
            var f = function () {
                scope.$apply(attrs.onDragEnter);
            };
            elm.on("dragenter", f);
            scope.$on("$destroy", function(){ elm.off("dragenter", f);} );
        }
    };
})

.directive('onDragLeave', function () {
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

                return { x: x, y : y };
            };

            var e = getXY(event.originalEvent);

            // Check the mouseEvent coordinates are outside of the rectangle
            if (e.x > rect.left + rect.width - 1 || e.x < rect.left || e.y > rect.top + rect.height - 1 || e.y < rect.top) {
                scope.$apply(attrs.onDragLeave);
            }
        };

        elm.on("dragleave", f);
        scope.$on("$destroy", function(){ elm.off("dragleave", f);} );
    };
})

.directive('onDragOver', function () {
    return {
        link: function (scope, elm, attrs) {
            var f = function () {
                scope.$apply(attrs.onDragOver);
            };
            elm.on("dragover", f);
            scope.$on("$destroy", function(){ elm.off("dragover", f);} );
        }
    };
})

.directive('onDragStart', function () {
    return {
        link: function (scope, elm, attrs) {
            var f = function () {
                scope.$apply(attrs.onDragStart);
            };
            elm.on("dragstart", f);
            scope.$on("$destroy", function(){ elm.off("dragstart", f);} );
        }
    };
})

.directive('onDragEnd', function () {
    return {
        link: function (scope, elm, attrs) {
            var f = function () {
                scope.$apply(attrs.onDragEnd);
            };
            elm.on("dragend", f);
            scope.$on("$destroy", function(){ elm.off("dragend", f);} );
        }
    };
})

.directive('onDrop', function () {
    return {
        link: function (scope, elm, attrs) {
            var f = function () {
                scope.$apply(attrs.onDrop);
            };
            elm.on("drop", f);
            scope.$on("$destroy", function(){ elm.off("drop", f);} );
        }
    };
})

.directive('onOutsideClick', function ($timeout) {
    return function (scope, element, attrs) {

        var eventBindings = [];

        function oneTimeClick(event) {
                var el = event.target.nodeName;

                //ignore link and button clicks
                var els = ["INPUT","A","BUTTON"];
                if(els.indexOf(el) >= 0){return;}

                // ignore children of links and buttons
                // ignore clicks on new overlay
                var parents = $(event.target).parents("a,button,.umb-overlay");
                if(parents.length > 0){
                    return;
                }

                // ignore clicks on dialog from old dialog service
                var oldDialog = $(el).parents("#old-dialog-service");
                if (oldDialog.length === 1) {
                    return;
                }

                // ignore clicks in tinyMCE dropdown(floatpanel)
                var floatpanel = $(el).parents(".mce-floatpanel");
                if (floatpanel.length === 1) {
                    return;
                }

                //ignore clicks inside this element
                if( $(element).has( $(event.target) ).length > 0 ){
                    return;
                }

                scope.$apply(attrs.onOutsideClick);
        }


        $timeout(function(){

            if ("bindClickOn" in attrs) {

                eventBindings.push(scope.$watch(function() {
                    return attrs.bindClickOn;
                }, function(newValue) {
                    if (newValue === "true") {
                        $(document).on("click", oneTimeClick);
                    } else {
                        $(document).off("click", oneTimeClick);
                    }
                }));

            } else {
                $(document).on("click", oneTimeClick);
            }

            scope.$on("$destroy", function() {
                $(document).off("click", oneTimeClick);

                // unbind watchers
                for (var e in eventBindings) {
                    eventBindings[e]();
                }

            });
        }); // Temp removal of 1 sec timeout to prevent bug where overlay does not open. We need to find a better solution.

    };
})

.directive('onRightClick',function(){

    document.oncontextmenu = function (e) {
       if(e.target.hasAttribute('on-right-click')) {
           e.preventDefault();
           e.stopPropagation();
           return false;
       }
    };

    return function(scope,el,attrs){
        el.on('contextmenu',function(e){
            e.preventDefault();
            e.stopPropagation();
            scope.$apply(attrs.onRightClick);
            return false;
        });
    };
})

.directive('onDelayedMouseleave', function ($timeout, $parse) {
        return {

            restrict: 'A',

            link: function (scope, element, attrs, ctrl) {
                var active = false;
                var fn = $parse(attrs.onDelayedMouseleave);

                var leave_f = function(event) {
                    var callback = function() {
                        fn(scope, {$event:event});
                    };

                    active = false;
                    $timeout(function(){
                        if(active === false){
                            scope.$apply(callback);
                        }
                    }, 650);
                };

                var enter_f = function(event, args){
                    active = true;
                };


                element.on("mouseleave", leave_f);
                element.on("mouseenter", enter_f);

                //unsub events
                scope.$on("$destroy", function(){
                    element.off("mouseleave", leave_f);
                    element.off("mouseenter", enter_f);
                });
            }
        };
    });

/*
  
  http://vitalets.github.io/checklist-model/
  <label ng-repeat="role in roles">
    <input type="checkbox" checklist-model="user.roles" checklist-value="role.id"> {{role.text}}
  </label>
*/
angular.module('umbraco.directives')
.directive('checklistModel', ['$parse', '$compile', function($parse, $compile) {
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

  // http://stackoverflow.com/a/19228302/1458162
  function postLinkFn(scope, elem, attrs) {
    // compile with `ng-model` pointing to `checked`
    $compile(elem)(scope);

    // getter / setter for original model
    var getter = $parse(attrs.checklistModel);
    var setter = getter.assign;

    // value added to list
    var value = $parse(attrs.checklistValue)(scope.$parent);

    // watch UI checked change
    scope.$watch('checked', function(newValue, oldValue) {
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
    scope.$parent.$watch(attrs.checklistModel, function(newArr, oldArr) {
      scope.checked = contains(newArr, value);
    }, true);
  }

  return {
    restrict: 'A',
    priority: 1000,
    terminal: true,
    scope: true,
    compile: function(tElement, tAttrs) {
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
}]);
angular.module("umbraco.directives")
.directive("contenteditable", function() {
  
  return {
    require: "ngModel",
    link: function(scope, element, attrs, ngModel) {

      function read() {
        ngModel.$setViewValue(element.html());
      }

      ngModel.$render = function() {
        element.html(ngModel.$viewValue || "");
      };

      
      element.bind("focus", function(){
          
          var range = document.createRange();
          range.selectNodeContents(element[0]);

          var sel = window.getSelection();
          sel.removeAllRanges();
          sel.addRange(range);

      });

      element.bind("blur keyup change", function() {
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
        restrict: "A",
        require: "ngModel",

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
angular.module('umbraco.directives').directive("fixNumber", fixNumber);
angular.module("umbraco.directives").directive('focusWhen', function ($timeout) {
    return {
        restrict: 'A',
        link: function (scope, elm, attrs, ctrl) {
            attrs.$observe("focusWhen", function (newValue) {
                if (newValue === "true") {
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
        restrict: "A",
        link: function (scope, element, attr, formCtrl) {

            var origColor = null;
            if (attr.hexBgOrig) {
                //set the orig based on the attribute if there is one
                origColor = attr.hexBgOrig;
            }
            
            attr.$observe("hexBgColor", function (newVal) {
                if (newVal) {
                    if (!origColor) {
                        //get the orig color before changing it
                        origColor = element.css("border-color");
                    }
                    //validate it - test with and without the leading hash.
                    if (/^([0-9a-f]{3}|[0-9a-f]{6})$/i.test(newVal)) {
                        element.css("background-color", "#" + newVal);
                        return;
                    }
                    if (/^#([0-9a-f]{3}|[0-9a-f]{6})$/i.test(newVal)) {
                        element.css("background-color", newVal);
                        return;
                    }
                }
                element.css("background-color", origColor);
            });

        }
    };
}
angular.module('umbraco.directives').directive("hexBgColor", hexBgColor);
/**
 * @ngdoc directive
 * @name umbraco.directives.directive:headline
 **/
angular.module("umbraco.directives")
    .directive('hotkey', function($window, keyboardService, $log) {

        return function(scope, el, attrs) {

            var options = {};
            var keyCombo = attrs.hotkey;

            if (!keyCombo) {
                //support data binding
                keyCombo = scope.$eval(attrs["hotkey"]);
            }

            function activate() {

                if (keyCombo) {

                    // disable shortcuts in input fields if keycombo is 1 character
                    if (keyCombo.length === 1) {
                        options = {
                            inputDisabled: true
                        };
                    }

                    keyboardService.bind(keyCombo, function() {

                        var element = $(el);
                        var activeElementType = document.activeElement.tagName;
                        var clickableElements = ["A", "BUTTON"];

                        if (element.is("a,div,button,input[type='button'],input[type='submit'],input[type='checkbox']") && !element.is(':disabled')) {

                            if (element.is(':visible') || attrs.hotkeyWhenHidden) {

                                if (attrs.hotkeyWhen && attrs.hotkeyWhen === "false") {
                                    return;
                                }

                                // when keycombo is enter and a link or button has focus - click the link or button instead of using the hotkey
                                if (keyCombo === "enter" && clickableElements.indexOf(activeElementType) === 0) {
                                    document.activeElement.click();
                                } else {
                                    element.click();
                                }

                            }

                        } else {
                            element.focus();
                        }

                    }, options);

                    el.on('$destroy', function() {
                        keyboardService.unbind(keyCombo);
                    });

                }

            }

            activate();

        };
    });

/**
* @ngdoc directive
* @name umbraco.directives.directive:preventDefault
**/
angular.module("umbraco.directives")
    .directive('preventDefault', function() {
        return function(scope, element, attrs) {

            var enabled = true;
            //check if there's a value for the attribute, if there is and it's false then we conditionally don't 
            //prevent default.
            if (attrs.preventDefault) {
                attrs.$observe("preventDefault", function (newVal) {
                    enabled = (newVal === "false" || newVal === 0 || newVal === false) ? false : true;
                });
            }

            $(element).click(function (event) {
                if (event.metaKey || event.ctrlKey) {
                    return;
                }
                else {
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
angular.module("umbraco.directives")
    .directive('preventEnterSubmit', function() {
        return function(scope, element, attrs) {

            var enabled = true;
            //check if there's a value for the attribute, if there is and it's false then we conditionally don't 
            //prevent default.
            if (attrs.preventEnterSubmit) {
                attrs.$observe("preventEnterSubmit", function (newVal) {
                    enabled = (newVal === "false" || newVal === 0 || newVal === false) ? false : true;
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
angular.module("umbraco.directives")
  .directive('resizeToContent', function ($window, $timeout) {
    return function (scope, el, attrs) {
       var iframe = el[0];
       var iframeWin = iframe.contentWindow || iframe.contentDocument.parentWindow;
       if (iframeWin.document.body) {

          $timeout(function(){
              var height = iframeWin.document.documentElement.scrollHeight || iframeWin.document.body.scrollHeight;
              el.height(height);
          }, 3000);
       }
    };
  });

angular.module("umbraco.directives")
  .directive('selectOnFocus', function () {
    return function (scope, el, attrs) {
        $(el).bind("click", function () {
            var editmode = $(el).data("editmode");
            //If editmode is true a click is handled like a normal click
            if (!editmode) {
                //Initial click, select entire text
                this.select();
                //Set the edit mode so subsequent clicks work normally
                $(el).data("editmode", true);
            }
        }).
        bind("blur", function () {
            //Reset on focus lost
            $(el).data("editmode", false);
        });
    };
  });

angular.module("umbraco.directives")
    .directive('umbAutoFocus', function($timeout) {

        return function(scope, element, attr){
            var update = function() {
                //if it uses its default naming
                if(element.val() === "" || attr.focusOnFilled){
                    element.focus();
                }
            };

            $timeout(function() {
                update();
            });
    };
});

angular.module("umbraco.directives")
   .directive('umbAutoResize', function($timeout) {
      return {
         require: ["^?umbTabs", "ngModel"],
         link: function(scope, element, attr, controllersArr) {

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
               var msie = ua.indexOf("MSIE ");

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
               if (isIEFlag === true && domElType === "text") {
                  setupInternetExplorerElements();
               }

            }

            function setupInternetExplorerElements() {

               if (!wrapper.length) {
                  wrapper = angular.element('<div id="umb-ie-resize-input-wrapper" style="position:fixed; top:-999px; left:0;"></div>');
                  angular.element('body').append(wrapper);
               }

               angular.forEach(['fontFamily', 'fontSize', 'fontWeight', 'fontStyle',
                  'letterSpacing', 'textTransform', 'wordSpacing', 'textIndent',
                  'boxSizing', 'borderRightWidth', 'borderLeftWidth', 'borderLeftStyle', 'borderRightStyle',
                  'paddingLeft', 'paddingRight', 'marginLeft', 'marginRight'
               ], function(value) {
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

               if(!ngModelController.$modelValue && attr.placeholder) {
                  attr.$set('size', attr.placeholder.length);
                  element.width('auto');
               }

            }

            function resizeTextarea() {

               if(domEl.scrollHeight !== domEl.clientHeight) {

                  element.height(domEl.scrollHeight);

               }

            }

            var update = function(force) {


               if (force === true) {

                  if (domElType === "textarea") {
                     element.height(0);
                  } else if (domElType === "text") {
                     element.width(0);
                  }

               }


               if (isIEFlag === true && domElType === "text") {

                  resizeInternetExplorerInput();

               } else {

                  if (domElType === "textarea") {

                     resizeTextarea();

                  } else if (domElType === "text") {

                     resizeInput();

                  }

               }

            };

            activate();

            //listen for tab changes
            if (umbTabsController != null) {
               umbTabsController.onTabShown(function(args) {
                  update();
               });
            }

            // listen for ng-model changes
            var unbindModelWatcher = scope.$watch(function() {
               return ngModelController.$modelValue;
            }, function(newValue) {
               update(true);
            });

            scope.$on('$destroy', function() {
               element.unbind('keyup keydown keypress change', update);
               element.unbind('blur', update(true));
               unbindModelWatcher();

               // clean up IE dom element
               if (isIEFlag === true && domElType === "text") {
                  mirror.remove();
               }

            });
         }
      };
   });

/*
example usage: <textarea json-edit="myObject" rows="8" class="form-control"></textarea>

jsonEditing is a string which we edit in a textarea. we try parsing to JSON with each change. when it is valid, propagate model changes via ngModelCtrl

use isolate scope to prevent model propagation when invalid - will update manually. cannot replace with template, or will override ngModelCtrl, and not hide behind facade

will override element type to textarea and add own attribute ngModel tied to jsonEditing
 */

angular.module("umbraco.directives")
	.directive('umbRawModel', function () {
		return {
			restrict: 'A',
			require: 'ngModel',
			template: '<textarea ng-model="jsonEditing"></textarea>',
			replace : true,
			scope: {
				model: '=umbRawModel',
				validateOn:'='
			},
			link: function (scope, element, attrs, ngModelCtrl) {

				function setEditing (value) {
					scope.jsonEditing = angular.copy( jsonToString(value));
				}

				function updateModel (value) {
					scope.model = stringToJson(value);
				}

				function setValid() {
					ngModelCtrl.$setValidity('json', true);
				}

				function setInvalid () {
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

				var onInputChange = function(newval,oldval){
					if (newval !== oldval) {
						if (isValidJson(newval)) {
							setValid();
							updateModel(newval);
						} else {
							setInvalid();
						}
					}
				};

				if(scope.validateOn){
					element.on(scope.validateOn, function(){
						scope.$apply(function(){
							onInputChange(scope.jsonEditing);
						});
					});
				}else{
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

(function() {
    'use strict';

    function SelectWhen($timeout) {

        function link(scope, el, attr, ctrl) {

            attr.$observe("umbSelectWhen", function(newValue) {
                if (newValue === "true") {
                    $timeout(function() {
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

})();

angular.module("umbraco.directives")
    .directive('gridRte', function (tinyMceService, stylesheetResource, angularHelper, assetsService, $q, $timeout) {
        return {
            scope: {
                uniqueId: '=',
                value: '=',
                onClick: '&',
                onFocus: '&',
                onBlur: '&',
                configuration:"=",
                onMediaPickerClick: "=",
                onEmbedClick: "=",
                onMacroPickerClick: "=",
                onLinkPickerClick: "="
            },
            template: "<textarea ng-model=\"value\" rows=\"10\" class=\"mceNoEditor\" style=\"overflow:hidden\" id=\"{{uniqueId}}\"></textarea>",
            replace: true,
            link: function (scope, element, attrs) {

                var initTiny = function () {

                    //we always fetch the default one, and then override parts with our own
                    tinyMceService.configuration().then(function (tinyMceConfig) {



                        //config value from general tinymce.config file
                        var validElements = tinyMceConfig.validElements;
                        var fallbackStyles = [{title: "Page header", block: "h2"}, {title: "Section header", block: "h3"}, {title: "Paragraph header", block: "h4"}, {title: "Normal", block: "p"}, {title: "Quote", block: "blockquote"}, {title: "Code", block: "code"}];

                        //These are absolutely required in order for the macros to render inline
                        //we put these as extended elements because they get merged on top of the normal allowed elements by tiny mce
                        var extendedValidElements = "@[id|class|style],-div[id|dir|class|align|style],ins[datetime|cite],-ul[class|style],-li[class|style],-h1[id|dir|class|align|style],-h2[id|dir|class|align|style],-h3[id|dir|class|align|style],-h4[id|dir|class|align|style],-h5[id|dir|class|align|style],-h6[id|style|dir|class|align],span[id|class|style]";

                        var invalidElements = tinyMceConfig.inValidElements;
                        var plugins = _.map(tinyMceConfig.plugins, function (plugin) {
                            if (plugin.useOnFrontend) {
                                return plugin.name;
                            }
                        }).join(" ") + " autoresize";

                        //config value on the data type
                        var toolbar = ["code", "styleselect", "bold", "italic", "alignleft", "aligncenter", "alignright", "bullist", "numlist", "link", "umbmediapicker", "umbembeddialog"].join(" | ");
                        var stylesheets = [];

                        var styleFormats = [];
                        var await = [];

                        //queue file loading
                        if (typeof (tinymce) === "undefined") {
                                await.push(assetsService.loadJs("lib/tinymce/tinymce.min.js", scope));
                        }


                        if(scope.configuration && scope.configuration.toolbar){
                            toolbar = scope.configuration.toolbar.join(' | ');
                        }


                        if(scope.configuration && scope.configuration.stylesheets){
                            angular.forEach(scope.configuration.stylesheets, function(stylesheet, key){

                                    stylesheets.push(Umbraco.Sys.ServerVariables.umbracoSettings.cssPath + "/" + stylesheet + ".css");
                                    await.push(stylesheetResource.getRulesByName(stylesheet).then(function (rules) {
                                        angular.forEach(rules, function (rule) {
                                          var r = {};
                                          r.title = rule.name;
                                          if (rule.selector[0] === ".") {
                                              r.inline = "span";
                                              r.classes = rule.selector.substring(1);
                                          }else if (rule.selector[0] === "#") {
                                              //Even though this will render in the style drop down, it will not actually be applied
                                              // to the elements, don't think TinyMCE even supports this and it doesn't really make much sense
                                              // since only one element can have one id.
                                              r.inline = "span";
                                              r.attributes = { id: rule.selector.substring(1) };
                                          }else {
                                              r.block = rule.selector;
                                          }
                                          styleFormats.push(r);
                                        });
                                    }));
                            });
                        }else{
                            stylesheets.push("views/propertyeditors/grid/config/grid.default.rtestyles.css");
                            styleFormats = fallbackStyles;
                        }

                        //stores a reference to the editor
                        var tinyMceEditor = null;
                        $q.all(await).then(function () {

                            var uniqueId = scope.uniqueId;

                            //create a baseline Config to exten upon
                            var baseLineConfigObj = {
                                mode: "exact",
                                skin: "umbraco",
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
                                autoresize_bottom_margin: 0
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
                                            }
                                            catch (e) {
                                                //cannot parse, we'll just leave it
                                            }
                                        }
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
                                    editor.getBody().style.overflow = "hidden";

                                    $timeout(function(){
                                        if(scope.value === null){
                                            editor.focus();
                                        }
                                    }, 400);

                                });

                                //when we leave the editor (maybe)
                                editor.on('blur', function (e) {
                                    editor.save();
                                    angularHelper.safeApply(scope, function () {
                                        scope.value = editor.getContent();

                                        var _toolbar = $(editor.editorContainer)
                                             .find(".mce-toolbar");

                                        if(scope.onBlur){
                                            scope.onBlur();
                                        }

                                    });
                                });

                                // Focus on editor
                                editor.on('focus', function (e) {
                                    angularHelper.safeApply(scope, function () {

                                        if(scope.onFocus){
                                            scope.onFocus();
                                        }

                                    });
                                });

                                // Click on editor
                                editor.on('click', function (e) {
                                    angularHelper.safeApply(scope, function () {

                                        if(scope.onClick){
                                            scope.onClick();
                                        }

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
                                    var qs = "?width=" + e.width + "&height=" + e.height;
                                    var srcAttr = $(e.target).attr("src");
                                    var path = srcAttr.split("?")[0];
                                    $(e.target).attr("data-mce-src", path + qs);
                                });

                                //Create the insert link plugin
                                tinyMceService.createLinkPicker(editor, scope, function(currentTarget, anchorElement){
                                    if(scope.onLinkPickerClick) {
                                        scope.onLinkPickerClick(editor, currentTarget, anchorElement);
                                    }
                                });

                                //Create the insert media plugin
                                tinyMceService.createMediaPicker(editor, scope, function(currentTarget, userData){
                                    if(scope.onMediaPickerClick) {
                                        scope.onMediaPickerClick(editor, currentTarget, userData);
                                    }
                                });

                                //Create the embedded plugin
                                tinyMceService.createInsertEmbeddedMedia(editor, scope, function(){
                                    if(scope.onEmbedClick) {
                                        scope.onEmbedClick(editor);
                                    }
                                });

                                //Create the insert macro plugin
                                tinyMceService.createInsertMacro(editor, scope, function(dialogData){
                                    if(scope.onMacroPickerClick) {
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

                            //listen for formSubmitting event (the result is callback used to remove the event subscription)
                            var unsubscribe = scope.$on("formSubmitting", function () {
                                //TODO: Here we should parse out the macro rendered content so we can save on a lot of bytes in data xfer
                                // we do parse it out on the server side but would be nice to do that on the client side before as well.
                                scope.value = tinyMceEditor.getContent();
                            });

                            //when the element is disposed we need to unsubscribe!
                            // NOTE: this is very important otherwise if this is part of a modal, the listener still exists because the dom
                            // element might still be there even after the modal has been hidden.
                            scope.$on('$destroy', function () {
                                unsubscribe();
                            });

                        });

                    });

                };

                initTiny();

            }
        };
    });

/**
* @ngdoc directive
* @name umbraco.directives.directive:umbProperty
* @restrict E
**/
angular.module("umbraco.directives.html")
    .directive('umbControlGroup', function (localizationService) {
        return {
            scope: {
                label: "@label",
                description: "@",
                hideLabel: "@",
                alias: "@"
            },
            require: '?^form',
            transclude: true,
            restrict: 'E',
            replace: true,
            templateUrl: 'views/components/html/umb-control-group.html',
            link: function (scope, element, attr, formCtrl) {

                scope.formValid = function() {
                    if (formCtrl) {
                        return formCtrl.$valid;
                    }
                    //there is no form.
                    return true;
                };

                if (scope.label && scope.label[0] === "@") {
                    scope.labelstring = localizationService.localize(scope.label.substring(1));
                }
                else {
                    scope.labelstring = scope.label;
                }

                if (scope.description && scope.description[0] === "@") {
                    scope.descriptionstring = localizationService.localize(scope.description.substring(1));
                }
                else {
                    scope.descriptionstring = scope.description;
                }

            }
        };
    });

/**
* @ngdoc directive
* @name umbraco.directives.directive:umbProperty
* @restrict E
**/
angular.module("umbraco.directives.html")
    .directive('umbPane', function () {
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
angular.module("umbraco.directives.html")
	.directive('umbPanel', function($timeout, $log){
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
angular.module("umbraco.directives")
	.directive('umbImageCrop',
		function ($timeout, localizationService, cropperHelper,  $log) {
	    return {
				restrict: 'E',
				replace: true,
				templateUrl: 'views/components/imaging/umb-image-crop.html',
				scope: {
					src: '=',
					width: '@',
					height: '@',
					crop: "=",
					center: "=",
					maxSize: '@'
				},

				link: function(scope, element, attrs) {
					scope.width = 400;
					scope.height = 320;

					scope.dimensions = {
						image: {},
						cropper:{},
						viewport:{},
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
							'height': (parseInt(scope.dimensions.viewport.height, 10)) + 'px',
							'width': (parseInt(scope.dimensions.viewport.width, 10)) + 'px'
						};
					};


					//elements
					var $viewport = element.find(".viewport");
					var $image = element.find("img");
					var $overlay = element.find(".overlay");
					var $container = element.find(".crop-container");

					//default constraints for drag n drop
					var constraints = {left: {max: scope.dimensions.margin, min: scope.dimensions.margin}, top: {max: scope.dimensions.margin, min: scope.dimensions.margin}, };
					scope.constraints = constraints;


					//set constaints for cropping drag and drop
					var setConstraints = function(){
						constraints.left.min = scope.dimensions.margin + scope.dimensions.cropper.width - scope.dimensions.image.width;
						constraints.top.min = scope.dimensions.margin + scope.dimensions.cropper.height - scope.dimensions.image.height;
					};


					var setDimensions = function(originalImage){
						originalImage.width("auto");
						originalImage.height("auto");

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
						var _viewPortW =  parseInt(scope.width, 10);
						var _viewPortH =  parseInt(scope.height, 10);

						//if we set a constraint we will scale it down if needed
						if(scope.maxSize){
							var ratioCalculation = cropperHelper.scaleToMaxSize(
									_viewPortW,
									_viewPortH,
									scope.maxSize);

							//so if we have a max size, override the thumb sizes
							_viewPortW = ratioCalculation.width;
							_viewPortH = ratioCalculation.height;
						}

						scope.dimensions.viewport.width = _viewPortW + 2 * scope.dimensions.margin;
						scope.dimensions.viewport.height = _viewPortH + 2 * scope.dimensions.margin;
						scope.dimensions.cropper.width = _viewPortW; // scope.dimensions.viewport.width - 2 * scope.dimensions.margin;
						scope.dimensions.cropper.height = _viewPortH; //  scope.dimensions.viewport.height - 2 * scope.dimensions.margin;
					};


					//when loading an image without any crop info, we center and fit it
					var resizeImageToEditor = function(){
						//returns size fitting the cropper
						var size = cropperHelper.calculateAspectRatioFit(
								scope.dimensions.image.width,
								scope.dimensions.image.height,
								scope.dimensions.cropper.width,
								scope.dimensions.cropper.height,
								true);

						//sets the image size and updates the scope
						scope.dimensions.image.width = size.width;
						scope.dimensions.image.height = size.height;

						//calculate the best suited ratios
						scope.dimensions.scale.min = size.ratio;
						scope.dimensions.scale.max = 2;
						scope.dimensions.scale.current = size.ratio;

						//center the image
						var position = cropperHelper.centerInsideViewPort(scope.dimensions.image, scope.dimensions.cropper);
						scope.dimensions.top = position.top;
						scope.dimensions.left = position.left;

						setConstraints();
					};

					//resize to a given ratio
					var resizeImageToScale = function(ratio){
						//do stuff
						var size = cropperHelper.calculateSizeToRatio(scope.dimensions.image.originalWidth, scope.dimensions.image.originalHeight, ratio);
						scope.dimensions.image.width = size.width;
						scope.dimensions.image.height = size.height;

						setConstraints();
						validatePosition(scope.dimensions.image.left, scope.dimensions.image.top);
					};

					//resize the image to a predefined crop coordinate
					var resizeImageToCrop = function(){
						scope.dimensions.image = cropperHelper.convertToStyle(
												scope.crop,
												{width: scope.dimensions.image.originalWidth, height: scope.dimensions.image.originalHeight},
												scope.dimensions.cropper,
												scope.dimensions.margin);

						var ratioCalculation = cropperHelper.calculateAspectRatioFit(
								scope.dimensions.image.originalWidth,
								scope.dimensions.image.originalHeight,
								scope.dimensions.cropper.width,
								scope.dimensions.cropper.height,
								true);

						scope.dimensions.scale.current = scope.dimensions.image.ratio;

						//min max based on original width/height
						scope.dimensions.scale.min = ratioCalculation.ratio;
						scope.dimensions.scale.max = 2;
					};



					var validatePosition = function(left, top){
						if(left > constraints.left.max)
						{
							left = constraints.left.max;
						}

						if(left <= constraints.left.min){
							left = constraints.left.min;
						}

						if(top > constraints.top.max)
						{
							top = constraints.top.max;
						}
						if(top <= constraints.top.min){
							top = constraints.top.min;
						}

						if(scope.dimensions.image.left !== left){
							scope.dimensions.image.left = left;
						}

						if(scope.dimensions.image.top !== top){
							scope.dimensions.image.top = top;
						}
					};


					//sets scope.crop to the recalculated % based crop
					var calculateCropBox = function(){
						scope.crop = cropperHelper.pixelsToCoordinates(scope.dimensions.image, scope.dimensions.cropper.width, scope.dimensions.cropper.height, scope.dimensions.margin);
					};


					//Drag and drop positioning, using jquery ui draggable
					var onStartDragPosition, top, left;
					$overlay.draggable({
						drag: function(event, ui) {
							scope.$apply(function(){
								validatePosition(ui.position.left, ui.position.top);
							});
						},
						stop: function(event, ui){
							scope.$apply(function(){
								//make sure that every validates one more time...
								validatePosition(ui.position.left, ui.position.top);

								calculateCropBox();
								scope.dimensions.image.rnd = Math.random();
							});
						}
					});



					var init = function(image){
						scope.loaded = false;

						//set dimensions on image, viewport, cropper etc
						setDimensions(image);

						//if we have a crop already position the image
						if(scope.crop){
							resizeImageToCrop();
						}else{
							resizeImageToEditor();
						}

						//sets constaints for the cropper
						setConstraints();
						scope.loaded = true;
					};


					/// WATCHERS ////
					scope.$watchCollection('[width, height]', function(newValues, oldValues){
							//we have to reinit the whole thing if
							//one of the external params changes
							if(newValues !== oldValues){
								setDimensions($image);
								setConstraints();
							}
					});

					var throttledResizing = _.throttle(function(){
						resizeImageToScale(scope.dimensions.scale.current);
						calculateCropBox();
					}, 100);


					//happens when we change the scale
					scope.$watch("dimensions.scale.current", function(){
						if(scope.loaded){
							throttledResizing();
						}
					});

					//ie hack
					if(window.navigator.userAgent.indexOf("MSIE ")){
						var ranger = element.find("input");
						ranger.bind("change",function(){
							scope.$apply(function(){
								scope.dimensions.scale.current = ranger.val();
							});
						});
					}

					//// INIT /////
					$image.load(function(){
						$timeout(function(){
							init($image);
						});
					});
				}
			};
		});

/**
* @ngdoc directive
* @name umbraco.directives.directive:umbCropsy
* @restrict E
* @function
* @description
* Used by editors that require naming an entity. Shows a textbox/headline with a required validator within it's own form.
**/
angular.module("umbraco.directives")
	.directive('umbImageGravity', function ($timeout, localizationService, $log) {
	    return {
				restrict: 'E',
				replace: true,
				templateUrl: 'views/components/imaging/umb-image-gravity.html',
				scope: {
					src: '=',
					center: "="
				},
				link: function(scope, element, attrs) {

					//Internal values for keeping track of the dot and the size of the editor
					scope.dimensions = {
						width: 0,
						height: 0,
						left: 0,
						top: 0
					};

					//elements
					var $viewport = element.find(".viewport");
					var $image = element.find("img");
					var $overlay = element.find(".overlay");

					scope.style = function () {
						if(scope.dimensions.width <= 0){
							setDimensions();
						}

						return {
							'top': scope.dimensions.top + 'px',
							'left': scope.dimensions.left + 'px'
						};
					};

					var setDimensions = function(){
						scope.dimensions.width = $image.width();
						scope.dimensions.height = $image.height();

						if(scope.center){
							scope.dimensions.left =  scope.center.left * scope.dimensions.width -10;
							scope.dimensions.top =  scope.center.top * scope.dimensions.height -10;
						}else{
							scope.center = { left: 0.5, top: 0.5 };
						}
					};

					var calculateGravity = function(){
						scope.dimensions.left = $overlay[0].offsetLeft;
						scope.dimensions.top =  $overlay[0].offsetTop;

						scope.center.left =  (scope.dimensions.left+10) / scope.dimensions.width;
						scope.center.top =  (scope.dimensions.top+10) / scope.dimensions.height;
					};

					var lazyEndEvent = _.debounce(function(){
						scope.$apply(function(){
							scope.$emit("imageFocalPointStop");
						});
					}, 2000);


					//Drag and drop positioning, using jquery ui draggable
					//TODO ensure that the point doesnt go outside the box
					$overlay.draggable({
						containment: "parent",
						start: function(){
							scope.$apply(function(){
								scope.$emit("imageFocalPointStart");
							});
						},
						stop: function() {
							scope.$apply(function(){
								calculateGravity();
							});

							lazyEndEvent();
						}
					});

					//// INIT /////
					$image.load(function(){
						$timeout(function(){
							setDimensions();
						});
					});
				}
			};
		});

/**
* @ngdoc directive
* @name umbraco.directives.directive:umbCropsy
* @restrict E
* @function
* @description
* Used by editors that require naming an entity. Shows a textbox/headline with a required validator within it's own form.
**/
angular.module("umbraco.directives")
	.directive('umbImageThumbnail',
		function ($timeout, localizationService, cropperHelper, $log) {
	    return {
				restrict: 'E',
				replace: true,
				templateUrl: 'views/components/imaging/umb-image-thumbnail.html',

				scope: {
					src: '=',
					width: '@',
					height: '@',
					center: "=",
					crop: "=",
					maxSize: '@'
				},

				link: function(scope, element, attrs) {
					//// INIT /////
					var $image = element.find("img");

					$image.load(function(){
						$timeout(function(){
							$image.width("auto");
							$image.height("auto");

							scope.image = {};
							scope.image.width = $image[0].width;
							scope.image.height = $image[0].height;

							//we force a lower thumbnail size to fit the max size
							//we do not compare to the image dimensions, but the thumbs
							if(scope.maxSize){
								var ratioCalculation = cropperHelper.calculateAspectRatioFit(
										scope.width,
										scope.height,
										scope.maxSize,
										scope.maxSize,
										false);

								//so if we have a max size, override the thumb sizes
								scope.width = ratioCalculation.width;
								scope.height = ratioCalculation.height;
							}

							setPreviewStyle();
						});
					});

					/// WATCHERS ////
					scope.$watchCollection('[crop, center]', function(newValues, oldValues){
							//we have to reinit the whole thing if
							//one of the external params changes
							setPreviewStyle();
					});

					scope.$watch("center", function(){
						setPreviewStyle();
					}, true);

					function setPreviewStyle(){
						if(scope.crop && scope.image){
							scope.preview = cropperHelper.convertToStyle(
												scope.crop,
												scope.image,
												{width: scope.width, height: scope.height},
												0);
						}else if(scope.image){

							//returns size fitting the cropper
							var p = cropperHelper.calculateAspectRatioFit(
									scope.image.width,
									scope.image.height,
									scope.width,
									scope.height,
									true);


							if(scope.center){
								var xy = cropperHelper.alignToCoordinates(p, scope.center, {width: scope.width, height: scope.height});
								p.top = xy.top;
								p.left = xy.left;
							}else{

							}

							p.position = "absolute";
							scope.preview = p;
						}
					}
				}
			};
		});

angular.module("umbraco.directives")

    .directive('localize', function ($log, localizationService) {
        return {
            restrict: 'E',
            scope:{
                key: '@'
            },
            replace: true,

            link: function (scope, element, attrs) {
                var key = scope.key;
                localizationService.localize(key).then(function(value){
                    element.html(value);
                });
            }
        };
    })

    .directive('localize', function ($log, localizationService) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var keys = attrs.localize.split(',');

                angular.forEach(keys, function(value, key){
                    var attr = element.attr(value);

                    if(attr){
                        if(attr[0] === '@'){

                            var t = localizationService.tokenize(attr.substring(1), scope);
                            localizationService.localize(t.key, t.tokens).then(function(val){
                                    element.attr(value, val);
                            });

                        }
                    }
                });
            }
        };

    });
/**
 * @ngdoc directive
 * @name umbraco.directives.directive:umbNotifications
 */

(function() {
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
         restrict: "E",
         replace: true,
         templateUrl: 'views/components/notifications/umb-notifications.html',
         link: link
      };

      return directive;

   }

   angular.module('umbraco.directives').directive('umbNotifications', NotificationDirective);

})();

/**
 * @ngdoc directive
 * @name umbraco.directives.directive:umbProperty
 * @restrict E
 **/

(function() {
   'use strict';

   function OverlayDirective($timeout, formHelper, overlayHelper, localizationService) {

      function link(scope, el, attr, ctrl) {

          scope.directive = {
              enableConfirmButton: false
          };

         var overlayNumber = 0;
         var numberOfOverlays = 0;
         var isRegistered = false;

         var modelCopy = {};

         function activate() {

            setView();

            setButtonText();

            modelCopy = makeModelCopy(scope.model);

            $timeout(function() {

               if (scope.position === "target") {
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

               if (scope.view.indexOf(".html") === -1) {
                  var viewAlias = scope.view.toLowerCase();
                  scope.view = "views/common/overlays/" + viewAlias + "/" + viewAlias + ".html";
               }

            }

         }

         function setButtonText() {
             if (!scope.model.closeButtonLabelKey && !scope.model.closeButtonLabel) {
                 scope.model.closeButtonLabel = localizationService.localize("general_close");
             }
             if (!scope.model.submitButtonLabelKey && !scope.model.submitButtonLabel) {
                 scope.model.submitButtonLabel = localizationService.localize("general_submit");
             }
         }

         function registerOverlay() {

            overlayNumber = overlayHelper.registerOverlay();

            $(document).bind("keydown.overlay-" + overlayNumber, function(event) {

               if (event.which === 27) {

                  numberOfOverlays = overlayHelper.getNumberOfOverlays();

                  if(numberOfOverlays === overlayNumber) {
                     scope.closeOverLay();
                  }

                  event.preventDefault();
               }

               if (event.which === 13) {

                  numberOfOverlays = overlayHelper.getNumberOfOverlays();

                  if(numberOfOverlays === overlayNumber) {

                     var activeElementType = document.activeElement.tagName;
                     var clickableElements = ["A", "BUTTON"];
                     var submitOnEnter = document.activeElement.hasAttribute("overlay-submit-on-enter");

                     if(clickableElements.indexOf(activeElementType) === 0) {
                        document.activeElement.click();
                        event.preventDefault();
                     } else if(activeElementType === "TEXTAREA" && !submitOnEnter) {


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

            if(isRegistered) {

               overlayHelper.unregisterOverlay();

               $(document).unbind("keydown.overlay-" + overlayNumber);

               isRegistered = false;
            }

         }

         function makeModelCopy(object) {

            var newObject = {};

            for (var key in object) {
               if (key !== "event") {
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

            if(scope.position === "center" || scope.position === "target") {
               var overlayTopPosition = el.context.offsetTop;
               el.css('top', overlayTopPosition + indentSize);
            }

         }

         function setTargetPosition() {

            var container = $("#contentwrapper");
            var containerLeft = container[0].offsetLeft;
            var containerRight = containerLeft + container[0].offsetWidth;
            var containerTop = container[0].offsetTop;
            var containerBottom = containerTop + container[0].offsetHeight;

            var mousePositionClickX = null;
            var mousePositionClickY = null;
            var elementHeight = null;
            var elementWidth = null;

            var position = {
               right: "inherit",
               left: "inherit",
               top: "inherit",
               bottom: "inherit"
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
               position.left = mousePositionClickX - (elementWidth / 2);
               position.top = mousePositionClickY - (elementHeight / 2);

               // check to see if element is outside screen
               // outside right
               if (position.left + elementWidth > containerRight) {
                  position.right = 10;
                  position.left = "inherit";
               }

               // outside bottom
               if (position.top + elementHeight > containerBottom) {
                  position.bottom = 10;
                  position.top = "inherit";
               }

               // outside left
               if (position.left < containerLeft) {
                  position.left = containerLeft + 10;
                  position.right = "inherit";
               }

               // outside top
               if (position.top < containerTop) {
                  position.top = 10;
                  position.bottom = "inherit";
               }

               el.css(position);

            }

         }

         scope.submitForm = function(model) {
            if(scope.model.submit) {
                 if (formHelper.submitForm({scope: scope})) {
                    formHelper.resetForm({ scope: scope });

                    if(scope.model.confirmSubmit && scope.model.confirmSubmit.enable && !scope.directive.enableConfirmButton) {
                        scope.model.submit(model, modelCopy, scope.directive.enableConfirmButton);
                    } else {
                        unregisterOverlay();
                        scope.model.submit(model, modelCopy, scope.directive.enableConfirmButton);
                    }

                 }
             }
         };

         scope.cancelConfirmSubmit = function() {
             scope.model.confirmSubmit.show = false;
         };

         scope.closeOverLay = function() {

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
         if (attr.hasOwnProperty("ngShow")) {
            scope.$watch("ngShow", function(value) {
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

         scope.$on('$destroy', function(){
            unregisterOverlay();
         });

      }

      var directive = {
         transclude: true,
         restrict: 'E',
         replace: true,
         templateUrl: 'views/components/overlays/umb-overlay.html',
         scope: {
            ngShow: "=",
            model: "=",
            view: "=",
            position: "@"
         },
         link: link
      };

      return directive;
   }

   angular.module('umbraco.directives').directive('umbOverlay', OverlayDirective);

})();

(function() {
   'use strict';

   function OverlayBackdropDirective(overlayHelper) {

      function link(scope, el, attr, ctrl) {

         scope.numberOfOverlays = 0;

         scope.$watch(function(){
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

})();

/**
* @ngdoc directive
* @name umbraco.directives.directive:umbProperty
* @restrict E
**/
angular.module("umbraco.directives")
    .directive('umbProperty', function (umbPropEditorHelper) {
        return {
            scope: {
                property: "="
            },
            transclude: true,
            restrict: 'E',
            replace: true,        
            templateUrl: 'views/components/property/umb-property.html',
            link: function(scope) {
                scope.propertyAlias = Umbraco.Sys.ServerVariables.isDebuggingEnabled === true ? scope.property.alias : null;
            },
            //Define a controller for this directive to expose APIs to other directives
            controller: function ($scope, $timeout) {
               
                var self = this;

                //set the API properties/methods
                
                self.property = $scope.property;
                self.setPropertyError = function(errorMsg) {
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
                model: "=",
                isPreValue: "@",
                preview: "@"
            },
            
            require: "^form",
            restrict: 'E',
            replace: true,      
            templateUrl: 'views/components/property/umb-property-editor.html',
            link: function (scope, element, attrs, ctrl) {

                //we need to copy the form controller val to our isolated scope so that
                //it get's carried down to the child scopes of this!
                //we'll also maintain the current form name.
                scope[ctrl.$name] = ctrl;

                if(!scope.model.alias){
                   scope.model.alias = Math.random().toString(36).slice(2);
                }

                scope.$watch("model.view", function(val){
                    scope.propertyEditorView = umbPropEditorHelper.getViewPath(scope.model.view, scope.isPreValue);
                });
            }
        };
    };

//Preffered is the umb-property-editor as its more explicit - but we keep umb-editor for backwards compat
angular.module("umbraco.directives").directive('umbPropertyEditor', _umbPropertyEditor);
angular.module("umbraco.directives").directive('umbEditor', _umbPropertyEditor);

angular.module("umbraco.directives.html")
    .directive('umbPropertyGroup', function () {
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
angular.module("umbraco.directives")
.directive('umbTab', function ($parse, $timeout) {
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
angular.module("umbraco.directives")
.directive('umbTabs', function () {
    return {
		restrict: 'A',
		controller: function ($scope, $element, $attrs) {
            
		    var callbacks = [];
		    this.onTabShown = function(cb) {
		        callbacks.push(cb);
		    };

            function tabShown(event) {

                var curr = $(event.target);         // active tab
                var prev = $(event.relatedTarget);  // previous tab

                for (var c in callbacks) {
                    callbacks[c].apply(this, [{current: curr, previous: prev}]);
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
(function() {
  'use strict';

  function UmbTabsContentDirective() {

    function link(scope, el, attr, ctrl) {

      scope.view = attr.view;

   }

    var directive = {
      restrict: "E",
      replace: true,
      transclude: 'true',
      templateUrl: "views/components/tabs/umb-tabs-content.html",
      link: link
    };

    return directive;
  }

  angular.module('umbraco.directives').directive('umbTabsContent', UmbTabsContentDirective);

})();

(function() {
  'use strict';

  function UmbTabsNavDirective($timeout) {

    function link(scope, el, attr) {

      function activate() {

        $timeout(function () {

          //use bootstrap tabs API to show the first one
          el.find("a:first").tab('show');

          //enable the tab drop
          el.tabdrop();

        });

      }

      var unbindModelWatch = scope.$watch('model', function(newValue, oldValue){

        activate();

      });


      scope.$on('$destroy', function () {

          //ensure to destroy tabdrop (unbinds window resize listeners)
          el.tabdrop("destroy");

          unbindModelWatch();

      });

    }

    var directive = {
      restrict: "E",
      replace: true,
      templateUrl: "views/components/tabs/umb-tabs-nav.html",
      scope: {
        model: "=",
        tabdrop: "="
      },
      link: link
    };

    return directive;
  }

  angular.module('umbraco.directives').directive('umbTabsNav', UmbTabsNavDirective);

})();

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
            //Custom query string arguments to pass in to the tree as a string, example: "startnodeid=123&something=value"
            customtreeparams: '@',
            eventhandler: '=',
            enablecheckboxes: '@',
            enablelistviewsearch: '@'
        },

        compile: function(element, attrs) {
            //config
            //var showheader = (attrs.showheader !== 'false');
            var hideoptions = (attrs.hideoptions === 'true') ? "hide-options" : "";
            var template = '<ul class="umb-tree ' + hideoptions + '"><li class="root">';
            template += '<div ng-hide="hideheader" on-right-click="altSelect(tree.root, $event)">' +
                '<h5>' +
                '<a href="#/{{section}}" ng-click="select(tree.root, $event)"  class="root-link"><i ng-if="enablecheckboxes == \'true\'" ng-class="selectEnabledNodeClass(tree.root)"></i> {{tree.name}}</a></h5>' +
                '<a class="umb-options" ng-hide="tree.root.isContainer || !tree.root.menuUrl" ng-click="options(tree.root, $event)" ng-swipe-right="options(tree.root, $event)"><i></i><i></i><i></i></a>' +
                '</div>';
            template += '<ul>' +
                '<umb-tree-item ng-repeat="child in tree.root.children" eventhandler="eventhandler" node="child" current-node="currentNode" tree="this" section="{{section}}" ng-animate="animation()"></umb-tree-item>' +
                '</ul>' +
                '</li>' +
                '</ul>';

            element.replaceWith(template);

            return function(scope, elem, attr, controller) {

                //flag to track the last loaded section when the tree 'un-loads'. We use this to determine if we should
                // re-load the tree again. For example, if we hover over 'content' the content tree is shown. Then we hover
                // outside of the tree and the tree 'un-loads'. When we re-hover over 'content', we don't want to re-load the
                // entire tree again since we already still have it in memory. Of course if the section is different we will
                // reload it. This saves a lot on processing if someone is navigating in and out of the same section many times
                // since it saves on data retreival and DOM processing.
                var lastSection = "";

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

                        scope.eventhandler.clearCache = function(section) {
                            treeService.clearCache({ section: section });
                        };

                        scope.eventhandler.load = function(section) {
                            scope.section = section;
                            loadTree();
                        };

                        scope.eventhandler.reloadNode = function(node) {

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
                        scope.eventhandler.syncTree = function(args) {
                            if (!args) {
                                throw "args cannot be null";
                            }
                            if (!args.path) {
                                throw "args.path cannot be null";
                            }

                            var deferred = $q.defer();

                            //this is super complex but seems to be working in other places, here we're listening for our
                            // own events, once the tree is sycned we'll resolve our promise.
                            scope.eventhandler.one("treeSynced", function (e, syncArgs) {
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

                            args.path = _.filter(args.path, function (item) { return (item !== "init" && item !== "-1"); });

                            //Once those are filtered we need to check if the current user has a special start node id,
                            // if they do, then we're going to trim the start of the array for anything found from that start node
                            // and previous so that the tree syncs properly. The tree syncs from the top down and if there are parts
                            // of the tree's path in there that don't actually exist in the dom/model then syncing will not work.

                            userService.getCurrentUser().then(function(userData) {

                                var startNodes = [userData.startContentId, userData.startMediaId];
                                _.each(startNodes, function (i) {
                                    var found = _.find(args.path, function (p) {
                                        return String(p) === String(i);
                                    });
                                    if (found) {
                                        args.path = args.path.splice(_.indexOf(args.path, found));
                                    }
                                });


                                loadPath(args.path, args.forceReload, args.activate);

                            });



                            return deferred.promise;
                        };

                        /**
                            Internal method that should ONLY be used by the legacy API wrapper, the legacy API used to
                            have to set an active tree and then sync, the new API does this in one method by using syncTree.
                            loadChildren is optional but if it is set, it will set the current active tree and load the root
                            node's children - this is synonymous with the legacy refreshTree method - again should not be used
                            and should only be used for the legacy code to work.
                        */
                        scope.eventhandler._setActiveTreeType = function(treeAlias, loadChildren) {
                            loadActiveTree(treeAlias, loadChildren);
                        };
                    }
                }


                //helper to load a specific path on the active tree as soon as its ready
                function loadPath(path, forceReload, activate) {

                    if (scope.activeTree) {
                        syncTree(scope.activeTree, path, forceReload, activate);
                    }
                    else {
                        scope.eventhandler.one("activeTreeLoaded", function (e, args) {
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
                            if(node && node.metaData && node.metaData.treeAlias) {
                                return node.metaData.treeAlias.toUpperCase() === treeAlias.toUpperCase();
                            }
                            return false;
                        });

                        if (!scope.activeTree) {
                            throw "Could not find the tree " + treeAlias + ", activeTree has not been set";
                        }

                        //This is only used for the legacy tree method refreshTree!
                        if (loadChildren) {
                            scope.activeTree.expanded = true;
                            scope.loadChildren(scope.activeTree, false).then(function() {
                                emitEvent("activeTreeLoaded", { tree: scope.activeTree });
                            });
                        }
                        else {
                            emitEvent("activeTreeLoaded", { tree: scope.activeTree });
                        }
                    }

                    if (scope.tree) {
                        doLoad(scope.tree.root);
                    }
                    else {
                        scope.eventhandler.one("treeLoaded", function(e, args) {
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
                        var args = { section: scope.section, tree: scope.treealias, cacheKey: scope.cachekey, isDialog: scope.isdialog ? scope.isdialog : false };

                        //add the extra query string params if specified
                        if (scope.customtreeparams) {
                            args["queryString"] = scope.customtreeparams;
                        }

                        treeService.getTree(args)
                            .then(function(data) {
                                //set the data once we have it
                                scope.tree = data;

                                enableDeleteAnimations();

                                scope.loading = false;

                                //set the root as the current active tree
                                scope.activeTree = scope.tree.root;
                                emitEvent("treeLoaded", { tree: scope.tree });
                                emitEvent("treeNodeExpanded", { tree: scope.tree, node: scope.tree.root, children: scope.tree.root.children });

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

                        emitEvent("treeSynced", { node: data, activate: activate });

                        enableDeleteAnimations();
                    });

                }

                scope.selectEnabledNodeClass = function (node) {
                    return node ?
                        node.selected ?
                        'icon umb-tree-icon sprTree icon-check blue temporary' :
                        '' :
                        '';
                };

                /** method to set the current animation for the node.
                 *  This changes dynamically based on if we are changing sections or just loading normal tree data.
                 *  When changing sections we don't want all of the tree-ndoes to do their 'leave' animations.
                 */
                scope.animation = function() {
                    if (deleteAnimations && scope.tree && scope.tree.root && scope.tree.root.expanded) {
                        return { leave: 'tree-node-delete-leave' };
                    }
                    else {
                        return {};
                    }
                };

                /* helper to force reloading children of a tree node */
                scope.loadChildren = function(node, forceReload) {
                    var deferred = $q.defer();

                    //emit treeNodeExpanding event, if a callback object is set on the tree
                    emitEvent("treeNodeExpanding", { tree: scope.tree, node: node });

                    //standardising
                    if (!node.children) {
                        node.children = [];
                    }

                    if (forceReload || (node.hasChildren && node.children.length === 0)) {
                        //get the children from the tree service
                        treeService.loadNodeChildren({ node: node, section: scope.section })
                            .then(function(data) {
                                //emit expanded event
                                emitEvent("treeNodeExpanded", { tree: scope.tree, node: node, children: data });

                                enableDeleteAnimations();

                                deferred.resolve(data);
                            });
                    }
                    else {
                        emitEvent("treeNodeExpanded", { tree: scope.tree, node: node, children: node.children });
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
                scope.options = function(n, ev) {
                    emitEvent("treeOptionsClick", { element: elem, node: n, event: ev });
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

                    emitEvent("treeNodeSelect", { element: elem, node: n, event: ev });
                };

                scope.altSelect = function(n, ev) {
                    emitEvent("treeNodeAltSelect", { element: elem, tree: scope.tree, node: n, event: ev });
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

                setupExternalEvents();
                loadTree();
            };
        }
    };
}

angular.module("umbraco.directives").directive('umbTree', umbTreeDirective);

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
angular.module("umbraco.directives")
.directive('umbTreeItem', function ($compile, $http, $templateCache, $interpolate, $log, $location, $rootScope, $window, treeService, $timeout, localizationService) {
    return {
        restrict: 'E',
        replace: true,

        scope: {
            section: '@',
            eventhandler: '=',
            currentNode: '=',
            node: '=',
            tree: '='
        },

        //TODO: Remove more of the binding from this template and move the DOM manipulation to be manually done in the link function,
        // this will greatly improve performance since there's potentially a lot of nodes being rendered = a LOT of watches!

        template: '<li ng-class="{\'current\': (node == currentNode)}" on-right-click="altSelect(node, $event)">' +
            '<div ng-class="getNodeCssClass(node)" ng-swipe-right="options(node, $event)" >' +
            //NOTE: This ins element is used to display the search icon if the node is a container/listview and the tree is currently in dialog
            //'<ins ng-if="tree.enablelistviewsearch && node.metaData.isContainer" class="umb-tree-node-search icon-search" ng-click="searchNode(node, $event)" alt="searchAltText"></ins>' + 
            '<ins ng-class="{\'icon-navigation-right\': !node.expanded, \'icon-navigation-down\': node.expanded}" ng-click="load(node)">&nbsp;</ins>' +
            '<i class="icon umb-tree-icon sprTree" ng-click="select(node, $event)"></i>' +
            '<a href="#/{{node.routePath}}" ng-click="select(node, $event)"></a>' +
            //NOTE: These are the 'option' elipses
            '<a class="umb-options" ng-click="options(node, $event)"><i></i><i></i><i></i></a>' +
            '<div ng-show="node.loading" class="l"><div></div></div>' +
            '</div>' +
            '</li>',
        
        link: function (scope, element, attrs) {

            localizationService.localize("general_search").then(function (value) {
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

                element.find("a:first").text(node.name);

                if (!node.menuUrl) {
                    element.find("a.umb-options").remove();
                }

                if (node.style) {
                    element.find("i:first").attr("style", node.style);
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
                var css = [];                
                if (node.cssClasses) {
                    _.each(node.cssClasses, function(c) {
                        css.push(c);
                    });
                }
                if (node.selected) {
                    css.push("umb-tree-node-checked");
                }
                return css.join(" ");
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
                emitEvent("treeOptionsClick", { element: element, tree: scope.tree, node: n, event: ev });
            };

            /**
              Method called when an item is clicked in the tree, this passes the 
              DOM element, the tree node object and the original click
              and emits it as a treeNodeSelect element if there is a callback object
              defined on the tree
            */
            scope.select = function (n, ev) {
                if (ev.ctrlKey ||
                    ev.shiftKey ||
                    ev.metaKey || // apple
                    (ev.button && ev.button === 1) // middle click, >IE9 + everyone else
                ) {
                    return;
                }

                emitEvent("treeNodeSelect", { element: element, tree: scope.tree, node: n, event: ev });
                ev.preventDefault();
            };

            /**
              Method called when an item is right-clicked in the tree, this passes the 
              DOM element, the tree node object and the original click
              and emits it as a treeNodeSelect element if there is a callback object
              defined on the tree
            */
            scope.altSelect = function (n, ev) {
                emitEvent("treeNodeAltSelect", { element: element, tree: scope.tree, node: n, event: ev });
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
                    deleteAnimations = false;
                    emitEvent("treeNodeCollapsing", { tree: scope.tree, node: node, element: element });
                    node.expanded = false;
                }
                else {
                    scope.loadChildren(node, false);
                }
            };

            /* helper to force reloading children of a tree node */
            scope.loadChildren = function (node, forceReload) {
                //emit treeNodeExpanding event, if a callback object is set on the tree
                emitEvent("treeNodeExpanding", { tree: scope.tree, node: node });

                if (node.hasChildren && (forceReload || !node.children || (angular.isArray(node.children) && node.children.length === 0))) {
                    //get the children from the tree service
                    treeService.loadNodeChildren({ node: node, section: scope.section })
                        .then(function (data) {
                            //emit expanded event
                            emitEvent("treeNodeExpanded", { tree: scope.tree, node: node, children: data });
                            enableDeleteAnimations();
                        });
                }
                else {
                    emitEvent("treeNodeExpanded", { tree: scope.tree, node: node, children: node.children });
                    node.expanded = true;
                    enableDeleteAnimations();
                }
            };            

            //if the current path contains the node id, we will auto-expand the tree item children

            setupNodeDom(scope.node, scope.tree);

            var template = '<ul ng-class="{collapsed: !node.expanded}"><umb-tree-item  ng-repeat="child in node.children" eventhandler="eventhandler" tree="tree" current-node="currentNode" node="child" section="{{section}}" ng-animate="animation()"></umb-tree-item></ul>';
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
            searchFromId: "@",
            searchFromName: "@",
            showSearch: "@",
            section: "@",
            hideSearchCallback: "=",
            searchCallback: "="
        },
        restrict: "E",    // restrict to an element
        replace: true,   // replace the html element with the template
        templateUrl: 'views/components/tree/umb-tree-search-box.html',
        link: function (scope, element, attrs, ctrl) {

            scope.term = "";
            scope.hideSearch = function() {
                scope.term = "";
                scope.hideSearchCallback();
            };

            localizationService.localize("general_typeToSearch").then(function (value) {
                scope.searchPlaceholderText = value;
            });

            if (!scope.showSearch) {
                scope.showSearch = "false";
            }

            //used to cancel any request in progress if another one needs to take it's place
            var canceler = null;

            function performSearch() {
                if (scope.term) {
                    scope.results = [];

                    //a canceler exists, so perform the cancelation operation and reset
                    if (canceler) {
                        console.log("CANCELED!");
                        canceler.resolve();
                        canceler = $q.defer();
                    }
                    else {
                        canceler = $q.defer();
                    }

                    var searchArgs = {
                        term: scope.term,
                        canceler: canceler
                    };

                    //append a start node context if there is one
                    if (scope.searchFromId) {
                        searchArgs["searchFrom"] = scope.searchFromId;
                    }

                    searcher(searchArgs).then(function (data) {
                        scope.searchCallback(data);
                        //set back to null so it can be re-created
                        canceler = null;
                    });
                }
            }

            scope.$watch("term", _.debounce(function(newVal, oldVal) {
                scope.$apply(function() {
                    if (newVal !== null && newVal !== undefined && newVal !== oldVal) {
                        performSearch();
                    }
                });
            }, 200));

            var searcher = searchService.searchContent;
            //search
            if (scope.section === "member") {
                searcher = searchService.searchMembers;
            }
            else if (scope.section === "media") {
                searcher = searchService.searchMedia;
            }
        }
    };
}
angular.module('umbraco.directives').directive("umbTreeSearchBox", treeSearchBox);

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
            results: "=",
            selectResultCallback: "="
        },
        restrict: "E",    // restrict to an element
        replace: true,   // replace the html element with the template
        templateUrl: 'views/components/tree/umb-tree-search-results.html',
        link: function (scope, element, attrs, ctrl) {

        }
    };
}
angular.module('umbraco.directives').directive("umbTreeSearchResults", treeSearchResults);

angular.module("umbraco.directives")
    .directive('umbGenerateAlias', function ($timeout, entityResource) {
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
                var generateAliasTimeout = "";
                var updateAlias = false;

                scope.locked = true;
                scope.placeholderText = "Enter alias...";

                function generateAlias(value) {

                  if (generateAliasTimeout) {
                    $timeout.cancel(generateAliasTimeout);
                  }

                  if( value !== undefined && value !== "" && value !== null) {

                      scope.alias = "";
                    scope.placeholderText = "Generating Alias...";

                    generateAliasTimeout = $timeout(function () {
                       updateAlias = true;
                        entityResource.getSafeAlias(value, true).then(function (safeAlias) {
                            if (updateAlias) {
                              scope.alias = safeAlias.alias;
                           }
                      });
                    }, 500);

                  } else {
                    updateAlias = true;
                    scope.alias = "";
                    scope.placeholderText = "Enter alias...";
                  }

                }

                // if alias gets unlocked - stop watching alias
                eventBindings.push(scope.$watch('locked', function(newValue, oldValue){
                    if(newValue === false) {
                       bindWatcher = false;
                    }
                }));

                // validate custom entered alias
                eventBindings.push(scope.$watch('alias', function(newValue, oldValue){

                  if(scope.alias === "" && bindWatcher === true || scope.alias === null && bindWatcher === true) {
                    // add watcher
                    eventBindings.push(scope.$watch('aliasFrom', function(newValue, oldValue) {
                       if(bindWatcher) {
                          generateAlias(newValue);
                       }
                    }));
                  }

               }));

               // clean up
               scope.$on('$destroy', function(){
                 // unbind watchers
                 for(var e in eventBindings) {
                   eventBindings[e]();
                  }
               });

            }
        };
    });

(function() {
    'use strict';

    function ChildSelectorDirective() {

        function link(scope, el, attr, ctrl) {

            var eventBindings = [];
            scope.dialogModel = {};
            scope.showDialog = false;

            scope.removeChild = function(selectedChild, $index) {
               if(scope.onRemove) {
                  scope.onRemove(selectedChild, $index);
               }
            };

            scope.addChild = function($event) {
               if(scope.onAdd) {
                  scope.onAdd($event);
               }
            };

            function syncParentName() {

              // update name on available item
              angular.forEach(scope.availableChildren, function(availableChild){
                if(availableChild.id === scope.parentId) {
                  availableChild.name = scope.parentName;
                }
              });

              // update name on selected child
              angular.forEach(scope.selectedChildren, function(selectedChild){
                if(selectedChild.id === scope.parentId) {
                  selectedChild.name = scope.parentName;
                }
              });

            }

            function syncParentIcon() {

              // update icon on available item
              angular.forEach(scope.availableChildren, function(availableChild){
                if(availableChild.id === scope.parentId) {
                  availableChild.icon = scope.parentIcon;
                }
              });

              // update icon on selected child
              angular.forEach(scope.selectedChildren, function(selectedChild){
                if(selectedChild.id === scope.parentId) {
                  selectedChild.icon = scope.parentIcon;
                }
              });

            }

            eventBindings.push(scope.$watch('parentName', function(newValue, oldValue){

              if (newValue === oldValue) { return; }
              if ( oldValue === undefined || newValue === undefined) { return; }

              syncParentName();

            }));

            eventBindings.push(scope.$watch('parentIcon', function(newValue, oldValue){

              if (newValue === oldValue) { return; }
              if ( oldValue === undefined || newValue === undefined) { return; }

              syncParentIcon();
            }));

            // clean up
            scope.$on('$destroy', function(){
              // unbind watchers
              for(var e in eventBindings) {
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
                availableChildren: "=",
                parentName: "=",
                parentIcon: "=",
                parentId: "=",
                onRemove: "=",
                onAdd: "="
            },
            link: link
        };

        return directive;
    }

    angular.module('umbraco.directives').directive('umbChildSelector', ChildSelectorDirective);

})();

/**
 * @ngdoc directive
 * @name umbraco.directives.directive:umbConfirm
 * @function
 * @description
 * A confirmation dialog
 *
 * @restrict E
 */
function confirmDirective() {
    return {
        restrict: "E",    // restrict to an element
        replace: true,   // replace the html element with the template
        templateUrl: 'views/components/umb-confirm.html',
        scope: {
            onConfirm: '=',
            onCancel: '=',
            caption: '@'
        },
        link: function (scope, element, attr, ctrl) {

        }
    };
}
angular.module('umbraco.directives').directive("umbConfirm", confirmDirective);

(function() {
  'use strict';

  function ConfirmAction() {

    function link(scope, el, attr, ctrl) {

      scope.clickConfirm = function() {
          if(scope.onConfirm) {
              scope.onConfirm();
          }
      };

      scope.clickCancel = function() {
          if(scope.onCancel) {
              scope.onCancel();
          }
      };

    }

    var directive = {
      restrict: 'E',
      replace: true,
      templateUrl: 'views/components/umb-confirm-action.html',
      scope: {
        direction: "@",
        onConfirm: "&",
        onCancel: "&"
      },
      link: link
    };

    return directive;
  }

  angular.module('umbraco.directives').directive('umbConfirmAction', ConfirmAction);

})();

(function() {
   'use strict';

   function ContentGridDirective() {

      function link(scope, el, attr, ctrl) {

         scope.clickItem = function(item, $event, $index) {
            if(scope.onClick) {
               scope.onClick(item, $event, $index);
            }
         };

         scope.clickItemName = function(item, $event, $index) {
            if(scope.onClickName) {
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
            contentProperties: "=",
            onClick: "=",
            onClickName: "="
         },
         link: link
      };

      return directive;
   }

   angular.module('umbraco.directives').directive('umbContentGrid', ContentGridDirective);

})();

(function() {
  'use strict';

  function UmbDisableFormValidation() {

      var directive = {
          restrict: 'A',
          require: '?form',
          link: function (scope, elm, attrs, ctrl) {
              //override the $setValidity function of the form to disable validation
              ctrl.$setValidity = function () { };
          }
      };

    return directive;
  }

  angular.module('umbraco.directives').directive('umbDisableFormValidation', UmbDisableFormValidation);

})();

(function() {
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

})();

(function() {
   'use strict';

   function FolderGridDirective() {

      function link(scope, el, attr, ctrl) {

         scope.clickFolder = function(folder, $event, $index) {
            if(scope.onClick) {
               scope.onClick(folder, $event, $index);
            }
         };

         scope.clickFolderName = function(folder, $event, $index) {
            if(scope.onClickName) {
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
            onClick: "=",
            onClickName: "="
         },
         link: link
      };

      return directive;
   }

   angular.module('umbraco.directives').directive('umbFolderGrid', FolderGridDirective);

})();

(function() {
    'use strict';

    function GridSelector() {

        function link(scope, el, attr, ctrl) {

            var eventBindings = [];
            scope.dialogModel = {};
            scope.showDialog = false;
            scope.itemLabel = "";

            // set default item name
            if(!scope.itemName){
                scope.itemLabel = "item";
            } else {
                scope.itemLabel = scope.itemName;
            }

            scope.removeItem = function(selectedItem) {
                var selectedItemIndex = scope.selectedItems.indexOf(selectedItem);
                scope.selectedItems.splice(selectedItemIndex, 1);
            };

            scope.removeDefaultItem = function() {

                // it will be the last item so we can clear the array
                scope.selectedItems = [];

                // remove as default item
                scope.defaultItem = null;

            };

            scope.openItemPicker = function($event){
                scope.dialogModel = {
                    view: "itempicker",
                    title: "Choose " + scope.itemLabel,
                    availableItems: scope.availableItems,
                    selectedItems: scope.selectedItems,
                    event: $event,
                    show: true,
                    submit: function(model) {
                        scope.selectedItems.push(model.selectedItem);

                        // if no default item - set item as default
                        if(scope.defaultItem === null) {
                            scope.setAsDefaultItem(model.selectedItem);
                        }

                        scope.dialogModel.show = false;
                        scope.dialogModel = null;
                    }
                };
            };

            scope.setAsDefaultItem = function(selectedItem) {

                // clear default item
                scope.defaultItem = {};

                // set as default item
                scope.defaultItem = selectedItem;
            };

            function updatePlaceholders() {

              // update default item
              if(scope.defaultItem !== null && scope.defaultItem.placeholder) {

                scope.defaultItem.name = scope.name;

                if(scope.alias !== null && scope.alias !== undefined) {
                  scope.defaultItem.alias = scope.alias;
                }

              }

              // update selected items
              angular.forEach(scope.selectedItems, function(selectedItem) {
                if(selectedItem.placeholder) {

                  selectedItem.name = scope.name;

                  if(scope.alias !== null && scope.alias !== undefined) {
                    selectedItem.alias = scope.alias;
                  }

                }
              });

              // update availableItems
              angular.forEach(scope.availableItems, function(availableItem) {
                if(availableItem.placeholder) {

                  availableItem.name = scope.name;

                  if(scope.alias !== null && scope.alias !== undefined) {
                    availableItem.alias = scope.alias;
                  }

                }
              });

            }

            function activate() {

              // add watchers for updating placeholde name and alias
              if(scope.updatePlaceholder) {
                eventBindings.push(scope.$watch('name', function(newValue, oldValue){
                  updatePlaceholders();
                }));

                eventBindings.push(scope.$watch('alias', function(newValue, oldValue){
                  updatePlaceholders();
                }));
              }

            }

            activate();

            // clean up
            scope.$on('$destroy', function(){

              // clear watchers
              for(var e in eventBindings) {
                eventBindings[e]();
               }

            });

        }

        var directive = {
            restrict: 'E',
            replace: true,
            templateUrl: 'views/components/umb-grid-selector.html',
            scope: {
              name: "=",
              alias: "=",
              selectedItems: '=',
              availableItems: "=",
              defaultItem: "=",
              itemName: "@",
              updatePlaceholder: "="
            },
            link: link
        };

        return directive;
    }

    angular.module('umbraco.directives').directive('umbGridSelector', GridSelector);

})();

(function() {
  'use strict';

  function GroupsBuilderDirective(contentTypeHelper, contentTypeResource, mediaTypeResource, dataTypeHelper, dataTypeResource, $filter, iconHelper, $q, $timeout, notificationsService, localizationService) {

    function link(scope, el, attr, ctrl) {

        var validationTranslated = "";
        var tabNoSortOrderTranslated = "";

      scope.sortingMode = false;
      scope.toolbar = [];
      scope.sortableOptionsGroup = {};
      scope.sortableOptionsProperty = {};
      scope.sortingButtonKey = "general_reorder";

      function activate() {

          setSortingOptions();

          // set placeholder property on each group
          if (scope.model.groups.length !== 0) {
            angular.forEach(scope.model.groups, function(group) {
              addInitProperty(group);
            });
          }

          // add init tab
          addInitGroup(scope.model.groups);

          activateFirstGroup(scope.model.groups);

          // localize texts
          localizationService.localize("validation_validation").then(function(value) {
              validationTranslated = value;
          });

          localizationService.localize("contentTypeEditor_tabHasNoSortOrder").then(function(value) {
              tabNoSortOrderTranslated = value;
          });
      }

      function setSortingOptions() {

        scope.sortableOptionsGroup = {
          distance: 10,
          tolerance: "pointer",
          opacity: 0.7,
          scroll: true,
          cursor: "move",
          placeholder: "umb-group-builder__group-sortable-placeholder",
          zIndex: 6000,
          handle: ".umb-group-builder__group-handle",
          items: ".umb-group-builder__group-sortable",
          start: function(e, ui) {
            ui.placeholder.height(ui.item.height());
          },
          stop: function(e, ui) {
            updateTabsSortOrder();
          },
        };

        scope.sortableOptionsProperty = {
          distance: 10,
          tolerance: "pointer",
          connectWith: ".umb-group-builder__properties",
          opacity: 0.7,
          scroll: true,
          cursor: "move",
          placeholder: "umb-group-builder__property_sortable-placeholder",
          zIndex: 6000,
          handle: ".umb-group-builder__property-handle",
          items: ".umb-group-builder__property-sortable",
          start: function(e, ui) {
            ui.placeholder.height(ui.item.height());
          },
          stop: function(e, ui) {
            updatePropertiesSortOrder();
          }
        };

      }

      function updateTabsSortOrder() {

        var first = true;
        var prevSortOrder = 0;

        scope.model.groups.map(function(group){

          var index = scope.model.groups.indexOf(group);

          if(group.tabState !== "init") {

            // set the first not inherited tab to sort order 0
            if(!group.inherited && first) {

              // set the first tab sort order to 0 if prev is 0
              if( prevSortOrder === 0 ) {
                group.sortOrder = 0;
              // when the first tab is inherited and sort order is not 0
              } else {
                group.sortOrder = prevSortOrder + 1;
              }

              first = false;

            } else if(!group.inherited && !first) {

                // find next group
                var nextGroup = scope.model.groups[index + 1];

                // if a groups is dropped in the middle of to groups with
                // same sort order. Give it the dropped group same sort order
                if( prevSortOrder === nextGroup.sortOrder ) {
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

            var selectedContentTypeAliases = selecting ?
                //the user has selected the item so add to the current list
                _.union(scope.compositionsDialogModel.compositeContentTypes, [selectedContentType.alias]) :
                //the user has unselected the item so remove from the current list
                _.reject(scope.compositionsDialogModel.compositeContentTypes, function(i) {
                    return i === selectedContentType.alias;
                });

            //get the currently assigned property type aliases - ensure we pass these to the server side filer
            var propAliasesExisting = _.filter(_.flatten(_.map(scope.model.groups, function(g) {
                return _.map(g.properties, function(p) {
                    return p.alias;
                });
            })), function (f) {
                return f !== null && f !== undefined;
            });

            //use a different resource lookup depending on the content type type
            var resourceLookup = scope.contentType === "documentType" ? contentTypeResource.getAvailableCompositeContentTypes : mediaTypeResource.getAvailableCompositeContentTypes;

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
                    current.allowed = scope.model.lockedCompositeContentTypes.indexOf(current.contentType.alias) === -1 &&
                        (selectedContentTypeAliases.indexOf(current.contentType.alias) !== -1) || ((found !== null && found !== undefined) ? found.allowed : false);

                });
            });
        }

      function updatePropertiesSortOrder() {

        angular.forEach(scope.model.groups, function(group){
          if( group.tabState !== "init" ) {
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

      scope.toggleSortingMode = function(tool) {

          if (scope.sortingMode === true) {

              var sortOrderMissing = false;

              for (var i = 0; i < scope.model.groups.length; i++) {
                  var group = scope.model.groups[i];
                  if (group.tabState !== "init" && group.sortOrder === undefined) {
                      sortOrderMissing = true;
                      group.showSortOrderMissing = true;
                      notificationsService.error(validationTranslated + ": " + group.name + " " + tabNoSortOrderTranslated);
                  }
              }

              if (!sortOrderMissing) {
                  scope.sortingMode = false;
                  scope.sortingButtonKey = "general_reorder";
              }

          } else {

              scope.sortingMode = true;
              scope.sortingButtonKey = "general_reorderDone";

          }

      };

      scope.openCompositionsDialog = function() {

        scope.compositionsDialogModel = {
            title: "Compositions",
            contentType: scope.model,
            compositeContentTypes: scope.model.compositeContentTypes,
            view: "views/common/overlays/contenttypeeditor/compositions/compositions.html",
            confirmSubmit: {
                title: "Warning",
                description: "Removing a composition will delete all the associated property data. Once you save the document type there's no way back, are you sure?",
                checkboxLabel: "I know what I'm doing",
                enable: true
            },
            submit: function(model, oldModel, confirmed) {

                var compositionRemoved = false;

                // check if any compositions has been removed
                for(var i = 0; oldModel.compositeContentTypes.length > i; i++) {

                    var oldComposition = oldModel.compositeContentTypes[i];

                    if(_.contains(model.compositeContentTypes, oldComposition) === false) {
                        compositionRemoved = true;
                    }

                }

                // show overlay confirm box if compositions has been removed.
                if(compositionRemoved && confirmed === false) {

                    scope.compositionsDialogModel.confirmSubmit.show = true;

                // submit overlay if no compositions has been removed
                // or the action has been confirmed
                } else {

                    // make sure that all tabs has an init property
                    if (scope.model.groups.length !== 0) {
                      angular.forEach(scope.model.groups, function(group) {
                        addInitProperty(group);
                      });
                    }

                    // remove overlay
                    scope.compositionsDialogModel.show = false;
                    scope.compositionsDialogModel = null;
                }

            },
            close: function(oldModel) {

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
                    var resourceLookup = scope.contentType === "documentType" ? contentTypeResource.getById : mediaTypeResource.getById;

                    resourceLookup(selectedContentType.id).then(function (composition) {
                        //based on the above filtering we shouldn't be able to select an invalid one, but let's be safe and
                        // double check here.
                        var overlappingAliases = contentTypeHelper.validateAddingComposition(scope.model, composition);
                        if (overlappingAliases.length > 0) {
                            //this will create an invalid composition, need to uncheck it
                            scope.compositionsDialogModel.compositeContentTypes.splice(
                                scope.compositionsDialogModel.compositeContentTypes.indexOf(composition.alias), 1);
                            //dissallow this until something else is unchecked
                            selectedContentType.allowed = false;
                        }
                        else {
                            contentTypeHelper.mergeCompositeContentType(scope.model, composition);
                        }

                        //based on the selection, we need to filter the available composite types list
                        filterAvailableCompositions(selectedContentType, newSelection).then(function () {
                            //TODO: Here we could probably re-enable selection if we previously showed a throbber or something
                        });
                    });
                }
                else {
                    // split composition from content type
                    contentTypeHelper.splitCompositeContentType(scope.model, selectedContentType);

                    //based on the selection, we need to filter the available composite types list
                    filterAvailableCompositions(selectedContentType, newSelection).then(function () {
                        //TODO: Here we could probably re-enable selection if we previously showed a throbber or something
                    });
                }

            }
        };

        var availableContentTypeResource = scope.contentType === "documentType" ? contentTypeResource.getAvailableCompositeContentTypes : mediaTypeResource.getAvailableCompositeContentTypes;
        var countContentTypeResource = scope.contentType === "documentType" ? contentTypeResource.getCount : mediaTypeResource.getCount;

          //get the currently assigned property type aliases - ensure we pass these to the server side filer
          var propAliasesExisting = _.filter(_.flatten(_.map(scope.model.groups, function(g) {
              return _.map(g.properties, function(p) {
                  return p.alias;
              });
          })), function(f) {
              return f !== null && f !== undefined;
          });
          $q.all([
              //get available composite types
              availableContentTypeResource(scope.model.id, [], propAliasesExisting).then(function (result) {
                  setupAvailableContentTypesModel(result);
              }),
              //get content type count
              countContentTypeResource().then(function(result) {
                  scope.compositionsDialogModel.totalContentTypes = parseInt(result, 10);
              })
          ]).then(function() {
              //resolves when both other promises are done, now show it
              scope.compositionsDialogModel.show = true;
          });

      };


      /* ---------- GROUPS ---------- */

      scope.addGroup = function(group) {

        // set group sort order
        var index = scope.model.groups.indexOf(group);
        var prevGroup = scope.model.groups[index - 1];

        if( index > 0) {
          // set index to 1 higher than the previous groups sort order
          group.sortOrder = prevGroup.sortOrder + 1;

        } else {
          // first group - sort order will be 0
          group.sortOrder = 0;
        }

        // activate group
        scope.activateGroup(group);

      };

      scope.activateGroup = function(selectedGroup) {

        // set all other groups that are inactive to active
        angular.forEach(scope.model.groups, function(group) {
          // skip init tab
          if (group.tabState !== "init") {
            group.tabState = "inActive";
          }
        });

        selectedGroup.tabState = "active";

      };

      scope.removeGroup = function(groupIndex) {
        scope.model.groups.splice(groupIndex, 1);
        addInitGroup(scope.model.groups);
      };

      scope.updateGroupTitle = function(group) {
        if (group.properties.length === 0) {
          addInitProperty(group);
        }
      };

      scope.changeSortOrderValue = function(group) {

          if (group.sortOrder !== undefined) {
              group.showSortOrderMissing = false;
          }
          scope.model.groups = $filter('orderBy')(scope.model.groups, 'sortOrder');
      };

      function addInitGroup(groups) {

        // check i init tab already exists
        var addGroup = true;

        angular.forEach(groups, function(group) {
          if (group.tabState === "init") {
            addGroup = false;
          }
        });

        if (addGroup) {
          groups.push({
            properties: [],
            parentTabContentTypes: [],
            parentTabContentTypeNames: [],
            name: "",
            tabState: "init"
          });
        }

        return groups;
      }

      function activateFirstGroup(groups) {
          if (groups && groups.length > 0) {
              var firstGroup = groups[0];
              if(!firstGroup.tabState || firstGroup.tabState === "inActive") {
                  firstGroup.tabState = "active";
              }
          }
      }

      /* ---------- PROPERTIES ---------- */

      scope.addProperty = function(property, group) {

        // set property sort order
        var index = group.properties.indexOf(property);
        var prevProperty = group.properties[index - 1];

        if( index > 0) {
          // set index to 1 higher than the previous property sort order
          property.sortOrder = prevProperty.sortOrder + 1;

        } else {
          // first property - sort order will be 0
          property.sortOrder = 0;
        }

        // open property settings dialog
        scope.editPropertyTypeSettings(property, group);

      };

      scope.editPropertyTypeSettings = function(property, group) {

        if (!property.inherited && !property.locked) {

          scope.propertySettingsDialogModel = {};
          scope.propertySettingsDialogModel.title = "Property settings";
          scope.propertySettingsDialogModel.property = property;
          scope.propertySettingsDialogModel.contentType = scope.contentType;
          scope.propertySettingsDialogModel.contentTypeName = scope.model.name;
          scope.propertySettingsDialogModel.view = "views/common/overlays/contenttypeeditor/propertysettings/propertysettings.html";
          scope.propertySettingsDialogModel.show = true;

          // set state to active to access the preview
          property.propertyState = "active";

          // set property states
          property.dialogIsOpen = true;

          scope.propertySettingsDialogModel.submit = function(model) {

            property.inherited = false;
            property.dialogIsOpen = false;

            // update existing data types
            if(model.updateSameDataTypes) {
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

          scope.propertySettingsDialogModel.close = function(oldModel) {

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

            // because we set state to active, to show a preview, we have to check if has been filled out
            // label is required so if it is not filled we know it is a placeholder
            if(oldModel.property.editor === undefined || oldModel.property.editor === null || oldModel.property.editor === "") {
              property.propertyState = "init";
            } else {
              property.propertyState = oldModel.property.propertyState;
            }

            // remove dialog
            scope.propertySettingsDialogModel.show = false;
            scope.propertySettingsDialogModel = null;

          };

        }
      };

      scope.deleteProperty = function(tab, propertyIndex) {

        // remove property
        tab.properties.splice(propertyIndex, 1);

        // if the last property in group is an placeholder - remove add new tab placeholder
        if(tab.properties.length === 1 && tab.properties[0].propertyState === "init") {

          angular.forEach(scope.model.groups, function(group, index, groups){
            if(group.tabState === 'init') {
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
          propertyState: "init",
          validation: {
            mandatory: false,
            pattern: null
          }
        };

        // check if there already is an init property
        angular.forEach(group.properties, function(property) {
          if (property.propertyState === "init") {
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
        angular.forEach(scope.model.groups, function(group){
          angular.forEach(group.properties, function(property){

            if(property.dataTypeId === newProperty.dataTypeId) {

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


      var unbindModelWatcher = scope.$watch('model', function(newValue, oldValue) {
        if (newValue !== undefined && newValue.groups !== undefined) {
          activate();
        }
      });

      // clean up
      scope.$on('$destroy', function(){
        unbindModelWatcher();
      });

    }

    var directive = {
      restrict: "E",
      replace: true,
      templateUrl: "views/components/umb-groups-builder.html",
      scope: {
        model: "=",
        compositions: "=",
        sorting: "=",
        contentType: "@"
      },
      link: link
    };

    return directive;
  }

  angular.module('umbraco.directives').directive('umbGroupsBuilder', GroupsBuilderDirective);

})();

(function() {
  'use strict';

  function KeyboardShortcutsOverviewDirective() {

    function link(scope, el, attr, ctrl) {

      scope.shortcutOverlay = false;

      scope.toggleShortcutsOverlay = function() {
        scope.shortcutOverlay = !scope.shortcutOverlay;
      };

    }

    var directive = {
      restrict: 'E',
      replace: true,
      templateUrl: 'views/components/umb-keyboard-shortcuts-overview.html',
      link: link,
      scope: {
        model: "="
      }
    };

    return directive;
  }

  angular.module('umbraco.directives').directive('umbKeyboardShortcutsOverview', KeyboardShortcutsOverviewDirective);

})();

/**
* @ngdoc directive
* @name umbraco.directives.directive:umbLaunchMiniEditor 
* @restrict E
* @function
* @description 
* Used on a button to launch a mini content editor editor dialog
**/
angular.module("umbraco.directives")
    .directive('umbLaunchMiniEditor', function (dialogService, editorState, fileManager, contentEditingHelper) {
        return {
            restrict: 'A',
            replace: false,
            scope: {
                node: '=umbLaunchMiniEditor',
            },
            link: function(scope, element, attrs) {

                var launched = false;

                element.click(function() {

                    if (launched === true) {
                        return;
                    }

                    launched = true;

                    //We need to store the current files selected in the file manager locally because the fileManager
                    // is a singleton and is shared globally. The mini dialog will also be referencing the fileManager 
                    // and we don't want it to be sharing the same files as the main editor. So we'll store the current files locally here,
                    // clear them out and then launch the dialog. When the dialog closes, we'll reset the fileManager to it's previous state.
                    var currFiles = _.groupBy(fileManager.getFiles(), "alias");
                    fileManager.clearFiles();

                    //We need to store the original editorState entity because it will need to change when the mini editor is loaded so that
                    // any property editors that are working with editorState get given the correct entity, otherwise strange things will 
                    // start happening.
                    var currEditorState = editorState.getCurrent();

                    dialogService.open({
                        template: "views/common/dialogs/content/edit.html",
                        id: scope.node.id,
                        closeOnSave: true,
                        tabFilter: ["Generic properties"],
                        callback: function (data) {

                            //set the node name back
                            scope.node.name = data.name;

                            //reset the fileManager to what it was
                            fileManager.clearFiles();
                            _.each(currFiles, function (val, key) {
                                fileManager.setFiles(key, _.map(currFiles['upload'], function (i) { return i.file; }));
                            });

                            //reset the editor state
                            editorState.set(currEditorState);

                            //Now we need to check if the content item that was edited was actually the same content item
                            // as the main content editor and if so, update all property data	                
                            if (data.id === currEditorState.id) {
                                var changed = contentEditingHelper.reBindChangedProperties(currEditorState, data);
                            }

                            launched = false;
                        },
                        closeCallback: function () {
                            //reset the fileManager to what it was
                            fileManager.clearFiles();
                            _.each(currFiles, function (val, key) {
                                fileManager.setFiles(key, _.map(currFiles['upload'], function (i) { return i.file; }));
                            });

                            //reset the editor state
                            editorState.set(currEditorState);

                            launched = false;
                        }
                    });

                });

            }
        };
    });
(function() {
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

            if(numberOfAllowedLayouts === 1) {
               scope.showLayoutSelector = false;
            }

         }

         function getNumberOfAllowedLayouts(layouts) {

            var allowedLayouts = 0;

            for (var i = 0; layouts.length > i; i++) {

               var layout = layouts[i];

               if(layout.selected === true) {
                  allowedLayouts++;
               }

            }

            return allowedLayouts;
         }

         function setActiveLayout(layouts) {

            for (var i = 0; layouts.length > i; i++) {
               var layout = layouts[i];
               if(layout.path === scope.activeLayout.path) {
                  layout.active = true;
               }
            }

         }

         scope.pickLayout = function(selectedLayout) {
             if(scope.onLayoutSelect) {
                 scope.onLayoutSelect(selectedLayout);
                 scope.layoutDropDownIsOpen = false;
             }
         };

         scope.toggleLayoutDropdown = function() {
            scope.layoutDropDownIsOpen = !scope.layoutDropDownIsOpen;
         };

         scope.closeLayoutDropdown = function() {
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
            onLayoutSelect: "="
         },
         link: link
      };

      return directive;
   }

   angular.module('umbraco.directives').directive('umbLayoutSelector', LayoutSelectorDirective);

})();

(function() {
   'use strict';

   function ListViewLayoutDirective() {

      function link(scope, el, attr, ctrl) {

         scope.getContent = function(contentId) {
            if(scope.onGetContent) {
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

})();

(function() {
  'use strict';

  function ListViewSettingsDirective(contentTypeResource, dataTypeResource, dataTypeHelper) {

    function link(scope, el, attr, ctrl) {

      scope.dataType = {};
      scope.editDataTypeSettings = false;
      scope.customListViewCreated = false;

      /* ---------- INIT ---------- */

      function activate() {

        if(scope.enableListView) {

          dataTypeResource.getByName(scope.listViewName)
            .then(function(dataType) {

              scope.dataType = dataType;

              scope.customListViewCreated = checkForCustomListView();

            });

        } else {

          scope.dataType = {};

        }

      }

      /* ----------- LIST VIEW SETTINGS --------- */

      scope.toggleEditListViewDataTypeSettings = function() {
        scope.editDataTypeSettings = !scope.editDataTypeSettings;
      };

      scope.saveListViewDataType = function() {

          var preValues = dataTypeHelper.createPreValueProps(scope.dataType.preValues);

          dataTypeResource.save(scope.dataType, preValues, false).then(function(dataType) {

              // store data type
              scope.dataType = dataType;

              // hide settings panel
              scope.editDataTypeSettings = false;

          });

      };


      /* ---------- CUSTOM LIST VIEW ---------- */

      scope.createCustomListViewDataType = function() {

          dataTypeResource.createCustomListView(scope.modelAlias).then(function(dataType) {

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

      scope.removeCustomListDataType = function() {

          scope.editDataTypeSettings = false;

          // delete custom list view data type
          dataTypeResource.deleteById(scope.dataType.id).then(function(dataType) {

              // set list view name on scope
              if(scope.contentType === "documentType") {

                 scope.listViewName = "List View - Content";

              } else if(scope.contentType === "mediaType") {

                 scope.listViewName = "List View - Media";

              }

              // get default data type
              dataTypeResource.getByName(scope.listViewName)
                  .then(function(dataType) {

                      // store data type
                      scope.dataType = dataType;

                      // change state to default list view
                      scope.customListViewCreated = false;

                  });
          });

      };

      /* ----------- SCOPE WATCHERS ----------- */
      var unbindEnableListViewWatcher = scope.$watch('enableListView', function(newValue, oldValue){

        if(newValue !== undefined) {
          activate();
        }

      });

      // clean up
      scope.$on('$destroy', function(){
        unbindEnableListViewWatcher();
      });

      /* ----------- METHODS ---------- */

      function checkForCustomListView() {
          return scope.dataType.name === "List View - " + scope.modelAlias;
      }

    }

    var directive = {
      restrict: 'E',
      replace: true,
      templateUrl: 'views/components/umb-list-view-settings.html',
      scope: {
        enableListView: "=",
        listViewName: "=",
        modelAlias: "=",
        contentType: "@"
      },
      link: link
    };

    return directive;
  }

  angular.module('umbraco.directives').directive('umbListViewSettings', ListViewSettingsDirective);

})();

(function() {
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

})();

/**
* @ngdoc directive
* @name umbraco.directives.directive:umbContentName 
* @restrict E
* @function
* @description 
* Used by editors that require naming an entity. Shows a textbox/headline with a required validator within it's own form.
**/
(function() {
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
				    scope.regexValidation = "^[a-zA-Z]\\w.*$";
				}

				if (scope.serverValidationField === undefined || scope.serverValidationField === null) {
				    scope.serverValidationField = "";
				}

				// if locked state is not defined as an attr set default state
				if (scope.placeholderText === undefined || scope.placeholderText === null) {
					scope.placeholderText = "Enter value...";
				}

			}

			scope.lock = function() {
				scope.locked = true;
			};

			scope.unlock = function() {
				scope.locked = false;
			};

			activate();

		}

		var directive = {
			require: "ngModel",
			restrict: 'E',
			replace: true,
			templateUrl: 'views/components/umb-locked-field.html',
			scope: {
			    ngModel: "=",
				locked: "=?",
				placeholderText: "=?",
				regexValidation: "=?",
				serverValidationField: "@"
			},
			link: link
		};

		return directive;

	}

	angular.module('umbraco.directives').directive('umbLockedField', LockedFieldDirective);

})();

(function() {
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

                if (scope.itemMinWidth) {
                    itemMinHeight = scope.itemMinHeight;
                }

                for (var i = 0; scope.items.length > i; i++) {
                    var item = scope.items[i];
                    setItemData(item);
                    setOriginalSize(item, itemMaxHeight);
                }

                if (scope.items.length > 0) {
                    setFlexValues(scope.items);
                }

            }

            function setItemData(item) {
                item.isFolder = !mediaHelper.hasFilePropertyType(item);
                if (!item.isFolder) {
                    item.thumbnail = mediaHelper.resolveFile(item, true);
                    item.image = mediaHelper.resolveFile(item, false);
                }
            }

            function setOriginalSize(item, maxHeight) {

                //set to a square by default
                item.width = itemDefaultWidth;
                item.height = itemDefaultHeight;
                item.aspectRatio = 1;

                var widthProp = _.find(item.properties, function(v) {
                    return (v.alias === "umbracoWidth");
                });

                if (widthProp && widthProp.value) {
                    item.width = parseInt(widthProp.value, 10);
                    if (isNaN(item.width)) {
                        item.width = itemDefaultWidth;
                    }
                }

                var heightProp = _.find(item.properties, function(v) {
                    return (v.alias === "umbracoHeight");
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
                    }
                    // portrait
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
                        "flex": flex + " 1 " + imageMinFlexWidth + "px",
                        "max-width": mediaItem.width + "px",
                        "min-width": itemMinWidth + "px",
                        "min-height": itemMinHeight + "px"
                    };

                    mediaItem.flexStyle = flexStyle;

                }

            }

            scope.clickItem = function(item, $event, $index) {
                if (scope.onClick) {
                    scope.onClick(item, $event, $index);
                }
            };

            scope.clickItemName = function(item, $event, $index) {
                if (scope.onClickName) {
                    scope.onClickName(item, $event, $index);
                    $event.stopPropagation();
                }
            };

            scope.hoverItemDetails = function(item, $event, hover) {
                if (scope.onDetailsHover) {
                    scope.onDetailsHover(item, $event, hover);
                }
            };

            var unbindItemsWatcher = scope.$watch('items', function(newValue, oldValue) {
                if (angular.isArray(newValue)) {
                    activate();
                }
            });

            scope.$on('$destroy', function() {
                unbindItemsWatcher();
            });

        }

        var directive = {
            restrict: 'E',
            replace: true,
            templateUrl: 'views/components/umb-media-grid.html',
            scope: {
                items: '=',
                onDetailsHover: "=",
                onClick: '=',
                onClickName: "=",
                filterBy: "=",
                itemMaxWidth: "@",
                itemMaxHeight: "@",
                itemMinWidth: "@",
                itemMinHeight: "@"
            },
            link: link
        };

        return directive;
    }

    angular.module('umbraco.directives').directive('umbMediaGrid', MediaGridDirective);

})();

(function() {
   'use strict';

   function PaginationDirective() {

      function link(scope, el, attr, ctrl) {

         function activate() {

            scope.pagination = [];

            var i = 0;

            if (scope.totalPages <= 10) {
                for (i = 0; i < scope.totalPages; i++) {
                    scope.pagination.push({
                        val: (i + 1),
                        isActive: scope.pageNumber === (i + 1)
                    });
                }
            }
            else {
                //if there is more than 10 pages, we need to do some fancy bits

                //get the max index to start
                var maxIndex = scope.totalPages - 10;
                //set the start, but it can't be below zero
                var start = Math.max(scope.pageNumber - 5, 0);
                //ensure that it's not too far either
                start = Math.min(maxIndex, start);

                for (i = start; i < (10 + start) ; i++) {
                    scope.pagination.push({
                        val: (i + 1),
                        isActive: scope.pageNumber === (i + 1)
                    });
                }

                //now, if the start is greater than 0 then '1' will not be displayed, so do the elipses thing
                if (start > 0) {
                    scope.pagination.unshift({ name: "First", val: 1, isActive: false }, {val: "...",isActive: false});
                }

                //same for the end
                if (start < maxIndex) {
                    scope.pagination.push({ val: "...", isActive: false }, { name: "Last", val: scope.totalPages, isActive: false });
                }
            }

         }

         scope.next = function() {
            if (scope.onNext && scope.pageNumber < scope.totalPages) {
               scope.pageNumber++;
               scope.onNext(scope.pageNumber);
            }
         };

         scope.prev = function(pageNumber) {
            if (scope.onPrev && scope.pageNumber > 1) {
                scope.pageNumber--;
                scope.onPrev(scope.pageNumber);
            }
         };

         scope.goToPage = function(pageNumber) {
            if(scope.onGoToPage) {
               scope.pageNumber = pageNumber + 1;
               scope.onGoToPage(scope.pageNumber);
            }
         };

         var unbindPageNumberWatcher = scope.$watch('pageNumber', function(newValue, oldValue){
            activate();
         });

         scope.$on('$destroy', function(){
           unbindPageNumberWatcher();
         });

         activate();

      }

      var directive = {
         restrict: 'E',
         replace: true,
         templateUrl: 'views/components/umb-pagination.html',
         scope: {
            pageNumber: "=",
            totalPages: "=",
            onNext: "=",
            onPrev: "=",
            onGoToPage: "="
         },
         link: link
      };

      return directive;

   }

   angular.module('umbraco.directives').directive('umbPagination', PaginationDirective);

})();

(function() {
   'use strict';

   function StickyBarDirective($rootScope) {

      function link(scope, el, attr, ctrl) {

         var bar = $(el);
         var scrollableContainer = null;
         var clonedBar = null;
         var cloneIsMade = false;
         var barTop = bar.context.offsetTop;

         function activate() {

            if (attr.scrollableContainer) {
               scrollableContainer = $(attr.scrollableContainer);
            } else {
               scrollableContainer = $(window);
            }

            scrollableContainer.on('scroll.umbStickyBar', determineVisibility).trigger("scroll");
            $(window).on('resize.umbStickyBar', determineVisibility);

            scope.$on('$destroy', function() {
               scrollableContainer.off('.umbStickyBar');
               $(window).off('.umbStickyBar');
            });

         }

         function determineVisibility() {

            var scrollTop = scrollableContainer.scrollTop();

            if (scrollTop > barTop) {

               if (!cloneIsMade) {

                  createClone();

                  clonedBar.css({
                     'visibility': 'visible'
                  });

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

})();

(function() {
   'use strict';

   function TableDirective() {

      function link(scope, el, attr, ctrl) {

         scope.clickItem = function(item, $event) {
            if(scope.onClick) {
               scope.onClick(item);
               $event.stopPropagation();
            }
         };

        scope.selectItem = function(item, $index, $event) {
            if(scope.onSelect) {
               scope.onSelect(item, $index, $event);
               $event.stopPropagation();
            }
        };

        scope.selectAll = function($event) {
            if(scope.onSelectAll) {
                scope.onSelectAll($event);
            }
        };

        scope.isSelectedAll = function() {
            if(scope.onSelectedAll && scope.items && scope.items.length > 0) {
                return scope.onSelectedAll();
            }
        };

        scope.isSortDirection = function (col, direction) {
            if (scope.onSortingDirection) {
                return scope.onSortingDirection(col, direction);
            }
        };

        scope.sort = function(field, allow) {
            if(scope.onSort) {
                scope.onSort(field, allow);
            }
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

})();

(function() {
   'use strict';

   function TooltipDirective($timeout) {

      function link(scope, el, attr, ctrl) {

         scope.tooltipStyles = {};
         scope.tooltipStyles.left = 0;
         scope.tooltipStyles.top = 0;

         function activate() {

            $timeout(function() {
               setTooltipPosition(scope.event);
            });

         }

         function setTooltipPosition(event) {

            var container = $("#contentwrapper");
            var containerLeft = container[0].offsetLeft;
            var containerRight = containerLeft + container[0].offsetWidth;
            var containerTop = container[0].offsetTop;
            var containerBottom = containerTop + container[0].offsetHeight;

            var elementHeight = null;
            var elementWidth = null;

            var position = {
               right: "inherit",
               left: "inherit",
               top: "inherit",
               bottom: "inherit"
            };

            // element size
            elementHeight = el.context.clientHeight;
            elementWidth = el.context.clientWidth;

            position.left = event.pageX - (elementWidth / 2);
            position.top = event.pageY;

            // check to see if element is outside screen
            // outside right
            if (position.left + elementWidth > containerRight) {
               position.right = 10;
               position.left = "inherit";
            }

            // outside bottom
            if (position.top + elementHeight > containerBottom) {
               position.bottom = 10;
               position.top = "inherit";
            }

            // outside left
            if (position.left < containerLeft) {
               position.left = containerLeft + 10;
               position.right = "inherit";
            }

            // outside top
            if (position.top < containerTop) {
               position.top = 10;
               position.bottom = "inherit";
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
         scope: {
            event: "="
         },
         link: link
      };

      return directive;
   }

   angular.module('umbraco.directives').directive('umbTooltip', TooltipDirective);

})();

/**
* @ngdoc directive
* @name umbraco.directives.directive:umbContentName
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

angular.module("umbraco.directives")

.directive('umbFileDropzone', function ($timeout, Upload, localizationService, umbRequestHelper) {
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

			filesQueued: '=',
			handleFile: '=',
			filesUploaded: '='
		},

		link: function(scope, element, attrs) {

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
				if(ignoreFileNames.indexOf(file.name) === -1 &&
					ignoreFileTypes.indexOf(file.type) === -1 &&
					file.name.indexOf(".") !== 0) {
					return true;
				} else {
					return false;
				}

			}

			function _filesQueued(files, event){

				//Push into the queue
				angular.forEach(files, function(file){

					if(_filterFile(file) === true) {

						if(file.$error) {
							scope.rejected.push(file);
						} else {
							scope.queue.push(file);
						}

					}

				});

				//when queue is done, kick the uploader
				if(!scope.working){
					_processQueueItem();
				}
			}


			function _processQueueItem(){

				if(scope.queue.length > 0){
					scope.currentFile = scope.queue.shift();
					_upload(scope.currentFile);
				}else if(scope.done.length > 0){

					if(scope.filesUploaded){
						//queue is empty, trigger the done action
						scope.filesUploaded(scope.done);
					}

					//auto-clear the done queue after 3 secs
					var currentLength = scope.done.length;
					$timeout(function(){
						scope.done.splice(0, currentLength);
					}, 3000);
				}
			}

			function _upload(file) {

				scope.propertyAlias = scope.propertyAlias ? scope.propertyAlias : "umbracoFile";
				scope.contentTypeAlias = scope.contentTypeAlias ? scope.contentTypeAlias : "Image";

				Upload.upload({
					url: umbRequestHelper.getApiUrl("mediaApiBaseUrl", "PostAddFile"),
					fields: {
						'currentFolder': scope.parentId,
						'contentTypeAlias': scope.contentTypeAlias,
						'propertyAlias': scope.propertyAlias,
						'path': file.path
					},
					file: file
				}).progress(function (evt) {

					// calculate progress in percentage
					var progressPercentage = parseInt(100.0 * evt.loaded / evt.total, 10);

					// set percentage property on file
					file.uploadProgress = progressPercentage;

					// set uploading status on file
					file.uploadStatus = "uploading";

				}).success(function (data, status, headers, config) {

					if(data.notifications && data.notifications.length > 0) {

						// set error status on file
						file.uploadStatus = "error";

						// Throw message back to user with the cause of the error
						file.serverErrorMessage = data.notifications[0].message;

						// Put the file in the rejected pool
						scope.rejected.push(file);

					} else {

						// set done status on file
						file.uploadStatus = "done";

						// set date/time for when done - used for sorting
						file.doneDate = new Date();

						// Put the file in the done pool
						scope.done.push(file);

					}

					scope.currentFile = undefined;

					//after processing, test if everthing is done
					_processQueueItem();

				}).error( function (evt, status, headers, config) {

					// set status done
					file.uploadStatus = "error";

					//if the service returns a detailed error
					if (evt.InnerException) {
					    file.serverErrorMessage = evt.InnerException.ExceptionMessage;

					    //Check if its the common "too large file" exception
					    if (evt.InnerException.StackTrace && evt.InnerException.StackTrace.indexOf("ValidateRequestEntityLength") > 0) {
					        file.serverErrorMessage = "File too large to upload";
					    }

					} else if (evt.Message) {
					    file.serverErrorMessage = evt.Message;
					}

					// If file not found, server will return a 404 and display this message
					if(status === 404 ) {
						file.serverErrorMessage = "File not found";
					}

					//after processing, test if everthing is done
					scope.rejected.push(file);
					scope.currentFile = undefined;

					_processQueueItem();
				});
			}


			scope.handleFiles = function(files, event){
				if(scope.filesQueued){
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
        restrict: "A",
        scope: true,        //create a new scope
        link: function (scope, el, attrs) {
            el.bind('change', function (event) {
                var files = event.target.files;
                //emit event upward
                scope.$emit("filesSelected", { files: files });                           
            });
        }
    };
}

angular.module('umbraco.directives').directive("umbFileUpload", umbFileUpload);
/**
* @ngdoc directive
* @name umbraco.directives.directive:umbFileUpload
* @function
* @restrict A
* @scope
* @description
*  A single file upload field that will reset itself based on the object passed in for the rebuild parameter. This
*  is required because the only way to reset an upload control is to replace it's html.
**/
function umbSingleFileUpload($compile) {
    return {
        restrict: "E",
        scope: {
            rebuild: "="
        },
        replace: true,
        template: "<div><input type='file' umb-file-upload /></div>",
        link: function (scope, el, attrs) {

            scope.$watch("rebuild", function (newVal, oldVal) {
                if (newVal && newVal !== oldVal) {
                    //recompile it!
                    el.html("<input type='file' umb-file-upload />");
                    $compile(el.contents())(scope);
                }
            });

        }
    };
}

angular.module('umbraco.directives').directive("umbSingleFileUpload", umbSingleFileUpload);
/**
* @ngdoc directive
* @name umbraco.directives.directive:noDirtyCheck
* @restrict A
* @description Can be attached to form inputs to prevent them from setting the form as dirty (http://stackoverflow.com/questions/17089090/prevent-input-from-setting-form-dirty-angularjs)
**/
function noDirtyCheck() {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elm, attrs, ctrl) {
            elm.focus(function () {
                ctrl.$pristine = false;
            });
        }
    };
}
angular.module('umbraco.directives.validation').directive("noDirtyCheck", noDirtyCheck);
(function() {
    'use strict';

    function SetDirtyOnChange() {

        function link(scope, el, attr, ctrl) {

            var initValue = attr.umbSetDirtyOnChange;

            attr.$observe("umbSetDirtyOnChange", function (newValue) {
                if(newValue !== initValue) {
                    ctrl.$setDirty();
                }
            });

        }

        var directive = {
            require: "^form",
            restrict: 'A',
            link: link
        };

        return directive;
    }

    angular.module('umbraco.directives').directive('umbSetDirtyOnChange', SetDirtyOnChange);

})();

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
angular.module('umbraco.directives.validation')
.directive('valCustom', function () {

  return {
    restrict: 'A',
    require: 'ngModel',
    link: function (scope, elm, attrs, ctrl) {
      var validateFn, watch, validators = {},
        validateExpr = scope.$eval(attrs.valCustom);

      if (!validateExpr){ return;}

      if (angular.isString(validateExpr)) {
        validateExpr = { validator: validateExpr };
      }

      angular.forEach(validateExpr, function (exprssn, key) {
        validateFn = function (valueToValidate) {
          var expression = scope.$eval(exprssn, { '$value' : valueToValidate });
          if (angular.isObject(expression) && angular.isFunction(expression.then)) {
            // expression is a promise
            expression.then(function(){
              ctrl.$setValidity(key, true);
            }, function(){
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

      function apply_watch(watch)
      {
          //string - update all validators on expression change
          if (angular.isString(watch))
          {
              scope.$watch(watch, function(){
                  angular.forEach(validators, function(validatorFn){
                      validatorFn(ctrl.$modelValue);
                  });
              });
              return;
          }

          //array - update all validators on change of any expression
          if (angular.isArray(watch))
          {
              angular.forEach(watch, function(expression){
                  scope.$watch(expression, function()
                  {
                      angular.forEach(validators, function(validatorFn){
                          validatorFn(ctrl.$modelValue);
                      });
                  });
              });
              return;
          }

          //object - update appropriate validator
          if (angular.isObject(watch))
          {
              angular.forEach(watch, function(expression, validatorKey)
              {
                  //value is string - look after one expression
                  if (angular.isString(expression))
                  {
                      scope.$watch(expression, function(){
                          validators[validatorKey](ctrl.$modelValue);
                      });
                  }

                  //value is array - look after all expressions in array
                  if (angular.isArray(expression))
                  {
                      angular.forEach(expression, function(intExpression)
                      {
                          scope.$watch(intExpression, function(){
                              validators[validatorKey](ctrl.$modelValue);
                          });
                      });
                  }
              });
          }
      }
      // Support for val-custom-watch
      if (attrs.valCustomWatch){
          apply_watch( scope.$eval(attrs.valCustomWatch) );
      }
    }
  };
});
/**
* @ngdoc directive
* @name umbraco.directives.directive:valHighlight
* @restrict A
* @description Used on input fields when you want to signal that they are in error, this will highlight the item for 1 second
**/
function valHighlight($timeout) {
    return {
        restrict: "A",
        link: function (scope, element, attrs, ctrl) {

            attrs.$observe("valHighlight", function (newVal) {
                if (newVal === "true") {
                    element.addClass("highlight-error");
                    $timeout(function () {
                        //set the bound scope property to false
                        scope[attrs.valHighlight] = false;
                    }, 1000);
                }
                else {
                    element.removeClass("highlight-error");
                }
            });

        }
    };
}
angular.module('umbraco.directives.validation').directive("valHighlight", valHighlight);

angular.module('umbraco.directives.validation')
	.directive('valCompare',function () {
	return {
	        require: "ngModel", 
	        link: function (scope, elem, attrs, ctrl) {
	            
	            //TODO: Pretty sure this should be done using a requires ^form in the directive declaration	            
	            var otherInput = elem.inheritedData("$formController")[attrs.valCompare];

	            ctrl.$parsers.push(function(value) {
	                if(value === otherInput.$viewValue) {
	                    ctrl.$setValidity("valCompare", true);
	                    return value;
	                }
	                ctrl.$setValidity("valCompare", false);
	            });

	            otherInput.$parsers.push(function(value) {
	                ctrl.$setValidity("valCompare", value === ctrl.$viewValue);
	                return value;
	            });
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
        restrict: "A",
        link: function (scope, elm, attrs, ctrl) {
            
            var patternValidator = function (viewValue) {
                //NOTE: we don't validate on empty values, use required validator for that
                if (!viewValue || valEmailExpression.EMAIL_REGEXP.test(viewValue)) {
                    // it is valid
                    ctrl.$setValidity('valEmail', true);
                    //assign a message to the validator
                    ctrl.errorMsg = "";
                    return viewValue;
                }
                else {
                    // it is invalid, return undefined (no model update)
                    ctrl.$setValidity('valEmail', false);
                    //assign a message to the validator
                    ctrl.errorMsg = "Invalid email";
                    return undefined;
                }
            };

            ctrl.$parsers.push(patternValidator);
        }
    };
}

angular.module('umbraco.directives.validation')
    .directive("valEmail", valEmail)
    .factory('valEmailExpression', function() {
        return {
            EMAIL_REGEXP: /^[a-z0-9!#$%&'*+\/=?^_`{|}~.-]+@[a-z0-9]([a-z0-9-]*[a-z0-9])?(\.[a-z0-9]([a-z0-9-]*[a-z0-9])?)*$/i
        };
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
        require: "form",
        restrict: "A",
        controller: function($scope) {
            //This exposes an API for direct use with this directive

            var unsubscribe = [];
            var self = this;

            //This is basically the same as a directive subscribing to an event but maybe a little
            // nicer since the other directive can use this directive's API instead of a magical event
            this.onValidationStatusChanged = function (cb) {
                unsubscribe.push($scope.$on("valStatusChanged", function(evt, args) {
                    cb.apply(self, [evt, args]);
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
                scope.$broadcast("valStatusChanged", { form: formCtrl });
                
                //find all invalid elements' .control-group's and apply the error class
                var inError = element.find(".control-group .ng-invalid").closest(".control-group");
                inError.addClass("error");

                //find all control group's that have no error and ensure the class is removed
                var noInError = element.find(".control-group .ng-valid").closest(".control-group").not(inError);
                noInError.removeClass("error");

            }, true);
            
            var className = attr.valShowValidation ? attr.valShowValidation : "show-validation";
            var savingEventName = attr.savingEvent ? attr.savingEvent : "formSubmitting";
            var savedEvent = attr.savedEvent ? attr.savingEvent : "formSubmitted";

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
            unsubscribe.push(scope.$on(savingEventName, function(ev, args) {
                element.addClass(className);

                //set the flag so we can check to see if we should display the error.
                isSavingNewItem = $routeParams.create;
            }));

            //listen for the forms saved event
            unsubscribe.push(scope.$on(savedEvent, function(ev, args) {
                //remove validation class
                element.removeClass(className);

                //clear form state as at this point we retrieve new data from the server
                //and all validation will have cleared at this point    
                formCtrl.$setPristine();
            }));

            //This handles the 'unsaved changes' dialog which is triggered when a route is attempting to be changed but
            // the form has pending changes
            var locationEvent = $rootScope.$on('$locationChangeStart', function(event, nextLocation, currentLocation) {
                if (!formCtrl.$dirty || isSavingNewItem) {
                    return;
                }

                var path = nextLocation.split("#")[1];
                if (path) {
                    if (path.indexOf("%253") || path.indexOf("%252")) {
                        path = decodeURIComponent(path);
                    }

                    if (!notificationsService.hasView()) {
                        var msg = { view: "confirmroutechange", args: { path: path, listener: locationEvent } };
                        notificationsService.add(msg);
                    }

                    //prevent the route!
                    event.preventDefault();

                    //raise an event
                    eventsService.emit("valFormManager.pendingChanges", true);
                }

            });
            unsubscribe.push(locationEvent);

            //Ensure to remove the event handler when this instance is destroyted
            scope.$on('$destroy', function() {
                for (var u in unsubscribe) {
                    unsubscribe[u]();
                }
            });

            $timeout(function(){
                formCtrl.$setPristine();
            }, 1000);
        }
    };
}
angular.module('umbraco.directives.validation').directive("valFormManager", valFormManager);
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
        scope: {
            property: "="
        },
        require: "^form",   //require that this directive is contained within an ngForm
        replace: true,      //replace the element with the template
        restrict: "E",      //restrict to element
        template: "<div ng-show=\"errorMsg != ''\" class='alert alert-error property-error' >{{errorMsg}}</div>",

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
                    var err = serverValidationManager.getPropertyError(scope.property.alias, "");
                    //if there's an error message use it
                    if (err && err.errorMsg) {
                        return err.errorMsg;
                    }
                    else {
                        return scope.property.propertyErrorMessage ? scope.property.propertyErrorMessage : "Property has errors";
                    }

                }
                return "Property has errors";
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
                    watcher = scope.$watch("property.value", function (newValue, oldValue) {
                        
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

                        if ((errCount === 1 && angular.isArray(formCtrl.$error.valPropertyMsg)) || (formCtrl.$invalid && angular.isArray(formCtrl.$error.valServer))) {
                            scope.errorMsg = "";
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
            scope.errorMsg = "";

            var unsubscribe = [];

            //listen for form error changes
            unsubscribe.push(scope.$on("valStatusChanged", function(evt, args) {
                if (args.form.$invalid) {

                    //first we need to check if the valPropertyMsg validity is invalid
                    if (formCtrl.$error.valPropertyMsg && formCtrl.$error.valPropertyMsg.length > 0) {
                        //since we already have an error we'll just return since this means we've already set the 
                        // hasError and errorMsg properties which occurs below in the serverValidationManager.subscribe
                        return;
                    }
                    else if (element.closest(".umb-control-group").find(".ng-invalid").length > 0) {
                        //check if it's one of the properties that is invalid in the current content property
                        hasError = true;
                        //update the validation message if we don't already have one assigned.
                        if (showValidation && scope.errorMsg === "") {
                            scope.errorMsg = getErrorMsg();
                        }
                    }
                    else {
                        hasError = false;
                        scope.errorMsg = "";
                    }
                }
                else {
                    hasError = false;
                    scope.errorMsg = "";
                }
            }, true));

            //listen for the forms saving event
            unsubscribe.push(scope.$on("formSubmitting", function(ev, args) {
                showValidation = true;
                if (hasError && scope.errorMsg === "") {
                    scope.errorMsg = getErrorMsg();
                }
                else if (!hasError) {
                    scope.errorMsg = "";
                    stopWatch();
                }
            }));

            //listen for the forms saved event
            unsubscribe.push(scope.$on("formSubmitted", function(ev, args) {
                showValidation = false;
                scope.errorMsg = "";
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

            if (scope.property) { //this can be null if no property was assigned
                serverValidationManager.subscribe(scope.property.alias, "", function (isValid, propertyErrors, allErrors) {
                    hasError = !isValid;
                    if (hasError) {
                        //set the error message to the server message
                        scope.errorMsg = propertyErrors[0].errorMsg;
                        //flag that the current validator is invalid
                        formCtrl.$setValidity('valPropertyMsg', false);
                        startWatch();
                    }
                    else {
                        scope.errorMsg = "";
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
                    serverValidationManager.unsubscribe(scope.property.alias, "");
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
angular.module('umbraco.directives.validation').directive("valPropertyMsg", valPropertyMsg);
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
        scope: {
            valPropertyValidator: "="
        },

        // The element must have ng-model attribute and be inside an umbProperty directive
        require: ['ngModel', '?^umbProperty'],

        restrict: "A",

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
                    throw "The result object from valPropertyValidator does not contain required properties: isValid, errorKey, errorMsg";
                }
                if (result.isValid === true) {
                    // Tell the controller that the value is valid
                    modelCtrl.$setValidity(result.errorKey, true);
                    if (propCtrl) {
                        propCtrl.setPropertyError(null);
                    }                    
                }
                else {
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
angular.module('umbraco.directives.validation').directive("valPropertyValidator", valPropertyValidator);

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
        restrict: "A",
        link: function (scope, elm, attrs, ctrl) {

            var flags = "";
            var regex;
            var eventBindings = [];

            attrs.$observe("valRegexFlags", function (newVal) {
                if (newVal) {
                    flags = newVal;
                }
            });

            attrs.$observe("valRegex", function (newVal) {
                if (newVal) {
                    try {
                        var resolved = newVal;
                        if (resolved) {
                            regex = new RegExp(resolved, flags);
                        }
                        else {
                            regex = new RegExp(attrs.valRegex, flags);
                        }
                    }
                    catch (e) {
                        regex = new RegExp(attrs.valRegex, flags);
                    }
                }
            });

            eventBindings.push(scope.$watch('ngModel', function(newValue, oldValue){
                if(newValue && newValue !== oldValue) {
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
                        ctrl.errorMsg = "";
                        return viewValue;
                    }
                    else {
                        // it is invalid, return undefined (no model update)
                        ctrl.$setValidity('valRegex', false);
                        //assign a message to the validator
                        ctrl.errorMsg = "Value is invalid, it does not match the correct pattern";
                        return undefined;
                    }
                }
            };

            scope.$on('$destroy', function(){
              // unbind watchers
              for(var e in eventBindings) {
                eventBindings[e]();
               }
            });

        }
    };
}
angular.module('umbraco.directives.validation').directive("valRegex", valRegex);

/**
    * @ngdoc directive
    * @name umbraco.directives.directive:valServer
    * @restrict A
    * @description This directive is used to associate a content property with a server-side validation response
    *               so that the validators in angular are updated based on server-side feedback.
    **/
function valServer(serverValidationManager) {
    return {
        require: ['ngModel', '?^umbProperty'],
        restrict: "A",
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
            var fieldName = "value";
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
                }
                else {
                    modelCtrl.$setValidity('valServer', true);
                    //reset the error message
                    modelCtrl.errorMsg = "";
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
angular.module('umbraco.directives.validation').directive("valServer", valServer);
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
        restrict: "A",
        link: function (scope, element, attr, ctrl) {
            
            var fieldName = null;
            var eventBindings = [];

            attr.$observe("valServerField", function (newVal) {
                if (newVal && fieldName === null) {
                    fieldName = newVal;

                    //subscribe to the changed event of the view model. This is required because when we
                    // have a server error we actually invalidate the form which means it cannot be 
                    // resubmitted. So once a field is changed that has a server error assigned to it
                    // we need to re-validate it for the server side validator so the user can resubmit
                    // the form. Of course normal client-side validators will continue to execute.
                    eventBindings.push(scope.$watch('ngModel', function(newValue){
                        if (ctrl.$invalid) {
                            ctrl.$setValidity('valServerField', true);
                        }
                    }));

                    //subscribe to the server validation changes
                    serverValidationManager.subscribe(null, fieldName, function (isValid, fieldErrors, allErrors) {
                        if (!isValid) {
                            ctrl.$setValidity('valServerField', false);
                            //assign an error msg property to the current validator
                            ctrl.errorMsg = fieldErrors[0].errorMsg;
                        }
                        else {
                            ctrl.$setValidity('valServerField', true);
                            //reset the error message
                            ctrl.errorMsg = "";
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

            scope.$on('$destroy', function(){
              // unbind watchers
              for(var e in eventBindings) {
                eventBindings[e]();
               }
            });

        }
    };
}
angular.module('umbraco.directives.validation').directive("valServerField", valServerField);


/**
* @ngdoc directive
* @name umbraco.directives.directive:valTab
* @restrict A
* @description Used to show validation warnings for a tab to indicate that the tab content has validations errors in its data.
* In order for this directive to work, the valFormManager directive must be placed on the containing form.
**/
function valTab() {
    return {
        require: ['^form', '^valFormManager'],
        restrict: "A",
        link: function (scope, element, attr, ctrs) {

            var valFormManager = ctrs[1];
            var tabId = "tab" + scope.tab.id;                        
            scope.tabHasError = false;

            //listen for form validation changes
            valFormManager.onValidationStatusChanged(function (evt, args) {
                if (!args.form.$valid) {
                    var tabContent = element.closest(".umb-panel").find("#" + tabId);
                    //check if the validation messages are contained inside of this tabs 
                    if (tabContent.find(".ng-invalid").length > 0) {
                        scope.tabHasError = true;
                    } else {
                        scope.tabHasError = false;
                    }
                }
                else {
                    scope.tabHasError = false;
                }
            });

        }
    };
}
angular.module('umbraco.directives.validation').directive("valTab", valTab);
function valToggleMsg(serverValidationManager) {
    return {
        require: "^form",
        restrict: "A",

        /**
            Our directive requries a reference to a form controller which gets passed in to this parameter
         */
        link: function (scope, element, attr, formCtrl) {

            if (!attr.valToggleMsg){
                throw "valToggleMsg requires that a reference to a validator is specified";
            }
            if (!attr.valMsgFor){
                throw "valToggleMsg requires that the attribute valMsgFor exists on the element";
            }
            if (!formCtrl[attr.valMsgFor]) {
                throw "valToggleMsg cannot find field " + attr.valMsgFor + " on form " + formCtrl.$name;
            }

            //if there's any remaining errors in the server validation service then we should show them.
            var showValidation = serverValidationManager.items.length > 0;
            var hasCustomMsg = element.contents().length > 0;

            //add a watch to the validator for the value (i.e. myForm.value.$error.required )
            scope.$watch(function () {
                //sometimes if a dialog closes in the middle of digest we can get null references here
                
                return (formCtrl && formCtrl[attr.valMsgFor]) ? formCtrl[attr.valMsgFor].$error[attr.valToggleMsg] : null;
            }, function () {
                //sometimes if a dialog closes in the middle of digest we can get null references here
                if ((formCtrl && formCtrl[attr.valMsgFor])) {
                    if (formCtrl[attr.valMsgFor].$error[attr.valToggleMsg] && showValidation) {                        
                        element.show();
                        //display the error message if this element has no contents
                        if (!hasCustomMsg) {
                            element.html(formCtrl[attr.valMsgFor].errorMsg);
                        }
                    }
                    else {
                        element.hide();
                    }
                }
            });
            
            var unsubscribe = [];

            //listen for the saving event (the result is a callback method which is called to unsubscribe)
            unsubscribe.push(scope.$on("formSubmitting", function(ev, args) {
                showValidation = true;
                if (formCtrl[attr.valMsgFor].$error[attr.valToggleMsg]) {
                    element.show();
                    //display the error message if this element has no contents
                    if (!hasCustomMsg) {
                        element.html(formCtrl[attr.valMsgFor].errorMsg);
                    }
                }
                else {
                    element.hide();
                }
            }));
            
            //listen for the saved event (the result is a callback method which is called to unsubscribe)
            unsubscribe.push(scope.$on("formSubmitted", function(ev, args) {
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
angular.module('umbraco.directives.validation').directive("valToggleMsg", valToggleMsg);
angular.module('umbraco.directives.validation')
.directive('valTriggerChange', function($sniffer) {
	return {
		link : function(scope, elem, attrs) {
			elem.bind('click', function(){
				$(attrs.valTriggerChange).trigger($sniffer.hasEvent('input') ? 'input' : 'change');
			});
		},
		priority : 1	
	};
});
/**
 * Konami Code directive for AngularJS
 * @version v0.0.1
 * @license MIT License, http://www.opensource.org/licenses/MIT
 */

angular.module('umbraco.directives')
  .directive('konamiCode', ['$document', function ($document) {
      var konamiKeysDefault = [38, 38, 40, 40, 37, 39, 37, 39, 66, 65];

      return {
          restrict: 'A',
          link: function (scope, element, attr) {

              if (!attr.konamiCode) {
                  throw ('Konami directive must receive an expression as value.');
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
  }]);
(function() {
  'use strict';

  function ValRequireComponentDirective() {

    function link(scope, el, attr, ngModel) {

      var unbindModelWatcher = scope.$watch(function () {
        return ngModel.$modelValue;
      }, function(newValue) {

        if(newValue === undefined || newValue === null || newValue === "") {
          ngModel.$setValidity("valRequiredComponent", false);
        } else {
          ngModel.$setValidity("valRequiredComponent", true);
        }

      });

      // clean up
      scope.$on('$destroy', function(){
        unbindModelWatcher();
      });

    }

    var directive = {
      require: 'ngModel',
      restrict: "A",
      link: link
    };

    return directive;
  }

  angular.module('umbraco.directives').directive('valRequireComponent', ValRequireComponentDirective);

})();

/**
* @ngdoc directive
* @name umbraco.directives.directive:valSubView
* @restrict A
* @description Used to show validation warnings for a editor sub view to indicate that the section content has validation errors in its data.
* In order for this directive to work, the valFormManager directive must be placed on the containing form.
**/
(function() {
  'use strict';

  function valSubViewDirective() {

    function link(scope, el, attr, ctrl) {

      var valFormManager = ctrl[1];
      scope.subView.hasError = false;

      //listen for form validation changes
      valFormManager.onValidationStatusChanged(function (evt, args) {
          if (!args.form.$valid) {

             var subViewContent = el.find(".ng-invalid");

             if (subViewContent.length > 0) {
                 scope.subView.hasError = true;
             } else {
                 scope.subView.hasError = false;
             }

          }
          else {
             scope.subView.hasError = false;
          }
      });

    }

    var directive = {
      require: ['^form', '^valFormManager'],
      restrict: "A",
      link: link
    };

    return directive;
  }

  angular.module('umbraco.directives').directive('valSubView', valSubViewDirective);

})();


})();