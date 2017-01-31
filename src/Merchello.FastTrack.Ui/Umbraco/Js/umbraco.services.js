/*! umbraco
 * https://github.com/umbraco/umbraco-cms/
 * Copyright (c) 2017 Umbraco HQ;
 * Licensed 
 */

(function() { 

angular.module("umbraco.services", ["umbraco.security", "umbraco.resources"]);

/**
 * @ngdoc service
 * @name umbraco.services.angularHelper
 * @function
 *
 * @description
 * Some angular helper/extension methods
 */
function angularHelper($log, $q) {
    return {

        /**
         * @ngdoc function
         * @name umbraco.services.angularHelper#rejectedPromise
         * @methodOf umbraco.services.angularHelper
         * @function
         *
         * @description
         * In some situations we need to return a promise as a rejection, normally based on invalid data. This
         * is a wrapper to do that so we can save on writing a bit of code.
         *
         * @param {object} objReject The object to send back with the promise rejection
         */
        rejectedPromise: function (objReject) {
            var deferred = $q.defer();
            //return an error object including the error message for UI
            deferred.reject(objReject);
            return deferred.promise;
        },

        /**
         * @ngdoc function
         * @name safeApply
         * @methodOf umbraco.services.angularHelper
         * @function
         *
         * @description
         * This checks if a digest/apply is already occuring, if not it will force an apply call
         */
        safeApply: function (scope, fn) {
            if (scope.$$phase || scope.$root.$$phase) {
                if (angular.isFunction(fn)) {
                    fn();
                }
            }
            else {
                if (angular.isFunction(fn)) {
                    scope.$apply(fn);
                }
                else {
                    scope.$apply();
                }
            }
        },

        /**
         * @ngdoc function
         * @name getCurrentForm
         * @methodOf umbraco.services.angularHelper
         * @function
         *
         * @description
         * Returns the current form object applied to the scope or null if one is not found
         */
        getCurrentForm: function (scope) {

            //NOTE: There isn't a way in angular to get a reference to the current form object since the form object
            // is just defined as a property of the scope when it is named but you'll always need to know the name which
            // isn't very convenient. If we want to watch for validation changes we need to get a form reference.
            // The way that we detect the form object is a bit hackerific in that we detect all of the required properties 
            // that exist on a form object.
            //
            //The other way to do it in a directive is to require "^form", but in a controller the only other way to do it
            // is to inject the $element object and use: $element.inheritedData('$formController');

            var form = null;
            //var requiredFormProps = ["$error", "$name", "$dirty", "$pristine", "$valid", "$invalid", "$addControl", "$removeControl", "$setValidity", "$setDirty"];
            var requiredFormProps = ["$addControl", "$removeControl", "$setValidity", "$setDirty", "$setPristine"];

            // a method to check that the collection of object prop names contains the property name expected
            function propertyExists(objectPropNames) {
                //ensure that every required property name exists on the current scope property
                return _.every(requiredFormProps, function (item) {

                    return _.contains(objectPropNames, item);
                });
            }

            for (var p in scope) {

                if (_.isObject(scope[p]) && p !== "this" && p.substr(0, 1) !== "$") {
                    //get the keys of the property names for the current property
                    var props = _.keys(scope[p]);
                    //if the length isn't correct, try the next prop
                    if (props.length < requiredFormProps.length) {
                        continue;
                    }

                    //ensure that every required property name exists on the current scope property
                    var containProperty = propertyExists(props);

                    if (containProperty) {
                        form = scope[p];
                        break;
                    }
                }
            }

            return form;
        },

        /**
         * @ngdoc function
         * @name validateHasForm
         * @methodOf umbraco.services.angularHelper
         * @function
         *
         * @description
         * This will validate that the current scope has an assigned form object, if it doesn't an exception is thrown, if
         * it does we return the form object.
         */
        getRequiredCurrentForm: function (scope) {
            var currentForm = this.getCurrentForm(scope);
            if (!currentForm || !currentForm.$name) {
                throw "The current scope requires a current form object (or ng-form) with a name assigned to it";
            }
            return currentForm;
        },

        /**
         * @ngdoc function
         * @name getNullForm
         * @methodOf umbraco.services.angularHelper
         * @function
         *
         * @description
         * Returns a null angular FormController, mostly for use in unit tests
         *      NOTE: This is actually the same construct as angular uses internally for creating a null form but they don't expose
         *          any of this publicly to us, so we need to create our own.
         *
         * @param {string} formName The form name to assign
         */
        getNullForm: function (formName) {
            return {
                $addControl: angular.noop,
                $removeControl: angular.noop,
                $setValidity: angular.noop,
                $setDirty: angular.noop,
                $setPristine: angular.noop,
                $name: formName
                //NOTE: we don't include the 'properties', just the methods.
            };
        }
    };
}
angular.module('umbraco.services').factory('angularHelper', angularHelper);
/**
 * @ngdoc service
 * @name umbraco.services.appState
 * @function
 *
 * @description
 * Tracks the various application state variables when working in the back office, raises events when state changes.
 *
 * ##Samples
 *
 * ####Subscribe to global state changes:
 * 
 * <pre>
  *    scope.showTree = appState.getGlobalState("showNavigation");
  *
  *    eventsService.on("appState.globalState.changed", function (e, args) {
  *               if (args.key === "showNavigation") {
  *                   scope.showTree = args.value;
  *               }
  *           });  
  * </pre>
 *
 * ####Subscribe to section-state changes
 *
 * <pre>
 *    scope.currentSection = appState.getSectionState("currentSection");
 *
 *    eventsService.on("appState.sectionState.changed", function (e, args) {
 *               if (args.key === "currentSection") {
 *                   scope.currentSection = args.value;
 *               }
 *           });  
 * </pre>
 */
function appState(eventsService) {
    
    //Define all variables here - we are never returning this objects so they cannot be publicly mutable
    // changed, we only expose methods to interact with the values.

    var globalState = {
        showNavigation: null,
        touchDevice: null,
        showTray: null,
        stickyNavigation: null,
        navMode: null,
        isReady: null,
        isTablet: null
    };
    
    var sectionState = {
        //The currently active section
        currentSection: null,
        showSearchResults: null
    };

    var treeState = {
        //The currently selected node
        selectedNode: null,
        //The currently loaded root node reference - depending on the section loaded this could be a section root or a normal root.
        //We keep this reference so we can lookup nodes to interact with in the UI via the tree service
        currentRootNode: null
    };
    
    var menuState = {
        //this list of menu items to display
        menuActions: null,
        //the title to display in the context menu dialog
        dialogTitle: null,
        //The tree node that the ctx menu is launched for
        currentNode: null,
        //Whether the menu's dialog is being shown or not
        showMenuDialog: null,
        //Whether the context menu is being shown or not
        showMenu: null
    };

    /** function to validate and set the state on a state object */
    function setState(stateObj, key, value, stateObjName) {
        if (!_.has(stateObj, key)) {
            throw "The variable " + key + " does not exist in " + stateObjName;
        }
        var changed = stateObj[key] !== value;
        stateObj[key] = value;
        if (changed) {
            eventsService.emit("appState." + stateObjName + ".changed", { key: key, value: value });
        }
    }
    
    /** function to validate and set the state on a state object */
    function getState(stateObj, key, stateObjName) {
        if (!_.has(stateObj, key)) {
            throw "The variable " + key + " does not exist in " + stateObjName;
        }
        return stateObj[key];
    }

    return {

        /**
         * @ngdoc function
         * @name umbraco.services.angularHelper#getGlobalState
         * @methodOf umbraco.services.appState
         * @function
         *
         * @description
         * Returns the current global state value by key - we do not return an object reference here - we do NOT want this
         * to be publicly mutable and allow setting arbitrary values
         *
         */
        getGlobalState: function (key) {
            return getState(globalState, key, "globalState");
        },

        /**
         * @ngdoc function
         * @name umbraco.services.angularHelper#setGlobalState
         * @methodOf umbraco.services.appState
         * @function
         *
         * @description
         * Sets a global state value by key
         *
         */
        setGlobalState: function (key, value) {
            setState(globalState, key, value, "globalState");
        },

        /**
         * @ngdoc function
         * @name umbraco.services.angularHelper#getSectionState
         * @methodOf umbraco.services.appState
         * @function
         *
         * @description
         * Returns the current section state value by key - we do not return an object here - we do NOT want this
         * to be publicly mutable and allow setting arbitrary values
         *
         */
        getSectionState: function (key) {
            return getState(sectionState, key, "sectionState");            
        },
        
        /**
         * @ngdoc function
         * @name umbraco.services.angularHelper#setSectionState
         * @methodOf umbraco.services.appState
         * @function
         *
         * @description
         * Sets a section state value by key
         *
         */
        setSectionState: function(key, value) {
            setState(sectionState, key, value, "sectionState");
        },

        /**
         * @ngdoc function
         * @name umbraco.services.angularHelper#getTreeState
         * @methodOf umbraco.services.appState
         * @function
         *
         * @description
         * Returns the current tree state value by key - we do not return an object here - we do NOT want this
         * to be publicly mutable and allow setting arbitrary values
         *
         */
        getTreeState: function (key) {
            return getState(treeState, key, "treeState");
        },
        
        /**
         * @ngdoc function
         * @name umbraco.services.angularHelper#setTreeState
         * @methodOf umbraco.services.appState
         * @function
         *
         * @description
         * Sets a section state value by key
         *
         */
        setTreeState: function (key, value) {
            setState(treeState, key, value, "treeState");
        },

        /**
         * @ngdoc function
         * @name umbraco.services.angularHelper#getMenuState
         * @methodOf umbraco.services.appState
         * @function
         *
         * @description
         * Returns the current menu state value by key - we do not return an object here - we do NOT want this
         * to be publicly mutable and allow setting arbitrary values
         *
         */
        getMenuState: function (key) {
            return getState(menuState, key, "menuState");
        },
        
        /**
         * @ngdoc function
         * @name umbraco.services.angularHelper#setMenuState
         * @methodOf umbraco.services.appState
         * @function
         *
         * @description
         * Sets a section state value by key
         *
         */
        setMenuState: function (key, value) {
            setState(menuState, key, value, "menuState");
        },

    };
}
angular.module('umbraco.services').factory('appState', appState);

/**
 * @ngdoc service
 * @name umbraco.services.editorState
 * @function
 *
 * @description
 * Tracks the parent object for complex editors by exposing it as 
 * an object reference via editorState.current.entity
 *
 * it is possible to modify this object, so should be used with care
 */
angular.module('umbraco.services').factory("editorState", function() {

    var current = null;
    var state = {

        /**
         * @ngdoc function
         * @name umbraco.services.angularHelper#set
         * @methodOf umbraco.services.editorState
         * @function
         *
         * @description
         * Sets the current entity object for the currently active editor
         * This is only used when implementing an editor with a complex model
         * like the content editor, where the model is modified by several
         * child controllers. 
         */
        set: function (entity) {
            current = entity;
        },

        /**
         * @ngdoc function
         * @name umbraco.services.angularHelper#reset
         * @methodOf umbraco.services.editorState
         * @function
         *
         * @description
         * Since the editorstate entity is read-only, you cannot set it to null
         * only through the reset() method
         */
        reset: function() {
            current = null;
        },

        /**
         * @ngdoc function
         * @name umbraco.services.angularHelper#getCurrent
         * @methodOf umbraco.services.editorState
         * @function
         *
         * @description
         * Returns an object reference to the current editor entity.
         * the entity is the root object of the editor.
         * EditorState is used by property/parameter editors that need
         * access to the entire entity being edited, not just the property/parameter 
         *
         * editorState.current can not be overwritten, you should only read values from it
         * since modifying individual properties should be handled by the property editors
         */
        getCurrent: function() {
            return current;
        }
    };

    //TODO: This shouldn't be removed! use getCurrent() method instead of a hacked readonly property which is confusing.

    //create a get/set property but don't allow setting
    Object.defineProperty(state, "current", {
        get: function () {
            return current;
        },
        set: function (value) {
            throw "Use editorState.set to set the value of the current entity";
        },
    });

    return state;
});
/**
 * @ngdoc service
 * @name umbraco.services.assetsService
 *
 * @requires $q 
 * @requires angularHelper
 *  
 * @description
 * Promise-based utillity service to lazy-load client-side dependencies inside angular controllers.
 * 
 * ##usage
 * To use, simply inject the assetsService into any controller that needs it, and make
 * sure the umbraco.services module is accesible - which it should be by default.
 *
 * <pre>
 *      angular.module("umbraco").controller("my.controller". function(assetsService){
 *          assetsService.load(["script.js", "styles.css"], $scope).then(function(){
 *                 //this code executes when the dependencies are done loading
 *          });
 *      });
 * </pre> 
 *
 * You can also load individual files, which gives you greater control over what attibutes are passed to the file, as well as timeout
 *
 * <pre>
 *      angular.module("umbraco").controller("my.controller". function(assetsService){
 *          assetsService.loadJs("script.js", $scope, {charset: 'utf-8'}, 10000 }).then(function(){
 *                 //this code executes when the script is done loading
 *          });
 *      });
 * </pre>
 *
 * For these cases, there are 2 individual methods, one for javascript, and one for stylesheets:
 *
 * <pre>
 *      angular.module("umbraco").controller("my.controller". function(assetsService){
 *          assetsService.loadCss("stye.css", $scope, {media: 'print'}, 10000 }).then(function(){
 *                 //loadcss cannot determine when the css is done loading, so this will trigger instantly
 *          });
 *      });
 * </pre>  
 */
angular.module('umbraco.services')
.factory('assetsService', function ($q, $log, angularHelper, umbRequestHelper, $rootScope, $http) {

    var initAssetsLoaded = false;
    var appendRnd = function (url) {
        //if we don't have a global umbraco obj yet, the app is bootstrapping
        if (!Umbraco.Sys.ServerVariables.application) {
            return url;
        }

        var rnd = Umbraco.Sys.ServerVariables.application.version + "." + Umbraco.Sys.ServerVariables.application.cdf;
        var _op = (url.indexOf("?") > 0) ? "&" : "?";
        url = url + _op + "umb__rnd=" + rnd;
        return url;
    };

    function convertVirtualPath(path) {
        //make this work for virtual paths
        if (path.startsWith("~/")) {
            path = umbRequestHelper.convertVirtualToAbsolutePath(path);
        }
        return path;
    }

    var service = {
        loadedAssets: {},

        _getAssetPromise: function (path) {

            if (this.loadedAssets[path]) {
                return this.loadedAssets[path];
            } else {
                var deferred = $q.defer();
                this.loadedAssets[path] = { deferred: deferred, state: "new", path: path };
                return this.loadedAssets[path];
            }
        },
        /** 
            Internal method. This is called when the application is loading and the user is already authenticated, or once the user is authenticated.
            There's a few assets the need to be loaded for the application to function but these assets require authentication to load.
        */
        _loadInitAssets: function () {
            var deferred = $q.defer();
            //here we need to ensure the required application assets are loaded
            if (initAssetsLoaded === false) {
                var self = this;
                self.loadJs(umbRequestHelper.getApiUrl("serverVarsJs", "", ""), $rootScope).then(function () {
                    initAssetsLoaded = true;

                    //now we need to go get the legacyTreeJs - but this can be done async without waiting.
                    self.loadJs(umbRequestHelper.getApiUrl("legacyTreeJs", "", ""), $rootScope);

                    deferred.resolve();
                });
            }
            else {
                deferred.resolve();
            }
            return deferred.promise;
        },

        /**
         * @ngdoc method
         * @name umbraco.services.assetsService#loadCss
         * @methodOf umbraco.services.assetsService
         *
         * @description
         * Injects a file as a stylesheet into the document head
         * 
         * @param {String} path path to the css file to load
         * @param {Scope} scope optional scope to pass into the loader
         * @param {Object} keyvalue collection of attributes to pass to the stylesheet element  
         * @param {Number} timeout in milliseconds
         * @returns {Promise} Promise object which resolves when the file has loaded
         */
        loadCss: function (path, scope, attributes, timeout) {

            path = convertVirtualPath(path);

            var asset = this._getAssetPromise(path); // $q.defer();
            var t = timeout || 5000;
            var a = attributes || undefined;

            if (asset.state === "new") {
                asset.state = "loading";
                LazyLoad.css(appendRnd(path), function () {
                    if (!scope) {
                        asset.state = "loaded";
                        asset.deferred.resolve(true);
                    } else {
                        asset.state = "loaded";
                        angularHelper.safeApply(scope, function () {
                            asset.deferred.resolve(true);
                        });
                    }
                });
            } else if (asset.state === "loaded") {
                asset.deferred.resolve(true);
            }
            return asset.deferred.promise;
        },

        /**
         * @ngdoc method
         * @name umbraco.services.assetsService#loadJs
         * @methodOf umbraco.services.assetsService
         *
         * @description
         * Injects a file as a javascript into the document
         * 
         * @param {String} path path to the js file to load
         * @param {Scope} scope optional scope to pass into the loader
         * @param {Object} keyvalue collection of attributes to pass to the script element  
         * @param {Number} timeout in milliseconds
         * @returns {Promise} Promise object which resolves when the file has loaded
         */
        loadJs: function (path, scope, attributes, timeout) {

            path = convertVirtualPath(path);

            var asset = this._getAssetPromise(path); // $q.defer();
            var t = timeout || 5000;
            var a = attributes || undefined;

            if (asset.state === "new") {
                asset.state = "loading";

                LazyLoad.js(appendRnd(path), function () {
                    if (!scope) {
                        asset.state = "loaded";
                        asset.deferred.resolve(true);
                    } else {
                        asset.state = "loaded";
                        angularHelper.safeApply(scope, function () {
                            asset.deferred.resolve(true);
                        });
                    }
                });

            } else if (asset.state === "loaded") {
                asset.deferred.resolve(true);
            }

            return asset.deferred.promise;
        },

        /**
         * @ngdoc method
         * @name umbraco.services.assetsService#load
         * @methodOf umbraco.services.assetsService
         *
         * @description
         * Injects a collection of files, this can be ONLY js files
         * 
         *
         * @param {Array} pathArray string array of paths to the files to load
         * @param {Scope} scope optional scope to pass into the loader
         * @returns {Promise} Promise object which resolves when all the files has loaded
         */
        load: function (pathArray, scope) {
            var promise;

            if (!angular.isArray(pathArray)) {
                throw "pathArray must be an array";
            }

            var nonEmpty = _.reject(pathArray, function (item) {
                return item === undefined || item === "";
            });


            //don't load anything if there's nothing to load
            if (nonEmpty.length > 0) {
                var promises = [];
                var assets = [];

                //compile a list of promises
                //blocking
                _.each(nonEmpty, function (path) {

                    path = convertVirtualPath(path);

                    var asset = service._getAssetPromise(path);
                    //if not previously loaded, add to list of promises
                    if (asset.state !== "loaded") {
                        if (asset.state === "new") {
                            asset.state = "loading";
                            assets.push(asset);
                        }

                        //we need to always push to the promises collection to monitor correct 
                        //execution                        
                        promises.push(asset.deferred.promise);
                    }
                });


                //gives a central monitoring of all assets to load
                promise = $q.all(promises);

                _.each(assets, function (asset) {
                    LazyLoad.js(appendRnd(asset.path), function () {
                        asset.state = "loaded";
                        if (!scope) {
                            asset.deferred.resolve(true);
                        }
                        else {
                            angularHelper.safeApply(scope, function () {
                                asset.deferred.resolve(true);
                            });
                        }
                    });
                });
            }
            else {
                //return and resolve
                var deferred = $q.defer();
                promise = deferred.promise;
                deferred.resolve(true);
            }


            return promise;
        }
    };

    return service;
});

/**
* @ngdoc service
* @name umbraco.services.contentEditingHelper
* @description A helper service for most editors, some methods are specific to content/media/member model types but most are used by
* all editors to share logic and reduce the amount of replicated code among editors.
**/
function contentEditingHelper(fileManager, $q, $location, $routeParams, notificationsService, serverValidationManager, dialogService, formHelper, appState) {

    function isValidIdentifier(id){
        //empty id <= 0
        if(angular.isNumber(id) && id > 0){
            return true;
        }

        //empty guid
        if(id === "00000000-0000-0000-0000-000000000000"){
            return false;
        }

        //empty string / alias
        if(id === ""){
            return false;
        }

        return true;
    }

    return {

        /** Used by the content editor and mini content editor to perform saving operations */
        contentEditorPerformSave: function (args) {
            if (!angular.isObject(args)) {
                throw "args must be an object";
            }
            if (!args.scope) {
                throw "args.scope is not defined";
            }
            if (!args.content) {
                throw "args.content is not defined";
            }
            if (!args.statusMessage) {
                throw "args.statusMessage is not defined";
            }
            if (!args.saveMethod) {
                throw "args.saveMethod is not defined";
            }

            var redirectOnFailure = args.redirectOnFailure !== undefined ? args.redirectOnFailure : true;

            var self = this;

            //we will use the default one for content if not specified
            var rebindCallback = args.rebindCallback === undefined ? self.reBindChangedProperties : args.rebindCallback;

            var deferred = $q.defer();

            if (!args.scope.busy && formHelper.submitForm({ scope: args.scope, statusMessage: args.statusMessage, action: args.action })) {

                args.scope.busy = true;

                args.saveMethod(args.content, $routeParams.create, fileManager.getFiles())
                    .then(function (data) {

                        formHelper.resetForm({ scope: args.scope, notifications: data.notifications });

                        self.handleSuccessfulSave({
                            scope: args.scope,
                            savedContent: data,
                            rebindCallback: function() {
                                rebindCallback.apply(self, [args.content, data]);
                            }
                        });

                        args.scope.busy = false;
                        deferred.resolve(data);

                    }, function (err) {
                        self.handleSaveError({
                            redirectOnFailure: redirectOnFailure,
                            err: err,
                            rebindCallback: function() {
                                rebindCallback.apply(self, [args.content, err.data]);
                            }
                        });
                        //show any notifications
                        if (angular.isArray(err.data.notifications)) {
                            for (var i = 0; i < err.data.notifications.length; i++) {
                                notificationsService.showNotification(err.data.notifications[i]);
                            }
                        }
                        args.scope.busy = false;
                        deferred.reject(err);
                    });
            }
            else {
                deferred.reject();
            }

            return deferred.promise;
        },


        /** Returns the action button definitions based on what permissions the user has.
        The content.allowedActions parameter contains a list of chars, each represents a button by permission so
        here we'll build the buttons according to the chars of the user. */
        configureContentEditorButtons: function (args) {

            if (!angular.isObject(args)) {
                throw "args must be an object";
            }
            if (!args.content) {
                throw "args.content is not defined";
            }
            if (!args.methods) {
                throw "args.methods is not defined";
            }
            if (!args.methods.saveAndPublish || !args.methods.sendToPublish || !args.methods.save || !args.methods.unPublish) {
                throw "args.methods does not contain all required defined methods";
            }

            var buttons = {
                defaultButton: null,
                subButtons: []
            };

            function createButtonDefinition(ch) {
                switch (ch) {
                    case "U":
                        //publish action
                        return {
                            letter: ch,
                            labelKey: "buttons_saveAndPublish",
                            handler: args.methods.saveAndPublish,
                            hotKey: "ctrl+p",
                            hotKeyWhenHidden: true
                        };
                    case "H":
                        //send to publish
                        return {
                            letter: ch,
                            labelKey: "buttons_saveToPublish",
                            handler: args.methods.sendToPublish,
                            hotKey: "ctrl+p",
                            hotKeyWhenHidden: true
                        };
                    case "A":
                        //save
                        return {
                            letter: ch,
                            labelKey: "buttons_save",
                            handler: args.methods.save,
                            hotKey: "ctrl+s",
                            hotKeyWhenHidden: true
                        };
                    case "Z":
                        //unpublish
                        return {
                            letter: ch,
                            labelKey: "content_unPublish",
                            handler: args.methods.unPublish,
                            hotKey: "ctrl+u",
                            hotKeyWhenHidden: true
                        };
                    default:
                        return null;
                }
            }

            //reset
            buttons.subButtons = [];

            //This is the ideal button order but depends on circumstance, we'll use this array to create the button list
            // Publish, SendToPublish, Save
            var buttonOrder = ["U", "H", "A"];

            //Create the first button (primary button)
            //We cannot have the Save or SaveAndPublish buttons if they don't have create permissions when we are creating a new item.
            if (!args.create || _.contains(args.content.allowedActions, "C")) {
                for (var b in buttonOrder) {
                    if (_.contains(args.content.allowedActions, buttonOrder[b])) {
                        buttons.defaultButton = createButtonDefinition(buttonOrder[b]);
                        break;
                    }
                }
            }

            //Now we need to make the drop down button list, this is also slightly tricky because:
            //We cannot have any buttons if there's no default button above.
            //We cannot have the unpublish button (Z) when there's no publish permission.
            //We cannot have the unpublish button (Z) when the item is not published.
            if (buttons.defaultButton) {

                //get the last index of the button order
                var lastIndex = _.indexOf(buttonOrder, buttons.defaultButton.letter);
                //add the remaining
                for (var i = lastIndex + 1; i < buttonOrder.length; i++) {
                    if (_.contains(args.content.allowedActions, buttonOrder[i])) {
                        buttons.subButtons.push(createButtonDefinition(buttonOrder[i]));
                    }
                }


                //if we are not creating, then we should add unpublish too,
                // so long as it's already published and if the user has access to publish
                if (!args.create) {
                    if (args.content.publishDate && _.contains(args.content.allowedActions, "U")) {
                        buttons.subButtons.push(createButtonDefinition("Z"));
                    }
                }
            }

            return buttons;
        },

        /**
         * @ngdoc method
         * @name umbraco.services.contentEditingHelper#getAllProps
         * @methodOf umbraco.services.contentEditingHelper
         * @function
         *
         * @description
         * Returns all propertes contained for the content item (since the normal model has properties contained inside of tabs)
         */
        getAllProps: function (content) {
            var allProps = [];

            for (var i = 0; i < content.tabs.length; i++) {
                for (var p = 0; p < content.tabs[i].properties.length; p++) {
                    allProps.push(content.tabs[i].properties[p]);
                }
            }

            return allProps;
        },


        /**
         * @ngdoc method
         * @name umbraco.services.contentEditingHelper#configureButtons
         * @methodOf umbraco.services.contentEditingHelper
         * @function
         *
         * @description
         * Returns a letter array for buttons, with the primary one first based on content model, permissions and editor state
         */
         getAllowedActions : function(content, creating){

                //This is the ideal button order but depends on circumstance, we'll use this array to create the button list
                // Publish, SendToPublish, Save
                var actionOrder = ["U", "H", "A"];
                var defaultActions;
                var actions = [];

                //Create the first button (primary button)
                //We cannot have the Save or SaveAndPublish buttons if they don't have create permissions when we are creating a new item.
                if (!creating || _.contains(content.allowedActions, "C")) {
                    for (var b in actionOrder) {
                        if (_.contains(content.allowedActions, actionOrder[b])) {
                            defaultAction = actionOrder[b];
                            break;
                        }
                    }
                }

                actions.push(defaultAction);

                //Now we need to make the drop down button list, this is also slightly tricky because:
                //We cannot have any buttons if there's no default button above.
                //We cannot have the unpublish button (Z) when there's no publish permission.
                //We cannot have the unpublish button (Z) when the item is not published.
                if (defaultAction) {
                    //get the last index of the button order
                    var lastIndex = _.indexOf(actionOrder, defaultAction);

                    //add the remaining
                    for (var i = lastIndex + 1; i < actionOrder.length; i++) {
                        if (_.contains(content.allowedActions, actionOrder[i])) {
                            actions.push(actionOrder[i]);
                        }
                    }

                    //if we are not creating, then we should add unpublish too,
                    // so long as it's already published and if the user has access to publish
                    if (!creating) {
                        if (content.publishDate && _.contains(content.allowedActions,"U")) {
                            actions.push("Z");
                        }
                    }
                }

                return actions;
         },

         /**
          * @ngdoc method
          * @name umbraco.services.contentEditingHelper#getButtonFromAction
          * @methodOf umbraco.services.contentEditingHelper
          * @function
          *
          * @description
          * Returns a button object to render a button for the tabbed editor
          * currently only returns built in system buttons for content and media actions
          * returns label, alias, action char and hot-key
          */
          getButtonFromAction : function(ch){
            switch (ch) {
                case "U":
                    return {
                        letter: ch,
                        labelKey: "buttons_saveAndPublish",
                        handler: "saveAndPublish",
                        hotKey: "ctrl+p"
                    };
                case "H":
                    //send to publish
                    return {
                        letter: ch,
                        labelKey: "buttons_saveToPublish",
                        handler: "sendToPublish",
                        hotKey: "ctrl+p"
                    };
                case "A":
                    return {
                        letter: ch,
                        labelKey: "buttons_save",
                        handler: "save",
                        hotKey: "ctrl+s"
                    };
                case "Z":
                    return {
                        letter: ch,
                        labelKey: "content_unPublish",
                        handler: "unPublish"
                    };

                default:
                    return null;
            }

          },
        /**
         * @ngdoc method
         * @name umbraco.services.contentEditingHelper#reBindChangedProperties
         * @methodOf umbraco.services.contentEditingHelper
         * @function
         *
         * @description
         * re-binds all changed property values to the origContent object from the savedContent object and returns an array of changed properties.
         */
        reBindChangedProperties: function (origContent, savedContent) {

            var changed = [];

            //get a list of properties since they are contained in tabs
            var allOrigProps = this.getAllProps(origContent);
            var allNewProps = this.getAllProps(savedContent);

            function getNewProp(alias) {
                return _.find(allNewProps, function (item) {
                    return item.alias === alias;
                });
            }

            //a method to ignore built-in prop changes
            var shouldIgnore = function(propName) {
                return _.some(["tabs", "notifications", "ModelState", "tabs", "properties"], function(i) {
                    return i === propName;
                });
            };
            //check for changed built-in properties of the content
            for (var o in origContent) {

                //ignore the ones listed in the array
                if (shouldIgnore(o)) {
                    continue;
                }

                if (!_.isEqual(origContent[o], savedContent[o])) {
                    origContent[o] = savedContent[o];
                }
            }

            //check for changed properties of the content
            for (var p in allOrigProps) {
                var newProp = getNewProp(allOrigProps[p].alias);
                if (newProp && !_.isEqual(allOrigProps[p].value, newProp.value)) {

                    //they have changed so set the origContent prop to the new one
                    var origVal = allOrigProps[p].value;
                    allOrigProps[p].value = newProp.value;

                    //instead of having a property editor $watch their expression to check if it has
                    // been updated, instead we'll check for the existence of a special method on their model
                    // and just call it.
                    if (angular.isFunction(allOrigProps[p].onValueChanged)) {
                        //send the newVal + oldVal
                        allOrigProps[p].onValueChanged(allOrigProps[p].value, origVal);
                    }

                    changed.push(allOrigProps[p]);
                }
            }

            return changed;
        },

        /**
         * @ngdoc function
         * @name umbraco.services.contentEditingHelper#handleSaveError
         * @methodOf umbraco.services.contentEditingHelper
         * @function
         *
         * @description
         * A function to handle what happens when we have validation issues from the server side
         */
        handleSaveError: function (args) {

            if (!args.err) {
                throw "args.err cannot be null";
            }
            if (args.redirectOnFailure === undefined || args.redirectOnFailure === null) {
                throw "args.redirectOnFailure must be set to true or false";
            }

            //When the status is a 400 status with a custom header: X-Status-Reason: Validation failed, we have validation errors.
            //Otherwise the error is probably due to invalid data (i.e. someone mucking around with the ids or something).
            //Or, some strange server error
            if (args.err.status === 400) {
                //now we need to look through all the validation errors
                if (args.err.data && (args.err.data.ModelState)) {

                    //wire up the server validation errs
                    formHelper.handleServerValidation(args.err.data.ModelState);

                    if (!args.redirectOnFailure || !this.redirectToCreatedContent(args.err.data.id, args.err.data.ModelState)) {
                        //we are not redirecting because this is not new content, it is existing content. In this case
                        // we need to detect what properties have changed and re-bind them with the server data. Then we need
                        // to re-bind any server validation errors after the digest takes place.

                        if (args.rebindCallback && angular.isFunction(args.rebindCallback)) {
                            args.rebindCallback();
                        }

                        serverValidationManager.executeAndClearAllSubscriptions();
                    }

                    //indicates we've handled the server result
                    return true;
                }
                else {
                    dialogService.ysodDialog(args.err);
                }
            }
            else {
                dialogService.ysodDialog(args.err);
            }

            return false;
        },

        /**
         * @ngdoc function
         * @name umbraco.services.contentEditingHelper#handleSuccessfulSave
         * @methodOf umbraco.services.contentEditingHelper
         * @function
         *
         * @description
         * A function to handle when saving a content item is successful. This will rebind the values of the model that have changed
         * ensure the notifications are displayed and that the appropriate events are fired. This will also check if we need to redirect
         * when we're creating new content.
         */
        handleSuccessfulSave: function (args) {

            if (!args) {
                throw "args cannot be null";
            }
            if (!args.savedContent) {
                throw "args.savedContent cannot be null";
            }

            if (!this.redirectToCreatedContent(args.redirectId ? args.redirectId : args.savedContent.id)) {

                //we are not redirecting because this is not new content, it is existing content. In this case
                // we need to detect what properties have changed and re-bind them with the server data.
                //call the callback
                if (args.rebindCallback && angular.isFunction(args.rebindCallback)) {
                    args.rebindCallback();
                }
            }
        },

        /**
         * @ngdoc function
         * @name umbraco.services.contentEditingHelper#redirectToCreatedContent
         * @methodOf umbraco.services.contentEditingHelper
         * @function
         *
         * @description
         * Changes the location to be editing the newly created content after create was successful.
         * We need to decide if we need to redirect to edito mode or if we will remain in create mode.
         * We will only need to maintain create mode if we have not fulfilled the basic requirements for creating an entity which is at least having a name and ID
         */
        redirectToCreatedContent: function (id, modelState) {

            //only continue if we are currently in create mode and if there is no 'Name' modelstate errors
            // since we need at least a name to create content.
            if ($routeParams.create && (isValidIdentifier(id) && (!modelState || !modelState["Name"]))) {

                //need to change the location to not be in 'create' mode. Currently the route will be something like:
                // /belle/#/content/edit/1234?doctype=newsArticle&create=true
                // but we need to remove everything after the query so that it is just:
                // /belle/#/content/edit/9876 (where 9876 is the new id)

                //clear the query strings
                $location.search("");

                //change to new path
                $location.path("/" + $routeParams.section + "/" + $routeParams.tree  + "/" + $routeParams.method + "/" + id);
                //don't add a browser history for this
                $location.replace();
                return true;
            }
            return false;
        }
    };
}
angular.module('umbraco.services').factory('contentEditingHelper', contentEditingHelper);

/**
 * @ngdoc service
 * @name umbraco.services.contentTypeHelper
 * @description A helper service for the content type editor
 **/
function contentTypeHelper(contentTypeResource, dataTypeResource, $filter, $injector, $q) {

    var contentTypeHelperService = {

        createIdArray: function(array) {

          var newArray = [];

          angular.forEach(array, function(arrayItem){

            if(angular.isObject(arrayItem)) {
              newArray.push(arrayItem.id);
            } else {
              newArray.push(arrayItem);
            }

          });

          return newArray;

        },

        generateModels: function () {
            var deferred = $q.defer();
            var modelsResource = $injector.has("modelsBuilderResource") ? $injector.get("modelsBuilderResource") : null;
            var modelsBuilderEnabled = Umbraco.Sys.ServerVariables.umbracoPlugins.modelsBuilder.enabled;
            if (modelsBuilderEnabled && modelsResource) {
                modelsResource.buildModels().then(function(result) {
                    deferred.resolve(result);

                    //just calling this to get the servar back to life
                    modelsResource.getModelsOutOfDateStatus();

                }, function(e) {
                    deferred.reject(e);
                });
            }
            else {                
                deferred.resolve(false);                
            }
            return deferred.promise;
        },

        checkModelsBuilderStatus: function () {
            var deferred = $q.defer();
            var modelsResource = $injector.has("modelsBuilderResource") ? $injector.get("modelsBuilderResource") : null;
            var modelsBuilderEnabled = (Umbraco && Umbraco.Sys && Umbraco.Sys.ServerVariables && Umbraco.Sys.ServerVariables.umbracoPlugins && Umbraco.Sys.ServerVariables.umbracoPlugins.modelsBuilder && Umbraco.Sys.ServerVariables.umbracoPlugins.modelsBuilder.enabled === true);            
            
            if (modelsBuilderEnabled && modelsResource) {
                modelsResource.getModelsOutOfDateStatus().then(function(result) {
                    //Generate models buttons should be enabled if it is 0
                    deferred.resolve(result.status === 0);
                });
            }
            else {
                deferred.resolve(false);
            }
            return deferred.promise;
        },

        makeObjectArrayFromId: function (idArray, objectArray) {
           var newArray = [];

           for (var idIndex = 0; idArray.length > idIndex; idIndex++) {
             var id = idArray[idIndex];

             for (var objectIndex = 0; objectArray.length > objectIndex; objectIndex++) {
                 var object = objectArray[objectIndex];
                 if (id === object.id) {
                    newArray.push(object);
                 }
             }

           }

           return newArray;
        },

        validateAddingComposition: function(contentType, compositeContentType) {

            //Validate that by adding this group that we are not adding duplicate property type aliases

            var propertiesAdding = _.flatten(_.map(compositeContentType.groups, function(g) {
                return _.map(g.properties, function(p) {
                    return p.alias;
                });
            }));
            var propAliasesExisting = _.filter(_.flatten(_.map(contentType.groups, function(g) {
                return _.map(g.properties, function(p) {
                    return p.alias;
                });
            })), function(f) {
                return f !== null && f !== undefined;
            });

            var intersec = _.intersection(propertiesAdding, propAliasesExisting);
            if (intersec.length > 0) {
                //return the overlapping property aliases
                return intersec;
            }

            //no overlapping property aliases
            return [];
        },

        mergeCompositeContentType: function(contentType, compositeContentType) {

            //Validate that there are no overlapping aliases
            var overlappingAliases = this.validateAddingComposition(contentType, compositeContentType);
            if (overlappingAliases.length > 0) {
                throw new Error("Cannot add this composition, these properties already exist on the content type: " + overlappingAliases.join());
            }

           angular.forEach(compositeContentType.groups, function(compositionGroup) {

              // order composition groups based on sort order
              compositionGroup.properties = $filter('orderBy')(compositionGroup.properties, 'sortOrder');

              // get data type details
              angular.forEach(compositionGroup.properties, function(property) {
                 dataTypeResource.getById(property.dataTypeId)
                    .then(function(dataType) {
                       property.dataTypeIcon = dataType.icon;
                       property.dataTypeName = dataType.name;
                    });
              });

              // set inherited state on tab
              compositionGroup.inherited = true;

              // set inherited state on properties
              angular.forEach(compositionGroup.properties, function(compositionProperty) {
                 compositionProperty.inherited = true;
              });

              // set tab state
              compositionGroup.tabState = "inActive";

              // if groups are named the same - merge the groups
              angular.forEach(contentType.groups, function(contentTypeGroup) {

                 if (contentTypeGroup.name === compositionGroup.name) {

                    // set flag to show if properties has been merged into a tab
                    compositionGroup.groupIsMerged = true;

                    // make group inherited
                    contentTypeGroup.inherited = true;

                    // add properties to the top of the array
                    contentTypeGroup.properties = compositionGroup.properties.concat(contentTypeGroup.properties);

                    // update sort order on all properties in merged group
                    contentTypeGroup.properties = contentTypeHelperService.updatePropertiesSortOrder(contentTypeGroup.properties);

                    // make parentTabContentTypeNames to an array so we can push values
                    if (contentTypeGroup.parentTabContentTypeNames === null || contentTypeGroup.parentTabContentTypeNames === undefined) {
                       contentTypeGroup.parentTabContentTypeNames = [];
                    }

                    // push name to array of merged composite content types
                    contentTypeGroup.parentTabContentTypeNames.push(compositeContentType.name);

                    // make parentTabContentTypes to an array so we can push values
                    if (contentTypeGroup.parentTabContentTypes === null || contentTypeGroup.parentTabContentTypes === undefined) {
                       contentTypeGroup.parentTabContentTypes = [];
                    }

                    // push id to array of merged composite content types
                    contentTypeGroup.parentTabContentTypes.push(compositeContentType.id);

                    // get sort order from composition
                    contentTypeGroup.sortOrder = compositionGroup.sortOrder;

                    // splice group to the top of the array
                    var contentTypeGroupCopy = angular.copy(contentTypeGroup);
                    var index = contentType.groups.indexOf(contentTypeGroup);
                    contentType.groups.splice(index, 1);
                    contentType.groups.unshift(contentTypeGroupCopy);

                 }

              });

              // if group is not merged - push it to the end of the array - before init tab
              if (compositionGroup.groupIsMerged === false || compositionGroup.groupIsMerged === undefined) {

                 // make parentTabContentTypeNames to an array so we can push values
                 if (compositionGroup.parentTabContentTypeNames === null || compositionGroup.parentTabContentTypeNames === undefined) {
                    compositionGroup.parentTabContentTypeNames = [];
                 }

                 // push name to array of merged composite content types
                 compositionGroup.parentTabContentTypeNames.push(compositeContentType.name);

                 // make parentTabContentTypes to an array so we can push values
                 if (compositionGroup.parentTabContentTypes === null || compositionGroup.parentTabContentTypes === undefined) {
                    compositionGroup.parentTabContentTypes = [];
                 }

                 // push id to array of merged composite content types
                 compositionGroup.parentTabContentTypes.push(compositeContentType.id);
                  
                 // push group before placeholder tab
                 contentType.groups.unshift(compositionGroup);

              }

           });

           // sort all groups by sortOrder property
           contentType.groups = $filter('orderBy')(contentType.groups, 'sortOrder');

           return contentType;

        },

        splitCompositeContentType: function (contentType, compositeContentType) {

            var groups = [];

            angular.forEach(contentType.groups, function(contentTypeGroup){

                if( contentTypeGroup.tabState !== "init" ) {

                    var idIndex = contentTypeGroup.parentTabContentTypes.indexOf(compositeContentType.id);
                    var nameIndex = contentTypeGroup.parentTabContentTypeNames.indexOf(compositeContentType.name);
                    var groupIndex = contentType.groups.indexOf(contentTypeGroup);


                    if( idIndex !== -1  ) {

                        var properties = [];

                        // remove all properties from composite content type
                        angular.forEach(contentTypeGroup.properties, function(property){
                            if(property.contentTypeId !== compositeContentType.id) {
                                properties.push(property);
                            }
                        });

                        // set new properties array to properties
                        contentTypeGroup.properties = properties;

                        // remove composite content type name and id from inherited arrays
                        contentTypeGroup.parentTabContentTypes.splice(idIndex, 1);
                        contentTypeGroup.parentTabContentTypeNames.splice(nameIndex, 1);

                        // remove inherited state if there are no inherited properties
                        if(contentTypeGroup.parentTabContentTypes.length === 0) {
                            contentTypeGroup.inherited = false;
                        }

                        // remove group if there are no properties left
                        if(contentTypeGroup.properties.length > 1) {
                            //contentType.groups.splice(groupIndex, 1);
                            groups.push(contentTypeGroup);
                        }

                    } else {
                      groups.push(contentTypeGroup);
                    }

                } else {
                  groups.push(contentTypeGroup);
                }

                // update sort order on properties
                contentTypeGroup.properties = contentTypeHelperService.updatePropertiesSortOrder(contentTypeGroup.properties);

            });

            contentType.groups = groups;

        },

        updatePropertiesSortOrder: function (properties) {

          var sortOrder = 0;

          angular.forEach(properties, function(property) {
            if( !property.inherited && property.propertyState !== "init") {
              property.sortOrder = sortOrder;
            }
            sortOrder++;
          });

          return properties;

        },

        getTemplatePlaceholder: function() {

          var templatePlaceholder = {
            "name": "",
            "icon": "icon-layout",
            "alias": "templatePlaceholder",
            "placeholder": true
          };

          return templatePlaceholder;

        },

        insertDefaultTemplatePlaceholder: function(defaultTemplate) {

          // get template placeholder
          var templatePlaceholder = contentTypeHelperService.getTemplatePlaceholder();

          // add as default template
          defaultTemplate = templatePlaceholder;

          return defaultTemplate;

        },

        insertTemplatePlaceholder: function(array) {

          // get template placeholder
          var templatePlaceholder = contentTypeHelperService.getTemplatePlaceholder();

          // add as selected item
          array.push(templatePlaceholder);

          return array;

       },

       insertChildNodePlaceholder: function (array, name, icon, id) {

         var placeholder = {
           "name": name,
           "icon": icon,
           "id": id
         };

         array.push(placeholder);

       }

    };

    return contentTypeHelperService;
}
angular.module('umbraco.services').factory('contentTypeHelper', contentTypeHelper);

/**
* @ngdoc service
* @name umbraco.services.cropperHelper
* @description A helper object used for dealing with image cropper data
**/
function cropperHelper(umbRequestHelper, $http) {
	var service = {

		/**
		* @ngdoc method
		* @name umbraco.services.cropperHelper#configuration
		* @methodOf umbraco.services.cropperHelper
		*
		* @description
		* Returns a collection of plugins available to the tinyMCE editor
		*
		*/
		configuration: function (mediaTypeAlias) {
			return umbRequestHelper.resourcePromise(
				$http.get(
					umbRequestHelper.getApiUrl(
						"imageCropperApiBaseUrl",
						"GetConfiguration",
						[{ mediaTypeAlias: mediaTypeAlias}])),
				'Failed to retrieve tinymce configuration');
		},


		//utill for getting either min/max aspect ratio to scale image after
		calculateAspectRatioFit : function(srcWidth, srcHeight, maxWidth, maxHeight, maximize) {
			var ratio = [maxWidth / srcWidth, maxHeight / srcHeight ];

			if(maximize){
				ratio = Math.max(ratio[0], ratio[1]);
			}else{
				ratio = Math.min(ratio[0], ratio[1]);
			}

			return { width:srcWidth*ratio, height:srcHeight*ratio, ratio: ratio};
		},

		//utill for scaling width / height given a ratio
		calculateSizeToRatio : function(srcWidth, srcHeight, ratio) {
			return { width:srcWidth*ratio, height:srcHeight*ratio, ratio: ratio};
		},

		scaleToMaxSize : function(srcWidth, srcHeight, maxSize) {
			
			var retVal = {height: srcHeight, width: srcWidth};

			if(srcWidth > maxSize ||srcHeight > maxSize){
				var ratio = [maxSize / srcWidth, maxSize / srcHeight ];
				ratio = Math.min(ratio[0], ratio[1]);
				
				retVal.height = srcHeight * ratio;
				retVal.width = srcWidth * ratio;
			}
			
			return retVal;			
		},

		//returns a ng-style object with top,left,width,height pixel measurements
		//expects {left,right,top,bottom} - {width,height}, {width,height}, int
		//offset is just to push the image position a number of pixels from top,left    
		convertToStyle : function(coordinates, originalSize, viewPort, offset){

			var coordinates_px = service.coordinatesToPixels(coordinates, originalSize, offset);
			var _offset = offset || 0;

			var x = 1 - (coordinates.x1 + Math.abs(coordinates.x2));
			var left_of_x =  originalSize.width * x;
			var ratio = viewPort.width / left_of_x;

			var style = {
				position: "absolute",
				top:  -(coordinates_px.y1*ratio)+ _offset,
				left:  -(coordinates_px.x1* ratio)+ _offset,
				width: Math.floor(originalSize.width * ratio),
				height: Math.floor(originalSize.height * ratio),
				originalWidth: originalSize.width,
				originalHeight: originalSize.height,
				ratio: ratio
			};

			return style;
		},

		 
		coordinatesToPixels : function(coordinates, originalSize, offset){

			var coordinates_px = {
				x1: Math.floor(coordinates.x1 * originalSize.width),
				y1: Math.floor(coordinates.y1 * originalSize.height),
				x2: Math.floor(coordinates.x2 * originalSize.width),
				y2: Math.floor(coordinates.y2 * originalSize.height)								 
			};

			return coordinates_px;
		},

		pixelsToCoordinates : function(image, width, height, offset){

			var x1_px = Math.abs(image.left-offset);
			var y1_px = Math.abs(image.top-offset);

			var x2_px = image.width - (x1_px + width);
			var y2_px = image.height - (y1_px + height);


			//crop coordinates in %
			var crop = {};
			crop.x1 = x1_px / image.width;
			crop.y1 = y1_px / image.height;
			crop.x2 = x2_px / image.width;
			crop.y2 = y2_px / image.height;

			for(var coord in crop){
				if(crop[coord] < 0){
				    crop[coord] = 0;
				}
			} 

			return crop;
		},

		centerInsideViewPort : function(img, viewport){
			var left = viewport.width/ 2 - img.width / 2,
				top = viewport.height / 2 - img.height / 2;
			
			return {left: left, top: top};
		},

		alignToCoordinates : function(image, center, viewport){
			
			var min_left = (image.width) - (viewport.width);
			var min_top =  (image.height) - (viewport.height);

			var c_top = -(center.top * image.height) + (viewport.height / 2);
			var c_left = -(center.left * image.width) + (viewport.width / 2);

			if(c_top < -min_top){
				c_top = -min_top;
			}
			if(c_top > 0){
				c_top = 0;
			}
			if(c_left < -min_left){
				c_left = -min_left;
			}
			if(c_left > 0){
				c_left = 0;
			}
			return {left: c_left, top: c_top};
		},


		syncElements : function(source, target){
				target.height(source.height());
				target.width(source.width());

				target.css({
					"top": source[0].offsetTop,
					"left": source[0].offsetLeft
				});
		}
	};

	return service;
}

angular.module('umbraco.services').factory('cropperHelper', cropperHelper);

/**
 * @ngdoc service
 * @name umbraco.services.dataTypeHelper
 * @description A helper service for data types
 **/
function dataTypeHelper() {

    var dataTypeHelperService = {

        createPreValueProps: function(preVals) {

            var preValues = [];

            for (var i = 0; i < preVals.length; i++) {
                preValues.push({
                    hideLabel: preVals[i].hideLabel,
                    alias: preVals[i].key,
                    description: preVals[i].description,
                    label: preVals[i].label,
                    view: preVals[i].view,
                    value: preVals[i].value
                });
            }

            return preValues;

        },

        rebindChangedProperties: function (origContent, savedContent) {

            //a method to ignore built-in prop changes
            var shouldIgnore = function (propName) {
                return _.some(["notifications", "ModelState"], function (i) {
                    return i === propName;
                });
            };
            //check for changed built-in properties of the content
            for (var o in origContent) {

                //ignore the ones listed in the array
                if (shouldIgnore(o)) {
                    continue;
                }

                if (!_.isEqual(origContent[o], savedContent[o])) {
                    origContent[o] = savedContent[o];
                }
            }
        }

    };

    return dataTypeHelperService;
}
angular.module('umbraco.services').factory('dataTypeHelper', dataTypeHelper);
/**
 * @ngdoc service
 * @name umbraco.services.dialogService
 *
 * @requires $rootScope
 * @requires $compile
 * @requires $http
 * @requires $log
 * @requires $q
 * @requires $templateCache
 *
 * @description
 * Application-wide service for handling modals, overlays and dialogs
 * By default it injects the passed template url into a div to body of the document
 * And renders it, but does also support rendering items in an iframe, incase
 * serverside processing is needed, or its a non-angular page
 *
 * ##usage
 * To use, simply inject the dialogService into any controller that needs it, and make
 * sure the umbraco.services module is accesible - which it should be by default.
 *
 * <pre>
 *    var dialog = dialogService.open({template: 'path/to/page.html', show: true, callback: done});
 *    functon done(data){
 *      //The dialog has been submitted
 *      //data contains whatever the dialog has selected / attached
 *    }
 * </pre>
 */

angular.module('umbraco.services')
.factory('dialogService', function ($rootScope, $compile, $http, $timeout, $q, $templateCache, appState, eventsService) {

    var dialogs = [];

    /** Internal method that removes all dialogs */
    function removeAllDialogs(args) {
        for (var i = 0; i < dialogs.length; i++) {
            var dialog = dialogs[i];

            //very special flag which means that global events cannot close this dialog - currently only used on the login
            // dialog since it's special and cannot be closed without logging in.
            if (!dialog.manualClose) {
                dialog.close(args);
            }

        }
    }

    /** Internal method that closes the dialog properly and cleans up resources */
    function closeDialog(dialog) {

        if (dialog.element) {
            dialog.element.modal('hide');

            //this is not entirely enough since the damn webforms scriploader still complains
            if (dialog.iframe) {
                dialog.element.find("iframe").attr("src", "about:blank");
            }

            dialog.scope.$destroy();

            //we need to do more than just remove the element, this will not destroy the
            // scope in angular 1.1x, in angular 1.2x this is taken care of but if we dont
            // take care of this ourselves we have memory leaks.
            dialog.element.remove();

            //remove 'this' dialog from the dialogs array
            dialogs = _.reject(dialogs, function (i) { return i === dialog; });
        }
    }

    /** Internal method that handles opening all dialogs */
    function openDialog(options) {
        var defaults = {
            container: $("body"),
            animation: "fade",
            modalClass: "umb-modal",
            width: "100%",
            inline: false,
            iframe: false,
            show: true,
            template: "views/common/notfound.html",
            callback: undefined,
            closeCallback: undefined,
            element: undefined,
            // It will set this value as a property on the dialog controller's scope as dialogData,
            // used to pass in custom data to the dialog controller's $scope. Though this is near identical to
            // the dialogOptions property that is also set the the dialog controller's $scope object.
            // So there's basically 2 ways of doing the same thing which we're now stuck with and in fact
            // dialogData has another specially attached property called .selection which gets used.
            dialogData: undefined
        };

        var dialog = angular.extend(defaults, options);

        //NOTE: People should NOT pass in a scope object that is legacy functoinality and causes problems. We will ALWAYS
        // destroy the scope when the dialog is closed regardless if it is in use elsewhere which is why it shouldn't be done.
        var scope = options.scope || $rootScope.$new();

        //Modal dom obj and set id to old-dialog-service - used until we get all dialogs moved the the new overlay directive
        dialog.element = $('<div ng-swipe-right="swipeHide($event)"  data-backdrop="false"></div>');
        var id = "old-dialog-service";

        if (options.inline) {
            dialog.animation = "";
        }
        else {
            dialog.element.addClass("modal");
            dialog.element.addClass("hide");
        }

        //set the id and add classes
        dialog.element
            .attr('id', id)
            .addClass(dialog.animation)
            .addClass(dialog.modalClass);

        //push the modal into the global modal collection
        //we halt the .push because a link click will trigger a closeAll right away
        $timeout(function () {
            dialogs.push(dialog);
        }, 500);


        dialog.close = function (data) {
            if (dialog.closeCallback) {
                dialog.closeCallback(data);
            }

            closeDialog(dialog);
        };

        //if iframe is enabled, inject that instead of a template
        if (dialog.iframe) {
            var html = $("<iframe src='" + dialog.template + "' class='auto-expand' style='border: none; width: 100%; height: 100%;'></iframe>");
            dialog.element.html(html);

            //append to body or whatever element is passed in as options.containerElement
            dialog.container.append(dialog.element);

            // Compile modal content
            $timeout(function () {
                $compile(dialog.element)(dialog.scope);
            });

            dialog.element.css("width", dialog.width);

            //Autoshow
            if (dialog.show) {
                dialog.element.modal('show');
            }

            dialog.scope = scope;
            return dialog;
        }
        else {

            //We need to load the template with an httpget and once it's loaded we'll compile and assign the result to the container
            // object. However since the result could be a promise or just data we need to use a $q.when. We still need to return the
            // $modal object so we'll actually return the modal object synchronously without waiting for the promise. Otherwise this openDialog
            // method will always need to return a promise which gets nasty because of promises in promises plus the result just needs a reference
            // to the $modal object which will not change (only it's contents will change).
            $q.when($templateCache.get(dialog.template) || $http.get(dialog.template, { cache: true }).then(function (res) { return res.data; }))
                .then(function onSuccess(template) {

                    // Build modal object
                    dialog.element.html(template);

                    //append to body or other container element
                    dialog.container.append(dialog.element);

                    // Compile modal content
                    $timeout(function () {
                        $compile(dialog.element)(scope);
                    });

                    scope.dialogOptions = dialog;

                    //Scope to handle data from the modal form
                    scope.dialogData = dialog.dialogData ? dialog.dialogData : {};
                    scope.dialogData.selection = [];

                    // Provide scope display functions
                    //this passes the modal to the current scope
                    scope.$modal = function (name) {
                        dialog.element.modal(name);
                    };

                    scope.swipeHide = function (e) {

                        if (appState.getGlobalState("touchDevice")) {
                            var selection = window.getSelection();
                            if (selection.type !== "Range") {
                                scope.hide();
                            }
                        }
                    };

                    //NOTE: Same as 'close' without the callbacks
                    scope.hide = function () {
                        closeDialog(dialog);
                    };

                    //basic events for submitting and closing
                    scope.submit = function (data) {
                        if (dialog.callback) {
                            dialog.callback(data);
                        }

                        closeDialog(dialog);
                    };

                    scope.close = function (data) {
                        dialog.close(data);
                    };

                    //NOTE: This can ONLY ever be used to show the dialog if dialog.show is false (autoshow).
                    // You CANNOT call show() after you call hide(). hide = close, they are the same thing and once
                    // a dialog is closed it's resources are disposed of.
                    scope.show = function () {
                        if (dialog.manualClose === true) {
                            //show and configure that the keyboard events are not enabled on this modal
                            dialog.element.modal({ keyboard: false });
                        }
                        else {
                            //just show normally
                            dialog.element.modal('show');
                        }

                    };

                    scope.select = function (item) {
                        var i = scope.dialogData.selection.indexOf(item);
                        if (i < 0) {
                            scope.dialogData.selection.push(item);
                        } else {
                            scope.dialogData.selection.splice(i, 1);
                        }
                    };

                    //NOTE: Same as 'close' without the callbacks
                    scope.dismiss = scope.hide;

                    // Emit modal events
                    angular.forEach(['show', 'shown', 'hide', 'hidden'], function (name) {
                        dialog.element.on(name, function (ev) {
                            scope.$emit('modal-' + name, ev);
                        });
                    });

                    // Support autofocus attribute
                    dialog.element.on('shown', function (event) {
                        $('input[autofocus]', dialog.element).first().trigger('focus');
                    });

                    dialog.scope = scope;

                    //Autoshow
                    if (dialog.show) {
                        scope.show();
                    }

                });

            //Return the modal object outside of the promise!
            return dialog;
        }
    }

    /** Handles the closeDialogs event */
    eventsService.on("app.closeDialogs", function (evt, args) {
        removeAllDialogs(args);
    });

    return {
        /**
         * @ngdoc method
         * @name umbraco.services.dialogService#open
         * @methodOf umbraco.services.dialogService
         *
         * @description
         * Opens a modal rendering a given template url.
         *
         * @param {Object} options rendering options
         * @param {DomElement} options.container the DOM element to inject the modal into, by default set to body
         * @param {Function} options.callback function called when the modal is submitted
         * @param {String} options.template the url of the template
         * @param {String} options.animation animation csss class, by default set to "fade"
         * @param {String} options.modalClass modal css class, by default "umb-modal"
         * @param {Bool} options.show show the modal instantly
         * @param {Bool} options.iframe load template in an iframe, only needed for serverside templates
         * @param {Int} options.width set a width on the modal, only needed for iframes
         * @param {Bool} options.inline strips the modal from any animation and wrappers, used when you want to inject a dialog into an existing container
         * @returns {Object} modal object
         */
        open: function (options) {
            return openDialog(options);
        },

        /**
         * @ngdoc method
         * @name umbraco.services.dialogService#close
         * @methodOf umbraco.services.dialogService
         *
         * @description
         * Closes a specific dialog
         * @param {Object} dialog the dialog object to close
         * @param {Object} args if specified this object will be sent to any callbacks registered on the dialogs.
         */
        close: function (dialog, args) {
            if (dialog) {
                dialog.close(args);
            }
        },

        /**
         * @ngdoc method
         * @name umbraco.services.dialogService#closeAll
         * @methodOf umbraco.services.dialogService
         *
         * @description
         * Closes all dialogs
         * @param {Object} args if specified this object will be sent to any callbacks registered on the dialogs.
         */
        closeAll: function (args) {
            removeAllDialogs(args);
        },

        /**
         * @ngdoc method
         * @name umbraco.services.dialogService#mediaPicker
         * @methodOf umbraco.services.dialogService
         *
         * @description
         * Opens a media picker in a modal, the callback returns an array of selected media items
         * @param {Object} options mediapicker dialog options object
         * @param {Boolean} options.onlyImages Only display files that have an image file-extension
         * @param {Function} options.callback callback function
         * @returns {Object} modal object
         */
        mediaPicker: function (options) {
            options.template = 'views/common/dialogs/mediaPicker.html';
            options.show = true;
            return openDialog(options);
        },


        /**
         * @ngdoc method
         * @name umbraco.services.dialogService#contentPicker
         * @methodOf umbraco.services.dialogService
         *
         * @description
         * Opens a content picker tree in a modal, the callback returns an array of selected documents
         * @param {Object} options content picker dialog options object
         * @param {Boolean} options.multiPicker should the picker return one or multiple items
         * @param {Function} options.callback callback function
         * @returns {Object} modal object
         */
        contentPicker: function (options) {

            options.treeAlias = "content";
            options.section = "content";

            return this.treePicker(options);
        },

        /**
         * @ngdoc method
         * @name umbraco.services.dialogService#linkPicker
         * @methodOf umbraco.services.dialogService
         *
         * @description
         * Opens a link picker tree in a modal, the callback returns a single link
         * @param {Object} options content picker dialog options object
         * @param {Function} options.callback callback function
         * @returns {Object} modal object
         */
        linkPicker: function (options) {
            options.template = 'views/common/dialogs/linkPicker.html';
            options.show = true;
            return openDialog(options);
        },

        /**
         * @ngdoc method
         * @name umbraco.services.dialogService#macroPicker
         * @methodOf umbraco.services.dialogService
         *
         * @description
         * Opens a mcaro picker in a modal, the callback returns a object representing the macro and it's parameters
         * @param {Object} options macropicker dialog options object
         * @param {Function} options.callback callback function
         * @returns {Object} modal object
         */
        macroPicker: function (options) {
            options.template = 'views/common/dialogs/insertmacro.html';
            options.show = true;
            options.modalClass = "span7 umb-modal";
            return openDialog(options);
        },

        /**
         * @ngdoc method
         * @name umbraco.services.dialogService#memberPicker
         * @methodOf umbraco.services.dialogService
         *
         * @description
         * Opens a member picker in a modal, the callback returns a object representing the selected member
         * @param {Object} options member picker dialog options object
         * @param {Boolean} options.multiPicker should the tree pick one or multiple members before returning
         * @param {Function} options.callback callback function
         * @returns {Object} modal object
         */
        memberPicker: function (options) {

            options.treeAlias = "member";
            options.section = "member";

            return this.treePicker(options);
        },

        /**
         * @ngdoc method
         * @name umbraco.services.dialogService#memberGroupPicker
         * @methodOf umbraco.services.dialogService
         *
         * @description
         * Opens a member group picker in a modal, the callback returns a object representing the selected member
         * @param {Object} options member group picker dialog options object
         * @param {Boolean} options.multiPicker should the tree pick one or multiple members before returning
         * @param {Function} options.callback callback function
         * @returns {Object} modal object
         */
        memberGroupPicker: function (options) {
            options.template = 'views/common/dialogs/memberGroupPicker.html';
            options.show = true;
            return openDialog(options);
        },

        /**
         * @ngdoc method
         * @name umbraco.services.dialogService#iconPicker
         * @methodOf umbraco.services.dialogService
         *
         * @description
         * Opens a icon picker in a modal, the callback returns a object representing the selected icon
         * @param {Object} options iconpicker dialog options object
         * @param {Function} options.callback callback function
         * @returns {Object} modal object
         */
        iconPicker: function (options) {
            options.template = 'views/common/dialogs/iconPicker.html';
            options.show = true;
            return openDialog(options);
        },

        /**
         * @ngdoc method
         * @name umbraco.services.dialogService#treePicker
         * @methodOf umbraco.services.dialogService
         *
         * @description
         * Opens a tree picker in a modal, the callback returns a object representing the selected tree item
         * @param {Object} options iconpicker dialog options object
         * @param {String} options.section tree section to display
         * @param {String} options.treeAlias specific tree to display
         * @param {Boolean} options.multiPicker should the tree pick one or multiple items before returning
         * @param {Function} options.callback callback function
         * @returns {Object} modal object
         */
        treePicker: function (options) {
            options.template = 'views/common/dialogs/treePicker.html';
            options.show = true;
            return openDialog(options);
        },

        /**
         * @ngdoc method
         * @name umbraco.services.dialogService#propertyDialog
         * @methodOf umbraco.services.dialogService
         *
         * @description
         * Opens a dialog with a chosen property editor in, a value can be passed to the modal, and this value is returned in the callback
         * @param {Object} options mediapicker dialog options object
         * @param {Function} options.callback callback function
         * @param {String} editor editor to use to edit a given value and return on callback
         * @param {Object} value value sent to the property editor
         * @returns {Object} modal object
         */
        //TODO: Wtf does this do? I don't think anything!
        propertyDialog: function (options) {
            options.template = 'views/common/dialogs/property.html';
            options.show = true;
            return openDialog(options);
        },

        /**
        * @ngdoc method
        * @name umbraco.services.dialogService#embedDialog
        * @methodOf umbraco.services.dialogService
        * @description
        * Opens a dialog to an embed dialog
        */
        embedDialog: function (options) {
            options.template = 'views/common/dialogs/rteembed.html';
            options.show = true;
            return openDialog(options);
        },
        /**
        * @ngdoc method
        * @name umbraco.services.dialogService#ysodDialog
        * @methodOf umbraco.services.dialogService
        *
        * @description
        * Opens a dialog to show a custom YSOD
        */
        ysodDialog: function (ysodError) {

            var newScope = $rootScope.$new();
            newScope.error = ysodError;
            return openDialog({
                modalClass: "umb-modal wide ysod",
                scope: newScope,
                //callback: options.callback,
                template: 'views/common/dialogs/ysod.html',
                show: true
            });
        },

        confirmDialog: function (ysodError) {

            options.template = 'views/common/dialogs/confirm.html';
            options.show = true;
            return openDialog(options);
        }
    };
});

/** Used to broadcast and listen for global events and allow the ability to add async listeners to the callbacks */

/*
    Core app events: 

    app.ready
    app.authenticated
    app.notAuthenticated
    app.closeDialogs
*/

function eventsService($q, $rootScope) {
	
    return {
        
        /** raise an event with a given name, returns an array of promises for each listener */
        emit: function (name, args) {            

            //there are no listeners
            if (!$rootScope.$$listeners[name]) {
                return;
                //return [];
            }

            //send the event
            $rootScope.$emit(name, args);


            //PP: I've commented out the below, since we currently dont
            // expose the eventsService as a documented api
            // and think we need to figure out our usecases for this
            // since the below modifies the return value of the then on() method
            /*
            //setup a deferred promise for each listener
            var deferred = [];
            for (var i = 0; i < $rootScope.$$listeners[name].length; i++) {
                deferred.push($q.defer());
            }*/

            //create a new event args object to pass to the 
            // $emit containing methods that will allow listeners
            // to return data in an async if required
            /*
            var eventArgs = {
                args: args,
                reject: function (a) {
                    deferred.pop().reject(a);
                },
                resolve: function (a) {
                    deferred.pop().resolve(a);
                }
            };*/
            
            
            
            /*
            //return an array of promises
            var promises = _.map(deferred, function(p) {
                return p.promise;
            });
            return promises;*/
        },

        /** subscribe to a method, or use scope.$on = same thing */
		on: function(name, callback) {
		    return $rootScope.$on(name, callback);
		},
		
        /** pass in the result of 'on' to this method, or just call the method returned from 'on' to unsubscribe */
		unsubscribe: function(handle) {
		    if (angular.isFunction(handle)) {
		        handle();
		    }		    
		}
	};
}

angular.module('umbraco.services').factory('eventsService', eventsService);
/**
 * @ngdoc service
 * @name umbraco.services.fileManager
 * @function
 *
 * @description
 * Used by editors to manage any files that require uploading with the posted data, normally called by property editors
 * that need to attach files.
 * When a route changes successfully, we ensure that the collection is cleared.
 */
function fileManager() {

    var fileCollection = [];

    return {
        /**
         * @ngdoc function
         * @name umbraco.services.fileManager#addFiles
         * @methodOf umbraco.services.fileManager
         * @function
         *
         * @description
         *  Attaches files to the current manager for the current editor for a particular property, if an empty array is set
         *   for the files collection that effectively clears the files for the specified editor.
         */
        setFiles: function(propertyAlias, files) {
            //this will clear the files for the current property and then add the new ones for the current property
            fileCollection = _.reject(fileCollection, function (item) {
                return item.alias === propertyAlias;
            });
            for (var i = 0; i < files.length; i++) {
                //save the file object to the files collection
                fileCollection.push({ alias: propertyAlias, file: files[i] });
            }
        },
        
        /**
         * @ngdoc function
         * @name umbraco.services.fileManager#getFiles
         * @methodOf umbraco.services.fileManager
         * @function
         *
         * @description
         *  Returns all of the files attached to the file manager
         */
        getFiles: function() {
            return fileCollection;
        },
        
        /**
         * @ngdoc function
         * @name umbraco.services.fileManager#clearFiles
         * @methodOf umbraco.services.fileManager
         * @function
         *
         * @description
         *  Removes all files from the manager
         */
        clearFiles: function () {
            fileCollection = [];
        }
};
}

angular.module('umbraco.services').factory('fileManager', fileManager);
/**
 * @ngdoc service
 * @name umbraco.services.formHelper
 * @function
 *
 * @description
 * A utility class used to streamline how forms are developed, to ensure that validation is check and displayed consistently and to ensure that the correct events
 * fire when they need to.
 */
function formHelper(angularHelper, serverValidationManager, $timeout, notificationsService, dialogService, localizationService) {
    return {

        /**
         * @ngdoc function
         * @name umbraco.services.formHelper#submitForm
         * @methodOf umbraco.services.formHelper
         * @function
         *
         * @description
         * Called by controllers when submitting a form - this ensures that all client validation is checked, 
         * server validation is cleared, that the correct events execute and status messages are displayed.
         * This returns true if the form is valid, otherwise false if form submission cannot continue.
         * 
         * @param {object} args An object containing arguments for form submission
         */
        submitForm: function (args) {

            var currentForm;

            if (!args) {
                throw "args cannot be null";
            }
            if (!args.scope) {
                throw "args.scope cannot be null";
            }
            if (!args.formCtrl) {
                //try to get the closest form controller
                currentForm = angularHelper.getRequiredCurrentForm(args.scope);
            }
            else {
                currentForm = args.formCtrl;
            }
            //if no statusPropertyName is set we'll default to formStatus.
            if (!args.statusPropertyName) {
                args.statusPropertyName = "formStatus";
            }
            //if no statusTimeout is set, we'll  default to 2500 ms
            if (!args.statusTimeout) {
                args.statusTimeout = 2500;
            }
            
            //the first thing any form must do is broadcast the formSubmitting event
            args.scope.$broadcast("formSubmitting", { scope: args.scope, action: args.action });

            //then check if the form is valid
            if (!args.skipValidation) {                
                if (currentForm.$invalid) {
                    return false;
                }
            }

            //reset the server validations
            serverValidationManager.reset();
            
            //check if a form status should be set on the scope
            if (args.statusMessage) {
                args.scope[args.statusPropertyName] = args.statusMessage;

                //clear the message after the timeout
                $timeout(function () {
                    args.scope[args.statusPropertyName] = undefined;
                }, args.statusTimeout);
            }

            return true;
        },
        
        /**
         * @ngdoc function
         * @name umbraco.services.formHelper#submitForm
         * @methodOf umbraco.services.formHelper
         * @function
         *
         * @description
         * Called by controllers when a form has been successfully submitted. the correct events execute 
         * and that the notifications are displayed if there are any.
         * 
         * @param {object} args An object containing arguments for form submission
         */
        resetForm: function (args) {
            if (!args) {
                throw "args cannot be null";
            }
            if (!args.scope) {
                throw "args.scope cannot be null";
            }
            
            //if no statusPropertyName is set we'll default to formStatus.
            if (!args.statusPropertyName) {
                args.statusPropertyName = "formStatus";
            }
            //clear the status
            args.scope[args.statusPropertyName] = null;

            if (angular.isArray(args.notifications)) {
                for (var i = 0; i < args.notifications.length; i++) {
                    notificationsService.showNotification(args.notifications[i]);
                }
            }

            args.scope.$broadcast("formSubmitted", { scope: args.scope });
        },
        
        /**
         * @ngdoc function
         * @name umbraco.services.formHelper#handleError
         * @methodOf umbraco.services.formHelper
         * @function
         *
         * @description
         * Needs to be called when a form submission fails, this will wire up all server validation errors in ModelState and
         * add the correct messages to the notifications. If a server error has occurred this will show a ysod.
         * 
         * @param {object} err The error object returned from the http promise
         */
        handleError: function (err) {            
            //When the status is a 400 status with a custom header: X-Status-Reason: Validation failed, we have validation errors.
            //Otherwise the error is probably due to invalid data (i.e. someone mucking around with the ids or something).
            //Or, some strange server error
            if (err.status === 400) {
                //now we need to look through all the validation errors
                if (err.data && (err.data.ModelState)) {

                    //wire up the server validation errs
                    this.handleServerValidation(err.data.ModelState);

                    //execute all server validation events and subscribers
                    serverValidationManager.executeAndClearAllSubscriptions();                    
                }
                else {
                    dialogService.ysodDialog(err);
                }
            }
            else {
                dialogService.ysodDialog(err);
            }
        },

        /**
         * @ngdoc function
         * @name umbraco.services.formHelper#handleServerValidation
         * @methodOf umbraco.services.formHelper
         * @function
         *
         * @description
         * This wires up all of the server validation model state so that valServer and valServerField directives work
         * 
         * @param {object} err The error object returned from the http promise
         */
        handleServerValidation: function (modelState) {
            for (var e in modelState) {

                //This is where things get interesting....
                // We need to support validation for all editor types such as both the content and content type editors.
                // The Content editor ModelState is quite specific with the way that Properties are validated especially considering
                // that each property is a User Developer property editor.
                // The way that Content Type Editor ModelState is created is simply based on the ASP.Net validation data-annotations 
                // system. 
                // So, to do this (since we need to support backwards compat), we need to hack a little bit. For Content Properties,
                // which are user defined, we know that they will exist with a prefixed ModelState of "_Properties.", so if we detect
                // this, then we know it's a Property.

                //the alias in model state can be in dot notation which indicates
                // * the first part is the content property alias
                // * the second part is the field to which the valiation msg is associated with
                //There will always be at least 2 parts for properties since all model errors for properties are prefixed with "Properties"
                //If it is not prefixed with "Properties" that means the error is for a field of the object directly.

                var parts = e.split(".");

                //Check if this is for content properties - specific to content/media/member editors because those are special 
                // user defined properties with custom controls.
                if (parts.length > 1 && parts[0] === "_Properties") {

                    var propertyAlias = parts[1];

                    //if it contains 2 '.' then we will wire it up to a property's field
                    if (parts.length > 2) {
                        //add an error with a reference to the field for which the validation belongs too
                        serverValidationManager.addPropertyError(propertyAlias, parts[2], modelState[e][0]);
                    }
                    else {
                        //add a generic error for the property, no reference to a specific field
                        serverValidationManager.addPropertyError(propertyAlias, "", modelState[e][0]);
                    }

                }
                else {

                    //Everthing else is just a 'Field'... the field name could contain any level of 'parts' though, for example:
                    // Groups[0].Properties[2].Alias
                    serverValidationManager.addFieldError(e, modelState[e][0]);
                }

                //add to notifications
                notificationsService.error("Validation", modelState[e][0]);

            }
        }
    };
}
angular.module('umbraco.services').factory('formHelper', formHelper);
angular.module('umbraco.services')
	.factory('gridService', function ($http, $q){

	    var configPath = Umbraco.Sys.ServerVariables.umbracoUrls.gridConfig;
        var service = {
			getGridEditors: function () {
				return $http.get(configPath);
			}
		};

		return service;

	});

angular.module('umbraco.services')
	.factory('helpService', function ($http, $q){
		var helpTopics = {};

		var defaultUrl = "http://our.umbraco.org/rss/help";
		var tvUrl = "http://umbraco.tv/feeds/help";

		function getCachedHelp(url){
			if(helpTopics[url]){
				return helpTopics[cacheKey];
			}else{
				return null;
			}
		}

		function setCachedHelp(url, data){
			helpTopics[url] = data;
		}

		function fetchUrl(url){
			var deferred = $q.defer();
			var found = getCachedHelp(url);

			if(found){
				deferred.resolve(found);
			}else{

				var proxyUrl = "dashboard/feedproxy.aspx?url=" + url; 
				$http.get(proxyUrl).then(function(data){
					var feed = $(data.data);
					var topics = [];

					$('item', feed).each(function (i, item) {
						var topic = {};
						topic.thumbnail = $(item).find('thumbnail').attr('url');
						topic.title = $("title", item).text();
						topic.link = $("guid", item).text();
						topic.description = $("description", item).text();
						topics.push(topic);
					});

					setCachedHelp(topics);
					deferred.resolve(topics);
				});
			}

			return deferred.promise;
		}



		var service = {
			findHelp: function (args) {
				var url = service.getUrl(defaultUrl, args);
				return fetchUrl(url);
			},

			findVideos: function (args) {
				var url = service.getUrl(tvUrl, args);
				return fetchUrl(url);
			},

			getUrl: function(url, args){
				return url + "?" + $.param(args);
			}
		};

		return service;

	});
/**
 * @ngdoc service
 * @name umbraco.services.historyService
 *
 * @requires $rootScope 
 * @requires $timeout
 * @requires angularHelper
 *	
 * @description
 * Service to handle the main application navigation history. Responsible for keeping track
 * of where a user navigates to, stores an icon, url and name in a collection, to make it easy
 * for the user to go back to a previous editor / action
 *
 * **Note:** only works with new angular-based editors, not legacy ones
 *
 * ##usage
 * To use, simply inject the historyService into any controller that needs it, and make
 * sure the umbraco.services module is accesible - which it should be by default.
 *
 * <pre>
 *      angular.module("umbraco").controller("my.controller". function(historyService){
 *         historyService.add({
 *								icon: "icon-class",
 *								name: "Editing 'articles',
 *								link: "/content/edit/1234"}
 *							);
 *      }); 
 * </pre> 
 */
angular.module('umbraco.services')
.factory('historyService', function ($rootScope, $timeout, angularHelper, eventsService) {

	var nArray = [];

	function add(item) {

	    if (!item) {
	        return null;
	    }

	    var listWithoutThisItem = _.reject(nArray, function(i) {
	        return i.link === item.link;
	    });

        //put it at the top and reassign
	    listWithoutThisItem.splice(0, 0, item);
	    nArray = listWithoutThisItem;
	    return nArray[0];

	}

	return {
		/**
		 * @ngdoc method
		 * @name umbraco.services.historyService#add
		 * @methodOf umbraco.services.historyService
		 *
		 * @description
		 * Adds a given history item to the users history collection.
		 *
		 * @param {Object} item the history item
		 * @param {String} item.icon icon css class for the list, ex: "icon-image", "icon-doc"
		 * @param {String} item.link route to the editor, ex: "/content/edit/1234"
		 * @param {String} item.name friendly name for the history listing
		 * @returns {Object} history item object
		 */
		add: function (item) {
			var icon = item.icon || "icon-file";
			angularHelper.safeApply($rootScope, function () {
			    var result = add({ name: item.name, icon: icon, link: item.link, time: new Date() });
			    eventsService.emit("historyService.add", {added: result, all: nArray});
			    return result;
			});
		},
		/**
		 * @ngdoc method
		 * @name umbraco.services.historyService#remove
		 * @methodOf umbraco.services.historyService
		 *
		 * @description
		 * Removes a history item from the users history collection, given an index to remove from.
		 *
		 * @param {Int} index index to remove item from
		 */
		remove: function (index) {
			angularHelper.safeApply($rootScope, function() {
			    var result = nArray.splice(index, 1);
			    eventsService.emit("historyService.remove", { removed: result, all: nArray });
			});
		},

		/**
		 * @ngdoc method
		 * @name umbraco.services.historyService#removeAll
		 * @methodOf umbraco.services.historyService
		 *
		 * @description
		 * Removes all history items from the users history collection
		 */
		removeAll: function () {
			angularHelper.safeApply($rootScope, function() {
			    nArray = [];
			    eventsService.emit("historyService.removeAll");
			});
		},

		/**
		 * @ngdoc method
		 * @name umbraco.services.historyService#getCurrent
		 * @methodOf umbraco.services.historyService
		 *
		 * @description
		 * Method to return the current history collection.
		 *
		 */
		getCurrent: function(){
			return nArray;
		}
	};
});
/**
* @ngdoc service
* @name umbraco.services.iconHelper
* @description A helper service for dealing with icons, mostly dealing with legacy tree icons
**/
function iconHelper($q, $timeout) {

    var converter = [
        { oldIcon: ".sprNew", newIcon: "add" },
        { oldIcon: ".sprDelete", newIcon: "remove" },
        { oldIcon: ".sprMove", newIcon: "enter" },
        { oldIcon: ".sprCopy", newIcon: "documents" },
        { oldIcon: ".sprSort", newIcon: "navigation-vertical" },
        { oldIcon: ".sprPublish", newIcon: "globe" },
        { oldIcon: ".sprRollback", newIcon: "undo" },
        { oldIcon: ".sprProtect", newIcon: "lock" },
        { oldIcon: ".sprAudit", newIcon: "time" },
        { oldIcon: ".sprNotify", newIcon: "envelope" },
        { oldIcon: ".sprDomain", newIcon: "home" },
        { oldIcon: ".sprPermission", newIcon: "lock" },
        { oldIcon: ".sprRefresh", newIcon: "refresh" },
        { oldIcon: ".sprBinEmpty", newIcon: "trash" },
        { oldIcon: ".sprExportDocumentType", newIcon: "download-alt" },
        { oldIcon: ".sprImportDocumentType", newIcon: "page-up" },
        { oldIcon: ".sprLiveEdit", newIcon: "edit" },
        { oldIcon: ".sprCreateFolder", newIcon: "add" },
        { oldIcon: ".sprPackage2", newIcon: "box" },
        { oldIcon: ".sprLogout", newIcon: "logout" },
        { oldIcon: ".sprSave", newIcon: "save" },
        { oldIcon: ".sprSendToTranslate", newIcon: "envelope-alt" },
        { oldIcon: ".sprToPublish", newIcon: "mail-forward" },
        { oldIcon: ".sprTranslate", newIcon: "comments" },
        { oldIcon: ".sprUpdate", newIcon: "save" },
        
        { oldIcon: ".sprTreeSettingDomain", newIcon: "icon-home" },
        { oldIcon: ".sprTreeDoc", newIcon: "icon-document" },
        { oldIcon: ".sprTreeDoc2", newIcon: "icon-diploma-alt" },
        { oldIcon: ".sprTreeDoc3", newIcon: "icon-notepad" },
        { oldIcon: ".sprTreeDoc4", newIcon: "icon-newspaper-alt" },
        { oldIcon: ".sprTreeDoc5", newIcon: "icon-notepad-alt" },

        { oldIcon: ".sprTreeDocPic", newIcon: "icon-picture" },        
        { oldIcon: ".sprTreeFolder", newIcon: "icon-folder" },
        { oldIcon: ".sprTreeFolder_o", newIcon: "icon-folder" },
        { oldIcon: ".sprTreeMediaFile", newIcon: "icon-music" },
        { oldIcon: ".sprTreeMediaMovie", newIcon: "icon-movie" },
        { oldIcon: ".sprTreeMediaPhoto", newIcon: "icon-picture" },
        
        { oldIcon: ".sprTreeMember", newIcon: "icon-user" },
        { oldIcon: ".sprTreeMemberGroup", newIcon: "icon-users" },
        { oldIcon: ".sprTreeMemberType", newIcon: "icon-users" },
        
        { oldIcon: ".sprTreeNewsletter", newIcon: "icon-file-text-alt" },
        { oldIcon: ".sprTreePackage", newIcon: "icon-box" },
        { oldIcon: ".sprTreeRepository", newIcon: "icon-server-alt" },
        
        { oldIcon: ".sprTreeSettingDataType", newIcon: "icon-autofill" },

        //TODO:
        /*
        { oldIcon: ".sprTreeSettingAgent", newIcon: "" },
        { oldIcon: ".sprTreeSettingCss", newIcon: "" },
        { oldIcon: ".sprTreeSettingCssItem", newIcon: "" },
        
        { oldIcon: ".sprTreeSettingDataTypeChild", newIcon: "" },
        { oldIcon: ".sprTreeSettingDomain", newIcon: "" },
        { oldIcon: ".sprTreeSettingLanguage", newIcon: "" },
        { oldIcon: ".sprTreeSettingScript", newIcon: "" },
        { oldIcon: ".sprTreeSettingTemplate", newIcon: "" },
        { oldIcon: ".sprTreeSettingXml", newIcon: "" },
        { oldIcon: ".sprTreeStatistik", newIcon: "" },
        { oldIcon: ".sprTreeUser", newIcon: "" },
        { oldIcon: ".sprTreeUserGroup", newIcon: "" },
        { oldIcon: ".sprTreeUserType", newIcon: "" },
        */

        { oldIcon: "folder.png", newIcon: "icon-folder" },
        { oldIcon: "mediaphoto.gif", newIcon: "icon-picture" },
        { oldIcon: "mediafile.gif", newIcon: "icon-document" },

        { oldIcon: ".sprTreeDeveloperCacheItem", newIcon: "icon-box" },
        { oldIcon: ".sprTreeDeveloperCacheTypes", newIcon: "icon-box" },
        { oldIcon: ".sprTreeDeveloperMacro", newIcon: "icon-cogs" },
        { oldIcon: ".sprTreeDeveloperRegistry", newIcon: "icon-windows" },
        { oldIcon: ".sprTreeDeveloperPython", newIcon: "icon-linux" }
    ];

    var imageConverter = [
            {oldImage: "contour.png", newIcon: "icon-umb-contour"}
            ];

    var collectedIcons;
            
    return {
        
        /** Used by the create dialogs for content/media types to format the data so that the thumbnails are styled properly */
        formatContentTypeThumbnails: function (contentTypes) {
            for (var i = 0; i < contentTypes.length; i++) {

                if (contentTypes[i].thumbnailIsClass === undefined || contentTypes[i].thumbnailIsClass) {
                    contentTypes[i].cssClass = this.convertFromLegacyIcon(contentTypes[i].thumbnail);
                }else {
                    contentTypes[i].style = "background-image: url('" + contentTypes[i].thumbnailFilePath + "');height:36px; background-position:4px 0px; background-repeat: no-repeat;background-size: 35px 35px;";
                    //we need an 'icon-' class in there for certain styles to work so if it is image based we'll add this
                    contentTypes[i].cssClass = "custom-file";
                }
            }
            return contentTypes;
        },
        formatContentTypeIcons: function (contentTypes) {
            for (var i = 0; i < contentTypes.length; i++) {
                if (!contentTypes[i].icon) {
                    //just to be safe (e.g. when focus was on close link and hitting save)
                    contentTypes[i].icon = "icon-document"; // default icon
                } else {
                    contentTypes[i].icon = this.convertFromLegacyIcon(contentTypes[i].icon);
                }

                //couldnt find replacement
                if(contentTypes[i].icon.indexOf(".") > 0){
                     contentTypes[i].icon = "icon-document-dashed-line";
                }
            }
            return contentTypes;
        },
        /** If the icon is file based (i.e. it has a file path) */
        isFileBasedIcon: function (icon) {
            //if it doesn't start with a '.' but contains one then we'll assume it's file based
            if (icon.startsWith('..') || (!icon.startsWith('.') && icon.indexOf('.') > 1)) {
                return true;
            }
            return false;
        },
        /** If the icon is legacy */
        isLegacyIcon: function (icon) {
            if(!icon) {
                return false;
            }

            if(icon.startsWith('..')){
                return false;
            }

            if (icon.startsWith('.')) {
                return true;
            }
            return false;
        },
        /** If the tree node has a legacy icon */
        isLegacyTreeNodeIcon: function(treeNode){
            if (treeNode.iconIsClass) {
                return this.isLegacyIcon(treeNode.icon);
            }
            return false;
        },

        /** Return a list of icons, optionally filter them */
        /** It fetches them directly from the active stylesheets in the browser */
        getIcons: function(){
            var deferred = $q.defer();
            $timeout(function(){
                if(collectedIcons){
                    deferred.resolve(collectedIcons);
                }else{
                    collectedIcons = [];
                    var c = ".icon-";

                    for (var i = document.styleSheets.length - 1; i >= 0; i--) {
                        var classes = document.styleSheets[i].rules || document.styleSheets[i].cssRules;
                        
                        if (classes !== null) {
                            for(var x=0;x<classes.length;x++) {
                                var cur = classes[x];
                                if(cur.selectorText && cur.selectorText.indexOf(c) === 0) {
                                    var s = cur.selectorText.substring(1);
                                    var hasSpace = s.indexOf(" ");
                                    if(hasSpace>0){
                                        s = s.substring(0, hasSpace);
                                    }
                                    var hasPseudo = s.indexOf(":");
                                    if(hasPseudo>0){
                                        s = s.substring(0, hasPseudo);
                                    }

                                    if(collectedIcons.indexOf(s) < 0){
                                        collectedIcons.push(s);
                                    }
                                }
                            }
                        }
                    }
                    deferred.resolve(collectedIcons);
                }
            }, 100);
            
            return deferred.promise;
        },

        /** Converts the icon from legacy to a new one if an old one is detected */
        convertFromLegacyIcon: function (icon) {
            if (this.isLegacyIcon(icon)) {
                //its legacy so convert it if we can
                var found = _.find(converter, function (item) {
                    return item.oldIcon.toLowerCase() === icon.toLowerCase();
                });
                return (found ? found.newIcon : icon);
            }
            return icon;
        },

        convertFromLegacyImage: function (icon) {
                var found = _.find(imageConverter, function (item) {
                    return item.oldImage.toLowerCase() === icon.toLowerCase();
                });
                return (found ? found.newIcon : undefined);
        },

        /** If we detect that the tree node has legacy icons that can be converted, this will convert them */
        convertFromLegacyTreeNodeIcon: function (treeNode) {
            if (this.isLegacyTreeNodeIcon(treeNode)) {
                return this.convertFromLegacyIcon(treeNode.icon);
            }
            return treeNode.icon;
        }
    };
}
angular.module('umbraco.services').factory('iconHelper', iconHelper);
/**
* @ngdoc service
* @name umbraco.services.imageHelper
* @deprecated
**/
function imageHelper(umbRequestHelper, mediaHelper) {
    return {
        /**
         * @ngdoc function
         * @name umbraco.services.imageHelper#getImagePropertyValue
         * @methodOf umbraco.services.imageHelper
         * @function    
         *
         * @deprecated
         */
        getImagePropertyValue: function (options) {
            return mediaHelper.getImagePropertyValue(options);
        },
        /**
         * @ngdoc function
         * @name umbraco.services.imageHelper#getThumbnail
         * @methodOf umbraco.services.imageHelper
         * @function    
         *
         * @deprecated
         */
        getThumbnail: function (options) {
            return mediaHelper.getThumbnail(options);
        },

        /**
         * @ngdoc function
         * @name umbraco.services.imageHelper#scaleToMaxSize
         * @methodOf umbraco.services.imageHelper
         * @function    
         *
         * @deprecated
         */
        scaleToMaxSize: function (maxSize, width, height) {
            return mediaHelper.scaleToMaxSize(maxSize, width, height);
        },

        /**
         * @ngdoc function
         * @name umbraco.services.imageHelper#getThumbnailFromPath
         * @methodOf umbraco.services.imageHelper
         * @function    
         *
         * @deprecated
         */
        getThumbnailFromPath: function (imagePath) {
            return mediaHelper.getThumbnailFromPath(imagePath);
        },

        /**
         * @ngdoc function
         * @name umbraco.services.imageHelper#detectIfImageByExtension
         * @methodOf umbraco.services.imageHelper
         * @function    
         *
         * @deprecated
         */
        detectIfImageByExtension: function (imagePath) {
            return mediaHelper.detectIfImageByExtension(imagePath);
        }
    };
}
angular.module('umbraco.services').factory('imageHelper', imageHelper);
// This service was based on OpenJS library available in BSD License
// http://www.openjs.com/scripts/events/keyboard_shortcuts/index.php

function keyboardService($window, $timeout) {
    
    var keyboardManagerService = {};
    
    var defaultOpt = {
        'type':             'keydown',
        'propagate':        false,
        'inputDisabled':    false,
        'target':           $window.document,
        'keyCode':          false
    };

    // Work around for stupid Shift key bug created by using lowercase - as a result the shift+num combination was broken
    var shift_nums = {
        "`": "~",
        "1": "!",
        "2": "@",
        "3": "#",
        "4": "$",
        "5": "%",
        "6": "^",
        "7": "&",
        "8": "*",
        "9": "(",
        "0": ")",
        "-": "_",
        "=": "+",
        ";": ":",
        "'": "\"",
        ",": "<",
        ".": ">",
        "/": "?",
        "\\": "|"
    };

    // Special Keys - and their codes
    var special_keys = {
        'esc': 27,
        'escape': 27,
        'tab': 9,
        'space': 32,
        'return': 13,
        'enter': 13,
        'backspace': 8,

        'scrolllock': 145,
        'scroll_lock': 145,
        'scroll': 145,
        'capslock': 20,
        'caps_lock': 20,
        'caps': 20,
        'numlock': 144,
        'num_lock': 144,
        'num': 144,

        'pause': 19,
        'break': 19,

        'insert': 45,
        'home': 36,
        'delete': 46,
        'end': 35,

        'pageup': 33,
        'page_up': 33,
        'pu': 33,

        'pagedown': 34,
        'page_down': 34,
        'pd': 34,

        'left': 37,
        'up': 38,
        'right': 39,
        'down': 40,

        'f1': 112,
        'f2': 113,
        'f3': 114,
        'f4': 115,
        'f5': 116,
        'f6': 117,
        'f7': 118,
        'f8': 119,
        'f9': 120,
        'f10': 121,
        'f11': 122,
        'f12': 123
    };

    var isMac = navigator.platform.toUpperCase().indexOf('MAC')>=0;

    // The event handler for bound element events
    function eventHandler(e) {
        e = e || $window.event;

        var code, k;

        // Find out which key is pressed
        if (e.keyCode)
        {
            code = e.keyCode;
        }
        else if (e.which) {
            code = e.which;
        }

        var character = String.fromCharCode(code).toLowerCase();

        if (code === 188){character = ",";} // If the user presses , when the type is onkeydown
        if (code === 190){character = ".";} // If the user presses , when the type is onkeydown

        var propagate = true;

        //Now we need to determine which shortcut this event is for, we'll do this by iterating over each 
        //registered shortcut to find the match. We use Find here so that the loop exits as soon
        //as we've found the one we're looking for
        _.find(_.keys(keyboardManagerService.keyboardEvent), function(key) {

            var shortcutLabel = key;
            var shortcutVal = keyboardManagerService.keyboardEvent[key];

            // Key Pressed - counts the number of valid keypresses - if it is same as the number of keys, the shortcut function is invoked
            var kp = 0;

            // Some modifiers key
            var modifiers = {
                shift: {
                    wanted: false,
                    pressed: e.shiftKey ? true : false
                },
                ctrl: {
                    wanted: false,
                    pressed: e.ctrlKey ? true : false
                },
                alt: {
                    wanted: false,
                    pressed: e.altKey ? true : false
                },
                meta: { //Meta is Mac specific
                    wanted: false,
                    pressed: e.metaKey ? true : false
                }
            };

            var keys = shortcutLabel.split("+");
            var opt = shortcutVal.opt;
            var callback = shortcutVal.callback;

            // Foreach keys in label (split on +)
            var l = keys.length;
            for (var i = 0; i < l; i++) {

                var k = keys[i];
                switch (k) {
                    case 'ctrl':
                    case 'control':
                        kp++;
                        modifiers.ctrl.wanted = true;
                        break;
                    case 'shift':
                    case 'alt':
                    case 'meta':
                        kp++;
                        modifiers[k].wanted = true;
                        break;
                }

                if (k.length > 1) { // If it is a special key
                    if (special_keys[k] === code) {
                        kp++;
                    }
                }
                else if (opt['keyCode']) { // If a specific key is set into the config
                    if (opt['keyCode'] === code) {
                        kp++;
                    }
                }
                else { // The special keys did not match
                    if (character === k) {
                        kp++;
                    }
                    else {
                        if (shift_nums[character] && e.shiftKey) { // Stupid Shift key bug created by using lowercase
                            character = shift_nums[character];
                            if (character === k) {
                                kp++;
                            }
                        }
                    }
                }

            } //for end

            if (kp === keys.length &&
                modifiers.ctrl.pressed === modifiers.ctrl.wanted &&
                modifiers.shift.pressed === modifiers.shift.wanted &&
                modifiers.alt.pressed === modifiers.alt.wanted &&
                modifiers.meta.pressed === modifiers.meta.wanted) {

                //found the right callback!

                // Disable event handler when focus input and textarea
                if (opt['inputDisabled']) {
                    var elt;
                    if (e.target) {
                        elt = e.target;
                    } else if (e.srcElement) {
                        elt = e.srcElement;
                    }

                    if (elt.nodeType === 3) { elt = elt.parentNode; }
                    if (elt.tagName === 'INPUT' || elt.tagName === 'TEXTAREA') {
                        //This exits the Find loop
                        return true;
                    }
                }

                $timeout(function () {
                    callback(e);
                }, 1);

                if (!opt['propagate']) { // Stop the event
                    propagate = false;
                }

                //This exits the Find loop
                return true;
            }

            //we haven't found one so continue looking
            return false;

        });

        // Stop the event if required
        if (!propagate) {
            // e.cancelBubble is supported by IE - this will kill the bubbling process.
            e.cancelBubble = true;
            e.returnValue = false;

            // e.stopPropagation works in Firefox.
            if (e.stopPropagation) {
                e.stopPropagation();
                e.preventDefault();
            }
            return false;
        }
    }

    // Store all keyboard combination shortcuts
    keyboardManagerService.keyboardEvent = {};

    // Add a new keyboard combination shortcut
    keyboardManagerService.bind = function (label, callback, opt) {

        //replace ctrl key with meta key
        if(isMac && label !== "ctrl+space"){
            label = label.replace("ctrl","meta");
        }

        var elt;
        // Initialize opt object
        opt   = angular.extend({}, defaultOpt, opt);
        label = label.toLowerCase();
        elt   = opt.target;
        if(typeof opt.target === 'string'){
            elt = document.getElementById(opt.target);
        }
        
        //Ensure we aren't double binding to the same element + type otherwise we'll end up multi-binding
        // and raising events for now reason. So here we'll check if the event is already registered for the element
        var boundValues = _.values(keyboardManagerService.keyboardEvent);
        var found = _.find(boundValues, function (i) {
            return i.target === elt && i.event === opt['type'];
        });

        // Store shortcut
        keyboardManagerService.keyboardEvent[label] = {
            'callback': callback,
            'target':   elt,
            'opt':      opt
        };

        if (!found) {
            //Attach the function with the event
            if (elt.addEventListener) {
                elt.addEventListener(opt['type'], eventHandler, false);
            } else if (elt.attachEvent) {
                elt.attachEvent('on' + opt['type'], eventHandler);
            } else {
                elt['on' + opt['type']] = eventHandler;
            }
        }
        
    };
    // Remove the shortcut - just specify the shortcut and I will remove the binding
    keyboardManagerService.unbind = function (label) {
        label = label.toLowerCase();
        var binding = keyboardManagerService.keyboardEvent[label];
        delete(keyboardManagerService.keyboardEvent[label]);

        if(!binding){return;}

        var type	= binding['event'],
		elt			= binding['target'],
		callback	= binding['callback'];

        if(elt.detachEvent){
            elt.detachEvent('on' + type, callback);
        }else if(elt.removeEventListener){
            elt.removeEventListener(type, callback, false);
        }else{
            elt['on'+type] = false;
        }
    };
    //

    return keyboardManagerService;
}angular.module('umbraco.services').factory('keyboardService', ['$window', '$timeout', keyboardService]);
/**
 @ngdoc service
 * @name umbraco.services.listViewHelper
 *
 *
 * @description
 * Service for performing operations against items in the list view UI. Used by the built-in internal listviews
 * as well as custom listview.
 *
 * A custom listview is always used inside a wrapper listview, so there are a number of inherited values on its
 * scope by default:
 *
 * **$scope.selection**: Array containing all items currently selected in the listview
 *
 * **$scope.items**: Array containing all items currently displayed in the listview
 *
 * **$scope.folders**: Array containing all folders in the current listview (only for media)
 *
 * **$scope.options**: configuration object containing information such as pagesize, permissions, order direction etc.
 *
 * **$scope.model.config.layouts**: array of available layouts to apply to the listview (grid, list or custom layout)
 *
 * ##Usage##
 * To use, inject listViewHelper into custom listview controller, listviewhelper expects you
 * to pass in the full collection of items in the listview in several of its methods
 * this collection is inherited from the parent controller and is available on $scope.selection
 *
 * <pre>
 *      angular.module("umbraco").controller("my.listVieweditor". function($scope, listViewHelper){
 *
 *          //current items in the listview
 *          var items = $scope.items;
 *
 *          //current selection
 *          var selection = $scope.selection;
 *
 *          //deselect an item , $scope.selection is inherited, item is picked from inherited $scope.items
 *          listViewHelper.deselectItem(item, $scope.selection);
 *
 *          //test if all items are selected, $scope.items + $scope.selection are inherited
 *          listViewhelper.isSelectedAll($scope.items, $scope.selection);
 *      });
 * </pre>
 */
(function () {
    'use strict';

    function listViewHelper(localStorageService) {

        var firstSelectedIndex = 0;
        var localStorageKey = "umblistViewLayout";

        /**
        * @ngdoc method
        * @name umbraco.services.listViewHelper#getLayout
        * @methodOf umbraco.services.listViewHelper
        *
        * @description
        * Method for internal use, based on the collection of layouts passed, the method selects either
        * any previous layout from local storage, or picks the first allowed layout
        *
        * @param {Number} nodeId The id of the current node displayed in the content editor
        * @param {Array} availableLayouts Array of all allowed layouts, available from $scope.model.config.layouts
        */

        function getLayout(nodeId, availableLayouts) {

            var storedLayouts = [];

            if (localStorageService.get(localStorageKey)) {
                storedLayouts = localStorageService.get(localStorageKey);
            }

            if (storedLayouts && storedLayouts.length > 0) {
                for (var i = 0; storedLayouts.length > i; i++) {
                    var layout = storedLayouts[i];
                    if (layout.nodeId === nodeId) {
                        return setLayout(nodeId, layout, availableLayouts);
                    }
                }

            }

            return getFirstAllowedLayout(availableLayouts);

        }

        /**
        * @ngdoc method
        * @name umbraco.services.listViewHelper#setLayout
        * @methodOf umbraco.services.listViewHelper
        *
        * @description
        * Changes the current layout used by the listview to the layout passed in. Stores selection in localstorage
        *
        * @param {Number} nodeID Id of the current node displayed in the content editor
        * @param {Object} selectedLayout Layout selected as the layout to set as the current layout
        * @param {Array} availableLayouts Array of all allowed layouts, available from $scope.model.config.layouts
        */

        function setLayout(nodeId, selectedLayout, availableLayouts) {

            var activeLayout = {};
            var layoutFound = false;

            for (var i = 0; availableLayouts.length > i; i++) {
                var layout = availableLayouts[i];
                if (layout.path === selectedLayout.path) {
                    activeLayout = layout;
                    layout.active = true;
                    layoutFound = true;
                } else {
                    layout.active = false;
                }
            }

            if (!layoutFound) {
                activeLayout = getFirstAllowedLayout(availableLayouts);
            }

            saveLayoutInLocalStorage(nodeId, activeLayout);

            return activeLayout;

        }

        /**
        * @ngdoc method
        * @name umbraco.services.listViewHelper#saveLayoutInLocalStorage
        * @methodOf umbraco.services.listViewHelper
        *
        * @description
        * Stores a given layout as the current default selection in local storage
        *
        * @param {Number} nodeId Id of the current node displayed in the content editor
        * @param {Object} selectedLayout Layout selected as the layout to set as the current layout
        */

        function saveLayoutInLocalStorage(nodeId, selectedLayout) {
            var layoutFound = false;
            var storedLayouts = [];

            if (localStorageService.get(localStorageKey)) {
                storedLayouts = localStorageService.get(localStorageKey);
            }

            if (storedLayouts.length > 0) {
                for (var i = 0; storedLayouts.length > i; i++) {
                    var layout = storedLayouts[i];
                    if (layout.nodeId === nodeId) {
                        layout.path = selectedLayout.path;
                        layoutFound = true;
                    }
                }
            }

            if (!layoutFound) {
                var storageObject = {
                    "nodeId": nodeId,
                    "path": selectedLayout.path
                };
                storedLayouts.push(storageObject);
            }

            localStorageService.set(localStorageKey, storedLayouts);

        }

        /**
        * @ngdoc method
        * @name umbraco.services.listViewHelper#getFirstAllowedLayout
        * @methodOf umbraco.services.listViewHelper
        *
        * @description
        * Returns currently selected layout, or alternatively the first layout in the available layouts collection
        *
        * @param {Array} layouts Array of all allowed layouts, available from $scope.model.config.layouts
        */

        function getFirstAllowedLayout(layouts) {

            var firstAllowedLayout = {};

            for (var i = 0; layouts.length > i; i++) {
                var layout = layouts[i];
                if (layout.selected === true) {
                    firstAllowedLayout = layout;
                    break;
                }
            }

            return firstAllowedLayout;
        }

        /**
        * @ngdoc method
        * @name umbraco.services.listViewHelper#selectHandler
        * @methodOf umbraco.services.listViewHelper
        *
        * @description
        * Helper method for working with item selection via a checkbox, internally it uses selectItem and deselectItem.
        * Working with this method, requires its triggered via a checkbox which can then pass in its triggered $event
        * When the checkbox is clicked, this method will toggle selection of the associated item so it matches the state of the checkbox
        *
        * @param {Object} selectedItem Item being selected or deselected by the checkbox
        * @param {Number} selectedIndex Index of item being selected/deselected, usually passed as $index
        * @param {Array} items All items in the current listview, available as $scope.items
        * @param {Array} selection All selected items in the current listview, available as $scope.selection
        * @param {Event} $event Event triggered by the checkbox being checked to select / deselect an item
        */

        function selectHandler(selectedItem, selectedIndex, items, selection, $event) {

            var start = 0;
            var end = 0;
            var item = null;

            if ($event.shiftKey === true) {

                if (selectedIndex > firstSelectedIndex) {

                    start = firstSelectedIndex;
                    end = selectedIndex;

                    for (; end >= start; start++) {
                        item = items[start];
                        selectItem(item, selection);
                    }

                } else {

                    start = firstSelectedIndex;
                    end = selectedIndex;

                    for (; end <= start; start--) {
                        item = items[start];
                        selectItem(item, selection);
                    }

                }

            } else {

                if (selectedItem.selected) {
                    deselectItem(selectedItem, selection);
                } else {
                    selectItem(selectedItem, selection);
                }

                firstSelectedIndex = selectedIndex;

            }

        }

        /**
        * @ngdoc method
        * @name umbraco.services.listViewHelper#selectItem
        * @methodOf umbraco.services.listViewHelper
        *
        * @description
        * Selects a given item to the listview selection array, requires you pass in the inherited $scope.selection collection
        *
        * @param {Object} item Item to select
        * @param {Array} selection Listview selection, available as $scope.selection
        */

        function selectItem(item, selection) {
            var isSelected = false;
            for (var i = 0; selection.length > i; i++) {
                var selectedItem = selection[i];
                if (item.id === selectedItem.id) {
                    isSelected = true;
                }
            }
            if (!isSelected) {
                selection.push({ id: item.id });
                item.selected = true;
            }
        }

        /**
        * @ngdoc method
        * @name umbraco.services.listViewHelper#deselectItem
        * @methodOf umbraco.services.listViewHelper
        *
        * @description
        * Deselects a given item from the listviews selection array, requires you pass in the inherited $scope.selection collection
        *
        * @param {Object} item Item to deselect
        * @param {Array} selection Listview selection, available as $scope.selection
        */

        function deselectItem(item, selection) {
            for (var i = 0; selection.length > i; i++) {
                var selectedItem = selection[i];
                if (item.id === selectedItem.id) {
                    selection.splice(i, 1);
                    item.selected = false;
                }
            }
        }

        /**
        * @ngdoc method
        * @name umbraco.services.listViewHelper#clearSelection
        * @methodOf umbraco.services.listViewHelper
        *
        * @description
        * Removes a given number of items and folders from the listviews selection array
        * Folders can only be passed in if the listview is used in the media section which has a concept of folders.
        *
        * @param {Array} items Items to remove, can be null
        * @param {Array} folders Folders to remove, can be null
        * @param {Array} selection Listview selection, available as $scope.selection
        */

        function clearSelection(items, folders, selection) {

            var i = 0;

            selection.length = 0;

            if (angular.isArray(items)) {
                for (i = 0; items.length > i; i++) {
                    var item = items[i];
                    item.selected = false;
                }
            }

         if(angular.isArray(folders)) {
                for (i = 0; folders.length > i; i++) {
                    var folder = folders[i];
                    folder.selected = false;
                }
            }
        }

        /**
        * @ngdoc method
        * @name umbraco.services.listViewHelper#selectAllItems
        * @methodOf umbraco.services.listViewHelper
        *
        * @description
        * Helper method for toggling the select state on all items in the active listview
        * Can only be used from a checkbox as a checkbox $event is required to pass in.
        *
        * @param {Array} items Items to toggle selection on, should be $scope.items
        * @param {Array} selection Listview selection, available as $scope.selection
        * @param {$event} $event Event passed from the checkbox being toggled
        */

        function selectAllItems(items, selection, $event) {

            var checkbox = $event.target;
            var clearSelection = false;

            if (!angular.isArray(items)) {
                return;
            }

            selection.length = 0;

            for (var i = 0; i < items.length; i++) {

                var item = items[i];

                if (checkbox.checked) {
                    selection.push({ id: item.id });
                } else {
                    clearSelection = true;
                }

                item.selected = checkbox.checked;

            }

            if (clearSelection) {
                selection.length = 0;
            }

        }

        /**
        * @ngdoc method
        * @name umbraco.services.listViewHelper#isSelectedAll
        * @methodOf umbraco.services.listViewHelper
        *
        * @description
        * Method to determine if all items on the current page in the list has been selected
        * Given the current items in the view, and the current selection, it will return true/false
        *
        * @param {Array} items Items to test if all are selected, should be $scope.items
        * @param {Array} selection Listview selection, available as $scope.selection
        * @returns {Boolean} boolean indicate if all items in the listview have been selected
        */

        function isSelectedAll(items, selection) {

            var numberOfSelectedItem = 0;

            for (var itemIndex = 0; items.length > itemIndex; itemIndex++) {
                var item = items[itemIndex];

                for (var selectedIndex = 0; selection.length > selectedIndex; selectedIndex++) {
                    var selectedItem = selection[selectedIndex];

                    if (item.id === selectedItem.id) {
                        numberOfSelectedItem++;
                    }
                }

            }

            if (numberOfSelectedItem === items.length) {
                return true;
            }

        }

        /**
        * @ngdoc method
        * @name umbraco.services.listViewHelper#setSortingDirection
        * @methodOf umbraco.services.listViewHelper
        *
        * @description
        * *Internal* method for changing sort order icon
        * @param {String} col Column alias to order after
        * @param {String} direction Order direction `asc` or `desc`
        * @param {Object} options object passed from the parent listview available as $scope.options
        */

        function setSortingDirection(col, direction, options) {
            return options.orderBy.toUpperCase() === col.toUpperCase() && options.orderDirection === direction;
        }

        /**
        * @ngdoc method
        * @name umbraco.services.listViewHelper#setSorting
        * @methodOf umbraco.services.listViewHelper
        *
        * @description
        * Method for setting the field on which the listview will order its items after.
        *
        * @param {String} field Field alias to order after
        * @param {Boolean} allow Determines if the user is allowed to set this field, normally true
        * @param {Object} options Options object passed from the parent listview available as $scope.options
        */

        function setSorting(field, allow, options) {
            if (allow) {
                if (options.orderBy === field && options.orderDirection === 'asc') {
                    options.orderDirection = "desc";
                } else {
                    options.orderDirection = "asc";
                }
                options.orderBy = field;
            }
        }

        //This takes in a dictionary of Ids with Permissions and determines
        // the intersect of all permissions to return an object representing the
        // listview button permissions
        function getButtonPermissions(unmergedPermissions, currentIdsWithPermissions) {

            if (currentIdsWithPermissions == null) {
                currentIdsWithPermissions = {};
            }

            //merge the newly retrieved permissions to the main dictionary
            _.each(unmergedPermissions, function (value, key, list) {
                currentIdsWithPermissions[key] = value;
            });

            //get the intersect permissions
            var arr = [];
            _.each(currentIdsWithPermissions, function (value, key, list) {
                arr.push(value);
            });

            //we need to use 'apply' to call intersection with an array of arrays,
            //see: http://stackoverflow.com/a/16229480/694494
            var intersectPermissions = _.intersection.apply(_, arr);

            return {
                canCopy: _.contains(intersectPermissions, 'O'), //Magic Char = O
                canCreate: _.contains(intersectPermissions, 'C'), //Magic Char = C
                canDelete: _.contains(intersectPermissions, 'D'), //Magic Char = D
                canMove: _.contains(intersectPermissions, 'M'), //Magic Char = M
                canPublish: _.contains(intersectPermissions, 'U'), //Magic Char = U
                canUnpublish: _.contains(intersectPermissions, 'U'), //Magic Char = Z (however UI says it can't be set, so if we can publish 'U' we can unpublish)
            };
        }

        var service = {

          getLayout: getLayout,
          getFirstAllowedLayout: getFirstAllowedLayout,
          setLayout: setLayout,
          saveLayoutInLocalStorage: saveLayoutInLocalStorage,
          selectHandler: selectHandler,
          selectItem: selectItem,
          deselectItem: deselectItem,
          clearSelection: clearSelection,
          selectAllItems: selectAllItems,
          isSelectedAll: isSelectedAll,
          setSortingDirection: setSortingDirection,
          setSorting: setSorting,
          getButtonPermissions: getButtonPermissions

        };

        return service;

    }


    angular.module('umbraco.services').factory('listViewHelper', listViewHelper);


})();

/**
 @ngdoc service
 * @name umbraco.services.listViewPrevalueHelper
 *
 *
 * @description
 * Service for accessing the prevalues of a list view being edited in the inline list view editor in the doctype editor
 */
(function () {
    'use strict';

    function listViewPrevalueHelper() {

        var prevalues = [];

        /**
        * @ngdoc method
        * @name umbraco.services.listViewPrevalueHelper#getPrevalues
        * @methodOf umbraco.services.listViewPrevalueHelper
        *
        * @description
        * Set the collection of prevalues
        */

        function getPrevalues() {
            return prevalues;
        }

        /**
        * @ngdoc method
        * @name umbraco.services.listViewPrevalueHelper#setPrevalues
        * @methodOf umbraco.services.listViewPrevalueHelper
        *
        * @description
        * Changes the current layout used by the listview to the layout passed in. Stores selection in localstorage
        *
        * @param {Array} values Array of prevalues
        */

        function setPrevalues(values) {
            prevalues = values;
        }

        

        var service = {

            getPrevalues: getPrevalues,
            setPrevalues: setPrevalues

        };

        return service;

    }


    angular.module('umbraco.services').factory('listViewPrevalueHelper', listViewPrevalueHelper);


})();

/**
 * @ngdoc service
 * @name umbraco.services.localizationService
 *
 * @requires $http
 * @requires $q
 * @requires $window
 * @requires $filter
 *
 * @description
 * Application-wide service for handling localization
 *
 * ##usage
 * To use, simply inject the localizationService into any controller that needs it, and make
 * sure the umbraco.services module is accesible - which it should be by default.
 *
 * <pre>
 *    localizationService.localize("area_key").then(function(value){
 *        element.html(value);
 *    });
 * </pre>
 */

angular.module('umbraco.services')
.factory('localizationService', function ($http, $q, eventsService, $window, $filter, userService) {

    //TODO: This should be injected as server vars
    var url = "LocalizedText";
    var resourceFileLoadStatus = "none";
    var resourceLoadingPromise = [];

    function _lookup(value, tokens, dictionary) {

        //strip the key identifier if its there
        if (value && value[0] === "@") {
            value = value.substring(1);
        }

        //if no area specified, add general_
        if (value && value.indexOf("_") < 0) {
            value = "general_" + value;
        }

        var entry = dictionary[value];
        if (entry) {
            if (tokens) {
                for (var i = 0; i < tokens.length; i++) {
                    entry = entry.replace("%" + i + "%", tokens[i]);
                }
            }
            return entry;
        }
        return "[" + value + "]";
    }

    var service = {
        // array to hold the localized resource string entries
        dictionary: [],

        // loads the language resource file from the server
        initLocalizedResources: function () {
            var deferred = $q.defer();

            if (resourceFileLoadStatus === "loaded") {
                deferred.resolve(service.dictionary);
                return deferred.promise;
            }

            //if the resource is already loading, we don't want to force it to load another one in tandem, we'd rather
            // wait for that initial http promise to finish and then return this one with the dictionary loaded
            if (resourceFileLoadStatus === "loading") {
                //add to the list of promises waiting
                resourceLoadingPromise.push(deferred);

                //exit now it's already loading
                return deferred.promise;
            }

            resourceFileLoadStatus = "loading";

            // build the url to retrieve the localized resource file
            $http({ method: "GET", url: url, cache: false })
                .then(function (response) {
                    resourceFileLoadStatus = "loaded";
                    service.dictionary = response.data;

                    eventsService.emit("localizationService.updated", response.data);

                    deferred.resolve(response.data);
                    //ensure all other queued promises are resolved
                    for (var p in resourceLoadingPromise) {
                        resourceLoadingPromise[p].resolve(response.data);
                    }
                }, function (err) {
                    deferred.reject("Something broke");
                    //ensure all other queued promises are resolved
                    for (var p in resourceLoadingPromise) {
                        resourceLoadingPromise[p].reject("Something broke");
                    }
                });
            return deferred.promise;
        },

        /**
         * @ngdoc method
         * @name umbraco.services.localizationService#tokenize
         * @methodOf umbraco.services.localizationService
         *
         * @description
         * Helper to tokenize and compile a localization string
         * @param {String} value the value to tokenize
         * @param {Object} scope the $scope object 
         * @returns {String} tokenized resource string
         */
        tokenize: function (value, scope) {
            if (value) {
                var localizer = value.split(':');
                var retval = { tokens: undefined, key: localizer[0].substring(0) };
                if (localizer.length > 1) {
                    retval.tokens = localizer[1].split(',');
                    for (var x = 0; x < retval.tokens.length; x++) {
                        retval.tokens[x] = scope.$eval(retval.tokens[x]);
                    }
                }

                return retval;
            }
            return value;
        },

        /**
         * @ngdoc method
         * @name umbraco.services.localizationService#localize
         * @methodOf umbraco.services.localizationService
         *
         * @description
         * Checks the dictionary for a localized resource string
         * @param {String} value the area/key to localize
         * @param {Array} tokens if specified this array will be sent as parameter values 
         * @returns {String} localized resource string
         */
        localize: function (value, tokens) {
            return service.initLocalizedResources().then(function (dic) {
                var val = _lookup(value, tokens, dic);
                return val;
            });
        },

    };

    //This happens after login / auth and assets loading
    eventsService.on("app.authenticated", function () {
        resourceFileLoadStatus = "none";
        resourceLoadingPromise = [];
    });

    // return the local instance when called
    return service;
});

/**
 * @ngdoc service
 * @name umbraco.services.macroService
 *
 *  
 * @description
 * A service to return macro information such as generating syntax to insert a macro into an editor
 */
function macroService() {

    return {
       
        /** parses the special macro syntax like <?UMBRACO_MACRO macroAlias="Map" /> and returns an object with the macro alias and it's parameters */
        parseMacroSyntax: function (syntax) {
            
            //This regex will match an alias of anything except characters that are quotes or new lines (for legacy reasons, when new macros are created
            // their aliases are cleaned an invalid chars are stripped)
            var expression = /(<\?UMBRACO_MACRO (?:.+?)?macroAlias=["']([^\"\'\n\r]+?)["'][\s\S]+?)(\/>|>.*?<\/\?UMBRACO_MACRO>)/i;
            var match = expression.exec(syntax);
            if (!match || match.length < 3) {
                return null;
            }
            var alias = match[2];

            //this will leave us with just the parameters
            var paramsChunk = match[1].trim().replace(new RegExp("UMBRACO_MACRO macroAlias=[\"']" + alias + "[\"']"), "").trim();
            
            var paramExpression = /(\w+?)=['\"]([\s\S]*?)['\"]/g;
            
            var paramMatch;
            var returnVal = {
                macroAlias: alias,
                macroParamsDictionary: {}
            };
            while (paramMatch = paramExpression.exec(paramsChunk)) {
                returnVal.macroParamsDictionary[paramMatch[1]] = paramMatch[2];
            }
            return returnVal;
        },

        /**
         * @ngdoc function
         * @name umbraco.services.macroService#generateWebFormsSyntax
         * @methodOf umbraco.services.macroService
         * @function    
         *
         * @description
         * generates the syntax for inserting a macro into a rich text editor - this is the very old umbraco style syntax
         * 
         * @param {object} args an object containing the macro alias and it's parameter values
         */
        generateMacroSyntax: function (args) {

            // <?UMBRACO_MACRO macroAlias="BlogListPosts" />

            var macroString = '<?UMBRACO_MACRO macroAlias=\"' + args.macroAlias + "\" ";

            if (args.macroParamsDictionary) {

                _.each(args.macroParamsDictionary, function (val, key) {
                    //check for null
                    val = val ? val : "";
                    //need to detect if the val is a string or an object
                    var keyVal;
                    if (angular.isString(val)) {
                        keyVal = key + "=\"" + (val ? val : "") + "\" ";
                    }
                    else {
                        //if it's not a string we'll send it through the json serializer
                        var json = angular.toJson(val);
                        //then we need to url encode it so that it's safe
                        var encoded = encodeURIComponent(json);
                        keyVal = key + "=\"" + encoded + "\" ";
                    }
                    
                    macroString += keyVal;
                });

            }

            macroString += "/>";

            return macroString;
        },

        /**
         * @ngdoc function
         * @name umbraco.services.macroService#generateWebFormsSyntax
         * @methodOf umbraco.services.macroService
         * @function    
         *
         * @description
         * generates the syntax for inserting a macro into a webforms templates
         * 
         * @param {object} args an object containing the macro alias and it's parameter values
         */
        generateWebFormsSyntax: function(args) {
            
            var macroString = '<umbraco:Macro ';

            if (args.macroParamsDictionary) {
                
                _.each(args.macroParamsDictionary, function (val, key) {
                    var keyVal = key + "=\"" + (val ? val : "") + "\" ";
                    macroString += keyVal;
                });

            }

            macroString += "Alias=\"" + args.macroAlias + "\" runat=\"server\"></umbraco:Macro>";

            return macroString;
        },
        
        /**
         * @ngdoc function
         * @name umbraco.services.macroService#generateMvcSyntax
         * @methodOf umbraco.services.macroService
         * @function    
         *
         * @description
         * generates the syntax for inserting a macro into an mvc template
         * 
         * @param {object} args an object containing the macro alias and it's parameter values
         */
        generateMvcSyntax: function (args) {

            var macroString = "@Umbraco.RenderMacro(\"" + args.macroAlias + "\"";

            var hasParams = false;
            var paramString;
            if (args.macroParamsDictionary) {
                
                paramString = ", new {";

                _.each(args.macroParamsDictionary, function(val, key) {

                    hasParams = true;
                    
                    var keyVal = key + "=\"" + (val ? val : "") + "\", ";

                    paramString += keyVal;
                });
                
                //remove the last , 
                paramString = paramString.trimEnd(", ");

                paramString += "}";
            }
            if (hasParams) {
                macroString += paramString;
            }

            macroString += ")";
            return macroString;
        },

        collectValueData: function(macro, macroParams, renderingEngine) {

            var paramDictionary = {};
            var macroAlias = macro.alias;
            var syntax;

            _.each(macroParams, function (item) {

                var val = item.value;

                if (item.value !== null && item.value !== undefined && !_.isString(item.value)) {
                    try {
                        val = angular.toJson(val);
                    }
                    catch (e) {
                        // not json
                    }
                }

                //each value needs to be xml escaped!! since the value get's stored as an xml attribute
                paramDictionary[item.alias] = _.escape(val);

            });

            //get the syntax based on the rendering engine
            if (renderingEngine && renderingEngine === "WebForms") {
                syntax = this.generateWebFormsSyntax({ macroAlias: macroAlias, macroParamsDictionary: paramDictionary });
            }
            else if (renderingEngine && renderingEngine === "Mvc") {
                syntax = this.generateMvcSyntax({ macroAlias: macroAlias, macroParamsDictionary: paramDictionary });
            }
            else {
                syntax = this.generateMacroSyntax({ macroAlias: macroAlias, macroParamsDictionary: paramDictionary });
            }

            var macroObject = {
                "macroParamsDictionary": paramDictionary,
                "macroAlias": macroAlias,
                "syntax": syntax
            };

            return macroObject;

        }

    };

}

angular.module('umbraco.services').factory('macroService', macroService);

/**
* @ngdoc service
* @name umbraco.services.mediaHelper
* @description A helper object used for dealing with media items
**/
function mediaHelper(umbRequestHelper) {
    
    //container of fileresolvers
    var _mediaFileResolvers = {};

    return {
        /**
         * @ngdoc function
         * @name umbraco.services.mediaHelper#getImagePropertyValue
         * @methodOf umbraco.services.mediaHelper
         * @function    
         *
         * @description
         * Returns the file path associated with the media property if there is one
         * 
         * @param {object} options Options object
         * @param {object} options.mediaModel The media object to retrieve the image path from
         * @param {object} options.imageOnly Optional, if true then will only return a path if the media item is an image
         */
        getMediaPropertyValue: function (options) {
            if (!options || !options.mediaModel) {
                throw "The options objet does not contain the required parameters: mediaModel";
            }

            //combine all props, TODO: we really need a better way then this
            var props = [];
            if (options.mediaModel.properties) {
                props = options.mediaModel.properties;
            } else {
                $(options.mediaModel.tabs).each(function (i, tab) {
                    props = props.concat(tab.properties);
                });
            }

            var mediaRoot = Umbraco.Sys.ServerVariables.umbracoSettings.mediaPath;
            var imageProp = _.find(props, function (item) {
                if (item.alias === "umbracoFile") {
                    return true;
                }

                //this performs a simple check to see if we have a media file as value
                //it doesnt catch everything, but better then nothing
                if (angular.isString(item.value) &&  item.value.indexOf(mediaRoot) === 0) {
                    return true;
                }

                return false;
            });

            if (!imageProp) {
                return "";
            }

            var mediaVal;

            //our default images might store one or many images (as csv)
            var split = imageProp.value.split(',');
            var self = this;
            mediaVal = _.map(split, function (item) {
                return { file: item, isImage: self.detectIfImageByExtension(item) };
            });

            //for now we'll just return the first image in the collection.
            //TODO: we should enable returning many to be displayed in the picker if the uploader supports many.
            if (mediaVal.length && mediaVal.length > 0) {
                if (!options.imageOnly || (options.imageOnly === true && mediaVal[0].isImage)) {
                    return mediaVal[0].file;
                }
            }

            return "";
        },
        
        /**
         * @ngdoc function
         * @name umbraco.services.mediaHelper#getImagePropertyValue
         * @methodOf umbraco.services.mediaHelper
         * @function    
         *
         * @description
         * Returns the actual image path associated with the image property if there is one
         * 
         * @param {object} options Options object
         * @param {object} options.imageModel The media object to retrieve the image path from
         */
        getImagePropertyValue: function (options) {
            if (!options || (!options.imageModel && !options.mediaModel)) {
                throw "The options objet does not contain the required parameters: imageModel";
            }

            //required to support backwards compatibility.
            options.mediaModel = options.imageModel ? options.imageModel : options.mediaModel;

            options.imageOnly = true;

            return this.getMediaPropertyValue(options);
        },
        /**
         * @ngdoc function
         * @name umbraco.services.mediaHelper#getThumbnail
         * @methodOf umbraco.services.mediaHelper
         * @function    
         *
         * @description
         * formats the display model used to display the content to the model used to save the content
         * 
         * @param {object} options Options object
         * @param {object} options.imageModel The media object to retrieve the image path from
         */
        getThumbnail: function (options) {

            if (!options || !options.imageModel) {
                throw "The options objet does not contain the required parameters: imageModel";
            }

            var imagePropVal = this.getImagePropertyValue(options);
            if (imagePropVal !== "") {
                return this.getThumbnailFromPath(imagePropVal);
            }
            return "";
        },

        registerFileResolver: function(propertyEditorAlias, func){
            _mediaFileResolvers[propertyEditorAlias] = func;
        },

        /**
         * @ngdoc function
         * @name umbraco.services.mediaHelper#resolveFileFromEntity
         * @methodOf umbraco.services.mediaHelper
         * @function    
         *
         * @description
         * Gets the media file url for a media entity returned with the entityResource
         * 
         * @param {object} mediaEntity A media Entity returned from the entityResource
         * @param {boolean} thumbnail Whether to return the thumbnail url or normal url
         */
        resolveFileFromEntity : function(mediaEntity, thumbnail) {
            
            if (!angular.isObject(mediaEntity.metaData)) {
                throw "Cannot resolve the file url from the mediaEntity, it does not contain the required metaData";
            }

            var values = _.values(mediaEntity.metaData);
            for (var i = 0; i < values.length; i++) {
                var val = values[i];
                if (angular.isObject(val) && val.PropertyEditorAlias) {
                    for (var resolver in _mediaFileResolvers) {
                        if (val.PropertyEditorAlias === resolver) {
                            //we need to format a property variable that coincides with how the property would be structured
                            // if it came from the mediaResource just to keep things slightly easier for the file resolvers.
                            var property = { value: val.Value };

                            return _mediaFileResolvers[resolver](property, mediaEntity, thumbnail);
                        }
                    }
                }
            }

            return "";
        },

        /**
         * @ngdoc function
         * @name umbraco.services.mediaHelper#resolveFile
         * @methodOf umbraco.services.mediaHelper
         * @function    
         *
         * @description
         * Gets the media file url for a media object returned with the mediaResource
         * 
         * @param {object} mediaEntity A media Entity returned from the entityResource
         * @param {boolean} thumbnail Whether to return the thumbnail url or normal url
         */
        /*jshint loopfunc: true */
        resolveFile : function(mediaItem, thumbnail){
            
            function iterateProps(props){
                var res = null;
                for(var resolver in _mediaFileResolvers) {
                    var property = _.find(props, function(prop){ return prop.editor === resolver; });
                    if(property){
                        res = _mediaFileResolvers[resolver](property, mediaItem, thumbnail);
                        break;
                    }
                }

                return res;    
            }

            //we either have properties raw on the object, or spread out on tabs
            var result = "";
            if(mediaItem.properties){
                result = iterateProps(mediaItem.properties);
            }else if(mediaItem.tabs){
                for(var tab in mediaItem.tabs) {
                    if(mediaItem.tabs[tab].properties){
                        result = iterateProps(mediaItem.tabs[tab].properties);
                        if(result){
                            break;
                        }
                    }
                }
            }
            return result;            
        },

        /*jshint loopfunc: true */
        hasFilePropertyType : function(mediaItem){
           function iterateProps(props){
               var res = false;
               for(var resolver in _mediaFileResolvers) {
                   var property = _.find(props, function(prop){ return prop.editor === resolver; });
                   if(property){
                       res = true;
                       break;
                   }
               }
               return res;
           }

           //we either have properties raw on the object, or spread out on tabs
           var result = false;
           if(mediaItem.properties){
               result = iterateProps(mediaItem.properties);
           }else if(mediaItem.tabs){
               for(var tab in mediaItem.tabs) {
                   if(mediaItem.tabs[tab].properties){
                       result = iterateProps(mediaItem.tabs[tab].properties);
                       if(result){
                           break;
                       }
                   }
               }
           }
           return result;
        },

        /**
         * @ngdoc function
         * @name umbraco.services.mediaHelper#scaleToMaxSize
         * @methodOf umbraco.services.mediaHelper
         * @function    
         *
         * @description
         * Finds the corrct max width and max height, given maximum dimensions and keeping aspect ratios
         * 
         * @param {number} maxSize Maximum width & height
         * @param {number} width Current width
         * @param {number} height Current height
         */
        scaleToMaxSize: function (maxSize, width, height) {
            var retval = { width: width, height: height };

            var maxWidth = maxSize; // Max width for the image
            var maxHeight = maxSize;    // Max height for the image
            var ratio = 0;  // Used for aspect ratio

            // Check if the current width is larger than the max
            if (width > maxWidth) {
                ratio = maxWidth / width;   // get ratio for scaling image

                retval.width = maxWidth;
                retval.height = height * ratio;

                height = height * ratio;    // Reset height to match scaled image
                width = width * ratio;    // Reset width to match scaled image
            }

            // Check if current height is larger than max
            if (height > maxHeight) {
                ratio = maxHeight / height; // get ratio for scaling image

                retval.height = maxHeight;
                retval.width = width * ratio;
                width = width * ratio;    // Reset width to match scaled image
            }

            return retval;
        },

        /**
         * @ngdoc function
         * @name umbraco.services.mediaHelper#getThumbnailFromPath
         * @methodOf umbraco.services.mediaHelper
         * @function    
         *
         * @description
         * Returns the path to the thumbnail version of a given media library image path
         * 
         * @param {string} imagePath Image path, ex: /media/1234/my-image.jpg
         */
        getThumbnailFromPath: function (imagePath) {

            //If the path is not an image we cannot get a thumb
            if (!this.detectIfImageByExtension(imagePath)) {
                return null;
            }

            //get the proxy url for big thumbnails (this ensures one is always generated)
            var thumbnailUrl = umbRequestHelper.getApiUrl(
                "imagesApiBaseUrl",
                "GetBigThumbnail",
                [{ originalImagePath: imagePath }]);

            //var ext = imagePath.substr(imagePath.lastIndexOf('.'));
            //return imagePath.substr(0, imagePath.lastIndexOf('.')) + "_big-thumb" + ".jpg";

            return thumbnailUrl;
        },

        /**
         * @ngdoc function
         * @name umbraco.services.mediaHelper#detectIfImageByExtension
         * @methodOf umbraco.services.mediaHelper
         * @function    
         *
         * @description
         * Returns true/false, indicating if the given path has an allowed image extension
         * 
         * @param {string} imagePath Image path, ex: /media/1234/my-image.jpg
         */
        detectIfImageByExtension: function (imagePath) {

            if (!imagePath) {
                return false;
            }
            
            var lowered = imagePath.toLowerCase();
            var ext = lowered.substr(lowered.lastIndexOf(".") + 1);
            return ("," + Umbraco.Sys.ServerVariables.umbracoSettings.imageFileTypes + ",").indexOf("," + ext + ",") !== -1;
        },

        /**
         * @ngdoc function
         * @name umbraco.services.mediaHelper#formatFileTypes
         * @methodOf umbraco.services.mediaHelper
         * @function
         *
         * @description
         * Returns a string with correctly formated file types for ng-file-upload
         *
         * @param {string} file types, ex: jpg,png,tiff
         */
        formatFileTypes: function(fileTypes) {

           var fileTypesArray = fileTypes.split(',');
           var newFileTypesArray = [];

           for (var i = 0; i < fileTypesArray.length; i++) {
              var fileType = fileTypesArray[i];

              if (fileType.indexOf(".") !== 0) {
                 fileType = ".".concat(fileType);
              }

              newFileTypesArray.push(fileType);
           }

           return newFileTypesArray.join(",");

        }
        
    };
}angular.module('umbraco.services').factory('mediaHelper', mediaHelper);

/**
 * @ngdoc service
 * @name umbraco.services.mediaTypeHelper
 * @description A helper service for the media types
 **/
function mediaTypeHelper(mediaTypeResource, $q) {

    var mediaTypeHelperService = {

        getAllowedImagetypes: function (mediaId){
				
            // Get All allowedTypes
            return mediaTypeResource.getAllowedTypes(mediaId)
                .then(function(types){
                    
                    var allowedQ = types.map(function(type){
                        return mediaTypeResource.getById(type.id);
                    });

                    // Get full list
                    return $q.all(allowedQ).then(function(fullTypes){

                        // Find all the media types with an Image Cropper property editor
                        var filteredTypes = mediaTypeHelperService.getTypeWithEditor(fullTypes, ['Umbraco.ImageCropper']);

                        // If there is only one media type with an Image Cropper we will return this one
                        if(filteredTypes.length === 1) {
                            return filteredTypes;
                        // If there is more than one Image cropper, custom media types have been added, and we return all media types with and Image cropper or UploadField
                        } else {
                            return mediaTypeHelperService.getTypeWithEditor(fullTypes, ['Umbraco.ImageCropper', 'Umbraco.UploadField']);
                        }

                    });
            });
		},

        getTypeWithEditor: function (types, editors) {

            return types.filter(function (mediatype) {
                for (var i = 0; i < mediatype.groups.length; i++) {
                    var group = mediatype.groups[i];
                    for (var j = 0; j < group.properties.length; j++) {
                        var property = group.properties[j];
                        if( editors.indexOf(property.editor) !== -1 ) {
                            return mediatype;
                        }
                    }
                }
            });

        }

    };

    return mediaTypeHelperService;
}
angular.module('umbraco.services').factory('mediaTypeHelper', mediaTypeHelper);

/**
 * @ngdoc service
 * @name umbraco.services.umbracoMenuActions
 *
 * @requires q
 * @requires treeService
 *	
 * @description
 * Defines the methods that are called when menu items declare only an action to execute
 */
function umbracoMenuActions($q, treeService, $location, navigationService, appState) {
    
    return {
        
        /**
         * @ngdoc method
         * @name umbraco.services.umbracoMenuActions#RefreshNode
         * @methodOf umbraco.services.umbracoMenuActions
         * @function
         *
         * @description
         * Clears all node children and then gets it's up-to-date children from the server and re-assigns them
         * @param {object} args An arguments object
         * @param {object} args.entity The basic entity being acted upon
         * @param {object} args.treeAlias The tree alias associated with this entity
         * @param {object} args.section The current section
         */
        "RefreshNode": function (args) {
            
            ////just in case clear any tree cache for this node/section
            //treeService.clearCache({
            //    cacheKey: "__" + args.section, //each item in the tree cache is cached by the section name
            //    childrenOf: args.entity.parentId //clear the children of the parent
            //});

            //since we're dealing with an entity, we need to attempt to find it's tree node, in the main tree
            // this action is purely a UI thing so if for whatever reason there is no loaded tree node in the UI
            // we can safely ignore this process.
            
            //to find a visible tree node, we'll go get the currently loaded root node from appState
            var treeRoot = appState.getTreeState("currentRootNode");
            if (treeRoot && treeRoot.root) {
                var treeNode = treeService.getDescendantNode(treeRoot.root, args.entity.id, args.treeAlias);
                if (treeNode) {
                    treeService.loadNodeChildren({ node: treeNode, section: args.section });
                }                
            }

            
        },
        
        /**
         * @ngdoc method
         * @name umbraco.services.umbracoMenuActions#CreateChildEntity
         * @methodOf umbraco.services.umbracoMenuActions
         * @function
         *
         * @description
         * This will re-route to a route for creating a new entity as a child of the current node
         * @param {object} args An arguments object
         * @param {object} args.entity The basic entity being acted upon
         * @param {object} args.treeAlias The tree alias associated with this entity
         * @param {object} args.section The current section
         */
        "CreateChildEntity": function (args) {

            navigationService.hideNavigation();

            var route = "/" + args.section + "/" + args.treeAlias + "/edit/" + args.entity.id;
            //change to new path
            $location.path(route).search({ create: true });
            
        }
    };
} 

angular.module('umbraco.services').factory('umbracoMenuActions', umbracoMenuActions);
/**
 * @ngdoc service
 * @name umbraco.services.navigationService
 *
 * @requires $rootScope
 * @requires $routeParams
 * @requires $log
 * @requires $location
 * @requires dialogService
 * @requires treeService
 * @requires sectionResource
 *
 * @description
 * Service to handle the main application navigation. Responsible for invoking the tree
 * Section navigation and search, and maintain their state for the entire application lifetime
 *
 */
function navigationService($rootScope, $routeParams, $log, $location, $q, $timeout, $injector, dialogService, umbModelMapper, treeService, notificationsService, historyService, appState, angularHelper) {


    //used to track the current dialog object
    var currentDialog = null;

    //the main tree event handler, which gets assigned via the setupTreeEvents method
    var mainTreeEventHandler = null;
    //tracks the user profile dialog
    var userDialog = null;

    function setMode(mode) {
        switch (mode) {
        case 'tree':
            appState.setGlobalState("navMode", "tree");
            appState.setGlobalState("showNavigation", true);
            appState.setMenuState("showMenu", false);
            appState.setMenuState("showMenuDialog", false);
            appState.setGlobalState("stickyNavigation", false);
            appState.setGlobalState("showTray", false);

            //$("#search-form input").focus();
            break;
        case 'menu':
            appState.setGlobalState("navMode", "menu");
            appState.setGlobalState("showNavigation", true);
            appState.setMenuState("showMenu", true);
            appState.setMenuState("showMenuDialog", false);
            appState.setGlobalState("stickyNavigation", true);
            break;
        case 'dialog':
            appState.setGlobalState("navMode", "dialog");
            appState.setGlobalState("stickyNavigation", true);
            appState.setGlobalState("showNavigation", true);
            appState.setMenuState("showMenu", false);
            appState.setMenuState("showMenuDialog", true);
            break;
        case 'search':
            appState.setGlobalState("navMode", "search");
            appState.setGlobalState("stickyNavigation", false);
            appState.setGlobalState("showNavigation", true);
            appState.setMenuState("showMenu", false);
            appState.setSectionState("showSearchResults", true);
            appState.setMenuState("showMenuDialog", false);

            //TODO: This would be much better off in the search field controller listening to appState changes
            $timeout(function() {
                $("#search-field").focus();
            });

            break;
        default:
            appState.setGlobalState("navMode", "default");
            appState.setMenuState("showMenu", false);
            appState.setMenuState("showMenuDialog", false);
            appState.setSectionState("showSearchResults", false);
            appState.setGlobalState("stickyNavigation", false);
            appState.setGlobalState("showTray", false);

            if (appState.getGlobalState("isTablet") === true) {
                appState.setGlobalState("showNavigation", false);
            }

            break;
        }
    }

    var service = {

        /** initializes the navigation service */
        init: function() {

            //keep track of the current section - initially this will always be undefined so
            // no point in setting it now until it changes.
            $rootScope.$watch(function () {
                return $routeParams.section;
            }, function (newVal, oldVal) {
                appState.setSectionState("currentSection", newVal);
            });


        },

        /**
         * @ngdoc method
         * @name umbraco.services.navigationService#load
         * @methodOf umbraco.services.navigationService
         *
         * @description
         * Shows the legacy iframe and loads in the content based on the source url
         * @param {String} source The URL to load into the iframe
         */
        loadLegacyIFrame: function (source) {
            $location.path("/" + appState.getSectionState("currentSection") + "/framed/" + encodeURIComponent(source));
        },

        /**
         * @ngdoc method
         * @name umbraco.services.navigationService#changeSection
         * @methodOf umbraco.services.navigationService
         *
         * @description
         * Changes the active section to a given section alias
         * If the navigation is 'sticky' this will load the associated tree
         * and load the dashboard related to the section
         * @param {string} sectionAlias The alias of the section
         */
        changeSection: function(sectionAlias, force) {
            setMode("default-opensection");

            if (force && appState.getSectionState("currentSection") === sectionAlias) {
                appState.setSectionState("currentSection", "");
            }

            appState.setSectionState("currentSection", sectionAlias);
            this.showTree(sectionAlias);

            $location.path(sectionAlias);
        },

        /**
         * @ngdoc method
         * @name umbraco.services.navigationService#showTree
         * @methodOf umbraco.services.navigationService
         *
         * @description
         * Displays the tree for a given section alias but turning on the containing dom element
         * only changes if the section is different from the current one
		 * @param {string} sectionAlias The alias of the section to load
         * @param {Object} syncArgs Optional object of arguments for syncing the tree for the section being shown
		 */
        showTree: function (sectionAlias, syncArgs) {
            if (sectionAlias !== appState.getSectionState("currentSection")) {
                appState.setSectionState("currentSection", sectionAlias);

                if (syncArgs) {
                    this.syncTree(syncArgs);
                }
            }
            setMode("tree");
        },

        showTray: function () {
            appState.setGlobalState("showTray", true);
        },

        hideTray: function () {
            appState.setGlobalState("showTray", false);
        },

        /**
            Called to assign the main tree event handler - this is called by the navigation controller.
            TODO: Potentially another dev could call this which would kind of mung the whole app so potentially there's a better way.
        */
        setupTreeEvents: function(treeEventHandler) {
            mainTreeEventHandler = treeEventHandler;

            //when a tree is loaded into a section, we need to put it into appState
            mainTreeEventHandler.bind("treeLoaded", function(ev, args) {
                appState.setTreeState("currentRootNode", args.tree);
            });

            //when a tree node is synced this event will fire, this allows us to set the currentNode
            mainTreeEventHandler.bind("treeSynced", function (ev, args) {

                if (args.activate === undefined || args.activate === true) {
                    //set the current selected node
                    appState.setTreeState("selectedNode", args.node);
                    //when a node is activated, this is the same as clicking it and we need to set the
                    //current menu item to be this node as well.
                    appState.setMenuState("currentNode", args.node);
                }
            });

            //this reacts to the options item in the tree
            mainTreeEventHandler.bind("treeOptionsClick", function(ev, args) {
                ev.stopPropagation();
                ev.preventDefault();

                //Set the current action node (this is not the same as the current selected node!)
                appState.setMenuState("currentNode", args.node);

                if (args.event && args.event.altKey) {
                    args.skipDefault = true;
                }

                service.showMenu(ev, args);
            });

            mainTreeEventHandler.bind("treeNodeAltSelect", function(ev, args) {
                ev.stopPropagation();
                ev.preventDefault();

                args.skipDefault = true;
                service.showMenu(ev, args);
            });

            //this reacts to tree items themselves being clicked
            //the tree directive should not contain any handling, simply just bubble events
            mainTreeEventHandler.bind("treeNodeSelect", function (ev, args) {
                var n = args.node;
                ev.stopPropagation();
                ev.preventDefault();

                if (n.metaData && n.metaData["jsClickCallback"] && angular.isString(n.metaData["jsClickCallback"]) && n.metaData["jsClickCallback"] !== "") {
                    //this is a legacy tree node!
                    var jsPrefix = "javascript:";
                    var js;
                    if (n.metaData["jsClickCallback"].startsWith(jsPrefix)) {
                        js = n.metaData["jsClickCallback"].substr(jsPrefix.length);
                    }
                    else {
                        js = n.metaData["jsClickCallback"];
                    }
                    try {
                        var func = eval(js);
                        //this is normally not necessary since the eval above should execute the method and will return nothing.
                        if (func != null && (typeof func === "function")) {
                            func.call();
                        }
                    }
                    catch(ex) {
                        $log.error("Error evaluating js callback from legacy tree node: " + ex);
                    }
                }
                else if (n.routePath) {
                    //add action to the history service
                    historyService.add({ name: n.name, link: n.routePath, icon: n.icon });

                    //put this node into the tree state
                    appState.setTreeState("selectedNode", args.node);
                    //when a node is clicked we also need to set the active menu node to this node
                    appState.setMenuState("currentNode", args.node);

                    //not legacy, lets just set the route value and clear the query string if there is one.
                    $location.path(n.routePath).search("");
                }
                else if (args.element.section) {
                    $location.path(args.element.section).search("");
                }

                service.hideNavigation();
            });
        },
        /**
         * @ngdoc method
         * @name umbraco.services.navigationService#syncTree
         * @methodOf umbraco.services.navigationService
         *
         * @description
         * Syncs a tree with a given path, returns a promise
         * The path format is: ["itemId","itemId"], and so on
         * so to sync to a specific document type node do:
         * <pre>
         * navigationService.syncTree({tree: 'content', path: ["-1","123d"], forceReload: true});
         * </pre>
         * @param {Object} args arguments passed to the function
         * @param {String} args.tree the tree alias to sync to
         * @param {Array} args.path the path to sync the tree to
         * @param {Boolean} args.forceReload optional, specifies whether to force reload the node data from the server even if it already exists in the tree currently
         * @param {Boolean} args.activate optional, specifies whether to set the synced node to be the active node, this will default to true if not specified
         */
        syncTree: function (args) {
            if (!args) {
                throw "args cannot be null";
            }
            if (!args.path) {
                throw "args.path cannot be null";
            }
            if (!args.tree) {
                throw "args.tree cannot be null";
            }

            if (mainTreeEventHandler) {
                //returns a promise
                return mainTreeEventHandler.syncTree(args);
            }

            //couldn't sync
            return angularHelper.rejectedPromise();
        },

        /**
            Internal method that should ONLY be used by the legacy API wrapper, the legacy API used to
            have to set an active tree and then sync, the new API does this in one method by using syncTree
        */
        _syncPath: function(path, forceReload) {
            if (mainTreeEventHandler) {
                mainTreeEventHandler.syncTree({ path: path, forceReload: forceReload });
            }
        },

        //TODO: This should return a promise
        reloadNode: function(node) {
            if (mainTreeEventHandler) {
                mainTreeEventHandler.reloadNode(node);
            }
        },

        //TODO: This should return a promise
        reloadSection: function(sectionAlias) {
            if (mainTreeEventHandler) {
                mainTreeEventHandler.clearCache({ section: sectionAlias });
                mainTreeEventHandler.load(sectionAlias);
            }
        },

        /**
            Internal method that should ONLY be used by the legacy API wrapper, the legacy API used to
            have to set an active tree and then sync, the new API does this in one method by using syncTreePath
        */
        _setActiveTreeType: function (treeAlias, loadChildren) {
            if (mainTreeEventHandler) {
                mainTreeEventHandler._setActiveTreeType(treeAlias, loadChildren);
            }
        },

        /**
         * @ngdoc method
         * @name umbraco.services.navigationService#hideTree
         * @methodOf umbraco.services.navigationService
         *
         * @description
         * Hides the tree by hiding the containing dom element
         */
        hideTree: function() {

            if (appState.getGlobalState("isTablet") === true && !appState.getGlobalState("stickyNavigation")) {
                //reset it to whatever is in the url
                appState.setSectionState("currentSection", $routeParams.section);
                setMode("default-hidesectiontree");
            }

        },

        /**
         * @ngdoc method
         * @name umbraco.services.navigationService#showMenu
         * @methodOf umbraco.services.navigationService
         *
         * @description
         * Hides the tree by hiding the containing dom element.
         * This always returns a promise!
         *
         * @param {Event} event the click event triggering the method, passed from the DOM element
         */
        showMenu: function(event, args) {

            var deferred = $q.defer();
            var self = this;

            treeService.getMenu({ treeNode: args.node })
                .then(function(data) {

                    //check for a default
                    //NOTE: event will be undefined when a call to hideDialog is made so it won't re-load the default again.
                    // but perhaps there's a better way to deal with with an additional parameter in the args ? it works though.
                    if (data.defaultAlias && !args.skipDefault) {

                        var found = _.find(data.menuItems, function(item) {
                            return item.alias = data.defaultAlias;
                        });

                        if (found) {

                            //NOTE: This is assigning the current action node - this is not the same as the currently selected node!
                            appState.setMenuState("currentNode", args.node);

                            //ensure the current dialog is cleared before creating another!
                            if (currentDialog) {
                                dialogService.close(currentDialog);
                            }

                            var dialog = self.showDialog({
                                node: args.node,
                                action: found,
                                section: appState.getSectionState("currentSection")
                            });

                            //return the dialog this is opening.
                            deferred.resolve(dialog);
                            return;
                        }
                    }

                    //there is no default or we couldn't find one so just continue showing the menu

                    setMode("menu");

                    appState.setMenuState("currentNode", args.node);
                    appState.setMenuState("menuActions", data.menuItems);
                    appState.setMenuState("dialogTitle", args.node.name);

                    //we're not opening a dialog, return null.
                    deferred.resolve(null);
                });

            return deferred.promise;
        },

        /**
         * @ngdoc method
         * @name umbraco.services.navigationService#hideMenu
         * @methodOf umbraco.services.navigationService
         *
         * @description
         * Hides the menu by hiding the containing dom element
         */
        hideMenu: function() {
            //SD: Would we ever want to access the last action'd node instead of clearing it here?
            appState.setMenuState("currentNode", null);
            appState.setMenuState("menuActions", []);
            setMode("tree");
        },

        /** Executes a given menu action */
        executeMenuAction: function (action, node, section) {

            if (!action) {
                throw "action cannot be null";
            }
            if (!node) {
                throw "node cannot be null";
            }
            if (!section) {
                throw "section cannot be null";
            }

            if (action.metaData && action.metaData["actionRoute"] && angular.isString(action.metaData["actionRoute"])) {
                //first check if the menu item simply navigates to a route
                var parts = action.metaData["actionRoute"].split("?");
                $location.path(parts[0]).search(parts.length > 1 ? parts[1] : "");
                this.hideNavigation();
                return;
            }
            else if (action.metaData && action.metaData["jsAction"] && angular.isString(action.metaData["jsAction"])) {

                //we'll try to get the jsAction from the injector
                var menuAction = action.metaData["jsAction"].split('.');
                if (menuAction.length !== 2) {

                    //if it is not two parts long then this most likely means that it's a legacy action
                    var js = action.metaData["jsAction"].replace("javascript:", "");
                    //there's not really a different way to acheive this except for eval
                    eval(js);
                }
                else {
                    var menuActionService = $injector.get(menuAction[0]);
                    if (!menuActionService) {
                        throw "The angular service " + menuAction[0] + " could not be found";
                    }

                    var method = menuActionService[menuAction[1]];

                    if (!method) {
                        throw "The method " + menuAction[1] + " on the angular service " + menuAction[0] + " could not be found";
                    }

                    method.apply(this, [{
                        //map our content object to a basic entity to pass in to the menu handlers,
                        //this is required for consistency since a menu item needs to be decoupled from a tree node since the menu can
                        //exist standalone in the editor for which it can only pass in an entity (not tree node).
                        entity: umbModelMapper.convertToEntityBasic(node),
                        action: action,
                        section: section,
                        treeAlias: treeService.getTreeAlias(node)
                    }]);
                }
            }
            else {
                service.showDialog({
                    node: node,
                    action: action,
                    section: section
                });
            }
        },

        /**
         * @ngdoc method
         * @name umbraco.services.navigationService#showUserDialog
         * @methodOf umbraco.services.navigationService
         *
         * @description
         * Opens the user dialog, next to the sections navigation
         * template is located in views/common/dialogs/user.html
         */
        showUserDialog: function () {
            // hide tray and close help dialog
            if (service.helpDialog) {
                service.helpDialog.close();
            }
            service.hideTray();

            if (service.userDialog) {
                service.userDialog.close();
                service.userDialog = undefined;
            }

            service.userDialog = dialogService.open(
            {
                template: "views/common/dialogs/user.html",
                modalClass: "umb-modal-left",
                show: true
            });

            return service.userDialog;
        },

        /**
         * @ngdoc method
         * @name umbraco.services.navigationService#showUserDialog
         * @methodOf umbraco.services.navigationService
         *
         * @description
         * Opens the user dialog, next to the sections navigation
         * template is located in views/common/dialogs/user.html
         */
        showHelpDialog: function () {
            // hide tray and close user dialog
            service.hideTray();
            if (service.userDialog) {
                service.userDialog.close();
            }

            if(service.helpDialog){
                service.helpDialog.close();
                service.helpDialog = undefined;
            }

            service.helpDialog = dialogService.open(
            {
                template: "views/common/dialogs/help.html",
                modalClass: "umb-modal-left",
                show: true
            });

            return service.helpDialog;
        },

        /**
         * @ngdoc method
         * @name umbraco.services.navigationService#showDialog
         * @methodOf umbraco.services.navigationService
         *
         * @description
         * Opens a dialog, for a given action on a given tree node
         * uses the dialogService to inject the selected action dialog
         * into #dialog div.umb-panel-body
         * the path to the dialog view is determined by:
         * "views/" + current tree + "/" + action alias + ".html"
         * The dialog controller will get passed a scope object that is created here with the properties:
         *  scope.currentNode = the selected tree node
         *  scope.currentAction = the selected menu item
         *  so that the dialog controllers can use these properties
         *
         * @param {Object} args arguments passed to the function
         * @param {Scope} args.scope current scope passed to the dialog
         * @param {Object} args.action the clicked action containing `name` and `alias`
         */
        showDialog: function(args) {

            if (!args) {
                throw "showDialog is missing the args parameter";
            }
            if (!args.action) {
                throw "The args parameter must have an 'action' property as the clicked menu action object";
            }
            if (!args.node) {
                throw "The args parameter must have a 'node' as the active tree node";
            }

            //ensure the current dialog is cleared before creating another!
            if (currentDialog) {
                dialogService.close(currentDialog);
                currentDialog = null;
            }

            setMode("dialog");

            //NOTE: Set up the scope object and assign properties, this is legacy functionality but we have to live with it now.
            // we should be passing in currentNode and currentAction using 'dialogData' for the dialog, not attaching it to a scope.
            // This scope instance will be destroyed by the dialog so it cannot be a scope that exists outside of the dialog.
            // If a scope instance has been passed in, we'll have to create a child scope of it, otherwise a new root scope.
            var dialogScope = args.scope ? args.scope.$new() : $rootScope.$new();
            dialogScope.currentNode = args.node;
            dialogScope.currentAction = args.action;

            //the title might be in the meta data, check there first
            if (args.action.metaData["dialogTitle"]) {
                appState.setMenuState("dialogTitle", args.action.metaData["dialogTitle"]);
            }
            else {
                appState.setMenuState("dialogTitle", args.action.name);
            }

            var templateUrl;
            var iframe;

            if (args.action.metaData["actionUrl"]) {
                templateUrl = args.action.metaData["actionUrl"];
                iframe = true;
            }
            else if (args.action.metaData["actionView"]) {
                templateUrl = args.action.metaData["actionView"];
                iframe = false;
            }
            else {

                //by convention we will look into the /views/{treetype}/{action}.html
                // for example: /views/content/create.html

                //we will also check for a 'packageName' for the current tree, if it exists then the convention will be:
                // for example: /App_Plugins/{mypackage}/backoffice/{treetype}/create.html

                var treeAlias = treeService.getTreeAlias(args.node);
                var packageTreeFolder = treeService.getTreePackageFolder(treeAlias);

                if (!treeAlias) {
                    throw "Could not get tree alias for node " + args.node.id;
                }

                if (packageTreeFolder) {
                    templateUrl = Umbraco.Sys.ServerVariables.umbracoSettings.appPluginsPath +
                        "/" + packageTreeFolder +
                        "/backoffice/" + treeAlias + "/" + args.action.alias + ".html";
                }
                else {
                    templateUrl = "views/" + treeAlias + "/" + args.action.alias + ".html";
                }

                iframe = false;
            }

            //TODO: some action's want to launch a new window like live editing, we support this in the menu item's metadata with
            // a key called: "actionUrlMethod" which can be set to either: Dialog, BlankWindow. Normally this is always set to Dialog
            // if a URL is specified in the "actionUrl" metadata. For now I'm not going to implement launching in a blank window,
            // though would be v-easy, just not sure we want to ever support that?

            var dialog = dialogService.open(
                {
                    container: $("#dialog div.umb-modalcolumn-body"),
                    //The ONLY reason we're passing in scope to the dialogService (which is legacy functionality) is
                    // for backwards compatibility since many dialogs require $scope.currentNode or $scope.currentAction
                    // to exist
                    scope: dialogScope,
                    inline: true,
                    show: true,
                    iframe: iframe,
                    modalClass: "umb-dialog",
                    template: templateUrl,

                    //These will show up on the dialog controller's $scope under dialogOptions
                    currentNode: args.node,
                    currentAction: args.action,
                });

            //save the currently assigned dialog so it can be removed before a new one is created
            currentDialog = dialog;
            return dialog;
        },

        /**
	     * @ngdoc method
	     * @name umbraco.services.navigationService#hideDialog
	     * @methodOf umbraco.services.navigationService
	     *
	     * @description
	     * hides the currently open dialog
	     */
        hideDialog: function (showMenu) {

            setMode("default");

            if(showMenu){
                this.showMenu(undefined, { skipDefault: true, node: appState.getMenuState("currentNode") });
            }
        },
        /**
          * @ngdoc method
          * @name umbraco.services.navigationService#showSearch
          * @methodOf umbraco.services.navigationService
          *
          * @description
          * shows the search pane
          */
        showSearch: function() {
            setMode("search");
        },
        /**
          * @ngdoc method
          * @name umbraco.services.navigationService#hideSearch
          * @methodOf umbraco.services.navigationService
          *
          * @description
          * hides the search pane
        */
        hideSearch: function() {
            setMode("default-hidesearch");
        },
        /**
          * @ngdoc method
          * @name umbraco.services.navigationService#hideNavigation
          * @methodOf umbraco.services.navigationService
          *
          * @description
          * hides any open navigation panes and resets the tree, actions and the currently selected node
          */
        hideNavigation: function() {
            appState.setMenuState("menuActions", []);
            setMode("default");
        }
    };

    return service;
}

angular.module('umbraco.services').factory('navigationService', navigationService);

/**
 * @ngdoc service
 * @name umbraco.services.notificationsService
 *
 * @requires $rootScope 
 * @requires $timeout
 * @requires angularHelper
 *	
 * @description
 * Application-wide service for handling notifications, the umbraco application 
 * maintains a single collection of notications, which the UI watches for changes.
 * By default when a notication is added, it is automaticly removed 7 seconds after
 * This can be changed on add()
 *
 * ##usage
 * To use, simply inject the notificationsService into any controller that needs it, and make
 * sure the umbraco.services module is accesible - which it should be by default.
 *
 * <pre>
 *		notificationsService.success("Document Published", "hooraaaay for you!");
 *      notificationsService.error("Document Failed", "booooh");
 * </pre> 
 */
angular.module('umbraco.services')
.factory('notificationsService', function ($rootScope, $timeout, angularHelper) {

	var nArray = [];
	function setViewPath(view){
		if(view.indexOf('/') < 0)
		{
			view = "views/common/notifications/" + view;
		}

		if(view.indexOf('.html') < 0)
		{
			view = view + ".html";
		}
		return view;
	}

	var service = {

		/**
		* @ngdoc method
		* @name umbraco.services.notificationsService#add
		* @methodOf umbraco.services.notificationsService
		*
		* @description
		* Lower level api for adding notifcations, support more advanced options
		* @param {Object} item The notification item
		* @param {String} item.headline Short headline
		* @param {String} item.message longer text for the notication, trimmed after 200 characters, which can then be exanded
		* @param {String} item.type Notification type, can be: "success","warning","error" or "info" 
		* @param {String} item.url url to open when notification is clicked
		* @param {String} item.view path to custom view to load into the notification box
		* @param {Array} item.actions Collection of button actions to append (label, func, cssClass)
		* @param {Boolean} item.sticky if set to true, the notification will not auto-close
		* @returns {Object} args notification object
		*/

		add: function(item) {
			angularHelper.safeApply($rootScope, function () {

				if(item.view){
					item.view = setViewPath(item.view);
					item.sticky = true;
					item.type = "form";
					item.headline = null;
				}


				//add a colon after the headline if there is a message as well
				if (item.message) {
					item.headline += ": ";
					if(item.message.length > 200) {
						item.sticky = true;
					}
				}	
			
				//we need to ID the item, going by index isn't good enough because people can remove at different indexes 
				// whenever they want. Plus once we remove one, then the next index will be different. The only way to 
				// effectively remove an item is by an Id.
				item.id = String.CreateGuid();

				nArray.push(item);

				if(!item.sticky) {
					$timeout(function() {
						var found = _.find(nArray, function(i) {
						return i.id === item.id;
					});

					if (found) {
						var index = nArray.indexOf(found);
						nArray.splice(index, 1);
					}

					}, 7000);
				}

				return item;
			});

		},

		hasView : function(view){
			if(!view){
				return _.find(nArray, function(notification){ return notification.view;});
			}else{
				view = setViewPath(view).toLowerCase();
				return _.find(nArray, function(notification){ return notification.view.toLowerCase() === view;});
			}	
		},
		addView: function(view, args){
			var item = {
				args: args,
				view: view
			};

			service.add(item);
		},

	    /**
		 * @ngdoc method
		 * @name umbraco.services.notificationsService#showNotification
		 * @methodOf umbraco.services.notificationsService
		 *
		 * @description
		 * Shows a notification based on the object passed in, normally used to render notifications sent back from the server
		 *		 
		 * @returns {Object} args notification object
		 */
        showNotification: function(args) {
            if (!args) {
                throw "args cannot be null";
            }
            if (args.type === undefined || args.type === null) {
                throw "args.type cannot be null";
            }
            if (!args.header) {
                throw "args.header cannot be null";
            }
            
            switch(args.type) {
                case 0:
                    //save
                    this.success(args.header, args.message);
                    break;
                case 1:
                    //info
                    this.success(args.header, args.message);
                    break;
                case 2:
                    //error
                    this.error(args.header, args.message);
                    break;
                case 3:
                    //success
                    this.success(args.header, args.message);
                    break;
                case 4:
                    //warning
                    this.warning(args.header, args.message);
                    break;
            }
        },

	    /**
		 * @ngdoc method
		 * @name umbraco.services.notificationsService#success
		 * @methodOf umbraco.services.notificationsService
		 *
		 * @description
		 * Adds a green success notication to the notications collection
		 * This should be used when an operations *completes* without errors
		 *
		 * @param {String} headline Headline of the notification
		 * @param {String} message longer text for the notication, trimmed after 200 characters, which can then be exanded
		 * @returns {Object} notification object
		 */
	    success: function (headline, message) {
	        return service.add({ headline: headline, message: message, type: 'success', time: new Date() });
	    },
		/**
		 * @ngdoc method
		 * @name umbraco.services.notificationsService#error
		 * @methodOf umbraco.services.notificationsService
		 *
		 * @description
		 * Adds a red error notication to the notications collection
		 * This should be used when an operations *fails* and could not complete
		 * 
		 * @param {String} headline Headline of the notification
		 * @param {String} message longer text for the notication, trimmed after 200 characters, which can then be exanded
		 * @returns {Object} notification object
		 */
	    error: function (headline, message) {
	        return service.add({ headline: headline, message: message, type: 'error', time: new Date() });
		},

		/**
		 * @ngdoc method
		 * @name umbraco.services.notificationsService#warning
		 * @methodOf umbraco.services.notificationsService
		 *
		 * @description
		 * Adds a yellow warning notication to the notications collection
		 * This should be used when an operations *completes* but something was not as expected
		 * 
		 *
		 * @param {String} headline Headline of the notification
		 * @param {String} message longer text for the notication, trimmed after 200 characters, which can then be exanded
		 * @returns {Object} notification object
		 */
	    warning: function (headline, message) {
	        return service.add({ headline: headline, message: message, type: 'warning', time: new Date() });
		},

		/**
		 * @ngdoc method
		 * @name umbraco.services.notificationsService#warning
		 * @methodOf umbraco.services.notificationsService
		 *
		 * @description
		 * Adds a yellow warning notication to the notications collection
		 * This should be used when an operations *completes* but something was not as expected
		 * 
		 *
		 * @param {String} headline Headline of the notification
		 * @param {String} message longer text for the notication, trimmed after 200 characters, which can then be exanded
		 * @returns {Object} notification object
		 */
	    info: function (headline, message) {
	        return service.add({ headline: headline, message: message, type: 'info', time: new Date() });
		},

		/**
		 * @ngdoc method
		 * @name umbraco.services.notificationsService#remove
		 * @methodOf umbraco.services.notificationsService
		 *
		 * @description
		 * Removes a notification from the notifcations collection at a given index 
		 *
		 * @param {Int} index index where the notication should be removed from
		 */
		remove: function (index) {
			if(angular.isObject(index)){
				var i = nArray.indexOf(index);
				angularHelper.safeApply($rootScope, function() {
				    nArray.splice(i, 1);
				});
			}else{
				angularHelper.safeApply($rootScope, function() {
				    nArray.splice(index, 1);
				});	
			}
		},

		/**
		 * @ngdoc method
		 * @name umbraco.services.notificationsService#removeAll
		 * @methodOf umbraco.services.notificationsService
		 *
		 * @description
		 * Removes all notifications from the notifcations collection 
		 */
	    removeAll: function () {
	        angularHelper.safeApply($rootScope, function() {
	            nArray = [];
	        });
		},

		/**
		 * @ngdoc property
		 * @name umbraco.services.notificationsService#current
		 * @propertyOf umbraco.services.notificationsService
		 *
		 * @description
		 * Returns an array of current notifications to display
		 *
		 * @returns {string} returns an array
		 */
		current: nArray,

		/**
		 * @ngdoc method
		 * @name umbraco.services.notificationsService#getCurrent
		 * @methodOf umbraco.services.notificationsService
		 *
		 * @description
		 * Method to return all notifications from the notifcations collection 
		 */
		getCurrent: function(){
			return nArray;
		}
	};

	return service;
});
(function() {
   'use strict';

   function overlayHelper() {

      var numberOfOverlays = 0;

      function registerOverlay() {
         numberOfOverlays++;
         return numberOfOverlays;
      }

      function unregisterOverlay() {
         numberOfOverlays--;
         return numberOfOverlays;
      }

      function getNumberOfOverlays() {
         return numberOfOverlays;
      }

      var service = {
         numberOfOverlays: numberOfOverlays,
         registerOverlay: registerOverlay,
         unregisterOverlay: unregisterOverlay,
         getNumberOfOverlays: getNumberOfOverlays
      };

      return service;

   }


   angular.module('umbraco.services').factory('overlayHelper', overlayHelper);


})();

/**
 * @ngdoc service
 * @name umbraco.services.searchService
 *
 *  
 * @description
 * Service for handling the main application search, can currently search content, media and members
 *
 * ##usage
 * To use, simply inject the searchService into any controller that needs it, and make
 * sure the umbraco.services module is accesible - which it should be by default.
 *
 * <pre>
 *      searchService.searchMembers({term: 'bob'}).then(function(results){
 *          angular.forEach(results, function(result){
 *                  //returns:
 *                  {name: "name", id: 1234, menuUrl: "url", editorPath: "url", metaData: {}, subtitle: "/path/etc" }
 *           })          
 *           var result = 
 *       }) 
 * </pre> 
 */
angular.module('umbraco.services')
.factory('searchService', function ($q, $log, entityResource, contentResource, umbRequestHelper) {

    function configureMemberResult(member) {
        member.menuUrl = umbRequestHelper.getApiUrl("memberTreeBaseUrl", "GetMenu", [{ id: member.id }, { application: 'member' }]);
        member.editorPath = "member/member/edit/" + (member.key ? member.key : member.id);
        angular.extend(member.metaData, { treeAlias: "member" });
        member.subTitle = member.metaData.Email;
    }
    
    function configureMediaResult(media)
    {
        media.menuUrl = umbRequestHelper.getApiUrl("mediaTreeBaseUrl", "GetMenu", [{ id: media.id }, { application: 'media' }]);
        media.editorPath = "media/media/edit/" + media.id;
        angular.extend(media.metaData, { treeAlias: "media" });
    }
    
    function configureContentResult(content) {
        content.menuUrl = umbRequestHelper.getApiUrl("contentTreeBaseUrl", "GetMenu", [{ id: content.id }, { application: 'content' }]);
        content.editorPath = "content/content/edit/" + content.id;
        angular.extend(content.metaData, { treeAlias: "content" });
        content.subTitle = content.metaData.Url;        
    }

    return {

        /**
        * @ngdoc method
        * @name umbraco.services.searchService#searchMembers
        * @methodOf umbraco.services.searchService
        *
        * @description
        * Searches the default member search index
        * @param {Object} args argument object
        * @param {String} args.term seach term
        * @returns {Promise} returns promise containing all matching members
        */
        searchMembers: function(args) {

            if (!args.term) {
                throw "args.term is required";
            }

            return entityResource.search(args.term, "Member", args.searchFrom).then(function (data) {
                _.each(data, function(item) {
                    configureMemberResult(item);
                });         
                return data;
            });
        },

        /**
        * @ngdoc method
        * @name umbraco.services.searchService#searchContent
        * @methodOf umbraco.services.searchService
        *
        * @description
        * Searches the default internal content search index
        * @param {Object} args argument object
        * @param {String} args.term seach term
        * @returns {Promise} returns promise containing all matching content items
        */
        searchContent: function(args) {

            if (!args.term) {
                throw "args.term is required";
            }

            return entityResource.search(args.term, "Document", args.searchFrom, args.canceler).then(function (data) {
                _.each(data, function (item) {
                    configureContentResult(item);
                });
                return data;
            });
        },

        /**
        * @ngdoc method
        * @name umbraco.services.searchService#searchMedia
        * @methodOf umbraco.services.searchService
        *
        * @description
        * Searches the default media search index
        * @param {Object} args argument object
        * @param {String} args.term seach term
        * @returns {Promise} returns promise containing all matching media items
        */
        searchMedia: function(args) {

            if (!args.term) {
                throw "args.term is required";
            }

            return entityResource.search(args.term, "Media", args.searchFrom).then(function (data) {
                _.each(data, function (item) {
                    configureMediaResult(item);
                });
                return data;
            });
        },

        /**
        * @ngdoc method
        * @name umbraco.services.searchService#searchAll
        * @methodOf umbraco.services.searchService
        *
        * @description
        * Searches all available indexes and returns all results in one collection
        * @param {Object} args argument object
        * @param {String} args.term seach term
        * @returns {Promise} returns promise containing all matching items
        */
        searchAll: function (args) {
            
            if (!args.term) {
                throw "args.term is required";
            }

            return entityResource.searchAll(args.term, args.canceler).then(function (data) {

                _.each(data, function(resultByType) {
                    switch(resultByType.type) {
                        case "Document":
                            _.each(resultByType.results, function (item) {
                                configureContentResult(item);
                            });
                            break;
                        case "Media":
                            _.each(resultByType.results, function (item) {
                                configureMediaResult(item);
                            });                            
                            break;
                        case "Member":
                            _.each(resultByType.results, function (item) {
                                configureMemberResult(item);
                            });                            
                            break;
                    }
                });

                return data;
            });
            
        },

        //TODO: This doesn't do anything!
        setCurrent: function(sectionAlias) {

            var currentSection = sectionAlias;
        }
    };
});
/**
 * @ngdoc service
 * @name umbraco.services.serverValidationManager
 * @function
 *
 * @description
 * Used to handle server side validation and wires up the UI with the messages. There are 2 types of validation messages, one
 * is for user defined properties (called Properties) and the other is for field properties which are attached to the native 
 * model objects (not user defined). The methods below are named according to these rules: Properties vs Fields.
 */
function serverValidationManager($timeout) {

    var callbacks = [];
    
    /** calls the callback specified with the errors specified, used internally */
    function executeCallback(self, errorsForCallback, callback) {

        callback.apply(self, [
                 false,                  //pass in a value indicating it is invalid
                 errorsForCallback,      //pass in the errors for this item
                 self.items]);           //pass in all errors in total
    }

    function getFieldErrors(self, fieldName) {
        if (!angular.isString(fieldName)) {
            throw "fieldName must be a string";
        }

        //find errors for this field name
        return _.filter(self.items, function (item) {
            return (item.propertyAlias === null && item.fieldName === fieldName);
        });
    }
    
    function getPropertyErrors(self, propertyAlias, fieldName) {
        if (!angular.isString(propertyAlias)) {
            throw "propertyAlias must be a string";
        }
        if (fieldName && !angular.isString(fieldName)) {
            throw "fieldName must be a string";
        }

        //find all errors for this property
        return _.filter(self.items, function (item) {            
            return (item.propertyAlias === propertyAlias && (item.fieldName === fieldName || (fieldName === undefined || fieldName === "")));
        });
    }

    return {
        
        /**
         * @ngdoc function
         * @name umbraco.services.serverValidationManager#subscribe
         * @methodOf umbraco.services.serverValidationManager
         * @function
         *
         * @description
         *  This method needs to be called once all field and property errors are wired up. 
         * 
         *  In some scenarios where the error collection needs to be persisted over a route change 
         *   (i.e. when a content item (or any item) is created and the route redirects to the editor) 
         *   the controller should call this method once the data is bound to the scope
         *   so that any persisted validation errors are re-bound to their controls. Once they are re-binded this then clears the validation
         *   colleciton so that if another route change occurs, the previously persisted validation errors are not re-bound to the new item.
         */
        executeAndClearAllSubscriptions: function() {

            var self = this;

            $timeout(function () {
                
                for (var cb in callbacks) {
                    if (callbacks[cb].propertyAlias === null) {
                        //its a field error callback
                        var fieldErrors = getFieldErrors(self, callbacks[cb].fieldName);
                        if (fieldErrors.length > 0) {
                            executeCallback(self, fieldErrors, callbacks[cb].callback);
                        }
                    }
                    else {
                        //its a property error
                        var propErrors = getPropertyErrors(self, callbacks[cb].propertyAlias, callbacks[cb].fieldName);
                        if (propErrors.length > 0) {
                            executeCallback(self, propErrors, callbacks[cb].callback);
                        }
                    }
                }
                //now that they are all executed, we're gonna clear all of the errors we have
                self.clear();
            });
        },

        /**
         * @ngdoc function
         * @name umbraco.services.serverValidationManager#subscribe
         * @methodOf umbraco.services.serverValidationManager
         * @function
         *
         * @description
         *  Adds a callback method that is executed whenever validation changes for the field name + property specified.
         *  This is generally used for server side validation in order to match up a server side validation error with 
         *  a particular field, otherwise we can only pinpoint that there is an error for a content property, not the 
         *  property's specific field. This is used with the val-server directive in which the directive specifies the 
         *  field alias to listen for.
         *  If propertyAlias is null, then this subscription is for a field property (not a user defined property).
         */
        subscribe: function (propertyAlias, fieldName, callback) {
            if (!callback) {
                return;
            }
            
            if (propertyAlias === null) {
                //don't add it if it already exists
                var exists1 = _.find(callbacks, function (item) {
                    return item.propertyAlias === null && item.fieldName === fieldName;
                });
                if (!exists1) {
                    callbacks.push({ propertyAlias: null, fieldName: fieldName, callback: callback });
                }
            }
            else if (propertyAlias !== undefined) {
                //don't add it if it already exists
                var exists2 = _.find(callbacks, function (item) {
                    return item.propertyAlias === propertyAlias && item.fieldName === fieldName;
                });
                if (!exists2) {
                    callbacks.push({ propertyAlias: propertyAlias, fieldName: fieldName, callback: callback });
                }
            }
        },
        
        unsubscribe: function (propertyAlias, fieldName) {
            
            if (propertyAlias === null) {

                //remove all callbacks for the content field
                callbacks = _.reject(callbacks, function (item) {
                    return item.propertyAlias === null && item.fieldName === fieldName;
                });

            }
            else if (propertyAlias !== undefined) {
                
                //remove all callbacks for the content property
                callbacks = _.reject(callbacks, function (item) {
                    return item.propertyAlias === propertyAlias &&
                    (item.fieldName === fieldName ||
                        ((item.fieldName === undefined || item.fieldName === "") && (fieldName === undefined || fieldName === "")));
                });
            }

            
        },
        
        
        /**
         * @ngdoc function
         * @name getPropertyCallbacks
         * @methodOf umbraco.services.serverValidationManager
         * @function
         *
         * @description
         * Gets all callbacks that has been registered using the subscribe method for the propertyAlias + fieldName combo.
         * This will always return any callbacks registered for just the property (i.e. field name is empty) and for ones with an 
         * explicit field name set.
         */
        getPropertyCallbacks: function (propertyAlias, fieldName) {
            var found = _.filter(callbacks, function (item) {
                //returns any callback that have been registered directly against the field and for only the property
                return (item.propertyAlias === propertyAlias && (item.fieldName === fieldName || (item.fieldName === undefined || item.fieldName === "")));
            });
            return found;
        },
        
        /**
         * @ngdoc function
         * @name getFieldCallbacks
         * @methodOf umbraco.services.serverValidationManager
         * @function
         *
         * @description
         * Gets all callbacks that has been registered using the subscribe method for the field.         
         */
        getFieldCallbacks: function (fieldName) {
            var found = _.filter(callbacks, function (item) {
                //returns any callback that have been registered directly against the field
                return (item.propertyAlias === null && item.fieldName === fieldName);
            });
            return found;
        },
        
        /**
         * @ngdoc function
         * @name addFieldError
         * @methodOf umbraco.services.serverValidationManager
         * @function
         *
         * @description
         * Adds an error message for a native content item field (not a user defined property, for Example, 'Name')
         */
        addFieldError: function(fieldName, errorMsg) {
            if (!fieldName) {
                return;
            }
            
            //only add the item if it doesn't exist                
            if (!this.hasFieldError(fieldName)) {
                this.items.push({
                    propertyAlias: null,
                    fieldName: fieldName,
                    errorMsg: errorMsg
                });
            }
            
            //find all errors for this item
            var errorsForCallback = getFieldErrors(this, fieldName);
            //we should now call all of the call backs registered for this error
            var cbs = this.getFieldCallbacks(fieldName);
            //call each callback for this error
            for (var cb in cbs) {
                executeCallback(this, errorsForCallback, cbs[cb].callback);
            }
        },

        /**
         * @ngdoc function
         * @name addPropertyError
         * @methodOf umbraco.services.serverValidationManager
         * @function
         *
         * @description
         * Adds an error message for the content property
         */
        addPropertyError: function (propertyAlias, fieldName, errorMsg) {
            if (!propertyAlias) {
                return;
            }
            
            //only add the item if it doesn't exist                
            if (!this.hasPropertyError(propertyAlias, fieldName)) {
                this.items.push({
                    propertyAlias: propertyAlias,
                    fieldName: fieldName,
                    errorMsg: errorMsg
                });
            }
            
            //find all errors for this item
            var errorsForCallback = getPropertyErrors(this, propertyAlias, fieldName);
            //we should now call all of the call backs registered for this error
            var cbs = this.getPropertyCallbacks(propertyAlias, fieldName);
            //call each callback for this error
            for (var cb in cbs) {
                executeCallback(this, errorsForCallback, cbs[cb].callback);
            }
        },        
        
        /**
         * @ngdoc function
         * @name removePropertyError
         * @methodOf umbraco.services.serverValidationManager
         * @function
         *
         * @description
         * Removes an error message for the content property
         */
        removePropertyError: function (propertyAlias, fieldName) {

            if (!propertyAlias) {
                return;
            }
            //remove the item
            this.items = _.reject(this.items, function (item) {
                return (item.propertyAlias === propertyAlias && (item.fieldName === fieldName || (fieldName === undefined || fieldName === "")));
            });
        },
        
        /**
         * @ngdoc function
         * @name reset
         * @methodOf umbraco.services.serverValidationManager
         * @function
         *
         * @description
         * Clears all errors and notifies all callbacks that all server errros are now valid - used when submitting a form
         */
        reset: function () {
            this.clear();
            for (var cb in callbacks) {
                callbacks[cb].callback.apply(this, [
                        true,       //pass in a value indicating it is VALID
                        [],         //pass in empty collection
                        []]);       //pass in empty collection
            }
        },
        
        /**
         * @ngdoc function
         * @name clear
         * @methodOf umbraco.services.serverValidationManager
         * @function
         *
         * @description
         * Clears all errors
         */
        clear: function() {
            this.items = [];
        },
        
        /**
         * @ngdoc function
         * @name getPropertyError
         * @methodOf umbraco.services.serverValidationManager
         * @function
         *
         * @description
         * Gets the error message for the content property
         */
        getPropertyError: function (propertyAlias, fieldName) {
            var err = _.find(this.items, function (item) {
                //return true if the property alias matches and if an empty field name is specified or the field name matches
                return (item.propertyAlias === propertyAlias && (item.fieldName === fieldName || (fieldName === undefined || fieldName === "")));
            });
            return err;
        },
        
        /**
         * @ngdoc function
         * @name getFieldError
         * @methodOf umbraco.services.serverValidationManager
         * @function
         *
         * @description
         * Gets the error message for a content field
         */
        getFieldError: function (fieldName) {
            var err = _.find(this.items, function (item) {
                //return true if the property alias matches and if an empty field name is specified or the field name matches
                return (item.propertyAlias === null && item.fieldName === fieldName);
            });
            return err;
        },
        
        /**
         * @ngdoc function
         * @name hasPropertyError
         * @methodOf umbraco.services.serverValidationManager
         * @function
         *
         * @description
         * Checks if the content property + field name combo has an error
         */
        hasPropertyError: function (propertyAlias, fieldName) {
            var err = _.find(this.items, function (item) {
                //return true if the property alias matches and if an empty field name is specified or the field name matches
                return (item.propertyAlias === propertyAlias && (item.fieldName === fieldName || (fieldName === undefined || fieldName === "")));
            });
            return err ? true : false;
        },
        
        /**
         * @ngdoc function
         * @name hasFieldError
         * @methodOf umbraco.services.serverValidationManager
         * @function
         *
         * @description
         * Checks if a content field has an error
         */
        hasFieldError: function (fieldName) {
            var err = _.find(this.items, function (item) {
                //return true if the property alias matches and if an empty field name is specified or the field name matches
                return (item.propertyAlias === null && item.fieldName === fieldName);
            });
            return err ? true : false;
        },
        
        /** The array of error messages */
        items: []
    };
}

angular.module('umbraco.services').factory('serverValidationManager', serverValidationManager);
/**
 * @ngdoc service
 * @name umbraco.services.tinyMceService
 *
 *  
 * @description
 * A service containing all logic for all of the Umbraco TinyMCE plugins
 */
function tinyMceService(dialogService, $log, imageHelper, $http, $timeout, macroResource, macroService, $routeParams, umbRequestHelper, angularHelper, userService) {
    return {

        /**
        * @ngdoc method
        * @name umbraco.services.tinyMceService#configuration
        * @methodOf umbraco.services.tinyMceService
        *
        * @description
        * Returns a collection of plugins available to the tinyMCE editor
        *
        */
        configuration: function () {
               return umbRequestHelper.resourcePromise(
                  $http.get(
                      umbRequestHelper.getApiUrl(
                          "rteApiBaseUrl",
                          "GetConfiguration"), { cache: true }),
                  'Failed to retrieve tinymce configuration');
        },

        /**
        * @ngdoc method
        * @name umbraco.services.tinyMceService#defaultPrevalues
        * @methodOf umbraco.services.tinyMceService
        *
        * @description
        * Returns a default configration to fallback on in case none is provided
        *
        */
        defaultPrevalues: function () {
               var cfg = {};
                       cfg.toolbar = ["code", "bold", "italic", "styleselect","alignleft", "aligncenter", "alignright", "bullist","numlist", "outdent", "indent", "link", "image", "umbmediapicker", "umbembeddialog", "umbmacro"];
                       cfg.stylesheets = [];
                       cfg.dimensions = { height: 500 };
                       cfg.maxImageSize = 500;
                return cfg;
        },

        /**
        * @ngdoc method
        * @name umbraco.services.tinyMceService#createInsertEmbeddedMedia
        * @methodOf umbraco.services.tinyMceService
        *
        * @description
        * Creates the umbrco insert embedded media tinymce plugin
        *
        * @param {Object} editor the TinyMCE editor instance        
        * @param {Object} $scope the current controller scope
        */
        createInsertEmbeddedMedia: function (editor, scope, callback) {
            editor.addButton('umbembeddialog', {
                icon: 'custom icon-tv',
                tooltip: 'Embed',
                onclick: function () {
                    if (callback) {
                        callback();
                    }
                }
            });
        },

        insertEmbeddedMediaInEditor: function(editor, preview) {
            editor.insertContent(preview);
        },

        /**
        * @ngdoc method
        * @name umbraco.services.tinyMceService#createMediaPicker
        * @methodOf umbraco.services.tinyMceService
        *
        * @description
        * Creates the umbrco insert media tinymce plugin
        *
        * @param {Object} editor the TinyMCE editor instance        
        * @param {Object} $scope the current controller scope
        */
        createMediaPicker: function (editor, scope, callback) {
            editor.addButton('umbmediapicker', {
                icon: 'custom icon-picture',
                tooltip: 'Media Picker',
                onclick: function () {

                    var selectedElm = editor.selection.getNode(),
                        currentTarget;


                    if(selectedElm.nodeName === 'IMG'){
                        var img = $(selectedElm);
                        currentTarget = {
                            altText: img.attr("alt"),
                            url: img.attr("src"),
                            id: img.attr("rel")
                        };
                    }

                    userService.getCurrentUser().then(function(userData) {
                        if(callback) {
                            callback(currentTarget, userData);
                        }
                    });

                }
            });
        },

        insertMediaInEditor: function(editor, img) {
            if(img) {

               var data = {
                   alt: img.altText || "",
                   src: (img.url) ? img.url : "nothing.jpg",
                   rel: img.id,
                   'data-id': img.id,
                   id: '__mcenew'
               };

               editor.insertContent(editor.dom.createHTML('img', data));

               $timeout(function () {
                   var imgElm = editor.dom.get('__mcenew');
                   var size = editor.dom.getSize(imgElm);

                   if (editor.settings.maxImageSize && editor.settings.maxImageSize !== 0) {
                        var newSize = imageHelper.scaleToMaxSize(editor.settings.maxImageSize, size.w, size.h);

                        var s = "width: " + newSize.width + "px; height:" + newSize.height + "px;";
                        editor.dom.setAttrib(imgElm, 'style', s);
                        editor.dom.setAttrib(imgElm, 'id', null);

                        if (img.url) {
                            var src = img.url + "?width=" + newSize.width + "&height=" + newSize.height;
                            editor.dom.setAttrib(imgElm, 'data-mce-src', src);
                        }
                   }
               }, 500);
            }
        },

        /**
        * @ngdoc method
        * @name umbraco.services.tinyMceService#createUmbracoMacro
        * @methodOf umbraco.services.tinyMceService
        *
        * @description
        * Creates the insert umbrco macro tinymce plugin
        *
        * @param {Object} editor the TinyMCE editor instance      
        * @param {Object} $scope the current controller scope
        */
        createInsertMacro: function (editor, $scope, callback) {

            var createInsertMacroScope = this;

            /** Adds custom rules for the macro plugin and custom serialization */
            editor.on('preInit', function (args) {
                //this is requires so that we tell the serializer that a 'div' is actually allowed in the root, otherwise the cleanup will strip it out
                editor.serializer.addRules('div');

                /** This checks if the div is a macro container, if so, checks if its wrapped in a p tag and then unwraps it (removes p tag) */
                editor.serializer.addNodeFilter('div', function (nodes, name) {
                    for (var i = 0; i < nodes.length; i++) {
                        if (nodes[i].attr("class") === "umb-macro-holder" && nodes[i].parent && nodes[i].parent.name.toUpperCase() === "P") {
                            nodes[i].parent.unwrap();
                        }
                    }
                });
            
            });

            /**
            * Because the macro gets wrapped in a P tag because of the way 'enter' works, this 
            * method will return the macro element if not wrapped in a p, or the p if the macro
            * element is the only one inside of it even if we are deep inside an element inside the macro
            */
            function getRealMacroElem(element) {
                var e = $(element).closest(".umb-macro-holder");
                if (e.length > 0) {
                    if (e.get(0).parentNode.nodeName === "P") {
                        //now check if we're the only element                    
                        if (element.parentNode.childNodes.length === 1) {
                            return e.get(0).parentNode;
                        }
                    }
                    return e.get(0);
                }
                return null;
            }

            /** Adds the button instance */
            editor.addButton('umbmacro', {
                icon: 'custom icon-settings-alt',
                tooltip: 'Insert macro',
                onPostRender: function () {

                    var ctrl = this;
                    var isOnMacroElement = false;

                    /**
                     if the selection comes from a different element that is not the macro's
                     we need to check if the selection includes part of the macro, if so we'll force the selection
                     to clear to the next element since if people can select part of the macro markup they can then modify it.
                    */
                    function handleSelectionChange() {

                        if (!editor.selection.isCollapsed()) {
                            var endSelection = tinymce.activeEditor.selection.getEnd();
                            var startSelection = tinymce.activeEditor.selection.getStart();
                            //don't proceed if it's an entire element selected
                            if (endSelection !== startSelection) { 
                                
                                //if the end selection is a macro then move the cursor
                                //NOTE: we don't have to handle when the selection comes from a previous parent because
                                // that is automatically taken care of with the normal onNodeChanged logic since the 
                                // evt.element will be the macro once it becomes part of the selection.
                                var $testForMacro = $(endSelection).closest(".umb-macro-holder");
                                if ($testForMacro.length > 0) {
                                    
                                    //it came from before so move after, if there is no after then select ourselves
                                    var next = $testForMacro.next();
                                    if (next.length > 0) {
                                        editor.selection.setCursorLocation($testForMacro.next().get(0));
                                    }
                                    else {
                                        selectMacroElement($testForMacro.get(0));
                                    }

                                }
                            }
                        }
                    }

                    /** helper method to select the macro element */
                    function selectMacroElement(macroElement) {

                        // move selection to top element to ensure we can't edit this
                        editor.selection.select(macroElement);

                        // check if the current selection *is* the element (ie bug)
                        var currentSelection = editor.selection.getStart();
                        if (tinymce.isIE) {
                            if (!editor.dom.hasClass(currentSelection, 'umb-macro-holder')) {
                                while (!editor.dom.hasClass(currentSelection, 'umb-macro-holder') && currentSelection.parentNode) {
                                    currentSelection = currentSelection.parentNode;
                                }
                                editor.selection.select(currentSelection);
                            }
                        }
                    }

                    /**
                    * Add a node change handler, test if we're editing a macro and select the whole thing, then set our isOnMacroElement flag.
                    * If we change the selection inside this method, then we end up in an infinite loop, so we have to remove ourselves
                    * from the event listener before changing selection, however, it seems that putting a break point in this method
                    * will always cause an 'infinite' loop as the caret keeps changing.
                    */
                    function onNodeChanged(evt) {

                        //set our macro button active when on a node of class umb-macro-holder
                        var $macroElement = $(evt.element).closest(".umb-macro-holder");
                        
                        handleSelectionChange();

                        //set the button active
                        ctrl.active($macroElement.length !== 0);

                        if ($macroElement.length > 0) {
                            var macroElement = $macroElement.get(0);

                            //remove the event listener before re-selecting
                            editor.off('NodeChange', onNodeChanged);

                            selectMacroElement(macroElement);

                            //set the flag
                            isOnMacroElement = true;

                            //re-add the event listener
                            editor.on('NodeChange', onNodeChanged);
                        }
                        else {
                            isOnMacroElement = false;
                        }

                    }

                    /** when the contents load we need to find any macros declared and load in their content */
                    editor.on("LoadContent", function (o) {
                        
                        //get all macro divs and load their content
                        $(editor.dom.select(".umb-macro-holder.mceNonEditable")).each(function() {
                            createInsertMacroScope.loadMacroContent($(this), null, $scope);
                        });

                    });
                    
                    /** This prevents any other commands from executing when the current element is the macro so the content cannot be edited */
                    editor.on('BeforeExecCommand', function (o) {                        
                        if (isOnMacroElement) {
                            if (o.preventDefault) {
                                o.preventDefault();
                            }
                            if (o.stopImmediatePropagation) {
                                o.stopImmediatePropagation();
                            }
                            return;
                        }
                    });
                    
                    /** This double checks and ensures you can't paste content into the rendered macro */
                    editor.on("Paste", function (o) {                        
                        if (isOnMacroElement) {
                            if (o.preventDefault) {
                                o.preventDefault();
                            }
                            if (o.stopImmediatePropagation) {
                                o.stopImmediatePropagation();
                            }
                            return;
                        }
                    });

                    //set onNodeChanged event listener
                    editor.on('NodeChange', onNodeChanged);

                    /** 
                    * Listen for the keydown in the editor, we'll check if we are currently on a macro element, if so
                    * we'll check if the key down is a supported key which requires an action, otherwise we ignore the request
                    * so the macro cannot be edited.
                    */
                    editor.on('KeyDown', function (e) {
                        if (isOnMacroElement) {
                            var macroElement = editor.selection.getNode();

                            //get the 'real' element (either p or the real one)
                            macroElement = getRealMacroElem(macroElement);

                            //prevent editing
                            e.preventDefault();
                            e.stopPropagation();

                            var moveSibling = function (element, isNext) {
                                var $e = $(element);
                                var $sibling = isNext ? $e.next() : $e.prev();
                                if ($sibling.length > 0) {
                                    editor.selection.select($sibling.get(0));
                                    editor.selection.collapse(true);
                                }
                                else {
                                    //if we're moving previous and there is no sibling, then lets recurse and just select the next one
                                    if (!isNext) {
                                        moveSibling(element, true);
                                        return;
                                    }

                                    //if there is no sibling we'll generate a new p at the end and select it
                                    editor.setContent(editor.getContent() + "<p>&nbsp;</p>");
                                    editor.selection.select($(editor.dom.getRoot()).children().last().get(0));
                                    editor.selection.collapse(true);

                                }
                            };

                            //supported keys to move to the next or prev element (13-enter, 27-esc, 38-up, 40-down, 39-right, 37-left)
                            //supported keys to remove the macro (8-backspace, 46-delete)
                            //TODO: Should we make the enter key insert a line break before or leave it as moving to the next element?
                            if ($.inArray(e.keyCode, [13, 40, 39]) !== -1) {
                                //move to next element
                                moveSibling(macroElement, true);
                            }
                            else if ($.inArray(e.keyCode, [27, 38, 37]) !== -1) {
                                //move to prev element
                                moveSibling(macroElement, false);
                            }
                            else if ($.inArray(e.keyCode, [8, 46]) !== -1) {
                                //delete macro element

                                //move first, then delete
                                moveSibling(macroElement, false);
                                editor.dom.remove(macroElement);
                            }
                            return ;
                        }
                    });

                },
                
                /** The insert macro button click event handler */
                onclick: function () {

                    var dialogData = {
                        //flag for use in rte so we only show macros flagged for the editor
                        richTextEditor: true  
                    };

                    //when we click we could have a macro already selected and in that case we'll want to edit the current parameters
                    //so we'll need to extract them and submit them to the dialog.
                    var macroElement = editor.selection.getNode();                    
                    macroElement = getRealMacroElem(macroElement);
                    if (macroElement) {
                        //we have a macro selected so we'll need to parse it's alias and parameters
                        var contents = $(macroElement).contents();                        
                        var comment = _.find(contents, function(item) {
                            return item.nodeType === 8;
                        });
                        if (!comment) {
                            throw "Cannot parse the current macro, the syntax in the editor is invalid";
                        }
                        var syntax = comment.textContent.trim();
                        var parsed = macroService.parseMacroSyntax(syntax);
                        dialogData = {
                            macroData: parsed  
                        };
                    }

                    if(callback) {
                        callback(dialogData);
                    }

                }
            });
        },

        insertMacroInEditor: function(editor, macroObject, $scope) {

            //put the macro syntax in comments, we will parse this out on the server side to be used
            //for persisting.
            var macroSyntaxComment = "<!-- " + macroObject.syntax + " -->";
            //create an id class for this element so we can re-select it after inserting
            var uniqueId = "umb-macro-" + editor.dom.uniqueId();
            var macroDiv = editor.dom.create('div',
                {
                    'class': 'umb-macro-holder ' + macroObject.macroAlias + ' mceNonEditable ' + uniqueId
                },
                macroSyntaxComment + '<ins>Macro alias: <strong>' + macroObject.macroAlias + '</strong></ins>');

            editor.selection.setNode(macroDiv);

            var $macroDiv = $(editor.dom.select("div.umb-macro-holder." + uniqueId));

            //async load the macro content
            this.loadMacroContent($macroDiv, macroObject, $scope);

        },

        /** loads in the macro content async from the server */
        loadMacroContent: function($macroDiv, macroData, $scope) {

            //if we don't have the macroData, then we'll need to parse it from the macro div
            if (!macroData) {
                var contents = $macroDiv.contents();
                var comment = _.find(contents, function (item) {
                    return item.nodeType === 8;
                });
                if (!comment) {
                    throw "Cannot parse the current macro, the syntax in the editor is invalid";
                }
                var syntax = comment.textContent.trim();
                var parsed = macroService.parseMacroSyntax(syntax);
                macroData = parsed;
            }

            var $ins = $macroDiv.find("ins");

            //show the throbber
            $macroDiv.addClass("loading");

            var contentId = $routeParams.id;

            //need to wrap in safe apply since this might be occuring outside of angular
            angularHelper.safeApply($scope, function() {
                macroResource.getMacroResultAsHtmlForEditor(macroData.macroAlias, contentId, macroData.macroParamsDictionary)
                .then(function (htmlResult) {

                    $macroDiv.removeClass("loading");
                    htmlResult = htmlResult.trim();
                    if (htmlResult !== "") {
                        $ins.html(htmlResult);
                    }
                });
            });

        },

        createLinkPicker: function(editor, $scope, onClick) {

            function createLinkList(callback) {
                return function() {
                    var linkList = editor.settings.link_list;

                    if (typeof(linkList) === "string") {
                        tinymce.util.XHR.send({
                            url: linkList,
                            success: function(text) {
                                callback(tinymce.util.JSON.parse(text));
                            }
                        });
                    } else {
                        callback(linkList);
                    }
                };
            }

            function showDialog(linkList) {
                var data = {}, selection = editor.selection, dom = editor.dom, selectedElm, anchorElm, initialText;
                var win, linkListCtrl, relListCtrl, targetListCtrl;

                function linkListChangeHandler(e) {
                    var textCtrl = win.find('#text');

                    if (!textCtrl.value() || (e.lastControl && textCtrl.value() === e.lastControl.text())) {
                        textCtrl.value(e.control.text());
                    }

                    win.find('#href').value(e.control.value());
                }

                function buildLinkList() {
                    var linkListItems = [{
                        text: 'None',
                        value: ''
                    }];

                    tinymce.each(linkList, function(link) {
                        linkListItems.push({
                            text: link.text || link.title,
                            value: link.value || link.url,
                            menu: link.menu
                        });
                    });

                    return linkListItems;
                }

                function buildRelList(relValue) {
                    var relListItems = [{
                        text: 'None',
                        value: ''
                    }];

                    tinymce.each(editor.settings.rel_list, function(rel) {
                        relListItems.push({
                            text: rel.text || rel.title,
                            value: rel.value,
                            selected: relValue === rel.value
                        });
                    });

                    return relListItems;
                }

                function buildTargetList(targetValue) {
                    var targetListItems = [{
                        text: 'None',
                        value: ''
                    }];

                    if (!editor.settings.target_list) {
                        targetListItems.push({
                            text: 'New window',
                            value: '_blank'
                        });
                    }

                    tinymce.each(editor.settings.target_list, function(target) {
                        targetListItems.push({
                            text: target.text || target.title,
                            value: target.value,
                            selected: targetValue === target.value
                        });
                    });

                    return targetListItems;
                }

                function buildAnchorListControl(url) {
                    var anchorList = [];

                    tinymce.each(editor.dom.select('a:not([href])'), function(anchor) {
                        var id = anchor.name || anchor.id;

                        if (id) {
                            anchorList.push({
                                text: id,
                                value: '#' + id,
                                selected: url.indexOf('#' + id) !== -1
                            });
                        }
                    });

                    if (anchorList.length) {
                        anchorList.unshift({
                            text: 'None',
                            value: ''
                        });

                        return {
                            name: 'anchor',
                            type: 'listbox',
                            label: 'Anchors',
                            values: anchorList,
                            onselect: linkListChangeHandler
                        };
                    }
                }

                function updateText() {
                    if (!initialText && data.text.length === 0) {
                        this.parent().parent().find('#text')[0].value(this.value());
                    }
                }

                selectedElm = selection.getNode();
                anchorElm = dom.getParent(selectedElm, 'a[href]');

                data.text = initialText = anchorElm ? (anchorElm.innerText || anchorElm.textContent) : selection.getContent({format: 'text'});
                data.href = anchorElm ? dom.getAttrib(anchorElm, 'href') : '';
                data.target = anchorElm ? dom.getAttrib(anchorElm, 'target') : '';
                data.rel = anchorElm ? dom.getAttrib(anchorElm, 'rel') : '';

                if (selectedElm.nodeName === "IMG") {
                    data.text = initialText = " ";
                }

                if (linkList) {
                    linkListCtrl = {
                        type: 'listbox',
                        label: 'Link list',
                        values: buildLinkList(),
                        onselect: linkListChangeHandler
                    };
                }

                if (editor.settings.target_list !== false) {
                    targetListCtrl = {
                        name: 'target',
                        type: 'listbox',
                        label: 'Target',
                        values: buildTargetList(data.target)
                    };
                }

                if (editor.settings.rel_list) {
                    relListCtrl = {
                        name: 'rel',
                        type: 'listbox',
                        label: 'Rel',
                        values: buildRelList(data.rel)
                    };
                }

                var injector = angular.element(document.getElementById("umbracoMainPageBody")).injector();
                var dialogService = injector.get("dialogService");
                var currentTarget = null;

                //if we already have a link selected, we want to pass that data over to the dialog
                if(anchorElm){
                    var anchor = $(anchorElm);
                    currentTarget = {
                        name: anchor.attr("title"),
                        url: anchor.attr("href"),
                        target: anchor.attr("target")
                    };

                    //locallink detection, we do this here, to avoid poluting the dialogservice
                    //so the dialog service can just expect to get a node-like structure
                    if(currentTarget.url.indexOf("localLink:") > 0){
                        currentTarget.id = currentTarget.url.substring(currentTarget.url.indexOf(":")+1,currentTarget.url.length-1);
                    }
                }

                if(onClick) {
                    onClick(currentTarget, anchorElm);
                }

            }

            editor.addButton('link', {
                icon: 'link',
                tooltip: 'Insert/edit link',
                shortcut: 'Ctrl+K',
                onclick: createLinkList(showDialog),
                stateSelector: 'a[href]'
            });

            editor.addButton('unlink', {
                icon: 'unlink',
                tooltip: 'Remove link',
                cmd: 'unlink',
                stateSelector: 'a[href]'
            });

            editor.addShortcut('Ctrl+K', '', createLinkList(showDialog));
            this.showDialog = showDialog;

            editor.addMenuItem('link', {
                icon: 'link',
                text: 'Insert link',
                shortcut: 'Ctrl+K',
                onclick: createLinkList(showDialog),
                stateSelector: 'a[href]',
                context: 'insert',
                prependToContext: true
            });

        },

        insertLinkInEditor: function(editor, target, anchorElm) {

            var href = target.url;

            function insertLink() {
                if (anchorElm) {
                    editor.dom.setAttribs(anchorElm, {
                        href: href,
                        title: target.name,
                        target: target.target ? target.target : null,
                        rel: target.rel ? target.rel : null,
                        'data-id': target.id ? target.id : null
                    });

                    editor.selection.select(anchorElm);
                    editor.execCommand('mceEndTyping');
                } else {
                    editor.execCommand('mceInsertLink', false, {
                        href: href,
                        title: target.name,
                        target: target.target ? target.target : null,
                        rel: target.rel ? target.rel : null,
                        'data-id': target.id ? target.id : null
                    });
                }
            }

            if (!href) {
                editor.execCommand('unlink');
                return;
            }

            //if we have an id, it must be a locallink:id, aslong as the isMedia flag is not set
            if(target.id && (angular.isUndefined(target.isMedia) || !target.isMedia)){
                href = "/{localLink:" + target.id + "}";
                insertLink();
                return;
            }

            // Is email and not //user@domain.com
            if (href.indexOf('@') > 0 && href.indexOf('//') === -1 && href.indexOf('mailto:') === -1) {
                href = 'mailto:' + href;
                insertLink();
                return;
            }

            // Is www. prefixed
            if (/^\s*www\./i.test(href)) {
                href = 'http://' + href;
                insertLink();
                return;
            }

            insertLink();

        }

    };
}

angular.module('umbraco.services').factory('tinyMceService', tinyMceService);


/**
 * @ngdoc service
 * @name umbraco.services.treeService
 * @function
 *
 * @description
 * The tree service factory, used internally by the umbTree and umbTreeItem directives
 */
function treeService($q, treeResource, iconHelper, notificationsService, eventsService) {

    //SD: Have looked at putting this in sessionStorage (not localStorage since that means you wouldn't be able to work
    // in multiple tabs) - however our tree structure is cyclical, meaning a node has a reference to it's parent and it's children
    // which you cannot serialize to sessionStorage. There's really no benefit of session storage except that you could refresh
    // a tab and have the trees where they used to be - supposed that is kind of nice but would mean we'd have to store the parent
    // as a nodeid reference instead of a variable with a getParent() method.
    var treeCache = {};
    
    var standardCssClass = 'icon umb-tree-icon sprTree';

    function getCacheKey(args) {
        //if there is no cache key they return null - it won't be cached.
        if (!args || !args.cacheKey) {
            return null;
        }        

        var cacheKey = args.cacheKey;
        cacheKey += "_" + args.section;
        return cacheKey;
    }

    return {  

        /** Internal method to return the tree cache */
        _getTreeCache: function() {
            return treeCache;
        },

        /** Internal method that ensures there's a routePath, parent and level property on each tree node and adds some icon specific properties so that the nodes display properly */
        _formatNodeDataForUseInUI: function (parentNode, treeNodes, section, level) {
            //if no level is set, then we make it 1   
            var childLevel = (level ? level : 1);
            //set the section if it's not already set
            if (!parentNode.section) {
                parentNode.section = section;
            }
            //create a method outside of the loop to return the parent - otherwise jshint blows up
            var funcParent = function() {
                return parentNode;
            };
            for (var i = 0; i < treeNodes.length; i++) {

                treeNodes[i].level = childLevel;

                //create a function to get the parent node, we could assign the parent node but 
                // then we cannot serialize this entity because we have a cyclical reference.
                // Instead we just make a function to return the parentNode.
                treeNodes[i].parent = funcParent;

                //set the section for each tree node - this allows us to reference this easily when accessing tree nodes
                treeNodes[i].section = section;

                //if there is not route path specified, then set it automatically,
                //if this is a tree root node then we want to route to the section's dashboard
                if (!treeNodes[i].routePath) {
                    
                    if (treeNodes[i].metaData && treeNodes[i].metaData["treeAlias"]) {
                        //this is a root node
                        treeNodes[i].routePath = section;                        
                    }
                    else {
                        var treeAlias = this.getTreeAlias(treeNodes[i]);
                        treeNodes[i].routePath = section + "/" + treeAlias + "/edit/" + treeNodes[i].id;
                    }
                }

                //now, format the icon data
                if (treeNodes[i].iconIsClass === undefined || treeNodes[i].iconIsClass) {
                    var converted = iconHelper.convertFromLegacyTreeNodeIcon(treeNodes[i]);
                    treeNodes[i].cssClass = standardCssClass + " " + converted;
                    if (converted.startsWith('.')) {
                        //its legacy so add some width/height
                        treeNodes[i].style = "height:16px;width:16px;";
                    }
                    else {
                        treeNodes[i].style = "";
                    }
                }
                else {
                    treeNodes[i].style = "background-image: url('" + treeNodes[i].iconFilePath + "');";
                    //we need an 'icon-' class in there for certain styles to work so if it is image based we'll add this
                    treeNodes[i].cssClass = standardCssClass + " legacy-custom-file";
                }
            }
        },

        /**
         * @ngdoc method
         * @name umbraco.services.treeService#getTreePackageFolder
         * @methodOf umbraco.services.treeService
         * @function
         *
         * @description
         * Determines if the current tree is a plugin tree and if so returns the package folder it has declared
         * so we know where to find it's views, otherwise it will just return undefined.
         * 
         * @param {String} treeAlias The tree alias to check
         */
        getTreePackageFolder: function(treeAlias) {            
            //we determine this based on the server variables
            if (Umbraco.Sys.ServerVariables.umbracoPlugins &&
                Umbraco.Sys.ServerVariables.umbracoPlugins.trees &&
                angular.isArray(Umbraco.Sys.ServerVariables.umbracoPlugins.trees)) {

                var found = _.find(Umbraco.Sys.ServerVariables.umbracoPlugins.trees, function(item) {
                    return item.alias === treeAlias;
                });
                
                return found ? found.packageFolder : undefined;
            }
            return undefined;
        },

        /**
         * @ngdoc method
         * @name umbraco.services.treeService#clearCache
         * @methodOf umbraco.services.treeService
         * @function
         *
         * @description
         * Clears the tree cache - with optional cacheKey, optional section or optional filter.
         * 
         * @param {Object} args arguments
         * @param {String} args.cacheKey optional cachekey - this is used to clear specific trees in dialogs
         * @param {String} args.section optional section alias - clear tree for a given section
         * @param {String} args.childrenOf optional parent ID - only clear the cache below a specific node
         */
        clearCache: function (args) {
            //clear all if not specified
            if (!args) {
                treeCache = {};
            }
            else {
                //if section and cache key specified just clear that cache
                if (args.section && args.cacheKey) {
                    var cacheKey = getCacheKey(args);
                    if (cacheKey && treeCache && treeCache[cacheKey] != null) {
                        treeCache = _.omit(treeCache, cacheKey);
                    }
                }
                else if (args.childrenOf) {
                    //if childrenOf is supplied a cacheKey must be supplied as well
                    if (!args.cacheKey) {
                        throw "args.cacheKey is required if args.childrenOf is supplied";
                    }
                    //this will clear out all children for the parentId passed in to this parameter, we'll 
                    // do this by recursing and specifying a filter
                    var self = this;
                    this.clearCache({
                        cacheKey: args.cacheKey,
                        filter: function(cc) {
                            //get the new parent node from the tree cache
                            var parent = self.getDescendantNode(cc.root, args.childrenOf);
                            if (parent) {
                                //clear it's children and set to not expanded
                                parent.children = null;
                                parent.expanded = false;
                            }
                            //return the cache to be saved
                            return cc;
                        }
                    });
                }
                else if (args.filter && angular.isFunction(args.filter)) {
                    //if a filter is supplied a cacheKey must be supplied as well
                    if (!args.cacheKey) {
                        throw "args.cacheKey is required if args.filter is supplied";
                    }

                    //if a filter is supplied the function needs to return the data to keep
                    var byKey = treeCache[args.cacheKey];
                    if (byKey) {
                        var result = args.filter(byKey);

                        if (result) {
                            //set the result to the filtered data
                            treeCache[args.cacheKey] = result;
                        }
                        else {                            
                            //remove the cache
                            treeCache = _.omit(treeCache, args.cacheKey);
                        }

                    }

                }
                else if (args.cacheKey) {
                    //if only the cache key is specified, then clear all cache starting with that key
                    var allKeys1 = _.keys(treeCache);
                    var toRemove1 = _.filter(allKeys1, function (k) {
                        return k.startsWith(args.cacheKey + "_");
                    });
                    treeCache = _.omit(treeCache, toRemove1);
                }
                else if (args.section) {
                    //if only the section is specified then clear all cache regardless of cache key by that section
                    var allKeys2 = _.keys(treeCache);
                    var toRemove2 = _.filter(allKeys2, function (k) {
                        return k.endsWith("_" + args.section);
                    });
                    treeCache = _.omit(treeCache, toRemove2);
                }               
            }
        },

        /**
         * @ngdoc method
         * @name umbraco.services.treeService#loadNodeChildren
         * @methodOf umbraco.services.treeService
         * @function
         *
         * @description
         * Clears all node children, gets it's up-to-date children from the server and re-assigns them and then
         * returns them in a promise.
         * @param {object} args An arguments object
         * @param {object} args.node The tree node
         * @param {object} args.section The current section
         */
        loadNodeChildren: function(args) {
            if (!args) {
                throw "No args object defined for loadNodeChildren";
            }
            if (!args.node) {
                throw "No node defined on args object for loadNodeChildren";
            }
            
            this.removeChildNodes(args.node);
            args.node.loading = true;

            return this.getChildren(args)
                .then(function(data) {

                    //set state to done and expand (only if there actually are children!)
                    args.node.loading = false;
                    args.node.children = data;
                    if (args.node.children && args.node.children.length > 0) {
                        args.node.expanded = true;
                        args.node.hasChildren = true;
                    }
                    return data;

                }, function(reason) {

                    //in case of error, emit event
                    eventsService.emit("treeService.treeNodeLoadError", {error: reason } );

                    //stop show the loading indicator  
                    args.node.loading = false;

                    //tell notications about the error
                    notificationsService.error(reason);

                    return reason;
                });

        },

        /**
         * @ngdoc method
         * @name umbraco.services.treeService#removeNode
         * @methodOf umbraco.services.treeService
         * @function
         *
         * @description
         * Removes a given node from the tree
         * @param {object} treeNode the node to remove
         */
        removeNode: function(treeNode) {
            if (!angular.isFunction(treeNode.parent)) {
                return;
            }

            if (treeNode.parent() == null) {
                throw "Cannot remove a node that doesn't have a parent";
            }
            //remove the current item from it's siblings
            treeNode.parent().children.splice(treeNode.parent().children.indexOf(treeNode), 1);            
        },
        
        /**
         * @ngdoc method
         * @name umbraco.services.treeService#removeChildNodes
         * @methodOf umbraco.services.treeService
         * @function
         *
         * @description
         * Removes all child nodes from a given tree node 
         * @param {object} treeNode the node to remove children from
         */
        removeChildNodes : function(treeNode) {
            treeNode.expanded = false;
            treeNode.children = [];
            treeNode.hasChildren = false;
        },

        /**
         * @ngdoc method
         * @name umbraco.services.treeService#getChildNode
         * @methodOf umbraco.services.treeService
         * @function
         *
         * @description
         * Gets a child node with a given ID, from a specific treeNode
         * @param {object} treeNode to retrive child node from
         * @param {int} id id of child node
         */
        getChildNode: function (treeNode, id) {
            if (!treeNode.children) {
                return null;
            }
            var found = _.find(treeNode.children, function (child) {
                return String(child.id).toLowerCase() === String(id).toLowerCase();
            });
            return found === undefined ? null : found;
        },

        /**
         * @ngdoc method
         * @name umbraco.services.treeService#getDescendantNode
         * @methodOf umbraco.services.treeService
         * @function
         *
         * @description
         * Gets a descendant node by id
         * @param {object} treeNode to retrive descendant node from
         * @param {int} id id of descendant node
         * @param {string} treeAlias - optional tree alias, if fetching descendant node from a child of a listview document
         */
        getDescendantNode: function(treeNode, id, treeAlias) {

            //validate if it is a section container since we'll need a treeAlias if it is one
            if (treeNode.isContainer === true && !treeAlias) {
                throw "Cannot get a descendant node from a section container node without a treeAlias specified";
            }

            //if it is a section container, we need to find the tree to be searched
            if (treeNode.isContainer) {
                var foundRoot = null;
                for (var c = 0; c < treeNode.children.length; c++) {
                    if (this.getTreeAlias(treeNode.children[c]) === treeAlias) {
                        foundRoot = treeNode.children[c];
                        break;
                    }
                }
                if (!foundRoot) {
                    throw "Could not find a tree in the current section with alias " + treeAlias;
                }
                treeNode = foundRoot;
            }

            //check this node
            if (treeNode.id === id) {
                return treeNode;
            }

            //check the first level
            var found = this.getChildNode(treeNode, id);
            if (found) {
                return found;
            }
           
            //check each child of this node
            if (!treeNode.children) {
                return null;
            }

            for (var i = 0; i < treeNode.children.length; i++) {
                if (treeNode.children[i].children && angular.isArray(treeNode.children[i].children) && treeNode.children[i].children.length > 0) {
                    //recurse
                    found = this.getDescendantNode(treeNode.children[i], id);
                    if (found) {
                        return found;
                    }
                }
            }
            
            //not found
            return found === undefined ? null : found;
        },

        /**
         * @ngdoc method
         * @name umbraco.services.treeService#getTreeRoot
         * @methodOf umbraco.services.treeService
         * @function
         *
         * @description
         * Gets the root node of the current tree type for a given tree node
         * @param {object} treeNode to retrive tree root node from
         */
        getTreeRoot: function (treeNode) {
            if (!treeNode) {
                throw "treeNode cannot be null";
            }

            //all root nodes have metadata key 'treeAlias'
            var root = null;
            var current = treeNode;            
            while (root === null && current) {
                
                if (current.metaData && current.metaData["treeAlias"]) {
                    root = current;
                }
                else if (angular.isFunction(current.parent)) {
                    //we can only continue if there is a parent() method which means this
                    // tree node was loaded in as part of a real tree, not just as a single tree
                    // node from the server.
                    current = current.parent();
                }
                else {
                    current = null;
                }
            }
            return root;
        },

        /** Gets the node's tree alias, this is done by looking up the meta-data of the current node's root node */
        /**
         * @ngdoc method
         * @name umbraco.services.treeService#getTreeAlias
         * @methodOf umbraco.services.treeService
         * @function
         *
         * @description
         * Gets the node's tree alias, this is done by looking up the meta-data of the current node's root node 
         * @param {object} treeNode to retrive tree alias from
         */
        getTreeAlias : function(treeNode) {
            var root = this.getTreeRoot(treeNode);
            if (root) {
                return root.metaData["treeAlias"];
            }
            return "";
        },

        /**
         * @ngdoc method
         * @name umbraco.services.treeService#getTree
         * @methodOf umbraco.services.treeService
         * @function
         *
         * @description
         * gets the tree, returns a promise 
         * @param {object} args Arguments
         * @param {string} args.section Section alias
         * @param {string} args.cacheKey Optional cachekey
         */
        getTree: function (args) {

            var deferred = $q.defer();

            //set defaults
            if (!args) {
                args = { section: 'content', cacheKey: null };
            }
            else if (!args.section) {
                args.section = 'content';
            }

            var cacheKey = getCacheKey(args);
            
            //return the cache if it exists
            if (cacheKey && treeCache[cacheKey] !== undefined) {
                deferred.resolve(treeCache[cacheKey]);
                return deferred.promise;
            }

            var self = this;
            treeResource.loadApplication(args)
                .then(function(data) {
                    //this will be called once the tree app data has loaded
                    var result = {
                        name: data.name,
                        alias: args.section,
                        root: data
                    };
                    //we need to format/modify some of the node data to be used in our app.
                    self._formatNodeDataForUseInUI(result.root, result.root.children, args.section);

                    //cache this result if a cache key is specified - generally a cache key should ONLY
                    // be specified for application trees, dialog trees should not be cached.
                    if (cacheKey) {                        
                        treeCache[cacheKey] = result;
                        deferred.resolve(treeCache[cacheKey]);
                    }

                    //return un-cached
                    deferred.resolve(result);
                });
            
            return deferred.promise;
        },

        /**
         * @ngdoc method
         * @name umbraco.services.treeService#getMenu
         * @methodOf umbraco.services.treeService
         * @function
         *
         * @description
         * Returns available menu actions for a given tree node
         * @param {object} args Arguments
         * @param {string} args.treeNode tree node object to retrieve the menu for
         */
        getMenu: function (args) {

            if (!args) {
                throw "args cannot be null";
            }
            if (!args.treeNode) {
                throw "args.treeNode cannot be null";
            }

            return treeResource.loadMenu(args.treeNode)
                .then(function(data) {
                    //need to convert the icons to new ones
                    for (var i = 0; i < data.length; i++) {
                        data[i].cssclass = iconHelper.convertFromLegacyIcon(data[i].cssclass);
                    }
                    return data;
                });
        },
        
        /**
         * @ngdoc method
         * @name umbraco.services.treeService#getChildren
         * @methodOf umbraco.services.treeService
         * @function
         *
         * @description
         * Gets the children from the server for a given node 
         * @param {object} args Arguments
         * @param {object} args.node tree node object to retrieve the children for
         * @param {string} args.section current section alias
         */
        getChildren: function (args) {

            if (!args) {
                throw "No args object defined for getChildren";
            }
            if (!args.node) {
                throw "No node defined on args object for getChildren";
            }

            var section = args.section || 'content';
            var treeItem = args.node;

            var self = this;

            return treeResource.loadNodes({ node: treeItem })
                .then(function (data) {
                    //now that we have the data, we need to add the level property to each item and the view
                    self._formatNodeDataForUseInUI(treeItem, data, section, treeItem.level + 1);
                    return data;
                });
        },
        
        /**
         * @ngdoc method
         * @name umbraco.services.treeService#reloadNode
         * @methodOf umbraco.services.treeService
         * @function
         *
         * @description
         * Re-loads the single node from the server
         * @param {object} node Tree node to reload
         */
        reloadNode: function(node) {
            if (!node) {
                throw "node cannot be null";
            }
            if (!node.parent()) {
                throw "cannot reload a single node without a parent";
            }
            if (!node.section) {
                throw "cannot reload a single node without an assigned node.section";
            }
            
            var deferred = $q.defer();
            
            //set the node to loading
            node.loading = true;

            this.getChildren({ node: node.parent(), section: node.section }).then(function(data) {

                //ok, now that we have the children, find the node we're reloading
                var found = _.find(data, function(item) {
                    return item.id === node.id;
                });
                if (found) {
                    //now we need to find the node in the parent.children collection to replace
                    var index = _.indexOf(node.parent().children, node);
                    //the trick here is to not actually replace the node - this would cause the delete animations
                    //to fire, instead we're just going to replace all the properties of this node.

                    //there should always be a method assigned but we'll check anyways
                    if (angular.isFunction(node.parent().children[index].updateNodeData)) {
                        node.parent().children[index].updateNodeData(found);
                    }
                    else {
                        //just update as per normal - this means styles, etc.. won't be applied
                        _.extend(node.parent().children[index], found);
                    }
                    
                    //set the node loading
                    node.parent().children[index].loading = false;
                    //return
                    deferred.resolve(node.parent().children[index]);
                }
                else {
                    deferred.reject();
                }
            }, function() {
                deferred.reject();
            });
            
            return deferred.promise;
        },

        /**
         * @ngdoc method
         * @name umbraco.services.treeService#getPath
         * @methodOf umbraco.services.treeService
         * @function
         *
         * @description
         * This will return the current node's path by walking up the tree 
         * @param {object} node Tree node to retrieve path for
         */
        getPath: function(node) {
            if (!node) {
                throw "node cannot be null";                
            }
            if (!angular.isFunction(node.parent)) {
                throw "node.parent is not a function, the path cannot be resolved";
            }
            //all root nodes have metadata key 'treeAlias'
            var reversePath = [];
            var current = node;
            while (current != null) {
                reversePath.push(current.id);                
                if (current.metaData && current.metaData["treeAlias"]) {
                    current = null;
                }
                else {
                    current = current.parent();
                }
            }
            return reversePath.reverse();
        },

        syncTree: function(args) {
            
            if (!args) {
                throw "No args object defined for syncTree";
            }
            if (!args.node) {
                throw "No node defined on args object for syncTree";
            }
            if (!args.path) {
                throw "No path defined on args object for syncTree";
            }
            if (!angular.isArray(args.path)) {
                throw "Path must be an array";
            }
            if (args.path.length < 1) {
                //if there is no path, make -1 the path, and that should sync the tree root
                args.path.push("-1");
            }

            var deferred = $q.defer();

            //get the rootNode for the current node, we'll sync based on that
            var root = this.getTreeRoot(args.node);
            if (!root) {
                throw "Could not get the root tree node based on the node passed in";
            }
            
            //now we want to loop through the ids in the path, first we'll check if the first part
            //of the path is the root node, otherwise we'll search it's children.
            var currPathIndex = 0;
            //if the first id is the root node and there's only one... then consider it synced
            if (String(args.path[currPathIndex]).toLowerCase() === String(args.node.id).toLowerCase()) {
                if (args.path.length === 1) {
                    //return the root
                    deferred.resolve(root);
                    return deferred.promise;
                }
                else {
                    //move to the next path part and continue
                    currPathIndex = 1;
                }
            }
           
            //now that we have the first id to lookup, we can start the process

            var self = this;
            var node = args.node;

            var doSync = function () {
                //check if it exists in the already loaded children
                var child = self.getChildNode(node, args.path[currPathIndex]);
                if (child) {
                    if (args.path.length === (currPathIndex + 1)) {
                        //woot! synced the node
                        if (!args.forceReload) {
                            deferred.resolve(child);
                        }
                        else {
                            //even though we've found the node if forceReload is specified
                            //we want to go update this single node from the server
                            self.reloadNode(child).then(function (reloaded) {
                                deferred.resolve(reloaded);
                            }, function () {
                                deferred.reject();
                            });
                        }
                    }
                    else {
                        //now we need to recurse with the updated node/currPathIndex
                        currPathIndex++;
                        node = child;
                        //recurse
                        doSync();
                    }
                }
                else {
                    //couldn't find it in the 
                    self.loadNodeChildren({ node: node, section: node.section }).then(function () {
                        //ok, got the children, let's find it
                        var found = self.getChildNode(node, args.path[currPathIndex]);
                        if (found) {
                            if (args.path.length === (currPathIndex + 1)) {
                                //woot! synced the node
                                deferred.resolve(found);
                            }
                            else {
                                //now we need to recurse with the updated node/currPathIndex
                                currPathIndex++;
                                node = found;
                                //recurse
                                doSync();
                            }
                        }
                        else {
                            //fail!
                            deferred.reject();
                        }
                    }, function () {
                        //fail!
                        deferred.reject();
                    });
                }
            };

            //start
            doSync();
            
            return deferred.promise;

        }
        
    };
}

angular.module('umbraco.services').factory('treeService', treeService);
/**
* @ngdoc service
* @name umbraco.services.umbRequestHelper
* @description A helper object used for sending requests to the server
**/
function umbRequestHelper($http, $q, umbDataFormatter, angularHelper, dialogService, notificationsService, eventsService) {
    return {

        /**
         * @ngdoc method
         * @name umbraco.services.umbRequestHelper#convertVirtualToAbsolutePath
         * @methodOf umbraco.services.umbRequestHelper
         * @function
         *
         * @description
         * This will convert a virtual path (i.e. ~/App_Plugins/Blah/Test.html ) to an absolute path
         * 
         * @param {string} a virtual path, if this is already an absolute path it will just be returned, if this is a relative path an exception will be thrown
         */
        convertVirtualToAbsolutePath: function(virtualPath) {
            if (virtualPath.startsWith("/")) {
                return virtualPath;
            }
            if (!virtualPath.startsWith("~/")) {
                throw "The path " + virtualPath + " is not a virtual path";
            }
            if (!Umbraco.Sys.ServerVariables.application.applicationPath) { 
                throw "No applicationPath defined in Umbraco.ServerVariables.application.applicationPath";
            }
            return Umbraco.Sys.ServerVariables.application.applicationPath + virtualPath.trimStart("~/");
        },

        /**
         * @ngdoc method
         * @name umbraco.services.umbRequestHelper#dictionaryToQueryString
         * @methodOf umbraco.services.umbRequestHelper
         * @function
         *
         * @description
         * This will turn an array of key/value pairs into a query string
         * 
         * @param {Array} queryStrings An array of key/value pairs
         */
        dictionaryToQueryString: function (queryStrings) {
            
            if (angular.isArray(queryStrings)) {
                return _.map(queryStrings, function (item) {
                    var key = null;
                    var val = null;
                    for (var k in item) {
                        key = k;
                        val = item[k];
                        break;
                    }
                    if (key === null || val === null) {
                        throw "The object in the array was not formatted as a key/value pair";
                    }
                    return encodeURIComponent(key) + "=" + encodeURIComponent(val);
                }).join("&");
            }
            else if (angular.isObject(queryStrings)) {

                //this allows for a normal object to be passed in (ie. a dictionary)
                return decodeURIComponent($.param(queryStrings));
            }
            
            throw "The queryString parameter is not an array or object of key value pairs";
        },

        /**
         * @ngdoc method
         * @name umbraco.services.umbRequestHelper#getApiUrl
         * @methodOf umbraco.services.umbRequestHelper
         * @function
         *
         * @description
         * This will return the webapi Url for the requested key based on the servervariables collection
         * 
         * @param {string} apiName The webapi name that is found in the servervariables["umbracoUrls"] dictionary
         * @param {string} actionName The webapi action name 
         * @param {object} queryStrings Can be either a string or an array containing key/value pairs
         */
        getApiUrl: function (apiName, actionName, queryStrings) {
            if (!Umbraco || !Umbraco.Sys || !Umbraco.Sys.ServerVariables || !Umbraco.Sys.ServerVariables["umbracoUrls"]) {
                throw "No server variables defined!";
            }

            if (!Umbraco.Sys.ServerVariables["umbracoUrls"][apiName]) {
                throw "No url found for api name " + apiName;
            }

            return Umbraco.Sys.ServerVariables["umbracoUrls"][apiName] + actionName +
                (!queryStrings ? "" : "?" + (angular.isString(queryStrings) ? queryStrings : this.dictionaryToQueryString(queryStrings)));

        },

        /**
         * @ngdoc function
         * @name umbraco.services.umbRequestHelper#resourcePromise
         * @methodOf umbraco.services.umbRequestHelper
         * @function
         *
         * @description
         * This returns a promise with an underlying http call, it is a helper method to reduce
         *  the amount of duplicate code needed to query http resources and automatically handle any 
         *  500 Http server errors. 
         *
         * @param {object} opts A mixed object which can either be a `string` representing the error message to be
         *   returned OR an `object` containing either:
         *     { success: successCallback, errorMsg: errorMessage }
         *          OR
         *     { success: successCallback, error: errorCallback }
         *   In both of the above, the successCallback must accept these parameters: `data`, `status`, `headers`, `config`
         *   If using the errorCallback it must accept these parameters: `data`, `status`, `headers`, `config`
         *   The success callback must return the data which will be resolved by the deferred object.
         *   The error callback must return an object containing: {errorMsg: errorMessage, data: originalData, status: status }
         */
        resourcePromise: function (httpPromise, opts) {
            var deferred = $q.defer();

            /** The default success callback used if one is not supplied in the opts */
            function defaultSuccess(data, status, headers, config) {
                //when it's successful, just return the data
                return data;
            }

            /** The default error callback used if one is not supplied in the opts */
            function defaultError(data, status, headers, config) {
                return {
                    //NOTE: the default error message here should never be used based on the above docs!
                    errorMsg: (angular.isString(opts) ? opts : 'An error occurred!'),
                    data: data,
                    status: status
                };
            }

            //create the callbacs based on whats been passed in.
            var callbacks = {
                success: ((!opts || !opts.success) ? defaultSuccess : opts.success),
                error: ((!opts || !opts.error) ? defaultError : opts.error)
            };

            httpPromise.success(function (data, status, headers, config) {

                //invoke the callback 
                var result = callbacks.success.apply(this, [data, status, headers, config]);

                //when it's successful, just return the data
                deferred.resolve(result);

            }).error(function (data, status, headers, config) {

                //invoke the callback
                var result = callbacks.error.apply(this, [data, status, headers, config]);

                //when there's a 500 (unhandled) error show a YSOD overlay if debugging is enabled.
                if (status >= 500 && status < 600) {

                    //show a ysod dialog
                    if (Umbraco.Sys.ServerVariables["isDebuggingEnabled"] === true) {
                        eventsService.emit('app.ysod',
                        {
                            errorMsg: 'An error occured',
                            data: data
                        });
                    }
                    else {
                        //show a simple error notification                         
                        notificationsService.error("Server error", "Contact administrator, see log for full details.<br/><i>" + result.errorMsg + "</i>");
                    }
                    
                }

                //return an error object including the error message for UI
                deferred.reject({
                    errorMsg: result.errorMsg,
                    data: result.data,
                    status: result.status
                });


            });

            return deferred.promise;

        },

        /** Used for saving media/content specifically */
        postSaveContent: function (args) {

            if (!args.restApiUrl) {
                throw "args.restApiUrl is a required argument";
            }
            if (!args.content) {
                throw "args.content is a required argument";
            }
            if (!args.action) {
                throw "args.action is a required argument";
            }
            if (!args.files) {
                throw "args.files is a required argument";
            }
            if (!args.dataFormatter) {
                throw "args.dataFormatter is a required argument";
            }


            var deferred = $q.defer();

            //save the active tab id so we can set it when the data is returned.
            var activeTab = _.find(args.content.tabs, function (item) {
                return item.active;
            });
            var activeTabIndex = (activeTab === undefined ? 0 : _.indexOf(args.content.tabs, activeTab));

            //save the data
            this.postMultiPartRequest(
                args.restApiUrl,
                { key: "contentItem", value: args.dataFormatter(args.content, args.action) },
                function (data, formData) {
                    //now add all of the assigned files
                    for (var f in args.files) {
                        //each item has a property alias and the file object, we'll ensure that the alias is suffixed to the key
                        // so we know which property it belongs to on the server side
                        formData.append("file_" + args.files[f].alias, args.files[f].file);
                    }

                },
                function (data, status, headers, config) {
                    //success callback

                    //reset the tabs and set the active one
                    _.each(data.tabs, function (item) {
                        item.active = false;
                    });
                    data.tabs[activeTabIndex].active = true;

                    //the data returned is the up-to-date data so the UI will refresh
                    deferred.resolve(data);
                },
                function (data, status, headers, config) {
                    //failure callback

                    //when there's a 500 (unhandled) error show a YSOD overlay if debugging is enabled.
                    if (status >= 500 && status < 600) {

                        //This is a bit of a hack to check if the error is due to a file being uploaded that is too large,
                        // we have to just check for the existence of a string value but currently that is the best way to
                        // do this since it's very hacky/difficult to catch this on the server
                        if (typeof data !== "undefined" && typeof data.indexOf === "function" && data.indexOf("Maximum request length exceeded") >= 0) {
                            notificationsService.error("Server error", "The uploaded file was too large, check with your site administrator to adjust the maximum size allowed");
                        }                        
                        else if (Umbraco.Sys.ServerVariables["isDebuggingEnabled"] === true) {
                            //show a ysod dialog
                            eventsService.emit('app.ysod',
                            {
                                errorMsg: 'An error occured',
                                data: data
                            });
                        }
                        else {
                            //show a simple error notification                         
                            notificationsService.error("Server error", "Contact administrator, see log for full details.<br/><i>" + data.ExceptionMessage + "</i>");
                        }
                        
                    }
                    
                    //return an error object including the error message for UI
                    deferred.reject({
                        errorMsg: 'An error occurred',
                        data: data,
                        status: status
                    });
                   

                });

            return deferred.promise;
        },

        /** Posts a multi-part mime request to the server */
        postMultiPartRequest: function (url, jsonData, transformCallback, successCallback, failureCallback) {

            //validate input, jsonData can be an array of key/value pairs or just one key/value pair.
            if (!jsonData) { throw "jsonData cannot be null"; }

            if (angular.isArray(jsonData)) {
                _.each(jsonData, function (item) {
                    if (!item.key || !item.value) { throw "jsonData array item must have both a key and a value property"; }
                });
            }
            else if (!jsonData.key || !jsonData.value) { throw "jsonData object must have both a key and a value property"; }


            $http({
                method: 'POST',
                url: url,
                //IMPORTANT!!! You might think this should be set to 'multipart/form-data' but this is not true because when we are sending up files
                // the request needs to include a 'boundary' parameter which identifies the boundary name between parts in this multi-part request
                // and setting the Content-type manually will not set this boundary parameter. For whatever reason, setting the Content-type to 'false'
                // will force the request to automatically populate the headers properly including the boundary parameter.
                headers: { 'Content-Type': false },
                transformRequest: function (data) {
                    var formData = new FormData();
                    //add the json data
                    if (angular.isArray(data)) {
                        _.each(data, function (item) {
                            formData.append(item.key, !angular.isString(item.value) ? angular.toJson(item.value) : item.value);
                        });
                    }
                    else {
                        formData.append(data.key, !angular.isString(data.value) ? angular.toJson(data.value) : data.value);
                    }

                    //call the callback
                    if (transformCallback) {
                        transformCallback.apply(this, [data, formData]);
                    }

                    return formData;
                },
                data: jsonData
            }).
            success(function (data, status, headers, config) {
                if (successCallback) {
                    successCallback.apply(this, [data, status, headers, config]);
                }
            }).
            error(function (data, status, headers, config) {
                if (failureCallback) {
                    failureCallback.apply(this, [data, status, headers, config]);
                }
            });
        }
    };
}
angular.module('umbraco.services').factory('umbRequestHelper', umbRequestHelper);

angular.module('umbraco.services')
    .factory('userService', function ($rootScope, eventsService, $q, $location, $log, securityRetryQueue, authResource, dialogService, $timeout, angularHelper, $http) {

        var currentUser = null;
        var lastUserId = null;
        var loginDialog = null;
        //this tracks the last date/time that the user's remainingAuthSeconds was updated from the server
        // this is used so that we know when to go and get the user's remaining seconds directly.
        var lastServerTimeoutSet = null;

        function openLoginDialog(isTimedOut) {
            if (!loginDialog) {
                loginDialog = dialogService.open({

                    //very special flag which means that global events cannot close this dialog
                    manualClose: true,

                    template: 'views/common/dialogs/login.html',
                    modalClass: "login-overlay",
                    animation: "slide",
                    show: true,
                    callback: onLoginDialogClose,
                    dialogData: {
                        isTimedOut: isTimedOut
                    }
                });
            }
        }

        function onLoginDialogClose(success) {
            loginDialog = null;

            if (success) {
                securityRetryQueue.retryAll(currentUser.name);
            }
            else {
                securityRetryQueue.cancelAll();
                $location.path('/');
            }
        }

        /**
        This methods will set the current user when it is resolved and
        will then start the counter to count in-memory how many seconds they have
        remaining on the auth session
        */
        function setCurrentUser(usr) {
            if (!usr.remainingAuthSeconds) {
                throw "The user object is invalid, the remainingAuthSeconds is required.";
            }
            currentUser = usr;
            lastServerTimeoutSet = new Date();
            //start the timer
            countdownUserTimeout();
        }

        /**
        Method to count down the current user's timeout seconds,
        this will continually count down their current remaining seconds every 5 seconds until
        there are no more seconds remaining.
        */
        function countdownUserTimeout() {

            $timeout(function () {

                if (currentUser) {
                    //countdown by 5 seconds since that is how long our timer is for.
                    currentUser.remainingAuthSeconds -= 5;

                    //if there are more than 30 remaining seconds, recurse!
                    if (currentUser.remainingAuthSeconds > 30) {

                        //we need to check when the last time the timeout was set from the server, if
                        // it has been more than 30 seconds then we'll manually go and retrieve it from the
                        // server - this helps to keep our local countdown in check with the true timeout.
                        if (lastServerTimeoutSet != null) {
                            var now = new Date();
                            var seconds = (now.getTime() - lastServerTimeoutSet.getTime()) / 1000;

                            if (seconds > 30) {

                                //first we'll set the lastServerTimeoutSet to null - this is so we don't get back in to this loop while we
                                // wait for a response from the server otherwise we'll be making double/triple/etc... calls while we wait.
                                lastServerTimeoutSet = null;

                                //now go get it from the server
                                //NOTE: the safeApply because our timeout is set to not run digests (performance reasons)
                                angularHelper.safeApply($rootScope, function () {
                                    authResource.getRemainingTimeoutSeconds().then(function (result) {
                                        setUserTimeoutInternal(result);
                                    });
                                });
                            }
                        }

                        //recurse the countdown!
                        countdownUserTimeout();
                    }
                    else {

                        //we are either timed out or very close to timing out so we need to show the login dialog.
                        if (Umbraco.Sys.ServerVariables.umbracoSettings.keepUserLoggedIn !== true) {
                            //NOTE: the safeApply because our timeout is set to not run digests (performance reasons)
                            angularHelper.safeApply($rootScope, function () {
                                try {
                                    //NOTE: We are calling this again so that the server can create a log that the timeout has expired, we
                                    // don't actually care about this result.
                                    authResource.getRemainingTimeoutSeconds();
                                }
                                finally {
                                    userAuthExpired();
                                }
                            });
                        }
                        else {
                            //we've got less than 30 seconds remaining so let's check the server

                            if (lastServerTimeoutSet != null) {
                                //first we'll set the lastServerTimeoutSet to null - this is so we don't get back in to this loop while we
                                // wait for a response from the server otherwise we'll be making double/triple/etc... calls while we wait.
                                lastServerTimeoutSet = null;

                                //now go get it from the server
                                //NOTE: the safeApply because our timeout is set to not run digests (performance reasons)
                                angularHelper.safeApply($rootScope, function () {
                                    authResource.getRemainingTimeoutSeconds().then(function (result) {
                                        setUserTimeoutInternal(result);
                                    });
                                });
                            }

                            //recurse the countdown!
                            countdownUserTimeout();

                        }
                    }
                }
            }, 5000, //every 5 seconds
                false); //false = do NOT execute a digest for every iteration
        }

        /** Called to update the current user's timeout */
        function setUserTimeoutInternal(newTimeout) {


            var asNumber = parseFloat(newTimeout);
            if (!isNaN(asNumber) && currentUser && angular.isNumber(asNumber)) {
                currentUser.remainingAuthSeconds = newTimeout;
                lastServerTimeoutSet = new Date();
            }
        }

        /** resets all user data, broadcasts the notAuthenticated event and shows the login dialog */
        function userAuthExpired(isLogout) {
            //store the last user id and clear the user
            if (currentUser && currentUser.id !== undefined) {
                lastUserId = currentUser.id;
            }

            if (currentUser) {
                currentUser.remainingAuthSeconds = 0;
            }

            lastServerTimeoutSet = null;
            currentUser = null;

            //broadcast a global event that the user is no longer logged in
            eventsService.emit("app.notAuthenticated");

            openLoginDialog(isLogout === undefined ? true : !isLogout);
        }

        // Register a handler for when an item is added to the retry queue
        securityRetryQueue.onItemAddedCallbacks.push(function (retryItem) {
            if (securityRetryQueue.hasMore()) {
                userAuthExpired();
            }
        });

        return {

            /** Internal method to display the login dialog */
            _showLoginDialog: function () {
                openLoginDialog();
            },

            /** Returns a promise, sends a request to the server to check if the current cookie is authorized  */
            isAuthenticated: function () {
                //if we've got a current user then just return true
                if (currentUser) {
                    var deferred = $q.defer();
                    deferred.resolve(true);
                    return deferred.promise;
                }
                return authResource.isAuthenticated();
            },

            /** Returns a promise, sends a request to the server to validate the credentials  */
            authenticate: function (login, password) {

                return authResource.performLogin(login, password)
                    .then(function (data) {

                        //when it's successful, return the user data
                        setCurrentUser(data);

                        var result = { user: data, authenticated: true, lastUserId: lastUserId };

                        //broadcast a global event
                        eventsService.emit("app.authenticated", result);
                        return result;
                    });
            },

            /** Logs the user out
             */
            logout: function () {

                return authResource.performLogout()
                    .then(function(data) {
                        userAuthExpired();
                        //done!
                        return null;
                    });
            },

            /** Returns the current user object in a promise  */
            getCurrentUser: function (args) {
                var deferred = $q.defer();

                if (!currentUser) {
                    authResource.getCurrentUser()
                        .then(function (data) {

                            var result = { user: data, authenticated: true, lastUserId: lastUserId };

                            //TODO: This is a mega backwards compatibility hack... These variables SHOULD NOT exist in the server variables
                            // since they are not supposed to be dynamic but I accidentally added them there in 7.1.5 IIRC so some people might
                            // now be relying on this :(
                            if (Umbraco && Umbraco.Sys && Umbraco.Sys.ServerVariables) {
                                Umbraco.Sys.ServerVariables["security"] = {
                                    startContentId: data.startContentId,
                                    startMediaId: data.startMediaId
                                };
                            }

                            if (args && args.broadcastEvent) {
                                //broadcast a global event, will inform listening controllers to load in the user specific data
                                eventsService.emit("app.authenticated", result);
                            }

                            setCurrentUser(data);

                            deferred.resolve(currentUser);
                        });

                }
                else {
                    deferred.resolve(currentUser);
                }

                return deferred.promise;
            },

            /** Called whenever a server request is made that contains a x-umb-user-seconds response header for which we can update the user's remaining timeout seconds */
            setUserTimeout: function (newTimeout) {
                setUserTimeoutInternal(newTimeout);
            }
        };

    });

/*Contains multiple services for various helper tasks */
function versionHelper() {

    return {

        //see: https://gist.github.com/TheDistantSea/8021359
        versionCompare: function(v1, v2, options) {
            var lexicographical = options && options.lexicographical,
                zeroExtend = options && options.zeroExtend,
                v1parts = v1.split('.'),
                v2parts = v2.split('.');

            function isValidPart(x) {
                return (lexicographical ? /^\d+[A-Za-z]*$/ : /^\d+$/).test(x);
            }

            if (!v1parts.every(isValidPart) || !v2parts.every(isValidPart)) {
                return NaN;
            }

            if (zeroExtend) {
                while (v1parts.length < v2parts.length) {
                    v1parts.push("0");
                }
                while (v2parts.length < v1parts.length) {
                    v2parts.push("0");
                }
            }

            if (!lexicographical) {
                v1parts = v1parts.map(Number);
                v2parts = v2parts.map(Number);
            }

            for (var i = 0; i < v1parts.length; ++i) {
                if (v2parts.length === i) {
                    return 1;
                }

                if (v1parts[i] === v2parts[i]) {
                    continue;
                }
                else if (v1parts[i] > v2parts[i]) {
                    return 1;
                }
                else {
                    return -1;
                }
            }

            if (v1parts.length !== v2parts.length) {
                return -1;
            }

            return 0;
        }
    };
}
angular.module('umbraco.services').factory('versionHelper', versionHelper);

function dateHelper() {

    return {
        
        convertToServerStringTime: function(momentLocal, serverOffsetMinutes, format) {

            //get the formatted offset time in HH:mm (server time offset is in minutes)
            var formattedOffset = (serverOffsetMinutes > 0 ? "+" : "-") +
                moment()
                .startOf('day')
                .minutes(Math.abs(serverOffsetMinutes))
                .format('HH:mm');

            var server = moment.utc(momentLocal).utcOffset(formattedOffset);
            return server.format(format ? format : "YYYY-MM-DD HH:mm:ss");
        },

        convertToLocalMomentTime: function (strVal, serverOffsetMinutes) {

            //get the formatted offset time in HH:mm (server time offset is in minutes)
            var formattedOffset = (serverOffsetMinutes > 0 ? "+" : "-") +
                moment()
                .startOf('day')
                .minutes(Math.abs(serverOffsetMinutes))
                .format('HH:mm');

            //convert to the iso string format
            var isoFormat = moment(strVal).format("YYYY-MM-DDTHH:mm:ss") + formattedOffset;

            //create a moment with the iso format which will include the offset with the correct time
            // then convert it to local time
            return moment.parseZone(isoFormat).local();
        }

    };
}
angular.module('umbraco.services').factory('dateHelper', dateHelper);

function packageHelper(assetsService, treeService, eventsService, $templateCache) {

    return {

        /** Called when a package is installed, this resets a bunch of data and ensures the new package assets are loaded in */
        packageInstalled: function () {

            //clears the tree
            treeService.clearCache();

            //clears the template cache
            $templateCache.removeAll();

            //emit event to notify anything else
            eventsService.emit("app.reInitialize");
        }

    };
}
angular.module('umbraco.services').factory('packageHelper', packageHelper);

//TODO: I believe this is obsolete
function umbPhotoFolderHelper($compile, $log, $timeout, $filter, imageHelper, mediaHelper, umbRequestHelper) {
    return {
        /** sets the image's url, thumbnail and if its a folder */
        setImageData: function(img) {
            
            img.isFolder = !mediaHelper.hasFilePropertyType(img);

            if(!img.isFolder){
                img.thumbnail = mediaHelper.resolveFile(img, true);
                img.image = mediaHelper.resolveFile(img, false);    
            }
        },

        /** sets the images original size properties - will check if it is a folder and if so will just make it square */
        setOriginalSize: function(img, maxHeight) {
            //set to a square by default
            img.originalWidth = maxHeight;
            img.originalHeight = maxHeight;

            var widthProp = _.find(img.properties, function(v) { return (v.alias === "umbracoWidth"); });
            if (widthProp && widthProp.value) {
                img.originalWidth = parseInt(widthProp.value, 10);
                if (isNaN(img.originalWidth)) {
                    img.originalWidth = maxHeight;
                }
            }
            var heightProp = _.find(img.properties, function(v) { return (v.alias === "umbracoHeight"); });
            if (heightProp && heightProp.value) {
                img.originalHeight = parseInt(heightProp.value, 10);
                if (isNaN(img.originalHeight)) {
                    img.originalHeight = maxHeight;
                }
            }
        },

        /** sets the image style which get's used in the angular markup */
        setImageStyle: function(img, width, height, rightMargin, bottomMargin) {
            img.style = { width: width + "px", height: height + "px", "margin-right": rightMargin + "px", "margin-bottom": bottomMargin + "px" };
            img.thumbStyle = {
                "background-image": "url('" + img.thumbnail + "')",
                "background-repeat": "no-repeat",
                "background-position": "center",
                "background-size": Math.min(width, img.originalWidth) + "px " + Math.min(height, img.originalHeight) + "px"
            };
        }, 

        /** gets the image's scaled wdith based on the max row height */
        getScaledWidth: function(img, maxHeight) {
            var scaled = img.originalWidth * maxHeight / img.originalHeight;
            return scaled;
            //round down, we don't want it too big even by half a pixel otherwise it'll drop to the next row
            //return Math.floor(scaled);
        },

        /** returns the target row width taking into account how many images will be in the row and removing what the margin is */
        getTargetWidth: function(imgsPerRow, maxRowWidth, margin) {
            //take into account the margin, we will have 1 less margin item than we have total images
            return (maxRowWidth - ((imgsPerRow - 1) * margin));
        },

        /** 
            This will determine the row/image height for the next collection of images which takes into account the 
            ideal image count per row. It will check if a row can be filled with this ideal count and if not - if there
            are additional images available to fill the row it will keep calculating until they fit.

            It will return the calculated height and the number of images for the row.

            targetHeight = optional;
        */
        getRowHeightForImages: function(imgs, maxRowHeight, minDisplayHeight, maxRowWidth, idealImgPerRow, margin, targetHeight) {

            var idealImages = imgs.slice(0, idealImgPerRow);
            //get the target row width without margin
            var targetRowWidth = this.getTargetWidth(idealImages.length, maxRowWidth, margin);
            //this gets the image with the smallest height which equals the maximum we can scale up for this image block
            var maxScaleableHeight = this.getMaxScaleableHeight(idealImages, maxRowHeight);
            //if the max scale height is smaller than the min display height, we'll use the min display height
            targetHeight =  targetHeight !== undefined ? targetHeight : Math.max(maxScaleableHeight, minDisplayHeight);
            
            var attemptedRowHeight = this.performGetRowHeight(idealImages, targetRowWidth, minDisplayHeight, targetHeight);

            if (attemptedRowHeight != null) {

                //if this is smaller than the min display then we need to use the min display,
                // which means we'll need to remove one from the row so we can scale up to fill the row
                if (attemptedRowHeight < minDisplayHeight) {

                    if (idealImages.length > 1) {
                        
                        //we'll generate a new targetHeight that is halfway between the max and the current and recurse, passing in a new targetHeight                        
                        targetHeight += Math.floor((maxRowHeight - targetHeight) / 2);
                        return this.getRowHeightForImages(imgs, maxRowHeight, minDisplayHeight, maxRowWidth, idealImgPerRow - 1, margin, targetHeight);
                    }
                    else {                        
                        //this will occur when we only have one image remaining in the row but it's still going to be too wide even when 
                        // using the minimum display height specified. In this case we're going to have to just crop the image in it's center
                        // using the minimum display height and the full row width
                        return { height: minDisplayHeight, imgCount: 1 };
                    }
                }
                else {
                    //success!
                    return { height: attemptedRowHeight, imgCount: idealImages.length };
                }
            }

            //we know the width will fit in a row, but we now need to figure out if we can fill 
            // the entire row in the case that we have more images remaining than the idealImgPerRow.

            if (idealImages.length === imgs.length) {
                //we have no more remaining images to fill the space, so we'll just use the calc height
                return { height: targetHeight, imgCount: idealImages.length };
            }
            else if (idealImages.length === 1) {
                //this will occur when we only have one image remaining in the row to process but it's not really going to fit ideally
                // in the row. 
                return { height: minDisplayHeight, imgCount: 1 };
            }
            else if (idealImages.length === idealImgPerRow && targetHeight < maxRowHeight) {

                //if we're already dealing with the ideal images per row and it's not quite wide enough, we can scale up a little bit so 
                // long as the targetHeight is currently less than the maxRowHeight. The scale up will be half-way between our current
                // target height and the maxRowHeight (we won't loop forever though - if there's a difference of 5 px we'll just quit)
                
                while (targetHeight < maxRowHeight && (maxRowHeight - targetHeight) > 5) {
                    targetHeight += Math.floor((maxRowHeight - targetHeight) / 2);
                    attemptedRowHeight = this.performGetRowHeight(idealImages, targetRowWidth, minDisplayHeight, targetHeight);
                    if (attemptedRowHeight != null) {
                        //success!
                        return { height: attemptedRowHeight, imgCount: idealImages.length };
                    }
                }

                //Ok, we couldn't actually scale it up with the ideal row count we'll just recurse with a lesser image count.
                return this.getRowHeightForImages(imgs, maxRowHeight, minDisplayHeight, maxRowWidth, idealImgPerRow - 1, margin);
            }
            else if (targetHeight === maxRowHeight) {

                //This is going to happen when:
                // * We can fit a list of images in a row, but they come up too short (based on minDisplayHeight)
                // * Then we'll try to remove an image, but when we try to scale to fit, the width comes up too narrow but the images are already at their
                //      maximum height (maxRowHeight)
                // * So we're stuck, we cannot precicely fit the current list of images, so we'll render a row that will be max height but won't be wide enough
                //      which is better than rendering a row that is shorter than the minimum since that could be quite small.

                return { height: targetHeight, imgCount: idealImages.length };
            }
            else {

                //we have additional images so we'll recurse and add 1 to the idealImgPerRow until it fits
                return this.getRowHeightForImages(imgs, maxRowHeight, minDisplayHeight, maxRowWidth, idealImgPerRow + 1, margin);
            }

        },

        performGetRowHeight: function(idealImages, targetRowWidth, minDisplayHeight, targetHeight) {

            var currRowWidth = 0;

            for (var i = 0; i < idealImages.length; i++) {
                var scaledW = this.getScaledWidth(idealImages[i], targetHeight);
                currRowWidth += scaledW;
            }

            if (currRowWidth > targetRowWidth) {
                //get the new scaled height to fit
                var newHeight = targetRowWidth * targetHeight / currRowWidth;
                
                return newHeight;
            }
            else if (idealImages.length === 1 && (currRowWidth <= targetRowWidth) && !idealImages[0].isFolder) {
                //if there is only one image, then return the target height
                return targetHeight;
            }
            else if (currRowWidth / targetRowWidth > 0.90) {
                //it's close enough, it's at least 90% of the width so we'll accept it with the target height
                return targetHeight;
            }
            else {
                //if it's not successful, return null
                return null;
            }
        },

        /** builds an image grid row */
        buildRow: function(imgs, maxRowHeight, minDisplayHeight, maxRowWidth, idealImgPerRow, margin, totalRemaining) {
            var currRowWidth = 0;
            var row = { images: [] };

            var imageRowHeight = this.getRowHeightForImages(imgs, maxRowHeight, minDisplayHeight, maxRowWidth, idealImgPerRow, margin);
            var targetWidth = this.getTargetWidth(imageRowHeight.imgCount, maxRowWidth, margin);

            var sizes = [];
            //loop through the images we know fit into the height
            for (var i = 0; i < imageRowHeight.imgCount; i++) {
                //get the lower width to ensure it always fits
                var scaledWidth = Math.floor(this.getScaledWidth(imgs[i], imageRowHeight.height));
                
                if (currRowWidth + scaledWidth <= targetWidth) {
                    currRowWidth += scaledWidth;                    
                    sizes.push({
                        width:scaledWidth,
                        //ensure that the height is rounded
                        height: Math.round(imageRowHeight.height)
                    });
                    row.images.push(imgs[i]);
                }
                else if (imageRowHeight.imgCount === 1 && row.images.length === 0) {
                    //the image is simply too wide, we'll crop/center it
                    sizes.push({
                        width: maxRowWidth,
                        //ensure that the height is rounded
                        height: Math.round(imageRowHeight.height)
                    });
                    row.images.push(imgs[i]);
                }
                else {
                    //the max width has been reached
                    break;
                }
            }

            //loop through the images for the row and apply the styles
            for (var j = 0; j < row.images.length; j++) {
                var bottomMargin = margin;
                //make the margin 0 for the last one
                if (j === (row.images.length - 1)) {
                    margin = 0;
                }
                this.setImageStyle(row.images[j], sizes[j].width, sizes[j].height, margin, bottomMargin);
            }

            if (row.images.length === 1 && totalRemaining > 1) {
                //if there's only one image on the row and there are more images remaining, set the container to max width
                row.images[0].style.width = maxRowWidth + "px"; 
            }
            

            return row;
        },

        /** Returns the maximum image scaling height for the current image collection */
        getMaxScaleableHeight: function(imgs, maxRowHeight) {

            var smallestHeight = _.min(imgs, function(item) { return item.originalHeight; }).originalHeight;

            //adjust the smallestHeight if it is larger than the static max row height
            if (smallestHeight > maxRowHeight) {
                smallestHeight = maxRowHeight;
            }
            return smallestHeight;
        },

        /** Creates the image grid with calculated widths/heights for images to fill the grid nicely */
        buildGrid: function(images, maxRowWidth, maxRowHeight, startingIndex, minDisplayHeight, idealImgPerRow, margin,imagesOnly) {

            var rows = [];
            var imagesProcessed = 0; 

            //first fill in all of the original image sizes and URLs
            for (var i = startingIndex; i < images.length; i++) {
                var item = images[i];

                this.setImageData(item);
                this.setOriginalSize(item, maxRowHeight);

                if(imagesOnly && !item.isFolder && !item.thumbnail){
                    images.splice(i, 1);
                    i--;
                }
            }

            while ((imagesProcessed + startingIndex) < images.length) {
                //get the maxHeight for the current un-processed images
                var currImgs = images.slice(imagesProcessed);

                //build the row
                var remaining = images.length - imagesProcessed;
                var row = this.buildRow(currImgs, maxRowHeight, minDisplayHeight, maxRowWidth, idealImgPerRow, margin, remaining);
                if (row.images.length > 0) {
                    rows.push(row);
                    imagesProcessed += row.images.length;
                }
                else {

                    if (currImgs.length > 0) {
                        throw "Could not fill grid with all images, images remaining: " + currImgs.length;
                    }

                    //if there was nothing processed, exit
                    break;
                }
            }

            return rows;
        }
    };
}
angular.module("umbraco.services").factory("umbPhotoFolderHelper", umbPhotoFolderHelper);

/**
 * @ngdoc function
 * @name umbraco.services.umbModelMapper
 * @function
 *
 * @description
 * Utility class to map/convert models
 */
function umbModelMapper() {

    return {


        /**
         * @ngdoc function
         * @name umbraco.services.umbModelMapper#convertToEntityBasic
         * @methodOf umbraco.services.umbModelMapper
         * @function
         *
         * @description
         * Converts the source model to a basic entity model, it will throw an exception if there isn't enough data to create the model.
         * @param {Object} source The source model
         * @param {Number} source.id The node id of the model
         * @param {String} source.name The node name
         * @param {String} source.icon The models icon as a css class (.icon-doc)
         * @param {Number} source.parentId The parentID, if no parent, set to -1
         * @param {path} source.path comma-separated string of ancestor IDs (-1,1234,1782,1234)
         */

        /** This converts the source model to a basic entity model, it will throw an exception if there isn't enough data to create the model */
        convertToEntityBasic: function (source) {
            var required = ["id", "name", "icon", "parentId", "path"];            
            _.each(required, function (k) {
                if (!_.has(source, k)) {
                    throw "The source object does not contain the property " + k;
                }
            });
            var optional = ["metaData", "key", "alias"];
            //now get the basic object
            var result = _.pick(source, required.concat(optional));
            return result;
        }

    };
}
angular.module('umbraco.services').factory('umbModelMapper', umbModelMapper);

/**
 * @ngdoc function
 * @name umbraco.services.umbSessionStorage
 * @function
 *
 * @description
 * Used to get/set things in browser sessionStorage but always prefixes keys with "umb_" and converts json vals so there is no overlap 
 * with any sessionStorage created by a developer.
 */
function umbSessionStorage($window) {

    //gets the sessionStorage object if available, otherwise just uses a normal object
    // - required for unit tests.
    var storage = $window['sessionStorage'] ? $window['sessionStorage'] : {};

    return {

        get: function (key) {
            return angular.fromJson(storage["umb_" + key]);
        },
        
        set : function(key, value) {
            storage["umb_" + key] = angular.toJson(value);
        }
        
    };
}
angular.module('umbraco.services').factory('umbSessionStorage', umbSessionStorage);

/**
 * @ngdoc function
 * @name umbraco.services.updateChecker
 * @function
 *
 * @description
 * used to check for updates and display a notifcation
 */
function updateChecker($http, umbRequestHelper) {
    return {
        
         /**
          * @ngdoc function
          * @name umbraco.services.updateChecker#check
          * @methodOf umbraco.services.updateChecker
          * @function
          *
          * @description
          * Called to load in the legacy tree js which is required on startup if a user is logged in or 
          * after login, but cannot be called until they are authenticated which is why it needs to be lazy loaded. 
          */
         check: function() {
                
            return umbRequestHelper.resourcePromise(
               $http.get(
                   umbRequestHelper.getApiUrl(
                       "updateCheckApiBaseUrl",
                       "GetCheck")),
               'Failed to retrieve update status');
        }  
    };
}
angular.module('umbraco.services').factory('updateChecker', updateChecker);

/**
* @ngdoc service
* @name umbraco.services.umbPropertyEditorHelper
* @description A helper object used for property editors
**/
function umbPropEditorHelper() {
    return {
        /**
         * @ngdoc function
         * @name getImagePropertyValue
         * @methodOf umbraco.services.umbPropertyEditorHelper
         * @function    
         *
         * @description
         * Returns the correct view path for a property editor, it will detect if it is a full virtual path but if not then default to the internal umbraco one
         * 
         * @param {string} input the view path currently stored for the property editor
         */
        getViewPath: function(input, isPreValue) {
            var path = String(input);

            if (path.startsWith('/')) {

                //This is an absolute path, so just leave it
                return path;
            } else {

                if (path.indexOf("/") >= 0) {
                    //This is a relative path, so just leave it
                    return path;
                } else {
                    if (!isPreValue) {
                        //i.e. views/propertyeditors/fileupload/fileupload.html
                        return "views/propertyeditors/" + path + "/" + path + ".html";
                    } else {
                        //i.e. views/prevalueeditors/requiredfield.html
                        return "views/prevalueeditors/" + path + ".html";
                    }
                }

            }
        }
    };
}
angular.module('umbraco.services').factory('umbPropEditorHelper', umbPropEditorHelper);


/**
* @ngdoc service
* @name umbraco.services.umbDataFormatter
* @description A helper object used to format/transform JSON Umbraco data, mostly used for persisting data to the server
**/
function umbDataFormatter() {
    return {
        
        formatContentTypePostData: function (displayModel, action) {

            //create the save model from the display model
            var saveModel = _.pick(displayModel,
                'compositeContentTypes', 'isContainer', 'allowAsRoot', 'allowedTemplates', 'allowedContentTypes',
                'alias', 'description', 'thumbnail', 'name', 'id', 'icon', 'trashed',
                'key', 'parentId', 'alias', 'path');

            //TODO: Map these
            saveModel.allowedTemplates = _.map(displayModel.allowedTemplates, function (t) { return t.alias; });
            saveModel.defaultTemplate = displayModel.defaultTemplate ? displayModel.defaultTemplate.alias : null;
            var realGroups = _.reject(displayModel.groups, function(g) {
                //do not include these tabs
                return g.tabState === "init";
            });
            saveModel.groups = _.map(realGroups, function (g) {

                var saveGroup = _.pick(g, 'inherited', 'id', 'sortOrder', 'name');

                var realProperties = _.reject(g.properties, function (p) {
                    //do not include these properties
                    return p.propertyState === "init" || p.inherited === true;
                });

                var saveProperties = _.map(realProperties, function (p) {
                    var saveProperty = _.pick(p, 'id', 'alias', 'description', 'validation', 'label', 'sortOrder', 'dataTypeId', 'groupId', 'memberCanEdit', 'showOnMemberProfile');
                    return saveProperty;
                });

                saveGroup.properties = saveProperties;

                //if this is an inherited group and there are not non-inherited properties on it, then don't send up the data
                if (saveGroup.inherited === true && saveProperties.length === 0) {
                    return null;
                }

                return saveGroup;
            });
            
            //we don't want any null groups
            saveModel.groups = _.reject(saveModel.groups, function(g) {
                return !g;
            });

            return saveModel;
        },

        /** formats the display model used to display the data type to the model used to save the data type */
        formatDataTypePostData: function(displayModel, preValues, action) {
            var saveModel = {
                parentId: displayModel.parentId,
                id: displayModel.id,
                name: displayModel.name,
                selectedEditor: displayModel.selectedEditor,
                //set the action on the save model
                action: action,
                preValues: []
            };
            for (var i = 0; i < preValues.length; i++) {

                saveModel.preValues.push({
                    key: preValues[i].alias,
                    value: preValues[i].value
                });
            }
            return saveModel;
        },

        /** formats the display model used to display the member to the model used to save the member */
        formatMemberPostData: function(displayModel, action) {
            //this is basically the same as for media but we need to explicitly add the username,email, password to the save model

            var saveModel = this.formatMediaPostData(displayModel, action);

            saveModel.key = displayModel.key;
            
            var genericTab = _.find(displayModel.tabs, function (item) {
                return item.id === 0;
            });

            //map the member login, email, password and groups
            var propLogin = _.find(genericTab.properties, function (item) {
                return item.alias === "_umb_login";
            });
            var propEmail = _.find(genericTab.properties, function (item) {
                return item.alias === "_umb_email";
            });
            var propPass = _.find(genericTab.properties, function (item) {
                return item.alias === "_umb_password";
            });
            var propGroups = _.find(genericTab.properties, function (item) {
                return item.alias === "_umb_membergroup";
            });
            saveModel.email = propEmail.value;
            saveModel.username = propLogin.value;
            saveModel.password = propPass.value;
            
            var selectedGroups = [];
            for (var n in propGroups.value) {
                if (propGroups.value[n] === true) {
                    selectedGroups.push(n);
                }
            }
            saveModel.memberGroups = selectedGroups;
            
            //turn the dictionary into an array of pairs
            var memberProviderPropAliases = _.pairs(displayModel.fieldConfig);
            _.each(displayModel.tabs, function (tab) {
                _.each(tab.properties, function (prop) {
                    var foundAlias = _.find(memberProviderPropAliases, function(item) {
                        return prop.alias === item[1];
                    });
                    if (foundAlias) {
                        //we know the current property matches an alias, now we need to determine which membership provider property it was for
                        // by looking at the key
                        switch (foundAlias[0]) {
                            case "umbracoMemberLockedOut":
                                saveModel.isLockedOut = prop.value.toString() === "1" ? true : false;
                                break;
                            case "umbracoMemberApproved":
                                saveModel.isApproved = prop.value.toString() === "1" ? true : false;
                                break;
                            case "umbracoMemberComments":
                                saveModel.comments = prop.value;
                                break;
                        }
                    }                
                });
            });



            return saveModel;
        },

        /** formats the display model used to display the media to the model used to save the media */
        formatMediaPostData: function(displayModel, action) {
            //NOTE: the display model inherits from the save model so we can in theory just post up the display model but 
            // we don't want to post all of the data as it is unecessary.
            var saveModel = {
                id: displayModel.id,
                properties: [],
                name: displayModel.name,
                contentTypeAlias: displayModel.contentTypeAlias,
                parentId: displayModel.parentId,
                //set the action on the save model
                action: action
            };

            _.each(displayModel.tabs, function (tab) {

                _.each(tab.properties, function (prop) {

                    //don't include the custom generic tab properties
                    if (!prop.alias.startsWith("_umb_")) {
                        saveModel.properties.push({
                            id: prop.id,
                            alias: prop.alias,
                            value: prop.value
                        });
                    }
                    
                });
            });

            return saveModel;
        },

        /** formats the display model used to display the content to the model used to save the content  */
        formatContentPostData: function (displayModel, action) {

            //this is basically the same as for media but we need to explicitly add some extra properties
            var saveModel = this.formatMediaPostData(displayModel, action);

            var genericTab = _.find(displayModel.tabs, function (item) {
                return item.id === 0;
            });
            
            var propExpireDate = _.find(genericTab.properties, function(item) {
                return item.alias === "_umb_expiredate";
            });
            var propReleaseDate = _.find(genericTab.properties, function (item) {
                return item.alias === "_umb_releasedate";
            });
            var propTemplate = _.find(genericTab.properties, function (item) {
                return item.alias === "_umb_template";
            });
            saveModel.expireDate = propExpireDate.value;
            saveModel.releaseDate = propReleaseDate.value;
            saveModel.templateAlias = propTemplate.value;

            return saveModel;
        }
    };
}
angular.module('umbraco.services').factory('umbDataFormatter', umbDataFormatter);



/**
 * @ngdoc service
 * @name umbraco.services.windowResizeListener
 * @function
 *
 * @description
 * A single window resize listener... we don't want to have more than one in theory to ensure that
 * there aren't too many events raised. This will debounce the event with 100 ms intervals and force
 * a $rootScope.$apply when changed and notify all listeners
 *
 */
function windowResizeListener($rootScope) {

    var WinReszier = (function () {
        var registered = [];
        var inited = false;        
        var resize = _.debounce(function(ev) {
            notify();
        }, 100);
        var notify = function () {
            var h = $(window).height();
            var w = $(window).width();
            //execute all registrations inside of a digest
            $rootScope.$apply(function() {
                for (var i = 0, cnt = registered.length; i < cnt; i++) {
                    registered[i].apply($(window), [{ width: w, height: h }]);
                }
            });
        };
        return {
            register: function (fn) {
                registered.push(fn);
                if (inited === false) {
                    $(window).bind('resize', resize);
                    inited = true;
                }
            },
            unregister: function (fn) {
                var index = registered.indexOf(fn);
                if (index > -1) {
                    registered.splice(index, 1);
                }
            }
        };
    }());

    return {

        /**
         * Register a callback for resizing
         * @param {Function} cb 
         */
        register: function (cb) {
            WinReszier.register(cb);
        },

        /**
         * Removes a registered callback
         * @param {Function} cb 
         */
        unregister: function(cb) {
            WinReszier.unregister(cb);
        }

    };
}
angular.module('umbraco.services').factory('windowResizeListener', windowResizeListener);
/**
 * @ngdoc service
 * @name umbraco.services.xmlhelper
 * @function
 *
 * @description
 * Used to convert legacy xml data to json and back again
 */
function xmlhelper($http) {
    /*
     Copyright 2011 Abdulla Abdurakhmanov
     Original sources are available at https://code.google.com/p/x2js/

     Licensed under the Apache License, Version 2.0 (the "License");
     you may not use this file except in compliance with the License.
     You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

     Unless required by applicable law or agreed to in writing, software
     distributed under the License is distributed on an "AS IS" BASIS,
     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     See the License for the specific language governing permissions and
     limitations under the License.
     */

    function X2JS() {
        var VERSION = "1.0.11";
        var escapeMode = false;

        var DOMNodeTypes = {
            ELEMENT_NODE: 1,
            TEXT_NODE: 3,
            CDATA_SECTION_NODE: 4,
            DOCUMENT_NODE: 9
        };

        function getNodeLocalName(node) {
            var nodeLocalName = node.localName;
            if (nodeLocalName == null) {
                nodeLocalName = node.baseName;
            } // Yeah, this is IE!! 

            if (nodeLocalName === null || nodeLocalName === "") {
                nodeLocalName = node.nodeName;
            } // =="" is IE too

            return nodeLocalName;
        }

        function getNodePrefix(node) {
            return node.prefix;
        }

        function escapeXmlChars(str) {
            if (typeof (str) === "string") {
                return str.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/'/g, '&#x27;').replace(/\//g, '&#x2F;');
            } else {
                return str;
            }
        }

        function unescapeXmlChars(str) {
            return str.replace(/&amp;/g, '&').replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace(/&quot;/g, '"').replace(/&#x27;/g, "'").replace(/&#x2F;/g, '\/');
        }

        function parseDOMChildren(node) {
            var result, child, childName;

            if (node.nodeType === DOMNodeTypes.DOCUMENT_NODE) {
                result = {};
                child = node.firstChild;
                childName = getNodeLocalName(child);
                result[childName] = parseDOMChildren(child);
                return result;
            }
            else {

                if (node.nodeType === DOMNodeTypes.ELEMENT_NODE) {
                    result = {};
                    result.__cnt = 0;
                    var nodeChildren = node.childNodes;

                    // Children nodes
                    for (var cidx = 0; cidx < nodeChildren.length; cidx++) {
                        child = nodeChildren.item(cidx); // nodeChildren[cidx];
                        childName = getNodeLocalName(child);

                        result.__cnt++;
                        if (result[childName] === null) {
                            result[childName] = parseDOMChildren(child);
                            result[childName + "_asArray"] = new Array(1);
                            result[childName + "_asArray"][0] = result[childName];
                        }
                        else {
                            if (result[childName] !== null) {
                                if (!(result[childName] instanceof Array)) {
                                    var tmpObj = result[childName];
                                    result[childName] = [];
                                    result[childName][0] = tmpObj;

                                    result[childName + "_asArray"] = result[childName];
                                }
                            }
                            var aridx = 0;
                            while (result[childName][aridx] !== null) {
                                aridx++;
                            }

                            (result[childName])[aridx] = parseDOMChildren(child);
                        }
                    }

                    // Attributes
                    for (var aidx = 0; aidx < node.attributes.length; aidx++) {
                        var attr = node.attributes.item(aidx); // [aidx];
                        result.__cnt++;
                        result["_" + attr.name] = attr.value;
                    }

                    // Node namespace prefix
                    var nodePrefix = getNodePrefix(node);
                    if (nodePrefix !== null && nodePrefix !== "") {
                        result.__cnt++;
                        result.__prefix = nodePrefix;
                    }

                    if (result.__cnt === 1 && result["#text"] !== null) {
                        result = result["#text"];
                    }

                    if (result["#text"] !== null) {
                        result.__text = result["#text"];
                        if (escapeMode) {
                            result.__text = unescapeXmlChars(result.__text);
                        }

                        delete result["#text"];
                        delete result["#text_asArray"];
                    }
                    if (result["#cdata-section"] != null) {
                        result.__cdata = result["#cdata-section"];
                        delete result["#cdata-section"];
                        delete result["#cdata-section_asArray"];
                    }

                    if (result.__text != null || result.__cdata != null) {
                        result.toString = function () {
                            return (this.__text != null ? this.__text : '') + (this.__cdata != null ? this.__cdata : '');
                        };
                    }
                    return result;
                }
                else {
                    if (node.nodeType === DOMNodeTypes.TEXT_NODE || node.nodeType === DOMNodeTypes.CDATA_SECTION_NODE) {
                        return node.nodeValue;
                    }
                }
            }
        }

        function startTag(jsonObj, element, attrList, closed) {
            var resultStr = "<" + ((jsonObj != null && jsonObj.__prefix != null) ? (jsonObj.__prefix + ":") : "") + element;
            if (attrList != null) {
                for (var aidx = 0; aidx < attrList.length; aidx++) {
                    var attrName = attrList[aidx];
                    var attrVal = jsonObj[attrName];
                    resultStr += " " + attrName.substr(1) + "='" + attrVal + "'";
                }
            }
            if (!closed) {
                resultStr += ">";
            } else {
                resultStr += "/>";
            }

            return resultStr;
        }

        function endTag(jsonObj, elementName) {
            return "</" + (jsonObj.__prefix !== null ? (jsonObj.__prefix + ":") : "") + elementName + ">";
        }

        function endsWith(str, suffix) {
            return str.indexOf(suffix, str.length - suffix.length) !== -1;
        }

        function jsonXmlSpecialElem(jsonObj, jsonObjField) {
            if (endsWith(jsonObjField.toString(), ("_asArray")) || jsonObjField.toString().indexOf("_") === 0 || (jsonObj[jsonObjField] instanceof Function)) {
                return true;
            } else {
                return false;
            }
        }

        function jsonXmlElemCount(jsonObj) {
            var elementsCnt = 0;
            if (jsonObj instanceof Object) {
                for (var it in jsonObj) {
                    if (jsonXmlSpecialElem(jsonObj, it)) {
                        continue;
                    }
                    elementsCnt++;
                }
            }
            return elementsCnt;
        }

        function parseJSONAttributes(jsonObj) {
            var attrList = [];
            if (jsonObj instanceof Object) {
                for (var ait in jsonObj) {
                    if (ait.toString().indexOf("__") === -1 && ait.toString().indexOf("_") === 0) {
                        attrList.push(ait);
                    }
                }
            }

            return attrList;
        }

        function parseJSONTextAttrs(jsonTxtObj) {
            var result = "";

            if (jsonTxtObj.__cdata != null) {
                result += "<![CDATA[" + jsonTxtObj.__cdata + "]]>";
            }

            if (jsonTxtObj.__text != null) {
                if (escapeMode) {
                    result += escapeXmlChars(jsonTxtObj.__text);
                } else {
                    result += jsonTxtObj.__text;
                }
            }
            return result;
        }

        function parseJSONTextObject(jsonTxtObj) {
            var result = "";

            if (jsonTxtObj instanceof Object) {
                result += parseJSONTextAttrs(jsonTxtObj);
            }
            else {
                if (jsonTxtObj != null) {
                    if (escapeMode) {
                        result += escapeXmlChars(jsonTxtObj);
                    } else {
                        result += jsonTxtObj;
                    }
                }
            }


            return result;
        }

        function parseJSONArray(jsonArrRoot, jsonArrObj, attrList) {
            var result = "";
            if (jsonArrRoot.length === 0) {
                result += startTag(jsonArrRoot, jsonArrObj, attrList, true);
            }
            else {
                for (var arIdx = 0; arIdx < jsonArrRoot.length; arIdx++) {
                    result += startTag(jsonArrRoot[arIdx], jsonArrObj, parseJSONAttributes(jsonArrRoot[arIdx]), false);
                    result += parseJSONObject(jsonArrRoot[arIdx]);
                    result += endTag(jsonArrRoot[arIdx], jsonArrObj);
                }
            }
            return result;
        }

        function parseJSONObject(jsonObj) {
            var result = "";

            var elementsCnt = jsonXmlElemCount(jsonObj);

            if (elementsCnt > 0) {
                for (var it in jsonObj) {
                    if (jsonXmlSpecialElem(jsonObj, it)) {
                        continue;
                    }

                    var subObj = jsonObj[it];
                    var attrList = parseJSONAttributes(subObj);

                    if (subObj === null || subObj === undefined) {
                        result += startTag(subObj, it, attrList, true);
                    } else {
                        if (subObj instanceof Object) {

                            if (subObj instanceof Array) {
                                result += parseJSONArray(subObj, it, attrList);
                            } else {
                                var subObjElementsCnt = jsonXmlElemCount(subObj);
                                if (subObjElementsCnt > 0 || subObj.__text !== null || subObj.__cdata !== null) {
                                    result += startTag(subObj, it, attrList, false);
                                    result += parseJSONObject(subObj);
                                    result += endTag(subObj, it);
                                } else {
                                    result += startTag(subObj, it, attrList, true);
                                }
                            }

                        } else {
                            result += startTag(subObj, it, attrList, false);
                            result += parseJSONTextObject(subObj);
                            result += endTag(subObj, it);
                        }
                    }
                }
            }
            result += parseJSONTextObject(jsonObj);

            return result;
        }

        this.parseXmlString = function (xmlDocStr) {
            var xmlDoc;
            if (window.DOMParser) {
                var parser = new window.DOMParser();
                xmlDoc = parser.parseFromString(xmlDocStr, "text/xml");
            }
            else {
                // IE :(
                if (xmlDocStr.indexOf("<?") === 0) {
                    xmlDocStr = xmlDocStr.substr(xmlDocStr.indexOf("?>") + 2);
                }
                xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
                xmlDoc.async = "false";
                xmlDoc.loadXML(xmlDocStr);
            }
            return xmlDoc;
        };

        this.xml2json = function (xmlDoc) {
            return parseDOMChildren(xmlDoc);
        };

        this.xml_str2json = function (xmlDocStr) {
            var xmlDoc = this.parseXmlString(xmlDocStr);
            return this.xml2json(xmlDoc);
        };

        this.json2xml_str = function (jsonObj) {
            return parseJSONObject(jsonObj);
        };

        this.json2xml = function (jsonObj) {
            var xmlDocStr = this.json2xml_str(jsonObj);
            return this.parseXmlString(xmlDocStr);
        };

        this.getVersion = function () {
            return VERSION;
        };

        this.escapeMode = function (enabled) {
            escapeMode = enabled;
        };
    }

    var x2js = new X2JS();
    return {
        /** Called to load in the legacy tree js which is required on startup if a user is logged in or 
         after login, but cannot be called until they are authenticated which is why it needs to be lazy loaded. */
        toJson: function (xml) {
            var json = x2js.xml_str2json(xml);
            return json;
        },
        fromJson: function (json) {
            var xml = x2js.json2xml_str(json);
            return xml;
        }
    };
}
angular.module('umbraco.services').factory('xmlhelper', xmlhelper);

})();