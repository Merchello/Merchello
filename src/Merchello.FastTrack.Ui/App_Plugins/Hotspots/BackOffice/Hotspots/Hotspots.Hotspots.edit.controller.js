'use strict';
(function () {

	//register the controller
	angular.module("umbraco").controller('ETC.HotspotsController', ['$scope', '$http', '$timeout', '$routeParams', '$location', 'appState', 'navigationService', 'notificationsService', 'dialogService', '$attrs', function HotSpotController($scope, $http, $timeout, $routeParams, $location, appState, navigationService, notificationsService, dialogService, $attrs) {

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
		//$scope.model = $scope.model || {};

		$scope.data = {
			"Id" : $routeParams.id, "Data" : '', Name :''
		};
			
		// Data inlezen vanaf server
		$scope.GetHotspotData = function () {

			$http.get('backoffice/Hotspots/HotspotBackOffice/GetHotspotData/' + $scope.id, {
				cache: false
			}).then(function (response) {

				// todo, jsonspecs, kwam voorheen met de route mee.
				$scope.data = response.data;

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
			
			$http.post('backoffice/Hotspots/HotSpotBackOffice/SaveHotspotData/', { "Id" : $scope.id, "Data" : data, "Name": $scope.data.Name } ).then(function (res) {
							
				var newId = res.data.Id;
				notificationsService.success("De wijzigingen zijn succesvol opgelagen");

				$scope.contentForm.$dirty = false;

				if (newId != $scope.id) {
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
				
				navigationService.syncTree({ tree: "Hotspots", path: newId.toString(), forceReload: true, activate : true }).then(function (syncArgs) {
					$scope.page.menu.currentNode = syncArgs.node;
				});
				
			}, function (err) {
				notificationsService.error("Niet verwijderd", err);
				dialogService.closeAll();
			});


			return false;

		}

		$scope.GetHotspotData();

		if (!$scope.dialog) {
			$timeout(function () {
				navigationService.syncTree({ tree: "Hotspots", path: $scope.id.toString() }).then(function (syncArgs) {
					$scope.page.menu.currentNode = syncArgs.node;
				});
			}, 100);

		}
		
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
							$.image_map_pro_init_editor(hotspot, $.WCPEditorSettings);
						}

						initDone = true;

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
										$.image_map_pro_init_editor(hotspot, $.WCPEditorSettings);
										initDone = true;
									}
									else {
										// Fire event
										$.wcpEditorEventImportedJSON(hotspot);
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
