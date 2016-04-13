angular.module('merchello.directives').directive('merchelloNotesTable', [
    '$q', 'userService', 'localizationService', 'dialogService', 'noteResource', 'noteDisplayBuilder',
    function($q, userService, localizationService, dialogService, noteResource, noteDisplayBuilder) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                entityType: '=',
                notes: '=',
                delete: '&',
                save: '&',
                noTitle: '@?'
            },
            templateUrl: '/App_Plugins/Merchello/Backoffice/Merchello/directives/merchellonotestable.tpl.html',
            link: function (scope, elm, attr) {

                scope.addNote = openNotesDialog;
                scope.editNote = openEditNote;
                scope.deleteNote = deleteNote;
                scope.smallText = '';

                function init() {
                    var smallTextKey = 'merchelloNotes_' + scope.entityType.toLowerCase() + 'Notes';
                    localizationService.localize(smallTextKey).then(function(txt) {
                       scope.smallText = txt;
                    });
                }
                
                function openNotesDialog() {
                    getNoteData().then(function(data) {
                        var dialogData = {};
                        dialogData.title = data[0];
                        dialogData.note = noteDisplayBuilder.createDefault();
                        dialogData.note.internalOnly = true;
                        dialogData.note.author = data[1].email;
                        dialogService.open({
                            template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/notes.addeditnote.dialog.html',
                            show: true,
                            callback: processAddNoteDialog,
                            dialogData: dialogData
                        });
                    });
                }

                function openEditNote(note) {
                    localizationService.localize('merchelloNotes_editNote').then(function(title) {
                        var dialogData = {};
                        dialogData.title = title;
                        dialogData.note = angular.extend(noteDisplayBuilder.createDefault(), note);
                        dialogService.open({
                            template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/notes.addeditnote.dialog.html',
                            show: true,
                            callback: processEditNoteDialog,
                            dialogData: dialogData
                        });
                    });
                }

                function deleteNote(note) {
                    var dialogData = {};
                    dialogData.name = note.message;
                    dialogData.note = note;
                    dialogService.open({
                        template: '/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/delete.confirmation.html',
                        show: true,
                        callback: processDeleteNoteDialog,
                        dialogData: dialogData
                    });
                }

                function processEditNoteDialog(dialogData) {
                    var note = _.find(scope.notes, function(n) {
                        return n.key === dialogData.note.key;
                    });
                    if (note !== null && note !== undefined) {
                        note.message = dialogData.note.message;
                        note.internalOnly = dialogData.note.internalOnly;
                    }
                    
                    scope.save();
                }

                function processAddNoteDialog(dialogData) {
                    scope.notes.push(dialogData.note);
                    scope.save();
                }

                function processDeleteNoteDialog(dialogData) {
                    scope.delete()(dialogData.note);
                }

                function getNoteData() {
                    var promises = [
                        localizationService.localize('merchelloNotes_addNote'),
                        userService.getCurrentUser(),
                    ];

                    return $q.all(promises);
                }

                // initialize the directive
                init();
            }
        }
    }]);
