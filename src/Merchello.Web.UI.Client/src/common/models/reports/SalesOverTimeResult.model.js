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

    self.startDate = '';
    self.endDate = '';
    self.month = '';
    self.year = '';
    self.salesCount = 0;
    self.totals = [];
};

SalesOverTimeResult.prototype = (function() {

    function getDateLabel() {
        return this.month + ' ' + this.year.substring(2, 4);
    }

    return {
        getDateLabel : getDateLabel
    };

})();

angular.module('merchello.models').constant('SalesOverTimeResult', SalesOverTimeResult);