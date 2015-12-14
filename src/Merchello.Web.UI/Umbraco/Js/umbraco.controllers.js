/*! umbraco
 * https://github.com/umbraco/umbraco-cms/
 * Copyright (c) 2015 Umbraco HQ;
 * Licensed MIT
 */

(function() { 


/**
 * @ngdoc controller
 * @name Umbraco.MainController
 * @function
 * 
 * @description
 * The main application controller
 * 
 */
function MainController($scope, $rootScope, $location, $routeParams, $timeout, $http, $log, appState, treeService, notificationsService, userService, navigationService, historyService, updateChecker, assetsService, eventsService, umbRequestHelper, tmhDynamicLocale) {

    //the null is important because we do an explicit bool check on this in the view
    //the avatar is by default the umbraco logo    
    $scope.authenticated = null;
    $scope.avatar = [
        { value: "assets/img/application/logo.png" },
        { value: "assets/img/application/logo@2x.png" },
        { value: "assets/img/application/logo@3x.png" }
    ];
    $scope.touchDevice = appState.getGlobalState("touchDevice");
    

    $scope.removeNotification = function (index) {
        notificationsService.remove(index);
    };

    $scope.closeDialogs = function (event) {
        //only close dialogs if non-link and non-buttons are clicked
        var el = event.target.nodeName;
        var els = ["INPUT","A","BUTTON"];

        if(els.indexOf(el) >= 0){return;}

        var parents = $(event.target).parents("a,button");
        if(parents.length > 0){
            return;
        }

        //SD: I've updated this so that we don't close the dialog when clicking inside of the dialog
        var nav = $(event.target).parents("#dialog");
        if (nav.length === 1) {
            return;
        }

        eventsService.emit("app.closeDialogs", event);
    };

    var evts = [];

    //when a user logs out or timesout
    evts.push(eventsService.on("app.notAuthenticated", function() {
        $scope.authenticated = null;
        $scope.user = null;
    }));
    
    //when the app is read/user is logged in, setup the data
    evts.push(eventsService.on("app.ready", function (evt, data) {
        
        $scope.authenticated = data.authenticated;
        $scope.user = data.user;

        updateChecker.check().then(function(update){
            if(update && update !== "null"){
                if(update.type !== "None"){
                    var notification = {
                           headline: "Update available",
                           message: "Click to download",
                           sticky: true,
                           type: "info",
                           url: update.url
                    };
                    notificationsService.add(notification);
                }
            }
        })

        //if the user has changed we need to redirect to the root so they don't try to continue editing the
        //last item in the URL (NOTE: the user id can equal zero, so we cannot just do !data.lastUserId since that will resolve to true)
        if (data.lastUserId !== undefined && data.lastUserId !== null && data.lastUserId !== data.user.id) {
            $location.path("/").search("");
            historyService.removeAll();
            treeService.clearCache();
        }

        //Load locale file
        if ($scope.user.locale) {
            tmhDynamicLocale.set($scope.user.locale);
        }

        if ($scope.user.emailHash) {

            //let's attempt to load the avatar, it might not exist or we might not have 
            // internet access so we'll detect it first
            $http.get("https://www.gravatar.com/avatar/" + $scope.user.emailHash + ".jpg?s=64&d=404")
                .then(
                    function successCallback(response) {                        
                        $("#avatar-img").fadeTo(1000, 0, function () {
                            $scope.$apply(function () {
                                //this can be null if they time out
                                if ($scope.user && $scope.user.emailHash) {
	                            var avatarBaseUrl = "https://www.gravatar.com/avatar/",
	                                hash = $scope.user.emailHash;

	                            $scope.avatar = [
	                                { value: avatarBaseUrl + hash + ".jpg?s=30&d=mm" },
	                                { value: avatarBaseUrl + hash + ".jpg?s=60&d=mm" },
	                                { value: avatarBaseUrl + hash + ".jpg?s=90&d=mm" }
	                            ];
                                }
                            });
                            $("#avatar-img").fadeTo(1000, 1);
                        });
                    }, function errorCallback(response) {
                        //cannot load it from the server so we cannot do anything
                    });
        }

    }));

    //ensure to unregister from all events!
    $scope.$on('$destroy', function () {
        for (var e in evts) {
            eventsService.unsubscribe(evts[e]);
        }
    });

}


//register it
angular.module('umbraco').controller("Umbraco.MainController", MainController).
        config(function (tmhDynamicLocaleProvider) {
            //Set url for locale files
            tmhDynamicLocaleProvider.localeLocationPattern('lib/angular/1.1.5/i18n/angular-locale_{{locale}}.js');
        });

/**
 * @ngdoc controller
 * @name Umbraco.NavigationController
 * @function
 *
 * @description
 * Handles the section area of the app
 *
 * @param {navigationService} navigationService A reference to the navigationService
 */
function NavigationController($scope, $rootScope, $location, $log, $routeParams, $timeout, appState, navigationService, keyboardService, dialogService, historyService, eventsService, sectionResource, angularHelper) {

    //TODO: Need to think about this and an nicer way to acheive what this is doing.
    //the tree event handler i used to subscribe to the main tree click events
    $scope.treeEventHandler = $({});
    navigationService.setupTreeEvents($scope.treeEventHandler);

    //Put the navigation service on this scope so we can use it's methods/properties in the view.
    // IMPORTANT: all properties assigned to this scope are generally available on the scope object on dialogs since
    //   when we create a dialog we pass in this scope to be used for the dialog's scope instead of creating a new one.
    $scope.nav = navigationService;
    // TODO: Lets fix this, it is less than ideal to be passing in the navigationController scope to something else to be used as it's scope,
    // this is going to lead to problems/confusion. I really don't think passing scope's around is very good practice.
    $rootScope.nav = navigationService;

    //set up our scope vars
    $scope.showContextMenuDialog = false;
    $scope.showContextMenu = false;
    $scope.showSearchResults = false;
    $scope.menuDialogTitle = null;
    $scope.menuActions = [];
    $scope.menuNode = null;

    $scope.currentSection = appState.getSectionState("currentSection");
    $scope.showNavigation = appState.getGlobalState("showNavigation");

    //trigger search with a hotkey:
    keyboardService.bind("ctrl+shift+s", function () {
        navigationService.showSearch();
    });

    //trigger dialods with a hotkey:
    keyboardService.bind("esc", function () {
        eventsService.emit("app.closeDialogs");
    });

    $scope.selectedId = navigationService.currentId;

    var evts = [];

    //Listen for global state changes
    evts.push(eventsService.on("appState.globalState.changed", function(e, args) {
        if (args.key === "showNavigation") {
            $scope.showNavigation = args.value;
        }
    }));

    //Listen for menu state changes
    evts.push(eventsService.on("appState.menuState.changed", function(e, args) {
        if (args.key === "showMenuDialog") {
            $scope.showContextMenuDialog = args.value;
        }
        if (args.key === "showMenu") {
            $scope.showContextMenu = args.value;
        }
        if (args.key === "dialogTitle") {
            $scope.menuDialogTitle = args.value;
        }
        if (args.key === "menuActions") {
            $scope.menuActions = args.value;
        }
        if (args.key === "currentNode") {
            $scope.menuNode = args.value;
        }
    }));

    //Listen for section state changes
    evts.push(eventsService.on("appState.treeState.changed", function(e, args) {
        var f = args;
        if (args.value.root && args.value.root.metaData.containsTrees === false) {
            $rootScope.emptySection = true;
        }
        else {
            $rootScope.emptySection = false;
        }
    }));

    //Listen for section state changes
    evts.push(eventsService.on("appState.sectionState.changed", function(e, args) {
        //section changed
        if (args.key === "currentSection") {
            $scope.currentSection = args.value;
        }
        //show/hide search results
        if (args.key === "showSearchResults") {
            $scope.showSearchResults = args.value;
        }
    }));

    //This reacts to clicks passed to the body element which emits a global call to close all dialogs
    evts.push(eventsService.on("app.closeDialogs", function(event) {
        if (appState.getGlobalState("stickyNavigation")) {
            navigationService.hideNavigation();
            //TODO: don't know why we need this? - we are inside of an angular event listener.
            angularHelper.safeApply($scope);
        }
    }));

    //when a user logs out or timesout
    evts.push(eventsService.on("app.notAuthenticated", function() {
        $scope.authenticated = false;
    }));

    //when the application is ready and the user is authorized setup the data
    evts.push(eventsService.on("app.ready", function(evt, data) {
        $scope.authenticated = true;
    }));

    //this reacts to the options item in the tree
    //todo, migrate to nav service
    $scope.searchShowMenu = function (ev, args) {
        //always skip default
        args.skipDefault = true;
        navigationService.showMenu(ev, args);
    };

    //todo, migrate to nav service
    $scope.searchHide = function () {
        navigationService.hideSearch();
    };

    //the below assists with hiding/showing the tree
    var treeActive = false;

    //Sets a service variable as soon as the user hovers the navigation with the mouse
    //used by the leaveTree method to delay hiding
    $scope.enterTree = function (event) {
        treeActive = true;
    };

    // Hides navigation tree, with a short delay, is cancelled if the user moves the mouse over the tree again
    $scope.leaveTree = function(event) {
        //this is a hack to handle IE touch events which freaks out due to no mouse events so the tree instantly shuts down
        if (!event) {
            return;
        }
        if (!appState.getGlobalState("touchDevice")) {
            treeActive = false;
            $timeout(function() {
                if (!treeActive) {
                    navigationService.hideTree();
                }
            }, 300);
        }
    };

    //ensure to unregister from all events!
    $scope.$on('$destroy', function () {
        for (var e in evts) {
            eventsService.unsubscribe(evts[e]);
        }
    });
}

//register it
angular.module('umbraco').controller("Umbraco.NavigationController", NavigationController);

/**
 * @ngdoc controller
 * @name Umbraco.SearchController
 * @function
 * 
 * @description
 * Controls the search functionality in the site
 *  
 */
function SearchController($scope, searchService, $log, $location, navigationService, $q) {

    $scope.searchTerm = null;
    $scope.searchResults = [];
    $scope.isSearching = false;
    $scope.selectedResult = -1;


    $scope.navigateResults = function(ev){
        //38: up 40: down, 13: enter

        switch(ev.keyCode){
            case 38:
                    iterateResults(true);
                break;
            case 40:
                    iterateResults(false);
                break;
            case 13:
                if ($scope.selectedItem) {
                    $location.path($scope.selectedItem.editorPath);
                    navigationService.hideSearch();
                }                
                break;
        }
    };


    var group = undefined;
    var groupIndex = -1;
    var itemIndex = -1;
    $scope.selectedItem = undefined;
        

    function iterateResults(up){
        //default group
        if(!group){
            group = $scope.groups[0];
            groupIndex = 0;
        }

        if(up){
            if(itemIndex === 0){
                if(groupIndex === 0){
                    gotoGroup($scope.groups.length-1, true);
                }else{
                    gotoGroup(groupIndex-1, true);
                }
            }else{
                gotoItem(itemIndex-1);
            }
        }else{
            if(itemIndex < group.results.length-1){
                gotoItem(itemIndex+1);
            }else{
                if(groupIndex === $scope.groups.length-1){
                    gotoGroup(0);
                }else{
                    gotoGroup(groupIndex+1);
                }
            }
        }
    }

    function gotoGroup(index, up){
        groupIndex = index;
        group = $scope.groups[groupIndex];
        
        if(up){
            gotoItem(group.results.length-1);
        }else{
            gotoItem(0); 
        }
    }

    function gotoItem(index){
        itemIndex = index;
        $scope.selectedItem = group.results[itemIndex];
    }

    //used to cancel any request in progress if another one needs to take it's place
    var canceler = null;

    $scope.$watch("searchTerm", _.debounce(function (newVal, oldVal) {
        $scope.$apply(function() {
            if ($scope.searchTerm) {
                if (newVal !== null && newVal !== undefined && newVal !== oldVal) {
                    $scope.isSearching = true;
                    navigationService.showSearch();
                    $scope.selectedItem = undefined;

                    //a canceler exists, so perform the cancelation operation and reset
                    if (canceler) {
                        console.log("CANCELED!");
                        canceler.resolve();
                        canceler = $q.defer();
                    }
                    else {
                        canceler = $q.defer();
                    }

                    searchService.searchAll({ term: $scope.searchTerm, canceler: canceler }).then(function(result) {
                        $scope.groups = _.filter(result, function (group) { return group.results.length > 0; });
                        //set back to null so it can be re-created
                        canceler = null;
                    });
                }
            }
            else {
                $scope.isSearching = false;
                navigationService.hideSearch();
                $scope.selectedItem = undefined;
            }
        });
    }, 200));

}
//register it
angular.module('umbraco').controller("Umbraco.SearchController", SearchController);

/**
 * @ngdoc controller
 * @name Umbraco.MainController
 * @function
 * 
 * @description
 * The controller for the AuthorizeUpgrade login page
 * 
 */
function AuthorizeUpgradeController($scope, $window) {
    
    //Add this method to the scope - this method will be called by the login dialog controller when the login is successful
    // then we'll handle the redirect.
    $scope.submit = function (event) {

        var qry = $window.location.search.trimStart("?").split("&");
        var redir = _.find(qry, function(item) {
            return item.startsWith("redir=");
        });
        if (redir) {
            $window.location = decodeURIComponent(redir.split("=")[1]);
        }
        else {
            $window.location = "/";
        }
        
    };

}

angular.module('umbraco').controller("Umbraco.AuthorizeUpgradeController", AuthorizeUpgradeController);
/**
 * @ngdoc controller
 * @name Umbraco.DashboardController
 * @function
 * 
 * @description
 * Controls the dashboards of the application
 * 
 */
 
function DashboardController($scope, $routeParams, dashboardResource, localizationService) {
    $scope.dashboard = {};
    localizationService.localize("sections_" + $routeParams.section).then(function(name){
    	$scope.dashboard.name = name;
    });
    
    dashboardResource.getDashboard($routeParams.section).then(function(tabs){
   		$scope.dashboard.tabs = tabs;
    });
}


//register it
angular.module('umbraco').controller("Umbraco.DashboardController", DashboardController);
angular.module("umbraco")
    .controller("Umbraco.Dialogs.ApprovedColorPickerController", function ($scope, $http, umbPropEditorHelper, assetsService) {
        assetsService.loadJs("lib/cssparser/cssparser.js")
			    .then(function () {

			        var cssPath = $scope.dialogData.cssPath;
			        $scope.cssClass = $scope.dialogData.cssClass;

			        $scope.classes = [];

			        $scope.change = function (newClass) {
			            $scope.model.value = newClass;
			        }

			        $http.get(cssPath)
                        .success(function (data) {
                            var parser = new CSSParser();
                            $scope.classes = parser.parse(data, false, false).cssRules;
                            $scope.classes.splice(0, 0, "noclass");
                        })

			        assetsService.loadCss("/App_Plugins/Lecoati.uSky.Grid/lib/uSky.Grid.ApprovedColorPicker.css");
			        assetsService.loadCss(cssPath);
			    });
});
function ContentEditDialogController($scope, editorState, $routeParams, $q, $timeout, $window, appState, contentResource, entityResource, navigationService, notificationsService, angularHelper, serverValidationManager, contentEditingHelper, treeService, fileManager, formHelper, umbRequestHelper, umbModelMapper, $http) {
    
    $scope.defaultButton = null;
    $scope.subButtons = [];
    var dialogOptions = $scope.$parent.dialogOptions;

    // This is a helper method to reduce the amount of code repitition for actions: Save, Publish, SendToPublish
    function performSave(args) {
        contentEditingHelper.contentEditorPerformSave({
            statusMessage: args.statusMessage,
            saveMethod: args.saveMethod,
            scope: $scope,
            content: $scope.content
        }).then(function (content) {
            //success            
            if (dialogOptions.closeOnSave) {
                $scope.submit(content);
            }

        }, function(err) {
            //error
        });
    }

    function filterTabs(entity, blackList) {
        if (blackList) {
            _.each(entity.tabs, function (tab) {
                tab.hide = _.contains(blackList, tab.alias);
            });
        }

        return entity;
    };
    
    function init(content) {
        var buttons = contentEditingHelper.configureContentEditorButtons({
            create: $routeParams.create,
            content: content,
            methods: {
                saveAndPublish: $scope.saveAndPublish,
                sendToPublish: $scope.sendToPublish,
                save: $scope.save,
                unPublish: angular.noop
            }
        });
        $scope.defaultButton = buttons.defaultButton;
        $scope.subButtons = buttons.subButtons;

        //This is a total hack but we have really no other way of sharing data to the property editors of this
        // content item, so we'll just set the property on the content item directly
        $scope.content.isDialogEditor = true;

        editorState.set($scope.content);
    }

    //check if the entity is being passed in, otherwise load it from the server
    if (angular.isObject(dialogOptions.entity)) {
        $scope.loaded = true;
        $scope.content = filterTabs(dialogOptions.entity, dialogOptions.tabFilter);
        init($scope.content);
    }
    else {
        contentResource.getById(dialogOptions.id)
            .then(function(data) {
                $scope.loaded = true;
                $scope.content = filterTabs(data, dialogOptions.tabFilter);
                init($scope.content);
                //in one particular special case, after we've created a new item we redirect back to the edit
                // route but there might be server validation errors in the collection which we need to display
                // after the redirect, so we will bind all subscriptions which will show the server validation errors
                // if there are any and then clear them so the collection no longer persists them.
                serverValidationManager.executeAndClearAllSubscriptions();
            });
    }  

    $scope.sendToPublish = function () {
        performSave({ saveMethod: contentResource.sendToPublish, statusMessage: "Sending..." });
    };

    $scope.saveAndPublish = function () {
        performSave({ saveMethod: contentResource.publish, statusMessage: "Publishing..." });
    };

    $scope.save = function () {
        performSave({ saveMethod: contentResource.save, statusMessage: "Saving..." });
    };

    // this method is called for all action buttons and then we proxy based on the btn definition
    $scope.performAction = function (btn) {

        if (!btn || !angular.isFunction(btn.handler)) {
            throw "btn.handler must be a function reference";
        }

        if (!$scope.busy) {
            btn.handler.apply(this);
        }
    };

}


angular.module("umbraco")
	.controller("Umbraco.Dialogs.Content.EditController", ContentEditDialogController);
angular.module("umbraco")
    .controller("Umbraco.Dialogs.HelpController", function ($scope, $location, $routeParams, helpService, userService, localizationService) {
        $scope.section = $routeParams.section;
        $scope.version = Umbraco.Sys.ServerVariables.application.version + " assembly: " + Umbraco.Sys.ServerVariables.application.assemblyVersion;
        
        if(!$scope.section){
            $scope.section = "content";
        }

        $scope.sectionName = $scope.section;

        var rq = {};
        rq.section = $scope.section;

        //translate section name
        localizationService.localize("sections_" + rq.section).then(function (value) {
            $scope.sectionName = value;
        });
        
        userService.getCurrentUser().then(function(user){
        	
        	rq.usertype = user.userType;
        	rq.lang = user.locale;

    	    if($routeParams.url){
    	    	rq.path = decodeURIComponent($routeParams.url);
    	    	
    	    	if(rq.path.indexOf(Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath) === 0){
    				rq.path = rq.path.substring(Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath.length);	
    			}

    			if(rq.path.indexOf(".aspx") > 0){
    				rq.path = rq.path.substring(0, rq.path.indexOf(".aspx"));
    			}
    					
    	    }else{
    	    	rq.path = rq.section + "/" + $routeParams.tree + "/" + $routeParams.method;
    	    }

    	    helpService.findHelp(rq).then(function(topics){
    	    	$scope.topics = topics;
    	    });

    	    helpService.findVideos(rq).then(function(videos){
    	        $scope.videos = videos;
    	    });

        });

        
    });
//used for the icon picker dialog
angular.module("umbraco")
    .controller("Umbraco.Dialogs.IconPickerController",
        function ($scope, iconHelper) {
            
            iconHelper.getIcons().then(function(icons){
            	$scope.icons = icons;
            });

			$scope.submitClass = function(icon){
				if($scope.color)
				{
					$scope.submit(icon + " " + $scope.color);
				}else{
					$scope.submit(icon);	
				}
			};
		}
	);
/**
 * @ngdoc controller
 * @name Umbraco.Dialogs.InsertMacroController
 * @function
 * 
 * @description
 * The controller for the custom insert macro dialog. Until we upgrade the template editor to be angular this 
 * is actually loaded into an iframe with full html.
 */
function InsertMacroController($scope, entityResource, macroResource, umbPropEditorHelper, macroService, formHelper) {

    /** changes the view to edit the params of the selected macro */
    function editParams() {
        //get the macro params if there are any
        macroResource.getMacroParameters($scope.selectedMacro.id)
            .then(function (data) {

                //go to next page if there are params otherwise we can just exit
                if (!angular.isArray(data) || data.length === 0) {
                    //we can just exist!
                    submitForm();

                } else {
                    $scope.wizardStep = "paramSelect";
                    $scope.macroParams = data;
                    
                    //fill in the data if we are editing this macro
                    if ($scope.dialogData && $scope.dialogData.macroData && $scope.dialogData.macroData.macroParamsDictionary) {
                        _.each($scope.dialogData.macroData.macroParamsDictionary, function (val, key) {
                            var prop = _.find($scope.macroParams, function (item) {
                                return item.alias == key;
                            });
                            if (prop) {

                                if (_.isString(val)) {
                                    //we need to unescape values as they have most likely been escaped while inserted 
                                    val = _.unescape(val);

                                    //detect if it is a json string
                                    if (val.detectIsJson()) {
                                        try {
                                            //Parse it to json
                                            prop.value = angular.fromJson(val);
                                        }
                                        catch (e) {
                                            // not json
                                            prop.value = val;
                                        }
                                    }
                                    else {
                                        prop.value = val;
                                    }
                                }
                                else {
                                    prop.value = val;
                                }
                            }
                        });

                    }
                }
            });
    }

    /** submit the filled out macro params */
    function submitForm() {
        
        //collect the value data, close the dialog and send the data back to the caller

        //create a dictionary for the macro params
        var paramDictionary = {};
        _.each($scope.macroParams, function (item) {

            var val = item.value;

            if (item.value != null && item.value != undefined && !_.isString(item.value)) {
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
        
        //need to find the macro alias for the selected id
        var macroAlias = $scope.selectedMacro.alias;

        //get the syntax based on the rendering engine
        var syntax;
        if ($scope.dialogData.renderingEngine && $scope.dialogData.renderingEngine === "WebForms") {
            syntax = macroService.generateWebFormsSyntax({ macroAlias: macroAlias, macroParamsDictionary: paramDictionary });
        }
        else if ($scope.dialogData.renderingEngine && $scope.dialogData.renderingEngine === "Mvc") {
            syntax = macroService.generateMvcSyntax({ macroAlias: macroAlias, macroParamsDictionary: paramDictionary });
        }
        else {
            syntax = macroService.generateMacroSyntax({ macroAlias: macroAlias, macroParamsDictionary: paramDictionary });
        }

        $scope.submit({ syntax: syntax, macroAlias: macroAlias, macroParamsDictionary: paramDictionary });
    }

    $scope.macros = [];
    $scope.selectedMacro = null;
    $scope.wizardStep = "macroSelect";
    $scope.macroParams = [];
    
    $scope.submitForm = function () {
        
        if (formHelper.submitForm({ scope: $scope })) {
        
            formHelper.resetForm({ scope: $scope });

            if ($scope.wizardStep === "macroSelect") {
                editParams();
            }
            else {
                submitForm();
            }

        }
    };

    //here we check to see if we've been passed a selected macro and if so we'll set the
    //editor to start with parameter editing
    if ($scope.dialogData && $scope.dialogData.macroData) {
        $scope.wizardStep = "paramSelect";
    }
    
    //get the macro list - pass in a filter if it is only for rte
    entityResource.getAll("Macro", ($scope.dialogData && $scope.dialogData.richTextEditor && $scope.dialogData.richTextEditor === true) ? "UseInEditor=true" : null)
        .then(function (data) {

            //if 'allowedMacros' is specified, we need to filter
            if (angular.isArray($scope.dialogData.allowedMacros) && $scope.dialogData.allowedMacros.length > 0) {
                $scope.macros = _.filter(data, function(d) {
                    return _.contains($scope.dialogData.allowedMacros, d.alias);
                });
            }
            else {
                $scope.macros = data;
            }
            

            //check if there's a pre-selected macro and if it exists
            if ($scope.dialogData && $scope.dialogData.macroData && $scope.dialogData.macroData.macroAlias) {
                var found = _.find(data, function (item) {
                    return item.alias === $scope.dialogData.macroData.macroAlias;
                });
                if (found) {
                    //select the macro and go to next screen
                    $scope.selectedMacro = found;
                    editParams();
                    return;
                }
            }
            //we don't have a pre-selected macro so ensure the correct step is set
            $scope.wizardStep = "macroSelect";
        });


}

angular.module("umbraco").controller("Umbraco.Dialogs.InsertMacroController", InsertMacroController);

/**
 * @ngdoc controller
 * @name Umbraco.Dialogs.LegacyDeleteController
 * @function
 * 
 * @description
 * The controller for deleting content
 */
function LegacyDeleteController($scope, legacyResource, treeService, navigationService) {

    $scope.performDelete = function() {

        //mark it for deletion (used in the UI)
        $scope.currentNode.loading = true;

        legacyResource.deleteItem({            
            nodeId: $scope.currentNode.id,
            nodeType: $scope.currentNode.nodeType,
            alias: $scope.currentNode.name,
        }).then(function () {
            $scope.currentNode.loading = false;
            //TODO: Need to sync tree, etc...
            treeService.removeNode($scope.currentNode);
            navigationService.hideMenu();
        });

    };

    $scope.cancel = function() {
        navigationService.hideDialog();
    };
}

angular.module("umbraco").controller("Umbraco.Dialogs.LegacyDeleteController", LegacyDeleteController);

//used for the media picker dialog
angular.module("umbraco").controller("Umbraco.Dialogs.LinkPickerController",
	function ($scope, eventsService, dialogService, entityResource, contentResource, mediaHelper, userService, localizationService) {
	    var dialogOptions = $scope.dialogOptions;

	    var searchText = "Search...";
	    localizationService.localize("general_search").then(function (value) {
	        searchText = value + "...";
	    });

	    $scope.dialogTreeEventHandler = $({});
	    $scope.target = {};
	    $scope.searchInfo = {
	        searchFromId: null,
	        searchFromName: null,
	        showSearch: false,
	        results: [],
	        selectedSearchResults: []
	    }

	    if (dialogOptions.currentTarget) {
	        $scope.target = dialogOptions.currentTarget;

	        //if we have a node ID, we fetch the current node to build the form data
	        if ($scope.target.id) {

	            if (!$scope.target.path) {
	                entityResource.getPath($scope.target.id, "Document").then(function (path) {
	                    $scope.target.path = path;
	                    //now sync the tree to this path
	                    $scope.dialogTreeEventHandler.syncTree({ path: $scope.target.path, tree: "content" });
	                });
	            }

	            contentResource.getNiceUrl($scope.target.id).then(function (url) {
	                $scope.target.url = url;
	            });
	        }
	    }

	    function nodeSelectHandler(ev, args) {
	        args.event.preventDefault();
	        args.event.stopPropagation();

	        if (args.node.metaData.listViewNode) {
	            //check if list view 'search' node was selected

	            $scope.searchInfo.showSearch = true;
	            $scope.searchInfo.searchFromId = args.node.metaData.listViewNode.id;
	            $scope.searchInfo.searchFromName = args.node.metaData.listViewNode.name;
	        }	      
	        else {
	            eventsService.emit("dialogs.linkPicker.select", args);

	            if ($scope.currentNode) {
	                //un-select if there's a current one selected
	                $scope.currentNode.selected = false;
	            }

	            $scope.currentNode = args.node;
	            $scope.currentNode.selected = true;
	            $scope.target.id = args.node.id;
	            $scope.target.name = args.node.name;

	            if (args.node.id < 0) {
	                $scope.target.url = "/";
	            }
	            else {
	                contentResource.getNiceUrl(args.node.id).then(function (url) {
	                    $scope.target.url = url;
	                });
	            }

	            if (!angular.isUndefined($scope.target.isMedia)) {
	                delete $scope.target.isMedia;
	            }
	        }	        
	    }

	    function nodeExpandedHandler(ev, args) {
	        if (angular.isArray(args.children)) {

	            //iterate children
	            _.each(args.children, function (child) {
	                //check if any of the items are list views, if so we need to add a custom 
	                // child: A node to activate the search
	                if (child.metaData.isContainer) {
	                    child.hasChildren = true;
	                    child.children = [
	                        {
	                            level: child.level + 1,
	                            hasChildren: false,
	                            name: searchText,
	                            metaData: {
	                                listViewNode: child,
	                            },
	                            cssClass: "icon umb-tree-icon sprTree icon-search",
	                            cssClasses: ["not-published"]
	                        }
	                    ];
	                }	                
	            });
	        }
	    }

	    $scope.switchToMediaPicker = function () {
	        userService.getCurrentUser().then(function (userData) {
	            dialogService.mediaPicker({
	                startNodeId: userData.startMediaId,
	                callback: function (media) {
	                    $scope.target.id = media.id;
	                    $scope.target.isMedia = true;
	                    $scope.target.name = media.name;
	                    $scope.target.url = mediaHelper.resolveFile(media);
	                }
	            });
	        });
	    };

	    $scope.hideSearch = function () {
	        $scope.searchInfo.showSearch = false;
	        $scope.searchInfo.searchFromId = null;
	        $scope.searchInfo.searchFromName = null;
	        $scope.searchInfo.results = [];
	    }

	    // method to select a search result 
	    $scope.selectResult = function (evt, result) {
	        result.selected = result.selected === true ? false : true;
	        nodeSelectHandler(evt, {event: evt, node: result});
	    };

        //callback when there are search results 
	    $scope.onSearchResults = function (results) {
	        $scope.searchInfo.results = results;
            $scope.searchInfo.showSearch = true;
	    };

	    $scope.dialogTreeEventHandler.bind("treeNodeSelect", nodeSelectHandler);
	    $scope.dialogTreeEventHandler.bind("treeNodeExpanded", nodeExpandedHandler);

	    $scope.$on('$destroy', function () {
	        $scope.dialogTreeEventHandler.unbind("treeNodeSelect", nodeSelectHandler);
	        $scope.dialogTreeEventHandler.unbind("treeNodeExpanded", nodeExpandedHandler);
	    });
	});
angular.module("umbraco").controller("Umbraco.Dialogs.LoginController",
    function ($scope, localizationService, userService, externalLoginInfo) {

        /**
         * @ngdoc function
         * @name signin
         * @methodOf MainController
         * @function
         *
         * @description
         * signs the user in
         */
        var d = new Date();
        //var weekday = new Array("Super Sunday", "Manic Monday", "Tremendous Tuesday", "Wonderful Wednesday", "Thunder Thursday", "Friendly Friday", "Shiny Saturday");
        localizationService.localize("login_greeting" + d.getDay()).then(function (label) {
            $scope.greeting = label;
        }); // weekday[d.getDay()];

        $scope.errorMsg = "";

        $scope.externalLoginFormAction = Umbraco.Sys.ServerVariables.umbracoUrls.externalLoginsUrl;
        $scope.externalLoginProviders = externalLoginInfo.providers;
        $scope.externalLoginInfo = externalLoginInfo;

        $scope.loginSubmit = function (login, password) {

            //if the login and password are not empty we need to automatically 
            // validate them - this is because if there are validation errors on the server
            // then the user has to change both username & password to resubmit which isn't ideal,
            // so if they're not empty , we'l just make sure to set them to valid.
            if (login && password && login.length > 0 && password.length > 0) {
                $scope.loginForm.username.$setValidity('auth', true);
                $scope.loginForm.password.$setValidity('auth', true);
            }


            if ($scope.loginForm.$invalid) {
                return;
            }

            userService.authenticate(login, password)
                .then(function (data) {
                    $scope.submit(true);
                }, function (reason) {
                    $scope.errorMsg = reason.errorMsg;

                    //set the form inputs to invalid
                    $scope.loginForm.username.$setValidity("auth", false);
                    $scope.loginForm.password.$setValidity("auth", false);
                });

            //setup a watch for both of the model values changing, if they change
            // while the form is invalid, then revalidate them so that the form can 
            // be submitted again.
            $scope.loginForm.username.$viewChangeListeners.push(function () {
                if ($scope.loginForm.username.$invalid) {
                    $scope.loginForm.username.$setValidity('auth', true);
                }
            });
            $scope.loginForm.password.$viewChangeListeners.push(function () {
                if ($scope.loginForm.password.$invalid) {
                    $scope.loginForm.password.$setValidity('auth', true);
                }
            });
        };
    });

//used for the macro picker dialog
angular.module("umbraco").controller("Umbraco.Dialogs.MacroPickerController", function ($scope, macroFactory, umbPropEditorHelper) {
	$scope.macros = macroFactory.all(true);
	$scope.dialogMode = "list";

	$scope.configureMacro = function(macro){
		$scope.dialogMode = "configure";
		$scope.dialogData.macro = macroFactory.getMacro(macro.alias);
	    //set the correct view for each item
		for (var i = 0; i < dialogData.macro.properties.length; i++) {
		    dialogData.macro.properties[i].editorView = umbPropEditorHelper.getViewPath(dialogData.macro.properties[i].view);
		}
	};
});
//used for the media picker dialog
angular.module("umbraco")
    .controller("Umbraco.Dialogs.MediaPickerController",
        function ($scope, mediaResource, umbRequestHelper, entityResource, $log, mediaHelper, eventsService, treeService, $cookies, $element, $timeout, notificationsService) {

            var dialogOptions = $scope.dialogOptions;

            $scope.onlyImages = dialogOptions.onlyImages;
            $scope.showDetails = dialogOptions.showDetails;
            $scope.multiPicker = (dialogOptions.multiPicker && dialogOptions.multiPicker !== "0") ? true : false;
            $scope.startNodeId = dialogOptions.startNodeId ? dialogOptions.startNodeId : -1;
            $scope.cropSize = dialogOptions.cropSize;
            
            $scope.filesUploading = 0;
            $scope.dropping = false;
            $scope.progress = 0;

            $scope.options = {
                url: umbRequestHelper.getApiUrl("mediaApiBaseUrl", "PostAddFile") + "?origin=blueimp",
                autoUpload: true,
                dropZone: $element.find(".umb-dialogs-mediapicker.browser"),
                fileInput: $element.find("input.uploader"),
                formData: {
                    currentFolder: -1
                }
            };

            //preload selected item
            $scope.target = undefined;
            if(dialogOptions.currentTarget){
                $scope.target = dialogOptions.currentTarget;
            }

            $scope.submitFolder = function(e) {
                if (e.keyCode === 13) {
                    e.preventDefault();
                    
                    mediaResource
                        .addFolder($scope.newFolderName, $scope.options.formData.currentFolder)
                        .then(function(data) {
                            $scope.showFolderInput = false;
                            $scope.newFolderName = "";

                            //we've added a new folder so lets clear the tree cache for that specific item
                            treeService.clearCache({
                                cacheKey: "__media", //this is the main media tree cache key
                                childrenOf: data.parentId //clear the children of the parent
                            });

                            $scope.gotoFolder(data);
                        });
                }
            };

            $scope.gotoFolder = function(folder) {

                if(!folder){
                    folder = {id: -1, name: "Media", icon: "icon-folder"};
                }

                if (folder.id > 0) {
                    entityResource.getAncestors(folder.id, "media")
                        .then(function(anc) {
                            // anc.splice(0,1);  
                            $scope.path = _.filter(anc, function (f) {
                                return f.path.indexOf($scope.startNodeId) !== -1;
                            });
                        });
                }
                else {
                    $scope.path = [];
                }

                //mediaResource.rootMedia()
                mediaResource.getChildren(folder.id)
                    .then(function(data) {
                        $scope.searchTerm = "";
                        $scope.images = data.items ? data.items : [];
                    });

                $scope.options.formData.currentFolder = folder.id;
                $scope.currentFolder = folder;      
            };
            
            //This executes prior to the whole processing which we can use to get the UI going faster,
            //this also gives us the start callback to invoke to kick of the whole thing
            $scope.$on('fileuploadadd', function (e, data) {
                $scope.$apply(function () {
                    $scope.filesUploading++;
                });
            });

            //when one is finished
            $scope.$on('fileuploaddone', function (e, data) {
                $scope.filesUploading--;
                if ($scope.filesUploading == 0) {
                    $scope.$apply(function () {
                        $scope.progress = 0;
                        $scope.gotoFolder($scope.currentFolder);
                    });
                }
                //Show notifications!!!!
                if (data.result && data.result.notifications && angular.isArray(data.result.notifications)) {
                    for (var n = 0; n < data.result.notifications.length; n++) {
                        notificationsService.showNotification(data.result.notifications[n]);
                    }
                }
            });

            // All these sit-ups are to add dropzone area and make sure it gets removed if dragging is aborted! 
            $scope.$on('fileuploaddragover', function (e, data) {
                if (!$scope.dragClearTimeout) {
                    $scope.$apply(function () {
                        $scope.dropping = true;
                    });
                }
                else {
                    $timeout.cancel($scope.dragClearTimeout);
                }
                $scope.dragClearTimeout = $timeout(function () {
                    $scope.dropping = null;
                    $scope.dragClearTimeout = null;
                }, 300);
            });

            $scope.clickHandler = function(image, ev, select) {
                ev.preventDefault();
                
                if (image.isFolder && !select) {
                    $scope.gotoFolder(image);
                }else{
                    eventsService.emit("dialogs.mediaPicker.select", image);
                    
                    //we have 3 options add to collection (if multi) show details, or submit it right back to the callback
                    if ($scope.multiPicker) {
                        $scope.select(image);
                        image.cssclass = ($scope.dialogData.selection.indexOf(image) > -1) ? "selected" : "";
                    }else if($scope.showDetails) {
                        $scope.target= image;
                        $scope.target.url = mediaHelper.resolveFile(image);
                    }else{
                        $scope.submit(image);
                    }
                }
            };

            $scope.exitDetails = function(){
                if(!$scope.currentFolder){
                    $scope.gotoFolder();
                }

                $scope.target = undefined;
            };

           

            //default root item
            if(!$scope.target){
                $scope.gotoFolder({ id: $scope.startNodeId, name: "Media", icon: "icon-folder" });  
            }
        });
//used for the member picker dialog
angular.module("umbraco").controller("Umbraco.Dialogs.MemberGroupPickerController",
    function($scope, eventsService, entityResource, searchService, $log) {
        var dialogOptions = $scope.dialogOptions;
        $scope.dialogTreeEventHandler = $({});
        $scope.multiPicker = dialogOptions.multiPicker;

        /** Method used for selecting a node */
        function select(text, id) {

            if (dialogOptions.multiPicker) {
                $scope.select(id);              
            }
            else {
                $scope.submit(id);               
            }
        }
        
        function nodeSelectHandler(ev, args) {
            args.event.preventDefault();
            args.event.stopPropagation();
            
            eventsService.emit("dialogs.memberGroupPicker.select", args);
            
            //This is a tree node, so we don't have an entity to pass in, it will need to be looked up
            //from the server in this method.
            select(args.node.name, args.node.id);

            //toggle checked state
            args.node.selected = args.node.selected === true ? false : true;
        }

        $scope.dialogTreeEventHandler.bind("treeNodeSelect", nodeSelectHandler);

        $scope.$on('$destroy', function () {
            $scope.dialogTreeEventHandler.unbind("treeNodeSelect", nodeSelectHandler);
        });
    });
angular.module("umbraco").controller("Umbraco.Dialogs.RteEmbedController", function ($scope, $http, umbRequestHelper) {
    $scope.form = {};
    $scope.form.url = "";
    $scope.form.width = 360;
    $scope.form.height = 240;
    $scope.form.constrain = true;
    $scope.form.preview = "";
    $scope.form.success = false;
    $scope.form.info = "";
    $scope.form.supportsDimensions = false;
    
    var origWidth = 500;
    var origHeight = 300;
    
    $scope.showPreview = function() {

        if ($scope.form.url) {
            $scope.form.show = true;
            $scope.form.preview = "<div class=\"umb-loader\" style=\"height: 10px; margin: 10px 0px;\"></div>";
            $scope.form.info = "";
            $scope.form.success = false;

            $http({ method: 'GET', url: umbRequestHelper.getApiUrl("embedApiBaseUrl", "GetEmbed"), params: { url: $scope.form.url, width: $scope.form.width, height: $scope.form.height } })
                .success(function (data) {
                    
                    $scope.form.preview = "";
                    
                    switch (data.Status) {
                        case 0:
                            //not supported
                            $scope.form.info = "Not supported";
                            break;
                        case 1:
                            //error
                            $scope.form.info = "Computer says no";
                            break;
                        case 2:
                            $scope.form.preview = data.Markup;
                            $scope.form.supportsDimensions = data.SupportsDimensions;
                            $scope.form.success = true;
                            break;
                    }
                })
                .error(function () {
                    $scope.form.supportsDimensions = false;
                    $scope.form.preview = "";
                    $scope.form.info = "Computer says no";
                });
        } else {
            $scope.form.supportsDimensions = false;
            $scope.form.preview = "";
            $scope.form.info = "Please enter a URL";
        }
    };

    $scope.changeSize = function (type) {
        var width, height;
        
        if ($scope.form.constrain) {
            width = parseInt($scope.form.width, 10);
            height = parseInt($scope.form.height, 10);
            if (type == 'width') {
                origHeight = Math.round((width / origWidth) * height);
                $scope.form.height = origHeight;
            } else {
                origWidth = Math.round((height / origHeight) * width);
                $scope.form.width = origWidth;
            }
        }
        if ($scope.form.url != "") {
            $scope.showPreview();
        }

    };
    
    $scope.insert = function(){
        $scope.submit($scope.form.preview);
    };
});
angular.module("umbraco").controller('Umbraco.Dialogs.Template.QueryBuilderController',
		function($scope, $http, dialogService){
			

            $http.get("backoffice/UmbracoApi/TemplateQuery/GetAllowedProperties").then(function(response) {
                $scope.properties = response.data;
            });

            $http.get("backoffice/UmbracoApi/TemplateQuery/GetContentTypes").then(function (response) {
                $scope.contentTypes = response.data;
            });

            $http.get("backoffice/UmbracoApi/TemplateQuery/GetFilterConditions").then(function (response) {
                $scope.conditions = response.data;
            });


			$scope.query = {
				contentType: {
					name: "Everything"
				},
				source:{
					name: "My website"
				}, 
				filters:[
					{
						property:undefined,
						operator: undefined
					}
				],
				sort:{
					property:{
						alias: "",
						name: "",
					},
					direction: "Ascending"
				}
			};



			$scope.chooseSource = function(query){
				dialogService.contentPicker({
				    callback: function (data) {

				        if (data.id > 0) {
				            query.source = { id: data.id, name: data.name };
				        } else {
				            query.source.name = "My website";
				            delete query.source.id;
				        }
					}
				});
			};

		    var throttledFunc = _.throttle(function() {

		        $http.post("backoffice/UmbracoApi/TemplateQuery/PostTemplateQuery", $scope.query).then(function (response) {
		            $scope.result = response.data;
		        });

		    }, 200);

		    $scope.$watch("query", function(value) {
		        throttledFunc();
		    }, true);

			$scope.getPropertyOperators = function (property) {

			    var conditions = _.filter($scope.conditions, function(condition) {
			        var index = condition.appliesTo.indexOf(property.type);
			        return index >= 0;
			    });
			    return conditions;
			};

			
			$scope.addFilter = function(query){				
			    query.filters.push({});
			};

			$scope.trashFilter = function (query) {
			    query.filters.splice(query,1);
			};

			$scope.changeSortOrder = function(query){
				if(query.sort.direction === "ascending"){
					query.sort.direction = "descending";
				}else{
					query.sort.direction = "ascending";
				}
			};

			$scope.setSortProperty = function(query, property){
				query.sort.property = property;
				if(property.type === "datetime"){
					query.sort.direction = "descending";
				}else{
					query.sort.direction = "ascending";
				}
			};
		});
angular.module("umbraco").controller('Umbraco.Dialogs.Template.SnippetController',
		function($scope) {
		    $scope.type = $scope.dialogOptions.type;
		    $scope.section = {
                name: "",
                required: false
		    };
		});
//used for the media picker dialog
angular.module("umbraco").controller("Umbraco.Dialogs.TreePickerController",
	function ($scope, entityResource, eventsService, $log, searchService, angularHelper, $timeout, localizationService, treeService) {

	    var tree = null;
	    var dialogOptions = $scope.dialogOptions;
	    $scope.dialogTreeEventHandler = $({});
	    $scope.section = dialogOptions.section;
	    $scope.treeAlias = dialogOptions.treeAlias;
	    $scope.multiPicker = dialogOptions.multiPicker;
	    $scope.hideHeader = true; 	    	    
        $scope.searchInfo = {
            searchFromId: dialogOptions.startNodeId,
            searchFromName: null,
            showSearch: false,
            results: [],
            selectedSearchResults: []
        }

	    //create the custom query string param for this tree
	    $scope.customTreeParams = dialogOptions.startNodeId ? "startNodeId=" + dialogOptions.startNodeId : "";
	    $scope.customTreeParams += dialogOptions.customTreeParams ? "&" + dialogOptions.customTreeParams : "";

	    var searchText = "Search...";
	    localizationService.localize("general_search").then(function (value) {
	        searchText = value + "...";
	    });

        // Allow the entity type to be passed in but defaults to Document for backwards compatibility.
	    var entityType = dialogOptions.entityType ? dialogOptions.entityType : "Document";
	    

	    //min / max values
	    if (dialogOptions.minNumber) {
	        dialogOptions.minNumber = parseInt(dialogOptions.minNumber, 10);
	    }
	    if (dialogOptions.maxNumber) {
	        dialogOptions.maxNumber = parseInt(dialogOptions.maxNumber, 10);
	    }

	    if (dialogOptions.section === "member") {
	        entityType = "Member";            
	    }
	    else if (dialogOptions.section === "media") {	    
	        entityType = "Media";
	    }

	    //Configures filtering
	    if (dialogOptions.filter) {

	        dialogOptions.filterExclude = false;
	        dialogOptions.filterAdvanced = false;

	        //used advanced filtering
	        if (angular.isFunction(dialogOptions.filter)) {
	            dialogOptions.filterAdvanced = true;
	        }
            else if (angular.isObject(dialogOptions.filter)) {
                dialogOptions.filterAdvanced = true;
            }
            else {
                if (dialogOptions.filter.startsWith("!")) {
                    dialogOptions.filterExclude = true;
                    dialogOptions.filter = dialogOptions.filter.substring(1);
                }

                //used advanced filtering
                if (dialogOptions.filter.startsWith("{")) {
                    dialogOptions.filterAdvanced = true;
                    //convert to object
                    dialogOptions.filter = angular.fromJson(dialogOptions.filter);
                }
            }
	    } 

	    function nodeExpandedHandler(ev, args) {            
	        if (angular.isArray(args.children)) {

                //iterate children
	            _.each(args.children, function (child) {

	                //check if any of the items are list views, if so we need to add some custom 
	                // children: A node to activate the search, any nodes that have already been 
	                // selected in the search
	                if (child.metaData.isContainer) {
	                    child.hasChildren = true;
	                    child.children = [
	                        {
                                level: child.level + 1,
                                hasChildren: false,
                                parent: function () {
                                    return child;
                                },
	                            name: searchText,
	                            metaData: {
	                                listViewNode: child,
	                            },
	                            cssClass: "icon-search",
	                            cssClasses: ["not-published"]
	                        }
	                    ];
                        //add base transition classes to this node
	                    child.cssClasses.push("tree-node-slide-up");

	                    var listViewResults = _.filter($scope.searchInfo.selectedSearchResults, function(i) {
	                        return i.parentId == child.id;
	                    });
	                    _.each(listViewResults, function(item) {
	                        child.children.unshift({
	                            id: item.id,
	                            name: item.name,
	                            cssClass: "icon umb-tree-icon sprTree " + item.icon,
	                            level: child.level + 1,
	                            metaData: {
	                                isSearchResult: true
	                            },
	                            hasChildren: false,
	                            parent: function () {
	                                return child;
	                            }
	                        });
	                    });
	                }

	                //now we need to look in the already selected search results and 
	                // toggle the check boxes for those ones that are listed
	                var exists = _.find($scope.searchInfo.selectedSearchResults, function (selected) {
	                    return child.id == selected.id;
	                });
	                if (exists) {
	                    child.selected = true;
	                }
	            });

	            //check filter
	            performFiltering(args.children);	            
	        }
	    }

        //gets the tree object when it loads
	    function treeLoadedHandler(ev, args) {
	        tree = args.tree;
	    }

	    //wires up selection
	    function nodeSelectHandler(ev, args) {
	        args.event.preventDefault();
	        args.event.stopPropagation();
	        
	        if (args.node.metaData.listViewNode) {
	            //check if list view 'search' node was selected

                $scope.searchInfo.showSearch = true;                
                $scope.searchInfo.searchFromId = args.node.metaData.listViewNode.id;
                $scope.searchInfo.searchFromName = args.node.metaData.listViewNode.name;

                //add transition classes
	            var listViewNode = args.node.parent();
	            listViewNode.cssClasses.push('tree-node-slide-up-hide-active');
	        }
            else if (args.node.metaData.isSearchResult) {
                //check if the item selected was a search result from a list view

                //unselect
                select(args.node.name, args.node.id);

                //remove it from the list view children
                var listView = args.node.parent();
	            listView.children = _.reject(listView.children, function(child) {
	                return child.id == args.node.id;
	            });

                //remove it from the custom tracked search result list
	            $scope.searchInfo.selectedSearchResults = _.reject($scope.searchInfo.selectedSearchResults, function (i) {
	                return i.id == args.node.id;
	            });
	        }
            else {
                eventsService.emit("dialogs.treePickerController.select", args);

                if (args.node.filtered) {
                    return;
                }

                //This is a tree node, so we don't have an entity to pass in, it will need to be looked up
                //from the server in this method.
                select(args.node.name, args.node.id);

                //toggle checked state
                args.node.selected = args.node.selected === true ? false : true;
            }	        
	    }

	    /** Method used for selecting a node */
	    function select(text, id, entity) {
	        //if we get the root, we just return a constructed entity, no need for server data
	        if (id < 0) {
	            if ($scope.multiPicker) {
	                $scope.select(id);
	            }
	            else {
	                var node = {
	                    alias: null,
	                    icon: "icon-folder",
	                    id: id,
	                    name: text
	                };
	                $scope.submit(node);
	            }
	        }
	        else {
	            
	            if ($scope.multiPicker) {
	                $scope.select(Number(id));
	            }
	            else {
                    
	                $scope.hideSearch();

	                //if an entity has been passed in, use it
	                if (entity) {
	                    $scope.submit(entity);
	                } else {
	                    //otherwise we have to get it from the server
	                    entityResource.getById(id, entityType).then(function (ent) {
	                        $scope.submit(ent);
	                    });
	                }
	            }
	        }
	    }

	    function performFiltering(nodes) {

	        if (!dialogOptions.filter) {
	            return;
	        }

	        //remove any list view search nodes from being filtered since these are special nodes that always must
	        // be allowed to be clicked on
	        nodes = _.filter(nodes, function(n) {
	            return !angular.isObject(n.metaData.listViewNode);
	        });

	        if (dialogOptions.filterAdvanced) {

                //filter either based on a method or an object
	            var filtered = angular.isFunction(dialogOptions.filter)
	                ? _.filter(nodes, dialogOptions.filter)
	                : _.where(nodes, dialogOptions.filter);

	            angular.forEach(filtered, function (value, key) {
	                value.filtered = true;
	                if (dialogOptions.filterCssClass) {
                        if (!value.cssClasses) {
                            value.cssClasses = [];
                        }
	                    value.cssClasses.push(dialogOptions.filterCssClass);
	                }
	            });
	        } else {
	            var a = dialogOptions.filter.toLowerCase().split(',');
	            angular.forEach(nodes, function (value, key) {

	                var found = a.indexOf(value.metaData.contentType.toLowerCase()) >= 0;

	                if (!dialogOptions.filterExclude && !found || dialogOptions.filterExclude && found) {
	                    value.filtered = true;

	                    if (dialogOptions.filterCssClass) {
	                        if (!value.cssClasses) {
	                            value.cssClasses = [];
	                        }
	                        value.cssClasses.push(dialogOptions.filterCssClass);
	                    }
	                }
	            });
	        }
	    }
        
	    $scope.multiSubmit = function (result) {
	        entityResource.getByIds(result, entityType).then(function (ents) {
	            $scope.submit(ents);
	        });
	    };
        
	    /** method to select a search result */
	    $scope.selectResult = function (evt, result) {

            if (result.filtered) {
                return;
            }

	        result.selected = result.selected === true ? false : true;

	        //since result = an entity, we'll pass it in so we don't have to go back to the server
	        select(result.name, result.id, result);

	        //add/remove to our custom tracked list of selected search results
            if (result.selected) {
                $scope.searchInfo.selectedSearchResults.push(result);
            }
            else {
                $scope.searchInfo.selectedSearchResults = _.reject($scope.searchInfo.selectedSearchResults, function(i) {
                    return i.id == result.id;
                });
            }

	        //ensure the tree node in the tree is checked/unchecked if it already exists there
	        if (tree) {	            
	            var found = treeService.getDescendantNode(tree.root, result.id);
                if (found) {
                    found.selected = result.selected;
                }
	        }
	        
	    };

	    $scope.hideSearch = function () {
            	    
            //Traverse the entire displayed tree and update each node to sync with the selected search results
	        if (tree) {

	            //we need to ensure that any currently displayed nodes that get selected
	            // from the search get updated to have a check box!
                function checkChildren(children) {
                    _.each(children, function (child) {
                        //check if the id is in the selection, if so ensure it's flagged as selected
                        var exists = _.find($scope.searchInfo.selectedSearchResults, function (selected) {
                            return child.id == selected.id;
                        });
                        //if the curr node exists in selected search results, ensure it's checked
                        if (exists) {
                            child.selected = true;
                        }
                        //if the curr node does not exist in the selected search result, and the curr node is a child of a list view search result
                        else if (child.metaData.isSearchResult) {
                            //if this tree node is under a list view it means that the node was added
                            // to the tree dynamically under the list view that was searched, so we actually want to remove
                            // it all together from the tree
                            var listView = child.parent();
                            listView.children = _.reject(listView.children, function(c) {
                                return c.id == child.id;
                            });
                        }
                        
                        //check if the current node is a list view and if so, check if there's any new results
                        // that need to be added as child nodes to it based on search results selected
                        if (child.metaData.isContainer) {

                            child.cssClasses = _.reject(child.cssClasses, function(c) {
                                return c === 'tree-node-slide-up-hide-active';
                            });

                            var listViewResults = _.filter($scope.searchInfo.selectedSearchResults, function (i) {
                                return i.parentId == child.id;
                            });
                            _.each(listViewResults, function (item) {
                                var childExists = _.find(child.children, function(c) {
                                    return c.id == item.id;
                                });
                                if (!childExists) {
                                    var parent = child;
                                    child.children.unshift({
                                        id: item.id,
                                        name: item.name,
                                        cssClass: "icon umb-tree-icon sprTree " + item.icon,
                                        level: child.level + 1,
                                        metaData: {
                                            isSearchResult: true
                                        },
                                        hasChildren: false,
                                        parent: function () {
                                            return parent;
                                        }
                                    });
                                }                                
                            });
                        }

                        //recurse
                        if (child.children && child.children.length > 0) {
                            checkChildren(child.children);
                        }
                    });
                }
                checkChildren(tree.root.children);
	        }
	        

            $scope.searchInfo.showSearch = false;            
            $scope.searchInfo.searchFromId = dialogOptions.startNodeId;
            $scope.searchInfo.searchFromName = null;
            $scope.searchInfo.results = [];
        }

	    $scope.onSearchResults = function(results) {
	        
            //filter all items - this will mark an item as filtered
	        performFiltering(results);

	        //now actually remove all filtered items so they are not even displayed
	        results = _.filter(results, function(item) {
	            return !item.filtered;
	        });

	        $scope.searchInfo.results = results;

            //sync with the curr selected results
	        _.each($scope.searchInfo.results, function (result) {
	            var exists = _.find($scope.dialogData.selection, function (selectedId) {
	                return result.id == selectedId;
	            });
	            if (exists) {
	                result.selected = true;
	            }	            
	        });

	        $scope.searchInfo.showSearch = true;
	    };

	    $scope.dialogTreeEventHandler.bind("treeLoaded", treeLoadedHandler);
	    $scope.dialogTreeEventHandler.bind("treeNodeExpanded", nodeExpandedHandler);
	    $scope.dialogTreeEventHandler.bind("treeNodeSelect", nodeSelectHandler);

	    $scope.$on('$destroy', function () {
	        $scope.dialogTreeEventHandler.unbind("treeLoaded", treeLoadedHandler);
	        $scope.dialogTreeEventHandler.unbind("treeNodeExpanded", nodeExpandedHandler);
	        $scope.dialogTreeEventHandler.unbind("treeNodeSelect", nodeSelectHandler);
	    });
	});
angular.module("umbraco")
    .controller("Umbraco.Dialogs.UserController", function ($scope, $location, $timeout, userService, historyService, eventsService, externalLoginInfo, authResource, currentUserResource, formHelper) {

        $scope.history = historyService.getCurrent();
        $scope.version = Umbraco.Sys.ServerVariables.application.version + " assembly: " + Umbraco.Sys.ServerVariables.application.assemblyVersion;

        $scope.externalLoginProviders = externalLoginInfo.providers;
        $scope.externalLinkLoginFormAction = Umbraco.Sys.ServerVariables.umbracoUrls.externalLinkLoginsUrl;
        var evts = [];
        evts.push(eventsService.on("historyService.add", function (e, args) {
            $scope.history = args.all;
        }));
        evts.push(eventsService.on("historyService.remove", function (e, args) {
            $scope.history = args.all;
        }));
        evts.push(eventsService.on("historyService.removeAll", function (e, args) {
            $scope.history = [];
        }));

        $scope.logout = function () {

            //Add event listener for when there are pending changes on an editor which means our route was not successful
            var pendingChangeEvent = eventsService.on("valFormManager.pendingChanges", function (e, args) {
                //one time listener, remove the event
                pendingChangeEvent();
                $scope.close();
            });


            //perform the path change, if it is successful then the promise will resolve otherwise it will fail
            $scope.close();
            $location.path("/logout");
        };

        $scope.gotoHistory = function (link) {
            $location.path(link);
            $scope.close();
        };

        //Manually update the remaining timeout seconds
        function updateTimeout() {
            $timeout(function () {
                if ($scope.remainingAuthSeconds > 0) {
                    $scope.remainingAuthSeconds--;
                    $scope.$digest();
                    //recurse
                    updateTimeout();
                }

            }, 1000, false); // 1 second, do NOT execute a global digest    
        }

        function updateUserInfo() {
            //get the user
            userService.getCurrentUser().then(function (user) {
                $scope.user = user;
                if ($scope.user) {
                    $scope.remainingAuthSeconds = $scope.user.remainingAuthSeconds;
                    $scope.canEditProfile = _.indexOf($scope.user.allowedSections, "users") > -1;
                    //set the timer
                    updateTimeout();

                    authResource.getCurrentUserLinkedLogins().then(function(logins) {
                        //reset all to be un-linked
                        for (var provider in $scope.externalLoginProviders) {
                            $scope.externalLoginProviders[provider].linkedProviderKey = undefined;
                        }

                        //set the linked logins
                        for (var login in logins) {
                            var found = _.find($scope.externalLoginProviders, function (i) {
                                return i.authType == login;
                            });
                            if (found) {
                                found.linkedProviderKey = logins[login];
                            }
                        }
                    });
                }
            });
        }

        $scope.unlink = function (e, loginProvider, providerKey) {
            var result = confirm("Are you sure you want to unlink this account?");
            if (!result) {
                e.preventDefault();
                return;
            }

            authResource.unlinkLogin(loginProvider, providerKey).then(function (a, b, c) {
                updateUserInfo();
            });
        }

        updateUserInfo();

        //remove all event handlers
        $scope.$on('$destroy', function () {
            for (var e = 0; e < evts.length; e++) {
                evts[e]();
            }

        });

        //create the initial model for change password property editor
        $scope.changePasswordModel = {
            alias: "_umb_password",
            view: "changepassword",
            config: {},
            value: {}
        };

        //go get the config for the membership provider and add it to the model
        currentUserResource.getMembershipProviderConfig().then(function (data) {
            $scope.changePasswordModel.config = data;
            //ensure the hasPassword config option is set to true (the user of course has a password already assigned)
            //this will ensure the oldPassword is shown so they can change it
            $scope.changePasswordModel.config.hasPassword = true;
            $scope.changePasswordModel.config.disableToggle = true;
        });

        ////this is the model we will pass to the service
        //$scope.profile = {};

        $scope.changePassword = function () {

            if (formHelper.submitForm({ scope: $scope })) {
                currentUserResource.changePassword($scope.changePasswordModel.value).then(function (data) {

                    //if the password has been reset, then update our model
                    if (data.value) {
                        $scope.changePasswordModel.value.generatedPassword = data.value;
                    }

                    formHelper.resetForm({ scope: $scope, notifications: data.notifications });

                }, function (err) {

                    formHelper.handleError(err);

                });
            }
        };

    });

/**
 * @ngdoc controller
 * @name Umbraco.Dialogs.LegacyDeleteController
 * @function
 * 
 * @description
 * The controller for deleting content
 */
function YsodController($scope, legacyResource, treeService, navigationService) {
    
    if ($scope.error && $scope.error.data && $scope.error.data.StackTrace) {
        //trim whitespace
        $scope.error.data.StackTrace = $scope.error.data.StackTrace.trim();
    }

    $scope.closeDialog = function() {
        $scope.dismiss();
    };

}

angular.module("umbraco").controller("Umbraco.Dialogs.YsodController", YsodController);

/**
 * @ngdoc controller
 * @name Umbraco.LegacyController
 * @function
 * 
 * @description
 * A controller to control the legacy iframe injection
 * 
*/
function LegacyController($scope, $routeParams, $element) {

    var url = decodeURIComponent($routeParams.url.replace(/javascript\:/gi, ""));
    //split into path and query
    var urlParts = url.split("?");
    var extIndex = urlParts[0].lastIndexOf(".");
    var ext = extIndex === -1 ? "" : urlParts[0].substr(extIndex);
    //path cannot be a js file
    if (ext !== ".js" || ext === "") {
        //path cannot contain any of these chars
        var toClean = "*(){}[];:<>\\|'\"";
        for (var i = 0; i < toClean.length; i++) {
            var reg = new RegExp("\\" + toClean[i], "g");
            urlParts[0] = urlParts[0].replace(reg, "");
        }
        //join cleaned path and query back together
        url = urlParts[0] + (urlParts.length === 1 ? "" : ("?" + urlParts[1]));
        $scope.legacyPath = url;
    }
    else {
        throw "Invalid url";
    }
}

angular.module("umbraco").controller('Umbraco.LegacyController', LegacyController);
/** This controller is simply here to launch the login dialog when the route is explicitly changed to /login */
angular.module('umbraco').controller("Umbraco.LoginController", function (eventsService, $scope, userService, $location, $rootScope) {

    userService._showLoginDialog(); 
       
    var evtOn = eventsService.on("app.ready", function(evt, data){
        $scope.avatar = "assets/img/application/logo.png";

        var path = "/";

        //check if there's a returnPath query string, if so redirect to it
        var locationObj = $location.search();
        if (locationObj.returnPath) {
            path = decodeURIComponent(locationObj.returnPath);
        }

        $location.url(path);
    });

    $scope.$on('$destroy', function () {
        eventsService.unsubscribe(evtOn);
    });

});

//used for the media picker dialog
angular.module("umbraco").controller("Umbraco.Notifications.ConfirmRouteChangeController",
	function ($scope, $location, $log, notificationsService) {	

		$scope.discard = function(not){
			not.args.listener();
			
			$location.search("");

		    //we need to break the path up into path and query
		    var parts = not.args.path.split("?");
		    var query = {};
            if (parts.length > 1) {
                _.each(parts[1].split("&"), function(q) {
                    var keyVal = q.split("=");
                    query[keyVal[0]] = keyVal[1];
                });
            }

            $location.path(parts[0]).search(query);
			notificationsService.remove(not);
		};

		$scope.stay = function(not){
			notificationsService.remove(not);
		};

	});
angular.module("umbraco").controller("Umbraco.Editors.Content.CopyController",
	function ($scope, eventsService, contentResource, navigationService, appState, treeService, localizationService) {

	    var dialogOptions = $scope.dialogOptions;
	    var searchText = "Search...";
	    localizationService.localize("general_search").then(function (value) {
	        searchText = value + "...";
	    });

	    $scope.recursive = true;
	    $scope.relateToOriginal = false;
	    $scope.dialogTreeEventHandler = $({});
	    $scope.busy = false;
	    $scope.searchInfo = {
	        searchFromId: null,
	        searchFromName: null,
	        showSearch: false,
	        results: [],
	        selectedSearchResults: []
	    }

	    var node = dialogOptions.currentNode;

	    function nodeSelectHandler(ev, args) {
	        args.event.preventDefault();
	        args.event.stopPropagation();

	        if (args.node.metaData.listViewNode) {
	            //check if list view 'search' node was selected

	            $scope.searchInfo.showSearch = true;
	            $scope.searchInfo.searchFromId = args.node.metaData.listViewNode.id;
	            $scope.searchInfo.searchFromName = args.node.metaData.listViewNode.name;
	        }
	        else {
	            eventsService.emit("editors.content.copyController.select", args);

	            if ($scope.target) {
	                //un-select if there's a current one selected
	                $scope.target.selected = false;
	            }

	            $scope.target = args.node;
	            $scope.target.selected = true;
	        }
	        
	    }

	    function nodeExpandedHandler(ev, args) {
	        if (angular.isArray(args.children)) {

	            //iterate children
	            _.each(args.children, function (child) {
	                //check if any of the items are list views, if so we need to add a custom 
	                // child: A node to activate the search
	                if (child.metaData.isContainer) {
	                    child.hasChildren = true;
	                    child.children = [
	                        {
	                            level: child.level + 1,
	                            hasChildren: false,
	                            name: searchText,
	                            metaData: {
	                                listViewNode: child,
	                            },
	                            cssClass: "icon umb-tree-icon sprTree icon-search",
	                            cssClasses: ["not-published"]
	                        }
	                    ];
	                }
	            });
	        }
	    }

	    $scope.hideSearch = function () {
	        $scope.searchInfo.showSearch = false;
	        $scope.searchInfo.searchFromId = null;
	        $scope.searchInfo.searchFromName = null;
	        $scope.searchInfo.results = [];
	    }

	    // method to select a search result 
	    $scope.selectResult = function (evt, result) {
	        result.selected = result.selected === true ? false : true;
	        nodeSelectHandler(evt, { event: evt, node: result });
	    };

	    //callback when there are search results 
	    $scope.onSearchResults = function (results) {
	        $scope.searchInfo.results = results;
	        $scope.searchInfo.showSearch = true;
	    };
        
	    $scope.copy = function () {

	        $scope.busy = true;
	        $scope.error = false;

	        contentResource.copy({ parentId: $scope.target.id, id: node.id, relateToOriginal: $scope.relateToOriginal, recursive: $scope.recursive })
                .then(function (path) {
                    $scope.error = false;
                    $scope.success = true;
                    $scope.busy = false;

                    //get the currently edited node (if any)
                    var activeNode = appState.getTreeState("selectedNode");

                    //we need to do a double sync here: first sync to the copied content - but don't activate the node,
                    //then sync to the currenlty edited content (note: this might not be the content that was copied!!)

                    navigationService.syncTree({ tree: "content", path: path, forceReload: true, activate: false }).then(function (args) {
                        if (activeNode) {
                            var activeNodePath = treeService.getPath(activeNode).join();
                            //sync to this node now - depending on what was copied this might already be synced but might not be
                            navigationService.syncTree({ tree: "content", path: activeNodePath, forceReload: false, activate: true });
                        }
                    });

                }, function (err) {
                    $scope.success = false;
                    $scope.error = err;
                    $scope.busy = false;
                });
	    };

	    $scope.dialogTreeEventHandler.bind("treeNodeSelect", nodeSelectHandler);
	    $scope.dialogTreeEventHandler.bind("treeNodeExpanded", nodeExpandedHandler);

	    $scope.$on('$destroy', function () {
	        $scope.dialogTreeEventHandler.unbind("treeNodeSelect", nodeSelectHandler);
	        $scope.dialogTreeEventHandler.unbind("treeNodeExpanded", nodeExpandedHandler);
	    });
	});
/**
 * @ngdoc controller
 * @name Umbraco.Editors.Content.CreateController
 * @function
 * 
 * @description
 * The controller for the content creation dialog
 */
function contentCreateController($scope, $routeParams, contentTypeResource, iconHelper) {

    contentTypeResource.getAllowedTypes($scope.currentNode.id).then(function(data) {
        $scope.allowedTypes = iconHelper.formatContentTypeIcons(data);
    });
}

angular.module('umbraco').controller("Umbraco.Editors.Content.CreateController", contentCreateController);
/**
 * @ngdoc controller
 * @name Umbraco.Editors.ContentDeleteController
 * @function
 * 
 * @description
 * The controller for deleting content
 */
function ContentDeleteController($scope, contentResource, treeService, navigationService, editorState, $location, dialogService, notificationsService) {

    $scope.performDelete = function() {

        //mark it for deletion (used in the UI)
        $scope.currentNode.loading = true;

        contentResource.deleteById($scope.currentNode.id).then(function () {
            $scope.currentNode.loading = false;

            //get the root node before we remove it
            var rootNode = treeService.getTreeRoot($scope.currentNode);

            treeService.removeNode($scope.currentNode);

            if (rootNode) {
                //ensure the recycle bin has child nodes now            
                var recycleBin = treeService.getDescendantNode(rootNode, -20);
                if (recycleBin) {
                    recycleBin.hasChildren = true;
                }
            }
            
            //if the current edited item is the same one as we're deleting, we need to navigate elsewhere
            if (editorState.current && editorState.current.id == $scope.currentNode.id) {

                //If the deleted item lived at the root then just redirect back to the root, otherwise redirect to the item's parent
                var location = "/content";
                if ($scope.currentNode.parentId.toString() !== "-1")
                    location = "/content/content/edit/" + $scope.currentNode.parentId;

                $location.path(location);
            }

            navigationService.hideMenu();
        }, function(err) {

            $scope.currentNode.loading = false;

            //check if response is ysod
            if (err.status && err.status >= 500) {
                dialogService.ysodDialog(err);
            }
            
            if (err.data && angular.isArray(err.data.notifications)) {
                for (var i = 0; i < err.data.notifications.length; i++) {
                    notificationsService.showNotification(err.data.notifications[i]);
                }
            }
        });

    };

    $scope.cancel = function() {
        navigationService.hideDialog();
    };
}

angular.module("umbraco").controller("Umbraco.Editors.Content.DeleteController", ContentDeleteController);

/**
 * @ngdoc controller
 * @name Umbraco.Editors.Content.EditController
 * @function
 * 
 * @description
 * The controller for the content editor
 */
function ContentEditController($scope, $rootScope, $routeParams, $q, $timeout, $window, appState, contentResource, entityResource, navigationService, notificationsService, angularHelper, serverValidationManager, contentEditingHelper, treeService, fileManager, formHelper, umbRequestHelper, keyboardService, umbModelMapper, editorState, $http) {

    //setup scope vars
    $scope.defaultButton = null;
    $scope.subButtons = [];    
    $scope.currentSection = appState.getSectionState("currentSection");
    $scope.currentNode = null; //the editors affiliated node
    $scope.isNew = $routeParams.create;
    
    function init(content) {

        var buttons = contentEditingHelper.configureContentEditorButtons({
            create: $routeParams.create,
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

        editorState.set($scope.content);

        //We fetch all ancestors of the node to generate the footer breadcrumb navigation
        if (!$routeParams.create) {
            if (content.parentId && content.parentId != -1) {
                entityResource.getAncestors(content.id, "document")
               .then(function (anc) {
                   $scope.ancestors = anc;
               });
            }
        }
    }

    /** Syncs the content item to it's tree node - this occurs on first load and after saving */
    function syncTreeNode(content, path, initialLoad) {

        if (!$scope.content.isChildOfListView) {
            navigationService.syncTree({ tree: "content", path: path.split(","), forceReload: initialLoad !== true }).then(function (syncArgs) {
                $scope.currentNode = syncArgs.node;
            });
        }
        else if (initialLoad === true) {

            //it's a child item, just sync the ui node to the parent
            navigationService.syncTree({ tree: "content", path: path.substring(0, path.lastIndexOf(",")).split(","), forceReload: initialLoad !== true });
            
            //if this is a child of a list view and it's the initial load of the editor, we need to get the tree node 
            // from the server so that we can load in the actions menu.
            umbRequestHelper.resourcePromise(
                $http.get(content.treeNodeUrl),
                'Failed to retrieve data for child node ' + content.id).then(function (node) {
                    $scope.currentNode = node;
                });
        }
    }

    // This is a helper method to reduce the amount of code repitition for actions: Save, Publish, SendToPublish
    function performSave(args) {
        var deferred = $q.defer();

        contentEditingHelper.contentEditorPerformSave({
            statusMessage: args.statusMessage,
            saveMethod: args.saveMethod,
            scope: $scope,
            content: $scope.content
        }).then(function (data) {
            //success            
            init($scope.content);
            syncTreeNode($scope.content, data.path);

            deferred.resolve(data);
        }, function (err) {
            //error
            if (err) {
                editorState.set($scope.content);
            }
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

    if ($routeParams.create) {
        //we are creating so get an empty content item
        contentResource.getScaffold($routeParams.id, $routeParams.doctype)
            .then(function (data) {
                $scope.loaded = true;
                $scope.content = data;

                init($scope.content);                

                resetLastListPageNumber($scope.content);
            });
    }
    else {
        //we are editing so get the content item from the server
        contentResource.getById($routeParams.id)
            .then(function (data) {
                $scope.loaded = true;
                $scope.content = data;

                if (data.isChildOfListView && data.trashed === false) {
                    $scope.listViewPath = ($routeParams.page)
                        ? "/content/content/edit/" + data.parentId + "?page=" + $routeParams.page
                        : "/content/content/edit/" + data.parentId;
                }

                init($scope.content);

                //in one particular special case, after we've created a new item we redirect back to the edit
                // route but there might be server validation errors in the collection which we need to display
                // after the redirect, so we will bind all subscriptions which will show the server validation errors
                // if there are any and then clear them so the collection no longer persists them.
                serverValidationManager.executeAndClearAllSubscriptions();

                syncTreeNode($scope.content, data.path, true);

                resetLastListPageNumber($scope.content);
            });
    }


    $scope.unPublish = function () {

        if (formHelper.submitForm({ scope: $scope, statusMessage: "Unpublishing...", skipValidation: true })) {

            contentResource.unPublish($scope.content.id)
                .then(function (data) {

                    formHelper.resetForm({ scope: $scope, notifications: data.notifications });

                    contentEditingHelper.handleSuccessfulSave({
                        scope: $scope,
                        savedContent: data,
                        rebindCallback: contentEditingHelper.reBindChangedProperties($scope.content, data)
                    });

                    init($scope.content);

                    syncTreeNode($scope.content, data.path);

                });
        }

    };

    $scope.sendToPublish = function () {
        return performSave({ saveMethod: contentResource.sendToPublish, statusMessage: "Sending..." });
    };

    $scope.saveAndPublish = function () {
        return performSave({ saveMethod: contentResource.publish, statusMessage: "Publishing..." });
    };

    $scope.save = function () {
        return performSave({ saveMethod: contentResource.save, statusMessage: "Saving..." });
    };

    $scope.preview = function (content) {


        if (!$scope.busy) {

            // Chromes popup blocker will kick in if a window is opened 
            // outwith the initial scoped request. This trick will fix that.
            //  
            var previewWindow = $window.open('preview/?id=' + content.id, 'umbpreview');
            $scope.save().then(function (data) {
                // Build the correct path so both /#/ and #/ work.
                var redirect = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath + '/preview/?id=' + data.id;
                previewWindow.location.href = redirect;
            });


        }
 
    };

    // this method is called for all action buttons and then we proxy based on the btn definition
    $scope.performAction = function (btn) {

        if (!btn || !angular.isFunction(btn.handler)) {
            throw "btn.handler must be a function reference";
        }

        if (!$scope.busy) {
            btn.handler.apply(this);
        }
    };

}

angular.module("umbraco").controller("Umbraco.Editors.Content.EditController", ContentEditController);

/**
 * @ngdoc controller
 * @name Umbraco.Editors.Content.EmptyRecycleBinController
 * @function
 * 
 * @description
 * The controller for deleting content
 */
function ContentEmptyRecycleBinController($scope, contentResource, treeService, navigationService) {

    $scope.performDelete = function() {

        //(used in the UI)
        $scope.currentNode.loading = true;

        contentResource.emptyRecycleBin($scope.currentNode.id).then(function () {
            $scope.currentNode.loading = false;
            //TODO: Need to sync tree, etc...
            treeService.removeChildNodes($scope.currentNode);
            navigationService.hideMenu();
        });

    };

    $scope.cancel = function() {
        navigationService.hideDialog();
    };
}

angular.module("umbraco").controller("Umbraco.Editors.Content.EmptyRecycleBinController", ContentEmptyRecycleBinController);

angular.module("umbraco").controller("Umbraco.Editors.Content.MoveController",
	function ($scope, eventsService, contentResource, navigationService, appState, treeService, localizationService) {

	    var dialogOptions = $scope.dialogOptions;
	    var searchText = "Search...";
	    localizationService.localize("general_search").then(function (value) {
	        searchText = value + "...";
	    });

	    $scope.dialogTreeEventHandler = $({});
	    $scope.busy = false;
	    $scope.searchInfo = {
	        searchFromId: null,
	        searchFromName: null,
	        showSearch: false,
	        results: [],
	        selectedSearchResults: []
	    }

	    var node = dialogOptions.currentNode;

	    function nodeSelectHandler(ev, args) {
	        args.event.preventDefault();
	        args.event.stopPropagation();

	        if (args.node.metaData.listViewNode) {
	            //check if list view 'search' node was selected

	            $scope.searchInfo.showSearch = true;
	            $scope.searchInfo.searchFromId = args.node.metaData.listViewNode.id;
	            $scope.searchInfo.searchFromName = args.node.metaData.listViewNode.name;
	        }
	        else {
	            eventsService.emit("editors.content.moveController.select", args);

	            if ($scope.target) {
	                //un-select if there's a current one selected
	                $scope.target.selected = false;
	            }

	            $scope.target = args.node;
	            $scope.target.selected = true;
	        }	        
	    }

	    function nodeExpandedHandler(ev, args) {
	        if (angular.isArray(args.children)) {

	            //iterate children
	            _.each(args.children, function (child) {
	                //check if any of the items are list views, if so we need to add a custom 
	                // child: A node to activate the search
	                if (child.metaData.isContainer) {
	                    child.hasChildren = true;
	                    child.children = [
	                        {
	                            level: child.level + 1,
	                            hasChildren: false,
	                            name: searchText,
	                            metaData: {
	                                listViewNode: child,
	                            },
	                            cssClass: "icon umb-tree-icon sprTree icon-search",
	                            cssClasses: ["not-published"]
	                        }
	                    ];
	                }
	            });
	        }
	    }

	    $scope.hideSearch = function () {
	        $scope.searchInfo.showSearch = false;
	        $scope.searchInfo.searchFromId = null;
	        $scope.searchInfo.searchFromName = null;
	        $scope.searchInfo.results = [];
	    }

	    // method to select a search result 
	    $scope.selectResult = function (evt, result) {
	        result.selected = result.selected === true ? false : true;
	        nodeSelectHandler(evt, { event: evt, node: result });
	    };

	    //callback when there are search results 
	    $scope.onSearchResults = function (results) {
	        $scope.searchInfo.results = results;
	        $scope.searchInfo.showSearch = true;
	    };

	    $scope.move = function () {

	        $scope.busy = true;
	        $scope.error = false;

	        contentResource.move({ parentId: $scope.target.id, id: node.id })
                .then(function (path) {
                    $scope.error = false;
                    $scope.success = true;
                    $scope.busy = false;

                    //first we need to remove the node that launched the dialog
                    treeService.removeNode($scope.currentNode);

                    //get the currently edited node (if any)
                    var activeNode = appState.getTreeState("selectedNode");

                    //we need to do a double sync here: first sync to the moved content - but don't activate the node,
                    //then sync to the currenlty edited content (note: this might not be the content that was moved!!)

                    navigationService.syncTree({ tree: "content", path: path, forceReload: true, activate: false }).then(function (args) {
                        if (activeNode) {
                            var activeNodePath = treeService.getPath(activeNode).join();
                            //sync to this node now - depending on what was copied this might already be synced but might not be
                            navigationService.syncTree({ tree: "content", path: activeNodePath, forceReload: false, activate: true });
                        }
                    });

                }, function (err) {
                    $scope.success = false;
                    $scope.error = err;
                    $scope.busy = false;
                });
	    };

	    $scope.dialogTreeEventHandler.bind("treeNodeSelect", nodeSelectHandler);
	    $scope.dialogTreeEventHandler.bind("treeNodeExpanded", nodeExpandedHandler);

	    $scope.$on('$destroy', function () {
	        $scope.dialogTreeEventHandler.unbind("treeNodeSelect", nodeSelectHandler);
	        $scope.dialogTreeEventHandler.unbind("treeNodeExpanded", nodeExpandedHandler);
	    });
	});
/**
 * @ngdoc controller
 * @name Umbraco.Editors.Content.RecycleBinController
 * @function
 * 
 * @description
 * Controls the recycle bin for content
 * 
 */

function ContentRecycleBinController($scope, $routeParams, dataTypeResource, navigationService) {

    //ensures the list view doesn't actually load until we query for the list view config
    // for the section
    $scope.listViewPath = null;

    $routeParams.id = "-20";
    dataTypeResource.getById(-95).then(function (result) {
        _.each(result.preValues, function (i) {
            $scope.model.config[i.key] = i.value;
        });
        $scope.listViewPath = 'views/propertyeditors/listview/listview.html';
    });

    $scope.model = { config: { entityType: $routeParams.section } };

    // sync tree node
    navigationService.syncTree({ tree: "content", path: ["-1", $routeParams.id], forceReload: false });
}

angular.module('umbraco').controller("Umbraco.Editors.Content.RecycleBinController", ContentRecycleBinController);

angular.module("umbraco").controller("Umbraco.Editors.Content.RestoreController",
	function ($scope, relationResource, contentResource, navigationService, appState, treeService) {
		var dialogOptions = $scope.dialogOptions;

		var node = dialogOptions.currentNode;

		$scope.error = null;
	    $scope.success = false;

		relationResource.getByChildId(node.id, "relateParentDocumentOnDelete").then(function (data) {

            if (data.length == 0) {
                $scope.success = false;
                $scope.error = {
                    errorMsg: "Cannot automatically restore this item",
                    data: {
                        Message: "There is no 'restore' relation found for this node. Use the Move menu item to move it manually."
                    }
                }
                return;
            }

		    $scope.relation = data[0];

			if ($scope.relation.parentId == -1) {
				$scope.target = { id: -1, name: "Root" };

			} else {
			    contentResource.getById($scope.relation.parentId).then(function (data) {
					$scope.target = data;

				}, function (err) {
					$scope.success = false;
					$scope.error = err;
				});
			}

		}, function (err) {
			$scope.success = false;
			$scope.error = err;
		});

		$scope.restore = function () {
			// this code was copied from `content.move.controller.js`
			contentResource.move({ parentId: $scope.target.id, id: node.id })
				.then(function (path) {

					$scope.success = true;

					//first we need to remove the node that launched the dialog
					treeService.removeNode($scope.currentNode);

					//get the currently edited node (if any)
					var activeNode = appState.getTreeState("selectedNode");

					//we need to do a double sync here: first sync to the moved content - but don't activate the node,
					//then sync to the currenlty edited content (note: this might not be the content that was moved!!)

					navigationService.syncTree({ tree: "content", path: path, forceReload: true, activate: false }).then(function (args) {
						if (activeNode) {
							var activeNodePath = treeService.getPath(activeNode).join();
							//sync to this node now - depending on what was copied this might already be synced but might not be
							navigationService.syncTree({ tree: "content", path: activeNodePath, forceReload: false, activate: true });
						}
					});

				}, function (err) {
					$scope.success = false;
					$scope.error = err;
				});
		};
	});
/**
 * @ngdoc controller
 * @name Umbraco.Editors.ContentType.EditController
 * @function
 * 
 * @description
 * The controller for the content type editor
 */
function ContentTypeEditController($scope, $routeParams, $log, notificationsService, angularHelper, serverValidationManager, contentEditingHelper, entityResource) {
    
    $scope.tabs = [];
    $scope.page = {};
    $scope.contentType = {tabs: [], name: "My content type", alias:"myType", icon:"icon-folder", allowedChildren: [], allowedTemplate: []};
    $scope.contentType.tabs = [
            {name: "Content", properties:[ {name: "test"}]},
            {name: "Generic Properties", properties:[]}
        ];


        
    $scope.dataTypesOptions ={
    	group: "properties",
    	onDropHandler: function(item, args){
    		args.sourceScope.move(args);
    	},
    	onReleaseHandler: function(item, args){
    		var a = args;
    	}
    };

    $scope.tabOptions ={
    	group: "tabs",
    	drop: false,
    	nested: true,
    	onDropHandler: function(item, args){
    		
    	},
    	onReleaseHandler: function(item, args){
    		
    	}
    };

    $scope.propertiesOptions ={
    	group: "properties",
    	onDropHandler: function(item, args){
    		//alert("dropped on properties");
			//args.targetScope.ngModel.$modelValue.push({name: "bong"});
    	},
    	onReleaseHandler: function(item, args){
    		//alert("released from properties");
			//args.targetScope.ngModel.$modelValue.push({name: "bong"});
    	},
    };


    $scope.omg = function(){
    	alert("wat");
    };   

    entityResource.getAll("Datatype").then(function(data){
        $scope.page.datatypes = data;
    });
}

angular.module("umbraco").controller("Umbraco.Editors.ContentType.EditController", ContentTypeEditController);
function startUpVideosDashboardController($scope, xmlhelper, $log, $http) {
    $scope.videos = [];
    $scope.init = function(url){
        var proxyUrl = "dashboard/feedproxy.aspx?url=" + url;
        $http.get(proxyUrl).then(function(data){
              var feed = $(data.data);
              $('item', feed).each(function (i, item) {
                  var video = {};
                  video.thumbnail = $(item).find('thumbnail').attr('url');
                  video.title = $("title", item).text();
                  video.link = $("guid", item).text();
                  $scope.videos.push(video);
              });
        });
    };
}
angular.module("umbraco").controller("Umbraco.Dashboard.StartupVideosController", startUpVideosDashboardController);


function FormsController($scope, $route, $cookieStore, packageResource) {
    $scope.installForms = function(){
        $scope.state = "Installng package";
        packageResource
            .fetch("CD44CF39-3D71-4C19-B6EE-948E1FAF0525")
            .then(function(pack){
              $scope.state = "importing";
              return packageResource.import(pack);
            }, $scope.error)
            .then(function(pack){
              $scope.state = "Installing";
              return packageResource.installFiles(pack);
            }, $scope.error)
            .then(function(pack){
              $scope.state = "Restarting, please hold...";
              return packageResource.installData(pack);
            }, $scope.error)
            .then(function(pack){
              $scope.state = "All done, your browser will now refresh";
              return packageResource.cleanUp(pack);
            }, $scope.error)
            .then($scope.complete, $scope.error);
    };

    $scope.complete = function(result){
        var url = window.location.href + "?init=true";
        $cookieStore.put("umbPackageInstallId", result.packageGuid); 
        window.location.reload(true);
    };

    $scope.error = function(err){
        $scope.state = undefined;
        $scope.error = err;
    };


    function Video_player (videoId) {
      // Get dom elements
      this.container      = document.getElementById(videoId);
      this.video          = this.container.getElementsByTagName('video')[0];

      //Create controls
      this.controls = document.createElement('div');
      this.controls.className="video-controls";

      this.seek_bar = document.createElement('input');
      this.seek_bar.className="seek-bar";
      this.seek_bar.type="range";
      this.seek_bar.setAttribute('value', '0');

      this.loader = document.createElement('div');
      this.loader.className="loader";

      this.progress_bar = document.createElement('span');
      this.progress_bar.className="progress-bar";

      // Insert controls
      this.controls.appendChild(this.seek_bar);
      this.container.appendChild(this.controls);
      this.controls.appendChild(this.loader);
      this.loader.appendChild(this.progress_bar);
    }


    Video_player.prototype
      .seeking = function() {
        // get the value of the seekbar (hidden input[type="range"])
        var time = this.video.duration * (this.seek_bar.value / 100);

        // Update video to seekbar value
        this.video.currentTime = time;
      };

    // Stop video when user initiates seeking
    Video_player.prototype
      .start_seek = function() {
        this.video.pause();
      };

    // Start video when user stops seeking
    Video_player.prototype
      .stop_seek = function() {
        this.video.play();
      };

    // Update the progressbar (span.loader) according to video.currentTime
    Video_player.prototype
      .update_progress_bar = function() {
        // Get video progress in %
        var value = (100 / this.video.duration) * this.video.currentTime;

        // Update progressbar
        this.progress_bar.style.width = value + '%';
      };

    // Bind progressbar to mouse when seeking
    Video_player.prototype
      .handle_mouse_move = function(event) {
        // Get position of progressbar relative to browser window
        var pos = this.progress_bar.getBoundingClientRect().left;

        // Make sure event is reckonized cross-browser
        event = event || window.event;

        // Update progressbar
        this.progress_bar.style.width = (event.clientX - pos) + "px";
      };

    // Eventlisteners for seeking
    Video_player.prototype
      .video_event_handler = function(videoPlayer, interval) {
        // Update the progress bar
        var animate_progress_bar = setInterval(function () {
              videoPlayer.update_progress_bar();
            }, interval);

        // Fire when input value changes (user seeking)
        videoPlayer.seek_bar
          .addEventListener("change", function() {
              videoPlayer.seeking();
          });

        // Fire when user clicks on seekbar
        videoPlayer.seek_bar
          .addEventListener("mousedown", function (clickEvent) {
              // Pause video playback
              videoPlayer.start_seek();

              // Stop updating progressbar according to video progress
              clearInterval(animate_progress_bar);

              // Update progressbar to where user clicks
              videoPlayer.handle_mouse_move(clickEvent);

              // Bind progressbar to cursor
              window.onmousemove = function(moveEvent){
                videoPlayer.handle_mouse_move(moveEvent);
              };
          });

        // Fire when user releases seekbar
        videoPlayer.seek_bar
          .addEventListener("mouseup", function () {

              // Unbind progressbar from cursor
              window.onmousemove = null;

              // Start video playback
              videoPlayer.stop_seek();

              // Animate the progressbar
              animate_progress_bar = setInterval(function () {
                  videoPlayer.update_progress_bar();
              }, interval);
          });
      };


    var videoPlayer = new Video_player('video_1');
    videoPlayer.video_event_handler(videoPlayer, 17);
}

angular.module("umbraco").controller("Umbraco.Dashboard.FormsDashboardController", FormsController);

function startupLatestEditsController($scope) {

}
angular.module("umbraco").controller("Umbraco.Dashboard.StartupLatestEditsController", startupLatestEditsController);

function MediaFolderBrowserDashboardController($rootScope, $scope, assetsService, $routeParams, $timeout, $element, $location, umbRequestHelper,navigationService, mediaResource, $cookies) {
        var dialogOptions = $scope.dialogOptions;

        $scope.filesUploading = [];
        $scope.nodeId = -1;

        $scope.onUploadComplete = function () {
            navigationService.reloadSection("media");
        }

}
angular.module("umbraco").controller("Umbraco.Dashboard.MediaFolderBrowserDashboardController", MediaFolderBrowserDashboardController);


function ChangePasswordDashboardController($scope, xmlhelper, $log, currentUserResource, formHelper) {

    //create the initial model for change password property editor
    $scope.changePasswordModel = {
        alias: "_umb_password",
        view: "changepassword",
        config: {},
        value: {}
    };

    //go get the config for the membership provider and add it to the model
    currentUserResource.getMembershipProviderConfig().then(function(data) {
        $scope.changePasswordModel.config = data;
        //ensure the hasPassword config option is set to true (the user of course has a password already assigned)
        //this will ensure the oldPassword is shown so they can change it
        $scope.changePasswordModel.config.hasPassword = true;
        $scope.changePasswordModel.config.disableToggle = true;
    });

    ////this is the model we will pass to the service
    //$scope.profile = {};

    $scope.changePassword = function() {

        if (formHelper.submitForm({ scope: $scope })) {
            currentUserResource.changePassword($scope.changePasswordModel.value).then(function(data) {

                //if the password has been reset, then update our model
                if (data.value) {
                    $scope.changePasswordModel.value.generatedPassword = data.value;
                }

                formHelper.resetForm({ scope: $scope, notifications: data.notifications });

            }, function (err) {

                formHelper.handleError(err);

            });
        }
    };
}
angular.module("umbraco").controller("Umbraco.Dashboard.StartupChangePasswordController", ChangePasswordDashboardController);

function examineMgmtController($scope, umbRequestHelper, $log, $http, $q, $timeout) {

    $scope.indexerDetails = [];
    $scope.searcherDetails = [];
    $scope.loading = true;

    function checkProcessing(indexer, checkActionName) {
        umbRequestHelper.resourcePromise(
                $http.post(umbRequestHelper.getApiUrl("examineMgmtBaseUrl", checkActionName, { indexerName: indexer.name })),
                'Failed to check index processing')
            .then(function(data) {

                if (data !== null && data !== "null") {

                    //copy all resulting properties
                    for (var k in data) {
                        indexer[k] = data[k];
                    }
                    indexer.isProcessing = false;
                }
                else {
                    $timeout(function () {
                        //don't continue if we've tried 100 times
                        if (indexer.processingAttempts < 100) {
                            checkProcessing(indexer, checkActionName);
                            //add an attempt
                            indexer.processingAttempts++;
                        }
                        else {
                            //we've exceeded 100 attempts, stop processing
                            indexer.isProcessing = false;
                        }
                    }, 1000);
                }
            });
    }

    $scope.search = function (searcher, e) {
        if (e && e.keyCode !== 13) {
            return;
        }

        umbRequestHelper.resourcePromise(
                $http.get(umbRequestHelper.getApiUrl("examineMgmtBaseUrl", "GetSearchResults", {
                    searcherName: searcher.name,
                    query: encodeURIComponent(searcher.searchText),
                    queryType: searcher.searchType
                })),
                'Failed to search')
            .then(function(searchResults) {
                searcher.isSearching = true;
                searcher.searchResults = searchResults;
            });
    }
    
    $scope.toggle = function(provider, propName) {
        if (provider[propName] !== undefined) {
            provider[propName] = !provider[propName];
        }
        else {
            provider[propName] = true;
        }
    }

    $scope.rebuildIndex = function(indexer) {
        if (confirm("This will cause the index to be rebuilt. " +
                        "Depending on how much content there is in your site this could take a while. " +
                        "It is not recommended to rebuild an index during times of high website traffic " +
                        "or when editors are editing content.")) {

            indexer.isProcessing = true;
            indexer.processingAttempts = 0;

            umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl("examineMgmtBaseUrl", "PostRebuildIndex", { indexerName: indexer.name })),
                    'Failed to rebuild index')
                .then(function () {

                    //rebuilding has started, nothing is returned accept a 200 status code.
                    //lets poll to see if it is done.
                    $timeout(function () {
                        checkProcessing(indexer, "PostCheckRebuildIndex");
                    }, 1000);

                });
        }
    }

    $scope.optimizeIndex = function(indexer) {
        if (confirm("This will cause the index to be optimized which will improve its performance. " +
                        "It is not recommended to optimize an index during times of high website traffic " +
                        "or when editors are editing content.")) {
            indexer.isProcessing = true;

            umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl("examineMgmtBaseUrl", "PostOptimizeIndex", { indexerName: indexer.name })),
                    'Failed to optimize index')
                .then(function () {

                    //optimizing has started, nothing is returned accept a 200 status code.
                    //lets poll to see if it is done.
                    $timeout(function () {
                        checkProcessing(indexer, "PostCheckOptimizeIndex");
                    }, 1000);

                });
        }
    }

    $scope.closeSearch = function(searcher) {
        searcher.isSearching = true;
    }

     
    //go get the data

    //combine two promises and execute when they are both done
    $q.all([

        //get the indexer details
        umbRequestHelper.resourcePromise(
            $http.get(umbRequestHelper.getApiUrl("examineMgmtBaseUrl", "GetIndexerDetails")),
            'Failed to retrieve indexer details')
        .then(function(data) {
            $scope.indexerDetails = data; 
        }),

        //get the searcher details
        umbRequestHelper.resourcePromise(
            $http.get(umbRequestHelper.getApiUrl("examineMgmtBaseUrl", "GetSearcherDetails")),
            'Failed to retrieve searcher details')
        .then(function(data) {
            $scope.searcherDetails = data;
            for (var s in $scope.searcherDetails) {
                $scope.searcherDetails[s].searchType = "text";
            }
        })

    ]).then(function () {
        //all init loading is complete
        $scope.loading = false;
    });


}
angular.module("umbraco").controller("Umbraco.Dashboard.ExamineMgmtController", examineMgmtController);
function xmlDataIntegrityReportController($scope, umbRequestHelper, $log, $http, $q, $timeout) {

    function check(item) {
        var action = item.check;
        umbRequestHelper.resourcePromise(
                $http.get(umbRequestHelper.getApiUrl("xmlDataIntegrityBaseUrl", action)),
                'Failed to retrieve data integrity status')
            .then(function(result) {
                item.checking = false;
                item.invalid = result === "false";
            });
    }

    $scope.fix = function(item) {
        var action = item.fix;
        if (item.fix) {
            if (confirm("This will cause all xml structures for this type to be rebuilt. " +
                "Depending on how much content there is in your site this could take a while. " +
                "It is not recommended to rebuild xml structures if they are not out of sync, during times of high website traffic " +
                "or when editors are editing content.")) {
                item.fixing = true;
                umbRequestHelper.resourcePromise(
                    $http.post(umbRequestHelper.getApiUrl("xmlDataIntegrityBaseUrl", action)),
                    'Failed to retrieve data integrity status')
                .then(function (result) {
                    item.fixing = false;
                    item.invalid = result === "false";
                });
            }
        }
    }

    $scope.items = {
        "contentXml": {
            label: "Content in the cmsContentXml table",
            checking: true,
            fixing: false,
            fix: "FixContentXmlTable",
            check: "CheckContentXmlTable"
        },
        "mediaXml": {
            label: "Media in the cmsContentXml table",
            checking: true,
            fixing: false,
            fix: "FixMediaXmlTable",
            check: "CheckMediaXmlTable"
        },
        "memberXml": {
            label: "Members in the cmsContentXml table",
            checking: true,
            fixing: false,
            fix: "FixMembersXmlTable",
            check: "CheckMembersXmlTable"
        }
    };

    for (var i in $scope.items) {
        check($scope.items[i]);
    }

}
angular.module("umbraco").controller("Umbraco.Dashboard.XmlDataIntegrityReportController", xmlDataIntegrityReportController);
/**
 * @ngdoc controller
 * @name Umbraco.Editors.ContentDeleteController
 * @function
 * 
 * @description
 * The controller for deleting content
 */
function DataTypeDeleteController($scope, dataTypeResource, treeService, navigationService) {

    $scope.performDelete = function() {

        //mark it for deletion (used in the UI)
        $scope.currentNode.loading = true;
        dataTypeResource.deleteById($scope.currentNode.id).then(function () {
            $scope.currentNode.loading = false;

            //get the root node before we remove it
            var rootNode = treeService.getTreeRoot($scope.currentNode);
            
            //TODO: Need to sync tree, etc...
            treeService.removeNode($scope.currentNode);
            navigationService.hideMenu();
        });

    };

    $scope.cancel = function() {
        navigationService.hideDialog();
    };
}

angular.module("umbraco").controller("Umbraco.Editors.DataType.DeleteController", DataTypeDeleteController);

/**
 * @ngdoc controller
 * @name Umbraco.Editors.DataType.EditController
 * @function
 * 
 * @description
 * The controller for the content editor
 */
function DataTypeEditController($scope, $routeParams, $location, appState, navigationService, treeService, dataTypeResource, notificationsService,  angularHelper, serverValidationManager, contentEditingHelper, formHelper, editorState) {

    //setup scope vars    
    $scope.currentSection = appState.getSectionState("currentSection");
    $scope.currentNode = null; //the editors affiliated node

    //method used to configure the pre-values when we retrieve them from the server
    function createPreValueProps(preVals) {
        $scope.preValues = [];
        for (var i = 0; i < preVals.length; i++) {
            $scope.preValues.push({
                hideLabel: preVals[i].hideLabel,
                alias: preVals[i].key,
                description: preVals[i].description,
                label: preVals[i].label,
                view: preVals[i].view,
                value: preVals[i].value
            });
        }
    }

    //set up the standard data type props
    $scope.properties = {
        selectedEditor: {
            alias: "selectedEditor",
            description: "Select a property editor",
            label: "Property editor"
        },
        selectedEditorId: {
            alias: "selectedEditorId",
            label: "Property editor alias"
        }
    };
    
    //setup the pre-values as props
    $scope.preValues = [];

    if ($routeParams.create) {
        //we are creating so get an empty data type item
        dataTypeResource.getScaffold()
            .then(function(data) {
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                $scope.content = data;

                //set a shared state
                editorState.set($scope.content);
            });
    }
    else {
        //we are editing so get the content item from the server
        dataTypeResource.getById($routeParams.id)
            .then(function(data) {
                $scope.loaded = true;
                $scope.preValuesLoaded = true;
                $scope.content = data;

                createPreValueProps($scope.content.preValues);
                
                //share state
                editorState.set($scope.content);

                //in one particular special case, after we've created a new item we redirect back to the edit
                // route but there might be server validation errors in the collection which we need to display
                // after the redirect, so we will bind all subscriptions which will show the server validation errors
                // if there are any and then clear them so the collection no longer persists them.
                serverValidationManager.executeAndClearAllSubscriptions();
                
                navigationService.syncTree({ tree: "datatype", path: [String(data.id)] }).then(function (syncArgs) {
                    $scope.currentNode = syncArgs.node;
                });
            });
    }
    
    $scope.$watch("content.selectedEditor", function (newVal, oldVal) {

        //when the value changes, we need to dynamically load in the new editor
        if (newVal && (newVal != oldVal && (oldVal || $routeParams.create))) {
            //we are editing so get the content item from the server
            var currDataTypeId = $routeParams.create ? undefined : $routeParams.id;
            dataTypeResource.getPreValues(newVal, currDataTypeId)
                .then(function (data) {
                    $scope.preValuesLoaded = true;
                    $scope.content.preValues = data;
                    createPreValueProps($scope.content.preValues);
                    
                    //share state
                    editorState.set($scope.content);
                });
        }
    });

    $scope.save = function() {

        if (formHelper.submitForm({ scope: $scope, statusMessage: "Saving..." })) {
            
            dataTypeResource.save($scope.content, $scope.preValues, $routeParams.create)
                .then(function(data) {

                    formHelper.resetForm({ scope: $scope, notifications: data.notifications });

                    contentEditingHelper.handleSuccessfulSave({
                        scope: $scope,
                        savedContent: data,
                        rebindCallback: function() {
                            createPreValueProps(data.preValues);
                        }
                    });

                    //share state
                    editorState.set($scope.content);

                    navigationService.syncTree({ tree: "datatype", path: [String(data.id)], forceReload: true }).then(function (syncArgs) {
                        $scope.currentNode = syncArgs.node;
                    });
                    
                }, function(err) {

                    //NOTE: in the case of data type values we are setting the orig/new props 
                    // to be the same thing since that only really matters for content/media.
                    contentEditingHelper.handleSaveError({
                        redirectOnFailure: false,
                        err: err
                    });
                    
                    //share state
                    editorState.set($scope.content);
                });
        }

    };

}

angular.module("umbraco").controller("Umbraco.Editors.DataType.EditController", DataTypeEditController);

/**
 * @ngdoc controller
 * @name Umbraco.Editors.Media.CreateController
 * @function
 * 
 * @description
 * The controller for the media creation dialog
 */
function mediaCreateController($scope, $routeParams, mediaTypeResource, iconHelper) {
    
    mediaTypeResource.getAllowedTypes($scope.currentNode.id).then(function(data) {
        $scope.allowedTypes = iconHelper.formatContentTypeIcons(data);
    });
    
}

angular.module('umbraco').controller("Umbraco.Editors.Media.CreateController", mediaCreateController);
/**
 * @ngdoc controller
 * @name Umbraco.Editors.ContentDeleteController
 * @function
 * 
 * @description
 * The controller for deleting content
 */
function MediaDeleteController($scope, mediaResource, treeService, navigationService, editorState, $location, dialogService, notificationsService) {

    $scope.performDelete = function() {

        //mark it for deletion (used in the UI)
        $scope.currentNode.loading = true;

        mediaResource.deleteById($scope.currentNode.id).then(function () {
            $scope.currentNode.loading = false;

            //get the root node before we remove it
            var rootNode = treeService.getTreeRoot($scope.currentNode);

            treeService.removeNode($scope.currentNode);

            if (rootNode) {
                //ensure the recycle bin has child nodes now            
                var recycleBin = treeService.getDescendantNode(rootNode, -21);
                if (recycleBin) {
                    recycleBin.hasChildren = true;
                }
            }
            
            //if the current edited item is the same one as we're deleting, we need to navigate elsewhere
            if (editorState.current && editorState.current.id == $scope.currentNode.id) {

            	//If the deleted item lived at the root then just redirect back to the root, otherwise redirect to the item's parent
            	var location = "/media";
            	if ($scope.currentNode.parentId.toString() !== "-1")
            		location = "/media/media/edit/" + $scope.currentNode.parentId;

                $location.path(location);
            }

            navigationService.hideMenu();

        }, function (err) {

            $scope.currentNode.loading = false;

            //check if response is ysod
            if (err.status && err.status >= 500) {
                dialogService.ysodDialog(err);
            }

            if (err.data && angular.isArray(err.data.notifications)) {
                for (var i = 0; i < err.data.notifications.length; i++) {
                    notificationsService.showNotification(err.data.notifications[i]);
                }
            }
        });
    };

    $scope.cancel = function() {
        navigationService.hideDialog();
    };
}

angular.module("umbraco").controller("Umbraco.Editors.Media.DeleteController", MediaDeleteController);

/**
 * @ngdoc controller
 * @name Umbraco.Editors.Media.EditController
 * @function
 * 
 * @description
 * The controller for the media editor
 */
function mediaEditController($scope, $routeParams, appState, mediaResource, entityResource, navigationService, notificationsService, angularHelper, serverValidationManager, contentEditingHelper, fileManager, treeService, formHelper, umbModelMapper, editorState, umbRequestHelper, $http) {

    //setup scope vars
    $scope.currentSection = appState.getSectionState("currentSection");
    $scope.currentNode = null; //the editors affiliated node

    /** Syncs the content item to it's tree node - this occurs on first load and after saving */
    function syncTreeNode(content, path, initialLoad) {

        if (!$scope.content.isChildOfListView) {
            navigationService.syncTree({ tree: "media", path: path.split(","), forceReload: initialLoad !== true }).then(function (syncArgs) {
                $scope.currentNode = syncArgs.node;
            });
        }
        else if (initialLoad === true) {

            //it's a child item, just sync the ui node to the parent
            navigationService.syncTree({ tree: "media", path: path.substring(0, path.lastIndexOf(",")).split(","), forceReload: initialLoad !== true });

            //if this is a child of a list view and it's the initial load of the editor, we need to get the tree node 
            // from the server so that we can load in the actions menu.
            umbRequestHelper.resourcePromise(
                $http.get(content.treeNodeUrl),
                'Failed to retrieve data for child node ' + content.id).then(function (node) {
                    $scope.currentNode = node;
                });
        }
    }

    if ($routeParams.create) {

        mediaResource.getScaffold($routeParams.id, $routeParams.doctype)
            .then(function (data) {
                $scope.loaded = true;
                $scope.content = data;

                editorState.set($scope.content);
            });
    }
    else {
        mediaResource.getById($routeParams.id)
            .then(function (data) {
                $scope.loaded = true;
                $scope.content = data;
                
                if (data.isChildOfListView && data.trashed === false) {
                    $scope.listViewPath = ($routeParams.page)
                        ? "/media/media/edit/" + data.parentId + "?page=" + $routeParams.page
                        : "/media/media/edit/" + data.parentId;
                }

                editorState.set($scope.content);

                //in one particular special case, after we've created a new item we redirect back to the edit
                // route but there might be server validation errors in the collection which we need to display
                // after the redirect, so we will bind all subscriptions which will show the server validation errors
                // if there are any and then clear them so the collection no longer persists them.
                serverValidationManager.executeAndClearAllSubscriptions();

                syncTreeNode($scope.content, data.path, true);
               
                if ($scope.content.parentId && $scope.content.parentId != -1) {
                    //We fetch all ancestors of the node to generate the footer breadcrump navigation
                    entityResource.getAncestors($routeParams.id, "media")
                        .then(function (anc) {
                            $scope.ancestors = anc;
                        });
                }

            });  
    }
    
    $scope.save = function () {

        if (!$scope.busy && formHelper.submitForm({ scope: $scope, statusMessage: "Saving..." })) {

            $scope.busy = true;

            mediaResource.save($scope.content, $routeParams.create, fileManager.getFiles())
                .then(function(data) {

                    formHelper.resetForm({ scope: $scope, notifications: data.notifications });

                    contentEditingHelper.handleSuccessfulSave({
                        scope: $scope,
                        savedContent: data,
                        rebindCallback: contentEditingHelper.reBindChangedProperties($scope.content, data)
                    });

                    editorState.set($scope.content);
                    $scope.busy = false;

                    syncTreeNode($scope.content, data.path);

                }, function(err) {

                    contentEditingHelper.handleSaveError({
                        err: err,
                        redirectOnFailure: true,
                        rebindCallback: contentEditingHelper.reBindChangedProperties($scope.content, err.data)
                    });
                    
                    //show any notifications
                    if (angular.isArray(err.data.notifications)) {
                        for (var i = 0; i < err.data.notifications.length; i++) {
                            notificationsService.showNotification(err.data.notifications[i]);
                        }
                    }

                    editorState.set($scope.content);
                    $scope.busy = false;
                });
        }else{
            $scope.busy = false;
        }
        
    };
}

angular.module("umbraco")
    .controller("Umbraco.Editors.Media.EditController", mediaEditController);

/**
 * @ngdoc controller
 * @name Umbraco.Editors.Media.EmptyRecycleBinController
 * @function
 * 
 * @description
 * The controller for deleting media
 */
function MediaEmptyRecycleBinController($scope, mediaResource, treeService, navigationService) {

    $scope.performDelete = function() {

        //(used in the UI)
        $scope.currentNode.loading = true;

        mediaResource.emptyRecycleBin($scope.currentNode.id).then(function () {
            $scope.currentNode.loading = false;
            //TODO: Need to sync tree, etc...
            treeService.removeChildNodes($scope.currentNode);
            navigationService.hideMenu();
        });

    };

    $scope.cancel = function() {
        navigationService.hideDialog();
    };
}

angular.module("umbraco").controller("Umbraco.Editors.Media.EmptyRecycleBinController", MediaEmptyRecycleBinController);

//used for the media picker dialog
angular.module("umbraco").controller("Umbraco.Editors.Media.MoveController",
	function ($scope, eventsService, mediaResource, appState, treeService, navigationService) {
	    var dialogOptions = $scope.dialogOptions;

	    $scope.dialogTreeEventHandler = $({});
	    var node = dialogOptions.currentNode;

	    function nodeSelectHandler(ev, args) {
	        args.event.preventDefault();
	        args.event.stopPropagation();

	        eventsService.emit("editors.media.moveController.select", args);

	        if ($scope.target) {
	            //un-select if there's a current one selected
	            $scope.target.selected = false;
	        }

	        $scope.target = args.node;
	        $scope.target.selected = true;
	    }

	    $scope.dialogTreeEventHandler.bind("treeNodeSelect", nodeSelectHandler);


	    $scope.move = function () {
	        mediaResource.move({ parentId: $scope.target.id, id: node.id })
                .then(function (path) {
                    $scope.error = false;
                    $scope.success = true;

                    //first we need to remove the node that launched the dialog
                    treeService.removeNode($scope.currentNode);

                    //get the currently edited node (if any)
                    var activeNode = appState.getTreeState("selectedNode");

                    //we need to do a double sync here: first sync to the moved content - but don't activate the node,
                    //then sync to the currenlty edited content (note: this might not be the content that was moved!!)

                    navigationService.syncTree({ tree: "media", path: path, forceReload: true, activate: false }).then(function (args) {
                        if (activeNode) {
                            var activeNodePath = treeService.getPath(activeNode).join();
                            //sync to this node now - depending on what was copied this might already be synced but might not be
                            navigationService.syncTree({ tree: "media", path: activeNodePath, forceReload: false, activate: true });
                        }
                    });

                }, function (err) {
                    $scope.success = false;
                    $scope.error = err;
                });
	    };

	    $scope.$on('$destroy', function () {
	        $scope.dialogTreeEventHandler.unbind("treeNodeSelect", nodeSelectHandler);
	    });
	});
/**
 * @ngdoc controller
 * @name Umbraco.Editors.Content.MediaRecycleBinController
 * @function
 * 
 * @description
 * Controls the recycle bin for media
 * 
 */

function MediaRecycleBinController($scope, $routeParams, dataTypeResource, navigationService) {

    //ensures the list view doesn't actually load until we query for the list view config
    // for the section
    $scope.listViewPath = null;

    $routeParams.id = "-21";
    dataTypeResource.getById(-96).then(function (result) {
        _.each(result.preValues, function (i) {
            $scope.model.config[i.key] = i.value;
        });
        $scope.listViewPath = 'views/propertyeditors/listview/listview.html';
    });

    $scope.model = { config: { entityType: $routeParams.section } };

    // sync tree node
    navigationService.syncTree({ tree: "media", path: ["-1", $routeParams.id], forceReload: false });
}

angular.module('umbraco').controller("Umbraco.Editors.Media.RecycleBinController", MediaRecycleBinController);

/**
 * @ngdoc controller
 * @name Umbraco.Editors.Member.CreateController
 * @function
 * 
 * @description
 * The controller for the member creation dialog
 */
function memberCreateController($scope, $routeParams, memberTypeResource, iconHelper) {
    
    memberTypeResource.getTypes($scope.currentNode.id).then(function (data) {
        $scope.allowedTypes = iconHelper.formatContentTypeIcons(data);
    });
    
}

angular.module('umbraco').controller("Umbraco.Editors.Member.CreateController", memberCreateController);
/**
 * @ngdoc controller
 * @name Umbraco.Editors.Member.DeleteController
 * @function
 * 
 * @description
 * The controller for deleting content
 */
function MemberDeleteController($scope, memberResource, treeService, navigationService, editorState, $location, $routeParams) {

    $scope.performDelete = function() {

        //mark it for deletion (used in the UI)
        $scope.currentNode.loading = true;

        memberResource.deleteByKey($scope.currentNode.id).then(function () {
            $scope.currentNode.loading = false;

            treeService.removeNode($scope.currentNode);
            
            //if the current edited item is the same one as we're deleting, we need to navigate elsewhere
            if (editorState.current && editorState.current.key == $scope.currentNode.id) {
                $location.path("/member/member/list/" + ($routeParams.listName ? $routeParams.listName : 'all-members'));
            }

            navigationService.hideMenu();
        });

    };

    $scope.cancel = function() {
        navigationService.hideDialog();
    };
}

angular.module("umbraco").controller("Umbraco.Editors.Member.DeleteController", MemberDeleteController);

/**
 * @ngdoc controller
 * @name Umbraco.Editors.Member.EditController
 * @function
 * 
 * @description
 * The controller for the member editor
 */
function MemberEditController($scope, $routeParams, $location, $q, $window, appState, memberResource, entityResource, navigationService, notificationsService, angularHelper, serverValidationManager, contentEditingHelper, fileManager, formHelper, umbModelMapper, editorState, umbRequestHelper, $http) {
    
    //setup scope vars
    $scope.currentSection = appState.getSectionState("currentSection");
    $scope.currentNode = null; //the editors affiliated node

    $scope.listViewPath = ($routeParams.page && $routeParams.listName)
        ? "/member/member/list/" + $routeParams.listName + "?page=" + $routeParams.page
        : null;

    //build a path to sync the tree with
    function buildTreePath(data) {
        return $routeParams.listName ? "-1," + $routeParams.listName : "-1";
    }

    if ($routeParams.create) {
        
        //if there is no doc type specified then we are going to assume that 
        // we are not using the umbraco membership provider
        if ($routeParams.doctype) {
            //we are creating so get an empty member item
            memberResource.getScaffold($routeParams.doctype)
                .then(function(data) {
                    $scope.loaded = true;
                    $scope.content = data;

                    editorState.set($scope.content);
                });
        }
        else {
            memberResource.getScaffold()
                .then(function (data) {
                    $scope.loaded = true;
                    $scope.content = data;

                    editorState.set($scope.content);
                });
        }
        
    }
    else {
        //so, we usually refernce all editors with the Int ID, but with members we have
        //a different pattern, adding a route-redirect here to handle this: 
        //isNumber doesnt work here since its seen as a string

        //TODO: Why is this here - I don't understand why this would ever be an integer? This will not work when we support non-umbraco membership providers.

        if ($routeParams.id && $routeParams.id.length < 9) {
            entityResource.getById($routeParams.id, "Member").then(function(entity) {
                $location.path("member/member/edit/" + entity.key);
            });
        }
        else {
            //we are editing so get the content item from the server
            memberResource.getByKey($routeParams.id)
                .then(function(data) {
                    $scope.loaded = true;
                    $scope.content = data;

                    editorState.set($scope.content);
                    
                    var path = buildTreePath(data);

                    //sync the tree (only for ui purposes)
                    navigationService.syncTree({ tree: "member", path: path.split(",") });

                    //it's the initial load of the editor, we need to get the tree node 
                    // from the server so that we can load in the actions menu.
                    umbRequestHelper.resourcePromise(
                        $http.get(data.treeNodeUrl),
                        'Failed to retrieve data for child node ' + data.key).then(function (node) {
                            $scope.currentNode = node;
                        });

                    //in one particular special case, after we've created a new item we redirect back to the edit
                    // route but there might be server validation errors in the collection which we need to display
                    // after the redirect, so we will bind all subscriptions which will show the server validation errors
                    // if there are any and then clear them so the collection no longer persists them.
                    serverValidationManager.executeAndClearAllSubscriptions();
                });
        }

    }
    
    $scope.save = function() {

        if (!$scope.busy && formHelper.submitForm({ scope: $scope, statusMessage: "Saving..." })) {
            
            $scope.busy = true;

            memberResource.save($scope.content, $routeParams.create, fileManager.getFiles())
                .then(function(data) {

                    formHelper.resetForm({ scope: $scope, notifications: data.notifications });

                    contentEditingHelper.handleSuccessfulSave({
                        scope: $scope,
                        savedContent: data,
                        //specify a custom id to redirect to since we want to use the GUID
                        redirectId: data.key,
                        rebindCallback: contentEditingHelper.reBindChangedProperties($scope.content, data)
                    });
                    
                    editorState.set($scope.content);
                    $scope.busy = false;
                    
                    var path = buildTreePath(data);

                    //sync the tree (only for ui purposes)
                    navigationService.syncTree({ tree: "member", path: path.split(","), forceReload: true });

            }, function (err) {
                    
                    contentEditingHelper.handleSaveError({
                        redirectOnFailure: false,
                        err: err,
                        rebindCallback: contentEditingHelper.reBindChangedProperties($scope.content, err.data)
                    });
                    
                    editorState.set($scope.content);
                    $scope.busy = false;
                });
        }else{
            $scope.busy = false;
        }
        
    };

}

angular.module("umbraco").controller("Umbraco.Editors.Member.EditController", MemberEditController);

/**
 * @ngdoc controller
 * @name Umbraco.Editors.Member.ListController
 * @function
 * 
 * @description
 * The controller for the member list view
 */
function MemberListController($scope, $routeParams, $location, $q, $window, appState, memberResource, entityResource, navigationService, notificationsService, angularHelper, serverValidationManager, contentEditingHelper, fileManager, formHelper, umbModelMapper, editorState, localizationService) {
    
    //setup scope vars
    $scope.currentSection = appState.getSectionState("currentSection");
    $scope.currentNode = null; //the editors affiliated node
    
    //we are editing so get the content item from the server
    memberResource.getListNode($routeParams.id)
        .then(function (data) {
            $scope.loaded = true;
            $scope.content = data;

            //translate "All Members"
            if ($scope.content != null && $scope.content.name != null && $scope.content.name.replace(" ", "").toLowerCase() == "allmembers") {
                localizationService.localize("member_allMembers").then(function (value) {
                    $scope.content.name = value;
                });
            }

            editorState.set($scope.content);

            navigationService.syncTree({ tree: "member", path: data.path.split(",") }).then(function (syncArgs) {
                $scope.currentNode = syncArgs.node;
            });

            //in one particular special case, after we've created a new item we redirect back to the edit
            // route but there might be server validation errors in the collection which we need to display
            // after the redirect, so we will bind all subscriptions which will show the server validation errors
            // if there are any and then clear them so the collection no longer persists them.
            serverValidationManager.executeAndClearAllSubscriptions();
        });
}

angular.module("umbraco").controller("Umbraco.Editors.Member.ListController", MemberListController);

function imageFilePickerController($scope, dialogService, mediaHelper) {

    $scope.pick = function() {
        dialogService.mediaPicker({
            multiPicker: false,
            callback: function(data) {
                 $scope.model.value = mediaHelper.resolveFile(data, false);
            }
        });
    };

}

angular.module('umbraco').controller("Umbraco.PrevalueEditors.ImageFilePickerController",imageFilePickerController);

//this controller simply tells the dialogs service to open a mediaPicker window
//with a specified callback, this callback will receive an object with a selection on it
function mediaPickerController($scope, dialogService, entityResource, $log, iconHelper) {

    function trim(str, chr) {
        var rgxtrim = (!chr) ? new RegExp('^\\s+|\\s+$', 'g') : new RegExp('^' + chr + '+|' + chr + '+$', 'g');
        return str.replace(rgxtrim, '');
    }

    $scope.renderModel = [];   

    var dialogOptions = {
        multiPicker: false,
        entityType: "Media",
        section: "media",
        treeAlias: "media",
        callback: function(data) {
            if (angular.isArray(data)) {
                _.each(data, function (item, i) {
                    $scope.add(item);
                });
            }
            else {
                $scope.clear();
                $scope.add(data);
            }
        }
    };

    $scope.openContentPicker = function(){
        var d = dialogService.treePicker(dialogOptions);
    };

    $scope.remove =function(index){
        $scope.renderModel.splice(index, 1);
    };

    $scope.clear = function() {
        $scope.renderModel = [];
    };

    $scope.add = function (item) {
        var currIds = _.map($scope.renderModel, function (i) {
            return i.id;
        });
        if (currIds.indexOf(item.id) < 0) {
            item.icon = iconHelper.convertFromLegacyIcon(item.icon);
            $scope.renderModel.push({name: item.name, id: item.id, icon: item.icon});
        }	
    };

    var unsubscribe = $scope.$on("formSubmitting", function (ev, args) {
        var currIds = _.map($scope.renderModel, function (i) {
            return i.id;
        });
        $scope.model.value = trim(currIds.join(), ",");
    });

    //when the scope is destroyed we need to unsubscribe
    $scope.$on('$destroy', function () {
        unsubscribe();
    });

    //load media data
    var modelIds = $scope.model.value ? $scope.model.value.split(',') : [];
    entityResource.getByIds(modelIds, dialogOptions.entityType).then(function (data) {
        _.each(data, function (item, i) {
            item.icon = iconHelper.convertFromLegacyIcon(item.icon);
            $scope.renderModel.push({ name: item.name, id: item.id, icon: item.icon });
        });
    });
    
}

angular.module('umbraco').controller("Umbraco.PrevalueEditors.MediaPickerController",mediaPickerController);
angular.module("umbraco").controller("Umbraco.PrevalueEditors.MultiValuesController",
    function ($scope, $timeout) {
       
        //NOTE: We need to make each item an object, not just a string because you cannot 2-way bind to a primitive.

        $scope.newItem = "";
        $scope.hasError = false;
       
        if (!angular.isArray($scope.model.value)) {

            //make an array from the dictionary
            var items = [];
            for (var i in $scope.model.value) { 
                items.push({ 
                    value: $scope.model.value[i].value,
                    sortOrder: $scope.model.value[i].sortOrder,
                    id: i
                });
            }

            //ensure the items are sorted by the provided sort order
            items.sort(function (a, b) { return (a.sortOrder > b.sortOrder) ? 1 : ((b.sortOrder > a.sortOrder) ? -1 : 0); });
            
            //now make the editor model the array
            $scope.model.value = items;
        }

        $scope.remove = function(item, evt) {
            evt.preventDefault();

            $scope.model.value = _.reject($scope.model.value, function (x) {
                return x.value === item.value;
            });
            
        };

        $scope.add = function (evt) {
            evt.preventDefault();
            
            
            if ($scope.newItem) {
                if (!_.contains($scope.model.value, $scope.newItem)) {                
                    $scope.model.value.push({ value: $scope.newItem });
                    $scope.newItem = "";
                    $scope.hasError = false;
                    return;
                }
            }

            //there was an error, do the highlight (will be set back by the directive)
            $scope.hasError = true;            
        };

        $scope.sortableOptions = {
            axis: 'y',
            containment: 'parent',
            cursor: 'move',
            items: '> div.control-group',
            tolerance: 'pointer',
            update: function (e, ui) {
                // Get the new and old index for the moved element (using the text as the identifier, so 
                // we'd have a problem if two prevalues were the same, but that would be unlikely)
                var newIndex = ui.item.index();
                var movedPrevalueText = $('input[type="text"]', ui.item).val();
                var originalIndex = getElementIndexByPrevalueText(movedPrevalueText);

                // Move the element in the model
                if (originalIndex > -1) {
                    var movedElement = $scope.model.value[originalIndex];
                    $scope.model.value.splice(originalIndex, 1);
                    $scope.model.value.splice(newIndex, 0, movedElement);
                }
            }
        };

        function getElementIndexByPrevalueText(value) {
            for (var i = 0; i < $scope.model.value.length; i++) {
                if ($scope.model.value[i].value === value) {
                    return i;
                }
            }

            return -1;
        }

    });

//this controller simply tells the dialogs service to open a mediaPicker window
//with a specified callback, this callback will receive an object with a selection on it
angular.module('umbraco')
.controller("Umbraco.PrevalueEditors.TreePickerController",
	
	function($scope, dialogService, entityResource, $log, iconHelper){
		$scope.renderModel = [];
		$scope.ids = [];


	    var config = {
	        multiPicker: false,
	        entityType: "Document",
	        type: "content",
	        treeAlias: "content"
	    };
		
		if($scope.model.value){
			$scope.ids = $scope.model.value.split(',');
			entityResource.getByIds($scope.ids, config.entityType).then(function (data) {
			    _.each(data, function (item, i) {
					item.icon = iconHelper.convertFromLegacyIcon(item.icon);
					$scope.renderModel.push({name: item.name, id: item.id, icon: item.icon});
				});
			});
		}
		

		$scope.openContentPicker =function() {
		    var d = dialogService.treePicker({
		        section: config.type,
		        treeAlias: config.treeAlias,
		        multiPicker: config.multiPicker,
		        callback: populate
		    });
		};

		$scope.remove =function(index){
			$scope.renderModel.splice(index, 1);
			$scope.ids.splice(index, 1);
			$scope.model.value = trim($scope.ids.join(), ",");
		};

		$scope.clear = function() {
		    $scope.model.value = "";
		    $scope.renderModel = [];
		    $scope.ids = [];
		};
		
		$scope.add =function(item){
			if($scope.ids.indexOf(item.id) < 0){
				item.icon = iconHelper.convertFromLegacyIcon(item.icon);

				$scope.ids.push(item.id);
				$scope.renderModel.push({name: item.name, id: item.id, icon: item.icon});
				$scope.model.value = trim($scope.ids.join(), ",");
			}	
		};


	    var unsubscribe = $scope.$on("formSubmitting", function (ev, args) {
			$scope.model.value = trim($scope.ids.join(), ",");
	    });

	    //when the scope is destroyed we need to unsubscribe
	    $scope.$on('$destroy', function () {
	        unsubscribe();
	    });

		function trim(str, chr) {
			var rgxtrim = (!chr) ? new RegExp('^\\s+|\\s+$', 'g') : new RegExp('^'+chr+'+|'+chr+'+$', 'g');
			return str.replace(rgxtrim, '');
		}

		function populate(data){
			if(angular.isArray(data)){
			    _.each(data, function (item, i) {
					$scope.add(item);
				});
			}else{
				$scope.clear();
				$scope.add(data);
			}
		}
});
//this controller simply tells the dialogs service to open a mediaPicker window
//with a specified callback, this callback will receive an object with a selection on it
angular.module('umbraco')
.controller("Umbraco.PrevalueEditors.TreeSourceController",
	
	function($scope, dialogService, entityResource, $log, iconHelper){

	    if (!$scope.model) {
	        $scope.model = {};
	    }
	    if (!$scope.model.value) {
	        $scope.model.value = {
	            type: "content"
	        };
	    }

		if($scope.model.value.id && $scope.model.value.type !== "member"){
			var ent = "Document";
			if($scope.model.value.type === "media"){
				ent = "Media";
			}
			
			entityResource.getById($scope.model.value.id, ent).then(function(item){
				item.icon = iconHelper.convertFromLegacyIcon(item.icon);
				$scope.node = item;
			});
		}


		$scope.openContentPicker =function(){
			var d = dialogService.treePicker({
								section: $scope.model.value.type,
								treeAlias: $scope.model.value.type,
								multiPicker: false,
								callback: populate});
		};

		$scope.clear = function() {
			$scope.model.value.id = undefined;
			$scope.node = undefined;
			$scope.model.value.query = undefined;
		};
		

		//we always need to ensure we dont submit anything broken
	    var unsubscribe = $scope.$on("formSubmitting", function (ev, args) {
	    	if($scope.model.value.type === "member"){
	    		$scope.model.value.id = -1;
	    		$scope.model.value.query = "";
	    	}
	    });

	    //when the scope is destroyed we need to unsubscribe
	    $scope.$on('$destroy', function () {
	        unsubscribe();
	    });

		function populate(item){
				$scope.clear();
				item.icon = iconHelper.convertFromLegacyIcon(item.icon);
				$scope.node = item;
				$scope.model.value.id = item.id;
		}
});
function booleanEditorController($scope, $rootScope, assetsService) {

    function setupViewModel() {
        $scope.renderModel = {
            value: false
        };
        if ($scope.model && $scope.model.value && ($scope.model.value.toString() === "1" || angular.lowercase($scope.model.value) === "true")) {
            $scope.renderModel.value = true;
        }
    }

    setupViewModel();

    $scope.$watch("renderModel.value", function (newVal) {
        $scope.model.value = newVal === true ? "1" : "0";
    });
    
    //here we declare a special method which will be called whenever the value has changed from the server
    //this is instead of doing a watch on the model.value = faster
    $scope.model.onValueChanged = function (newVal, oldVal) {
        //update the display val again if it has changed from the server
        setupViewModel();
    };

}
angular.module("umbraco").controller("Umbraco.PropertyEditors.BooleanController", booleanEditorController);
angular.module("umbraco").controller("Umbraco.PropertyEditors.ChangePasswordController",
    function ($scope, $routeParams) {
        
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

            //set defaults if they are not available
            if (!$scope.model.config || $scope.model.config.disableToggle === undefined) {
                $scope.model.config.disableToggle = false;
            }
            if (!$scope.model.config || $scope.model.config.hasPassword === undefined) {
                $scope.model.config.hasPassword = false;
            }
            if (!$scope.model.config || $scope.model.config.enablePasswordRetrieval === undefined) {
                $scope.model.config.enablePasswordRetrieval = true;
            }
            if (!$scope.model.config || $scope.model.config.requiresQuestionAnswer === undefined) {
                $scope.model.config.requiresQuestionAnswer = false;
            }
            if (!$scope.model.config || $scope.model.config.enableReset === undefined) {
                $scope.model.config.enableReset = true;
            }
            if (!$scope.model.config || $scope.model.config.minPasswordLength === undefined) {
                $scope.model.config.minPasswordLength = 0;
            }
            
            //set the model defaults
            if (!angular.isObject($scope.model.value)) {
                //if it's not an object then just create a new one
                $scope.model.value = {
                    newPassword: null,
                    oldPassword: null,
                    reset: null,
                    answer: null
                };
            }
            else {
                //just reset the values

                if (!isNew) {
                    //if it is new, then leave the generated pass displayed
                    $scope.model.value.newPassword = null;
                    $scope.model.value.oldPassword = null;
                }
                $scope.model.value.reset = null;
                $scope.model.value.answer = null;
            }

            //the value to compare to match passwords
            if (!isNew) {
                $scope.model.confirm = "";
            }
            else if ($scope.model.value.newPassword && $scope.model.value.newPassword.length > 0) {
                //if it is new and a new password has been set, then set the confirm password too
                $scope.model.confirm = $scope.model.value.newPassword;
            }
            
        }

        resetModel($routeParams.create);

        //if there is no password saved for this entity , it must be new so we do not allow toggling of the change password, it is always there
        //with validators turned on.
        $scope.changing = $scope.model.config.disableToggle === true || !$scope.model.config.hasPassword;

        //we're not currently changing so set the model to null
        if (!$scope.changing) {
            $scope.model.value = null;
        }

        $scope.doChange = function() {
            resetModel();
            $scope.changing = true;
            //if there was a previously generated password displaying, clear it
            $scope.model.value.generatedPassword = null;
        };

        $scope.cancelChange = function() {
            $scope.changing = false;
            //set model to null
            $scope.model.value = null;
        };

        var unsubscribe = [];

        //listen for the saved event, when that occurs we'll 
        //change to changing = false;
        unsubscribe.push($scope.$on("formSubmitted", function() {
            if ($scope.model.config.disableToggle === false) {
                $scope.changing = false;
            }
        }));
        unsubscribe.push($scope.$on("formSubmitting", function() {
            //if there was a previously generated password displaying, clear it
            if ($scope.changing && $scope.model.value) {
                $scope.model.value.generatedPassword = null;
            }
            else if (!$scope.changing) {
                //we are not changing, so the model needs to be null
                $scope.model.value = null;
            }
        }));

        //when the scope is destroyed we need to unsubscribe
        $scope.$on('$destroy', function () {
            for (var u in unsubscribe) {
                unsubscribe[u]();
            }
        });

        $scope.showReset = function() {
            return $scope.model.config.hasPassword && $scope.model.config.enableReset;
        };

        $scope.showOldPass = function() {
            return $scope.model.config.hasPassword &&
                !$scope.model.config.allowManuallyChangingPassword &&
                !$scope.model.config.enablePasswordRetrieval && !$scope.model.value.reset;
        };

        $scope.showNewPass = function () {
            return !$scope.model.value.reset;
        };

        $scope.showConfirmPass = function() {
            return !$scope.model.value.reset;
        };
        
        $scope.showCancelBtn = function() {
            return $scope.model.config.disableToggle !== true && $scope.model.config.hasPassword;
        };

    });

angular.module("umbraco").controller("Umbraco.PropertyEditors.CheckboxListController",
    function($scope) {
        
        if (angular.isObject($scope.model.config.items)) {
            
            //now we need to format the items in the dictionary because we always want to have an array
            var newItems = [];
            var vals = _.values($scope.model.config.items);
            var keys = _.keys($scope.model.config.items);
            for (var i = 0; i < vals.length; i++) {
                newItems.push({ id: keys[i], sortOrder: vals[i].sortOrder, value: vals[i].value });
            }

            //ensure the items are sorted by the provided sort order
            newItems.sort(function (a, b) { return (a.sortOrder > b.sortOrder) ? 1 : ((b.sortOrder > a.sortOrder) ? -1 : 0); });

            //re-assign
            $scope.model.config.items = newItems;

        }

        function setupViewModel() {
            $scope.selectedItems = [];

            //now we need to check if the value is null/undefined, if it is we need to set it to "" so that any value that is set
            // to "" gets selected by default
            if ($scope.model.value === null || $scope.model.value === undefined) {
                $scope.model.value = [];
            }

            for (var i = 0; i < $scope.model.config.items.length; i++) {
                var isChecked = _.contains($scope.model.value, $scope.model.config.items[i].id);
                $scope.selectedItems.push({
                    checked: isChecked,
                    key: $scope.model.config.items[i].id,
                    val: $scope.model.config.items[i].value
                });
            }

        }

        setupViewModel();
        

        //update the model when the items checked changes
        $scope.$watch("selectedItems", function(newVal, oldVal) {

            $scope.model.value = [];
            for (var x = 0; x < $scope.selectedItems.length; x++) {
                if ($scope.selectedItems[x].checked) {
                    $scope.model.value.push($scope.selectedItems[x].key);
                }
            }

        }, true);
        
        //here we declare a special method which will be called whenever the value has changed from the server
        //this is instead of doing a watch on the model.value = faster
        $scope.model.onValueChanged = function (newVal, oldVal) {
            //update the display val again if it has changed from the server
            setupViewModel();
        };

    });

function ColorPickerController($scope) {
    $scope.toggleItem = function (color) {
        if ($scope.model.value == color) {
            $scope.model.value = "";
            //this is required to re-validate
            $scope.propertyForm.modelValue.$setViewValue($scope.model.value);
        }
        else {
            $scope.model.value = color;
            //this is required to re-validate
            $scope.propertyForm.modelValue.$setViewValue($scope.model.value);
        }
    };
    // Method required by the valPropertyValidator directive (returns true if the property editor has at least one color selected)
    $scope.validateMandatory = function () {
        return {
            isValid: !$scope.model.validation.mandatory || ($scope.model.value != null && $scope.model.value != ""),
            errorMsg: "Value cannot be empty",
            errorKey: "required"
        };
    }
    $scope.isConfigured = $scope.model.config && $scope.model.config.items && _.keys($scope.model.config.items).length > 0;
}

angular.module("umbraco").controller("Umbraco.PropertyEditors.ColorPickerController", ColorPickerController);

angular.module("umbraco").controller("Umbraco.PrevalueEditors.MultiColorPickerController",
    function ($scope, $timeout, assetsService, angularHelper, $element) {
        //NOTE: We need to make each color an object, not just a string because you cannot 2-way bind to a primitive.
        var defaultColor = "000000";
        
        $scope.newColor = defaultColor;
        $scope.hasError = false;

        assetsService.load([
            //"lib/spectrum/tinycolor.js",
            "lib/spectrum/spectrum.js"          
        ], $scope).then(function () {
            var elem = $element.find("input");
            elem.spectrum({
                color: null,
                showInitial: false,
                chooseText: "choose", // TODO: These can be localised
                cancelText: "cancel", // TODO: These can be localised
                preferredFormat: "hex",
                showInput: true,
                clickoutFiresChange: true,
                hide: function (color) {
                    //show the add butotn
                    $element.find(".btn.add").show();                    
                },
                change: function (color) {
                    angularHelper.safeApply($scope, function () {
                        $scope.newColor = color.toHexString().trimStart("#"); // #ff0000
                    });
                },
                show: function() {
                    //hide the add butotn
                    $element.find(".btn.add").hide();
                }
            });
        });

        if (!angular.isArray($scope.model.value)) {
            //make an array from the dictionary
            var items = [];
            for (var i in $scope.model.value) {
                items.push({
                    value: $scope.model.value[i],
                    id: i
                });
            }
            //now make the editor model the array
            $scope.model.value = items;
        }

        $scope.remove = function (item, evt) {

            evt.preventDefault();

            $scope.model.value = _.reject($scope.model.value, function (x) {
                return x.value === item.value;
            });

        };

        $scope.add = function (evt) {

            evt.preventDefault();

            if ($scope.newColor) {
                var exists = _.find($scope.model.value, function(item) {
                    return item.value.toUpperCase() == $scope.newColor.toUpperCase();
                });
                if (!exists) {
                    $scope.model.value.push({ value: $scope.newColor });
                    //$scope.newColor = defaultColor;
                    // set colorpicker to default color
                    //var elem = $element.find("input");
                    //elem.spectrum("set", $scope.newColor);
                    $scope.hasError = false;
                    return;
                }

                //there was an error, do the highlight (will be set back by the directive)
                $scope.hasError = true;
            }

        };

        //load the separate css for the editor to avoid it blocking our js loading
        assetsService.loadCss("lib/spectrum/spectrum.css");
    });

//this controller simply tells the dialogs service to open a mediaPicker window
//with a specified callback, this callback will receive an object with a selection on it

function contentPickerController($scope, dialogService, entityResource, editorState, $log, iconHelper, $routeParams, fileManager, contentEditingHelper, angularHelper, navigationService, $location) {

    function trim(str, chr) {
        var rgxtrim = (!chr) ? new RegExp('^\\s+|\\s+$', 'g') : new RegExp('^' + chr + '+|' + chr + '+$', 'g');
        return str.replace(rgxtrim, '');
    }

    function startWatch() {
        //We need to watch our renderModel so that we can update the underlying $scope.model.value properly, this is required
        // because the ui-sortable doesn't dispatch an event after the digest of the sort operation. Any of the events for UI sortable
        // occur after the DOM has updated but BEFORE the digest has occured so the model has NOT changed yet - it even states so in the docs.
        // In their source code there is no event so we need to just subscribe to our model changes here.
        //This also makes it easier to manage models, we update one and the rest will just work.
        $scope.$watch(function () {
            //return the joined Ids as a string to watch
            return _.map($scope.renderModel, function (i) {
                return i.id;
            }).join();
        }, function (newVal) {
            var currIds = _.map($scope.renderModel, function (i) {
                return i.id;
            });
            $scope.model.value = trim(currIds.join(), ",");

            //Validate!
            if ($scope.model.config && $scope.model.config.minNumber && parseInt($scope.model.config.minNumber) > $scope.renderModel.length) {
                $scope.contentPickerForm.minCount.$setValidity("minCount", false);
            }
            else {
                $scope.contentPickerForm.minCount.$setValidity("minCount", true);
            }

            if ($scope.model.config && $scope.model.config.maxNumber && parseInt($scope.model.config.maxNumber) < $scope.renderModel.length) {
                $scope.contentPickerForm.maxCount.$setValidity("maxCount", false);
            }
            else {
                $scope.contentPickerForm.maxCount.$setValidity("maxCount", true);
            }
        });
    }

    $scope.renderModel = [];
	    
    $scope.dialogEditor = editorState && editorState.current && editorState.current.isDialogEditor === true;

    //the default pre-values
    var defaultConfig = {
        multiPicker: false,
        showOpenButton: false,
        showEditButton: false,
        showPathOnHover: false,
        startNode: {
            query: "",
            type: "content",
	            id: $scope.model.config.startNodeId ? $scope.model.config.startNodeId : -1 // get start node for simple Content Picker
        }
    };

    if ($scope.model.config) {
        //merge the server config on top of the default config, then set the server config to use the result
        $scope.model.config = angular.extend(defaultConfig, $scope.model.config);
    }

    //Umbraco persists boolean for prevalues as "0" or "1" so we need to convert that!
    $scope.model.config.multiPicker = ($scope.model.config.multiPicker === "1" ? true : false);
    $scope.model.config.showOpenButton = ($scope.model.config.showOpenButton === "1" ? true : false);
    $scope.model.config.showEditButton = ($scope.model.config.showEditButton === "1" ? true : false);
    $scope.model.config.showPathOnHover = ($scope.model.config.showPathOnHover === "1" ? true : false);
 
    var entityType = $scope.model.config.startNode.type === "member"
        ? "Member"
        : $scope.model.config.startNode.type === "media"
        ? "Media"
        : "Document";
    $scope.allowOpenButton = entityType === "Document" || entityType === "Media";
    $scope.allowEditButton = entityType === "Document";

    //the dialog options for the picker
    var dialogOptions = {
        multiPicker: $scope.model.config.multiPicker,
        entityType: entityType,
        filterCssClass: "not-allowed not-published",
        startNodeId: null,
        callback: function (data) {
            if (angular.isArray(data)) {
                _.each(data, function (item, i) {
                    $scope.add(item);
                });
            } else {
                $scope.clear();
                $scope.add(data);
            }
        angularHelper.getCurrentForm($scope).$setDirty();
        },
        treeAlias: $scope.model.config.startNode.type,
        section: $scope.model.config.startNode.type
    };

    //since most of the pre-value config's are used in the dialog options (i.e. maxNumber, minNumber, etc...) we'll merge the 
    // pre-value config on to the dialog options
    angular.extend(dialogOptions, $scope.model.config);

    //We need to manually handle the filter for members here since the tree displayed is different and only contains
    // searchable list views
    if (entityType === "Member") {
        //first change the not allowed filter css class
        dialogOptions.filterCssClass = "not-allowed";
        var currFilter = dialogOptions.filter;
        //now change the filter to be a method
        dialogOptions.filter = function(i) {
            //filter out the list view nodes
            if (i.metaData.isContainer) {
                return true;
            }
            if (!currFilter) {
                return false;
            }
            //now we need to filter based on what is stored in the pre-vals, this logic duplicates what is in the treepicker.controller, 
            // but not much we can do about that since members require special filtering.
            var filterItem = currFilter.toLowerCase().split(',');
            var found = filterItem.indexOf(i.metaData.contentType.toLowerCase()) >= 0;
            if (!currFilter.startsWith("!") && !found || currFilter.startsWith("!") && found) {
                return true;
            }

            return false;
        }
    }


    //if we have a query for the startnode, we will use that. 
    if ($scope.model.config.startNode.query) {
        var rootId = $routeParams.id;
        entityResource.getByQuery($scope.model.config.startNode.query, rootId, "Document").then(function (ent) {
            dialogOptions.startNodeId = ent.id;
        });
    } else {
        dialogOptions.startNodeId = $scope.model.config.startNode.id;
    }        
    
    //dialog
    $scope.openContentPicker = function () {                
        var d = dialogService.treePicker(dialogOptions);
    };

    $scope.remove = function (index) {
        $scope.renderModel.splice(index, 1);
        angularHelper.getCurrentForm($scope).$setDirty();
    };

    $scope.showNode = function (index) {
        var item = $scope.renderModel[index];
        var id = item.id;
        var section = $scope.model.config.startNode.type.toLowerCase();

        entityResource.getPath(id, entityType).then(function (path) {
            navigationService.changeSection(section);
            navigationService.showTree(section, {
                tree: section, path: path, forceReload: false, activate: true
            });
            var routePath = section + "/" + section + "/edit/" + id.toString();
            $location.path(routePath).search("");
        });
    }
        
    $scope.add = function (item) {
        var currIds = _.map($scope.renderModel, function (i) {
            return i.id;
        });

        if (currIds.indexOf(item.id) < 0) {
            item.icon = iconHelper.convertFromLegacyIcon(item.icon);
            $scope.renderModel.push({ name: item.name, id: item.id, icon: item.icon, path: item.path });
        }
    };

    $scope.clear = function () {
        $scope.renderModel = [];
    };
        
    var unsubscribe = $scope.$on("formSubmitting", function (ev, args) {
        var currIds = _.map($scope.renderModel, function (i) {
            return i.id;
        });
        $scope.model.value = trim(currIds.join(), ",");
    });

    //when the scope is destroyed we need to unsubscribe
    $scope.$on('$destroy', function () {
        unsubscribe();
    });

    //load current data
    var modelIds = $scope.model.value ? $scope.model.value.split(',') : [];
    entityResource.getByIds(modelIds, entityType).then(function (data) {

        //Ensure we populate the render model in the same order that the ids were stored!
        _.each(modelIds, function (id, i) {
            var entity = _.find(data, function (d) {                
                return d.id == id;
            });
           
            if (entity) {
                entity.icon = iconHelper.convertFromLegacyIcon(entity.icon);
                $scope.renderModel.push({ name: entity.name, id: entity.id, icon: entity.icon, path: entity.path });
            }
            
           
        });

        //everything is loaded, start the watch on the model
        startWatch();

    });
}

angular.module('umbraco').controller("Umbraco.PropertyEditors.ContentPickerController", contentPickerController);

function dateTimePickerController($scope, notificationsService, assetsService, angularHelper, userService, $element) {

    //setup the default config
    var config = {
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

    //map the user config
    $scope.model.config = angular.extend(config, $scope.model.config);
    //ensure the format doesn't get overwritten with an empty string
    if ($scope.model.config.format === "" || $scope.model.config.format === undefined || $scope.model.config.format === null) {
        $scope.model.config.format = $scope.model.config.pickTime ? "YYYY-MM-DD HH:mm:ss" : "YYYY-MM-DD";
    }

    $scope.hasDatetimePickerValue = $scope.model.value ? true : false;
    $scope.datetimePickerValue = null;

    //hide picker if clicking on the document 
    $scope.hidePicker = function () {
        //$element.find("div:first").datetimepicker("hide");
        // Sometimes the statement above fails and generates errors in the browser console. The following statements fix that.
        var dtp = $element.find("div:first");
        if (dtp && dtp.datetimepicker) {
            dtp.datetimepicker("hide");
        }
    };
    $(document).bind("click", $scope.hidePicker);

    //handles the date changing via the api
    function applyDate(e) {
        angularHelper.safeApply($scope, function() {
            // when a date is changed, update the model
            if (e.date && e.date.isValid()) {
                $scope.datePickerForm.datepicker.$setValidity("pickerError", true);
                $scope.hasDatetimePickerValue = true;
                $scope.datetimePickerValue = e.date.format($scope.model.config.format);
            }
            else {
                $scope.hasDatetimePickerValue = false;
                $scope.datetimePickerValue = null;
            }
            
            if (!$scope.model.config.pickTime) {
                $element.find("div:first").datetimepicker("hide", 0);
            }
        });
    }

    var picker = null;

    $scope.clearDate = function() {
        $scope.hasDatetimePickerValue = false;
        $scope.datetimePickerValue = null;
        $scope.model.value = null;
        $scope.datePickerForm.datepicker.$setValidity("pickerError", true);
    }

    //get the current user to see if we can localize this picker
    userService.getCurrentUser().then(function (user) {

        assetsService.loadCss('lib/datetimepicker/bootstrap-datetimepicker.min.css').then(function() {

        var filesToLoad = ["lib/moment/moment-with-locales.js",
						   "lib/datetimepicker/bootstrap-datetimepicker.js"];

            
		$scope.model.config.language = user.locale;
		

		assetsService.load(filesToLoad, $scope).then(
            function () {
				//The Datepicker js and css files are available and all components are ready to use.

				// Get the id of the datepicker button that was clicked
				var pickerId = $scope.model.alias;

			    var element = $element.find("div:first");

				// Open the datepicker and add a changeDate eventlistener
			    element
			        .datetimepicker(angular.extend({ useCurrent: true }, $scope.model.config))
			        .on("dp.change", applyDate)
			        .on("dp.error", function(a, b, c) {
			            $scope.hasDatetimePickerValue = false;
			            $scope.datePickerForm.datepicker.$setValidity("pickerError", false);
			        });

			    if ($scope.hasDatetimePickerValue) {

			        //assign value to plugin/picker
			        var dateVal = $scope.model.value ? moment($scope.model.value, $scope.model.config.format) : moment();
			        element.datetimepicker("setValue", dateVal);
			        $scope.datetimePickerValue = moment($scope.model.value).format($scope.model.config.format);
			    }

			    element.find("input").bind("blur", function() {
			        //we need to force an apply here
			        $scope.$apply();
			    });

				//Ensure to remove the event handler when this instance is destroyted
			    $scope.$on('$destroy', function () {
			        element.find("input").unbind("blur");
					element.datetimepicker("destroy");
			    });


			    var unsubscribe = $scope.$on("formSubmitting", function (ev, args) {
			        if ($scope.hasDatetimePickerValue) {
			            var elementData = $element.find("div:first").data().DateTimePicker;
			            if ($scope.model.config.pickTime) {
			                $scope.model.value = elementData.getDate().format("YYYY-MM-DD HH:mm:ss");
			            }
			            else {
			                $scope.model.value = elementData.getDate().format("YYYY-MM-DD");
			            }
			        }
			        else {
			            $scope.model.value = null;
			        }
			    });
			    //unbind doc click event!
			    $scope.$on('$destroy', function () {
			        unsubscribe();
			    });


			});
        });
        
    });

    var unsubscribe = $scope.$on("formSubmitting", function (ev, args) {
        if ($scope.hasDatetimePickerValue) {
            if ($scope.model.config.pickTime) {
                $scope.model.value = $element.find("div:first").data().DateTimePicker.getDate().format("YYYY-MM-DD HH:mm:ss");
            }
            else {
                $scope.model.value = $element.find("div:first").data().DateTimePicker.getDate().format("YYYY-MM-DD");
            }
        }
        else {
            $scope.model.value = null;
        }
    });

    //unbind doc click event!
    $scope.$on('$destroy', function () {
        $(document).unbind("click", $scope.hidePicker);
        unsubscribe();
    });
}

angular.module("umbraco").controller("Umbraco.PropertyEditors.DatepickerController", dateTimePickerController);

angular.module("umbraco").controller("Umbraco.PropertyEditors.DropdownController",
    function($scope) {

        //setup the default config
        var config = {
            items: [],
            multiple: false
        };

        //map the user config
        angular.extend(config, $scope.model.config);

        //map back to the model
        $scope.model.config = config;
        
        function convertArrayToDictionaryArray(model){
            //now we need to format the items in the dictionary because we always want to have an array
            var newItems = [];
            for (var i = 0; i < model.length; i++) {
                newItems.push({ id: model[i], sortOrder: 0, value: model[i] });
            }

            return newItems;
        }


        function convertObjectToDictionaryArray(model){
            //now we need to format the items in the dictionary because we always want to have an array
            var newItems = [];
            var vals = _.values($scope.model.config.items);
            var keys = _.keys($scope.model.config.items);

            for (var i = 0; i < vals.length; i++) {
                var label = vals[i].value ? vals[i].value : vals[i]; 
                newItems.push({ id: keys[i], sortOrder: vals[i].sortOrder, value: label });
            }

            return newItems;
        }

        if (angular.isArray($scope.model.config.items)) {
            //PP: I dont think this will happen, but we have tests that expect it to happen..
            //if array is simple values, convert to array of objects
            if(!angular.isObject($scope.model.config.items[0])){
                $scope.model.config.items = convertArrayToDictionaryArray($scope.model.config.items);
            }
        }
        else if (angular.isObject($scope.model.config.items)) {
            $scope.model.config.items = convertObjectToDictionaryArray($scope.model.config.items);
        }
        else {
            throw "The items property must be either an array or a dictionary";
        }
        

        //sort the values
        $scope.model.config.items.sort(function (a, b) { return (a.sortOrder > b.sortOrder) ? 1 : ((b.sortOrder > a.sortOrder) ? -1 : 0); });

        //now we need to check if the value is null/undefined, if it is we need to set it to "" so that any value that is set
        // to "" gets selected by default
        if ($scope.model.value === null || $scope.model.value === undefined) {
            if ($scope.model.config.multiple) {
                $scope.model.value = [];
            }
            else {
                $scope.model.value = "";
            }
        }
        
    });

/** A drop down list or multi value select list based on an entity type, this can be re-used for any entity types */
function entityPicker($scope, entityResource) {

    //set the default to DocumentType
    if (!$scope.model.config.entityType) {
        $scope.model.config.entityType = "DocumentType";
    }

    //Determine the select list options and which value to publish
    if (!$scope.model.config.publishBy) {
        $scope.selectOptions = "entity.id as entity.name for entity in entities";
    }
    else {
        $scope.selectOptions = "entity." + $scope.model.config.publishBy + " as entity.name for entity in entities";
    }

    entityResource.getAll($scope.model.config.entityType).then(function (data) {
        //convert the ids to strings so the drop downs work properly when comparing
        _.each(data, function(d) {
            d.id = d.id.toString();
        });
        $scope.entities = data;
    });

    if ($scope.model.value === null || $scope.model.value === undefined) {
        if ($scope.model.config.multiple) {
            $scope.model.value = [];
        }
        else {
            $scope.model.value = "";
        }
    }
    else {
        //if it's multiple, change the value to an array
        if ($scope.model.config.multiple === "1") {
            if (_.isString($scope.model.value)) {
                $scope.model.value = $scope.model.value.split(',');
            }
        }
    }
}
angular.module('umbraco').controller("Umbraco.PropertyEditors.EntityPickerController", entityPicker);
/**
 * @ngdoc controller
 * @name Umbraco.Editors.FileUploadController
 * @function
 * 
 * @description
 * The controller for the file upload property editor. It is important to note that the $scope.model.value
 *  doesn't necessarily depict what is saved for this property editor. $scope.model.value can be empty when we 
 *  are submitting files because in that case, we are adding files to the fileManager which is what gets peristed
 *  on the server. However, when we are clearing files, we are setting $scope.model.value to "{clearFiles: true}"
 *  to indicate on the server that we are removing files for this property. We will keep the $scope.model.value to 
 *  be the name of the file selected (if it is a newly selected file) or keep it to be it's original value, this allows
 *  for the editors to check if the value has changed and to re-bind the property if that is true.
 * 
*/
function fileUploadController($scope, $element, $compile, imageHelper, fileManager, umbRequestHelper, mediaHelper) {

    /** Clears the file collections when content is saving (if we need to clear) or after saved */
    function clearFiles() {
        //clear the files collection (we don't want to upload any!)
        fileManager.setFiles($scope.model.alias, []);
        //clear the current files
        $scope.files = [];
        if ($scope.propertyForm.fileCount) {
            //this is required to re-validate
            $scope.propertyForm.fileCount.$setViewValue($scope.files.length);
        }
       
    }

    /** this method is used to initialize the data and to re-initialize it if the server value is changed */
    function initialize(index) {

        clearFiles();

        if (!index) {
            index = 1;
        }

        //this is used in order to tell the umb-single-file-upload directive to 
        //rebuild the html input control (and thus clearing the selected file) since
        //that is the only way to manipulate the html for the file input control.
        $scope.rebuildInput = {
            index: index
        };
        //clear the current files
        $scope.files = [];
        //store the original value so we can restore it if the user clears and then cancels clearing.
        $scope.originalValue = $scope.model.value;

        //create the property to show the list of files currently saved
        if ($scope.model.value != "") {

            var images = $scope.model.value.split(",");

            $scope.persistedFiles = _.map(images, function (item) {
                return { file: item, isImage: imageHelper.detectIfImageByExtension(item) };
            });
        }
        else {
            $scope.persistedFiles = [];
        }

        _.each($scope.persistedFiles, function (file) {

            var thumbnailUrl = umbRequestHelper.getApiUrl(
                        "imagesApiBaseUrl",
                        "GetBigThumbnail",
                        [{ originalImagePath: file.file }]);

            file.thumbnail = thumbnailUrl;
        });

        $scope.clearFiles = false;
    }

    initialize();

    // Method required by the valPropertyValidator directive (returns true if the property editor has at least one file selected)
    $scope.validateMandatory = function () {
        return {
            isValid: !$scope.model.validation.mandatory || ((($scope.persistedFiles != null && $scope.persistedFiles.length > 0) || ($scope.files != null && $scope.files.length > 0)) && !$scope.clearFiles),
            errorMsg: "Value cannot be empty",
            errorKey: "required"
        };
    }

    //listen for clear files changes to set our model to be sent up to the server
    $scope.$watch("clearFiles", function (isCleared) {
        if (isCleared == true) {
            $scope.model.value = { clearFiles: true };
            clearFiles();
        }
        else {
            //reset to original value
            $scope.model.value = $scope.originalValue;
            //this is required to re-validate
            $scope.propertyForm.fileCount.$setViewValue($scope.files.length);
        }
    });

    //listen for when a file is selected
    $scope.$on("filesSelected", function (event, args) {
        $scope.$apply(function () {
            //set the files collection
            fileManager.setFiles($scope.model.alias, args.files);
            //clear the current files
            $scope.files = [];
            var newVal = "";
            for (var i = 0; i < args.files.length; i++) {
                //save the file object to the scope's files collection
                $scope.files.push({ alias: $scope.model.alias, file: args.files[i] });
                newVal += args.files[i].name + ",";
            }

            //this is required to re-validate
            $scope.propertyForm.fileCount.$setViewValue($scope.files.length);

            //set clear files to false, this will reset the model too
            $scope.clearFiles = false;
            //set the model value to be the concatenation of files selected. Please see the notes
            // in the description of this controller, it states that this value isn't actually used for persistence,
            // but we need to set it so that the editor and the server can detect that it's been changed, and it is used for validation.
            $scope.model.value = { selectedFiles: newVal.trimEnd(",") };
        });
    });

    //listen for when the model value has changed
    $scope.$watch("model.value", function (newVal, oldVal) {
        //cannot just check for !newVal because it might be an empty string which we 
        //want to look for.
        if (newVal !== null && newVal !== undefined && newVal !== oldVal) {
            //now we need to check if we need to re-initialize our structure which is kind of tricky
            // since we only want to do that if the server has changed the value, not if this controller
            // has changed the value. There's only 2 scenarios where we change the value internall so 
            // we know what those values can be, if they are not either of them, then we'll re-initialize.

            if (newVal.clearFiles !== true && newVal !== $scope.originalValue && !newVal.selectedFiles) {
                initialize($scope.rebuildInput.index + 1);
            }

        }
    });
};
angular.module("umbraco")
    .controller('Umbraco.PropertyEditors.FileUploadController', fileUploadController)
    .run(function(mediaHelper, umbRequestHelper){
        if (mediaHelper && mediaHelper.registerFileResolver) {

            //NOTE: The 'entity' can be either a normal media entity or an "entity" returned from the entityResource
            // they contain different data structures so if we need to query against it we need to be aware of this.
            mediaHelper.registerFileResolver("Umbraco.UploadField", function(property, entity, thumbnail){
                if (thumbnail) {

                    if (mediaHelper.detectIfImageByExtension(property.value)) {

                        var thumbnailUrl = umbRequestHelper.getApiUrl(
                            "imagesApiBaseUrl",
                            "GetBigThumbnail",
                            [{ originalImagePath: property.value }]);
                            
                        return thumbnailUrl;
                    }
                    else {
                        return null;
                    }
                    
                }
                else {
                    return property.value;
                }
            });
        }
    });
angular.module("umbraco")

.controller("Umbraco.PropertyEditors.FolderBrowserController",
    function ($rootScope, $scope, $routeParams, $timeout, editorState, navigationService) {

        var dialogOptions = $scope.dialogOptions;
        $scope.creating = $routeParams.create;
        $scope.nodeId = $routeParams.id;

        $scope.onUploadComplete = function () {

            //sync the tree - don't force reload since we're not updating this particular node (i.e. its name or anything),
            // then we'll get the resulting tree node which we can then use to reload it's children.
            var path = editorState.current.path;
            navigationService.syncTree({ tree: "media", path: path, forceReload: false }).then(function (syncArgs) {
                navigationService.reloadNode(syncArgs.node);
            });

        }
});

angular.module("umbraco")
.controller("Umbraco.PropertyEditors.GoogleMapsController",
    function ($element, $rootScope, $scope, notificationsService, dialogService, assetsService, $log, $timeout) {

        assetsService.loadJs('http://www.google.com/jsapi')
            .then(function () {
                google.load("maps", "3",
                            {
                                callback: initMap,
                                other_params: "sensor=false"
                            });
            });

        function initMap() {
            //Google maps is available and all components are ready to use.
            var valueArray = $scope.model.value.split(',');
            var latLng = new google.maps.LatLng(valueArray[0], valueArray[1]);
            var mapDiv = document.getElementById($scope.model.alias + '_map');
            var mapOptions = {
                zoom: $scope.model.config.zoom,
                center: latLng,
                mapTypeId: google.maps.MapTypeId[$scope.model.config.mapType]
            };
            var geocoder = new google.maps.Geocoder();
            var map = new google.maps.Map(mapDiv, mapOptions);

            var marker = new google.maps.Marker({
                map: map,
                position: latLng,
                draggable: true
            });

            google.maps.event.addListener(map, 'click', function (event) {

                dialogService.mediaPicker({
                    callback: function (data) {
                        var image = data.selection[0].src;

                        var latLng = event.latLng;
                        var marker = new google.maps.Marker({
                            map: map,
                            icon: image,
                            position: latLng,
                            draggable: true
                        });

                        google.maps.event.addListener(marker, "dragend", function (e) {
                            var newLat = marker.getPosition().lat();
                            var newLng = marker.getPosition().lng();

                            codeLatLng(marker.getPosition(), geocoder);

                            //set the model value
                            $scope.model.vvalue = newLat + "," + newLng;
                        });

                    }
                });
            });

            var tabShown = function(e) {
                google.maps.event.trigger(map, 'resize');
            };

            //listen for tab changes
            if (tabsCtrl != null) {
                tabsCtrl.onTabShown(function (args) {
                    tabShown();
                });
            }

            $element.closest('.umb-panel.tabbable').on('shown', '.nav-tabs a', tabShown);

            $scope.$on('$destroy', function () {
                $element.closest('.umb-panel.tabbable').off('shown', '.nav-tabs a', tabShown);
            });
        }

        function codeLatLng(latLng, geocoder) {
            geocoder.geocode({ 'latLng': latLng },
                function (results, status) {
                    if (status == google.maps.GeocoderStatus.OK) {
                        var location = results[0].formatted_address;
                        $rootScope.$apply(function () {
                            notificationsService.success("Peter just went to: ", location);
                        });
                    }
                });
        }

        //here we declare a special method which will be called whenever the value has changed from the server
        //this is instead of doing a watch on the model.value = faster
        $scope.model.onValueChanged = function (newVal, oldVal) {
            //update the display val again if it has changed from the server
            initMap();
        };
    });
angular.module("umbraco")
    .controller("Umbraco.PropertyEditors.GridController.Dialogs.Config",
    function ($scope, $http) {

        var placeHolder = "{0}";
        var addModifier = function(val, modifier){
            if (!modifier || modifier.indexOf(placeHolder) < 0) {
                return val;
            } else {
                return modifier.replace(placeHolder, val);
            }
        }

        var stripModifier = function (val, modifier) {
            if (!val || !modifier || modifier.indexOf(placeHolder) < 0) {
                return val;
            } else {
                var paddArray = modifier.split(placeHolder);
                if(paddArray.length == 1){
                    if (modifier.indexOf(placeHolder) === 0) {
                        return val.slice(0, -paddArray[0].length);
                    } else {
                        return val.slice(paddArray[0].length, 0);
                    }
                } else {
                    if (paddArray[1].length === 0) {
                        return val.slice(paddArray[0].length);
                    }
                    return val.slice(paddArray[0].length, -paddArray[1].length); 
                }
            }
        }


        $scope.styles = _.filter( angular.copy($scope.dialogOptions.config.items.styles), function(item){return (item.applyTo === undefined || item.applyTo === $scope.dialogOptions.itemType); });
        $scope.config = _.filter( angular.copy($scope.dialogOptions.config.items.config), function(item){return (item.applyTo === undefined || item.applyTo === $scope.dialogOptions.itemType); });


        var element = $scope.dialogOptions.gridItem;
        if(angular.isObject(element.config)){
            _.each($scope.config, function(cfg){
                var val = element.config[cfg.key];
                if(val){
                    cfg.value = stripModifier(val, cfg.modifier);
                }
            });
        }

        if(angular.isObject(element.styles)){
            _.each($scope.styles, function(style){
                var val = element.styles[style.key];
                if(val){
                    style.value = stripModifier(val, style.modifier);
                }
            });
        }


        $scope.saveAndClose = function(){
            var styleObject = {};
            var configObject = {};

            _.each($scope.styles, function(style){
                if(style.value){
                    styleObject[style.key] = addModifier(style.value, style.modifier);
                }
            });
            _.each($scope.config, function (cfg) {
                if (cfg.value) {
                    configObject[cfg.key] = addModifier(cfg.value, cfg.modifier);
                }
            });

            $scope.submit({config: configObject, styles: styleObject});
        };

    });

angular.module("umbraco")
    .controller("Umbraco.PropertyEditors.GridPrevalueEditorController.Dialogs.EditConfig",
    function ($scope, $http) {

            $scope.renderModel = {};
            $scope.renderModel.name = $scope.dialogOptions.name;
            $scope.renderModel.config = $scope.dialogOptions.config;

            $scope.saveAndClose = function(isValid){
                if(isValid){
                    $scope.submit($scope.renderModel.config);
                }
            };

    });

angular.module("umbraco")
    .controller("Umbraco.PropertyEditors.GridPrevalueEditor.LayoutConfigController",
    function ($scope) {

    		$scope.currentLayout = $scope.dialogOptions.currentLayout;
    		$scope.columns = $scope.dialogOptions.columns;
    		$scope.rows = $scope.dialogOptions.rows;

    		$scope.scaleUp = function(section, max, overflow){
    		   var add = 1;
    		   if(overflow !== true){
    		        add = (max > 1) ? 1 : max;
    		   }
    		   //var add = (max > 1) ? 1 : max;
    		   section.grid = section.grid+add;
    		};

    		$scope.scaleDown = function(section){
    		   var remove = (section.grid > 1) ? 1 : section.grid;
    		   section.grid = section.grid-remove;
    		};

    		$scope.percentage = function(spans){
    		    return ((spans / $scope.columns) * 100).toFixed(8);
    		};

    		$scope.toggleCollection = function(collection, toggle){
    		    if(toggle){
    		        collection = [];
    		    }else{
    		        delete collection;
    		    }
    		};



    		/****************
    		    Section
    		*****************/
    		$scope.configureSection = function(section, template){
    		   if(section === undefined){
    		        var space = ($scope.availableLayoutSpace > 4) ? 4 : $scope.availableLayoutSpace;
    		        section = {
    		            grid: space
    		        };
    		        template.sections.push(section);
    		    }
    		    
    		    $scope.currentSection = section;
    		};


    		$scope.deleteSection = function(index){
    		    $scope.currentTemplate.sections.splice(index, 1);
    		};
    		
    		$scope.closeSection = function(){
    		    $scope.currentSection = undefined;
    		};

    		$scope.$watch("currentLayout", function(layout){
    		    if(layout){
    		        var total = 0;
    		        _.forEach(layout.sections, function(section){
    		            total = (total + section.grid);
    		        });

    		        $scope.availableLayoutSpace = $scope.columns - total;
    		    }
    		}, true);
    });
function RowConfigController($scope) {
    
    $scope.currentRow = angular.copy($scope.dialogOptions.currentRow);
    $scope.editors = $scope.dialogOptions.editors;
    $scope.columns = $scope.dialogOptions.columns;

    $scope.scaleUp = function(section, max, overflow) {
        var add = 1;
        if (overflow !== true) {
            add = (max > 1) ? 1 : max;
        }
        //var add = (max > 1) ? 1 : max;
        section.grid = section.grid + add;
    };

    $scope.scaleDown = function(section) {
        var remove = (section.grid > 1) ? 1 : section.grid;
        section.grid = section.grid - remove;
    };

    $scope.percentage = function(spans) {
        return ((spans / $scope.columns) * 100).toFixed(8);
    };

    $scope.toggleCollection = function(collection, toggle) {
        if (toggle) {
            collection = [];
        }
        else {
            delete collection;
        }
    };


    /****************
        area
    *****************/
    $scope.configureCell = function(cell, row) {
        if ($scope.currentCell && $scope.currentCell === cell) {
            delete $scope.currentCell;
        }
        else {
            if (cell === undefined) {
                var available = $scope.availableRowSpace;
                var space = 4;

                if (available < 4 && available > 0) {
                    space = available;
                }

                cell = {
                    grid: space
                };

                row.areas.push(cell);
            }
            $scope.currentCell = cell;
        }
    };

    $scope.deleteArea = function(index) {
        $scope.currentRow.areas.splice(index, 1);
    };
    $scope.closeArea = function() {
        $scope.currentCell = undefined;
    };

    $scope.nameChanged = false;
    var originalName = $scope.currentRow.name;
    $scope.$watch("currentRow", function(row) {
        if (row) {

            var total = 0;
            _.forEach(row.areas, function(area) {
                total = (total + area.grid);
            });

            $scope.availableRowSpace = $scope.columns - total;

            if (originalName) {
                if (originalName != row.name) {
                    $scope.nameChanged = true;
                }
                else {
                    $scope.nameChanged = false;
                }
            }
        }
    }, true);

    $scope.complete = function () {
        angular.extend($scope.dialogOptions.currentRow, $scope.currentRow);
        $scope.close();
    }
}

angular.module("umbraco").controller("Umbraco.PropertyEditors.GridPrevalueEditor.RowConfigController", RowConfigController);
angular.module("umbraco")
    .controller("Umbraco.PropertyEditors.Grid.EmbedController",
    function ($scope, $rootScope, $timeout, dialogService) {

    	$scope.setEmbed = function(){
    		dialogService.embedDialog({
                    callback: function (data) {
                        $scope.control.value = data;
                    }
                });
    	};

    	$timeout(function(){
    		if($scope.control.$initializing){
    			$scope.setEmbed();
    		}
    	}, 200);
});


angular.module("umbraco")
    .controller("Umbraco.PropertyEditors.Grid.MacroController",
    function ($scope, $rootScope, $timeout, dialogService, macroResource, macroService,  $routeParams) {

        $scope.title = "Click to insert macro";

        $scope.setMacro = function(){
            dialogService.macroPicker({
                dialogData: {
                    richTextEditor: true,  
                    macroData: $scope.control.value || {
                        macroAlias: $scope.control.editor.config && $scope.control.editor.config.macroAlias
                          ? $scope.control.editor.config.macroAlias : ""
                    }
                },
                callback: function (data) {
                    $scope.control.value = {
                            macroAlias: data.macroAlias,
                            macroParamsDictionary: data.macroParamsDictionary
                    };

                    $scope.setPreview($scope.control.value );
                }
            });
    	};

        $scope.setPreview = function(macro){
            var contentId = $routeParams.id;

            macroResource.getMacroResultAsHtmlForEditor(macro.macroAlias, contentId, macro.macroParamsDictionary)
            .then(function (htmlResult) {
                $scope.title = macro.macroAlias;
                if(htmlResult.trim().length > 0 && htmlResult.indexOf("Macro:") < 0){
                    $scope.preview = htmlResult;
                }
            });

        };

    	$timeout(function(){
    		if($scope.control.$initializing){
    			$scope.setMacro();
    		}else if($scope.control.value){
                $scope.setPreview($scope.control.value);
            }
    	}, 200);
});


angular.module("umbraco")
    .controller("Umbraco.PropertyEditors.Grid.MediaController",
    function ($scope, $rootScope, $timeout, dialogService) {

        $scope.setImage = function(){

            dialogService.mediaPicker({
                startNodeId: $scope.control.editor.config && $scope.control.editor.config.startNodeId ? $scope.control.editor.config.startNodeId : undefined,
                multiPicker: false,
                cropSize:  $scope.control.editor.config && $scope.control.editor.config.size ? $scope.control.editor.config.size : undefined,
                showDetails: true,
                callback: function (data) {

                    $scope.control.value = {
                        focalPoint: data.focalPoint,
                        id: data.id,
                        image: data.image,
                        altText: data.altText
                    };

                    $scope.setUrl();
                }
            });
        };

        $scope.setUrl = function(){

            if($scope.control.value.image){
                var url = $scope.control.value.image;

                if($scope.control.editor.config && $scope.control.editor.config.size){
                    url += "?width=" + $scope.control.editor.config.size.width;
                    url += "&height=" + $scope.control.editor.config.size.height;

                    if($scope.control.value.focalPoint){
                        url += "&center=" + $scope.control.value.focalPoint.top +"," + $scope.control.value.focalPoint.left;
                        url += "&mode=crop";
                    }
                }

                $scope.url = url;
            }
        };

        $timeout(function(){
            if($scope.control.$initializing){
                $scope.setImage();
            }else if($scope.control.value){
                $scope.setUrl();
            }
        }, 200);
});

angular.module("umbraco")
    .controller("Umbraco.PropertyEditors.Grid.TextStringController",
    function ($scope, $rootScope, $timeout, dialogService) {

        

    });


angular.module("umbraco")
    .controller("Umbraco.PropertyEditors.GridController",
    function ($scope, $http, assetsService, $rootScope, dialogService, gridService, mediaResource, imageHelper, $timeout, umbRequestHelper) {

        // Grid status variables
        $scope.currentRow = null;
        $scope.currentCell = null;
        $scope.currentToolsControl = null;
        $scope.currentControl = null;
        $scope.openRTEToolbarId = null;
        $scope.hasSettings = false;

        // *********************************************
        // Sortable options
        // *********************************************

        var draggedRteSettings;

        $scope.sortableOptions = {
            distance: 10,
            cursor: "move",
            placeholder: "ui-sortable-placeholder",
            handle: ".cell-tools-move",
            forcePlaceholderSize: true,
            tolerance: "pointer",
            zIndex: 999999999999999999,
            scrollSensitivity: 100,
            cursorAt: {
                top: 45,
                left: 90
            },

            sort: function (event, ui) {
                /* prevent vertical scroll out of the screen */
                var max = $(".usky-grid").width() - 150;
                if (parseInt(ui.helper.css("left")) > max) {
                    ui.helper.css({ "left": max + "px" });
                }
                if (parseInt(ui.helper.css("left")) < 20) {
                    ui.helper.css({ "left": 20 });
                }
            },

            start: function (e, ui) {
                draggedRteSettings = {};
                ui.item.find(".mceNoEditor").each(function () {
                    // remove all RTEs in the dragged row and save their settings
                    var id = $(this).attr("id");
                    draggedRteSettings[id] = _.findWhere(tinyMCE.editors, { id: id }).settings;
                    tinyMCE.execCommand("mceRemoveEditor", false, id);
                });
            },

            stop: function (e, ui) {
                // reset all RTEs affected by the dragging
                ui.item.parents(".usky-column").find(".mceNoEditor").each(function () {
                    var id = $(this).attr("id");
                    draggedRteSettings[id] = draggedRteSettings[id] || _.findWhere(tinyMCE.editors, { id: id }).settings;
                    tinyMCE.execCommand("mceRemoveEditor", false, id);
                    tinyMCE.init(draggedRteSettings[id]);
                });
            }
        };

        var notIncludedRte = [];
        var cancelMove = false;

        $scope.sortableOptionsCell = {
            distance: 10,
            cursor: "move",
            placeholder: "ui-sortable-placeholder",
            handle: ".cell-tools-move",
            connectWith: ".usky-cell",
            forcePlaceholderSize: true,
            tolerance: "pointer",
            zIndex: 999999999999999999,
            scrollSensitivity: 100,
            cursorAt: {
                top: 45,
                left: 90
            },

            sort: function (event, ui) {
                /* prevent vertical scroll out of the screen */
                var position = parseInt(ui.item.parent().offset().left) + parseInt(ui.helper.css("left")) - parseInt($(".usky-grid").offset().left);
                var max = $(".usky-grid").width() - 220;
                if (position > max) {
                    ui.helper.css({ "left": max - parseInt(ui.item.parent().offset().left) + parseInt($(".usky-grid").offset().left) + "px" });
                }
                if (position < 0) {
                    ui.helper.css({ "left": 0 - parseInt(ui.item.parent().offset().left) + parseInt($(".usky-grid").offset().left) + "px" });
                }
            },

            over: function (event, ui) {
                var allowedEditors = $(event.target).scope().area.allowed;

                if ($.inArray(ui.item.scope().control.editor.alias, allowedEditors) < 0 && allowedEditors) {
                    ui.placeholder.hide();
                    cancelMove = true;
                }
                else {
                    ui.placeholder.show();
                    cancelMove = false;
                }

            },

            update: function (event, ui) {
                // add all RTEs which are affected by the dragging
                if (!ui.sender) {
                    if (cancelMove) {
                        ui.item.sortable.cancel();
                    }
                    ui.item.parents(".usky-cell").find(".mceNoEditor").each(function () {
                        if ($.inArray($(this).attr("id"), notIncludedRte) < 0) {
                            notIncludedRte.splice(0, 0, $(this).attr("id"));
                        }
                    });
                }
                else {
                    $(event.target).find(".mceNoEditor").each(function () {
                        if ($.inArray($(this).attr("id"), notIncludedRte) < 0) {
                            notIncludedRte.splice(0, 0, $(this).attr("id"));
                        }
                    });
                }

            },

            start: function (e, ui) {
                // reset dragged RTE settings in case a RTE isn't dragged
                draggedRteSettings = undefined;

                ui.item.find(".mceNoEditor").each(function () {
                    notIncludedRte = [];

                    // save the dragged RTE settings
                    draggedRteSettings = _.findWhere(tinyMCE.editors, { id: $(this).attr("id") }).settings;

                    // remove the dragged RTE
                    tinyMCE.execCommand("mceRemoveEditor", false, $(this).attr("id"));
                });
            },

            stop: function (e, ui) {
                ui.item.parents(".usky-cell").find(".mceNoEditor").each(function () {
                    if ($.inArray($(this).attr("id"), notIncludedRte) < 0) {
                        // add all dragged's neighbouring RTEs in the new cell
                        notIncludedRte.splice(0, 0, $(this).attr("id"));
                    }
                });
                $timeout(function () {
                    // reconstruct the dragged RTE (could be undefined when dragging something else than RTE)
                    if (draggedRteSettings !== undefined) {
                        tinyMCE.init(draggedRteSettings);
                    }

                    _.forEach(notIncludedRte, function (id) {
                        // reset all the other RTEs
                        if (draggedRteSettings === undefined || id !== draggedRteSettings.id) {
                            var rteSettings = _.findWhere(tinyMCE.editors, { id: id }).settings;
                            tinyMCE.execCommand("mceRemoveEditor", false, id);
                            tinyMCE.init(rteSettings);
                        }
                    });
                }, 500, false);
            }

        };

        // *********************************************
        // Add items overlay menu
        // *********************************************
        $scope.overlayMenu = {
            show: false,
            style: {},
            area: undefined,
            key: undefined
        };

        $scope.addItemOverlay = function (event, area, index, key) {
            $scope.overlayMenu.area = area;
            $scope.overlayMenu.index = index;
            $scope.overlayMenu.style = {};
            $scope.overlayMenu.key = key;

            //todo calculate position...
            var offset = $(event.target).offset();
            var height = $(window).height();

            if ((height - offset.top) < 250) {
                $scope.overlayMenu.style.bottom = 0;
                $scope.overlayMenu.style.top = "initial";
            } else if (offset.top < 300) {
                $scope.overlayMenu.style.top = 190;
            }

            $scope.overlayMenu.show = true;
        };

        $scope.closeItemOverlay = function () {
            $scope.currentControl = null;
            $scope.overlayMenu.show = false;
            $scope.overlayMenu.key = undefined;
        };

        // *********************************************
        // Template management functions
        // *********************************************

        $scope.addTemplate = function (template) {
            $scope.model.value = angular.copy(template);

            //default row data
            _.forEach($scope.model.value.sections, function (section) {
                $scope.initSection(section);
            });
        };


        // *********************************************
        // Row management function
        // *********************************************

        $scope.setCurrentRow = function (row) {
            $scope.currentRow = row;
        };

        $scope.disableCurrentRow = function () {
            $scope.currentRow = null;
        };

        $scope.setWarnighlightRow = function (row) {
            $scope.currentWarnhighlightRow = row;
        };

        $scope.disableWarnhighlightRow = function () {
            $scope.currentWarnhighlightRow = null;
        };

        $scope.setInfohighlightRow = function (row) {
            $scope.currentInfohighlightRow = row;
        };

        $scope.disableInfohighlightRow = function () {
            $scope.currentInfohighlightRow = null;
        };

        function getAllowedLayouts(section) {

            var layouts = $scope.model.config.items.layouts;

            //This will occur if it is a new section which has been
            // created from a 'template'
            if (section.allowed && section.allowed.length > 0) {
                return _.filter(layouts, function (layout) {
                    return _.indexOf(section.allowed, layout.name) >= 0;
                });
            }
            else {


                return layouts;
            }
        }

        $scope.addRow = function (section, layout) {

            //copy the selected layout into the rows collection
            var row = angular.copy(layout);

            // Init row value
            row = $scope.initRow(row);

            // Push the new row
            if (row) {
                section.rows.push(row);
            }
        };

        $scope.removeRow = function (section, $index) {
            if (section.rows.length > 0) {
                section.rows.splice($index, 1);
                $scope.currentRow = null;
                $scope.openRTEToolbarId = null;

                //$scope.initContent();
            }
        };

        $scope.editGridItemSettings = function (gridItem, itemType) {

            dialogService.open(
                {
                    template: "views/propertyeditors/grid/dialogs/config.html",
                    gridItem: gridItem,
                    config: $scope.model.config,
                    itemType: itemType,
                    callback: function (data) {

                        gridItem.styles = data.styles;
                        gridItem.config = data.config;

                    }
                });

        };

        // *********************************************
        // Area management functions
        // *********************************************

        $scope.setCurrentCell = function (cell) {
            $scope.currentCell = cell;
        };

        $scope.disableCurrentCell = function () {
            $scope.currentCell = null;
        };

        $scope.cellPreview = function (cell) {
            if (cell && cell.$allowedEditors) {
                var editor = cell.$allowedEditors[0];
                return editor.icon;
            } else {
                return "icon-layout";
            }
        };

        $scope.setInfohighlightArea = function (cell) {
            $scope.currentInfohighlightArea = cell;
        };

        $scope.disableInfohighlightArea = function () {
            $scope.currentInfohighlightArea = null;
        };


        // *********************************************
        // Control management functions
        // *********************************************
        $scope.setCurrentControl = function (Control) {
            $scope.currentControl = Control;
        };

        $scope.disableCurrentControl = function (Control) {
            $scope.currentControl = null;
        };

        $scope.setCurrentToolsControl = function (Control) {
            $scope.currentToolsControl = Control;
        };

        $scope.disableCurrentToolsControl = function (Control) {
            $scope.currentToolsControl = null;
        };

        $scope.setWarnhighlightControl = function (Control) {
            $scope.currentWarnhighlightControl = Control;
        };

        $scope.disableWarnhighlightControl = function (Control) {
            $scope.currentWarnhighlightControl = null;
        };

        $scope.setInfohighlightControl = function (Control) {
            $scope.currentInfohighlightControl = Control;
        };

        $scope.disableInfohighlightControl = function (Control) {
            $scope.currentInfohighlightControl = null;
        };


        var guid = (function () {
            function s4() {
                return Math.floor((1 + Math.random()) * 0x10000)
                           .toString(16)
                           .substring(1);
            }
            return function () {
                return s4() + s4() + "-" + s4() + "-" + s4() + "-" +
                       s4() + "-" + s4() + s4() + s4();
            };
        })();

        $scope.setUniqueId = function (cell, index) {
            return guid();
        };

        $scope.addControl = function (editor, cell, index, initialize) {
            $scope.closeItemOverlay();
            initialize = (initialize !== false);

            var newControl = {
                value: null,
                editor: editor,
                $initializing: initialize
            };

            if (index === undefined) {
                index = cell.controls.length;
            }

            //populate control
            $scope.initControl(newControl, index + 1);

            cell.controls.splice(index + 1, 0, newControl);
        };

        $scope.addTinyMce = function (cell) {
            var rte = $scope.getEditor("rte");
            $scope.addControl(rte, cell);
        };

        $scope.getEditor = function (alias) {
            return _.find($scope.availableEditors, function (editor) { return editor.alias === alias; });
        };

        $scope.removeControl = function (cell, $index) {
            $scope.currentControl = null;
            cell.controls.splice($index, 1);
        };

        $scope.percentage = function (spans) {
            return ((spans / $scope.model.config.items.columns) * 100).toFixed(8);
        };


        $scope.clearPrompt = function (scopedObject, e) {
            scopedObject.deletePrompt = false;
            e.preventDefault();
            e.stopPropagation();
        };

        $scope.showPrompt = function (scopedObject) {
            scopedObject.deletePrompt = true;
        };


        // *********************************************
        // INITIALISATION
        // these methods are called from ng-init on the template
        // so we can controll their first load data
        //
        // intialisation sets non-saved data like percentage sizing, allowed editors and
        // other data that should all be pre-fixed with $ to strip it out on save
        // *********************************************

        // *********************************************
        // Init template + sections
        // *********************************************
        $scope.initContent = function () {
            var clear = true;

            //settings indicator shortcut
            if ( ($scope.model.config.items.config && $scope.model.config.items.config.length > 0) || ($scope.model.config.items.styles && $scope.model.config.items.styles.length > 0)) {
                $scope.hasSettings = true;
            }

            //ensure the grid has a column value set,
            //if nothing is found, set it to 12
            if ($scope.model.config.items.columns && angular.isString($scope.model.config.items.columns)) {
                $scope.model.config.items.columns = parseInt($scope.model.config.items.columns);
            } else {
                $scope.model.config.items.columns = 12;
            }

            if ($scope.model.value && $scope.model.value.sections && $scope.model.value.sections.length > 0) {

                if ($scope.model.value.name && angular.isArray($scope.model.config.items.templates)) {

                    //This will occur if it is an existing value, in which case
                    // we need to determine which layout was applied by looking up
                    // the name
                    // TODO: We need to change this to an immutable ID!!

                    var found = _.find($scope.model.config.items.templates, function (t) {
                        return t.name === $scope.model.value.name;
                    });

                    if (found && angular.isArray(found.sections) && found.sections.length === $scope.model.value.sections.length) {

                        //Cool, we've found the template associated with our current value with matching sections counts, now we need to
                        // merge this template data on to our current value (as if it was new) so that we can preserve what is and isn't
                        // allowed for this template based on the current config.

                        _.each(found.sections, function (templateSection, index) {
                            angular.extend($scope.model.value.sections[index], angular.copy(templateSection));
                        });

                    }
                }

                _.forEach($scope.model.value.sections, function (section, index) {

                    if (section.grid > 0) {
                        $scope.initSection(section);

                        //we do this to ensure that the grid can be reset by deleting the last row
                        if (section.rows.length > 0) {
                            clear = false;
                        }
                    } else {
                        $scope.model.value.sections.splice(index, 1);
                    }
                });
            } else if ($scope.model.config.items.templates && $scope.model.config.items.templates.length === 1) {
                $scope.addTemplate($scope.model.config.items.templates[0]);
                clear = false;
            }

            if (clear) {
                $scope.model.value = undefined;
            }
        };

        $scope.initSection = function (section) {
            section.$percentage = $scope.percentage(section.grid);

            section.$allowedLayouts = getAllowedLayouts(section);

            if (!section.rows || section.rows.length === 0) {
                section.rows = [];
                if(section.$allowedLayouts.length === 1){
                    $scope.addRow(section, section.$allowedLayouts[0]);
                }
            } else {
                _.forEach(section.rows, function (row, index) {
                    if (!row.$initialized) {
                        var initd = $scope.initRow(row);

                        //if init fails, remove
                        if (!initd) {
                            section.rows.splice(index, 1);
                        } else {
                            section.rows[index] = initd;
                        }
                    }
                });
            }
        };


        // *********************************************
        // Init layout / row
        // *********************************************
        $scope.initRow = function (row) {

            //merge the layout data with the original config data
            //if there are no config info on this, splice it out
            var original = _.find($scope.model.config.items.layouts, function (o) { return o.name === row.name; });

            if (!original) {
                return null;
            } else {
                //make a copy to not touch the original config
                original = angular.copy(original);
                original.styles = row.styles;
                original.config = row.config;

                //sync area configuration
                _.each(original.areas, function (area, areaIndex) {


                    if (area.grid > 0) {
                        var currentArea = row.areas[areaIndex];

                        if (currentArea) {
                            area.config = currentArea.config;
                            area.styles = currentArea.styles;
                        }

                        //set editor permissions
                        if (!area.allowed || area.allowAll === true) {
                            area.$allowedEditors = $scope.availableEditors;
                            area.$allowsRTE = true;
                        } else {
                            area.$allowedEditors = _.filter($scope.availableEditors, function (editor) {
                                return _.indexOf(area.allowed, editor.alias) >= 0;
                            });

                            if (_.indexOf(area.allowed, "rte") >= 0) {
                                area.$allowsRTE = true;
                            }
                        }

                        //copy over existing controls into the new areas
                        if (row.areas.length > areaIndex && row.areas[areaIndex].controls) {
                            area.controls = currentArea.controls;

                            _.forEach(area.controls, function (control, controlIndex) {
                                $scope.initControl(control, controlIndex);
                            });

                        } else {
                            //if empty
                            area.controls = [];

                            //if only one allowed editor
                            if(area.$allowedEditors.length === 1){
                                $scope.addControl(area.$allowedEditors[0], area, 0, false);
                            }
                        }

                        //set width
                        area.$percentage = $scope.percentage(area.grid);
                        area.$uniqueId = $scope.setUniqueId();

                    } else {
                        original.areas.splice(areaIndex, 1);
                    }
                });

                //replace the old row
                original.$initialized = true;

                //set a disposable unique ID
                original.$uniqueId = $scope.setUniqueId();

                //set a no disposable unique ID (util for row styling)
                original.id = !row.id ? $scope.setUniqueId() : row.id;

                return original;
            }

        };


        // *********************************************
        // Init control
        // *********************************************

        $scope.initControl = function (control, index) {
            control.$index = index;
            control.$uniqueId = $scope.setUniqueId();

            //error handling in case of missing editor..
            //should only happen if stripped earlier
            if (!control.editor) {
                control.$editorPath = "views/propertyeditors/grid/editors/error.html";
            }

            if (!control.$editorPath) {
                var editorConfig = $scope.getEditor(control.editor.alias);

                if (editorConfig) {
                    control.editor = editorConfig;

                    //if its an absolute path
                    if (control.editor.view.startsWith("/") || control.editor.view.startsWith("~/")) {
                        control.$editorPath = umbRequestHelper.convertVirtualToAbsolutePath(control.editor.view);
                    }
                    else {
                        //use convention
                        control.$editorPath = "views/propertyeditors/grid/editors/" + control.editor.view + ".html";
                    }
                }
                else {
                    control.$editorPath = "views/propertyeditors/grid/editors/error.html";
                }
            }


        };


        gridService.getGridEditors().then(function (response) {
            $scope.availableEditors = response.data;

            $scope.contentReady = true;

            // *********************************************
            // Init grid
            // *********************************************
            $scope.initContent();

        });

        //Clean the grid value before submitting to the server, we don't need
        // all of that grid configuration in the value to be stored!! All of that
        // needs to be merged in at runtime to ensure that the real config values are used
        // if they are ever updated.

        var unsubscribe = $scope.$on("formSubmitting", function () {

            if ($scope.model.value && $scope.model.value.sections) {
                _.each($scope.model.value.sections, function(section) {
                    if (section.rows) {
                        _.each(section.rows, function (row) {
                            if (row.areas) {
                                _.each(row.areas, function (area) {

                                    //Remove the 'editors' - these are the allowed editors, these will
                                    // be injected at runtime to this editor, it should not be persisted

                                    if (area.editors) {
                                        delete area.editors;
                                    }

                                    if (area.controls) {
                                        _.each(area.controls, function (control) {
                                            if (control.editor) {
                                                //replace
                                                var alias = control.editor.alias;
                                                control.editor = {
                                                    alias: alias
                                                };
                                            }
                                        });
                                    }
                                });
                            }
                        });
                    }
                });
            }
        });

        //when the scope is destroyed we need to unsubscribe
        $scope.$on("$destroy", function () {
            unsubscribe();
        });

    });

angular.module("umbraco")
    .controller("Umbraco.PropertyEditors.GridPrevalueEditorController",
    function ($scope, $http, assetsService, $rootScope, dialogService, mediaResource, gridService, imageHelper, $timeout) {

        var emptyModel = {
            styles:[
                {
                    label: "Set a background image",
                    description: "Set a row background",
                    key: "background-image",
                    view: "imagepicker",
                    modifier: "url({0})"
                }
            ],

            config:[
                {
                    label: "Class",
                    description: "Set a css class",
                    key: "class",
                    view: "textstring"
                }
            ],

            columns: 12,
            templates:[
                {
                    name: "1 column layout",
                    sections: [
                        {
                            grid: 12,
                        }
                    ]
                },
                {
                    name: "2 column layout",
                    sections: [
                        {
                            grid: 4,
                        },
                        {
                            grid: 8
                        }
                    ]
                }
            ],


            layouts:[
                {
                    name: "Headline",
                    areas: [
                        {
                            grid: 12,
                            editors: ["headline"]
                        }
                    ]
                },
                {
                    name: "Article",
                    areas: [
                        {
                            grid: 4
                        },
                        {
                            grid: 8
                        }
                    ]
                }
            ]
        };

        /****************
            template
        *****************/

        $scope.configureTemplate = function(template){
           if(template === undefined){
                template = {
                    name: "",
                    sections:[

                    ]
                };
                $scope.model.value.templates.push(template);
           }    
           
           dialogService.open(
               {
                   template: "views/propertyEditors/grid/dialogs/layoutconfig.html",
                   currentLayout: template,
                   rows: $scope.model.value.layouts,
                   columns: $scope.model.value.columns
               }
           );

        };

        $scope.deleteTemplate = function(index){
            $scope.model.value.templates.splice(index, 1);
        };
        

        /****************
            Row
        *****************/

        $scope.configureLayout = function(layout){

            if(layout === undefined){
                 layout = {
                     name: "",
                     areas:[

                     ]
                 };
                 $scope.model.value.layouts.push(layout);
            }

            dialogService.open(
                {
                    template: "views/propertyEditors/grid/dialogs/rowconfig.html",
                    currentRow: layout,
                    editors: $scope.editors,
                    columns: $scope.model.value.columns
                }
            );
        };

        //var rowDeletesPending = false;
        $scope.deleteLayout = function (index) {
            //rowDeletesPending = true;

            //show ok/cancel dialog
            var confirmDialog = dialogService.open(
               {
                   template: "views/propertyEditors/grid/dialogs/rowdeleteconfirm.html",
                   show: true,
                   callback: function() {
                       $scope.model.value.layouts.splice(index, 1);
                   },
                   dialogData: {
                       rowName: $scope.model.value.layouts[index].name
                   }
               }
           );
            
        };
        


        /****************
            utillities
        *****************/
        $scope.toggleCollection = function(collection, toggle){
            if(toggle){
                collection = [];
            }else{
                delete collection;
            }
        };

        $scope.percentage = function(spans){
            return ((spans / $scope.model.value.columns) * 100).toFixed(8);
        };

        $scope.zeroWidthFilter = function (cell) {
                return cell.grid > 0;
        };

        /****************
            Config
        *****************/

        $scope.removeConfigValue = function(collection, index){
            collection.splice(index, 1);
        };

        var editConfigCollection = function(configValues, title, callbackOnSave){
            dialogService.open(
                {
                    template: "views/propertyeditors/grid/dialogs/editconfig.html",
                    config: configValues,
                    name: title,
                    callback: function(data){
                        callbackOnSave(data);
                    }
                });
        };

        $scope.editConfig = function(){
            editConfigCollection($scope.model.value.config, "Settings", function(data){
                $scope.model.value.config = data;
            });
	    };

        $scope.editStyles = function(){
            editConfigCollection($scope.model.value.styles, "Styling", function(data){
                $scope.model.value.styles = data;
            });
        };


        /****************
            editors
        *****************/
        gridService.getGridEditors().then(function(response){
            $scope.editors = response.data;
        });


        /* init grid data */
        if (!$scope.model.value || $scope.model.value === "" || !$scope.model.value.templates) {
            $scope.model.value = emptyModel;
        } else {

            if (!$scope.model.value.columns) {
                $scope.model.value.columns = emptyModel.columns;
            }


            if (!$scope.model.value.config) {
                $scope.model.value.config = [];
            }

            if (!$scope.model.value.styles) {
                $scope.model.value.styles = [];
            }
        }

        /****************
            Clean up
        *****************/
        var unsubscribe = $scope.$on("formSubmitting", function (ev, args) {
            var ts = $scope.model.value.templates;
            var ls = $scope.model.value.layouts;

            _.each(ts, function(t){
                _.each(t.sections, function(section, index){
                   if(section.grid === 0){
                    t.sections.splice(index, 1);
                   }
               });
            });

            _.each(ls, function(l){
                _.each(l.areas, function(area, index){
                   if(area.grid === 0){
                    l.areas.splice(index, 1);
                   }
               });
            });
        });

        //when the scope is destroyed we need to unsubscribe
        $scope.$on('$destroy', function () {
            unsubscribe();
        });

    });

//this controller simply tells the dialogs service to open a mediaPicker window
//with a specified callback, this callback will receive an object with a selection on it
angular.module('umbraco')
    .controller("Umbraco.PropertyEditors.ImageCropperController",
    function ($rootScope, $routeParams, $scope, $log, mediaHelper, cropperHelper, $timeout, editorState, umbRequestHelper, fileManager) {

        var config = angular.copy($scope.model.config);

        //move previously saved value to the editor
        if ($scope.model.value) {
            //backwards compat with the old file upload (incase some-one swaps them..)
            if (angular.isString($scope.model.value)) {
                config.src = $scope.model.value;
                $scope.model.value = config;
            } else if ($scope.model.value.crops) {
                //sync any config changes with the editor and drop outdated crops
                _.each($scope.model.value.crops, function (saved) {
                    var configured = _.find(config.crops, function (item) { return item.alias === saved.alias });

                    if (configured && configured.height === saved.height && configured.width === saved.width) {
                        configured.coordinates = saved.coordinates;
                    }
                });
                $scope.model.value.crops = config.crops;

                //restore focalpoint if missing
                if (!$scope.model.value.focalPoint) {
                    $scope.model.value.focalPoint = { left: 0.5, top: 0.5 };
                }
            }

            $scope.imageSrc = $scope.model.value.src;
        }


        //crop a specific crop
        $scope.crop = function (crop) {
            $scope.currentCrop = crop;
            $scope.currentPoint = undefined;
        };

        //done cropping
        $scope.done = function () {
            $scope.currentCrop = undefined;
            $scope.currentPoint = undefined;
        };

        //crop a specific crop
        $scope.clear = function (crop) {
            //clear current uploaded files
            fileManager.setFiles($scope.model.alias, []);

            //clear the ui
            $scope.imageSrc = undefined;
            if ($scope.model.value) {
                delete $scope.model.value;
            }
        };

        //show previews
        $scope.togglePreviews = function () {
            if ($scope.showPreviews) {
                $scope.showPreviews = false;
                $scope.tempShowPreviews = false;
            } else {
                $scope.showPreviews = true;
            }
        };

        //on image selected, update the cropper
        $scope.$on("filesSelected", function (ev, args) {
            $scope.model.value = config;

            if (args.files && args.files[0]) {

                fileManager.setFiles($scope.model.alias, args.files);

                var reader = new FileReader();
                reader.onload = function (e) {

                    $scope.$apply(function () {
                        $scope.imageSrc = e.target.result;
                    });

                };

                reader.readAsDataURL(args.files[0]);
            }
        });


        //here we declare a special method which will be called whenever the value has changed from the server
        $scope.model.onValueChanged = function (newVal, oldVal) {
            //clear current uploaded files
            fileManager.setFiles($scope.model.alias, []);
        };

        var unsubscribe = $scope.$on("formSubmitting", function () {
            $scope.done();
        });

        $scope.$on('$destroy', function () {
            unsubscribe();
        });
    })
    .run(function (mediaHelper, umbRequestHelper) {
        if (mediaHelper && mediaHelper.registerFileResolver) {

            //NOTE: The 'entity' can be either a normal media entity or an "entity" returned from the entityResource
            // they contain different data structures so if we need to query against it we need to be aware of this.
            mediaHelper.registerFileResolver("Umbraco.ImageCropper", function (property, entity, thumbnail) {
                if (property.value.src) {

                    if (thumbnail === true) {
                        return property.value.src + "?width=500&mode=max";
                    }
                    else {
                        return property.value.src;
                    }

                    //this is a fallback in case the cropper has been asssigned a upload field
                }
                else if (angular.isString(property.value)) {
                    if (thumbnail) {

                        if (mediaHelper.detectIfImageByExtension(property.value)) {

                            var thumbnailUrl = umbRequestHelper.getApiUrl(
                                "imagesApiBaseUrl",
                                "GetBigThumbnail",
                                [{ originalImagePath: property.value }]);

                            return thumbnailUrl;
                        }
                        else {
                            return null;
                        }

                    }
                    else {
                        return property.value;
                    }
                }

                return null;
            });
        }
    });
angular.module("umbraco").controller("Umbraco.PrevalueEditors.CropSizesController",
	function ($scope, $timeout) {

	    if (!$scope.model.value) {
	        $scope.model.value = [];
	    }

	    $scope.remove = function (item, evt) {
	        evt.preventDefault();
	        $scope.model.value = _.reject($scope.model.value, function (x) {
	            return x.alias === item.alias;
	        });
	    };

	    $scope.edit = function (item, evt) {
	        evt.preventDefault();
	        $scope.newItem = item;
	    };

	    $scope.cancel = function (evt) {
	        evt.preventDefault();
	        $scope.newItem = null;
	    };

	    $scope.add = function (evt) {
	        evt.preventDefault();

	        if ($scope.newItem && $scope.newItem.alias && 
                angular.isNumber($scope.newItem.width) && angular.isNumber($scope.newItem.height) &&
                $scope.newItem.width > 0 && $scope.newItem.height > 0) {

	            var exists = _.find($scope.model.value, function (item) { return $scope.newItem.alias === item.alias; });
	            if (!exists) {
	                $scope.model.value.push($scope.newItem);
	                $scope.newItem = {};
	                $scope.hasError = false;
	                return;
	            }
	        }

	        //there was an error, do the highlight (will be set back by the directive)
	        $scope.hasError = true;
	    };
	});
function includePropsPreValsController($rootScope, $scope, localizationService, contentTypeResource) {

    if (!$scope.model.value) {
        $scope.model.value = [];
    }

    $scope.propertyAliases = [];
    $scope.selectedField = null;
    $scope.systemFields = [
        { value: "sortOrder" },
        { value: "updateDate" },
        { value: "updater" },
        { value: "createDate" },
        { value: "owner" },
        { value: "published"},
        { value: "contentTypeAlias" },
        { value: "email" },
        { value: "username" }
    ];

    $scope.getLocalizedKey = function(alias) {
        switch (alias) {
            case "name":
                return "general_name";
            case "sortOrder":
                return "general_sort";
            case "updateDate":
                return "content_updateDate";
            case "updater":
                return "content_updatedBy";
            case "createDate":
                return "content_createDate";
            case "owner":
                return "content_createBy";
            case "published":
                return "content_isPublished";
            case "contentTypeAlias":
                //NOTE: This will just be 'Document' type even if it's for media/members since this is just a pre-val editor and we don't have a key for 'Content Type Alias'
                return "content_documentType";
            case "email":
                return "general_email";
            case "username":
                return "general_username";
        }
        return alias;
    }

    $scope.removeField = function(e) {
        $scope.model.value = _.reject($scope.model.value, function (x) {
            return x.alias === e.alias;
        }); 
    }

    //now we'll localize these strings, for some reason the directive doesn't work inside of the select group with an ng-model declared
    _.each($scope.systemFields, function (e, i) {
        var key = $scope.getLocalizedKey(e.value);
        localizationService.localize(key).then(function (v) {
            e.name = v;

            switch (e.value) {
                case "updater":
                    e.name += " (Content only)";
                    break;
                case "published":
                    e.name += " (Content only)";
                    break;
                case "email":
                    e.name += " (Members only)";
                    break;
                case "username":
                    e.name += " (Members only)";
                    break;
            }

        });
    });

    // Return a helper with preserved width of cells
    var fixHelper = function (e, ui) {
        var h = ui.clone();

        h.children().each(function () {
            $(this).width($(this).width());            
        });
        h.css("background-color", "lightgray");

        return h;
    };

    $scope.sortableOptions = {
        helper: fixHelper,
        handle: ".handle",
        opacity: 0.5,
        axis: 'y',
        containment: 'parent',
        cursor: 'move',
        items: '> tr',
        tolerance: 'pointer',
        update: function (e, ui) {
            
            // Get the new and old index for the moved element (using the text as the identifier)
            var newIndex = ui.item.index();
            var movedAlias = $('.alias-value', ui.item).text().trim();
            var originalIndex = getAliasIndexByText(movedAlias);

            // Move the element in the model
            if (originalIndex > -1) {
                var movedElement = $scope.model.value[originalIndex];
                $scope.model.value.splice(originalIndex, 1);
                $scope.model.value.splice(newIndex, 0, movedElement);
            }
        }
    };

    contentTypeResource.getAllPropertyTypeAliases().then(function(data) {
        $scope.propertyAliases = data;
    });

    $scope.addField = function () {

        var val = $scope.selectedField;
        var isSystem = val.startsWith("_system_");
        if (isSystem) {
            val = val.trimStart("_system_");
        }

        var exists = _.find($scope.model.value, function (i) {
            return i.alias === val;
        });
        if (!exists) {
            $scope.model.value.push({
                alias: val,
                isSystem: isSystem ? 1 : 0
            });
        }
    }

    function getAliasIndexByText(value) {
        for (var i = 0; i < $scope.model.value.length; i++) {
            if ($scope.model.value[i].alias === value) {
                return i;
            }
        }

        return -1;
    }

}


angular.module("umbraco").controller("Umbraco.PrevalueEditors.IncludePropertiesListViewController", includePropsPreValsController);
function listViewController($rootScope, $scope, $routeParams, $injector, notificationsService, iconHelper, dialogService, editorState, localizationService, $location, appState, $timeout, $q) {

    //this is a quick check to see if we're in create mode, if so just exit - we cannot show children for content 
    // that isn't created yet, if we continue this will use the parent id in the route params which isn't what
    // we want. NOTE: This is just a safety check since when we scaffold an empty model on the server we remove
    // the list view tab entirely when it's new.
    if ($routeParams.create) {
        $scope.isNew = true;
        return;
    }

    //Now we need to check if this is for media, members or content because that will depend on the resources we use
    var contentResource, getContentTypesCallback, getListResultsCallback, deleteItemCallback, getIdCallback, createEditUrlCallback;
    
    //check the config for the entity type, or the current section name (since the config is only set in c#, not in pre-vals)
    if (($scope.model.config.entityType && $scope.model.config.entityType === "member") || (appState.getSectionState("currentSection") === "member")) {
        $scope.entityType = "member";
        contentResource = $injector.get('memberResource');
        getContentTypesCallback = $injector.get('memberTypeResource').getTypes;
        getListResultsCallback = contentResource.getPagedResults;
        deleteItemCallback = contentResource.deleteByKey;
        getIdCallback = function(selected) {
            return selected.key;
        };
        createEditUrlCallback = function(item) {
            return "/" + $scope.entityType + "/" + $scope.entityType + "/edit/" + item.key + "?page=" + $scope.options.pageNumber + "&listName=" + $scope.contentId;
        };
    }
    else {
        //check the config for the entity type, or the current section name (since the config is only set in c#, not in pre-vals)
        if (($scope.model.config.entityType && $scope.model.config.entityType === "media") || (appState.getSectionState("currentSection") === "media")) {
            $scope.entityType = "media";
            contentResource = $injector.get('mediaResource');
            getContentTypesCallback = $injector.get('mediaTypeResource').getAllowedTypes;                        
        }
        else {
            $scope.entityType = "content";
            contentResource = $injector.get('contentResource');
            getContentTypesCallback = $injector.get('contentTypeResource').getAllowedTypes;            
        }
        getListResultsCallback = contentResource.getChildren;
        deleteItemCallback = contentResource.deleteById;
        getIdCallback = function(selected) {
            return selected.id;
        };
        createEditUrlCallback = function(item) {
            return "/" + $scope.entityType + "/" + $scope.entityType + "/edit/" + item.id + "?page=" + $scope.options.pageNumber;
        };
    }

    $scope.pagination = [];
    $scope.isNew = false;
    $scope.actionInProgress = false;
    $scope.listViewResultSet = {
        totalPages: 0,
        items: []
    };

    $scope.options = {
        pageSize: $scope.model.config.pageSize ? $scope.model.config.pageSize : 10,
        pageNumber: ($routeParams.page && Number($routeParams.page) != NaN && Number($routeParams.page) > 0) ? $routeParams.page : 1,
        filter: '',
        orderBy: ($scope.model.config.orderBy ? $scope.model.config.orderBy : 'VersionDate').trim(),
        orderDirection: $scope.model.config.orderDirection ? $scope.model.config.orderDirection.trim() : "desc",
        includeProperties: $scope.model.config.includeProperties ? $scope.model.config.includeProperties : [
            { alias: 'updateDate', header: 'Last edited', isSystem: 1 },
            { alias: 'updater', header: 'Last edited by', isSystem: 1 }
        ],
        allowBulkPublish: true,
        allowBulkUnpublish: true,
        allowBulkDelete: true,        
    };

    //update all of the system includeProperties to enable sorting
    _.each($scope.options.includeProperties, function(e, i) {
        
        if (e.isSystem) {

            //NOTE: special case for contentTypeAlias, it's a system property that cannot be sorted
            // to do that, we'd need to update the base query for content to include the content type alias column
            // which requires another join and would be slower. BUT We are doing this for members so not sure it makes a diff?
            if (e.alias != "contentTypeAlias") {
                e.allowSorting = true;
            }
            
            //localize the header
            var key = getLocalizedKey(e.alias);
            localizationService.localize(key).then(function (v) {
                e.header = v;
            });
        }
    });

    function showNotificationsAndReset(err, reload, successMsg) {

        //check if response is ysod
        if(err.status && err.status >= 500) {
            dialogService.ysodDialog(err);
        }

        $timeout(function() {
            $scope.bulkStatus = "";
            $scope.actionInProgress = false;
        }, 500);

        if (reload === true) {
            $scope.reloadView($scope.contentId);
        }

        if (err.data && angular.isArray(err.data.notifications)) {
            for (var i = 0; i < err.data.notifications.length; i++) {
                notificationsService.showNotification(err.data.notifications[i]);
            }
        }
        else if (successMsg) {
            notificationsService.success("Done", successMsg);
        }
    }

    $scope.isSortDirection = function (col, direction) {
        return $scope.options.orderBy.toUpperCase() == col.toUpperCase() && $scope.options.orderDirection == direction;
    }

    $scope.next = function() {
        if ($scope.options.pageNumber < $scope.listViewResultSet.totalPages) {
            $scope.options.pageNumber++;
            $scope.reloadView($scope.contentId);
            //TODO: this would be nice but causes the whole view to reload
            //$location.search("page", $scope.options.pageNumber);
        }
    };

    $scope.goToPage = function(pageNumber) {
        $scope.options.pageNumber = pageNumber + 1;
        $scope.reloadView($scope.contentId);
        //TODO: this would be nice but causes the whole view to reload
        //$location.search("page", $scope.options.pageNumber);
    };

    $scope.sort = function(field, allow) {
        if (allow) {
            $scope.options.orderBy = field;

            if ($scope.options.orderDirection === "desc") {
                $scope.options.orderDirection = "asc";
            }
            else {
                $scope.options.orderDirection = "desc";
            }

            $scope.reloadView($scope.contentId);
        }
    };

    $scope.prev = function() {
        if ($scope.options.pageNumber > 1) {
            $scope.options.pageNumber--;
            $scope.reloadView($scope.contentId);
            //TODO: this would be nice but causes the whole view to reload
            //$location.search("page", $scope.options.pageNumber);
        }
    };
    

    /*Loads the search results, based on parameters set in prev,next,sort and so on*/
    /*Pagination is done by an array of objects, due angularJS's funky way of monitoring state
    with simple values */

    $scope.reloadView = function (id) {

        getListResultsCallback(id, $scope.options).then(function (data) {

            $scope.actionInProgress = false;

            $scope.listViewResultSet = data;

            //update all values for display
            if ($scope.listViewResultSet.items) {
                _.each($scope.listViewResultSet.items, function (e, index) {
                    setPropertyValues(e);
                });
            }

            //NOTE: This might occur if we are requesting a higher page number than what is actually available, for example
            // if you have more than one page and you delete all items on the last page. In this case, we need to reset to the last
            // available page and then re-load again
            if ($scope.options.pageNumber > $scope.listViewResultSet.totalPages) {
                $scope.options.pageNumber = $scope.listViewResultSet.totalPages;

                //reload!
                $scope.reloadView(id);
            }

            $scope.pagination = [];

            //list 10 pages as per normal
            if ($scope.listViewResultSet.totalPages <= 10) {
                for (var i = 0; i < $scope.listViewResultSet.totalPages; i++) {
                    $scope.pagination.push({
                        val: (i + 1),
                        isActive: $scope.options.pageNumber == (i + 1)
                    });
                }
            }
            else {
                //if there is more than 10 pages, we need to do some fancy bits

                //get the max index to start
                var maxIndex = $scope.listViewResultSet.totalPages - 10;
                //set the start, but it can't be below zero
                var start = Math.max($scope.options.pageNumber - 5, 0);
                //ensure that it's not too far either
                start = Math.min(maxIndex, start);

                for (var i = start; i < (10 + start) ; i++) {
                    $scope.pagination.push({
                        val: (i + 1),
                        isActive: $scope.options.pageNumber == (i + 1)
                    });
                }

                //now, if the start is greater than 0 then '1' will not be displayed, so do the elipses thing
                if (start > 0) {
                    $scope.pagination.unshift({ name: "First", val: 1, isActive: false }, {val: "...",isActive: false});
                }

                //same for the end
                if (start < maxIndex) {
                    $scope.pagination.push({ val: "...", isActive: false }, { name: "Last", val: $scope.listViewResultSet.totalPages, isActive: false });
                }
            }

        });
    };

    $scope.$watch(function() {
        return $scope.options.filter;
    }, _.debounce(function(newVal, oldVal) {
        $scope.$apply(function() {
            if (newVal !== null && newVal !== undefined && newVal !== oldVal) {
                $scope.options.pageNumber = 1;
                $scope.actionInProgress = true;
                $scope.reloadView($scope.contentId);
            }
        });
    }, 1000));

    $scope.filterResults = function (ev) {
        //13: enter

        switch (ev.keyCode) {
            case 13:
                $scope.options.pageNumber = 1;
                $scope.actionInProgress = true;
                $scope.reloadView($scope.contentId);
                break;
        }
    };

    $scope.enterSearch = function ($event) {
        $($event.target).next().focus();
    };

    $scope.selectAll = function($event) {
        var checkbox = $event.target;
        if (!angular.isArray($scope.listViewResultSet.items)) {
            return;
        }
        for (var i = 0; i < $scope.listViewResultSet.items.length; i++) {
            var entity = $scope.listViewResultSet.items[i];
            entity.selected = checkbox.checked;
        }
    };

    $scope.isSelectedAll = function() {
        if (!angular.isArray($scope.listViewResultSet.items)) {
            return false;
        }
        return _.every($scope.listViewResultSet.items, function(item) {
            return item.selected;
        });
    };

    $scope.isAnythingSelected = function() {
        if (!angular.isArray($scope.listViewResultSet.items)) {
            return false;
        }
        return _.some($scope.listViewResultSet.items, function(item) {
            return item.selected;
        });
    };

    $scope.getIcon = function(entry) {
        return iconHelper.convertFromLegacyIcon(entry.icon);
    };

    function serial(selected, fn, getStatusMsg, index) {
        return fn(selected, index).then(function (content) {
            index++;
            $scope.bulkStatus = getStatusMsg(index, selected.length);
            return index < selected.length ? serial(selected, fn, getStatusMsg, index) : content;
        }, function (err) {
            var reload = index > 0;
            showNotificationsAndReset(err, reload);
            return err;
        });
    }

    function applySelected(fn, getStatusMsg, getSuccessMsg, confirmMsg) {
        var selected = _.filter($scope.listViewResultSet.items, function (item) {
            return item.selected;
        });
        if (selected.length === 0)
            return;
        if (confirmMsg && !confirm(confirmMsg))
            return;

        $scope.actionInProgress = true;
        $scope.bulkStatus = getStatusMsg(0, selected.length);

        serial(selected, fn, getStatusMsg, 0).then(function (result) {
            // executes once the whole selection has been processed
            // in case of an error (caught by serial), result will be the error
            if (!(result.data && angular.isArray(result.data.notifications)))
                showNotificationsAndReset(result, true, getSuccessMsg(selected.length));
        });
    };

    $scope.delete = function () {
        applySelected(
            function (selected, index) { return deleteItemCallback(getIdCallback(selected[index])) },
            function (count, total) { return "Deleted " + count + " out of " + total + " document" + (total > 1 ? "s" : "") },
            function (total) { return "Deleted " + total + " document" + (total > 1 ? "s" : "") },
            "Sure you want to delete?");
    };

    $scope.publish = function () {
        applySelected(
            function (selected, index) { return contentResource.publishById(getIdCallback(selected[index])); },
            function (count, total) { return "Published " + count + " out of " + total + " document" + (total > 1 ? "s" : "") },
            function (total) { return "Published " + total + " document" + (total > 1 ? "s" : "") });
    };

    $scope.unpublish = function() {
        applySelected(
            function (selected, index) { return contentResource.unPublish(getIdCallback(selected[index])); },
            function (count, total) { return "Unpublished " + count + " out of " + total + " document" + (total > 1 ? "s" : "") },
            function (total) { return "Unpublished " + total + " document" + (total > 1 ? "s" : "") });
    };

    function getCustomPropertyValue(alias, properties) {
        var value = '';
        var index = 0;
        var foundAlias = false;
        for (var i = 0; i < properties.length; i++) {
            if (properties[i].alias == alias) {
                foundAlias = true;
                break;
            }
            index++;
        }

        if (foundAlias) {
            value = properties[index].value;
        }

        return value;
    };

    /** This ensures that the correct value is set for each item in a row, we don't want to call a function during interpolation or ng-bind as performance is really bad that way */
    function setPropertyValues(result) {

        //set the edit url
        result.editPath = createEditUrlCallback(result);

        _.each($scope.options.includeProperties, function (e, i) {

            var alias = e.alias;

            // First try to pull the value directly from the alias (e.g. updatedBy)        
            var value = result[alias];
            
            // If this returns an object, look for the name property of that (e.g. owner.name)
            if (value === Object(value)) {
                value = value['name'];
            }

            // If we've got nothing yet, look at a user defined property
            if (typeof value === 'undefined') {
                value = getCustomPropertyValue(alias, result.properties);
            }

            // If we have a date, format it
            if (isDate(value)) {
                value = value.substring(0, value.length - 3);
            }

            // set what we've got on the result
            result[alias] = value;
        });


    };

    function isDate(val) {
        if (angular.isString(val)) {
            return val.match(/^(\d{4})\-(\d{2})\-(\d{2})\ (\d{2})\:(\d{2})\:(\d{2})$/);
        }
        return false;
    };

    function initView() {
        if ($routeParams.id) {
            $scope.listViewAllowedTypes = getContentTypesCallback($routeParams.id);

            $scope.contentId = $routeParams.id;
            $scope.isTrashed = $routeParams.id === "-20" || $routeParams.id === "-21";

            $scope.reloadView($scope.contentId);
        }
    };

    function getLocalizedKey(alias) {

        switch (alias) {
            case "sortOrder":
                return "general_sort";
            case "updateDate":
                return "content_updateDate";
            case "updater":
                return "content_updatedBy";
            case "createDate":
                return "content_createDate";
            case "owner":
                return "content_createBy";
            case "published":
                return "content_isPublished";
            case "contentTypeAlias":
                //TODO: Check for members
                return $scope.entityType === "content" ? "content_documentType" : "content_mediatype";
            case "email":
                return "general_email";
            case "username":
                return "general_username";
        }
        return alias;
    }

    //GO!
    initView();
}


angular.module("umbraco").controller("Umbraco.PropertyEditors.ListViewController", listViewController);
function sortByPreValsController($rootScope, $scope, localizationService) {

    $scope.sortByFields = [
        { value: "SortOrder", key: "general_sort" },
        { value: "Name", key: "general_name" },
        { value: "VersionDate", key: "content_updateDate" },
        { value: "Updater", key: "content_updatedBy" },
        { value: "CreateDate", key: "content_createDate" },
        { value: "Owner", key: "content_createBy" },
        { value: "ContentTypeAlias", key: "content_documentType" },
        { value: "Published", key: "content_isPublished" },
        { value: "Email", key: "general_email" },
        { value: "Username", key: "general_username" }
    ];
    
    //now we'll localize these strings, for some reason the directive doesn't work inside of the select group with an ng-model declared
    _.each($scope.sortByFields, function (e, i) {
        localizationService.localize(e.key).then(function (v) {
            e.name = v;

            switch (e.value) {
                case "Updater":
                    e.name += " (Content only)";
                    break;
                case "Published":
                    e.name += " (Content only)";
                    break;
                case "Email":
                    e.name += " (Members only)";
                    break;
                case "Username":
                    e.name += " (Members only)";
                    break;
            }
        });
    });

}


angular.module("umbraco").controller("Umbraco.PrevalueEditors.SortByListViewController", sortByPreValsController);
//DO NOT DELETE THIS, this is in use... 
angular.module('umbraco')
.controller("Umbraco.PropertyEditors.MacroContainerController",
	
	function($scope, dialogService, entityResource, macroService){
		$scope.renderModel = [];
		
		if($scope.model.value){
			var macros = $scope.model.value.split('>');

			angular.forEach(macros, function(syntax, key){
				if(syntax && syntax.length > 10){
					//re-add the char we split on
					syntax = syntax + ">";
					var parsed = macroService.parseMacroSyntax(syntax);
					if(!parsed){
						parsed = {};
					}
					
					parsed.syntax = syntax;
					collectDetails(parsed);
					$scope.renderModel.push(parsed);
				}
			});
		}

		
		function collectDetails(macro){
			macro.details = "";
			if(macro.macroParamsDictionary){
				angular.forEach((macro.macroParamsDictionary), function(value, key){
					macro.details += key + ": " + value + " ";	
				});	
			}		
		}

		function openDialog(index){
			var dialogData = {
			    allowedMacros: $scope.model.config.allowed
			};

			if(index !== null && $scope.renderModel[index]) {
				var macro = $scope.renderModel[index];
				dialogData["macroData"] = macro;
			}
			
			dialogService.macroPicker({
				dialogData : dialogData,
				callback: function(data) {

					collectDetails(data);

					//update the raw syntax and the list...
					if(index !== null && $scope.renderModel[index]) {
						$scope.renderModel[index] = data;
					} else {
						$scope.renderModel.push(data);
					}
				}
			});
		}



		$scope.edit =function(index){
				openDialog(index);
		};

		$scope.add = function () {

		    if ($scope.model.config.max && $scope.model.config.max > 0 && $scope.renderModel.length >= $scope.model.config.max) {
                //cannot add more than the max
		        return;
		    }
		    
			openDialog();
		};

		$scope.remove =function(index){
			$scope.renderModel.splice(index, 1);
		};

	    $scope.clear = function() {
	        $scope.model.value = "";
	        $scope.renderModel = [];
	    };

	    var unsubscribe = $scope.$on("formSubmitting", function (ev, args) {	
			var syntax = [];
	    	angular.forEach($scope.renderModel, function(value, key){
	    		syntax.push(value.syntax);
	    	});

			$scope.model.value = syntax.join("");
	    });

	    //when the scope is destroyed we need to unsubscribe
	    $scope.$on('$destroy', function () {
	        unsubscribe();
	    });


		function trim(str, chr) {
			var rgxtrim = (!chr) ? new RegExp('^\\s+|\\s+$', 'g') : new RegExp('^'+chr+'+|'+chr+'+$', 'g');
			return str.replace(rgxtrim, '');
		}

});

function MacroListController($scope, entityResource) {

    $scope.items = [];
    
    entityResource.getAll("Macro").then(function(items) {
        _.each(items, function(i) {
            $scope.items.push({ name: i.name, alias: i.alias });
        });
        
    });


}

angular.module("umbraco").controller("Umbraco.PrevalueEditors.MacroList", MacroListController);

//inject umbracos assetsServce and dialog service
function MarkdownEditorController($scope, assetsService, dialogService, $timeout) {

    //tell the assets service to load the markdown.editor libs from the markdown editors
    //plugin folder

    if ($scope.model.value === null || $scope.model.value === "") {
        $scope.model.value = $scope.model.config.defaultValue;
    }

    assetsService
        .load([
            "lib/markdown/markdown.converter.js",
            "lib/markdown/markdown.sanitizer.js",
            "lib/markdown/markdown.editor.js"
        ])
        .then(function () {

            // we need a short delay to wait for the textbox to appear.
            setTimeout(function () {
                //this function will execute when all dependencies have loaded
                // but in the case that they've been previously loaded, we can only 
                // init the md editor after this digest because the DOM needs to be ready first
                // so run the init on a timeout
                $timeout(function () {
                    var converter2 = new Markdown.Converter();
                    var editor2 = new Markdown.Editor(converter2, "-" + $scope.model.alias);
                    editor2.run();

                    //subscribe to the image dialog clicks
                    editor2.hooks.set("insertImageDialog", function (callback) {

                        dialogService.mediaPicker({
                            callback: function (data) {
                                callback(data.image);
                            }
                        });

                        return true; // tell the editor that we'll take care of getting the image url
                    });
                }, 200);
            });

            //load the seperat css for the editor to avoid it blocking our js loading TEMP HACK
            assetsService.loadCss("lib/markdown/markdown.css");
        })
}

angular.module("umbraco").controller("Umbraco.PropertyEditors.MarkdownEditorController", MarkdownEditorController);
//this controller simply tells the dialogs service to open a mediaPicker window
//with a specified callback, this callback will receive an object with a selection on it
angular.module('umbraco').controller("Umbraco.PropertyEditors.MediaPickerController",
    function ($rootScope, $scope, dialogService, entityResource, mediaResource, mediaHelper, $timeout, userService, angularHelper) {

        //check the pre-values for multi-picker
        var multiPicker = $scope.model.config.multiPicker && $scope.model.config.multiPicker !== '0' ? true : false;

        if (!$scope.model.config.startNodeId) {
            userService.getCurrentUser().then(function (userData) {
                $scope.model.config.startNodeId = userData.startMediaId;
            });
        }
            

         
        function setupViewModel() {
            $scope.images = [];
            $scope.ids = []; 

            if ($scope.model.value) {
                var ids = $scope.model.value.split(',');

                //NOTE: We need to use the entityResource NOT the mediaResource here because
                // the mediaResource has server side auth configured for which the user must have
                // access to the media section, if they don't they'll get auth errors. The entityResource
                // acts differently in that it allows access if the user has access to any of the apps that
                // might require it's use. Therefore we need to use the metatData property to get at the thumbnail
                // value.

                entityResource.getByIds(ids, "Media").then(function (medias) {

                    _.each(medias, function (media, i) {
                        
                        //only show non-trashed items
                        if (media.parentId >= -1) {

                            if (!media.thumbnail) { 
                                media.thumbnail = mediaHelper.resolveFileFromEntity(media, true);
                            }

                            $scope.images.push(media);
                            $scope.ids.push(media.id);   
                        }
                    });

                    $scope.sync();
                });
            }
        }

        setupViewModel();

        $scope.remove = function(index) {
            $scope.images.splice(index, 1);
            $scope.ids.splice(index, 1);
            angularHelper.getCurrentForm($scope).$setDirty();
            $scope.sync();
        };

        $scope.add = function() {
            dialogService.mediaPicker({
                startNodeId: $scope.model.config.startNodeId,
                multiPicker: multiPicker,
                callback: function(data) {
                    
                    //it's only a single selector, so make it into an array
                    if (!multiPicker) {
                        data = [data];
                    }
                    
                    _.each(data, function(media, i) {

                        if (!media.thumbnail) {
                            media.thumbnail = mediaHelper.resolveFileFromEntity(media, true);
                        }

                        $scope.images.push(media);
                        $scope.ids.push(media.id);
                        angularHelper.getCurrentForm($scope).$setDirty();
                    });

                    $scope.sync();
                }
            });
        };

       $scope.sortableOptions = {
           update: function(e, ui) {
               var r = [];
               //TODO: Instead of doing this with a half second delay would be better to use a watch like we do in the 
               // content picker. THen we don't have to worry about setting ids, render models, models, we just set one and let the 
               // watch do all the rest.
                $timeout(function(){
                    angular.forEach($scope.images, function(value, key){
                        r.push(value.id);
                    });

                    $scope.ids = r;
                    $scope.sync();
                }, 500, false);
            }
        };

        $scope.sync = function() {
            $scope.model.value = $scope.ids.join();
        };

        $scope.showAdd = function () {
            if (!multiPicker) {
                if ($scope.model.value && $scope.model.value !== "") {
                    return false;
                }
            }
            return true;
        };

        //here we declare a special method which will be called whenever the value has changed from the server
        //this is instead of doing a watch on the model.value = faster
        $scope.model.onValueChanged = function (newVal, oldVal) {
            //update the display val again if it has changed from the server
            setupViewModel();
        };

    });

//this controller simply tells the dialogs service to open a memberPicker window
//with a specified callback, this callback will receive an object with a selection on it
function memberGroupPicker($scope, dialogService){

    function trim(str, chr) {
        var rgxtrim = (!chr) ? new RegExp('^\\s+|\\s+$', 'g') : new RegExp('^' + chr + '+|' + chr + '+$', 'g');
        return str.replace(rgxtrim, '');
    }

    $scope.renderModel = [];
    
    if ($scope.model.value) {
        var modelIds = $scope.model.value.split(',');
        _.each(modelIds, function (item, i) {
            $scope.renderModel.push({ name: item, id: item, icon: 'icon-users' });
        });
    }
	    
    var dialogOptions = {
        multiPicker: true,
        entityType: "MemberGroup",
        section: "membergroup",
        treeAlias: "memberGroup",
        filter: "",
        filterCssClass: "not-allowed",
        callback: function (data) {
            if (angular.isArray(data)) {
                _.each(data, function (item, i) {
                    $scope.add(item);
                });
            } else {
                $scope.clear();
                $scope.add(data);

            }
        }
    };

    //since most of the pre-value config's are used in the dialog options (i.e. maxNumber, minNumber, etc...) we'll merge the 
    // pre-value config on to the dialog options
    if($scope.model.config){
        angular.extend(dialogOptions, $scope.model.config);
    }

    $scope.openMemberGroupPicker =function() {
        var d = dialogService.memberGroupPicker(dialogOptions);
    };


    $scope.remove =function(index){
        $scope.renderModel.splice(index, 1);
    };

    $scope.add = function (item) {
        var currIds = _.map($scope.renderModel, function (i) {
            return i.id;
        });

        if (currIds.indexOf(item) < 0) {
            $scope.renderModel.push({ name: item, id: item, icon: 'icon-users' });
        }	
    };

    $scope.clear = function() {
        $scope.renderModel = [];
    };

    var unsubscribe = $scope.$on("formSubmitting", function (ev, args) {
        var currIds = _.map($scope.renderModel, function (i) {
            return i.id;
        });
        $scope.model.value = trim(currIds.join(), ",");
    });

    //when the scope is destroyed we need to unsubscribe
    $scope.$on('$destroy', function () {
        unsubscribe();
    });

}

angular.module('umbraco').controller("Umbraco.PropertyEditors.MemberGroupPickerController", memberGroupPicker);
function memberGroupController($rootScope, $scope, dialogService, mediaResource, imageHelper, $log) {

    //set the available to the keys of the dictionary who's value is true
    $scope.getAvailable = function () {
        var available = [];
        for (var n in $scope.model.value) {
            if ($scope.model.value[n] === false) {
                available.push(n);
            }
        }
        return available;
    };
    //set the selected to the keys of the dictionary who's value is true
    $scope.getSelected = function () {
        var selected = [];
        for (var n in $scope.model.value) {
            if ($scope.model.value[n] === true) {
                selected.push(n);
            }
        }
        return selected;
    };

    $scope.addItem = function(item) {
        //keep the model up to date
        $scope.model.value[item] = true;
    };
    
    $scope.removeItem = function (item) {
        //keep the model up to date
        $scope.model.value[item] = false;
    };


}
angular.module('umbraco').controller("Umbraco.PropertyEditors.MemberGroupController", memberGroupController);
//this controller simply tells the dialogs service to open a memberPicker window
//with a specified callback, this callback will receive an object with a selection on it
function memberPickerController($scope, dialogService, entityResource, $log, iconHelper, angularHelper){

    function trim(str, chr) {
        var rgxtrim = (!chr) ? new RegExp('^\\s+|\\s+$', 'g') : new RegExp('^' + chr + '+|' + chr + '+$', 'g');
        return str.replace(rgxtrim, '');
    }

    $scope.renderModel = [];

    var dialogOptions = {
        multiPicker: false,
        entityType: "Member",
        section: "member",
        treeAlias: "member",
        filter: function(i) {
            return i.metaData.isContainer == true;
        },
        filterCssClass: "not-allowed",
        callback: function(data) {
            if (angular.isArray(data)) {
                _.each(data, function (item, i) {
                    $scope.add(item);
                });
            } else {
                $scope.clear();
                $scope.add(data);
            }
            angularHelper.getCurrentForm($scope).$setDirty();
        }
    };

    //since most of the pre-value config's are used in the dialog options (i.e. maxNumber, minNumber, etc...) we'll merge the 
    // pre-value config on to the dialog options
    if ($scope.model.config) {
        angular.extend(dialogOptions, $scope.model.config);
    }
    
    $scope.openMemberPicker =function() {
        var d = dialogService.memberPicker(dialogOptions);
    };


    $scope.remove =function(index){
        $scope.renderModel.splice(index, 1);
    };

    $scope.add = function (item) {
        var currIds = _.map($scope.renderModel, function (i) {
            return i.id;
        });

        if (currIds.indexOf(item.id) < 0) {
            item.icon = iconHelper.convertFromLegacyIcon(item.icon);
            $scope.renderModel.push({name: item.name, id: item.id, icon: item.icon});				
        }	
    };

    $scope.clear = function() {
        $scope.renderModel = [];
    };
	
    var unsubscribe = $scope.$on("formSubmitting", function (ev, args) {
        var currIds = _.map($scope.renderModel, function (i) {
            return i.id;
        });
        $scope.model.value = trim(currIds.join(), ",");
    });

    //when the scope is destroyed we need to unsubscribe
    $scope.$on('$destroy', function () {
        unsubscribe();
    });

    //load member data
    var modelIds = $scope.model.value ? $scope.model.value.split(',') : [];
    entityResource.getByIds(modelIds, "Member").then(function (data) {
        _.each(data, function (item, i) {
            item.icon = iconHelper.convertFromLegacyIcon(item.icon);
            $scope.renderModel.push({ name: item.name, id: item.id, icon: item.icon });
        });
    });
}


angular.module('umbraco').controller("Umbraco.PropertyEditors.MemberPickerController", memberPickerController);
function MultipleTextBoxController($scope) {

    $scope.sortableOptions = {
        axis: 'y',
        containment: 'parent',
        cursor: 'move',
        items: '> div.control-group',
        tolerance: 'pointer'
    };

    if (!$scope.model.value) {
        $scope.model.value = [];
    }
    
    //add any fields that there isn't values for
    if ($scope.model.config.min > 0) {
        for (var i = 0; i < $scope.model.config.min; i++) {
            if ((i + 1) > $scope.model.value.length) {
                $scope.model.value.push({ value: "" });
            }
        }
    }

    $scope.add = function () {
        if ($scope.model.config.max <= 0 || $scope.model.value.length < $scope.model.config.max) {
            $scope.model.value.push({ value: "" });
        }
    };

    $scope.remove = function(index) {
        var remainder = [];
        for (var x = 0; x < $scope.model.value.length; x++) {
            if (x !== index) {
                remainder.push($scope.model.value[x]);
            }
        }
        $scope.model.value = remainder;
    };

}

angular.module("umbraco").controller("Umbraco.PropertyEditors.MultipleTextBoxController", MultipleTextBoxController);

angular.module("umbraco").controller("Umbraco.PropertyEditors.RadioButtonsController",
    function($scope) {
        
        if (angular.isObject($scope.model.config.items)) {
            
            //now we need to format the items in the dictionary because we always want to have an array
            var newItems = [];
            var vals = _.values($scope.model.config.items);
            var keys = _.keys($scope.model.config.items);
            for (var i = 0; i < vals.length; i++) {
                newItems.push({ id: keys[i], sortOrder: vals[i].sortOrder, value: vals[i].value });
            }

            //ensure the items are sorted by the provided sort order
            newItems.sort(function (a, b) { return (a.sortOrder > b.sortOrder) ? 1 : ((b.sortOrder > a.sortOrder) ? -1 : 0); });

            //re-assign
            $scope.model.config.items = newItems;

        }

    });

/**
 * @ngdoc controller
 * @name Umbraco.Editors.ReadOnlyValueController
 * @function
 * 
 * @description
 * The controller for the readonlyvalue property editor. 
 *  This controller offer more functionality than just a simple label as it will be able to apply formatting to the 
 *  value to be displayed. This means that we also have to apply more complex logic of watching the model value when 
 *  it changes because we are creating a new scope value called displayvalue which will never change based on the server data.
 *  In some cases after a form submission, the server will modify the data that has been persisted, especially in the cases of 
 *  readonlyvalues so we need to ensure that after the form is submitted that the new data is reflected here.
*/
function ReadOnlyValueController($rootScope, $scope, $filter) {

    function formatDisplayValue() {
        
        if ($scope.model.config &&
        angular.isArray($scope.model.config) &&
        $scope.model.config.length > 0 &&
        $scope.model.config[0] &&
        $scope.model.config.filter) {

            if ($scope.model.config.format) {
                $scope.displayvalue = $filter($scope.model.config.filter)($scope.model.value, $scope.model.config.format);
            } else {
                $scope.displayvalue = $filter($scope.model.config.filter)($scope.model.value);
            }
        } else {
            $scope.displayvalue = $scope.model.value;
        }

    }

    //format the display value on init:
    formatDisplayValue();
    
    $scope.$watch("model.value", function (newVal, oldVal) {
        //cannot just check for !newVal because it might be an empty string which we 
        //want to look for.
        if (newVal !== null && newVal !== undefined && newVal !== oldVal) {
            //update the display val again
            formatDisplayValue();
        }
    });
}

angular.module('umbraco').controller("Umbraco.PropertyEditors.ReadOnlyValueController", ReadOnlyValueController);
angular.module("umbraco")
    .controller("Umbraco.PropertyEditors.RelatedLinksController",
        function ($rootScope, $scope, dialogService) {

            if (!$scope.model.value) {
                $scope.model.value = [];
            }
            
            $scope.newCaption = '';
            $scope.newLink = 'http://';
            $scope.newNewWindow = false;
            $scope.newInternal = null;
            $scope.newInternalName = '';
            $scope.addExternal = true;
            $scope.currentEditLink = null;
            $scope.hasError = false;

            $scope.internal = function ($event) {
                $scope.currentEditLink = null;
                var d = dialogService.contentPicker({ multipicker: false, callback: select });
                $event.preventDefault();
            };
            
            $scope.selectInternal = function ($event, link) {

                $scope.currentEditLink = link;
                var d = dialogService.contentPicker({ multipicker: false, callback: select });
                $event.preventDefault();
            };

            $scope.edit = function (idx) {
                for (var i = 0; i < $scope.model.value.length; i++) {
                    $scope.model.value[i].edit = false;
                }
                $scope.model.value[idx].edit = true;
            };
  

            $scope.saveEdit = function (idx) {
                $scope.model.value[idx].title = $scope.model.value[idx].caption;
                $scope.model.value[idx].edit = false;
            };

            $scope.delete = function (idx) {               
                $scope.model.value.splice(idx, 1);               
            };

            $scope.add = function ($event) {
                if ($scope.newCaption == "") {
                    $scope.hasError = true;
                } else {
                    if ($scope.addExternal) {
                        var newExtLink = new function() {
                            this.caption = $scope.newCaption;
                            this.link = $scope.newLink;
                            this.newWindow = $scope.newNewWindow;
                            this.edit = false;
                            this.isInternal = false;
                            this.type = "external";
                            this.title = $scope.newCaption;
                        };
                        $scope.model.value.push(newExtLink);
                    } else {
                        var newIntLink = new function() {
                            this.caption = $scope.newCaption;
                            this.link = $scope.newInternal;
                            this.newWindow = $scope.newNewWindow;
                            this.internal = $scope.newInternal;
                            this.edit = false;
                            this.isInternal = true;
                            this.internalName = $scope.newInternalName;
                            this.type = "internal";
                            this.title = $scope.newCaption;
                        };
                        $scope.model.value.push(newIntLink);
                    }
                    $scope.newCaption = '';
                    $scope.newLink = 'http://';
                    $scope.newNewWindow = false;
                    $scope.newInternal = null;
                    $scope.newInternalName = '';

                }
                $event.preventDefault();
            };

            $scope.switch = function ($event) {
                $scope.addExternal = !$scope.addExternal;
                $event.preventDefault();
            };
            
            $scope.switchLinkType = function ($event, link) {
                link.isInternal = !link.isInternal;                
                link.type = link.isInternal ? "internal" : "external";
                if (!link.isInternal)
                    link.link = $scope.newLink;
                $event.preventDefault();
            };

            $scope.move = function (index, direction) {
                var temp = $scope.model.value[index];
                $scope.model.value[index] = $scope.model.value[index + direction];
                $scope.model.value[index + direction] = temp;                
            };

            $scope.sortableOptions = {
                containment: 'parent',
                cursor: 'move',
                helper: function (e, ui) {
                    // When sorting <trs>, the cells collapse.  This helper fixes that: http://www.foliotek.com/devblog/make-table-rows-sortable-using-jquery-ui-sortable/
                    ui.children().each(function () {
                        $(this).width($(this).width());
                    });
                    return ui;
                },
                items: '> tr',
                tolerance: 'pointer',
                update: function (e, ui) {
                    // Get the new and old index for the moved element (using the URL as the identifier)
                    var newIndex = ui.item.index();
                    var movedLinkUrl = ui.item.attr('data-link');
                    var originalIndex = getElementIndexByUrl(movedLinkUrl);

                    // Move the element in the model
                    var movedElement = $scope.model.value[originalIndex];
                    $scope.model.value.splice(originalIndex, 1);
                    $scope.model.value.splice(newIndex, 0, movedElement);
                }
            };

            function getElementIndexByUrl(url) {
                for (var i = 0; i < $scope.model.value.length; i++) {
                    if ($scope.model.value[i].link == url) {
                        return i;
                    }
                }

                return -1;
            }

            function select(data) {
                if ($scope.currentEditLink != null) {
                    $scope.currentEditLink.internal = data.id;
                    $scope.currentEditLink.internalName = data.name;
                    $scope.currentEditLink.link = data.id;
                } else {
                    $scope.newInternal = data.id;
                    $scope.newInternalName = data.name;
                }
            }            
        });
angular.module("umbraco")
    .controller("Umbraco.PropertyEditors.RTEController",
    function ($rootScope, $scope, $q, dialogService, $log, imageHelper, assetsService, $timeout, tinyMceService, angularHelper, stylesheetResource) {
        
        $scope.isLoading = true;

        //To id the html textarea we need to use the datetime ticks because we can have multiple rte's per a single property alias
        // because now we have to support having 2x (maybe more at some stage) content editors being displayed at once. This is because
        // we have this mini content editor panel that can be launched with MNTP.
        var d = new Date();
        var n = d.getTime();
        $scope.textAreaHtmlId = $scope.model.alias + "_" + n + "_rte";

        var alreadyDirty = false;
        function syncContent(editor){
            editor.save();
            angularHelper.safeApply($scope, function () {
                $scope.model.value = editor.getContent();
            });

            if (!alreadyDirty) {
                //make the form dirty manually so that the track changes works, setting our model doesn't trigger
                // the angular bits because tinymce replaces the textarea.
                var currForm = angularHelper.getCurrentForm($scope);
                currForm.$setDirty();
                alreadyDirty = true;
            }
        }

        tinyMceService.configuration().then(function (tinyMceConfig) {

            //config value from general tinymce.config file
            var validElements = tinyMceConfig.validElements;

            //These are absolutely required in order for the macros to render inline
            //we put these as extended elements because they get merged on top of the normal allowed elements by tiny mce
            var extendedValidElements = "@[id|class|style],-div[id|dir|class|align|style],ins[datetime|cite],-ul[class|style],-li[class|style]";

            var invalidElements = tinyMceConfig.inValidElements;
            var plugins = _.map(tinyMceConfig.plugins, function (plugin) {
                if (plugin.useOnFrontend) {
                    return plugin.name;
                }
            }).join(" ");

            var editorConfig = $scope.model.config.editor;
            if (!editorConfig || angular.isString(editorConfig)) {
                editorConfig = tinyMceService.defaultPrevalues();
            }

            //config value on the data type
            var toolbar = editorConfig.toolbar.join(" | ");
            var stylesheets = [];
            var styleFormats = [];
            var await = [];
            if (!editorConfig.maxImageSize && editorConfig.maxImageSize != 0) {
                editorConfig.maxImageSize = tinyMceService.defaultPrevalues().maxImageSize;
            }

            //queue file loading
            if (typeof tinymce === "undefined") { // Don't reload tinymce if already loaded
                await.push(assetsService.loadJs("lib/tinymce/tinymce.min.js", $scope));
            }

            //queue rules loading
            angular.forEach(editorConfig.stylesheets, function (val, key) {
                stylesheets.push(Umbraco.Sys.ServerVariables.umbracoSettings.cssPath + "/" + val + ".css?" + new Date().getTime());
                await.push(stylesheetResource.getRulesByName(val).then(function (rules) {
                    angular.forEach(rules, function (rule) {
                        var r = {};
                        r.title = rule.name;
                        if (rule.selector[0] == ".") {
                            r.inline = "span";
                            r.classes = rule.selector.substring(1);
                        }
                        else if (rule.selector[0] == "#") {
                            r.inline = "span";
                            r.attributes = { id: rule.selector.substring(1) };
                        }
                        else if (rule.selector[0] != "." && rule.selector.indexOf(".") > -1) {
                            var split = rule.selector.split(".");
                            r.block = split[0];
                            r.classes = rule.selector.substring(rule.selector.indexOf(".") + 1).replace(".", " ");
                        }
                        else if (rule.selector[0] != "#" && rule.selector.indexOf("#") > -1) {
                            var split = rule.selector.split("#");
                            r.block = split[0];
                            r.classes = rule.selector.substring(rule.selector.indexOf("#") + 1);
                        }
                        else {
                            r.block = rule.selector;
                        }

                        styleFormats.push(r);
                    });
                }));
            });


            //stores a reference to the editor
            var tinyMceEditor = null;

            //wait for queue to end
            $q.all(await).then(function () {

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
                    height: editorConfig.dimensions.height,
                    width: editorConfig.dimensions.width,
                    maxImageSize: editorConfig.maxImageSize,
                    toolbar: toolbar,
                    content_css: stylesheets.join(','),
                    relative_urls: false,
                    style_formats: styleFormats
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
                baseLineConfigObj.elements = $scope.textAreaHtmlId; //this is the exact textarea id to replace!
                baseLineConfigObj.setup = function (editor) {

                    //set the reference
                    tinyMceEditor = editor;

                    //enable browser based spell checking
                    editor.on('init', function (e) {
                        editor.getBody().setAttribute('spellcheck', true);
                    });

                    //We need to listen on multiple things here because of the nature of tinymce, it doesn't 
                    //fire events when you think!
                    //The change event doesn't fire when content changes, only when cursor points are changed and undo points
                    //are created. the blur event doesn't fire if you insert content into the editor with a button and then 
                    //press save. 
                    //We have a couple of options, one is to do a set timeout and check for isDirty on the editor, or we can 
                    //listen to both change and blur and also on our own 'saving' event. I think this will be best because a 
                    //timer might end up using unwanted cpu and we'd still have to listen to our saving event in case they clicked
                    //save before the timeout elapsed.

                    //TODO: We need to re-enable something like this to ensure the track changes is working with tinymce
                    // so we can detect if the form is dirty or not, Per has some better events to use as this one triggers
                    // even if you just enter/exit with mouse cursuor which doesn't really mean it's changed.
                    // see: http://issues.umbraco.org/issue/U4-4485
                    //var alreadyDirty = false;
                    //editor.on('change', function (e) {
                    //    angularHelper.safeApply($scope, function () {
                    //        $scope.model.value = editor.getContent();

                    //        if (!alreadyDirty) {
                    //            //make the form dirty manually so that the track changes works, setting our model doesn't trigger
                    //            // the angular bits because tinymce replaces the textarea.
                    //            var currForm = angularHelper.getCurrentForm($scope);
                    //            currForm.$setDirty();
                    //            alreadyDirty = true;
                    //        }
                            
                    //    });
                    //});

                    //when we leave the editor (maybe)
                    editor.on('blur', function (e) {
                        editor.save();
                        angularHelper.safeApply($scope, function () {
                            $scope.model.value = editor.getContent();
                        });
                    });

                    //when buttons modify content
                    editor.on('ExecCommand', function (e) {
                        syncContent(editor);
                    });

                    // Update model on keypress
                    editor.on('KeyUp', function (e) {
                        syncContent(editor);
                    });

                    // Update model on change, i.e. copy/pasted text, plugins altering content
                    editor.on('SetContent', function (e) {
                        if (!e.initial) {
                            syncContent(editor);
                        }
                    });


                    editor.on('ObjectResized', function (e) {
                        var qs = "?width=" + e.width + "&height=" + e.height;
                        var srcAttr = $(e.target).attr("src");
                        var path = srcAttr.split("?")[0];
                        $(e.target).attr("data-mce-src", path + qs);
                        
                        syncContent(editor);
                    });


                    //Create the insert media plugin
                    tinyMceService.createMediaPicker(editor, $scope);

                    //Create the embedded plugin
                    tinyMceService.createInsertEmbeddedMedia(editor, $scope);

                    //Create the insert macro plugin
                    tinyMceService.createInsertMacro(editor, $scope);
                };




                /** Loads in the editor */
                function loadTinyMce() {

                    //we need to add a timeout here, to force a redraw so TinyMCE can find
                    //the elements needed
                    $timeout(function () {
                        tinymce.DOM.events.domLoaded = true;
                        tinymce.init(baseLineConfigObj);

                        $scope.isLoading = false;

                    }, 200, false);
                }




                loadTinyMce();

                //here we declare a special method which will be called whenever the value has changed from the server
                //this is instead of doing a watch on the model.value = faster
                $scope.model.onValueChanged = function (newVal, oldVal) {
                    //update the display val again if it has changed from the server;
                    tinyMceEditor.setContent(newVal, { format: 'raw' });
                    //we need to manually fire this event since it is only ever fired based on loading from the DOM, this
                    // is required for our plugins listening to this event to execute
                    tinyMceEditor.fire('LoadContent', null);
                };

                //listen for formSubmitting event (the result is callback used to remove the event subscription)
                var unsubscribe = $scope.$on("formSubmitting", function () {
                    //TODO: Here we should parse out the macro rendered content so we can save on a lot of bytes in data xfer
                    // we do parse it out on the server side but would be nice to do that on the client side before as well.
                    $scope.model.value = tinyMceEditor.getContent();
                });

                //when the element is disposed we need to unsubscribe!
                // NOTE: this is very important otherwise if this is part of a modal, the listener still exists because the dom 
                // element might still be there even after the modal has been hidden.
                $scope.$on('$destroy', function () {
                    unsubscribe();
                });
            });
        });

    });

angular.module("umbraco").controller("Umbraco.PrevalueEditors.RteController",
    function ($scope, $timeout, $log, tinyMceService, stylesheetResource) {
        var cfg = tinyMceService.defaultPrevalues();

        if($scope.model.value){
            if(angular.isString($scope.model.value)){
                $scope.model.value = cfg;
            }
        }else{
            $scope.model.value = cfg;
        }

        if (!$scope.model.value.stylesheets) {
            $scope.model.value.stylesheets = [];
        }
        if (!$scope.model.value.toolbar) {
            $scope.model.value.toolbar = [];
        }
        if (!$scope.model.value.maxImageSize && $scope.model.value.maxImageSize != 0) {
            $scope.model.value.maxImageSize = cfg.maxImageSize;
        }

        tinyMceService.configuration().then(function(config){
            $scope.tinyMceConfig = config;

        });

        stylesheetResource.getAll().then(function(stylesheets){
            $scope.stylesheets = stylesheets;
        });

        $scope.selected = function(cmd, alias, lookup){
            if (lookup && angular.isArray(lookup)) {
                cmd.selected = lookup.indexOf(alias) >= 0;
                return cmd.selected;
            }
            return false;
        };

        $scope.selectCommand = function(command){
            var index = $scope.model.value.toolbar.indexOf(command.frontEndCommand);

            if(command.selected && index === -1){
                $scope.model.value.toolbar.push(command.frontEndCommand);
            }else if(index >= 0){
                $scope.model.value.toolbar.splice(index, 1);
            }
        };

        $scope.selectStylesheet = function (css) {
            
            var index = $scope.model.value.stylesheets.indexOf(css.name);

            if(css.selected && index === -1){
                $scope.model.value.stylesheets.push(css.name);
            }else if(index >= 0){
                $scope.model.value.stylesheets.splice(index, 1);
            }
        };

        var unsubscribe = $scope.$on("formSubmitting", function (ev, args) {

            var commands = _.where($scope.tinyMceConfig.commands, {selected: true});
            $scope.model.value.toolbar = _.pluck(commands, "frontEndCommand");
            
        });

        //when the scope is destroyed we need to unsubscribe
        $scope.$on('$destroy', function () {
            unsubscribe();
        });

    });

function sliderController($scope, $log, $element, assetsService, angularHelper) {

    //configure some defaults
    if (!$scope.model.config.orientation) {
        $scope.model.config.orientation = "horizontal";
    }
    if (!$scope.model.config.initVal1) {
        $scope.model.config.initVal1 = 0;
    }
    else {
        $scope.model.config.initVal1 = parseFloat($scope.model.config.initVal1);
    }
    if (!$scope.model.config.initVal2) {
        $scope.model.config.initVal2 = 0;
    }
    else {
        $scope.model.config.initVal2 = parseFloat($scope.model.config.initVal2);
    }
    if (!$scope.model.config.minVal) {
        $scope.model.config.minVal = 0;
    }
    else {
        $scope.model.config.minVal = parseFloat($scope.model.config.minVal);
    }
    if (!$scope.model.config.maxVal) {
        $scope.model.config.maxVal = 100;
    }
    else {
        $scope.model.config.maxVal = parseFloat($scope.model.config.maxVal);
    }
    if (!$scope.model.config.step) {
        $scope.model.config.step = 1;
    }
    else {
        $scope.model.config.step = parseFloat($scope.model.config.step);
    }
    
    /** This creates the slider with the model values - it's called on startup and if the model value changes */
    function createSlider() {

        //the value that we'll give the slider - if it's a range, we store our value as a comma separated val but this slider expects an array
        var sliderVal = null;

        //configure the model value based on if range is enabled or not
        if ($scope.model.config.enableRange === "1") {
            //If no value saved yet - then use default value
            if (!$scope.model.value) {
                var i1 = parseFloat($scope.model.config.initVal1);
                var i2 = parseFloat($scope.model.config.initVal2);
                sliderVal = [
                    isNaN(i1) ? $scope.model.config.minVal : (i1 >= $scope.model.config.minVal ? i1 : $scope.model.config.minVal),
                    isNaN(i2) ? $scope.model.config.maxVal : (i2 > i1 ? (i2 <= $scope.model.config.maxVal ? i2 : $scope.model.config.maxVal) : $scope.model.config.maxVal)
                ];
            }
            else {
                //this will mean it's a delimited value stored in the db, convert it to an array
                sliderVal = _.map($scope.model.value.split(','), function (item) {
                    return parseFloat(item);
                });
            }
        }
        else {
            //If no value saved yet - then use default value
            if ($scope.model.value) {
                sliderVal = parseFloat($scope.model.value);
            }
            else {
                sliderVal = $scope.model.config.initVal1;
            }
        }

        // Initialise model value if not set
        if (!$scope.model.value) {
            setModelValueFromSlider(sliderVal);
        }

        //initiate slider, add event handler and get the instance reference (stored in data)
        var slider = $element.find('.slider-item').slider({
            max: $scope.model.config.maxVal,
            min: $scope.model.config.minVal,
            orientation: $scope.model.config.orientation,
            selection: "after",
            step: $scope.model.config.step,
            tooltip: "show",
            //set the slider val - we cannot do this with data- attributes when using ranges
            value: sliderVal
        }).on('slideStop', function () {
            angularHelper.safeApply($scope, function () {
                setModelValueFromSlider(slider.getValue());
            });
        }).data('slider');
    }

    /** Called on start-up when no model value has been applied and on change of the slider via the UI - updates
        the model with the currently selected slider value(s) **/
    function setModelValueFromSlider(sliderVal) {
        //Get the value from the slider and format it correctly, if it is a range we want a comma delimited value
        if ($scope.model.config.enableRange === "1") {
            $scope.model.value = sliderVal.join(",");
        }
        else {
            $scope.model.value = sliderVal.toString();
        }
    }

    //tell the assetsService to load the bootstrap slider
    //libs from the plugin folder
    assetsService
        .loadJs("lib/slider/js/bootstrap-slider.js")
        .then(function () {

            createSlider();
            
            //here we declare a special method which will be called whenever the value has changed from the server
            //this is instead of doing a watch on the model.value = faster
            $scope.model.onValueChanged = function (newVal, oldVal) {                
                if (newVal != oldVal) {
                    createSlider();
                }
            };

        });

    //load the separate css for the editor to avoid it blocking our js loading
    assetsService.loadCss("lib/slider/slider.css");

}
angular.module("umbraco").controller("Umbraco.PropertyEditors.SliderController", sliderController);
angular.module("umbraco")
.controller("Umbraco.PropertyEditors.TagsController",
    function ($rootScope, $scope, $log, assetsService, umbRequestHelper, angularHelper, $timeout, $element) {

        var $typeahead;

        $scope.isLoading = true;
        $scope.tagToAdd = "";

        assetsService.loadJs("lib/typeahead.js/typeahead.bundle.min.js").then(function () {

            $scope.isLoading = false;

            //load current value

            if ($scope.model.value) {
                if (!$scope.model.config.storageType || $scope.model.config.storageType !== "Json") {
                    //it is csv
                    if (!$scope.model.value) {
                        $scope.model.value = [];
                    }
                    else {
                        $scope.model.value = $scope.model.value.split(",");
                    }
                }
            }
            else {
                $scope.model.value = [];
            }

            // Method required by the valPropertyValidator directive (returns true if the property editor has at least one tag selected)
            $scope.validateMandatory = function () {
                return {
                    isValid: !$scope.model.validation.mandatory || ($scope.model.value != null && $scope.model.value.length > 0),
                    errorMsg: "Value cannot be empty",
                    errorKey: "required"
                };
            }

            //Helper method to add a tag on enter or on typeahead select
            function addTag(tagToAdd) {
                if (tagToAdd != null && tagToAdd.length > 0) {
                    if ($scope.model.value.indexOf(tagToAdd) < 0) {
                        $scope.model.value.push(tagToAdd);
                        //this is required to re-validate
                        $scope.propertyForm.tagCount.$setViewValue($scope.model.value.length);
                    }
                }
            }

            $scope.addTagOnEnter = function (e) {
                var code = e.keyCode || e.which;
                if (code == 13) { //Enter keycode   
                    if ($element.find('.tags-' + $scope.model.alias).parent().find(".tt-dropdown-menu .tt-cursor").length === 0) {
                        //this is required, otherwise the html form will attempt to submit.
                        e.preventDefault();
                        $scope.addTag();
                    }
                }
            };

            $scope.addTag = function () {
                //ensure that we're not pressing the enter key whilst selecting a typeahead value from the drop down
                //we need to use jquery because typeahead duplicates the text box
                addTag($scope.tagToAdd);
                $scope.tagToAdd = "";
                //this clears the value stored in typeahead so it doesn't try to add the text again
                // http://issues.umbraco.org/issue/U4-4947
                $typeahead.typeahead('val', '');
            };



            $scope.removeTag = function (tag) {
                var i = $scope.model.value.indexOf(tag);
                if (i >= 0) {
                    $scope.model.value.splice(i, 1);
                    //this is required to re-validate
                    $scope.propertyForm.tagCount.$setViewValue($scope.model.value.length);
                }
            };

            //vice versa
            $scope.model.onValueChanged = function (newVal, oldVal) {
                //update the display val again if it has changed from the server
                $scope.model.value = newVal;

                if (!$scope.model.config.storageType || $scope.model.config.storageType !== "Json") {
                    //it is csv
                    if (!$scope.model.value) {
                        $scope.model.value = [];
                    }
                    else {
                        $scope.model.value = $scope.model.value.split(",");
                    }
                }
            };

            //configure the tags data source

            //helper method to format the data for bloodhound
            function dataTransform(list) {
                //transform the result to what bloodhound wants
                var tagList = _.map(list, function (i) {
                    return { value: i.text };
                });
                // remove current tags from the list
                return $.grep(tagList, function (tag) {
                    return ($.inArray(tag.value, $scope.model.value) === -1);
                });
            }

            // helper method to remove current tags
            function removeCurrentTagsFromSuggestions(suggestions) {
                return $.grep(suggestions, function (suggestion) {
                    return ($.inArray(suggestion.value, $scope.model.value) === -1);
                });
            }

            var tagsHound = new Bloodhound({
                datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
                queryTokenizer: Bloodhound.tokenizers.whitespace,
                dupDetector : function(remoteMatch, localMatch) {
                    return (remoteMatch["value"] == localMatch["value"]);
                },
                //pre-fetch the tags for this category
                prefetch: {
                    url: umbRequestHelper.getApiUrl("tagsDataBaseUrl", "GetTags", [{ tagGroup: $scope.model.config.group }]),
                    //TTL = 5 minutes
                    ttl: 300000,
                    filter: dataTransform
                },
                //dynamically get the tags for this category (they may have changed on the server)
                remote: {
                    url: umbRequestHelper.getApiUrl("tagsDataBaseUrl", "GetTags", [{ tagGroup: $scope.model.config.group }]),
                    filter: dataTransform
                }
            });

            tagsHound.initialize(true);

            //configure the type ahead
            $timeout(function () {

                $typeahead = $element.find('.tags-' + $scope.model.alias).typeahead(
                {
                    //This causes some strangeness as it duplicates the textbox, best leave off for now.
                    hint: false,
                    highlight: true,
                    cacheKey: new Date(),  // Force a cache refresh each time the control is initialized
                    minLength: 1
                }, {
                    //see: https://github.com/twitter/typeahead.js/blob/master/doc/jquery_typeahead.md#options
                    // name = the data set name, we'll make this the tag group name
                    name: $scope.model.config.group,
                    displayKey: "value",
                    source: function (query, cb) {
                        tagsHound.get(query, function (suggestions) {
                            cb(removeCurrentTagsFromSuggestions(suggestions));
                        });
                    },
                }).bind("typeahead:selected", function (obj, datum, name) {
                    angularHelper.safeApply($scope, function () {
                        addTag(datum["value"]);
                        $scope.tagToAdd = "";
                        // clear the typed text
                        $typeahead.typeahead('val', '');
                    });

                }).bind("typeahead:autocompleted", function (obj, datum, name) {
                    angularHelper.safeApply($scope, function () {
                        addTag(datum["value"]);
                        $scope.tagToAdd = "";
                    });

                }).bind("typeahead:opened", function (obj) {
                    //console.log("opened ");
                });
            });

            $scope.$on('$destroy', function () {
                tagsHound.clearPrefetchCache();
                tagsHound.clearRemoteCache();
                $element.find('.tags-' + $scope.model.alias).typeahead('destroy');
                delete tagsHound;
            });

        });

    }
);
//this controller simply tells the dialogs service to open a mediaPicker window
//with a specified callback, this callback will receive an object with a selection on it
angular.module('umbraco').controller("Umbraco.PropertyEditors.EmbeddedContentController",
	function($rootScope, $scope, $log){
    
	$scope.showForm = false;
	$scope.fakeData = [];

	$scope.create = function(){
		$scope.showForm = true;
		$scope.fakeData = angular.copy($scope.model.config.fields);
	};

	$scope.show = function(){
		$scope.showCode = true;
	};

	$scope.add = function(){
		$scope.showForm = false;
		if ( !($scope.model.value instanceof Array)) {
			$scope.model.value = [];
		}

		$scope.model.value.push(angular.copy($scope.fakeData));
		$scope.fakeData = [];
	};
});
angular.module('umbraco').controller("Umbraco.PropertyEditors.UrlListController",
	function($rootScope, $scope, $filter) {

	    function formatDisplayValue() {            
	        if (angular.isArray($scope.model.value)) {
	            //it's the json value
	            $scope.renderModel = _.map($scope.model.value, function (item) {
	                return {
	                    url: item.url,
	                    linkText: item.linkText,
	                    urlTarget: (item.target) ? item.target : "_blank",
	                    icon: (item.icon) ? item.icon : "icon-out"
	                };
	            });
	        }
	        else {
                //it's the default csv value
	            $scope.renderModel = _.map($scope.model.value.split(","), function (item) {
	                return {
	                    url: item,
	                    linkText: "",
	                    urlTarget: ($scope.config && $scope.config.target) ? $scope.config.target : "_blank",
	                    icon: ($scope.config && $scope.config.icon) ? $scope.config.icon : "icon-out"
	                };
	            });
	        }
        }

	    $scope.getUrl = function(valueUrl) {
	        if (valueUrl.indexOf("/") >= 0) {
	            return valueUrl;
	        }
	        return "#";
	    };

	    formatDisplayValue();
	    
	    //here we declare a special method which will be called whenever the value has changed from the server
	    //this is instead of doing a watch on the model.value = faster
	    $scope.model.onValueChanged = function(newVal, oldVal) {
	        //update the display val again
	        formatDisplayValue();
	    };

	});

})();