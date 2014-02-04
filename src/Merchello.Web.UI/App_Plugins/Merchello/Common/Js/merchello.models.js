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

}(window.merchello.Models = window.merchello.Models || {}));