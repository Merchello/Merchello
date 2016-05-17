angular.module('merchello.directives').directive('notificationRazorTemplateSelection',
    ['$q', 'assetsService', 'dialogService', 'localizationService', 'eventsService', 'vieweditorResource', 'pluginViewEditorContentBuilder',
    function($q, assetsService, dialogService, localizationService, eventsService, vieweditorResource, pluginViewEditorContentBuilder) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                message: '=',
                monitor: '=',
                ready: '=',
                refresh: '&',
                save: '&'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/Directives/notificationrazortemplate.selection.tpl.html',
            link: function(scope, elm, attr) {

                var saveEventName = 'notification.message.saved';

                scope.loaded = false;
                scope.templates = [];
                scope.selectedTemplate = '';
                scope.fileName = '';
                scope.showCreateButton = false;

                scope.editorOptions = {
                    lineNumbers: true,
                    mode: 'javascript',
                    gutters: ['CodeMirror-lint-markers'],
                    lint: true
                };

                eventsService.on(saveEventName, saveRazorTemplate);

                scope.$watch('ready', function(newVal, oldVal) {
                    if (newVal) {
                        init();
                    }
                });

                scope.checkFileName = function() {
                    var matched = findMatchingTemplate(scope.fileName);
                    if (matched !== undefined) {
                        scope.selectedTemplate = matched;
                        scope.showCreateButton = false;
                    } else {
                        if (scope.selectedTemplate.fileName !== '') {
                            scope.selectedTemplate = scope.templates[0];
                            scope.showCreateButton = true;
                        }
                    }
                };

                scope.setFileName = function() {
                    if (scope.selectedTemplate === null) {
                        scope.selectedTemplate = scope.templates[0];
                    }

                    if (scope.selectedTemplate.fileName === '') {
                        scope.showCreateButton = true;
                        scope.message.bodyText = '';
                        scope.viewBody = '';
                        scope.fileName = getFileNameFromSubject();

                    } else {
                        scope.showCreateButton = false;
                        scope.message.bodyText = scope.selectedTemplate.virtualPath + scope.selectedTemplate.fileName;
                        scope.viewBody = scope.selectedTemplate.viewBody;
                        scope.fileName = scope.selectedTemplate.fileName;
                    }
                };

                scope.createView = function() {
                    var viewData = scope.selectedTemplate;
                    viewData.fileName = scope.fileName;

                    viewData.modelTypeName = scope.monitor.modelTypeName;
                    vieweditorResource.addNewView(viewData).then(function(result) {
                        init();
                        scope.showCreateButton = false;
                    });
                }
                
                function init() {

                    $q.all([
                        vieweditorResource.getAllNotificationViews(),
                        localizationService.localize('general_create')

                    ]).then(function(data) {


                        var empty = pluginViewEditorContentBuilder.createDefault();
                        empty.label = '-- ' + data[1] + ' --';

                        data[0].unshift(empty);

                        scope.templates = data[0];
                        scope.selectedTemplate = _.find(scope.templates, function(t) {
                            return t.virtualPath + t.fileName === scope.message.bodyText;
                        });

                        setDefaultFileName();
                    });
                }

                function setDefaultFileName() {
                    var matched = findByBodyText();
                    if (matched.fileName === '') {
                        var fileName = getFileNameFromSubject();
                        if (checkExists(fileName)) {
                            scope.fileName = fileName;
                            scope.selectedTemplate = findMatchingTemplate(fileName);
                            scope.message.bodyText = scope.selectedTemplate.virtualPath + scope.selectedTemplate.fileName;
                        } else {
                            scope.fileName = fileName;
                            scope.showCreateButton = true;
                        }
                    } else {
                        scope.selectedTemplate = matched;
                        scope.fileName = matched.fileName;
                        scope.message.bodyText = scope.selectedTemplate.virtualPath + scope.selectedTemplate.fileName;
                    }
                    scope.loaded = true;
                }

                function getFileNameFromSubject() {
                    return scope.message.name.replace(/\W/g, '') + '.cshtml';
                }

                function checkExists(fileName) {
                    return findMatchingTemplate(fileName) !== undefined;
                }

                function findMatchingTemplate(fileName) {
                   return _.find(scope.templates, function(t) { return t.fileName === fileName });
                }

                function findByBodyText() {
                    return _.find(scope.templates, function(t) {
                       return t.virtualPath + t.fileName === scope.message.bodyText;
                    });
                }

                function saveRazorTemplate(name, args) {
                    if (scope.selectedTemplate.fileName !== '') {
                        vieweditorResource.saveView(scope.selectedTemplate);
                    }
                }
            }
        };
}]);
