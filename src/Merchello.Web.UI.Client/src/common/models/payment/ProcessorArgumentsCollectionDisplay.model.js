    /**
     * @ngdoc model
     * @name ProcessorArgumentCollectionDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ProcessorArgumentCollectionDisplay object
     */
     var ProcessorArgumentCollectionDisplay = function() {
        var self = this;
        self.items = [];
    };

    ProcessorArgumentCollectionDisplay.prototype = (function() {

        function toArray() {
            return this.items;
        };

        return {
            toArray: toArray
        }

    }());

    angular.module('merchello.models').constant('ProcessorArgumentCollectionDisplay', ProcessorArgumentCollectionDisplay);