/**
 * @ngdoc model
 * @name EntityCollectionDisplay
 * @function
 *
 * @description
 * Represents a JS version of Merchello's EntityCollectionDisplay object
 */
var EntityCollectionDisplay = function() {
    var self = this;
    self.key = '';
    self.parentKey = '';
    self.entityTfKey = '';
    self.entityType = '';
    self.entityTypeField = {};
    self.providerKey = '';
    self.name = '';
    self.sortOrder = 0;
};


angular.module('merchello.models').constant('EntityCollectionDisplay', EntityCollectionDisplay);

