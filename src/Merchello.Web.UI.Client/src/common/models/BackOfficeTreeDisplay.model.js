/**
 * @ngdoc model
 * @name BackOfficeTreeDisplay
 * @function
 *
 * @description
 * Represents a JS version of  BackOfficeTreeDisplay object
 */
var BackOfficeTreeDisplay = function() {
    var self = this;
    self.routeId = '';
    self.parentRouteId = '';
    self.title = '';
    self.icon = '';
    self.routePath = '';
    self.sortOrder = 0;
};

angular.module('merchello.models').constant('BackOfficeTreeDisplay', BackOfficeTreeDisplay);