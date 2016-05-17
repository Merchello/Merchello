var ItemCacheInstruction = function() {
    var self = this;
    self.customerKey = '';
    self.entityKey = '';
    self.quantity = 0;
    self.itemCacheType = '';
};

angular.module('merchello.models').constant('ItemCacheInstruction', ItemCacheInstruction);