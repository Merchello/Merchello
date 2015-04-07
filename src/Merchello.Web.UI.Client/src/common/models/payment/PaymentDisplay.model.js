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
        self.paymentMethodType = '';
        self.paymentMethodName = '';
        self.referenceNumber = '';
        self.amount = 0.0;
        self.authorized = false;
        self.voided = false;
        self.collected = false;
        self.exported = false;
        self.extendedData = {};
        self.appliedPayments = [];
    };

    PaymentDisplay.prototype = (function() {

        // private
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

        function appliedAmount() {
            var applied = 0;
            angular.forEach(this.appliedPayments, function(ap) {
                applied += ap.amount;
            });
            return applied;
        }

        // public
        return {
            getStatus: getStatus,
            hasAmount: hasAmount,
            appliedAmount: appliedAmount
        };
    }());

    angular.module('merchello.models').constant('PaymentDisplay', PaymentDisplay);