    /**
     * @ngdoc model
     * @name Merchello.Models.ListQuery
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ListQueryDisplay object
     */
    Merchello.Models.ListQuery = function(sortBy) {
        self.currentPage = 0;
        self.itemsPerPage = 0;
        self.parameters = [];
        self.sortBy = sortBy !== undefined ? sortBy : '';
        self.sortDirection = 'Ascending'; // valid options are 'Ascending' and 'Descending'
    };
