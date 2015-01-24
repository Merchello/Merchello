    /**
     * @ngdoc model
     * @name DeleteCustomerDialogData
     * @function
     *
     * @description
     *  A dialog data object for deleting CustomerDisplay objects
     */
    var DeleteCustomerDialogData = function() {
        var self = this;
        self.customer = {};
        self.name = '';
    };

    angular.module('merchello.models').constant('DeleteCustomerDialogData', DeleteCustomerDialogData);
