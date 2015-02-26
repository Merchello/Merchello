    /**
     * @ngdoc model
     * @name OrderDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's OrderDisplay object
     */
    var OrderDisplay = function() {
        var self = this;
        self.key = '';
        self.versionKey = '';
        self.invoiceKey = '';
        self.orderNumberPrefix = '';
        self.orderNumber = '';
        self.orderDate = '';
        self.orderStatusKey = ''; // to do this is not needed since we have the OrderStatusDisplay
        self.orderStatus = {};
        self.exported = false;
        self.items = [];
    };

    OrderDisplay.prototype = (function() {

        function getUnShippedItems() {
            return _.filter(this.items, function(item) {
                return (item.shipmentKey === '' || item.shipmentKey === null) && item.extendedData.getValue('merchShippable').toLowerCase() === 'true';
            });
        }

        // public
        return {
            getUnShippedItems: getUnShippedItems
        };

    }());

    angular.module('merchello.models').constant('OrderDisplay', OrderDisplay);