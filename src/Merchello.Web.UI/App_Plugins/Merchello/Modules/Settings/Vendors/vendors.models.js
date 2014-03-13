(function (models, undefined) {

    models.Vendor = function (vendorFromServer) {

        var self = this;

        if (vendorFromServer == undefined) {
            self.key = "";
            self.name = "";
            self.contact = "";
            self.phone = "";
            self.address1 = "";
            self.address2 = "";
            self.locality = "";
            self.region = "";
            self.postalCode = "";
            self.country = "";
        } else {
            self.key = vendorFromServer.key;
            self.name = vendorFromServer.name;
            self.contact = vendorFromServer.contact;
            self.phone = vendorFromServer.phone;
            self.address1 = vendorFromServer.address1;
            self.address2 = vendorFromServer.address2;
            self.locality = vendorFromServer.locality;
            self.region = vendorFromServer.region;
            self.postalCode = vendorFromServer.postalCode;
            self.country = vendorFromServer.country;
        };

    };

}(window.merchello.Models = window.merchello.Models || {}));