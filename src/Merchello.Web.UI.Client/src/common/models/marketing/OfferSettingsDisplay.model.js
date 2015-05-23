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
        self.offerStartsDate = '';
        self.offerEndsDate = '';
        self.active = true;
        self.componentDefinitions = [];
    };

    angular.module('merchello.models').constant('OfferSettingsDisplay', OfferSettingsDisplay);