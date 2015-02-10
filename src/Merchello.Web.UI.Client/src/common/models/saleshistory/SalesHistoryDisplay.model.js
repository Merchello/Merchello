    /**
     * @ngdoc model
     * @name SalesHistoryDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's SalesHistoryDisplay object
     */
    var SalesHistoryDisplay = function() {
        var self = this;
        self.dailyLogs = [];
    };

    SalesHistoryDisplay.prototype = (function() {

        // utility method to push a daily log
        function addDailyLog(dailyLog) {
            this.dailyLogs.push(dailyLog);
        }

        return {
            addDailyLog: addDailyLog
        };

    }());

    angular.module('merchello.models').constant('SalesHistoryDisplay', SalesHistoryDisplay);