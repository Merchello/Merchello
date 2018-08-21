'use strict';
(function () {


	//register the controller
	angular.module("umbraco").controller('VWA.SpiderController', ['$scope', '$http', '$timeout', '$routeParams', '$location', 'appState', 'navigationService', 'notificationsService', function SpiderController($scope, $http, $timeout, $routeParams, $location, appState, navigationService, notificationsService) {


		//setup scope vars
		$scope.page = {};
		$scope.page.loading = false;
		$scope.page.nameLocked = false;
		$scope.page.menu = {};
		$scope.page.menu.currentSection = appState.getSectionState("currentSection");
		$scope.page.menu.currentNode = null;

		//set a property on the scope equal to the current route id
		$scope.id = $routeParams.id;
		//$scope.model = $scope.model || {};

		// Data eigenschap, spider classes van server
		$scope.Data = {};

		$scope.Tabs = [{ id: 0, label: 'Algemeen', alias: 'Algemeen' }, { id: 1, label: 'Vaste mapping', alias: 'Vaste mapping' }
			, { id: 2, label: 'Property mapping', alias: 'Property mapping' }
			, { id: 3, label: 'Uitsluit mapping', alias: 'Uitsluit mapping' }		];

		$scope.actionInProgress = false;
		$scope.bulkStatus = '';

		$scope.Data.Name = '';
		$scope.Data.Description = '';
		$scope.Data.SoldSpecID = "0";
		$scope.Data.DetailLinkRuleSpec = [];

		$scope.Data.RootNodeProperty = null;
		$scope.SpiderSites = [];

		// Data inlezen vanaf server
		$scope.GetSpider = function (spiderID) {

			// Alleen als er een site gekozen is.
			if ($scope.SpiderSiteID <= 0) {
				return;
			}

			$http.get('backoffice/ETCConnector/Spider/GetProperties/' + spiderID, {
				cache: false
			}).then(function (response) {

				// todo, jsonspecs, kwam voorheen met de route mee.
				$scope.Specs = response.data;

				$http.get('backoffice/ETCConnector/Spider/GetSite/' + spiderID, {
					cache: false
				}).then(function (response) {
					// todo, jsonspecs, kwam voorheen met de route mee.
					// $scope.Specs = jsonspecs;

					//for (var a in response.data.SpiderSpecs) {
					//    response.data.SpiderSpecs[a].Rule.SpecID = response.data.SpiderSpecs[a].Rule.SpecID.toString();
					//}

					$scope.Data = response.data;

					CompleteData($scope);


					//           $scope.Data.RootNodeProperty =
					//{
					//    "label": "Type kas", "description": "Wat wordt het model van de kas",
					//    "view": "contentpicker",
					//    "config":
					//        {
					//            "multiPicker": "1",
					//            "showOpenButton": "0",
					//            "showEditButton": "0",
					//            "showPathOnHover": "0", "idType": "int",
					//            "startNode": {
					//                "type": "content", "id": 1925
					//            }, "filter": null,
					//            "minNumber": null,
					//            "maxNumber": null
					//        }, "hideLabel": false,
					//    "validation": {
					//        "mandatory": false,
					//        "pattern": null
					//    },
					//    "id": 8232,
					//    "value": "1926,1927,1928",
					//    "alias": "modelKas",
					//    "editor": "Umbraco.MultiNodeTreePicker"
					//};



				}, function (error) {
					notificationsService.error("Fout bij het laden", error);
				});


			}, function (error) {
				notificationsService.error("Fout bij het laden", error);

			});



		}

		$scope.DoSpider = function (ignoreProperties) {
			var url = ignoreProperties ? "backoffice/ETCConnector/Spider/GetStartSpiderSkipProperties/" : "backoffice/ETCConnector/Spider/GetStartSpider/";

			$scope.actionInProgress = true;
			$scope.bulkStatus = 'Bezig met spideren';

			$http.get(url + $scope.Data.SpiderSiteID, {
				cache: false
			}).then(function (response) {
				$scope.actionInProgress = false;

				notificationsService.add({ 'headline': 'Success', 'message': 'Document succesvol processed', type: 'success', sticky: true });

				//notificationsService.success("Document gespiderd", response.data);
				// todo, jsonspecs, kwam voorheen met de route mee.
				// $scope.Specs = jsonspecs;

				//for (var a in response.data.SpiderSpecs) {
				//    response.data.SpiderSpecs[a].Rule.SpecID = response.data.SpiderSpecs[a].Rule.SpecID.toString();
				//}

			}, function (error) {
				$scope.actionInProgress = false;
				notificationsService.add({ 'headline': 'Failure', 'message': 'Error processing ' + error.data, type: 'error', sticky: true });
			});

		}

		$timeout(function () {
			navigationService.syncTree({ tree: "ETCConnector", path: $scope.id }).then(function (syncArgs) {
				$scope.page.menu.currentNode = syncArgs.node;
			});
		}, 100);

		function CompleteData($scope) {
			$scope.Data.IDRuleSpec = { "Fixed": true, "Rule": $scope.Data.IDRule };
			$scope.Data.DetailLinkRuleSpec = [];

			for (var ruleID in $scope.Data.DetailLinkRule) {
				$scope.Data.DetailLinkRuleSpec.push({ "Fixed": true, "Rule": $scope.Data.DetailLinkRule[ruleID] });
			}

			$scope.Data.PagerRuleSpec = { "Fixed": true, "Rule": $scope.Data.PagerRule };
			$scope.Data.CategoryRuleSpec = { "Fixed": true, "Rule": $scope.Data.CategoryRule };
			$scope.Data.SoldSpecID = $scope.Data.SoldSpecID.toString();
		}

		// De spiderssites ophalen
		//$scope.GetSpiderSites = function () {
		//    $http.get('../Content/sys/update/spiderdata.ashx?cmd=getsites').then(function (response) {
		//        $scope.SpiderSites = response.data;

		//        if ($scope.SpiderSiteID == 0 && $scope.SpiderSites.length) {
		//            $scope.SpiderSiteID = $scope.SpiderSites[0].Key;
		//        }

		//    }, function (error) {
		//        alert(error);
		//    });
		//}

		// Regels opslaan op de server
		$scope.Commit = function (a, b) {

			if (!$scope.Data.Name || !$scope.Data.Name.length) {
				notificationsService.error("Niet opgeslagen", 'naam is een verplicht veld');
				return;
			}
			
			if (!$scope.Data.SpiderDatabase && (!$scope.Data.ListSelector || !$scope.Data.ListSelector.length)) {
				notificationsService.error("Niet opgeslagen", 'Object selector is een verplicht veld');
				return;
			}

			for (var specID in $scope.Data.SpiderSpecs) {
				$scope.Data.SpiderSpecs[specID].Rule.SpecID *= 1;
			}

			$http.post('backoffice/ETCConnector/Spider/Save/', $scope.Data).then(function (res) {
				if (!res.data.ok) {
					notificationsService.error("Niet opgeslagen", res.data.message);
					return;
				}

				$scope.Data = res.data.data;
				$scope.contentForm.$dirty = false;

				CompleteData($scope);

				if ($scope.SpiderSiteID == 0) {
					$location.path("vwa-spider/ETCConnector/edit/" + $scope.Data.SpiderSiteID.toString());
				}
				$scope.SpiderSiteID = $scope.Data.SpiderSiteID.toString();

				notificationsService.success("Document Published", "Gelukt, de wijzigingen zijn succesvol opgelagen");


			}, function (err) {
				notificationsService.error("Niet opgeslagen", err);
			});
			return false;
		}

		// Delete eigenschap zetten, zodat deze bij een commit actie op de server verwijderd kunnen worden.,
		$scope.Delete = function (spec, index) {
			spec.Rule.Deleted = true;
		}

		// Filter, alleen regels tonen die niet verwijderd zijn, en die geen exclude regel zijn
		$scope.ruleFilter = function (spec) {
			return !spec.Rule.Deleted && !spec.Rule.Exclude;
		}

		// Filter, alleen regels tonen die niet verwijderd zijn, en die wel exclude regel zijn.
		$scope.ruleExcludeFilter = function (spec) {
			return !spec.Rule.Deleted && spec.Rule.Exclude;
		}


		// SpidersiteID in de gaten houden, bij wijzigingen de data opnieuwe ophalen
		//$scope.$watch('SpiderSiteID', function (newValue, oldValue) {
		//    if (newValue != oldValue)
		//        $scope.GetSpider(newValue);
		//});

		// Regel toevoegen, lege regel ophalen op server en toevoegen aan collectie, angulars databinding doet de rest.
		$scope.Add = function (exclude) {

			$http.get('backoffice/ETCConnector/Spider/GetBlankRule/' + $scope.SpiderSiteID).then(function (response) {
				response.data.Rule.Exclude = !!(exclude);
				$scope.Data.SpiderSpecs.push(response.data);
			}, function (error) {
				notificationsService.error("Fout bij het laden regel", error);
			});

		}

		$scope.AddDetailLink = function () {
			$http.get('backoffice/ETCConnector/Spider/GetBlankDetailRule/' + $scope.SpiderSiteID).then(function (response) {
				$scope.Data.DetailLinkRule.push(response.data);
				$scope.Data.DetailLinkRuleSpec.push({ "Fixed": true, "Rule": $scope.Data.DetailLinkRule[$scope.Data.DetailLinkRule.length - 1].Rule });
			}, function (error) {
				notificationsService.error("Fout bij het laden regel", error);
			});
		}

		// Lege Mapping regel toevoegen, angulars databinding doet de rest.
		$scope.addMapping = function (spiderSpec) {
			spiderSpec.Rule.Mapping.push({ "First": "", "Seconde": "" });
		}


		// Regel controleren, op  server worden 3 sites gespirder op de opgegeven regel.
		$scope.check = function (spiderSpec) {
			notificationsService.error("TOOD ", "Not implemented yet");
			return false;

			//top.HourGlassShow("Bezig met ophalen gegevens, moment aub");

			//$http.get('../Content/sys/update/spiderdata.ashx?spidersite=' + $scope.SpiderSiteID + "&cmd=checkrule&ruleID=" + spiderSpec.Rule.RuleID).then(function (response) {

			//    spiderSpec.Examples = response.data;


			//}, function (error) {
			//    notificationsService.error("Fout bij het laden", error);

			//});
		}


		// SpidersiteID zetten, ID wordt gezet boveningestelde watch roept GetSpiderSite aan.
		$scope.SpiderSiteID = $scope.id;
		$scope.GetSpider($scope.SpiderSiteID);

	}]).directive('spiderRule', function () {

		return {
			templateUrl: '/app_plugins/ETCConnector/Directives/spider-rule.html',
			restrict: 'E',
			transclude: true,
			scope: {
				Specs: '=specs',
				SpiderSpec: '=spiderspec'
			}, link: function (scope, element, attr) {
				var a = scope;
			}, controller: ['$scope', function ($scope) {
				// var SpiderSpec = $scope.SpiderSpec;

				//$scope.$watch('SpiderSpec.SpecID', function (newValue, oldValue) {
				//    if (newValue != oldValue)
				//        SpiderSpec.SpecID = SpiderSpec.SpecID * 1;
				//});

				$scope.Delete = function (spec, index) {
					spec.Rule.Deleted = true;

				}

			}]

		}
	}).directive('databaseRule', function () {

		return {
			templateUrl: '/app_plugins/ETCConnector/Directives/database-rule.html',
			restrict: 'E',
			transclude: true,
			scope: {
				Specs: '=specs',
				SpiderSpec: '=spiderspec'
			}, link: function (scope, element, attr) {
				var a = scope;
			}, controller: ['$scope', function ($scope) {
				// var SpiderSpec = $scope.SpiderSpec;

				//$scope.$watch('SpiderSpec.SpecID', function (newValue, oldValue) {
				//    if (newValue != oldValue)
				//        SpiderSpec.SpecID = SpiderSpec.SpecID * 1;
				//});
				


				$scope.Delete = function (spec, index) {
					spec.Rule.Deleted = true;

				}

			}]

		}
	});




	//adds the resource to umbraco.resources module:
	angular.module('umbraco.resources').factory('spiderSitesResource',
		function ($q, $http) {
			//the factory object returned
			return {
				//this cals the Api Controller we setup earlier
				getAll: function () {
					return $http.get("backoffice/Spider/Spider/getall");
				},
				doSpider: function () {
					return $http.get("backoffice/Spider/Spider/Spider");
				}
			};
		}
	);


})();
