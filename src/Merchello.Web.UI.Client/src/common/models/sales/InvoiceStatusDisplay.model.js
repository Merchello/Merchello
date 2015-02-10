    /**
     * @ngdoc model
     * @name InvoiceStatusDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's InvoiceStatusDisplay object
     */
    var InvoiceStatusDisplay = function() {
        var self = this;

        self.key = '';
        self.name = '';
        self.alias = '';
        self.reportable = '';
        self.active = '';
        self.sortOrder = '';
    };

    angular.module('merchello.models').constant('InvoiceStatusDisplay', InvoiceStatusDisplay);