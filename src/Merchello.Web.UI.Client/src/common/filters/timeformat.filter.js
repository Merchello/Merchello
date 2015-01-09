    /**
     * @ngdoc filter
     * @name timeformat
     * @function
     *
     * @description
     * Filter to convert time formats between 24 hour and 12 hour
     */

    var timeFormat = function() {
        return function(date, setting) {
            if (setting === 'am-pm')
            {
                var hours = date.getHours();
                var minutes = date.getMinutes();
                var ampm = hours >= 12 ? 'pm' : 'am';
                hours = hours % 12;
                hours = hours ? hours : 12; // the hour '0' should be '12'
                minutes = minutes < 10 ? '0'+minutes : minutes;
                var strTime = hours + ':' + minutes + ' ' + ampm;
                return strTime;
            }
            else
            {
                return date.getHours() + ':' + date.getMinutes();
            }
        };
    };

    angular.module('merchello.filters').filter('timeformat', timeFormat);
