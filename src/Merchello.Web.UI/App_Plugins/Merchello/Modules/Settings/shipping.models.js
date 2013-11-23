(function (models, undefined) {

    models.ProvinceData = function (provinceDataFromServer) {

        var self = this;

        if (provinceDataFromServer == undefined) {
            self.name = "";
            self.adjustRate = "";
            self.isSupportedDestination = true;
        } else {
            self.name = provinceDataFromServer.name;
            self.adjustRate = provinceDataFromServer.adjustRate;
            self.isSupportedDestination = provinceDataFromServer.isSupportedDestination;
        }

    };


    models.ShippingCountry = function (shippingCountryFromServer) {

        var self = this;

        if (shippingRegionFromServer == undefined) {
            self.pk = "";
            self.catalogKey = "";
            self.countryCode = "";
            self.name = "";
            self.shipMethods = [];
        } else {
            self.pk = shippingCountryFromServer.pk;
            self.catalogKey = shippingCountryFromServer.catalogKey;
            self.countryCode = shippingCountryFromServer.countryCode;
            self.name = shippingCountryFromServer.name;
            self.shipMethods = _.map(shippingCountryFromServer, function (attribute) {
                return new merchello.Models
            });
        };

        self.addMethod = function (shippingMethod) {
            if (shippingMethod) {
                var newShippingMethod = shippingMethod;
            } else {
                var newShippingMethod = new merchello.Models.ShippingMethod();
            }
            self.shipMethods.push(newShippingMethod);
        };

        self.removeMethod = function (idx) {
            self.shipMethods.splice(idx, 1);
        };
    };

    models.ShippingMethod = function (shippingMethodFromServer) {

        var self = this;

        if (shippingMethodFromServer == undefined) {
            self.pk = "";
            self.name = "";
            self.shipCountryKey = "";
            self.providerKey = "";
            self.shipMethodTfKey = "";
            self.surcharge = "";
            self.serviceCode = "";
            self.taxable = false;
            self.provinceData = [];
        } else {
            self.pk = shippingMethodFromServer.pk;
            self.name = shippingMethodFromServer.name;
            self.shipCountryKey = shippingMethodFromServer.shipCountryKey;
            self.providerKey = shippingMethodFromServer.providerKey;
            self.shipMethodTfKey = shippingMethodFromServer.shipMethodTfKey;
            self.surcharge = shippingMethodFromServer.surcharge;
            self.serviceCode = shippingMethodFromServer.serviceCode;
            self.taxable = shippingMethodFromServer.taxable;
            self.provinceData = _.map(shippingMethodFromServer, function (attribute) {
                return new merchello.Models.ProvinceData(attribute);
            });
        }
        // Helper to add a shipping region adjustment to this shipping method.
        self.addProvince = function (province) {
            if (province) {
                var newShippingRegion = province;
            } else {
                var newShippingRegion = new merchello.Models.ShippingRegion();
            }
            // Note From Kyle: Not sure what preferred method we have on this project to inject the properties (if any) into the newly created region.
            self.provinceData.push(newShippingRegion);
        };

        // Helper to remove a shipping region adjustment from this shipping method.
        self.removeProvince = function (idx) {
            self.provinceData.splice(idx, 1);
        };

    };

    models.ShippingRateTier = function (shippingRateTierFromServer) {

        var self = this;

        if (shippingRateTierFromServer == undefined) {
            self.pk = "";
            self.shipMethodKey = "";
            self.rangeLow = "";
            self.rangeHigh = "";
            self.rate = "";
        } else {
            self.pk = shippingRateTierFromServer.pk;
            self.shipMethodKey = shippingRateTierFromServer.shipMethodKey;
            self.rangeLow = shippingRateTierFromServer.rangeLow;
            self.rangeHigh = shippingRateTierFromServer.rangeHigh;
            self.rate = shippingRateTierFromServer.rate;
        }


    };

    models.Warehouse = function (warehouseFromServer) {

        var self = this;

        if (warehouseFromServer == undefined) {
            self.pk = "";
            self.name = "";
            self.address1 = "";
            self.address2 = "";
            self.locality = "";
            self.region = "";
            self.postalCode = "";
            self.countryCode = "";
            self.phone = "";
            self.email = "";
            self.isDefault = true;
        } else {
            self.pk = warehouseFromServer.pk;
            self.name = warehouseFromServer.name;
            self.address1 = warehouseFromServer.address1;
            self.address2 = warehouseFromServer.address2;
            self.locality = warehouseFromServer.locality;
            self.region = warehouseFromServer.region;
            self.postalCode = warehouseFromServer.postalCode;
            self.countryCode = warehouseFromServer.countryCode;
            self.phone = warehouseFromServer.phone;
            self.email = warehouseFromServer.email;
            self.isDefault = warehouseFromServer.isDefault;
        }

    };

    models.WarehouseInventory = function (warehouseInventoryFromSever) {

        var self = this;

        if (warehouseInventoryFromSever == undefined) {
            self.catalogKey = "";
            self.warehouseKey = "";
            self.productVariantKey = "";
            self.count = "";
            self.lowCount = "";
            self.catalogName = "";
        } else {
            self.catalogKey = warehouseInventoryFromSever.catalogKey;
            self.warehouseKey = warehouseInventoryFromSever.warehouseKey;
            self.productVariantKey = warehouseInventoryFromSever.productVariantKey;
            self.count = warehouseInventoryFromSever.count;
            self.lowCount = warehouseInventoryFromSever.lowCount;
            self.catalogName = warehouseInventoryFromSever.catalogName;
        }

    };

}(window.merchello.Models = window.merchello.Models || {}));

