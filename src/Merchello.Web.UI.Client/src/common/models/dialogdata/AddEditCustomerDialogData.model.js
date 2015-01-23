    /**
     * @ngdoc model
     * @name AddEditCustomerDialogData
     * @function
     *
     * @description
     *  A dialog data object for adding or editing CustomerDisplay objects
     */
    var AddEditCustomerDialogData = function() {
        var self = this;
        self.firstName = '';
        self.lastName = '';
        self.email = '';
    };

    angular.module('merchello.models').constant('AddEditCustomerDialogData', AddEditCustomerDialogData);
