    /**
     * @ngdoc model
     * @name ShipFixedRateTableDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's ShipFixedRateTableDisplay object
     */
    var ShipFixedRateTableDisplay = function() {
        var self = this;
        self.shipMethodKey = '';
        self.shipCountryKey = '';
        self.rows = [];
    };

    ShipFixedRateTableDisplay.prototype = (function() {

        // pushes a new row into the rate table rows collection
        function addRow(row) {
            this.rows.push(row);
        }

        // removes an existing row from the rate table
        function removeRow(row) {
            this.rows = _.reject(this.rows, function(r) { return r.key === row.key; });
        }

        return {
            addRow: addRow,
            removeRow: removeRow
        };
    }());

    angular.module('merchello.models').constant('ShipFixedRateTableDisplay', ShipFixedRateTableDisplay);