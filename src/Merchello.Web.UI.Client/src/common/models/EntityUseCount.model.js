var EntityUseCount = function() {
  var self = this;
    self.key = '';
    self.count = 0;
};


angular.module('merchello.models').constant('EntityUseCount', EntityUseCount);
