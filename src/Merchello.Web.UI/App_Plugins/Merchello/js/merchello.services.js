/*! merchello
 * https://github.com/meritage/Merchello
 * Copyright (c) 2015 Merchello;
 * Licensed MIT
 */

(function() { 


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
                var totals = [];
                angular.forEach(invoices, function(inv) {
                    var cc = inv.getCurrencyCode();
                    var total = inv.total;
                    var existing = _.find(totals, function(t) { return t.currencyCode === cc });
                    if (existing === null || existing === undefined) {
                        totals.push({ currencyCode: cc, total: total })
                    } else {
                        existing.total += total;
                    }
                });
                return _.sortBy(totals, function(o) { return o.currencyCode });
            }


    }]);


})();