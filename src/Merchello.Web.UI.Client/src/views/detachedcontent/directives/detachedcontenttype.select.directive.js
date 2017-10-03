angular.module('merchello.directives').directive('detachedContentTypeSelect',
        function(detachedContentResource, localizationService, detachedContentTypeDisplayBuilder) {
        return {
            restrict: 'E',
            replace: true,
            terminal: false,

            scope: {
                entityType: '@',
                selectedContentType: '=',
                setSelectedContentTypeKey: '=?',
                showSave: '=?',
                save: '&'
            },
            template: '<div class="detached-content-select">' +
            '<div data-ng-show="detachedContentTypes.length > 0">' +
            '<label><localize key="merchelloDetachedContent_productContentTypes" /></label>' +
            '<select data-ng-model="selectedContentType" data-ng-options="ct.name for ct in detachedContentTypes track by ct.key" data-ng-show="loaded" class="form-control umb-editor">' +
            '<option value="">{{ noSelection }}</option>' +
            '</select>' +
            ' <merchello-save-icon show-save="showSave" do-save="save()"></merchello-save-icon>' +
            '</div>' +
                '<div data-ng-hide="detachedContentTypes.length > 0 && loaded" style="text-align: center">' +
                '<localize key="merchelloDetachedContent_noDetachedContentTypes" />' +
                '</div>' +
            '</div>',
            link: function(scope, elm, attr) {

                scope.loaded = false;
                scope.detachedContentTypes = [];
                scope.noSelection = '';

                function init() {
                    localizationService.localize('merchelloDetachedContent_selectContentType').then(function(value) {
                        scope.noSelection = value;
                        loadDetachedContentTypes();
                    });
                }

                function loadDetachedContentTypes() {
                    detachedContentResource.getDetachedContentTypeByEntityType(scope.entityType).then(function(results) {
                        scope.detachedContentTypes = detachedContentTypeDisplayBuilder.transform(results);

                        if (('setSelectedContentTypeKey' in attr)) {
                            var fnd = _.find(scope.detachedContentTypes, function(dtc) {
                               return dtc.key === scope.setSelectedContentTypeKey;
                            });

                            if (fnd) {
                                scope.selectedContentType = fnd;
                            }
                        }

                        scope.loaded = true;

                    });
                }

                // initialize the directive
                init();
            }
        };

});
