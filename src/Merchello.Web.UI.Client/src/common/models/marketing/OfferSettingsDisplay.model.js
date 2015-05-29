    /**
     * @ngdoc model
     * @name OfferSettingsDisplay
     * @function
     *
     * @description
     * Represents a JS version of Merchello's OfferSettingsDisplay object
     */
    var OfferSettingsDisplay = function() {
        var self = this;
        self.key = '';
        self.name = '';
        self.offerCode = '';
        self.offerProviderKey = '';
        self.offerExpires = false;
        self.offerStartsDate = '';
        self.offerEndsDate = '';
        self.active = true;
        self.componentDefinitions = [];
    };

    OfferSettingsDisplay.prototype = (function() {
        function localDateString(val) {
            var raw = new Date(val);
            return new Date(raw.getTime() + raw.getTimezoneOffset()*60000).toLocaleDateString();
        }

        function offerStartsDateLocalDateString() {
            return localDateString(this.offerStartsDate);
        }

        function offerEndsDateLocalDateString() {
            return localDateString(this.offerEndsDate);
        }

        return {
            offerStartsDateLocalDateString: offerStartsDateLocalDateString,
            offerEndsDateLocalDateString: offerEndsDateLocalDateString
        }

    }());

    angular.module('merchello.models').constant('OfferSettingsDisplay', OfferSettingsDisplay);