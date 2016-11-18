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
    self.isFilter = false;
    self.extendedData = {};
    self.sortOrder = 0;
};


EntityCollectionDisplay.prototype = (function() {

    function clone() {
        var c = angular.extend(new EntityCollectionDisplay(), this);
        if (c.filters) {
            var collections = c.filters;
            c.filters = [];
            angular.forEach(collections, function(ac) {
                var atclone = angular.extend(new EntityCollectionDisplay(), ac);
                c.filters.push(atclone);
            });
        }
        return c;
    }

    return {
      clone: clone
    };
}());


angular.module('merchello.models').constant('EntityCollectionDisplay', EntityCollectionDisplay);

