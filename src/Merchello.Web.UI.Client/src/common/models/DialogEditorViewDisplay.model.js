    /**
     * @ngdoc model
     * @name DialogEditorViewDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's DialogEditorViewDisplay object
     */
    var DialogEditorViewDisplay = function() {
        var self = this;
        self.title = '';
        self.description = '';
        self.editorView = '';
    };

    angular.module('merchello.models').constant('DialogEditorViewDisplay', DialogEditorViewDisplay);