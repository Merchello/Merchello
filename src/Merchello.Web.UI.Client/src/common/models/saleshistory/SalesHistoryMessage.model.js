    /**
     * @ngdoc model
     * @name SalesHistoryMessageDisplay
     * @function
     *
     * @description
     * Represents a sales history message object
     */
    var SalesHistoryMessageDisplay = function() {
        var self = this;
        self.area = '';
        self.key = '';
        self.formattedMessage = '';
    };

    SalesHistoryMessageDisplay.prototype = (function() {

        // constructs a localization key
        function localizationKey() {
            return this.area + '_' + this.key;
        }

        // any extra properties on this object are assumed to be tokens used in the localized
        // message
        function localizationTokens() {
            var allKeys = Object.keys(this);
            var tokens = [];
            for(var i = 0; i < allKeys.length; i++) {
                if (allKeys[i] !== 'area' && allKeys[i] !== 'key')
                {
                    tokens.push(this[allKeys[i]]);
                }
            }
            return tokens;
        }

        return {
            localizationKey: localizationKey,
            localizationTokens: localizationTokens
        };
    }());

    angular.module('merchello.models').constant('SalesHistoryMessageDisplay', SalesHistoryMessageDisplay);