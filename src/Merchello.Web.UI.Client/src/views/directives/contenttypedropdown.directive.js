angular.module('merchello.directives').directive('contentTypeDropDown',
    function(localizationService, eventsService, detachedContentResource, umbContentTypeDisplayBuilder) {
    return {
        restrict: "E",
        replace: true,
        scope: {
            selectedContentType: '=',
        },
        template:
        '<div class="control-group">' +
        '<label><localize key="merchelloDetachedContent_contentTypes" /></label>' +
        '<select class="span11" data-ng-model="selectedContentType" data-ng-options="ct.name for ct in contentTypes track by ct.key" data-ng-change="emitChanged()" data-ng-show="loaded">' +
            '<option value="">{{ noSelection }}</option>' +
        '</select>' +
        '</div>',
        link: function (scope, element, attrs, ctrl) {

            scope.loaded = false;
            scope.contentTypes = [];
            scope.noSelection = '';
            scope.emitChanged = emitChanged;

            var eventName = 'merchello.contenttypedropdown.changed';

            function init() {
                localizationService.localize('merchelloDetachedContent_selectContentType').then(function(value) {
                    scope.noSelection = value;
                    loadContentTypes();
                });
            }

            function loadContentTypes() {
                detachedContentResource.getContentTypes().then(function(results) {
                    scope.contentTypes = umbContentTypeDisplayBuilder.transform(results);
                    scope.loaded = true;
                });
            }

            function emitChanged() {
                // clone the arg first so it's immutable
                var value = angular.extend(umbContentTypeDisplayBuilder.createDefault(), scope.selectedContentType);
                eventsService.emit(eventName, value);
            }

            init();
        }
    };
});
