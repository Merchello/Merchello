    /**
     * @ngdoc model
     * @name Merchello.Models.Province
     * @function
     *
     * @description
     * Represents a JS version of Merchello's SettingDisplay object
     */
    // TODO this should be renamed in the Core and updated here
    Merchello.Models.Setting = function() {
        self.currencyCode = '';
        self.nextOrderNumber = 0;
        self.nextInvoiceNumber = 0;
        self.nextShipmentNumber = 0;
        self.dateFormat = '';
        self.timeFormat = '';
        self.unitSystem = '';
        self.globalShippable = false;
        self.globalTaxable = false;
        self.globalTrackInventory = false;
        self.globalShippingIsTaxable = false;
    };
