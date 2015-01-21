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
        self.customer = {};
    };

    angular.module('merchello.models').constant('AddEditCustomerDialogData', AddEditCustomerDialogData);
