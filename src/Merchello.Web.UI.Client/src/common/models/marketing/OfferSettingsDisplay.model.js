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
        self.expired = false;
        self.active = true;
        self.componentDefinitions = [];
    };

    OfferSettingsDisplay.prototype = (function() {

        // adjusts date with timezone
        function localDateString(val) {
            var raw = new Date(val);
            return new Date(raw.getTime() + raw.getTimezoneOffset()*60000).toLocaleDateString();
        }

        // gets the local start date string
        function offerStartsDateLocalDateString() {
            return localDateString(this.offerStartsDate);
        }

        // gets the local end date string
        function offerEndsDateLocalDateString() {
            return localDateString(this.offerEndsDate);
        }

        function componentDefinitionExtendedDataToArray() {
            angular.forEach(this.componentDefinitions, function(cd) {
                cd.extendedData = cd.extendedData.toArray();
            });
        }

        return {
            offerStartsDateLocalDateString: offerStartsDateLocalDateString,
            offerEndsDateLocalDateString: offerEndsDateLocalDateString,
            componentDefinitionExtendedDataToArray: componentDefinitionExtendedDataToArray
        }

    }());

    angular.module('merchello.models').constant('OfferSettingsDisplay', OfferSettingsDisplay);