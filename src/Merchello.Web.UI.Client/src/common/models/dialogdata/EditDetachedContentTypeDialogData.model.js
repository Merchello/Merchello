/**
 * @ngdoc model
 * @name EditDetachedContentTypeDialogData
 * @function
 *
 * @description
 * A back office dialogData model used for editing detached content types to the dialogService
 *
 */
var EditDetachedContentTypeDialogData = function() {
    var self = this;
    self.contentType = {};
};

angular.module('merchello.models').constant('EditDetachedContentTypeDialogData', EditDetachedContentTypeDialogData);