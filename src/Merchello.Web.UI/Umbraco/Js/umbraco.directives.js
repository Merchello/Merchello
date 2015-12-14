/*! umbraco
 * https://github.com/umbraco/umbraco-cms/
 * Copyright (c) 2015 Umbraco HQ;
 * Licensed MIT
 */

(function() { 

angular.module("umbraco.directives", ["umbraco.directives.editors", "umbraco.directives.html", "umbraco.directives.validation", "ui.sortable"]);
angular.module("umbraco.directives.editors", []);
angular.module("umbraco.directives.html", []);
angular.module("umbraco.directives.validation", []);
/**
* @ngdoc directive
* @name umbraco.directives.directive:umbProperty
* @restrict E
**/
angular.module("umbraco.directives")
    .directive('buttonGroup', function (contentEditingHelper) {
        return {
            scope: {
                actions: "=",
                handler: "="
            },
            transclude: true,
            restrict: 'E',
            replace: true,        
            templateUrl: 'views/directives/button-group.html',
            link: function (scope, element, attrs, ctrl) {

                scope.buttons = [];

                scope.handle = function(action){
                    if(scope.handler){
                        
                    }
                };
                function processActions() {
                    var buttons = [];

                    angular.forEach(scope.actions, function(action){
                        if(angular.isObject(action)){
                            buttons.push(action);
                        }else{
                            var btn  = contentEditingHelper.getButtonFromAction(action);
                            if(btn){
                                buttons.push(btn);
                            }
                        }
                    });

                    scope.defaultButton = buttons.pop(0);
                    scope.buttons = buttons;
                }
                
                scope.$watchCollection(scope.actions, function(){
                    processActions();
                });
            }
        };
    });
angular.module("umbraco.directives")
    .directive('umbAutoFocus', function($timeout) {

        return function(scope, element, attr){
            var update = function() {
                //if it uses its default naming
                if(element.val() === ""){
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
            require: "^?umbTabs",
            link: function(scope, element, attr, tabsCtrl) {
                var domEl = element[0];
                var update = function(force) {
                    if (force === true) {
                        element.height(0);
                    }

                    if (domEl.scrollHeight !== domEl.clientHeight) {
                        element.height(domEl.scrollHeight);
                    }
                };
                var blur = function() {
                    update(true);
                };

                element.bind('keyup keydown keypress change', update);
                element.bind('blur', blur);

                $timeout(function() {
                    update(true);
                }, 200);


                //listen for tab changes
                if (tabsCtrl != null) {
                    tabsCtrl.onTabShown(function(args) {
                        update();
                    });
                }

                scope.$on('$destroy', function () {
                    element.unbind('keyup keydown keypress change', update);
                    element.unbind('blur', blur);
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
			templateUrl: 'views/directives/umb-content-name.html',
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
angular.module("umbraco.directives")
    .directive('gridRte', function (tinyMceService, stylesheetResource, angularHelper, assetsService, $q, $timeout) {
        return {
            scope: {
                uniqueId: '=',
                value: '=',
                onClick: '&',
                onFocus: '&',
                onBlur: '&',
                configuration:"="
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
                        var extendedValidElements = "@[id|class|style],-div[id|dir|class|align|style],ins[datetime|cite],-ul[class|style],-li[class|style],-h1[id|dir|class|align|style],-h2[id|dir|class|align|style],-h3[id|dir|class|align|style],-h4[id|dir|class|align|style],-h5[id|dir|class|align|style],-h6[id|style|dir|class|align]";

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
                                content_css: stylesheets.join(','),
                                style_formats: styleFormats
                            };


                            if (tinyMceConfig.customConfig) {
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

                                    //hide toolbar by default
                                    $(editor.editorContainer)
                                        .find(".mce-toolbar")
                                        .css("visibility", "hidden");

                                    //force overflow to hidden to prevent no needed scroll
                                    editor.getBody().style.overflow = "hidden";

                                    $timeout(function(){
                                        if(scope.value === null){
                                            editor.focus();
                                        }
                                    }, 400);

                                });
                                
                                // pin toolbar to top of screen if we have focus and it scrolls off the screen
                                var pinToolbar = function () {

                                    var _toolbar = $(editor.editorContainer).find(".mce-toolbar");
                                    var toolbarHeight = _toolbar.height();

                                    var _tinyMce = $(editor.editorContainer);
                                    var tinyMceRect = _tinyMce[0].getBoundingClientRect();
                                    var tinyMceTop = tinyMceRect.top;
                                    var tinyMceBottom = tinyMceRect.bottom;

                                    if (tinyMceTop < 100 && (tinyMceBottom > (100 + toolbarHeight))) {
                                        _toolbar
                                            .css("visibility", "visible")
                                            .css("position", "fixed")
                                            .css("top", "100px")
                                            .css("margin-top", "0");
                                    } else {
                                        _toolbar
                                            .css("visibility", "visible")
                                            .css("position", "absolute")
                                            .css("top", "auto")
                                            .css("margin-top", (-toolbarHeight - 2) + "px");
                                    }
                                };

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

                                        _toolbar.css("visibility", "hidden");
                                        $('.umb-panel-body').off('scroll', pinToolbar);
                                    });
                                });

                                // Focus on editor
                                editor.on('focus', function (e) {
                                    angularHelper.safeApply(scope, function () {

                                        if(scope.onFocus){
                                            scope.onFocus();
                                        }

                                        pinToolbar();
                                        $('.umb-panel-body').on('scroll', pinToolbar);
                                    });
                                });

                                // Click on editor
                                editor.on('click', function (e) {
                                    angularHelper.safeApply(scope, function () {

                                        if(scope.onClick){
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
                                    var qs = "?width=" + e.width + "&height=" + e.height;
                                    var srcAttr = $(e.target).attr("src");
                                    var path = srcAttr.split("?")[0];
                                    $(e.target).attr("data-mce-src", path + qs);
                                });


                                //Create the insert media plugin
                                tinyMceService.createMediaPicker(editor, scope);

                                //Create the embedded plugin
                                tinyMceService.createInsertEmbeddedMedia(editor, scope);

                                //Create the insert link plugin
                                //tinyMceService.createLinkPicker(editor, scope);

                                //Create the insert macro plugin
                                tinyMceService.createInsertMacro(editor, scope);

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
            templateUrl: 'views/directives/html/umb-control-group.html',
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
        templateUrl: 'views/directives/umb-header.html',
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
* @name umbraco.directives.directive:umbProperty
* @restrict E
**/
angular.module("umbraco.directives.html")
    .directive('umbPane', function () {
        return {
            transclude: true,
            restrict: 'E',
            replace: true,        
            templateUrl: 'views/directives/html/umb-pane.html'
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
			templateUrl: 'views/directives/html/umb-panel.html'
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
            templateUrl: 'views/directives/html/umb-photo-folder.html',
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
* @name umbraco.directives.directive:umbTab 
* @restrict E
**/
angular.module("umbraco.directives")
.directive('umbTab', function ($parse, $timeout) {
    return {
		restrict: 'E',
		replace: true,		
        transclude: 'true',
		templateUrl: 'views/directives/umb-tab.html'		
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
		templateUrl: 'views/directives/umb-tab-view.html'
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
			templateUrl: 'views/directives/html/umb-upload-dropzone.html'
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
				templateUrl: 'views/directives/imaging/umb-image-crop.html',
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
* @name umbraco.directives.directive:umbImageFolder
* @restrict E
* @function
**/
function umbImageFolder($rootScope, assetsService, $timeout, $log, umbRequestHelper, mediaResource, imageHelper, notificationsService) {
    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'views/directives/imaging/umb-image-folder.html',
        scope: {
            options: '=',
            nodeId: '@',
            onUploadComplete: '='
        },
        link: function (scope, element, attrs) {
            //NOTE: Blueimp handlers are documented here: https://github.com/blueimp/jQuery-File-Upload/wiki/Options
            //NOTE: We are using a Blueimp version specifically ~9.4.0 because any higher than that and we get crazy errors with jquery, also note
            // that the jquery UI version 1.10.3 is required for this blueimp version! if we go higher to 1.10.4 it breaks! seriously! 
            // It's do to with the widget framework in jquery ui changes which must have broken a whole lot of stuff. So don't change it for now.

            if (scope.onUploadComplete && !angular.isFunction(scope.onUploadComplete)) {
                throw "onUploadComplete must be a function callback";
            }

            scope.uploading = false;
            scope.files = [];
            scope.progress = 0;

            var defaultOptions = {
                url: umbRequestHelper.getApiUrl("mediaApiBaseUrl", "PostAddFile") + "?origin=blueimp",
                //we'll start it manually to make sure the UI is all in order before processing
                autoUpload: true,
                disableImageResize: /Android(?!.*Chrome)|Opera/
                    .test(window.navigator.userAgent),
                previewMaxWidth: 150,
                previewMaxHeight: 150,
                previewCrop: true,
                dropZone: element.find(".drop-zone"),
                fileInput: element.find("input.uploader"),
                formData: {
                    currentFolder: scope.nodeId
                }
            };

            //merge options
            scope.blueimpOptions = angular.extend(defaultOptions, scope.options);

            function loadChildren(id) {
                mediaResource.getChildren(id)
                    .then(function(data) {
                        scope.images = data.items;
                    });
            }

            function checkComplete(e, data) {
                scope.$apply(function () {
                    //remove the amount of files complete
                    //NOTE: function is here instead of in the loop otherwise jshint blows up
                    function findFile(file) { return file === data.files[i]; }
                    for (var i = 0; i < data.files.length; i++) {
                        var found = _.find(scope.files, findFile);
                        found.completed = true;
                    }

                    //Show notifications!!!!
                    if (data.result && data.result.notifications && angular.isArray(data.result.notifications)) {
                        for (var n = 0; n < data.result.notifications.length; n++) {
                            notificationsService.showNotification(data.result.notifications[n]);
                        }
                    }

                    //when none are left resync everything
                    var remaining = _.filter(scope.files, function (file) { return file.completed !== true; });
                    if (remaining.length === 0) {

                        scope.progress = 100;

                        //just the ui transition isn't too abrupt, just wait a little here
                        $timeout(function () {
                            scope.progress = 0;
                            scope.files = [];
                            scope.uploading = false;

                            loadChildren(scope.nodeId);

                            //call the callback
                            scope.onUploadComplete.apply(this, [data]);


                        }, 200);


                    }
                });
            }
            
            //when one is finished
            scope.$on('fileuploaddone', function(e, data) {
                checkComplete(e, data);
            });

            //This handler gives us access to the file 'preview', this is the only handler that makes this available for whatever reason
            // so we'll use this to also perform the adding of files to our collection
            scope.$on('fileuploadprocessalways', function(e, data) {
                scope.$apply(function() {
                    scope.uploading = true;
                    scope.files.push(data.files[data.index]);
                });
            });

            //This is a bit of a hack to check for server errors, currently if there's a non
            //known server error we will tell them to check the logs, otherwise we'll specifically 
            //check for the file size error which can only be done with dodgy string checking
            scope.$on('fileuploadfail', function (e, data) {
                if (data.jqXHR.status === 500 && data.jqXHR.responseText.indexOf("Maximum request length exceeded") >= 0) {
                    notificationsService.error(data.errorThrown, "The uploaded file was too large, check with your site administrator to adjust the maximum size allowed");

                }
                else {
                    notificationsService.error(data.errorThrown, data.jqXHR.statusText);
                }

                checkComplete(e, data);
            });

            //This executes prior to the whole processing which we can use to get the UI going faster,
            //this also gives us the start callback to invoke to kick of the whole thing
            scope.$on('fileuploadadd', function(e, data) {
                scope.$apply(function() {
                    scope.uploading = true;
                });
            });

            // All these sit-ups are to add dropzone area and make sure it gets removed if dragging is aborted! 
            scope.$on('fileuploaddragover', function(e, data) {
                if (!scope.dragClearTimeout) {
                    scope.$apply(function() {
                        scope.dropping = true;
                    });
                }
                else {
                    $timeout.cancel(scope.dragClearTimeout);
                }
                scope.dragClearTimeout = $timeout(function() {
                    scope.dropping = null;
                    scope.dragClearTimeout = null;
                }, 300);
            });

            //init load
            loadChildren(scope.nodeId);

        }
    };
}

angular.module("umbraco.directives")
    .directive("umbUploadPreview", function($parse) {
        return {
            link: function(scope, element, attr, ctrl) {
                var fn = $parse(attr.umbUploadPreview),
                    file = fn(scope);
                if (file.preview) {
                    element.append(file.preview);
                }
            }
        };
    })
    .directive('umbImageFolder', umbImageFolder);
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
				templateUrl: 'views/directives/imaging/umb-image-gravity.html',
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
				templateUrl: 'views/directives/imaging/umb-image-thumbnail.html',
				
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
/**
* @ngdoc directive
* @name umbraco.directives.directive:umbImageFileUpload
* @restrict E
* @function
* @description 
* This is a wrapper around the blueimp angular file-upload directive so that we can expose a proper API to other directives, the blueimp
* directive isn't actually made very well and only exposes an API/events on the $scope so we can't do things like require: "^fileUpload" and use
* it's instance.
**/
function umbImageUpload($compile) {
    return {
        restrict: 'A', 
        scope: true,
        link: function (scope, element, attrs) {
            //set inner scope variable to assign to file-upload directive in the template
            scope.innerOptions = scope.$eval(attrs.umbImageUpload);

            //compile an inner blueimp file-upload with our scope

            var x = angular.element('<div file-upload="innerOptions" />');
            element.append(x);
            $compile(x)(scope);
        },

        //Define a controller for this directive to expose APIs to other directives
        controller: function ($scope, $element, $attrs) {
            

            //create a method to allow binding to a blueimp event (which is based on it's directives scope)
            this.bindEvent = function (e, callback) {
                $scope.$on(e, callback);
            };

        }
    };
}

angular.module("umbraco.directives").directive('umbImageUpload', umbImageUpload);
/**
* @ngdoc directive
* @name umbraco.directives.directive:umbImageUploadProgress
* @restrict E
* @function
**/
function umbImageUploadProgress($rootScope, assetsService, $timeout, $log, umbRequestHelper, mediaResource, imageHelper) {
    return {
        require: '^umbImageUpload',
        restrict: 'E',
        replace: true,
        template: '<div class="progress progress-striped active"><div class="bar" ng-style="{width: progress + \'%\'}"></div></div>',
        
        link: function (scope, element, attrs, umbImgUploadCtrl) {

            umbImgUploadCtrl.bindEvent('fileuploadprogressall', function (e, data) {
                scope.progress = parseInt(data.loaded / data.total * 100, 10);
            });
        }
    };
}

angular.module("umbraco.directives").directive('umbImageUploadProgress', umbImageUploadProgress);
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
                                var apps = $("#applications");
                                var bottomBar = contentPanel.find(".umb-bottom-bar");
                                var navOffeset = $("#navOffset");

                                var leftPanelWidth = ui.element.width() + apps.width();

                                contentPanel.css({ left: leftPanelWidth });
                                bottomBar.css({ left: leftPanelWidth });

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
        templateUrl: 'views/directives/umb-item-sorter.html',
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
        templateUrl: 'views/directives/umb-confirm.html',
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
        templateUrl: 'views/directives/umb-contextmenu.html',
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
* @function
* @name umbraco.directives.directive:umbEditor 
* @requires formController
* @restrict E
**/
angular.module("umbraco.directives")
    .directive('umbEditor', function (umbPropEditorHelper) {
        return {
            scope: {
                model: "=",
                isPreValue: "@"
            },
            require: "^form",
            restrict: 'E',
            replace: true,      
            templateUrl: 'views/directives/umb-editor.html',
            link: function (scope, element, attrs, ctrl) {

                //we need to copy the form controller val to our isolated scope so that
                //it get's carried down to the child scopes of this!
                //we'll also maintain the current form name.
                scope[ctrl.$name] = ctrl;

                if(!scope.model.alias){
                   scope.model.alias = Math.random().toString(36).slice(2);
                }

                scope.propertyEditorView = umbPropEditorHelper.getViewPath(scope.model.view, scope.isPreValue);
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
* @name umbraco.directives.directive:login
* @function
* @element ANY
* @restrict E
**/
function loginDirective() {
    return {
        restrict: "E",    // restrict to an element
        replace: true,   // replace the html element with the template
        templateUrl: 'views/directives/umb-login.html'        
    };
}

angular.module('umbraco.directives').directive("umbLogin", loginDirective);

/**
* @ngdoc directive
* @name umbraco.directives.directive:umbNavigation
* @restrict E
**/
function umbNavigationDirective() {
    return {
        restrict: "E",    // restrict to an element
        replace: true,   // replace the html element with the template
        templateUrl: 'views/directives/umb-navigation.html'
    };
}

angular.module('umbraco.directives').directive("umbNavigation", umbNavigationDirective);
/**
 * @ngdoc directive
 * @name umbraco.directives.directive:umbNotifications
 */
function notificationDirective(notificationsService) {
    return {
        restrict: "E",    // restrict to an element
        replace: true,   // replace the html element with the template
        templateUrl: 'views/directives/umb-notifications.html',
        link: function (scope, element, attr, ctrl) {

            //subscribes to notifications in the notification service
            scope.notifications = notificationsService.current;
            scope.$watch('notificationsService.current', function (newVal, oldVal, scope) {
                if (newVal) {
                    scope.notifications = newVal;
                }
            });

        }
    };
}

angular.module('umbraco.directives').directive("umbNotifications", notificationDirective);
angular.module("umbraco.directives")
.directive('umbOptionsMenu', function ($injector, treeService, navigationService, umbModelMapper, appState) {
    return {
        scope: {
            currentSection: "@",
            currentNode: "="
        },
        restrict: 'E',
        replace: true,
        templateUrl: 'views/directives/umb-optionsmenu.html',
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
            templateUrl: 'views/directives/umb-property.html',
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
* @name umbraco.directives.directive:umbSections
* @restrict E
**/
function sectionsDirective($timeout, $window, navigationService, treeService, sectionResource, appState, eventsService, $location) {
    return {
        restrict: "E",    // restrict to an element
        replace: true,   // replace the html element with the template
        templateUrl: 'views/directives/umb-sections.html',
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
				navigationService.showUserDialog();
			};

			scope.helpClick = function(){
				navigationService.showHelpDialog();
			};

			scope.sectionClick = function (event, section) {

			    if (event.ctrlKey ||
			        event.shiftKey ||
			        event.metaKey || // apple
			        (event.button && event.button === 1) // middle click, >IE9 + everyone else
			    ) {
			        return;
			    }


			    if (navigationService.userDialog) {
			        navigationService.userDialog.close();
			    }
			    if (navigationService.helpDialog) {
			        navigationService.helpDialog.close();
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
			    if (navigationService.userDialog) {
			        navigationService.userDialog.close();
			    }
			    if (navigationService.helpDialog) {
			        navigationService.helpDialog.close();
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
                '<i ng-if="enablecheckboxes == \'true\'" ng-class="selectEnabledNodeClass(tree.root)"></i>' +
                '<a href="#/{{section}}" ng-click="select(tree.root, $event)"  class="root-link">{{tree.name}}</a></h5>' +
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
        templateUrl: 'views/directives/umb-tree-search-box.html',
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
        templateUrl: 'views/directives/umb-tree-search-results.html',
        link: function (scope, element, attrs, ctrl) {
            
        }
    };
}
angular.module('umbraco.directives').directive("umbTreeSearchResults", treeSearchResults);
/**
* @description Utility directives for key and field events
**/
angular.module('umbraco.directives')

.directive('onKeyup', function () {
    return function (scope, elm, attrs) {
        elm.bind("keyup", function () {
            scope.$apply(attrs.onKeyup);
        });
    };
})

.directive('onKeydown', function () {
    return {
        link: function (scope, elm, attrs) {
            scope.$apply(attrs.onKeydown);
        }
    };
})

.directive('onBlur', function () {
    return function (scope, elm, attrs) {
        elm.bind("blur", function () {
            scope.$apply(attrs.onBlur);
        });
    };
})

.directive('onFocus', function () {
    return function (scope, elm, attrs) {
        elm.bind("focus", function () {
            scope.$apply(attrs.onFocus);
        });
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
        el.bind('contextmenu',function(e){
            e.preventDefault();
            e.stopPropagation();
            scope.$apply(attrs.onRightClick);
            return false;
        });
    };
});
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
    .directive('delayedMouseleave', function ($timeout, $parse) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs, ctrl) {
                var active = false;
                var fn = $parse(attrs.delayedMouseleave);

                function mouseLeave(event) {
                    var callback = function () {
                        fn(scope, { $event: event });
                    };

                    active = false;
                    $timeout(function () {
                        if (active === false) {
                            scope.$apply(callback);
                        }
                    }, 650);
                }

                function mouseEnter(event, args){
                    active = true;
                }

                element.on("mouseleave", mouseLeave);
                element.on("mouseenter", mouseEnter);

                //unbind!!
                scope.$on('$destroy', function () {
                    element.off("mouseleave", mouseLeave);
                    element.off("mouseenter", mouseEnter);
                });
            }
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
                    $timeout(function() {
                        elm.focus();
                    });
                }
            });
        }
    };
});

/**
* @ngdoc directive
* @name umbraco.directives.directive:headline
**/
angular.module("umbraco.directives")
  .directive('hotkey', function ($window, keyboardService, $log) {

      return function (scope, el, attrs) {
          
          //support data binding
    
          var keyCombo = scope.$eval(attrs["hotkey"]);
          if (!keyCombo) {
              keyCombo = attrs["hotkey"];
          }

          keyboardService.bind(keyCombo, function() {
              var element = $(el);

              if(element.is("a,button,input[type='button'],input[type='submit']") && !element.is(':disabled') ){
                element.click();
              }else{
                element.focus();
              }
          });
          
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
        ctrl.$formatters.push(validateFn);
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
            
            scope.$watch(function() {
                return scope.$eval(attrs.valHighlight);
            }, function(newVal, oldVal) {
                if (newVal === true) {
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

            ctrl.$formatters.push(patternValidator);
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

            // Formatters are invoked when the model is modified in the code.
            modelCtrl.$formatters.push(validate);

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
            if (attrs.valRegexFlags) {
                try {
                    flags = scope.$eval(attrs.valRegexFlags);
                    if (!flags) {
                        flags = attrs.valRegexFlags;
                    }
                }
                catch (e) {
                    flags = attrs.valRegexFlags;
                }
            }
            var regex;
            try {
                var resolved = scope.$eval(attrs.valRegex);                
                if (resolved) {
                    regex = new RegExp(resolved, flags);
                }
                else {
                    regex = new RegExp(attrs.valRegex, flags);
                }
            }
            catch(e) {
                regex = new RegExp(attrs.valRegex, flags);
            }

            var patternValidator = function (viewValue) {
                //NOTE: we don't validate on empty values, use required validator for that
                if (!viewValue || regex.test(viewValue)) {
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
            };

            ctrl.$formatters.push(patternValidator);
            ctrl.$parsers.push(patternValidator);
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
            
            if (!attr.valServerField) {
                throw "valServerField must have a field name for referencing server errors";
            }

            var fieldName = attr.valServerField;
            
            //subscribe to the changed event of the view model. This is required because when we
            // have a server error we actually invalidate the form which means it cannot be 
            // resubmitted. So once a field is changed that has a server error assigned to it
            // we need to re-validate it for the server side validator so the user can resubmit
            // the form. Of course normal client-side validators will continue to execute.
            ctrl.$viewChangeListeners.push(function () {
                if (ctrl.$invalid) {
                    ctrl.$setValidity('valServerField', true);
                }
            });
            
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
        require: "^form",
        restrict: "A",
        link: function (scope, element, attr, formCtrl) {
            
            var tabId = "tab" + scope.tab.id;
                        
            scope.tabHasError = false;

            //listen for form validation changes
            scope.$on("valStatusChanged", function(evt, args) {
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

})();