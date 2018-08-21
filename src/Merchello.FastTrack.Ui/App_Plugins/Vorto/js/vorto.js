angular.module("umbraco").controller("Our.Umbraco.PropertyEditors.Vorto.vortoEditor", [
    '$scope',
    '$rootScope',
    'appState',
    'editorState',
    'formHelper',
    'umbPropEditorHelper',
    'Our.Umbraco.Resources.Vorto.vortoResources',
    'Our.Umbraco.Services.Vorto.vortoLocalStorageService',
    function ($scope, $rootScope, appState, editorState, formHelper, umbPropEditorHelper, vortoResources, localStorageService) {
			
        // Get node context
        // DTGE/NC expose the context on the scope
        // to avoid overwriting the editorState
        // so check for a context on the scope first
        var parentScope = $scope;
        var nodeContext = undefined;
        while (!nodeContext && parentScope.$id !== $rootScope.$id) {
            parentScope = parentScope.$parent;
            nodeContext = parentScope.nodeContext;
        }
        if (!nodeContext) {
            nodeContext = editorState.current;
        }

        $scope.languages = [];
        $scope.pinnedLanguages = [];
        $scope.filledInLanguages = [];
        $scope.$rootScope = $rootScope;

        $scope.currentLanguage = undefined;
        $scope.activeLanguage = undefined;
        $scope.realActiveLanguage = undefined;

        var cookieUnsyncedProps = localStorageService.get("vortoUnsyncedProps", []);
        $scope.sync = !_.contains(cookieUnsyncedProps, $scope.model.id);

        $scope.model.hideLabel = $scope.model.config.hideLabel == 1;
        $scope.model.showCheckMark = $scope.model.config.showFilledLanguages == 1;

        $scope.property = {
            config: {},
            view: ""
        };

        if (!angular.isObject($scope.model.value))
            $scope.model.value = undefined;

        $scope.model.value = $scope.model.value || {
            values: {},
            dtdGuid: "00000000-0000-0000-0000-000000000000"
        };

        // Fix mixed spelling of dtdGuid
        if ($scope.model.value.dtdguid) {
            $scope.model.value.dtdGuid = $scope.model.value.dtdguid;
            delete $scope.model.value.dtdguid;
        }

        $scope.setCurrentLanguage = function (language, dontBroadcast) {

            if (!dontBroadcast && $scope.sync) {

                // Update cookie
                localStorageService.set('vortoCurrentLanguage', language.isoCode);
                localStorageService.set('vortoActiveLanguage', language.isoCode);

                // Broadcast a resync
                $rootScope.$broadcast("reSync");

            } else {
                $scope.currentLanguage = $scope.activeLanguage = language;
            }

            // Close the menu (Not really the right way to do it :))
            $("#vorto-" + $scope.model.id)
                .find(".vorto-tabs__item--menu")
                .removeClass("active")
                .find(".vorto-menu").hide();
        };

        $scope.setActiveLanguage = function (language, dontBroadcast) {

            if (!dontBroadcast && $scope.sync) {

                // Update cookie
                localStorageService.set('vortoActiveLanguage', language.isoCode);

                // Broadcast a resync
                $rootScope.$broadcast("reSync");

            } else {
                $scope.activeLanguage = language;
            }
        };

        $scope.pinLanguage = function (language) {
            if ($scope.sync) {

                // Update cookie
                var cookiePinnedLanguages = localStorageService.get('vortoPinnedLanguages', []);
                cookiePinnedLanguages.push(language.isoCode);
                cookiePinnedLanguages = _.uniq(cookiePinnedLanguages);
                localStorageService.set('vortoPinnedLanguages', cookiePinnedLanguages);

                // Broadcast a resync
                $rootScope.$broadcast("reSync");

            } else {
                $scope.pinnedLanguages.push(language);
            }
        };

        $scope.unpinLanguage = function (language) {
            if ($scope.sync) {

                // Update cookie
                var cookiePinnedLanguages = localStorageService.get('vortoPinnedLanguages', []);
                cookiePinnedLanguages = _.reject(cookiePinnedLanguages, function (itm) {
                    return itm == language.isoCode;
                });
                localStorageService.set('vortoPinnedLanguages', cookiePinnedLanguages);

                // Broadcast a resync
                $rootScope.$broadcast("reSync");

            } else {
                $scope.pinnedLanguages = _.reject($scope.pinnedLanguages, function (itm) {
                    return itm.isoCode == language.isoCode;
                });
            }
        };

        $scope.isFilledIn = function (language) {
            if (!$scope.model.showCheckMark) return;
            if (language == undefined) return;
            return _.find($scope.filledInLanguages, function (itm) {
                return itm.isoCode == language.isoCode;
            });
        };

        $scope.isPinnable = function (language) {
            return $scope.currentLanguage.isoCode != language.isoCode && !_.find($scope.pinnedLanguages, function (itm) {
                return itm.isoCode == language.isoCode;
            });
        };

        var unsubReSync = $scope.$on("reSync", function (evt) {
            reSync();
        });

        $scope.$on("$destroy", function() {
            unsubReSync();
        });

        $scope.$watchCollection("pinnedLanguages", function (pinnedLanguages) {

            var activePinnedLanguage = _.find(pinnedLanguages, function (itm) {
                return itm.isoCode == $scope.activeLanguage.isoCode;
            });
            if (!activePinnedLanguage) {
                $scope.activeLanguage = $scope.currentLanguage;
            }

        });

        $scope.$watch("currentLanguage", function (language) {

            // If same as a pinned language, remove the pinned language
            var pinned = _.find($scope.pinnedLanguages, function (itm) {
                return itm.isoCode == language.isoCode;
            });
            if (pinned) {
                $scope.unpinLanguage(pinned);
            }

        });

        $scope.$watch("activeLanguage", function(newLanguage) {
            if (newLanguage && $scope.realActiveLanguage && $scope.realActiveLanguage.isoCode !== newLanguage.isoCode) {
                $scope.$broadcast("vortoSyncLanguageValue", { language: $scope.realActiveLanguage.isoCode });
            }
            $scope.realActiveLanguage = $scope.activeLanguage;

            // When the language changes, check for filled in languages again
            // as the editor may have filled the previous language in
            detectFilledInLanguages();
        });

        $scope.$watch("sync", function (shouldSync) {
            var tmp;
            if (shouldSync) {
                tmp = localStorageService.get('vortoUnsyncedProps', []);
                tmp = _.reject(tmp, function (itm) {
                    return itm == $scope.model.id;
                });
                localStorageService.set('vortoUnsyncedProps', tmp);
                reSync();
            } else {
                tmp = localStorageService.get('vortoUnsyncedProps', []);
                tmp.push($scope.model.id);
                tmp = _.uniq(tmp);
                localStorageService.set('vortoUnsyncedProps', tmp);
            }
        });

        var unsubscribe = $scope.$on("formSubmitting", function (ev, args) {
            $scope.$broadcast("vortoSyncLanguageValue", { language: $scope.realActiveLanguage.isoCode });
            validateProperty();
            if ($scope.vortoForm.$valid) {
                // Strip out empty entries
                var cleanValue = {};
                _.each($scope.languages, function(language) {
                    if ($scope.model.value.values[language.isoCode] && JSON.stringify($scope.model.value.values[language.isoCode]).length > 0) {
                        cleanValue[language.isoCode] = $scope.model.value.values[language.isoCode];
                    }
                });
                $scope.model.value.values = !_.isEmpty(cleanValue) ? cleanValue : undefined;
            }
        });

        $scope.$on('$destroy', function () {
            unsubscribe();
        });

        var reSync = function () {
            if ($scope.sync) {

                // Handle current language change
                var cookieCurrentLanguage = localStorageService.get('vortoCurrentLanguage');
                var currentLanguage = _.find($scope.languages, function (itm) {
                    return itm.isoCode == cookieCurrentLanguage;
                }) || $scope.currentLanguage;

                if (!$scope.currentLanguage || $scope.currentLanguage.isoCode != currentLanguage) {
                    $scope.setCurrentLanguage(currentLanguage, true);
                }

                // Handle active language change
                var cookieActiveLanguage = localStorageService.get('vortoActiveLanguage');
                var activeLanguage = _.find($scope.languages, function (itm) {
                    return itm.isoCode == cookieActiveLanguage;
                }) || $scope.activeLanguage;

                if (!$scope.activeLanguage || $scope.activeLanguage.isoCode != activeLanguage) {
                    $scope.setActiveLanguage(activeLanguage, true);
                }

                // Handle pinned language change
                var cookiePinnedLanguages = localStorageService.get('vortoPinnedLanguages', []);
                var pinnedLanguages = _.filter($scope.languages, function (itm) {
                    return _.contains(cookiePinnedLanguages, itm.isoCode);
                });

                $scope.pinnedLanguages = pinnedLanguages;

            }
        }

        var detectFilledInLanguages = function () {
            $scope.filledInLanguages = [];
            _.each($scope.languages, function (language) {
                if (language.isoCode in $scope.model.value.values &&
                    $scope.model.value.values[language.isoCode]) {
                    $scope.filledInLanguages.push(language);
                }
            });
        }

        var validateProperty = function () {
            // Validate value changes
            if ($scope.model.validation.mandatory) {

                var mandatoryBehaviour = $scope.model.config.mandatoryBehaviour;
                var primaryLanguage = $scope.model.config.primaryLanguage;

                if (mandatoryBehaviour == "primary" && primaryLanguage == undefined) {
                    mandatoryBehaviour = "ignore";
                }

                //TODO: Might be better if we could get the inner control to validate this?

                var isValid = true;
                switch (mandatoryBehaviour) {
                    case "all":
                        _.each($scope.languages, function (language) {
                            if (!(language.isoCode in $scope.model.value.values) ||
                                !$scope.model.value.values[language.isoCode]) {
                                isValid = false;
                                return;
                            }
                        });
                        break;
                    case "any":
                        isValid = false;
                        _.each($scope.languages, function (language) {
                            if (language.isoCode in $scope.model.value.values &&
                                $scope.model.value.values[language.isoCode]) {
                                isValid = true;
                                return;
                            }
                        });
                        break;
                    case "primary":
                        if (primaryLanguage in $scope.model.value.values
                            && $scope.model.value.values[primaryLanguage]) {
                            isValid = true;
                        } else {
                            isValid = false;
                        }
                        break;
                }

                $scope.vortoForm.$setValidity("required", isValid);

                // TODO: Regex
            }
        }

        var getCurrentSection = function() {
        	var currentSection = appState.getSectionState("currentSection");

            // The newer back office now shows a preview of property editors in the doc type editor
            // so the current section will always be settings. If we are in the settings section
            // then look for why type of content editor we are and set the current section accordingly.
            // NB: Member types is normally in the members section so that should actually work.
        	if (currentSection === "settings") {
        		if (window.location.hash.match(new RegExp("mediaTypes"))) {
        			currentSection = "media";
        		}
        		else if (window.location.hash.match(new RegExp("documentTypes"))) {
        			currentSection = "content";
        		}
        	}

	        return currentSection;
        }

        // Load the datatype
        vortoResources.getDataTypeById($scope.model.config.dataType.guid).then(function (dataType) {

            // Stash the config in scope for reuse
            $scope.property.config = dataType.preValues;

            // Get the view path
            $scope.property.viewPath = umbPropEditorHelper.getViewPath(dataType.view);

            // Get the property alias
            var propAlias = $scope.model.propertyAlias || $scope.model.alias;

        	// Get the content type alias
            var contentTypeAlias = nodeContext.contentTypeAlias || nodeContext.alias;

            // Work out what section we are in
			var currentSection = getCurrentSection();

        	// Get the current properties datatype
            vortoResources.getDataTypeByAlias(currentSection, contentTypeAlias, propAlias).then(function (dataType2) {

                $scope.model.value.dtdGuid = dataType2.guid;

                // Load the languages (this will trigger everything else to bind)
                vortoResources.getLanguages(currentSection, editorState.current.id, editorState.current.parentId, dataType2.guid)
                    .then(function (languages) {

                        $scope.languages = languages;

                        if (!$scope.model.value.values) {
                            $scope.model.value.values = {};
                        }

                        _.each($scope.languages, function (language) {
                            if (!$scope.model.value.values.hasOwnProperty(language.isoCode)) {
                                $scope.model.value.values[language.isoCode] = $scope.model.value.values[language.isoCode];
                            }
                        });

                        $scope.currentLanguage = $scope.activeLanguage = _.find(languages, function (itm) {
                            return itm.isDefault;
                        });

                        reSync();

                        validateProperty();

                        detectFilledInLanguages();
                    });
            });
        });


    }
]);

angular.module("umbraco").controller("Our.Umbraco.PreValueEditors.Vorto.propertyEditorPicker", [
    '$scope',
    'Our.Umbraco.Resources.Vorto.vortoResources',
    function ($scope, vortoResources) {

        $scope.model.dataTypes = [];
        $scope.model.value = $scope.model.value || {
            guid: "0cc0eba1-9960-42c9-bf9b-60e150b429ae",
            name: "Textstring",
            propertyEditorAlias: "Umbraco.Textbox"
        };

        vortoResources.getNonVortoDataTypes().then(function (data) {
            $scope.model.dataTypes = data;
        });

    }]
);

angular.module("umbraco").controller("Our.Umbraco.PreValueEditors.Vorto.languagePicker", [
    '$scope',
    'Our.Umbraco.Resources.Vorto.vortoResources',
    function ($scope, vortoResources) {

        $scope.model.languages = [];

        vortoResources.getInstalledLanguages().then(function (data) {
            $scope.model.languages = data;
        });

    }]
);

/* Directives */
angular.module("umbraco.directives").directive('vortoProperty',
    function ($compile, $http, umbPropEditorHelper, $timeout, $rootScope, $q) {

        var link = function (scope, element, attrs, ctrl) {
            scope[ctrl.$name] = ctrl;

            scope.model = {};

            // Some core property editors update the prevalues
            // but then fail to check them incase the config
            // is in the desired format, so to get round this
            // we give each instance a clone of the original
            // config so that changes made aren't remebered
            // between tab loads
            // bug here http://issues.umbraco.org/issue/U4-8266
            scope.model.config = angular.copy(scope.config);

            scope.model.alias = scope.propertyAlias + "." + scope.language;
            scope.model.value = scope.value.values ? scope.value.values[scope.language] : undefined;

            var unsubscribe = scope.$on("vortoSyncLanguageValue", function (ev, args) {
                if (args.language === scope.language) {
                    scope.$broadcast("formSubmitting", { scope: scope });
                    if (!scope.value.values)
                        scope.value.values = {};
                    scope.value.values[scope.language] = scope.model.value;
                }
            });

            scope.$on('$destroy', function () {
                unsubscribe();
            });
        };

        return {
            require: "^form",
            restrict: "E",
            rep1ace: true,
            link: link,
            template: '<div ng-include="propertyEditorView"></div>',
            scope: {
                propertyEditorView: '=view',
                config: '=',
                language: '=',
                propertyAlias: '=',
                value: '='
            }
        };
    });

/* Resources */
angular.module('umbraco.resources').factory('Our.Umbraco.Resources.Vorto.vortoResources',
    function ($q, $http, umbRequestHelper) {
        return {
            getNonVortoDataTypes: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.vorto.apiBaseUrl + "GetNonVortoDataTypes"),
                    'Failed to retrieve datatypes'
                );
            },
            getDataTypeById: function (id) {
                return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.vorto.apiBaseUrl + "GetDataTypeById?id=" + id),
                    'Failed to retrieve datatype'
                );
            },
            getDataTypeByAlias: function (contentType, contentTypeAlias, propertyAlias) {
                return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.vorto.apiBaseUrl + "GetDataTypeByAlias?contentType=" + contentType + "&contentTypeAlias=" + contentTypeAlias + "&propertyAlias=" + propertyAlias),
                    'Failed to retrieve datatype'
                );
            },
            getLanguages: function (section, id, parentId, dtdGuid) {
                return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.vorto.apiBaseUrl + "GetLanguages?section=" + section + "&id=" + id + "&parentId=" + parentId + "&dtdGuid=" + dtdGuid),
                    'Failed to retrieve languages'
                );
            },
            getInstalledLanguages: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get(Umbraco.Sys.ServerVariables.vorto.apiBaseUrl + "GetInstalledLanguages"),
                    'Failed to retrieve languages'
                );
            }
        };
    }
);

/* Services */
angular.module('umbraco.services').factory('Our.Umbraco.Services.Vorto.vortoLocalStorageService',
    function ($cookies) {

        var supportsLocalStorage = function () {
            try {
                return 'localStorage' in window && window['localStorage'] !== null;
            } catch (e) {
                return false;
            }
        }

        var stash = function (key, value) {
            if (supportsLocalStorage()) {
                localStorage.setItem(key, value);
            } else {
                $cookies[key] = value;
            }
        }

        var unstash = function (key) {
            if (supportsLocalStorage()) {
                return localStorage.getItem(key);
            } else {
                return $cookies[key];
            }
        }

        return {
            get: function (key, fallback) {
                var rawVal = unstash(key);
                if (!rawVal) return fallback;
                return JSON.parse(rawVal);
            },
            set: function (key, obj) {
                stash(key, JSON.stringify(obj));
            }
        };
    }
);

/* Directives */
angular.module('umbraco.directives').directive('jsonText', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attr, ngModel) {
            function into(input) {
                return JSON.parse(input);
            }
            function out(data) {
                return JSON.stringify(data);
            }
            ngModel.$parsers.push(into);
            ngModel.$formatters.push(out);

        }
    };
});

$(function () {

    var over = function () {
        var self = this;
        $(self).addClass("active").find(".vorto-menu").show().css('z-index', 9000);
    };

    var out = function () {
        var self = this;
        $(self).removeClass("active").find(".vorto-menu").hide().css('z-index', 0);
    };

    $("body").hoverIntent({
        over: over,
        out: out,
        interval: 10,
        timeout: 250,
        selector: ".vorto-tabs__item--menu"
    });

});

