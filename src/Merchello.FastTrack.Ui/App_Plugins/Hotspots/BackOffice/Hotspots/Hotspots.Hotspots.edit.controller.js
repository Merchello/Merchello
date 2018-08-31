'use strict';
(function () {

	//register the controller
	angular.module("umbraco").controller('ETC.HotspotsController', ['$scope', '$http', '$timeout', '$routeParams', '$location', 'appState', '$rootScope', 'navigationService', 'notificationsService', 'dialogService', 'contentResource', '$attrs', function HotSpotController($scope, $http, $timeout, $routeParams, $location, appState, $rootScope, navigationService, notificationsService, dialogService, contentResource, $attrs) {

		//setup scope vars
		$scope.page = {};
		$scope.page.loading = false;
		$scope.page.nameLocked = false;
		$scope.page.menu = {};
		$scope.page.menu.currentSection = appState.getSectionState("currentSection");
		$scope.page.menu.currentNode = null;
		$scope.dialog = $attrs.dialog == 'true';

		//set a property on the scope equal to the current route id
		$scope.id = $routeParams.id;

		// only set parent on creation
		$scope.parentId = $routeParams.parentid || -1;

		$scope.tabId = 0;

		// breakpoint tabs
		$scope.Tabs = [{ id: 0, label: 'Default', alias: 'Default' }, { id: 1, label: 'Tablet', alias: 'Tablet' }, { id: 2, label: 'Mobile', alias: 'Mobile' }];

		$scope.data = {};

		// tab change, load new data.
		var deregistration = $rootScope.$on('app.tabChange', function (a, b) {
			$scope.GetHotspotData(b.id);
			$scope.tabId = b.id;
		});

		// tab change, load new data.
		$scope.$on('$destroy', function (a, b) {
			deregistration();
		});

		// Data inlezen vanaf server
		$scope.GetHotspotData = function(breakpointid) {

			$http.get('backoffice/Hotspots/HotspotBackOffice/GetHotspotData/' + $scope.id + "/?breakpointid=" + breakpointid, {
				cache: false
			}).then(function (response) {

				// todo, jsonspecs, kwam voorheen met de route mee.
				$scope.data = response.data;
				
				navigationService.syncTree({ tree: "Hotspots", path: $scope.id }).then(function (syncArgs) {
					$scope.page.menu.currentNode = syncArgs.node;
				});

			}, function (error) {
				notificationsService.error("Fout bij het laden", error);

			});
		}

		$scope.Commit = function () {


			if (!$scope.data.Name || !$scope.data.Name.length) {
				notificationsService.error("Vul aub een naam in voor deze hotspot");
				return false;
			}

			var data = $.wcpEditorGetExportJSON();
			
			$http.post('backoffice/Hotspots/HotSpotBackOffice/SaveHotspotData/', {
				"Id": $scope.data.Id,
				"Data": data,
				"Name": $scope.data.Name,
				"StartNodeId": $scope.data.StartNodeId,
				"NodeId": $scope.data.NodeId,
				"ParentId": $scope.data.ParentId,
				"BreakPointId": $scope.data.BreakPointId
			}).then(function (res) {
							
				var newId = res.data.Id;
				notificationsService.success("De wijzigingen zijn succesvol opgelagen");

				$scope.data = res.data;

				$scope.contentForm.$dirty = false;

				if ($scope.id == "0" && newId != $scope.id) {
					$location.path("/Hotspots/Hotspots/edit/" + newId);
					return;
				}

			}, function (err) {
				notificationsService.error("Niet opgeslagen", err);
			});

			return false;

		}


		$scope.Cancel = function () {			
			navigationService.hideNavigation();
		}

		$scope.Delete = function () {
			
			$http.post('backoffice/Hotspots/HotSpotBackOffice/DeleteHotspotData/', { "Id": $scope.id, "Data": $scope.data.Data, "Name": $scope.data.Name }).then(function (res) {

				var newId = res.data.Id;
				navigationService.hideNavigation();

				notificationsService.success("De hotspot is succesvol verwijderd");

				$location.path("/Hotspots/Hotspots/edit/" + newId);
				
				navigationService.syncTree({ tree: "Hotspots", path:[ "-1", newId.toString()], forceReload: true, activate : true }).then(function (syncArgs) {
					$scope.page.menu.currentNode = syncArgs.node;
				});
				
			}, function (err) {
				notificationsService.error("Niet verwijderd", err);
				dialogService.closeAll();
			});


			return false;

		}

		$scope.GetMedia = function() {

			dialogService.mediaPicker({
				onlyImages: true,
				startNodeId: $scope.data.StartNodeId || -1,
				callback: function (e, b) {
					if (e.isFolder) {
						return false;
					}
					$('#wcp-editor-form-control-image_url input').val(e.image).change();
					$scope.data.StartNodeId = e.parentId;
					$scope.data.NodeId = e.id;
				}
			});

		}

		$scope.GetLink = function () {
			dialogService.contentPicker({
				//startNodeId: $scope.data.LinkNodeId || -1,
				callback: function (e, b) {
					contentResource.getById(e.id).then(function (a,b) {
						$('#wcp-editor-form-control-link input').val(a.urls[0]).change();
					});										
				}
			});
		}

		//if (!$scope.dialog) {
		//	$timeout(function () {
		//		navigationService.syncTree({ tree: "Hotspots", path: $scope.data.path }).then(function (syncArgs) {
		//			$scope.page.menu.currentNode = syncArgs.node;
		//		});
		//	}, 100);
		//}
		
	}]).directive('hotspotEditor', ['$timeout', 'assetsService', function ($timeout, assetsService) {

		/*
		 *	Directive for the hotspot editor.
		 *	Loads the hotspot files, init the hotspot plugin with the data from the server
		 *	Watch on $scope.data.Data, when this changes the hotspot plugin is reinitialized. 
		 */

		return {
			template: '<div id=\'wcp-editor\'></div>',
			restrict: 'E',
			transclude: true,

			link: function (scope, element, attr) {


				var hotspot = undefined;
				var initDone = false;

				function initMap() {
					$.image_map_pro_init_editor(hotspot, $.WCPEditorSettings);
					initEvents();

					initDone = true;
				}

				function initEvents() {
					$('#wcp-editor-form-control-image_url .wcp-editor-control-button').click(function () {
						scope.GetMedia();
					});

					$('.wcp-editor-form-tabs-content-wrap').on('click', '#wcp-editor-form-control-choose_link_from_library', function () {
						scope.GetLink();
					});
				}

				assetsService
					.load([
						"~/App_Plugins/Hotspots/BackOffice/Hotspots/submodules/squares/js/squares-renderer.js",
						"~/App_Plugins/Hotspots/BackOffice/Hotspots/submodules/squares/js/squares.js",
						"~/App_Plugins/Hotspots/BackOffice/Hotspots/submodules/squares/js/squares-elements-jquery.js",
						"~/App_Plugins/Hotspots/BackOffice/Hotspots/submodules/squares/js/squares-controls.js",

						"~/App_Plugins/Hotspots/BackOffice/Hotspots/submodules/wcp-editor/js/wcp-editor.js",
						"~/App_Plugins/Hotspots/BackOffice/Hotspots/submodules/wcp-editor/js/wcp-editor-controls.js",
						"~/App_Plugins/Hotspots/BackOffice/Hotspots/submodules/wcp-compress/js/wcp-compress.js",
						"~/App_Plugins/Hotspots/BackOffice/Hotspots/submodules/wcp-icons/js/wcp-icons.js",

						"~/App_Plugins/Hotspots/BackOffice/Hotspots/js/image-map-pro-defaults.js",
						"~/App_Plugins/Hotspots/BackOffice/Hotspots/js/image-map-pro-editor.js",
						"~/App_Plugins/Hotspots/BackOffice/Hotspots/js/image-map-pro-editor-content.js",
						"~/App_Plugins/Hotspots/BackOffice/Hotspots/js/image-map-pro-editor-local-storage.js",
						"~/App_Plugins/Hotspots/BackOffice/Hotspots/js/image-map-pro-editor-init-jquery.js",
						"~/App_Plugins/Hotspots/BackOffice/Hotspots/js/image-map-pro.js"
					])
					.then(function () {
						// dependencies loaded.
						if (scope.data.Data) {
							try {
								hotspot = JSON.parse(scope.data.Data)
							}
							catch (ex) {
								console.log(ex);
								hotspot = undefined;
							}
							
						}
						if (!initDone) {
							initMap();
						}
					
						scope.$watch("data.Data", function (newValue, oldValue) {
							if (newValue == oldValue) {
								return;
							}

							if (newValue) {
								// Validate JSON
								try {
									hotspot = JSON.parse(newValue);
								} catch (err) {
									console.log('error decoding JSON!');
								}

								if (hotspot === undefined) {
									// Show error text
									$('#wcp-editor-import-error').show();
								} else {
									// No error
									$('#wcp-editor-import-error').hide();

									if (!initDone) {
										initMap();
									}
									else {
										// Fire event
										$.wcpEditorEventImportedJSON(hotspot);

										setTimeout(initEvents, 200);
									}

								}
							}
						});

					}, function (er) {
						alert(er);

					});

			}, controller: ['$scope', function ($scope) {

			}]
		}
	}]);
	
	// ETC.HotspotsPickerController (property editor)
	angular.module("umbraco").controller('ETC.HotspotsPickerController', ['$scope', '$http', 'dialogService', function HotSpotController($scope, $http, dialogService) {

		

		//setup scope vars
		//$scope.model.value =  "the eagel has landed";
		// var a = $scope;

		//$scope.status = { selectedId: 0 };

		//if ($scope.model && $scope.model.value && $scope.model.value.Id) {
		//	$scope.status.selectedId = $scope.model.value.Id;
		//}

		//$scope.$watch('status.selectedId', function (newValue, oldValue) {
		//	if (newValue != oldValue) {

		//		$scope.model.value = newValue;

		//		//for (var a in $scope.hotspots) {
		//		//	if ($scope.hotspots[a].Id == newValue) {
		//		//		$scope.model.value = $scope.hotspots[a];
		//		//		break;
		//		//	}
		//		//}
		//	}
		//});

		// todo, create factory like this : https://our.umbraco.com/documentation/Tutorials/creating-a-property-editor/part-4
		$http.get('backoffice/Hotspots/HotspotBackOffice/GetAllHotspots/', {
			cache: false
		}).then(function (response) {
			$scope.hotspots = response.data;
		}, function (error) {
			notificationsService.error("Fout bij het laden van de hotspots", error);
		});


	}]);



})();
