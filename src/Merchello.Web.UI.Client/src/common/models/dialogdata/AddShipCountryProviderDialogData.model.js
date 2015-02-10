    /**
     * @ngdoc model
     * @name AddShipCountryProviderDialogData
     * @function
     *
     * @description
     * A back office dialogData model used for adding a shipping provider to ship countries.
     */
    var AddShipCountryProviderDialogData = function() {
        var self = this;
        self.showProvidersDropDown = true;
        self.availableProviders = [];
        self.selectedProvider = {};
        self.shipMethodName = '';
        self.selectedResource = {};
        self.country = {};
    };

    angular.module('merchello.models').constant('AddShipCountryProviderDialogData', AddShipCountryProviderDialogData);
