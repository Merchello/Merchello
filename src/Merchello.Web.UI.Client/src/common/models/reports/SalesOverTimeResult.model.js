/**
 * @ngdoc model
 * @name SalesOverTimeResult
 * @function
 *
 * @description
 * Represents a JS version of Merchello's SalesOverTimeResult object
 */
var SalesOverTimeResult = function() {

    var self = this;

    self.month = '';
    self.year = '';
    self.salesCount = 0;
};

angular.module('merchello.models').constant('SalesOverTimeResult', SalesOverTimeResult);