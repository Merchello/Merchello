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

        if (shippingCountryFromServer == undefined) {
            self.key = "";
            self.catalogKey = "";
            self.countryCode = "";
            self.name = "";
            self.provinces = [];
            self.shipMethods = [];
        } else {
            self.key = shippingCountryFromServer.key;
            self.catalogKey = shippingCountryFromServer.catalogKey;
            self.countryCode = shippingCountryFromServer.countryCode;
            self.name = shippingCountryFromServer.name;
            self.provinces = _.map(shippingCountryFromServer.provinces, function (province) {
                return new merchello.Models.Province(province)
            });
            //self.shipMethods = _.map(shippingCountryFromServer., function (method) {
            //    return new merchello.Models.ShippingMethod(method);
            //});
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

        self.fromCountry = function (country)
        {
            self.countryCode = country.countryCode;
            self.name = country.name;
            self.provinces = _.map(country.provinces, function (province) {
                return new merchello.Models.Province(province)
            });
        };
    };

    models.GatewayProvider = function (gatewayProviderFromServer) {

        var self = this;

        if (gatewayProviderFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.description = "";
        } else {
            self.key = gatewayProviderFromServer.key;
            self.name = gatewayProviderFromServer.name;
            self.description = gatewayProviderFromServer.description;
        }
    };

    models.ShippingMethod = function (shippingMethodFromServer) {

        var self = this;

        if (shippingMethodFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.shipCountryKey = "";
            self.providerKey = "";
            self.shipMethodTfKey = "";
            self.surcharge = "";
            self.serviceCode = "";
            self.taxable = false;
            self.provinceData = [];
        } else {
            self.key = shippingMethodFromServer.key;
            self.name = shippingMethodFromServer.name;
            self.shipCountryKey = shippingMethodFromServer.shipCountryKey;
            self.providerKey = shippingMethodFromServer.providerKey;
            self.shipMethodTfKey = shippingMethodFromServer.shipMethodTfKey;
            self.surcharge = shippingMethodFromServer.surcharge;
            self.serviceCode = shippingMethodFromServer.serviceCode;
            self.taxable = shippingMethodFromServer.taxable;
            self.provinceData = _.map(shippingMethodFromServer.provinces, function (province) {
                return new merchello.Models.ProvinceData(province);
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
            self.key = "";
            self.shipMethodKey = "";
            self.rangeLow = "";
            self.rangeHigh = "";
            self.rate = "";
        } else {
            self.key = shippingRateTierFromServer.key;
            self.shipMethodKey = shippingRateTierFromServer.shipMethodKey;
            self.rangeLow = shippingRateTierFromServer.rangeLow;
            self.rangeHigh = shippingRateTierFromServer.rangeHigh;
            self.rate = shippingRateTierFromServer.rate;
        }


    };

    models.Warehouse = function (warehouseFromServer) {

        var self = this;

        if (warehouseFromServer == undefined) {
            self.key = "";
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
            self.warehouseCatalogs = [];
        } else {
            self.key = warehouseFromServer.key;
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

            self.warehouseCatalogs = _.map(warehouseFromServer.warehouseCatalogs, function (warehouseCatalog) {
                return new merchello.Models.WarehouseCatalog(warehouseCatalog);
            });
        }

    };

    models.WarehouseCatalog = function (warehouseCatalogFromServer) {

        var self = this;

        if (warehouseCatalogFromServer == undefined) {
            self.key = "";
            self.warehouseKey = "";
            self.name = "";
            self.description = "";
        } else {
            self.key = warehouseCatalogFromServer.key;
            self.warehouseKey = warehouseCatalogFromServer.warehouseKey;
            self.name = warehouseCatalogFromServer.name;
            self.description = warehouseCatalogFromServer.description;
        }

    };

    models.CatalogInventory = function (catalogInventoryFromServer) {

        var self = this;

        if (catalogInventoryFromServer == undefined) {
            self.catalogKey = "";
            self.warehouseKey = "";
            self.productVariantKey = "";
            self.count = 0;
            self.lowCount = 0;
            self.catalogName = "";
        } else {
            self.catalogKey = catalogInventoryFromServer.catalogKey;
            self.warehouseKey = catalogInventoryFromServer.warehouseKey;
            self.productVariantKey = catalogInventoryFromServer.productVariantKey;
            self.count = catalogInventoryFromServer.count;
            self.lowCount = catalogInventoryFromServer.lowCount;
            self.catalogName = catalogInventoryFromServer.catalogName;
        }
    };

}(window.merchello.Models = window.merchello.Models || {}));

