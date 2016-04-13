angular.module('merchello.models').factory('noteDisplayBuilder',
    ['genericModelBuilder', 'typeFieldDisplayBuilder', 'NoteDisplay',
    function(genericModelBuilder, typeFieldDisplayBuilder, NoteDisplay) {

        var Constructor = NoteDisplay;
        return {
            createDefault: function() {
                    var note = new Constructor();
                    note.noteTypeField = typeFieldDisplayBuilder.createDefault();
                    return note;
            },
            transform: function(jsonResult) {
                var notes = genericModelBuilder.transform(jsonResult, Constructor);
                if(angular.isArray(notes)) {
                    for(var i = 0; i < notes.length; i++) {
                        notes[ i ].noteTypeField = typeFieldDisplayBuilder.transform(jsonResult[ i ].noteTypeField);
                    }
                } else {
                    notes.noteTypeField = typeFieldDisplayBuilder.transform(jsonResult.noteTypeField);
                }
                return notes;
            }
        };
}]);
