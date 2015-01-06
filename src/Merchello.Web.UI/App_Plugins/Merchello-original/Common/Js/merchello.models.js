(function (models, undefined) {


    models.Flyout = function (initiallyOpen, flyoutSemaphoreCallBack, options) {

        var self = this;

        // PROPERTIES
        self.isOpen = initiallyOpen;
        self.semaphore = flyoutSemaphoreCallBack;
        self.model = null;

        // defaults
        self.options = {
            open: function (model) { },
            close: function () { },
            toggle: function () { },
            confirm: function () { },
            clear: function () { }
        };
        _.extend(self.options, options);

        // METHODS

        self.open = function (model) {
            self.model = model;
            self.semaphore(true);
            self.isOpen = true;
            self.options.open(self.model);
        };

        self.close = function () {
            self.semaphore(false);
            self.isOpen = false;
            self.options.close();
        };

        self.toggle = function () {
            self.isOpen = !self.isOpen;
            self.semaphore(self.isOpen);
            self.options.toggle();
        };

        self.confirm = function () {
            self.options.confirm();
        };

        self.clear = function () {
            self.model = null;
            self.options.clear();
        };

        self.getModel = function () {
            return self.model;
        };
    };


    models.Address = function(data) {
            var self = this;

            if (data == undefined) {
                self.address1 = '';
                self.address2 = '';
                self.locality = '';
                self.region = '';
                self.postalCode = '';
                self.countryCode = '';
                self.addressType = '';
                self.organization = '';
                self.name = '';
                self.phone = '';
                self.email = '';
                self.isCommercial = false;
            } else {
                self.address1 = data.address1;
                self.address2 = data.address2;
                self.locality = data.locality;
                self.region = data.region;
                self.postalCode = data.postalCode;
                self.countryCode = data.countryCode;
                self.addressType = data.addressType;
                self.organization = data.organization;                
                self.name = data.name;
                self.phone = data.phone;
                self.email = data.email;
                self.isCommercial = data.isCommercial;
            }

   };
   

}(window.merchello.Models = window.merchello.Models || {}));