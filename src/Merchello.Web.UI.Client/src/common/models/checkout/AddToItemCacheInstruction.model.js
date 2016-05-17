var AddToItemCacheInstruction = function() {
    var self = this;
    self.customerKey = '';
    self.items = [];
    self.itemCacheType = '';
};

angular.module('merchello.models').constant('AddToItemCacheInstruction', AddToItemCacheInstruction);
