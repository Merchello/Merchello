    /**
     * @ngdoc model
     * @name CurrencyDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's CurrencyDisplay object
     */
    var CurrencyDisplay = function() {
        var self = this;
        self.name = '';
        self.currencyCode = '';
        self.symbol = '';
    };

    angular.module('merchello.models').constant('CurrencyDisplay', CurrencyDisplay);
