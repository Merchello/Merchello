/**
 * @ngdoc model
 * @name DeletePaymentMethodDialogData
 * @function
 *
 * @description
 * A back office dialogData model used for deleting payment methods.
 */
    var DeletePaymentMethodDialogData = function() {
        var self = this;
        self.paymentMethod = {};
        self.name = '';
    };

    angular.module('merchello.models').constant('DeletePaymentMethodDialogData', DeletePaymentMethodDialogData);
