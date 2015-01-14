    /**
    * @ngdoc model
    * @name AddressDisplay
    * @function
    * 
    * @description
    * Represents a JS version of Merchello's AddressDisplay object
    */
    var AddressDisplay = function () {

        var self = this;

        self.name = '';
        self.address1 = '';
        self.address2 = '';
        self.locality = '';
        self.region = '';
        self.postalCode = '';
        self.countryCode = '';
        self.addressType = '';
        self.organization = '';
        self.phone = '';
        self.email = '';
        self.isCommercial = false;
    };

    AddressDisplay.prototype = (function() {

        function isEmpty() {
            var result = false;
            if (this.address1 === '' || this.locality === '' || this.address1 === null || this.locality === null) {
                result = true;
            }
            return result;
        }

        return {
            isEmpty: isEmpty
        };
    }());

    angular.module('merchello.models').constant('AddressDisplay', AddressDisplay);
