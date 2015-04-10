    /**
     * @ngdoc model
     * @name CreateShipmentDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for passing shipment creation information to the dialogService
     *
     * @note
     * Presently there is not a corresponding Merchello.Web model
     */
    var CreateShipmentDialogData = function() {
        var self = this;
        self.invoiceKey = '';
        self.order = {};
        self.shipmentStatuses = [];
        self.shipmentStatus = {};
        self.shipmentRequest = {};
        self.shipMethods = {};
        self.trackingNumber = '';
        self.currencySymbol = '';
    };

    angular.module('merchello.models').constant('CreateShipmentDialogData', CreateShipmentDialogData);