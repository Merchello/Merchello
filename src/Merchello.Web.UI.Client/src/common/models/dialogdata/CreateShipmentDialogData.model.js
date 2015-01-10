    /**
     * @ngdoc model
     * @name CreateShipmentDialogData
     * @function
     *
     * @description
     * A back office model used for passing shipment creation information to the dialogService
     *
     * @note
     * Presently there is not a corresponding Merchello.Web model
     */
    var CreateShipmentDialogData = function() {
        var self = this;
        self.order = {};
        self.shipmentStatuses = [];
        self.shipment = {};
        self.shipMethods = {};
    };

    angular.module('merchello.models').constant('CreateShipmentDialogData', CreateShipmentDialogData);