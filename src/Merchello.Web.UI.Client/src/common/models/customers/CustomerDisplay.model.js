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

    CustomerDisplay.prototype = (function() {

        function getLastInvoice() {
            if (this.invoices.length > 0) {
                var sorted = _.sortBy(this.invoices, function(invoice) {
                    return -1 * invoice.invoiceNumber;
                });
                if(sorted === undefined || sorted === null) {
                    return new InvoiceDisplay();
                } else {
                    return sorted[0];
                }
            } else {
                return new InvoiceDisplay();
            }
        }

        return {
            getLastInvoice: getLastInvoice
        }

    }());

    angular.module('merchello.models').constant('CustomerDisplay', CustomerDisplay);
