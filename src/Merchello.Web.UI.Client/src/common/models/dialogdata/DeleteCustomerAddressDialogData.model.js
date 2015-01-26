    /**
     * @ngdoc model
     * @name DeleteCustomerAddressDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for passing customer addresses for deletion
     */
    var DeleteCustomerAddressDialogData = function() {
        var self = this;
        self.customerAddress = {};
        self.name = '';
    };

    angular.module('merchello.models').constant('DeleteCustomerAddressDialogData', DeleteCustomerAddressDialogData);