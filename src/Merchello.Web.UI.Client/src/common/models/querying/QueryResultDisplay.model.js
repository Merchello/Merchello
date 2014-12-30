
    /**
     * @ngdoc model
     * @name QueryResultDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's QueryResultDisplay object
     */
    var QueryResultDisplay = function() {
        var self = this;
        self.currentPage = 0;
        self.items = [];
        self.itemsPerPage = 0;
        self.totalItems = 0;
        self.totalPages = 0;
    };

    QueryResultDisplay.prototype = (function() {
        function addItem(item) {
            this.items.push(item);
        }

        return {
            addItem: addItem
        };
    }());

    angular.module('merchello.models').constant('QueryResultDisplay', QueryResultDisplay);