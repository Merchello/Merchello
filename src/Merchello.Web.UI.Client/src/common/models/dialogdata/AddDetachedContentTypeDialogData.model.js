/**
 * @ngdoc model
 * @name AddDetachedContentTypeDialogData
 * @function
 *
 * @description
 *  A dialog data object for adding or editing DetachedContentTypeDisplay objects
 */
var AddDetachedContentTypeDialogData = function() {
    var self = this;
    self.contentType = {};
};

angular.module('merchello.models').constant('AddDetachedContentTypeDialogData', AddDetachedContentTypeDialogData);