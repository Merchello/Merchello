(function (models, undefined) {

    models.ProvinceData = function (provinceDataFromServer) {

        var self = this;

        if (provinceDataFromServer == undefined) {
            self.name = "";
            self.code = "";
            self.allowShipping = false;
            self.rateAdjustment = "";
            self.rateAdjustmentType = 1;
        } else {
            self.name = provinceDataFromServer.name;
            self.code = provinceDataFromServer.code;
            self.allowShipping = provinceDataFromServer.allowShipping;
            self.rateAdjustment = provinceDataFromServer.rateAdjustment;
            self.rateAdjustmentType = provinceDataFromServer.rateAdjustmentType;
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
            self.shippingGatewayProviders = [];
            self.sortHelper = "0";
        } else {
            self.key = shippingCountryFromServer.key;
            self.catalogKey = shippingCountryFromServer.catalogKey;
            self.countryCode = shippingCountryFromServer.countryCode;
            self.name = shippingCountryFromServer.name;
            self.provinces = _.map(shippingCountryFromServer.provinces, function (province) {
                return new merchello.Models.Province(province);
            });
            self.shippingGatewayProviders = [];
            self.sortHelper = _.isEqual(self.name, "Everywhere Else") ? "1" + self.name : "0" + self.name;
        };

        self.fromCountry = function (country)
        {
            self.countryCode = country.countryCode;
            self.name = country.name;
            self.provinces = _.map(country.provinces, function (province) {
                return new merchello.Models.Province(province);
            });
        };
    };

    models.ShippingGatewayProvider = function (shippingGatewayProviderFromServer) {

        var self = this;

        if (shippingGatewayProviderFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.typeFullName = "";
            self.shipMethods = [];
        } else {
            self.key = shippingGatewayProviderFromServer.key;
            self.name = shippingGatewayProviderFromServer.name;
            self.typeFullName = shippingGatewayProviderFromServer.typeFullName;
            self.shipMethods = _.map(shippingGatewayProviderFromServer.shipMethods, function (method) {
                return new merchello.Models.ShippingMethod(method);
            });
        }

        self.addMethod = function (shippingMethod) {
            var newShippingMethod = shippingMethod;
            if (newShippingMethod == undefined) {
                newShippingMethod = new merchello.Models.ShippingMethod();
            }
            self.shipMethods.push(newShippingMethod);
        };

        self.removeMethod = function (shippingMethod) {
            self.shipMethods = _.reject(self.shipMethods, function (m) { return m.key == shippingMethod.key; });
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
            self.provinces = [];
            self.dialogEditorView = new merchello.Models.DialogEditorView();

        } else {
            self.key = shippingMethodFromServer.key;
            self.name = shippingMethodFromServer.name;
            self.shipCountryKey = shippingMethodFromServer.shipCountryKey;
            self.providerKey = shippingMethodFromServer.providerKey;
            self.shipMethodTfKey = shippingMethodFromServer.shipMethodTfKey;
            self.surcharge = shippingMethodFromServer.surcharge;
            self.serviceCode = shippingMethodFromServer.serviceCode;
            self.taxable = shippingMethodFromServer.taxable;
            if (shippingMethodFromServer.provinces) {
                self.provinces = _.map(shippingMethodFromServer.provinces, function(province) {
                    return new merchello.Models.ProvinceData(province);
                });
            } else {
                self.provinces = [];
            }
            self.dialogEditorView = new merchello.Models.DialogEditorView(shippingMethodFromServer.dialogEditorView);
        }
        // Helper to add a shipping region adjustment to this shipping method.
        self.addProvince = function (province) {
            var newShippingRegion;
            if (province) {
                newShippingRegion = province;
            } else {
                newShippingRegion = new merchello.Models.ProvinceData();
            }
            // Note From Kyle: Not sure what preferred method we have on this project to inject the properties (if any) into the newly created region.
            self.provinces.push(newShippingRegion);
        };

        // Helper to remove a shipping region adjustment from this shipping method.
        self.removeProvince = function (idx) {
            self.provinces.splice(idx, 1);
        };

        self.displayEditor = function () {
            return self.dialogEditorView.editorView;
        };

    };

    models.Range = function (low, high) {

        var self = this;

        self.low = 0;
        self.high = 0;

        if (low != undefined) {
            self.low = low;
        } 
        if (high != undefined) {
            self.high = high;
        }
    };

    models.FixedRateShippingMethod = function (shippingMethodFromServer) {

        var self = this;

        if (shippingMethodFromServer == undefined) {
            self.shipMethod = new merchello.Models.ShippingMethod();
            self.gatewayResource = new merchello.Models.GatewayResource();
            self.rateTable = new merchello.Models.ShipRateTable();
            self.rateTableType = "";
        } else {
            self.shipMethod = new merchello.Models.ShippingMethod(shippingMethodFromServer.shipMethod);
            self.gatewayResource = new merchello.Models.GatewayResource(shippingMethodFromServer.gatewayResource);
            self.rateTable = new merchello.Models.ShipRateTable(shippingMethodFromServer.rateTable);
            self.rateTableType = shippingMethodFromServer.rateTableType;
        }

        // Helper to calculate the range of all rateTiers
        self.tierRange = function () {
            var range = new merchello.Models.Range();

            var lowTier = _.min(self.rateTable.rows, function (tier) { return parseFloat(tier.rangeLow); });
            var highTier = _.max(self.rateTable.rows, function (tier) { return parseFloat(tier.rangeHigh); });

            if (lowTier) {
                range.low = lowTier.rangeLow;
            }
            if (highTier) {
                range.high = highTier.rangeHigh;
            }

            return range;
        };

        // Helper to calculate the range of all rateTier prices
        self.tierPriceRange = function () {
            var range = new merchello.Models.Range();

            var lowTier = _.min(self.rateTable.rows, function (tier) { return parseFloat(tier.rate); });
            var highTier = _.max(self.rateTable.rows, function (tier) { return parseFloat(tier.rate); });

            if (lowTier) {
                range.low = lowTier.rate;
            }
            if (highTier) {
                range.high = highTier.rate;
            }

            return range;
        };
    };

    models.ShipRateTable = function (shipRateTableFromServer) {

        var self = this;

        if (shipRateTableFromServer == undefined) {
            self.shipMethodKey = "";
            self.shipCountryKey = "";
            self.rows = [];
        } else {
            self.shipMethodKey = shipRateTableFromServer.shipMethodKey;
            self.shipCountryKey = shipRateTableFromServer.shipCountryKey;
            self.rows = _.map(shipRateTableFromServer.rows, function (row) {
                return new merchello.Models.ShippingRateTier(row);
            });
        }

        self.addRow = function(row) {
            self.rows.push(row);
        };

        self.removeRow = function (rowToRemove) {
            self.rows = _.reject(self.rows, function(row) { return row.$$hashKey == rowToRemove.$$hashKey; });
        };
    };

    models.ShippingRateTier = function (shippingRateTierFromServer) {

        var self = this;

        if (shippingRateTierFromServer == undefined) {
            self.key = "";
            self.shipMethodKey = "";
            self.rangeLow = 0;
            self.rangeHigh = 0;
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

        self.findDefaultCatalog = function () {
            return _.find(self.warehouseCatalogs, function (catalog) { return catalog.isDefault; });
        };

    };

    models.WarehouseCatalog = function (warehouseCatalogFromServer) {

        var self = this;

        if (warehouseCatalogFromServer == undefined) {
            self.description = "";
            self.isDefault = false;
            self.key = "";
            self.name = "";
            self.warehouseKey = "";
        } else {
            self.description = warehouseCatalogFromServer.description;
            if (warehouseCatalogFromServer.isDefault) {
                self.isDefault = true;
            } else {
                self.isDefault = false;
            }
            self.key = warehouseCatalogFromServer.key;
            self.name = warehouseCatalogFromServer.name;
            self.warehouseKey = warehouseCatalogFromServer.warehouseKey;
        }

    };

    models.CatalogInventory = function (catalogInventoryFromServer) {

        var self = this;

        if (catalogInventoryFromServer == undefined) {
            self.catalogName = "";
            self.catalogKey = "";
            self.count = 0;
            self.location = "";
            self.lowCount = 0;
            self.productVariantKey = "";
            self.warehouseKey = "";
            self.productInCatalog = false;
            self.warehouse = {};
            self.catalog = {};
        } else {
            self.catalogKey = catalogInventoryFromServer.catalogKey;
            self.catalogName = catalogInventoryFromServer.catalogName;
            self.count = catalogInventoryFromServer.count;
            self.location = catalogInventoryFromServer.location;
            self.lowCount = catalogInventoryFromServer.lowCount;
            self.productVariantKey = catalogInventoryFromServer.productVariantKey;
            self.warehouseKey = catalogInventoryFromServer.warehouseKey;
            self.productInCatalog = false;
            self.warehouse = {};
            self.catalog = {};
        }

        self.setWarehouse = function (warehouse) {
            self.warehouse = warehouse;
            self.warehouseKey = warehouse.key;
        };

        self.setCatalog = function (catalog) {
            self.catalog = catalog;
            self.catalogKey = catalog.key;
        };

        self.findMyWarehouse = function (warehouses) {
            return _.find(warehouses, function (warehouse) { return warehouse.key == self.warehouseKey; });
        };

        self.findMyCatalog = function (catalogs) {
            return _.find(catalogs, function (catalog) { return catalog.key == self.catalogKey; });
        };

    };

}(window.merchello.Models = window.merchello.Models || {}));

