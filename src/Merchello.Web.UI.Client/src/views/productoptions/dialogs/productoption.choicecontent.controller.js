angular.module('merchello').controller('Merchello.ProductOption.Dialogs.ProductOptionChoiceContentController',
    ['$scope', '$timeout', 'editorState', 'merchelloTabsFactory', 'contentResource', 'productOptionResource',
        'detachedContentResource', 'detachedContentHelper',
    function($scope, $timeout, editorState, merchelloTabsFactory, contentResource, productOptionResource,
             detachedContentResource, detachedContentHelper) {

        $scope.currentTabs = undefined;
        $scope.tabs = [];
        $scope.ready = false;

        var umbracoTabs = [];

        var editor = {
            detachedContentType: null,
            scaffold: null,
            ready: function(dtc) {
                return this.detachedContentType !== null && dtc !== null && this.detachedContentType.key === dtc.key;
            }
        };


        $scope.save = function() {

            console.info('Save attribute content');
            /*
            var args = {
                saveMethod: productOptionResource.saveProductContent,
                content: product,
                scope: $scope,
                statusMessage: 'Saving...'
            };
            */

            //detachedContentHelper.detachedContentPerformSave(args);
        }

        function init() {
            editor.detachedContentType = $scope.dialogData.contentType;

            $scope.tabs = merchelloTabsFactory.createDefault();

            contentResource.getScaffold(-20, editor.detachedContentType.umbContentType.alias).then(function(scaffold) {
                editor.scaffold = scaffold;
                filterTabs(scaffold);
                fillValues();
                if (umbracoTabs.length > 0) {
                    switchTab(umbracoTabs[0]);
                }
                $scope.ready = true;
            });
        }


        function filterTabs(scaffold) {
            $scope.contentTabs = _.filter(scaffold.tabs, function(t) { return t.id !== 0 });
            if ($scope.contentTabs.length > 0) {
                angular.forEach($scope.contentTabs, function(ct) {
                    $scope.tabs.addActionTab(ct.id, ct.label, switchTab);
                    umbracoTabs.push(ct.id);
                });
            }
        }

        function fillValues() {

            if ($scope.contentTabs.length > 0) {
                angular.forEach($scope.contentTabs, function(ct) {
                    angular.forEach(ct.properties, function(p) {
                        var stored = $scope.dialogData.choice.detachedDataValues.getValue(p.alias);
                        if (stored !== '') {
                            try {
                                p.value = angular.fromJson(stored);
                            }
                            catch (e) {
                                // Hack fix for some property editors
                                p.value = '';
                            }
                        }
                    });
                });
            }
        }

        function switchTab(id) {
            var exists = _.find(umbracoTabs, function(ut) {
                return ut === id;
            });
            if (exists !== undefined) {
                var fnd = _.find($scope.contentTabs, function (ct) {
                    return ct.id === id;
                });
                $scope.currentTab = fnd;
                $scope.tabs.setActive(id);
            }
        }


        init();

}]);
