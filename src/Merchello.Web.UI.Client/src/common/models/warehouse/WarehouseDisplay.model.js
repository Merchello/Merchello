    /**
     * @ngdoc model
     * @name WarehouseDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's WarehouseDisplay object
     */
    var WarehouseDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.address1 = '';
        self.address2 = '';
        self.locality = '';
        self.region = '';
        self.postalCode = '';
        self.countryCode = '';
        self.countryName = '';
        self.phone = '';
        self.email = '';
        self.isDefault = true;
        self.warehouseCatalogs = [];
    };

    WarehouseDisplay.prototype = (function() {

        function getAddress() {
            var adr = new AddressDisplay();
            adr.name = this.name;
            adr.address1 = this.address1;
            adr.address2 = this.address2;
            adr.locality = this.locality;
            adr.region = this.region;
            adr.postalCode = this.postalCode;
            adr.countryCode = this.countryCode;
            adr.countryName = this.countryName;
            adr.phone = this.phone;
            adr.email = this.email;
            adr.addressType = 'shipping';
            return adr;
        }

        function setAddress(address) {
            this.name = address.name;
            this.address1 = address.address1;
            this.address2 = address.address2;
            this.locality = address.locality;
            this.region = address.region;
            this.postalCode = address.postalCode;
            this.countryCode = address.countryCode;
            this.countryName = address.countryName;
            this.phone = address.phone;
            this.email = address.email;
        }

        function findDefaultCatalog() {
            return _.find(this.warehouseCatalogs, function (catalog) { return catalog.isDefault; });
        }

        return {
            getAddress: getAddress,
            setAddress: setAddress,
            findDefaultCatalog: findDefaultCatalog
        };
    }());

    angular.module('merchello.models').constant('WarehouseDisplay', WarehouseDisplay);