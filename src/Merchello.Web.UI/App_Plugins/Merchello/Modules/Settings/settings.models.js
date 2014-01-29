(function (models, undefined) {                                                                                                          

    models.SettingDisplay = function (settingsFromServer) {

        var self = this;
        if (settingsFromServer == undefined) {
            self.currencyCode = "";
            self.nextOrderNumber = 0;
            self.nextInvoiceNumber = 0;
            self.dateFormat = "";
            self.timeFormat = "";
            self.globalShippable = false;
            self.globalTaxable = false;
            self.globalTrackInventory = false;
        }
        else {
            self.currencyCode = settingsFromServer.currencyCode;
            self.nextOrderNumber = settingsFromServer.nextOrderNumber;
            self.nextInvoiceNumber = settingsFromServer.nextInvoiceNumber;
            self.dateFormat = settingsFromServer.dateFormat;
            self.timeFormat = settingsFromServer.timeFormat;
            self.globalShippable = settingsFromServer.globalShippable;
            self.globalTaxable = settingsFromServer.globalTaxable;
            self.globalTrackInventory = settingsFromServer.globalTrackInventory;
        }
    };

}(window.merchello.Models = window.merchello.Models || {}));