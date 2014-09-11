(function (models, undefined) {                                                                                                          

    models.Province = function (provinceFromServer) {

        var self = this;

        if (provinceFromServer == undefined) {
            self.name = "";
            self.code = "";
        } else {
            self.name = provinceFromServer.name;
            self.code = provinceFromServer.code;
        }

    };

    models.Address = function (addressFromServer) {

        var self = this;

        if (addressFromServer == undefined) {
            self.name = "";
            self.address1 = "";
            self.address2 = "";
            self.locality = "";
            self.region = "";
            self.postalCode = "";
            self.countryCode = "";
            self.email = "";
            self.phone = "";
            self.organization = "";
            self.isCommercial = false;
        } else {
            self.name = addressFromServer.name;
            self.address1 = addressFromServer.address1;
            self.address2 = addressFromServer.address2;
            self.locality = addressFromServer.locality;
            self.region = addressFromServer.region;
            self.postalCode = addressFromServer.postalCode;
            self.countryCode = addressFromServer.countryCode;
            self.email = addressFromServer.email;
            self.phone = addressFromServer.phone;
            self.organization = addressFromServer.organization;
            self.isCommercial = addressFromServer.isCommercial;
        };

    };

    models.Country = function (countryFromServer) {

        var self = this;

        if (countryFromServer == undefined) {
            self.key = "";
            self.countryCode = "";
            self.name = "";
            self.provinceLabel = "";
            self.provinces = [];
        } else {
            self.key = countryFromServer.key;
            self.countryCode = countryFromServer.countryCode;
            self.name = countryFromServer.name;
            self.provinceLabel = countryFromServer.provinceLabel;
            self.provinces = _.map(countryFromServer.provinces, function (province) {
                return new merchello.Models.Province(province);
            });
        };

    };

    models.Currency = function(currencyFromServer) {

        var self = this;

        if (currencyFromServer == undefined) {
            self.name = "";
            self.currencyCode = "";
            self.symbol = "";
        } else {
            self.name = currencyFromServer.name;
            self.currencyCode = currencyFromServer.currencyCode;
            self.symbol = currencyFromServer.symbol;
        }
    };

    models.ListQuery = function(data) {

        var self = this;

        if (data === undefined) {
            self.currentPage = 0;
            self.itemsPerPage = 0;
            self.parameters = [];
            self.sortBy = 'invoicenumber'; // valid options are 'invoicenumber', 'billtoname', and 'invoicedate'
            self.sortDirection = 'Ascending'; // valid options are 'Ascending' and 'Descending'
        } else {
            self.currentPage = data.currentPage;
            self.itemsPerPage = data.itemsPerPage;
            if (data.parameters) {
                self.parameters = _.map(data.parameters, function(item) {
                    return new merchello.Models.ListQueryParameter(item);
                });
            } else {
                self.parameters = [];
            }
            self.sortBy = data.sortBy;
            self.sortDirection = data.sortDirection;
        }
    };

    models.ListQueryParameter = function(data) {

        var self = this;

        if (data === undefined) {
            self.fieldName = "";
            self.value = "";
        } else {
            self.fieldName = data.fieldName;
            self.value = data.value;
        }
    };

    models.QueryResult = function (data) {

        var self = this;

        if (data === undefined) {
            self.currentPage = 0;
            self.items = [];
            self.itemsPerPage = 0;
            self.totalItems = 0;
            self.totalPages = 0;
        } else {
            self.currentPage = data.currentPage;
            self.items = _.map(data.items, function (item) {
                return item;
            });
            self.itemsPerPage = data.itemsPerPage;
            self.totalItems = data.totalItems;
            self.totalPages = data.totalPages;
        }
    };

    models.TypeField = function (typeFromServer) {

        var self = this;

        if (typeFromServer == undefined) {
            self.alias = "";
            self.name = "";
            self.typeKey = "";
        } else {
            self.alias = typeFromServer.alias;
            self.name = typeFromServer.name;
            self.typeKey = typeFromServer.typeKey;
        }
    };

    models.StoreSettings = function (settingsFromServer) {

        var self = this;

        if (settingsFromServer == undefined) {
            self.currencyCode = "";
            self.nextOrderNumber = 0;
            self.nextInvoiceNumber = 0;
            self.dateFormat = "";
            self.timeFormat = "";
            self.unitSystem = "";
            self.globalShippable = false;
            self.globalTaxable = false;
            self.globalTrackInventory = false;
            self.globalShippingIsTaxable = false;
        }
        else {
            self.currencyCode = settingsFromServer.currencyCode;
            self.nextOrderNumber = parseInt(settingsFromServer.nextOrderNumber);
            self.nextInvoiceNumber = parseInt(settingsFromServer.nextInvoiceNumber);
            self.dateFormat = settingsFromServer.dateFormat;
            self.timeFormat = settingsFromServer.timeFormat;
            self.unitSystem = settingsFromServer.unitSystem;
            self.globalShippable = settingsFromServer.globalShippable;
            self.globalTaxable = settingsFromServer.globalTaxable;
            self.globalTrackInventory = settingsFromServer.globalTrackInventory;
            self.globalShippingIsTaxable = settingsFromServer.globalShippingIsTaxable;
        }
    };

    models.GatewayResource = function (gatewayResourceFromServer) {

        var self = this;

        if (gatewayResourceFromServer == undefined) {
            self.name = "";
            self.serviceCode = "";
        } else {
            self.name = gatewayResourceFromServer.name;
            self.serviceCode = gatewayResourceFromServer.serviceCode;
        }
    };

    models.GatewayProvider = function (gatewayProviderFromServer) {

        var self = this;

        if (gatewayProviderFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.providerTfKey = "";
            self.description = "";
            self.extendedData = [];
            self.encryptExtendedData = false;
            self.activated = false;
            self.dialogEditorView = "";
        } else {
            self.key = gatewayProviderFromServer.key;
            self.name = gatewayProviderFromServer.name;
            self.providerTfKey = gatewayProviderFromServer.providerTfKey;
            self.description = gatewayProviderFromServer.description;
            self.extendedData = gatewayProviderFromServer.extendedData;
            self.encryptExtendedData = gatewayProviderFromServer.encryptExtendedData;
            self.activated = gatewayProviderFromServer.activated;
            self.dialogEditorView = new merchello.Models.DialogEditorView(gatewayProviderFromServer.dialogEditorView);
        }
        self.resources = [];

        self.displayEditor = function() {
            return self.activated && self.dialogEditorView.editorView;
        };

        // TODO: get this from API or somehow better
        self.isFixedRate = function () {
            if (self.key == "aec7a923-9f64-41d0-b17b-0ef64725f576") {
                return true;
            } else {
                return false;
            }
        };

    };

    models.DialogEditorView = function(dialogEditorFromServer) {

        var self = this;
        
        if (dialogEditorFromServer == undefined) {

            self.title = "";
            self.description = "";
            self.editorView = "";

        } else {
            self.title = dialogEditorFromServer.title;
            self.description = dialogEditorFromServer.description;
            self.editorView = dialogEditorFromServer.editorView;
        }
            
    };

}(window.merchello.Models = window.merchello.Models || {}));