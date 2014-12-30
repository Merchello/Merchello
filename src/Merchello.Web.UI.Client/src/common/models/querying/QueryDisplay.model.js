    /**
     * @ngdoc model
     * @name QueryDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's QueryDisplay object
     */
    var QueryDisplay = function() {
        self.currentPage = 0;
        self.itemsPerPage = 0;
        self.parameters = [];
        self.sortBy = '';
        self.sortDirection = 'Ascending'; // valid options are 'Ascending' and 'Descending'
    };

    angular.module('merchello.models').constant('QueryDisplay', QueryDisplay);