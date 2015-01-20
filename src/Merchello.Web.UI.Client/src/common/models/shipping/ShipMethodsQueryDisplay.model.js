    /**
     * @ngdoc model
     * @name ShipMethodsQueryDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ShipMethodsQueryDisplay object
     */
    var ShipMethodsQueryDisplay = function() {
        var self = this;
        self.selected = {};
        self.alternatives = [];
    };

    angular.module('merchello.models').constant('ShipMethodsQueryDisplay', ShipMethodsQueryDisplay);