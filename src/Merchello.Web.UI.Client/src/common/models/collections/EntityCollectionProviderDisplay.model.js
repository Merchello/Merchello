/**
 * @ngdoc model
 * @name EntityCollectionDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's EntityCollectionProviderDisplay object
 */
var EntityCollectionProviderDisplay = function() {
    var self = this;
    self.key = '';
    self.name = '';
    self.description = '';
    self.entityTypeField = {};
    self.managesUniqueCollection = true;
    self.entityType = '';
    self.managedCollections = [];
    // self.dialogEditorView = undefined;  we use this model for both collections and filters
    //                                     this is just to indicate that the property is there in some models
};


angular.module('merchello.models').constant('EntityCollectionProviderDisplay', EntityCollectionProviderDisplay);