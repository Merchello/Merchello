    /**
     * @ngdoc model
     * @name PaymentDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's PaymentDisplay object
     */
    var PaymentDisplay = function() {
        var self = this;
        self.key = '';
        self.customerKey = '';
        self.paymentMethodKey = '';
        self.paymentTypeFieldKey = '';
        self.paymentMethodName = '';
        self.referenceNumber = '';
        self.amount = 0.0;
        self.authorized = false;
        self.collected = false;
        self.exported = false;
        self.extendedData = {};
        self.appliedPayments = [];
    };

    PaymentDisplay.prototype = (function() {

        function getStatus() {
            var statusArr = [];
            if (this.authorized) {
                statusArr.push("Authorized");
            }
            if (this.collected) {
                statusArr.push("Captured");
            }
            if (this.exported) {
                statusArr.push("Exported");
            }

            return statusArr.join("/");
        }

        function hasAmount() {
            return this.amount > 0;
        }

        return {
            getStatus: getStatus,
            hasAmount: hasAmount
        };
    }());

    angular.module('merchello.models').constant('PaymentDisplay', PaymentDisplay);