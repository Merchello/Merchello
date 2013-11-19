(function (models, undefined) {

    models.shippingCountry = function (shippingCountryFromServer) {
        
        var self = this;

        if (shippingRegionFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.shippingMethods = [];
        } else {
            self.key = shippingCountryFromServer.key;
            self.name = shippingCountryFromServer.name;
            self.shippingMethods = _.map(shippingCountryFromServer, function (attribute) {
                return new merchello.Models.shippingMethod(attribute);
            });
        }

    };

    models.ShippingMethod = function (shippingMethodFromServer) {

        var self = this;

        if (shippingMethodFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.warehousesSupported = [];
            var 
        } else {

        }

    };

    models.Warehouse = function (warehouseFromServer) {

        var self = this;

        if (warehouseFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.address = {
                streetAddress: "",
                streetAddress2: "",
                locality: "",
                region: "",
                postalCode: "",
                country: ""
            };
            self.isPrimary = true;
        } else {
            self.key = warehouseFromServer.key;
            self.name = warehouseFromServer.name;
            self.address = warehouseFromServer.address;
            self.isPrimary = warehouseFromServer.isPrimary;
        }

    };

}(window.merchello.Models = window.merchello.Models || {}));

