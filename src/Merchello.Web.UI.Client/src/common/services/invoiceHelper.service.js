/**
 * @ngdoc service
 * @name invoiceHelper
 * @description Helper functions for an invoice.
 **/
angular.module('merchello.services').service('invoiceHelper',
    [
    function() {

        /**
         * @ngdoc method
         * @name getTotalsByCurrencyCode
         * @function
         *
         * @description
         * Totals a collection of invoices by currency code.
         */
        // TODO this should be moved to a prototype method for consistency
        this.getTotalsByCurrencyCode = function(invoices) {
            var self = this;
            var totals = [];
            angular.forEach(invoices, function(inv) {
                var cc = inv.getCurrencyCode();
                var total = self.round(inv.total, 2);
                var existing = _.find(totals, function(t) { return t.currencyCode === cc; });
                if (existing === null || existing === undefined) {
                    totals.push({ currencyCode: cc, total: total });
                } else {
                    existing.total += total;
                }
            });
            return _.sortBy(totals, function(o) { return o.currencyCode; });
        },

        /**
         * @ngdoc method
         * @name round
         * @function
         *
         * @description
         * Rounds a decimal to a specific number of places.
         */
        this.round = function(num, places) {
            var rounded = +(Math.round(num + "e+" + places) + "e-" + places);
            return isNaN(rounded) ? 0 : rounded;
        },

        /**
         * @ngdoc method
         * @name valueIsInRange
         * @function
         *
         * @description
         * Verifies a value is within a range of values.
         */
        this.valueIsInRage = function(str,min, max) {
            n = parseFloat(str);
            return (!isNaN(n) && n >= min && n <= max);
        },

        this.padLeft = function(str, char, num) {
            var pad = '';
            for(var i = 0; i < num; i++) {
                pad += char;
            }
            return (pad + str).slice(-num);
        };

}]);
