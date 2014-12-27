    /**
     * @ngdoc model
     * @name Merchello.Models.QueryResult
     * @function
     *
     * @description
     * Represents a JS version of Merchello's QueryResultDisplay object
     */
    Merchello.Models.QueryResult = function() {
        var self = this;
        self.currentPage = 0;
        self.items = [];
        self.itemsPerPage = 0;
        self.totalItems = 0;
        self.totalPages = 0;
    };