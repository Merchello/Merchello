    /**
     * @ngdoc model
     * @name EditShipmentDialogData
     * @function
     *
     * @description
     * A back office model used for shipment data to the dialogService
     *
     */
    var EditShipmentDialogData = function() {
        var self = this;
        self.shipment = {};
        self.shipmentStatuses = [];
    };

    angular.module('merchello.models').constant('EditShipmentDialogData', EditShipmentDialogData);