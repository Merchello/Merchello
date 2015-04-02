
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
             * @name rounds a decimal to a specific number of places
             * @function
             *
             * @description
             * Totals a collection of invoices by currency code.
             */
            this.round = function(num, places) {
                var rounded = +(Math.round(num + "e+" + places) + "e-" + places);
                return isNaN(rounded) ? 0 : rounded;
            }

    }]);
