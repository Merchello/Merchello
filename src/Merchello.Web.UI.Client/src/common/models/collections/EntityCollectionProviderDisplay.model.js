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
};


angular.module('merchello.models').constant('EntityCollectionProviderDisplay', EntityCollectionProviderDisplay);