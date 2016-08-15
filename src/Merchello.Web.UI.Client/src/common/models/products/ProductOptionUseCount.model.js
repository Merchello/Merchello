var ProductOptionUseCount = function() {
    var self = this;
    self.option = 0;
    self.shared = false;
    self.choices = [];
};

angular.module('merchello.models').constant('ProductOptionUseCount', ProductOptionUseCount);
