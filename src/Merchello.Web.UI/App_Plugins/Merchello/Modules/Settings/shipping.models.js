(function (models, undefined) {

    models.ShippingCountry = function (shippingCountryFromServer) {
        
        var self = this;

        if (shippingRegionFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.shippingMethods = [];
        } else {
            self.key = shippingCountryFromServer.key;
            self.name = shippingCountryFromServer.name;
            self.shippingMethods = _.map(shippingCountryFromServer, function (attribute) {
                return new merchello.Models.ShippingMethod(attribute);
            });
        }

    };

    models.ShippingMethod = function (shippingMethodFromServer) {

        var self = this;

        if (shippingMethodFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.areWarehousesSupported = [];
            self.type = "";
            self.minVariance = 0;
            self.maxVariance = 0;
            self.cost = 0;
            self.shippingRegions = [];
        } else {
            self.key = shippingMethodFromServer.key;
            self.name = shippingMethodFromServer.name;
            self.areWarehousesSupported = shippingMethodFromServer.areWarehousesSupported;
            self.type = shippingMethodFromServer.type;
            self.minVariance = shippingMethodFromServer.minVariance;
            self.maxVariance = shippingMethodFromServer.maxVariance;
            self.cost = shippingMethodFromServer.cost;
            self.shippingRegions = _.map(shippingMethodFromServer, function (attribute) {
                return new merchello.Models.ShippingRegion(attribute);
            });
        }

    };

    models.ShippingRegion = function (shippingRegionFromServer) {

        var self = this;

        if (shippingRegionFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.adjustRate = 0;
            self.finalRate = 0;
        } else {
            self.key = shippingRegionFromServer.key;
            self.name = shippingRegionFromServer.name;
            self.adjustRate = shippingRegionFromServer.adjustRate;
            self.finalRate = shippingRegionFromServer.finalRate;
        };

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

