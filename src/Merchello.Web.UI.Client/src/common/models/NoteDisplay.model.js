var NoteDisplay = function() {
    var self = this;
    self.key = '';
    self.author = '';
    self.message = '';
    self.entityKey = '';
    self.entityTfKey = '';
    self.entityType = '';
    self.noteTypeField = {};
    self.recordDate = '';
    self.internalOnly = true;
};

NoteDisplay.prototype = (function () {

    function toDateString() {
        return this.recordDate.split('T')[0];
    }

    function toTimeString() {
        var time = this.recordDate.split('T')[1];
        return time.split(':')[0] + ':' + time.split(':')[1];
    }

    return {
        toDateString: toDateString,
        toTimeString: toTimeString
    };

}());

angular.module('merchello.models').constant('NoteDisplay', NoteDisplay);
