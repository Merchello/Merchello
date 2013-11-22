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

    models.ShippingMethod = function (shippingMethodFromServer) {

        var self = this;

        if (shippingMethodFromServer == undefined) {
            self.pk = "";
            self.name = "";
            self.regionKey = "";
            self.providerKey = "";
            self.shipMethodTfKey = "";
            self.surcharge = "";
            self.serviceCode = "";
            self.taxable = false;
            self.provinceData = [];
        } else {
            self.pk = shippingMethodFromServer.pk;
            self.name = shippingMethodFromServer.name;
            self.regionKey = shippingMethodFromServer.regionKey;
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

    models.ShippingRegion = function (shippingRegionFromServer) {

        var self = this;

        if (shippingRegionFromServer == undefined) {
            self.pk = "";
            self.warehouseKey = "";
            self.code = "";
            self.name = "";
        } else {
            self.pk = shippingRegionFromServer.pk;
            self.warehouseKey = shippingRegionFromServer.warehouseKey;
            self.code = shippingRegionFromServer.code;
            self.name = shippingRegionFromServer.name;
        };

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
            self.primary = true;
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
            self.primary = warehouseFromServer.primary;
        }

    };

}(window.merchello.Models = window.merchello.Models || {}));

