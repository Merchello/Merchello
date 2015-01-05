/*! umbraco - v7.1.8 - 2014-10-08
 * https://github.com/umbraco/umbraco-cms/
 * Copyright (c) 2014 Umbraco HQ;
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
function MainController($scope, $rootScope, $location, $routeParams, $timeout, $http, $log, appState, treeService, notificationsService, userService, navigationService, historyService, updateChecker, assetsService, eventsService, umbRequestHelper) {

    //the null is important because we do an explicit bool check on this in the view
    //the avatar is by default the umbraco logo    
    $scope.authenticated = null;
    $scope.avatar = "assets/img/application/logo.png";
    $scope.touchDevice = appState.getGlobalState("touchDevice");
    //subscribes to notifications in the notification service
    $scope.notifications = notificationsService.current;
    $scope.$watch('notificationsService.current', function (newVal, oldVal, scope) {
        if (newVal) {
            $scope.notifications = newVal;
        }
    });

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

    //when a user logs out or timesout
    eventsService.on("app.notAuthenticated", function() {
        $scope.authenticated = null;
        $scope.user = null;
    });
    
    //when the app is read/user is logged in, setup the data
    eventsService.on("app.ready", function (evt, data) {
        
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
        });

        //if the user has changed we need to redirect to the root so they don't try to continue editing the
        //last item in the URL (NOTE: the user id can equal zero, so we cannot just do !data.lastUserId since that will resolve to true)
        if (data.lastUserId !== undefined && data.lastUserId !== null && data.lastUserId !== data.user.id) {
            $location.path("/").search("");
            historyService.removeAll();
            treeService.clearCache();
        }

        if($scope.user.emailHash){
            $timeout(function () {                
                //yes this is wrong.. 
                $("#avatar-img").fadeTo(1000, 0, function () {
                    $timeout(function () {
                        //this can be null if they time out
                        if ($scope.user && $scope.user.emailHash) {
                            $scope.avatar = "http://www.gravatar.com/avatar/" + $scope.user.emailHash + ".jpg?s=64&d=mm";
                        }
                    });
                    $("#avatar-img").fadeTo(1000, 1);
                });
                
              }, 3000);  
        }

    });

}


//register it
angular.module('umbraco').controller("Umbraco.MainController", MainController);

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

    //Listen for global state changes
    eventsService.on("appState.globalState.changed", function (e, args) {
        if (args.key === "showNavigation") {
            $scope.showNavigation = args.value;
        }
    });

    //Listen for menu state changes
    eventsService.on("appState.menuState.changed", function (e, args) {
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
    });

    //Listen for section state changes
    eventsService.on("appState.sectionState.changed", function (e, args) {
        //section changed
        if (args.key === "currentSection") {
            $scope.currentSection = args.value;
        }
        //show/hide search results
        if (args.key === "showSearchResults") {
            $scope.showSearchResults = args.value;
        }
    });

    //This reacts to clicks passed to the body element which emits a global call to close all dialogs
    eventsService.on("app.closeDialogs", function (event) {
        if (appState.getGlobalState("stickyNavigation")) {
            navigationService.hideNavigation();
            //TODO: don't know why we need this? - we are inside of an angular event listener.
            angularHelper.safeApply($scope);
        }
    });

    //when a user logs out or timesout
    eventsService.on("app.notAuthenticated", function () {
        $scope.authenticated = false;
    });

    //when the application is ready and the user is authorized setup the data
    eventsService.on("app.ready", function (evt, data) {
        $scope.authenticated = true;
    });

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
function SearchController($scope, searchService, $log, $location, navigationService) {

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

    //watch the value change but don't do the search on every change - that's far too many queries
    // we need to debounce
    var debounced = _.debounce(function () {
        if ($scope.searchTerm) {
            $scope.isSearching = true;
            navigationService.showSearch();
            $scope.selectedItem = undefined;
            searchService.searchAll({ term: $scope.searchTerm }).then(function (result) {
                $scope.groups = _.filter(result, function(group){return group.results.length > 0;});
            });
        }else{
            $scope.isSearching = false;
            navigationService.hideSearch();
            $scope.selectedItem = undefined;
        }
    }, 300);

    
    $scope.$watch("searchTerm", debounced);

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
function ContentEditDialogController($scope) {
    //setup scope vars
    $scope.model = {};
    $scope.model.defaultButton = null;
    $scope.model.subButtons = [];
    
    var dialogOptions = $scope.dialogOptions;
    if(dialogOptions.entity){
    	$scope.model.entity = dialogOptions.entity;
    	$scope.loaded = true;	
    }
}

angular.module("umbraco")
	.controller("Umbraco.Dialogs.Content.EditController", ContentEditDialogController);

//TODO: SD: Pretty sure we don't want this anymore and we should just be using the treepicker.html instead for all tree picking.

//used for the content picker dialog
angular.module("umbraco").controller("Umbraco.Dialogs.ContentPickerController",
	function ($scope, eventsService, entityResource, searchService, $log) {	
	var dialogOptions = $scope.dialogOptions;
	$scope.dialogTreeEventHandler = $({});
	$scope.results = [];

	$scope.selectResult = function(result){
		entityResource.getById(result.id, "Document").then(function(ent){
			if(dialogOptions && dialogOptions.multiPicker){
				
				$scope.showSearch = false;
				$scope.results = [];
				$scope.term = "";
				$scope.oldTerm = undefined;

				$scope.select(ent);
			}else{
				$scope.submit(ent);
			}
		});
	};

	$scope.performSearch = function(){
		if($scope.term){
			if($scope.oldTerm !== $scope.term){
				$scope.results = [];
			    searchService.searchContent({ term: $scope.term }).then(function(data) {
			        $scope.results = data;
			    });
				$scope.showSearch = true;
				$scope.oldTerm = $scope.term;
			}
		}else{
			$scope.oldTerm = "";
			$scope.showSearch = false;
			$scope.results = [];
		}
	};


	$scope.dialogTreeEventHandler.bind("treeNodeSelect", function(ev, args){
		args.event.preventDefault();
		args.event.stopPropagation();

		eventsService.emit("dialogs.contentPicker.select", args);
	    
		if (dialogOptions && dialogOptions.multiPicker) {

		    var c = $(args.event.target.parentElement);
		    if (!args.node.selected) {
		        args.node.selected = true;

		        var temp = "<i class='icon umb-tree-icon sprTree icon-check blue temporary'></i>";
		        var icon = c.find("i.umb-tree-icon");
		        if (icon.length > 0) {
		            icon.hide().after(temp);
		        } else {
		            c.prepend(temp);
		        }

		    } else {
		        args.node.selected = false;
		        c.find(".temporary").remove();
		        c.find("i.umb-tree-icon").show();
		    }

		    $scope.select(args.node);

		} else {
		    $scope.submit(args.node);
		}

	});
});
angular.module("umbraco")
    .controller("Umbraco.Dialogs.HelpController", function ($scope, $location, $routeParams, helpService, userService) {
        $scope.section = $routeParams.section;
        $scope.version = Umbraco.Sys.ServerVariables.application.version + " assembly: " + Umbraco.Sys.ServerVariables.application.assemblyVersion;
        
        if(!$scope.section){
            $scope.section ="content";
        }

        var rq = {};
        rq.section = $scope.section;
        
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

            if (!_.isString(item.value)) {
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

            $scope.macros = data;

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
	function ($scope, eventsService, dialogService, entityResource, contentResource, mediaHelper, $log) {
	var dialogOptions = $scope.dialogOptions;
	
	$scope.dialogTreeEventHandler = $({});
	$scope.target = {};

	if(dialogOptions.currentTarget){
		$scope.target = dialogOptions.currentTarget;

		//if we have a node ID, we fetch the current node to build the form data
		if($scope.target.id){

			if(!$scope.target.path) {
			    entityResource.getPath($scope.target.id, "Document").then(function (path) {
			        $scope.target.path = path;
			        //now sync the tree to this path
			        $scope.dialogTreeEventHandler.syncTree({ path: $scope.target.path, tree: "content" });
			    });
			}

			contentResource.getNiceUrl($scope.target.id).then(function(url){
				$scope.target.url = url;
			});
		}
	}

	$scope.switchToMediaPicker = function () {
	    dialogService.mediaPicker(
            {
                callback: function (media) {
                    $scope.target.id = media.id;
                    $scope.target.isMedia = true;
                    $scope.target.name = media.name;
                    $scope.target.url = mediaHelper.resolveFile(media);
                }
            });
	};


	$scope.dialogTreeEventHandler.bind("treeNodeSelect", function(ev, args){
		args.event.preventDefault();
		args.event.stopPropagation();

		eventsService.emit("dialogs.linkPicker.select", args);
	    
		var c = $(args.event.target.parentElement);

	    //renewing
		if (args.node !== $scope.target) {
		    if ($scope.selectedEl) {
		        $scope.selectedEl.find(".temporary").remove();
		        $scope.selectedEl.find("i.umb-tree-icon").show();
		    }

		    $scope.selectedEl = c;
		    $scope.target.id = args.node.id;
		    $scope.target.name = args.node.name;

		    $scope.selectedEl.find("i.umb-tree-icon")
             .hide()
             .after("<i class='icon umb-tree-icon sprTree icon-check blue temporary'></i>");

		    if (args.node.id < 0) {
		        $scope.target.url = "/";
		    } else {
		        contentResource.getNiceUrl(args.node.id).then(function (url) {
		            $scope.target.url = url;
		        });
		    }
		} else {
		    $scope.target = undefined;
		    //resetting
		    if ($scope.selectedEl) {
		        $scope.selectedEl.find(".temporary").remove();
		        $scope.selectedEl.find("i.umb-tree-icon").show();
		    }
		}

		if (!angular.isUndefined($scope.target.isMedia)) {
		    delete $scope.target.isMedia;
		}
	});
});
angular.module("umbraco").controller("Umbraco.Dialogs.LoginController", function ($scope, localizationService, userService) {
    
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
    localizationService.localize("login_greeting"+d.getDay()).then(function(label){
        $scope.greeting = label;
    }); // weekday[d.getDay()];
    
    $scope.errorMsg = "";
    
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
        function ($scope, mediaResource, umbRequestHelper, entityResource, $log, mediaHelper, eventsService, treeService, $cookies) {

            var dialogOptions = $scope.dialogOptions;

            $scope.onlyImages = dialogOptions.onlyImages;
            $scope.showDetails = dialogOptions.showDetails;
            $scope.multiPicker = (dialogOptions.multiPicker && dialogOptions.multiPicker !== "0") ? true : false;
            $scope.startNodeId = dialogOptions.startNodeId ? dialogOptions.startNodeId : -1;


            $scope.options = {
                url: umbRequestHelper.getApiUrl("mediaApiBaseUrl", "PostAddFile"),
                autoUpload: true,
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
                    $scope.showFolderInput = false;

                    mediaResource
                        .addFolder($scope.newFolderName, $scope.options.formData.currentFolder)
                        .then(function(data) {

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

            $scope.$on('fileuploadstop', function(event, files) {
                $scope.gotoFolder($scope.currentFolder);
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
        $scope.results = [];
        $scope.dialogData = [];
        
        /** Method used for selecting a node */
        function select(text, id) {

           
            $scope.showSearch = false;
            $scope.results = [];
            $scope.term = "";
            $scope.oldTerm = undefined;

            if (dialogOptions.multiPicker) {
                if ($scope.dialogData.indexOf(id) == -1) {
                    $scope.dialogData.push(id);
                }
            }
            else {
                $scope.submit(id);
               
            }
        }
        
        function remove(text, id) {
            var index = $scope.dialogData.indexOf(id);
         
            if (index > -1) {
                $scope.dialogData.splice(index, 1);
            }
        }


        $scope.dialogTreeEventHandler.bind("treeNodeSelect", function(ev, args) {
            args.event.preventDefault();
            args.event.stopPropagation();


            eventsService.emit("dialogs.memberGroupPicker.select", args);
            
            //This is a tree node, so we don't have an entity to pass in, it will need to be looked up
            //from the server in this method.
            select(args.node.name, args.node.id);

            if (dialogOptions.multiPicker) {
                var c = $(args.event.target.parentElement);
                if (!args.node.selected) {
                    args.node.selected = true;
                    c.find("i.umb-tree-icon").hide()
                        .after("<i class='icon umb-tree-icon sprTree icon-check blue temporary'></i>");
                }
                else {

                    remove(args.node.name, args.node.id);

                    args.node.selected = false;
                    c.find(".temporary").remove();
                    c.find("i.umb-tree-icon").show();
                }
            }

        });
    });
//used for the member picker dialog
angular.module("umbraco").controller("Umbraco.Dialogs.MemberPickerController",
    function($scope, eventsService, entityResource, searchService, $log) {
        var dialogOptions = $scope.dialogOptions;
        $scope.dialogTreeEventHandler = $({});
        $scope.results = [];

        /** Method used for selecting a node */
        function select(text, key, entity) {

            $scope.showSearch = false;
            $scope.results = [];
            $scope.term = "";
            $scope.oldTerm = undefined;

            if (dialogOptions.multiPicker) {
                $scope.select(key);
            }
            else {
                //if an entity has been passed in, use it
                if (entity) {
                    $scope.submit(entity);
                }
                else {
                    //otherwise we have to get it from the server
                    entityResource.getById(key, "Member").then(function (ent) {
                        $scope.submit(ent);
                    });
                }
            }
        }

        $scope.performSearch = function() {
            if ($scope.term) {
                if ($scope.oldTerm !== $scope.term) {
                    $scope.results = [];
                    searchService.searchMembers({ term: $scope.term }).then(function(data) {
                        $scope.results = data;
                    });
                    $scope.showSearch = true;
                    $scope.oldTerm = $scope.term;
                }
            }
            else {
                $scope.oldTerm = "";
                $scope.showSearch = false;
                $scope.results = [];
            }
        };

        /** method to select a search result */
        $scope.selectResult = function(result) {
            //since result = an entity, we'll pass it in so we don't have to go back to the server
            select(result.name, result.id, result);
        };

        $scope.dialogTreeEventHandler.bind("treeNodeSelect", function(ev, args) {
            args.event.preventDefault();
            args.event.stopPropagation();

            if (args.node.nodeType === "member-folder") {
                return;
            }

            eventsService.emit("dialogs.memberPickerController", args);
            
            //This is a tree node, so we don't have an entity to pass in, it will need to be looked up
            //from the server in this method.
            select(args.node.name, args.node.id);

            if (dialogOptions && dialogOptions.multipicker) {

                var c = $(args.event.target.parentElement);
                if (!args.node.selected) {
                    args.node.selected = true;
                    c.find("i.umb-tree-icon").hide()
                        .after("<i class='icon umb-tree-icon sprTree icon-check blue temporary'></i>");
                }
                else {
                    args.node.selected = false;
                    c.find(".temporary").remove();
                    c.find("i.umb-tree-icon").show();
                }
            }

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
    
    $scope.showPreview = function(){

        if ($scope.form.url != "") {
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
                            $scope.form.info = "Not Supported";
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
                .error(function() {
                    $scope.form.preview = "";
                    $scope.form.info = "Computer says no";
                });

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
//used for the media picker dialog
angular.module("umbraco").controller("Umbraco.Dialogs.TreePickerController",
	function ($scope, entityResource, eventsService, $log, searchService) {
		
		var dialogOptions = $scope.dialogOptions;
		$scope.dialogTreeEventHandler = $({});
		$scope.section = dialogOptions.section;
		$scope.treeAlias = dialogOptions.treeAlias;
		$scope.multiPicker = dialogOptions.multiPicker;
		$scope.hideHeader = dialogOptions.startNodeId ? true : false;
		$scope.startNodeId = dialogOptions.startNodeId ? dialogOptions.startNodeId : -1;

	    //create the custom query string param for this tree
		$scope.customTreeParams = dialogOptions.startNodeId ? "startNodeId=" + dialogOptions.startNodeId : "";
		$scope.customTreeParams += dialogOptions.customTreeParams ? "&" + dialogOptions.customTreeParams : "";

		//search defaults
		$scope.searcher = searchService.searchContent;
		$scope.entityType = "Document";
		$scope.results = [];

		//min / max values
		if(dialogOptions.minNumber){
			dialogOptions.minNumber = parseInt(dialogOptions.minNumber, 10);
		}
		if(dialogOptions.maxNumber){
			dialogOptions.maxNumber = parseInt(dialogOptions.maxNumber, 10);
		}

		//search
		if (dialogOptions.section === "member") {
			$scope.searcher = searchService.searchMembers;
			$scope.entityType = "Member";
		}
		else if (dialogOptions.section === "media") {
			$scope.searcher = searchService.searchMedia;
			$scope.entityType = "Media";
		}

		//Configures filtering
		if (dialogOptions.filter) {

			dialogOptions.filterExclude = false;
			dialogOptions.filterAdvanced = false;

			if(dialogOptions.filter[0] === "!"){
				dialogOptions.filterExclude = true;
				dialogOptions.filter = dialogOptions.filter.substring(1);		
			}

			//used advanced filtering
			if(dialogOptions.filter[0] === "{"){
				dialogOptions.filterAdvanced = true;
			}

			$scope.dialogTreeEventHandler.bind("treeNodeExpanded", function (ev, args) {
				if (angular.isArray(args.children)) {
					performFiltering(args.children);
				}
			});
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
			}else {
				$scope.showSearch = false;
				$scope.results = [];
				$scope.term = "";
				$scope.oldTerm = undefined;

				if ($scope.multiPicker) {
					$scope.select(id);
				}else {
					//if an entity has been passed in, use it
					if (entity) {
						$scope.submit(entity);
					}else {
						//otherwise we have to get it from the server
						entityResource.getById(id, $scope.entityType).then(function (ent) {
							$scope.submit(ent);
						});
					}
				}
			}
		}

		function performFiltering(nodes){
			if(dialogOptions.filterAdvanced){
				angular.forEach(_.where(nodes, angular.fromJson(dialogOptions.filter)), function(value, key){
					value.filtered = true;
					if(dialogOptions.filterCssClass){
						value.cssClasses.push(dialogOptions.filterCssClass);
					}
				});
			}else{
				var a = dialogOptions.filter.split(',');
				angular.forEach(nodes, function (value, key) {

					var found = a.indexOf(value.metaData.contentType) >= 0;

					if (!dialogOptions.filterExclude && !found || dialogOptions.filterExclude && found){
						value.filtered = true;
						
						if(dialogOptions.filterCssClass){
							value.cssClasses.push(dialogOptions.filterCssClass);
						}
					}
				});
			}
		}



	$scope.multiSubmit = function (result) {
		entityResource.getByIds(result, $scope.entityType).then(function (ents) {
			$scope.submit(ents);
		});
	};


	/** method to select a search result */ 
	$scope.selectResult = function (result) {
		//since result = an entity, we'll pass it in so we don't have to go back to the server
		select(result.name, result.id, result);
	};


	$scope.performSearch = function () {
		if ($scope.term) {
			if ($scope.oldTerm !== $scope.term) {
				$scope.results = [];

				$scope.searcher({ term: $scope.term }).then(function (data) {
					$scope.results = data;
				});

				$scope.showSearch = true;
				$scope.oldTerm = $scope.term;
			}
		}
		else {
			$scope.oldTerm = "";
			$scope.showSearch = false;
			$scope.results = [];
		}
	};


	

	//wires up selection
	$scope.dialogTreeEventHandler.bind("treeNodeSelect", function (ev, args) {
		args.event.preventDefault();
		args.event.stopPropagation();

		eventsService.emit("dialogs.treePickerController.select", args);
	    
		if (args.node.filtered) {
		    return;
		}

	    //This is a tree node, so we don't have an entity to pass in, it will need to be looked up
	    //from the server in this method.
		select(args.node.name, args.node.id);

	    //ui...
		if ($scope.multiPicker) {
		    var c = $(args.event.target.parentElement);
		    if (!args.node.selected) {
		        args.node.selected = true;
		        var temp = "<i class='icon umb-tree-icon sprTree icon-check blue temporary'></i>";
		        var icon = c.find("i.umb-tree-icon");
		        if (icon.length > 0) {
		            icon.hide().after(temp);
		        }
		        else {
		            c.prepend(temp);
		        }
		    }
		    else {
		        args.node.selected = false;
		        c.find(".temporary").remove();
		        c.find("i.umb-tree-icon").show();
		    }
		}
	});
});
angular.module("umbraco")
    .controller("Umbraco.Dialogs.UserController", function ($scope, $location, $timeout, userService, historyService, eventsService) {

        $scope.user = userService.getCurrentUser();
        $scope.history = historyService.getCurrent();
        $scope.version = Umbraco.Sys.ServerVariables.application.version + " assembly: " + Umbraco.Sys.ServerVariables.application.assemblyVersion;

        var evtHandlers = [];
        evtHandlers.push(eventsService.on("historyService.add", function (e, args) {
            $scope.history = args.all;
        }));
        evtHandlers.push(eventsService.on("historyService.remove", function (e, args) {
            $scope.history = args.all;
        }));
        evtHandlers.push(eventsService.on("historyService.removeAll", function (e, args) {
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

        //get the user
        userService.getCurrentUser().then(function (user) {
            $scope.user = user;
            if ($scope.user) {
                $scope.remainingAuthSeconds = $scope.user.remainingAuthSeconds;
                $scope.canEditProfile = _.indexOf($scope.user.allowedSections, "users") > -1;
                //set the timer
                updateTimeout();
            }
        });

        //remove all event handlers
        $scope.$on('$destroy', function () {
            for (var i = 0; i < evtHandlers.length; i++) {
                evtHandlers[i]();
            }

        });

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
	$scope.legacyPath = decodeURIComponent($routeParams.url);
}

angular.module("umbraco").controller('Umbraco.LegacyController', LegacyController);
/** This controller is simply here to launch the login dialog when the route is explicitly changed to /login */
angular.module('umbraco').controller("Umbraco.LoginController", function (eventsService, $scope, userService, $location) {

    userService._showLoginDialog(); 
       
    eventsService.on("app.ready", function(){
    	$scope.avatar = "assets/img/application/logo.png";
    	$location.path("/").search("");
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
angular.module("umbraco")
	.controller("Umbraco.Editors.Content.CopyController",
	function ($scope, eventsService, contentResource, navigationService, appState, treeService) {

	    var dialogOptions = $scope.dialogOptions;

	    $scope.relateToOriginal = false;
	    $scope.dialogTreeEventHandler = $({});

	    var node = dialogOptions.currentNode;

	    $scope.dialogTreeEventHandler.bind("treeNodeSelect", function (ev, args) {
	        args.event.preventDefault();
	        args.event.stopPropagation();

	        eventsService.emit("editors.content.copyController.select", args);

	        var c = $(args.event.target.parentElement);
	        if ($scope.selectedEl) {
	            $scope.selectedEl.find(".temporary").remove();
	            $scope.selectedEl.find("i.umb-tree-icon").show();
	        }

	        var temp = "<i class='icon umb-tree-icon sprTree icon-check blue temporary'></i>";
	        var icon = c.find("i.umb-tree-icon");
	        if (icon.length > 0) {
	            icon.hide().after(temp);
	        } else {
	            c.prepend(temp);
	        }

	        $scope.target = args.node;
	        $scope.selectedEl = c;

	    });

	    $scope.copy = function () {
	        contentResource.copy({ parentId: $scope.target.id, id: node.id, relateToOriginal: $scope.relateToOriginal })
                .then(function (path) {
                    $scope.error = false;
                    $scope.success = true;

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
                });
	    };
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
function ContentDeleteController($scope, contentResource, treeService, navigationService) {

    $scope.performDelete = function() {

        //mark it for deletion (used in the UI)
        $scope.currentNode.loading = true;

        contentResource.deleteById($scope.currentNode.id).then(function () {
            $scope.currentNode.loading = false;

            //get the root node before we remove it
            var rootNode = treeService.getTreeRoot($scope.currentNode);

            //TODO: Need to sync tree, etc...
            treeService.removeNode($scope.currentNode);

            //ensure the recycle bin has child nodes now            
            var recycleBin = treeService.getDescendantNode(rootNode, -20);
            recycleBin.hasChildren = true;

            navigationService.hideMenu();
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
function ContentEditController($scope, $routeParams, $q, $timeout, $window, appState, contentResource, entityResource, navigationService, notificationsService, angularHelper, serverValidationManager, contentEditingHelper, treeService, fileManager, formHelper, umbRequestHelper, keyboardService, umbModelMapper, editorState, $http) {

    //setup scope vars
    $scope.defaultButton = null;
    $scope.subButtons = [];
    $scope.nav = navigationService;
    $scope.currentSection = appState.getSectionState("currentSection");
    $scope.currentNode = null; //the editors affiliated node


    //This sets up the action buttons based on what permissions the user has.
    //The allowedActions parameter contains a list of chars, each represents a button by permission so 
    //here we'll build the buttons according to the chars of the user.
    function configureButtons(content) {
        //reset
        $scope.subButtons = [];

        //This is the ideal button order but depends on circumstance, we'll use this array to create the button list
        // Publish, SendToPublish, Save
        var buttonOrder = ["U", "H", "A"];

        //Create the first button (primary button)
        //We cannot have the Save or SaveAndPublish buttons if they don't have create permissions when we are creating a new item.
        if (!$routeParams.create || _.contains(content.allowedActions, "C")) {
            for (var b in buttonOrder) {
                if (_.contains(content.allowedActions, buttonOrder[b])) {
                    $scope.defaultButton = createButtonDefinition(buttonOrder[b]);
                    break;
                }
            }
        }

        //Now we need to make the drop down button list, this is also slightly tricky because:
        //We cannot have any buttons if there's no default button above.
        //We cannot have the unpublish button (Z) when there's no publish permission.    
        //We cannot have the unpublish button (Z) when the item is not published.           
        if ($scope.defaultButton) {

            //get the last index of the button order
            var lastIndex = _.indexOf(buttonOrder, $scope.defaultButton.letter);
            //add the remaining
            for (var i = lastIndex + 1; i < buttonOrder.length; i++) {
                if (_.contains(content.allowedActions, buttonOrder[i])) {
                    $scope.subButtons.push(createButtonDefinition(buttonOrder[i]));
                }
            }


            //if we are not creating, then we should add unpublish too, 
            // so long as it's already published and if the user has access to publish
            if (!$routeParams.create) {
                if (content.publishDate && _.contains(content.allowedActions, "U")) {
                    $scope.subButtons.push(createButtonDefinition("Z"));
                }
            }
        }

        //We fetch all ancestors of the node to generate the footer breadcrump navigation
        if (!$routeParams.create) {
            entityResource.getAncestors(content.id, "document")
                .then(function (anc) {
                    anc.pop();
                    $scope.ancestors = anc;
                });
        }
    }

    function createButtonDefinition(ch) {
        switch (ch) {
            case "U":
                //publish action
                keyboardService.bind("ctrl+p", $scope.saveAndPublish);

                return {
                    letter: ch,
                    labelKey: "buttons_saveAndPublish",
                    handler: $scope.saveAndPublish,
                    hotKey: "ctrl+p"
                };
            case "H":
                //send to publish
                keyboardService.bind("ctrl+p", $scope.sendToPublish);

                return {
                    letter: ch,
                    labelKey: "buttons_saveToPublish",
                    handler: $scope.sendToPublish,
                    hotKey: "ctrl+p"
                };
            case "A":
                //save
                keyboardService.bind("ctrl+s", $scope.save);
                return {
                    letter: ch,
                    labelKey: "buttons_save",
                    handler: $scope.save,
                    hotKey: "ctrl+s"
                };
            case "Z":
                //unpublish
                keyboardService.bind("ctrl+u", $scope.unPublish);

                return {
                    letter: ch,
                    labelKey: "content_unPublish",
                    handler: $scope.unPublish
                };
            default:
                return null;
        }
    }

    /** Syncs the content item to it's tree node - this occurs on first load and after saving */
    function syncTreeNode(content, path, initialLoad) {

        //If this is a child of a list view then we can't actually sync the real tree
        if (!$scope.content.isChildOfListView) {
            navigationService.syncTree({ tree: "content", path: path.split(","), forceReload: initialLoad !== true }).then(function (syncArgs) {
                $scope.currentNode = syncArgs.node;
            });
        }
        else if (initialLoad === true) {
            //if this is a child of a list view and it's the initial load of the editor, we need to get the tree node 
            // from the server so that we can load in the actions menu.
            umbRequestHelper.resourcePromise(
                $http.get(content.treeNodeUrl),
                'Failed to retrieve data for child node ' + content.id).then(function (node) {
                    $scope.currentNode = node;
                });
        }
    }

    /** This is a helper method to reduce the amount of code repitition for actions: Save, Publish, SendToPublish */
    function performSave(args) {
        var deferred = $q.defer();

        $scope.busy = true;

        if (formHelper.submitForm({ scope: $scope, statusMessage: args.statusMessage })) {

            args.saveMethod($scope.content, $routeParams.create, fileManager.getFiles())
                .then(function (data) {

                    formHelper.resetForm({ scope: $scope, notifications: data.notifications });

                    contentEditingHelper.handleSuccessfulSave({
                        scope: $scope,
                        savedContent: data,
                        rebindCallback: contentEditingHelper.reBindChangedProperties($scope.content, data)
                    });

                    editorState.set($scope.content);
                    $scope.busy = false;

                    configureButtons(data);

                    syncTreeNode($scope.content, data.path);

                    deferred.resolve(data);

                }, function (err) {

                    contentEditingHelper.handleSaveError({
                        redirectOnFailure: true,
                        err: err,
                        rebindCallback: contentEditingHelper.reBindChangedProperties($scope.content, err.data)
                    });

                    editorState.set($scope.content);
                    $scope.busy = false;

                    deferred.reject(err);
                });
        }
        else {
            $scope.busy = false;
            deferred.reject();
        }

        return deferred.promise;
    }

    if ($routeParams.create) {
        //we are creating so get an empty content item
        contentResource.getScaffold($routeParams.id, $routeParams.doctype)
            .then(function (data) {
                $scope.loaded = true;
                $scope.content = data;

                editorState.set($scope.content);

                configureButtons($scope.content);
            });
    }
    else {
        //we are editing so get the content item from the server
        contentResource.getById($routeParams.id)
            .then(function (data) {
                $scope.loaded = true;
                $scope.content = data;

                editorState.set($scope.content);

                configureButtons($scope.content);

                //in one particular special case, after we've created a new item we redirect back to the edit
                // route but there might be server validation errors in the collection which we need to display
                // after the redirect, so we will bind all subscriptions which will show the server validation errors
                // if there are any and then clear them so the collection no longer persists them.
                serverValidationManager.executeAndClearAllSubscriptions();

                syncTreeNode($scope.content, data.path, true);

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

                    editorState.set($scope.content);

                    configureButtons(data);

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
            $scope.save().then(function (data) {

                // Chromes popup blocker will kick in if a window is opened 
                // outwith the initial scoped request. This trick will fix that.
                var previewWindow = $window.open("/umbraco/views/content/umbpreview.html", "umbpreview");


                // Build the correct path so both /#/ and #/ work.
                var redirect = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath + '/dialogs/preview.aspx?id=' + data.id;
                previewWindow.location.href = redirect;
            });
        }        
    };

    /** this method is called for all action buttons and then we proxy based on the btn definition */
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

//used for the media picker dialog
angular.module("umbraco").controller("Umbraco.Editors.Content.MoveController",
	function ($scope, eventsService, contentResource, navigationService, appState, treeService) {
	var dialogOptions = $scope.dialogOptions;
	
	$scope.dialogTreeEventHandler = $({});
	var node = dialogOptions.currentNode;

	$scope.dialogTreeEventHandler.bind("treeNodeSelect", function(ev, args){
		args.event.preventDefault();
		args.event.stopPropagation();

		eventsService.emit("editors.content.moveController.select", args);
	    
		var c = $(args.event.target.parentElement);

		if ($scope.selectedEl) {
		    $scope.selectedEl.find(".temporary").remove();
		    $scope.selectedEl.find("i.umb-tree-icon").show();
		}

		var temp = "<i class='icon umb-tree-icon sprTree icon-check blue temporary'></i>";
		var icon = c.find("i.umb-tree-icon");
		if (icon.length > 0) {
		    icon.hide().after(temp);
		} else {
		    c.prepend(temp);
		}


		$scope.target = args.node;
		$scope.selectedEl = c;
	});

	$scope.move = function(){
		contentResource.move({parentId: $scope.target.id, id: node.id})
			.then(function(path){
				$scope.error = false;
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

			},function(err){
				$scope.success = false;
				$scope.error = err;
			});
	};
});
/**
 * @ngdoc controller
 * @name Umbraco.Dashboard.RecycleBinController
 * @function
 * 
 * @description
 * Controls the recycle bin dashboards
 * 
 */

function RecycleBinController($scope, $routeParams) {
	if ($routeParams.section) {

		if ($routeParams.section === "content") {
			$routeParams.id = "-20";
		}
		else if ($routeParams.section === "media") {
			$routeParams.id = "-21";
		}

		$scope.model = { config: { entityType: $routeParams.section } };
	}
}

angular.module('umbraco').controller("Umbraco.Dashboard.RecycleBinController", RecycleBinController);

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

function startupLatestEditsController($scope) {
    
}
angular.module("umbraco").controller("Umbraco.Dashboard.StartupLatestEditsController", startupLatestEditsController);

function MediaFolderBrowserDashboardController($rootScope, $scope, assetsService, $routeParams, $timeout, $element, $location, umbRequestHelper,navigationService, mediaResource, $cookies) {
        var dialogOptions = $scope.dialogOptions;

        $scope.filesUploading = [];
        $scope.options = {
            url: umbRequestHelper.getApiUrl("mediaApiBaseUrl", "PostAddFile"),
            autoUpload: true,
            disableImageResize: /Android(?!.*Chrome)|Opera/
            .test(window.navigator.userAgent),
            previewMaxWidth: 200,
            previewMaxHeight: 200,
            previewCrop: true,
            formData:{
                currentFolder: -1
            }
        };


        $scope.loadChildren = function(){
            mediaResource.getChildren(-1)
                .then(function(data) {
                    $scope.images = data.items;
                });    
        };

        $scope.$on('fileuploadstop', function(event, files){
            $scope.loadChildren($scope.options.formData.currentFolder);
            $scope.queue = [];
            $scope.filesUploading = [];
            navigationService.reloadSection("media");
        });

        $scope.$on('fileuploadprocessalways', function(e,data) {
            var i;
            $scope.$apply(function() {
                $scope.filesUploading.push(data.files[data.index]);
            });
        });

        // All these sit-ups are to add dropzone area and make sure it gets removed if dragging is aborted! 
        $scope.$on('fileuploaddragover', function(event, files) {
            if (!$scope.dragClearTimeout) {
                $scope.$apply(function() {
                    $scope.dropping = true;
                });
            } else {
                $timeout.cancel($scope.dragClearTimeout);
            }
            $scope.dragClearTimeout = $timeout(function () {
                $scope.dropping = null;
                $scope.dragClearTimeout = null;
            }, 300);
        });
        
        //init load
        $scope.loadChildren();
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
    $scope.nav = navigationService;
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
function MediaDeleteController($scope, mediaResource, treeService, navigationService) {

    $scope.performDelete = function() {

        //mark it for deletion (used in the UI)
        $scope.currentNode.loading = true;

        mediaResource.deleteById($scope.currentNode.id).then(function () {
            $scope.currentNode.loading = false;

            //get the root node before we remove it
            var rootNode = treeService.getTreeRoot($scope.currentNode);

            //TODO: Need to sync tree, etc...
            treeService.removeNode($scope.currentNode);

            //ensure the recycle bin has child nodes now            
            var recycleBin = treeService.getDescendantNode(rootNode, -21);
            if(recycleBin){
                recycleBin.hasChildren = true;
            }
            
            navigationService.hideMenu();

        },function() {
            $scope.currentNode.loading = false;
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
    $scope.nav = navigationService;
    $scope.currentSection = appState.getSectionState("currentSection");
    $scope.currentNode = null; //the editors affiliated node

    /** Syncs the content item to it's tree node - this occurs on first load and after saving */
    function syncTreeNode(content, path, initialLoad) {

        //If this is a child of a list view then we can't actually sync the real tree
        if (!$scope.content.isChildOfListView) {
            navigationService.syncTree({ tree: "media", path: path.split(","), forceReload: initialLoad !== true }).then(function (syncArgs) {
                $scope.currentNode = syncArgs.node;
            });
        }
        else if (initialLoad === true) {
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
                
                editorState.set($scope.content);

                //in one particular special case, after we've created a new item we redirect back to the edit
                // route but there might be server validation errors in the collection which we need to display
                // after the redirect, so we will bind all subscriptions which will show the server validation errors
                // if there are any and then clear them so the collection no longer persists them.
                serverValidationManager.executeAndClearAllSubscriptions();

                syncTreeNode($scope.content, data.path, true);
                
            });

        //We fetch all ancestors of the node to generate the footer breadcrump navigation
        entityResource.getAncestors($routeParams.id, "media")
            .then(function(anc) {
                anc.pop();
                $scope.ancestors = anc;
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

	$scope.dialogTreeEventHandler.bind("treeNodeSelect", function(ev, args){
		args.event.preventDefault();
		args.event.stopPropagation();

		eventsService.emit("editors.media.moveController.select", args);
	    
		var c = $(args.event.target.parentElement);

		if ($scope.selectedEl) {
		    $scope.selectedEl.find(".temporary").remove();
		    $scope.selectedEl.find("i.umb-tree-icon").show();
		}

		var temp = "<i class='icon umb-tree-icon sprTree icon-check blue temporary'></i>";
		var icon = c.find("i.umb-tree-icon");
		if (icon.length > 0) {
		    icon.hide().after(temp);
		} else {
		    c.prepend(temp);
		}


		$scope.target = args.node;
		$scope.selectedEl = c;
	});


	$scope.move = function(){
		mediaResource.move({parentId: $scope.target.id, id: node.id})
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

			},function(err){
				$scope.success = false;
				$scope.error = err;
			});
	};
});
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
function MemberDeleteController($scope, memberResource, treeService, navigationService) {

    $scope.performDelete = function() {

        //mark it for deletion (used in the UI)
        $scope.currentNode.loading = true;

        memberResource.deleteByKey($scope.currentNode.id).then(function () {
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

angular.module("umbraco").controller("Umbraco.Editors.Member.DeleteController", MemberDeleteController);

/**
 * @ngdoc controller
 * @name Umbraco.Editors.Member.EditController
 * @function
 * 
 * @description
 * The controller for the member editor
 */
function MemberEditController($scope, $routeParams, $location, $q, $window, appState, memberResource, entityResource, navigationService, notificationsService, angularHelper, serverValidationManager, contentEditingHelper, fileManager, formHelper, umbModelMapper, editorState) {
    
    //setup scope vars
    $scope.nav = navigationService;
    $scope.currentSection = appState.getSectionState("currentSection");
    $scope.currentNode = null; //the editors affiliated node

    //build a path to sync the tree with
    function buildTreePath(data) {
        //TODO: Will this work for the 'other' list ?
        var path = data.name[0].toLowerCase() + "," + data.key;
        return path;
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

                    navigationService.syncTree({ tree: "member", path: path.split(",") }).then(function (syncArgs) {
                        $scope.currentNode = syncArgs.node;
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

                    navigationService.syncTree({ tree: "member", path: path.split(","), forceReload: true }).then(function (syncArgs) {
                        $scope.currentNode = syncArgs.node;
                    });

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

//this controller simply tells the dialogs service to open a mediaPicker window
//with a specified callback, this callback will receive an object with a selection on it
angular.module('umbraco')
.controller("Umbraco.PrevalueEditors.MediaPickerController",

	function($scope, dialogService, entityResource, $log, iconHelper){
		$scope.renderModel = [];
		$scope.ids = [];

		$scope.cfg = {
			multiPicker: false,
			entityType: "Media",
			type: "media",
			treeAlias: "media"
		};

		if($scope.model.value){
			$scope.ids = $scope.model.value.split(',');
			entityResource.getByIds($scope.ids, $scope.cfg.entityType).then(function(data){
				_.each(data, function (item, i) {
					item.icon = iconHelper.convertFromLegacyIcon(item.icon);
					$scope.renderModel.push({name: item.name, id: item.id, icon: item.icon});
				});
			});
		}


		$scope.openContentPicker =function(){
			var d = dialogService.treePicker({
				section: $scope.cfg.type,
				treeAlias: $scope.cfg.type,
				multiPicker: $scope.cfg.multiPicker,
				callback: populate});
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


		$scope.$on("formSubmitting", function (ev, args) {
			$scope.model.value = trim($scope.ids.join(), ",");
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


	    $scope.cfg = {
	        multiPicker: false,
	        entityType: "Document",
	        type: "content",
	        treeAlias: "content"
	    };
		
		if($scope.model.value){
			$scope.ids = $scope.model.value.split(',');
			entityResource.getByIds($scope.ids, $scope.cfg.entityType).then(function(data){
			    _.each(data, function (item, i) {
					item.icon = iconHelper.convertFromLegacyIcon(item.icon);
					$scope.renderModel.push({name: item.name, id: item.id, icon: item.icon});
				});
			});
		}
		

		$scope.openContentPicker =function(){
			var d = dialogService.treePicker({
								section: $scope.cfg.type,
								treeAlias: $scope.cfg.type,
								multiPicker: $scope.cfg.multiPicker,
								callback: populate});
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


	    $scope.$on("formSubmitting", function (ev, args) {
			$scope.model.value = trim($scope.ids.join(), ",");
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
	    $scope.$on("formSubmitting", function (ev, args) {
	    	if($scope.model.value.type === "member"){
	    		$scope.model.value.id = -1;
	    		$scope.model.value.query = "";
	    	}
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
            else if ($scope.model.value.newPassword.length > 0) {
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
        
        //listen for the saved event, when that occurs we'll 
        //change to changing = false;
        $scope.$on("formSubmitted", function () {
            if ($scope.model.config.disableToggle === false) {
                $scope.changing = false;
            }            
        });
        $scope.$on("formSubmitting", function() {
            //if there was a previously generated password displaying, clear it
            if ($scope.changing && $scope.model.value) {
                $scope.model.value.generatedPassword = null;
            }
            else if (!$scope.changing) {
                //we are not changing, so the model needs to be null
                $scope.model.value = null;
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
        }
        else {
            $scope.model.value = color;
        }
    };
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
        ]).then(function () {
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
angular.module('umbraco')
.controller("Umbraco.PropertyEditors.ContentPickerController",

	function($scope, dialogService, entityResource, editorState, $log, iconHelper, $routeParams){
	    $scope.renderModel = [];
	    $scope.ids = $scope.model.value ? $scope.model.value.split(',') : [];
        
	    //configuration
	    $scope.cfg = {
	        multiPicker: "0",
	        entityType: "Document",
			filterCssClass: "not-allowed not-published",

	        startNode: {
				query: "",
	            type: "content",
	            id: -1
	        }
	    };

	    if ($scope.model.config) {
	        $scope.cfg = angular.extend($scope.cfg, $scope.model.config);
	    }

	    //Umbraco persists boolean for prevalues as "0" or "1" so we need to convert that!
	    $scope.cfg.multiPicker = ($scope.cfg.multiPicker === "0" ? false : true);

	    if ($scope.cfg.startNode.type === "member") {
	        $scope.cfg.entityType = "Member";
	    }
	    else if ($scope.cfg.startNode.type === "media") {
	        $scope.cfg.entityType = "Media";
	    }

		//if we have a query for the startnode, we will use that. 
		if($scope.cfg.startNode.query){
			var rootId = $routeParams.id; 
			entityResource.getByQuery($scope.cfg.startNode.query, rootId, "Document").then(function(ent){
				$scope.cfg.startNodeId = ent.id;	
			});	
		}else{
			$scope.cfg.startNodeId = $scope.cfg.startNode.id;
		}

	    $scope.cfg.callback = populate;
	    $scope.cfg.treeAlias = $scope.cfg.startNode.type;
		$scope.cfg.section = $scope.cfg.startNode.type;

	    //load current data
	    entityResource.getByIds($scope.ids, $scope.cfg.entityType).then(function (data) {
	        _.each(data, function (item, i) {
	            item.icon = iconHelper.convertFromLegacyIcon(item.icon);
	            $scope.renderModel.push({ name: item.name, id: item.id, icon: item.icon });
	        });
	    });


	    //dialog
	    $scope.openContentPicker = function () {
	        var d = dialogService.treePicker($scope.cfg);
	    };


	    $scope.remove = function (index) {
	        $scope.renderModel.splice(index, 1);
	    };


	    $scope.add = function (item) {
	        if ($scope.ids.indexOf(item.id) < 0) {
	            item.icon = iconHelper.convertFromLegacyIcon(item.icon);
	            $scope.renderModel.push({ name: item.name, id: item.id, icon: item.icon });
	        }
	    };

	    $scope.clear = function () {
	        $scope.renderModel = [];
	    };

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
	        $scope.ids = _.map($scope.renderModel, function (i) {
	            return i.id;
	        });
	        $scope.model.value = trim($scope.ids.join(), ",");

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

	    $scope.$on("formSubmitting", function (ev, args) {
	        $scope.model.value = trim($scope.ids.join(), ",");
	    });

	    function trim(str, chr) {
	        var rgxtrim = (!chr) ? new RegExp('^\\s+|\\s+$', 'g') : new RegExp('^' + chr + '+|' + chr + '+$', 'g');
	        return str.replace(rgxtrim, '');
	    }

	    function populate(data) {
	        if (angular.isArray(data)) {
	            _.each(data, function (item, i) {
	                $scope.add(item);
	            });
	        } else {
	            $scope.clear();
	            $scope.add(data);
	        }
	    }
	});
function dateTimePickerController($scope, notificationsService, assetsService, angularHelper, userService, $element) {

    //lists the custom language files that we currently support
    var customLangs = ["pt-BR"];

    //setup the default config
    var config = {
        pickDate: true,
        pickTime: true,
        pick12HourFormat: false,
        format: "yyyy-MM-dd hh:mm:ss"
    };

    //map the user config
    $scope.model.config = angular.extend(config, $scope.model.config);

    //handles the date changing via the api
    function applyDate(e) {
        angularHelper.safeApply($scope, function() {
            // when a date is changed, update the model
            if (e.localDate) {
                if ($scope.model.config.format == "yyyy-MM-dd hh:mm:ss") {
                    $scope.model.value = e.localDate.toIsoDateTimeString();
                }
                else {
                    $scope.model.value = e.localDate.toIsoDateString();
                }
            }
        });
    }

    //get the current user to see if we can localize this picker
    userService.getCurrentUser().then(function (user) {

        assetsService.loadCss('lib/datetimepicker/bootstrap-datetimepicker.min.css').then(function() {
            var filesToLoad = ["lib/datetimepicker/bootstrap-datetimepicker.min.js"];

            //if we support this custom culture, set it, then we'll need to load in that lang file
            if (_.contains(customLangs, user.locale)) {
                $scope.model.config.language = user.locale;
                filesToLoad.push("lib/datetimepicker/langs/datetimepicker." + user.locale + ".js");
            }

            assetsService.load(filesToLoad).then(
                function () {
                    //The Datepicker js and css files are available and all components are ready to use.

                    // Get the id of the datepicker button that was clicked
                    var pickerId = $scope.model.alias;

                    // Open the datepicker and add a changeDate eventlistener
                    $element.find("div:first")
                        .datetimepicker($scope.model.config)
                        .on("changeDate", applyDate);

                    if ($scope.model.value) {
                        //manually assign the date to the plugin
                        $element.find("div:first").datetimepicker("setValue", $scope.model.value);
                    }

                    //Ensure to remove the event handler when this instance is destroyted
                    $scope.$on('$destroy', function () {
                        $element.find("div:first").datetimepicker("destroy");
                    });
                });
        });

        
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

    //listen for clear files changes to set our model to be sent up to the server
    $scope.$watch("clearFiles", function (isCleared) {
        if (isCleared == true) {
            $scope.model.value = { clearFiles: true };
            clearFiles();
        }
        else {
            //reset to original value
            $scope.model.value = $scope.originalValue;
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
.directive("umbUploadPreview",function($parse){
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
.controller("Umbraco.PropertyEditors.FolderBrowserController",
    function ($rootScope, $scope, assetsService, $routeParams, $timeout, $element, $location, $log, umbRequestHelper, mediaResource, imageHelper, navigationService, editorState) {
        var dialogOptions = $scope.dialogOptions;

        $scope.creating = $routeParams.create;

        if(!$scope.creating){

            $scope.filesUploading = [];
            $scope.options = {                
                url: umbRequestHelper.getApiUrl("mediaApiBaseUrl", "PostAddFile"),
                autoUpload: true,
                disableImageResize: /Android(?!.*Chrome)|Opera/
                .test(window.navigator.userAgent),
                previewMaxWidth: 200,
                previewMaxHeight: 200,
                previewCrop: true,
                formData:{
                    currentFolder: $routeParams.id
                }
            };


            $scope.loadChildren = function(id){
                mediaResource.getChildren(id)
                    .then(function(data) {
                        $scope.images = data.items;
                    });    
            };

            $scope.$on('fileuploadstop', function(event, files){
                $scope.loadChildren($scope.options.formData.currentFolder);
                
                //sync the tree - don't force reload since we're not updating this particular node (i.e. its name or anything),
                // then we'll get the resulting tree node which we can then use to reload it's children.
                var path = editorState.current.path;
                navigationService.syncTree({ tree: "media", path: path, forceReload: false }).then(function (syncArgs) {
                    navigationService.reloadNode(syncArgs.node);
                });

                $scope.queue = [];
                $scope.filesUploading = [];
            });

            $scope.$on('fileuploadprocessalways', function(e,data) {
                var i;
                $scope.$apply(function() {
                    $scope.filesUploading.push(data.files[data.index]);
                });
            });

            // All these sit-ups are to add dropzone area and make sure it gets removed if dragging is aborted! 
            $scope.$on('fileuploaddragover', function(event, files) {
                if (!$scope.dragClearTimeout) {
                    $scope.$apply(function() {
                        $scope.dropping = true;
                    });
                } else {
                    $timeout.cancel($scope.dragClearTimeout);
                }
                $scope.dragClearTimeout = $timeout(function () {
                    $scope.dropping = null;
                    $scope.dragClearTimeout = null;
                }, 300);
            });
            
            //init load
            $scope.loadChildren($routeParams.id);
        }
});

angular.module("umbraco")
.controller("Umbraco.PropertyEditors.GoogleMapsController",
    function ($rootScope, $scope, notificationsService, dialogService, assetsService, $log, $timeout) {

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

            $('a[data-toggle="tab"]').on('shown', function (e) {
                google.maps.event.trigger(map, 'resize');
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
'use strict';
//this controller simply tells the dialogs service to open a mediaPicker window
//with a specified callback, this callback will receive an object with a selection on it
angular.module("umbraco").controller("Umbraco.PropertyEditors.GridController",
  function($rootScope, $scope, dialogService, $log){
    //we most likely will need some iframe-motherpage interop here
    
    //we most likely will need some iframe-motherpage interop here
       $scope.openMediaPicker =function(){
               var d = dialogService.mediaPicker({callback: renderImages});
       };

       $scope.openPropertyDialog =function(){
               var d = dialogService.property({callback: renderProperty});
       };

       $scope.openMacroDialog =function(){
               var d = dialogService.macroPicker({callback: renderMacro});
       };

       function renderProperty(data){
          $scope.currentElement.html("<h1>boom, property!</h1>"); 
       }

       function renderMacro(data){
       //   $scope.currentElement.html( macroFactory.renderMacro(data.macro, -1) ); 
       }
      
       function renderImages(data) {
           var list = $("<ul class='thumbnails'></ul>");
           $.each(data.selection, function(i, image) {
               list.append( $("<li class='span2'><img class='thumbnail' src='" + image.src + "'></li>") );
           });

           $scope.currentElement.html( list[0].outerHTML); 
       }

       $(window).bind("umbraco.grid.click", function(event){

           $scope.$apply(function () {
               $scope.currentEditor = event.editor;
               $scope.currentElement = $(event.element);

               if(event.editor == "macro")
                   $scope.openMacroDialog();
               else if(event.editor == "image")
                   $scope.openMediaPicker();
               else
                   $scope.propertyDialog();
           });
       })
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
function listViewController($rootScope, $scope, $routeParams, $injector, notificationsService, iconHelper, dialogService) {

    //this is a quick check to see if we're in create mode, if so just exit - we cannot show children for content 
    // that isn't created yet, if we continue this will use the parent id in the route params which isn't what
    // we want. NOTE: This is just a safety check since when we scaffold an empty model on the server we remove
    // the list view tab entirely when it's new.
    if ($routeParams.create) {
        $scope.isNew = true;
        return;
    }

    //Now we need to check if this is for media or content because that will depend on the resources we use
    var contentResource, contentTypeResource;
    if ($scope.model.config.entityType && $scope.model.config.entityType === "media") {
        contentResource = $injector.get('mediaResource');
        contentTypeResource = $injector.get('mediaTypeResource');
        $scope.entityType = "media";
    }
    else {
        contentResource = $injector.get('contentResource');
        contentTypeResource = $injector.get('contentTypeResource');
        $scope.entityType = "content";
    }

    $scope.isNew = false;
    $scope.actionInProgress = false;
    $scope.listViewResultSet = {
        totalPages: 0,
        items: []
    };

    $scope.options = {
        pageSize: 10,
        pageNumber: 1,
        filter: '',
        orderBy: 'UpdateDate',
        orderDirection: "desc"
    };


    $scope.next = function () {
        if ($scope.options.pageNumber < $scope.listViewResultSet.totalPages) {
            $scope.options.pageNumber++;
            $scope.reloadView($scope.contentId);
        }
    };

    $scope.goToPage = function (pageNumber) {
        $scope.options.pageNumber = pageNumber + 1;
        $scope.reloadView($scope.contentId);
    };

    $scope.sort = function (field) {

        $scope.options.orderBy = field;


        if ($scope.options.orderDirection === "desc") {
            $scope.options.orderDirection = "asc";
        } else {
            $scope.options.orderDirection = "desc";
        }


        $scope.reloadView($scope.contentId);
    };

    $scope.prev = function () {
        if ($scope.options.pageNumber > 1) {
            $scope.options.pageNumber--;
            $scope.reloadView($scope.contentId);
        }
    };

    /*Loads the search results, based on parameters set in prev,next,sort and so on*/
    /*Pagination is done by an array of objects, due angularJS's funky way of monitoring state
    with simple values */

    $scope.reloadView = function (id) {
        contentResource.getChildren(id, $scope.options).then(function (data) {

            $scope.listViewResultSet = data;
            $scope.pagination = [];            

            for (var i = $scope.listViewResultSet.totalPages - 1; i >= 0; i--) {
                $scope.pagination[i] = { index: i, name: i + 1 };
            }

            if ($scope.options.pageNumber > $scope.listViewResultSet.totalPages) {
                $scope.options.pageNumber = $scope.listViewResultSet.totalPages;
            }

        });
    };

    //assign debounce method to the search to limit the queries
    $scope.search = _.debounce(function () {
        $scope.options.pageNumber = 1;
        $scope.reloadView($scope.contentId);
    }, 100);

    $scope.selectAll = function ($event) {
        var checkbox = $event.target;
        if (!angular.isArray($scope.listViewResultSet.items)) {
            return;
        }
        for (var i = 0; i < $scope.listViewResultSet.items.length; i++) {
            var entity = $scope.listViewResultSet.items[i];
            entity.selected = checkbox.checked;
        }
    };

    $scope.isSelectedAll = function () {
        if (!angular.isArray($scope.listViewResultSet.items)) {
            return false;
        }
        return _.every($scope.listViewResultSet.items, function (item) {
            return item.selected;
        });
    };

    $scope.isAnythingSelected = function () {
        if (!angular.isArray($scope.listViewResultSet.items)) {
            return false;
        }
        return _.some($scope.listViewResultSet.items, function (item) {
            return item.selected;
        });
    };

    $scope.getIcon = function (entry) {
        return iconHelper.convertFromLegacyIcon(entry.icon);
    };

    $scope.delete = function () {
        var selected = _.filter($scope.listViewResultSet.items, function (item) {
            return item.selected;
        });
        var total = selected.length;
        if (total === 0) {
            return;
        }

        if (confirm("Sure you want to delete?") == true) {
            $scope.actionInProgress = true;
            $scope.bulkStatus = "Starting with delete";
            var current = 1;

            for (var i = 0; i < selected.length; i++) {
                $scope.bulkStatus = "Deleted doc " + current + " out of " + total + " documents";
                contentResource.deleteById(selected[i].id).then(function (data) {
                    if (current === total) {
                        notificationsService.success("Bulk action", "Deleted " + total + "documents");
                        $scope.bulkStatus = "";
                        $scope.reloadView($scope.contentId);
                        $scope.actionInProgress = false;
                    }
                    current++;
                });
            }
        }

    };

    $scope.publish = function () {
        var selected = _.filter($scope.listViewResultSet.items, function (item) {
            return item.selected;
        });
        var total = selected.length;
        if (total === 0) {
            return;
        }

        $scope.actionInProgress = true;
        $scope.bulkStatus = "Starting with publish";
        var current = 1;

        for (var i = 0; i < selected.length; i++) {
            $scope.bulkStatus = "Publishing " + current + " out of " + total + " documents";

            contentResource.publishById(selected[i].id)
                .then(function (content) {
                    if (current == total) {
                        notificationsService.success("Bulk action", "Published " + total + "documents");
                        $scope.bulkStatus = "";
                        $scope.reloadView($scope.contentId);
                        $scope.actionInProgress = false;
                    }
                    current++;
                }, function (err) {

                    $scope.bulkStatus = "";
                    $scope.reloadView($scope.contentId);
                    $scope.actionInProgress = false;

                    //if there are validation errors for publishing then we need to show them
                    if (err.status === 400 && err.data && err.data.Message) {
                        notificationsService.error("Publish error", err.data.Message);
                    }
                    else {
                        dialogService.ysodDialog(err);
                    }
                });

        }
    };

    $scope.unpublish = function () {
        var selected = _.filter($scope.listViewResultSet.items, function (item) {
            return item.selected;
        });
        var total = selected.length;
        if (total === 0) {
            return;
        }

        $scope.actionInProgress = true;
        $scope.bulkStatus = "Starting with publish";
        var current = 1;

        for (var i = 0; i < selected.length; i++) {
            $scope.bulkStatus = "Unpublishing " + current + " out of " + total + " documents";

            contentResource.unPublish(selected[i].id)
                .then(function (content) {

                    if (current == total) {
                        notificationsService.success("Bulk action", "Published " + total + "documents");
                        $scope.bulkStatus = "";
                        $scope.reloadView($scope.contentId);
                        $scope.actionInProgress = false;
                    }

                    current++;
                });
        }
    };

    if ($routeParams.id) {
        $scope.pagination = new Array(10);
        $scope.listViewAllowedTypes = contentTypeResource.getAllowedTypes($routeParams.id);
        $scope.reloadView($routeParams.id);

        $scope.contentId = $routeParams.id;
        $scope.isTrashed = $routeParams.id === "-20" || $routeParams.id === "-21";

    }

}

angular.module("umbraco").controller("Umbraco.PropertyEditors.ListViewController", listViewController);
//DO NOT DELETE THIS, this is in use... 
angular.module('umbraco')
.controller("Umbraco.PropertyEditors.MacroContainerController",
	
	function($scope, dialogService, entityResource, macroService, macroResource){
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
			var dialogData = {};

			if(index !== null && $scope.renderModel[index]) {
				var macro = $scope.renderModel[index];
				dialogData = {macroData: macro};
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

		$scope.add =function(){
				openDialog();
		};

		$scope.remove =function(index){
			$scope.renderModel.splice(index, 1);
			$scope.macros.splice(index, 1);
			$scope.model.value = trim($scope.macros.join(), ",");
		};

	    $scope.clear = function() {
	        $scope.model.value = "";
	        $scope.renderModel = [];
	        $scope.macros = [];
	    };

	    $scope.$on("formSubmitting", function (ev, args) {	
			var syntax = [];
	    	angular.forEach($scope.renderModel, function(value, key){
	    		syntax.push(value.syntax);
	    	});

			$scope.model.value = syntax.join("");
	    });


		function trim(str, chr) {
			var rgxtrim = (!chr) ? new RegExp('^\\s+|\\s+$', 'g') : new RegExp('^'+chr+'+|'+chr+'+$', 'g');
			return str.replace(rgxtrim, '');
		}

});
angular.module("umbraco")
.controller("Umbraco.PropertyEditors.MarkdownEditorController",
//inject umbracos assetsServce and dialog service
function ($scope, assetsService, dialogService, $log, imageHelper) {

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

		    //this function will execute when all dependencies have loaded
		    var converter2 = new Markdown.Converter();
		    var editor2 = new Markdown.Editor(converter2, "-" + $scope.model.alias);
		    editor2.run();

		    //subscribe to the image dialog clicks
		    editor2.hooks.set("insertImageDialog", function (callback) {

		        dialogService.mediaPicker({ callback: function (data) {
					callback(data.url);
		        	}
		        });

		        return true; // tell the editor that we'll take care of getting the image url
		    });

		});

    //load the seperat css for the editor to avoid it blocking our js loading TEMP HACK
    assetsService.loadCss("lib/markdown/markdown.css");
});
//this controller simply tells the dialogs service to open a mediaPicker window
//with a specified callback, this callback will receive an object with a selection on it
angular.module('umbraco').controller("Umbraco.PropertyEditors.MediaPickerController",
    function ($rootScope, $scope, dialogService, entityResource, mediaResource, mediaHelper, $timeout) {

        //check the pre-values for multi-picker
        var multiPicker = $scope.model.config.multiPicker && $scope.model.config.multiPicker !== '0' ? true : false;

        if (!$scope.model.config.startNodeId)
             $scope.model.config.startNodeId = -1;

         
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
angular.module('umbraco')
.controller("Umbraco.PropertyEditors.MemberGroupPickerController",
	
	function($scope, dialogService){
		$scope.renderModel = [];
		$scope.ids = [];

		
	    
	    if ($scope.model.value) {
	        $scope.ids = $scope.model.value.split(',');

	        $($scope.ids).each(function (i, item) {
	           
	            $scope.renderModel.push({ name: item, id: item, icon: 'icon-users' });
	        });
	    }
	    
	    $scope.cfg = {multiPicker: true, entityType: "MemberGroup", type: "membergroup", treeAlias: "memberGroup", filter: ""};
		if($scope.model.config){
			$scope.cfg = angular.extend($scope.cfg, $scope.model.config);
		}

		

		$scope.openMemberGroupPicker =function(){
				var d = dialogService.memberGroupPicker(
							{
								multiPicker: $scope.cfg.multiPicker,
								filter: $scope.cfg.filter,
								filterCssClass: "not-allowed", 
								callback: populate}
								);
		};


		$scope.remove =function(index){
			$scope.renderModel.splice(index, 1);
			$scope.ids.splice(index, 1);
			$scope.model.value = trim($scope.ids.join(), ",");
		};

		$scope.add =function(item){
			if($scope.ids.indexOf(item) < 0){
				//item.icon = iconHelper.convertFromLegacyIcon(item.icon);

				$scope.ids.push(item);
				$scope.renderModel.push({ name: item, id: item, icon: 'icon-users' });
				$scope.model.value = trim($scope.ids.join(), ",");
			}	
		};

	    $scope.clear = function() {
	        $scope.model.value = "";
	        $scope.renderModel = [];
	        $scope.ids = [];
	    };
	   


	    $scope.$on("formSubmitting", function (ev, args) {
			$scope.model.value = trim($scope.ids.join(), ",");
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
angular.module('umbraco')
.controller("Umbraco.PropertyEditors.MemberPickerController",
	
	function($scope, dialogService, entityResource, $log, iconHelper){
		$scope.renderModel = [];
		$scope.ids = $scope.model.value.split(',');

		$scope.cfg = {multiPicker: false, entityType: "Member", type: "member", treeAlias: "member", filter: ""};
		if($scope.model.config){
			$scope.cfg = angular.extend($scope.cfg, $scope.model.config);
		}

		entityResource.getByIds($scope.ids, $scope.cfg.entityType).then(function(data){
		    _.each(data, function (item, i) {
				item.icon = iconHelper.convertFromLegacyIcon(item.icon);
				$scope.renderModel.push({name: item.name, id: item.id, icon: item.icon});
			});
		});

		$scope.openMemberPicker =function(){
				var d = dialogService.memberPicker(
							{
								multiPicker: $scope.cfg.multiPicker,
								filter: $scope.cfg.filter,
								filterCssClass: "not-allowed", 
								callback: populate}
								);
		};


		$scope.remove =function(index){
			$scope.renderModel.splice(index, 1);
		};

		$scope.add =function(item){
			if($scope.ids.indexOf(item.id) < 0){
				item.icon = iconHelper.convertFromLegacyIcon(item.icon);
				$scope.renderModel.push({name: item.name, id: item.id, icon: item.icon});				
			}	
		};

	    $scope.clear = function() {
	        $scope.renderModel = [];
	    };
	   
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
	        $scope.ids = _.map($scope.renderModel, function (i) {
	            return i.id;
	        });
	        $scope.model.value = trim($scope.ids.join(), ",");
	    });

	    $scope.$on("formSubmitting", function (ev, args) {
			$scope.model.value = trim($scope.ids.join(), ",");
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
function MultipleTextBoxController($scope) {

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

            $scope.cancelEdit = function(idx) {
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
                    if ($scope.model.value[i].link === url) {
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
                stylesheets.push("../css/" + val + ".css?" + new Date().getTime());

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
                    angular.extend(baseLineConfigObj, tinyMceConfig.customConfig);
                }

                //set all the things that user configs should not be able to override
                baseLineConfigObj.elements = $scope.model.alias + "_rte";
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
                        editor.save();
                        angularHelper.safeApply($scope, function () {
                            $scope.model.value = editor.getContent();
                        });
                    });

                    // Update model on keypress
                    editor.on('KeyUp', function (e) {
                        editor.save();
                        angularHelper.safeApply($scope, function () {
                            $scope.model.value = editor.getContent();
                        });
                    });

                    // Update model on change, i.e. copy/pasted text, plugins altering content
                    editor.on('SetContent', function (e) {
                        if (!e.initial) {
                            editor.save();
                            angularHelper.safeApply($scope, function () {
                                $scope.model.value = editor.getContent();
                            });
                        }
                    });


                    editor.on('ObjectResized', function (e) {
                        var qs = "?width=" + e.width + "px&height=" + e.height + "px";
                        var srcAttr = $(e.target).attr("src");
                        var path = srcAttr.split("?")[0];
                        $(e.target).attr("data-mce-src", path + qs);
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

        $scope.$on("formSubmitting", function (ev, args) {

            var commands = _.where($scope.tinyMceConfig.commands, {selected: true});
            $scope.model.value.toolbar = _.pluck(commands, "frontEndCommand");

            
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

        //the value that we'll give the slider - if it's a range, we store our value as a comma seperated val but this slider expects an array
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

    //load the seperate css for the editor to avoid it blocking our js loading
    assetsService.loadCss("lib/slider/slider.css");

}
angular.module("umbraco").controller("Umbraco.PropertyEditors.SliderController", sliderController);
angular.module("umbraco")
.controller("Umbraco.PropertyEditors.TagsController",
    function ($rootScope, $scope, $log, assetsService, umbRequestHelper, angularHelper, $timeout, $element) {

        var $typeahead;

        $scope.isLoading = true;
        $scope.tagToAdd = "";

        assetsService.loadJs("lib/typeahead/typeahead.bundle.min.js").then(function () {

            $scope.isLoading = false;

            //load current value
            $scope.currentTags = [];
            if ($scope.model.value) {
                if ($scope.model.config.storageType && $scope.model.config.storageType === "Json") {
                    //it's a json array already
                    $scope.currentTags = $scope.model.value;
                }
                else {
                    //it is csv
                    if (!$scope.model.value) {
                        $scope.currentTags = [];
                    }
                    else {
                        $scope.currentTags = $scope.model.value.split(",");
                    }
                }
            }

            //Helper method to add a tag on enter or on typeahead select
            function addTag(tagToAdd) {
                if (tagToAdd.length > 0) {
                    if ($scope.currentTags.indexOf(tagToAdd) < 0) {                       
                        $scope.currentTags.push(tagToAdd);
                    }
                }
            }

            $scope.addTag = function (e) {
                var code = e.keyCode || e.which;
                if (code == 13) { //Enter keycode   

                    //ensure that we're not pressing the enter key whilst selecting a typeahead value from the drop down
                    if ($element.find('.tags-' + $scope.model.alias).parent().find(".tt-dropdown-menu .tt-cursor").length === 0) {
                        //this is required, otherwise the html form will attempt to submit.
                        e.preventDefault();
                        //we need to use jquery because typeahead duplicates the text box
                        addTag($scope.tagToAdd);
                        $scope.tagToAdd = "";
                        //this clears the value stored in typeahead so it doesn't try to add the text again
                        // http://issues.umbraco.org/issue/U4-4947
                        $typeahead.typeahead('val', '');
                    }

                }
            };

            $scope.removeTag = function (tag) {
                var i = $scope.currentTags.indexOf(tag);
                if (i >= 0) {
                    $scope.currentTags.splice(i, 1);
                }
            };

            //sync model on submit, always push up a json array
            $scope.$on("formSubmitting", function (ev, args) {
                $scope.model.value = $scope.currentTags;
            });

            //vice versa
            $scope.model.onValueChanged = function (newVal, oldVal) {
                //update the display val again if it has changed from the server
                $scope.model.value = newVal;

                if ($scope.model.config.storageType && $scope.model.config.storageType === "Json") {
                    //it's a json array already
                    $scope.currentTags = $scope.model.value;
                }
                else {
                    //it is csv
                    if (!$scope.model.value) {
                        $scope.currentTags = [];
                    }
                    else {
                        $scope.currentTags = $scope.model.value.split(",");
                    }
                }
            };

            //configure the tags data source
            //TODO: We'd like to be able to filter the shown list items to not show the tags that are currently
            // selected but that is difficult, i've tried a number of things and also this link suggests we cannot do 
            // it currently without a lot of hacking:
            // http://stackoverflow.com/questions/21044906/twitter-typeahead-js-remove-datum-upon-selection

            //helper method to format the data for bloodhound
            function dataTransform(list) {
                //transform the result to what bloodhound wants
                return _.map(list, function (i) {
                    return { value: i.text };
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

            tagsHound.initialize();

            //configure the type ahead
            $timeout(function () {

                $typeahead = $element.find('.tags-' + $scope.model.alias).typeahead(
                {
                    //This causes some strangeness as it duplicates the textbox, best leave off for now.
                    hint: false,
                    highlight: true,
                    minLength: 1
                }, {
                    //see: https://github.com/twitter/typeahead.js/blob/master/doc/jquery_typeahead.md#options
                    // name = the data set name, we'll make this the tag group name
                    name: $scope.model.config.group,
                    displayKey: "value",
                    source: tagsHound.ttAdapter(),
                }).bind("typeahead:selected", function (obj, datum, name) {
                    angularHelper.safeApply($scope, function () {
                        addTag(datum["value"]);
                        $scope.tagToAdd = "";
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
            $scope.renderModel = _.map($scope.model.value.split(","), function (item) {
                return {
                    url: item,
                    urlTarget: ($scope.config && $scope.config.target) ? $scope.config.target : "_blank"
                };
            });
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
/**
 * @ngdoc controller
 * @name Umbraco.Editors.Settings.Template.EditController
 * @function
 * 
 * @description
 * The controller for editing templates
 */
function TemplateEditController($scope, navigationService) {
    $scope.template = "<html><body><h1>Hej</h1></body></html>";
}

angular.module("umbraco").controller("Umbraco.Editors.Settings.Template.EditController", TemplateEditController);


})();