    /**
     * @ngdoc model
     * @name CustomerDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's CustomerDisplay object
     */
    var CustomerDisplay = function() {
        var self = this;
        self.firstName = '';
        self.key = '';
        self.lastActivityDate = '';
        self.lastName = '';
        self.loginName = '';
        self.notes = '';
        self.email = '';
        self.taxExempt = false;
        self.extendedData = {};
        self.addresses = [];
        self.invoices = [];
    };

    angular.module('merchello.models').constant('CustomerDisplay', CustomerDisplay);
