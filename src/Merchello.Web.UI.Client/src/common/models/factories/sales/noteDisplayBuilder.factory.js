/**
 * @ngdoc service
 * @name merchello.models.noteDisplayBuilder
 *
 * @description
 * A utility service that builds noteDisplayBuilder models
 */
angular.module('merchello.models')
    .factory('noteDisplayBuilder',
        ['genericModelBuilder', 'NoteDisplay',
            function (genericModelBuilder, NoteDisplay) {

                var Constructor = NoteDisplay;

                return {
                    createDefault: function () {
                        return new Constructor();
                    },
                    transform: function (jsonResult) {
                        var noteDisplay = genericModelBuilder.transform(jsonResult, Constructor);

                        noteDisplay.message = jsonResult.message;

                        return noteDisplay;
                    }
                };
            }]);