/**
    * @ngdoc model
    * @name NoteDisplay
    * @function
    *
    * @description
    * Represents a JS version of Merchello's NoteDisplay object
    */
var NoteDisplay = function () {
    var self = this;

    self.entityKey = '';
    self.entityTfKey = '';
    self.entityType = '';
    self.key = '';
    self.message = {};
    self.recordDate = '';
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
