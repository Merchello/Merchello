    /**
     * @ngdoc model
     * @name EditShipmentDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for shipment data to the dialogService
     *
     */
    var EditShipmentDialogData = function() {
        var self = this;
        self.shipment = {};
        self.shipmentStatuses = [];
        self.showPhone = false;
        self.showEmail = false;
        self.showIsCommercial = false;
    };

    angular.module('merchello.models').constant('EditShipmentDialogData', EditShipmentDialogData);