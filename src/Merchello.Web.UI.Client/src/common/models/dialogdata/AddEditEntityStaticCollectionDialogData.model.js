/**
 * @ngdoc model
 * @name AddEditEntityStaticCollectionDialog
 * @function
 *
 * @description
 *  A dialog data object for adding or editing Static Collection objects
 */
var AddEditEntityStaticCollectionDialog = function () {
    var self = this;
    self.entityType = '';
    self.collectionKeys = [];
};

AddEditEntityStaticCollectionDialog.prototype = (function() {

    function exists(key) {
        if (this.collectionKeys.length === 0) {
            return false;
        }
        var found = _.find(this.collectionKeys, function(k) {
            return k === key;
        });
        return found !== undefined;
    }

    function addCollectionKey(key) {
        if (!exists.call(this, key)) {
            this.collectionKeys.push(key);
        }
    }

    function removeCollectionKey(key) {
        this.collectionKeys = _.reject(this.collectionKeys, function(item) {
          return item === key;
        });
    }

    return {
        exists : exists,
        addCollectionKey: addCollectionKey,
        removeCollectionKey: removeCollectionKey
    };
})();

angular.module('merchello.models').constant('AddEditEntityStaticCollectionDialog', AddEditEntityStaticCollectionDialog);
