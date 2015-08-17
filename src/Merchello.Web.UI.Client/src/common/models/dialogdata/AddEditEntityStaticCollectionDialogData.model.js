/**
 * @ngdoc model
 * @name AddEditEntityStaticCollectionDialog
 * @function
 *
 * @description
 *  A dialog data object for adding or editing Static Collection objects
 */
function AddEditEntityStaticCollectionDialog() {
    var self = this;
    self.entityType = '';
    self.collectionKeys = [];
};


angular.module('merchello.models').constant('AddEditEntityStaticCollectionDialog', AddEditEntityStaticCollectionDialog);
